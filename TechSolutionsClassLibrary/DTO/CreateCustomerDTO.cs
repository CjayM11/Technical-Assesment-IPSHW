﻿namespace TechSolutionsClassLibrary.DTO
{
    public class CreateCustomerDTO
    {
        public required int CustomerId { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Email { get; set; }
        public required string PhoneNumber { get; set; }

    }
}
