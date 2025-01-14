﻿namespace TechSolutionsClassLibrary.Models;

public partial class Customer
{
    public int CustomerId { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? PhoneNumber { get; set; }

    public DateTime? InvoiceDate { get; set; }

    public List<Address>? Addresses { get; set; }

}
