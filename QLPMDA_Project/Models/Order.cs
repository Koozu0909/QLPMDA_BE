using System;
using System.Collections.Generic;

namespace QLPMDA_Project.Models;

public partial class Order
{
    public int Id { get; set; }

    public DateTime OrderDate { get; set; }

    public string IdRaw { get; set; } = null!;

    public string? CustomerName { get; set; }

    public decimal? TotalAmount { get; set; }

    public bool Active { get; set; }

    public DateTime InsertedDate { get; set; }

    public int InsertedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public int? UpdatedBy { get; set; }

    public virtual ICollection<OrderDetail> OrderDetail { get; set; } = new List<OrderDetail>();
}
