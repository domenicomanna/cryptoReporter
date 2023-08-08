using Api.Controllers.FiatCurrencies.Common;
using Api.Domain.Models;
using AutoMapper;

namespace Api.Controllers.FiatCurrencies;

public class FiatCurrencyTypesMappingProfile : Profile
{
    public FiatCurrencyTypesMappingProfile()
    {
        CreateMap<FiatCurrencyType, FiatCurrencyTypeDTO>();
    }
}
