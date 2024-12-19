using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TechSolutionsAPI.Data;
using TechSolutionsClassLibrary.DTO;
using TechSolutionsClassLibrary.Models;

namespace TechSolutionsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {

        private readonly TechSolutionsDbContext _context;

        public CustomerController(TechSolutionsDbContext context)
        {
            _context = context;
        }

        // POST: api/Customer
        //Create a new customer
        [HttpPost("create")]
        public async Task<IActionResult> CreateCustomer([FromBody] CreateCustomerDTO createCustomerDTO)
        {
            if (createCustomerDTO == null)
            {
                return BadRequest("Customer data is null");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Create the customer, excluding addresses since they're not in the DTO
            var newCustomer = new Customer
            {
                CustomerId = createCustomerDTO.CustomerId,
                FirstName = createCustomerDTO.FirstName,
                LastName = createCustomerDTO.LastName,
                Email = createCustomerDTO.Email,
                PhoneNumber = createCustomerDTO.PhoneNumber

            };

            // Check if a customer with the same email already exists
            var existingCustomer = await _context.Customers.FirstOrDefaultAsync(c => c.Email == createCustomerDTO.Email);

            if (existingCustomer != null)
            {
                return BadRequest($"A customer with the email {createCustomerDTO.Email} already exists.");
            }

            // Add the new customer to the context
            _context.Customers.Add(newCustomer);

            // Save the changes to the database
            await _context.SaveChangesAsync();

            // Map back to CustomerDTO for the response, but without addresses
            var responseDTO = new CustomerDTO
            {
                CustomerId = newCustomer.CustomerId,
                FirstName = newCustomer.FirstName,
                LastName = newCustomer.LastName,
                Email = newCustomer.Email,
                PhoneNumber = newCustomer.PhoneNumber
            };

            return CreatedAtAction(nameof(GetCustomerById), new { id = newCustomer.CustomerId }, responseDTO);
        }
        // GET: api/Customer
        //Retrieves a list of all customers from the database.
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Customer>>> GetAllCustomers()
        {
            var customers = await _context.Customers
                .Include(c => c.Addresses)
                .ToListAsync();
            return Ok(customers);
        }
        // GET: api/Customer/{id}
        //Retrieves a specific customer by their unique ID.
        [HttpGet("{id}")]
        public async Task<ActionResult<Customer>> GetCustomerById(int id)
        {
            var customer = await _context.Customers.FindAsync(id);

            if (customer == null)
            {
                return NotFound();
            }

            return customer;
        }

        // PUT: api/Customer/{id}
        //Updates the details of an existing customer using their ID.
        [HttpPost("update")]
        public async Task<IActionResult> UpdateCustomer([FromBody] CustomerUpdateDTO customerUpdateDto)
        {
            if (customerUpdateDto == null)
            {
                return BadRequest("Customer data is null");
            }
            Console.WriteLine(customerUpdateDto.CustomerId);
            var existingCustomer = await _context.Customers.FindAsync(customerUpdateDto.CustomerId);
            Console.WriteLine("CustomerId");
            Console.WriteLine(existingCustomer);

            if (existingCustomer == null)
            {
                return NotFound($"Customer with ID {customerUpdateDto.CustomerId} not found.");
            }

            // Update the customer fields
            existingCustomer.FirstName = customerUpdateDto.FirstName;
            existingCustomer.LastName = customerUpdateDto.LastName;
            existingCustomer.Email = customerUpdateDto.Email;
            //existingCustomer.PhoneNumber = customerUpdateDto.PhoneNumber;

            // Save changes to the database
            await _context.SaveChangesAsync();

            // Return the updated customer as a response
            return Ok(existingCustomer);
        }

        // DELETE: api/Customer/{id}
        //Deletes a customer record from the database.
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }

            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();

            return NoContent();
        }


        // Check if customer exist , separate for reuse-ability with in the controller
        private bool CustomerExists(int id)
        {
            return _context.Customers.Any(e => e.CustomerId == id);
        }
    }
}
