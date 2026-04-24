using HamburguerDev.Frontend.Components;
using HamburguerDev.Frontend.Configuration;
using HamburguerDev.Frontend.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.Configure<ApiSettings>(builder.Configuration.GetSection(ApiSettings.SectionName));

builder.Services.AddHttpClient<IPedidoApiService, PedidoApiService>((provider, client) =>
{
    var apiSettings = provider.GetRequiredService<Microsoft.Extensions.Options.IOptions<ApiSettings>>().Value;
    var baseUrl = string.IsNullOrWhiteSpace(apiSettings.BaseUrl) ? "https://localhost:7113/" : apiSettings.BaseUrl;
    client.BaseAddress = new Uri(baseUrl);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();


app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
