namespace Api.Apis.CoinMarketCap;

public interface ICoinMarketCapHttpClient
{
    Task<HttpResponseMessage> SendAsync(HttpRequestMessage request);
}

public class CoinMarketCapHttpClient : ICoinMarketCapHttpClient
{
    private readonly HttpClient _httpClient;

    public CoinMarketCapHttpClient(HttpClient httpClient, string apiKey)
    {
        _httpClient = httpClient;
        _httpClient.DefaultRequestHeaders.Add("X-CMC_PRO_API_KEY", apiKey);
        _httpClient.BaseAddress = new Uri("https://pro-api.coinmarketcap.com/");
    }

    public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request)
    {
        HttpResponseMessage response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
        return response;
    }
}
