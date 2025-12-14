using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xwear.Api.Models;

namespace Xwear.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ModelsController : ControllerBase
{
    private readonly DbExwearContext _db;

    public ModelsController(DbExwearContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Model>>> Get()
    {
        return await _db.Models.ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Model>> GetById(int id)
    {
        var model = await _db.Models.FindAsync(id);
        if (model == null) return NotFound();
        return model;
    }

    [HttpPost]
    public async Task<ActionResult<Model>> Create(Model model)
    {
        _db.Models.Add(model);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = model.Id }, model);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Model model)
    {
        if (id != model.Id) return BadRequest();
        _db.Entry(model).State = EntityState.Modified;
        try
        {
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await _db.Models.AnyAsync(m => m.Id == id)) return NotFound();
            throw;
        }
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var model = await _db.Models.FindAsync(id);
        if (model == null) return NotFound();
        _db.Models.Remove(model);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
