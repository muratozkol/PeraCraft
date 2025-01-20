# Peracraft E-Ticaret Projesi

## 📋 Proje Özeti
Peracraft, modern web teknolojileri kullanılarak geliştirilmiş kapsamlı bir e-ticaret platformudur. Kullanıcılar için kolay ve güvenli bir alışveriş deneyimi sunarken, yöneticiler için etkili bir yönetim paneli sağlar.

---

## 🎯 Temel Özellikler

### 👤 Kullanıcı Yönetimi
- Kayıt ve Giriş Sistemi
- Profil Yönetimi
- Adres Yönetimi
- Sipariş Takibi

### 🛍️ Alışveriş Özellikleri
- Ürün Listeleme ve Filtreleme
- Sepet İşlemleri
- Sipariş Oluşturma
- Ödeme Sistemi

### 👨‍💼 Admin Paneli
- Ürün Yönetimi
- Kategori Yönetimi
- Sipariş Yönetimi
- Kullanıcı Yönetimi

---

## 🛠️ Kullanılan Teknolojiler

### Backend Teknolojileri
- **.NET 6.0**: Modern ve hızlı web uygulamaları için geliştirme platformu.
- **ASP.NET Core MVC**: Model-View-Controller mimarisi.
- **Entity Framework Core**: ORM (Object-Relational Mapping) aracı.
- **MySQL**: Veritabanı yönetim sistemi.
- **LINQ**: Veri sorgulama ve manipülasyon.
- **C# 10**: Programlama dili.

### Frontend Teknolojileri
- **HTML5/CSS3**: Sayfa yapısı ve stillendirme.
- **JavaScript/jQuery**: Dinamik kullanıcı deneyimi.
- **Bootstrap 5**: Responsive tasarım framework'ü.
- **AJAX**: Asenkron veri transferi.
- **Font Awesome**: İkon kütüphanesi.

### Güvenlik
- **Session Yönetimi**: Güvenli oturum işlemleri.
- **MD5 Şifreleme**: Parola şifreleme.
- **Anti-Forgery Token**: CSRF koruması.
- **Veri Doğrulama**: Veri bütünlüğünü sağlama.

---

## 🚀 Kurulum

### Gereksinimler
- .NET 6.0 SDK
- MySQL Server 8.0
- Visual Studio 2022 veya VS Code

### Veritabanı Kurulumu
1. MySQL Server'ı kurun.
2. Yeni bir veritabanı oluşturun:
   ```sql
   CREATE DATABASE peracraft_db;
   ```
3. `appsettings.json` dosyasındaki bağlantı ayarlarını güncelleyin.

### Proje Kurulumu
1. Depoyu klonlayın:
   ```bash
   git clone <depo-linki>
   cd Peracraft
   ```
2. Migration'ları uygulayın:
   ```bash
   dotnet ef database update
   ```
3. Projeyi çalıştırın:
   ```bash
   dotnet run
   ```
4. Tarayıcınızda açın:
   ```
   http://localhost:5152
   ```

---

## 🛡️ Güvenlik Önlemleri
- Şifre şifreleme (MD5)
- CSRF koruması
- Session yönetimi
- Veri doğrulama
- XSS koruması
- SQL Injection koruması

---

## 📦 Proje Yapısı
- **Models**: Veri modelleri.
- **Controllers**: Uygulama mantığı.
- **Views**: Frontend şablonları.
- **Migrations**: Veritabanı migration'ları.

---

## 🌟 Özellikler ve Yetenekler
- Responsive tasarım.
- SEO dostu URL yapısı.
- Hızlı sayfa yükleme.
- Kullanıcı dostu yönetim paneli.
- Güvenli ödeme sistemi.
- Detaylı raporlama.

---

## 📝 Lisans
Bu proje [MIT Lisansı](LICENSE) altında lisanslanmıştır.

---

## 👥 Katkıda Bulunanlar
[MURAT ÖZKOL]
[ALİ ŞEYHO]

---

## 📞 İletişim
[İletişim bilgileri]

