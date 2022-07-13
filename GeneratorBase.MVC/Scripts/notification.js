
//function IconDataUnRead() {
//    var jsonIncon1 = [{ "InternalName": "fa-envelope fa-2x text-warning", "Name": "fa-envelope" }, { "InternalName": "fa-user fa-2x text-dark", "Name": "fa-user" }, { "InternalName": "fa-user-cog fa-2x text-primary", "Name": "fa-user-cog" }, { "InternalName": "fa-bullhorn fa-2x text-info", "Name": "fa-bullhorn" }]
//    return jsonIncon1;
//}
//function IconDataRead() {
//    var jsonIncon2 = [{ "InternalName": "fa-envelope fa-2x text-warning", "Name": "fa-envelope" }, { "InternalName": "fa-user fa-2x text-dark", "Name": "fa-user" }, { "InternalName": "fa-user-cog fa-2x text-primary", "Name": "fa-user-cog" }, { "InternalName": "fa-bullhorn fa-2x text-info", "Name": "fa-bullhorn" }]
//    return jsonIncon2;
//}
function BindUserRoleMultiSelect(RoleMdd, userMdd, span, span1) {
    debugger;
    var validateMDD = false;
    if ($('#' + RoleMdd + ' option:selected').length == 0) {
        $("#" + span1).show();
        validateMDD = true;
    }
    if ($('#' + userMdd + ' option:selected').length == 0) {
        $("#" + span).show();
        validateMDD = true;
    }

    var selVal = "";
    for (var o = 0; o < $('#' + RoleMdd + ' option:selected').length; o++) {
        selVal += $('#' + RoleMdd + ' option:selected')[o].value + ",";
    }
    //if ($('#' + RoleMdd + 'Value').val() != "All")
    $('#' + RoleMdd + 'Value').val(selVal);

    selVal = "";
    for (var u = 0; u < $('#' + userMdd + ' option:selected').length; u++) {
        selVal += $('#' + userMdd + ' option:selected')[u].value + ",";
    }
    $('#' + userMdd + 'Value').val(selVal);
    return validateMDD;
}
function BindUsersOLoad(RoleMdd, userMdd) {
    //bind user multiselect
    var txtvalue = $("#" + userMdd + "Value").val();
    if (txtvalue != undefined && txtvalue.length > 0) {
        var separated = txtvalue.split(",");
        for (var i = 0, length = separated.length; i < length; i++) {
            var chunk = $.trim(separated[i]);
            var ele = document.getElementById(userMdd);
            for (var o = 0; o < ele.options.length; o++) {
                if ($.trim(ele.options[o].value) == chunk) {
                    ele.options[o].selected = true;
                }
            }
        } $("#" + userMdd).multiselect('refresh');
    }
    //
    //bind role multiselect
    var txtvaluerole = $("#" + RoleMdd + "Value").val();
    if (txtvaluerole != undefined && txtvaluerole.length > 0) {
        var separated = txtvaluerole.split(",");
        for (var i = 0, length = separated.length; i < length; i++) {
            var chunk = $.trim(separated[i]);
            var ele = document.getElementById(RoleMdd);
            for (var o = 0; o < ele.options.length; o++) {
                if ($.trim(ele.options[o].value) == chunk) {
                    ele.options[o].selected = true;
                }
            }
        }
        $("#" + RoleMdd).multiselect('refresh');
    }
    //
}

