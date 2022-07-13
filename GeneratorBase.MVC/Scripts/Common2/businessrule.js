function AssignHiddenValues(dataurl, entityname, form, cannotview) {
    var result = form;
    dataurl = addParameterToURL(dataurl, "cannotview", cannotview);
    dataurl = dataurl.replace("businessruletype", "AssignHiddenValues")
    $.ajax({
        async: false,
        type: "POST",
        url: dataurl,
        data: form,
        success: function (data) {
            for (var key in data) {
                result += "&" + key + "=" + encodeURIComponent(data[key]);
            }

        },
        error: function (jqXHR, textStatus, errorThrown) {

        }
    });
    return result;
}
function ApplyBusinessRuleOnSubmit(dataurl, entityname, isinline, lblerrormsg, form, type, assocname, assocdispname) {
    
    var flagvalidate = true;
    var flagmandatory = true;
    var array = type.split(',');
    if (array.indexOf("10") >= 0) {
        flagvalidate = ValidateBeforeSavePropertiesRule(dataurl.replace("businessruletype", "GetValidateBeforeSaveProperties"), entityname, isinline, lblerrormsg, form, assocname, assocdispname);
    }
    if (array.indexOf("15") >= 0 && flagvalidate) {
        flagvalidate = ValidateBeforeSavePropertiesRule(dataurl.replace("businessruletype", "GetValidateBeforeSavePropertiesForPopupConfirm"), entityname, isinline, lblerrormsg, form, assocname, assocdispname);
    }
    flagmandatory = MandatoryPropertiesRule(dataurl.replace("businessruletype", "GetMandatoryProperties"), entityname, isinline, lblerrormsg, form, assocname, assocdispname);
    return flagmandatory && flagvalidate;
}

function ApplyBusinessRuleOnCreate(type, dataurl, entityname, isinline, lblerrormsg, obj, assocname, assocdispname) {
    
    form = obj.serialize();
    var array = type.split(',');
    if (array.indexOf("13") >= 0)
        GetUIAlertBusinessRules(dataurl.replace("businessruletype", "GetUIAlertBusinessRules"), entityname, isinline, lblerrormsg, form, assocname, assocdispname);
    if (array.indexOf("4") >= 0) {
        ReadOnlyPropertiesRule(dataurl.replace("businessruletype", "GetReadOnlyProperties"), entityname, isinline, lblerrormsg, form, assocname, assocdispname);
    }
}

