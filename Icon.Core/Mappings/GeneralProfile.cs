using AutoMapper;
using Icon.Core.DTOs.UserDTOs;
using Icon.Core.Models;

namespace AIcon.Core.Mappings
{
    public class GeneralProfile : Profile
    {
        public GeneralProfile()
        {
            CreateMap<UserInputDTO, ApplicationUser>();
            CreateMap<ApplicationUser, UserInputDTO>();
        }
    }
}
