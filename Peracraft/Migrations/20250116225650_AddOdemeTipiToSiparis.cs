using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Peracraft.Migrations
{
    /// <inheritdoc />
    public partial class AddOdemeTipiToSiparis : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OdemeTipi",
                table: "Siparisler",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OdemeTipi",
                table: "Siparisler");
        }
    }
}
