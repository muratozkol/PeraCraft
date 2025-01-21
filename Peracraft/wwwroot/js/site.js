// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

let sliderPositions = {};

function initializeSliders() {
    const sections = document.querySelectorAll('.product-slider-section');
    sections.forEach(section => {
        const track = section.querySelector('.slider-track');
        const slides = track.querySelectorAll('.product-card');
        const dots = section.querySelectorAll('.slider-dot');
        
        // İlk slaytı ortala
        track.style.transition = 'none'; // İlk yüklemede transition'ı kapat
        centerSlide(section, 0);
        
        // Force reflow
        track.offsetHeight;
        
        // Transition'ı geri aç
        track.style.transition = 'transform 0.5s cubic-bezier(0.4, 0.0, 0.2, 1)';
        
        // Dot'lara tıklama olayı ekle
        dots.forEach((dot, index) => {
            dot.addEventListener('click', () => {
                centerSlide(section, index);
            });
        });
        
        // Touch ve mouse olaylarını ekle
        let isDown = false;
        let startX;
        let scrollLeft;
        let startTime;
        let startPosition;
        
        track.addEventListener('mousedown', e => {
            if (e.target.closest('.quantity-btn') || 
                e.target.closest('.shop-now-btn') || 
                e.target.closest('.slider-arrow')) {
                return;
            }
            
            isDown = true;
            track.style.transition = 'none'; // Sürükleme sırasında transition'ı kapat
            track.style.cursor = 'grabbing';
            startX = e.pageX;
            startPosition = getCurrentTranslate(track);
            scrollLeft = startPosition;
            startTime = Date.now();
        });

        track.addEventListener('mousemove', e => {
            if (!isDown) return;
            e.preventDefault();
            const x = e.pageX;
            const walk = (x - startX);
            track.style.transform = `translateX(${scrollLeft + walk}px)`;
        });

        const handleDragEnd = () => {
            if (!isDown) return;
            isDown = false;
            track.style.cursor = 'grab';
            
            const endTime = Date.now();
            const timeElapsed = endTime - startTime;
            const endPosition = getCurrentTranslate(track);
            const distance = endPosition - startPosition;
            
            // Hız bazlı animasyon için
            const velocity = Math.abs(distance / timeElapsed);
            track.style.transition = `transform ${0.3 + velocity * 0.2}s cubic-bezier(0.4, 0.0, 0.2, 1)`;
            
            snapToNearestSlide(section);
        };

        track.addEventListener('mouseleave', handleDragEnd);
        track.addEventListener('mouseup', handleDragEnd);

        // Touch olayları
        track.addEventListener('touchstart', e => {
            if (e.target.closest('.quantity-btn') || 
                e.target.closest('.shop-now-btn') || 
                e.target.closest('.slider-arrow')) {
                return;
            }
            
            isDown = true;
            track.style.transition = 'none';
            startX = e.touches[0].pageX;
            startPosition = getCurrentTranslate(track);
            scrollLeft = startPosition;
            startTime = Date.now();
        }, { passive: false });

        track.addEventListener('touchmove', e => {
            if (!isDown) return;
            e.preventDefault();
            const x = e.touches[0].pageX;
            const walk = (x - startX);
            track.style.transform = `translateX(${scrollLeft + walk}px)`;
        }, { passive: false });

        track.addEventListener('touchend', handleDragEnd);
    });
}

function getCurrentTranslate(track) {
    const style = window.getComputedStyle(track);
    const matrix = new WebKitCSSMatrix(style.transform);
    return matrix.m41;
}

function getCurrentIndex(section) {
    const dots = section.querySelectorAll('.slider-dot');
    return Array.from(dots).findIndex(dot => dot.classList.contains('active'));
}

function centerSlide(section, index) {
    const track = section.querySelector('.slider-track');
    const slides = track.querySelectorAll('.product-card');
    const slideWidth = slides[0].offsetWidth + (window.innerWidth <= 768 ? 20 : 30);
    const containerWidth = section.offsetWidth;
    
    const centerPosition = (containerWidth / 2) - (slideWidth / 2);
    const slideOffset = index * slideWidth;
    const position = centerPosition - slideOffset;
    
    // Transition süresini artır ve easing fonksiyonunu değiştir
    track.style.transition = 'transform 0.5s cubic-bezier(0.4, 0.0, 0.2, 1)';
    track.style.transform = `translateX(${position}px)`;
    
    updateDots(section, index);
}

