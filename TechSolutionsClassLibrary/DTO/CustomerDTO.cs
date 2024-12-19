namespace TechSolutionsClassLibrary.DTO
{
    public class CustomerDTO
    {
        public required int CustomerId { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Email { get; set; }
        public required string? PhoneNumber { get; set; }
        public List<AddressDTO>? Addresses { get; set; }
        public bool ShowDetails { get; set; }
        public bool IsEditing { get; set; }

        // Shallow Clone
        public CustomerDTO Clone()
        {
            return (CustomerDTO)this.MemberwiseClone();
        }
    }
}
