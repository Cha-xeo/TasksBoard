appsettings.Development.json should ressemble something like that where it links to your local mysql database.
TaskDatabase will be used in Develpoment environment.

{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "TaskDatabase": "server=localhost;user=;password=;database=",
  }
}

Please note that appsettings.Development.json shoould never be pushed
