using System;
using System.Collections.Generic;

namespace QLPMDA_Project.Models;

public partial class Users
{
    public int Id { get; set; }

    public bool Active { get; set; }

    public DateTime InsertedDate { get; set; }

    public int InsertedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public int? UpdatedBy { get; set; }

    public bool HasVerifiedEmail { get; set; }

    public string? Password { get; set; }

    public string? UserName { get; set; }

    public string? Salt { get; set; }

    public DateTime? LastLogin { get; set; }

    public string? Email { get; set; }

    public string? FullName { get; set; }

    public string? Avatar { get; set; }

    public string? Role { get; set; }
}