function ApplyBusinessRuleOnPageLoad(type, dataurl, entityname, isinline, lblerrormsg, obj, assocname, assocdispname) {
    
    form = obj.serialize();
    var array = type.split(',');
    if (array.indexOf("1") >= 0 || array.indexOf("11") >= 0)
        LockBusinessRules(dataurl.replace("businessruletype", "GetLockBusinessRules"), entityname, isinline, lblerrormsg, form, assocname, assocdispname);
    if (array.indexOf("4") >= 0)
        ReadOnlyPropertiesRule(dataurl.replace("businessruletype", "GetReadOnlyProperties"), entityname, isinline, lblerrormsg, form, assocname, assocdispname);
    if (array.indexOf("13") >= 0)
        GetUIAlertBusinessRules(dataurl.replace("businessruletype", "GetUIAlertBusinessRules"), entityname, isinline, lblerrormsg, form, assocname, assocdispname);
    if (array.indexOf("16") >= 0)
        GetHiddenVerb(dataurl.replace("businessruletype", "GetHiddenVerb"), entityname, isinline, lblerrormsg, form, assocname, assocdispname);
    //  
}
function ApplyBusinessRuleOnPageLoadInline(type, dataurl, entityname, isinline, lblerrormsg, obj, assocname, assocdispname) {
    form = obj.serialize();
    var array = type.split(',');
    if (array.indexOf("1") >= 0 || array.indexOf("11") >= 0)
        LockBusinessRules(dataurl.replace("businessruletype", "GetLockBusinessRules"), entityname, isinline, lblerrormsg, form, assocname, assocdispname);
    if (array.indexOf("4") >= 0)
        ReadOnlyPropertiesRule(dataurl.replace("businessruletype", "GetReadOnlyProperties"), entityname, isinline, lblerrormsg, form, assocname, assocdispname);
    if (array.indexOf("13") >= 0)
        GetUIAlertBusinessRules(dataurl.replace("businessruletype", "GetUIAlertBusinessRules"), entityname, isinline, lblerrormsg, form, assocname, assocdispname);
    if (array.indexOf("16") >= 0)
        GetHiddenVerb(dataurl.replace("businessruletype", "GetHiddenVerb"), entityname, isinline, lblerrormsg, form, assocname, assocdispname);
    //  
}
function GetHiddenVerb(dataurl, entityname, isinline, lblerrormsg, form, assocname, assocdispname) {
    $.ajax({
        type: "POST",
        url: dataurl,
        data: form,
        success: function (data) {
            if (data.dict != undefined)
                for (var key in data.dict) {

                    $("#vrb" + data.dict[key]).remove();
                    $("#vrb1" + data.dict[key]).remove();
                }
        },
        error: function (jqXHR, textStatus, errorThrown) {
        }
    });
}
function MandatoryPropertiesRule(dataurl, entityname, isinline, lblerrormsg, form, assocname, assocdispname) {
    document.getElementById(lblerrormsg).value = "";
    $("#divDisplayBRmsgMandatory1").empty();
    var flag = true;
    var failuremsg = "";
    var infomsg = "";
    $.ajax({
        async: false,
        type: "POST",
        url: dataurl,
        data: form,
        success: function (data) {
            $('[businessrule="mandatory"]').each(function () {
                $(this).removeAttr('required');
            });

            for (var key in data.dict) {
                var propKey = key;
                if (isinline)
                    propKey = assocname.toLowerCase() + "_" + key;

                if (!(key.toLowerCase().indexOf("informationmessage") >= 0)) {
                    if ($('#' + propKey).length>0 || $("input[type='radio'][name='" + propKey + "']").length > 0)
                        if (($('#' + propKey).is(':checkbox') && $('#' + propKey).prop("checked") == false)
                            || 
                            ($("input[type='radio'][name='" + propKey + "']").length > 0 && ($.trim($("input[type='radio'][name='" + propKey + "']:checked").val()).length == 0 || $("input[type='radio'][name='" + propKey + "']:checked").val() == '0'))
                            || ($('#' + propKey).length > 0 && $.trim($('#' + propKey).val()).length == 0)
                            ) {
                        if (key.toLowerCase().indexOf("failuremessage") >= 0) {
                        }
                        else {
                            if (isinline)
                                failuremsg += assocdispname + "." + data.dict[key] + ", ";
                            else
                                failuremsg += data.dict[key] + ", ";
                            $('#' + propKey).attr('required', 'required');
                            $('#' + propKey).attr('businessrule', 'mandatory');
                            if ($("input[type='radio'][name='" + propKey + "']").length > 0) {
                                $("span[data-valmsg-for=" + propKey + "]").attr('class', 'field-validation-error').html('The field is required');
                                $("#divDisplayBRmsgBeforeSaveProp").html(propKey + ' is required');
                            }
                            flag = false;
                        }
                    }
                }
                else {
                    if (data.dict[key] != null)
                        infomsg += data.dict[key]  + " | ";
                }
            }
            if (infomsg != "")
                if (flag)
                    alert(infomsg.replace(infomsg.substring(infomsg.lastIndexOf(" | ")), ""));
            if (failuremsg != "")
                if (!flag) {
                    //// failuremsg = failuremsg.replace(failuremsg.substring(failuremsg.lastIndexOf(",")), "")
                   // failuremsg = failuremsg.replace(/,\s*$/, "");
                    if ($("#addPopup").is(':visible')) {

                        
                        document.getElementById(lblerrormsg).value += failuremsg;
                        var message = data.template.replace('###Message###', failuremsg.replace(/,\s*$/, ""))
                        $("#divDisplayBRmsgMandatory", "#addPopup").removeAttr("style");
                        $("#divDisplayBRmsgMandatory", "#addPopup").html(message);
                        $("#divDisplayBRmsgMandatory1", "#addPopup").html(message);

                      //  $("#divDisplayBRmsgMandatory", "#addPopup").html(getMsgTableMandatory());
                      //  document.getElementById("ErrMsgMandatory", "#addPopup").innerHTML = document.getElementById(lblerrormsg).value;
                     //   $("#divDisplayBRmsgMandatory1", "#addPopup").html($("#divDisplayBRmsgMandatory", "#addPopup").html());
                    }
                    else {
                        document.getElementById(lblerrormsg).value += failuremsg;
                        var message = data.template.replace('###Message###', failuremsg.replace(/,\s*$/, ""))
                        $("#divDisplayBRmsgMandatory").removeAttr("style");
                     //   $("#divDisplayBRmsgMandatory").html(getMsgTableMandatory());
                       // document.getElementById("ErrMsgMandatory").innerHTML = document.getElementById(lblerrormsg).value;
                        //$("#divDisplayBRmsgMandatory1").html($("#divDisplayBRmsgMandatory").html());
                        $("#divDisplayBRmsgMandatory").html(message);
                        $("#divDisplayBRmsgMandatory1").html(message);
                    }
                }
        },
        error: function (jqXHR, textStatus, errorThrown) {
        }
    });
    return flag;
}


