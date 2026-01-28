# Script para registrar los servicios en el Service Discovery
# Ejecutar después de que todos los contenedores estén corriendo

Write-Host "Esperando a que los servicios estén listos..."
Start-Sleep -Seconds 10

$discoveryUrl = "http://localhost:5000"

# Registrar ApiCliente
Write-Host "Registrando ApiCliente..."
$clienteBody = @{
    name = "ApiCliente"
    url = "http://api-cliente:80"
} | ConvertTo-Json

try {
    Invoke-RestMethod -Uri "$discoveryUrl/register" -Method Post -Body $clienteBody -ContentType "application/json"
    Write-Host "ApiCliente registrado exitosamente" -ForegroundColor Green
} catch {
    Write-Host "Error registrando ApiCliente: $_" -ForegroundColor Red
}

# Registrar ApiEmpresa
Write-Host "Registrando ApiEmpresa..."
$empresaBody = @{
    name = "ApiEmpresa"
    url = "http://api-empresa:80"
} | ConvertTo-Json

try {
    Invoke-RestMethod -Uri "$discoveryUrl/register" -Method Post -Body $empresaBody -ContentType "application/json"
    Write-Host "ApiEmpresa registrado exitosamente" -ForegroundColor Green
} catch {
    Write-Host "Error registrando ApiEmpresa: $_" -ForegroundColor Red
}

Write-Host ""
Write-Host "Servicios registrados. Verificando..."
Write-Host ""

# Verificar servicios registrados
try {
    $services = Invoke-RestMethod -Uri "$discoveryUrl/services" -Method Get
    Write-Host "Servicios disponibles:" -ForegroundColor Cyan
    $services | ForEach-Object { Write-Host "  - $($_.name): $($_.url) (Healthy: $($_.isHealthy))" }
} catch {
    Write-Host "Error obteniendo servicios: $_" -ForegroundColor Red
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Yellow
Write-Host "ENDPOINTS DISPONIBLES:" -ForegroundColor Yellow
Write-Host "========================================" -ForegroundColor Yellow
Write-Host "API Gateway: http://localhost:5003" -ForegroundColor Cyan
Write-Host "  - GET    /api/clientes" -ForegroundColor White
Write-Host "  - GET    /api/clientes/{id}" -ForegroundColor White
Write-Host "  - POST   /api/clientes" -ForegroundColor White
Write-Host "  - PUT    /api/clientes/{id}" -ForegroundColor White
Write-Host "  - DELETE /api/clientes/{id}" -ForegroundColor White
Write-Host ""
Write-Host "  - GET    /api/empresas" -ForegroundColor White
Write-Host "  - GET    /api/empresas/{id}" -ForegroundColor White
Write-Host "  - POST   /api/empresas" -ForegroundColor White
Write-Host "  - PUT    /api/empresas/{id}" -ForegroundColor White
Write-Host "  - DELETE /api/empresas/{id}" -ForegroundColor White
Write-Host ""
Write-Host "Service Discovery: http://localhost:5000" -ForegroundColor Cyan
Write-Host "  - GET  /services (ver todos los servicios)" -ForegroundColor White
Write-Host "  - GET  /health" -ForegroundColor White
Write-Host "========================================"  -ForegroundColor Yellow
