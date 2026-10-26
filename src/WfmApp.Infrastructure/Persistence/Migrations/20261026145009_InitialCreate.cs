using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WfmApp.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:btree_gist", ",,");

            migrationBuilder.CreateTable(
                name: "warehouse_hubs",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<string>(type: "text", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    city = table.Column<string>(type: "text", nullable: false),
                    site = table.Column<string>(type: "text", nullable: false),
                    time_zone = table.Column<string>(type: "text", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_warehouse_hubs", x => x.id);
                    table.CheckConstraint("ck_warehouse_hubs_code_format", "code ~ '^[A-Z0-9-]{3,32}$'");
                });

            migrationBuilder.CreateTable(
                name: "warehouse_employees",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    staff_id = table.Column<string>(type: "text", nullable: false),
                    full_name = table.Column<string>(type: "text", nullable: false),
                    hub_id = table.Column<Guid>(type: "uuid", nullable: false),
                    role = table.Column<int>(type: "integer", nullable: false),
                    employment_type = table.Column<int>(type: "integer", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_warehouse_employees", x => x.id);
                    table.ForeignKey(
                        name: "fk_warehouse_employees_hub",
                        column: x => x.hub_id,
                        principalTable: "warehouse_hubs",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "warehouse_shifts",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    employee_id = table.Column<Guid>(type: "uuid", nullable: false),
                    hub_id = table.Column<Guid>(type: "uuid", nullable: false),
                    work_zone = table.Column<int>(type: "integer", nullable: false),
                    start_time = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false),
                    end_time = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false),
                    break_start = table.Column<DateTimeOffset>(type: "timestamptz", nullable: true),
                    break_minutes = table.Column<int>(type: "integer", nullable: true),
                    checked_in_at = table.Column<DateTimeOffset>(type: "timestamptz", nullable: true),
                    checked_out_at = table.Column<DateTimeOffset>(type: "timestamptz", nullable: true),
                    status = table.Column<int>(type: "integer", nullable: false),
                    is_confirmed = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    access_control_event_id = table.Column<string>(type: "text", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_warehouse_shifts", x => x.id);
                    table.CheckConstraint("ck_ws_break_inside", "break_start IS NULL OR (break_start >= start_time AND break_start + make_interval(mins => break_minutes) <= end_time)");
                    table.CheckConstraint("ck_ws_break_minimum", "break_minutes IS NULL OR break_minutes >= 45");
                    table.CheckConstraint("ck_ws_checkout_after_checkin", "checked_out_at IS NULL OR checked_in_at IS NULL OR checked_out_at > checked_in_at");
                    table.CheckConstraint("ck_ws_max_elapsed", "end_time - start_time <= INTERVAL '12 hours'");
                    table.CheckConstraint("ck_ws_start_before_end", "start_time < end_time");
                    table.ForeignKey(
                        name: "fk_warehouse_shifts_employee",
                        column: x => x.employee_id,
                        principalTable: "warehouse_employees",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_warehouse_shifts_hub",
                        column: x => x.hub_id,
                        principalTable: "warehouse_hubs",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_warehouse_employees_hub",
                table: "warehouse_employees",
                column: "hub_id");

            migrationBuilder.CreateIndex(
                name: "uq_warehouse_employees_staff_id",
                table: "warehouse_employees",
                column: "staff_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "uq_warehouse_hubs_code",
                table: "warehouse_hubs",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_warehouse_shifts_employee_id",
                table: "warehouse_shifts",
                column: "employee_id");

            migrationBuilder.CreateIndex(
                name: "ix_warehouse_shifts_hub_status",
                table: "warehouse_shifts",
                columns: new[] { "hub_id", "status" });

            migrationBuilder.CreateIndex(
                name: "uq_ws_access_control_event",
                table: "warehouse_shifts",
                column: "access_control_event_id",
                unique: true);

            migrationBuilder.Sql(
                "CREATE INDEX ix_warehouse_shifts_employee_start ON warehouse_shifts (employee_id, start_time DESC);");

            migrationBuilder.Sql(@"ALTER TABLE warehouse_shifts
    ADD CONSTRAINT ex_warehouse_shifts_no_overlap
    EXCLUDE USING gist (
        employee_id WITH =,
        tstzrange(start_time, end_time, '[)') WITH &&
    );");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("ALTER TABLE warehouse_shifts DROP CONSTRAINT ex_warehouse_shifts_no_overlap;");
            migrationBuilder.Sql("DROP INDEX ix_warehouse_shifts_employee_start;");

            migrationBuilder.DropTable(
                name: "warehouse_shifts");

            migrationBuilder.DropTable(
                name: "warehouse_employees");

            migrationBuilder.DropTable(
                name: "warehouse_hubs");
        }
    }
}
