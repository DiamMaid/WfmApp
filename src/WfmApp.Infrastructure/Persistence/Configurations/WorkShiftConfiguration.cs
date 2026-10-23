using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WfmApp.Domain.Common;
using WfmApp.Domain.Entities;

namespace WfmApp.Infrastructure.Persistence.Configurations;

public class WorkShiftConfiguration : IEntityTypeConfiguration<WorkShift>
{
    public void Configure(EntityTypeBuilder<WorkShift> builder)
    {
        builder.ToTable("work_shifts", t =>
        {
            t.HasCheckConstraint("ck_work_shifts_start_before_end", "start_time < end_time");
            t.HasCheckConstraint("ck_work_shifts_max_duration", "end_time - start_time <= INTERVAL '24 hours'");
        });

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .HasColumnName("id")
            .HasConversion(id => id.Value, value => new WorkShiftId(value))
            .HasColumnType("uuid");

        builder.Property(e => e.EmployeeId)
            .HasColumnName("employee_id")
            .HasConversion(id => id.Value, value => new EmployeeId(value))
            .HasColumnType("uuid");

        builder.Property(e => e.IsConfirmed)
            .HasColumnName("is_confirmed")
            .HasColumnType("boolean")
            .HasDefaultValue(false);

        builder.OwnsOne(e => e.Interval, interval =>
        {
            interval.Property(p => p.Start)
                .HasColumnName("start_time")
                .HasColumnType("timestamptz");

            interval.Property(p => p.End)
                .HasColumnName("end_time")
                .HasColumnType("timestamptz");

            interval.Ignore(p => p.Duration);
        });

        builder.Property<uint>("xmin").IsRowVersion();
    }
}
