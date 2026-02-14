using Api.Controllers.Transactions;
using Api.Controllers.Transactions.Common.Features;
using Api.Database;
using Api.Domain.Models;
using Api.Utils;
using AutoMapper;
using Fixtures;
using Moq;

namespace ApiTests.Controllers.Transactions.Features;

public class GetTransactionsTests : IClassFixture<DatabaseFixture>
{
    IMapper _mapper = null!;
    Mock<ICurrentUserAccessor> _currentUserAccessorMock = null!;
    DatabaseFixture _databaseFixture;

    public GetTransactionsTests(DatabaseFixture databaseFixture)
    {
        MapperConfiguration mapperConfiguration = new MapperConfiguration(opts =>
        {
            opts.AddProfile(new TransactionsMappingProfile());
        });
        _mapper = mapperConfiguration.CreateMapper();
        _currentUserAccessorMock = new Mock<ICurrentUserAccessor>();
        _databaseFixture = databaseFixture;
    }

    [Fact]
    public async Task TransactionsShouldBeSuccessfullyRetrieved()
    {
        AppDbContext appDbContext = await _databaseFixture.CreateContext();
        User user = new User() { FiatCurrencyType = appDbContext.FiatCurrencyTypes.First() };
        appDbContext.Users.Add(user);

        appDbContext.Transactions.AddRange(
            new List<Transaction>
            {
                new Transaction()
                {
                    Date = new DateOnly(2022, 1, 1),
                    CryptoTicker = "BTC",
                    QuantityTransacted = 100,
                    Price = 10000,
                    Fee = 1,
                    TransactionType = appDbContext
                        .TransactionTypes.Where(x => x.Id == TransactionTypeId.Purchase)
                        .First(),
                    User = user,
                },
                new Transaction()
                {
                    Date = new DateOnly(2022, 1, 1),
                    CryptoTicker = "ETH",
                    QuantityTransacted = 100,
                    Price = 1000,
                    Fee = 1,
                    TransactionType = appDbContext.TransactionTypes.Where(x => x.Id == TransactionTypeId.Sale).First(),
                    User = user,
                },
            }
        );
        appDbContext.SaveChanges();

        _currentUserAccessorMock.Setup(x => x.GetCurrentUserId()).Returns(user.Id);

        GetTransactionsHandler handler = new GetTransactionsHandler(
            _mapper,
            appDbContext,
            _currentUserAccessorMock.Object
        );
        GetTransactionsRequest request = new GetTransactionsRequest
        {
            CryptoTickers = "BTC,ETH",
            TransactionTypes = "purchase, sale",
            SortBy = "Date asc, TransactionType desc",
        };

        var paginationResult = await handler.Handle(request);

        Assert.Equal(2, paginationResult.Records.Count);
    }
}
