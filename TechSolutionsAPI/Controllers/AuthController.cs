using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TechSolutionsAPI.Models;

namespace TechSolutionsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IConfiguration _configuration;

        public AuthController(UserManager<ApplicationUser> userManager,
                              SignInManager<ApplicationUser> signInManager,
                              IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
        }


        //Login Method to handle user login
        //Using EF, ensures a user is generated a JWT token to authenticate the user after sign in
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] Login model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return Unauthorized("Invalid credentials");

            var result = await _signInManager.PasswordSignInAsync(user, model.Password, false, false);
            if (!result.Succeeded)
                return Unauthorized("Invalid credentials");

            // Generate JWT Token
            var token = GenerateJwtToken(user);

            return Ok(new { Token = token });
        }

        // Register method to handle user registration
        //Using EF , ApplicationUser(Models) allows customization down the line
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] Register model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    return Ok(new { Message = "User registered successfully." });
                }
                else
                {
                    // Return the errors from Identity
                    return BadRequest(result.Errors);
                }
            }

            return BadRequest("Invalid registration request.");
        }

        //Generate JWT token to authenticate the user 
        private string GenerateJwtToken(ApplicationUser user)
        {
            // Check if user is null
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "User cannot be null.");
            }

            // Check for null or empty values for the claims
            if (string.IsNullOrEmpty(user.Id))
            {
                throw new ArgumentException("User ID cannot be null or empty.", nameof(user.Id));
            }

            if (string.IsNullOrEmpty(user.UserName))
            {
                throw new ArgumentException("Username cannot be null or empty.", nameof(user.UserName));
            }

            var claims = new[]
            {
        new Claim(ClaimTypes.NameIdentifier, user.Id),
        new Claim(ClaimTypes.Name, user.UserName),
        // Add more claims as needed
    };

            // Ensure the JWT Secret is not null or empty
            var jwtSecret = _configuration["Jwt:Secret"];
            if (string.IsNullOrEmpty(jwtSecret))
            {
                throw new InvalidOperationException("JWT Secret is not configured. Please add a valid JWT secret.");
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Audience"],
                claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}