function GetUIAlertBusinessRules(dataurl, entityname, isinline, lblerrormsg, form, assocname, assocdispname) {
    
    document.getElementById(lblerrormsg).value = "";
    $("#divDisplayBRmsgBeforeSaveProp1").empty();
    var flag = true;
    var failuremsg = "";
    var infomsg = "";
    $.ajax({

        type: "POST",
        url: dataurl,
        data: form,
        success: function (data) {
            for (var key in data.dict) {
                if (!(key.toLowerCase().indexOf("informationmessage") >= 0)) {
                    flag = false;;
                    if (key.toLowerCase().indexOf("failuremessage") >= 0) {
                        if (data.dict[key] != null)
                            failuremsg += data.dict[key] + ", ";
                    }
                    else {

                        if (!isinline) {
                            
                            failuremsg += data.dict[key] + " ";
                        }
                        else
                            failuremsg += assocdispname + "." + data.dict[key] + " ";
                    }
                }
                else {
                    if (data.dict[key] != null)
                        infomsg += data.dict[key] + " | ";
                }

            }
            if (infomsg != "")
                if (flag)
                    alert(infomsg.replace(infomsg.substring(infomsg.lastIndexOf(" | ")), ""));
            if (failuremsg != "")
                if (!flag) {
                  
                        if ($("#addPopup").is(':visible')) {
                         //   document.getElementById(lblerrormsg).value += failuremsg;
                            var message = data.template.replace('###Message###', failuremsg.replace(/,\s*$/, ""))
                        $("#divDisplayBRmsgBeforeSaveProp","#addPopup").removeAttr("style");
                     //   $("#divDisplayBRmsgBeforeSaveProp", "#addPopup").html(getMsgTableUIAlert());
                      //  document.getElementById("ErrMsgRuleBeforeSaveProp", "#addPopup").innerHTML = document.getElementById(lblerrormsg).value;

                            $("#divDisplayBRmsgBeforeSaveProp1", "#addPopup").html(message);
                                $("#divDisplayBRmsgBeforeSaveProp", "#addPopup").html(message);
                    }
                    else
                        {
                            var message = data.template.replace('###Message###', failuremsg.replace(/,\s*$/, ""))
                    //    document.getElementById(lblerrormsg).value += failuremsg;
                        $("#divDisplayBRmsgBeforeSaveProp").removeAttr("style");
                      //  $("#divDisplayBRmsgBeforeSaveProp").html(getMsgTableUIAlert());
                    //    document.getElementById("ErrMsgRuleBeforeSaveProp").innerHTML = document.getElementById(lblerrormsg).value;

                            $("#divDisplayBRmsgBeforeSaveProp1").html(message);
                                $("#divDisplayBRmsgBeforeSaveProp").html(message);
                    }
                }
        },
        error: function (jqXHR, textStatus, errorThrown) {
        }
    });
    return flag;
}

