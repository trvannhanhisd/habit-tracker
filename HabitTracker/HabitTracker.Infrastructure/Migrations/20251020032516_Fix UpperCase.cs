using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HabitTracker.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixUpperCase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "petName",
                table: "Habits",
                newName: "PetName");

            migrationBuilder.RenameColumn(
                name: "lastCompletedAt",
                table: "Habits",
                newName: "LastCompletedAt");

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastCompletedAt",
                table: "Habits",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PetName",
                table: "Habits",
                newName: "petName");

            migrationBuilder.RenameColumn(
                name: "LastCompletedAt",
                table: "Habits",
                newName: "lastCompletedAt");

            migrationBuilder.AlterColumn<DateTime>(
                name: "lastCompletedAt",
                table: "Habits",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);
        }
    }
}
