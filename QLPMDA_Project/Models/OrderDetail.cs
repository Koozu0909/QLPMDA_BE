using System;
using System.Collections.Generic;

namespace QLPMDA_Project.Models;

public partial class OrderDetail
{
    public int Id { get; set; }

    public int OrderId { get; set; }

    public int? ProductId { get; set; }

    public string? ProductName { get; set; }

    public int? Quantity { get; set; }

    public decimal? Price { get; set; }

    public bool Active { get; set; }

    public DateTime InsertedDate { get; set; }

    public int InsertedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public int? UpdatedBy { get; set; }

    public virtual Order Order { get; set; } = null!;
}
