using System;
using System.Collections.Generic;

namespace Xwear.Api.Models;

public partial class Image
{
    public int Id { get; set; }

    public int ProductId { get; set; }

    public string Path { get; set; } = null!;

    public sbyte IsMain { get; set; }

    public virtual Product Product { get; set; } = null!;
}
