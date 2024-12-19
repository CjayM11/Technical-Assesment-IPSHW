namespace TechSolutionsClassLibrary.DTO
{
    public class InvoiceDTO
    {
        public int InvoiceId { get; set; }

        public int CustomerId { get; set; }

        public decimal ShippingAddressId { get; set; }

        public DateTime DateCreated { get; set; }

        public decimal Amount { get; set; }

        public string? Status { get; set; }
    }
}
