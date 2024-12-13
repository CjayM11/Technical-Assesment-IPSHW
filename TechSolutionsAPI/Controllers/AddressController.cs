using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TechSolutionsAPI.Data;
using TechSolutionsAPI.DTO;
using TechSolutionsAPI.Models;

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

        // POST: api/Addresses/create
        //Allows users to create a new address.
        [HttpPost("create")]
        public async Task<ActionResult<Address>> CreateAddress([FromBody] AddressDTO addressDto)
        {
            if (addressDto == null)
            {
                return BadRequest("Address data is null");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Validate the customer ID
            var existingCustomer = await _context.Customers.FindAsync(addressDto.CustomerId);
            if (existingCustomer == null)
            {
                return BadRequest($"Customer with ID {addressDto.CustomerId} does not exist.");
            }

            // Map AddressDTO to Address entity
            var newAddress = new Address
            {
                StreetAddress = addressDto.StreetAddress,
                City = addressDto.City,
                Province = addressDto.Province,
                PostalCode = addressDto.PostalCode,
                Country = addressDto.Country,
                IsPrimary = false,
                CustomerId = addressDto.CustomerId
            };

            // Add the new address to the context
            _context.Addresses.Add(newAddress);

            // Save the changes to the database
            await _context.SaveChangesAsync();

            // Return a response with the created address and a 201 status code
            return CreatedAtAction(nameof(GetAddressById), new { id = newAddress.AddressId }, newAddress);
        }

        // GET: api/Addresses/1
        //Retrieves Client specific addresses from the database
        [HttpGet("{id}")]
        public async Task<ActionResult<Address>> GetAddressById(int id)
        {
            var addresses = await _context.Addresses.FindAsync(id);

            if (addresses == null)
            {
                return NotFound();
            }

            return addresses;
        }

        // PUT: api/Addresses/update/5
        //updates Client Specific Addresses
        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateAddress(int id, AddressDTO addressDto)
        {
            if (id != addressDto.AddressId)
            {
                return BadRequest("Address ID mismatch.");
            }

            var existingAddress = await _context.Addresses.FindAsync(id);
            if (existingAddress == null)
            {
                return NotFound("Address not found.");
            }

            existingAddress.StreetAddress = addressDto.StreetAddress;
            existingAddress.City = addressDto.City;
            existingAddress.Province = addressDto.Province;
            existingAddress.PostalCode = addressDto.PostalCode;
            existingAddress.Country = addressDto.Country;
            existingAddress.IsPrimary = addressDto.IsPrimary;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AddressExists(id))
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

        private bool AddressExists(int id)
        {
            return _context.Addresses.Any(e => e.AddressId == id);
        }
    }
}
