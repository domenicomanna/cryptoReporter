using Api.Controllers.Transactions.Common;
using Api.Controllers.Transactions.Common.Features;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.Transactions;

public class TransactionsController : BaseApiController
{
    [HttpPost("batch")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<ActionResult<List<TransactionDTO>>> AddTransactions(
        [FromServices] AddTransactionsHandler handler,
        AddTransactionsRequest request
    )
    {
        List<TransactionDTO> transactions = await handler.Handle(request);
        return new ObjectResult(transactions) { StatusCode = 201 };
    }
}
