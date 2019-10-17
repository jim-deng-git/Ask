$(function() {
    // $(window).on('load', function() {
    if ($('.advertising-content .videos').length > 0) {
        $(".advertising-content .videos").each(function() {
            var _this_url = $(this).attr('href');
            if (parseVideo(_this_url).service === 'youtube') {
                $(this).attr("href", "https://youtube.com/embed/" + parseVideo(_this_url).id + "?rel=0&autoplay=1&modestbranding=0&wmode=transparent&start=0&controls=0&showinfo=0&loop=1");
            } else if (parseVideo(_this_url).service === 'vimeo') {
                $(this).attr("href", "https://player.vimeo.com/video/" + parseVideo(_this_url).id + '?loop=1&autoplay=1');
            } else {
                return false;
            }
        }).colorbox({
            width: 1200,
            height: 675,
            maxWidth: '95%',
            maxHeight: '78%',
            iframe: true,
            closeButton: true
        });
    }  

    $('.card-advertising .ad-close-btn').on("click", function() {
        $(this).parents('.card-advertising').slideUp();
    });

    // 蓋台廣告
    if ($('.card-advertising.card-overlay .overlay').length > 0) {

            var _this_overlay = $('.card-advertising.card-overlay .overlay'),
                _this_overlay_id = _this_overlay.attr('id')+'_'+_this_overlay.data('page-id');

        $(window).on('resize', function(){
            var $w = $(window).outerWidth();

            if ($w > 800) {
                $('.card-advertising.card-overlay .overlay').each(function() {

                    if($('.videoPopUp').length) {
                        var _this_url = $(this).attr('href');
                        if (parseVideo(_this_url).service === 'youtube') {
                            $(this).attr("href", "https://youtube.com/embed/" + parseVideo(_this_url).id + "?rel=0&autoplay=1&modestbranding=0&wmode=transparent&start=0&controls=0&showinfo=0&loop=1");
                        } else if (parseVideo(_this_url).service === 'vimeo') {
                            $(this).attr("href", "https://player.vimeo.com/video/" + parseVideo(_this_url).id + '?loop=1&autoplay=1');
                        } else {
                            return false;
                        }
                    }
                }).colorbox({
                    width: function() {
                        var _this_url = $(this).attr('href');
                        if (parseVideo(_this_url).service === 'youtube' || parseVideo(_this_url).service === 'vimeo') {
                            return 1200;
                        } else {
                            return false;
                        }
                    },
                    height: function() {
                        var _this_url = $(this).attr('href');
                        if (parseVideo(_this_url).service === 'youtube' || parseVideo(_this_url).service === 'vimeo') {
                            return 675;
                        } else {
                            return false;
                        }
                    },
                    maxWidth: '95%',
                    maxHeight: '78%',
                    closeButton: true,
                    overlayClose: false,
                    className: 'overlay_ad',
                    close: '&nbsp;',
                    iframe: function() {
                        var _this_url = $(this).attr('href');
                        if (parseVideo(_this_url).service === 'youtube' || parseVideo(_this_url).service === 'vimeo') {
                            return true;
                        } else {
                            return false;
                        }
                    },
                    onComplete: function() {
                        if ($(this).data('ad-link') > 0) {
                            var _this_url = $(this).data('ad-link');                      
                            if (parseVideo(_this_url).service === 'other' && $(this).data('ad-link') != '') {
                                $('img.cboxPhoto').wrap('<a href="' + _this_url + '" target="_blank"></a>');
                                $('.overlay_ad').addClass('images-link');
                            }
                        }
                        if ($(this).data('ad-delay') > 0) {
                            var ad_times = $(this).data('ad-delay');
                            if ($('.timer').length < 1) {
                                $('.overlay_ad #cboxContent').append('<div class="timer">廣告時間: <span class="value">' + ad_times + '</span></div>');

                                function counter() {
                                    ad_times--;
                                    $('.timer .value').html(ad_times);
                                    if (ad_times <= 0) {
                                        clearInterval(start);
                                        $('.overlay_ad').addClass('time-up');
                                    }
                                }
                                start = setInterval(function() { counter() }, 1000);
                            }
                        }
                    }
                });
            } else {
                $('.card-advertising.card-overlay .overlay').each(function() {
                    if($('.videoPopUp').length) {
                        var _this_url = $(this).data('mobile-href');
                        if (parseVideo(_this_url).service === 'youtube') {
                            $(this).attr("href", "https://youtube.com/embed/" + parseVideo(_this_url).id + "?rel=0&autoplay=1&modestbranding=0&wmode=transparent&start=0&controls=0&showinfo=0&loop=1");
                        } else if (parseVideo(_this_url).service === 'vimeo') {
                            $(this).attr("href", "https://player.vimeo.com/video/" + parseVideo(_this_url).id + '?loop=1&autoplay=1');
                        } else {
                            return false;
                        }
                    }
                    if($('.linkPopUp').length) {
                        var $img_mobile = $(this).find('img'),
                            $mobile_src = $img_mobile.data('mobile-src');

                        $(this).attr("href", $mobile_src);
                        $img_mobile.attr("src", $mobile_src);
                    }
                }).colorbox({
                    width: function() {
                        var _this_url = $(this).attr('href');
                        if (parseVideo(_this_url).service === 'youtube' || parseVideo(_this_url).service === 'vimeo') {
                            return 1200;
                        } else {
                            return false;
                        }
                    },
                    height: function() {
                        var _this_url = $(this).attr('href');
                        if (parseVideo(_this_url).service === 'youtube' || parseVideo(_this_url).service === 'vimeo') {
                            return 675;
                        } else {
                            return false;
                        }
                    },
                    maxWidth: '95%',
                    maxHeight: '78%',
                    closeButton: true,
                    overlayClose: false,
                    className: 'overlay_ad',
                    close: '&nbsp;',
                    iframe: function() {
                        var _this_url = $(this).attr('href');
                        if (parseVideo(_this_url).service === 'youtube' || parseVideo(_this_url).service === 'vimeo') {
                            return true;
                        } else {
                            return false;
                        }
                    },
                    onComplete: function() {
                        if ($(this).data('ad-link') > 0) {
                            var _this_url = $(this).data('ad-link');                      
                            if (parseVideo(_this_url).service === 'other' && $(this).data('ad-link') != '') {
                                $('img.cboxPhoto').attr('src', $mobile_src).wrap('<a href="' + _this_url + '" target="_blank"></a>');
                                $('.overlay_ad').addClass('images-link');
                            }
                        }
                        if ($(this).data('ad-delay') > 0) {
                            var ad_times = $(this).data('ad-delay');
                            if ($('.timer').length < 1) {
                                $('.overlay_ad #cboxContent').append('<div class="timer">廣告時間: <span class="value">' + ad_times + '</span></div>');

                                function counter() {
                                    ad_times--;
                                    $('.timer .value').html(ad_times);
                                    if (ad_times <= 0) {
                                        clearInterval(start);
                                        $('.overlay_ad').addClass('time-up');
                                    }
                                }
                                start = setInterval(function() { counter() }, 1000);
                            }
                        }
                    }
                });
            }
        }).trigger('resize');

        /**start random**/
        if (_this_overlay.data('random') != '') {
            var _max = 100,
                _min = 0,
                _chnage = _this_overlay.data('random'),
                current_num = Math.floor(Math.random() * (_max - _min + 1)) + _min;

            if (current_num < _chnage) {
                _this_overlay.click();
            }
            console.log('機率:' + current_num + '+' + _chnage);
        }

        /**start idle**/
        if (_this_overlay.data('idle') != '') {
            var _mouse_x,
                _temp_mouse_x,
                _mouse_y,
                _temp_mouse_y,
                _idle_time = _this_overlay.data('idle'),
                _counter_idle_time = 0;

            document.onmousemove = function(event) {
                var e = event || window.event;
                if (e.pageX || e.pageY) {
                    _mouse_x = e.pageX;
                    _mouse_y = e.pageY;
                } else {
                    _mouse_x = e.clientX + document.body.scrollLeft - document.body.clientLeft;
                    _mouse_y = e.clientY + document.body.scrollTop - document.body.clientTop;
                }
            }

            document.onmousedown = function (event) {
                console.log('clicked');
                _counter_idle_time = 0;
            }

            function idle_counter() {
                if (_temp_mouse_x != _mouse_x || _temp_mouse_y != _mouse_y) {
                    _temp_mouse_x = _mouse_x;
                    _temp_mouse_y = _mouse_y;
                    _counter_idle_time = 0;
                } else {
                    _counter_idle_time++;
                }
                if (_counter_idle_time >= _idle_time) {
                    _light_box_type = $('#colorbox').css('display');
                    if (_light_box_type != 'block') {
                        _this_overlay.click();
                    }
                }
                // console.log(_mouse_x +'+'+_mouse_y);
                // console.log($('#colorbox').css('display'));
            }

            // console.log(_mouse_x + '+' + _mouse_y);
            idle = setInterval(function() { idle_counter() }, 1000);
        }

        /**one-time**/
        if (_this_overlay.data('one-time') != '') {
            var _onetime = _this_overlay.data('one-time');
            var timestamp = Date.parse(new Date());
            if (_onetime === 'on') {
                if ($.cookie(_this_overlay_id) != null) {
                    console.log(_this_overlay_id + '已有值' + $.cookie(_this_overlay_id));
                } else {
                    $.cookie(_this_overlay_id, timestamp, { expires: 1, path: '/' });
                    _light_box_type = $('#colorbox').css('display');
                    if (_light_box_type != 'block') {
                        _this_overlay.click();
                    }
                }
            }
        }

        /**loop**/
        if (_this_overlay.data('loop') != '') {
            var _loop = _this_overlay.data('loop'),
                _this_time = Date.parse(new Date()),
                _last_time = $.cookie(_this_overlay_id);
            if (_last_time != null) {
                var intervals = (_this_time - _last_time) / 1000;
                if (intervals > _loop) {
                    _light_box_type = $('#colorbox').css('display');
                    if (_light_box_type != 'block') {
                        _this_overlay.click();
                    }
                    $.cookie(_this_overlay_id, _this_time, { expires: 1, path: '/' });
                }
                console.log('_this_time' + _this_time + '_last_time' + _last_time + 'intervals' + intervals);
            } else {
                _light_box_type = $('#colorbox').css('display');
                if (_light_box_type != 'block') {
                    _this_overlay.click();
                }
                $.cookie(_this_overlay_id, _this_time, { expires: 1, path: '/' });
            }
        }

    }
    // });
});

