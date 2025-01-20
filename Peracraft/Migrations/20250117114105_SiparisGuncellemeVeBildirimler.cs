using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Peracraft.Migrations
{
    /// <inheritdoc />
    public partial class SiparisGuncellemeVeBildirimler : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_SepetUrunleri",
                table: "SepetUrunleri");

            migrationBuilder.DropIndex(
                name: "IX_SepetUrunleri_UrunId_KullaniciId",
                table: "SepetUrunleri");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "SepetUrunleri");

            migrationBuilder.AddColumn<DateTime>(
                name: "GuncellenmeTarihi",
                table: "Siparisler",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "YuklenenGorseller",
                table: "Siparisler",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SepetUrunleri",
                table: "SepetUrunleri",
                columns: new[] { "UrunId", "KullaniciId" });

            migrationBuilder.CreateTable(
                name: "Bildirimler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    KullaniciId = table.Column<int>(type: "int", nullable: false),
                    Mesaj = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Tarih = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Okundu = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bildirimler", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Bildirimler_Kullanicilar_KullaniciId",
                        column: x => x.KullaniciId,
                        principalTable: "Kullanicilar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Bildirimler_KullaniciId",
                table: "Bildirimler",
                column: "KullaniciId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Bildirimler");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SepetUrunleri",
                table: "SepetUrunleri");

            migrationBuilder.DropColumn(
                name: "GuncellenmeTarihi",
                table: "Siparisler");

            migrationBuilder.DropColumn(
                name: "YuklenenGorseller",
                table: "Siparisler");

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
        }
    }
}
