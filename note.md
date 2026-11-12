dotnet ef migrations add InitialCreate --project Restia.Common --startup-project Restia.Web

dotnet ef database update --project Restia.Common --startup-project Restia.Web

dotnet ef database update 0 --project Restia.Common --startup-project Restia.Web

dotnet ef migrations remove --project Restia.Common --startup-project Restia.Web

dotnet ef database drop --force --project Restia.Common --startup-project Restia.Web

dotnet tool update --global dotnet-ef --version 10.0.15