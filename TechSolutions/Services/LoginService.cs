using System.Net.Http.Json;
using System.Text.Json;
using TechSolutionsClassLibrary.Models;

namespace TechSolutions.Services
{
    public class LoginService
    {
        private readonly HttpClient _httpClient;

        public LoginService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // Login Service returning Token and ID, To Authenticate and UseId for adding addresses
        public async Task<(string Token, string UserID)> Login(string email, string password)
        {
            // Use your existing LoginDTO
            var loginModel = new Login
            {
                Email = email,
                Password = password
            };

            try
            {
                // Send the login request
                var response = await _httpClient.PostAsJsonAsync("http://localhost:5215/api/Auth/login", loginModel);

                // Debugging: Log the response status
                //Console.WriteLine($"Response status code: {response.StatusCode}");

                if (response.IsSuccessStatusCode)
                {
                    // Read the raw response as a string for debugging
                    string rawResponse = await response.Content.ReadAsStringAsync();
                    // Console.WriteLine($"Raw response: {rawResponse}");

                    // Parse the JSON response
                    var result = JsonSerializer.Deserialize<JsonElement>(rawResponse);

                    // Debugging: Log the result of the JSON response
                    // Console.WriteLine($"Response JSON: {result}");

                    // Ensure the response contains both token and userID
                    if (result.TryGetProperty("token", out var tokenProp) && result.TryGetProperty("userID", out var userIdProp))
                    {
                        string token = tokenProp.GetString();
                        string userId = userIdProp.GetString();

                        // Return both Token and UserID
                        return (token, userId);
                    }
                    else
                    {
                        throw new Exception("Missing token or userID in the response.");
                    }
                }
                else
                {
                    // Console.WriteLine($"Login failed with status code: {response.StatusCode}");
                    throw new Exception("Login failed. Invalid credentials.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception during login: {ex.Message}");
                throw;
            }
        }
    }
}
