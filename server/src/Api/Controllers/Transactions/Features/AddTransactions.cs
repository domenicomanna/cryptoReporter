using Api.Common.Attributes;
using Api.Common.ExtensionMethods;
using Api.Database;
using Api.Domain.Models;
using Api.Services;
using AutoMapper;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers.Transactions.Common.Features;

public class SingleTransaction
{
    public DateOnly Date { get; set; }
    public string CryptoTicker { get; set; } = string.Empty;
    public decimal QuantityTransacted { get; set; }
    public decimal Price { get; set; }
    public decimal Fee { get; set; }
    public string TransactionType { get; set; } = string.Empty;
    public string? Exchange { get; set; }
    public decimal NumberOfCoinsSold { get; set; }
    public string? Notes { get; set; }
}

public class SingleTransactionValidator : AbstractValidator<SingleTransaction>
{
    public SingleTransactionValidator()
    {
        List<string> validTransactionTypes = Enum.GetValues<TransactionTypeId>()
            .Select(x => x.GetDescription().ToLower())
            .ToList();

        RuleFor(x => x.Date).NotEmpty();
        RuleFor(x => x.CryptoTicker).NotEmpty();
        RuleFor(x => x.QuantityTransacted).NotNull();
        RuleFor(x => x.Price).NotNull();
        RuleFor(x => x.Fee).NotNull();
        RuleFor(x => x.TransactionType)
            .NotEmpty()
            .Must(x => validTransactionTypes.Contains(x.ToLower()))
            .WithMessage($"Transaction type must be one of {string.Join(", ", validTransactionTypes)}");
    }
}

public class AddTransactionsRequest
{
    public List<SingleTransaction> Transactions { get; set; } = new List<SingleTransaction>();
    public bool DeleteExistingTransactions { get; set; } = false;
}

public class AddTransactionsRequestValidator : AbstractValidator<AddTransactionsRequest>
{
    public AddTransactionsRequestValidator()
    {
        RuleForEach(x => x.Transactions).SetValidator(new SingleTransactionValidator());
    }
}

[Inject]
public class AddTransactionsHandler
{
    IMapper _mapper;
    AppDbContext _appDbContext;
    ICurrentUserAccessor _currentUserAccessor;

    public AddTransactionsHandler(IMapper mapper, AppDbContext appDbContext, ICurrentUserAccessor currentUserAccessor)
    {
        _mapper = mapper;
        _appDbContext = appDbContext;
        _currentUserAccessor = currentUserAccessor;
    }

    public async Task<List<TransactionDTO>> Handle(AddTransactionsRequest request)
    {
        using var transaction = await _appDbContext.Database.BeginTransactionAsync();

        if (request.DeleteExistingTransactions)
        {
            await _appDbContext.Transactions.ExecuteDeleteAsync();
        }
        List<Transaction> transactions = await CreateTransactions(request);

        await transaction.CommitAsync();

        return _mapper.Map<List<TransactionDTO>>(transactions);
    }

    private async Task<List<Transaction>> CreateTransactions(AddTransactionsRequest request)
    {
        int currentUserId = _currentUserAccessor.GetCurrentUserId();
        User user = await _appDbContext.Users.FirstAsync(x => x.Id == currentUserId);

        List<TransactionType> transactionTypes = _appDbContext.TransactionTypes.ToList();
        List<Transaction> transactions = request.Transactions
            .Select(
                x =>
                    new Transaction
                    {
                        Date = x.Date,
                        CryptoTicker = x.CryptoTicker.ToUpper(),
                        QuantityTransacted = x.QuantityTransacted,
                        Price = x.Price,
                        Fee = x.Fee,
                        TransactionType = transactionTypes.First(
                            transactionType => transactionType.Name.ToLower() == x.TransactionType.ToLower()
                        ),
                        Exchange = x.Exchange,
                        NumberOfCoinsSold = x.NumberOfCoinsSold,
                        Notes = x.Notes,
                        User = user,
                    }
            )
            .ToList();
        _appDbContext.Transactions.AddRange(transactions);
        await _appDbContext.SaveChangesAsync();

        return transactions;
    }
}
