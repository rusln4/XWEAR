using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xwear.Api.Models;

namespace Xwear.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ImagesController : ControllerBase
{
    private readonly DbExwearContext _db;

    public ImagesController(DbExwearContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Image>>> Get()
    {
        return await _db.Images.ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Image>> GetById(int id)
    {
        var image = await _db.Images.FindAsync(id);
        if (image == null) return NotFound();
        return image;
    }

    [HttpGet("by-product/{productId}")]
    public async Task<ActionResult<IEnumerable<Image>>> GetByProduct(int productId)
    {
        return await _db.Images.Where(i => i.ProductId == productId).ToListAsync();
    }

    [HttpPost]
    public async Task<ActionResult<Image>> Create(Image image)
    {
        _db.Images.Add(image);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = image.Id }, image);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Image image)
    {
        if (id != image.Id) return BadRequest();
        _db.Entry(image).State = EntityState.Modified;
        try
        {
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await _db.Images.AnyAsync(i => i.Id == id)) return NotFound();
            throw;
        }
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var image = await _db.Images.FindAsync(id);
        if (image == null) return NotFound();
        _db.Images.Remove(image);
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpPost("set-main/{id}")]
    public async Task<IActionResult> SetMain(int id)
    {
        var image = await _db.Images.FindAsync(id);
        if (image == null) return NotFound();
        var productId = image.ProductId;
        var images = await _db.Images.Where(i => i.ProductId == productId).ToListAsync();
        foreach (var img in images)
        {
            img.IsMain = (sbyte)(img.Id == id ? 1 : 0);
        }
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
