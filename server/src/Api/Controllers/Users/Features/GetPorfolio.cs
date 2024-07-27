using Api.Common.Attributes;
using Api.Database;
using Api.Utils;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
using Api.Domain.Models;
using Api.Common.Exceptions;
using System.Net;
using Api.Apis.CoinMarketCap;
using Api.Apis.CoinMarketCap.QuotesLatest.Models;

namespace Api.Controllers.Users.Common.Features;

public class InitialAggregatedAsset
{
    public string CryptoTicker { get; set; } = string.Empty;
    public decimal CoinsOwned { get; set; }
    public decimal InvestmentAmountIncludingFee { get; set; }
}

public class AggregatedAsset
{
    public string CryptoTicker { get; set; } = string.Empty;
    public decimal? CurrentPrice { get; set; }
    public decimal TotalCoinsOwned { get; set; }
    public decimal TotalInvestedIncludingFees { get; set; }
    public decimal? CurrentValue { get; set; }
    public decimal PortfolioPercentage { get; set; }
    public decimal CostBasis { get; set; }
}

public class Portfolio
{
    public List<AggregatedAsset> Holdings { get; set; } = new();
    public decimal TotalInvested { get; set; }
    public decimal CurrentValue { get; set; }
}

[Inject]
public class GetPortfolioHandler
{
    AppDbContext _appDbContext;
    ICurrentUserAccessor _currentUserAccessor;
    ICoinMarketCapApiClient _coinMarketCapApiClient;

    public GetPortfolioHandler(
        AppDbContext appDbContext,
        ICurrentUserAccessor currentUserAccessor,
        ICoinMarketCapApiClient coinMarketCapApiClient
    )
    {
        _appDbContext = appDbContext;
        _currentUserAccessor = currentUserAccessor;
        _coinMarketCapApiClient = coinMarketCapApiClient;
    }

    public async Task<Portfolio> Handle(int userId)
    {
        int currentUserId = _currentUserAccessor.GetCurrentUserId();
        if (currentUserId != userId)
        {
            throw new ApiException(HttpStatusCode.Forbidden);
        }

        User user = await _appDbContext.Users.Include(x => x.FiatCurrencyType).FirstAsync(x => x.Id == currentUserId);
        List<InitialAggregatedAsset> aggregatedAssets = await AggregateAssets(userId);
        decimal totalInvested = aggregatedAssets.Sum(x => x.InvestmentAmountIncludingFee);
        List<string> tickers = aggregatedAssets.Select(x => x.CryptoTicker).ToList();
        QuotesLatestResponse quotesResponse = await _coinMarketCapApiClient.QuotesLatest.GetQuotes(
            tickers,
            user.FiatCurrencyType.Name
        );

        List<AggregatedAsset> holdings = aggregatedAssets
            .Select(x =>
            {
                CryptoCurrency? crypto = quotesResponse.Data
                    .GetValueOrDefault(x.CryptoTicker.ToUpper())
                    ?.FirstOrDefault();
                QuoteInfo? quote = crypto?.Quote?.GetValueOrDefault(user.FiatCurrencyType.Name);

                return new AggregatedAsset
                {
                    CryptoTicker = x.CryptoTicker,
                    CurrentPrice = quote?.Price,
                    TotalCoinsOwned = x.CoinsOwned,
                    TotalInvestedIncludingFees = x.InvestmentAmountIncludingFee,
                    CostBasis = x.InvestmentAmountIncludingFee / x.CoinsOwned,
                    PortfolioPercentage = x.InvestmentAmountIncludingFee / totalInvested,
                    CurrentValue = quote is null || quote.Price is null ? null : (decimal)quote.Price * x.CoinsOwned
                };
            })
            .OrderByDescending(x => x.PortfolioPercentage)
            .ToList();

        return new Portfolio()
        {
            TotalInvested = totalInvested,
            CurrentValue = holdings.Sum(x => x.CurrentValue ?? 0),
            Holdings = holdings
        };
    }

    public async Task<List<InitialAggregatedAsset>> AggregateAssets(int userId)
    {
        return await _appDbContext.Transactions
            .Include(x => x.TransactionType)
            .Where(
                x =>
                    x.User.Id == userId
                    && (
                        x.TransactionType.Id == TransactionTypeId.Purchase
                        || x.TransactionType.Id == TransactionTypeId.Reward
                    )
            )
            .GroupBy(x => x.CryptoTicker)
            .Select(
                group =>
                    new InitialAggregatedAsset()
                    {
                        CryptoTicker = group.Key,
                        // round to ensure the calculated values don't exceed the maximum number of digits allowed in a decimal
                        CoinsOwned = Math.Round(
                            group.Sum(
                                transaction =>
                                    ((transaction.QuantityTransacted - transaction.Fee) / transaction.Price) // coins acquired from this transaction
                                    - transaction.NumberOfCoinsSold
                            ),
                            6
                        ),
                        InvestmentAmountIncludingFee = Math.Round(
                            group.Sum(
                                transaction =>
                                    transaction.QuantityTransacted
                                    - (
                                        transaction.NumberOfCoinsSold
                                        *
                                        // price per coin
                                        (
                                            transaction.QuantityTransacted
                                            / ((transaction.QuantityTransacted - transaction.Fee) / transaction.Price)
                                        )
                                    )
                            ),
                            6
                        )
                    }
            )
            .ToListAsync();
    }
}
