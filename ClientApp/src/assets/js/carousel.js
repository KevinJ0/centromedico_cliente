

(function carousel() {
    var swiper = new Swiper('.swiper-container', {
        spaceBetween: 20,
        observer: true,
        autoHeight: true,
        navigation: true, preloadImages: true,
        centerInsufficientSlides: true,
        breakpoints: {
            413: {
                slidesPerView: 2,
                spaceBetween: 20,
                slidesPerGroup: 2,
            },
            558: {
                slidesPerView: 3,
                spaceBetween: 20,
                slidesPerGroup: 3,
            },

            704: {
                slidesPerView: 4,
                spaceBetween: 20,
                slidesPerGroup: 4,
            },
            1100: {
                slidesPerView: 5,
                spaceBetween: 20,
                slidesPerGroup: 5,
            },
            1300: {
                slidesPerView: 6,
                spaceBetween: 30,
                slidesPerGroup: 6,
            }
        },
        navigation: {
            nextEl: '.swiper-button-next',
            prevEl: '.swiper-button-prev',
        },
    });



})();