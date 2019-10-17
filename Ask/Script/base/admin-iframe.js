$(function () {

    //datepicker
    $('.datetimepicker').datetimepicker({
        format: "YYYY.MM.DD HH:mm" //carrie 20170814
    });
    $('.datepicker').datetimepicker({
        format: "YYYY.MM.DD"
    });
    $('.datepickerM').datetimepicker({
        format: "YYYY.MM"
    });
    $('.timepicker').datetimepicker({
        format: "LT"
    });

    //carrie 201707017
    $('.dropdown-button').dropdown({
        constrainWidth: false,
        stopPropagation: true
    });
    $('.dropdown-content.closeOnClick').on('click', function (e) {
        e.stopPropagation();
    });
    //carrie 201707017 end

    if ($("#SliderTabs").length) {
        $("#SliderTabs").sliderTabs({
            mousewheel: false,
            tabHeight: "auto"
        });
    }

    //colorbox
    $('.openCopyMenu, .openEdit').colorbox({
        width: function () {
            var NW = "400";
            if ($.isNumeric($(this).data("width"))) {
                NW = $(this).data("width");
            }
            return NW;
        },
        height: function () {
            var NH = "250";
            if ($.isNumeric($(this).data("height"))) {
                NH = $(this).data("height");
            }
            return NH;
        },
        transition: false,
        maxWidth: "85%",
        maxHeight: "80%",
        opacity: 0,
        left: function () {
            var NH = "20";
            if ($.isNumeric($(this).data("left"))) {
                NH = $(this).data("left");
            }
            return NH;
        },
        bottom: "50",
        iframe: true,
        fadeOut: 100
    });

    $('.openEdit-c').colorbox({
        width: function () {
            var NW = "430";
            if ($.isNumeric($(this).data("width"))) {
                NW = $(this).data("width");
            }
            return NW;
        },
        height: function () {
            var NH = "300";
            if ($.isNumeric($(this).data("height"))) {
                NH = $(this).data("height");
            }
            return NH;
        },
        transition: false,
        maxWidth: "85%",
        maxHeight: "85%",
        opacity: 0,
        iframe: true,
        fadeOut: 100
    });

    $('.openInlineEdit').colorbox({
        width: function () {
            var NW = "75%";
            if ($.isNumeric($(this).data("width"))) {
                NW = $(this).data("width");
            }
            //console.log("NW:" + NW);
            return NW;
        },
        height: function () {
            var NH = "85";
            if ($.isNumeric($(this).data("height"))) {
                NH = $(this).data("height");
            }
            return NH;
        },
        transition: false,
        maxWidth: "75%",
        maxHeight: "30%",
        opacity: 0,
        left: function () {
            var NH = "70";
            if ($.isNumeric($(this).data("left"))) {
                NH = $(this).data("left");
            }
            return NH;
        },
        bottom: "60",
        inline: true,
        fadeOut: 300,
        fixed: true
    });

    $(".openEdit-m").on('click', function (e) {
        e.preventDefault();
        window.parent.$.colorbox({
            href: $(this).attr('href'),
            width: "600",
            height: "95%",
            right: "20",
            iframe: true,
            transition: false,
            speed: 0,
            fadeOut: 100,
            onOpen: function () {
                window.parent.$("#loader").fadeIn();
            },
            onComplete: function () {
                window.parent.$("#loader").fadeOut();
            },
            onClosed: function () {
                var formBox = window.parent.$("#leftEditBox");

                if (formBox.length) {
                    if (formBox.hasClass('active')) {
                        formBox.fadeOut(300).removeClass('active');
                    }
                }
            }
        });
    });

    $(".openEdit-m-l").on('click', function (e) {
        e.preventDefault();
        window.parent.$.colorbox({
            href: $(this).attr('href'),
            width: "600",
            height: "95%",
            left: "20",
            iframe: true,
            transition: false,
            speed: 0,
            fadeOut: 100,
            onOpen: function () {
                window.parent.$("#loader").fadeIn();
            },
            onComplete: function () {
                window.parent.$("#loader").fadeOut();
            }
        });
    });

    $(".openEdit-lg").on('click', function (e) {
        e.preventDefault();
        window.parent.$.colorbox({
            href: $(this).attr('href'),
            width: "95%",
            height: "95%",

            iframe: true,
            transition: false,
            speed: 0,
            fadeOut: 100
        });
    });

    $('select').material_select();

    //tooltipster
    $('.tooltip').tooltipster({
        maxWidth: 250,
        // trigger: 'click'
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
    //    parent.window.swal({
    //        // title: 'Are you sure?',
    //        text: '刪除後，資料無法復原，並且連同下層的子選單都會一併刪除！確定刪除本單元？',
    //        type: 'warning',
    //        showCancelButton: true,
    //        // confirmButtonColor: '#3085d6',
    //        // cancelButtonColor: '#d33',
    //        confirmButtonText: '刪除',
    //        cancelButtonText: '取消',
    //        animation: false,
    //        customClass: 'animated fadeIn'
    //    });
    //});
    $('.data-del').on('click', function () {
        parent.window.swal({
            // title: 'Are you sure?',
            text: '刪除後，資料無法復原，確定刪除？',
            type: 'warning',
            showCancelButton: true,
            // confirmButtonColor: '#3085d6',
            // cancelButtonColor: '#d33',
            confirmButtonText: '刪除',
            cancelButtonText: '取消',
            animation: false,
            customClass: 'animated fadeIn'
        });
    });
    //20190402 add susan 徵才模組新增
    $('.data-accompany').on('click', function () {
        parent.window.swal({
            // title: 'Are you sure?',
            text: '確定解除同行關係？',
            type: 'warning',
            showCancelButton: true,
            // confirmButtonColor: '#3085d6',
            // cancelButtonColor: '#d33',
            confirmButtonText: '解除',
            cancelButtonText: '取消',
            animation: false,
            customClass: 'animated fadeIn'
        });
    });
    $('.data-Published').on('click', function () {
        if ($(this).prop('checked') == true) {
            parent.window.swal({
                // title: 'Are you sure?',
                text: '是否一併下架？',
                type: 'warning',
                showCancelButton: true,
                // confirmButtonColor: '#3085d6',
                // cancelButtonColor: '#d33',
                confirmButtonText: '確認',
                cancelButtonText: '取消',
                animation: false,
                customClass: 'animated fadeIn'
            });
        }
    });
    $('.data-MFC').on('click', function () {
        parent.window.swal({
            // title: 'Are you sure?',
            text: '確認發放獎金？',
            type: 'warning',
            showCancelButton: true,
            // confirmButtonColor: '#3085d6',
            // cancelButtonColor: '#d33',
            confirmButtonText: '確認',
            cancelButtonText: '取消',
            animation: false,
            customClass: 'animated fadeIn'
        });
    });
    $('.copy-web').on('click', function (e) {
        var url = this.href;
        parent.window.swal({
            // title: 'Are you sure?',
            text: '確定複製本網站？',
            type: 'warning',
            showCancelButton: true,
            // confirmButtonColor: '#3085d6',
            // cancelButtonColor: '#d33',
            confirmButtonText: '確定',
            cancelButtonText: '取消',
            animation: false,
            customClass: 'animated fadeIn',
            allowOutsideClick: true,
        },
            function () {
                parent.window.open(url, '_blank');
            });
        return false;
    });

    // 寫成一個function Carrie 20170612
    function openTheformBox(src, formBox) {
        if (formBox.is(':visible'))
            formBox.hide();
        var iframe = formBox.find('.iframe');
        iframe.prop('src', src);
        iframe.on('load', function () {
            window.parent.closeCenterEdit = function () {
                formBox.fadeOut(300).removeClass('active');
            };
            //20190419 susan add 燈箱載入畫面
            //console.log('open fadeOut')
            window.parent.$("#loader").fadeOut();
            formBox.fadeIn(300).addClass('active');

        });
    }
    $('.openCenterEdit-m').on('click', function (e) {
        //20190419 susan add 燈箱載入畫面
        //console.log('open fadeIn')
        window.parent.$("#loader").fadeIn();
        var formBox = window.parent.$("#centerEditBox-middle");
        e.preventDefault();
        if (formBox.hasClass('active')) {
            formBox.fadeOut(300).removeClass('active');
        } else {
            openTheformBox(this.href, formBox);
        }
    });
    $('.openLeftEdit').on('click', function (e) {
        var formBox = window.parent.$("#leftEditBox");
        e.preventDefault();
        if (formBox.hasClass('active')) {
            formBox.fadeOut(300).removeClass('active');
        } else {
            openTheformBox(this.href, formBox);
        }
    });
});

//關閉Box
function CloseBox() {
    parent.$.colorbox.close();
}

//修改選單資料
function UpdateSiderBar(SiteID, AreaID, Id, Title, ShowStatus) {
    ShowStatus = ShowStatus || 1;
    if (parent.$("#sideBarMenu").length > 0) {
        parent.$("#sideBarMenu").find("div.divSiderMenu").each(function (index, item) {
            var $div = $(this);
            if ($div.data("areaid") == AreaID) {
                var isNew = true;
                $div.find(".dd-item").each(function () {
                    var $li = $(this);
                    if ($li.data('id') == Id) {
                        isNew = false;
                        $li.find("div.dd-handle").find("span.menu-title").eq(0).text(Title);
                        $iShowStatus = $li.find("div.dd-handle").find("span.icons-box").find("i").eq(0);
                        $iShowStatus.removeClass();
                        $.get("/Backend/menus/GetShowStatusClass/?ShowStatus=" + ShowStatus, function (data) {
                            $iShowStatus.addClass(data);
                        });
                    }
                });

                if (isNew) {
                    $.get("/Backend/menus/GetAddMenuTag/" + Id, { SiteID: SiteID, AreaID: AreaID, ID: Id }, function (data) {
                        $div.find("ol").eq(0).append(data);
                    });
                }

            }

        });
    }
    CloseBox();
}


