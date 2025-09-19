using System.ComponentModel.DataAnnotations;

namespace HybridDDDArchitecture.Core.Application.DataTransferObjects
{
    /// <summary>
    /// DTO para crear un nuevo automóvil (POST).
    /// </summary>
    public class AutomovilCreateDto
    {
        [Required(ErrorMessage = "La marca es obligatoria")]
        [StringLength(50, ErrorMessage = "La marca no puede exceder los 50 caracteres")]
        public string Marca { get; set; } = string.Empty;

        [Required(ErrorMessage = "El modelo es obligatorio")]
        [StringLength(50, ErrorMessage = "El modelo no puede exceder los 50 caracteres")]
        public string Modelo { get; set; } = string.Empty;

        [Required(ErrorMessage = "El color es obligatorio")]
        [StringLength(30, ErrorMessage = "El color no puede exceder los 30 caracteres")]
        public string Color { get; set; } = string.Empty;

        [Required(ErrorMessage = "El año de fabricación es obligatorio")]
        [Range(1900, 2030, ErrorMessage = "El año de fabricación debe estar entre 1900 y 2030")]
        public int Fabricacion { get; set; }

        [Required(ErrorMessage = "El número de motor es obligatorio")]
        [StringLength(20, ErrorMessage = "El número de motor no puede exceder los 20 caracteres")]
        public string NumeroMotor { get; set; } = string.Empty;

