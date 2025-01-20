using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Peracraft.Data;
using Peracraft.Models;
using System.Text.Json;

namespace Peracraft.Controllers
{
    public class SepetController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public SepetController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            var kullaniciId = HttpContext.Session.GetInt32("KullaniciId");
            if (kullaniciId == null)
            {
                return RedirectToAction("Giris", "Hesap");
            }

            var sepetItems = _context.SepetUrunleri
                .Include(s => s.Urun)
                .Where(s => s.KullaniciId == kullaniciId)
                .ToList();

            var sepetViewModel = new SepetViewModel
            {
                Items = sepetItems,
                ToplamTutar = sepetItems.Sum(x => x.Urun.Fiyat * x.Miktar)
            };

            return View(sepetViewModel);
        }

        [HttpPost]
        public IActionResult MiktarGuncelle(int urunId, int miktar)
        {
            try
            {
                var kullaniciId = HttpContext.Session.GetInt32("KullaniciId");
                if (kullaniciId == null)
                {
                    return Json(new { success = false, message = "Oturum açmanız gerekiyor." });
                }

                var sepetItem = _context.SepetUrunleri
                    .Include(s => s.Urun)
                    .FirstOrDefault(s => s.KullaniciId == kullaniciId && s.UrunId == urunId);

                if (sepetItem == null)
                {
                    return Json(new { success = false, message = "Ürün sepette bulunamadı." });
                }

                // Yeni miktarı hesapla
                var yeniMiktar = sepetItem.Miktar + miktar;

                // Miktar 0 veya daha az ise ürünü sepetten kaldır
                if (yeniMiktar <= 0)
                {
                    _context.SepetUrunleri.Remove(sepetItem);
                }
                else
                {
                    sepetItem.Miktar = yeniMiktar;
                }

                _context.SaveChanges();

                // Yeni toplam tutarı hesapla
                var toplamTutar = _context.SepetUrunleri
                    .Where(s => s.KullaniciId == kullaniciId)
                    .Sum(s => s.Urun.Fiyat * s.Miktar);

                return Json(new { success = true, totalAmount = toplamTutar });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "İşlem sırasında bir hata oluştu: " + ex.Message });
            }
        }

        public class UrunSilModel
        {
            public int UrunId { get; set; }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UrunSil([FromBody] UrunSilModel model)
        {
            try
            {
                var kullaniciId = HttpContext.Session.GetInt32("KullaniciId");
                if (kullaniciId == null)
                {
                    return Json(new { success = false, message = "Oturum açmanız gerekiyor." });
                }

                var sepetItem = _context.SepetUrunleri
                    .FirstOrDefault(x => x.UrunId == model.UrunId && x.KullaniciId == kullaniciId);

                if (sepetItem == null)
                {
                    return Json(new { success = false, message = "Ürün sepette bulunamadı." });
                }

                _context.SepetUrunleri.Remove(sepetItem);
                _context.SaveChanges();

                // Yeni sepet toplamını ve ürün sayısını hesapla
                var yeniSepet = _context.SepetUrunleri
                    .Include(s => s.Urun)
                    .Where(s => s.KullaniciId == kullaniciId)
                    .ToList();

                var toplamTutar = yeniSepet.Sum(x => x.Urun.Fiyat * x.Miktar);
                var urunSayisi = yeniSepet.Sum(x => x.Miktar);

                // Session'daki sepet sayısını güncelle
                HttpContext.Session.SetInt32("SepetUrunSayisi", urunSayisi);

                return Json(new { 
                    success = true, 
                    cartCount = urunSayisi,
                    totalPrice = toplamTutar.ToString("C")
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "İşlem sırasında bir hata oluştu: " + ex.Message });
            }
        }

        [HttpGet]
        public IActionResult ToplamUrun()
        {
            var kullaniciId = HttpContext.Session.GetInt32("KullaniciId");
            if (kullaniciId == null)
            {
                return Json(0);
            }

            var toplamMiktar = _context.SepetUrunleri
                .Where(s => s.KullaniciId == kullaniciId)
                .Sum(x => x.Miktar);

            return Json(toplamMiktar);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SepeteEkle([FromBody] SepeteEkleModel model)
        {
            try
            {
                var kullaniciId = HttpContext.Session.GetInt32("KullaniciId");
                if (kullaniciId == null)
                {
                    return Json(new { success = false, message = "Oturum açmanız gerekiyor." });
                }

                // Ürünün varlığını kontrol et
                var urun = await _context.Urunler.FindAsync(model.UrunId);
                if (urun == null)
                {
                    return Json(new { success = false, message = "Ürün bulunamadı." });
                }

                // Sepette aynı üründen var mı kontrol et
                var sepetItem = await _context.SepetUrunleri
                    .FirstOrDefaultAsync(x => x.UrunId == model.UrunId && x.KullaniciId == kullaniciId);

                if (sepetItem != null)
                {
                    // Ürün zaten sepette varsa miktarını artır
                    sepetItem.Miktar += model.Miktar;
                    _context.SepetUrunleri.Update(sepetItem);
                }
                else
                {
                    // Yeni ürün ekle
                    _context.SepetUrunleri.Add(new SepetItem
                    {
                        UrunId = model.UrunId,
                        KullaniciId = kullaniciId.Value,
                        Miktar = model.Miktar
                    });
                }

                await _context.SaveChangesAsync();

                // Sepetteki toplam ürün sayısını hesapla
                var toplamUrun = await _context.SepetUrunleri
                    .Where(s => s.KullaniciId == kullaniciId)
                    .SumAsync(x => x.Miktar);

                // Session'a kaydet
                HttpContext.Session.SetInt32("SepetUrunSayisi", toplamUrun);

                return Json(new { success = true, message = "Ürün sepete eklendi.", cartCount = toplamUrun });
            }
            catch (DbUpdateException ex)
            {
                return Json(new { success = false, message = "Veritabanı güncelleme hatası: " + ex.InnerException?.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "İşlem sırasında bir hata oluştu: " + ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SiparisOlustur(Siparis model)
        {
            // Bu metodu kaldırıyoruz çünkü artık SiparisOnayla kullanılıyor
            return RedirectToAction("SiparisVer");
        }

        [HttpGet]
        public async Task<IActionResult> SiparisVer()
        {
            var kullaniciId = HttpContext.Session.GetInt32("KullaniciId");
            if (kullaniciId == null)
            {
                return RedirectToAction("Giris", "Hesap");
            }

            // Kullanıcının sepetindeki ürünleri al
            var sepetItems = await _context.SepetUrunleri
                .Include(s => s.Urun)
                .Where(s => s.KullaniciId == kullaniciId)
                .ToListAsync();

            if (!sepetItems.Any())
            {
                return RedirectToAction("Index");
            }

            // Kullanıcı bilgilerini al
            var kullanici = await _context.Kullanicilar.FirstOrDefaultAsync(k => k.Id == kullaniciId);
            if (kullanici == null)
            {
                return RedirectToAction("Giris", "Hesap");
            }

            // Varsayılan adresi al
            var varsayilanAdres = await _context.Adresler
                .FirstOrDefaultAsync(a => a.KullaniciId == kullaniciId && a.VarsayilanMi);

            var araToplam = sepetItems.Sum(item => (item.Urun?.Fiyat ?? 0) * item.Miktar);
            var kargoUcreti = araToplam > 500 ? 0 : 29.90m;

            var viewModel = new SiparisViewModel
            {
                AdSoyad = $"{kullanici.Ad} {kullanici.Soyad}".Trim(),
                Email = kullanici.Eposta ?? string.Empty,
                Telefon = varsayilanAdres?.Telefon ?? kullanici.Telefon ?? string.Empty,
                Adres = varsayilanAdres?.AcikAdres ?? string.Empty,
                Sehir = varsayilanAdres?.Il?.ToLower() ?? string.Empty,
                PostaKodu = varsayilanAdres?.PostaKodu ?? string.Empty,
                SepetUrunleri = sepetItems,
                AraToplam = araToplam,
                KargoUcreti = kargoUcreti,
                ToplamTutar = araToplam + kargoUcreti
            };

            return View(viewModel);
        }

        [HttpGet]
        public IActionResult GetSepetUrunSayisi()
        {
            var kullaniciId = HttpContext.Session.GetInt32("KullaniciId");
            if (kullaniciId == null)
            {
                return Json(0);
            }

            var sepetUrunSayisi = _context.SepetUrunleri
                .Where(s => s.KullaniciId == kullaniciId)
                .Sum(s => s.Miktar);

            HttpContext.Session.SetInt32("SepetUrunSayisi", sepetUrunSayisi);
            return Json(sepetUrunSayisi);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SiparisOnayla(int SeciliAdresId, string? SiparisNotu, IFormFileCollection SiparisGorselleri)
        {
            try
            {
                var kullaniciId = HttpContext.Session.GetInt32("KullaniciId");
                if (kullaniciId == null)
                {
                    return RedirectToAction("Giris", "Hesap");
                }

                // Seçili adresi kontrol et
                var adres = await _context.Adresler
                    .FirstOrDefaultAsync(a => a.Id == SeciliAdresId && a.KullaniciId == kullaniciId);

                if (adres == null)
                {
                    TempData["Hata"] = "Seçili adres bulunamadı.";
                    return RedirectToAction("SiparisVer");
                }

                // Sepetteki ürünleri al
                var sepetItems = await _context.SepetUrunleri
                    .Include(s => s.Urun)
                    .Where(s => s.KullaniciId == kullaniciId)
                    .ToListAsync();

                if (!sepetItems.Any())
                {
                    TempData["Hata"] = "Sepetinizde ürün bulunmuyor.";
                    return RedirectToAction("Index");
                }

                // Sipariş kodunu oluştur
                var siparisKodu = $"SP{DateTime.Now:yyyyMMddHHmmss}{kullaniciId}";

                // Yeni siparişi oluştur
                var siparis = new Siparis
                {
                    KullaniciId = kullaniciId.Value,
                    AdresId = SeciliAdresId,
                    SiparisKodu = siparisKodu,
                    SiparisTarihi = DateTime.Now,
                    SiparisDurumu = "Beklemede",
                    ToplamTutar = sepetItems.Sum(item => item.Miktar * item.Urun.Fiyat),
                    SiparisNotu = SiparisNotu,
                    TeslimatAdresi = $"{adres.Mahalle} {adres.AcikAdres} {adres.Ilce}/{adres.Il}",
                    TeslimatTelefonu = adres.Telefon
                };

                await _context.Siparisler.AddAsync(siparis);
                await _context.SaveChangesAsync();

                // Sipariş detaylarını ekle
                foreach (var item in sepetItems)
                {
                    var siparisDetay = new SiparisDetay
                    {
                        SiparisId = siparis.SiparisId,
                        UrunId = item.UrunId,
                        Miktar = item.Miktar,
                        BirimFiyat = item.Urun.Fiyat,
                        ToplamFiyat = item.Urun.Fiyat * item.Miktar
                    };
                    _context.SiparisDetaylari.Add(siparisDetay);
                }

                // Sipariş görsellerini ekle
                if (SiparisGorselleri != null && SiparisGorselleri.Count > 0)
                {
                    var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "siparisler", siparis.SiparisId.ToString());
                    Directory.CreateDirectory(uploadsFolder);

                    foreach (var gorsel in SiparisGorselleri)
                    {
                        if (gorsel.Length > 0)
                        {
                            var uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(gorsel.FileName)}";
                            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                await gorsel.CopyToAsync(stream);
                            }

                            var siparisGorsel = new SiparisGorsel
                            {
                                SiparisId = siparis.SiparisId,
                                DosyaYolu = $"/uploads/siparisler/{siparis.SiparisId}/{uniqueFileName}",
                                YuklenmeTarihi = DateTime.Now
                            };
                            _context.SiparisGorselleri.Add(siparisGorsel);
                        }
                    }
                }

                // Sepeti temizle
                _context.SepetUrunleri.RemoveRange(sepetItems);
                await _context.SaveChangesAsync();

                // Session'daki sepet sayısını sıfırla
                HttpContext.Session.SetInt32("SepetUrunSayisi", 0);

                TempData["Basari"] = "Siparişiniz başarıyla oluşturuldu.";
                return RedirectToAction("Siparislerim", "Hesap");
            }
            catch (Exception ex)
            {
                TempData["Hata"] = "Sipariş oluşturulurken bir hata oluştu: " + ex.Message;
                return RedirectToAction("SiparisVer");
            }
        }

        [HttpPost]
        public async Task<IActionResult> SiparisiTamamla([FromForm] SiparisViewModel model)
        {
            try
            {
                var kullaniciId = HttpContext.Session.GetInt32("KullaniciId");
                if (kullaniciId == null)
                {
                    return Json(new { success = false, message = "Oturum süreniz dolmuş. Lütfen tekrar giriş yapın." });
                }

                // Sepetteki ürünleri kontrol et
                var sepetItems = await _context.SepetUrunleri
                    .Include(s => s.Urun)
                    .Where(s => s.KullaniciId == kullaniciId)
                    .ToListAsync();

                if (!sepetItems.Any())
                {
                    return Json(new { success = false, message = "Sepetinizde ürün bulunmuyor." });
                }

                // Yeni adres oluştur
                var yeniAdres = new Adres
                {
                    KullaniciId = kullaniciId.Value,
                    AdSoyad = model.AdSoyad,
                    Telefon = model.Telefon,
                    AcikAdres = model.Adres,
                    Il = model.Sehir,
                    PostaKodu = model.PostaKodu,
                    VarsayilanMi = false
                };

                _context.Adresler.Add(yeniAdres);
                await _context.SaveChangesAsync();

                // Sipariş kodunu oluştur
                var siparisKodu = $"SP{DateTime.Now:yyyyMMddHHmmss}{kullaniciId}";

                // Yeni siparişi oluştur
                var siparis = new Siparis
                {
                    KullaniciId = kullaniciId.Value,
                    AdresId = yeniAdres.Id,
                    SiparisKodu = siparisKodu,
                    SiparisTarihi = DateTime.Now,
                    SiparisDurumu = "Beklemede",
                    ToplamTutar = model.ToplamTutar,
                    OdemeTipi = model.OdemeTipi,
                    TeslimatAdresi = $"{model.Adres}, {model.Sehir}",
                    TeslimatTelefonu = model.Telefon
                };

                await _context.Siparisler.AddAsync(siparis);
                await _context.SaveChangesAsync();

                // Sipariş detaylarını ekle
                foreach (var item in sepetItems)
                {
                    var siparisDetay = new SiparisDetay
                    {
                        SiparisId = siparis.SiparisId,
                        UrunId = item.UrunId,
                        Miktar = item.Miktar,
                        BirimFiyat = item.Urun.Fiyat,
                        ToplamFiyat = item.Urun.Fiyat * item.Miktar
                    };
                    _context.SiparisDetaylari.Add(siparisDetay);
                }

                // Sipariş görsellerini ekle
                if (Request.Form.Files != null && Request.Form.Files.Count > 0)
                {
                    var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "siparisler", siparis.SiparisId.ToString());
                    Directory.CreateDirectory(uploadsFolder);

                    foreach (var gorsel in Request.Form.Files)
                    {
                        if (gorsel.Length > 0)
                        {
                            var uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(gorsel.FileName)}";
                            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                await gorsel.CopyToAsync(stream);
                            }

                            var siparisGorsel = new SiparisGorsel
                            {
                                SiparisId = siparis.SiparisId,
                                DosyaYolu = $"/uploads/siparisler/{siparis.SiparisId}/{uniqueFileName}",
                                YuklenmeTarihi = DateTime.Now
                            };
                            _context.SiparisGorselleri.Add(siparisGorsel);
                        }
                    }
                }

                // Sepeti temizle
                _context.SepetUrunleri.RemoveRange(sepetItems);
                await _context.SaveChangesAsync();

                // Session'daki sepet sayısını sıfırla
                HttpContext.Session.SetInt32("SepetUrunSayisi", 0);

                return Json(new { success = true, message = "Siparişiniz başarıyla oluşturuldu.", redirectUrl = "/Hesap/Siparislerim" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Sipariş oluşturulurken bir hata oluştu: " + ex.Message });
            }
        }
    }
} 