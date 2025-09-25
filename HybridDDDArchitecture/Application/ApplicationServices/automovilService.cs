using HybridDDDArchitecture.Core.Application.DataTransferObjects;
using HybridDDDArchitecture.Core.Domain.Entities;
using HybridDDDArchitecture.Core.Application.Repositories;
using HybridDDDArchitecture.Core.Application.Wrappers;
using Microsoft.Extensions.Logging;

namespace HybridDDDArchitecture.Core.Application.ApplicationServices
{
    public interface IAutomovilService
    {
        Task<ApiResponse<IEnumerable<AutomovilResponseDto>>> GetAllAsync();
        Task<ApiResponse<AutomovilResponseDto>> GetByIdAsync(int id);
        Task<ApiResponse<AutomovilResponseDto>> GetByChasisAsync(string numeroChasis);
        Task<ApiResponse<AutomovilResponseDto>> CreateAsync(AutomovilCreateDto createDto);
        Task<ApiResponse<AutomovilResponseDto>> UpdateAsync(int id, AutomovilUpdateDto updateDto);
        Task<ApiResponse<string>> DeleteAsync(int id);
    }

    public class AutomovilService : IAutomovilService
    {
        private readonly IAutomovilRepository _repository;
        private readonly ILogger<AutomovilService> _logger;

