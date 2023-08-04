using Api.Common.ExtensionMethods;
using Api.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Api.Database.Configurations;

public class TransactionTypeConfiguration : IEntityTypeConfiguration<TransactionType>
{
    public void Configure(EntityTypeBuilder<TransactionType> builder)
    {
        builder.Property(x => x.Id).HasConversion<int>();

        builder.HasData(
            Enum.GetValues<TransactionTypeId>().Select(x => new TransactionType { Id = x, Name = x.GetDescription() })
        );
    }
}
