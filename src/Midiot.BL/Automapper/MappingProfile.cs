using AutoMapper;
using Midiot.BL.Models.Auth;
using Midiot.BL.Models.UserProfile;
using Midiot.Data.Entities;

namespace Midiot.BL.Automapper;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<SignUpModel, UserEntity>()
            .ForMember(u => u.PasswordHash, opt => opt.Ignore());

        CreateMap<UserEntity, GetProfileModel>();
        CreateMap<UpdateProfileModel, UserEntity>();
    }
}
