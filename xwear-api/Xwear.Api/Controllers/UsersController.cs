using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xwear.Api.Models;

namespace Xwear.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly DbExwearContext _db;

    public UsersController(DbExwearContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<object>>> Get()
    {
        return await _db.Users.Select(u => new { u.Id, u.Email, u.Name, u.Phone }).ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<object>> GetById(int id)
    {
        var user = await _db.Users.Where(u => u.Id == id).Select(u => new { u.Id, u.Email, u.Name, u.Phone }).FirstOrDefaultAsync();
        if (user == null) return NotFound();
        return user;
    }

    [HttpGet("by-email/{email}")]
    public async Task<ActionResult<object>> GetByEmail(string email)
    {
        var user = await _db.Users.Where(u => u.Email == email).Select(u => new { u.Id, u.Email, u.Name, u.Phone }).FirstOrDefaultAsync();
        if (user == null) return NotFound();
        return user;
    }

    [HttpPost]
    public async Task<ActionResult<object>> Create(User user)
    {
        _db.Users.Add(user);
        await _db.SaveChangesAsync();
        var result = new { user.Id, user.Email, user.Name, user.Phone };
        return CreatedAtAction(nameof(GetById), new { id = user.Id }, result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, User user)
    {
        if (id != user.Id) return BadRequest();
        _db.Entry(user).State = EntityState.Modified;
        try
        {
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await _db.Users.AnyAsync(u => u.Id == id)) return NotFound();
            throw;
        }
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var user = await _db.Users.FindAsync(id);
        if (user == null) return NotFound();
        _db.Users.Remove(user);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