//function BindUsersRoleOLoadCreate(RoleMdd, userMdd, roleId) {
//    //var roleVal = $("#" + RoleMdd).val();
//    var allText = "";
//    $('div[name=dvNotifyToRoles]').each(function (e) {
//        rolebtn = $(this).attr('id');
//        //var firstInput = $("#" + rolebtn).find('input[type=hidden],input[type=text],input[type=password],input[type=radio],input[type=checkbox],textarea,select').filter(':visible:first');
//        var firstButton = $("#" + rolebtn).find('button').filter(':first');
//        //var firstSelect = $("#" + rolebtn).find('select').filter(':first');
//        if ($(firstButton)[0].innerText.trim() == "All" || roleId == "") {
//            allText = "All";
//            $("#" + RoleMdd + "Value").val('All');
//        }
//    });
//    if (allText == "All") {
//        var ele = document.getElementById(userMdd);
//        for (var o = 0; o < ele.options.length; o++) {
//            ele.options[o].selected = true;
//        }
//        $("#" + userMdd).multiselect('refresh');
//    }
//}
function BindUsersRoleLoad(RoleMdd, userMdd, roleIdforAll) {
    //var roleVal = $("#" + RoleMdd).val();
    //if (roleIdforAll == "All") {
    var eleuser = document.getElementById(userMdd);
    for (var o = 0; o < eleuser.options.length; o++) {
        eleuser.options[o].selected = true;
    }
    $("#" + userMdd).multiselect('refresh');
    ////
    //var elerole = document.getElementById(RoleMdd);
    //for (var o = 0; o < elerole.options.length; o++) {
    //    if (elerole.options[o].value != "All") {
    //        debugger;
    //        elerole.options[o].setAttribute("style", "display:none;");
    //    }
    //}
    //$("#" + RoleMdd).multiselect('refresh');
    //
    //}

}
function selectRoleForUser(obj, userDdd, spanrole) {
    debugger;
    var rolesid = $(obj).val();
    var UrlVal = $(obj).attr('urlattr');
    var rVal = encodeURIComponent(JSON.stringify(rolesid));
    $.ajax({
        async: false,
        type: "GET",
        url: UrlVal + '?rolesId=' + rVal,
        success: function (result) {
            $('#' + userDdd).empty();
            var listItems = "";
            for (var i = 0; i < result.length; i++) {
                listItems += "<option value='" + result[i].Id + "'>" + result[i].UserName + "</option>";
            }
            $('#' + userDdd).append(listItems).multiselect('rebuild');
            var roleId = $(obj).attr('id');
            if (rolesid != null) {
                // if (rolesid[0] == "All") {
                BindUsersRoleLoad(roleId, userDdd, rolesid[0])
                //}
            }
            var selelen = $('#' + roleId + ' option:selected').length;
            if (selelen > 0) {
                $("#" + spanrole).hide();
            }

        },
        error: function (jqXHR, textStatus, errorThrown) {

        }
    });
}
function LoadTabNotfication(dvName, username, url) {
    $("#" + dvName).html('');
    if ($.trim($("#" + dvName).html()).length == 0) {
        $("#" + dvName).load(url);
        $("#" + dvName).prev().hide()
    }
}
function UpdateNotification(userName, urlUpt, coutid) {
    $.ajax({
        async: true,
        type: "GET",
        url: urlUpt + '?username=' + userName,
        success: function (result) {
            if ($("#" + coutid) != undefined)
                $("#" + coutid).html('');
        },
        error: function (jqXHR, textStatus, errorThrown) {

        }
    });
}
function ClearNotification(userName, urlUpt, coutid, notiId) {
    $.ajax({
        async: false,
        type: "GET",
        url: urlUpt + '?username=' + userName,
        success: function (result) {
            if ($("#" + coutid) != undefined)
                $("#" + coutid).html('');
            //topnavNotifications
            //$("#topnav" + notiId + "s").click();
            //debugger;
            //$("#" + notiId + "pop").removeClass();
            //$("#" + notiId + "pop").addClass("dropdown-menu dropdown-menu-right animated float-lg-left show");
            //putavailable('Des_Table');
        },
        error: function (jqXHR, textStatus, errorThrown) {
        }
    });
}
function DeleteNotification(userName, urlUpt, coutid, notiId) {
    $.ajax({
        async: true,
        type: "GET",
        url: urlUpt,
        complete: function (result) {
            $('#NotificationSearch').click();
            $("#SearchStringNotification").keypress();
        },
        success: function (result) {
            if ($("#" + coutid) != undefined)
                $("#" + coutid).remove();
            $("#SearchStringNotification").keypress();
            $('#NotificationSearch').click();
        },
        error: function (jqXHR, textStatus, errorThrown) {
        }
    });
}
function ClearNotificationEach(userName, urlUpt,id) {
    $.ajax({
        async: false,
        type: "GET",
        url: urlUpt + '?username=' + userName + "&id=" + id,
        success: function (result) {
            if ($("#" + id) != undefined)
                $("#" + id).remove();
            $("#topnavNotifications").click();
        },
        error: function (jqXHR, textStatus, errorThrown) {
        }
    });
}
function selectUserforNotfiy(RoleMdd, span) {
    var selelen = $('#' + RoleMdd + ' option:selected').length;
    if (selelen > 0)
        $("#" + span).hide();
}
//function selectunreadicon(obj,unread)
//{
//    var unreadddval = $(obj).val();
//    $("#" + unread).val('');
//    $("#" + unread).val(unreadddval);
//}
//function selectreadicon(obj, read) {
//    var readddval = $(obj).val();
//    $("#" + read).val('');
//    $("#" + read).val(readddval);
//}
//function FillUnreadIcon(obj) {
//    var data = IconData();
//    var unreadico = $(obj);
//    $.each(data, function (index, value) {
//        // APPEND OR INSERT DATA TO SELECT ELEMENT.
//        unreadico.append('<option value="' + value.ID + '">' + value.Name + '</option>');
//    });
//}
//function FillreadIcon(obj) {
//    var data = IconData();
//    var readico = $(obj);
//    $.each(data, function (index, value) {
//        // APPEND OR INSERT DATA TO SELECT ELEMENT.
//        readico.append('<option value="' + value.ID + '">' + value.Name + '</option>');
//    });
//}
function dateComp(stardate, enddate, errorDate) {
    var errorMsg = false;
    var start_time = $('#' + stardate).val();
    var end_time = $('#' + enddate).val();
    if (end_time.length > 0) {
        if (Date.parse(start_time) > Date.parse(end_time)) {
            var errorMsgtext = "End Date can be older the Start Date"
            $("#" + errorDate).show();
            $("#" + errorDate).html(errorMsgtext)
            errorMsg = true;
        }
    }
    return errorMsg;
}
function putavailable(obj) {
    if ($("#" + obj).find(".list-group").html().trim().length == 0) {
        var str = "<div class='list-group-item list-group-item-action' style='padding: .75rem .50rem !important;'><div class='media'>No New Notifications</div></div>";
        $("#" + obj).find(".list-group").html(str)
        $("#btnClearAll").hide();
    }
}