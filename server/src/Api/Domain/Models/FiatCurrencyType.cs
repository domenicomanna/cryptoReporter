using System.ComponentModel;

namespace Api.Domain.Models;

public enum FiatCurrencyTypeId : int
{
    [Description("USD")]
    USD = 1,

    [Description("EUR")]
    EUR = 2,

    [Description("JPY")]
    JPY = 3,

    [Description("GBP")]
    GBP = 4,

    [Description("CNY")]
    CNY = 5,

    [Description("AUD")]
    AUD = 6,

    [Description("CAD")]
    CAD = 7,

    [Description("CHF")]
    CHF = 8,

    [Description("HKD")]
    HKD = 9,

    [Description("SGD")]
    SGD = 10,
}

public class FiatCurrencyType
{
    public FiatCurrencyTypeId Id { get; set; }
    public string Name { get; set; } = string.Empty;
}
