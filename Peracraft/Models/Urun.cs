using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Peracraft.Models
{
    public class Urun
    {
        [Key]
        public int UrunId { get; set; }

        [Required(ErrorMessage = "Ürün adı zorunludur.")]
        [StringLength(200, ErrorMessage = "Ürün adı en fazla 200 karakter olabilir.")]
        public string UrunAdi { get; set; } = string.Empty;

        [StringLength(1000, ErrorMessage = "Açıklama en fazla 1000 karakter olabilir.")]
        public string? Aciklama { get; set; }

        [Required(ErrorMessage = "Fiyat zorunludur.")]
        [Column(TypeName = "decimal(18,2)")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Fiyat 0'dan büyük olmalıdır.")]
        public decimal Fiyat { get; set; }

        [Required(ErrorMessage = "Stok miktarı zorunludur.")]
        [Range(0, int.MaxValue, ErrorMessage = "Stok miktarı negatif olamaz.")]
        public int StokMiktari { get; set; }

        [StringLength(200)]
        public string? ResimUrl { get; set; }

        public bool AktifMi { get; set; } = true;

        public DateTime EklenmeTarihi { get; set; } = DateTime.Now;

        public DateTime? GuncellenmeTarihi { get; set; }

        // Foreign key
        [Required(ErrorMessage = "Kategori seçimi zorunludur.")]
        public int KategoriId { get; set; }

        // Navigation properties
        [ForeignKey("KategoriId")]
        public virtual Kategori? Kategori { get; set; }

        public virtual ICollection<SepetItem> SepetUrunleri { get; set; } = new List<SepetItem>();
        public virtual ICollection<SiparisDetay> SiparisDetaylari { get; set; } = new List<SiparisDetay>();
    }
} 