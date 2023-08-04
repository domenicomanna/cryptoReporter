using Api.Common.ExtensionMethods;
using Api.Controllers.Transactions;
using Api.Controllers.Transactions.Common;
using Api.Controllers.Transactions.Common.Features;
using Api.Database;
using Api.Domain.Models;
using AutoMapper;
using FluentValidation.TestHelper;

namespace ApiTests.Controllers.Transactions.Features;

[TestClass]
public class AddTransactionsTests
{
    IMapper _mapper = null!;
    AppDbContextCreator _appDbContextCreator = null!;

    [TestInitialize]
    public void SetUp()
    {
        MapperConfiguration mapperConfiguration = new MapperConfiguration(opts =>
        {
            opts.AddProfile(new TransactionsMappingProfile());
        });
        _mapper = mapperConfiguration.CreateMapper();

        _appDbContextCreator = new AppDbContextCreator();
    }

    [TestMethod]
    public void ThereShouldBeAValidationErrorIfTheCurrencyIsInvalid()
    {
        SingleTransactionValidator validator = new SingleTransactionValidator();
        SingleTransaction transaction = new SingleTransaction
        {
            Date = DateOnly.FromDateTime(DateTime.Now),
            PriceCurrency = "xyz xyz",
            TransactionType = TransactionTypeId.Purchase.GetDescription(),
        };
        var result = validator.TestValidate(transaction);
        result.ShouldHaveValidationErrorFor(x => x.PriceCurrency);
    }

    [TestMethod]
    public void ThereShouldBeAValidationErrorIfTheTransactionTypeIsInvalid()
    {
        SingleTransactionValidator validator = new SingleTransactionValidator();
        SingleTransaction transaction = new SingleTransaction
        {
            Date = DateOnly.FromDateTime(DateTime.Now),
            PriceCurrency = "EUR",
            TransactionType = "test 1111",
        };
        var result = validator.TestValidate(transaction);
        result.ShouldHaveValidationErrorFor(x => x.TransactionType);
    }

    [TestMethod]
    public async Task TheTransactionsShouldBeAddedSuccessfully()
    {
        AppDbContext appDbContext = _appDbContextCreator.CreateContext();
        appDbContext.Transactions.Add(new Transaction { TransactionTypeId = TransactionTypeId.Purchase });
        appDbContext.SaveChanges();

        AddTransactionsHandler handler = new AddTransactionsHandler(_mapper, appDbContext);
        AddTransactionsRequest request = new AddTransactionsRequest
        {
            DeleteExistingTransactions = true,
            Transactions = new List<SingleTransaction>()
            {
                new SingleTransaction
                {
                    Date = DateOnly.FromDateTime(DateTime.Now),
                    QuantityTransacted = 100,
                    Price = 1,
                    PriceCurrency = "USD",
                    Fee = 0,
                    TransactionType = TransactionTypeId.Purchase.GetDescription(),
                    Exchange = "Coinbase",
                    NumberOfCoinsSold = 0,
                    Notes = "First purchase"
                },
                new SingleTransaction
                {
                    Date = DateOnly.FromDateTime(DateTime.Now),
                    QuantityTransacted = 200,
                    Price = 1,
                    PriceCurrency = "USD",
                    Fee = 0,
                    TransactionType = TransactionTypeId.Purchase.GetDescription(),
                    Exchange = "Coinbase",
                    NumberOfCoinsSold = 0,
                    Notes = "First purchase"
                },
            },
        };

        List<TransactionDTO> createdTransactions = await handler.Handle(request);
        Assert.AreEqual(request.Transactions.Count, createdTransactions.Count);
        Assert.AreEqual(request.Transactions.Count, appDbContext.Transactions.Count());
    }
}
