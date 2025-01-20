using System.Collections.Generic;

namespace Peracraft.Models
{
    public class SiparislerimViewModel
    {
        public List<Siparis> Siparisler { get; set; } = new List<Siparis>();
        public List<SiparisDetay> SiparisDetaylari { get; set; } = new List<SiparisDetay>();
        public List<SiparisGorsel> SiparisGorselleri { get; set; } = new List<SiparisGorsel>();
        public decimal ToplamSiparisTutari => Siparisler.Sum(s => s.ToplamTutar);
        public int ToplamSiparisSayisi => Siparisler.Count;
    }
} 