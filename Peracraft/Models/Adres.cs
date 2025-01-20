using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Peracraft.Models
{
    public class Adres
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Adres başlığı zorunludur")]
        [StringLength(50, ErrorMessage = "Adres başlığı en fazla 50 karakter olabilir")]
        [Display(Name = "Adres Başlığı")]
        public string AdresBasligi { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ad Soyad zorunludur")]
        [StringLength(100, ErrorMessage = "Ad Soyad en fazla 100 karakter olabilir")]
        [Display(Name = "Ad Soyad")]
        public string AdSoyad { get; set; } = string.Empty;

        [Required(ErrorMessage = "Telefon numarası zorunludur")]
        [StringLength(20, ErrorMessage = "Telefon numarası en fazla 20 karakter olabilir")]
        [Display(Name = "Telefon")]
        public string Telefon { get; set; } = string.Empty;

        [Required(ErrorMessage = "İl zorunludur")]
        [StringLength(50, ErrorMessage = "İl en fazla 50 karakter olabilir")]
        [Display(Name = "İl")]
        public string Il { get; set; } = string.Empty;

        [Required(ErrorMessage = "İlçe zorunludur")]
        [StringLength(50, ErrorMessage = "İlçe en fazla 50 karakter olabilir")]
        [Display(Name = "İlçe")]
        public string Ilce { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mahalle zorunludur")]
        [StringLength(100, ErrorMessage = "Mahalle en fazla 100 karakter olabilir")]
        [Display(Name = "Mahalle")]
        public string Mahalle { get; set; } = string.Empty;

        [Required(ErrorMessage = "Açık adres zorunludur")]
        [StringLength(500, ErrorMessage = "Açık adres en fazla 500 karakter olabilir")]
        [Display(Name = "Açık Adres")]
        public string AcikAdres { get; set; } = string.Empty;

        [StringLength(10, ErrorMessage = "Posta kodu en fazla 10 karakter olabilir")]
        [Display(Name = "Posta Kodu")]
        public string? PostaKodu { get; set; }

        [Display(Name = "Varsayılan Adres")]
        public bool VarsayilanMi { get; set; } = false;

        [Required]
        public int KullaniciId { get; set; }

        [ForeignKey("KullaniciId")]
        public virtual Kullanici Kullanici { get; set; } = null!;

        public DateTime OlusturulmaTarihi { get; set; } = DateTime.Now;
        public DateTime? GuncellenmeTarihi { get; set; }

        // Navigation property for Siparisler
        public virtual ICollection<Siparis> Siparisler { get; set; } = new List<Siparis>();
    }
} 