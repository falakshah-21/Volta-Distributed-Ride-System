using Blazored.SessionStorage;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Volta.Client;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddBlazoredSessionStorage();

// 🔥 DYNAMIC CONFIGURATION (The Magic Fix)
// 1. Get the current URL from the browser (e.g., "http://FALAK:5006" OR "http://192.168.1.5:5006")
var browserUri = new Uri(builder.HostEnvironment.BaseAddress);

// 2. Keep the Host (FALAK or IP) but switch the Port to 5000 (Gateway)
string gatewayUrl = $"{browserUri.Scheme}://{browserUri.Host}:5000";

// 3. Use this smart URL for the connection
builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri(gatewayUrl)
});

await builder.Build().RunAsync();