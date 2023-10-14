using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ButtsBlazor.Api.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Images",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Path = table.Column<string>(type: "TEXT", nullable: false),
                    Base64Hash = table.Column<string>(type: "TEXT", nullable: false),
                    Type = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Images", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PromptArgs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Prompt = table.Column<string>(type: "TEXT", nullable: false),
                    Negative = table.Column<string>(type: "TEXT", nullable: true),
                    ControlFilePath = table.Column<string>(type: "TEXT", nullable: true),
                    NumSteps = table.Column<int>(type: "INTEGER", nullable: true),
                    CannyLow = table.Column<int>(type: "INTEGER", nullable: true),
                    CannyHigh = table.Column<int>(type: "INTEGER", nullable: true),
                    ControlSize = table.Column<int>(type: "INTEGER", nullable: true),
                    ControlScale = table.Column<double>(type: "REAL", nullable: true),
                    ControlFile = table.Column<string>(type: "TEXT", nullable: true),
                    Enqueued = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
                    ProcessingStart = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
                    ProcessingCompleted = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
                    ImagePath = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PromptArgs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Prompts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ArgsId = table.Column<Guid>(type: "TEXT", nullable: true),
                    ControlImageId = table.Column<Guid>(type: "TEXT", nullable: true),
                    CannyImageId = table.Column<Guid>(type: "TEXT", nullable: true),
                    OutputImageId = table.Column<Guid>(type: "TEXT", nullable: true),
                    Enqueued = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    ProcessingStart = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
                    ProcessingCompleted = table.Column<DateTimeOffset>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Prompts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Prompts_Images_CannyImageId",
                        column: x => x.CannyImageId,
                        principalTable: "Images",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Prompts_Images_ControlImageId",
                        column: x => x.ControlImageId,
                        principalTable: "Images",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Prompts_Images_OutputImageId",
                        column: x => x.OutputImageId,
                        principalTable: "Images",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Prompts_PromptArgs_ArgsId",
                        column: x => x.ArgsId,
                        principalTable: "PromptArgs",
                        principalColumn: "Id");
                });

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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Prompts");

            migrationBuilder.DropTable(
                name: "Images");

            migrationBuilder.DropTable(
                name: "PromptArgs");
        }
    }
}
