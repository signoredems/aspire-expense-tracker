# Project Progress

## Completed

- Initial project planning
- Architecture design
- Data model design
- Authentication strategy
- Deployment strategy planning
- Memory bank creation
- Project structure setup
- Entity model implementation
- Database setup
- API endpoint creation
- Unit tests for API controllers

## In Progress

- Authentication implementation
- Integration testing
  - Resolving issues with missing `testhost.deps.json` file
  - Investigating test runner configuration

## Pending

- UI development
- Deployment configuration

## Known Issues

- ~~SQL Server configuration in AppHost using string literal for password instead of parameter resource~~ (Fixed)
- Integration tests failing due to missing `testhost.deps.json` file
  - `PreserveCompilationContext` property is set to `true` in the test project
  - Error: `System.ArgumentException : Argument --parentprocessid was not specified`

## Milestones

- [x] Project structure setup
- [x] Database and models implementation
- [ ] Authentication and authorization
- [ ] Basic expense tracking functionality
- [ ] Expense splitting and balance calculation
- [ ] UI refinement
- [ ] Deployment to Azure
