var Member = {};

Member.imageUpload = function (elm, basePath, uploadUrl, browserUrl, deleteUrl, socialLargePhoto) {
    var changeInputHtml =
       '<div class="fileuploader-items singleImg b-radius">' +
                  '<ul class="fileuploader-items-list">' +
                    '<li class="fileuploader-thumbnails-input">' +
                      '<a href="JavaScript:;" class="user"><i class="cc cc-user-o"></i></a>' +
                      '<a href="JavaScript:;" class="camera"><i class="cc cc-camera-o"></i></a>' +
                    '</li>' +
                  '</ul>' +
                '</div>';

    var itemHtml =
        '<li class="fileuploader-item">' +
                    '<div class="fileuploader-item-inner">' +
                      '<div class="thumbnail-holder b-radius">${image}</div>' +
                      '<div class="actions-holder">' +
                        '<a id="btnDeleteMemberPhoto" class="btn-del btn-white-o square transparent fileuploader-action-remove tooltip" title="刪除" href="javascript:"><i class="cc cc-close"></i></a>' +
                      '</div>' +
                      '<div class="progress-holder">${progressBar}</div>' +
                    '</div>' +
                  '</li>';
    var item2Html = '<li class="fileuploader-item">' +
                    '<div class="fileuploader-item-inner">' +
                      '<div class="thumbnail-holder b-radius">${image}</div>' +
                      '<div class="actions-holder">' +
                        '<a id="btnDeleteMemberPhoto" class="btn-del btn-white-o square transparent fileuploader-action-remove tooltip" title="刪除" href="javascript:"><i class="cc cc-close"></i></a>' +
                      '</div>' +
                    '</div>' +
                  '</li>';

    var fileElm = $(elm);

    var files = [];
    var imgs = $('#' + fileElm.attr('data-field')).val();
    if (imgs) {
       
        if (imgs.length > 0) {
            var img = imgs;
            var m = img.match(/\.([^\.]+)$/);
            if (m) {
                var name = img.split('/');
                name = name[name.length - 1];
                //console.log(browserUrl + '/' + encodeURIComponent(img));
                if (img.indexOf("http") == 0)
                    files.push({ name: name, file: socialLargePhoto, type: 'image/' + m[1], size: 0 });
                else
                    files.push({ name: name, file: browserUrl + encodeURIComponent(img), type: 'image/' + m[1], size: 0 });
            }
        }
    }

    function initNewItem(item) {
        item.html.find('div.progress-holder').html('');
        item.html.find('.tooltip').tooltipster({
            maxWidth: 100
        });
        item.html.find('div.actions-holder').click(function (e) {
            if (e.target == this) {
                //$.colorbox({ html: $(this).prev().find('img').clone() }); /*Joe 20190925 拿掉預覽圖*/
            }
        });
        item.html.disableSelection();
    }
    function DeleteMemberPhoto()
    {
        $.post(deleteUrl, function () {
            // location.replace(location.href);
            $(".fileuploader-items-list").children("li:first").remove();
            $("#imgMemberPhoto").remove();
        });
    }
    var imgsIndex = 0;
    var maxSize = fileElm.attr('data-size');
    maxSize = maxSize ? maxSize - 0 : 2;

    var limitNum = fileElm.attr('data-max');
    limitNum = limitNum ? limitNum - 0 : 1;

    fileElm.fileuploader({
        files: files,
        extensions: ['jpg', 'jpeg', 'png', 'gif'],
        fileMaxSize: maxSize,
       
        changeInput: changeInputHtml,
        theme: 'thumbnails',
        enableApi: true,
        addMore: true,
        thumbnails: {
            files: files,
            item: itemHtml,
            item2: item2Html,
            startImageRenderer: true,
            canvasImage: false,
            _selectors: {
                list: '.fileuploader-items-list',
                item: '.fileuploader-item',
                start: '.fileuploader-action-start',
                retry: '.fileuploader-action-retry',
                remove: '.fileuploader-action-remove'
            },
            onItemShow: function (item, listEl, parentEl, newInputEl, inputEl) {
                var plusInput = $('.fileuploader-thumbnails-input');// listEl.find('.fileuploader-thumbnails-input');
                plusInput.parent().prepend(item.html);
               // item.parent().html.insertBefore(plusInput);
                //plusInput.parent().append(item.html);
                //plusInput.insertAfter(item.html);
               
                if (item.format == 'image') {
                    item.html.find('.fileuploader-item-icon').hide();
                };

                if (item.appended) { // 以前上傳的檔案
                    item.html.find('input:hidden').val(imgs[imgsIndex++]);
                }

                initNewItem(item);
            }
        },
        upload: {
            url: uploadUrl,
            data: { type: 'img' }, // 當前未使用，目的:校驗上傳文件類型
            type: 'POST',
            enctype: 'multipart/form-data',
            start: true,
            beforeSend: function (item, listEl, parentEl, newInputEl, inputEl) {
                Member.imageUpload.uploading = true;
            },
            onSuccess: function (data, item, listEl, parentEl, newInputEl, inputEl, textStatus, jqXHR) {
                //console.log(data);
                if (!data)
                    return;


                item.html.find('input:hidden').val(data);
                if ($("span[id=imgMemberPhoto]").length > 0)
                    $("span[id=imgMemberPhoto]").attr("src", browserUrl + data);
                else
                {
                    if ($("span[id=imgMemberPhoto]").length > 0)
                    {
                        $("span[id=imgMemberPhoto]").prepend("<img src=\"" + browserUrl + data + "\" id=\"imgMemberPhoto\" />");
                    }
                }
                initNewItem(item);
                $("span[id=imgMemberPhoto]").removeAttr("style").attr("style", "background-image: url('" + browserUrl + data + "')");// Joe20190926 右上角大頭貼更新(更新選擇器)
                
                $("ul[class=fileuploader-items-list] li:eq(1)").remove(); //Joe 20190926 清除舊的大頭貼:刪掉ul class含fileuploader-items-list底下的第二個li
                reloadDIV();
            },
            onComplete: function (listEl, parentEl, newInputEl, inputEl, jqXHR, textStatus) {
                Member.imageUpload.uploading = false;
            }
        }
    });

    $("#btnDeleteMemberPhoto").click(function () {
        DeleteMemberPhoto();
    });
    //var itemOuterElm = fileElm.nextAll('div.fileuploader-items').find('ul');
    //itemOuterElm.sortable({ items: 'li' });

    //fileElm.closest('form').submit(function () {
    //    var field = fileElm.attr('data-field');
    //    var files = [];
    //    itemOuterElm.find('li').each(function (n) {
    //        $(this).find('input').each(function () {
    //            files.push($.trim(this.value));
    //        });
    //    });

    //    $('#' + field).val(JSON.stringify(files));
    //});
}

function reloadDIV() { //20190926更新右上角頭貼區域
    $("div[class=nav-dropdown member]").load(self);
} 