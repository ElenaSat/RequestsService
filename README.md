# RequestsService - Microservicio Empresarial (.NET 8)

Un servicio de gestión de solicitudes genérico, robusto y de grado empresarial, construido con **ASP.NET Core Web API**, siguiendo los principios de **Clean Architecture** y **SOLID**.

## 🚀 Características Principales
- **Arquitectura Empresarial**: API basada en Controllers diseñada para escalabilidad y complejidad.
- **Patrón CQRS**: Responsabilidades de Command y Query desacopladas utilizando **MediatR**.
- **API Versioning**: Soporte nativo para versionado (ej., `/api/v1/solicitudes`).
- **OpenAPI/Swagger**: Documentación completa de la API con soporte para versionado.
- **Validación**: Validación estricta de solicitudes utilizando **FluentValidation**.
- **Resiliencia**: Integración con **Azure Queue Storage** para mensajería de eventos confiable.
- **Pruebas**: Suite completa de **Unit Tests** e **Integration Tests** utilizando `xUnit`, `Moq` y `WebApplicationFactory`.

## 🏗️ Arquitectura
La solución sigue la regla de dependencia estricta de Clean Architecture:
`Domain` ← `Application` ← `Infrastructure` / `Api`

### Proyectos
- **RequestsService.Domain**: Entidades principales (`Solicitud`), enumeraciones e interfaces de repositorios.
- **RequestsService.Application**: Casos de uso implementados como Commands/Queries (CQRS), DTOs y Validadores.
- **RequestsService.Infrastructure**: Implementación de preocupaciones externas (Persistencia, Azure Queue).
- **RequestsService.Api**: Punto de entrada, Controllers, Middleware, configuración de DI.
- **RequestsService.Tests.Unit**: Pruebas unitarias para la lógica de Application.
- **RequestsService.Tests.Integration**: Pruebas de API de extremo a extremo (End-to-end).

## 🏃‍♂️ Primeros Pasos

### Prerrequisitos
- .NET 8 SDK
- Docker (opcional)
- Emulador de Azure Storage (Azurite) o Cuenta de Azure

### Ejecución Local
1. **Restaurar Dependencias**:
   ```bash
   dotnet restore
   ```
2. **Ejecutar la API**:
   ```bash
   dotnet run --project RequestsService.Api
   ```
   Acceda a la UI de Swagger en: `https://localhost:7198/swagger` (o puerto configurado).

### Configuración
Configure `appsettings.json` o Variables de Entorno para Azure Queue Storage:
```json
"AzureQueueStorage": {
  "ConnectionString": "UseDevelopmentStorage=true",
  "QueueName": "request-created-queue"
}
```

## 🧪 Ejecutando Pruebas
Ejecute la suite completa de pruebas (Unit + Integration):
```bash
dotnet test
```

## 📋 Endpoints de la API (v1)
- **POST** `/api/v1/solicitudes` - Crear una nueva solicitud.
- **GET** `/api/v1/solicitudes/{id}` - Obtener una solicitud por ID.
- **GET** `/api/v1/solicitudes` - Obtener todas las solicitudes.

## 🐳 Docker
Construir y ejecutar con Docker:
```bash
docker build -t requestsservice .
docker run -p 8080:8080 requestsservice
```