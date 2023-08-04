using System.ComponentModel;

namespace Api.Domain.Models;

public enum TransactionTypeId : int
{
    [Description("Purchase")]
    Purchase = 1,

    [Description("Reward")]
    Reward = 2,

    [Description("Sale")]
    Sale = 3,
}

public class TransactionType
{
    public TransactionTypeId Id { get; set; }
    public string Name { get; set; } = String.Empty;
}
