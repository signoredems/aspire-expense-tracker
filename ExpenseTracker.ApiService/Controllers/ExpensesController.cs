using ExpenseTracker.ApiService.Data;
using ExpenseTracker.ApiService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.ApiService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ExpensesController : ControllerBase
{
  private readonly ApplicationDbContext _context;
  private readonly ILogger<ExpensesController> _logger;

  public ExpensesController(ApplicationDbContext context, ILogger<ExpensesController> logger)
  {
    _context = context;
    _logger = logger;
  }

  // GET: api/expenses
  [HttpGet]
  public async Task<ActionResult<IEnumerable<Expense>>> GetExpenses()
  {
    return await _context.Expenses.ToListAsync();
  }

  // GET: api/expenses/5
  [HttpGet("{id}")]
  public async Task<ActionResult<Expense>> GetExpense(int id)
  {
    var expense = await _context.Expenses.FindAsync(id);

    if (expense == null)
    {
      return NotFound();
    }

    return expense;
  }

  // POST: api/expenses
  [HttpPost]
  public async Task<ActionResult<Expense>> CreateExpense(Expense expense)
  {
    expense.CreatedAt = DateTime.UtcNow;
    expense.CreatedBy = User.Identity?.Name ?? "System";

    _context.Expenses.Add(expense);
    await _context.SaveChangesAsync();

    return CreatedAtAction(nameof(GetExpense), new { id = expense.Id }, expense);
  }

  // PUT: api/expenses/5
  [HttpPut("{id}")]
  public async Task<IActionResult> UpdateExpense(int id, Expense expense)
  {
    if (id != expense.Id)
    {
      return BadRequest();
    }

    expense.UpdatedAt = DateTime.UtcNow;

    _context.Entry(expense).State = EntityState.Modified;
    _context.Entry(expense).Property(e => e.CreatedAt).IsModified = false;
    _context.Entry(expense).Property(e => e.CreatedBy).IsModified = false;

    try
    {
      await _context.SaveChangesAsync();
    }
    catch (DbUpdateConcurrencyException)
    {
      if (!ExpenseExists(id))
      {
        return NotFound();
      }
      else
      {
        throw;
      }
    }

    return NoContent();
  }

  // DELETE: api/expenses/5
  [HttpDelete("{id}")]
  public async Task<IActionResult> DeleteExpense(int id)
  {
    var expense = await _context.Expenses.FindAsync(id);
    if (expense == null)
    {
      return NotFound();
    }

    _context.Expenses.Remove(expense);
    await _context.SaveChangesAsync();

    return NoContent();
  }

  private bool ExpenseExists(int id)
  {
    return _context.Expenses.Any(e => e.Id == id);
  }
}
