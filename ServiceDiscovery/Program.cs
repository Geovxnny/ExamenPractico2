using System.Collections.Concurrent;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<ServiceRegistry>();
builder.Services.AddHostedService<HealthCheckService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Registrar un servicio
app.MapPost("/register", (ServiceInfo service, ServiceRegistry registry) =>
{
    registry.Register(service);
    return Results.Ok(new { message = $"Servicio {service.Name} registrado correctamente" });
});

// Desregistrar un servicio
app.MapDelete("/unregister/{name}", (string name, ServiceRegistry registry) =>
{
    registry.Unregister(name);
    return Results.Ok(new { message = $"Servicio {name} desregistrado" });
});

// Obtener un servicio por nombre
app.MapGet("/service/{name}", (string name, ServiceRegistry registry) =>
{
    var service = registry.GetService(name);
    if (service == null)
        return Results.NotFound(new { message = $"Servicio {name} no encontrado o no disponible" });
    return Results.Ok(service);
});

// Obtener todos los servicios
app.MapGet("/services", (ServiceRegistry registry) =>
{
    return Results.Ok(registry.GetAllServices());
});

// Health check del discovery
app.MapGet("/health", () => Results.Ok(new { status = "healthy", service = "ServiceDiscovery" }));

app.Run();

// Modelo de información del servicio
public class ServiceInfo
{
    public string Name { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public bool IsHealthy { get; set; } = true;
    public DateTime LastHealthCheck { get; set; } = DateTime.UtcNow;
}

// Registro de servicios
public class ServiceRegistry
{
    private readonly ConcurrentDictionary<string, ServiceInfo> _services = new();

    public void Register(ServiceInfo service)
    {
        service.LastHealthCheck = DateTime.UtcNow;
        service.IsHealthy = true;
        _services.AddOrUpdate(service.Name, service, (key, old) => service);
    }

    public void Unregister(string name)
    {
        _services.TryRemove(name, out _);
    }

    public ServiceInfo? GetService(string name)
    {
        if (_services.TryGetValue(name, out var service) && service.IsHealthy)
            return service;
        return null;
    }

    public IEnumerable<ServiceInfo> GetAllServices()
    {
        return _services.Values.ToList();
    }

    public void UpdateHealth(string name, bool isHealthy)
    {
        if (_services.TryGetValue(name, out var service))
        {
            service.IsHealthy = isHealthy;
            service.LastHealthCheck = DateTime.UtcNow;
        }
    }

    public IEnumerable<ServiceInfo> GetAllServicesForHealthCheck()
    {
        return _services.Values.ToList();
    }
}

// Servicio de health check periódico
public class HealthCheckService : BackgroundService
{
    private readonly ServiceRegistry _registry;
    private readonly HttpClient _httpClient;
    private readonly ILogger<HealthCheckService> _logger;

    public HealthCheckService(ServiceRegistry registry, ILogger<HealthCheckService> logger)
    {
        _registry = registry;
        _httpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(5) };
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            
            foreach (var service in _registry.GetAllServicesForHealthCheck())
            {
                try
                {
                    var response = await _httpClient.GetAsync($"{service.Url}/health", stoppingToken);
                    _registry.UpdateHealth(service.Name, response.IsSuccessStatusCode);
                    _logger.LogInformation("Health check for {Service}: {Status}", service.Name, response.IsSuccessStatusCode ? "Healthy" : "Unhealthy");
                }
                catch (Exception ex)
                {
                    _registry.UpdateHealth(service.Name, false);
                    _logger.LogWarning("Health check failed for {Service}: {Error}", service.Name, ex.Message);
                }
            }
        }
    }
}
