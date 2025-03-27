using ExpenseTracker.ApiService.Controllers;
using ExpenseTracker.ApiService.Data;
using ExpenseTracker.ApiService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace ExpenseTracker.ApiService.Tests.Controllers;

public class AuthorizedUsersControllerTests : TestBase
{
  [Fact]
  public async Task GetAuthorizedUsers_ReturnsAllUsers()
  {
    // Arrange
    using var context = CreateDbContext();
    var logger = CreateLogger<AuthorizedUsersController>();
    var controller = new AuthorizedUsersController(context, logger);

    List<AuthorizedUser> users = new()
    {
      new() {
        Email = "user1@example.com",
        Name = "User One",
        IsAdmin = true
      },
      new() {
        Email = "user2@example.com",
        Name = "User Two",
        IsAdmin = false
      }
    };

    await context.AuthorizedUsers.AddRangeAsync(users);
    await context.SaveChangesAsync();

    // Act
    var result = await controller.GetAuthorizedUsers();

    // Assert
    var actionResult = Assert.IsType<ActionResult<IEnumerable<AuthorizedUser>>>(result);
    var returnValue = Assert.IsAssignableFrom<IEnumerable<AuthorizedUser>>(actionResult.Value);
    Assert.Equal(2, returnValue.Count());
  }

  [Fact]
  public async Task GetAuthorizedUser_WithValidId_ReturnsUser()
  {
    // Arrange
    using var context = CreateDbContext();
    var logger = CreateLogger<AuthorizedUsersController>();
    var controller = new AuthorizedUsersController(context, logger);

    AuthorizedUser user = new()
    {
      Email = "user1@example.com",
      Name = "User One",
      IsAdmin = true
    };

    await context.AuthorizedUsers.AddAsync(user);
    await context.SaveChangesAsync();

    // Act
    var result = await controller.GetAuthorizedUser(user.Id);

    // Assert
    var actionResult = Assert.IsType<ActionResult<AuthorizedUser>>(result);
    var returnValue = Assert.IsType<AuthorizedUser>(actionResult.Value);
    Assert.Equal(user.Id, returnValue.Id);
    Assert.Equal(user.Email, returnValue.Email);
    Assert.Equal(user.Name, returnValue.Name);
    Assert.Equal(user.IsAdmin, returnValue.IsAdmin);
  }

  [Fact]
  public async Task GetAuthorizedUser_WithInvalidId_ReturnsNotFound()
  {
    // Arrange
    using var context = CreateDbContext();
    var logger = CreateLogger<AuthorizedUsersController>();
    var controller = new AuthorizedUsersController(context, logger);

    // Act
    var result = await controller.GetAuthorizedUser(999);

    // Assert
    Assert.IsType<NotFoundResult>(result.Result);
  }

  [Fact]
  public async Task CreateAuthorizedUser_AddsUserToDatabase()
  {
    // Arrange
    using var context = CreateDbContext();
    var logger = CreateLogger<AuthorizedUsersController>();
    var controller = new AuthorizedUsersController(context, logger);

    var newUser = new AuthorizedUser
    {
      Email = "newuser@example.com",
      Name = "New User",
      IsAdmin = false
    };

    // Act
    var result = await controller.CreateAuthorizedUser(newUser);

    // Assert
    var actionResult = Assert.IsType<ActionResult<AuthorizedUser>>(result);
    var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
    var returnValue = Assert.IsType<AuthorizedUser>(createdAtActionResult.Value);

    Assert.Equal(newUser.Email, returnValue.Email);
    Assert.Equal(newUser.Name, returnValue.Name);
    Assert.Equal(newUser.IsAdmin, returnValue.IsAdmin);

    // Verify it was added to the database
    Assert.Equal(1, await context.AuthorizedUsers.CountAsync());
  }

  [Fact]
  public async Task UpdateAuthorizedUser_WithValidId_UpdatesUser()
  {
    // Arrange
    using var context = CreateDbContext();
    var logger = CreateLogger<AuthorizedUsersController>();
    var controller = new AuthorizedUsersController(context, logger);

    var user = new AuthorizedUser
    {
      Email = "user@example.com",
      Name = "User",
      IsAdmin = false
    };

    await context.AuthorizedUsers.AddAsync(user);
    await context.SaveChangesAsync();

    // Detach the entity from the context to avoid tracking conflicts
    context.Entry(user).State = EntityState.Detached;

    var updatedUser = new AuthorizedUser
    {
      Id = user.Id,
      Email = user.Email,
      Name = "Updated User",
      IsAdmin = true
    };

    // Act
    var result = await controller.UpdateAuthorizedUser(user.Id, updatedUser);

    // Assert
    Assert.IsType<NoContentResult>(result);

    // Verify the update in the database
    var dbUser = await context.AuthorizedUsers.FindAsync(user.Id);
    Assert.NotNull(dbUser);
    Assert.Equal("Updated User", dbUser.Name);
    Assert.True(dbUser.IsAdmin);
  }

  [Fact]
  public async Task DeleteAuthorizedUser_WithValidId_RemovesUser()
  {
    // Arrange
    using var context = CreateDbContext();
    var logger = CreateLogger<AuthorizedUsersController>();
    var controller = new AuthorizedUsersController(context, logger);

    var user = new AuthorizedUser
    {
      Email = "user@example.com",
      Name = "User",
      IsAdmin = false
    };

    await context.AuthorizedUsers.AddAsync(user);
    await context.SaveChangesAsync();

    // Act
    var result = await controller.DeleteAuthorizedUser(user.Id);

    // Assert
    Assert.IsType<NoContentResult>(result);

    // Verify it was removed from the database
    Assert.Null(await context.AuthorizedUsers.FindAsync(user.Id));
  }

  [Fact]
  public async Task DeleteAuthorizedUser_WithInvalidId_ReturnsNotFound()
  {
    // Arrange
    using var context = CreateDbContext();
    var logger = CreateLogger<AuthorizedUsersController>();
    var controller = new AuthorizedUsersController(context, logger);

    // Act
    var result = await controller.DeleteAuthorizedUser(999);

    // Assert
    Assert.IsType<NotFoundResult>(result);
  }
}
