API de Gestión de Automóviles
Una API RESTful construida con .NET 8 que implementa operaciones CRUD completas para la gestión de automóviles, utilizando arquitectura DDD (Domain-Driven Design) y Entity Framework Core con SQL Server LocalDB.
Características
    • Arquitectura DDD Híbrida con separación clara de responsabilidades 
    • 6 endpoints REST para operaciones completas de CRUD 
    • Base de datos SQL Server LocalDB con Entity Framework Core 
    • Validaciones múltiples a nivel de DTO, servicio y base de datos 
    • Documentación automática con Swagger/OpenAPI 
    • Manejo centralizado de errores con filtros personalizados 
    • Respuestas estandarizadas con wrapper de API 
Tecnologías Utilizadas
    • .NET 8 - Framework principal 
    • ASP.NET Core Web API - Framework web 
    • Entity Framework Core 9 - ORM para acceso a datos 
    • SQL Server LocalDB - Base de datos 
    • FluentValidation - Validaciones de dominio 
    • Swagger/OpenAPI - Documentación de API 

    Modelo de Datos
Entidad Automóvil
Campo
Tipo
Descripción
Id
int
Identificador único (autoincremental)
Marca
string(50)
Marca del automóvil
Modelo
string(50)
Modelo del automóvil
Color
string(30)
Color del automóvil
Fabricacion
int
Año de fabricación (1900-2030)
NumeroMotor
string(20)
Número de motor (único)
NumeroChasis
string(17)
Número de chasis (único)
FechaCreacion
DateTime
Fecha de creación del registro
FechaActualizacion
DateTime
Fecha de última actualización
Endpoints de la API
Base URL: /api/v1/automovil
Método
Endpoint
Descripción
Status Codes
GET
/
Obtener todos los automóviles
200, 500
GET
/{id}
Obtener automóvil por ID
200, 404, 400, 500
GET
/chasis/{numeroChasis}
Obtener por número de chasis
200, 404, 400, 500
POST
/
Crear nuevo automóvil
201, 400, 409, 500
PUT
/{id}
Actualizar automóvil
200, 400, 404, 409, 500
DELETE
/{id}
Eliminar automóvil
200, 404, 400, 500
Ejemplos de Uso
Crear Automóvil
json
POST /api/v1/automovil
Content-Type: application/json

{
  "marca": "Toyota",
  "modelo": "Corolla",
  "color": "Blanco",
  "fabricacion": 2023,
  "numeroMotor": "TOY2023001",
  "numeroChasis": "1HGCM82633A123456"
}
Actualizar Automóvil
json
PUT /api/v1/automovil/1
Content-Type: application/json

{
  "color": "Rojo",
  "numeroMotor": "DEF789123"
}
Respuesta Estándar
json
{
  "success": true,
  "message": "Automóvil creado exitosamente",
  "data": {
    "id": 1,
    "marca": "Toyota",
    "modelo": "Corolla",
    "color": "Blanco",
    "fabricacion": 2023,
    "numeroMotor": "TOY2023001",
    "numeroChasis": "1HGCM82633A123456",
    "fechaCreacion": "2024-01-15T10:30:00Z",
    "fechaActualizacion": "2024-01-15T10:30:00Z"
  },
  "errors": []
}
Arquitectura
Capas Principales
    1. Presentation (Template-API) 
        ◦ Controladores REST 
        ◦ Filtros de excepción 
        ◦ Configuración de Swagger 
    2. Application (Core.Application + Application) 
        ◦ Servicios de aplicación 
        ◦ DTOs 
        ◦ Lógica de casos de uso 
    3. Domain (Core.Domain + Domain) 
        ◦ Entidades del dominio 
        ◦ Reglas de negocio 
        ◦ Validaciones 
    4. Infrastructure (Core.Infrastructure.*) 
        ◦ Repositorios 
        ◦ Contexto de base de datos 
        ◦ Servicios externos 
Patrones Implementados
    • Repository Pattern: Abstracción del acceso a datos 
    • Service Layer: Lógica de aplicación 
    • DTO Pattern: Transferencia de datos 
    • Dependency Injection: Inversión de control 
    • Response Wrapper: Respuestas estandarizadas 
    Desarrollo
Agregar Nueva Funcionalidad
    1. Crear entidad en Core.Domain 
    2. Definir DTOs en Core.Application 
    3. Implementar servicio en Application 
    4. Crear repositorio en Infrastructure 
    5. Agregar controlador en Template-API 
    6. Ejecutar migraciones 

Changelog
v11.0.0 (2025-09-25)
