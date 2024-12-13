using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TechSolutionsAPI.Data;
using TechSolutionsAPI.DTO;
using TechSolutionsAPI.Models;

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
        public async Task<IActionResult> CreateCustomer([FromBody] CustomerDTO customerDto)
        {
            if (customerDto == null)
            {
                return BadRequest("Customer data is null");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Map DTO to the Customer model
            var newCustomer = new Customer
            {
                FirstName = customerDto.FirstName,
                LastName = customerDto.LastName,
                Email = customerDto.Email,
                PhoneNumber = customerDto.PhoneNumber,
            };

            // Check if a customer with the same email already exists
            var existingCustomer = await _context.Customers.FirstOrDefaultAsync(c => c.Email == customerDto.Email);

            if (existingCustomer != null)
            {
                // Return a bad request with a specific message
                return BadRequest($"A customer with the email {customerDto.Email} already exists.");
            }

            // Add the new customer to the context
            _context.Customers.Add(newCustomer);

            // Save the changes to the database
            await _context.SaveChangesAsync();

            // Return a response with the created customer and a 201 status code
            return CreatedAtAction(nameof(GetCustomerById), new { id = newCustomer.CustomerId }, newCustomer);
        }

        // GET: api/Customer
        //Retrieves a list of all customers from the database.
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Customer>>> GetAllCustomers()
        {
            return await _context.Customers.ToListAsync();
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
        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateCustomer(int id, [FromBody] CustomerDTO customerDTO)
        {
            if (id != customerDTO.CustomerId)
            {
                return BadRequest("Customer ID mismatch");
            }

            var existingCustomer = await _context.Customers.FindAsync(id);

            if (existingCustomer == null)
            {
                return NotFound();
            }

            // Update only the customer fields (not the ID)
            existingCustomer.FirstName = customerDTO.FirstName;
            existingCustomer.LastName = customerDTO.LastName;
            existingCustomer.Email = customerDTO.Email;
            existingCustomer.PhoneNumber = customerDTO.PhoneNumber;

            // Mark the entity as modified
            _context.Entry(existingCustomer).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CustomerExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
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
