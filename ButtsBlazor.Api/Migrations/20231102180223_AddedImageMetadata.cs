using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ButtsBlazor.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddedImageMetadata : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ImageMetadata",
                columns: table => new
                {
                    RowId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Prompt = table.Column<string>(type: "TEXT", nullable: true),
                    Code = table.Column<string>(type: "TEXT", nullable: true),
                    InputImage = table.Column<string>(type: "TEXT", nullable: true),
                    ImageEntityId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImageMetadata", x => x.RowId);
                    table.ForeignKey(
                        name: "FK_ImageMetadata_Images_ImageEntityId",
                        column: x => x.ImageEntityId,
                        principalTable: "Images",
                        principalColumn: "RowId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ImageMetadata_ImageEntityId",
                table: "ImageMetadata",
                column: "ImageEntityId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ImageMetadata");
        }
    }
}
