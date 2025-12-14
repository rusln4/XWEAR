using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xwear.Api.Models;

namespace Xwear.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BrandsController : ControllerBase
{
    private readonly DbExwearContext _db;

    public BrandsController(DbExwearContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Brand>>> Get()
    {
        return await _db.Brands.ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Brand>> GetById(int id)
    {
        var brand = await _db.Brands.FindAsync(id);
        if (brand == null) return NotFound();
        return brand;
    }

    [HttpPost]
    public async Task<ActionResult<Brand>> Create(Brand brand)
    {
        _db.Brands.Add(brand);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = brand.Id }, brand);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Brand brand)
    {
        if (id != brand.Id) return BadRequest();
        _db.Entry(brand).State = EntityState.Modified;
        try
        {
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await _db.Brands.AnyAsync(b => b.Id == id)) return NotFound();
            throw;
        }
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var brand = await _db.Brands.FindAsync(id);
        if (brand == null) return NotFound();
        _db.Brands.Remove(brand);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
