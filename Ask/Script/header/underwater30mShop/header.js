$(function() {
    function slideMenuMobile(parent) {
        parent.on('click',function() {
            if ($(this).hasClass('open')) {
                $(this).removeClass('open').children('ul').slideUp();
                // $(this).removeClass('open');
            } else {
                $(this).addClass('open').children('ul').slideDown();
                // $(this).addClass('open');
                return false;
            }
        }).mouseleave(function() {
            $(this).removeClass('open').children('ul').slideUp();
            // $(this).removeClass('open');
        }); 
    }
    function slideMenuDesktop(parent) {
        parent.mouseenter(function(){
            if (!$(this).children('ul').is(":animated")) {
                $(this).addClass('open');
                // if ($(this).children('ul').children('ul li').length){
                //     $(this).children('ul').slideDown();
                // }
            }
        }).mouseleave(function(){
            $(this).removeClass('open');
            // $($(this)).children('ul').delay(200).slideUp();
        });
    }

    var $ul = $('.header').find('ul'),
        $drop = $('.nav-dropdown'),
        $li = $ul.find('li'),
        parent = $ul.find('li:has(ul)');

    parent.addClass('parent');

    //denise Current Menu
    uplvl = $ul.find('li:has(.current-menu)');
    uplvl.addClass('current-menu');

    $('.menu-toggle').on('click',function() {
        var $this = $(this);

        if ($this.hasClass('toggle-active')) {
            $this.removeClass('toggle-active');
            $('.mobile-menu').removeClass('menu-active');
        } else {
            $this.addClass('toggle-active');
            $('.mobile-menu').addClass('menu-active');
            return false;
        }
    });

    $('.search-btn').on('click',function() {
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
    $('.search-colse-btn').on('click',function() {
        $('.search-btn').removeClass('active');
        $('.webSearch').removeClass('active');
        $('body').removeClass('blur-body');
    });
    $('.search-colse-btn').on('click',function() {
        $('.search-btn').removeClass('active');
        $('.webSearch').removeClass('active');
        $('body').removeClass('blur-body');
    });
    $('.webSearch').on('click',function(e) {
        e.stopPropagation();
    });
    $('body').on('click', function(e) {
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

        $(document).scroll(function () {
            var y = $(this).scrollTop();
            if (y > 200) {
                $('.header').addClass('nav--hidden');
            }
            else {
                $('.header').removeClass('nav--hidden');
            }
        });

    } else {
        //for pc
        slideMenuDesktop(parent)
        slideMenuDesktop($drop)
    }

    // var prev = 100;
    // var $window = $(window);
    // var nav = $('.nav.top');

    // $window.on('scroll', function() {
    //     var scrollTop = $window.scrollTop();
    //     nav.toggleClass('hidden', scrollTop > prev);
    //     prev = scrollTop;
    // });    
    
    $("body").waypoint(function() {
        $('#header').toggleClass('header--shrink');
    }, {
        offset: -250
    });

    // 20181031 yulin 會員
    // $('.menu .main-menu .parent').on('mouseover', function () {
    //     $(this).children('ul').addClass('on');
    // }).on('mouseleave', function () {
    //     $(this).children('ul').removeClass('on');
    // });

});

