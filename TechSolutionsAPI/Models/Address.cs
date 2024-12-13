﻿using System;
using System.Collections.Generic;

namespace TechSolutionsAPI.Models;

public partial class Address
{
    public int AddressId { get; set; }

    public int? CustomerId { get; set; }

    public string StreetAddress { get; set; } = null!;

    public string City { get; set; } = null!;

    public string? Provice { get; set; }

    public string? PostalCode { get; set; }

    public string Country { get; set; } = null!;

    public bool? IsPrimary { get; set; }

    public virtual Customer? Customer { get; set; }

    public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
}
