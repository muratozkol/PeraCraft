namespace Peracraft.Models
{
    public class SiparisDetayViewModel
    {
        public int UrunId { get; set; }
        public string UrunAdi { get; set; } = string.Empty;
        public string ResimUrl { get; set; } = string.Empty;
        public int Miktar { get; set; }
        public decimal BirimFiyat { get; set; }
        public decimal ToplamFiyat { get; set; }
    }
} 