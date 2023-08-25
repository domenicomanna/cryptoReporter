namespace Api.Controllers.Transactions.Common;

public class TransactionDTO
{
    public int Id { get; set; }
    public string CryptoTicker { get; set; } = string.Empty;
    public DateOnly Date { get; set; }
    public decimal QuantityTransacted { get; set; }
    public decimal Price { get; set; }
    public decimal Fee { get; set; }
    public decimal CoinsTransacted => (QuantityTransacted - Fee) / Price;
    public string TransactionType { get; set; } = String.Empty;
    public string? Exchange { get; set; }
    public decimal NumberOfCoinsSold { get; set; }
    public string? Notes { get; set; }
    public int UserId { get; set; }
}
