using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xwear.Api.Models;

namespace Xwear.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SizesController : ControllerBase
{
    private readonly DbExwearContext _db;

    public SizesController(DbExwearContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Size>>> Get()
    {
        return await _db.Sizes.ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Size>> GetById(int id)
    {
        var size = await _db.Sizes.FindAsync(id);
        if (size == null) return NotFound();
        return size;
    }

    [HttpGet("by-product/{productId}")]
    public async Task<ActionResult<IEnumerable<Size>>> GetByProduct(int productId)
    {
        return await _db.Sizes.Where(s => s.ProductId == productId).ToListAsync();
    }

    [HttpPost]
    public async Task<ActionResult<Size>> Create(Size size)
    {
        _db.Sizes.Add(size);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = size.Id }, size);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Size size)
    {
        if (id != size.Id) return BadRequest();
        _db.Entry(size).State = EntityState.Modified;
        try
        {
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await _db.Sizes.AnyAsync(s => s.Id == id)) return NotFound();
            throw;
        }
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var size = await _db.Sizes.FindAsync(id);
        if (size == null) return NotFound();
        _db.Sizes.Remove(size);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