function ValidateBeforeSavePropertiesRule(dataurl, entityname, isinline, lblerrormsg, form, assocname, assocdispname) {
    document.getElementById(lblerrormsg).value = "";
    $("#divDisplayBRmsgBeforeSaveProp1").empty();
    var flag = true;
    var IsConfirm = false;
    var failuremsg = "";
    var infomsg = "";
    fd = form;
    if ($("#addPopup").is(':visible')) {
        if ($("#frmQEdit" + entityname).length > 0) {
            var disabled = $("#frmQEdit" + entityname).find(":input:disabled").removeAttr("disabled");
            fd = $("#frmQEdit" + entityname).serialize();
            disabled.attr("disabled", "disabled");
        }
    }
    else
    {
        if ($("#frm" + entityname).length > 0) {
            var disabled = $("#frm" + entityname).find(":input:disabled").removeAttr("disabled");
            fd = $("#frm" + entityname).serialize();
            disabled.attr("disabled", "disabled");
        }
    }
    $.ajax({
        async: false,
        type: "POST",
        url: dataurl,
        data: fd,
        success: function (data) {
            for (var key in data.dict) {
                
                if (!(key.toLowerCase().indexOf("informationmessage") >= 0)) {
                    flag = false;
                    if (key.toLowerCase().includes("type15")) {
                        IsConfirm = true;
                        if (key.toLowerCase().indexOf("failuremessage") >= 0) {
                            if (data.dict[key] != null)
                                failuremsg += "<br/>" + data.dict[key] + "<br/>";
                        }
                        else {
                            if (!isinline)
                                failuremsg += data.dict[key] + ",";
                            else
                                failuremsg += data.dict[key] + ",";
                        }
                    }
                    else {
                        if (key.toLowerCase().indexOf("failuremessage") >= 0) {
                            if (data.dict[key] != null)
                                failuremsg += "<br/>" + data.dict[key] + ", ";
                        }
                        else {

                            if (!isinline)
                                failuremsg +=  data.dict[key] + ", ";
                            else
                                failuremsg += assocdispname + "." + data.dict[key] + ", ";
                        }
                    }
                }
                else {
                    if (data.dict[key] != null)
                        infomsg += data.dict[key] + " | ";
                }

            }
            if (infomsg != "")
                if (flag)
                    alert(infomsg.replace(infomsg.substring(infomsg.lastIndexOf(" | ")), ""));
            if (failuremsg != "")
                if (!flag) {
                    //failuremsg = failuremsg.replace(failuremsg.substring(failuremsg.lastIndexOf(",")), "");
                    failuremsg = failuremsg.replace(/,\s*$/, "");
					if ($("#addPopup").is(':visible'))
                    {
                       // document.getElementById(lblerrormsg).value += failuremsg;
                        var message = data.template.replace('###Message###', failuremsg.replace(/,\s*$/, ""));
                        $("#divDisplayBRmsgBeforeSaveProp", "#addPopup").removeAttr("style");
                        $("#divDisplayBRmsgBeforeSaveProp1", "#addPopup").removeAttr("style");

                      //  $("#divDisplayBRmsgBeforeSaveProp", "#addPopup").html(getMsgTableBeforeSaveProp());
                       // $("#ErrMsgRuleBeforeSaveProp", "#addPopup").html(document.getElementById(lblerrormsg).value.replace(/,\s*$/, ""));
                        $("#divDisplayBRmsgBeforeSaveProp1", "#addPopup").html(message);
                        $("#divDisplayBRmsgBeforeSaveProp", "#addPopup").html(message);
                        if (IsConfirm) {
                            if (confirm(failuremsg.replace(/,\s*$/, ""))) {
                                flag = true;
                                //document.getElementById(lblerrormsg).value = "";
                               // document.getElementById("ErrMsgRuleBeforeSaveProp").innerHTML = "";
                                $("#divDisplayBRmsgBeforeSaveProp", "#addPopup").empty();
                                $("#divDisplayBRmsgBeforeSaveProp1", "#addPopup").empty();

                            } else {
                                $("#divDisplayBRmsgBeforeSaveProp", "#addPopup").hide()
                                $("#divDisplayBRmsgBeforeSaveProp1", "#addPopup").hide()
                            }
                        }
                    }
                    else 
					{
					
                   // document.getElementById(lblerrormsg).value += failuremsg;
                        var message = data.template.replace('###Message###', failuremsg.replace(/,\s*$/, ""))
                    $("#divDisplayBRmsgBeforeSaveProp").removeAttr("style");
                    $("#divDisplayBRmsgBeforeSaveProp1").removeAttr("style");
                 //   $("#divDisplayBRmsgBeforeSaveProp").html(getMsgTableBeforeSaveProp());
                        //  document.getElementById("ErrMsgRuleBeforeSaveProp").innerHTML = document.getElementById(lblerrormsg).value.replace(/,\s*$/, "");
                        $("#divDisplayBRmsgBeforeSaveProp1").html(message);
                        $("#divDisplayBRmsgBeforeSaveProp").html(message);
                    if (IsConfirm) {
                        if (confirm(failuremsg.replace(/,\s*$/, ""))) {
                            flag = true;
                           // document.getElementById(lblerrormsg).value = "";
                          //  document.getElementById("ErrMsgRuleBeforeSaveProp").innerHTML = "";
                            $("#divDisplayBRmsgBeforeSaveProp").empty();
                            $("#divDisplayBRmsgBeforeSaveProp1").empty();

                        } else {
                            $("#divDisplayBRmsgBeforeSaveProp").hide()
                            $("#divDisplayBRmsgBeforeSaveProp1").hide()
                        }
                    }
					
		}
                }
        },
        error: function (jqXHR, textStatus, errorThrown) {
        }
    });
    return flag;
}

