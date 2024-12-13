using System;
using System.Collections.Generic;

namespace TechSolutionsAPI.Models;

public partial class Invoice
{
    public int InvoiceId { get; set; }

    public int? CustomerId { get; set; }

    public DateTime? InvoiceDate { get; set; }

    public decimal Amount { get; set; }

    public int? ShippingAddressId { get; set; }

    public string? Status { get; set; }

    public virtual Customer? Customer { get; set; }

    public virtual Address? ShippingAddress { get; set; }
}
