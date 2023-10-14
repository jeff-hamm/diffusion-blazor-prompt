using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ButtsBlazor.Api.Migrations
{
    /// <inheritdoc />
    public partial class FixedPromptArgs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Prompts_PromptArgs_ArgsId",
                table: "Prompts");

            migrationBuilder.DropColumn(
                name: "Enqueued",
                table: "PromptArgs");

            migrationBuilder.DropColumn(
                name: "ImagePath",
                table: "PromptArgs");

            migrationBuilder.DropColumn(
                name: "ProcessingCompleted",
                table: "PromptArgs");

            migrationBuilder.DropColumn(
                name: "ProcessingStart",
                table: "PromptArgs");

            migrationBuilder.AlterColumn<Guid>(
                name: "ArgsId",
                table: "Prompts",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Prompts_PromptArgs_ArgsId",
                table: "Prompts",
                column: "ArgsId",
                principalTable: "PromptArgs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Prompts_PromptArgs_ArgsId",
                table: "Prompts");

            migrationBuilder.AlterColumn<Guid>(
                name: "ArgsId",
                table: "Prompts",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "TEXT");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "Enqueued",
                table: "PromptArgs",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImagePath",
                table: "PromptArgs",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ProcessingCompleted",
                table: "PromptArgs",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ProcessingStart",
                table: "PromptArgs",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Prompts_PromptArgs_ArgsId",
                table: "Prompts",
                column: "ArgsId",
                principalTable: "PromptArgs",
                principalColumn: "Id");
        }
    }
}
