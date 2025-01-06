using Microsoft.AspNetCore.Identity;
using TechSolutionsAPI.Data;
using TechSolutionsClassLibrary.Models;

namespace TechSolutionsAPI.SeedData
{
    public class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                var context = scope.ServiceProvider.GetRequiredService<TechSolutionsDbContext>();

                // Ensure the database is created
                context.Database.EnsureCreated();

                // Seed roles
                SeedRoles(roleManager);

                // Seed developer user
                var devEmail = "dev@techsolutions.com";
                var devPassword = "1267067!!@78Mm";

                if (userManager.FindByEmailAsync(devEmail).Result == null)
                {
                    var devUser = new ApplicationUser
                    {
                        UserName = devEmail,
                        Email = devEmail,
                        EmailConfirmed = true
                    };

                    var result = userManager.CreateAsync(devUser, devPassword).Result;
                    if (result.Succeeded)
                    {
                        // Assign the developer user to the Admin role
                        userManager.AddToRoleAsync(devUser, "Admin").Wait();

                        // Create related Customer and Addresses
                        SeedCustomerData(context, devUser.Id);
                    }
                }
            }
        }

        private static void SeedRoles(RoleManager<IdentityRole> roleManager)
        {
            // Define roles
            var roles = new[] { "Admin", "User" };

            foreach (var role in roles)
            {
                if (!roleManager.RoleExistsAsync(role).Result)
                {
                    roleManager.CreateAsync(new IdentityRole(role)).Wait();
                }
            }
        }

        private static void SeedCustomerData(TechSolutionsDbContext context, string userId)
        {
            // Check if the customer already exists
            if (!context.Customers.Any(c => c.Email == "dev@techsolutions.com"))
            {
                // Insert the dummy customer
                var customer = new Customer
                {
                    FirstName = "Dev",
                    LastName = "User",
                    Email = "dev@techsolutions.com",
                    PhoneNumber = "123-456-7890"
                };

                context.Customers.Add(customer);
                context.SaveChanges(); // This will automatically generate the CustomerId

                // Retrieve the CustomerId of the inserted customer
                var customerId = customer.CustomerId; // EF will set this after SaveChanges()

                // Insert dummy addresses for the customer
                var addresses = new[]
                {
            new Address
            {
                CustomerId = customerId, // Use the auto-generated CustomerId
                StreetAddress = "123 Tech St",
                City = "Techville",
                Province = "TechProvince",
                PostalCode = "12345",
                Country = "TechCountry",
                IsPrimary = true
            },
            new Address
            {
                CustomerId = customerId, // Use the auto-generated CustomerId
                StreetAddress = "456 Code Ave",
                City = "ProgramCity",
                Province = "ScriptProvince",
                PostalCode = "67890",
                Country = "CodeCountry",
                IsPrimary = false
            }
        };

                context.Addresses.AddRange(addresses);
                context.SaveChanges();
            }
        }

    }
}
