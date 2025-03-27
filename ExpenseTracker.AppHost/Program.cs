var builder = DistributedApplication.CreateBuilder(args);

// Add SQL Server container
var sqlPassword = builder.AddParameter("sqlPassword", Environment.GetEnvironmentVariable("SQL_PASSWORD") ?? throw new InvalidOperationException("SQL_PASSWORD environment variable not set."));
var sqlServer = builder.AddSqlServer("sql", sqlPassword)
    .AddDatabase("ExpenseTracker");

var apiService = builder.AddProject<Projects.ExpenseTracker_ApiService>("apiservice")
    .WithReference(sqlServer);

builder.AddProject<Projects.ExpenseTracker_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(apiService)
    .WaitFor(apiService);

builder.Build().Run();
