using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace Peracraft.Models
{
    public class SiparisViewModel
    {
        // Teslimat Bilgileri
        public string? AdSoyad { get; set; }
        public string? Email { get; set; }
        public string? Telefon { get; set; }

        // Adres Bilgileri
        public string? Adres { get; set; }
        public string? Sehir { get; set; }
        public string? PostaKodu { get; set; }

        // Ödeme Bilgileri
        public string? OdemeTipi { get; set; }
        public string? KartNumarasi { get; set; }
        public string? SonKullanmaTarihi { get; set; }
        public string? Cvv { get; set; }
        public string? KartSahibi { get; set; }

        // Sepet Bilgileri
        public List<SepetItem> SepetUrunleri { get; set; }
        public decimal AraToplam { get; set; }
        public decimal KargoUcreti { get; set; }
        public decimal ToplamTutar { get; set; }

        // Sipariş Görselleri
        public List<IFormFile>? SiparisGorselleri { get; set; }

        public SiparisViewModel()
        {
            SepetUrunleri = new List<SepetItem>();
            SiparisGorselleri = new List<IFormFile>();
        }
    }
} 