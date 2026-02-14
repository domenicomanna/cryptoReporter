using System.Linq.Dynamic.Core;
using Api.Common.Attributes;
using Api.Database;
using Api.Utils;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers.Transactions.Common.Features;

[Inject]
public class GetTransactedCryptosHandler
{
    AppDbContext _appDbContext;
    ICurrentUserAccessor _currentUserAccessor;

    public GetTransactedCryptosHandler(AppDbContext appDbContext, ICurrentUserAccessor currentUserAccessor)
    {
        _appDbContext = appDbContext;
        _currentUserAccessor = currentUserAccessor;
    }

    public async Task<List<string>> Handle()
    {
        int currentUserId = _currentUserAccessor.GetCurrentUserId();

        List<string> transactedCryptos = await _appDbContext
            .Transactions.Where(x => x.User.Id == currentUserId)
            .Select(x => x.CryptoTicker)
            .Distinct()
            .OrderBy(cryptoTicker => cryptoTicker)
            .ToListAsync();

        return transactedCryptos;
    }
}
