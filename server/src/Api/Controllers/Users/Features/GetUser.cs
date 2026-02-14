using System.Net;
using Api.Common.Attributes;
using Api.Common.Exceptions;
using Api.Database;
using Api.Domain.Models;
using Api.Utils;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers.Users.Common.Features;

[Inject]
public class GetUserHandler
{
    IMapper _mapper;
    ICurrentUserAccessor _currentUserAccessor;
    AppDbContext _appDbContext;

    public GetUserHandler(IMapper mapper, ICurrentUserAccessor currentUserAccessor, AppDbContext appDbContext)
    {
        _mapper = mapper;
        _currentUserAccessor = currentUserAccessor;
        _appDbContext = appDbContext;
    }

    public async Task<UserDTO> Handle(int userId)
    {
        int currentUserId = _currentUserAccessor.GetCurrentUserId();
        if (currentUserId != userId)
        {
            throw new ApiException(HttpStatusCode.Forbidden);
        }

        User? user = await _appDbContext
            .Users.Include(x => x.FiatCurrencyType)
            .FirstOrDefaultAsync(x => x.Id == currentUserId);
        if (user is null)
        {
            throw new ApiException(HttpStatusCode.NotFound);
        }

        return _mapper.Map<UserDTO>(user);
    }
}
