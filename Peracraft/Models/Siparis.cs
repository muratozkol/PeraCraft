using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Peracraft.Models
{
    public class Siparis
    {
        [Key]
        public int SiparisId { get; set; }
        public string SiparisKodu { get; set; } = Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper();
        public int KullaniciId { get; set; }
        public int AdresId { get; set; }
        public string TeslimatAdresi { get; set; } = string.Empty;
        public string TeslimatTelefonu { get; set; } = string.Empty;
        public string? SiparisNotu { get; set; }
        public decimal ToplamTutar { get; set; }
        public DateTime SiparisTarihi { get; set; } = DateTime.Now;
        public string SiparisDurumu { get; set; } = "Beklemede";
        public string OdemeTipi { get; set; } = "creditCard";

        // Navigation properties
        public virtual Kullanici Kullanici { get; set; } = null!;
        public virtual Adres Adres { get; set; } = null!;
        public virtual ICollection<SiparisDetay> SiparisDetaylari { get; set; } = new List<SiparisDetay>();
        public virtual ICollection<SiparisGorsel> SiparisGorselleri { get; set; } = new List<SiparisGorsel>();
    }
} 