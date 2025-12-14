using System;
using System.Collections.Generic;

namespace Xwear.Api.Models;

public partial class Order
{
    public int Id { get; set; }

    public DateTime CreatedAt { get; set; }

    public int UserId { get; set; }

    public string Country { get; set; } = null!;

    public string Street { get; set; } = null!;

    public string City { get; set; } = null!;

    public string NumberHome { get; set; } = null!;

    public string District { get; set; } = null!;

    public string Index { get; set; } = null!;

    public string Status { get; set; } = null!;

    public float TotalAmount { get; set; }

    public sbyte IsPaid { get; set; }

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual User User { get; set; } = null!;
}
