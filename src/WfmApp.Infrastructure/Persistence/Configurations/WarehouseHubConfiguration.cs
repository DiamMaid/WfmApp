using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WfmApp.Domain.Common;
using WfmApp.Domain.Entities;

namespace WfmApp.Infrastructure.Persistence.Configurations;

public class WarehouseHubConfiguration : IEntityTypeConfiguration<WarehouseHub>
{
    public void Configure(EntityTypeBuilder<WarehouseHub> builder)
    {
        builder.ToTable("warehouse_hubs", t =>
            t.HasCheckConstraint("ck_warehouse_hubs_code_format", "code ~ '^[A-Z0-9-]{3,32}$'"));

        builder.HasKey(h => h.Id);

        builder.Property(h => h.Id)
            .HasColumnName("id")
            .HasColumnType("uuid")
            .HasConversion(id => id.Value, value => new HubId(value));

        builder.Property(h => h.Code)
            .HasColumnName("code")
            .HasColumnType("text")
            .IsRequired();

        builder.HasIndex(h => h.Code)
            .HasDatabaseName("uq_warehouse_hubs_code")
            .IsUnique();

        builder.Property(h => h.Name)
            .HasColumnName("name")
            .HasColumnType("text")
            .IsRequired();

        builder.OwnsOne(h => h.Location, location =>
        {
            location.Property(l => l.City)
                .HasColumnName("city")
                .HasColumnType("text");

            location.Property(l => l.Site)
                .HasColumnName("site")
                .HasColumnType("text");
        });

        builder.Property(h => h.TimeZone)
            .HasColumnName("time_zone")
            .HasColumnType("text")
            .IsRequired();

        builder.Property(h => h.IsActive)
            .HasColumnName("is_active")
            .HasColumnType("boolean")
            .HasDefaultValue(true);

        builder.Property<uint>("xmin").IsRowVersion();
    }
}
