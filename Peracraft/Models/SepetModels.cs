using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Peracraft.Models
{
    public class SepetItem
    {
        [Key]
        public int UrunId { get; set; }
        public int KullaniciId { get; set; }
        public int Miktar { get; set; }

        public virtual Kullanici? Kullanici { get; set; }
        public virtual Urun? Urun { get; set; }

        [NotMapped]
        public string UrunAdi => Urun?.UrunAdi ?? string.Empty;

        [NotMapped]
        public string ResimUrl => Urun?.ResimUrl ?? string.Empty;

        [NotMapped]
        public decimal Fiyat => Urun?.Fiyat ?? 0;
    }

    public class SepetViewModel
    {
        public List<SepetItem> Items { get; set; } = new List<SepetItem>();
        public decimal ToplamTutar { get; set; }
    }

    public class SepeteEkleModel
    {
        public int UrunId { get; set; }
        public int Miktar { get; set; }
    }

    public class SepetGuncelleModel
    {
        public int UrunId { get; set; }
        public int Miktar { get; set; }
    }
} 