$(function () {
    $('#expander').on('click', function () {
        $('#frameContent').toggleClass('pushdown');
    });

    $('#sideBarMenu').mmenu({
        "offCanvas": false,
        "extensions": [
            "pagedim-black",
            "widescreen"
        ],
        "navbar": {
            "title": null
        },
        "navbars": [{
            "position": "top",
        }]
    }, {
            lazySubmenus: {
                "load": true
            }
        });

    //sideBarMenu open & close
    $('.toggleBox').each(function () {
        var $this = $(this),
            $parent = $(this).parent('.section-heading'),
            $toggleContent = $(this).siblings('.dd-list'),
            $toggleElm = $('.show');

        $this.on('click', function () {
            if ($toggleContent.hasClass('show')) {
                $toggleContent.slideUp('normal', function () {
                    $(this).removeClass('show');
                    $parent.removeClass('active');
                    $this.removeClass('active');
                    // $('.toggleBox').removeClass('active');
                });
                // return;
            } else {
                $toggleContent.slideDown('normal', function () {
                    $(this).addClass('show');
                    $parent.addClass('active');
                    $this.addClass('active');
                });
            }
        });
    });

    $('.nestable-topMenu').nestable({
        itemNodeName: "ol > li",
        maxDepth: 5,
        group: 1,
    }).on('change', function (e) {
        if (e.isTrigger)
            SaveMenuSort(1);
    });

    $('.nestable-mainMenu').nestable({
        itemNodeName: "ol > li",
        maxDepth: 5,
        group: 2,
    }).on('change', function (e) {
        if (e.isTrigger)
            SaveMenuSort(2);
    });

    $('.nestable-bottomMenu').nestable({
        itemNodeName: "ol > li",
        maxDepth: 5,
        group: 3,
    }).on('change', function (e) {
        if (e.isTrigger)
            SaveMenuSort(3);
    });

    //settings show
    if (/android/i.test(navigator.userAgent) || /iphone/i.test(navigator.userAgent)) {
        //for mobile
        $(".dd-item").on('click', function (e) {
            $(this).toggleClass('active-menu');

            //選單
            var searchObjs = $(this).find("i");
            var hrefL = $(this).attr("data-href");
            if (searchObjs.length > 0) {
                if (hrefL != "" && hrefL != null) {
                    if (searchObjs.first().attr("class").indexOf("openEdit-m") >= 0) {
                        window.top.mainWebContent.refreshViews = function () {
                            location = location.href;
                        };
                        $.colorbox({
                            href: hrefL,
                            width: "600",
                            height: "95%",
                            right: "20",
                            iframe: true,
                            transition: false,
                            speed: 0,
                            fadeOut: 100
                        });
                    } else {
                        if (hrefL != "" && hrefL != null) {
                            if ($(this).find('.openEdit').length == 0) {
                                e.stopPropagation();//20190326 susan add 阻止冒泡行為
                            }
                            window.top.mainWebContent.location.href = hrefL;
                        }
                    }
                }
            }
            else {
                if (hrefL != "" && hrefL != null) {
                    window.top.mainWebContent.location.href = hrefL;
                }
            }

        }).mouseleave(function () {
            $(this).removeClass('active-menu');
        });
    } else {

        //for pc
        $('.dd-item').mouseenter(function () {
            $(this).addClass('active-menu');
        }).mouseleave(function () {
            $(this).removeClass('active-menu');
        }).click(function (e) {
            //選單
            var searchObjs = $(this).find("i");
            var hrefL = $(this).attr("data-href");
            if (searchObjs.length > 0) {
                if (hrefL != "" && hrefL != null) {
                    if (searchObjs.first().attr("class").indexOf("openEdit-m") >= 0) {
                        window.top.mainWebContent.refreshViews = function () {
                            location = location.href;
                        };
                        $.colorbox({
                            href: hrefL,
                            width: "600",
                            height: "95%",
                            right: "20",
                            iframe: true,
                            transition: false,
                            speed: 0,
                            fadeOut: 100
                        });
                    } else {
                        if (hrefL != "" && hrefL != null) {
                            if ($(this).find('.openEdit').length == 0) {
                                e.stopPropagation();//20190326 susan add 阻止冒泡行為
                            }
                            window.top.mainWebContent.location.href = hrefL;
                        }
                    }
                }
            }
            else {
                if (hrefL != "" && hrefL != null) {
                    window.top.mainWebContent.location.href = hrefL;
                }
            }
        });
    }

    //colorbox
    $('.openEdit').colorbox({
        width: function () {
            var NW = "430";
            if ($.isNumeric($(this).data("width"))) {
                NW = $(this).data("width");
            }
            return NW;
        },
        height: function () {
            var NH = "520";
            if ($.isNumeric($(this).data("height"))) {
                NH = $(this).data("height");
            }
            return NH;
        },
        opacity: 0,
        top: "100",
        left: "276",
        iframe: true,
        transition: false,
        speed: 0,
        fadeOut: 100
    });
    $(".openEdit-m").colorbox({
        width: "600",
        height: "95%",
        right: "20",
        iframe: true,
        transition: false,
        speed: 0,
        fadeOut: 100
    });

    $(".openEdit-lg").colorbox({
        width: "95%",
        height: "95%",
        iframe: true,
        transition: false,
        speed: 0,
        fadeOut: 100
    });

    //tooltipster
    $('.tooltip').tooltipster({
        maxWidth: 250
    });
    $('.tooltip-content').tooltipster({
        minWidth: 150,
        maxWidth: 250
    });
    $('.tooltip-info').tooltipster({
        maxWidth: 250,
    });

    //alert
    //$('.menu-del').on('click', function() {
    //    swal({
    //        // title: 'Are you sure?',
    //        text: '刪除後，資料無法復原，並且連同下層的子選單都會一併刪除！確定刪除本單元？',
    //        type: 'warning',
    //        showCancelButton: true,
    //        // confirmButtonColor: '#3085d6',
    //        // cancelButtonColor: '#d33',
    //        confirmButtonText: '刪除',
    //        cancelButtonText: '取消',
    //    });
    //});

});


//siderbar 網頁DIV連結事件
function DragItemEvent(dragItem) {
    var id = dragItem.data("id");
    var hrefL = dragItem.data("href");

    if (hrefL != "" && hrefL != null) {
        window.top.mainWebContent.location.href = hrefL;
    }

}
// 後台iframe載入畫面
var indicator = $('#loader');
var content_start_loading = function () {
    //console.log('fadeIn')
    indicator.fadeIn();
};
function content_finished_loading(iframe) {
    //console.log('fadeOut')
    indicator.fadeOut();
    // inject the start loading handler when content finished loading
    iframe.contentWindow.onunload = content_start_loading;
};
