using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Peracraft.Data;
using Peracraft.Models;
using System.Security.Cryptography;
using System.Text;
using System.Globalization;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace Peracraft.Controllers
{
    public class HesapController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HesapController(ApplicationDbContext context)
        {
            _context = context;
        }

        private string FormatlaIsim(string isim)
        {
            if (string.IsNullOrEmpty(isim)) return isim;
            
            TextInfo textInfo = new CultureInfo("tr-TR", false).TextInfo;
            return textInfo.ToTitleCase(isim.ToLower());
        }

        public IActionResult Giris()
        {
            return View();
        }

        [HttpPost]
        public IActionResult GirisYap(string email, string sifre)
        {
            try
            {
                if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(sifre))
                {
                    return Json(new { success = false, message = "Email ve şifre alanları zorunludur!" });
                }

                Console.WriteLine($"Giriş denemesi başladı - Email: {email}");
                
                // Önce kullanıcıyı bul
                var kullanici = _context.Kullanicilar.FirstOrDefault(k => k.Eposta == email.Trim().ToLower());
                if (kullanici == null)
                {
                    Console.WriteLine("Kullanıcı bulunamadı");
                    return Json(new { success = false, message = "Email veya şifre hatalı!" });
                }

                Console.WriteLine($"Kullanıcı bulundu - ID: {kullanici.Id}, Ad: {kullanici.Ad}, Hash: {kullanici.Sifre}");

                var hashedSifre = HashPassword(sifre);
                Console.WriteLine($"Girilen şifrenin hash'i: {hashedSifre}");
                Console.WriteLine($"Veritabanındaki hash: {kullanici.Sifre}");

                if (kullanici.Sifre != hashedSifre)
                {
                    Console.WriteLine("Şifre eşleşmedi");
                    return Json(new { success = false, message = "Email veya şifre hatalı!" });
                }

                // Session'a kullanıcı bilgilerini kaydet
                HttpContext.Session.SetInt32("KullaniciId", kullanici.Id);
                HttpContext.Session.SetString("KullaniciAd", kullanici.Ad);
                HttpContext.Session.SetString("KullaniciSoyad", kullanici.Soyad);
                HttpContext.Session.SetString("KullaniciRol", kullanici.AdminMi ? "Admin" : "User");

                Console.WriteLine($"Giriş başarılı - Rol: {(kullanici.AdminMi ? "Admin" : "User")}");

                // Admin ise admin paneline yönlendir
                if (kullanici.AdminMi)
                {
                    return Json(new { success = true, redirectUrl = Url.Action("Index", "Admin") });
                }

                return Json(new { success = true, redirectUrl = Url.Action("Index", "Home") });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Hata oluştu: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return Json(new { success = false, message = "Giriş yapılırken bir hata oluştu: " + ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult KayitOl([FromForm] Kullanici model)
        {
            try
            {
                // Form verilerini logla
                Console.WriteLine($"Gelen form verileri: Ad={model.Ad}, Soyad={model.Soyad}, Eposta={model.Eposta}");

                // Model durumunu kontrol et
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                    var errorMessage = string.Join(", ", errors);
                    Console.WriteLine($"Model validation hataları: {errorMessage}");
                    TempData["Hata"] = errorMessage;
                    return RedirectToAction("Giris");
                }

                // E-posta kontrolü
                if (string.IsNullOrEmpty(model.Eposta))
                {
                    TempData["Hata"] = "E-posta adresi gereklidir.";
                    return RedirectToAction("Giris");
                }

                // E-posta benzersizlik kontrolü
                var existingUser = _context.Kullanicilar.FirstOrDefault(k => k.Eposta == model.Eposta.Trim().ToLower());
                if (existingUser != null)
                {
                    TempData["Hata"] = "Bu e-posta adresi zaten kayıtlı!";
                    return RedirectToAction("Giris");
                }

                // Şifre kontrolü
                if (string.IsNullOrEmpty(model.Sifre) || model.Sifre.Length < 6)
                {
                    TempData["Hata"] = "Şifre en az 6 karakter olmalıdır.";
                    return RedirectToAction("Giris");
                }

                // Yeni kullanıcı oluştur
                var yeniKullanici = new Kullanici
                {
                    Ad = FormatlaIsim(model.Ad ?? ""),
                    Soyad = FormatlaIsim(model.Soyad ?? ""),
                    Eposta = model.Eposta.Trim().ToLower(),
                    Sifre = HashPassword(model.Sifre),
                    Telefon = model.Telefon,
                    KayitTarihi = DateTime.Now,
                    AdminMi = false
                };

                try
                {
                    // Veritabanı işlemini logla
                    Console.WriteLine("Veritabanına kullanıcı ekleniyor...");
                    _context.Kullanicilar.Add(yeniKullanici);
                    _context.SaveChanges();

                    Console.WriteLine($"Kullanıcı başarıyla eklendi. ID: {yeniKullanici.Id}");
                    TempData["Basari"] = "Kayıt başarılı! Şimdi giriş yapabilirsiniz.";
                    return RedirectToAction("Giris");
                }
                catch (Exception dbEx)
                {
                    Console.WriteLine($"Veritabanı hatası: {dbEx.Message}");
                    Console.WriteLine($"Inner Exception: {dbEx.InnerException?.Message}");
                    throw; // Üst catch bloğuna gönder
                }
            }
            catch (Exception ex)
            {
                // Detaylı hata loglaması
                Console.WriteLine($"HATA - Kayıt işlemi başarısız:");
                Console.WriteLine($"Hata Mesajı: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                Console.WriteLine($"Inner Exception: {ex.InnerException?.Message}");
                
                TempData["Hata"] = "Kayıt sırasında bir hata oluştu. Lütfen tekrar deneyin.";
                return RedirectToAction("Giris");
            }
        }

        public IActionResult CikisYap()
        {
            var kullaniciId = HttpContext.Session.GetInt32("KullaniciId");
            if (kullaniciId.HasValue)
            {
                // Kullanıcının sepetini temizle
                var sepetItems = _context.SepetUrunleri.Where(s => s.KullaniciId == kullaniciId);
                _context.SepetUrunleri.RemoveRange(sepetItems);
                _context.SaveChanges();
            }

            // Session'ı temizle
            HttpContext.Session.Clear();
            return RedirectToAction("Giris");
        }

        private string HashPassword(string sifre)
        {
            if (string.IsNullOrEmpty(sifre)) return string.Empty;

            using (var md5 = MD5.Create())
            {
                var inputBytes = Encoding.UTF8.GetBytes(sifre);
                var hashBytes = md5.ComputeHash(inputBytes);
                var sb = new StringBuilder();
                
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("x2"));
                }
                
                return sb.ToString();
            }
        }

        // Profil işlemleri
        public IActionResult Profilim()
        {
            var kullaniciId = HttpContext.Session.GetInt32("KullaniciId");
            if (kullaniciId == null)
            {
                return RedirectToAction("Giris");
            }

            var kullanici = _context.Kullanicilar.Find(kullaniciId);
            if (kullanici == null)
            {
                return RedirectToAction("Giris");
            }

            return View(kullanici);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProfilGuncelle(Kullanici model, string? yeniSifre = null, IFormFile? profilePhoto = null)
        {
            try
            {
                var kullaniciId = HttpContext.Session.GetInt32("KullaniciId");
                if (kullaniciId == null)
                {
                    TempData["Hata"] = "Kullanıcı bulunamadı.";
                    return RedirectToAction("Giris");
                }

                var kullanici = _context.Kullanicilar.Find(kullaniciId);
                if (kullanici == null)
                {
                    TempData["Hata"] = "Kullanıcı bulunamadı.";
                    return RedirectToAction("Giris");
                }

                // E-posta değişikliği varsa ve yeni e-posta başka bir kullanıcıda varsa kontrol et
                if (!string.IsNullOrEmpty(model.Eposta) && 
                    model.Eposta != kullanici.Eposta && 
                    _context.Kullanicilar.Any(k => k.Eposta == model.Eposta && k.Id != model.Id))
                {
                    TempData["Hata"] = "Bu e-posta adresi başka bir kullanıcı tarafından kullanılıyor.";
                    return RedirectToAction("Profilim");
                }

                // Profil fotoğrafı yükleme
                if (profilePhoto != null && profilePhoto.Length > 0)
                {
                    // Dosya uzantısını kontrol et
                    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                    var extension = Path.GetExtension(profilePhoto.FileName).ToLowerInvariant();
                    
                    if (!allowedExtensions.Contains(extension))
                    {
                        TempData["Hata"] = "Sadece .jpg, .jpeg, .png ve .gif uzantılı dosyalar yüklenebilir.";
                        return RedirectToAction("Profilim");
                    }

                    // Dosya boyutunu kontrol et (max 5MB)
                    if (profilePhoto.Length > 5 * 1024 * 1024)
                    {
                        TempData["Hata"] = "Dosya boyutu 5MB'dan büyük olamaz.";
                        return RedirectToAction("Profilim");
                    }

                    try
                    {
                        // Eski fotoğrafı sil
                        if (!string.IsNullOrEmpty(kullanici.ProfilFotoUrl))
                        {
                            var oldImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", kullanici.ProfilFotoUrl.TrimStart('/'));
                            if (System.IO.File.Exists(oldImagePath))
                            {
                                System.IO.File.Delete(oldImagePath);
                            }
                        }

                        // Yeni fotoğrafı kaydet
                        var uniqueFileName = $"{Guid.NewGuid()}{extension}";
                        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "profiles");
                        
                        // Klasör yoksa oluştur
                        if (!Directory.Exists(uploadsFolder))
                        {
                            Directory.CreateDirectory(uploadsFolder);
                        }

                        var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await profilePhoto.CopyToAsync(fileStream);
                        }

                        kullanici.ProfilFotoUrl = $"/uploads/profiles/{uniqueFileName}";
                    }
                    catch (Exception ex)
                    {
                        TempData["Hata"] = "Fotoğraf yüklenirken bir hata oluştu: " + ex.Message;
                        return RedirectToAction("Profilim");
                    }
                }

                // Ad ve Soyad alanlarını düzenle
                kullanici.Ad = FormatlaIsim(model.Ad ?? kullanici.Ad);
                kullanici.Soyad = FormatlaIsim(model.Soyad ?? kullanici.Soyad);

                // Diğer alanları güncelle
                if (!string.IsNullOrEmpty(model.Eposta))
                {
                    kullanici.Eposta = model.Eposta.Trim().ToLower();
                }

                if (!string.IsNullOrEmpty(model.Telefon))
                {
                    kullanici.Telefon = model.Telefon;
                }

                if (!string.IsNullOrEmpty(model.Adres))
                {
                    kullanici.Adres = model.Adres;
                }

                // Şifre değişikliği varsa
                if (!string.IsNullOrEmpty(yeniSifre))
                {
                    if (yeniSifre.Length < 6)
                    {
                        TempData["Hata"] = "Yeni şifre en az 6 karakter olmalıdır.";
                        return RedirectToAction("Profilim");
                    }
                    kullanici.Sifre = HashPassword(yeniSifre);
                }

                _context.Update(kullanici);
                await _context.SaveChangesAsync();

                // Session'daki kullanıcı adını güncelle
                var yeniKullaniciAdi = $"{kullanici.Ad} {kullanici.Soyad}".Trim();
                if (!string.IsNullOrEmpty(yeniKullaniciAdi))
                {
                    HttpContext.Session.SetString("KullaniciAdi", yeniKullaniciAdi);
                }

                TempData["Basari"] = "Profil bilgileriniz başarıyla güncellendi.";
                return RedirectToAction("Profilim");
            }
            catch (Exception ex)
            {
                TempData["Hata"] = "Güncelleme sırasında bir hata oluştu: " + ex.Message;
                return RedirectToAction("Profilim");
            }
        }

        public IActionResult HesapSil()
        {
            var kullaniciId = HttpContext.Session.GetInt32("KullaniciId");
            if (kullaniciId == null)
            {
                return RedirectToAction("Giris");
            }

            try
            {
                var kullanici = _context.Kullanicilar.Find(kullaniciId);
                if (kullanici != null)
                {
                    // Önce kullanıcının sepetini temizle
                    var sepetItems = _context.SepetUrunleri.Where(s => s.KullaniciId == kullaniciId);
                    _context.SepetUrunleri.RemoveRange(sepetItems);

                    // Kullanıcıyı sil
                    _context.Kullanicilar.Remove(kullanici);
                    _context.SaveChanges();

                    // Session'ı temizle
                    HttpContext.Session.Clear();

                    TempData["Basari"] = "Hesabınız başarıyla silindi.";
                    return RedirectToAction("Giris");
                }

                TempData["Hata"] = "Hesap silinemedi: Kullanıcı bulunamadı.";
                return RedirectToAction("Profilim");
            }
            catch (Exception ex)
            {
                TempData["Hata"] = "Hesap silme sırasında bir hata oluştu: " + ex.Message;
                return RedirectToAction("Profilim");
            }
        }

        // Geçicişifre sıfırlama
        public IActionResult SifreSifirla(string eposta)
        {
            var kullanici = _context.Kullanicilar.FirstOrDefault(k => k.Eposta == eposta);
            if (kullanici != null)
            {
                kullanici.Sifre = HashPassword("123456"); // Geçici şifre
                _context.SaveChanges();
                TempData["Basari"] = "Şifreniz '123456' olarak değiştirildi.";
            }
            return RedirectToAction("Giris");
        }

        public async Task<IActionResult> Siparislerim()
        {
            var kullaniciId = HttpContext.Session.GetInt32("KullaniciId");
            if (kullaniciId == null)
            {
                return RedirectToAction("Giris");
            }

            var siparisler = await _context.Siparisler
                .Include(s => s.SiparisDetaylari)
                    .ThenInclude(sd => sd.Urun)
                .Include(s => s.SiparisGorselleri)
                .Where(s => s.KullaniciId == kullaniciId)
                .OrderByDescending(s => s.SiparisTarihi)
                .ToListAsync();

            return View(siparisler);
        }

        [HttpGet]
        public IActionResult AdminOlustur()
        {
            try
            {
                // Admin kullanıcısı var mı kontrol et
                var adminVarMi = _context.Kullanicilar.Any(k => k.Eposta == "admin@peracraft.com");
                if (adminVarMi)
                {
                    return Json(new { success = false, message = "Admin kullanıcısı zaten mevcut." });
                }

                // Admin kullanıcısını oluştur
                var admin = new Kullanici
                {
                    Eposta = "admin@peracraft.com",
                    Sifre = HashPassword("123456"), // Şifreyi hashle
                    Ad = "Admin",
                    Soyad = "User",
                    AdminMi = true,
                    AktifMi = true
                };

                _context.Kullanicilar.Add(admin);
                _context.SaveChanges();

                return Json(new { success = true, message = "Admin kullanıcısı oluşturuldu. Email: admin@peracraft.com, Şifre: 123456" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Admin kullanıcısı oluşturulurken bir hata oluştu: " + ex.Message });
            }
        }
    }
} 