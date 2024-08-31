using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; // Add this using statement
using API.Models;
using API.Data;
namespace API.Controllers; // Remove the semicolon from the namespace declaration

[ApiController]
[Route("api/[controller]")] // Remove the extra '[' character
public class UsersController : ControllerBase
{
    private readonly DataContext _context;
    public UsersController(DataContext context)
    {
        _context = context;
    }

    [HttpGet(Name = "GetUsers")]
    public async Task<ActionResult<IEnumerable<AppUser>>> GetUsers()
    {
        return await _context.Users.ToListAsync();
    }

    [HttpGet("{id}", Name = "GetUser")]
    public async Task<ActionResult<AppUser>> GetUser(int id)
    {
        return await _context.Users.FindAsync(id);
    }

    [HttpPut("{id}", Name = "UpdateUser")]
    public async Task<IActionResult> UpdateUser(int id, AppUser user)
    {
        if (id != user.Id)
        {
            return BadRequest();
        }

        _context.Entry(user).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!UserExists(id))
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

    [HttpPost(Name = "CreateUser")]
    public async Task<ActionResult<AppUser>> CreateUser(AppUser user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetUser", new { id = user.Id }, user);
    }

    [HttpDelete("{id}", Name = "DeleteUser")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool UserExists(int id)
    {
        return _context.Users.Any(e => e.Id == id);
    }
}