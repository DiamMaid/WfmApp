using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WfmApp.Domain.Common;
using WfmApp.Domain.Entities;

namespace WfmApp.Infrastructure.Persistence.Configurations;

public class WarehouseShiftConfiguration : IEntityTypeConfiguration<WarehouseShift>
{
    public void Configure(EntityTypeBuilder<WarehouseShift> builder)
    {
        builder.ToTable("warehouse_shifts", t =>
        {
            t.HasCheckConstraint("ck_ws_start_before_end", "start_time < end_time");
            t.HasCheckConstraint("ck_ws_max_elapsed", "end_time - start_time <= INTERVAL '12 hours'");
            t.HasCheckConstraint("ck_ws_break_minimum", "break_minutes IS NULL OR break_minutes >= 45");
            t.HasCheckConstraint("ck_ws_break_inside",
                "break_start IS NULL OR (break_start >= start_time AND break_start + make_interval(mins => break_minutes) <= end_time)");
            t.HasCheckConstraint("ck_ws_checkout_after_checkin",
                "checked_out_at IS NULL OR checked_in_at IS NULL OR checked_out_at > checked_in_at");
        });

        builder.HasKey(s => s.Id);

        builder.Property(s => s.Id)
            .HasColumnName("id")
            .HasColumnType("uuid")
            .HasConversion(id => id.Value, value => new WarehouseShiftId(value));

        builder.Property(s => s.EmployeeId)
            .HasColumnName("employee_id")
            .HasColumnType("uuid")
            .HasConversion(id => id.Value, value => new EmployeeId(value));

        builder.Property(s => s.HubId)
            .HasColumnName("hub_id")
            .HasColumnType("uuid")
            .HasConversion(id => id.Value, value => new HubId(value));

        builder.Property(s => s.Zone)
            .HasColumnName("work_zone")
            .HasConversion<int>();

        builder.OwnsOne(s => s.Planned, planned =>
        {
            planned.Property(p => p.Start)
                .HasColumnName("start_time")
                .HasColumnType("timestamptz");

            planned.Property(p => p.End)
                .HasColumnName("end_time")
                .HasColumnType("timestamptz");

            planned.Ignore(p => p.Elapsed);
            planned.Ignore(p => p.Worked);

            planned.OwnsOne(p => p.Break, shiftBreak =>
            {
                shiftBreak.Property(b => b.Start)
                    .HasColumnName("break_start")
                    .HasColumnType("timestamptz");

                shiftBreak.Property(b => b.Duration)
                    .HasColumnName("break_minutes")
                    .HasColumnType("integer")
                    .HasConversion(
                        duration => (int)Math.Round(duration.TotalMinutes, MidpointRounding.AwayFromZero),
                        minutes => TimeSpan.FromMinutes(minutes));
            });

            planned.Navigation(p => p.Break).IsRequired(false);
        });

        builder.Property(s => s.CheckedInAt)
            .HasColumnName("checked_in_at")
            .HasColumnType("timestamptz");

        builder.Property(s => s.CheckedOutAt)
            .HasColumnName("checked_out_at")
            .HasColumnType("timestamptz");

        builder.Property(s => s.Status)
            .HasColumnName("status")
            .HasConversion<int>();

        builder.Property(s => s.IsConfirmed)
            .HasColumnName("is_confirmed")
            .HasColumnType("boolean")
            .HasDefaultValue(false);

        builder.Property(s => s.AccessControlEventId)
            .HasColumnName("access_control_event_id")
            .HasColumnType("text");

        builder.HasIndex(s => s.AccessControlEventId)
            .HasDatabaseName("uq_ws_access_control_event")
            .IsUnique();

        builder.Ignore(s => s.WorkedDuration);

        builder.HasIndex(s => new { s.HubId, s.Status })
            .HasDatabaseName("ix_warehouse_shifts_hub_status");

        builder.HasOne<WarehouseEmployee>()
            .WithMany()
            .HasForeignKey(s => s.EmployeeId)
            .HasConstraintName("fk_warehouse_shifts_employee");

        builder.HasOne<WarehouseHub>()
            .WithMany()
            .HasForeignKey(s => s.HubId)
            .HasConstraintName("fk_warehouse_shifts_hub");

        builder.Property<uint>("xmin").IsRowVersion();
    }
}
