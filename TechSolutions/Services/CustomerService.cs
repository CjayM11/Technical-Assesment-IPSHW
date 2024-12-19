using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using TechSolutionsClassLibrary.DTO;

public class CustomerService
{
    private readonly HttpClient _httpClient;
    private readonly AddressService _addressService;
    private const string BaseUrl = "http://localhost:5215/api/customer";
    public CustomerService(HttpClient httpClient, AddressService addressService)
    {
        _httpClient = httpClient;
        _addressService = addressService;
    }

    public async Task<CustomerDTO> CreateCustomerAsync(CustomerDTO customerDto)
    {
        var response = await _httpClient.PostAsJsonAsync($"{BaseUrl}/create", customerDto);

        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<CustomerDTO>();
        }
        else
        {
            var error = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Failed to create customer: {error}");
            throw new Exception($"HTTP error {response.StatusCode}: {error}");
        }
    }

    public async Task<List<CustomerDTO>> GetCustomersAsync()
    {
        try
        {
            Console.WriteLine("Attempting to fetch customers from API...");
            var response = await _httpClient.GetAsync("http://localhost:5215/api/customer");
            var content = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Received response with status code: {response.StatusCode}");

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("Response was successful. Deserializing JSON...");
                // Deserialize the response into a JsonElement
                var data = JsonSerializer.Deserialize<JsonElement>(content);
                Console.WriteLine($"Deserialized JSON: {data}");

                // Access the $values array from the response
                if (data.TryGetProperty("$values", out JsonElement customersArray))
                {
                    var customers = new List<CustomerDTO>();

                    Console.WriteLine($"Number of customers in response: {customersArray.GetArrayLength()}");
                    foreach (var item in customersArray.EnumerateArray())
                    {
                        var customer = new CustomerDTO
                        {
                            CustomerId = item.GetProperty("customerId").GetInt32(),
                            FirstName = item.GetProperty("firstName").GetString(),
                            LastName = item.GetProperty("lastName").GetString(),
                            Email = item.GetProperty("email").GetString(),
                            PhoneNumber = item.TryGetProperty("phoneNumber", out JsonElement phoneNumber) ? phoneNumber.GetString() : null,
                            Addresses = new List<AddressDTO>()
                        };

                        Console.WriteLine($"Processing customer: {customer.CustomerId} - {customer.FirstName} {customer.LastName}");

                        // Check if the "addresses" property exists and has $values
                        if (item.TryGetProperty("addresses", out JsonElement addressesElement) &&
                            addressesElement.TryGetProperty("$values", out JsonElement addressValues))
                        {
                            Console.WriteLine($"Addresses for customer {customer.CustomerId}: {addressValues.GetArrayLength()}");
                            foreach (var addressItem in addressValues.EnumerateArray())
                            {
                                var address = new AddressDTO
                                {
                                    AddressId = addressItem.GetProperty("addressId").GetInt32(),
                                    CustomerId = addressItem.GetProperty("customerId").GetInt32(),
                                    StreetAddress = addressItem.TryGetProperty("streetAddress", out JsonElement streetAddress) ? streetAddress.GetString() : null,
                                    City = addressItem.TryGetProperty("city", out JsonElement city) ? city.GetString() : null,
                                    Province = addressItem.TryGetProperty("province", out JsonElement province) ? province.GetString() : null,
                                    PostalCode = addressItem.TryGetProperty("postalCode", out JsonElement postalCode) ? postalCode.GetString() : null,
                                    Country = addressItem.TryGetProperty("country", out JsonElement country) ? country.GetString() : null,
                                    IsPrimary = addressItem.TryGetProperty("isPrimary", out JsonElement isPrimary) ? isPrimary.GetBoolean() : false
                                };
                                customer.Addresses.Add(address);
                            }
                        }
                        else
                        {
                            // Console.WriteLine($"No addresses found for customer {customer.CustomerId}");
                        }

                        customers.Add(customer);
                    }

                    // Console.WriteLine($"Returning {customers.Count} customers.");
                    return customers;
                }
                else
                {
                    // Console.WriteLine("The $values property was not found in the response.");
                    return new List<CustomerDTO>();
                }
            }
            else
            {

                // Console.WriteLine($"API request failed: {response.StatusCode} - {content}");
                throw new Exception($"Error fetching data: {response.StatusCode}");
            }
        }
        catch (Exception ex)
        {
            //  Console.WriteLine($"An error occurred while fetching customers: {ex.Message}");
            return new List<CustomerDTO>(); // Return empty list on error
        }
    }

    public async Task<bool> UpdateCustomerAsync(CustomerUpdateDTO customerUpdateDto)
    {
        var jsonContent = new StringContent(
            JsonSerializer.Serialize(customerUpdateDto),
            Encoding.UTF8,
        "application/json"
        );

        var response = await _httpClient.PostAsync("http://localhost:5215/api/customer/update", jsonContent);

        if (response.IsSuccessStatusCode)
        {
            // The customer was successfully updated
            return true;
        }

        // If something went wrong, you can log or handle the error
        return false;
    }

    // This method checks if the customer values have changed after the update
    public async Task<bool> CheckCustomerUpdatedAsync(CustomerUpdateDTO customerUpdateDto)
    {
        // You can make a GET request to retrieve the customer and compare the data
        var response = await _httpClient.GetAsync($"http://localhost:5215/api/customer/{customerUpdateDto.CustomerId}");

        if (response.IsSuccessStatusCode)
        {
            var customer = await response.Content.ReadFromJsonAsync<CustomerDTO>();
            // Check if the customer details match
            return customer != null &&
                   customer.FirstName == customerUpdateDto.FirstName &&
                   customer.LastName == customerUpdateDto.LastName &&
                   customer.Email == customerUpdateDto.Email; //&&
                                                              // customer.PhoneNumber == customerUpdateDto.PhoneNumber;
        }

        return false;
    }

    public async Task<bool> DeleteCustomerAsync(int id)
    {
        try
        {
            // First, delete all addresses related to this customer
            if (!await _addressService.DeleteAddressesForCustomerAsync(id))
            {
                return false; // Failed to delete addresses
            }

            // Now delete the customer
            var deleteCustomerResponse = await _httpClient.DeleteAsync($"{BaseUrl}/delete/{id}");
            if (deleteCustomerResponse.IsSuccessStatusCode)
            {
                return true;
            }
            else
            {
                Console.WriteLine($"Failed to delete customer {id}: {deleteCustomerResponse.StatusCode}");
                return false;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while deleting customer {id}: {ex.Message}");
            return false;
        }
    }



}
