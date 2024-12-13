namespace TechSolutionsAPI.DTO
{
    public class CustomerDTO
    {
        public int CustomerID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }


        public string FullName => $"{FirstName} {LastName}";
    }
}
