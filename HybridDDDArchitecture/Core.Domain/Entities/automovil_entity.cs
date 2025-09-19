using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HybridDDDArchitecture.Core.Domain.Entities
{
    /// <summary>
    /// Entidad Automovil que se mapea a la tabla Automoviles en la base de datos.
    /// </summary>
    public class Automovil
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Marca { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Modelo { get; set; } = string.Empty;

        [Required]
        [StringLength(30)]
        public string Color { get; set; } = string.Empty;

        [Required]
        [Range(1900, 2030)]
        public int Fabricacion { get; set; }

        [Required]
        [StringLength(20)]
        public string NumeroMotor { get; set; } = string.Empty;

        [Required]
        [StringLength(17)]
        public string NumeroChasis { get; set; } = string.Empty;

        /// <summary>
        /// Fecha de creación del registro (UTC).
        /// Se setea automáticamente al agregar un automóvil.
        /// </summary>
        public DateTime FechaCreacion { get; set; }

        /// <summary>
        /// Fecha de última actualización (UTC).
        /// Se actualiza automáticamente al modificar un automóvil.
        /// </summary>
        public DateTime FechaActualizacion { get; set; }
    }
}
/*// Importamos atributos de validacion y metadatos para propiedades.
// - DataAnnotations: [Required], [StringLength], [Range], etc.
// - Schema: [Key], [DatabaseGenerated], DatabaseGeneratedOption
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutomovilAPI.Domain.Entities
{
    // Esta clase representa la entidad "Automovil".
    // En EF Core, una entidad se mapea a una tabla en la base de datos.
    // Cada propiedad de la clase se convierte en una columna.
    public class Automovil
    {
        // [Key] indica que esta propiedad es la clave primaria (PK) de la tabla.
        // [DatabaseGenerated(DatabaseGeneratedOption.Identity)] significa que la BD
        // genera el valor (por ejemplo, un auto-increment) al insertar el registro.
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        // [Required] => no puede ser NULL.
        // [StringLength(50)] => longitud maxima: 50 caracteres (se traduce a VARCHAR(50) normalmente).
        // Inicializamos con string.Empty para evitar null en tiempo de ejecucion.
        [Required]
        [StringLength(50)]
        public string Marca { get; set; } = string.Empty;

        // Igual que Marca: obligatorio, maximo 50 caracteres.
        [Required]
        [StringLength(50)]
        public string Modelo { get; set; } = string.Empty;

        // Color: obligatorio, maximo 30 caracteres.
        [Required]
        [StringLength(30)]
        public string Color { get; set; } = string.Empty;

        // Fabricacion: año de fabricacion. [Range] limita los valores permitidos.
        // Si alguien intenta guardar 1800 o 3000, la validacion fallara (en tiempo de validacion del modelo).
        [Required]
        [Range(1900, 2030)]
        public int Fabricacion { get; set; }

        // NumeroMotor: obligatorio, maximo 20 caracteres.
        [Required]
        [StringLength(20)]
        public string NumeroMotor { get; set; } = string.Empty;

        // NumeroChasis: obligatorio, maximo 17 caracteres.
        // 17 es la longitud estandar de un VIN (Vehicle Identification Number).
        [Required]
        [StringLength(17)] // VIN estandar
        public string NumeroChasis { get; set; } = string.Empty;

        // FechaCreacion y FechaActualizacion:
        // - Aqui se inicializan con DateTime.UtcNow al crear el objeto en memoria.
        // - Importante: esto asigna la fecha en el **momento en que se instancia la clase en .NET**,
        //   no en la BD. Si el DbContext tambien define un DEFAULT en SQL (ej: GETUTCDATE()),
        //   el valor enviado a la BD depende de si EF incluye el valor en el INSERT.
        // - Usar UtcNow es una buena practica para evitar problemas de zonas horarias.
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
        public DateTime FechaActualizacion { get; set; } = DateTime.UtcNow;
    }
}*/
/*
1) ¿Que es esta clase Automovil?

Es una entidad: un objeto C# que EF Core mapeara a una tabla en la base de datos (por ejemplo Automoviles).

Cuando usas DbContext y DbSet<Automovil>, EF entiende que debe crear/consultar la tabla correspondiente.

2) Atributos mas importantes y que significan

[Key]
Indica que la propiedad Id es la clave primaria (PK). Es lo que identifica de forma unica a cada fila.

[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
Dice que la base de datos generara el valor (p. ej. IDENTITY en SQL Server => autoincrement). Tu no debes asignar manualmente Id al crear un registro.

[Required]
Marca la propiedad como obligatoria. En EF esto genera una columna NOT NULL. En validacion del modelo (si usas la entidad directamente en un binding), tambien fuerza que no venga null.

[StringLength(n)]
Define la longitud maxima de la cadena. En la BD se mapeara a algo como VARCHAR(n) o NVARCHAR(n).

[Range(min, max)]
Valida que valores numericos esten dentro del rango indicado. Se usa tanto para validacion en C# como guia para la logica de negocio.

DateTime.UtcNow como inicializador
Pone la fecha actual en UTC cuando el objeto se crea en memoria. util para tener timestamps consistentes (no usar DateTime.Now si la app se distribuye en distintas zonas horarias).

3) ¿Que hace EF Core con esto?

Crea columnas con tipos y restricciones (longitud, not-null).

Crea la PK con autoincrement por el atributo DatabaseGenerated.

Si generas migraciones (dotnet ef migrations add ...), esos atributos influyen en el script SQL.

4) Puntos importantes y recomendaciones (practicas)

Separar validaciones entre DTOs y Entidad:

Es habitual (y recomendable) poner las reglas de validacion en los DTOs (los que recibe la API), no necesariamente en la entidad.

Ventaja: la entidad refleja el modelo de datos, el DTO refleja las reglas de entrada y mensajes de error para el usuario.

Evitar duplicidad de logica de fecha:

Si en DbContext.OnModelCreating usas .HasDefaultValueSql("GETUTCDATE()"), entonces la BD puede establecer la fecha.

Si ademas en la entidad defines FechaCreacion = DateTime.UtcNow, EF enviara ese valor al INSERT y la funcion SQL no se usara (porque ya hay valor).

Decide: ¿quieres que la BD ponga la fecha o que la app la ponga? Mantener una sola fuente evita inconsistencias.

Normalizar NumeroChasis y NumeroMotor:

En el flujo de creacion/actualizacion es buena idea Trim() y convertir a ToUpper() (como observaste en mappings). Asi evitas duplicados por mayusculas/minusculas.

indices unicos:

Si NumeroChasis y NumeroMotor deben ser unicos, debe agregarse indice unico (se hace en OnModelCreating o migracion). Esto evita duplicados a nivel BD.

Control de concurrencia:

Si varias solicitudes pueden actualizar el mismo auto, considerar un RowVersion (byte[] timestamp) para evitar sobrescribir cambios sin querer.

Nullability:

Las propiedades string aqui se inicializan con string.Empty y no son string?, por lo que son no-nullables a nivel C#. Eso ayuda a evitar excepciones de referencia nula.

5) Flujo tipico en una API

Cliente envia JSON para crear un auto (p. ej. AutomovilCreateDto).

Controller valida DTO, pasa datos al servicio.

Servicio mapea DTO a Automovil y guarda con DbContext.Automoviles.Add(automovil) + SaveChanges().

EF genera el INSERT; la BD asigna Id si esta configurado como Identity.

API devuelve respuesta con AutomovilResponseDto.
*/
/*using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutomovilAPI.Domain.Entities
{
    public class Automovil
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Marca { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Modelo { get; set; } = string.Empty;

        [Required]
        [StringLength(30)]
        public string Color { get; set; } = string.Empty;

        [Required]
        [Range(1900, 2030)]
        public int Fabricacion { get; set; }

        [Required]
        [StringLength(20)]
        public string NumeroMotor { get; set; } = string.Empty;

        [Required]
        [StringLength(17)] // VIN estandar
        public string NumeroChasis { get; set; } = string.Empty;

        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
        public DateTime FechaActualizacion { get; set; } = DateTime.UtcNow;
    }
}*/