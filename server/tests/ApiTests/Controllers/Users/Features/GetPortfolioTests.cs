using Moq;
using Api.Database;
using Api.Utils;
using Api.Apis.CoinMarketCap;
using Api.Domain.Models;
using Api.Controllers.Users.Common.Features;
using Api.Apis.CoinMarketCap.QuotesLatest.Models;
using Fixtures;
using Api.Common.ExtensionMethods;

namespace ApiTests.Controllers.Features.Users;

public class GetPortfolioTests : IClassFixture<DatabaseFixture>
{
    DatabaseFixture _databaseFixture;
    Mock<ICurrentUserAccessor> _currentUserAccessorMock;
    Mock<ICoinMarketCapApiClient> _coinMarketCapApiClientMock;

    public GetPortfolioTests(DatabaseFixture databaseFixture)
    {
        _databaseFixture = databaseFixture;
        _currentUserAccessorMock = new Mock<ICurrentUserAccessor>();
        _coinMarketCapApiClientMock = new Mock<ICoinMarketCapApiClient>();
    }

    [Fact]
    public async Task PortfolioHoldingsShouldBeCorrect()
    {
        AppDbContext appDbContext = await _databaseFixture.CreateContext();
        User user =
            new()
            {
                FiatCurrencyType = appDbContext.FiatCurrencyTypes
                    .Where(x => x.Name == FiatCurrencyTypeId.USD.GetDescription())
                    .First(),
            };
        appDbContext.Users.Add(user);

        appDbContext.Transactions.AddRange(
            new List<Transaction>
            {
                new()
                {
                    Date = new DateOnly(2022, 1, 1),
                    CryptoTicker = "BTC",
                    QuantityTransacted = 1000,
                    Price = 10000,
                    Fee = 10,
                    TransactionType = appDbContext.TransactionTypes
                        .Where(x => x.Id == TransactionTypeId.Purchase)
                        .First(),
                    User = user,
                    NumberOfCoinsSold = .02m
                },
                new()
                {
                    Date = new DateOnly(2022, 1, 1),
                    CryptoTicker = "BTC",
                    QuantityTransacted = 1000,
                    Price = 20000,
                    Fee = 10,
                    TransactionType = appDbContext.TransactionTypes
                        .Where(x => x.Id == TransactionTypeId.Purchase)
                        .First(),
                    User = user,
                    NumberOfCoinsSold = .01m
                },
                new()
                {
                    Date = new DateOnly(2022, 1, 1),
                    CryptoTicker = "ETH",
                    QuantityTransacted = 500,
                    Price = 1000,
                    Fee = 5,
                    TransactionType = appDbContext.TransactionTypes
                        .Where(x => x.Id == TransactionTypeId.Purchase)
                        .First(),
                    User = user,
                    NumberOfCoinsSold = .2m
                },
                new()
                {
                    Date = new DateOnly(2022, 2, 1),
                    CryptoTicker = "ETH",
                    QuantityTransacted = 1000,
                    Price = 1000,
                    Fee = 10,
                    TransactionType = appDbContext.TransactionTypes
                        .Where(x => x.Id == TransactionTypeId.Purchase)
                        .First(),
                    User = user,
                    NumberOfCoinsSold = .1m
                },
            }
        );
        appDbContext.SaveChanges();

        _currentUserAccessorMock.Setup(x => x.GetCurrentUserId()).Returns(user.Id);

        decimal bitcoinCurrentUsdPrice = 1405m;
        _coinMarketCapApiClientMock
            .Setup(x => x.QuotesLatest.GetQuotes(It.IsAny<List<string>>(), It.IsAny<string>()))
            .ReturnsAsync(
                new QuotesLatestResponse
                {
                    Data = new Dictionary<string, List<CryptoCurrency>>()
                    {
                        {
                            "BTC",
                            new List<CryptoCurrency>
                            {
                                new()
                                {
                                    Quote = new Dictionary<string, QuoteInfo>
                                    {
                                        {
                                            user.FiatCurrencyType.Name,
                                            new() { Price = bitcoinCurrentUsdPrice }
                                        }
                                    }
                                }
                            }
                        },
                    }
                }
            );

        GetPortfolioHandler handler = new GetPortfolioHandler(
            appDbContext,
            _currentUserAccessorMock.Object,
            _coinMarketCapApiClientMock.Object
        );
        Portfolio portfolio = await handler.Handle(user.Id);

        var btcHolding = portfolio.Holdings.First(x => x.CryptoTicker.ToLower() == "btc");
        var ethHolding = portfolio.Holdings.First(x => x.CryptoTicker.ToLower() == "eth");

        Assert.Equal(.1185m, btcHolding.TotalCoinsOwned, 4);
        Assert.Equal(1595.96m, btcHolding.TotalInvestedIncludingFees, 2);
        Assert.Equal(13468.01m, btcHolding.CostBasis, 2);
        Assert.Equal(btcHolding.TotalCoinsOwned * bitcoinCurrentUsdPrice, btcHolding.CurrentValue ?? 0, 2);
        Assert.Equal(.571m, btcHolding.PortfolioPercentage, 3);

        Assert.Equal(1196.97m, ethHolding.TotalInvestedIncludingFees, 2);
        Assert.Equal(1.185m, ethHolding.TotalCoinsOwned, 3);
        Assert.Equal(1010.10m, ethHolding.CostBasis, 2);
        // eth price is not provided in the above coin market cap api mock, so this should be null
        Assert.Null(ethHolding.CurrentValue);
        Assert.Equal(.429m, ethHolding.PortfolioPercentage, 3);
    }
}