function snapToNearestSlide(section) {
    const track = section.querySelector('.slider-track');
    const slides = track.querySelectorAll('.product-card');
    const slideWidth = slides[0].offsetWidth + (window.innerWidth <= 768 ? 20 : 30);
    const currentPosition = getCurrentTranslate(track);
    const containerWidth = section.offsetWidth;
    
    const centerOffset = (containerWidth / 2) - (slideWidth / 2);
    const relativePosition = centerOffset - currentPosition;
    
    const nearestIndex = Math.round(relativePosition / slideWidth);
    const finalIndex = Math.max(0, Math.min(nearestIndex, slides.length - 1));
    
    // Transition süresini artır ve easing fonksiyonunu değiştir
    track.style.transition = 'transform 0.5s cubic-bezier(0.4, 0.0, 0.2, 1)';
    centerSlide(section, finalIndex);
}

function updateDots(section, activeIndex) {
    const dots = section.querySelectorAll('.slider-dot');
    dots.forEach((dot, index) => {
        if (index === activeIndex) {
            dot.classList.add('active');
        } else {
            dot.classList.remove('active');
        }
    });
}

// Sepete ekleme fonksiyonu
function addToCart(urunId) {
    const quantityElement = document.getElementById(`quantity-${urunId}`);
    const quantity = parseInt(quantityElement?.textContent || '1');
    const token = $('input[name="__RequestVerificationToken"]').val();

    $.ajax({
        url: '/Sepet/SepeteEkle',
        type: 'POST',
        contentType: 'application/json',
        headers: {
            'RequestVerificationToken': token
        },
        data: JSON.stringify({
            urunId: urunId,
            miktar: quantity
        }),
        success: function(response) {
            if (response.success) {
                // Sepet sayısını animasyonlu güncelle
                refreshCartCount();
                
                // Bildirim göster
                showNotification();
                
                // Miktarı sıfırla
                if (quantityElement) {
                    quantityElement.textContent = '1';
                }
            } else {
                if (response.message === "Oturum açmanız gerekiyor.") {
                    window.location.href = '/Hesap/Giris';
                } else {
                    alert(response.message || 'Ürün sepete eklenirken bir hata oluştu.');
                }
            }
        },
        error: function(xhr, status, error) {
            console.error('Sepete ekleme hatası:', error);
            alert('Bir hata oluştu. Lütfen tekrar deneyin.');
        }
    });
}

// Bildirim gösterme fonksiyonu
function showNotification() {
    $('.cart-notification').remove();
    
    const notification = $(`
        <div class="cart-notification">
            <i class="fas fa-check-circle"></i>
            <span>Ürün sepete eklendi!</span>
            <a href="/Sepet" class="view-cart-btn">
                <i class="fas fa-shopping-cart"></i>
                Sepeti Gör
            </a>
        </div>
    `);
    
    $('body').append(notification);
    setTimeout(() => notification.addClass('show'), 100);
    setTimeout(() => {
        notification.removeClass('show');
        setTimeout(() => notification.remove(), 300);
    }, 3000);
}

// Miktar güncelleme fonksiyonu
function updateQuantity(urunId, change) {
    const quantityElement = $(`#quantity-${urunId}`);
    if (quantityElement.length) {
        let quantity = parseInt(quantityElement.text());
        quantity = Math.max(1, quantity + change);
        quantityElement.text(quantity);
    }
}

// Sepet sayısını güncelleme
function refreshCartCount() {
    $.get('/Sepet/GetSepetUrunSayisi', function(count) {
        const badge = $('.badge-modern');
        badge.text(count);
        
        if (count > 0) {
            // Sayı varsa animasyonlu göster
            badge.css('transform', 'scale(1.2)');
            setTimeout(() => {
                badge.css('transform', 'scale(1)');
            }, 200);
        }
        
        // Sayı 0 ise gizle, değilse göster
        badge.toggle(count > 0);
    });
}

// Sayfa yüklendiğinde
document.addEventListener('DOMContentLoaded', function() {
    initializeSliders();
    refreshCartCount();
});

