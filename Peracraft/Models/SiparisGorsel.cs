namespace Peracraft.Models
{
    public class SiparisGorsel
    {
        public int Id { get; set; }
        public int SiparisId { get; set; }
        public string DosyaYolu { get; set; } = string.Empty;
        public DateTime YuklenmeTarihi { get; set; } = DateTime.Now;

        // Navigation property
        public virtual Siparis Siparis { get; set; } = null!;
    }
} 