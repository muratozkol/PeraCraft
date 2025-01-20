using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Peracraft.Data;
using Peracraft.Models;

namespace Peracraft.Controllers
{
    public class AdresController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdresController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var kullaniciId = HttpContext.Session.GetInt32("KullaniciId");
            if (kullaniciId == null)
            {
                return RedirectToAction("Giris", "Hesap");
            }

            var adresler = await _context.Adresler
                .Where(a => a.KullaniciId == kullaniciId)
                .OrderByDescending(a => a.VarsayilanMi)
                .ThenByDescending(a => a.OlusturulmaTarihi)
                .ToListAsync();

            return View(adresler);
        }

        [HttpGet]
        public IActionResult Ekle()
        {
            var kullaniciId = HttpContext.Session.GetInt32("KullaniciId");
            if (kullaniciId == null)
            {
                return RedirectToAction("Giris", "Hesap");
            }

            // ReturnToSiparis değerini TempData'ya kaydet
            var returnToSiparis = Request.Query["returnToSiparis"].ToString();
            if (!string.IsNullOrEmpty(returnToSiparis) && returnToSiparis.ToLower() == "true")
            {
                TempData["ReturnToSiparis"] = "true";
            }

            return View(new Adres());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Ekle(Adres model)
        {
            try
            {
                var kullaniciId = HttpContext.Session.GetInt32("KullaniciId");
                if (kullaniciId == null)
                {
                    TempData["Hata"] = "Oturum süreniz dolmuş. Lütfen tekrar giriş yapın.";
                    return RedirectToAction("Giris", "Hesap");
                }

                // ModelState'den Kullanici validation hatasını kaldır
                ModelState.Remove("Kullanici");

                if (!ModelState.IsValid)
                {
                    var errors = string.Join(", ", ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage));
                    TempData["Hata"] = "Form hataları: " + errors;
                    return View(model);
                }

                model.KullaniciId = kullaniciId.Value;
                model.OlusturulmaTarihi = DateTime.Now;

                // İlk adres veya varsayılan adres işlemleri
                var mevcutAdresVarMi = await _context.Adresler.AnyAsync(a => a.KullaniciId == kullaniciId);
                if (!mevcutAdresVarMi || model.VarsayilanMi)
                {
                    var varsayilanAdresler = await _context.Adresler
                        .Where(a => a.KullaniciId == kullaniciId && a.VarsayilanMi)
                        .ToListAsync();

                    foreach (var adres in varsayilanAdresler)
                    {
                        adres.VarsayilanMi = false;
                        _context.Adresler.Update(adres);
                    }

                    model.VarsayilanMi = true;
                }

                // Adresi ekle
                _context.Adresler.Add(model);
                await _context.SaveChangesAsync();

                // Başarı mesajı ve yönlendirme
                if (TempData["ReturnToSiparis"]?.ToString() == "true")
                {
                    TempData["Basari"] = "Adres başarıyla kaydedildi. Siparişinizi tamamlayabilirsiniz.";
                    return RedirectToAction("SiparisVer", "Sepet");
                }

                TempData["Basari"] = "Adres başarıyla kaydedildi.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Hata"] = "Adres eklenirken bir hata oluştu: " + ex.Message;
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Duzenle(int id)
        {
            var kullaniciId = HttpContext.Session.GetInt32("KullaniciId");
            if (kullaniciId == null)
            {
                return RedirectToAction("Giris", "Hesap");
            }

            var adres = await _context.Adresler
                .FirstOrDefaultAsync(a => a.Id == id && a.KullaniciId == kullaniciId);

            if (adres == null)
            {
                return NotFound();
            }

            // ReturnToSiparis değerini TempData'ya kaydet
            var returnToSiparis = Request.Query["returnToSiparis"].ToString();
            if (!string.IsNullOrEmpty(returnToSiparis) && returnToSiparis.ToLower() == "true")
            {
                TempData["ReturnToSiparis"] = "true";
            }

            return View(adres);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Duzenle(int id, Adres model)
        {
            try
            {
                var kullaniciId = HttpContext.Session.GetInt32("KullaniciId");
                if (kullaniciId == null)
                {
                    return RedirectToAction("Giris", "Hesap");
                }

                if (id != model.Id)
                {
                    return NotFound();
                }

                // ModelState'den Kullanici validation hatasını kaldır
                ModelState.Remove("Kullanici");

                if (!ModelState.IsValid)
                {
                    var errors = string.Join(", ", ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage));
                    TempData["Hata"] = "Form hataları: " + errors;
                    return View(model);
                }

                var adres = await _context.Adresler
                    .FirstOrDefaultAsync(a => a.Id == id && a.KullaniciId == kullaniciId);

                if (adres == null)
                {
                    return NotFound();
                }

                // Adresi güncelle
                adres.AdresBasligi = model.AdresBasligi;
                adres.AdSoyad = model.AdSoyad;
                adres.Telefon = model.Telefon;
                adres.Il = model.Il;
                adres.Ilce = model.Ilce;
                adres.Mahalle = model.Mahalle;
                adres.AcikAdres = model.AcikAdres;
                adres.PostaKodu = model.PostaKodu;
                adres.GuncellenmeTarihi = DateTime.Now;

                // Varsayılan adres işlemleri
                if (model.VarsayilanMi && !adres.VarsayilanMi)
                {
                    var varsayilanAdresler = await _context.Adresler
                        .Where(a => a.KullaniciId == kullaniciId && a.VarsayilanMi)
                        .ToListAsync();

                    foreach (var digerAdres in varsayilanAdresler)
                    {
                        digerAdres.VarsayilanMi = false;
                        _context.Adresler.Update(digerAdres);
                    }
                }
                adres.VarsayilanMi = model.VarsayilanMi;

                await _context.SaveChangesAsync();

                TempData["Basari"] = "Adres başarıyla güncellendi.";

                // Eğer sipariş sayfasından geldiyse
                if (TempData["ReturnToSiparis"]?.ToString() == "true")
                {
                    return RedirectToAction("SiparisVer", "Sepet");
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Hata"] = "Adres güncellenirken bir hata oluştu: " + ex.Message;
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Sil(int id)
        {
            var kullaniciId = HttpContext.Session.GetInt32("KullaniciId");
            if (kullaniciId == null)
            {
                return RedirectToAction("Giris", "Hesap");
            }

            var adres = await _context.Adresler
                .FirstOrDefaultAsync(a => a.Id == id && a.KullaniciId == kullaniciId);

            if (adres == null)
            {
                return NotFound();
            }

            _context.Adresler.Remove(adres);
            await _context.SaveChangesAsync();

            // Eğer silinen adres varsayılan adres ise ve başka adres varsa
            if (adres.VarsayilanMi)
            {
                var digerAdres = await _context.Adresler
                    .FirstOrDefaultAsync(a => a.KullaniciId == kullaniciId);

                if (digerAdres != null)
                {
                    digerAdres.VarsayilanMi = true;
                    await _context.SaveChangesAsync();
                }
            }

            TempData["Basari"] = "Adres başarıyla silindi.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VarsayilanYap(int id)
        {
            var kullaniciId = HttpContext.Session.GetInt32("KullaniciId");
            if (kullaniciId == null)
            {
                return RedirectToAction("Giris", "Hesap");
            }

            // Önce tüm adreslerin varsayılan durumunu kaldır
            var tumAdresler = await _context.Adresler
                .Where(a => a.KullaniciId == kullaniciId)
                .ToListAsync();

            foreach (var adres in tumAdresler)
            {
                adres.VarsayilanMi = false;
            }

            // Seçilen adresi varsayılan yap
            var yeniVarsayilanAdres = tumAdresler.FirstOrDefault(a => a.Id == id);
            if (yeniVarsayilanAdres != null)
            {
                yeniVarsayilanAdres.VarsayilanMi = true;
                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }

            return Json(new { success = false });
        }
    }
} 