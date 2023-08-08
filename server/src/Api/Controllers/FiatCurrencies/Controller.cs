using Api.Controllers.FiatCurrencies.Common;
using Api.Controllers.FiatCurrencies.Common.Features;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.FiatCurrencies;

public class FiatCurrenciesController : BaseApiController
{
    [HttpGet("")]
    [AllowAnonymous]
    public async Task<List<FiatCurrencyTypeDTO>> GetFiatCurrencies([FromServices] GetFiatCurrencyTypesHandler handler)
    {
        return await handler.Handle();
    }
}
