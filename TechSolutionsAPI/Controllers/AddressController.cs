using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TechSolutionsAPI.Data;
using TechSolutionsClassLibrary.DTO;
using TechSolutionsClassLibrary.Models;

namespace TechSolutionsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AddressController : ControllerBase
    {
        private readonly TechSolutionsDbContext _context;

        public AddressController(TechSolutionsDbContext context)
        {
            _context = context;
        }

        [HttpGet("get/{addressId}")]
        public async Task<ActionResult<Address>> GetAddressById(int addressId)
        {
            var address = await _context.Addresses.FindAsync(addressId);

            if (address == null)
            {
                return NotFound();
            }

            return Ok(address);
        }

        // POST: api/Addresses/create
        //Allows users to create a new address.
        //[HttpPost("create")]
        //public async Task<ActionResult<Address>> CreateAddress([FromBody] AddressDTO addressDto)
        //{
        //    if (addressDto == null)
        //    {
        //        return BadRequest("Address data is null");
        //    }

        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    // Validate the customer ID
        //    var existingCustomer = await _context.Customers.FindAsync(addressDto.CustomerId);
        //    if (existingCustomer == null)
        //    {
        //        return BadRequest($"Customer with ID {addressDto.CustomerId} does not exist.");
        //    }

        //    // Map AddressDTO to Address entity
        //    var newAddress = new Address
        //    {
        //        StreetAddress = addressDto.StreetAddress,
        //        City = addressDto.City,
        //        Province = addressDto.Province,
        //        PostalCode = addressDto.PostalCode,
        //        Country = addressDto.Country,
        //        IsPrimary = false,
        //        CustomerId = addressDto.CustomerId
        //    };

        //    // Add the new address to the context
        //    _context.Addresses.Add(newAddress);

        //    // Save the changes to the database
        //    await _context.SaveChangesAsync();

        //    // Return a response with the created address and a 201 status code
        //    return CreatedAtAction(nameof(GetAddressById), new { id = newAddress.AddressId }, newAddress);
        //}

        // GET: api/Addresses/1
        //Retrieves Client specific addresses from the database
        // Get all addresses for the logged-in user
        [HttpGet("{customerId}")]
        public async Task<ActionResult<IEnumerable<Address>>> GetAllAddressesForCustomer(int customerId)
        {
            var addresses = await _context.Addresses
                .Where(a => a.CustomerId == customerId)
                .ToListAsync();

            if (addresses == null || !addresses.Any())
            {
                return NotFound();
            }

            return Ok(addresses);
        }

        // PUT: api/Addresses/update/5
        //updates Client Specific Addresses
        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateAddress(int id, [FromBody] AddressDTO addressDto)
        {
            if (addressDto == null)
            {
                return BadRequest("Address data is null");
            }

            Console.WriteLine(addressDto.AddressId);
            var existingAddress = await _context.Addresses.FindAsync(addressDto.AddressId);
            Console.WriteLine("AddressId");
            Console.WriteLine(existingAddress);

            if (existingAddress == null)
            {
                return NotFound($"Address with ID {addressDto.AddressId} not found.");
            }

            // Update the address fields
            existingAddress.StreetAddress = addressDto.StreetAddress;
            existingAddress.City = addressDto.City;
            existingAddress.Province = addressDto.Province;
            existingAddress.PostalCode = addressDto.PostalCode;
            existingAddress.Country = addressDto.Country;
            existingAddress.IsPrimary = addressDto.IsPrimary;

            // Save changes to the database
            await _context.SaveChangesAsync();

            // Return the updated address as a response
            return Ok(existingAddress);
        }

        // DELETE: api/Address/{id}
        //Deletes an Address record from the database.
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteAddress(int id)
        {
            var address = await _context.Addresses.FindAsync(id);
            if (address == null)
            {
                return NotFound();
            }

            _context.Addresses.Remove(address);
            await _context.SaveChangesAsync();

            return NoContent();
        }

    }
}
