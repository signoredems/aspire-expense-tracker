namespace ExpenseTracker.ApiService.Models;

public enum PaymentSource
{
  You,
  Partner
}

public enum SplitType
{
  Equal,
  Custom,
  YouPay,
  PartnerPays
}

public class Expense
{
  public int Id { get; set; }
  public string Description { get; set; } = string.Empty;
  public DateTime Date { get; set; }
  public decimal Amount { get; set; }
  public PaymentSource PaidBy { get; set; }
  public SplitType SplitType { get; set; }
  public decimal? YourPercentage { get; set; }
  public string Currency { get; set; } = "USD";
  public string? Category { get; set; }
  public string? Notes { get; set; }
  public DateTime CreatedAt { get; set; }
  public DateTime? UpdatedAt { get; set; }
  public string CreatedBy { get; set; } = string.Empty;
}
