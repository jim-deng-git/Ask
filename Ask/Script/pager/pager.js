 $.extend({
    pager: function(options) {
        var o = jQuery.extend({
            speed: 500,
            footer: "",
            theTarget: ".pager",
            easing: ""
        }, options);

        var setFooterHeight = $(o.footer).height();
        var setBodyHeight = $("body").height();
        var setDocumentHeight = $(window).height();

        var scrolled = setBodyHeight - setDocumentHeight - setFooterHeight;


        $(window).scroll(function(){
            if ($(this).scrollTop() >= scrolled) {
                $(".pager").css({
                    "position": "absolute",
                    "bottom": "auto"
                });
            } else {
                $(".pager").css({
                    "position": "fixed",
                    "bottom": ""
                })

            }
        });
    },
 });