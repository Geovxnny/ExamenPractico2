# Arquitectura de Microservicios con Service Discovery

## Descripción

Este proyecto implementa una arquitectura de microservicios con los siguientes componentes:

1. **API Gateway** (Puerto 5003) - Punto de entrada único para los usuarios
2. **Service Discovery** (Puerto 5000) - Registro y descubrimiento de servicios
3. **API Cliente** (Puerto 5001) - Gestión de clientes
4. **API Empresa** (Puerto 5002) - Gestión de empresas

## Arquitectura

```
Usuario → API Gateway → Service Discovery → API Cliente / API Empresa
```

## Modelos de Datos

### Cliente
- `id`: int
- `nombre`: string
- `apellido`: string
- `telefono`: string

### Empresa
- `id`: int
- `nombre`: string
- `ruc`: string
- `direccion`: string

## Requisitos

- Docker Desktop instalado y en ejecución
- .NET 8.0 SDK (para desarrollo local)

## Ejecución con Docker

### 1. Construir e iniciar los contenedores

```bash
docker-compose up --build -d
```

### 2. Registrar los servicios en el Service Discovery

```powershell
.\register-services.ps1
```

O manualmente:

```bash
# Registrar ApiCliente
curl -X POST http://localhost:5000/register -H "Content-Type: application/json" -d "{\"name\":\"ApiCliente\",\"url\":\"http://api-cliente:80\"}"

# Registrar ApiEmpresa
curl -X POST http://localhost:5000/register -H "Content-Type: application/json" -d "{\"name\":\"ApiEmpresa\",\"url\":\"http://api-empresa:80\"}"
```

### 3. Verificar servicios registrados

```bash
curl http://localhost:5000/services
```

## Endpoints del API Gateway

### Clientes (http://localhost:5003)

| Método | Endpoint | Descripción |
|--------|----------|-------------|
| GET | /api/clientes | Obtener todos los clientes |
| GET | /api/clientes/{id} | Obtener cliente por ID |
| POST | /api/clientes | Crear nuevo cliente |
| PUT | /api/clientes/{id} | Actualizar cliente |
| DELETE | /api/clientes/{id} | Eliminar cliente |

### Empresas (http://localhost:5003)

| Método | Endpoint | Descripción |
|--------|----------|-------------|
| GET | /api/empresas | Obtener todas las empresas |
| GET | /api/empresas/{id} | Obtener empresa por ID |
| POST | /api/empresas | Crear nueva empresa |
| PUT | /api/empresas/{id} | Actualizar empresa |
| DELETE | /api/empresas/{id} | Eliminar empresa |

## Ejemplos de Uso

### Obtener todos los clientes
```bash
curl http://localhost:5003/api/clientes
```

### Crear un cliente
```bash
curl -X POST http://localhost:5003/api/clientes -H "Content-Type: application/json" -d "{\"nombre\":\"Pedro\",\"apellido\":\"Martínez\",\"telefono\":\"0991111111\"}"
```

### Obtener todas las empresas
```bash
curl http://localhost:5003/api/empresas
```

### Crear una empresa
```bash
curl -X POST http://localhost:5003/api/empresas -H "Content-Type: application/json" -d "{\"nombre\":\"NuevaCorp\",\"ruc\":\"9999999999001\",\"direccion\":\"Calle Nueva 123\"}"
```

## Pruebas de Resiliencia

### Si el Service Discovery se cae:

```bash
# Detener el Service Discovery
docker stop service-discovery

# Intentar consultar clientes (debe fallar)
curl http://localhost:5003/api/clientes
# Respuesta: {"error":"Service Discovery no disponible"}

# Reactivar el Service Discovery
docker start service-discovery
```

### Si la API Empresa se cae:

```bash
# Detener API Empresa
docker stop api-empresa

# Intentar consultar empresas (debe fallar)
curl http://localhost:5003/api/empresas
# Respuesta: {"error":"ApiEmpresa no disponible"}

# La API Cliente sigue funcionando
curl http://localhost:5003/api/clientes
# Respuesta: [lista de clientes]

# Reactivar API Empresa
docker start api-empresa
```

## Detener todos los servicios

```bash
docker-compose down
```

## Desarrollo Local (sin Docker)

### 1. Ejecutar Service Discovery
```bash
cd ServiceDiscovery
dotnet run --urls=http://localhost:5000
```

### 2. Ejecutar API Cliente
```bash
cd ApiCliente
dotnet run --urls=http://localhost:5001
```

### 3. Ejecutar API Empresa
```bash
cd ApiEmpresa
dotnet run --urls=http://localhost:5002
```

### 4. Ejecutar API Gateway
```bash
cd ApiGateway
dotnet run --urls=http://localhost:5003
```

### 5. Registrar servicios (para desarrollo local)
```bash
curl -X POST http://localhost:5000/register -H "Content-Type: application/json" -d "{\"name\":\"ApiCliente\",\"url\":\"http://localhost:5001\"}"
curl -X POST http://localhost:5000/register -H "Content-Type: application/json" -d "{\"name\":\"ApiEmpresa\",\"url\":\"http://localhost:5002\"}"
```

## Estructura del Proyecto

```
ExamenPractico2/
├── ApiCliente/
│   ├── Controllers/
│   │   └── ClientesController.cs
│   ├── Models/
│   │   └── Cliente.cs
│   ├── Program.cs
│   ├── Dockerfile
│   └── ApiCliente.csproj
├── ApiEmpresa/
│   ├── Controllers/
│   │   └── EmpresasController.cs
│   ├── Models/
│   │   └── Empresa.cs
│   ├── Program.cs
│   ├── Dockerfile
│   └── ApiEmpresa.csproj
├── ServiceDiscovery/
│   ├── Program.cs
│   ├── Dockerfile
│   └── ServiceDiscovery.csproj
├── ApiGateway/
│   ├── Program.cs
│   ├── Dockerfile
│   ├── appsettings.json
│   └── ApiGateway.csproj
├── docker-compose.yml
├── register-services.ps1
└── README.md
```
