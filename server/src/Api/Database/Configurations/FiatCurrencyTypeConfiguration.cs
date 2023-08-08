using Api.Common.ExtensionMethods;
using Api.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Api.Database.Configurations;

public class FiatCurrencyTypeConfiguration : IEntityTypeConfiguration<FiatCurrencyType>
{
    public void Configure(EntityTypeBuilder<FiatCurrencyType> builder)
    {
        builder.Property(x => x.Id).HasConversion<int>();

        builder.HasData(
            Enum.GetValues<FiatCurrencyTypeId>().Select(x => new FiatCurrencyType { Id = x, Name = x.GetDescription() })
        );
    }
}
