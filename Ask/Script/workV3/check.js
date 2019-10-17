function checkRadioBox(field) {
    if (field.find('input:checked').length == 0)
        return '* 必選欄位';
}

function checkCheckBox(field) {
    if (field.find('input:checked').length == 0)
        return '* 請至少選擇一項';
}

function checkChinaMobile(field) {
    var v = $.trim(field.val());
    if (v && !/^1[3578][0-9]{9}$/.test(v))
        return '* 手機號碼不正確';
}

function checkChinaIDCard(field) {
    var v = $.trim(field.val());
    if (v && !/^[1－9][1-7]\d{4}(18|19|20)\d{2}(0[1-9]|1[12])(0[1-9]|[12]\d|3[01])\d{3}(\d|X)$/i.test(v))
        return '* 身份證號碼不正確';
}

function checkTwMobile(field) {
    var v = $.trim(field.val());
    if (v && !/^09\d{8}$/.test(v))
        return '* 手機號碼不正確';
}

function checkTwUniteSN(field) {

}

function checkTwIDCard(field) {
    var v = $.trim(field.val());
    if (v) {
        var num = v.toLowerCase();
        var patten = /^[a-z][12][0-9]{8}$/;
        if (!patten.test(num)) 
            return '* 身份證字號格式不正確';

        var h = "abcdefghjklmnpqrstuvxywzio";
        var x = 10 + h.indexOf(num.substring(0, 1));
        var chksum = (x - (x % 10)) / 10 + (x % 10) * 9;
        for (var i = 1; i != 9; ++i) {
            chksum += num.substring(i, i + 1) * (9 - i);
        }
        chksum = (10 - chksum % 10) % 10;
        if (chksum != num.substring(9, 10))
            return '* 身份證字號格式不正確';        
    }
}

function checkTwPhone(field) {
    var v = $.trim(field.val());
    if (v && !/^0[2－9]\d{7,8}$/.test(v))
        return '* 電話號碼不正確';
}

function checkUrl(field) {
    var v = $.trim(field.val());
    if (v && !/^https?:\/\//.test(v))
        return '* 連結必須以 http:// 或 https:// 開頭';
}

function checkUsSSN(field) {
    var v = $.trim(field.val());
    if (v && !/^\d{3}-\d{2}-\d{4}$/.test(v))
        return '* SSN 號碼格式不正確';
}

function checkLetter(field) {
    var v = $.trim(field.val());
    if (v && !/^[a-zA-Z]+$/.test(v))
        return '* 僅允許輸入英文字符';
}

function checkLetterOrNum(field) {
    var v = $.trim(field.val());
    if (v && !/^[a-zA-Z\d]+$/.test(v))
        return '* 僅允許輸入英文字符或數字';
}

function checkTextLength(field) {
    //var v = $.trim(field.val());                   //原本
    var v = $.trim(field.val().replace(/(\n)/g, ''));//Joe 20190912 查找所有換行，並替換成""
    if (v) {
        var min = field.data('min') - 0;
        if (min && v.length < min)
            return '* 不能少於 ' + min + ' 個字元';
        var max = field.data('max') - 0;
        if (max && v.length > max)
            return '* 不能多於 ' + max + ' 個字元';
    }
}

function checkTime(field) {
    var v = $.trim(field.val());
    if (v && !/^(\d|[01]\d|2[0-3]):[0-5]\d$/.test(v))
        return '* 時間格式不正確';
}

function checkTwDate(field) {
    var v = $.trim(field.val());
    if (v) {
        if (v.length > 9)
            return '* 民國日期格式不正確';

        v = v.replace(/^\d+/, function (year) {
            return year - 0 + 1911;
        });
        var date = Component.parseDate(v);
        //if(!v)                          //原本
        if (date==null)                   //20190912 Joe 改判斷date是否為null
            return '* 民國日期格式不正確';
    }
}

function checkTwDateTime(field) {
    var v = $.trim(field.val());
    if (v) {
        v = v.replace(/^\d+/, function (year) {
            return year - 0 + 1911;
        });
        var date = Component.parseDate(v);
        if (date == null)
            return '* 民國日期格式不正確';
    }
}

function checkDate(field) {
    var v = $.trim(field.val());
    if (v) {
        if (v.length > 10)
            return '* 西元日期格式不正確';
                
        var date = Component.parseDate(v);
        if (date == null)
            return '* 西元日期格式不正確';
    }
}

function checkDateTime(field) {
    var v = $.trim(field.val());
    if (v) {        
        var date = Component.parseDate(v);
        if (date == null)
            return '* 西元日期格式不正確';
    }
}

function checkFile(field) {
    if (field.find('li.fileuploader-item').length == 0)
        return '* 請至少上傳一個檔案';
}

function checkAddress(field) {
    var pass = true;
    field.find('select,:text').each(function () {
        var v = $.trim($(this).val());
        if (!v) {
            pass = false;
            return false;
        }
    });

    if (!pass)
        return '* 請將欄位填寫完整';
}

function checkSelect(field) {
    var v = field.find('select').val();
    if (!v)
        return '* 必選欄位';
}

function checkNum(field) {
    var v = $.trim(field.val());
    if (v && !/^\d+$/.test(v))
        return '* 僅允許輸入數字';
}