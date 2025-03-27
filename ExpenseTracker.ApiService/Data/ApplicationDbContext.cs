using ExpenseTracker.ApiService.Models;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.ApiService.Data;

public class ApplicationDbContext : DbContext
{
  public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : base(options)
  {
  }

  public DbSet<Expense> Expenses { get; set; } = null!;
  public DbSet<AuthorizedUser> AuthorizedUsers { get; set; } = null!;

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    base.OnModelCreating(modelBuilder);

    // Configure Expense entity
    modelBuilder.Entity<Expense>(entity =>
    {
      entity.HasKey(e => e.Id);
      entity.Property(e => e.Description).IsRequired().HasMaxLength(200);
      entity.Property(e => e.Date).IsRequired();
      entity.Property(e => e.Amount).IsRequired().HasPrecision(18, 2);
      entity.Property(e => e.YourPercentage).HasPrecision(5, 2);
      entity.Property(e => e.Currency).IsRequired().HasMaxLength(3).HasDefaultValue("USD");
      entity.Property(e => e.Category).HasMaxLength(100);
      entity.Property(e => e.CreatedAt).IsRequired().HasDefaultValueSql("GETUTCDATE()");
      entity.Property(e => e.CreatedBy).IsRequired().HasMaxLength(100);
    });

    // Configure AuthorizedUser entity
    modelBuilder.Entity<AuthorizedUser>(entity =>
    {
      entity.HasKey(e => e.Id);
      entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
      entity.Property(e => e.Name).HasMaxLength(100);
      entity.Property(e => e.IsAdmin).HasDefaultValue(false);
    });
  }
}
