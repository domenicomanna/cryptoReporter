namespace Api.Apis.CoinMarketCap.QuotesLatest.Models;

public class QuotesLatestResponse
{
    public Dictionary<string, List<CryptoCurrency>> Data { get; set; } = new();
}

public class CryptoCurrency
{
    public string Symbol { get; set; } = string.Empty;
    public Dictionary<string, QuoteInfo> Quote { get; set; } = new();
}

public class QuoteInfo
{
    public decimal? Price { get; set; }
}
