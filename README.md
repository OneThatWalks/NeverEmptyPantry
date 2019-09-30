# Never Empty Pantry

This is the home page for the NeverEmptyPantry application.  Never Empty Pantry is a side project in ASP.NET Core developed by Darrien Singleton.

## Getting Started

### Prerequisites

- ASP.NET Core Tools (Using dotnet core 3.0)
- SQL Server (This project uses entity framework core)

Once you have downloaded the tools and the source run these dotnet commands against the directory

```
dotnet restore

dotnet build

// To migrate the database use `dotnet migrate NeverEmptyPantry.Repository` command

dotnet run "project"
```

This will start a server depending on the project specified.  Navigate to the address in a new browser window and you are ready to go.

The API is easily tested using Postman (Postman colelction coming soon...)