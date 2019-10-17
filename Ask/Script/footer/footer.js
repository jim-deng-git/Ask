$(function() {
    function footerMenuMobile(parent) {
        parent.on('click',function() {
            if ($(this).hasClass('open')) {
                $(this).removeClass('open').children('ul').slideUp();
            } else {
                $(this).addClass('open').children('ul').slideDown();
                return false;
            }
        }).mouseleave(function() {
            $(this).removeClass('open').children('ul').slideUp();
        }); 
    }
    function footerMenuDesktop(parent) {
        parent.mouseenter(function(){
            if (!$(this).children('ul').is(":animated")) {
                $(this).addClass('open');
            }
        }).mouseleave(function(){
            $(this).removeClass('open');
        });
    }

    var $ul = $('.footer-nav').find('ul.nav-menu'),
        $li = $ul.find('li'),
        parent = $ul.find('li:has(ul)');

    parent.addClass('parent');

    //menu dropdown
    if (/android/i.test(navigator.userAgent) || /iphone/i.test(navigator.userAgent)) {
        //for mobile
        footerMenuMobile(parent)
        // footerMenuMobile($drop)

    } else {
        //for pc
        footerMenuDesktop(parent)
        // footerMenuDesktop($drop)
    } 

    if ($('#back-to-top').length) {
        var scrollTrigger = 300, // px
        backToTop = function () {
            var scrollTop = $(window).scrollTop();
            if (scrollTop > scrollTrigger) {
                $('#back-to-top').addClass('show');
            } else {
                $('#back-to-top').removeClass('show');
            }
        };
        backToTop();
        $(window).on('scroll', function () {
            backToTop();
        });
        $('#back-to-top').on('click', function (e) {
            e.preventDefault();
            $('html,body').animate({
                scrollTop: 0
            }, 700);
        });
    }

});