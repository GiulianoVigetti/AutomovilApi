/*using HybridDDDArchitecture.Core.Application.DataTransferObjects;
using HybridDDDArchitecture.Core.Application.Wrappers;
using System.Collections.Generic;
using System.Threading.Tasks;

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
}*/
/*using System.Collections.Generic;
using System.Threading.Tasks;
//using Application.DTOs;
using AutomovilAPI.Application.DTOs;

//namespace Application.Interfaces
namespace AutomovilAPI.Application.Interfaces
{
    public interface IAutomovilService
    {
        Task<AutomovilResponseDTO> CreateAsync(AutomovilDTO dto);
        Task<AutomovilResponseDTO> UpdateAsync(int id, AutomovilDTO dto);
        Task<bool> DeleteAsync(int id);
        Task<AutomovilResponseDTO> GetByIdAsync(int id);
        Task<AutomovilResponseDTO> GetByChasisAsync(string numeroChasis);
        Task<IEnumerable<AutomovilResponseDTO>> GetAllAsync();
    }
}*/