// Sayfa görünür olduğunda sepet sayısını güncelle
$(document).on('visibilitychange', function() {
    if (document.visibilityState === 'visible') {
        refreshCartCount();
    }
});

// Görsel renklerini analiz eden fonksiyon
function analyzeImageColors(imageElement, cardElement) {
    const canvas = document.createElement('canvas');
    const context = canvas.getContext('2d');
    
    // Görseli canvas'a çiz
    canvas.width = imageElement.naturalWidth;
    canvas.height = imageElement.naturalHeight;
    context.drawImage(imageElement, 0, 0);
    
    // Renk örnekleri al
    const imageData = context.getImageData(0, 0, canvas.width, canvas.height).data;
    const colorCounts = {};
    const sampleSize = 10; // Her 10 pikselde bir örnek al
    
    for (let i = 0; i < imageData.length; i += 4 * sampleSize) {
        const r = imageData[i];
        const g = imageData[i + 1];
        const b = imageData[i + 2];
        const rgb = `${r},${g},${b}`;
        
        colorCounts[rgb] = (colorCounts[rgb] || 0) + 1;
    }
    
    // En çok kullanılan renkleri bul
    const sortedColors = Object.entries(colorCounts)
        .sort(([, a], [, b]) => b - a)
        .slice(0, 3)
        .map(([color]) => color.split(',').map(Number));
    
    // Renkleri CSS değişkenlerine ata
    cardElement.style.setProperty('--image-color-1', `${sortedColors[0][0]}, ${sortedColors[0][1]}, ${sortedColors[0][2]}`);
    cardElement.style.setProperty('--image-color-2', `${sortedColors[1][0]}, ${sortedColors[1][1]}, ${sortedColors[1][2]}`);
    cardElement.style.setProperty('--image-color-3', `${sortedColors[2][0]}, ${sortedColors[2][1]}, ${sortedColors[2][2]}`);
}

// Sayfa yüklendiğinde tüm ürün kartlarını işle
document.addEventListener('DOMContentLoaded', () => {
    const productCards = document.querySelectorAll('.product-card');
    
    productCards.forEach(card => {
        const image = card.querySelector('.product-image');
        if (image.complete) {
            analyzeImageColors(image, card);
        } else {
            image.onload = () => analyzeImageColors(image, card);
        }
    });
    
    initializeSliders();
    refreshCartCount();
});

// Sepet güncellendiğinde sayacı güncelle
document.addEventListener('cartUpdated', refreshCartCount);

// Pencere boyutu değiştiğinde aktif slaytı yeniden ortala
window.addEventListener('resize', () => {
    const sections = document.querySelectorAll('.product-slider-section');
    sections.forEach(section => {
        const currentIndex = getCurrentIndex(section);
        centerSlide(section, currentIndex);
    });
});

// Sayfa yüklendiğinde ve pencere boyutu değiştiğinde slider'ı başlat
document.addEventListener('DOMContentLoaded', initializeSliders);
window.addEventListener('resize', initializeSliders);

// Sepetten ürün kaldırma fonksiyonu
function removeFromCart(urunId) {
    if (confirm('Bu ürünü sepetten kaldırmak istediğinize emin misiniz?')) {
        const token = $('input[name="__RequestVerificationToken"]').val();
        
        $.ajax({
            url: '/Sepet/UrunSil',
            type: 'POST',
            contentType: 'application/json',
            headers: {
                'RequestVerificationToken': token
            },
            data: JSON.stringify({ urunId: urunId }),
            success: function(response) {
                if (response.success) {
                    // Sepet sayısını güncelle
                    $('.badge-modern').text(response.cartCount);
                    
                    // Toplam tutarı güncelle
                    $('.total-amount').text(response.totalPrice);
                    
                    // Ürünü sayfadan kaldır
                    const cartItem = $(`.cart-item[data-urun-id="${urunId}"]`);
                    cartItem.fadeOut(300, function() {
                        $(this).remove();
                        
                        // Eğer sepet boşsa sayfayı yenile
                        if ($('.cart-item').length === 0) {
                            location.reload();
                        }
                    });
                } else {
                    alert(response.message || 'Ürün sepetten kaldırılırken bir hata oluştu.');
                }
            },
            error: function(xhr, status, error) {
                console.error('Sepetten ürün kaldırma hatası:', error);
                alert('Bir hata oluştu. Lütfen tekrar deneyin.');
            }
        });
    }
}

