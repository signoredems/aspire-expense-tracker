namespace ExpenseTracker.ApiService.Models;

public class AuthorizedUser
{
  public int Id { get; set; }
  public string Email { get; set; } = string.Empty;
  public string Name { get; set; } = string.Empty;
  public bool IsAdmin { get; set; }
}
