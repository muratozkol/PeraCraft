using Peracraft.Models;
using Peracraft.Data;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Peracraft.Services
{
    public class UrunService : IUrunService
    {
        private readonly ApplicationDbContext _context;

        public UrunService(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Urun> GetAll()
        {
            return _context.Urunler.ToList();
        }

        public Urun GetById(int id)
        {
            return _context.Urunler.FirstOrDefault(u => u.UrunId == id);
        }

        public List<Urun> GetByKategoriId(int kategoriId)
        {
            return _context.Urunler.Where(u => u.KategoriId == kategoriId).ToList();
        }
    }
} 