        public AutomovilService(
            IAutomovilRepository repository,
            ILogger<AutomovilService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<ApiResponse<IEnumerable<AutomovilResponseDto>>> GetAllAsync()
        {
            try
            {
                var automoviles = await _repository.GetAllAsync();
                var automovilesDto = automoviles.Select(MapToResponseDto);
                return new ApiResponse<IEnumerable<AutomovilResponseDto>>(true,
                    "Automóviles obtenidos exitosamente", automovilesDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los automóviles");
                return new ApiResponse<IEnumerable<AutomovilResponseDto>>(false,
                    "Error interno del servidor", null, new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<AutomovilResponseDto>> GetByIdAsync(int id)
        {
            try
            {
                var automovil = await _repository.GetByIdAsync(id);
                if (automovil == null)
                    return new ApiResponse<AutomovilResponseDto>(false, $"No se encontró un automóvil con ID {id}");

                var automovilDto = MapToResponseDto(automovil);
                return new ApiResponse<AutomovilResponseDto>(true, "Automóvil obtenido exitosamente", automovilDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener automóvil por ID {Id}", id);
                return new ApiResponse<AutomovilResponseDto>(false, "Error interno del servidor", null, new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<AutomovilResponseDto>> GetByChasisAsync(string numeroChasis)
        {
            try
            {
                var automovil = await _repository.GetByChasisAsync(numeroChasis);
                if (automovil == null)
                    return new ApiResponse<AutomovilResponseDto>(false, $"No se encontró un automóvil con número de chasis {numeroChasis}");

                var automovilDto = MapToResponseDto(automovil);
                return new ApiResponse<AutomovilResponseDto>(true, "Automóvil obtenido exitosamente", automovilDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener automóvil por chasis {NumeroChasis}", numeroChasis);
                return new ApiResponse<AutomovilResponseDto>(false, "Error interno del servidor", null, new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<AutomovilResponseDto>> CreateAsync(AutomovilCreateDto createDto)
        {
            try
            {
                if (await _repository.ExistsNumeroMotorAsync(createDto.NumeroMotor))
                    return new ApiResponse<AutomovilResponseDto>(false, "Ya existe un automóvil con este número de motor", null, new List<string> { "El número de motor debe ser único" });

                if (await _repository.ExistsNumeroChasisAsync(createDto.NumeroChasis))
                    return new ApiResponse<AutomovilResponseDto>(false, "Ya existe un automóvil con este número de chasis", null, new List<string> { "El número de chasis debe ser único" });

                var automovil = MapFromCreateDto(createDto);
                var createdAutomovil = await _repository.CreateAsync(automovil);
                var automovilDto = MapToResponseDto(createdAutomovil);

                return new ApiResponse<AutomovilResponseDto>(true, "Automóvil creado exitosamente", automovilDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear automóvil");
                return new ApiResponse<AutomovilResponseDto>(false, "Error interno del servidor", null, new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<AutomovilResponseDto>> UpdateAsync(int id, AutomovilUpdateDto updateDto)
        {
            try
            {
                if (!await _repository.ExistsAsync(id))
                    return new ApiResponse<AutomovilResponseDto>(false, $"No se encontró un automóvil con ID {id}");

                if (!string.IsNullOrEmpty(updateDto.NumeroMotor) &&
                    await _repository.ExistsNumeroMotorAsync(updateDto.NumeroMotor, id))
                    return new ApiResponse<AutomovilResponseDto>(false, "Ya existe un automóvil con este número de motor", null, new List<string> { "El número de motor debe ser único" });

                if (!string.IsNullOrEmpty(updateDto.NumeroChasis) &&
                    await _repository.ExistsNumeroChasisAsync(updateDto.NumeroChasis, id))
                    return new ApiResponse<AutomovilResponseDto>(false, "Ya existe un automóvil con este número de chasis", null, new List<string> { "El número de chasis debe ser único" });

                var automovilToUpdate = await _repository.GetByIdAsync(id);
                if (automovilToUpdate == null)
                    return new ApiResponse<AutomovilResponseDto>(false, "Error al actualizar el automóvil. No se encontró el auto.");

                // Mapeo manual de campos a actualizar
                if (!string.IsNullOrEmpty(updateDto.Color))
                    automovilToUpdate.Color = updateDto.Color;
                if (!string.IsNullOrEmpty(updateDto.NumeroMotor))
                    automovilToUpdate.NumeroMotor = updateDto.NumeroMotor;
                if (!string.IsNullOrEmpty(updateDto.NumeroChasis))
                    automovilToUpdate.NumeroChasis = updateDto.NumeroChasis;
                if (updateDto.Fabricacion.HasValue)
                    automovilToUpdate.Fabricacion = updateDto.Fabricacion.Value;

                var updatedAutomovil = await _repository.UpdateAsync(automovilToUpdate);
                var automovilDto = MapToResponseDto(updatedAutomovil);

                return new ApiResponse<AutomovilResponseDto>(true, "Automóvil actualizado exitosamente", automovilDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar automóvil con ID {Id}", id);
                return new ApiResponse<AutomovilResponseDto>(false, "Error interno del servidor", null, new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<string>> DeleteAsync(int id)
        {
            try
            {
                var deleted = await _repository.DeleteAsync(id);
                if (!deleted)
                    return new ApiResponse<string>(false, $"No se encontró un automóvil con ID {id}");

                return new ApiResponse<string>(true, "Automóvil eliminado exitosamente", "Eliminación confirmada");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar automóvil con ID {Id}", id);
                return new ApiResponse<string>(false, "Error interno del servidor", null, new List<string> { ex.Message });
            }
        }

        // Métodos de mapeo manual
        private AutomovilResponseDto MapToResponseDto(Automovil automovil)
        {
            return new AutomovilResponseDto
            {
                Id = automovil.Id,
                Marca = automovil.Marca,
                Modelo = automovil.Modelo,
                Color = automovil.Color,
                Fabricacion = automovil.Fabricacion,
                NumeroMotor = automovil.NumeroMotor,
                NumeroChasis = automovil.NumeroChasis,
                FechaCreacion = automovil.FechaCreacion,
                FechaActualizacion = automovil.FechaActualizacion
            };
        }

        private Automovil MapFromCreateDto(AutomovilCreateDto dto)
        {
            return new Automovil
            {
                Marca = dto.Marca,
                Modelo = dto.Modelo,
                Color = dto.Color,
                Fabricacion = dto.Fabricacion,
                NumeroMotor = dto.NumeroMotor,
                NumeroChasis = dto.NumeroChasis,
                FechaCreacion = DateTime.UtcNow,
                FechaActualizacion = DateTime.UtcNow
            };
        }
    }
}
/*using HybridDDDArchitecture.Core.Application.DataTransferObjects;
using HybridDDDArchitecture.Core.Domain.Entities;
using HybridDDDArchitecture.Core.Application.Repositories;
using HybridDDDArchitecture.Core.Application.Wrappers;
using AutoMapper;
using Microsoft.Extensions.Logging;

namespace HybridDDDArchitecture.Core.Application.ApplicationServices
{
    // Interfaz del servicio de Automóviles
    public interface IAutomovilService
    {
        Task<ApiResponse<IEnumerable<AutomovilResponseDto>>> GetAllAsync();
        Task<ApiResponse<AutomovilResponseDto>> GetByIdAsync(int id);
        Task<ApiResponse<AutomovilResponseDto>> GetByChasisAsync(string numeroChasis);
        Task<ApiResponse<AutomovilResponseDto>> CreateAsync(AutomovilCreateDto createDto);
        Task<ApiResponse<AutomovilResponseDto>> UpdateAsync(int id, AutomovilUpdateDto updateDto);
        Task<ApiResponse<string>> DeleteAsync(int id);
    }

    // Implementación de la interfaz
    public class AutomovilService : IAutomovilService
    {
        private readonly IAutomovilRepository _repository;
        private readonly IMapper _mapper;
        private readonly ILogger<AutomovilService> _logger;

        public AutomovilService(
            IAutomovilRepository repository,
            IMapper mapper,
            ILogger<AutomovilService> logger)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
        }

        // Obtener todos los autos
        public async Task<ApiResponse<IEnumerable<AutomovilResponseDto>>> GetAllAsync()
        {
            try
            {
                var automoviles = await _repository.GetAllAsync();
                var automovilesDto = _mapper.Map<IEnumerable<AutomovilResponseDto>>(automoviles);
                return new ApiResponse<IEnumerable<AutomovilResponseDto>>(true,
                    "Automóviles obtenidos exitosamente", automovilesDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los automóviles");
                return new ApiResponse<IEnumerable<AutomovilResponseDto>>(false,
                    "Error interno del servidor", null, new List<string> { ex.Message });
            }
        }

        // Obtener un auto por ID
        public async Task<ApiResponse<AutomovilResponseDto>> GetByIdAsync(int id)
        {
            try
            {
                var automovil = await _repository.GetByIdAsync(id);
                if (automovil == null)
                    return new ApiResponse<AutomovilResponseDto>(false, $"No se encontró un automóvil con ID {id}");

                var automovilDto = _mapper.Map<AutomovilResponseDto>(automovil);
                return new ApiResponse<AutomovilResponseDto>(true, "Automóvil obtenido exitosamente", automovilDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener automóvil por ID {Id}", id);
                return new ApiResponse<AutomovilResponseDto>(false, "Error interno del servidor", null, new List<string> { ex.Message });
            }
        }

        // Obtener un auto por número de chasis
        public async Task<ApiResponse<AutomovilResponseDto>> GetByChasisAsync(string numeroChasis)
        {
            try
            {
                var automovil = await _repository.GetByChasisAsync(numeroChasis);
                if (automovil == null)
                    return new ApiResponse<AutomovilResponseDto>(false, $"No se encontró un automóvil con número de chasis {numeroChasis}");

                var automovilDto = _mapper.Map<AutomovilResponseDto>(automovil);
                return new ApiResponse<AutomovilResponseDto>(true, "Automóvil obtenido exitosamente", automovilDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener automóvil por chasis {NumeroChasis}", numeroChasis);
                return new ApiResponse<AutomovilResponseDto>(false, "Error interno del servidor", null, new List<string> { ex.Message });
            }
        }

        // Crear un auto nuevo
        public async Task<ApiResponse<AutomovilResponseDto>> CreateAsync(AutomovilCreateDto createDto)
        {
            try
            {
                if (await _repository.ExistsNumeroMotorAsync(createDto.NumeroMotor))
                    return new ApiResponse<AutomovilResponseDto>(false, "Ya existe un automóvil con este número de motor", null, new List<string> { "El número de motor debe ser único" });

                if (await _repository.ExistsNumeroChasisAsync(createDto.NumeroChasis))
                    return new ApiResponse<AutomovilResponseDto>(false, "Ya existe un automóvil con este número de chasis", null, new List<string> { "El número de chasis debe ser único" });

                var automovil = _mapper.Map<Automovil>(createDto);
                var createdAutomovil = await _repository.CreateAsync(automovil);
                var automovilDto = _mapper.Map<AutomovilResponseDto>(createdAutomovil);

                return new ApiResponse<AutomovilResponseDto>(true, "Automóvil creado exitosamente", automovilDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear automóvil");
                return new ApiResponse<AutomovilResponseDto>(false, "Error interno del servidor", null, new List<string> { ex.Message });
            }
        }

        // Actualizar un auto existente
        public async Task<ApiResponse<AutomovilResponseDto>> UpdateAsync(int id, AutomovilUpdateDto updateDto)
        {
            try
            {
                if (!await _repository.ExistsAsync(id))
                    return new ApiResponse<AutomovilResponseDto>(false, $"No se encontró un automóvil con ID {id}");

                if (!string.IsNullOrEmpty(updateDto.NumeroMotor) &&
                    await _repository.ExistsNumeroMotorAsync(updateDto.NumeroMotor, id))
                    return new ApiResponse<AutomovilResponseDto>(false, "Ya existe un automóvil con este número de motor", null, new List<string> { "El número de motor debe ser único" });

                if (!string.IsNullOrEmpty(updateDto.NumeroChasis) &&
                    await _repository.ExistsNumeroChasisAsync(updateDto.NumeroChasis, id))
                    return new ApiResponse<AutomovilResponseDto>(false, "Ya existe un automóvil con este número de chasis", null, new List<string> { "El número de chasis debe ser único" });

                var automovilToUpdate = await _repository.GetByIdAsync(id);
                if (automovilToUpdate == null)
                    return new ApiResponse<AutomovilResponseDto>(false, "Error al actualizar el automóvil. No se encontró el auto.");

                _mapper.Map(updateDto, automovilToUpdate);
                var updatedAutomovil = await _repository.UpdateAsync(automovilToUpdate);
                var automovilDto = _mapper.Map<AutomovilResponseDto>(updatedAutomovil);

                return new ApiResponse<AutomovilResponseDto>(true, "Automóvil actualizado exitosamente", automovilDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar automóvil con ID {Id}", id);
                return new ApiResponse<AutomovilResponseDto>(false, "Error interno del servidor", null, new List<string> { ex.Message });
            }
        }

        // Eliminar un auto
        public async Task<ApiResponse<string>> DeleteAsync(int id)
        {
            try
            {
                var deleted = await _repository.DeleteAsync(id);
                if (!deleted)
                    return new ApiResponse<string>(false, $"No se encontró un automóvil con ID {id}");

                return new ApiResponse<string>(true, "Automóvil eliminado exitosamente", "Eliminación confirmada");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar automóvil con ID {Id}", id);
                return new ApiResponse<string>(false, "Error interno del servidor", null, new List<string> { ex.Message });
            }
        }
    }
}*/
/*using HybridDDDArchitecture.Core.Application.DataTransferObjects;
using HybridDDDArchitecture.Core.Domain.Entities;
using HybridDDDArchitecture.Core.Application.Repositories;
using HybridDDDArchitecture.Core.Application.Wrappers;
using AutoMapper;
using Microsoft.Extensions.Logging;

namespace HybridDDDArchitecture.Core.Application.ApplicationServices
{
    // Interfaz del servicio de Automóviles
    public interface IAutomovilService
    {
        Task<ApiResponse<IEnumerable<AutomovilResponseDto>>> GetAllAsync();
        Task<ApiResponse<AutomovilResponseDto>> GetByIdAsync(int id);
        Task<ApiResponse<AutomovilResponseDto>> GetByChasisAsync(string numeroChasis);
        Task<ApiResponse<AutomovilResponseDto>> CreateAsync(AutomovilCreateDto createDto);
        Task<ApiResponse<AutomovilResponseDto>> UpdateAsync(int id, AutomovilUpdateDto updateDto);
        Task<ApiResponse<string>> DeleteAsync(int id);
    }

    // Implementación de la interfaz
    public class AutomovilService : IAutomovilService
    {
        private readonly IAutomovilRepository _repository;
        private readonly IMapper _mapper;
        private readonly ILogger<AutomovilService> _logger;

        public AutomovilService(
            IAutomovilRepository repository,
            IMapper mapper,
            ILogger<AutomovilService> logger)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
        }

        // Obtener todos los autos
        public async Task<ApiResponse<IEnumerable<AutomovilResponseDto>>> GetAllAsync()
        {
            try
            {
                var automoviles = await _repository.GetAllAsync();
                var automovilesDto = _mapper.Map<IEnumerable<AutomovilResponseDto>>(automoviles);
                return new ApiResponse<IEnumerable<AutomovilResponseDto>>(true,
                    "Automóviles obtenidos exitosamente", automovilesDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los automóviles");
                return new ApiResponse<IEnumerable<AutomovilResponseDto>>(false,
                    "Error interno del servidor", null, new List<string> { ex.Message });
            }
        }

        // Obtener un auto por ID
        public async Task<ApiResponse<AutomovilResponseDto>> GetByIdAsync(int id)
        {
            try
            {
                var automovil = await _repository.GetByIdAsync(id);
                if (automovil == null)
                    return new ApiResponse<AutomovilResponseDto>(false, $"No se encontró un automóvil con ID {id}");

                var automovilDto = _mapper.Map<AutomovilResponseDto>(automovil);
                return new ApiResponse<AutomovilResponseDto>(true, "Automóvil obtenido exitosamente", automovilDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener automóvil por ID {Id}", id);
                return new ApiResponse<AutomovilResponseDto>(false, "Error interno del servidor", null, new List<string> { ex.Message });
            }
        }

        // Obtener un auto por número de chasis
        public async Task<ApiResponse<AutomovilResponseDto>> GetByChasisAsync(string numeroChasis)
        {
            try
            {
                var automovil = await _repository.GetByChasisAsync(numeroChasis);
                if (automovil == null)
                    return new ApiResponse<AutomovilResponseDto>(false, $"No se encontró un automóvil con número de chasis {numeroChasis}");

                var automovilDto = _mapper.Map<AutomovilResponseDto>(automovil);
                return new ApiResponse<AutomovilResponseDto>(true, "Automóvil obtenido exitosamente", automovilDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener automóvil por chasis {NumeroChasis}", numeroChasis);
                return new ApiResponse<AutomovilResponseDto>(false, "Error interno del servidor", null, new List<string> { ex.Message });
            }
        }

        // Crear un auto nuevo
        public async Task<ApiResponse<AutomovilResponseDto>> CreateAsync(AutomovilCreateDto createDto)
        {
            try
            {
                if (await _repository.ExistsNumeroMotorAsync(createDto.NumeroMotor))
                    return new ApiResponse<AutomovilResponseDto>(false, "Ya existe un automóvil con este número de motor", null, new List<string> { "El número de motor debe ser único" });

                if (await _repository.ExistsNumeroChasisAsync(createDto.NumeroChasis))
                    return new ApiResponse<AutomovilResponseDto>(false, "Ya existe un automóvil con este número de chasis", null, new List<string> { "El número de chasis debe ser único" });

                var automovil = _mapper.Map<Automovil>(createDto);
                var createdAutomovil = await _repository.CreateAsync(automovil);
                var automovilDto = _mapper.Map<AutomovilResponseDto>(createdAutomovil);

                return new ApiResponse<AutomovilResponseDto>(true, "Automóvil creado exitosamente", automovilDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear automóvil");
                return new ApiResponse<AutomovilResponseDto>(false, "Error interno del servidor", null, new List<string> { ex.Message });
            }
        }

        // Actualizar un auto existente
        public async Task<ApiResponse<AutomovilResponseDto>> UpdateAsync(int id, AutomovilUpdateDto updateDto)
        {
            try
            {
                if (!await _repository.ExistsAsync(id))
                    return new ApiResponse<AutomovilResponseDto>(false, $"No se encontró un automóvil con ID {id}");

                if (!string.IsNullOrEmpty(updateDto.NumeroMotor) &&
                    await _repository.ExistsNumeroMotorAsync(updateDto.NumeroMotor, id))
                    return new ApiResponse<AutomovilResponseDto>(false, "Ya existe un automóvil con este número de motor", null, new List<string> { "El número de motor debe ser único" });

                if (!string.IsNullOrEmpty(updateDto.NumeroChasis) &&
                    await _repository.ExistsNumeroChasisAsync(updateDto.NumeroChasis, id))
                    return new ApiResponse<AutomovilResponseDto>(false, "Ya existe un automóvil con este número de chasis", null, new List<string> { "El número de chasis debe ser único" });

                var automovilToUpdate = await _repository.GetByIdAsync(id);
                if (automovilToUpdate == null)
                    return new ApiResponse<AutomovilResponseDto>(false, "Error al actualizar el automóvil. No se encontró el auto.");

                _mapper.Map(updateDto, automovilToUpdate);
                var updatedAutomovil = await _repository.UpdateAsync(automovilToUpdate);
                var automovilDto = _mapper.Map<AutomovilResponseDto>(updatedAutomovil);

                return new ApiResponse<AutomovilResponseDto>(true, "Automóvil actualizado exitosamente", automovilDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar automóvil con ID {Id}", id);
                return new ApiResponse<AutomovilResponseDto>(false, "Error interno del servidor", null, new List<string> { ex.Message });
            }
        }

        // Eliminar un auto
        public async Task<ApiResponse<string>> DeleteAsync(int id)
        {
            try
            {
                var deleted = await _repository.DeleteAsync(id);
                if (!deleted)
                    return new ApiResponse<string>(false, $"No se encontró un automóvil con ID {id}");

                return new ApiResponse<string>(true, "Automóvil eliminado exitosamente", "Eliminación confirmada");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar automóvil con ID {Id}", id);
                return new ApiResponse<string>(false, "Error interno del servidor", null, new List<string> { ex.Message });
            }
        }
    }
}*/
/*using HybridDDDArchitecture.Core.Application.DataTransferObjects;
using HybridDDDArchitecture.Core.Domain.Entities;
using HybridDDDArchitecture.Core.Application.Repositories;
using HybridDDDArchitecture.Core.Application.Wrappers;
using AutoMapper;
using Microsoft.Extensions.Logging;

namespace HybridDDDArchitecture.Core.Application.ApplicationServices
{
    // Interfaz del servicio de Automóviles
    public interface IAutomovilService
    {
        Task<ApiResponse<IEnumerable<AutomovilResponseDto>>> GetAllAsync();
        Task<ApiResponse<AutomovilResponseDto>> GetByIdAsync(int id);
        Task<ApiResponse<AutomovilResponseDto>> GetByChasisAsync(string numeroChasis);
        Task<ApiResponse<AutomovilResponseDto>> CreateAsync(AutomovilCreateDto createDto);
        Task<ApiResponse<AutomovilResponseDto>> UpdateAsync(int id, AutomovilUpdateDto updateDto);
        Task<ApiResponse<string>> DeleteAsync(int id);
    }

    // Implementación de la interfaz
    public class AutomovilService : IAutomovilService
    {
        private readonly IAutomovilRepository _repository;
        private readonly IMapper _mapper;
        private readonly ILogger<AutomovilService> _logger;

        public AutomovilService(
            IAutomovilRepository repository,
            IMapper mapper,
            ILogger<AutomovilService> logger)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
        }

        // Obtener todos los autos
        public async Task<ApiResponse<IEnumerable<AutomovilResponseDto>>> GetAllAsync()
        {
            try
            {
                var automoviles = await _repository.GetAllAsync();
                var automovilesDto = _mapper.Map<IEnumerable<AutomovilResponseDto>>(automoviles);

                return new ApiResponse<IEnumerable<AutomovilResponseDto>>(true, "Automóviles obtenidos exitosamente", automovilesDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los automóviles");
                return new ApiResponse<IEnumerable<AutomovilResponseDto>>(false, "Error interno del servidor", null, new List<string> { ex.Message });
            }
        }

        // Obtener un auto por ID
        public async Task<ApiResponse<AutomovilResponseDto>> GetByIdAsync(int id)
        {
            try
            {
                var automovil = await _repository.GetByIdAsync(id);
                if (automovil == null)
                    return new ApiResponse<AutomovilResponseDto>(false, $"No se encontró un automóvil con ID {id}");

                var automovilDto = _mapper.Map<AutomovilResponseDto>(automovil);
                return new ApiResponse<AutomovilResponseDto>(true, "Automóvil obtenido exitosamente", automovilDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener automóvil por ID {Id}", id);
                return new ApiResponse<AutomovilResponseDto>(false, "Error interno del servidor", null, new List<string> { ex.Message });
            }
        }

        // Obtener un auto por número de chasis
        public async Task<ApiResponse<AutomovilResponseDto>> GetByChasisAsync(string numeroChasis)
        {
            try
            {
                var automovil = await _repository.GetByChasisAsync(numeroChasis);
                if (automovil == null)
                    return new ApiResponse<AutomovilResponseDto>(false, $"No se encontró un automóvil con número de chasis {numeroChasis}");

                var automovilDto = _mapper.Map<AutomovilResponseDto>(automovil);
                return new ApiResponse<AutomovilResponseDto>(true, "Automóvil obtenido exitosamente", automovilDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener automóvil por chasis {NumeroChasis}", numeroChasis);
                return new ApiResponse<AutomovilResponseDto>(false, "Error interno del servidor", null, new List<string> { ex.Message });
            }
        }

        // Crear un auto nuevo
        public async Task<ApiResponse<AutomovilResponseDto>> CreateAsync(AutomovilCreateDto createDto)
        {
            try
            {
                if (await _repository.ExistsNumeroMotorAsync(createDto.NumeroMotor))
                    return new ApiResponse<AutomovilResponseDto>(false, "Ya existe un automóvil con este número de motor", null, new List<string> { "El número de motor debe ser único" });

                if (await _repository.ExistsNumeroChasisAsync(createDto.NumeroChasis))
                    return new ApiResponse<AutomovilResponseDto>(false, "Ya existe un automóvil con este número de chasis", null, new List<string> { "El número de chasis debe ser único" });

                var automovil = _mapper.Map<Automovil>(createDto);
                var createdAutomovil = await _repository.CreateAsync(automovil);

                var automovilDto = _mapper.Map<AutomovilResponseDto>(createdAutomovil);
                return new ApiResponse<AutomovilResponseDto>(true, "Automóvil creado exitosamente", automovilDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear automóvil");
                return new ApiResponse<AutomovilResponseDto>(false, "Error interno del servidor", null, new List<string> { ex.Message });
            }
        }

        // Actualizar un auto existente
        public async Task<ApiResponse<AutomovilResponseDto>> UpdateAsync(int id, AutomovilUpdateDto updateDto)
        {
            try
            {
                if (!await _repository.ExistsAsync(id))
                    return new ApiResponse<AutomovilResponseDto>(false, $"No se encontró un automóvil con ID {id}");

                if (!string.IsNullOrEmpty(updateDto.NumeroMotor) &&
                    await _repository.ExistsNumeroMotorAsync(updateDto.NumeroMotor, id))
                    return new ApiResponse<AutomovilResponseDto>(false, "Ya existe un automóvil con este número de motor", null, new List<string> { "El número de motor debe ser único" });

                if (!string.IsNullOrEmpty(updateDto.NumeroChasis) &&
                    await _repository.ExistsNumeroChasisAsync(updateDto.NumeroChasis, id))
                    return new ApiResponse<AutomovilResponseDto>(false, "Ya existe un automóvil con este número de chasis", null, new List<string> { "El número de chasis debe ser único" });

                var automovilToUpdate = await _repository.GetByIdAsync(id);
                if (automovilToUpdate == null)
                    return new ApiResponse<AutomovilResponseDto>(false, "Error al actualizar el automóvil. No se encontró el auto.");

                _mapper.Map(updateDto, automovilToUpdate);
                var updatedAutomovil = await _repository.UpdateAsync(automovilToUpdate);

                var automovilDto = _mapper.Map<AutomovilResponseDto>(updatedAutomovil);
                return new ApiResponse<AutomovilResponseDto>(true, "Automóvil actualizado exitosamente", automovilDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar automóvil con ID {Id}", id);
                return new ApiResponse<AutomovilResponseDto>(false, "Error interno del servidor", null, new List<string> { ex.Message });
            }
        }

        // Eliminar un auto
        public async Task<ApiResponse<string>> DeleteAsync(int id)
        {
            try
            {
                var deleted = await _repository.DeleteAsync(id);
                if (!deleted)
                    return new ApiResponse<string>(false, $"No se encontró un automóvil con ID {id}");

                return new ApiResponse<string>(true, "Automóvil eliminado exitosamente", "Eliminación confirmada");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar automóvil con ID {Id}", id);
                return new ApiResponse<string>(false, "Error interno del servidor", null, new List<string> { ex.Message });
            }
        }
    }
}*/

/*
// Usamos los DTOs (objetos de transferencia de datos) para controlar que entra y que sale de la API.
using AutomovilAPI.Domain.DTOs; 

// Usamos las entidades que representan la tabla de Automoviles en la base de datos.
using AutomovilAPI.Domain.Entities; 

// Usamos el repositorio que sabe como hablar con la base de datos.
using AutomovilAPI.Infrastructure.Repositories; 

// Usamos AutoMapper para convertir automaticamente entre DTOs y Entidades.
using AutoMapper; 

namespace AutomovilAPI.Application.Services
{
    // Esta es la INTERFAZ del servicio.
    // Define que metodos tendra el servicio de Automoviles.
    // Es como un "contrato" que obliga a cualquier clase que lo implemente a tener estos metodos.
    public interface IAutomovilService
    {
        /*Task: En C#, Task es un objeto que representa una operación asincrónica*/
/*el código que la llama puede continuar ejecutándose mientras la operación asincrónica se completa en segundo plan*/
/**/
/*Task<ApiResponse<IEnumerable<AutomovilResponseDto>>> GetAllAsync(); // Devuelve todos los autos
        Task<ApiResponse<AutomovilResponseDto>> GetByIdAsync(int id); // Busca un auto por ID
        Task<ApiResponse<AutomovilResponseDto>> GetByChasisAsync(string numeroChasis); // Busca un auto por numero de chasis
        Task<ApiResponse<AutomovilResponseDto>> CreateAsync(AutomovilCreateDto createDto); // Crea un auto nuevo
        Task<ApiResponse<AutomovilResponseDto>> UpdateAsync(int id, AutomovilUpdateDto updateDto); // Actualiza un auto existente
        Task<ApiResponse<string>> DeleteAsync(int id); // Elimina un auto por ID
    }

    // Implementacion de la interfaz IAutomovilService.
    // Aqui va la logica de negocio real de los metodos.
    public class AutomovilService : IAutomovilService
    {
        // Dependencias necesarias para este servicio:
        private readonly IAutomovilRepository _repository; // Para interactuar con la base de datos
        private readonly IMapper _mapper; // Para convertir DTOs ↔ Entidades
        private readonly ILogger<AutomovilService> _logger; // Para registrar errores o informacion en logs

        // Constructor: recibe las dependencias desde la inyeccion de dependencias de .NET
        public AutomovilService(
            IAutomovilRepository repository,
            IMapper mapper,
            ILogger<AutomovilService> logger)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
        }

        
        // MeTODO: Obtener todos los autos
        
        public async Task<ApiResponse<IEnumerable<AutomovilResponseDto>>> GetAllAsync()
        {
            try
            {
                // 1. Pide todos los autos al repositorio (base de datos).
                var automoviles = await _repository.GetAllAsync();

                // 2. Convierte la lista de Entidades -> DTOs para devolver solo lo necesario.
                var automovilesDto = _mapper.Map<IEnumerable<AutomovilResponseDto>>(automoviles);

                // 3. Devuelve una respuesta con los datos.
                return new ApiResponse<IEnumerable<AutomovilResponseDto>>
                {
                    Success = true,
                    Message = "Automoviles obtenidos exitosamente",
                    Data = automovilesDto
                };
            }
            catch (Exception ex)
            {
                // Si ocurre un error, se registra en logs y se devuelve una respuesta de error.
                _logger.LogError(ex, "Error al obtener todos los automoviles");
                return new ApiResponse<IEnumerable<AutomovilResponseDto>>
                {
                    Success = false,
                    Message = "Error interno del servidor",
                    Errors = { ex.Message }
                };
            }
        }

        
        // MeTODO: Obtener un auto por ID
        
        public async Task<ApiResponse<AutomovilResponseDto>> GetByIdAsync(int id)
        {
            try
            {
                var automovil = await _repository.GetByIdAsync(id);
                if (automovil == null)
                {
                    return new ApiResponse<AutomovilResponseDto>
                    {
                        Success = false,
                        Message = $"No se encontro un automovil con ID {id}"
                    };
                }

                var automovilDto = _mapper.Map<AutomovilResponseDto>(automovil);
                return new ApiResponse<AutomovilResponseDto>
                {
                    Success = true,
                    Message = "Automovil obtenido exitosamente",
                    Data = automovilDto
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener automovil por ID {Id}", id);
                return new ApiResponse<AutomovilResponseDto>
                {
                    Success = false,
                    Message = "Error interno del servidor",
                    Errors = { ex.Message }
                };
            }
        }

        
        // MeTODO: Obtener auto por numero de chasis
        
        public async Task<ApiResponse<AutomovilResponseDto>> GetByChasisAsync(string numeroChasis)
        {
            try
            {
                var automovil = await _repository.GetByChasisAsync(numeroChasis);
                if (automovil == null)
                {
                    return new ApiResponse<AutomovilResponseDto>
                    {
                        Success = false,
                        Message = $"No se encontro un automovil con numero de chasis {numeroChasis}"
                    };
                }

                var automovilDto = _mapper.Map<AutomovilResponseDto>(automovil);
                return new ApiResponse<AutomovilResponseDto>
                {
                    Success = true,
                    Message = "Automovil obtenido exitosamente",
                    Data = automovilDto
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener automovil por chasis {NumeroChasis}", numeroChasis);
                return new ApiResponse<AutomovilResponseDto>
                {
                    Success = false,
                    Message = "Error interno del servidor",
                    Errors = { ex.Message }
                };
            }
        }

        
        // MeTODO: Crear un auto nuevo
        
        public async Task<ApiResponse<AutomovilResponseDto>> CreateAsync(AutomovilCreateDto createDto)
        {
            try
            {
                // Validar que el numero de motor no este repetido
                if (await _repository.ExistsNumeroMotorAsync(createDto.NumeroMotor))
                {
                    return new ApiResponse<AutomovilResponseDto>
                    {
                        Success = false,
                        Message = "Ya existe un automovil con este numero de motor",
                        Errors = { "El numero de motor debe ser unico" }
                    };
                }

                // Validar que el numero de chasis no este repetido
                if (await _repository.ExistsNumeroChasisAsync(createDto.NumeroChasis))
                {
                    return new ApiResponse<AutomovilResponseDto>
                    {
                        Success = false,
                        Message = "Ya existe un automovil con este numero de chasis",
                        Errors = { "El numero de chasis debe ser unico" }
                    };
                }

                // Convertimos el DTO de creacion -> Entidad Automovil
                var automovil = _mapper.Map<Automovil>(createDto);

                // Lo guardamos en la base de datos
                var createdAutomovil = await _repository.CreateAsync(automovil);

                // Convertimos la Entidad guardada -> DTO de respuesta
                var automovilDto = _mapper.Map<AutomovilResponseDto>(createdAutomovil);

                return new ApiResponse<AutomovilResponseDto>
                {
                    Success = true,
                    Message = "Automovil creado exitosamente",
                    Data = automovilDto
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear automovil");
                return new ApiResponse<AutomovilResponseDto>
                {
                    Success = false,
                    Message = "Error interno del servidor",
                    Errors = { ex.Message }
                };
            }
        }

        
        // MeTODO: Actualizar un auto
        
        public async Task<ApiResponse<AutomovilResponseDto>> UpdateAsync(int id, AutomovilUpdateDto updateDto)
        {
            try
            {
                // Validamos que exista un auto con ese ID
                if (!await _repository.ExistsAsync(id))
                {
                    return new ApiResponse<AutomovilResponseDto>
                    {
                        Success = false,
                        Message = $"No se encontro un automovil con ID {id}"
                    };
                }

                // Validamos que no se repita el numero de motor si lo esta cambiando
                if (!string.IsNullOrEmpty(updateDto.NumeroMotor) &&
                    await _repository.ExistsNumeroMotorAsync(updateDto.NumeroMotor, id))
                {
                    return new ApiResponse<AutomovilResponseDto>
                    {
                        Success = false,
                        Message = "Ya existe un automovil con este numero de motor",
                        Errors = { "El numero de motor debe ser unico" }
                    };
                }

                // Convertimos el DTO -> Entidad Automovil
                var automovilToUpdate = _mapper.Map<Automovil>(updateDto);

                // Usamos el repositorio para actualizar en la base de datos
                var updatedAutomovil = await _repository.UpdateAsync(id, automovilToUpdate);

                if (updatedAutomovil == null)
                {
                    return new ApiResponse<AutomovilResponseDto>
                    {
                        Success = false,
                        Message = "Error al actualizar el automovil"
                    };
                }

                var automovilDto = _mapper.Map<AutomovilResponseDto>(updatedAutomovil);
                return new ApiResponse<AutomovilResponseDto>
                {
                    Success = true,
                    Message = "Automovil actualizado exitosamente",
                    Data = automovilDto
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar automovil con ID {Id}", id);
                return new ApiResponse<AutomovilResponseDto>
                {
                    Success = false,
                    Message = "Error interno del servidor",
                    Errors = { ex.Message }
                };
            }
        }

        
        // MeTODO: Eliminar un auto
        
        public async Task<ApiResponse<string>> DeleteAsync(int id)
        {
            try
            {
                var deleted = await _repository.DeleteAsync(id);
                if (!deleted)
                {
                    return new ApiResponse<string>
                    {
                        Success = false,
                        Message = $"No se encontro un automovil con ID {id}"
                    };
                }

                return new ApiResponse<string>
                {
                    Success = true,
                    Message = "Automovil eliminado exitosamente",
                    Data = "Eliminacion confirmada"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar automovil con ID {Id}", id);
                return new ApiResponse<string>
                {
                    Success = false,
                    Message = "Error interno del servidor",
                    Errors = { ex.Message }
                };
            }
        }
    }
}*/

/*using AutomovilAPI.Domain.DTOs;
using AutomovilAPI.Domain.Entities;
using AutomovilAPI.Infrastructure.Repositories;
using AutoMapper;

namespace AutomovilAPI.Application.Services
{
    public interface IAutomovilService
    {
        Task<ApiResponse<IEnumerable<AutomovilResponseDto>>> GetAllAsync();
        Task<ApiResponse<AutomovilResponseDto>> GetByIdAsync(int id);
        Task<ApiResponse<AutomovilResponseDto>> GetByChasisAsync(string numeroChasis);
        Task<ApiResponse<AutomovilResponseDto>> CreateAsync(AutomovilCreateDto createDto);
        Task<ApiResponse<AutomovilResponseDto>> UpdateAsync(int id, AutomovilUpdateDto updateDto);
        Task<ApiResponse<string>> DeleteAsync(int id);
    }

    public class AutomovilService : IAutomovilService
    {
        private readonly IAutomovilRepository _repository;
        private readonly IMapper _mapper;
        private readonly ILogger<AutomovilService> _logger;

        public AutomovilService(
            IAutomovilRepository repository,
            IMapper mapper,
            ILogger<AutomovilService> logger)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ApiResponse<IEnumerable<AutomovilResponseDto>>> GetAllAsync()
        {
            try
            {
                var automoviles = await _repository.GetAllAsync();
                var automovilesDto = _mapper.Map<IEnumerable<AutomovilResponseDto>>(automoviles);

                return new ApiResponse<IEnumerable<AutomovilResponseDto>>
                {
                    Success = true,
                    Message = "Automoviles obtenidos exitosamente",
                    Data = automovilesDto
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los automoviles");
                return new ApiResponse<IEnumerable<AutomovilResponseDto>>
                {
                    Success = false,
                    Message = "Error interno del servidor",
                    Errors = { ex.Message }
                };
            }
        }

        public async Task<ApiResponse<AutomovilResponseDto>> GetByIdAsync(int id)
        {
            try
            {
                var automovil = await _repository.GetByIdAsync(id);
                if (automovil == null)
                {
                    return new ApiResponse<AutomovilResponseDto>
                    {
                        Success = false,
                        Message = $"No se encontro un automovil con ID {id}"
                    };
                }

                var automovilDto = _mapper.Map<AutomovilResponseDto>(automovil);
                return new ApiResponse<AutomovilResponseDto>
                {
                    Success = true,
                    Message = "Automovil obtenido exitosamente",
                    Data = automovilDto
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener automovil por ID {Id}", id);
                return new ApiResponse<AutomovilResponseDto>
                {
                    Success = false,
                    Message = "Error interno del servidor",
                    Errors = { ex.Message }
                };
            }
        }

        public async Task<ApiResponse<AutomovilResponseDto>> GetByChasisAsync(string numeroChasis)
        {
            try
            {
                var automovil = await _repository.GetByChasisAsync(numeroChasis);
                if (automovil == null)
                {
                    return new ApiResponse<AutomovilResponseDto>
                    {
                        Success = false,
                        Message = $"No se encontro un automovil con numero de chasis {numeroChasis}"
                    };
                }

                var automovilDto = _mapper.Map<AutomovilResponseDto>(automovil);
                return new ApiResponse<AutomovilResponseDto>
                {
                    Success = true,
                    Message = "Automovil obtenido exitosamente",
                    Data = automovilDto
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener automovil por chasis {NumeroChasis}", numeroChasis);
                return new ApiResponse<AutomovilResponseDto>
                {
                    Success = false,
                    Message = "Error interno del servidor",
                    Errors = { ex.Message }
                };
            }
        }

        public async Task<ApiResponse<AutomovilResponseDto>> CreateAsync(AutomovilCreateDto createDto)
        {
            try
            {
                // Validar unicidad del numero de motor
                if (await _repository.ExistsNumeroMotorAsync(createDto.NumeroMotor))
                {
                    return new ApiResponse<AutomovilResponseDto>
                    {
                        Success = false,
                        Message = "Ya existe un automovil con este numero de motor",
                        Errors = { "El numero de motor debe ser unico" }
                    };
                }

                // Validar unicidad del numero de chasis
                if (await _repository.ExistsNumeroChasisAsync(createDto.NumeroChasis))
                {
                    return new ApiResponse<AutomovilResponseDto>
                    {
                        Success = false,
                        Message = "Ya existe un automovil con este numero de chasis",
                        Errors = { "El numero de chasis debe ser unico" }
                    };
                }

                var automovil = _mapper.Map<Automovil>(createDto);
                var createdAutomovil = await _repository.CreateAsync(automovil);
                var automovilDto = _mapper.Map<AutomovilResponseDto>(createdAutomovil);

                return new ApiResponse<AutomovilResponseDto>
                {
                    Success = true,
                    Message = "Automovil creado exitosamente",
                    Data = automovilDto
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear automovil");
                return new ApiResponse<AutomovilResponseDto>
                {
                    Success = false,
                    Message = "Error interno del servidor",
                    Errors = { ex.Message }
                };
            }
        }

        public async Task<ApiResponse<AutomovilResponseDto>> UpdateAsync(int id, AutomovilUpdateDto updateDto)
        {
            try
            {
                if (!await _repository.ExistsAsync(id))
                {
                    return new ApiResponse<AutomovilResponseDto>
                    {
                        Success = false,
                        Message = $"No se encontro un automovil con ID {id}"
                    };
                }

                // Validar unicidad del numero de motor si se esta actualizando
                if (!string.IsNullOrEmpty(updateDto.NumeroMotor) &&
                    await _repository.ExistsNumeroMotorAsync(updateDto.NumeroMotor, id))
                {
                    return new ApiResponse<AutomovilResponseDto>
                    {
                        Success = false,
                        Message = "Ya existe un automovil con este numero de motor",
                        Errors = { "El numero de motor debe ser unico" }
                    };
                }

                var automovilToUpdate = _mapper.Map<Automovil>(updateDto);
                var updatedAutomovil = await _repository.UpdateAsync(id, automovilToUpdate);

                if (updatedAutomovil == null)
                {
                    return new ApiResponse<AutomovilResponseDto>
                    {
                        Success = false,
                        Message = "Error al actualizar el automovil"
                    };
                }

                var automovilDto = _mapper.Map<AutomovilResponseDto>(updatedAutomovil);
                return new ApiResponse<AutomovilResponseDto>
                {
                    Success = true,
                    Message = "Automovil actualizado exitosamente",
                    Data = automovilDto
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar automovil con ID {Id}", id);
                return new ApiResponse<AutomovilResponseDto>
                {
                    Success = false,
                    Message = "Error interno del servidor",
                    Errors = { ex.Message }
                };
            }
        }

        public async Task<ApiResponse<string>> DeleteAsync(int id)
        {
            try
            {
                var deleted = await _repository.DeleteAsync(id);
                if (!deleted)
                {
                    return new ApiResponse<string>
                    {
                        Success = false,
                        Message = $"No se encontro un automovil con ID {id}"
                    };
                }

                return new ApiResponse<string>
                {
                    Success = true,
                    Message = "Automovil eliminado exitosamente",
                    Data = "Eliminacion confirmada"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar automovil con ID {Id}", id);
                return new ApiResponse<string>
                {
                    Success = false,
                    Message = "Error interno del servidor",
                    Errors = { ex.Message }
                };
            }
        }
    }
}*/