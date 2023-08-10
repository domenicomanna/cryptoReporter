namespace Api.Domain.Models;

public class Transaction
{
    public int Id { get; set; }
    public DateOnly Date { get; set; }
    public string CryptoTicker { get; set; } = string.Empty;
    public decimal QuantityTransacted { get; set; }
    public decimal Price { get; set; }
    public decimal Fee { get; set; }
    public TransactionTypeId TransactionTypeId { get; set; }
    public TransactionType TransactionType { get; set; } = null!;
    public string? Exchange { get; set; }
    public decimal NumberOfCoinsSold { get; set; }
    public string? Notes { get; set; }
    public User User { get; set; } = null!;
}
