# Restia

## DB Migrations

### 1. Install tool:

```bash
dotnet tool install --global dotnet-ef
dotnet tool update --global dotnet-ef
```

### 2. Open Terminal in Restia.WebApi

```bash
dotnet ef migrations add InitialMigrations --project ../../Migrators/Restia.Migrators.MsSQL/ --context ApplicationDbContext -o Migrations/Application
```

```bash
dotnet ef database update --context ApplicationDbContext
```

```bash
dotnet ef migrations add InitialMigrations --project ../../Migrators/Restia.Migrators.MsSQL/ --context TenantDbContext -o Migrations/Tenant
```

```bash
dotnet ef database update --context TenantDbContext
```

If the migrations are already exist, just type

```bash
dotnet ef database update --context ApplicationDbContext
dotnet ef database update --context TenantDbContext
```
