var Form = {};

Form.imageUpload = function (elm, basePath, uploadUrl, browserUrl) {
    var changeInputHtml =
        '<div class="fileuploader-input">' +
        '   <div class="fileuploader-input-inner">' +
        '       <i class="cc cc-cloud-upload-o"></i>' +
        '       <h3 class="fileuploader-input-caption"><span>Drag and drop files here</span></h3>' +
        '       <p>or</p>' +
        '       <div class="fileuploader-input-button"><span>Browse Files</span></div>' +
        '   </div>' +
        '</div>';

    var itemHtml =
        '<li class="fileuploader-item">' +
        '   <div class="fileuploader-item-inner">' +
        '       <div class="thumbnail-holder">${image}</div>' +
        '       <div class="actions-holder">' +        
        '           <a class="btn-del btn-white-o square transparent fileuploader-action-remove tooltip" title="刪除" href="javascript:"><i class="cc cc-close"></i></a>' +
        '       </div>' +
        '       <div class="progress-holder">${progressBar}</div>' +
        '   </div>' +
        '   <input type="hidden" />' +
        '</li>';
    var item2Html = itemHtml;

    var fileElm = $(elm);
    
    var files = [];
    var imgs = $('#' + fileElm.attr('data-field')).val();
    if (imgs) {
        imgs = $.parseJSON(imgs);
        if (imgs.length > 0) {
            for (var i = 0, len = imgs.length; i < len; ++i) {
                var img = imgs[i];
                var m = img.match(/\.([^\.]+)$/);
                if (m) {
                    var name = img.split('/');
                    name = name[name.length - 1];
                    files.push({ name: name, file: browserUrl + '/' + encodeURIComponent(img), type: 'image/' + m[1], size: 0 });
                }
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
                $.colorbox({ html: $(this).prev().find('img').clone() });
            }
        });        
        item.html.disableSelection();
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
        limit: limitNum,
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
                var plusInput = listEl.find('.fileuploader-thumbnails-input');

                plusInput.insertAfter(item.html);

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
                Form.imageUpload.uploading = true;
            },
            onSuccess: function (data, item, listEl, parentEl, newInputEl, inputEl, textStatus, jqXHR) {
                if (!data)
                    return;
                data = $.parseJSON(data);

                item.html.find('input:hidden').val(data.Name);
                initNewItem(item);
            },
            onComplete: function (listEl, parentEl, newInputEl, inputEl, jqXHR, textStatus) {
                Form.imageUpload.uploading = false;
            }
        }
    });

    var itemOuterElm = fileElm.nextAll('div.fileuploader-items').find('ul');
    itemOuterElm.sortable({ items: 'li' });

    fileElm.closest('form').submit(function () {
        var field = fileElm.attr('data-field');
        var files = [];
        itemOuterElm.find('li').each(function (n) {
            $(this).find('input').each(function () {
                files.push($.trim(this.value));
            });
        });
        
        $('#' + field).val(JSON.stringify(files));
    });
}

