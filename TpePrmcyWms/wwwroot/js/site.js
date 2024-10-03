
//系統基本工能

//#region view-後台用,排序跟分頁送出的query submit
function goListQuery(QueryCondition) {
    let qc = QueryCondition.split('=');
    if (qc[0] == "sortOrder" && $("#sortOrder").val() == qc[1]) { qc[1] += "_desc"; }
    $("#" + qc[0]).val(qc[1]);
    $("#btn_submit").click();
}
//#endregion
//#region 共用post json for API
class AjaxJsonPostor {
    constructor(url, DataObj) {
        this.url = url;
        this.DataObj = DataObj;
    };
    Method = "POST";
    SuccessCallback = null;
    ErrorCallback = null;
    CompleteCallback =null;

    Post() {
        //console.log(JSON.stringify(this.DataObj));
        $.ajax({
            url: this.url,
            data: JSON.stringify(this.DataObj),
            type: this.Method,
            dataType: "json",
            contentType: "application/json;charset=utf-8",
            success: this.SuccessCallback,
            error: this.ErrorCallback,
            complete: this.CompleteCallback
            
        });
    }
}
//#endregion
//#region 共用 編輯頁submit送出Object 
function SubmitEditModel(url, EditModel, successCall) {    
    for (property in EditModel) {
        var propertyName = property.toString();
        let el = $("#" + propertyName);
        if ($(el).is("input")) {
            if ($("input#" + propertyName).val()) {
                let inputelm = $("input#" + propertyName);
                EditModel[property] = inputelm.val();
                if (inputelm.attr('type') == "checkbox") {
                    EditModel[property] = inputelm.is(":checked");
                }
            }
        }
        if ($(el).is("select")) {
            if ($("select#" + propertyName).val()) {
                EditModel[property] = $("select#" + propertyName).val();
            }
            else { EditModel[property] = 0; }
        }
        if ($(el).is("textarea")) {
            if ($("textarea#" + propertyName).val()) {
                EditModel[property] = $("textarea#" + propertyName).val();
            }
        }
    }
    console.log(JSON.stringify(EditModel));
    $.post({
        url: url,
        data: EditModel,
        success: function (result) {
            $(".text-danger").text("");
            if (result.code == "invalid") {
                //console.log(JSON.stringify(result.returnData));
                let messages = result.returnData ? result.returnData : result.message;
                for (i = 0; i < result.returnData.length; i++) {
                    let errorelement = $(".text-danger[data-valmsg-for='" + messages[i].name + "']");
                    errorelement.append(messages[i].errorMsg);
                    errorelement.removeClass("field-validation-valid").addClass("field-validation-error");
                }
            }
            else { AlertWin(result.returnData); }
            if (result.code == 0) {
                if (successCall) { successCall(); }
            }
        },
        error: function () {
            AlertWin("送出失敗");
        }
    });
}
//#endregion
//#region 共用 編輯頁submit送出Form
function SubmitFormModel(url, EditModel, successCall, failedCall) {    
    var formData = new FormData();
    //附加檔案
    var files = $('input[type=file]');
    for (var i = 0; i < files.length; i++) {
        if (!(files[i].value == "" || files[i].value == null)) { formData.append(files[i].name, files[i].files[0]); }
    }
    //附加view上有的輸入欄
    var formSerializeArray = $("form").serializeArray();
    for (var i = 0; i < formSerializeArray.length; i++) {
        formData.append(formSerializeArray[i].name, formSerializeArray[i].value);
        //console.log("form: " + formSerializeArray[i].name + " = " + formSerializeArray[i].value);
    }
    //附加view上沒有的值
    for (property in EditModel) {
        var propertyName = property.toString();
        if (!formData.get(propertyName)) {
            formData.append(propertyName, (EditModel[property] ? EditModel[property] : ""));
            //console.log(propertyName + " = " + formData.get(propertyName));
        }
    }
    
    $.post({
        url: url,
        data: formData,
        contentType: false,
        processData: false,
        cache: false,
        success: function (result) {
            console.log(JSON.stringify(result)); 
            $(".text-danger").text("");
            if (result.code == "invalid") {                
                let messages = result.returnData ? result.returnData : result.message;
                for (i = 0; i < messages.length; i++) {
                    let errorelement = $(".text-danger[data-valmsg-for='" + messages[i].name + "']");
                    errorelement.append(messages[i].errorMsg);
                    errorelement.removeClass("field-validation-valid").addClass("field-validation-error");
                }
                if (failedCall) { failedCall(); }
            }
            
            if (result.code == 0 || result.code == "0") {                
                if (successCall) { successCall(); }
                AlertWin(result.message);
            }
            else if (result.code != "invalid") {
                let messages = result.returnData ? result.returnData : result.message;
                AlertWin(messages);
                if (failedCall) { failedCall(); }
            }
        },
        error: function () {
            AlertWin("送出失敗");
            if (failedCall) { failedCall(); }
        }
    });
}
//#endregion
//#region share-左側系統目錄操作
function LeftMenuClick(Lv1Fid) {
    $(".MenuLeftLv2").each(function () {
        if ($(this).attr("MenuL2Up") == Lv1Fid) { $(this).show("slow") }
        else { $(this).hide("slow"); }
    });
}
//#endregion
//#region 共用 清單頁排序加css
function sortingAddClass(sortkey) {
    $(".btn_SortOrderCtrl").each(function () {
        $(this).removeClass("SortMe").removeClass("SortMeDesc");
        let sortname = sortkey.toLowerCase().indexOf("_desc") > 0 ? sortkey.slice(0, -5) : sortkey;
        let objcallparam = $(this).attr("onclick");
        objcallparam = objcallparam.substring(objcallparam.indexOf("sortOrder=") + 10).replace("')", "");
        if (objcallparam.toLowerCase() == sortname.toLowerCase()) {
            if (sortname.length == sortkey.length) { $(this).addClass("SortMe"); }
            else { $(this).addClass("SortMeDesc"); }
        }
    })
}
//#endregion

