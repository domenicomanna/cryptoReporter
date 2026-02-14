using Api.Common.Attributes;
using Api.Database;
using Api.Domain.Models;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers.FiatCurrencies.Common.Features;

[Inject]
public class GetFiatCurrencyTypesHandler
{
    IMapper _mapper;
    AppDbContext _appDbContext;

    public GetFiatCurrencyTypesHandler(IMapper mapper, AppDbContext appDbContext)
    {
        _mapper = mapper;
        _appDbContext = appDbContext;
    }

    public async Task<List<FiatCurrencyTypeDTO>> Handle()
    {
        List<FiatCurrencyType> fiatCurrencyTypes = await _appDbContext
            .FiatCurrencyTypes.OrderBy(x => x.Name.ToLower())
            .ToListAsync();

        return _mapper.Map<List<FiatCurrencyTypeDTO>>(fiatCurrencyTypes);
    }
}
