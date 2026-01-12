

(function carousel() {
    var swiper = new Swiper('.swiper-container', {
        spaceBetween: 20,
         slidesPerView: 'auto',
        setWrapperSize: true,
        navigation: true, preloadImages: true,
        centerInsufficientSlides: true,
       
        navigation: {
            nextEl: '.swiper-button-next',
            prevEl: '.swiper-button-prev',
        },
        breakpoints: {
            320: {
                slidesPerView: 1,
                spaceBetween: 0
            },
            600: {
                slidesPerView: 'auto',
                spaceBetween: 20
            }
        }
    });



})();
