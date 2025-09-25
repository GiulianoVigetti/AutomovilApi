using Microsoft.EntityFrameworkCore;
using HybridDDDArchitecture.Core.Domain.Entities;
//using AutomovilAPI.Domain.Entities;

namespace HybridDDDArchitecture.Core.Infraestructure.Repositories.Sql
{
    public class AutomovilDbContext : DbContext
    {
        public AutomovilDbContext(DbContextOptions<AutomovilDbContext> options) : base(options) { }

        public DbSet<Automovil> Automoviles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Automovil>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Marca)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Modelo)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Color)
                    .IsRequired()
                    .HasMaxLength(30);

                entity.Property(e => e.NumeroMotor)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.NumeroChasis)
                    .IsRequired()
                    .HasMaxLength(17);

                entity.Property(e => e.FechaCreacion)
                    .HasDefaultValueSql("GETUTCDATE()");

                entity.Property(e => e.FechaActualizacion)
                    .HasDefaultValueSql("GETUTCDATE()");

                entity.HasIndex(e => e.NumeroMotor)
                    .IsUnique()
                    .HasDatabaseName("IX_Automovil_NumeroMotor");

                entity.HasIndex(e => e.NumeroChasis)
                    .IsUnique()
                    .HasDatabaseName("IX_Automovil_NumeroChasis");

                entity.ToTable("Automoviles");
            });

            var fechaFija = new DateTime(2024, 01, 01);

            modelBuilder.Entity<Automovil>().HasData(
                new Automovil
                {
                    Id = 1,
                    Marca = "Toyota",
                    Modelo = "Corolla",
                    Color = "Blanco",
                    Fabricacion = 2022,
                    NumeroMotor = "TOY2022001",
                    NumeroChasis = "1HGCM82633A123456",
                    FechaCreacion = fechaFija,
                    FechaActualizacion = fechaFija
                },
                new Automovil
                {
                    Id = 2,
                    Marca = "Ford",
                    Modelo = "Mustang",
                    Color = "Rojo",
                    Fabricacion = 2023,
                    NumeroMotor = "FOR2023001",
                    NumeroChasis = "1FA6P8CF5H5123457",
                    FechaCreacion = fechaFija,
                    FechaActualizacion = fechaFija
                }
            );
        }

        public override int SaveChanges()
        {
            UpdateTimestamps();
            return base.SaveChanges();
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            UpdateTimestamps();
            return await base.SaveChangesAsync(cancellationToken);
        }

        private void UpdateTimestamps()
        {
            var entries = ChangeTracker.Entries()
                .Where(e => e.Entity is Automovil &&
                           (e.State == EntityState.Added || e.State == EntityState.Modified));

            foreach (var entityEntry in entries)
            {
                var automovil = (Automovil)entityEntry.Entity;
                automovil.FechaActualizacion = DateTime.UtcNow;

                if (entityEntry.State == EntityState.Added)
                {
                    automovil.FechaCreacion = DateTime.UtcNow;
                }
            }
        }
    }
}

