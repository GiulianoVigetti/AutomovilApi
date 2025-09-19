using HybridDDDArchitecture.Core.Domain.Entities;
using HybridDDDArchitecture.Core.Application.Repositories;
using Microsoft.EntityFrameworkCore;

namespace HybridDDDArchitecture.Core.Infraestructure.Repositories.Sql
{
    public class AutomovilRepository : IAutomovilRepository
    {
        private readonly AutomovilDbContext _context;

        public AutomovilRepository(AutomovilDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Automovil>> GetAllAsync()
        {
            return await _context.Automoviles
                .OrderByDescending(a => a.FechaCreacion)
                .ToListAsync();
        }

        public async Task<Automovil?> GetByIdAsync(int id)
        {
            return await _context.Automoviles
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<Automovil?> GetByChasisAsync(string numeroChasis)
        {
            return await _context.Automoviles
                .FirstOrDefaultAsync(a => a.NumeroChasis == numeroChasis);
        }

        public async Task<Automovil> CreateAsync(Automovil automovil)
        {
            _context.Automoviles.Add(automovil);
            await _context.SaveChangesAsync();
            return automovil;
        }

        public async Task<Automovil> UpdateAsync(Automovil automovil)
        {
            _context.Automoviles.Update(automovil);
            await _context.SaveChangesAsync();
            return automovil;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var automovil = await _context.Automoviles.FindAsync(id);
            if (automovil == null)
                return false;

            _context.Automoviles.Remove(automovil);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Automoviles.AnyAsync(a => a.Id == id);
        }

        public async Task<bool> ExistsNumeroMotorAsync(string numeroMotor, int? excludeId = null)
        {
            var query = _context.Automoviles.Where(a => a.NumeroMotor == numeroMotor);
            if (excludeId.HasValue)
                query = query.Where(a => a.Id != excludeId.Value);

            return await query.AnyAsync();
        }

        public async Task<bool> ExistsNumeroChasisAsync(string numeroChasis, int? excludeId = null)
        {
            var query = _context.Automoviles.Where(a => a.NumeroChasis == numeroChasis);
            if (excludeId.HasValue)
                query = query.Where(a => a.Id != excludeId.Value);

            return await query.AnyAsync();
        }
    }
}
/*// Importamos el namespace donde esta la entidad Automovil
using AutomovilAPI.Domain.Entities;

// Importamos EF Core para usar sus metodos de acceso a base de datos (DbContext, LINQ async, etc.)
using Microsoft.EntityFrameworkCore;

namespace AutomovilAPI.Infrastructure.Repositories
{
    // Definimos una interfaz: IAutomovilRepository
    // Una interfaz dice "que metodos deben existir", pero no como se implementan.
    // Esto nos ayuda a separar la logica de la aplicacion de los detalles de la BD (desacoplamiento).
    public interface IAutomovilRepository
    {
        // Devuelve todos los automoviles
        Task<IEnumerable<Automovil>> GetAllAsync();

        // Devuelve un automovil por su ID
        Task<Automovil?> GetByIdAsync(int id);

        // Devuelve un automovil por su numero de chasis
        Task<Automovil?> GetByChasisAsync(string numeroChasis);

        // Crea un nuevo automovil en la BD
        Task<Automovil> CreateAsync(Automovil automovil);

        // Actualiza un automovil existente
        Task<Automovil?> UpdateAsync(int id, Automovil automovil);

        // Elimina un automovil por ID
        Task<bool> DeleteAsync(int id);

        // Verifica si existe un automovil por ID
        Task<bool> ExistsAsync(int id);

        // Verifica si ya existe un numero de motor (puede excluir un ID para no validar contra si mismo)
        Task<bool> ExistsNumeroMotorAsync(string numeroMotor, int? excludeId = null);

        // Verifica si ya existe un numero de chasis (idem motor)
        Task<bool> ExistsNumeroChasisAsync(string numeroChasis, int? excludeId = null);
    }

    // Implementacion concreta de la interfaz IAutomovilRepository
    // Aqui si escribimos el "como" se hacen las operaciones con EF Core
    public class AutomovilRepository : IAutomovilRepository
    {
        // Inyectamos el DbContext que representa la conexion a la base de datos
        private readonly AutomovilDbContext _context;

        // Constructor: recibimos el contexto y lo guardamos en el campo privado _context
        public AutomovilRepository(AutomovilDbContext context)
        {
            _context = context;
        }

        // Metodo: obtener todos los automoviles
        public async Task<IEnumerable<Automovil>> GetAllAsync()
        {
            // Usamos LINQ: pedimos todos los automoviles, ordenados por fecha de creacion descendente
            return await _context.Automoviles
                .OrderByDescending(a => a.FechaCreacion)
                .ToListAsync(); // Ejecuta la consulta en la BD y devuelve una lista
        }

        // Metodo: obtener un automovil por ID
        public async Task<Automovil?> GetByIdAsync(int id)
        {
            // Busca el primer auto con ese ID, o null si no lo encuentra
            return await _context.Automoviles
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        // Metodo: obtener un automovil por numero de chasis
        public async Task<Automovil?> GetByChasisAsync(string numeroChasis)
        {
            return await _context.Automoviles
                .FirstOrDefaultAsync(a => a.NumeroChasis == numeroChasis);
        }

        // Metodo: crear un nuevo automovil
        public async Task<Automovil> CreateAsync(Automovil automovil)
        {
            // Agregamos el objeto al DbSet (todavia no se guarda en la BD)
            _context.Automoviles.Add(automovil);

            // Guardamos cambios en la BD (INSERT)
            await _context.SaveChangesAsync();

            // Devolvemos el auto ya guardado (con ID generado por la BD)
            return automovil;
        }

        // Metodo: actualizar un automovil
        public async Task<Automovil?> UpdateAsync(int id, Automovil automovil)
        {
            // Buscamos si existe el auto en la BD
            var existingAutomovil = await _context.Automoviles.FindAsync(id);
            if (existingAutomovil == null)
                return null; // Si no existe, devolvemos null

            // Actualizamos solo los campos que nos pasaron (evitamos sobreescribir todo)
            existingAutomovil.Color = automovil.Color ?? existingAutomovil.Color;
            existingAutomovil.NumeroMotor = automovil.NumeroMotor ?? existingAutomovil.NumeroMotor;
            existingAutomovil.Fabricacion = automovil.Fabricacion != 0 ? automovil.Fabricacion : existingAutomovil.Fabricacion;

            // Le decimos al contexto que se actualizo el objeto
            _context.Automoviles.Update(existingAutomovil);

            // Guardamos cambios en la BD (UPDATE)
            await _context.SaveChangesAsync();

            return existingAutomovil;
        }

        // Metodo: eliminar un automovil
        public async Task<bool> DeleteAsync(int id)
        {
            var automovil = await _context.Automoviles.FindAsync(id);
            if (automovil == null)
                return false;

            _context.Automoviles.Remove(automovil); // Marcamos para eliminar
            await _context.SaveChangesAsync(); // Ejecutamos DELETE
            return true;
        }

        // Metodo: verificar si existe un auto por ID
        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Automoviles.AnyAsync(a => a.Id == id);
        }

        // Metodo: verificar si existe un auto con un numero de motor
        public async Task<bool> ExistsNumeroMotorAsync(string numeroMotor, int? excludeId = null)
        {
            // Empezamos la consulta
            var query = _context.Automoviles.Where(a => a.NumeroMotor == numeroMotor);
            
            // Si excludeId tiene valor, excluimos ese auto de la busqueda (util al actualizar)
            if (excludeId.HasValue)
                query = query.Where(a => a.Id != excludeId.Value);

            // Verificamos si existe algun registro
            return await query.AnyAsync();
        }

        // Metodo: verificar si existe un auto con un numero de chasis
        public async Task<bool> ExistsNumeroChasisAsync(string numeroChasis, int? excludeId = null)
        {
            var query = _context.Automoviles.Where(a => a.NumeroChasis == numeroChasis);
            
            if (excludeId.HasValue)
                query = query.Where(a => a.Id != excludeId.Value);

            return await query.AnyAsync();
        }
    }
}*/
/*using AutomovilAPI.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AutomovilAPI.Infrastructure.Repositories
{
    public interface IAutomovilRepository
    {
        Task<IEnumerable<Automovil>> GetAllAsync();
        Task<Automovil?> GetByIdAsync(int id);
        Task<Automovil?> GetByChasisAsync(string numeroChasis);
        Task<Automovil> CreateAsync(Automovil automovil);
        Task<Automovil?> UpdateAsync(int id, Automovil automovil);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<bool> ExistsNumeroMotorAsync(string numeroMotor, int? excludeId = null);
        Task<bool> ExistsNumeroChasisAsync(string numeroChasis, int? excludeId = null);
    }

    public class AutomovilRepository : IAutomovilRepository
    {
        private readonly AutomovilDbContext _context;

        public AutomovilRepository(AutomovilDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Automovil>> GetAllAsync()
        {
            return await _context.Automoviles
                .OrderByDescending(a => a.FechaCreacion)
                .ToListAsync();
        }

        public async Task<Automovil?> GetByIdAsync(int id)
        {
            return await _context.Automoviles
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<Automovil?> GetByChasisAsync(string numeroChasis)
        {
            return await _context.Automoviles
                .FirstOrDefaultAsync(a => a.NumeroChasis == numeroChasis);
        }

        public async Task<Automovil> CreateAsync(Automovil automovil)
        {
            _context.Automoviles.Add(automovil);
            await _context.SaveChangesAsync();
            return automovil;
        }

        public async Task<Automovil?> UpdateAsync(int id, Automovil automovil)
        {
            var existingAutomovil = await _context.Automoviles.FindAsync(id);
            if (existingAutomovil == null)
                return null;

            // Actualizar solo los campos proporcionados
            existingAutomovil.Color = automovil.Color ?? existingAutomovil.Color;
            existingAutomovil.NumeroMotor = automovil.NumeroMotor ?? existingAutomovil.NumeroMotor;
            existingAutomovil.Fabricacion = automovil.Fabricacion != 0 ? automovil.Fabricacion : existingAutomovil.Fabricacion;

            _context.Automoviles.Update(existingAutomovil);
            await _context.SaveChangesAsync();
            return existingAutomovil;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var automovil = await _context.Automoviles.FindAsync(id);
            if (automovil == null)
                return false;

            _context.Automoviles.Remove(automovil);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Automoviles.AnyAsync(a => a.Id == id);
        }

        public async Task<bool> ExistsNumeroMotorAsync(string numeroMotor, int? excludeId = null)
        {
            var query = _context.Automoviles.Where(a => a.NumeroMotor == numeroMotor);
            
            if (excludeId.HasValue)
                query = query.Where(a => a.Id != excludeId.Value);

            return await query.AnyAsync();
        }

        public async Task<bool> ExistsNumeroChasisAsync(string numeroChasis, int? excludeId = null)
        {
            var query = _context.Automoviles.Where(a => a.NumeroChasis == numeroChasis);
            
            if (excludeId.HasValue)
                query = query.Where(a => a.Id != excludeId.Value);

            return await query.AnyAsync();
        }
    }
}*/