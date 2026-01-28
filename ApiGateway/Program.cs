using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();

// Configuración del Service Discovery
var discoveryUrl = builder.Configuration["ServiceDiscovery:Url"] ?? "http://localhost:5000";
builder.Services.AddSingleton(new DiscoveryConfig { Url = discoveryUrl });

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Helper para retornar error 503
IResult ServiceUnavailable(string message) => Results.Json(new { error = message }, statusCode: 503);

// Middleware para verificar que el Service Discovery esté disponible
async Task<bool> IsDiscoveryAvailable(HttpClient httpClient, string discoveryUrl)
{
    try
    {
        var response = await httpClient.GetAsync($"{discoveryUrl}/health");
        return response.IsSuccessStatusCode;
    }
    catch
    {
        return false;
    }
}

// Obtener URL del servicio desde el Discovery
async Task<string?> GetServiceUrl(HttpClient httpClient, string discoveryUrl, string serviceName)
{
    try
    {
        var response = await httpClient.GetAsync($"{discoveryUrl}/service/{serviceName}");
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            var serviceInfo = JsonSerializer.Deserialize<ServiceInfo>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return serviceInfo?.Url;
        }
    }
    catch
    {
        // Service not available
    }
    return null;
}

// Health check del Gateway
app.MapGet("/health", () => Results.Ok(new { status = "healthy", service = "ApiGateway" }));

// ============== ENDPOINTS DE CLIENTES ==============

app.MapGet("/api/clientes", async (HttpClient httpClient, DiscoveryConfig config) =>
{
    if (!await IsDiscoveryAvailable(httpClient, config.Url))
        return ServiceUnavailable("Service Discovery no disponible");

    var serviceUrl = await GetServiceUrl(httpClient, config.Url, "ApiCliente");
    if (serviceUrl == null)
        return ServiceUnavailable("ApiCliente no disponible");

    try
    {
        var response = await httpClient.GetAsync($"{serviceUrl}/api/clientes");
        var content = await response.Content.ReadAsStringAsync();
        return Results.Content(content, "application/json", statusCode: (int)response.StatusCode);
    }
    catch
    {
        return ServiceUnavailable("Error al conectar con ApiCliente");
    }
});

app.MapGet("/api/clientes/{id}", async (int id, HttpClient httpClient, DiscoveryConfig config) =>
{
    if (!await IsDiscoveryAvailable(httpClient, config.Url))
        return ServiceUnavailable("Service Discovery no disponible");

    var serviceUrl = await GetServiceUrl(httpClient, config.Url, "ApiCliente");
    if (serviceUrl == null)
        return ServiceUnavailable("ApiCliente no disponible");

    try
    {
        var response = await httpClient.GetAsync($"{serviceUrl}/api/clientes/{id}");
        var content = await response.Content.ReadAsStringAsync();
        return Results.Content(content, "application/json", statusCode: (int)response.StatusCode);
    }
    catch
    {
        return ServiceUnavailable("Error al conectar con ApiCliente");
    }
});

app.MapPost("/api/clientes", async (HttpContext context, HttpClient httpClient, DiscoveryConfig config) =>
{
    if (!await IsDiscoveryAvailable(httpClient, config.Url))
        return ServiceUnavailable("Service Discovery no disponible");

    var serviceUrl = await GetServiceUrl(httpClient, config.Url, "ApiCliente");
    if (serviceUrl == null)
        return ServiceUnavailable("ApiCliente no disponible");

    try
    {
        var body = await new StreamReader(context.Request.Body).ReadToEndAsync();
        var content = new StringContent(body, System.Text.Encoding.UTF8, "application/json");
        var response = await httpClient.PostAsync($"{serviceUrl}/api/clientes", content);
        var responseContent = await response.Content.ReadAsStringAsync();
        return Results.Content(responseContent, "application/json", statusCode: (int)response.StatusCode);
    }
    catch
    {
        return ServiceUnavailable("Error al conectar con ApiCliente");
    }
});

app.MapPut("/api/clientes/{id}", async (int id, HttpContext context, HttpClient httpClient, DiscoveryConfig config) =>
{
    if (!await IsDiscoveryAvailable(httpClient, config.Url))
        return ServiceUnavailable("Service Discovery no disponible");

    var serviceUrl = await GetServiceUrl(httpClient, config.Url, "ApiCliente");
    if (serviceUrl == null)
        return ServiceUnavailable("ApiCliente no disponible");

    try
    {
        var body = await new StreamReader(context.Request.Body).ReadToEndAsync();
        var content = new StringContent(body, System.Text.Encoding.UTF8, "application/json");
        var response = await httpClient.PutAsync($"{serviceUrl}/api/clientes/{id}", content);
        var responseContent = await response.Content.ReadAsStringAsync();
        return Results.Content(responseContent, "application/json", statusCode: (int)response.StatusCode);
    }
    catch
    {
        return ServiceUnavailable("Error al conectar con ApiCliente");
    }
});

