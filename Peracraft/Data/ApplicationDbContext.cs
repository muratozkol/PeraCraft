using Microsoft.EntityFrameworkCore;
using Peracraft.Models;

namespace Peracraft.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Kategori> Kategoriler { get; set; }
        public DbSet<Urun> Urunler { get; set; }
        public DbSet<Kullanici> Kullanicilar { get; set; }
        public DbSet<Siparis> Siparisler { get; set; }
        public DbSet<SiparisDetay> SiparisDetaylari { get; set; }
        public DbSet<SepetItem> SepetUrunleri { get; set; }
        public DbSet<Adres> Adresler { get; set; }
        public DbSet<SiparisGorsel> SiparisGorselleri { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Kategori - Ürün ilişkisi (1-n)
            modelBuilder.Entity<Urun>()
                .HasOne(u => u.Kategori)
                .WithMany(k => k.Urunler)
                .HasForeignKey(u => u.KategoriId)
                .OnDelete(DeleteBehavior.Restrict);

            // Kullanıcı - Sipariş ilişkisi (1-n)
            modelBuilder.Entity<Siparis>()
                .HasOne(s => s.Kullanici)
                .WithMany(k => k.Siparisler)
                .HasForeignKey(s => s.KullaniciId)
                .OnDelete(DeleteBehavior.Restrict);

            // Sipariş - SiparisDetay ilişkisi (1-n)
            modelBuilder.Entity<SiparisDetay>()
                .HasOne(sd => sd.Siparis)
                .WithMany(s => s.SiparisDetaylari)
                .HasForeignKey(sd => sd.SiparisId)
                .OnDelete(DeleteBehavior.Cascade);

            // SiparisDetay - Ürün ilişkisi (n-1)
            modelBuilder.Entity<SiparisDetay>()
                .HasOne(sd => sd.Urun)
                .WithMany(u => u.SiparisDetaylari)
                .HasForeignKey(sd => sd.UrunId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<SepetItem>(entity =>
            {
                entity.ToTable("SepetUrunleri");
                entity.HasKey(e => new { e.UrunId, e.KullaniciId });

                entity.HasOne(d => d.Urun)
                    .WithMany(p => p.SepetUrunleri)
                    .HasForeignKey(d => d.UrunId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.Kullanici)
                    .WithMany(p => p.SepetUrunleri)
                    .HasForeignKey(d => d.KullaniciId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Kullanıcı - Adres ilişkisi (1-n)
            modelBuilder.Entity<Adres>()
                .HasOne(a => a.Kullanici)
                .WithMany(k => k.Adresler)
                .HasForeignKey(a => a.KullaniciId)
                .OnDelete(DeleteBehavior.Cascade);

            // Unique index'ler
            modelBuilder.Entity<Kullanici>()
                .HasIndex(k => k.Eposta)
                .IsUnique();

            // Decimal precision ayarları
            modelBuilder.Entity<Urun>()
                .Property(u => u.Fiyat)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Siparis>()
                .Property(s => s.ToplamTutar)
                .HasPrecision(18, 2);

            modelBuilder.Entity<SiparisDetay>()
                .Property(sd => sd.BirimFiyat)
                .HasPrecision(18, 2);

            modelBuilder.Entity<SiparisDetay>()
                .Property(sd => sd.ToplamFiyat)
                .HasPrecision(18, 2);
        }
    }
} 