function LockBusinessRules(dataurl, entityname, isinline, lblerrormsg, form, assocname, assocdispname) {
    
    document.getElementById(lblerrormsg).value = "";
    $("#divDisplayLockRecord1").empty();
    var flag = true;
    var failuremsg = "";
    var infomsg = "";
    $.ajax({

        type: "POST",
        url: dataurl,
        data: form,
        success: function (data) {
            for (var key in data.dict) {
                if (!(key.toLowerCase().indexOf("informationmessage") >= 0)) {
                    if (key.toLowerCase().indexOf("style") >= 0 || ($.trim($('#' + key).val()).length == 0 && $.trim($("input[type='radio'][name='" + key + "']:checked").val()).length == 0)) {
                        flag = false;
                        if (key.toLowerCase().indexOf("failuremessage") >= 0) {
                            if (data.dict[key] != null)
                                failuremsg += "<br/>" + data.dict[key] + "<br/>";
                        }
                        else
                            failuremsg += data.dict[key] + ",";
                    }
                }
                else {
                    if (data.dict[key] != null)
                        infomsg += data.dict[key] + " | ";
                }

            }
            if (!flag) {
                if (!isinline)
                    $(':input:not([readonly])', 'form').attr('disabled', 'disabled').attr('readonly', 'readonly').trigger("chosen:updated");
                else
                    $('#dv' + assocname + 'ID :input:not([readonly])', 'form').attr('disabled', 'disabled').attr('readonly', 'readonly').trigger("chosen:updated");
				
                //dvT_CurrentEmployeeJobAssignmentID
            }
            if (infomsg != "")
                if (flag)
                    alert(infomsg.replace(infomsg.substring(infomsg.lastIndexOf(" | ")), ""));
            if (failuremsg != "")
                if (!flag) {

                  var message =   data.template.replace('###Message###', failuremsg.replace(/,\s*$/, ""))
                    $("#divDisplayLockRecord").removeAttr("style");
                    $("#divDisplayLockRecord").html(message);
                 $("#divDisplayLockRecord1").html(message);
                }
            if ($("#CancelQuickAdd").length > 0)
                $("#CancelQuickAdd").prop("disabled", "");
        },
        error: function (jqXHR, textStatus, errorThrown) {
        }
    });
}
function ResetToDefault(dataurl, groupname) {
    $.ajax({
        type: "POST",
        url: dataurl,
        success: function (data) {
            dvgroup = $('#dvGroup' + groupname);
            for (var key in data) {
                if ($("#" + key).attr('setvalue') == undefined || $("#" + key).attr('setvalue') != 'true') {
                    setValueInControl($("#" + key), data[key]);
                }
            }
        }
    })

}
function ResetToDefaultField(dataurl, entityname) {
    $.ajax({
        type: "POST",
        url: dataurl,
        success: function (data) {
            dvgroup = $('#' + entityname);
            for (var key in data) {
                if ($("#" + key).attr('setvalue') == undefined || $("#" + key).attr('setvalue') != 'true') {
                    setValueInControl($("#" + key), data[key]);
                }
            }
        }
    })

}
function ReadOnlyPropertiesRule(dataurl, entityname, isinline, lblerrormsg, form, assocname, assocdispname) {
    
    document.getElementById(lblerrormsg).value = "";
    $("#divDisplayBRReadOnly1").empty();
    var flag = true;
    var failuremsg = "";
    var infomsg = "";
    $.ajax({

        type: "POST",
        url: dataurl,
        data: form,
        success: function (data) {

            for (var key in data) {
                if (!(key.toLowerCase().indexOf("informationmessage") >= 0)) {
                    flag = false;
                    if (isinline)
                        key = assocname.toLowerCase() + "_" + key;
                    $('#' + key).attr('disabled', 'disabled').attr('readonly', 'readonly').trigger("chosen:updated");
                    $("#dv" + key + " :input").attr("disabled", "disabled").attr('readonly', 'readonly').trigger("chosen:updated");
                    $("#dv" + key + " a.btnupload").each(function (index) { $(this).remove() })
                    $("input[type='radio'][name='" + key + "']").attr('disabled', 'disabled').attr('readonly', 'readonly');
                    
					 if ($('#' + key).next('span.input-group-addon.btn-default.calendar').length > 0)
                        $('#' + key).next('span.input-group-addon.btn-default.calendar').hide();
					
					//if ($('#' + key).prev('span.input-group-addon.btn-default.calendar').length == 0)
                    //    $('form').append('<input type="hidden" name="' + key + '" id="' + key + '" value="' + $('#' + key).val() + '" />');
                    //if (key.toLowerCase().indexOf("failuremessage") >= 0) {
                    //    if (data[key] != null)
                    //        failuremsg += "<br/>" + data[key] + "<br/>";
                    //}
                    //else
                    //    failuremsg += data[key] + ",";
                }
                else {
                    if (data.dict != undefined && data.dict[key]  != null)
                        infomsg += data.dict[key]  + " | ";
                }

            }
            if (infomsg != "")
                if (flag) {
                    alert(infomsg.replace(infomsg.substring(infomsg.lastIndexOf(" | ")), ""));
                }
            if (failuremsg != "")
                if (!flag) {
                    
                    //document.getElementById(lblerrormsg).value += failuremsg;
                    $("#divDisplayBRReadOnly").removeAttr("style");
                    $("#divDisplayBRReadOnly").html(getMsgTableReadOnly());
                    document.getElementById("ErrmsgtrRuleReadOnlyProp").innerHTML = document.getElementById(lblerrormsg).value;
                    $("#divDisplayBRReadOnly1").html($("#divDisplayBRReadOnly").html());
                }

        },
        error: function (jqXHR, textStatus, errorThrown) {
        }
    });
}

