using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xwear.Api.Models;

namespace Xwear.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CartsController : ControllerBase
{
    private readonly DbExwearContext _db;

    public CartsController(DbExwearContext db)
    {
        _db = db;
    }

    [HttpGet("user/{userId}")]
    public async Task<ActionResult<IEnumerable<Cart>>> GetByUser(int userId)
    {
        return await _db.Carts
            .Where(c => c.UserId == userId)
            .Include(c => c.Product)
            .Include(c => c.Size)
            .ToListAsync();
    }

    [HttpPost]
    public async Task<ActionResult<Cart>> Add(Cart cart)
    {
        var existing = await _db.Carts.FirstOrDefaultAsync(c => c.UserId == cart.UserId && c.ProductId == cart.ProductId && c.SizeId == cart.SizeId);
        if (existing != null)
        {
            existing.Count += cart.Count;
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(GetByUser), new { userId = cart.UserId }, existing);
        }
        _db.Carts.Add(cart);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetByUser), new { userId = cart.UserId }, cart);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Cart cart)
    {
        if (id != cart.Id) return BadRequest();
        _db.Entry(cart).State = EntityState.Modified;
        try
        {
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await _db.Carts.AnyAsync(c => c.Id == id)) return NotFound();
            throw;
        }
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var cart = await _db.Carts.FindAsync(id);
        if (cart == null) return NotFound();
        _db.Carts.Remove(cart);
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("user/{userId}")]
    public async Task<IActionResult> Clear(int userId)
    {
        var items = await _db.Carts.Where(c => c.UserId == userId).ToListAsync();
        if (items.Count == 0) return NoContent();
        _db.Carts.RemoveRange(items);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
