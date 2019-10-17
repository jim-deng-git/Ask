(function ($) {
  $(document).ready(function () {

    if ($("#btnSearch").length > 0 && $("#searchForm").length > 0) {
      var formUrl = $("#searchForm").attr("action");
      $("#btnSearch").parent().parent().before().append("<div class=\"float-R\"><a id=\"btnClean\" class=\"btn-grey-darken-4-o btn-large m-R-5\" href=\"" + formUrl + "\"><i class=\"cc cc-reload2\"></i>清空</a></div>");

    }
    // jQuery reverse
    $.fn.reverse = [].reverse;

    // Hover behaviour: make sure this doesn't work on .click-to-toggle FABs!
    $(document).on('mouseenter.fixedActionBtn', '.tool-btn:not(.click-to-toggle)', function (e) {
      var $this = $(this);
      openFABMenu($this);
    });
    $(document).on('mouseleave.fixedActionBtn', '.tool-btn:not(.click-to-toggle)', function (e) {
      var $this = $(this);
      closeFABMenu($this);
    });

    // Toggle-on-click behaviour.
    $(document).on('click.fabClickToggle', '.tool-btn.click-to-toggle > a', function (e) {
      var $this = $(this);
      var $menu = $this.parent();
      if ($menu.hasClass('active')) {
        closeFABMenu($menu);
      } else {
        openFABMenu($menu);
      }
    });
  });

  $.fn.extend({
    openFAB: function () {
      openFABMenu($(this));
    },
    closeFAB: function () {
      closeFABMenu($(this));
    }
  });


  var openFABMenu = function (btn) {
    var $this = btn;
    if ($this.hasClass('active') === false) {

      // Get direction option
      var horizontal = $this.hasClass('horizontal');
      var offsetY, offsetX;

      if (horizontal === true) {
        offsetX = 30;
      } else {
        offsetY = 40;
      }

      $this.addClass('active');
      $this.find('ul [class*="btn-"]').velocity(
        { scaleY: ".4", scaleX: ".4", translateY: offsetY + 'px', translateX: offsetX + 'px' },
        { duration: 0 });

      var time = 0;
      $this.find('ul [class*="btn-"]').reverse().each(function () {
        $(this).velocity(
          { opacity: "1", scaleX: "1", scaleY: "1", translateY: "0", translateX: '0' },
          { duration: 80, delay: time });
        time += 40;
      });
    }
  };

  var closeFABMenu = function (btn) {
    var $this = btn;
    // Get direction option
    var horizontal = $this.hasClass('horizontal');
    var offsetY, offsetX;

    if (horizontal === true) {
      offsetX = 30;
    } else {
      offsetY = 40;
    }

    $this.removeClass('active');
    var time = 0;
    $this.find('ul [class*="btn-"]').velocity("stop", true);
    $this.find('ul [class*="btn-"]').velocity(
      { opacity: "0", scaleX: ".4", scaleY: ".4", translateY: offsetY + 'px', translateX: offsetX + 'px' },
      { duration: 80 }
    );
  };

}(jQuery));
