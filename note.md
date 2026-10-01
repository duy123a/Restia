dotnet ef migrations add InitialCreate --project Restia.Common --startup-project Restia.Web

dotnet ef database update --project Restia.Common --startup-project Restia.Web

dotnet ef database update 0

dotnet ef migrations remove

dotnet ef database drop --force