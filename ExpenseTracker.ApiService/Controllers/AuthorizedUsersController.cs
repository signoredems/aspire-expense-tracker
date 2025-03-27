using ExpenseTracker.ApiService.Data;
using ExpenseTracker.ApiService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.ApiService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthorizedUsersController : ControllerBase
{
  private readonly ApplicationDbContext _context;
  private readonly ILogger<AuthorizedUsersController> _logger;

  public AuthorizedUsersController(ApplicationDbContext context, ILogger<AuthorizedUsersController> logger)
  {
    _context = context;
    _logger = logger;
  }

  // GET: api/authorizedusers
  [HttpGet]
  public async Task<ActionResult<IEnumerable<AuthorizedUser>>> GetAuthorizedUsers()
  {
    return await _context.AuthorizedUsers.ToListAsync();
  }

  // GET: api/authorizedusers/5
  [HttpGet("{id}")]
  public async Task<ActionResult<AuthorizedUser>> GetAuthorizedUser(int id)
  {
    var authorizedUser = await _context.AuthorizedUsers.FindAsync(id);

    if (authorizedUser == null)
    {
      return NotFound();
    }

    return authorizedUser;
  }

  // POST: api/authorizedusers
  [HttpPost]
  public async Task<ActionResult<AuthorizedUser>> CreateAuthorizedUser(AuthorizedUser authorizedUser)
  {
    _context.AuthorizedUsers.Add(authorizedUser);
    await _context.SaveChangesAsync();

    return CreatedAtAction(nameof(GetAuthorizedUser), new { id = authorizedUser.Id }, authorizedUser);
  }

  // PUT: api/authorizedusers/5
  [HttpPut("{id}")]
  public async Task<IActionResult> UpdateAuthorizedUser(int id, AuthorizedUser authorizedUser)
  {
    if (id != authorizedUser.Id)
    {
      return BadRequest();
    }

    _context.Entry(authorizedUser).State = EntityState.Modified;

    try
    {
      await _context.SaveChangesAsync();
    }
    catch (DbUpdateConcurrencyException)
    {
      if (!AuthorizedUserExists(id))
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

  // DELETE: api/authorizedusers/5
  [HttpDelete("{id}")]
  public async Task<IActionResult> DeleteAuthorizedUser(int id)
  {
    var authorizedUser = await _context.AuthorizedUsers.FindAsync(id);
    if (authorizedUser == null)
    {
      return NotFound();
    }

    _context.AuthorizedUsers.Remove(authorizedUser);
    await _context.SaveChangesAsync();

    return NoContent();
  }

  private bool AuthorizedUserExists(int id)
  {
    return _context.AuthorizedUsers.Any(e => e.Id == id);
  }
}
