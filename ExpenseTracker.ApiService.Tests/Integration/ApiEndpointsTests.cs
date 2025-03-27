using ExpenseTracker.ApiService.Data;
using ExpenseTracker.ApiService.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Xunit;

namespace ExpenseTracker.ApiService.Tests.Integration;

public class ApiEndpointsTests : IClassFixture<WebApplicationFactory<Program>>
{
  private readonly WebApplicationFactory<Program> _factory;
  private readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };

  public ApiEndpointsTests(WebApplicationFactory<Program> factory)
  {
    _factory = factory.WithWebHostBuilder(builder =>
    {
      builder.ConfigureServices(services =>
      {
        // Remove the app's ApplicationDbContext registration
        var descriptor = services.SingleOrDefault(
          d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));

        if (descriptor != null)
        {
          services.Remove(descriptor);
        }

        // Add ApplicationDbContext using an in-memory database for testing
        services.AddDbContext<ApplicationDbContext>(options =>
        {
          options.UseInMemoryDatabase("InMemoryDbForTesting");
        });

        // Build the service provider
        var sp = services.BuildServiceProvider();

        // Create a scope to obtain a reference to the database context
        using var scope = sp.CreateScope();
        var scopedServices = scope.ServiceProvider;
        var db = scopedServices.GetRequiredService<ApplicationDbContext>();

        // Ensure the database is created
        db.Database.EnsureCreated();

        // Seed the database with test data
        SeedDatabase(db);
      });
    });
  }

  private void SeedDatabase(ApplicationDbContext context)
  {
    // Add test expenses
    context.Expenses.AddRange(
      new Expense
      {
        Description = "Test Expense 1",
        Date = DateTime.UtcNow.AddDays(-1),
        Amount = 100.00m,
        PaidBy = PaymentSource.You,
        SplitType = SplitType.Equal,
        Currency = "USD",
        Category = "Test",
        CreatedAt = DateTime.UtcNow,
        CreatedBy = "Test User"
      },
      new Expense
      {
        Description = "Test Expense 2",
        Date = DateTime.UtcNow.AddDays(-2),
        Amount = 200.00m,
        PaidBy = PaymentSource.Partner,
        SplitType = SplitType.Equal,
        Currency = "USD",
        Category = "Test",
        CreatedAt = DateTime.UtcNow,
        CreatedBy = "Test User"
      }
    );

    // Add test users
    context.AuthorizedUsers.AddRange(
      new AuthorizedUser
      {
        Email = "admin@example.com",
        Name = "Admin User",
        IsAdmin = true
      },
      new AuthorizedUser
      {
        Email = "user@example.com",
        Name = "Regular User",
        IsAdmin = false
      }
    );

    context.SaveChanges();
  }

  [Fact]
  public async Task GetExpenses_ReturnsSuccessAndAllExpenses()
  {
    // Arrange
    HttpClient client = _factory.CreateClient();

    // Act
    var response = await client.GetAsync("/api/expenses");

    // Assert
    response.EnsureSuccessStatusCode(); // Status Code 200-299
    List<Expense>? expenses = await response.Content.ReadFromJsonAsync<List<Expense>>(_jsonOptions);
    Assert.NotNull(expenses);
    Assert.Equal(2, expenses.Count);
  }

  [Fact]
  public async Task GetExpenseById_WithValidId_ReturnsExpense()
  {
    // Arrange
    HttpClient client = _factory.CreateClient();

    // First get all expenses to find a valid ID
    var allExpensesResponse = await client.GetAsync("/api/expenses");
    allExpensesResponse.EnsureSuccessStatusCode();
    List<Expense>? expenses = await allExpensesResponse.Content.ReadFromJsonAsync<List<Expense>>(_jsonOptions);
    Assert.NotNull(expenses);
    Assert.NotEmpty(expenses);
    var firstExpenseId = expenses[0].Id;

    // Act
    var response = await client.GetAsync($"/api/expenses/{firstExpenseId}");

    // Assert
    response.EnsureSuccessStatusCode();
    Expense? expense = await response.Content.ReadFromJsonAsync<Expense>(_jsonOptions);
    Assert.NotNull(expense);
    Assert.Equal(firstExpenseId, expense.Id);
  }

  [Fact]
  public async Task GetExpenseById_WithInvalidId_ReturnsNotFound()
  {
    // Arrange
    HttpClient client = _factory.CreateClient();
    var invalidId = 9999;

    // Act
    var response = await client.GetAsync($"/api/expenses/{invalidId}");

    // Assert
    Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
  }

  [Fact]
  public async Task CreateExpense_AddsNewExpense()
  {
    // Arrange
    HttpClient client = _factory.CreateClient();
    Expense newExpense = new()
    {
      Description = "New Test Expense",
      Date = DateTime.UtcNow,
      Amount = 150.00m,
      PaidBy = PaymentSource.You,
      SplitType = SplitType.Equal,
      Currency = "USD",
      Category = "Test"
    };

    StringContent content = new(
      JsonSerializer.Serialize(newExpense),
      Encoding.UTF8,
      "application/json");

    // Act
    var response = await client.PostAsync("/api/expenses", content);

    // Assert
    response.EnsureSuccessStatusCode();
    Expense? createdExpense = await response.Content.ReadFromJsonAsync<Expense>(_jsonOptions);
    Assert.NotNull(createdExpense);
    Assert.Equal(newExpense.Description, createdExpense.Description);
    Assert.Equal(newExpense.Amount, createdExpense.Amount);
  }

  [Fact]
  public async Task GetAuthorizedUsers_ReturnsSuccessAndAllUsers()
  {
    // Arrange
    HttpClient client = _factory.CreateClient();

    // Act
    var response = await client.GetAsync("/api/authorizedusers");

    // Assert
    response.EnsureSuccessStatusCode();
    List<AuthorizedUser>? users = await response.Content.ReadFromJsonAsync<List<AuthorizedUser>>(_jsonOptions);
    Assert.NotNull(users);
    Assert.Equal(2, users.Count);
  }

  [Fact]
  public async Task GetAuthorizedUserById_WithValidId_ReturnsUser()
  {
    // Arrange
    HttpClient client = _factory.CreateClient();

    // First get all users to find a valid ID
    var allUsersResponse = await client.GetAsync("/api/authorizedusers");
    allUsersResponse.EnsureSuccessStatusCode();
    List<AuthorizedUser>? users = await allUsersResponse.Content.ReadFromJsonAsync<List<AuthorizedUser>>(_jsonOptions);
    Assert.NotNull(users);
    Assert.NotEmpty(users);
    var firstUserId = users[0].Id;

    // Act
    var response = await client.GetAsync($"/api/authorizedusers/{firstUserId}");

    // Assert
    response.EnsureSuccessStatusCode();
    AuthorizedUser? user = await response.Content.ReadFromJsonAsync<AuthorizedUser>(_jsonOptions);
    Assert.NotNull(user);
    Assert.Equal(firstUserId, user.Id);
  }

  [Fact]
  public async Task CreateAuthorizedUser_AddsNewUser()
  {
    // Arrange
    HttpClient client = _factory.CreateClient();
    AuthorizedUser newUser = new()
    {
      Email = "newuser@example.com",
      Name = "New Test User",
      IsAdmin = false
    };

    StringContent content = new(
      JsonSerializer.Serialize(newUser),
      Encoding.UTF8,
      "application/json");

    // Act
    var response = await client.PostAsync("/api/authorizedusers", content);

    // Assert
    response.EnsureSuccessStatusCode();
    AuthorizedUser? createdUser = await response.Content.ReadFromJsonAsync<AuthorizedUser>(_jsonOptions);
    Assert.NotNull(createdUser);
    Assert.Equal(newUser.Email, createdUser.Email);
    Assert.Equal(newUser.Name, createdUser.Name);
    Assert.Equal(newUser.IsAdmin, createdUser.IsAdmin);
  }
}
