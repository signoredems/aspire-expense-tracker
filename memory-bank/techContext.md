# Technical Context

## Technology Stack

- **.NET 9**: Core framework for backend and frontend
- **.NET Aspire**: Cloud-ready distributed application framework
- **Blazor Server**: Server-side UI framework with interactive components
- **Entity Framework Core**: ORM for database operations
- **Azure SQL Database**: Cloud database (free tier)
- **Azure App Service**: Web application hosting
- **Google Authentication**: External identity provider
- **xUnit**: Testing framework for unit and integration tests
- **Moq**: Mocking framework for unit tests
- **Entity Framework Core InMemory**: In-memory database provider for testing
- **Microsoft.AspNetCore.Mvc.Testing**: Framework for integration testing of ASP.NET Core applications

## Development Environment

- **Visual Studio Code**: Primary development environment
- **Local SQL Server/SQLite**: For development database
- **Aspire Dashboard**: Local development monitoring
- **Aspire Parameter Resources**: For managing sensitive configuration values, including the use of environment variables for storing sensitive information.
- **Local .env files**: Recommended for storing environment variables during local development. Ensure these files are added to .gitignore and never committed to source control.
- **Test Runner**: xUnit test runner with integration test support

## Testing Configuration

- **Unit Tests**: Standard xUnit tests with in-memory database and mocking
- **Integration Tests**: Using WebApplicationFactory to create a test server
- **Test Project Configuration**:
  - PreserveCompilationContext set to true
  - Required dependencies: Microsoft.AspNetCore.Mvc.Testing, Microsoft.EntityFrameworkCore.InMemory
- **Known Issues**:
  - Missing testhost.deps.json file causing integration test failures
  - Error: System.ArgumentException : Argument --parentprocessid was not specified

## Key Dependencies

- Microsoft.EntityFrameworkCore.SqlServer
- Microsoft.AspNetCore.Authentication.Google
- Microsoft.AspNetCore.Components.Server
- Microsoft.Extensions.Diagnostics
- Microsoft.FluentUI.AspNetCore.Components

## Database Schema

```sql
CREATE TABLE Expenses (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Description NVARCHAR(200) NOT NULL,
    Date DATETIME2 NOT NULL,
    Amount DECIMAL(18,2) NOT NULL,
    PaidBy INT NOT NULL,
    SplitType INT NOT NULL DEFAULT 0,
    YourPercentage DECIMAL(5,2) NULL,
    PartnerPercentage DECIMAL(5,2) NULL,
    Currency NVARCHAR(3) NOT NULL DEFAULT 'USD',
    Category NVARCHAR(100) NULL,
    Notes NVARCHAR(MAX) NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NULL,
    CreatedBy NVARCHAR(100) NOT NULL
);

CREATE TABLE AuthorizedUsers (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Email NVARCHAR(100) NOT NULL,
    Name NVARCHAR(100) NULL,
    IsAdmin BIT NOT NULL DEFAULT 0
);
```

## Deployment Strategy

- Azure App Service for hosting
- Azure SQL Database for data storage
- Configuration stored in Azure App Configuration
- Authentication using Google OAuth
