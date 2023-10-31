using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ButtsBlazor.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddedPromptKeys : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ArgsId",
                table: "Prompts",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

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
                principalColumn: "RowId");

            migrationBuilder.AddForeignKey(
                name: "FK_Prompts_Images_ControlImageId",
                table: "Prompts",
                column: "ControlImageId",
                principalTable: "Images",
                principalColumn: "RowId");

            migrationBuilder.AddForeignKey(
                name: "FK_Prompts_Images_OutputImageId",
                table: "Prompts",
                column: "OutputImageId",
                principalTable: "Images",
                principalColumn: "RowId");

            migrationBuilder.AddForeignKey(
                name: "FK_Prompts_PromptArgs_ArgsId",
                table: "Prompts",
                column: "ArgsId",
                principalTable: "PromptArgs",
                principalColumn: "RowId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.DropColumn(
                name: "ArgsId",
                table: "Prompts");
        }
    }
}
