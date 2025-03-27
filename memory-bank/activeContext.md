# Active Context

## Current Focus

- Implementing the authentication and authorization system
- Creating the basic UI for expense entry and listing
- Testing the API endpoints
- Ensuring code quality through unit testing

## Recent Decisions

1. **Data Model Design**: Enhanced expense model to support various splitting scenarios and currency tracking
2. **Authentication Approach**: Using Google authentication with a simplified user management system
3. **Development Experience**: Implementing authentication bypass for local development
4. **Database Choice**: Using Azure SQL Database (free tier) for production
5. **Deployment Target**: Azure App Service for hosting the application
6. **Expense Model Simplification**: Removed redundant PartnerPercentage property as it can be calculated from YourPercentage
7. **Database Development**: Using SQL Server container for local development via Aspire
8. **SQL Server Configuration**: Updated SQL Server configuration to use parameter resource for password instead of string literal
9. **Testing Strategy**: Implemented unit tests for API controllers using xUnit, in-memory database, and Moq

## Open Questions

- Specific UI design and layout preferences
- Detailed category list for expenses
- Implementation of debt settlement tracking
- Reporting and visualization requirements

## Next Steps

1. Implement the authentication and authorization system
   - Add Google authentication
   - Create user management UI
   - Implement development bypass
2. Create the expense entry and listing UI
   - Design expense entry form
   - Implement expense listing with filtering
   - Add balance calculation and display
3. Test the API endpoints
   - Create test data
   - Verify CRUD operations
4. Implement the balance calculation functionality