        [Required(ErrorMessage = "El número de chasis es obligatorio")]
        [StringLength(17, ErrorMessage = "El número de chasis no puede exceder los 17 caracteres")]
        public string NumeroChasis { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO para actualizar un automóvil (PUT).
    /// Solo se envían los campos que se desean cambiar.
    /// </summary>
    public class AutomovilUpdateDto
    {
        [StringLength(30, ErrorMessage = "El color no puede exceder los 30 caracteres")]
        public string? Color { get; set; }

        [StringLength(20, ErrorMessage = "El número de motor no puede exceder los 20 caracteres")]
        public string? NumeroMotor { get; set; }

        [Range(1900, 2030, ErrorMessage = "El año de fabricación debe estar entre 1900 y 2030")]
        public int? Fabricacion { get; set; }
    }

    /// <summary>
    /// DTO de respuesta para devolver la información completa del automóvil.
    /// </summary>
    public class AutomovilResponseDto
    {
        public int Id { get; set; }
        public string Marca { get; set; } = string.Empty;
        public string Modelo { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public int Fabricacion { get; set; }
        public string NumeroMotor { get; set; } = string.Empty;
        public string NumeroChasis { get; set; } = string.Empty;
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaActualizacion { get; set; }
    }
}
/*using System.ComponentModel.DataAnnotations;

namespace Application.DataTransferObjects
{
    /// <summary>
    /// DTO para crear un nuevo automóvil (POST).
    /// </summary>
    public class AutomovilCreateDto
    {
        [Required(ErrorMessage = "La marca es obligatoria")]
        [StringLength(50, ErrorMessage = "La marca no puede exceder los 50 caracteres")]
        public string Marca { get; set; } = string.Empty;

        [Required(ErrorMessage = "El modelo es obligatorio")]
        [StringLength(50, ErrorMessage = "El modelo no puede exceder los 50 caracteres")]
        public string Modelo { get; set; } = string.Empty;

        [Required(ErrorMessage = "El color es obligatorio")]
        [StringLength(30, ErrorMessage = "El color no puede exceder los 30 caracteres")]
        public string Color { get; set; } = string.Empty;

        [Required(ErrorMessage = "El año de fabricación es obligatorio")]
        [Range(1900, 2030, ErrorMessage = "El año de fabricación debe estar entre 1900 y 2030")]
        public int Fabricacion { get; set; }

        [Required(ErrorMessage = "El número de motor es obligatorio")]
        [StringLength(20, ErrorMessage = "El número de motor no puede exceder los 20 caracteres")]
        public string NumeroMotor { get; set; } = string.Empty;

        [Required(ErrorMessage = "El número de chasis es obligatorio")]
        [StringLength(17, ErrorMessage = "El número de chasis no puede exceder los 17 caracteres")]
        public string NumeroChasis { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO para actualizar un automóvil (PUT).
    /// Solo se envían los campos que se desean cambiar.
    /// </summary>
    public class AutomovilUpdateDto
    {
        [StringLength(30, ErrorMessage = "El color no puede exceder los 30 caracteres")]
        public string? Color { get; set; }

        [StringLength(20, ErrorMessage = "El número de motor no puede exceder los 20 caracteres")]
        public string? NumeroMotor { get; set; }

        [Range(1900, 2030, ErrorMessage = "El año de fabricación debe estar entre 1900 y 2030")]
        public int? Fabricacion { get; set; }
    }

    /// <summary>
    /// DTO de respuesta para devolver la información completa del automóvil.
    /// </summary>
    public class AutomovilResponseDto
    {
        public int Id { get; set; }
        public string Marca { get; set; } = string.Empty;
        public string Modelo { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public int Fabricacion { get; set; }
        public string NumeroMotor { get; set; } = string.Empty;
        public string NumeroChasis { get; set; } = string.Empty;

        public DateTime FechaCreacion { get; set; }
        public DateTime FechaActualizacion { get; set; }
    }

    /// <summary>
    /// Envoltorio genérico para las respuestas de la API.
    /// </summary>
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
        public List<string> Errors { get; set; } = new();
    }
}*/
/*// Esta libreria se usa para aplicar validaciones sobre las propiedades.
// Por ejemplo: [Required], [StringLength], [Range], etc.
using System.ComponentModel.DataAnnotations;

//namespace AutomovilAPI.Domain.DTOs
namespace AutomovilAPI.Application.DTOs
{
    
    // DTO para CREAR un automovil (cuando llega un POST desde la API)
    
    public class AutomovilDto
    {
        // Marca es obligatoria y como maximo puede tener 50 caracteres
        [Required(ErrorMessage = "La marca es obligatoria")]
        [StringLength(50, ErrorMessage = "La marca no puede exceder los 50 caracteres")]
        public string Marca { get; set; } = string.Empty;

        // Modelo es obligatorio y como maximo puede tener 50 caracteres
        [Required(ErrorMessage = "El modelo es obligatorio")]
        [StringLength(50, ErrorMessage = "El modelo no puede exceder los 50 caracteres")]
        public string Modelo { get; set; } = string.Empty;

        // Color es obligatorio y como maximo puede tener 30 caracteres
        [Required(ErrorMessage = "El color es obligatorio")]
        [StringLength(30, ErrorMessage = "El color no puede exceder los 30 caracteres")]
        public string Color { get; set; } = string.Empty;

        // El año de fabricacion es obligatorio, y debe estar entre 1900 y 2030
        [Required(ErrorMessage = "El año de fabricacion es obligatorio")]
        [Range(1900, 2030, ErrorMessage = "El año de fabricacion debe estar entre 1900 y 2030")]
        public int Fabricacion { get; set; }

        // Numero de motor es obligatorio y no puede superar los 20 caracteres
        [Required(ErrorMessage = "El numero de motor es obligatorio")]
        [StringLength(20, ErrorMessage = "El numero de motor no puede exceder los 20 caracteres")]
        public string NumeroMotor { get; set; } = string.Empty;

        // Numero de chasis es obligatorio y no puede superar los 17 caracteres
        [Required(ErrorMessage = "El numero de chasis es obligatorio")]
        [StringLength(17, ErrorMessage = "El numero de chasis no puede exceder los 17 caracteres")]
        public string NumeroChasis { get; set; } = string.Empty;
    }

    
    // DTO para ACTUALIZAR un automovil (cuando llega un PUT desde la API)
    
    public class AutomovilUpdateDto
    {
        // En este caso las propiedades NO son obligatorias,
        // porque quizas queremos actualizar solo un campo especifico.
        
        // Si se envia, el color no puede tener mas de 30 caracteres
        [StringLength(30, ErrorMessage = "El color no puede exceder los 30 caracteres")]
        public string? Color { get; set; }

        // Si se envia, el numero de motor no puede tener mas de 20 caracteres
        [StringLength(20, ErrorMessage = "El numero de motor no puede exceder los 20 caracteres")]
        public string? NumeroMotor { get; set; }

        // Si se envia, el año debe estar entre 1900 y 2030
        [Range(1900, 2030, ErrorMessage = "El año de fabricacion debe estar entre 1900 y 2030")]
        public int? Fabricacion { get; set; }
    }

    
    // DTO para RESPONDER al cliente con datos de un automovil
    
    public class AutomovilResponseDto : AutomovilDTO
    {
        // El cliente puede ver el ID unico del automovil
        public int Id { get; set; }

        // Informacion completa del auto
        public string Marca { get; set; } = string.Empty;
        public string Modelo { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public int Fabricacion { get; set; }
        public string NumeroMotor { get; set; } = string.Empty;
        public string NumeroChasis { get; set; } = string.Empty;

        // Fechas utiles para saber cuando se creo y cuando se actualizo el auto
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaActualizacion { get; set; }
    }

    
    // Clase generica para ENVOLVER las respuestas de la API
    
    public class ApiResponse<T>
    {
        // Indica si la operacion fue exitosa o no
        public bool Success { get; set; }

        // Mensaje para el cliente (ejemplo: "Automovil creado con exito")
        public string Message { get; set; } = string.Empty;

        // Datos que queremos devolver (ej: un automovil, una lista, o nada)
        public T? Data { get; set; }

        // Lista de errores en caso de que algo haya salido mal
        public List<string> Errors { get; set; } = new();
    }
}
*/
/*using System.ComponentModel.DataAnnotations;

namespace AutomovilAPI.Domain.DTOs
{
    public class AutomovilCreateDto
    {
        [Required(ErrorMessage = "La marca es obligatoria")]
        [StringLength(50, ErrorMessage = "La marca no puede exceder los 50 caracteres")]
        public string Marca { get; set; } = string.Empty;

        [Required(ErrorMessage = "El modelo es obligatorio")]
        [StringLength(50, ErrorMessage = "El modelo no puede exceder los 50 caracteres")]
        public string Modelo { get; set; } = string.Empty;

        [Required(ErrorMessage = "El color es obligatorio")]
        [StringLength(30, ErrorMessage = "El color no puede exceder los 30 caracteres")]
        public string Color { get; set; } = string.Empty;

        [Required(ErrorMessage = "El año de fabricacion es obligatorio")]
        [Range(1900, 2030, ErrorMessage = "El año de fabricacion debe estar entre 1900 y 2030")]
        public int Fabricacion { get; set; }

        [Required(ErrorMessage = "El numero de motor es obligatorio")]
        [StringLength(20, ErrorMessage = "El numero de motor no puede exceder los 20 caracteres")]
        public string NumeroMotor { get; set; } = string.Empty;

        [Required(ErrorMessage = "El numero de chasis es obligatorio")]
        [StringLength(17, ErrorMessage = "El numero de chasis no puede exceder los 17 caracteres")]
        public string NumeroChasis { get; set; } = string.Empty;
    }

    public class AutomovilUpdateDto
    {
        [StringLength(30, ErrorMessage = "El color no puede exceder los 30 caracteres")]
        public string? Color { get; set; }

        [StringLength(20, ErrorMessage = "El numero de motor no puede exceder los 20 caracteres")]
        public string? NumeroMotor { get; set; }

        [Range(1900, 2030, ErrorMessage = "El año de fabricacion debe estar entre 1900 y 2030")]
        public int? Fabricacion { get; set; }
    }

    public class AutomovilResponseDto
    {
        public int Id { get; set; }
        public string Marca { get; set; } = string.Empty;
        public string Modelo { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public int Fabricacion { get; set; }
        public string NumeroMotor { get; set; } = string.Empty;
        public string NumeroChasis { get; set; } = string.Empty;
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaActualizacion { get; set; }
    }

    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
        public List<string> Errors { get; set; } = new();
    }
}*/