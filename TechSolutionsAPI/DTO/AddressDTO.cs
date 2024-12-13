namespace TechSolutionsAPI.DTO
{
    public class AddressDTO
    {
        public required int AddressId { get; set; }
        public required int CustomerId { get; set; }
        public string StreetAddress { get; set; } = null!;
        public string City { get; set; } = null!;
        public string? Province { get; set; }
        public string? PostalCode { get; set; }
        public string Country { get; set; } = null!;
        public bool? IsPrimary { get; set; }

    }
}
