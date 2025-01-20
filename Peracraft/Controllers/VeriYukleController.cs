using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Peracraft.Data;
using Peracraft.Models;

namespace Peracraft.Controllers
{
    public class VeriYukleController : Controller
    {
        private readonly ApplicationDbContext _context;

        public VeriYukleController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> UrunleriYukle()
        {
            try
            {
                // Önce mevcut ürünleri ve kategorileri silelim
                var mevcutUrunler = await _context.Urunler.ToListAsync();
                var mevcutKategoriler = await _context.Kategoriler.ToListAsync();
                _context.Urunler.RemoveRange(mevcutUrunler);
                _context.Kategoriler.RemoveRange(mevcutKategoriler);
                await _context.SaveChangesAsync();

                // Kategorileri manuel ID'lerle oluşturalım
                var bardaklar = new Kategori { Id = 1, Ad = "Bardaklar", Aciklama = "Özel tasarım bardaklar", AktifMi = true, EklenmeTarihi = DateTime.Now };
                var canvaslar = new Kategori { Id = 2, Ad = "Canvas Tablolar", Aciklama = "Modern canvas tablolar", AktifMi = true, EklenmeTarihi = DateTime.Now };
                var websiteler = new Kategori { Id = 3, Ad = "Kişiye Özel Web Siteleri", Aciklama = "Profesyonel web siteleri", AktifMi = true, EklenmeTarihi = DateTime.Now };

                _context.Kategoriler.Add(bardaklar);
                _context.Kategoriler.Add(canvaslar);
                _context.Kategoriler.Add(websiteler);
                await _context.SaveChangesAsync();

                // Bardaklar kategorisi için ürünler
                var bardakUrunleri = new List<Urun>
                {
                    new Urun { UrunAdi = "Işıklı LED Kupa", Aciklama = "Özel tasarım ışıklı LED kupa", Fiyat = 199.00M, StokMiktari = 10, ResimUrl = "/img/products/led-kupa.jpg", KategoriId = bardaklar.Id, AktifMi = true, EklenmeTarihi = DateTime.Now },
                    new Urun { UrunAdi = "Sihirli Kupa", Aciklama = "Isıya duyarlı renk değiştiren kupa", Fiyat = 179.00M, StokMiktari = 15, ResimUrl = "/img/products/sihirli-kupa.jpg", KategoriId = bardaklar.Id, AktifMi = true, EklenmeTarihi = DateTime.Now },
                    new Urun { UrunAdi = "Kişiye Özel Fotoğraflı Kupa", Aciklama = "Kendi fotoğrafınızla özel tasarım kupa", Fiyat = 149.00M, StokMiktari = 20, ResimUrl = "/img/products/foto-kupa.jpg", KategoriId = bardaklar.Id, AktifMi = true, EklenmeTarihi = DateTime.Now },
                    new Urun { UrunAdi = "Neon Işıklı Kupa", Aciklama = "Neon efektli LED kupa", Fiyat = 229.00M, StokMiktari = 8, ResimUrl = "/img/products/neon-kupa.jpg", KategoriId = bardaklar.Id, AktifMi = true, EklenmeTarihi = DateTime.Now },
                    new Urun { UrunAdi = "Premium Gold Kupa", Aciklama = "Altın detaylı özel tasarım kupa", Fiyat = 249.00M, StokMiktari = 5, ResimUrl = "/img/products/gold-kupa.jpg", KategoriId = bardaklar.Id, AktifMi = true, EklenmeTarihi = DateTime.Now }
                };

                // Canvas Tablolar kategorisi için ürünler
                var canvasUrunleri = new List<Urun>
                {
                    new Urun { UrunAdi = "Modern Soyut Canvas", Aciklama = "Modern sanat eseri canvas tablo", Fiyat = 299.00M, StokMiktari = 10, ResimUrl = "/img/products/modern-canvas.jpg", KategoriId = canvaslar.Id, AktifMi = true, EklenmeTarihi = DateTime.Now },
                    new Urun { UrunAdi = "Doğa Manzarası Canvas", Aciklama = "HD kalitede doğa manzarası", Fiyat = 349.00M, StokMiktari = 8, ResimUrl = "/img/products/doga-canvas.jpg", KategoriId = canvaslar.Id, AktifMi = true, EklenmeTarihi = DateTime.Now },
                    new Urun { UrunAdi = "Minimalist Canvas Set", Aciklama = "3 parçalı minimalist tasarım set", Fiyat = 499.00M, StokMiktari = 5, ResimUrl = "/img/products/minimal-canvas.jpg", KategoriId = canvaslar.Id, AktifMi = true, EklenmeTarihi = DateTime.Now },
                    new Urun { UrunAdi = "Şehir Silueti Canvas", Aciklama = "Özel tasarım şehir manzarası", Fiyat = 399.00M, StokMiktari = 7, ResimUrl = "/img/products/sehir-canvas.jpg", KategoriId = canvaslar.Id, AktifMi = true, EklenmeTarihi = DateTime.Now },
                    new Urun { UrunAdi = "Vintage Sanat Canvas", Aciklama = "Klasik sanat eseri reprodüksiyon", Fiyat = 449.00M, StokMiktari = 6, ResimUrl = "/img/products/vintage-canvas.jpg", KategoriId = canvaslar.Id, AktifMi = true, EklenmeTarihi = DateTime.Now }
                };

                // Web Siteleri kategorisi için ürünler
                var webUrunleri = new List<Urun>
                {
                    new Urun { UrunAdi = "Kişisel Portfolio Site", Aciklama = "Profesyonel portfolio web sitesi", Fiyat = 2999.00M, StokMiktari = 999, ResimUrl = "/img/products/portfolio-site.jpg", KategoriId = websiteler.Id, AktifMi = true, EklenmeTarihi = DateTime.Now },
                    new Urun { UrunAdi = "E-Ticaret Sitesi", Aciklama = "Tam donanımlı e-ticaret çözümü", Fiyat = 4999.00M, StokMiktari = 999, ResimUrl = "/img/products/eticaret-site.jpg", KategoriId = websiteler.Id, AktifMi = true, EklenmeTarihi = DateTime.Now },
                    new Urun { UrunAdi = "Blog Sitesi", Aciklama = "Kişisel blog web sitesi", Fiyat = 1999.00M, StokMiktari = 999, ResimUrl = "/img/products/blog-site.jpg", KategoriId = websiteler.Id, AktifMi = true, EklenmeTarihi = DateTime.Now },
                    new Urun { UrunAdi = "Kurumsal Web Sitesi", Aciklama = "Profesyonel kurumsal site", Fiyat = 3999.00M, StokMiktari = 999, ResimUrl = "/img/products/kurumsal-site.jpg", KategoriId = websiteler.Id, AktifMi = true, EklenmeTarihi = DateTime.Now },
                    new Urun { UrunAdi = "Landing Page", Aciklama = "Özel tasarım landing page", Fiyat = 1499.00M, StokMiktari = 999, ResimUrl = "/img/products/landing-site.jpg", KategoriId = websiteler.Id, AktifMi = true, EklenmeTarihi = DateTime.Now }
                };

                // Tüm ürünleri ekleyelim
                await _context.Urunler.AddRangeAsync(bardakUrunleri);
                await _context.Urunler.AddRangeAsync(canvasUrunleri);
                await _context.Urunler.AddRangeAsync(webUrunleri);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Ürünler başarıyla yüklendi." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Ürünler yüklenirken bir hata oluştu: " + ex.Message });
            }
        }
    }
} 