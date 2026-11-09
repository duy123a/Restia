# Multi-Tenant (`search_path`) Management – Design

Status: Approved (2026-11-09)

## 1. Understanding Summary

- **Goal**: Schema-per-tenant multi-tenancy on PostgreSQL using `search_path`, where each tenant has isolated Identity, Form, Permission, and Notification tables inside its own PostgreSQL schema.
- **URL & Routing**: All web and API routes carry a `{tenantId}` prefix: `/{tenantId}/{controller=Home}/{action=Index}/{id?}`. Root `/` redirects to `/{user.SchemaId}` when authenticated, or `/public/Account/Login` when unauthenticated.
- **Authentication & Isolation**:
  - Login occurs at `/{tenantId}/Account/Login`.
  - Every `AppUser` stores `SchemaId` (auto-assigned from the current tenant when creating users or seeding SuperAdmin) and includes `SchemaId` in user claims.
  - Any authenticated request whose route `{tenantId}` does not match the user's `SchemaId` claim (or targets a non-existent/inactive tenant) returns **404 NotFound**.
- **Public Schema & Tenant CRUD (`/public/Tenant`)**:
  - `public` is the primary management schema and hosts `public."Tenants"` (`Id`/SchemaName immutable after creation, `DisplayName`, `IsActive`, audit + soft-delete).
  - Only the **SuperAdmin of the `public` schema** can view and access the Tenant CRUD screens (`/public/Tenant`).
- **Automated Provisioning & Seeding**:
  - Creating a tenant on `/public/Tenant/Create` accepts SuperAdmin credentials (`AdminUserName`, `AdminEmail`, `AdminPassword`, `ConfirmPassword`), creates the PostgreSQL schema, runs EF Core migrations, seeds `Forms` and `Roles` (`SuperAdmin`, `Admin`, `Basic`), and creates the `SuperAdmin` account for that schema.
  - On startup, `public` is migrated and seeded using `SeedAdminSettings`, and all active tenants are migrated and seeded idempotently for `Forms` and `Roles`.
  - Deleting a tenant soft-deletes the `Tenant` record in `public."Tenants"` without dropping the PostgreSQL schema; `public` cannot be deactivated or deleted.
- **Non-Goals**:
  - Renaming PostgreSQL schemas after creation or running `DROP SCHEMA CASCADE` from the UI.
  - Cross-tenant session switching without re-authenticating.

## 2. Assumptions

- **Security (SQL Injection Prevention)**: `Tenant.Id` (`SchemaName`) must match `^[a-z][a-z0-9_]{1,62}$` and is always double-quoted (`"<schema>"`) in `CREATE SCHEMA` and `SET search_path`.
- **Performance & Scale**: Active tenants are cached in `IMemoryCache` for fast per-request validation in middleware and invalidated on Tenant CRUD operations.
- **Connection Pooling**: `TenantConnectionInterceptor` (`DbConnectionInterceptor`) executes `SET search_path TO "<SchemaId>"` whenever a connection opens, ensuring safe connection reuse across pooled connections.
- **Maintenance**: Reuses existing layout, `.data-table`, `_PaginationToolbar`, `_TempDataAlerts`, and localization resources (`SharedResources`).

## 3. Decision Log

| # | Decision | Alternatives | Reason |
|---|----------|--------------|--------|
| D1 | Route-based login `/{tenantId}/Account/Login`; `public."Tenants"` catalog; only `public` SuperAdmin accesses `/public/Tenant` | Single global login with tenant dropdown; in-app tenant switcher | Clean URL scoping and strict tenant isolation |
| D2 | Custom SuperAdmin credentials entered on `/public/Tenant/Create` (`public` still uses `SeedAdminSettings` on initial seed) | Fixed `SeedAdminSettings` for all tenants; manual background seed | Allows distinct initial SuperAdmin credentials per tenant |
| D3 | `Tenant` has immutable `Id` (SchemaName), editable `DisplayName` & `IsActive`; Delete performs soft-delete only | Physical `DROP SCHEMA CASCADE` on delete | Prevents irreversible data loss |
| D4 | Mismatched route `{tenantId}` vs logged-in `user.SchemaId` returns `404 NotFound` | Redirect to user's tenant; `403 AccessDenied` | Avoids leaking tenant existence/state across tenants |
| D5 | `DbConnectionInterceptor` (`SET search_path`) + per-schema `Database.MigrateAsync()` | `IModelCacheKeyFactory` dynamic schema; `GenerateCreateScript()` | Native PostgreSQL `search_path` behavior + full EF migration history per schema |

## 4. Final Design

### 4.1 Data Model & `search_path` Interceptor (`Restia.Common`)

