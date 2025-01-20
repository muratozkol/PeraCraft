using Peracraft.Models;
using System.Collections.Generic;

namespace Peracraft.Services
{
    public interface IUrunService
    {
        List<Urun> GetAll();
        Urun GetById(int id);
        List<Urun> GetByKategoriId(int kategoriId);
    }
} 