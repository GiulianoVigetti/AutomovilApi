using System.Collections.Generic;
using System.Threading.Tasks;
using HybridDDDArchitecture.Core.Domain.Entities;

namespace HybridDDDArchitecture.Core.Application.Repositories
{
    public interface IAutomovilRepository
    {
        Task<Automovil> CreateAsync(Automovil automovil);
        Task<Automovil> UpdateAsync(Automovil automovil);
        Task<bool> DeleteAsync(int id);
        Task<Automovil> GetByIdAsync(int id);
        Task<Automovil> GetByChasisAsync(string numeroChasis);
        Task<IEnumerable<Automovil>> GetAllAsync();
        Task<bool> ExistsAsync(int id);
        Task<bool> ExistsNumeroMotorAsync(string numeroMotor, int? excludeId = null);
        Task<bool> ExistsNumeroChasisAsync(string numeroChasis, int? excludeId = null);
    }
}
/*using System.Collections.Generic;
using System.Threading.Tasks;
using HybridDDDArchitecture.Core.Domain.Entities;

namespace HybridDDDArchitecture.Core.Application.Repositories
{
    public interface IAutomovilRepository
    {
        Task<Automovil> CreateAsync(Automovil automovil);
        Task<Automovil> UpdateAsync(Automovil automovil);
        Task<bool> DeleteAsync(int id);
        Task<Automovil?> GetByIdAsync(int id);
        Task<Automovil?> GetByChasisAsync(string numeroChasis);
        Task<IEnumerable<Automovil>> GetAllAsync();
        Task<bool> ExistsAsync(int id);
        Task<bool> ExistsNumeroMotorAsync(string numeroMotor, int? excludeId = null);
        Task<bool> ExistsNumeroChasisAsync(string numeroChasis, int? excludeId = null);
    }
}*/