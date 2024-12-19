using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using TechSolutions;
using TechSolutions.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);


// Add configuration
builder.Services.AddSingleton<IConfiguration>(builder.Configuration);

// Add MudBlazor services
builder.Services.AddMudServices();

// Register the LoginService
builder.Services.AddScoped<LoginService>();
builder.Services.AddScoped<CustomerService>();
builder.Services.AddScoped<AddressService>();


string baseUrl = builder.Configuration["ApiSettings:BaseUrl"];
builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri(baseUrl),
    DefaultRequestHeaders =
    {
        { "Accept", "application/json" },
        { "withCredentials", "true" }
    }
});



// Add the root component
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Build and run the app
await builder.Build().RunAsync();
