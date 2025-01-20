using System.ComponentModel.DataAnnotations;

namespace Peracraft.Models
{
    public class Kategori
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Kategori adı zorunludur.")]
        [StringLength(100, ErrorMessage = "Kategori adı en fazla 100 karakter olabilir.")]
        public string Ad { get; set; } = string.Empty;

        public string? Aciklama { get; set; }

        public string? ResimUrl { get; set; }

        public bool AktifMi { get; set; } = true;

        public DateTime EklenmeTarihi { get; set; } = DateTime.Now;

        public DateTime? GuncellenmeTarihi { get; set; }

        // Navigation property
        public virtual ICollection<Urun> Urunler { get; set; } = new List<Urun>();
    }
} 