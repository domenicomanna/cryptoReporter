using Api.Common.Attributes;
using Api.Common.ExtensionMethods.ValidationRules;
using Api.Common.Models;
using Api.Database;
using Api.Domain.Models;
using Api.Utils;
using AutoMapper;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;

namespace Api.Controllers.Transactions.Common.Features;

public class GetTransactionsRequest
{
    public int PageIndex { get; set; } = 0;
    public int PageSize { get; set; } = 100;
    public string CryptoTickers { get; set; } = string.Empty;
    public string TransactionTypes { get; set; } = string.Empty;
    public string SortBy { get; set; } = "Date asc";
}

public class GetTransactionsRequestValidator : AbstractValidator<GetTransactionsRequest>
{
    public GetTransactionsRequestValidator()
    {
        RuleFor(x => x.SortBy).SortMustBeValid(typeof(TransactionDTO));
    }
}

[Inject]
public class GetTransactionsHandler
{
    IMapper _mapper;
    AppDbContext _appDbContext;
    ICurrentUserAccessor _currentUserAccessor;

    public GetTransactionsHandler(IMapper mapper, AppDbContext appDbContext, ICurrentUserAccessor currentUserAccessor)
    {
        _mapper = mapper;
        _appDbContext = appDbContext;
        _currentUserAccessor = currentUserAccessor;
    }

    public async Task<PaginationResult<TransactionDTO>> Handle(GetTransactionsRequest request)
    {
        IQueryable<Transaction> query = BuildQuery(request);

        int recordCount = await query.CountAsync();

        List<Transaction> transactions = await query
            .Skip(request.PageIndex * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync();

        PaginationResult<TransactionDTO> paginationResult = new PaginationResult<TransactionDTO>
        {
            Records = _mapper.Map<List<TransactionDTO>>(transactions),
            TotalRecordCount = recordCount,
            CurrentPageIndex = request.PageIndex,
            PageSize = request.PageSize,
        };

        return paginationResult;
    }

    private IQueryable<Transaction> BuildQuery(GetTransactionsRequest request)
    {
        int currentUserId = _currentUserAccessor.GetCurrentUserId();

        IQueryable<Transaction> query = _appDbContext.Transactions
            .Include(x => x.User)
            .Include(x => x.TransactionType)
            .Where(x => x.User.Id == currentUserId);

        if (!string.IsNullOrEmpty(request.CryptoTickers))
        {
            List<string> cryptoTickers = CsvStringToListConverter.ConvertToList(
                request.CryptoTickers,
                LetterCase.Lower
            );
            query = query.Where(x => cryptoTickers.Contains(x.CryptoTicker.ToLower()));
        }

        if (!string.IsNullOrEmpty(request.TransactionTypes))
        {
            List<string> transactionTypes = CsvStringToListConverter.ConvertToList(
                request.TransactionTypes,
                LetterCase.Lower
            );
            query = query.Where(x => transactionTypes.Contains(x.TransactionType.Name.ToLower()));
        }

        if (!string.IsNullOrEmpty(request.SortBy))
        {
            string sortStatement = SortStatementGenerator.GenerateSortStatement(request.SortBy, typeof(TransactionDTO));
            query = query.OrderBy(sortStatement);
        }

        return query;
    }
}
