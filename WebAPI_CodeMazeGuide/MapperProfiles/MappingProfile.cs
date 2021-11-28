using AutoMapper;
using Entities.DTOs;
using Entities.Models;

namespace WebAPI_CodeMazeGuide.MapperProfiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Company, CompanyDTO>()
                .ForMember(cdto => cdto.FullAddress,
                    opt => opt.MapFrom(c => string.Join(' ', c.Address, c.Country)));
            CreateMap<CompanyForCreationDTO, Company>();
            CreateMap<CompanyForUpdatingDTO, Company>();

            CreateMap<Employee, EmployeeDTO>();
            CreateMap<EmployeeForCreationDTO, Employee>();
            CreateMap<EmployeeForUpdatingDTO, Employee>()
                .ReverseMap();
        }
    }
}
