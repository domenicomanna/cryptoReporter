using Api.Common.ExtensionMethods;
using Api.Controllers.Transactions;
using Api.Controllers.Transactions.Common;
using Api.Controllers.Transactions.Common.Features;
using Api.Database;
using Api.Domain.Models;
using Api.Utils;
using AutoMapper;
using Fixtures;
using FluentValidation.TestHelper;
using Moq;

namespace ApiTests.Controllers.Transactions.Features;

public class AddTransactionsTests : IClassFixture<DatabaseFixture>
{
    DatabaseFixture _databaseFixture;
    IMapper _mapper = null!;
    Mock<ICurrentUserAccessor> _currentUserAccessorMock = null!;

    public AddTransactionsTests(DatabaseFixture databaseFixture)
    {
        _databaseFixture = databaseFixture;
        MapperConfiguration mapperConfiguration = new MapperConfiguration(opts =>
        {
            opts.AddProfile(new TransactionsMappingProfile());
        });
        _mapper = mapperConfiguration.CreateMapper();
        _currentUserAccessorMock = new Mock<ICurrentUserAccessor>();
    }

    [Fact]
    public void ThereShouldBeAValidationErrorIfTheTransactionTypeIsInvalid()
    {
        SingleTransactionValidator validator = new SingleTransactionValidator();
        SingleTransaction transaction = new SingleTransaction
        {
            Date = DateOnly.FromDateTime(DateTime.Now),
            TransactionType = "test 1111",
        };
        var result = validator.TestValidate(transaction);
        result.ShouldHaveValidationErrorFor(x => x.TransactionType);
    }

    [Fact]
    public async Task TheTransactionsShouldBeAddedSuccessfully()
    {
        AppDbContext appDbContext = await _databaseFixture.CreateContext();
        User user = new User() { FiatCurrencyType = appDbContext.FiatCurrencyTypes.First() };
        appDbContext.Users.Add(user);
        appDbContext.SaveChanges();

        _currentUserAccessorMock.Setup(x => x.GetCurrentUserId()).Returns(user.Id);

        AddTransactionsHandler handler = new AddTransactionsHandler(
            _mapper,
            appDbContext,
            _currentUserAccessorMock.Object
        );
        AddTransactionsRequest request = new AddTransactionsRequest
        {
            DeleteExistingTransactions = true,
            Transactions = new List<SingleTransaction>()
            {
                new SingleTransaction
                {
                    Date = DateOnly.FromDateTime(DateTime.Now),
                    CryptoTicker = "BTC",
                    QuantityTransacted = 100,
                    Price = 1,
                    Fee = 0,
                    TransactionType = TransactionTypeId.Purchase.GetDescription(),
                    Exchange = "Coinbase",
                    NumberOfCoinsSold = 0,
                    Notes = "First purchase",
                },
                new SingleTransaction
                {
                    Date = DateOnly.FromDateTime(DateTime.Now),
                    CryptoTicker = "BTC",
                    QuantityTransacted = 200,
                    Price = 1,
                    Fee = 0,
                    TransactionType = TransactionTypeId.Purchase.GetDescription(),
                    Exchange = "Coinbase",
                    NumberOfCoinsSold = 0,
                    Notes = "First purchase",
                },
            },
        };

        List<TransactionDTO> createdTransactions = await handler.Handle(request);
        Assert.Equal(request.Transactions.Count, createdTransactions.Count);
        Assert.Equal(request.Transactions.Count, appDbContext.Transactions.Count());
    }
}