//#region 共用post時取token取 (暫無用)
function GetToken() {
    let form = $('#__AjaxAntiForgeryForm');
    let token = $('input[name="__RequestVerificationToken"]', form).val();
    return token;
}
//#endregion


//前台



//#region view-限制輸入的文字(後台應該都用class控制了)
function InputOnlyNumber(v) {
    return v.replace(/[^\d.-]+/g, '');
}
function InputEnNum(v) {
    return v.replace(/[^a-zA-Z\d]+/g, '');
}
//#endregion

//#region view-日期7字民國年轉換
function EraStringToDate(v, symbol) {
    return (parseInt(v.substring(0, 3)) + 1911).toString() + symbol + v.substring(3, 5) + symbol + v.substring(5, 7);
}
function DateToEraString(v, symbol) {
    return (parseInt(v.split(symbol)[0]) - 1911).toString() + v.split(symbol)[1] + v.split(symbol)[2];
}
//#endregion

//#region view-判斷日期格式
function isValidDate(v) {
    var date = new Date(v);
    return date instanceof Date && !isNaN(date.getTime())
}
//#endregion

//#region view-日期相減
function DiffTime_hms(bigDate, smallDate) {
    var diff = bigDate.getTime() - smallDate.getTime();
    var hours = Math.floor(diff / 1000 / 60 / 60);
    diff -= hours * 1000 * 60 * 60;
    var minutes = Math.floor(diff / 1000 / 60);
    diff -= minutes * 1000 * 60;
    var seconds = Math.floor(diff / 1000);
    return hours.toString().padStart(2, '0') + ":" + minutes.toString().padStart(2, '0') + ":" + seconds.toString().padStart(2, '0');
};
//#endregion

//#region view-領藥號碼 病歷號碼 的規則
var PrscptNoReg = /[A-Z]\d{4}/;
var PatientNoReg = /\d{8}/;

//#endregion

//特色功能
//#region 輸入錯誤的input,閃紅框 搭配css
function blinkfocus(selectorname) {
    let timesRun = 0;
    $(selectorname).focus();
    let timer_blinkalert = setInterval(function () {
        if (timesRun > 6) {
            clearInterval(timer_blinkalert);
        }
        $(selectorname).toggleClass("InputAlertFocus");
        timesRun++;
    }, 300);
}
function blinking(selectorname) {
    let timesRun = 0;
    let timer_blinkalert = setInterval(function () {
        if (timesRun > 6) {
            clearInterval(timer_blinkalert);
        }
        $(selectorname).toggleClass("InputAlertFocus");
        timesRun++;
    }, 300);
}
//#endregion




