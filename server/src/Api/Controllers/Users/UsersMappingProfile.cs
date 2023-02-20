using Api.Controllers.Users.Common;
using Api.Domain.Models;
using AutoMapper;

namespace Api.Controllers.Users;

public class UsersMappingProfile : Profile
{
    public UsersMappingProfile()
    {
        CreateMap<User, UserDTO>();
    }
}
