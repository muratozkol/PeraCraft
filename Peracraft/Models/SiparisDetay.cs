using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Peracraft.Models
{
    public class SiparisDetay
    {
        [Key]
        public int SiparisDetayId { get; set; }

        [Required]
        public int SiparisId { get; set; }

        [Required]
        public int UrunId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Miktar en az 1 olmalıdır.")]
        public int Miktar { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal BirimFiyat { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal ToplamFiyat { get; set; }

        // Navigation properties
        [ForeignKey("SiparisId")]
        public virtual Siparis? Siparis { get; set; }

        [ForeignKey("UrunId")]
        public virtual Urun? Urun { get; set; }
    }
} 