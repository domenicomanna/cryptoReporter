using Api.Apis.CoinMarketCap.QuotesLatest.Models;

namespace Api.Apis.CoinMarketCap.QuotesLatest;

public interface IQuotesLatestApiClient
{
    public Task<QuotesLatestResponse> GetQuotes(List<string> cryptoTickers, string currency);
}

public class QuotesLatestApiClient : IQuotesLatestApiClient
{
    private readonly ICoinMarketCapHttpClient _httpClient;

    public QuotesLatestApiClient(ICoinMarketCapHttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<QuotesLatestResponse> GetQuotes(List<string> cryptoTickers, string currency)
    {
        HttpRequestMessage httpRequest =
            new()
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(
                    $"v2/cryptocurrency/quotes/latest?symbol={string.Join(",", cryptoTickers)}&convert={currency}",
                    UriKind.Relative
                )
            };
        HttpResponseMessage response = await _httpClient.SendAsync(httpRequest);
        QuotesLatestResponse? quotesLatestResponse = await response.Content.ReadFromJsonAsync<QuotesLatestResponse>();
        return quotesLatestResponse!;
    }
}
