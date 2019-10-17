$(function () {
    function slideMenuMobile(parent) {
        parent.on('click', function () {
            if ($(this).hasClass('open')) {
                $(this).removeClass('open').children('ul').slideUp();
                // $(this).removeClass('open');
            } else {
                $(this).addClass('open').children('ul').slideDown();
                // $(this).addClass('open');
                return false;
            }
        })//.mouseleave(function () {
        //$(this).removeClass('open').children('ul').slideUp();
        // $(this).removeClass('open');
        //});
    }
    function slideMenuDesktop(parent) {

        parent.on('click', function () {
            if ($(this).hasClass('open')) {
                $(this).removeClass('open').children('ul').slideUp();
                // $(this).removeClass('open');
            } else {
                $(this).addClass('open').children('ul').slideDown();
                // $(this).addClass('open');
                return false;
            }
        })//.mouseleave(function () {
        //$(this).removeClass('open');
        // $($(this)).children('ul').delay(200).slideUp();
        //});
    }

    function slideMenuDesktop2(parent2) {
        parent2.mouseenter(function(){
            if (!$(this).children('ul').is(":animated")) {
                $(this).addClass('open');
            }
        }).mouseleave(function(){
            $(this).removeClass('open');
        });
    }

    var $ul = $('.header .main').find('ul'),
        $ul2 = $('.header .nav').find('ul'),
        $drop = $('.nav-dropdown'),
        $li = $ul.find('li'),
        parent = $ul.find('li:has(ul)'),
        parent2 = $ul2.find('li:has(ul)'),
        openIcon = '<span class="cc cc-lnr-chevron-down"></span>';

    parent.addClass('parent').children('a').append(openIcon);
    parent2.addClass('parent');
    //denise Current Menu
    uplvl = $ul.find('li:has(.current-menu)');
    uplvl.addClass('current-menu');

    $('.menu-toggle').on('click', function () {
        var $this = $(this);

        if ($this.hasClass('toggle-active')) {
            $this.removeClass('toggle-active');
            $('.menu').removeClass('menu-active');
        } else {
            $this.addClass('toggle-active');
            $('.menu').addClass('menu-active');
            return false;
        }
    });

    $('.search-btn').on('click', function () {
        var $this = $(this);

        if ($this.hasClass('active')) {
            $this.removeClass('active');
            $('.webSearch').removeClass('active');
            $('body').removeClass('blur-body');
        } else {
            $this.addClass('active');
            $('.webSearch').addClass('active');
            $('body').addClass('blur-body');
            return false;
        }
    });
    $('.search-colse-btn').on('click', function () {
        $('.search-btn').removeClass('active');
        $('.webSearch').removeClass('active');
        $('body').removeClass('blur-body');
    });
    $('.webSearch').on('click', function (e) {
        e.stopPropagation();
    });
    $('body').on('click', function (e) {
        if ($(this).hasClass('blur-body')) {
            $('.search-btn').removeClass('active');
            $('.webSearch').removeClass('active');
            $('body').removeClass('blur-body');
            e.stopPropagation();
        }
    });

    //選單下展
    if (/android/i.test(navigator.userAgent) || /iphone/i.test(navigator.userAgent)) {
        //for mobile
        slideMenuMobile(parent)
        slideMenuMobile($drop)

    } else {
        //for pc
        slideMenuDesktop(parent)
        slideMenuDesktop($drop)
        slideMenuDesktop2(parent2)
    }

    // var prev = 100;
    // var $window = $(window);
    // var nav = $('.nav.top');

    // $window.on('scroll', function() {
    //     var scrollTop = $window.scrollTop();
    //     nav.toggleClass('hidden', scrollTop > prev);
    //     prev = scrollTop;
    // });    

    //如果沒nav-social
    if ($('#header').find('.nav-social').length) {
        $('#header').removeClass('none-nav-social');
    } else {
        $('#header').addClass('none-nav-social');
    }

    $("body").waypoint(function () {
        $('#header').toggleClass('header--shrink');
    }, {
            offset: -60
        });

});

