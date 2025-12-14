using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xwear.Api.Models;

namespace Xwear.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrderDetailsController : ControllerBase
{
    private readonly DbExwearContext _db;

    public OrderDetailsController(DbExwearContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<OrderDetail>>> Get()
    {
        return await _db.OrderDetails.ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<OrderDetail>> GetById(int id)
    {
        var detail = await _db.OrderDetails.FindAsync(id);
        if (detail == null) return NotFound();
        return detail;
    }

    [HttpGet("by-order/{orderId}")]
    public async Task<ActionResult<IEnumerable<OrderDetail>>> GetByOrder(int orderId)
    {
        return await _db.OrderDetails.Where(d => d.OrderId == orderId).ToListAsync();
    }

    [HttpPost]
    public async Task<ActionResult<OrderDetail>> Create(OrderDetail detail)
    {
        _db.OrderDetails.Add(detail);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = detail.Id }, detail);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var detail = await _db.OrderDetails.FindAsync(id);
        if (detail == null) return NotFound();
        _db.OrderDetails.Remove(detail);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
