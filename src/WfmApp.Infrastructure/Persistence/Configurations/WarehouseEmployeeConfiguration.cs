using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WfmApp.Domain.Common;
using WfmApp.Domain.Entities;

namespace WfmApp.Infrastructure.Persistence.Configurations;

public class WarehouseEmployeeConfiguration : IEntityTypeConfiguration<WarehouseEmployee>
{
    public void Configure(EntityTypeBuilder<WarehouseEmployee> builder)
    {
        builder.ToTable("warehouse_employees");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .HasColumnName("id")
            .HasColumnType("uuid")
            .HasConversion(id => id.Value, value => new EmployeeId(value));

        builder.Property(e => e.StaffId)
            .HasColumnName("staff_id")
            .HasColumnType("text")
            .IsRequired();

        builder.HasIndex(e => e.StaffId)
            .HasDatabaseName("uq_warehouse_employees_staff_id")
            .IsUnique();

        builder.Property(e => e.FullName)
            .HasColumnName("full_name")
            .HasColumnType("text")
            .IsRequired();

        builder.Property(e => e.HubId)
            .HasColumnName("hub_id")
            .HasColumnType("uuid")
            .HasConversion(id => id.Value, value => new HubId(value));

        builder.Property(e => e.Role)
            .HasColumnName("role")
            .HasConversion<int>();

        builder.Property(e => e.EmploymentType)
            .HasColumnName("employment_type")
            .HasConversion<int>();

        builder.Property(e => e.Status)
            .HasColumnName("status")
            .HasConversion<int>();

        builder.HasIndex(e => e.HubId)
            .HasDatabaseName("ix_warehouse_employees_hub");

        builder.HasOne<WarehouseHub>()
            .WithMany()
            .HasForeignKey(e => e.HubId)
            .HasConstraintName("fk_warehouse_employees_hub");

        builder.Property<uint>("xmin").IsRowVersion();
    }
}
