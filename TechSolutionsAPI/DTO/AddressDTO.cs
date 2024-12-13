namespace TechSolutionsAPI.DTO
{
    public class AddressDTO
    {
        public required int AddressId { get; set; }

        public required int CustomerId { get; set; }

        public required string StreetAddress { get; set; }

        public required string City { get; set; }

        public required string Province { get; set; }

        public required string PostalCode { get; set; }

        public required string Country { get; set; }

        // You could optionally add a FullAddress for convenience
        public string FullAddress => $"{StreetAddress}, {City}, {Province}, {PostalCode}, {Country}";
    }
}
