tool install --global dotnet-ef

dotnet ef migrations add InitialMigration --project DAL --startup-project ConsoleApp 

(Optional)  dotnet ef migrations remove --project DAL --startup-project ConsoleApp

dotnet ef database update --project DAL --startup-project ConsoleApp