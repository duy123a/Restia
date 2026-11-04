# Role & Permission Management – Design

Status: Approved (2026-11-03)

## 1. Understanding Summary

- **Role page** `/Role`: Index (search/sort/paging like User) + Create/Edit (name only) + Delete.
- **Permission page** `/Permission`: tabs Role | User + target dropdown → permission matrix.
- Matrix rows = active leaf Forms (with Url) grouped by parent Form. Columns = 3 required radios
  `Available / ReferablePrintable / Unavailable` (default `Unavailable`).
- Stored in separate tables `RolePermission` / `UserPermission`. Evaluation: User permission first, then Role.
- `Default` is only a policy (`Form:X`) meaning "not Unavailable"; not part of the matrix.
- Seed Form `Permission` in `FormSeedWorker`.
- Review & clean existing code and CSS.

## 2. Assumptions

- Small scale (< 100 forms, hundreds of users) → no permission cache; single query per check.
- Permission change → `UpdateSecurityStamp` of affected users (cookie refresh).
- Policies: view = `Form:<Code>`, modify = `Form:<Code>:Available` (Role, Permission).
- SuperAdmin bypasses all checks; SuperAdmin role / users in it are shown read-only in the matrix.
- Last-write-wins concurrency is acceptable (internal admin tool).
- No xUnit project for now; verification = build + manual test.

## 3. Decision Log

| # | Decision | Alternatives | Reason |
|---|----------|--------------|--------|
| D1 | Permission page = assignment matrix | CRUD Form catalog; both | User choice |
| D2 | 3 required radios, no `None` column | 4 columns incl. "Inherit" | User choice; User overrides Role fully |
| D3 | Tabs Role/User + dropdown, Role page name-only | Embed matrix in Role edit; separate pages | Single place to manage permissions |
| D4 | Server-rendered GET form + PRG POST | AJAX partial | Less JS, MVC validation/antiforgery, bookmarkable |
| D5 | Role permission lookup by role **name** (join `Roles`) | Put role ids in claims | Fixes bug with minimal change |
| D6 | `UserPermission` no longer soft-deletable (migration) | Keep soft delete | Composite PK + soft delete blocks re-insert; aligns with `RolePermission` |
| D7 | Idempotent seed per Form `Code` | Seed only on empty DB | Existing DBs receive new forms |
| D8 | Generic CSS class names (`page-*`, `data-table`) | Per-page copies | Reuse across User/Role/Permission |
| D9 | No xUnit project | Add tests for `PermissionService` | User choice (deferred) |

## 4. Design

### 4.1 Data & Backend

- `UserPermission`: drop `BaseEntity` inheritance → `UserId, FormCode, FormAuthoritySec`. Migration drops audit/soft-delete/RV columns.
- Delete Role/User → also delete their permission rows (same transaction).
- `FormSeedWorker`: static list `(Code, Name, Url, Icon, ParentCode, Order)`; insert missing codes only. Adds `Permission` (`/Permission`, `bi-shield-check`, order 3).
- `PermissionConstants`: `User`, `Role`, `Permission` with same pattern.
- `PermissionService`:
  - `GetAuthoritySecAsync(userId, roleNames, formCode)`: user record first; else role records joined by role name. Priority Unavailable > Available > ReferablePrintable; none → Unavailable.
  - `PermissionHandler` delegates to `HasPermissionAsync` (no duplicated logic).
  - `GetMatrixAsync(type, id)`, `SaveMatrixAsync(type, id, items)` (validate target, block SuperAdmin, filter unknown FormCode, reject `None`, execution strategy + transaction, replace rows, bump security stamps), `GetTargetOptionsAsync(type)`.
- Fixes: `RoleService` uses execution strategy; `UserController.Delete` passes `User.GetLoginId()`.

### 4.2 Web

- `RoleController`: Index / Create / Edit / Delete; shared `_RoleForm.cshtml`; SuperAdmin cannot be renamed/deleted.
- `PermissionController`: `GET Index(type, id)`, `POST Save` → PRG with TempData.
- Matrix view: group rows (parent, colspan) + leaf rows with radios; "select whole column" header buttons; read-only when no modify permission or SuperAdmin target.
- `_TableActions`: optional `PermissionUrl` (shield icon) on Role/User Index.
- `site.js`: `[data-auto-submit]`, `[data-select-column]`.
- Localization: `Role_*`, `Permission_*` (page strings), `Form_Permission` (menu/matrix label), `FormAuthoritySec_*`.

### 4.3 CSS

- Rename `user-*` layout classes to generic names; column widths via table modifier.
- Tokens: `--focus-ring`, `--overlay-bg`; remove repeated focus-ring declarations.
- Replace `@import` with `<link>`; reduce `!important` / ID selectors; a11y-safe focus on `#sidebarToggle`; merge duplicate sidebar selectors; flat indentation; delete empty `_Layout.cshtml.css`.
- New: `.permission-matrix`, `.page-tabs`.

### 4.4 Edge Cases

- Unknown `id`/`type` → 404. Unknown FormCode ignored; missing rows → Unavailable; invalid Sec → validation error.
- Inactive forms hidden; their stored rows untouched.

### 4.5 Verification

- `dotnet build`, apply migration, manual: Role CRUD, matrix Role/User, user-overrides-role, role-by-name check works, sidebar visibility, read-only states.
