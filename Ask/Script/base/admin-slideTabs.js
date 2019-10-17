$(function () {

    if ($('.swiper-slide').length) {
        function setCurrentSlide(ele, index) {
            $(".swiper-tab-bar .swiper-slide").removeClass("selected");
            ele.addClass("selected");
            //swiper1.initialSlide=index;
        }

        var swiper1 = new Swiper('.swiper-tab-bar', {
            slidesPerView: 'auto',
            paginationClickable: true,
            spaceBetween: 0,
            freeMode: true,
            loop: false,
            nextButton: '.swiper-button-next',
            prevButton: '.swiper-button-prev',
        });


        var swiper2 = new Swiper('.swiper-tab-content', {
            direction: 'horizontal',
            slidesPerView: 'auto',
            loop: false,
            autoHeight: true,
            simulateTouch: false,
            onSlideChangeEnd: function (swiper) {
                var n = swiper.activeIndex;
                setCurrentSlide($(".swiper-tab-bar .swiper-slide").eq(n), n);
                swiper1.slideTo(n, 500, false);
            }
        });

        swiper1.slides.each(function (index, val) {
            var ele = $(this);
            ele.on("click", function () {
                setCurrentSlide(ele, index);
                swiper2.slideTo(index, 500, false);
                //mySwiper.initialSlide=index;
                // 20190426 yulin click slide to top
                $('html, body').animate({ scrollTop: 0 }, 500);
            });
        });

        $('.slideto').each(function (index, val) {
            var _slideto = $(this);
            _slideto.on("click", function () {
                var _index_tag = $(this).data('slideto');
                if (_index_tag != '' && _index_tag != null) {
                    $(".swiper-tab-bar .swiper-slide").removeClass("selected");
                    swiper1.slides.eq(_index_tag).addClass("selected");
                    swiper2.slideTo(_index_tag, 500, false);
                    $('html, body').animate({ scrollTop: 0 }, 500);
                }
            });
        });


    }
});