function Check1MThresholdLimit(e, form, dataurl, entityname, lblerrormsg) {
    document.getElementById(lblerrormsg).value = "";
    $("#divDisplayThresholdLimit1").empty();
    var flag = true;
    $.ajax({
        async: false,
        type: "POST",
        url: dataurl,
        data: form,
        success: function (data) {
            var list = document.createElement('ul');
            $.each(data, function (key, value) {
                flag = false;
                document.getElementById(lblerrormsg).value += "Threshold limit <u>[" + value + "]</u> is reached for <u>[" + key + "]</u><br/>";
            });
        },
        error: function (jqXHR, textStatus, errorThrown) {
            //alert(errorThrown + '-' + textStatus);
        }
    });
    if (!flag) {
        e.preventDefault();
        $("#divDisplayThresholdLimit").removeAttr("style");
        $("#divDisplayThresholdLimit").html(getMsgTableThresholdLimit());
        document.getElementById("ErrmsgtrThresholdLimit").innerHTML = document.getElementById(lblerrormsg).value;
        $("#divDisplayThresholdLimit1").html($("#divDisplayThresholdLimit").html());
    }
    return flag;
}

String.prototype.replaceAll = function (search, replacement) {
    var target = this;
    return target.replace(new RegExp(search, 'g'), replacement);
};


//dispaly msg
function getMsgTableEntitySave() {
    var Msgtable = "<div id='DisplayMessage'>" +
               "<div id='trEntitySave' class='DisplayMessageTitle' >" +
               "<span class='fa fa-exclamation-triangle'></span> Alert: </div>" +
               "<label id='ErrMsgEntitySave'></label>" +
               "</div>"
    "<script>" +
    "$('#trEntitySave').click(function () {" +
        "$(this).toggleClass('expand').nextUntil('tr.header').slideToggle(500);});" +
    "</script>";

    return Msgtable;
}
//function getMsgTableMandatory() {
//    var Msgtable = "<div id='DisplayMessage'>" +
//               "<div id='trRuleMandatory' class='DisplayMessageTitle' >" +
//               "<span class='fa fa-asterisk'></span> Mandatory Fields: </div>" +
//               "<label id='ErrMsgMandatory'></label>" +
//               "</div>" +

