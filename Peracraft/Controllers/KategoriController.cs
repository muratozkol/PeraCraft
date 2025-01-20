using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Peracraft.Data;
using Peracraft.Models;

namespace Peracraft.Controllers
{
    public class KategoriController : Controller
    {
        private readonly ApplicationDbContext _context;

        public KategoriController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Detay(int id)
        {
            var kategori = await _context.Kategoriler
                .FirstOrDefaultAsync(k => k.Id == id);

            if (kategori == null)
            {
                return NotFound();
            }

            var urunler = await _context.Urunler
                .Where(u => u.KategoriId == id)
                .OrderByDescending(u => u.EklenmeTarihi)
                .ToListAsync();

            ViewBag.KategoriAdi = kategori.Ad;
            ViewBag.KategoriAciklama = kategori.Aciklama;

            return View(urunler);
        }
    }
} 