/*// Importamos Entity Framework Core.
// Esta librería nos permite trabajar con bases de datos usando objetos de C#
// en vez de escribir consultas SQL directamente.
using Microsoft.EntityFrameworkCore; 

// Importamos la clase Automovil, que es la entidad (representa una tabla en la base de datos).
using AutomovilAPI.Domain.Entities;

//using AutomovilAPI.Infrastructure.Data;

namespace AutomovilAPI.Infrastructure.Data
{
    // Definimos la clase AutomovilDbContext.
    // DbContext es la clase base de EF Core que representa la conexión a la BD y las entidades que gestionamos.
    public class AutomovilDbContext : DbContext
    {
        // Constructor que recibe opciones de configuración (por ejemplo, la cadena de conexión a la BD).
        // "options" es pasado desde afuera cuando configuramos la aplicación.
        public AutomovilDbContext(DbContextOptions<AutomovilDbContext> options) : base(options)
        {
        }

        // DbSet representa una tabla en la base de datos.
        // En este caso, "Automoviles" será la tabla que guarda objetos del tipo Automovil.
        public DbSet<Automovil> Automoviles { get; set; }

        // Este método se ejecuta cuando se crea el modelo de la base de datos.
        // Aquí definimos las reglas de cómo se mapea la clase Automovil a la tabla.
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuración de la entidad Automovil
            modelBuilder.Entity<Automovil>(entity =>
            {
                // Definimos la clave primaria (primary key).
                entity.HasKey(e => e.Id);

                // Definimos reglas para las propiedades:
                // "IsRequired" significa que el campo no puede ser nulo.
                // "HasMaxLength" define el tamaño máximo en la base de datos.
                entity.Property(e => e.Marca)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Modelo)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Color)
                    .IsRequired()
                    .HasMaxLength(30);

                entity.Property(e => e.NumeroMotor)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.NumeroChasis)
                    .IsRequired()
                    .HasMaxLength(17);

                // Configuramos valores por defecto para fechas.
                // GETUTCDATE() es una función de SQL Server que devuelve la fecha actual en UTC.
                entity.Property(e => e.FechaCreacion)
                    .HasDefaultValueSql("GETUTCDATE()");

                entity.Property(e => e.FechaActualizacion)
                    .HasDefaultValueSql("GETUTCDATE()");

                // Índices únicos: esto evita que se repitan valores en la BD.
                // Por ejemplo, no puede haber dos autos con el mismo Nº de motor o chasis.
                entity.HasIndex(e => e.NumeroMotor)
                    .IsUnique()
                    .HasDatabaseName("IX_Automovil_NumeroMotor");

                entity.HasIndex(e => e.NumeroChasis)
                    .IsUnique()
                    .HasDatabaseName("IX_Automovil_NumeroChasis");

                // Configuración del nombre de la tabla en la base de datos.
                entity.ToTable("Automoviles");
            });

            // Datos iniciales (seed data).
            // Estos registros se insertan automáticamente en la BD cuando se crean las migraciones.
            modelBuilder.Entity<Automovil>().HasData(
                new Automovil
                {
                    Id = 1,
                    Marca = "Toyota",
                    Modelo = "Corolla",
                    Color = "Blanco",
                    Fabricacion = 2022,
                    NumeroMotor = "TOY2022001",
                    NumeroChasis = "1HGCM82633A123456",
                    FechaCreacion = new DateTime(2024, 01, 01),
                    FechaActualizacion = new DateTime(2024, 01, 01)
                    //FechaCreacion = DateTime.UtcNow,
                    //FechaActualizacion = DateTime.UtcNow
                },
                new Automovil
                {
                    Id = 2,
                    Marca = "Ford",
                    Modelo = "Mustang",
                    Color = "Rojo",
                    Fabricacion = 2023,
                    NumeroMotor = "FOR2023001",
                    NumeroChasis = "1FA6P8CF5H5123457",
                    FechaCreacion = new DateTime(2024, 01, 01),
                    FechaActualizacion = new DateTime(2024, 01, 01)
                    //FechaCreacion = DateTime.UtcNow, 
                    //FechaActualizacion = DateTime.UtcNow
                    //DateTime.UtcNow EF Core no soporta valores dinámicos en HasData. O sea, cuando Add - Migration, se queja porque UtcNow no es constante.
                }
            );
        }

        // Sobreescribimos SaveChanges (sincrónico).
        // Antes de guardar cambios en la BD, actualizamos las fechas de creación/actualización.
        public override int SaveChanges()
        {
            UpdateTimestamps(); // Método que pone las fechas correctas.
            return base.SaveChanges(); // Guardamos en la BD.
        }

        // Sobreescribimos SaveChangesAsync (asíncrono).
        // Igual que el anterior, pero para trabajar con operaciones asíncronas.
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            UpdateTimestamps();
            return await base.SaveChangesAsync(cancellationToken);
        }

        // Método privado para actualizar fechas antes de guardar.
        private void UpdateTimestamps()
        {
            // ChangeTracker: EF Core detecta qué entidades fueron agregadas o modificadas.
            var entries = ChangeTracker.Entries()
                .Where(e => e.Entity is Automovil && 
                           (e.State == EntityState.Added || e.State == EntityState.Modified));

            // Recorremos todas las entidades Automovil que están siendo agregadas o modificadas.
            foreach (var entityEntry in entries)
            {
                var automovil = (Automovil)entityEntry.Entity;

                // Siempre que se guarde algo, se actualiza la fecha de modificación.
                automovil.FechaActualizacion = DateTime.UtcNow;

                // Si es un registro nuevo (Added), también se asigna la fecha de creación.
                if (entityEntry.State == EntityState.Added)
                {
                    automovil.FechaCreacion = DateTime.UtcNow;
                }
            }
        }
    }
}
*/
/*using Microsoft.EntityFrameworkCore;
using AutomovilAPI.Domain.Entities;

namespace AutomovilAPI.Infrastructure.Data
{
    public class AutomovilDbContext : DbContext
    {
        public AutomovilDbContext(DbContextOptions<AutomovilDbContext> options) : base(options)
        {
        }

        public DbSet<Automovil> Automoviles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Automovil>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Marca)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Modelo)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Color)
                    .IsRequired()
                    .HasMaxLength(30);

                entity.Property(e => e.NumeroMotor)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.NumeroChasis)
                    .IsRequired()
                    .HasMaxLength(17);

                entity.Property(e => e.FechaCreacion)
                    .HasDefaultValueSql("GETUTCDATE()");

                entity.Property(e => e.FechaActualizacion)
                    .HasDefaultValueSql("GETUTCDATE()");

                // Índices únicos
                entity.HasIndex(e => e.NumeroMotor)
                    .IsUnique()
                    .HasDatabaseName("IX_Automovil_NumeroMotor");

                entity.HasIndex(e => e.NumeroChasis)
                    .IsUnique()
                    .HasDatabaseName("IX_Automovil_NumeroChasis");

                // Configuración de tabla
                entity.ToTable("Automoviles");
            });

            // Datos de prueba (seed data)
            modelBuilder.Entity<Automovil>().HasData(
                new Automovil
                {
                    Id = 1,
                    Marca = "Toyota",
                    Modelo = "Corolla",
                    Color = "Blanco",
                    Fabricacion = 2022,
                    NumeroMotor = "TOY2022001",
                    NumeroChasis = "1HGCM82633A123456",
                    FechaCreacion = DateTime.UtcNow,
                    FechaActualizacion = DateTime.UtcNow
                },
                new Automovil
                {
                    Id = 2,
                    Marca = "Ford",
                    Modelo = "Mustang",
                    Color = "Rojo",
                    Fabricacion = 2023,
                    NumeroMotor = "FOR2023001",
                    NumeroChasis = "1FA6P8CF5H5123457",
                    FechaCreacion = DateTime.UtcNow,
                    FechaActualizacion = DateTime.UtcNow
                }
            );
        }

        public override int SaveChanges()
        {
            UpdateTimestamps();
            return base.SaveChanges();
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            UpdateTimestamps();
            return await base.SaveChangesAsync(cancellationToken);
        }

        private void UpdateTimestamps()
        {
            var entries = ChangeTracker.Entries()
                .Where(e => e.Entity is Automovil && 
                           (e.State == EntityState.Added || e.State == EntityState.Modified));

            foreach (var entityEntry in entries)
            {
                var automovil = (Automovil)entityEntry.Entity;
                automovil.FechaActualizacion = DateTime.UtcNow;

                if (entityEntry.State == EntityState.Added)
                {
                    automovil.FechaCreacion = DateTime.UtcNow;
                }
            }
        }
    }
}*/