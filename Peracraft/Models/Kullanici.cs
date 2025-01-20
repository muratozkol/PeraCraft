using System.ComponentModel.DataAnnotations;

namespace Peracraft.Models
{
    public class Kullanici
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Ad zorunludur.")]
        [StringLength(50, ErrorMessage = "Ad en fazla 50 karakter olabilir.")]
        public string Ad { get; set; } = string.Empty;

        [Required(ErrorMessage = "Soyad zorunludur.")]
        [StringLength(50, ErrorMessage = "Soyad en fazla 50 karakter olabilir.")]
        public string Soyad { get; set; } = string.Empty;

        [Required(ErrorMessage = "E-posta zorunludur.")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz.")]
        [StringLength(100, ErrorMessage = "E-posta en fazla 100 karakter olabilir.")]
        public string Eposta { get; set; } = string.Empty;

        [Required(ErrorMessage = "Şifre zorunludur.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Şifre en az 6, en fazla 100 karakter olmalıdır.")]
        public string Sifre { get; set; } = string.Empty;

        [StringLength(20)]
        public string? Telefon { get; set; }

        [StringLength(200)]
        public string? Adres { get; set; }

        [StringLength(255)]
        public string? ProfilFotoUrl { get; set; }

        public bool AktifMi { get; set; } = true;

        public bool AdminMi { get; set; } = false;

        public DateTime KayitTarihi { get; set; } = DateTime.Now;

        public DateTime? SonGirisTarihi { get; set; }

        // Navigation properties
        public virtual ICollection<Siparis> Siparisler { get; set; } = new List<Siparis>();
        public virtual ICollection<SepetItem> SepetUrunleri { get; set; } = new List<SepetItem>();
        public virtual ICollection<Adres> Adresler { get; set; } = new List<Adres>();
    }
} 