/*
console.log('youtube:' + parseVideo(linksss).id + '  service:' + parseVideo(linksss).service);
*/
function parseVideo(str) {
    // remove surrounding whitespaces or linefeeds
    str = str.trim();

    // remove the '-nocookie' flag from youtube urls
    str = str.replace('-nocookie', '');

    // remove any leading `www.`
    str = str.replace('/www.', '/');

    var metadata = {};

    // Try to handle google redirection uri
    if (/\/\/google/.test(str)) {
        // Find the redirection uri
        var matches = str.match(/url=([^&]+)&/);

        // Decode the found uri and replace current url string - continue with final link
        if (matches) {
            // Javascript can get encoded URI
            str = decodeURIComponent(matches[1]);
        }
    }

    if (/youtube|youtu\.be|i.ytimg\./.test(str)) {
        metadata = {
            id: youtube(str),
            service: 'youtube'
        };
    } else if (/vimeo/.test(str)) {
        metadata = {
            id: vimeo(str),
            service: 'vimeo'
        };
    } else {
        metadata = {
            id: 'other',
            service: 'other'
        };
    }
    return metadata;
}


/**
 * Get the vimeo id.
 * @param {string} str - the url from which you want to extract the id
 * @returns {string|undefined}
 */
function vimeo(str) {
    if (str.indexOf('#') > -1) {
        str = str.split('#')[0];
    }
    if (str.indexOf('?') > -1) {
        str = str.split('?')[0];
    }

    var id;
    if (/https?:\/\/vimeo\.com\/[0-9]+$|https?:\/\/player\.vimeo\.com\/video\/[0-9]+$|https?:\/\/vimeo\.com\/channels|groups|album/igm.test(str)) {
        var arr = str.split('/');
        if (arr && arr.length) {
            id = arr.pop();
        }
    }
    return id;
}