Form.fileUpload = function (elm, basePath, uploadUrl, browserUrl) {
    var itemHtml =
        '<li class="fileuploader-item">' +
        '   <div class="columns">' +
        '       <div class="column-thumbnail">${image}</div>' +
        '       <div class="column-title">' +
        '          <div class="title-name" title="${name}">${name}</div>' +        
        '       </div>' +
        '       <div class="column-actions">' +
        '           <a href="javascript:" class="btn-sort square transparent fileuploader-action-sort tooltip" title="排序"><i class="cc cc-drag"></i></a>' +
        '           <a href="${file}" class="btn-download square transparent fileuploader-action-download tooltip" title="下載"><i class="cc cc-download"></i></a>' +        
        '           <a class="btn-del square transparent fileuploader-action-remove tooltip" title="刪除" href="javascript:"><i class="cc cc-close"></i></a>' +
        '       </div>' +
        '   </div>' +
        '   <div class="progress-bar2">${progressBar}<span></span></div>' +
        '</li>'

    var fileElm = $(elm);

    var fileList = [];
    var files = $('#' + fileElm.attr('data-field')).val();
    if (files) {
        files = $.parseJSON(files);
        if (files.length) {
            for (var i = 0, len = files.length; i < len; ++i) {
                var f = files[i];
                var name = f.split('/');
                name = name[name.length - 1];
                fileList.push({ name: name, file: browserUrl + '/' + f, size: 0 });
            }
        }
    }

    var fileIndex = 0;
    var maxSize = fileElm.attr('data-size');
    maxSize = maxSize ? maxSize - 0 : 2;

    var limitNum = fileElm.attr('data-max');
    limitNum = limitNum ? limitNum - 0 : 1;

    var extentions = ["doc", "docx", "xls", "xlsx", "ppt", "pptx", "pdf", "txt", "jpg", "jpeg", "png", "gif", "zip", "rar", "7z", "mp3", "mp4"];
    var formatType = fileElm.attr('data-type');
    if (formatType)
        extentions = formatType.split(',');

    function initNewItem(item) {
        item.html.find('div.progress-bar2').html('');
        item.html.find('.tooltip').tooltipster({
            maxWidth: 100
        });
        item.html.disableSelection();
    }

    fileElm.fileuploader({
        files: fileList,
        extensions: extentions,
        theme: 'dragdrop',
        fileMaxSize: maxSize,
        limit: limitNum,
        thumbnails: {
            item: itemHtml,
            item2: itemHtml,
            onItemShow: function (item, listEl, parentEl, newInputEl, inputEl) {
                var valElm = $('<input type="hidden" />');
                item.html.append(valElm);
                if (item.appended) { // 以前上傳的檔案                    
                    valElm.val(files[fileIndex++]);
                } 
                initNewItem(item);
            }
        },
        upload: {
            url: uploadUrl,            
            type: 'POST',
            enctype: 'multipart/form-data',
            start: true,
            beforeSend: function (item, listEl, parentEl, newInputEl, inputEl) {
                Form.fileUpload.uploading = true;
            },
            onSuccess: function (data, item, listEl, parentEl, newInputEl, inputEl, textStatus, jqXHR) {
                if (!data)
                    return;
                data = $.parseJSON(data);

                item.html.find('input:hidden').val(data.Name);
                item.html.find('a.btn-download').prop('href', browserUrl + '/' + data.Name);
                initNewItem(item);                
            },
            onComplete: function (listEl, parentEl, newInputEl, inputEl, jqXHR, textStatus) {
                Form.fileUpload.uploading = false;
            }
        }
    });

    var itemOuterElm = fileElm.nextAll('div.fileuploader-items').find('ul');
    itemOuterElm.sortable({ items: 'li', handle: '.btn-sort', helper: 'clone', appendTo: 'body' });

    fileElm.closest('form').submit(function () {
        var field = fileElm.attr('data-field');
        var files = [];
        itemOuterElm.find('li').each(function (n) {
            $(this).find('input').each(function () {
                files.push($.trim(this.value));
            });
        });

        $('#' + field).val(JSON.stringify(files));        
    });
}

