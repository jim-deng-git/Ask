if (typeof jQuery === 'undefined') {
  throw new Error('loadScript\'s JavaScript requires jQuery')
}
+function ($) {
  'use strict';
  var version = $.fn.jquery.split(' ')[0].split('.')
  if ((version[0] < 2 && version[1] < 9) || (version[0] == 1 && version[1] == 9 && version[2] < 1) || (version[0] > 3)) {
    throw new Error('loadScript\'s JavaScript requires jQuery version 1.9.1 or higher')
  }
}(jQuery);


+function ($) {
$(document).ready(function() {
  'use strict';

    // document.addEventListener("touchmove", function(e) {
    //     e.preventDefault();
    //     var touch = e.targetTouches[0];
    //     if (touch) {
    //         sway(touch.pageX, touch.pageY);
    //     }
    // });

    // 搭配圖組 才要引用
    $(".openImg").on('click', function(e) {
      e.preventDefault();
      var self = $(this);
      
      $.colorbox({
        href: self.data("href"),
        rel: 'group1',
        maxWidth: "90%",
        maxHeight: "90%"
      });
    });

    // 搭配影片（播放方式 直接播放 / 非自行上傳）才要引用
    $(".openVideoImg").on('click', function(e) {
      e.preventDefault();

      var video = '<div class="videoCurrentBox"><a href="javascriot:"><i class="cc cc-close"></i></a><iframe src="'+ $(this).data('href') +'"></iframe></div>';
      // var $video = $(this).find('.videoCurrentBox')[0];

      if ($(this).find('.videoCurrentBox').length) {
       // $video.remove();
       $(this).find('.videoCurrentBox').remove();
       $('.header').removeClass('playVideo'); 
       $(this).parents('.flexsliderV').removeClass('playVideo'); 

      } else {
        $('.videoCurrentBox').remove();
        $(this).prepend(video);
        $('.header').addClass('playVideo'); 
        $(this).parents('.flexsliderV').addClass('playVideo');
      }
    });
    // $('.openVideoImg > .videoCurrentBox > a').on('click', function(){
    //   $video.remove();
    //   $('.header').removeClass('playVideo'); 
    //   $(this).parents('.flexsliderV').removeClass('playVideo'); 
    // });

    // 搭配影片（播放方式 燈箱 / 非自行上傳）才要引用
    $(".openVideo").on('click', function(e) {
      e.preventDefault();
      var self = $(this);
      
      $.colorbox({
        href: self.data("href"),
        iframe:true,
        width: "1408px",
        height: "792px",
        maxWidth: "90%",
        maxHeight: "90%"
      });
    });

    // 搭配影片(播放方式 燈箱 / 自行上傳) 才要引用
    $(".openJWPlayer").on('click', function (e) {
      e.preventDefault();
      var self = $(this);

        $.colorbox({
            html: '<div id="' + self.data("id") + '"></div>',
            width: "1408px",
            height: "792px",
            maxWidth: "90%",
            maxHeight: "90%",
            scrolling: false,
            fixed: true,

            title: function() {
              var $title = self.data('title');
              if($title.length > 0) {
                return $title;
              }
            },
            onComplete: function() {
              jwplayer(self.data("id")).setup({
                  file: self.data("file"),
                  image: self.data("image"),
                  width: "auto",
                  height: "100%",
                  type: "mp4",
                  skin: {
                    name:"default",//選擇主題
                    active: "#2e2e2e",//選擇主色
                    inactive: "#ffffff",
                    background: "#000000"//選擇背景
                  },
                  showdownload: false,
                  aspectratio: "16:9",//影片比例
                  autostart: false //自動播放
              });
            },
            // onClosed: function() {
            // }
        });
    });

    // 搭配聲音 才要引用
    $(".openVoice").on('click', function (e) {
      e.preventDefault();
      var self = $(this);

        $.colorbox({
            html: '<div id="' + self.data("id") + '"></div>',
            width: "270px",
            height: "30px",
            maxWidth: "90%",
            maxHeight: "90%",
            scrolling: false,
            fixed: true,
            onComplete: function() {
              jwplayer(self.data("id")).setup({
                  file: self.data("voice"),
                  width: "270",
                  height: "30",
                  type: "mp3",
                  skin: {
                    name:"default",//選擇主題
                    active: "#2e2e2e",//選擇主色
                    inactive: "#ffffff",
                    background: "#000000"//選擇背景
                  },
                  autostart: true
              });
            }
        });
    });
    // if ($(".swiper-container").length > 0) {
    //   var swiper = new Swiper('.swiper-container', {
    //               // pagination: '.swiper-pagination',
    //               slidesPerView: 'auto',
    //               centeredSlides: true,
    //               paginationClickable: true,
    //               nextButton: '.swiper-button-next',
    //               prevButton: '.swiper-button-prev',
    //               spaceBetween: 0,
    //               initialSlide: 2
    //   });
    // }


    if (('#socialMore').length) {
      $('#socialMore').on('click', function () {
        var $this = $(this);
        if ($this.hasClass('icon-close')) {
            var moreShareCount = $this.attr("dada-count");
            if(moreShareCount!= null)
            {
                var items = $('#socialShareBox').children('a:gt('+moreShareCount+')');
                var len = items.length;
                // function fade(index) {
                //   if (index < 0) {
                //     $this.fadeOut(function () {
                //       $this.removeClass('icon-close').fadeIn();
                //     });
                //     return;
                //   }
                //   items.eq(index).fadeOut(function () {
                //     fade(--index);
                //   },0);
                // }
                // fade(len - 1);
                items.fadeOut();
                $this.removeClass('icon-close').fadeIn();
            }
        } else {
          var items = $('#socialShareBox').children('a:hidden');
          var len = items.length;
          // function fade(index) {
          //   if (index == len) {
          //     $this.fadeOut(function () {
          //       $this.addClass('icon-close').fadeIn();
          //     });
          //     return;
          //   }
          //   items.eq(index).fadeIn(function () {
          //     fade(++index);
          //   },0);
          // }
          // fade(0);
          items.fadeIn();
          $this.addClass('icon-close').fadeIn();
        }
      });
    }
    
    //手機版分享 點擊展開全部分享
    if (('.mobile-controlBars').length) {
      $(document).scroll(function () {
          var y = $(this).scrollTop();
          if (y > 200) {
              $('.mobile-controlBars').addClass('active');
          }
          else {
              $('.mobile-controlBars').removeClass('active');
          }
      });
      $('#mobile-socialMore').on('click',function(){
        if ( $(this).hasClass('active') ) {
          $(this).siblings('.extraDiv').slideUp(600);
          $(this).siblings('.overlayer').fadeOut();
          $(this).removeClass('active');
          //$('html,body').removeAttr('style');
        } else {
          $(this).siblings('.extraDiv').slideDown(600);
          $(this).siblings('.overlayer').fadeIn();
          $(this).addClass('active');
          //$('html,body').css({'position':'fixed','width':'100%','left':'0','top':'0','height':'0','overflow-y':'hidden'});
        }
      });
      $('#extra-close').on('click',function(){
        if ( $(this).closest('.extraDiv').siblings('#mobile-socialMore').hasClass('active') ) {
          $(this).closest('.extraDiv').slideUp(600);
          $(this).closest('.extraDiv').next('.overlayer').fadeOut();
          $(this).closest('.extraDiv').siblings('#mobile-socialMore').removeClass('active');
          //$('html,body').removeAttr('style');
        } else {
          $(this).closest('.extraDiv').slideDown(600);
          $(this).closest('.extraDiv').next('.overlayer').fadeIn();
          $(this).closest('.extraDiv').siblings('#mobile-socialMore').addClass('active');
          //$('html,body').css({'position':'fixed','width':'100%','left':'0','top':'0','height':'0','overflow-y':'hidden'});
        }
      });

    }
})
}(jQuery);
function breakpoint($winWidth = null, $centerWidth = null) {
        if ($.isMobile()) {
          $("[data-sticky_column]").trigger("sticky_kit:detach");

        } else {
          var $headerHeight = $('#header').outerHeight();
          $("[data-sticky_column]").stick_in_parent({
            parent: "[data-sticky_parent]",
            // bottoming : false,
            //spacer: false,
            //inner_scrolling: false,
            offset_top: $headerHeight
          });
          if($('.article-left').length) {
            $('.article-main').addClass('hasLeft');
          }
          if($('.article-right').length) {
            $('.article-main').addClass('hasRight');
          }
          
          $('.article-center .card').each(function(){
            var $this = $(this),
              $width = $this.width(),
              $pWidth = $this.parents('.groove').outerWidth(),
              //$winWidth = $(window).width(),
              $imgBox = $this.find('.card-img'),
              $slider = $this.find('.flexslider'),
              $imgInfoW = $this.find('.imgInfo');       

            if ($this.hasClass('imgWidth-large')) {
              if($slider.length) {
                $slider.css({
                  width: $pWidth
                });
              } else {
                $imgBox.css({
                  width: $pWidth
                });
              }
            }
            if ($this.hasClass('original')) {
            
              if($slider.length) {
                $slider.css({
                  width: $winWidth
                });
                $imgInfoW.css({
                  width: $centerWidth
                });
              }
              if($imgBox.hasClass('text-C')) {
                if($slider.length) {
                  $slider.css({
                    'margin-left': -(($winWidth/2)+20)
                  });
                }
              }
            }
          });
        }
      }