//                "<script>" +
//                "$('#trRuleMandatory').click(function () {" +
//                    "$(this).toggleClass('expand').nextUntil('tr.header').slideToggle(500);});" +
//                "</script>";

//    return Msgtable;
//}
//function getMsgTableUIAlert() {
//    var Msgtable = "<div id='DisplayMessage'>" +
//               "<div id='trRuleBeforeSaveProp' class='DisplayMessageTitle' >" +
//               "<span class='fa fa-exclamation-triangle'></span> Alert: </div>" +
//               "<label id='ErrMsgRuleBeforeSaveProp'></label>" +
//               "</div>" +
//                "<script>" +
//                "$('#trRuleBeforeSaveProp').click(function () {" +
//                    "$(this).toggleClass('expand').nextUntil('tr.header').slideToggle(500);});" +
//                "</script>";

//    return Msgtable;
//}
//function getMsgTableBeforeSaveProp() {
//    var Msgtable = "<div id='DisplayMessage'>" +
//               "<div id='trRuleBeforeSaveProp' class='DisplayMessageTitle' >" +
//               "<span class='fa fa-exclamation-triangle'></span> Alert: </div>" +
//               "<label id='ErrMsgRuleBeforeSaveProp'></label>" +
//               "</div>" +
//                "<script>" +
//                "$('#trRuleBeforeSaveProp').click(function () {" +
//                    "$(this).toggleClass('expand').nextUntil('tr.header').slideToggle(500);});" +
//                "</script>";

//    return Msgtable;

//}
//function getMsgTableLockBR() {
    //var Msgtable = "<div id='DisplayMessage'>" +
    //           "<div id='trRuleRLockRecord' class='DisplayMessageTitle' >" +
    //           "<span class='fa fa-lock'></span> Locked Record: </div>" +
    //           "<label id='ErrmsgLockRecord'></label>" +
    //           "</div>" +
    //           "<script>" +
    //           "$('#trRuleRLockRecord').click(function () {" +
    //               "$(this).toggleClass('expand').nextUntil('tr.header').slideToggle(500);});" +
    //           "</script>";


    //return Msgtable;

//}
//function getMsgTableReadOnly() {
//    var Msgtable = "<div id='DisplayMessage'>" +
//               "<div id='trRuleReadOnlyProp' class='DisplayMessageTitle' >" +
//               "<span class='fa fa-eye'></span> ReadOnly Properties: </div>" +
//               "<label id='ErrmsgtrRuleReadOnlyProp'></label>" +
//               "</div>" +
//               "<script>" +
//               "$('#trRuleReadOnlyProp').click(function () {" +
//                   "$(this).toggleClass('expand').nextUntil('tr.header').slideToggle(500);});" +
//               "</script>";

//    return Msgtable;
//}
function getMsgTableThresholdLimit() {
    var Msgtable = "<div id='DisplayMessage'>" +
               "<div id='trThresholdLimit' class='DisplayMessageTitle' >" +
               "Threshold Limit Message: </div>" +
                "<input  name='hdntxt' type='text' style='border:none;width:0px;height:0px' readonly required>" +
               "<label id='ErrmsgtrThresholdLimit'></label>" +
               "</div>" +
               "<script>"
    "$('#trThresholdLimit').click(function () {" +
        "$(this).toggleClass('expand').nextUntil('tr.header').slideToggle(500);});" +
    "</script>";

    return Msgtable;
}
function getMsgTableCodeFragment() {
    var Msgtable = "<div id='DisplayMessage'>" +
               "<div id='trCodeFragment' class='DisplayMessageTitle' >" +
               "<span class='fa fa-exclamation-triangle'></span> Alert: </div>" +
               "<label id='ErrmsgtrCodeFragment'></label>" +
               "</div>" +
               "<script>" +
               "$('#trCodeFragment').click(function () {" +
                   "$(this).toggleClass('expand').nextUntil('tr.header').slideToggle(500);});" +
               "</script>";

    return Msgtable;
}