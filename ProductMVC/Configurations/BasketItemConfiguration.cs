using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductMVC.Models;

namespace ProductMVC.Configurations;

public class BasketItemConfiguration : IEntityTypeConfiguration<BasketItem>
{
    public void Configure(EntityTypeBuilder<BasketItem> builder)
    {
        builder.HasOne(x => x.Product).WithMany(x => x.BasketItems).HasForeignKey(x => x.ProductId).HasPrincipalKey(x => x.Id).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(x => x.AppUser).WithMany(x => x.BasketItems).HasForeignKey(x => x.AppUserId).HasPrincipalKey(x => x.Id).OnDelete(DeleteBehavior.Cascade);

        builder.ToTable(opt =>
        {
            opt.HasCheckConstraint("CK_BasketItems_Count", "[Count]>0");
        });
    }
}