app.MapDelete("/api/clientes/{id}", async (int id, HttpClient httpClient, DiscoveryConfig config) =>
{
    if (!await IsDiscoveryAvailable(httpClient, config.Url))
        return ServiceUnavailable("Service Discovery no disponible");

    var serviceUrl = await GetServiceUrl(httpClient, config.Url, "ApiCliente");
    if (serviceUrl == null)
        return ServiceUnavailable("ApiCliente no disponible");

    try
    {
        var response = await httpClient.DeleteAsync($"{serviceUrl}/api/clientes/{id}");
        var responseContent = await response.Content.ReadAsStringAsync();
        return Results.Content(responseContent, "application/json", statusCode: (int)response.StatusCode);
    }
    catch
    {
        return ServiceUnavailable("Error al conectar con ApiCliente");
    }
});

// ============== ENDPOINTS DE EMPRESAS ==============

app.MapGet("/api/empresas", async (HttpClient httpClient, DiscoveryConfig config) =>
{
    if (!await IsDiscoveryAvailable(httpClient, config.Url))
        return ServiceUnavailable("Service Discovery no disponible");

    var serviceUrl = await GetServiceUrl(httpClient, config.Url, "ApiEmpresa");
    if (serviceUrl == null)
        return ServiceUnavailable("ApiEmpresa no disponible");

    try
    {
        var response = await httpClient.GetAsync($"{serviceUrl}/api/empresas");
        var content = await response.Content.ReadAsStringAsync();
        return Results.Content(content, "application/json", statusCode: (int)response.StatusCode);
    }
    catch
    {
        return ServiceUnavailable("Error al conectar con ApiEmpresa");
    }
});

app.MapGet("/api/empresas/{id}", async (int id, HttpClient httpClient, DiscoveryConfig config) =>
{
    if (!await IsDiscoveryAvailable(httpClient, config.Url))
        return ServiceUnavailable("Service Discovery no disponible");

    var serviceUrl = await GetServiceUrl(httpClient, config.Url, "ApiEmpresa");
    if (serviceUrl == null)
        return ServiceUnavailable("ApiEmpresa no disponible");

    try
    {
        var response = await httpClient.GetAsync($"{serviceUrl}/api/empresas/{id}");
        var content = await response.Content.ReadAsStringAsync();
        return Results.Content(content, "application/json", statusCode: (int)response.StatusCode);
    }
    catch
    {
        return ServiceUnavailable("Error al conectar con ApiEmpresa");
    }
});

app.MapPost("/api/empresas", async (HttpContext context, HttpClient httpClient, DiscoveryConfig config) =>
{
    if (!await IsDiscoveryAvailable(httpClient, config.Url))
        return ServiceUnavailable("Service Discovery no disponible");

    var serviceUrl = await GetServiceUrl(httpClient, config.Url, "ApiEmpresa");
    if (serviceUrl == null)
        return ServiceUnavailable("ApiEmpresa no disponible");

    try
    {
        var body = await new StreamReader(context.Request.Body).ReadToEndAsync();
        var content = new StringContent(body, System.Text.Encoding.UTF8, "application/json");
        var response = await httpClient.PostAsync($"{serviceUrl}/api/empresas", content);
        var responseContent = await response.Content.ReadAsStringAsync();
        return Results.Content(responseContent, "application/json", statusCode: (int)response.StatusCode);
    }
    catch
    {
        return ServiceUnavailable("Error al conectar con ApiEmpresa");
    }
});

app.MapPut("/api/empresas/{id}", async (int id, HttpContext context, HttpClient httpClient, DiscoveryConfig config) =>
{
    if (!await IsDiscoveryAvailable(httpClient, config.Url))
        return ServiceUnavailable("Service Discovery no disponible");

    var serviceUrl = await GetServiceUrl(httpClient, config.Url, "ApiEmpresa");
    if (serviceUrl == null)
        return ServiceUnavailable("ApiEmpresa no disponible");

    try
    {
        var body = await new StreamReader(context.Request.Body).ReadToEndAsync();
        var content = new StringContent(body, System.Text.Encoding.UTF8, "application/json");
        var response = await httpClient.PutAsync($"{serviceUrl}/api/empresas/{id}", content);
        var responseContent = await response.Content.ReadAsStringAsync();
        return Results.Content(responseContent, "application/json", statusCode: (int)response.StatusCode);
    }
    catch
    {
        return ServiceUnavailable("Error al conectar con ApiEmpresa");
    }
});

app.MapDelete("/api/empresas/{id}", async (int id, HttpClient httpClient, DiscoveryConfig config) =>
{
    if (!await IsDiscoveryAvailable(httpClient, config.Url))
        return ServiceUnavailable("Service Discovery no disponible");

    var serviceUrl = await GetServiceUrl(httpClient, config.Url, "ApiEmpresa");
    if (serviceUrl == null)
        return ServiceUnavailable("ApiEmpresa no disponible");

    try
    {
        var response = await httpClient.DeleteAsync($"{serviceUrl}/api/empresas/{id}");
        var responseContent = await response.Content.ReadAsStringAsync();
        return Results.Content(responseContent, "application/json", statusCode: (int)response.StatusCode);
    }
    catch
    {
        return ServiceUnavailable("Error al conectar con ApiEmpresa");
    }
});

app.Run();

// Clases auxiliares
public class DiscoveryConfig
{
    public string Url { get; set; } = string.Empty;
}

public class ServiceInfo
{
    public string Name { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public bool IsHealthy { get; set; }
}
