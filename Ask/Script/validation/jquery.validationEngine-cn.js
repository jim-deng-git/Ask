(function($) {
    $.fn.validationEngineLanguage = function() { };
    $.validationEngineLanguage = {
        newLang: function() {
            $.validationEngineLanguage.allRules = {
                "required": {
                    "regex": "none",
                    "alertText": "* 必填欄位",
                    "alertTextCheckboxMultiple": "* 請至少選擇一項",
                    "alertTextCheckboxe": "* 必選"
                },
                "length": {
                    "regex": "none",
                    "alertText": "* 長度必須在 ",
                    "alertText2": " 至 ",
                    "alertText3": " 個字元之間"
                },
                "maxCheckbox": {
                    "regex": "none",
                    "alertText": "* 最多允許選擇 ",
                    "alertText2": " 項"
                },
                "minCheckbox": {
                    "regex": "none",
                    "alertText": "* 最少必須選擇 ",
                    "alertText2": " 項"
                },
                "equals": {
                    "regex": "none",
                    "alertText": "* 兩次輸入的密碼不相符"
                },
                "email": {
                    "regex": /^((([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+(\.([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+)*)|((\x22)((((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(([\x01-\x08\x0b\x0c\x0e-\x1f\x7f]|\x21|[\x23-\x5b]|[\x5d-\x7e]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(\\([\x01-\x09\x0b\x0c\x0d-\x7f]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]))))*(((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(\x22)))@((([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.)+(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.?$/i,
                    "alertText": "* E-Mail 格式不正確"
                },
                "onlyNumber": {
                    "regex": /^-?\d+$/,
                    "alertText": "* 必須為整數"
                },
                "number": {
                    "regex": /^\d+$/,
                    "alertText": "* 必須為零或正整數"
                },
                "positive": {
                    "regex": /^[1-9]\d*$/,
                    "alertText": "* 必須為大於零的整數"
                },
                "image": {
                    "regex": /\.(jpe?g|gif|png)$/i,
                    "alertText": "* 僅允許以下格式的圖片：JPG、PNG、GIF"
                },
                "postcode": {
                    "regex": /^\d{3,6}$/,
                    "alertText": "* 郵政編碼格式不正確"
                },
                "phone": {
                    "regex": /^0\d{1}\-\d+$/,
                    "alertText": "* 請輸入正確格式"
                },
                "celephone": {
                    "regex": /^09\d{8}$/,
                    "alertText": "* 請輸入正確格式"
                },
                "imgDes": {
                    "regex": /^.{0,200}$/,
                    "alertText": "* 長度必須在200個字元內"
                }
            }
        }
    }
})(jQuery);

$(document).ready(function() {
    $.validationEngineLanguage.newLang();
    $.validation = $.validation || {};
    $.validation.extRules = function(rules) {
        for (var r in rules) {
            $.validationEngineLanguage.allRules[r] = rules[r];
        }
    }
});