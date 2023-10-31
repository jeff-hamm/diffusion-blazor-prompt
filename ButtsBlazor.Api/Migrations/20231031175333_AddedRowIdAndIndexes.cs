using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ButtsBlazor.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddedRowIdAndIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Prompts_Images_CannyImageId",
                table: "Prompts");

            migrationBuilder.DropForeignKey(
                name: "FK_Prompts_Images_ControlImageId",
                table: "Prompts");

            migrationBuilder.DropForeignKey(
                name: "FK_Prompts_Images_OutputImageId",
                table: "Prompts");

            migrationBuilder.DropForeignKey(
                name: "FK_Prompts_PromptArgs_ArgsId",
                table: "Prompts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Prompts",
                table: "Prompts");

            migrationBuilder.DropIndex(
                name: "IX_Prompts_ArgsId",
                table: "Prompts");

            migrationBuilder.DropIndex(
                name: "IX_Prompts_CannyImageId",
                table: "Prompts");

            migrationBuilder.DropIndex(
                name: "IX_Prompts_ControlImageId",
                table: "Prompts");

            migrationBuilder.DropIndex(
                name: "IX_Prompts_OutputImageId",
                table: "Prompts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PromptArgs",
                table: "PromptArgs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Images",
                table: "Images");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Prompts");

            migrationBuilder.DropColumn(
                name: "ArgsId",
                table: "Prompts");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "PromptArgs");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Images");

            migrationBuilder.AlterColumn<int>(
                name: "OutputImageId",
                table: "Prompts",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ControlImageId",
                table: "Prompts",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CannyImageId",
                table: "Prompts",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RowId",
                table: "Prompts",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0)
                .Annotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddColumn<int>(
                name: "RowId",
                table: "PromptArgs",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0)
                .Annotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddColumn<int>(
                name: "RowId",
                table: "Images",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0)
                .Annotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Prompts",
                table: "Prompts",
                column: "RowId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PromptArgs",
                table: "PromptArgs",
                column: "RowId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Images",
                table: "Images",
                column: "RowId");

            migrationBuilder.CreateIndex(
                name: "IX_Images_Path",
                table: "Images",
                column: "Path",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Images_Type",
                table: "Images",
                column: "Type");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Prompts",
                table: "Prompts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PromptArgs",
                table: "PromptArgs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Images",
                table: "Images");

            migrationBuilder.DropIndex(
                name: "IX_Images_Path",
                table: "Images");

            migrationBuilder.DropIndex(
                name: "IX_Images_Type",
                table: "Images");

            migrationBuilder.DropColumn(
                name: "RowId",
                table: "Prompts");

            migrationBuilder.DropColumn(
                name: "RowId",
                table: "PromptArgs");

            migrationBuilder.DropColumn(
                name: "RowId",
                table: "Images");

            migrationBuilder.AlterColumn<Guid>(
                name: "OutputImageId",
                table: "Prompts",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "ControlImageId",
                table: "Prompts",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "CannyImageId",
                table: "Prompts",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "Prompts",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "ArgsId",
                table: "Prompts",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "PromptArgs",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "Images",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_Prompts",
                table: "Prompts",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PromptArgs",
                table: "PromptArgs",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Images",
                table: "Images",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Prompts_ArgsId",
                table: "Prompts",
                column: "ArgsId");

            migrationBuilder.CreateIndex(
                name: "IX_Prompts_CannyImageId",
                table: "Prompts",
                column: "CannyImageId");

            migrationBuilder.CreateIndex(
                name: "IX_Prompts_ControlImageId",
                table: "Prompts",
                column: "ControlImageId");

            migrationBuilder.CreateIndex(
                name: "IX_Prompts_OutputImageId",
                table: "Prompts",
                column: "OutputImageId");

            migrationBuilder.AddForeignKey(
                name: "FK_Prompts_Images_CannyImageId",
                table: "Prompts",
                column: "CannyImageId",
                principalTable: "Images",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Prompts_Images_ControlImageId",
                table: "Prompts",
                column: "ControlImageId",
                principalTable: "Images",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Prompts_Images_OutputImageId",
                table: "Prompts",
                column: "OutputImageId",
                principalTable: "Images",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Prompts_PromptArgs_ArgsId",
                table: "Prompts",
                column: "ArgsId",
                principalTable: "PromptArgs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
