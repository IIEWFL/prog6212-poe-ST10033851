using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CoursePilotWebApp.Migrations
{
    /// <inheritdoc />
    public partial class graphtable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            
            migrationBuilder.AddColumn<bool>(
                name: "WeekFinished",
                table: "StudyRecords",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "GraphData",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ModuleCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserID = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    WeekStartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdealHours = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalHoursStudied = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GraphData", x => x.ID);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GraphData");

            migrationBuilder.DropColumn(
                name: "WeekFinished",
                table: "StudyRecords");

        }
    }
}