/**
 * Get the Youtube Video id.
 * @param {string} str - the url from which you want to extract the id
 * @returns {string|undefined}
 */
function youtube(str) {
    // shortcode
    var shortcode = /youtube:\/\/|https?:\/\/youtu\.be\//g;

    if (shortcode.test(str)) {
        var shortcodeid = str.split(shortcode)[1];
        return stripParameters(shortcodeid);
    }

    // /v/ or /vi/
    var inlinev = /\/v\/|\/vi\//g;

    if (inlinev.test(str)) {
        var inlineid = str.split(inlinev)[1];
        return stripParameters(inlineid);
    }

    // v= or vi=
    var parameterv = /v=|vi=/g;

    if (parameterv.test(str)) {
        var arr = str.split(parameterv);
        return arr[1].split('&')[0];
    }

    // v= or vi=
    var parameterwebp = /\/an_webp\//g;

    if (parameterwebp.test(str)) {
        var webp = str.split(parameterwebp)[1];
        return stripParameters(webp);
    }

    // embed
    var embedreg = /\/embed\//g;

    if (embedreg.test(str)) {
        var embedid = str.split(embedreg)[1];
        return stripParameters(embedid);
    }

    // user
    var userreg = /\/user\//g;

    if (userreg.test(str)) {
        var elements = str.split('/');
        return stripParameters(elements.pop());
    }

    // attribution_link
    var attrreg = /\/attribution_link\?.*v%3D([^%&]*)(%26|&|$)/;

    if (attrreg.test(str)) {
        return str.match(attrreg)[1];
    }
}

/**
 * Strip away any parameters following `?` or `/`
 * @param str
 * @returns {*}
 */
function stripParameters(str) {
    // Split parameters or split folder separator
    if (str.indexOf('?') > -1) {
        return str.split('?')[0];
    } else if (str.indexOf('/') > -1) {
        return str.split('/')[0];
    }
    return str;
}