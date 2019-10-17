var onReady = function() {
  var oBlur = $(".slider-blur")
  var oMain = $(".slider-main")
  var autoSlideTimeout;
  var autoSlideDelay = 5000;
  var autoPlay = true;
  var blurImage = oBlur.find(".slide");
  var mainImage = oMain.find(".slide");
  var imgLength = (blurImage.length) - 1;

  mainImage.each(function() {
    var self = $(this);
    if (self.index() == 0)
      self.scale(1.25)
    else
      self.stop().animate({'scale': .75});
      createNav(self);
  });

  var speed = oBlur.find(".slide-0").data("speed");
      speedFirst = (speed.length <= 0)? autoSlideDelay: speed;
  $(".slider-main .slide.slide-0").stop().animate({'scale': 1}, autoSlideDelay, 'easeOutExpo');
  $(".slider-navigation ul li.navItem-0 .li__info-mask").stop().animate({"width": "100%"}, 3000, 'easeOutExpo', function(){});
  $(".slider-navigation ul li.navItem-0 .li__hoverLine .l").stop().animate({"width": "100%"}, 1000, 'easeOutExpo', function(){});

  $(".slider-control.left").on("click", function() {
    var self = $(this);
    var nowindex = oBlur.find(".active").index();
    var nextindex = ((nowindex-1) == -1)?imgLength:(nowindex-1);
    move(nowindex, nextindex);
  });

  $(".slider-control.right").on("click", function() {
    var self = $(this);
    var nowindex = oBlur.find(".active").index();
    var nextindex = ((nowindex+1) >= (imgLength + 1))? 0:(nowindex+1);
    move(nowindex, nextindex);
  });

  $(".slider-navigation ul li").on("click", function() {
    var self = $(this);
    var nowindex = oBlur.find(".active").index();
    var nextindex = self.index()
    move(nowindex, nextindex);
  })


  var move = function(nowindex, nextindex){
    clearTimeout(autoSlideTimeout);
    var speed = oBlur.find(".slide-" + nextindex).data("speed");

    $(".slider-navigation ul li.navItem-"+nowindex+" .li__info-mask").stop().animate({"width": "0%"}, 3000, 'easeOutExpo', function(){});
    $(".slider-navigation ul li.navItem-"+nowindex+" .li__hoverLine .l").stop().animate({"width": "0%"}, 1000, 'easeOutExpo', function(){});
    oMain.find(".slide-" + nowindex).stop().animate({
      "scale": 0.75
    }, 1000, 'easeOutExpo', function(){
      $(".slider-navigation ul li.navItem-"+nextindex+" .li__info-mask").stop().animate({"width": "100%"}, 3000, 'easeOutExpo', function(){});
      $(".slider-navigation ul li.navItem-"+nextindex+" .li__hoverLine .l").stop().animate({"width": "100%"}, 1000, 'easeOutExpo', function(){});
        oBlur.find(".slide-" + nowindex).stop().animate({
          "top": "-100%"
        }, 1000, 'easeOutExpo', function(){
          $(this).removeClass("active");
          $(this).css("top", "100%");
          
        })
        oBlur.find(".slide-" + nextindex).stop().animate({
          "top": "0%"
        }, 1000, 'easeOutExpo', function(){
          $(this).addClass("active");
        })
        oMain.find(".slide-" + nextindex).stop().animate({
          "top": "0%"
        }, 1000, 'easeOutExpo', function(){
          $(this).addClass("active");
          $(this).stop().animate({
            'scale': 1
          })
        })
      $(this).stop().animate({
        "top": "100%"
      }, 1000, 'easeOutExpo', function(){
        $(this).removeClass("active");
        $(this).css("top", "-100%");
        
      })
    })
      speed = (speed.length <= 0)? autoSlideDelay: speed;
      autoSlide(speed);
  }
  function pad (str, max) {
      str = str.toString();
      return str.length < max ? pad("0" + str, max) : str;
  }
  function createNav(me) {
    var number = pad(me.index() + 1, 2);

    var html =  '<li class="navItem-'+ me.index() +'">';
        html += '<div class="li__info"><h5>'+ number +'</h5></div>';
        html += '<div class="li__info-mask">';
        html += '<div class="mask__infoContainer"><h5>'+ number +'</h5></div>';
        html += '</div>';
        html += '<div class="li__hoverLine">';
        html += '<div class="l"></div>';
        html += '</div';
        html += '</li>';

    $(".slider-navigation ul").append(html);
  }
  function autoSlide(speed) {
    if (autoPlay) {
      autoSlideTimeout = setTimeout(function() {
        $(".slider-control.right").click();
      }, speed);
    }
  };
  if (autoPlay && imgLength > 0)
    autoSlide(speedFirst);
  else
    $(".slider-control.left, .slider-control.right, .slider-navigation ul").hide()
}

$(document).ready(onReady);