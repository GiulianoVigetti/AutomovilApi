using Microsoft.AspNetCore.Mvc;
using HybridDDDArchitecture.Core.Application.ApplicationServices;
using HybridDDDArchitecture.Core.Application.DataTransferObjects;
using HybridDDDArchitecture.Core.Application.Wrappers;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace HybridDDDArchitecture.Presentation.Controllers
{
    [ApiController]
    [Route("api/v1/automovil")]
    [Produces("application/json")]
    public class AutomovilController : ControllerBase
    {
        private readonly IAutomovilService _automovilService;
        private readonly ILogger<AutomovilController> _logger;

        public AutomovilController(
            IAutomovilService automovilService,
            ILogger<AutomovilController> logger)
        {
            _automovilService = automovilService;
            _logger = logger;
        }

        // ------------------------------
        // ENDPOINTS
        // ------------------------------

        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<AutomovilResponseDto>>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<ApiResponse<IEnumerable<AutomovilResponseDto>>>> GetAll()
        {
            _logger.LogInformation("Iniciando obtención de todos los automóviles - {Timestamp}", DateTime.UtcNow);
            var result = await _automovilService.GetAllAsync();

            if (result.Success)
            {
                return Ok(result);
            }

            return StatusCode((int)HttpStatusCode.InternalServerError, result);
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(ApiResponse<AutomovilResponseDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<ApiResponse<AutomovilResponseDto>>> GetById([FromRoute] int id)
        {
            if (id <= 0)
            {
                return BadRequest(new ApiResponse<object>(false, "El ID debe ser un número positivo mayor a cero", null, new List<string> { $"ID inválido: {id}" }));
            }

            var result = await _automovilService.GetByIdAsync(id);

            if (result.Success)
            {
                return Ok(result);
            }

            return NotFound(result);
        }

        [HttpGet("chasis/{numeroChasis}")]
        [ProducesResponseType(typeof(ApiResponse<AutomovilResponseDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<ApiResponse<AutomovilResponseDto>>> GetByChasis([FromRoute][Required] string numeroChasis)
        {
            if (string.IsNullOrWhiteSpace(numeroChasis) || numeroChasis.Length != 17)
            {
                var errors = new List<string>();
                if (string.IsNullOrWhiteSpace(numeroChasis)) errors.Add("El número de chasis es requerido.");
                if (numeroChasis?.Length != 17) errors.Add("El número de chasis debe tener 17 caracteres.");

                return BadRequest(new ApiResponse<object>(false, "Datos inválidos", null, errors));
            }

            var result = await _automovilService.GetByChasisAsync(numeroChasis.ToUpper().Trim());

            if (result.Success)
            {
                return Ok(result);
            }

            return NotFound(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<AutomovilResponseDto>), (int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), (int)HttpStatusCode.Conflict)]
        [ProducesResponseType(typeof(ApiResponse<object>), (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<ApiResponse<AutomovilResponseDto>>> Create([FromBody] AutomovilCreateDto createDto)
        {
            if (createDto == null)
            {
                return BadRequest(new ApiResponse<object>(false, "Los datos del automóvil son requeridos."));
            }

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return BadRequest(new ApiResponse<object>(false, "Los datos proporcionados no son válidos.", null, errors));
            }

            var result = await _automovilService.CreateAsync(createDto);

            if (result.Success)
            {
                return CreatedAtAction(nameof(GetById), new { id = result.Data?.Id }, result);
            }

            return BadRequest(result);
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(ApiResponse<AutomovilResponseDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), (int)HttpStatusCode.Conflict)]
        [ProducesResponseType(typeof(ApiResponse<object>), (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<ApiResponse<AutomovilResponseDto>>> Update([FromRoute] int id, [FromBody] AutomovilUpdateDto updateDto)
        {
            if (id <= 0)
            {
                return BadRequest(new ApiResponse<object>(false, "El ID debe ser un número positivo."));
            }

            if (updateDto == null)
            {
                return BadRequest(new ApiResponse<object>(false, "Los datos de actualización son requeridos."));
            }

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return BadRequest(new ApiResponse<object>(false, "Los datos no son válidos.", null, errors));
            }

            var result = await _automovilService.UpdateAsync(id, updateDto);

            if (result.Success)
            {
                return Ok(result);
            }

            // Determina el tipo de error para devolver el código de estado correcto
            if (result.Message.Contains("No se encontró") || result.Message.Contains("No existe"))
            {
                return NotFound(result);
            }

            return BadRequest(result);
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(typeof(ApiResponse<string>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<ApiResponse<string>>> Delete([FromRoute] int id)
        {
            if (id <= 0)
            {
                return BadRequest(new ApiResponse<object>(false, "El ID debe ser positivo."));
            }

            var result = await _automovilService.DeleteAsync(id);

            if (result.Success)
            {
                return Ok(result);
            }

            return NotFound(result);
        }

        [HttpGet("health")]
        [ProducesResponseType(typeof(object), (int)HttpStatusCode.OK)]
        public ActionResult GetHealth()
        {
            return Ok(new
            {
                Controller = nameof(AutomovilController),
                Status = "Healthy",
                Timestamp = DateTime.UtcNow,
                Version = "1.0.0"
            });
        }
    }
}
/*444444444444444444444444444444444444444444444444444444444444444444444*/
/*using Microsoft.AspNetCore.Mvc;
using AutomovilAPI.Application.Interfaces;   // <- acá estaba el error, antes usabas Services directamente
using AutomovilAPI.Domain.DTOs;
using System.ComponentModel.DataAnnotations;

namespace AutomovilAPI.Presentation.Controllers
{
    [ApiController]
    [Route("api/v1/automovil")]
    [Produces("application/json")]
    public class AutomovilController : ControllerBase
    {
        private readonly IAutomovilService _automovilService;
        private readonly ILogger<AutomovilController> _logger;

        public AutomovilController(
            IAutomovilService automovilService,
            ILogger<AutomovilController> logger)
        {
            _automovilService = automovilService;
            _logger = logger;
        }

        // ------------------------------
        // ENDPOINTS
        // ------------------------------

        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<AutomovilResponseDto>>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<ActionResult<ApiResponse<IEnumerable<AutomovilResponseDto>>>> GetAll()
        {
            _logger.LogInformation("Iniciando obtención de todos los automóviles - {Timestamp}", DateTime.UtcNow);

            try
            {
                var result = await _automovilService.GetAllAsync();
                if (result.Success)
                    return Ok(result);

                return StatusCode(500, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al obtener todos los automóviles");
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "Error interno del servidor",
                    Errors = { ex.Message }
                });
            }
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ApiResponse<AutomovilResponseDto>>> GetById([FromRoute] int id)
        {
            if (id <= 0)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "El ID debe ser un número positivo mayor a cero",
                    Errors = { $"ID inválido: {id}" }
                });
            }

            try
            {
                var result = await _automovilService.GetByIdAsync(id);
                if (result.Success)
                    return Ok(result);

                return NotFound(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado en GetById {Id}", id);
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "Error interno del servidor",
                    Errors = { ex.Message }
                });
            }
        }

        [HttpGet("chasis/{numeroChasis}")]
        public async Task<ActionResult<ApiResponse<AutomovilResponseDto>>> GetByChasis([FromRoute][Required] string numeroChasis)
        {
            if (string.IsNullOrWhiteSpace(numeroChasis))
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "El número de chasis es requerido"
                });

            if (numeroChasis.Length != 17)
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "El número de chasis debe tener 17 caracteres"
                });

            try
            {
                var result = await _automovilService.GetByChasisAsync(numeroChasis.ToUpper().Trim());
                if (result.Success)
                    return Ok(result);

                return NotFound(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en GetByChasis {NumeroChasis}", numeroChasis);
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "Error interno del servidor",
                    Errors = { ex.Message }
                });
            }
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<AutomovilResponseDto>>> Create([FromBody] AutomovilCreateDto createDto)
        {
            if (createDto == null)
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Los datos del automóvil son requeridos"
                });

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Los datos proporcionados no son válidos",
                    Errors = errors
                });
            }

            try
            {
                var result = await _automovilService.CreateAsync(createDto);
                if (result.Success)
                {
                    return CreatedAtAction(nameof(GetById), new { id = result.Data?.Id }, result);
                }
                return BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado en Create");
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "Error interno del servidor",
                    Errors = { ex.Message }
                });
            }
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<ApiResponse<AutomovilResponseDto>>> Update([FromRoute] int id, [FromBody] AutomovilUpdateDto updateDto)
        {
            if (id <= 0)
                return BadRequest(new ApiResponse<object> { Success = false, Message = "El ID debe ser un número positivo" });

            if (updateDto == null)
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Los datos de actualización son requeridos" });

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Los datos no son válidos", Errors = errors });
            }

            try
            {
                var result = await _automovilService.UpdateAsync(id, updateDto);
                if (result.Success)
                    return Ok(result);

                if (result.Message.Contains("No se encontro"))
                    return NotFound(result);

                return BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado en Update {Id}", id);
                return StatusCode(500, new ApiResponse<object> { Success = false, Message = "Error interno del servidor", Errors = { ex.Message } });
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult<ApiResponse<string>>> Delete([FromRoute] int id)
        {
            if (id <= 0)
                return BadRequest(new ApiResponse<object> { Success = false, Message = "El ID debe ser positivo" });

            try
            {
                var result = await _automovilService.DeleteAsync(id);
                if (result.Success)
                    return Ok(result);

                return NotFound(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado en Delete {Id}", id);
                return StatusCode(500, new ApiResponse<object> { Success = false, Message = "Error interno del servidor", Errors = { ex.Message } });
            }
        }

        [HttpGet("health")]
        public ActionResult GetHealth()
        {
            return Ok(new
            {
                Controller = nameof(AutomovilController),
                Status = "Healthy",
                Timestamp = DateTime.UtcNow,
                Version = "1.0.0"
            });
        }
    }
}*/
/*333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333
// Estas son las librerias que se usan en este archivo:

// Microsoft.AspNetCore.Mvc necesaria para trabajar con ASP.NET Core MVC y crear controladores y endpoints.
using Microsoft.AspNetCore.Mvc;

// AutomovilAPI.Application.Services aca esta la logica de negocio (los "servicios" que manipulan automoviles).
using AutomovilAPI.Application.Services;

// AutomovilAPI.Domain.DTOs  aqui estan los DTOs (objetos de transferencia de datos) que se usan
// para enviar/recibir informacion de forma segura sin exponer directamente la entidad "Automovil".
using AutomovilAPI.Domain.DTOs;

// System.ComponentModel.DataAnnotations  contiene atributos de validacion como [Required], [StringLength], etc.
using System.ComponentModel.DataAnnotations;

namespace AutomovilAPI.Presentation.Controllers
{
    /// <summary>
    /// Este controlador es el "puente" entre el mundo exterior (clientes, navegadores, apps, etc.)
    /// y la logica de negocio de la aplicacion (los servicios).
    /// Se encarga de recibir peticiones HTTP (GET, POST, PUT, DELETE),
    /// procesarlas y devolver respuestas en formato JSON.
    /// </summary>
    [ApiController] 
    // Marca esta clase como un controlador de API (sin vistas, solo JSON).
    // Esto activa validaciones automaticas y comportamientos tipicos de APIs REST.

    [Route("api/v1/automovil")] 
    // Define la URL base para este controlador.
    // Ejemplo: https://localhost:5001/api/v1/automovil
    // A partir de aqui se agregan los endpoints: /{id}, /chasis/{num}, etc.

    [Produces("application/json")] 
    // Indica que todas las respuestas se devolveran en formato JSON.

    public class AutomovilController : ControllerBase 
    // ControllerBase = clase base para controladores de API (no incluye vistas).
    {
        // Dependencias: objetos que este controlador necesita para funcionar.
        private readonly IAutomovilService _automovilService; 
        // Servicio con la logica de negocio (ej: crear, buscar, actualizar autos).
        
        private readonly ILogger<AutomovilController> _logger; 
        // Logger para registrar informacion en consola, archivos, etc.

        /// <summary>
        /// Constructor: recibe los servicios que necesita este controlador.
        /// Gracias a la Inyeccion de Dependencias de ASP.NET Core, 
        /// no necesitamos crear los objetos a mano, el framework los provee.
        /// </summary>
        public AutomovilController(
            IAutomovilService automovilService, // Servicio de autos
            ILogger<AutomovilController> logger) // Logger
        {
            _automovilService = automovilService; // Se asignan a variables privadas
            _logger = logger;
        }

        // ------------------------------
        // ENDPOINTS (acciones del controlador)
        // ------------------------------

        /// <summary>
        /// Obtener todos los automoviles.
        /// Metodo HTTP: GET
        /// URL: GET /api/v1/automovil
        /// </summary>
        [HttpGet] 
        // Este metodo responde a las solicitudes GET en la URL base.

        [ProducesResponseType(typeof(ApiResponse<IEnumerable<AutomovilResponseDto>>), 200)] 
        // Documenta que devuelve 200 (exito) con una lista de automoviles.

        [ProducesResponseType(typeof(ApiResponse<object>), 500)] 
        // Documenta que puede devolver 500 (error del servidor).

        public async Task<ActionResult<ApiResponse<IEnumerable<AutomovilResponseDto>>>> GetAll()
        {
            _logger.LogInformation("Iniciando obtencion de todos los automoviles - {Timestamp}", DateTime.UtcNow);

            try
            {
                // Se pide al servicio la lista de autos.
                var result = await _automovilService.GetAllAsync();

                if (result.Success)
                {
                    _logger.LogInformation("Automoviles obtenidos exitosamente - Total: {Count}",
                        result.Data?.Count() ?? 0);

                    return Ok(result); // Devuelve 200 + lista de autos
                }

                _logger.LogWarning("No se pudieron obtener los automoviles: {Message}", result.Message);
                return StatusCode(500, result); // Devuelve 500 si hubo problema en el servicio
            }
            catch (Exception ex)
            {
                // Captura errores inesperados
                _logger.LogError(ex, "Error inesperado al obtener todos los automoviles");

                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "Error interno del servidor",
                    Errors = { ex.Message }
                });
            }
        }

        /// <summary>
        /// Obtener un automovil por su ID.
        /// Metodo: GET
        /// URL: GET /api/v1/automovil/{id}
        /// </summary>
        [HttpGet("{id:int}")] 
        // {id:int} significa que el parametro en la URL debe ser un numero entero.

        public async Task<ActionResult<ApiResponse<AutomovilResponseDto>>> GetById([FromRoute] int id)
        {
            _logger.LogInformation("Buscando automóvil con ID: {Id} - {Timestamp}", id, DateTime.UtcNow);

            if (id <= 0) // Validacion basica
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "El ID debe ser un numero positivo mayor a cero",
                    Errors = { $"ID invalido: {id}" }
                });
            }

            try
            {
                var result = await _automovilService.GetByIdAsync(id);

                if (result.Success)
                    return Ok(result); // Devuelve 200 si encontro el auto

                return NotFound(result); // Devuelve 404 si no existe
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "Error interno del servidor",
                    Errors = { ex.Message }
                });
            }
        }

        /// <summary>
        /// Obtener automovil por numero de chasis.
        /// URL: GET /api/v1/automovil/chasis/{numeroChasis}
        /// </summary>
        [HttpGet("chasis/{numeroChasis}")]
        public async Task<ActionResult<ApiResponse<AutomovilResponseDto>>> GetByChasis(
            [FromRoute] [Required] string numeroChasis)
        {
            if (string.IsNullOrWhiteSpace(numeroChasis))
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "El numero de chasis es requerido"
                });
            }

            if (numeroChasis.Length != 17) // Regla de negocio
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "El numero de chasis debe tener 17 caracteres"
                });
            }

            try
            {
                var result = await _automovilService.GetByChasisAsync(numeroChasis.ToUpper().Trim());

                if (result.Success)
                    return Ok(result);

                return NotFound(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "Error interno del servidor",
                    Errors = { ex.Message }
                });
            }
        }

        /// <summary>
        /// Crear un nuevo automovil.
        /// Metodo: POST
        /// URL: POST /api/v1/automovil
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<ApiResponse<AutomovilResponseDto>>> Create(
            [FromBody] AutomovilCreateDto createDto) // Datos vienen en el "cuerpo" del request
        {
            if (createDto == null) // Validacion inicial
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Los datos del automovil son requeridos"
                });
            }

            if (!ModelState.IsValid) // Valida atributos [Required], [StringLength], etc.
            {
                var errors = ModelState
                    .SelectMany(x => x.Value?.Errors ?? new())
                    .Select(x => x.ErrorMessage)
                    .ToList();

                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Los datos proporcionados no son validos",
                    Errors = errors
                });
            }

            try
            {
                var result = await _automovilService.CreateAsync(createDto);

                if (result.Success)
                {
                    // CreatedAtAction devuelve 201 Created y la URL del nuevo recurso
                    return CreatedAtAction(
                        nameof(GetById), // apunta al metodo que recupera por ID
                        new { id = result.Data?.Id },
                        result);
                }

                return BadRequest(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "Error interno del servidor",
                    Errors = { ex.Message }
                });
            }
        }

        /// <summary>
        /// Actualizar automovil por ID.
        /// Metodo: PUT
        /// URL: PUT /api/v1/automovil/{id}
        /// </summary>
        [HttpPut("{id:int}")]
        public async Task<ActionResult<ApiResponse<AutomovilResponseDto>>> Update(
            [FromRoute] int id,
            [FromBody] AutomovilUpdateDto updateDto)
        {
            if (id <= 0)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "El ID debe ser un numero positivo"
                });
            }

            if (updateDto == null)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Los datos de actualizacion son requeridos"
                });
            }

            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .SelectMany(x => x.Value?.Errors ?? new())
                    .Select(x => x.ErrorMessage)
                    .ToList();

                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Los datos no son validos",
                    Errors = errors
                });
            }

            try
            {
                var result = await _automovilService.UpdateAsync(id, updateDto);

                if (result.Success)
                    return Ok(result);

                if (result.Message.Contains("No se encontro"))
                    return NotFound(result);

                return BadRequest(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "Error interno del servidor",
                    Errors = { ex.Message }
                });
            }
        }

        /// <summary>
        /// Eliminar un automovil.
        /// Metodo: DELETE
        /// URL: DELETE /api/v1/automovil/{id}
        /// </summary>
        [HttpDelete("{id:int}")]
        public async Task<ActionResult<ApiResponse<string>>> Delete([FromRoute] int id)
        {
            if (id <= 0)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "El ID debe ser positivo"
                });
            }

            try
            {
                var result = await _automovilService.DeleteAsync(id);

                if (result.Success)
                    return Ok(result);

                return NotFound(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "Error interno del servidor",
                    Errors = { ex.Message }
                });
            }
        }

        /// <summary>
        /// Endpoint de prueba para verificar que el controlador funciona.
        /// URL: GET /api/v1/automovil/health
        /// </summary>
        [HttpGet("health")]
        public ActionResult GetHealth()
        {
            // Devuelve informacion basica del estado del controlador
            return Ok(new
            {
                Controller = "AutomovilController",
                Status = "Healthy",
                Timestamp = DateTime.UtcNow,
                Version = "1.0.0",
                AvailableEndpoints = new[]
                {
                    "GET /api/v1/automovil",
                    "GET /api/v1/automovil/{id}",
                    "GET /api/v1/automovil/chasis/{numeroChasis}",
                    "POST /api/v1/automovil",
                    "PUT /api/v1/automovil/{id}",
                    "DELETE /api/v1/automovil/{id}",
                    "GET /api/v1/automovil/health"
                }
            });
        }
    }
}
*/
/*2222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222// Estas son las librerías que se usan en este archivo
// - Microsoft.AspNetCore.Mvc: para crear controladores y endpoints de una API
// - AutomovilAPI.Application.Services: aquí está la lógica de negocio (servicios que manipulan automóviles)
// - AutomovilAPI.Domain.DTOs: aquí están los DTOs (objetos de transferencia de datos)
// - System.ComponentModel.DataAnnotations: para validaciones como [Required], [StringLength], etc.
using Microsoft.AspNetCore.Mvc;
using AutomovilAPI.Application.Services;
using AutomovilAPI.Domain.DTOs;
using System.ComponentModel.DataAnnotations;

namespace AutomovilAPI.Presentation.Controllers
{
    /// <summary>
    /// Controlador = clase que recibe las peticiones HTTP (como GET, POST, PUT, DELETE)
    /// y se comunica con los servicios para devolver respuestas.
    /// </summary>
    [ApiController] // Indica que esta clase es un controlador de API (no devuelve vistas, sino JSON).
    [Route("api/v1/automovil")] // Define la URL base para acceder: ej. https://localhost:5001/api/v1/automovil
    [Produces("application/json")] // Todas las respuestas serán en formato JSON.
    public class AutomovilController : ControllerBase // ControllerBase = controlador sin vistas (solo API).
    {
        // Dependencias que este controlador necesita:
        private readonly IAutomovilService _automovilService; // Servicio con la lógica de negocio para automóviles.
        private readonly ILogger<AutomovilController> _logger; // Logger para escribir mensajes en la consola o archivos.

        /// <summary>
        /// Constructor: recibe los servicios que necesita este controlador.
        /// ASP.NET Core se encarga de "inyectarlos" automáticamente (Inyección de Dependencias).
        /// </summary>
        public AutomovilController(
            IAutomovilService automovilService, // Servicio de autos
            ILogger<AutomovilController> logger) // Logger
        {
            _automovilService = automovilService; // Se guarda en variables privadas para usarlos en los métodos
            _logger = logger;
        }

        /// <summary>
        /// Endpoint para obtener todos los automóviles registrados.
        /// Método HTTP: GET
        /// URL: GET /api/v1/automovil
        /// </summary>
        [HttpGet] // Indica que responde a solicitudes GET
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<AutomovilResponseDto>>), 200)] // Respuesta exitosa
        [ProducesResponseType(typeof(ApiResponse<object>), 500)] // Respuesta en caso de error
        public async Task<ActionResult<ApiResponse<IEnumerable<AutomovilResponseDto>>>> GetAll()
        {
            _logger.LogInformation("Iniciando obtención de todos los automóviles - {Timestamp}", DateTime.UtcNow);

            try
            {
                // Llama al servicio para obtener los autos
                var result = await _automovilService.GetAllAsync();

                if (result.Success)
                {
                    _logger.LogInformation("Automóviles obtenidos exitosamente - Total: {Count}",
                        result.Data?.Count() ?? 0);
                    return Ok(result); // Devuelve 200 con la lista
                }

                _logger.LogWarning("No se pudieron obtener los automóviles: {Message}", result.Message);
                return StatusCode(500, result); // Devuelve 500 si hubo problema
            }
            catch (Exception ex)
            {
                // Si algo falla inesperadamente, lo captura aquí
                _logger.LogError(ex, "Error inesperado al obtener todos los automóviles");
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "Error interno del servidor",
                    Errors = { ex.Message }
                });
            }
        }

        /// <summary>
        /// Obtener un automóvil por su ID único.
        /// URL: GET /api/v1/automovil/{id}
        /// </summary>
        [HttpGet("{id:int}")] // "{id:int}" significa que la URL espera un número entero.
        public async Task<ActionResult<ApiResponse<AutomovilResponseDto>>> GetById([FromRoute] int id)
        {
            _logger.LogInformation("Iniciando búsqueda de automóvil por ID: {Id} - {Timestamp}", id, DateTime.UtcNow);

            if (id <= 0) // Validación básica: el ID debe ser mayor que 0
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "El ID debe ser un número positivo mayor a cero",
                    Errors = { $"ID inválido: {id}" }
                });
            }

            try
            {
                var result = await _automovilService.GetByIdAsync(id);

                if (result.Success)
                    return Ok(result); // Devuelve el auto si lo encontró

                return NotFound(result); // Devuelve 404 si no existe
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "Error interno del servidor",
                    Errors = { ex.Message }
                });
            }
        }

        /// <summary>
        /// Obtener un automóvil por su número de chasis.
        /// URL: GET /api/v1/automovil/chasis/{numeroChasis}
        /// </summary>
        [HttpGet("chasis/{numeroChasis}")]
        public async Task<ActionResult<ApiResponse<AutomovilResponseDto>>> GetByChasis(
            [FromRoute] [Required] string numeroChasis)
        {
            if (string.IsNullOrWhiteSpace(numeroChasis))
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "El número de chasis es requerido y no puede estar vacío"
                });
            }

            // Validación de longitud (un chasis estándar tiene 17 caracteres)
            if (numeroChasis.Length != 17)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "El número de chasis debe tener exactamente 17 caracteres"
                });
            }

            try
            {
                var result = await _automovilService.GetByChasisAsync(numeroChasis.ToUpper().Trim());

                if (result.Success)
                    return Ok(result);

                return NotFound(result); // 404 si no existe
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "Error interno del servidor",
                    Errors = { ex.Message }
                });
            }
        }

        /// <summary>
        /// Crear un nuevo automóvil.
        /// URL: POST /api/v1/automovil
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<ApiResponse<AutomovilResponseDto>>> Create(
            [FromBody] AutomovilCreateDto createDto) // El auto viene en el body de la petición
        {
            if (createDto == null)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Los datos del automóvil son requeridos"
                });
            }

            if (!ModelState.IsValid) // Verifica validaciones de atributos [Required], etc.
            {
                var errors = ModelState
                    .SelectMany(x => x.Value?.Errors ?? new())
                    .Select(x => x.ErrorMessage)
                    .ToList();

                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Los datos proporcionados no son válidos",
                    Errors = errors
                });
            }

            try
            {
                var result = await _automovilService.CreateAsync(createDto);

                if (result.Success)
                {
                    // CreatedAtAction devuelve 201 y además la URL para consultar el nuevo recurso
                    return CreatedAtAction(
                        nameof(GetById), // Indica que se puede obtener por ID
                        new { id = result.Data?.Id },
                        result);
                }

                return BadRequest(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "Error interno del servidor",
                    Errors = { ex.Message }
                });
            }
        }

        /// <summary>
        /// Actualizar un automóvil por ID.
        /// URL: PUT /api/v1/automovil/{id}
        /// </summary>
        [HttpPut("{id:int}")]
        public async Task<ActionResult<ApiResponse<AutomovilResponseDto>>> Update(
            [FromRoute] int id,
            [FromBody] AutomovilUpdateDto updateDto)
        {
            if (id <= 0)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "El ID debe ser un número positivo mayor a cero"
                });
            }

            if (updateDto == null)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Los datos de actualización son requeridos"
                });
            }

            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .SelectMany(x => x.Value?.Errors ?? new())
                    .Select(x => x.ErrorMessage)
                    .ToList();

                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Los datos proporcionados no son válidos",
                    Errors = errors
                });
            }

            try
            {
                var result = await _automovilService.UpdateAsync(id, updateDto);

                if (result.Success)
                    return Ok(result);

                if (result.Message.Contains("No se encontró"))
                    return NotFound(result);

                return BadRequest(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "Error interno del servidor",
                    Errors = { ex.Message }
                });
            }
        }

        /// <summary>
        /// Eliminar un automóvil por ID.
        /// URL: DELETE /api/v1/automovil/{id}
        /// </summary>
        [HttpDelete("{id:int}")]
        public async Task<ActionResult<ApiResponse<string>>> Delete([FromRoute] int id)
        {
            if (id <= 0)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "El ID debe ser un número positivo mayor a cero"
                });
            }

            try
            {
                var result = await _automovilService.DeleteAsync(id);

                if (result.Success)
                    return Ok(result);

                return NotFound(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "Error interno del servidor",
                    Errors = { ex.Message }
                });
            }
        }

        /// <summary>
        /// Endpoint de prueba para saber si este controlador está funcionando.
        /// URL: GET /api/v1/automovil/health
        /// </summary>
        [HttpGet("health")]
        public ActionResult GetHealth()
        {
            return Ok(new
            {
                Controller = "AutomovilController",
                Status = "Healthy",
                Timestamp = DateTime.UtcNow,
                Version = "1.0.0",
                AvailableEndpoints = new[]
                {
                    "GET /api/v1/automovil",
                    "GET /api/v1/automovil/{id}",
                    "GET /api/v1/automovil/chasis/{numeroChasis}",
                    "POST /api/v1/automovil",
                    "PUT /api/v1/automovil/{id}",
                    "DELETE /api/v1/automovil/{id}",
                    "GET /api/v1/automovil/health"
                }
            });
        }
    }
}*/

/*111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////*/
/*using Microsoft.AspNetCore.Mvc;
using AutomovilAPI.Application.Services;
using AutomovilAPI.Domain.DTOs;
using System.ComponentModel.DataAnnotations;

namespace AutomovilAPI.Presentation.Controllers
{
    /// <summary>
    /// Controlador para la gestión de automóviles
    /// </summary>
    [ApiController]
    [Route("api/v1/automovil")]
    [Produces("application/json")]
    public class AutomovilController : ControllerBase
    {
        private readonly IAutomovilService _automovilService;
        private readonly ILogger<AutomovilController> _logger;

        /// <summary>
        /// Constructor del controlador de automóviles
        /// </summary>
        /// <param name="automovilService">Servicio de automóviles</param>
        /// <param name="logger">Logger para registro de eventos</param>
        public AutomovilController(
            IAutomovilService automovilService,
            ILogger<AutomovilController> logger)
        {
            _automovilService = automovilService;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los automóviles registrados
        /// </summary>
        /// <returns>Lista de automóviles</returns>
        /// <response code="200">Lista de automóviles obtenida exitosamente</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<AutomovilResponseDto>>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<ActionResult<ApiResponse<IEnumerable<AutomovilResponseDto>>>> GetAll()
        {
            _logger.LogInformation("Iniciando obtención de todos los automóviles - {Timestamp}", DateTime.UtcNow);
            
            try
            {
                var result = await _automovilService.GetAllAsync();
                
                if (result.Success)
                {
                    _logger.LogInformation("Automóviles obtenidos exitosamente - Total: {Count}", 
                        result.Data?.Count() ?? 0);
                    return Ok(result);
                }
                
                _logger.LogWarning("No se pudieron obtener los automóviles: {Message}", result.Message);
                return StatusCode(500, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al obtener todos los automóviles");
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "Error interno del servidor",
                    Errors = { ex.Message }
                });
            }
        }

        /// <summary>
        /// Obtiene un automóvil por su ID único
        /// </summary>
        /// <param name="id">ID del automóvil</param>
        /// <returns>Automóvil encontrado</returns>
        /// <response code="200">Automóvil encontrado exitosamente</response>
        /// <response code="400">ID inválido proporcionado</response>
        /// <response code="404">Automóvil no encontrado</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(ApiResponse<AutomovilResponseDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<ActionResult<ApiResponse<AutomovilResponseDto>>> GetById(
            [FromRoute] int id)
        {
            _logger.LogInformation("Iniciando búsqueda de automóvil por ID: {Id} - {Timestamp}", id, DateTime.UtcNow);

            if (id <= 0)
            {
                _logger.LogWarning("ID inválido proporcionado: {Id}", id);
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "El ID debe ser un número positivo mayor a cero",
                    Errors = { $"ID inválido: {id}" }
                });
            }

            try
            {
                var result = await _automovilService.GetByIdAsync(id);
                
                if (result.Success)
                {
                    _logger.LogInformation("Automóvil encontrado exitosamente - ID: {Id}, Marca: {Marca}, Modelo: {Modelo}", 
                        id, result.Data?.Marca, result.Data?.Modelo);
                    return Ok(result);
                }
                
                _logger.LogInformation("Automóvil no encontrado - ID: {Id}", id);
                return NotFound(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al buscar automóvil por ID: {Id}", id);
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "Error interno del servidor",
                    Errors = { ex.Message }
                });
            }
        }

        /// <summary>
        /// Obtiene un automóvil por su número de chasis único
        /// </summary>
        /// <param name="numeroChasis">Número de chasis del automóvil</param>
        /// <returns>Automóvil encontrado</returns>
        /// <response code="200">Automóvil encontrado exitosamente</response>
        /// <response code="400">Número de chasis inválido</response>
        /// <response code="404">Automóvil no encontrado</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpGet("chasis/{numeroChasis}")]
        [ProducesResponseType(typeof(ApiResponse<AutomovilResponseDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<ActionResult<ApiResponse<AutomovilResponseDto>>> GetByChasis(
            [FromRoute] [Required] string numeroChasis)
        {
            _logger.LogInformation("Iniciando búsqueda de automóvil por chasis: {NumeroChasis} - {Timestamp}", 
                numeroChasis, DateTime.UtcNow);

            if (string.IsNullOrWhiteSpace(numeroChasis))
            {
                _logger.LogWarning("Número de chasis vacío o nulo proporcionado");
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "El número de chasis es requerido y no puede estar vacío",
                    Errors = { "Número de chasis inválido" }
                });
            }

            // Validar formato básico del número de chasis (VIN estándar)
            if (numeroChasis.Length != 17)
            {
                _logger.LogWarning("Número de chasis con formato inválido: {NumeroChasis} (Longitud: {Length})", 
                    numeroChasis, numeroChasis.Length);
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "El número de chasis debe tener exactamente 17 caracteres",
                    Errors = { $"Longitud actual: {numeroChasis.Length}, requerida: 17" }
                });
            }

            try
            {
                var result = await _automovilService.GetByChasisAsync(numeroChasis.ToUpper().Trim());
                
                if (result.Success)
                {
                    _logger.LogInformation("Automóvil encontrado por chasis - ID: {Id}, Marca: {Marca}, Modelo: {Modelo}", 
                        result.Data?.Id, result.Data?.Marca, result.Data?.Modelo);
                    return Ok(result);
                }
                
                _logger.LogInformation("Automóvil no encontrado con chasis: {NumeroChasis}", numeroChasis);
                return NotFound(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al buscar automóvil por chasis: {NumeroChasis}", numeroChasis);
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "Error interno del servidor",
                    Errors = { ex.Message }
                });
            }
        }

        /// <summary>
        /// Crea un nuevo automóvil en el sistema
        /// </summary>
        /// <param name="createDto">Datos del automóvil a crear</param>
        /// <returns>Automóvil creado con su ID asignado</returns>
        /// <response code="201">Automóvil creado exitosamente</response>
        /// <response code="400">Datos de entrada inválidos o conflicto de unicidad</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<AutomovilResponseDto>), 201)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<ActionResult<ApiResponse<AutomovilResponseDto>>> Create(
            [FromBody] AutomovilCreateDto createDto)
        {
            _logger.LogInformation("Iniciando creación de automóvil - Marca: {Marca}, Modelo: {Modelo} - {Timestamp}", 
                createDto?.Marca, createDto?.Modelo, DateTime.UtcNow);

            if (createDto == null)
            {
                _logger.LogWarning("Datos de automóvil nulos recibidos en la solicitud");
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Los datos del automóvil son requeridos",
                    Errors = { "Body de la solicitud no puede estar vacío" }
                });
            }

            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .SelectMany(x => x.Value?.Errors ?? new())
                    .Select(x => x.ErrorMessage)
                    .ToList();

                _logger.LogWarning("Datos de entrada inválidos para creación de automóvil: {Errors}", 
                    string.Join(", ", errors));

                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Los datos proporcionados no son válidos",
                    Errors = errors
                });
            }

            try
            {
                var result = await _automovilService.CreateAsync(createDto);
                
                if (result.Success)
                {
                    _logger.LogInformation("Automóvil creado exitosamente - ID: {Id}, Marca: {Marca}, Modelo: {Modelo}", 
                        result.Data?.Id, result.Data?.Marca, result.Data?.Modelo);
                    
                    return CreatedAtAction(
                        nameof(GetById),
                        new { id = result.Data?.Id },
                        result);
                }
                
                _logger.LogWarning("No se pudo crear el automóvil: {Message}", result.Message);
                return BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al crear automóvil");
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "Error interno del servidor",
                    Errors = { ex.Message }
                });
            }
        }

        /// <summary>
        /// Actualiza parcialmente un automóvil existente
        /// </summary>
        /// <param name="id">ID del automóvil a actualizar</param>
        /// <param name="updateDto">Datos a actualizar (solo campos proporcionados)</param>
        /// <returns>Automóvil actualizado</returns>
        /// <response code="200">Automóvil actualizado exitosamente</response>
        /// <response code="400">ID inválido o datos de entrada inválidos</response>
        /// <response code="404">Automóvil no encontrado</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(ApiResponse<AutomovilResponseDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<ActionResult<ApiResponse<AutomovilResponseDto>>> Update(
            [FromRoute] int id,
            [FromBody] AutomovilUpdateDto updateDto)
        {
            _logger.LogInformation("Iniciando actualización de automóvil - ID: {Id} - {Timestamp}", 
                id, DateTime.UtcNow);

            if (id <= 0)
            {
                _logger.LogWarning("ID inválido proporcionado para actualización: {Id}", id);
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "El ID debe ser un número positivo mayor a cero",
                    Errors = { $"ID inválido: {id}" }
                });
            }

            if (updateDto == null)
            {
                _logger.LogWarning("Datos de actualización nulos recibidos para ID: {Id}", id);
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Los datos de actualización son requeridos",
                    Errors = { "Body de la solicitud no puede estar vacío" }
                });
            }

            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .SelectMany(x => x.Value?.Errors ?? new())
                    .Select(x => x.ErrorMessage)
                    .ToList();

                _logger.LogWarning("Datos de entrada inválidos para actualización de automóvil ID {Id}: {Errors}", 
                    id, string.Join(", ", errors));

                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Los datos proporcionados no son válidos",
                    Errors = errors
                });
            }

            try
            {
                var result = await _automovilService.UpdateAsync(id, updateDto);
                
                if (result.Success)
                {
                    _logger.LogInformation("Automóvil actualizado exitosamente - ID: {Id}, Marca: {Marca}, Modelo: {Modelo}", 
                        id, result.Data?.Marca, result.Data?.Modelo);
                    return Ok(result);
                }
                
                if (result.Message.Contains("No se encontró"))
                {
                    _logger.LogInformation("Automóvil no encontrado para actualización - ID: {Id}", id);
                    return NotFound(result);
                }
                
                _logger.LogWarning("No se pudo actualizar el automóvil ID {Id}: {Message}", id, result.Message);
                return BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al actualizar automóvil ID: {Id}", id);
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "Error interno del servidor",
                    Errors = { ex.Message }
                });
            }
        }

        /// <summary>
        /// Elimina un automóvil del sistema por su ID
        /// </summary>
        /// <param name="id">ID del automóvil a eliminar</param>
        /// <returns>Confirmación de eliminación</returns>
        /// <response code="200">Automóvil eliminado exitosamente</response>
        /// <response code="400">ID inválido proporcionado</response>
        /// <response code="404">Automóvil no encontrado</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(typeof(ApiResponse<string>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<ActionResult<ApiResponse<string>>> Delete(
            [FromRoute] int id)
        {
            _logger.LogInformation("Iniciando eliminación de automóvil - ID: {Id} - {Timestamp}", 
                id, DateTime.UtcNow);

            if (id <= 0)
            {
                _logger.LogWarning("ID inválido proporcionado para eliminación: {Id}", id);
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "El ID debe ser un número positivo mayor a cero",
                    Errors = { $"ID inválido: {id}" }
                });
            }

            try
            {
                var result = await _automovilService.DeleteAsync(id);
                
                if (result.Success)
                {
                    _logger.LogInformation("Automóvil eliminado exitosamente - ID: {Id}", id);
                    return Ok(result);
                }
                
                _logger.LogInformation("Automóvil no encontrado para eliminación - ID: {Id}", id);
                return NotFound(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al eliminar automóvil ID: {Id}", id);
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "Error interno del servidor",
                    Errors = { ex.Message }
                });
            }
        }

        /// <summary>
        /// Endpoint de salud específico para el controlador de automóviles
        /// </summary>
        /// <returns>Estado del controlador</returns>
        [HttpGet("health")]
        [ProducesResponseType(typeof(object), 200)]
        public ActionResult GetHealth()
        {
            return Ok(new
            {
                Controller = "AutomovilController",
                Status = "Healthy",
                Timestamp = DateTime.UtcNow,
                Version = "1.0.0",
                AvailableEndpoints = new[]
                {
                    "GET /api/v1/automovil - Obtener todos los automóviles",
                    "GET /api/v1/automovil/{id} - Obtener automóvil por ID",
                    "GET /api/v1/automovil/chasis/{numeroChasis} - Obtener automóvil por chasis",
                    "POST /api/v1/automovil - Crear nuevo automóvil",
                    "PUT /api/v1/automovil/{id} - Actualizar automóvil",
                    "DELETE /api/v1/automovil/{id} - Eliminar automóvil",
                    "GET /api/v1/automovil/health - Estado del controlador"
                }
            });
        }
    }
}*/