Form.voiceUpload = function (elm, basePath, uploadUrl, browserUrl) {
    basePath = basePath.replace(/\/+$/, '');

    var changeInputHtml =
        '<div class="fileuploader-input">' +
        '   <div class="fileuploader-input-inner">' +
        '       <i class="cc cc-cloud-upload-o"></i>' +
        '       <h3 class="fileuploader-input-caption"><span>Drag and drop files here</span></h3>' +
        '       <p>or</p>' +
        '       <div class="fileuploader-input-button"><span>Browse Files</span></div>' +
        '   </div>' +
        '</div>';

    var boxHtml =
        '<div class="fileuploader-items voice">' +
        '   <ul class="fileuploader-items-list"></ul>' +
        '</div>';

    var itemHtml =
        '<li class="fileuploader-item">' +
        '   <div class="columns">' +
        '       <div class="column-title">' +
        '           <div class="title-name" title="${name}">${name}</div>' +
        '           <div class="voice-mp3"></div>' +
        '       </div>' +
        '       <div class="column-actions">' +
        '           <a href="javascript:" class="btn-sort square transparent fileuploader-action-sort tooltip" title="排序"><i class="cc cc-drag"></i></a>' +
        '           <a class="btn-download square transparent fileuploader-action-download tooltip" download title="下載" target="_blank"><i class="cc cc-download"></i></a>' +    
        '           <a class="btn-del square transparent fileuploader-action-remove tooltip" title="刪除" href="javascript:"><i class="cc cc-close"></i></a>' +
        '       </div>' +
        '   </div>' +
        '   <div class="progress-bar2">${progressBar}<span></span></div>' +
        '   <input type="hidden" />' +
        '</li>'
    var item2Html = itemHtml;

    var fileElm = $(elm);

    var fileList = [];
    var voices = $('#' + fileElm.attr('data-field')).val();
    if (voices) {
        voices = $.parseJSON(voices);
        if (voices.length) {
            for (var i = 0, len = voices.length; i < len; ++i) {
                var voice = voices[i];
                var name = voice.split('/');
                name = name[name.length - 1];
                fileList.push({ name: name, file: browserUrl + '/' + encodeURIComponent(voice) });
            }
        }
    }

    function player(id, fileName) {
        var jw = jwplayer(id).setup({
            file: browserUrl + '/' + encodeURIComponent(fileName),
            width: 240,
            height: 30,
            skin: {
                //name: "bekle",//主題
                active: "#2e2e2e"//控制列主色
                //inactive: "#ffffff",
                //background: "rgba(0,0,0,0)"//控制列背景
            }
        });

    }

    var voiceIndex = 0;
    var maxSize = fileElm.attr('data-size');
    maxSize = maxSize ? maxSize - 0 : 10;

    var limitNum = fileElm.attr('data-max');
    limitNum = limitNum ? limitNum - 0 : 1;

    fileElm.fileuploader({
        files: fileList,
        fileMaxSize: maxSize,
        limit: limitNum,
        extensions: ['mp3', 'm4a'],
        theme: 'dragdrop',
        changeInput: changeInputHtml,
        thumbnails: {
            box: boxHtml,
            item: itemHtml,
            item2: item2Html,
            onItemShow: function (item, listEl, parentEl, newInputEl, inputEl) {
                var valElm = item.html.find('input:hidden');
                if (item.appended) { // 以前上傳的檔案
                    var voiceItem = voices[voiceIndex++];
                    valElm.val(voiceItem);

                    item.html.find('div.progress-bar2').html('');
                    item.html.find('a.btn-download').prop('href', browserUrl + '/' + encodeURIComponent(voiceItem));

                    var uniqueId = Component.guid();
                    item.html.find('div.voice-mp3').prop('id', uniqueId);
                    player(uniqueId, voiceItem);
                } else {
                    item.html.find('.tooltip').tooltipster({
                        maxWidth: 100
                    });
                }
                
                item.html.disableSelection();
            }
        },
        upload: {
            url: uploadUrl,
            data: { type: 'sound' }, // 當前未使用，目的:校驗上傳文件類型
            type: 'POST',
            enctype: 'multipart/form-data',
            start: true,
            beforeSend: function (item, listEl, parentEl, newInputEl, inputEl) {
                Form.voiceUpload.uploading = true;
            },
            onSuccess: function (data, item, listEl, parentEl, newInputEl, inputEl, textStatus, jqXHR) {
                if (!data)
                    return;
                data = $.parseJSON(data);

                item.html.find('input:hidden').val(data.Name);
                item.html.find('div.progress-bar2').html('');
                item.html.find('a.btn-download').prop('href', browserUrl + '/' + data.Name);

                var uniqueId = Component.guid();
                item.html.find('div.voice-mp3').prop('id', uniqueId);
                player(uniqueId, data.Name);
            },
            onComplete: function (listEl, parentEl, newInputEl, inputEl, jqXHR, textStatus) {
                Form.voiceUpload.uploading = false;
            }
        }
    });

    var itemOuterElm = fileElm.nextAll('div.fileuploader-items').find('ul');
    itemOuterElm.sortable({ items: 'li', handle: '.btn-sort', helper: 'clone', appendTo: 'body' });

    fileElm.closest('form').submit(function () {
        var field = fileElm.attr('data-field');
        var files = [];
        itemOuterElm.find('li').each(function (n) {
            $(this).find('input').each(function () {
                files.push($.trim(this.value));
            });
        });

        $('#' + field).val(JSON.stringify(files));
    });
}