- **`Tenant` Entity (`Restia.Common/Tenant/Entities/Tenant.cs`)**:
  - Inherits `BaseEntity` (audit, soft-delete, `xmin` rowversion).
  - Properties: `Id` (`string`, PK, max 63), `DisplayName` (`string`, max 200), `IsActive` (`bool`, default `true`).
  - `TenantConfiguration`: `builder.ToTable("Tenants", "public")` so `AppDbContext.Tenants` always queries `public."Tenants"`.
- **`AppUser` Entity**:
  - Add `SchemaId` (`string`, max 63, required).
  - `AppUserClaimsPrincipalFactory` adds `CustomClaimTypes.SchemaId` (`$/identity/claims/schema_id`).
  - `ClaimsPrincipalExtension.GetSchemaId()` reads the claim.
- **`ITenantContext` & `TenantConnectionInterceptor`**:
  - `ITenantContext` (Scoped): holds `SchemaId` (default `"public"`) and `SetTenant(string schemaId)`.
  - `TenantConnectionInterceptor`: overrides `ConnectionOpened` / `ConnectionOpenedAsync` to validate `SchemaId` against `^[a-z][a-z0-9_]{1,62}$` and execute `SET search_path TO "<SchemaId>";`.

### 4.2 Routing, Middleware & Authentication (`Restia.Web` & `Restia.Common`)

- **Routes (`Program.cs`)**:
  - Root `/` endpoint redirects authenticated users to `/{user.GetSchemaId()}` and unauthenticated users to `/public/Account/Login`.
  - Default controller route: `"{tenantId}/{controller=Home}/{action=Index}/{id?}"`.
  - Notification API route: `"{tenantId}/api/[controller]"` (with SignalR hub at `/notificationHub` reading `SchemaId` from `Context.User`).
- **`TenantResolutionMiddleware`**:
  - Skips static assets, `/Error`, and `/notificationHub`.
  - Reads `{tenantId}` from route values; validates format and active status via cached `ITenantService.IsValidActiveTenantAsync(tenantId)`. Returns `404` if invalid/inactive.
  - If `HttpContext.User` is authenticated, verifies `User.GetSchemaId() == tenantId` (case-insensitive); returns `404` on mismatch.
  - Sets `tenantContext.SetTenant(tenantId)`.
  - Note: Cookie authentication `OnValidatePrincipal` (`SecurityStampValidator`) runs during `UseAuthentication()`. To ensure `SecurityStampValidator` queries the right schema, `TenantResolutionMiddleware` sets `tenantContext` before `UseAuthentication()` (using route value `{tenantId}`), and enforces the authenticated user `SchemaId == tenantId` check right after `UseAuthentication()`.
- **Cookie Redirects (`CookieAuthenticationExtensions`)**:
  - `OnRedirectToLogin` and `OnRedirectToAccessDenied` extract `{tenantId}` from route values (fallback `"public"`) and redirect to `/{tenantId}/Account/Login` and `/{tenantId}/Account/AccessDenied`.

### 4.3 Tenant CRUD, Provisioning & Startup Seeding

- **Form & Authorization**:
  - `FormCodes.Tenant = "Tenant"` (`/Tenant`, icon `bi-building`, under `Administrator`).
  - `FormService` and `PermissionService` hide/block `Tenant` unless `tenantContext.SchemaId == "public"` and `user.IsSuperAdmin()`.
- **`TenantService` & `TenantProvisioningService`**:
  - Validates `Id` (`^[a-z][a-z0-9_]{1,62}$`, not reserved PostgreSQL system schema `pg_*` / `information_schema`).
  - Executes `CREATE SCHEMA IF NOT EXISTS "<id>"`, inserts `Tenant` into `public."Tenants"`, switches `ITenantContext` to `<id>`, runs `db.Database.MigrateAsync()`, seeds `Roles` (`SuperAdmin`, `Admin`, `Basic`), seeds `Forms`, and creates the `SuperAdmin` user with `SchemaId = id`.
  - Invalidates `IMemoryCache` active-tenant set on Create/Update/Delete.
- **Startup Seeding (`TenantStartupWorker`)**:
  - Migrates `public`, ensures the `"public"` record exists in `public."Tenants"`, seeds `Forms`, `Roles`, and default `SuperAdmin` (from `SeedAdminSettings`) in `public`.
  - Iterates over all active tenants in `public."Tenants"` to run `MigrateAsync()` and seed `Forms` and `Roles` idempotently.

### 4.4 Verification Plan

- Generate EF Core migration and verify `dotnet build` succeeds with 0 warnings and 0 errors.
- Verify startup migration & seeding for `public` and tenant creation flow on `/public/Tenant`.
