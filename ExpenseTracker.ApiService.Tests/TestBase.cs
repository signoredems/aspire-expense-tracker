using ExpenseTracker.ApiService.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace ExpenseTracker.ApiService.Tests;

public abstract class TestBase
{
  protected ApplicationDbContext CreateDbContext()
  {
    var options = new DbContextOptionsBuilder<ApplicationDbContext>()
        .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
        .Options;

    var context = new ApplicationDbContext(options);
    context.Database.EnsureCreated();
    return context;
  }

  protected ILogger<T> CreateLogger<T>()
  {
    return new Mock<ILogger<T>>().Object;
  }
}