Form.videoUpload = function (elm, uploadUrl, browserUrl, jwplayerReady) {
    var boxHtml =
        '<div class="fileuploader-items singleImg video">' +
        '   <ul class="fileuploader-items-list">' +
        '       <li class="fileuploader-thumbnails-input">' +
        '           <div class="fileuploader-thumbnails-input-inner">' +
        '               <i class="cc cc-cloud-upload-o"></i>' +
        '               <h3 class="fileuploader-input-caption"><span>Drag and drop files here</span></h3>' +
        '               <p>or</p>' +
        '               <div class="fileuploader-input-button"><span>Browse Files</span></div>' +
        '           </div>' +
        '       </li>' +
        '   </ul>' +
        '</div>';

    var itemHtml =
        '<li class="fileuploader-item">' +
        '   <div class="fileuploader-item-inner">' +
        '       <div class="thumbnail-holder"><div class="video-mp4"></div></div>' +
        '       <div class="actions-holder">' +
        '           <a class="btn-del btn-white-o square transparent fileuploader-action-remove tooltip" title="刪除" href="javascript:"><i class="cc cc-close"></i></a>' +
        '       </div>' +
        '       <div class="progress-holder">${progressBar}</div>' +
        '       <input type="hidden" />' +
        '   </div>' +
        '</li>';
    var item2Html = itemHtml;

    var fileElm = $(elm);

    var file = null;
    var video = $('#' + fileElm.attr('data-field')).val();
    if (video) {
        file = { name: video, file: browserUrl + '/' + encodeURIComponent(video) };
    }

    var formatType = fileElm.attr('data-type');    
    formatType = formatType ? formatType.split(',') : [ 'mp4' ]
    
    function player(id, video) {
        jwplayer(id).setup({
            width: "100%",
            height: "100%",
            // type: formatType.join(),
            file: browserUrl + '/' + video,
            skin: {
                name: "default",//選擇主題
                active: "#2e2e2e",//選擇主色
                inactive: "#ffffff",
                background: "rgba(255,255,255,0)"//選擇背景
            },
            showdownload: false,
            aspectratio: "16:9",//影片比例
            autostart: false //自動播放
        });

        jwplayer(id).onBufferChange(jwplayerReady);
    }

    var maxSize = fileElm.attr('data-size');
    maxSize = maxSize ? maxSize - 0 : 20;
    fileElm.fileuploader({
        limit: 1,
        files: file,
        fileMaxSize: maxSize,
        extensions: formatType,
        changeInput: ' ',
        theme: 'thumbnails',
        enableApi: true,
        addMore: false,
        thumbnails: {
            box: boxHtml,
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
                var plusInput = listEl.find('.fileuploader-thumbnails-input'),
                    api = $.fileuploader.getInstance(inputEl.get(0));

                if (api.getFiles().length >= api.getOptions().limit) {
                    plusInput.hide();
                }

                plusInput.insertAfter(item.html);

                var valElm = item.html.find('input:hidden');
                if (item.appended) { // 以前上傳的檔案                    
                    valElm.val(video);

                    item.html.find('div.progress-holder').html('');

                    var uniqueId = Component.guid();
                    item.html.find('div.video-mp4').prop('id', uniqueId);
                    player(uniqueId, video);
                } else {
                    item.html.find('.tooltip').tooltipster({
                        maxWidth: 100
                    });
                }
            },
            onItemRemove: function (html, listEl, parentEl, newInputEl, inputEl) {
                var plusInput = listEl.find('.fileuploader-thumbnails-input'),
                    api = $.fileuploader.getInstance(inputEl.get(0));

                html.children().animate({ 'opacity': 0 }, 200, function () {
                    setTimeout(function () {
                        html.remove();

                        if (api.getFiles().length - 1 < api.getOptions().limit) {
                            plusInput.show();
                        }
                    }, 100);
                });
            }
        },
        afterRender: function (listEl, parentEl, newInputEl, inputEl) {
            var plusInput = listEl.find('.fileuploader-thumbnails-input'),
                api = $.fileuploader.getInstance(inputEl.get(0));

            plusInput.on('click', function () {
                api.open();
            });
        },
        upload: {
            url: uploadUrl,
            data: { type: 'video' }, // 當前未使用，目的:校驗上傳文件類型
            type: 'POST',
            enctype: 'multipart/form-data',
            start: true,
            beforeSend: function (item, listEl, parentEl, newInputEl, inputEl) {
                Form.videoUpload.uploading = true;
            },
            onSuccess: function (data, item, listEl, parentEl, newInputEl, inputEl, textStatus, jqXHR) {
                if (!data)
                    return;
                data = $.parseJSON(data);

                item.html.find('input:hidden').val(data.Name);
                item.html.find('div.progress-holder').html('');

                var uniqueId = Component.guid();
                item.html.find('div.video-mp4').prop('id', uniqueId);
                player(uniqueId, data.Name);
            },
            onComplete: function (listEl, parentEl, newInputEl, inputEl, jqXHR, textStatus) {
                Form.videoUpload.uploading = false;
            }
        }
    });

    var itemOuterElm = fileElm.nextAll('div.fileuploader-items').find('ul');
    fileElm.closest('form').submit(function () {
        var field = fileElm.attr('data-field');
        itemOuterElm.find('li').each(function (n) {
            $(this).find('input').each(function () {
                $('#' + field).val($.trim(this.value)); 
            });
        });
    });
}

