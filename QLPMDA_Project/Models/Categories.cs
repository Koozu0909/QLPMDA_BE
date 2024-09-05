using System;
using System.Collections.Generic;

namespace QLPMDA_Project.Models;

public partial class Categories
{
    public int Id { get; set; }

    public string CategoryName { get; set; } = null!;

    public string? Description { get; set; }

    public bool Active { get; set; }

    public DateTime InsertedDate { get; set; }

    public int InsertedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public int? UpdatedBy { get; set; }

    public virtual ICollection<Products> Products { get; set; } = new List<Products>();
}