// Sepet sayfasındaki miktar güncelleme
function updateCartQuantity(urunId, change) {
    const quantityElement = $(`.cart-item[data-urun-id="${urunId}"] .quantity-display`);
    const currentQuantity = parseInt(quantityElement.text());
    const newQuantity = currentQuantity + change;

    if (newQuantity <= 0) {
        removeFromCart(urunId);
        return;
    }

    $.ajax({
        url: '/Sepet/MiktarGuncelle',
        type: 'POST',
        data: { urunId: urunId, miktar: change },
        success: function(response) {
            if (response.success) {
                // Miktar göstergesini güncelle
                quantityElement.text(newQuantity);
                
                // Toplam tutarı güncelle
                $('.total-amount').text(response.totalAmount.toFixed(2) + ' ₺');
                
                // Sepet sayısını güncelle
                refreshCartCount();
            } else {
                alert(response.message || 'Miktar güncellenirken bir hata oluştu.');
            }
        },
        error: function() {
            alert('Bir hata oluştu. Lütfen tekrar deneyin.');
        }
    });
}

// Slider işlemleri
document.addEventListener('DOMContentLoaded', function() {
    const slider = document.querySelector('.slider-container');
    const track = document.querySelector('.slider-track');
    const items = Array.from(document.querySelectorAll('.product-card')).slice(0, 5); // Sadece 5 ürün
    const dotsContainer = document.querySelector('.slider-dots');
    
    let currentIndex = 0;
    let isMobile = window.innerWidth <= 992;
    const itemWidth = items[0].offsetWidth;
    const gap = 24;

    function initializeSlider() {
        // Önceki dot'ları temizle
        if (dotsContainer) {
            dotsContainer.innerHTML = '';
        }

        if (isMobile) {
            // Mobil görünüm için 5 dot oluştur
            items.forEach((_, index) => {
                const dot = document.createElement('button');
                dot.classList.add('slider-dot');
                if (index === 0) dot.classList.add('active');
                dot.addEventListener('click', () => goToSlide(index));
                dotsContainer?.appendChild(dot);
            });

            // İlk slide'ı ortala
            centerSlide(currentIndex);
        } else {
            // Desktop görünümde transform'u sıfırla
            if (track) {
                track.style.transform = '';
            }
        }
    }

    // Ürünü ortala (sadece mobil için)
    function centerSlide(index) {
        if (!isMobile || !track) return;

        const containerWidth = slider.offsetWidth;
        const offset = (containerWidth - itemWidth) / 2;
        const slideOffset = index * -(itemWidth + gap);
        track.style.transform = `translateX(${offset + slideOffset}px)`;
    }

    // Belirli bir slide'a git
    function goToSlide(index) {
        if (!isMobile) return;

        currentIndex = index;
        centerSlide(index);
        
        // Dot'ları güncelle
        document.querySelectorAll('.slider-dot').forEach((dot, i) => {
            dot.classList.toggle('active', i === index);
        });
    }

    // Sayfa yüklendiğinde ve yeniden boyutlandığında kontrol et
    window.addEventListener('load', initializeSlider);
    window.addEventListener('resize', () => {
        const wasDesktop = !isMobile;
        isMobile = window.innerWidth <= 992;
        
        // Sadece mobil/desktop geçişlerinde yeniden başlat
        if (wasDesktop !== isMobile) {
            initializeSlider();
        } else if (isMobile) {
            centerSlide(currentIndex);
        }
    });

    // İlk yüklemede başlat
    initializeSlider();
});

// Şifre kontrolü
function validatePassword(password) {
    if (password.length < 6) {
        return {
            isValid: false,
            message: 'Şifre en az 6 karakter olmalıdır.'
        };
    }
    return {
        isValid: true,
        message: ''
    };
}

// Kayıt formu gönderilmeden önce kontrol
document.getElementById('kayitForm')?.addEventListener('submit', function(e) {
    const sifre = document.getElementById('KayitSifre').value;
    const sifreKontrol = validatePassword(sifre);
    
    if (!sifreKontrol.isValid) {
        e.preventDefault();
        alert(sifreKontrol.message);
        return false;
    }
    return true;
});
