/**
 * @license Copyright (c) 2003-2017, CKSource - Frederico Knabben. All rights reserved.
 * For licensing, see LICENSE.md or http://ckeditor.com/license
 */

CKEDITOR.editorConfig = function( config ) {
	// Define changes to default configuration here. For example:
	// config.language = 'fr';
    // config.uiColor = '#AADC6E';
    /*太報客製需求 START*/
    config.font_names = "新細明體;微軟正黑體;標楷體;";
    config.fontSize_sizes = "12/12px;14/14px;16/16px;18/18px;20/20px;40/40px";
    config.font_defaultLabel = "微軟正黑體";
    config.colorButton_colors = '444444,e74c3c,2980b9';
    config.colorButton_enableAutomatic = false;
    config.colorButton_enableMore = false;
    //提示是否解除清除word 格式
    config.pasteFromWordPromptCleanup = true;
    //config.pasteFromWordRemoveFontStyles = false;
    //config.pasteFromWordRemoveStyles = false;
    //config.allowedContent = false;
    /*太報客製需求 END*/
};

CKEDITOR.config.allowedContent = true;