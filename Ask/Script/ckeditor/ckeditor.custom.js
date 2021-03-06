$(function () {

    var config = {
        /*開啟內容允許script*/
        allowedContent: {
            script: true,
            div: true
        },
        // 內文
        toolbar: [
			['Font', 'FontSize'],
			['TextColor', 'BGColor'],
			['JustifyLeft', 'JustifyCenter', 'JustifyRight'],
			['Bold', 'Italic', 'Underline', 'Strike', 'Subscript', 'Superscript'],
			['BulletedList', 'NumberedList', 'Indent', 'Outdent'],
			['Link', 'Smiley'],
			['RemoveFormat', 'Source', 'Sourcedialog', 'Maximize']
        ],
        /*太報客製需求 START*/
        font_names: "新細明體;微軟正黑體;標楷體;",
        fontSize_sizes: "12/12px;14/14px;16/16px;18/18px;20/20px;",
        font_defaultLabel: "微軟正黑體",
        colorButton_colors: '444444;e74c3c;2980b9',
        colorButton_enableAutomatic: false,
        colorButton_enableMore: false,
        /*太報客製需求 END*/
        height: "400",
        skin: 'minimalist',
        pasteFromWordPromptCleanup: true
        //allowedContent:false
    };
    var config2 = {
        /*開啟內容允許script*/
        allowedContent: {
            script: true,
            div: true
        },
        // 標題
        toolbar: [
			['Font', 'FontSize'],
			['TextColor', 'BGColor'],
			['JustifyLeft', 'JustifyCenter', 'JustifyRight'],
			['Bold', 'Italic', 'Underline', 'Strike', 'Subscript', 'Superscript'],
			['Link', 'Smiley'],
			['RemoveFormat', 'Source', 'Sourcedialog']
        ],
        height: "80",
        skin: "minimalist",
        pasteFromWordPromptCleanup: true
        //allowedContent:false
    };
    $("textarea.editor-txt").each(function () {
        CKEDITOR.replace(this, config);
    });

    $("textarea.editor-subTitle").each(function () {
        CKEDITOR.replace(this, config2);
    });
});