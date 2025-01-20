using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Peracraft.Data;
using Peracraft.Models;

namespace Peracraft.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var siparisler = await _context.Siparisler
                .Include(s => s.Kullanici)
                .Include(s => s.SiparisDetaylari)
                .Include(s => s.SiparisGorselleri)
                .OrderByDescending(s => s.SiparisTarihi)
                .ToListAsync();

            return View(siparisler);
        }

        public async Task<IActionResult> SiparisDetay(int id)
        {
            var siparis = await _context.Siparisler
                .Include(s => s.Kullanici)
                .Include(s => s.SiparisDetaylari)
                    .ThenInclude(sd => sd.Urun)
                .Include(s => s.SiparisGorselleri)
                .FirstOrDefaultAsync(s => s.SiparisId == id);

            if (siparis == null)
            {
                return NotFound();
            }

            return View(siparis);
        }

        [HttpPost]
        public async Task<IActionResult> DurumGuncelle(int siparisId, string yeniDurum)
        {
            var siparis = await _context.Siparisler.FindAsync(siparisId);
            
            if (siparis == null)
            {
                return Json(new { success = false, message = "Sipariş bulunamadı." });
            }

            siparis.SiparisDurumu = yeniDurum;
            await _context.SaveChangesAsync();

            return Json(new { 
                success = true, 
                message = "Sipariş durumu güncellendi.",
                yeniDurum = yeniDurum
            });
        }
    }
} 