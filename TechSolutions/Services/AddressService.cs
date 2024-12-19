using System.Net.Http.Json;
using System.Text.Json;
using TechSolutionsClassLibrary.DTO;
using TechSolutionsClassLibrary.Models;

public class AddressService
{
    private readonly HttpClient _httpClient;
    private const string BaseUrl = "http://localhost:5215/api/address";
    public AddressService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    // Get all addresses for a given customer (or logged-in user)
    public async Task<List<Address>> GetAddressesForUser(int customerId)
    {
        var response = await _httpClient.GetFromJsonAsync<List<Address>>($"{BaseUrl}/{customerId}");
        Console.WriteLine(response);
        // Log the raw response for debugging
        // Console.WriteLine($"Raw response for customer {customerId}: {JsonDes.SerializeObject(response, Formatting.Indented)}");

        if (response == null)
        {
            Console.WriteLine($"No addresses found for customer {customerId}.");
        }
        else
        {
            Console.WriteLine($"Addresses retrieved for customer {customerId}:");
            foreach (var address in response)
            {
                Console.WriteLine($"  - AddressId: {address.AddressId}, Street: {address.StreetAddress}, City: {address.City}");
            }
        }
        return response ?? new List<Address>();
    }

    public async Task<bool> DeleteAddressesForCustomerAsync(int customerId)
    {
        Console.WriteLine("DeleteAddressesForCustomer");

        var response = await _httpClient.GetAsync($"{BaseUrl}/{customerId}");
        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine($"Failed to fetch addresses for customer {customerId}: {response.StatusCode}");
            return false;
        }

        var content = await response.Content.ReadAsStringAsync();
        Console.WriteLine($"Raw JSON response for customer {customerId}: {content}");

        if (string.IsNullOrEmpty(content) || content == "{\"$id\":\"1\",\"$values\":[]}")
        {
            Console.WriteLine($"No addresses found for customer {customerId}.");
            return true; // No addresses to delete, consider it a success
        }

        try
        {
            // Deserialize the response into a JsonElement
            var data = JsonSerializer.Deserialize<JsonElement>(content);

            // Access the $values array from the response
            var addressesArray = data.GetProperty("$values");

            foreach (var address in addressesArray.EnumerateArray())
            {
                var addressId = address.GetProperty("addressId").GetInt32();
                var deleteResponse = await _httpClient.DeleteAsync($"{BaseUrl}/delete/{addressId}");
                if (!deleteResponse.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Failed to delete address {addressId}: {deleteResponse.StatusCode}");
                    return false;
                }
                else
                {
                    Console.WriteLine($"Successfully deleted address with ID: {addressId}");
                }
            }

            Console.WriteLine($"All addresses for customer {customerId} have been deleted successfully.");
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error while deleting addresses for customer {customerId}: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> CheckAddressUpdatedAsync(AddressDTO addressDto)
    {
        // Use the new endpoint for getting a specific address
        var response = await _httpClient.GetAsync($"{BaseUrl}/get/{addressDto.AddressId}");

        if (response.IsSuccessStatusCode)
        {
            var fetchedAddress = await response.Content.ReadFromJsonAsync<AddressDTO>();
            if (fetchedAddress != null)
            {
                // Check if the address details match
                return fetchedAddress.StreetAddress == addressDto.StreetAddress &&
                       fetchedAddress.City == addressDto.City &&
                       fetchedAddress.Province == addressDto.Province &&
                       fetchedAddress.PostalCode == addressDto.PostalCode &&
                       fetchedAddress.Country == addressDto.Country &&
                       fetchedAddress.IsPrimary == addressDto.IsPrimary;
            }
        }

        return false; // Address not found or check failed
    }

    public async Task<bool> UpdateAddressAsync(AddressDTO addressDto)
    {
        var response = await _httpClient.PutAsJsonAsync($"{BaseUrl}/update/{addressDto.AddressId}", addressDto);

        if (response.IsSuccessStatusCode)
        {
            return true;
        }
        else
        {
            var error = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Failed to update address: {error}");
            return false;
        }
    }
}
