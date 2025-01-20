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
        centerSlide(section, 0);
        
        // Touch ve mouse olaylarını ekle
        let isDown = false;
        let startX;
        let scrollLeft;
        
        track.addEventListener('mousedown', e => {
            if (e.target.closest('.quantity-btn') || 
                e.target.closest('.shop-now-btn') || 
                e.target.closest('.slider-arrow')) {
                return;
            }
            
            isDown = true;
            track.style.cursor = 'grabbing';
            startX = e.pageX;
            scrollLeft = getCurrentTranslate(track);
        });

        track.addEventListener('mousemove', e => {
            if (!isDown) return;
            e.preventDefault();
            const x = e.pageX;
            const walk = (x - startX) * 1.5; // Hassasiyeti artır
            track.style.transform = `translateX(${scrollLeft + walk}px)`;
        });

        track.addEventListener('mouseleave', () => {
            if (!isDown) return;
            isDown = false;
            track.style.cursor = 'grab';
            snapToNearestSlide(section);
        });

        track.addEventListener('mouseup', () => {
            if (!isDown) return;
            isDown = false;
            track.style.cursor = 'grab';
            snapToNearestSlide(section);
        });

        // Touch olayları
        track.addEventListener('touchstart', e => {
            if (e.target.closest('.quantity-btn') || 
                e.target.closest('.shop-now-btn') || 
                e.target.closest('.slider-arrow')) {
                return;
            }
            
            isDown = true;
            startX = e.touches[0].pageX;
            scrollLeft = getCurrentTranslate(track);
        }, { passive: false });

        track.addEventListener('touchmove', e => {
            if (!isDown) return;
            e.preventDefault();
            const x = e.touches[0].pageX;
            const walk = (x - startX) * 1.5; // Hassasiyeti artır
            track.style.transform = `translateX(${scrollLeft + walk}px)`;
        }, { passive: false });

        track.addEventListener('touchend', () => {
            if (!isDown) return;
            isDown = false;
            snapToNearestSlide(section);
        });
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
    const slideWidth = slides[0].offsetWidth + (window.innerWidth <= 768 ? 20 : 30); // Mobilde 20px, masaüstünde 30px gap
    const containerWidth = section.offsetWidth;
    
    // Ekranın ortası ile slayt genişliğinin yarısı kadar sola kaydır
    const centerPosition = (containerWidth / 2) - (slideWidth / 2);
    // Seçili slaytı sola kaydır
    const slideOffset = index * slideWidth;
    // Son pozisyon
    const position = centerPosition - slideOffset;
    
    track.style.transition = 'transform 0.3s ease-out';
    track.style.transform = `translateX(${position}px)`;
    
    // Dot'ları güncelle
    updateDots(section, index);
}

function snapToNearestSlide(section) {
    const track = section.querySelector('.slider-track');
    const slides = track.querySelectorAll('.product-card');
    const slideWidth = slides[0].offsetWidth + (window.innerWidth <= 768 ? 20 : 30);
    const currentPosition = getCurrentTranslate(track);
    const containerWidth = section.offsetWidth;
    
    // Ekranın ortasından uzaklığı hesapla
    const centerOffset = (containerWidth / 2) - (slideWidth / 2);
    const relativePosition = centerOffset - currentPosition;
    
    const nearestIndex = Math.round(relativePosition / slideWidth);
    const finalIndex = Math.max(0, Math.min(nearestIndex, slides.length - 1));
    
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

// Miktar güncelleme fonksiyonu
function updateQuantity(urunId, change) {
    const quantityElement = document.getElementById(`quantity-${urunId}`);
    let quantity = parseInt(quantityElement.textContent);
    quantity = Math.max(1, quantity + change); // Minimum 1 olacak şekilde
    quantityElement.textContent = quantity;
}

// Sepete ekleme fonksiyonu
function addToCart(urunId) {
    const quantityElement = document.getElementById(`quantity-${urunId}`);
    const quantity = parseInt(quantityElement.textContent);
    const token = document.querySelector('input[name="__RequestVerificationToken"]').value;
    
    fetch('/Sepet/SepeteEkle', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'RequestVerificationToken': token
        },
        body: JSON.stringify({
            urunId: urunId,
            miktar: quantity
        })
    })
    .then(response => {
        if (!response.ok) {
            throw new Error('Network response was not ok');
        }
        return response.json();
    })
    .then(data => {
        if (data.success) {
            updateCartCount(data.cartCount);
            showNotification();
            // Miktarı sıfırla
            quantityElement.textContent = '1';
        } else {
            alert(data.message || 'Ürün sepete eklenirken bir hata oluştu.');
        }
    })
    .catch(error => {
        console.error('Error:', error);
        alert('Bir hata oluştu. Lütfen tekrar deneyin.');
    });
}

// Sepet sayacı fonksiyonları
function updateCartCount(count) {
    const cartCount = document.querySelector('.cart-count');
    if (cartCount) {
        cartCount.textContent = count;
        cartCount.style.display = count > 0 ? 'flex' : 'none';
    }
}

function refreshCartCount() {
    fetch('/Sepet/ToplamUrun')
        .then(response => response.json())
        .then(data => {
            updateCartCount(data);
        })
        .catch(error => console.error('Sepet sayısı alınamadı:', error));
}

// Bildirim gösterme fonksiyonu
function showNotification() {
    const existingNotification = document.querySelector('.cart-notification');
    if (existingNotification) {
        existingNotification.remove();
    }

    const notification = document.createElement('div');
    notification.className = 'cart-notification';
    notification.innerHTML = `
        <i class="fas fa-check-circle"></i>
        <span>Ürün sepete eklendi!</span>
        <a href="/Sepet" class="view-cart-btn">
            <i class="fas fa-shopping-cart"></i>
            Sepeti Gör
        </a>
    `;

    document.body.appendChild(notification);
    setTimeout(() => notification.classList.add('show'), 100);
    setTimeout(() => {
        notification.classList.remove('show');
        setTimeout(() => notification.remove(), 300);
    }, 3000);
}

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

// Sayfa görünür olduğunda sepet sayısını güncelle
document.addEventListener('visibilitychange', () => {
    if (document.visibilityState === 'visible') {
        refreshCartCount();
    }
});

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

// Sepet işlemleri için fonksiyonlar
function updateCartCount() {
    $.ajax({
        url: '/Sepet/GetSepetUrunSayisi',
        type: 'GET',
        success: function (count) {
            $('.badge.bg-danger').text(count);
        }
    });
}

// Sepete ürün eklendiğinde
function addToCart(urunId, adet) {
    $.ajax({
        url: '/Sepet/SepeteEkle',
        type: 'POST',
        data: { urunId: urunId, adet: adet },
        success: function (response) {
            if (response.success) {
                updateCartCount();
                showNotification('Ürün sepete eklendi!');
            }
        }
    });
}

// Bildirim gösterme fonksiyonu
function showNotification(message) {
    const notification = $('<div class="cart-notification"><i class="fas fa-check-circle"></i><span>' + message + '</span></div>');
    $('body').append(notification);
    setTimeout(function() {
        notification.addClass('show');
    }, 100);
    setTimeout(function() {
        notification.removeClass('show');
        setTimeout(function() {
            notification.remove();
        }, 300);
    }, 3000);
}

// Sayfa yüklendiğinde sepet sayısını güncelle
$(document).ready(function() {
    updateCartCount();
});
