using ExpenseTracker.ApiService.Controllers;
using ExpenseTracker.ApiService.Data;
using ExpenseTracker.ApiService.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace ExpenseTracker.ApiService.Tests.Controllers;

public class ExpensesControllerTests : TestBase
{
  [Fact]
  public async Task GetExpenses_ReturnsAllExpenses()
  {
    // Arrange
    using var context = CreateDbContext();
    var logger = CreateLogger<ExpensesController>();
    var controller = new ExpensesController(context, logger);

    var expenses = new List<Expense>
    {
      new() {
        Description = "Dinner",
        Date = DateTime.UtcNow.AddDays(-1),
        Amount = 50.00m,
        PaidBy = PaymentSource.You,
        SplitType = SplitType.Equal,
        Currency = "USD",
        Category = "Food",
        CreatedAt = DateTime.UtcNow,
        CreatedBy = "Test User"
      },
      new() {
        Description = "Movie tickets",
        Date = DateTime.UtcNow.AddDays(-2),
        Amount = 30.00m,
        PaidBy = PaymentSource.Partner,
        SplitType = SplitType.Equal,
        Currency = "USD",
        Category = "Entertainment",
        CreatedAt = DateTime.UtcNow,
        CreatedBy = "Test User"
      }
    };

    await context.Expenses.AddRangeAsync(expenses);
    await context.SaveChangesAsync();

    // Act
    var result = await controller.GetExpenses();

    // Assert
    var actionResult = Assert.IsType<ActionResult<IEnumerable<Expense>>>(result);
    var returnValue = Assert.IsAssignableFrom<IEnumerable<Expense>>(actionResult.Value);
    Assert.Equal(2, returnValue.Count());
  }

  [Fact]
  public async Task GetExpense_WithValidId_ReturnsExpense()
  {
    // Arrange
    using var context = CreateDbContext();
    var logger = CreateLogger<ExpensesController>();
    var controller = new ExpensesController(context, logger);

    var expense = new Expense
    {
      Description = "Dinner",
      Date = DateTime.UtcNow.AddDays(-1),
      Amount = 50.00m,
      PaidBy = PaymentSource.You,
      SplitType = SplitType.Equal,
      Currency = "USD",
      Category = "Food",
      CreatedAt = DateTime.UtcNow,
      CreatedBy = "Test User"
    };

    await context.Expenses.AddAsync(expense);
    await context.SaveChangesAsync();

    // Act
    var result = await controller.GetExpense(expense.Id);

    // Assert
    var actionResult = Assert.IsType<ActionResult<Expense>>(result);
    var returnValue = Assert.IsType<Expense>(actionResult.Value);
    Assert.Equal(expense.Id, returnValue.Id);
    Assert.Equal(expense.Description, returnValue.Description);
  }

  [Fact]
  public async Task GetExpense_WithInvalidId_ReturnsNotFound()
  {
    // Arrange
    using var context = CreateDbContext();
    var logger = CreateLogger<ExpensesController>();
    var controller = new ExpensesController(context, logger);

    // Act
    var result = await controller.GetExpense(999);

    // Assert
    Assert.IsType<NotFoundResult>(result.Result);
  }

  [Fact]
  public async Task CreateExpense_AddsExpenseToDatabase()
  {
    // Arrange
    using var context = CreateDbContext();
    var logger = CreateLogger<ExpensesController>();
    var controller = new ExpensesController(context, logger);

    // Setup user identity
    var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
    {
      new(ClaimTypes.Name, "test@example.com")
    }));
    controller.ControllerContext = new ControllerContext
    {
      HttpContext = new DefaultHttpContext { User = user }
    };

    var newExpense = new Expense
    {
      Description = "Groceries",
      Date = DateTime.UtcNow,
      Amount = 75.50m,
      PaidBy = PaymentSource.You,
      SplitType = SplitType.Equal,
      Currency = "USD",
      Category = "Food"
    };

    // Act
    var result = await controller.CreateExpense(newExpense);

    // Assert
    var actionResult = Assert.IsType<ActionResult<Expense>>(result);
    var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
    var returnValue = Assert.IsType<Expense>(createdAtActionResult.Value);

    Assert.Equal(newExpense.Description, returnValue.Description);
    Assert.Equal(newExpense.Amount, returnValue.Amount);
    Assert.Equal("test@example.com", returnValue.CreatedBy);

    // Verify it was added to the database
    Assert.Equal(1, await context.Expenses.CountAsync());
  }

  [Fact]
  public async Task UpdateExpense_WithValidId_UpdatesExpense()
  {
    // Arrange
    using var context = CreateDbContext();
    var logger = CreateLogger<ExpensesController>();
    var controller = new ExpensesController(context, logger);

    var expense = new Expense
    {
      Description = "Dinner",
      Date = DateTime.UtcNow.AddDays(-1),
      Amount = 50.00m,
      PaidBy = PaymentSource.You,
      SplitType = SplitType.Equal,
      Currency = "USD",
      Category = "Food",
      CreatedAt = DateTime.UtcNow,
      CreatedBy = "Test User"
    };

    await context.Expenses.AddAsync(expense);
    await context.SaveChangesAsync();

    // Detach the entity from the context to avoid tracking conflicts
    context.Entry(expense).State = EntityState.Detached;

    var updatedExpense = new Expense
    {
      Id = expense.Id,
      Description = "Fancy Dinner",
      Date = expense.Date,
      Amount = 75.00m,
      PaidBy = expense.PaidBy,
      SplitType = expense.SplitType,
      Currency = expense.Currency,
      Category = expense.Category,
      CreatedAt = expense.CreatedAt,
      CreatedBy = expense.CreatedBy
    };

    // Act
    var result = await controller.UpdateExpense(expense.Id, updatedExpense);

    // Assert
    Assert.IsType<NoContentResult>(result);

    // Verify the update in the database
    var dbExpense = await context.Expenses.FindAsync(expense.Id);
    Assert.NotNull(dbExpense);
    Assert.Equal("Fancy Dinner", dbExpense.Description);
    Assert.Equal(75.00m, dbExpense.Amount);
    Assert.NotNull(dbExpense.UpdatedAt);
  }

  [Fact]
  public async Task DeleteExpense_WithValidId_RemovesExpense()
  {
    // Arrange
    using var context = CreateDbContext();
    var logger = CreateLogger<ExpensesController>();
    var controller = new ExpensesController(context, logger);

    var expense = new Expense
    {
      Description = "Dinner",
      Date = DateTime.UtcNow.AddDays(-1),
      Amount = 50.00m,
      PaidBy = PaymentSource.You,
      SplitType = SplitType.Equal,
      Currency = "USD",
      Category = "Food",
      CreatedAt = DateTime.UtcNow,
      CreatedBy = "Test User"
    };

    await context.Expenses.AddAsync(expense);
    await context.SaveChangesAsync();

    // Act
    var result = await controller.DeleteExpense(expense.Id);

    // Assert
    Assert.IsType<NoContentResult>(result);

    // Verify it was removed from the database
    Assert.Null(await context.Expenses.FindAsync(expense.Id));
  }

  [Fact]
  public async Task DeleteExpense_WithInvalidId_ReturnsNotFound()
  {
    // Arrange
    using var context = CreateDbContext();
    var logger = CreateLogger<ExpensesController>();
    var controller = new ExpensesController(context, logger);

    // Act
    var result = await controller.DeleteExpense(999);

    // Assert
    Assert.IsType<NotFoundResult>(result);
  }
}
