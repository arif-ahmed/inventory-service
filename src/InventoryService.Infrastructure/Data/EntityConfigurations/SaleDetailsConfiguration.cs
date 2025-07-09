using InventoryService.Domain.Entities.Sales;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Reflection.Emit;

namespace InventoryService.Infrastructure.Data.EntityConfigurations;

public class SaleDetailsConfiguration : IEntityTypeConfiguration<SaleDetails>
{
    public void Configure(EntityTypeBuilder<SaleDetails> builder)
    {
        builder.Property(x => x.Price)
            .HasPrecision(18, 2);

        builder.Property(x => x.Quantity)
            .HasPrecision(18, 2); 
    }
}
