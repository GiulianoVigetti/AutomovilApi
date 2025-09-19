using AutoMapper;
using HybridDDDArchitecture.Core.Application.DataTransferObjects;
using HybridDDDArchitecture.Core.Domain.Entities;

namespace HybridDDDArchitecture.Core.Application.Mappings
{
    /// <summary>
    /// El mapeo entre objetos debe ir definido aquí
    /// </summary>
    public class AutomovilMappingProfile : Profile
    {
        public AutomovilMappingProfile()
        {
            // Mapeo de DTO a Entidad
            CreateMap<AutomovilCreateDto, Automovil>();
            CreateMap<AutomovilUpdateDto, Automovil>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // Mapeo de Entidad a DTO
            CreateMap<Automovil, AutomovilResponseDto>();
        }
    }
}
/*using Application.DataTransferObjects;
using Application.DomainEvents;
using AutoMapper;
using Domain.Entities;

namespace Application.Mappings
{
    /// <summary>
    /// El mapeo entre objetos debe ir definido aqui
    /// </summary>
    public class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<DummyEntity, DummyEntityCreated>().ReverseMap();
            CreateMap<DummyEntity, DummyEntityUpdated>().ReverseMap();
            CreateMap<DummyEntity, DummyEntityDto>().ReverseMap();
        }
    }
}*/
