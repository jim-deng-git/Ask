
var CKEditorHelp = {};

CKEditorHelp.init = function (divId, minimize, pasteAsHTML, options, setFontSize) {
    var elm = document.getElementById(divId);    
    if (!elm)
        return;

  
    options = options || {};
    options.skin = 'minimalist';
    if (minimize) {
        //標題用options
        options.toolbar = [
			    ['Font', 'FontSize', 'Styles'],
			    ['TextColor', 'BGColor'],
			    ['JustifyLeft', 'JustifyCenter', 'JustifyRight'],
			    ['Bold', 'Italic', 'Underline', 'Strike', 'Subscript', 'Superscript'],
                //['BulletedList', 'NumberedList', 'Indent', 'Outdent'],
                ['Link', 'Smiley'],
                ['RemoveFormat', 'Source', 'Sourcedialog']

        ];
        options.height = '60px';
        options.font_defaultLabel = 'Microsoft JhengHei'; //carrie 201802262254
       
    } else {
        //內文用
        options.toolbar = [
			    ['Font', 'FontSize', 'Styles'],
			    ['TextColor', 'BGColor'],
                ['styles', 'styles'],
			    ['JustifyLeft', 'JustifyCenter', 'JustifyRight'],
			    ['Bold', 'Italic', 'Underline', 'Strike', 'Subscript', 'Superscript'],
                ['BulletedList', 'NumberedList', 'Indent', 'Outdent'],
			    ['Link', 'Smiley'],
			    ['RemoveFormat', 'Source', 'Sourcedialog', 'Maximize']
        ];
        options.font_defaultLabel = 'Microsoft JhengHei'; //carrie 201802262254
    }
 
    CKEDITOR.config.forcePasteAsPlainText = !pasteAsHTML;
    CKEDITOR.disableAutoInline = true;
    CKEDITOR.linkShowTargetTab = true;
    CKEDITOR.linkShowAdvancedTab = true;
    CKEDITOR.config.font_defaultLabel = 'Microsoft JhengHei'; //carrie 201802262254

    //設定font 預設數值
    if(!isNaN(setFontSize)){
        options.fontSize_defaultLabel = setFontSize;
    }

    var ck = CKEDITOR.inline(elm, options);

    //CKEDITOR.replace(document.getElementById(divId), options);
    $('form').submit(function () {
        var hidElm = $('<input type="hidden" name="' + divId + '" />');
        hidElm.val(CKEditorHelp.getHtml(divId));
        $('form').append(hidElm);
    });

    // 避免ckeditor 父層隱藏contenteditable變false
    ck.on('instanceReady', function (ev) {
        var editor = ev.editor;
        editor.setReadOnly(false);
    });
}

CKEditorHelp.getHtml = function (divId) {
    return eval('CKEDITOR.instances.' + divId + '.getData()');
}

CKEditorHelp.setHtml = function (divId, html) {
    var editor = eval('CKEDITOR.instances.' + divId);
    editor.setData(html);
}
