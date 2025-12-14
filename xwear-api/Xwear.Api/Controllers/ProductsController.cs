using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xwear.Api.Models;

namespace Xwear.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly DbExwearContext _db;

    public ProductsController(DbExwearContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Product>>> Get()
    {
        return await _db.Products
            .Include(p => p.Images)
            .Include(p => p.Sizes)
            .ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Product>> GetById(int id)
    {
        var product = await _db.Products
            .Include(p => p.Images)
            .Include(p => p.Sizes)
            .FirstOrDefaultAsync(p => p.Id == id);
        if (product == null) return NotFound();
        return product;
    }

    [HttpGet("by-category/{categoryId}")]
    public async Task<ActionResult<IEnumerable<Product>>> GetByCategory(int categoryId)
    {
        return await _db.Products
            .Where(p => p.CategoryId == categoryId)
            .Include(p => p.Images)
            .Include(p => p.Sizes)
            .ToListAsync();
    }

    [HttpGet("by-brand/{brandId}")]
    public async Task<ActionResult<IEnumerable<Product>>> GetByBrand(int brandId)
    {
        return await _db.Products
            .Where(p => p.BrandId == brandId)
            .Include(p => p.Images)
            .Include(p => p.Sizes)
            .ToListAsync();
    }

    [HttpGet("by-model/{modelId}")]
    public async Task<ActionResult<IEnumerable<Product>>> GetByModel(int modelId)
    {
        return await _db.Products
            .Where(p => p.ModelId == modelId)
            .Include(p => p.Images)
            .Include(p => p.Sizes)
            .ToListAsync();
    }

    [HttpGet("{id}/sizes")]
    public async Task<ActionResult<IEnumerable<Size>>> GetSizes(int id)
    {
        var exists = await _db.Products.AnyAsync(p => p.Id == id);
        if (!exists) return NotFound();
        return await _db.Sizes.Where(s => s.ProductId == id).ToListAsync();
    }

    [HttpGet("{id}/images")]
    public async Task<ActionResult<IEnumerable<Image>>> GetImages(int id)
    {
        var exists = await _db.Products.AnyAsync(p => p.Id == id);
        if (!exists) return NotFound();
        return await _db.Images.Where(i => i.ProductId == id).ToListAsync();
    }

    [HttpPost]
    public async Task<ActionResult<Product>> Create(Product product)
    {
        _db.Products.Add(product);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Product product)
    {
        if (id != product.Id) return BadRequest();
        _db.Entry(product).State = EntityState.Modified;
        try
        {
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await _db.Products.AnyAsync(p => p.Id == id)) return NotFound();
            throw;
        }
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var product = await _db.Products.FindAsync(id);
        if (product == null) return NotFound();
        _db.Products.Remove(product);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
