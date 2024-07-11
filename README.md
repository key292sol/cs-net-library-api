# Library Management API

Library Management system API created using C# .NET

## To Run:

1. Run the SQL code from `database_stuff.sql` file to create and populate the database and tables
1. Create `appsettings.json` file like a normal .net project
1. Add the values for `ConnectionStrings`, `AppSettings.PasswordKey` and `AppSettings.TokenKey` in the file as shown below:

   ```json
   	"ConnectionStrings": {
   		"Default": "Your connection string"
   	},
   	"AppSettings": {
   		"PasswordKey": "Random key string",
   		"TokenKey": "Random key string"
   	}
   ```

   The Connection string will be like - `Server=localhost; Database=LibraryProjectDatabase; Trusted_Connection=true; TrustServerCertificate=true`

1. Install the packages inside of the file `LibrarySystem.csproj`
1. Run the project using `dotnet run` or `dotnet watch run`

## TODO:

- Role Based Access
- Loan Overdue notification
- Loan extension