Form.initField = function (outerElm, rootUrl, fileUploadUrl, uploadUrl, addressUrl) {
    console.log(outerElm);
    outerElm = $(outerElm);

    //TODO： twDate、twDateTime 民國年
    outerElm.find('input.time').datetimepicker({ format: "HH:mm" });
    outerElm.find('input.date').datetimepicker({ format: "YYYY/MM/DD" });
    outerElm.find('input.datetime').datetimepicker({ format: "YYYY/MM/DD HH:mm" });

    outerElm.find('div[data-radioGroup]').each(function () {
        Component.radioList(this);
    });

    outerElm.find('div[data-checkGroup]').each(function () {
        Component.checkList(this);
        var vals = $(this).next('input').val();
        if (vals) {
            vals = $.parseJSON(vals);
            $(this).find(':checkbox').each(function () {
                if ($.inArray(this.value, vals) != -1) {
                    $(this).click();
                }
            });
        }
    });

    $('form').submit(function () {
        outerElm.find('div[data-checkGroup]').each(function () {
            var v = [];
            $(this).find('input').each(function () {
                if (this.checked)
                    v.push(this.value);
            });
            $(this).next().val(JSON.stringify(v));
        });
    });

    outerElm.find('input.fieldRegion').each(function () {
        var fix = $(this).data('fix');
        if (!fix)
            fix = [];
        Component.worldRegion(this, addressUrl, { fix: fix });
    });

    outerElm.find('input:file').each(function () {
        var $this = $(this);
        if ($this.hasClass('fieldfile')) {
            Form.fileUpload('#' + this.id, rootUrl, fileUploadUrl, uploadUrl);
        } else if ($this.hasClass('fieldimage')) {
            Form.imageUpload('#' + this.id, rootUrl, fileUploadUrl, uploadUrl);
        } else if ($this.hasClass('fieldvideo')) {
            Form.videoUpload('#' + this.id, fileUploadUrl, uploadUrl);
        } else if ($this.hasClass('fieldvoice')) {
            Form.voiceUpload('#' + this.id, rootUrl, fileUploadUrl, uploadUrl);
        }
    });    
}

Form.checkRepeat = function (outerElm, formId, formItemId, checkRepeatUrl, formSubmit) {
    outerElm = $(outerElm);

    function checkExist(formId, fieldId, value, formItemId) {
        var dtd = $.Deferred();

        var url = checkRepeatUrl + '?formId=' + formId + '&fieldId=' + fieldId + '&value=' + encodeURIComponent(value);
        if (formItemId)
            url += '&formItemId=' + formItemId;
        $.get(url, function (count) {
            count -= 0;
            var elm = $('#' + fieldId);
            var repeatCount = elm.attr('data-repeat') - 0;
            if (count >= repeatCount) {
                var msg = repeatCount == 1 ? "* 欄位值已存在" : "* 該欄位值已存在 " + repeatCount + " 筆資料，不允許再次使用該值";
                Component.showElementError('#' + fieldId, msg, true);
                dtd.reject();
            } else
                dtd.resolve();
        });

        return dtd.promise();
    }

    var notRepeatElms = outerElm.find('input[data-repeat]');
    if (notRepeatElms.length == 0) {
        formSubmit();
    } else {
        var checkFuns = [];
        notRepeatElms.each(function () {
            checkFuns.push(checkExist(formId, this.id, $.trim(this.value), formItemId));
        });
        $.when.apply(this, checkFuns).done(function () {
            formSubmit();
        });
    }
}