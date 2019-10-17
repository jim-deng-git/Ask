var currentPlayerMode = "youtube";
var currentVimeoPlayerName = "";
var currentVimeoPlaySeconds = 0;
var currentVimeoStatus = "";
var isSwitchVimeoPlayers = false;
var vimeoReadyCount = 0;
var isInitLoadVimeoPlay = false;
var myTimer,
    yt = {
        "main": "mainVideo", //上方youtube播放器id
        "fixed": "fixed-mainVideo",
        "fixedBox": ".fixed-mainVideo",
        "fixedShowedClassName": "showed",
        "showFixedBox": .75
    };
var controlPlayed = function (pos) {
    var target = $(".played")
    if (currentPlayerMode == "youtube") {
        $(yt.main).addClass("played");
    }
    if (currentPlayerMode == "vimeo") {
        if ($("#" + yt.main).length > 0) {
            $("#" + yt.main).addClass("played");
        }
        if (vimeoReadyCount != vimeoPlayerList.length)
            return;
        //if (!isInitLoadVimeoPlay) {
        //    isInitLoadVimeoPlay = true;
        //    window[yt.main].play();
        //    window[yt.fixed].play();
        //}

    }
    if (currentPlayerMode == "mp4") {
        target = $("#mainVideo");
    }
    var targetTop = target.position().top,
        targetHeight = target.height();
    if (targetHeight == null || targetHeight <= 0)
        targetHeight = 620;
    var targetBottom = targetTop + targetHeight,
    targetPos = targetTop + (targetHeight * yt.showFixedBox),
    openStatus = false;


    //---youtube 的播放器
    if (currentPlayerMode == "youtube") {
        if (pos >= targetPos) {
            if (window[yt.main].getPlayerState() == 1 && openStatus == false) {
                openStatus = true;
                target.css("opacity", .5);
                youTubePlayerPause();
                $(yt.fixedBox).addClass(yt.fixedShowedClassName);

                window[yt.fixed].seekTo(window[yt.main].getCurrentTime());
                window[yt.fixed].playVideo();
            }

        } else if (pos <= targetBottom) {
            if (window[yt.fixed].getPlayerState() == 1 && openStatus == false) {

                target.css("opacity", 1);
                $(yt.fixedBox).removeClass(yt.fixedShowedClassName);
                window[yt.fixed].pauseVideo();

                window[yt.main].seekTo(window[yt.fixed].getCurrentTime());
                window[yt.main].playVideo();
            }
        }

    }
    // vimeo 的播放器
    if (currentPlayerMode == "vimeo") {
        // scroll bar 的底部超出主影片的底 => show 小固定影片
        if (pos >= targetPos) {
            //console.log("A");
            //console.log(currentVimeoPlayerName + "-" + currentVimeoStatus + "-" + openStatus);
            if (currentVimeoPlayerName == "Main" && currentVimeoStatus=="play" && openStatus == false) {
                openStatus = true;
                //target.css("opacity", .5);

                $(yt.fixedBox).addClass(yt.fixedShowedClassName);
                
                isSwitchVimeoPlayers = true;
                currentVimeoPlayerName = "Fixed";
                currentVimeoPlaySeconds = seconds;
                currentVimeoStatus = "play";

                window[yt.main].pause();
                //window[yt.main].setVolume(0);
                var seconds = window[yt.main].getCurrentTime();
                window[yt.fixed].setCurrentTime(seconds);
                //window[yt.fixed].setVolume(1);
                window[yt.fixed].play();

                isSwitchVimeoPlayers = false;
                //console.log("Set to Fixed Play Done!");

                //window[yt.main].pause().then(function () {
                   
                //});
                //window[yt.main].getCurrentTime().then(function (seconds) {
                //    currentVimeoPlaySeconds = seconds;
                //    window[yt.fixed].setCurrentTime(seconds).then(function () {
                //        currentVimeoPlayerName = "Fixed";
                //        currentVimeoPlaySeconds = seconds;
                //        currentVimeoStatus = "play";
                //        isSwitchVimeoPlayers = false;
                //        window[yt.fixed].play();
                //        //window[yt.fixed].play().then(function (data) {
                //        //    currentVimeoPlayerName = "Fixed";
                //        //    currentVimeoPlaySeconds = data.seconds;
                //        //    currentVimeoStatus = "play";
                //        //    isSwitchVimeoPlayers = false;
                //        //    //window[yt.fixed].play();
                //        //});
                //    });
                //});
            }
        }
        // scroll bar 的上高於主影片的底 => show 主影片
        else if (pos <= targetBottom) {
            //console.log("B");
            //console.log(currentVimeoPlayerName + "-" + currentVimeoStatus + "-" + openStatus);
            if (currentVimeoPlayerName == "Fixed" && currentVimeoStatus == "play" && openStatus == false) {
                target.css("opacity", 1);

                isSwitchVimeoPlayers = true;
                currentVimeoPlayerName = "Main";
                currentVimeoPlaySeconds = seconds;
                currentVimeoStatus = "play";

                window[yt.fixed].pause();
                //window[yt.fixed].setVolume(0);
                var seconds = window[yt.fixed].getCurrentTime();
                window[yt.main].setCurrentTime(seconds);
                //window[yt.main].setVolume(1);
                window[yt.main].play();

                isSwitchVimeoPlayers = false;
                //console.log("Set to Main Play Done!");

                //currentVimeoPlayerName = "Main";
                //$(yt.fixedBox).removeClass(yt.fixedShowedClassName);
                //window[yt.fixed].pause().then(function () {
                //    isSwitchVimeoPlayers = true;
                //    window[yt.fixed].getCurrentTime().then(function (seconds) {
                //        currentVimeoPlaySeconds = seconds;
                //        window[yt.main].setCurrentTime(seconds).then(function () {
                //            currentVimeoPlayerName = "Main";
                //            currentVimeoPlaySeconds = seconds;
                //            currentVimeoStatus = "play";
                //            //window[yt.main].play().then(function (data) {
                //            //    currentVimeoPlayerName = "Main";
                //            //    currentVimeoPlaySeconds = data.seconds;
                //            //    currentVimeoStatus = "play";
                //            //    isSwitchVimeoPlayers = false;
                //            //});
                //        });
                //    });
                //});
            }
        }
    }
    //自訂上傳檔案的
    if (currentPlayerMode == "mp4") {
        // scroll bar 的底部超出主影片的底 => show 小固定影片
        if (pos >= targetPos) {
            ////console.log("A");
            var state = window[yt.main].getState();
            //console.log(state + "-" + openStatus);
            if (state == "playing" && openStatus == false) {
                openStatus = true;
                window[yt.main].pause();
                var video_position = window[yt.main].getPosition();
                //target.css("opacity", .5);
                //youTubePlayerPause();
                $(yt.fixedBox).addClass(yt.fixedShowedClassName);
                //console.log(window[yt.fixed].getState());
                if (window[yt.fixed].getState() != "playing") {
                    window[yt.fixed].pause();
                    window[yt.fixed].seek(video_position);
                    //$("#" + yt.fixed).show();
                    //window[yt.fixed].play();
                }
            }

        }
            // scroll bar 的上高於主影片的底 => show 主影片
        else if (pos <= targetBottom) {
            //console.log("B");
            var state = window[yt.fixed].getState();
            //console.log(state + "-" + openStatus);
            if (state == "playing" && openStatus == false)
            {
                window[yt.fixed].pause();
                var video_position = window[yt.fixed].getPosition();
                //$("#" + yt.fixed).hide();
                //target.css("opacity", 1);
                $(yt.fixedBox).removeClass(yt.fixedShowedClassName);
                //console.log(window[yt.main].getState());
                if (window[yt.main].getState() != "playing") {
                    window[yt.main].pause();
                    window[yt.main].seek(video_position);
                    //window[yt.main].play();
                }
            }
        }

    }
}
    var onReady = function() {
        $(yt.fixedBox).find("a.fxdVideo-colse").on("click", function() {
            $(yt.fixedBox).removeClass(yt.fixedShowedClassName);
            if (window[yt.fixed].stopVideo != null) {
                window[yt.fixed].stopVideo();
            }
            else {
                stopVimeoPlayer();
            }
            $(".played").css("opacity", 1);
        });

        if ((vimeoPlayerList.length) > 0) {
            currentPlayerMode = "vimeo";
            initVimeoVideos();
        }
        if ((mp4PlayerList.length) > 0) {
            currentPlayerMode = "mp4";
            initMp4Videos();
            //$("#" + yt.fixed).hide();
        }
    }

    $(document).ready(onReady);
    $(window).scroll(function () {

        if ((ytPlayerList.length) > 0) {
            currentPlayerMode = "youtube";
        }
        if ((vimeoPlayerList.length) > 0) {
            currentPlayerMode = "vimeo";
        }
        if ((mp4PlayerList.length) > 0) {
            currentPlayerMode = "mp4";
        }
        var scrollPos = $(this).scrollTop();
        if ($("#" + yt.main).length > 0)
            controlPlayed(scrollPos);
    });
    function onYouTubeIframeAPIReady() {
        'use strict';

        var suggestedQuality = 'large';
        var height = 480;
        var width = 640;

        function onError(event) {
            window[yt.main].personalPlayer.errors.push(event.data);
        }
        function onReady(event) {
            var player = event.target;
            player.cueVideoById({
                suggestedQuality: suggestedQuality,
                videoId: videoId
            });
            player.pauseVideo();
        }
        if ((ytPlayerList.length) > 0) {
            currentPlayerMode = "youtube";
            initVideos();
        }
    }   
    function initVimeoVideos() {
        
        for (var i = 0; i < vimeoPlayerList.length; i++) {
            var player = vimeoPlayerList[i];

            var width = "", height = "";
            if (player.width != null && player.width > 0)
                width = player.width + "px";
            if (player.height != null && player.height > 0)
                height = player.height + "px";

            var options = {
                id: player.VideoId,
                loop: 0,
                autopause: 0,
                autoplay: 0,
                width: width
            };

            if (player.DivId == "mainVideo") {
                var vimeo_player1 = new Vimeo.Player(player.DivId, options);
                vimeo_player1.setVolume(1);
                vimeo_player1.on('pause', function (data) {
                    vimeoStateChange(data, "init", "Main", "pause");
                });
                vimeo_player1.on('play', function (data) {
                    vimeoStateChange(data, "init", "Main", "play");
                });
                vimeo_player1.ready().then(function () {
                    vimeoReadyCount++;
                });
                currentVimeoPlayerName = "Main";
                currentVimeoStatus = "pause";
                window[player.Id] = vimeo_player1;
            }
            else if (player.DivId == "fixed-mainVideo") {
                var vimeo_player2 = new Vimeo.Player(player.DivId, options);
                vimeo_player2.setVolume(1);
                vimeo_player2.on('pause', function (data) {
                    vimeoStateChange(data, "init", "Fixed", "pause");
                });
                vimeo_player2.on('play', function (data) {
                    vimeoStateChange(data, "init", "Fixed", "play");
                });
                vimeo_player2.ready().then(function () {
                    vimeoReadyCount++;
                });
                window[player.Id] = vimeo_player2;
            }
            else {
                var vimeo_player3 = new Vimeo.Player(player.DivId, options);
                vimeo_player3.setVolume(1);
                
                vimeo_player3.ready().then(function () {
                    vimeoReadyCount++;
                });
                window[player.Id] = vimeo_player3;
            }
            //vimeo_player.ready().then(function () {
               // vimeoReadyCount++;
            //});
            //vimeo_player.play();
            //vimeo_player.play();
            //console.log("player.DivId:" + player.DivId + "; player.Id: " + player.Id + " Add to window ");
        }
        if (vimeoPlayerList.length > 0)
            playingId = vimeoPlayerList[0].Id;
    }

    function initMp4Videos() {
        for (var i = 0; i < mp4PlayerList.length; i++) {
            var player = mp4PlayerList[i];
            //try
            //{
            var width = "100%", height = "100%";
            if (player.width != null && player.width > 0)
                width = player.width + "px";
            if (player.height != null && player.height > 0)
                height = player.height + "px";
          var mp4_player = jwplayer(player.DivId).setup({
                width: width,
                    height: height,
                    type: "mp4",
                    file: player.VideoId,
                    image: player.VideoCover,
                    autostart: "0",
                    repeat: "0",
                    showdownload: false,
                    aspectratio: "16:9"//影片比例                
          });
          mp4_player.on("ready", function (e) {
              //$("#fixed-mainVideo").hide();

              $("#fixed-mainVideo").css("position", "fixed");
              $("#fixed-mainVideo").css("z-index", "100");
              $("#mainVideo").css("z-index", "200");
          });
          window[player.Id] = mp4_player;
            //}
            //catch(err)
            //{

            //}
        }
        if (mp4PlayerList.length > 0) {
            playingId = mp4PlayerList[0].Id;
        }
    }
    function initVideos() {
        for (var i = 0; i < ytPlayerList.length; i++) {
            var player = ytPlayerList[i];
            pl = new YT.Player(player.DivId, {
                height: '100%',
                width: '100%',
                videoId: player.VideoId,
                events: {
                    onStateChange: onPlayerStateChange
                },
                playerVars: {
                    rel: 0,
                    showinfo: 0,
                    mute: 0,
                    loop: 1,
                    playlist: player.VideoId,
                    autohide: 1,
                    modestbranding: 1,
                    autoplay: (player.autoPlay) ? 1 : 0
                },
            });
            window[player.Id] = pl;
        }
        if (ytPlayerList.length > 0)
            playingId = ytPlayerList[0].Id;
    }
    function vimeoStateChange(data, source, playername, state) {
        var message = source+" "+playername + " " + state;
        //console.log(message);
        //alert(message);
        if (source != null && source == "init")
        {
            //if (state == "play")
            //{
            //    if (playername == "Fixed") {
            //        window[yt.main].setVolume(0);
            //        window[yt.fixed].setVolume(1);
            //    }
            //    if (playername == "Main") {
            //        window[yt.fixed].setVolume(0);
            //        window[yt.main].setVolume(1);
            //    }
            //}
            currentVimeoPlayerName = playername;
            if (data != null) {
                currentVimeoPlaySeconds = data.seconds;
            }
            currentVimeoStatus = state;
        }
    }
    function stopVimeoPlayer() {
        //console.log("stopVimeoPlayer");
        if (!isSwitchVimeoPlayers) {
            if (currentVimeoPlayerName != "Main")
                window[yt.main].stop();
            if (currentVimeoPlayerName != "Fixed")
                window[yt.fixed].stop();
        }
    }
    function playVimeoPlayer() {
        if (!isSwitchVimeoPlayers) {
            if (currentVimeoPlayerName != "Main") {
                window[yt.main].setCurrentTime(currentVimeoPlaySeconds);
                window[yt.main].play();
            }
            if (currentVimeoPlayerName != "Fixed") {
                window[yt.fixed].setCurrentTime(currentVimeoPlaySeconds);
                window[yt.fixed].play();
            }
            currentVimeoStatus = "play";
        }
    }
    function youTubePlayerActive() {
        'use strict';
        return window[yt.main] && window[yt.main].hasOwnProperty('getPlayerState');
    }       
    function youTubePlayerPlay() {
        'use strict';

        if (youTubePlayerActive()) {
            window[yt.main].playVideo();
        }
    }
    function youTubePlayerPause() {
        'use strict';

        if (youTubePlayerActive()) {
            window[yt.main].pauseVideo();
        }
    }
    function youTubePlayerStop() {
        'use strict';

        if (youTubePlayerActive()) {
            window[yt.main].stopVideo();
            window[yt.main].clearVideo();
        }
    }   
    function onPlayerStateChange(event){
        var player = event.target;
        if (player.getPlayerState() == 1) {
            if (playingId != player.a['id']) {
                window[playingId].pauseVideo();
                playingId = player.a['id'];
            }
        }
        ////console.log(window[playingId].getCurrentTime());
    }

    (function () {
        //'use strict';

        function init() {
            var tag = document.createElement('script');
            tag.src = 'https://www.youtube.com/iframe_api';

            var first_script_tag = document.getElementsByTagName('script')[0];

            first_script_tag.parentNode.insertBefore(tag, first_script_tag);
            //setInterval(youTubePlayerDisplayInfos, 5000);
            var tag2 = document.createElement('script');
            tag2.src = 'https://player.vimeo.com/api/player.js';
            first_script_tag.parentNode.insertBefore(tag2, first_script_tag);

            var tag3 = document.createElement('script');
            tag3.src = '../../Script/jwplayer/jwplayer.js';
            first_script_tag.parentNode.insertBefore(tag3, first_script_tag);
        }


        if (window.addEventListener) {
            window.addEventListener('load', init);
        } else if (window.attachEvent) {
            window.attachEvent('onload', init);
        }
    }());