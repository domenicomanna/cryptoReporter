using Api.Apis.CoinMarketCap.QuotesLatest;

namespace Api.Apis.CoinMarketCap;

public interface ICoinMarketCapApiClient
{
    IQuotesLatestApiClient QuotesLatest { get; }
}

public class CoinMarketCapApiClient : ICoinMarketCapApiClient
{
    private readonly ICoinMarketCapHttpClient _coinMarketCapHttpClient;
    public IQuotesLatestApiClient QuotesLatest { get; }

    public CoinMarketCapApiClient(ICoinMarketCapHttpClient coinMarketCapHttpClient)
    {
        _coinMarketCapHttpClient = coinMarketCapHttpClient;
        QuotesLatest = new QuotesLatestApiClient(_coinMarketCapHttpClient);
    }
}
