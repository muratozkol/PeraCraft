using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Peracraft.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSepetItemStructure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SepetUrunleri_Urunler_UrunId",
                table: "SepetUrunleri");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SepetUrunleri",
                table: "SepetUrunleri");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "SepetUrunleri",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_SepetUrunleri",
                table: "SepetUrunleri",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_SepetUrunleri_UrunId_KullaniciId",
                table: "SepetUrunleri",
                columns: new[] { "UrunId", "KullaniciId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_SepetUrunleri_Urunler_UrunId",
                table: "SepetUrunleri",
                column: "UrunId",
                principalTable: "Urunler",
                principalColumn: "UrunId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SepetUrunleri_Urunler_UrunId",
                table: "SepetUrunleri");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SepetUrunleri",
                table: "SepetUrunleri");

            migrationBuilder.DropIndex(
                name: "IX_SepetUrunleri_UrunId_KullaniciId",
                table: "SepetUrunleri");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "SepetUrunleri");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SepetUrunleri",
                table: "SepetUrunleri",
                column: "UrunId");

            migrationBuilder.AddForeignKey(
                name: "FK_SepetUrunleri_Urunler_UrunId",
                table: "SepetUrunleri",
                column: "UrunId",
                principalTable: "Urunler",
                principalColumn: "UrunId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
