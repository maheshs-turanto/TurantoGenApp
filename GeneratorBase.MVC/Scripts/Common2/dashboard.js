BulkAddDropDown = "";
var GridQuery;
$(function () {
    $(window).bind("load resize", function () {
        if ($(this).width() < 768) {
            $('div.sidebar-collapse').addClass('collapse')
        } else {
            $('div.sidebar-collapse').removeClass('collapse')
        }
    })
})
function ShowViewAllInMultiSelect(result, element, elementName) {
    var openfalg = false;
    if (element == null || element == undefined || element.options == undefined)
        return false;
    var isHaveNullSelect = false;
    var countoptions = 0;
    var itmseleted = 0;
    for (var o = 0; o < element.options.length; o++) {
        //if (result.includes(element.options[o].value))
        if (result.indexOf(element.options[o].value) != -1)
            element.options[o].selected = true;
        else
            //if (result.includes("NULL"))
            if (result.indexOf("NULL") != -1)
                isHaveNullSelect = true;
        countoptions++;
    }
    if (itmseleted > 0) {
        if (!openfalg) {
            $("#A" + $("#" + elementName).closest('.panel-collapse').attr('id')).click();
            openfalg = true;
        }
    }
    var opt = document.createElement('option');
    opt.value = "NULL";
    opt.innerHTML = "None";
    if (isHaveNullSelect) {
        opt.selected = true;
        element.insertBefore(opt, element.firstChild);
    }
    //if (!isHaveNullSelect) {
    if ($("#" + elementName + " option[value=NULL]").length == 0) {
        element.insertBefore(opt, element.firstChild);
    }
    $("#" + elementName).multiselect("rebuild");
    if (countoptions >= 10) {
        var hostingentity = elementName;
        var urlGetAll = $('#' + hostingentity).attr("dataurl").replace("GetAllMultiSelectValue", "Index") + "?BulkOperation=multiple";
        var dispName = ($("label[for=\"" + hostingentity + "\"]").text());
        var link = "<a onclick=\"" + "OpenPopUpBulkOperation('PopupBulkOperation','" + hostingentity + "','" + dispName + "','dvPopupBulkOperation','" + urlGetAll + "')\">View All</a>";
        var getall = "<li class='disabled-result disabled-result' style='font-style:Italic;text-decoration:underline;' >" + link + "</li>";
        var $ul = $("ul", "#dv" + elementName);
        $("#dv" + elementName).find("ul").append($(getall))
    }
}
function MMDropdownGetAllValue1(entityName, SelectedListRemoved) {
    $("#Cnt" + entityName + "IDSelected").html($("#" + entityName + "IDSelected :selected").length);
    $("#" + entityName + "IDSelected option").each(function () {
        var value = SelectedListRemoved.split(',');
        for (i = 0; i < value.length; i++) {
            if (value != "" && value != "undefined") {
                var optionvalue = $(this).val();
                if (optionvalue == value[i])
                    $(this, "#" + entityName + "IDSelected").attr("style", "display:none;");
            }
        }
    });
}
function urlParam(name) {
    var results = new RegExp('[\?&]' + name + '=([^&#]*)').exec(window.location.href);
    var value = "";
    if (results != null && results != undefined)
        value = results[1]
    return value;
}
function AddValueInDropdown(result, element, elementName, dispvalue) {
    var flag = true;
    for (var o = 0; o < element.options.length; o++) {
        if (result == element.options[o].value) {
            element.options[o].selected = true; flag = false;
        }
    }
    if (flag) {
        var opt = document.createElement('option');
        opt.value = result;
        opt.text = dispvalue;
        opt.selected = true;
        element.insertBefore(opt, element.firstChild);
    }
    $("#" + elementName).multiselect({ buttonWidth: '100%', nonSelectedText: 'ALL' });
    $("#" + elementName).multiselect("rebuild");
}
function FacetedSearch(e, Entity, Asso, Prop, Prophdn, viewtype, sortby, isAsc, currentFilter, page) {
    //fSearch For validation
    var target;
    if (e.srcElement) target = e.srcElement;
    e = $.event.fix(e);
    if (e.currentTarget) target = e.currentTarget;
    e.preventDefault();
    var form = $(target).closest('form');
    if (!form.valid()) { return; }
    //
    var urlstring = $("#" + "fSearch" + Entity).attr("dataurl");
    var association = Asso.split(",");
    var property = Prop.split(",");
    //for datetime,timeonly
    var propertyhdn = Prophdn.split(",");
    for (i = 0; i < property.length; i++) {
        for (j = 0; j < propertyhdn.length; j++) {
            if (propertyhdn[j] == property[i] + "hdn") {
                ele = document.getElementById(property[i]);
                ele1 = document.getElementById(propertyhdn[j]);
                if (ele.value != null && ele.value != "") {
                    ele1.value = ele.value
                }
            }
        }
    }
    //
    //SaveServerTimeFsearch(document, false);
    var firstparam = 0;
    for (i = 0; i < association.length; i++) {
        var vals = "";
        ele = document.getElementById(association[i]);
        if (ele != null)
            for (var o = 0; o < ele.options.length; o++) {
                if (ele.options[o].selected)
                    vals += ele.options[o].value + ",";
            }
        if (vals.length > 0) {
            urlstring = addParameterToURL(urlstring, association[i], vals);
            //if (firstparam == 0)
            //    urlstring += "?" + association[i] + "=" + vals;
            //else
            //    urlstring += "&" + association[i] + "=" + vals;
            firstparam = 1;
        }
    }
    for (i = 0; i < property.length; i++) {
        ele = document.getElementById(property[i]);
        if (ele != null)
            if (ele.value.length > 0) {
                urlstring = addParameterToURL(urlstring, property[i], ele.value);
                //if (firstparam == 0)
                //    urlstring += "?" + property[i] + "=" + ele.value;
                //else
                //    urlstring += "&" + property[i] + "=" + ele.value;
                firstparam = 1;
            }
    }
    for (i = 0; i < propertyhdn.length; i++) {
        var ele = document.getElementById(propertyhdn[i]);
        if (ele != null)
            if (ele.value.length > 0) {
                urlstring = addParameterToURL(urlstring, propertyhdn[i], ele.value);
                //if (firstparam == 0)
                //    urlstring += "?" + propertyhdn[i] + "=" + ele.value;
                //else
                //    urlstring += "&" + propertyhdn[i] + "=" + ele.value;
                firstparam = 1;
            }
    }
    var page_sortstring = "";
    if (sortby != "") {
        page_sortstring = "&sortBy=" + sortby;
        if (isAsc != "")
            page_sortstring += "&isAsc=" + isAsc;
    }
    // if (viewtype != '') {
    //    page_sortstring += "&viewtype=" + viewtype;
    // }
    if (currentFilter != '') {
        if (firstparam == 0)
            urlstring += "?searchString=" + currentFilter + page_sortstring;
        else
            urlstring += "&searchString=" + currentFilter + page_sortstring;
    }
    if (firstparam == 0)
        urlstring += "?search=" + SanitizeURLString(document.getElementById("FSearch").value) + page_sortstring;
    else
        urlstring += "&search=" + SanitizeURLString(document.getElementById("FSearch").value) + page_sortstring;
    urlstring = addParameterToURL(urlstring, "SortOrder", $("#SortOrder").val());
    urlstring = addParameterToURL(urlstring, "GroupByColumn", $("#hdnGroupByColumn").val());
    urlstring = addParameterToURL(urlstring, "HideColumns", $("#HideColumns").val());
    urlstring = addParameterToURL(urlstring, "viewtype", $("#DisplayLayout").val());
    urlstring = addParameterToURL(urlstring, "FilterCondition", encodeURIComponent($("#FilterCondition").val()));
    window.location = (urlstring);
}
function sortClick(obj, PropertyName) {
    document.getElementById(PropertyName + "Sort").click();
}
//Inline Grid Edit
function tdDDValueChanged(obj, propertyName, dataurl, btnid) {
    var firstdiv = $('td.btnaction.active').first();
    if (firstdiv.length > 0) {
        var rowid = firstdiv.attr('rowid');
        if (rowid != undefined && rowid == btnid) {
            var input = $(this).find('input,textarea,select');
            if ($(input).attr('id') != $(obj).attr('id')) {
                var oldvalue = $(obj).attr('oldvalue');
                var newvalue = $(obj).val();
                if (oldvalue != newvalue) {
                    firstdiv.find('a').first().click();
                    return false;
                }
            }
        }
    }
}
function tdDoubleClick(obj, propertyName, dataurl, btnid) {
    var firstdiv = $('td.btnaction.active').first();
    var maxwidth = ($(obj).width());
    $('td.edit').each(function (e) {
        var input = $(this).find('input,textarea,select,radio');
        if ($(input).attr('id') != $(obj).attr('id')) {
            if ($(input).is(":checkbox")) {
                $(input).trigger('change');
            }
            else if ($(input).is(":radio")) {
                var newvalue = $(input).val();
                $(input).trigger('blur');
            }
            else
                if ($(input).is("select")) {
                    var offsetX = $(obj).positioncustom().left - $(input).positioncustom().left;
                    var offsetY = $(obj).positioncustom().top - $(input).positioncustom().top;
                    var cmbid = $(input).attr('id');
                    var valuecmb = $("#" + cmbid + " option:selected").text();
                    var oldvalue = $("#" + cmbid).attr('oldvalue');
                    var newvalue = $("#" + cmbid).val();
                    if (newvalue == undefined || newvalue.length == 0) valuecmb = 'null';
                    $('#cw' + cmbid).hide();
                    $("label", $('#cw' + cmbid).parent()).show();
                    if (oldvalue != newvalue) {
                        $("label", $('#cw' + cmbid).parent()).html(valuecmb);
                        $("label", $('#cw' + cmbid).parent()).css("color", "orange");
                    } else {
                        $("label", $('#cw' + cmbid).parent()).html(valuecmb);
                        $("label", $('#cw' + cmbid).parent()).css("color", "black");
                    }
                    $('#' + cmbid + '_chosen').attr('style', 'width:150px!important;position:absolute!important;left:' + offsetX + 'px!important;');
                }
                else {
                    $(input).attr("title", $(input).val());
                    $(input).trigger('blur');
                }
        }
    });

    if (firstdiv.length > 0) {
        var rowid = firstdiv.attr('rowid');
        if (rowid != undefined && rowid != btnid) {
            var r = confirm('Another row is already in edit mode, Do want to save the changes.');
            if (r == true) {
                firstdiv.find('a').first().click();
                return false;
            }
            else {
                firstdiv.find('a').last().click()
                return false;
            }
        }
    }

    if (!$(obj).hasClass("edit")) {
        var divcontrol = $(obj).find("div");
        var firstControl = divcontrol.find(':first');
        var controlname = firstControl.attr("controlname");
        // check before pageload bizrule (lock,readonlyproperties)
        var btn = $(obj).prevAll('td.btnaction').first();
        if (!btn.hasClass('active'))
            if (btn.length > 0) {
                var newbtn = $(btn).find("div");
                if (newbtn.length > 0) {
                    var dataurl = btn.attr('businessrule');
                    dataurl = addParameterToURL(dataurl, "loadtype", "OnEdit");
                    $.ajax({
                        type: "GET",
                        url: dataurl,
                        contentType: "application/json; charset=utf-8",
                        global: false,
                        cache: false,
                        async: false,
                        dataType: "json",
                        complete: function (jsonObj) {
                            var result = jsonObj.responseJSON.Result;
                            if (result != "Success") {
                                btn.attr(result, jsonObj.responseJSON.data)
                            }
                            if (result != "lock") {
                                btn.html(newbtn.html());
                                btn.addClass('active');
                            }
                            else { OpenAlertPopUp('Record is locked', jsonObj.responseJSON.data) }
                            $("body").css('cursor', 'default');
                        },
                        success: function (jsonObj) { },
                        error: function (jqXHR, textStatus, errorThrown) {
                            alert('some error');
                        }
                    });
                }
            }

        //
        if (btn.attr('lock') == undefined || btn.attr('lock').length == 0) {
            if (btn.attr('readonlyproperty') == undefined || btn.attr('readonlyproperty').length == 0 || btn.attr('readonlyproperty').indexOf(controlname) < 0) {
                if (firstControl.length > 0 && firstControl.is('select')) {
                    valuecmb = $("#" + firstControl.attr("id") + " option:selected").text();

                    $(obj).attr("style", "position:relative;overflow: visible !important;")
                    $(obj).html("<div id='cw" + firstControl.attr("id") + "' class='ChosenWrapper'>" + divcontrol.html().replace("class=\"form-control\"", "class=\"chosen-select form-control\"") + "</div><label style='font-weight:100;display:none;'>" + valuecmb + "</label>");
                    var config = {
                        '.chosen-select': {},
                        '.chosen-select-deselect': { allow_single_deselect: true },
                        '.chosen-select-no-single': { disable_search_threshold: 10 },
                        '.chosen-select-no-results': { no_results_text: 'Oops, nothing found!' },
                        '.chosen-select-width': { width: "95%" }
                    }
                    for (var selector in config) {
                        $(selector).chosen(config[selector]);
                    }
                    if (typeof FillDropInline == 'function') {
                        FillDropInline(firstControl.attr("id"))
                    }

                } else {
                    $(obj).html(divcontrol.html());
                }
                $(obj).addClass("edit");
                try {
                    document.getElementById(firstControl.attr("id")).focus();
                    if (firstControl.is('select')) {
                        $("#" + firstControl.attr("id")).trigger('chosen:activate');

                    }
                    else {
                        if ($(firstControl).val() == "0.00" || $(firstControl).val() == "0")
                            document.getElementById(firstControl.attr("id")).value = '';
                        else if ($(firstControl).val().length == 0)
                            document.getElementById(firstControl.attr("id")).value = '';
                    }
                } catch (ex) { }
            }
            else {
                $(obj).nextAll('td[onclick*="tdDoubleClick"]').first().click();
                return false;
            }
        }
        else {


        }
        //


    }
    else {
        var controlinside = $(obj).find(':first');
        if (controlinside.length > 0) {
            $(obj).find(':first').show();
            $("label", $(obj)).hide();
            document.getElementById(controlinside.attr("id")).focus();
            if (controlinside.hasClass('ChosenWrapper')) {
                $("#" + controlinside.find('select').attr("id")).trigger('chosen:activate');
            }
        }
    }
}
function tdDoubleClickInline(obj, propertyName, dataurl, btnid, pEntityName, assId, assoEntiy) {

    var firstdiv = $('td.btnaction.active').first();
    var maxwidth = ($(obj).width());

    $('td.edit').each(function (e) {
        var input = $(this).find('input,textarea,select');
        if ($(input).attr('id') != $(obj).attr('id')) {
            if ($(input).is(":checkbox")) {
                $(input).trigger('change');
            }
            else
                if ($(input).is("select")) {
                    var cmbid = $(input).attr('id');
                    var valuecmb = $("#" + cmbid + " option:selected").text();
                    var oldvalue = $("#" + cmbid).attr('oldvalue');
                    var newvalue = $("#" + cmbid).val();
                    if (newvalue == undefined || newvalue.length == 0) valuecmb = 'null';
                    $('#cw' + cmbid).hide();
                    $("label", $('#cw' + cmbid).parent()).show();
                    if (oldvalue != newvalue) {
                        $("label", $('#cw' + cmbid).parent()).html(valuecmb);
                        $("label", $('#cw' + cmbid).parent()).css("color", "orange");
                    } else {
                        $("label", $('#cw' + cmbid).parent()).html(valuecmb);
                        $("label", $('#cw' + cmbid).parent()).css("color", "black");
                    }
                }
                else { $(input).trigger('blur'); }
        }
    });

    if (firstdiv.length > 0) {
        var rowid = firstdiv.attr('rowid');
        if (rowid != undefined && rowid != btnid) {
            var r = confirm('Another row is already in edit mode, Do want to save the changes.');
            if (r == true) {
                firstdiv.find('a').first().click();
                return false;
            }
            else {
                firstdiv.find('a').last().click()
                return false;
            }
        }
    }

    if (!$(obj).hasClass("edit")) {
        var divcontrol = $(obj).find("div");
        var firstControl = divcontrol.find(':first');
        var controlname = firstControl.attr("controlname");
        // check before pageload bizrule (lock,readonlyproperties)
        var btn = $(obj).prevAll('td.btnaction').first();
        if (!btn.hasClass('active'))
            if (btn.length > 0) {

                var newbtn = $(btn).find("div#inline_" + assoEntiy + "_" + assId);
                if (newbtn.length > 0) {
                    var dataurl = btn.attr('businessrule');
                    $.ajax({
                        type: "GET",
                        url: dataurl,
                        contentType: "application/json; charset=utf-8",
                        global: false,
                        cache: false,
                        async: false,
                        dataType: "json",
                        complete: function (jsonObj) {
                            var result = jsonObj.responseJSON.Result;
                            if (result != "Success") {
                                btn.attr(result, jsonObj.responseJSON.data)
                            }
                            if (result != "lock") {
                                btn.html(newbtn.html());
                                btn.addClass('active');
                            }
                            else { OpenAlertPopUp('Record is locked', jsonObj.responseJSON.data) }
                            $("body").css('cursor', 'default');
                        },
                        success: function (jsonObj) { },
                        error: function (jqXHR, textStatus, errorThrown) {
                            alert('some error');
                        }
                    });
                }
            }

        //
        if (btn.attr('lock') == undefined || btn.attr('lock').length == 0) {
            if (btn.attr('readonlyproperty') == undefined || btn.attr('readonlyproperty').length == 0 || btn.attr('readonlyproperty').indexOf(controlname) < 0) {
                if (firstControl.length > 0 && firstControl.is('select')) {
                    valuecmb = $("#" + firstControl.attr("id") + " option:selected").text();
                    $(obj).html("<div id='cw" + firstControl.attr("id") + "' class='ChosenWrapper'>" + divcontrol.html().replace("class=\"form-control\"", "class=\"chosen-select form-control\"") + "</div><label style='font-weight:100;display:none;'>" + valuecmb + "</label>");
                    var config = {
                        '.chosen-select': {},
                        '.chosen-select-deselect': { allow_single_deselect: true },
                        '.chosen-select-no-single': { disable_search_threshold: 10 },
                        '.chosen-select-no-results': { no_results_text: 'Oops, nothing found!' },
                        '.chosen-select-width': { width: "95%" }
                    }
                    for (var selector in config) {
                        $(selector).chosen(config[selector]);
                    }
                } else {
                    $(obj).html(divcontrol.html());
                }
                $(obj).addClass("edit");
                try {
                    document.getElementById(firstControl.attr("id")).focus();
                    if (firstControl.is('select')) {
                        $("#" + firstControl.attr("id")).trigger('chosen:activate');
                    }
                    else {
                        if ($(firstControl).val() == "0.00" || $(firstControl).val() == "0")
                            document.getElementById(firstControl.attr("id")).value = '';
                    }
                } catch (ex) { }
            }
            else {
                $(obj).nextAll('td[onclick*="tdDoubleClickInline"]').first().click();
                return false;
            }
        }
        else {


        }
        //


    }
    else {
        var controlinside = $(obj).find(':first');
        if (controlinside.length > 0) {
            $(obj).find(':first').show();
            $("label", $(obj)).hide();
            document.getElementById(controlinside.attr("id")).focus();
            if (controlinside.hasClass('ChosenWrapper')) {
                $("#" + controlinside.find('select').attr("id")).trigger('chosen:activate');
            }
        }
    }
}

var IsConfirm = false;
function SavePropertiesValue(obj, entityName, ObjectId, url) {

    var postdata = [];
    var isDirty = false;
    var isError = false;

    $(document).keydown(function (event) {
        if (event.keyCode == 13) {
            event.preventDefault();
        }
    });

    $('td.edit').each(function () {
        var input = $(this).find('input,textarea,select,radio');
        var property = $(input).attr("controlname");
        var displayname = $(input).attr("displayname");
        var oldvalue = $(input).attr("oldvalue");
        var value = $(input).val();
        if ($(input).is(":checkbox")) {
            if ($(input).prop('checked') === true)
                value = true;
            else value = false;
        }
        else if ($(input).is(":radio")) {
            var value = $(input).val();
        }
        if (value == undefined)
            value = "";
        if (oldvalue != value) {
            postdata.push({ Key: property, Value: value });
            isDirty = true;
            var datatype = $(input).attr('datatype');
            if (displayname == undefined || displayname.length == 0) displayname = property;
            if (datatype != undefined && datatype.length > 0) {
                if (datatype == 'Decimal') {
                    var regex = /^(\+|-)?(\d*\.?\d*)$/;
                    if (!regex.test(value)) {
                        OpenAlertPopUp('The field ' + displayname + ' must be a valid decimal.');
                        isError = true;
                        IsConfirm = false;
                    }
                }
                if (datatype == 'Int32') {
                    var regex = /^(\+|-)?(\d*)$/;
                    if (!regex.test(value)) {
                        OpenAlertPopUp('The field ' + displayname + ' must be a valid number.');
                        isError = true;
                        IsConfirm = false;
                    }
                }
            }
        }
    });
    if (isDirty && !isError) {
        $(obj).parent().html("<span class='fa fa-spinner fa-spin' id='lblSaving" + ObjectId + "'><label>");
        $.ajax({
            type: "POST",
            data: { id: ObjectId, properties: postdata },
            url: url,
            asyc: false,
            success: function (jsonObj) {
                if (jsonObj.Result == "Success") {
                    $("#" + entityName + "Refresh").click();
                }
                else {

                    if (jsonObj.data.includes('Type15') && !IsConfirm) {
                        if (confirm(jsonObj.data.replace("Type15:", "").replace("  Type15:", ",").replace(/,\s*$/, ""))) {
                            IsConfirm = true;
                            SavePropertiesValue(obj, entityName, ObjectId, (url + "?IsConfirm=" + IsConfirm));

                        } else {
                            IsConfirm = false;
                            $("#" + entityName + "Refresh").click();
                            return true;
                        }
                    }
                    else {
                        if (!IsConfirm) {
                            OpenAlertPopUp("Record not updated.", jsonObj.data);
                            $("#" + entityName + "Refresh").click();
                            IsConfirm = false;
                        }
                        else {
                            IsConfirm = false;
                            $("#" + entityName + "Refresh").click();
                        }
                    }
                }
            },
            error: function (jqXHR, textStatus, errorThrown) {
                $("body").css('cursor', 'default');
                $('.fa.fa-spinner.fa-spin').attr('class', '');
                OpenAlertPopUp("Record not updated.", "");
                $("#" + entityName + "Refresh").click();
            }
        });
    } else { $("#" + entityName + "Refresh").click(); }
}
function SavePropertiesValueInline(obj, entityName, ObjectId, url, pEntity) {

    var postdata = [];
    var isDirty = false;
    var isError = false;
    $('td.edit').each(function () {
        var input = $(this).find('input,textarea,select');
        var property = $(input).attr("controlname");
        var displayname = $(input).attr("displayname");
        var oldvalue = $(input).attr("oldvalue");
        var value = $(input).val();
        if ($(input).is(":checkbox")) {
            if ($(input).prop('checked') === true)
                value = true;
            else value = false;
        }
        if (value == undefined)
            value = "";
        if (oldvalue != value) {
            postdata.push({ Key: property, Value: value });
            isDirty = true;
            var datatype = $(input).attr('datatype');
            if (displayname == undefined || displayname.length == 0) displayname = property;
            if (datatype != undefined && datatype.length > 0) {
                if (datatype == 'Decimal') {
                    var regex = /^(\+|-)?(\d*\.?\d*)$/;
                    if (!regex.test(value)) {
                        OpenAlertPopUp('The field ' + displayname + ' must be a valid decimal.');
                        isError = true;
                        IsConfirm = false;
                    }
                }
                if (datatype == 'Int32') {
                    var regex = /^(\+|-)?(\d*)$/;
                    if (!regex.test(value)) {
                        OpenAlertPopUp('The field ' + displayname + ' must be a valid number.');
                        isError = true;
                        IsConfirm = false;
                    }
                }
            }
        }
    });
    if (isDirty && !isError) {
        $(obj).parent().html("<span class='fa fa-spinner fa-spin' id='lblSaving" + ObjectId + "'><label>");
        $.ajax({
            type: "POST",
            data: { id: ObjectId, properties: postdata },
            url: url,
            asyc: false,
            success: function (jsonObj) {
                if (jsonObj.Result == "Success") {
                    $("#" + pEntity + "Refresh").click();
                }
                else {

                    if (jsonObj.data.includes('Type15') && !IsConfirm) {
                        if (confirm(jsonObj.data.replace("Type15:", "").replace("  Type15:", ",").replace(/,\s*$/, ""))) {
                            IsConfirm = true;
                            SavePropertiesValueInline(obj, entityName, ObjectId, (url + "?IsConfirm=" + IsConfirm), pEntity);

                        } else {
                            IsConfirm = false;
                            $("#" + pEntity + "Refresh").click();
                            return true;
                        }
                    }
                    else {
                        if (!IsConfirm) {
                            OpenAlertPopUp("Record not updated.", jsonObj.data);
                            $("#" + pEntity + "Refresh").click();
                            IsConfirm = false;
                        }
                        else {
                            IsConfirm = false;
                            $("#" + pEntity + "Refresh").click();
                        }
                    }
                }
            },
            error: function (jqXHR, textStatus, errorThrown) {
                $("body").css('cursor', 'default');
                $('.fa.fa-spinner.fa-spin').attr('class', '');
                OpenAlertPopUp("Record not updated.", "");
                $("#" + pEntity + "Refresh").click();
            }
        });
    } else { $("#" + pEntity + "Refresh").click(); }
}

function tdNextActionInline(obj, e, entityName, pEntityname) {
    var keyCode = e.keyCode || e.which;
    if (keyCode == 9) {
        if ($(obj).is(":checkbox")) {
            $(obj).trigger('change');
        }
        var extreme = false;
        var nextclick = undefined;
        if (e.shiftKey) {
            var prevEle = $(obj).parent().prevAll('td[onclick*="tdDoubleClickInline"]', $(obj).parent().parent()).first();
            if (prevEle.length == 0) {
                prevEle = $(obj).parent().nextAll('td[onclick*="tdDoubleClickInline"]', $(obj).parent().parent()).last();
                extreme = true;
            }
            nextclick = prevEle;
            e.preventDefault();
            prevEle.click();
        }
        else {
            var nextEle = $(obj).parent().nextAll('td[onclick*="tdDoubleClickInline"]', $(obj).parent().parent()).first();
            if (nextEle.length == 0) {
                nextEle = $(obj).parent().prevAll('td[onclick*="tdDoubleClickInline"]', $(obj).parent().parent()).last();
                extreme = true;
            }
            nextclick = nextEle;
            e.preventDefault();
            nextEle.click();
        }
        if (nextclick != undefined) {
            var firstControl = nextclick.find(':first');
            if (firstControl.length > 0) {
                if (firstControl.hasClass('ChosenWrapper')) {
                    var table = $(obj).closest('#Des_Table');
                    var leftPos = table.scrollLeft();
                    var width = table.width();
                    if (firstControl.position().left + 120 >= width)
                        if (evt.shiftKey)
                            if (!extreme)
                                table.animate({ scrollLeft: leftPos - 120 }, 'fast');
                            else
                                table.animate({ scrollLeft: width }, 'fast');
                        else
                            if (!extreme)
                                table.animate({ scrollLeft: leftPos + 120 }, 'fast');
                            else
                                table.animate({ scrollLeft: 0 }, 'fast');
                }
            }
        }

    } else
        if (keyCode == 27) {
            $("#" + pEntityname + "Refresh").click();
        } else
            if (keyCode == 13) {
                var firstdiv = $('td.btnaction.active').first();
                if (firstdiv.length > 0) {
                    if (!$(obj).is("textarea")) {
                        firstdiv.find('a').first().click();
                    }
                    else {
                        if (e.shiftKey) firstdiv.find('a').first().click();
                    }
                }
            } else if (keyCode == 32) {
                if ($(obj).is(":checkbox")) {
                    $(obj).parent().next('td').first().click();
                    if ($(obj).prop('checked'))
                        $(obj).prop('checked', false);
                    else
                        $(obj).prop('checked', true);
                    $(obj).trigger('change');
                } else { return false; }
            }
}
function tdNextAction(obj, e, entityName) {
    var keyCode = e.keyCode || e.which;
    if (keyCode == 9) {
        if ($(obj).is(":checkbox")) {
            $(obj).trigger('change');
        }
        var extreme = false;
        var nextclick = undefined;
        if (e.shiftKey) {
            var prevEle = $(obj).parent().prevAll('td[onclick*="tdDoubleClick"]', $(obj).parent().parent()).first();
            if (prevEle.length == 0) {
                prevEle = $(obj).parent().nextAll('td[onclick*="tdDoubleClick"]', $(obj).parent().parent()).last();
                extreme = true;
            }
            nextclick = prevEle;
            e.preventDefault();
            prevEle.click();
        }
        else {
            var nextEle = $(obj).parent().nextAll('td[onclick*="tdDoubleClick"]', $(obj).parent().parent()).first();
            if (nextEle.length == 0) {
                nextEle = $(obj).parent().prevAll('td[onclick*="tdDoubleClick"]', $(obj).parent().parent()).last();
                extreme = true;
            }
            nextclick = nextEle;
            e.preventDefault();
            nextEle.click();
        }
        if (nextclick != undefined) {
            var firstControl = nextclick.find(':first');
            if (firstControl.length > 0) {
                if (firstControl.hasClass('ChosenWrapper')) {
                    var table = $(obj).closest('#Des_Table');
                    var leftPos = table.scrollLeft();
                    var width = table.width();
                    if (firstControl.position().left + 120 >= width)
                        if (evt.shiftKey)
                            if (!extreme)
                                table.animate({ scrollLeft: leftPos - 120 }, 'fast');
                            else
                                table.animate({ scrollLeft: width }, 'fast');
                        else
                            if (!extreme)
                                table.animate({ scrollLeft: leftPos + 120 }, 'fast');
                            else
                                table.animate({ scrollLeft: 0 }, 'fast');
                }
            }
        }

    } else
        if (keyCode == 27) {
            $("#" + entityName + "Refresh").click();
        } else
            if (keyCode == 13) {
                var firstdiv = $('td.btnaction.active').first();
                if (firstdiv.length > 0) {
                    if (!$(obj).is("textarea")) {
                        firstdiv.find('a').first().click();
                    }
                    else {
                        if (e.shiftKey) firstdiv.find('a').first().click();
                    }
                }
            } else if (keyCode == 32) {
                if ($(obj).is(":checkbox")) {
                    $(obj).parent().next('td').first().click();
                    if ($(obj).prop('checked'))
                        $(obj).prop('checked', false);
                    else
                        $(obj).prop('checked', true);
                    $(obj).trigger('change');
                } else { return false; }
            }
}
function SavePropertyValue(obj, entityName, ObjectId, oldValue, url) {

    var value = undefined;
    if ($(obj).is(":checkbox")) {
        if (oldValue == 'True' || oldValue == 'true' || oldValue == true) oldValue = true;
        else oldValue = false;
        if ($(obj).prop('checked') === true)
            value = true;
        else value = false;
    }
    else {
        value = $(obj).val();
    }
    if (!$(obj).is('select')) {
        $(obj).hide();
        $("label", $(obj).parent()).show();
        $("label", $(obj).parent()).html(value.toString());
    }
    else {
    }
    if (value == undefined)
        value = "";
    if (value != oldValue) {
        $("label", $(obj).parent()).css("color", "blue").css("font-weight", "600");
    }
    else $("label", $(obj).parent()).css("color", "");
}
function addClassOnTD(obj) {
    $(obj).prevAll('td.btnaction').first().addClass('active');
    $(obj).addClass('edit');
}
function SelectAllRadio(obj, name, id, EnityName, url, Ids) {
    var checked = false;
    var table = $('input[name="' + name + id + '"]');
    if ($(obj).is(":checked")) {
        checked = true;
        $('body').append('<div id="over" style="position: absolute;top:0;left:0;width: 100%;height:100%;z-index:2;opacity:0.4;filter: alpha(opacity = 50)"></div>');
    }
    if (checked = true) {
        $(table).each(function () {
            if (this != obj && $(this).attr('name') == name + id && $(this).is(":checked") == false) {
                $(this).parent().removeAttr('onclick');
                $(this).removeAttr('onkeydown');
                $(this).removeAttr('onchange');
                //$(this).click();
                //this.checked=true;
                // $(this).attr('checked', true);
            }
        });
        //$('input[name="' + name + id + '"]').each(function () { this.checked = true; })
        var postdata = [];
        postdata.push({ Key: name, Value: id });
        $.ajax({
            type: "POST",
            data: { bulkIds: Ids, properties: postdata },
            url: url,
            asyc: false,
            success: function (jsonObj) {
                if (jsonObj.Result == "Success") {
                    $("#" + EnityName + "Refresh").click();
                    $("#over").remove();
                }
                else {
                    $("#over").remove();
                    OpenAlertPopUp("Record not updated.", jsonObj.data);
                    $("#" + EnityName + "Refresh").click();
                }
            },
            error: function (jqXHR, textStatus, errorThrown) {
                $("#over").remove();
                OpenAlertPopUp("Record not updated.", "fail");
                $("#" + EnityName + "Refresh").click();
            }
        });
        //$("#Des_Table").find('input[name=' + name + id + ']').prop("checked", true)
    }

}
function SelectRadio(obj, name, id, EnityName, url, Ids) {
    var checked = false;
    if ($(obj).is(":checked")) {
        checked = true;
    }
    if (checked = true) {
        var postdata = [];
        postdata.push({ Key: name, Value: id });
        $.ajax({
            type: "POST",
            data: { bulkIds: Ids, properties: postdata },
            url: url,
            asyc: false,
            success: function (jsonObj) {
                if (jsonObj.Result == "Success") {
                    $("#" + EnityName + "Refresh").click();
                }
                else {
                    OpenAlertPopUp("Record not updated.", jsonObj.data);
                    $("#" + EnityName + "Refresh").click();
                }
            },
            error: function (jqXHR, textStatus, errorThrown) {
                OpenAlertPopUp("Record not updated.", "fail");
                $("#" + EnityName + "Refresh").click();
            }
        });
    }

}
//End Inline Grid Edit
function FacetedSearchSave(e, Entity, Asso, Prop, Prophdn, viewtype, sortby, isAsc, currentFilter, page) {
    //fSearch For validation
    var target;
    if (e.srcElement) target = e.srcElement;
    e = $.event.fix(e);
    if (e.currentTarget) target = e.currentTarget;
    e.preventDefault();
    var form = $(target).closest('form');
    if (!form.valid()) { return; }
    //
    var urlstring = $("#" + "fSearch" + Entity).attr("dataurl");
    var popupurl = urlstring.replace(Entity, "T_FacetedSearch").replace("FSearch", "CreateQuick");
    var association = Asso.split(",");
    var property = Prop.split(",");
    //for datetime,timeonly
    var propertyhdn = Prophdn.split(",");
    for (i = 0; i < property.length; i++) {
        for (j = 0; j < propertyhdn.length; j++) {
            if (propertyhdn[j] == property[i] + "hdn") {
                ele = document.getElementById(property[i]);
                ele1 = document.getElementById(propertyhdn[j]);
                if (ele.value != null && ele.value != "") {
                    ele1.value = ele.value
                }
            }
        }
    }
    //
    SaveServerTimeFsearch(document, false);
    var firstparam = 0;
    for (i = 0; i < association.length; i++) {
        var vals = "";
        ele = document.getElementById(association[i]);
        if (ele != null)
            for (var o = 0; o < ele.options.length; o++) {
                if (ele.options[o].selected)
                    vals += ele.options[o].value + ",";
            }
        if (vals.length > 0) {
            if (firstparam == 0)
                urlstring += "?" + association[i] + "=" + vals;
            else
                urlstring += "&" + association[i] + "=" + vals;
            firstparam = 1;
        }
    }
    for (i = 0; i < property.length; i++) {
        ele = document.getElementById(property[i]);
        if (ele != null)
            if (ele.value.length > 0) {
                if (firstparam == 0)
                    urlstring += "?" + property[i] + "=" + ele.value;
                else
                    urlstring += "&" + property[i] + "=" + ele.value;
                firstparam = 1;
            }
    }
    for (i = 0; i < propertyhdn.length; i++) {
        var ele = document.getElementById(propertyhdn[i]);
        if (ele != null)
            if (ele.value.length > 0) {
                if (firstparam == 0)
                    urlstring += "?" + propertyhdn[i] + "=" + ele.value;
                else
                    urlstring += "&" + propertyhdn[i] + "=" + ele.value;
                firstparam = 1;
            }
    }
    var page_sortstring = "";
    if (sortby != "") {
        page_sortstring = "&sortBy=" + sortby;
        if (isAsc != "")
            page_sortstring += "&isAsc=" + isAsc;
    }
    // if (viewtype != '') {
    //    page_sortstring += "&viewtype=" + viewtype;
    // }
    if (currentFilter != '') {
        if (firstparam == 0)
            urlstring += "?searchString=" + currentFilter + page_sortstring;
        else
            urlstring += "&searchString=" + currentFilter + page_sortstring;
    }
    if (firstparam == 0)
        urlstring += "?search=" + SanitizeURLString(document.getElementById("FSearch").value) + page_sortstring;
    else
        urlstring += "&search=" + SanitizeURLString(document.getElementById("FSearch").value) + page_sortstring;
    urlstring = addParameterToURL(urlstring, "SortOrder", $("#SortOrder").val());
    urlstring = addParameterToURL(urlstring, "GroupByColumn", $("#hdnGroupByColumn").val());
    urlstring = addParameterToURL(urlstring, "HideColumns", $("#HideColumns").val());
    urlstring = addParameterToURL(urlstring, "viewtype", $("#DisplayLayout").val());
    urlstring = addParameterToURL(urlstring, "FilterCondition", $("#FilterCondition").val());
    var dvName = "dvPopup";
    addPopup = "addPopup";
    $("#" + addPopup + 'Label').html();
    $("#" + addPopup + 'Label').html("Save " + " Query");
    $("#" + addPopup).modal('show');
    $("#" + dvName).html('Loading..');//uncommented on 7/7/2017
    $("#" + dvName + "Error").html('');
    popupurl = addParameterToURL(popupurl, "EntityName", Entity);
    popupurl = addParameterToURL(popupurl, "Url", urlstring);
    $("#" + dvName).load(popupurl);
    //   return urlstring
}
//function ClearChildDD(child, obj, isreverse) {
//    $("#" + child).html("<option value=''>--Select--</option>");
//    $("#" + child).val('');
//    var IsReverse = isreverse;//$(obj).attr("IsReverse");
//    if (IsReverse == "true" || IsReverse == "True" && $(obj).val().length > 0) {
//        $("#" + child).removeAttr("lock");
//        $("#" + child).trigger('chosen:updated');
//        $("#" + child).trigger('chosen:open');
//        $("#" + child).trigger('change');
//        $("#" + child).attr("lock", "true");
//    }
//    else {
//        // if ($(obj).val().length > 0)
//        $("#" + child).removeAttr("lock");
//        // else
//        //  $("#" + child).attr("lock", "true");
//    }
//    $("#" + child).trigger('chosen:updated');
//}
function ClearChildDD(child, obj, isreverse) {
    var IsReverse = isreverse;//$(obj).attr("IsReverse");
    var childs = child.split(",");
    if (IsReverse == "true" || IsReverse == "True" && $(obj).val().length > 0) {
        for (var i = 0; i < childs.length; i++) {
            if ($("#" + childs[i]).attr("multiple") == undefined)
                $("#" + childs[i]).html("<option value=''>--Select--</option>");
            $("#" + childs[i]).val('');
            $("#" + childs[i]).removeAttr("lock");
            select(childs[i], "");
            $("#" + childs[i]).attr("lock", "true");
            $("#" + childs[i]).trigger('chosen:updated');
        }
        $(obj).trigger('chosen:close');
    } else {
        if ($("#" + child).attr("multiple") == undefined) {
            $("#" + child).html("<option value=''>--Select--</option>");
            $("#" + child).val('');
            $("#" + child).removeAttr("lock");
            $("#" + child).trigger('chosen:updated');
        } else {
            for (var i = 0; i < childs.length; i++) {
                $("#" + childs[i]).val('');
                $("#" + childs[i]).removeAttr("lock");
                select(childs[i], "");
                $("#" + childs[i]).attr("lock", "true");
                $("#" + childs[i]).trigger('chosen:updated');
            }
            $('#' + child).find('option[value=""]').remove();
        }
    }
}
function ClearMultiSelectChild(child) {
    $('#' + child).multiselect('clearSelection');
}
function FillCascadingDD(parent, child, childName, assoName) {
    {
        var parentDD = $("#" + parent);
        var childDD = $("#" + child);
        var selectedval = $("option:selected", parentDD).val();
        $.ajax({
            type: "GET",
            url: "/" + childName + "/GetValuesByAssociation/" + selectedval + "?HostedBy=" + assoName,
            contentType: "application/json; charset=utf-8",
            global: false,
            cache: false,
            async: true,
            dataType: "json",
            beforeSend: function () {
                $("#" + child).html("<option value=''>Please wait...</option>")
                $("body").css('cursor', 'wait');
                $("#" + child).attr("size", "1");
            },
            complete: function () {
                $("body").css('cursor', 'default');
            },
            success: function (jsonObj) {
                var listItems = "";
                $("#" + child).empty();
                listItems += "<option value=''>--None--</option>";
                for (i in jsonObj) {
                    listItems += "<option value='" + jsonObj[i].Id + "'>" + jsonObj[i].Name + "</option>";
                }
                $("#" + child).html(listItems);
                $("#" + child).focus(down).blur(up).focus();
            }
        });
        $("#" + child).click(function () {
            up();
        });
    } return false;
    function down() {
        var pos = $("#" + child).offset();
        var len = $("#" + child).find("option").length;
        if (len > 10) {
            len = 10;
        }
        $("#" + child).css("position", "absolute");
        $("#" + child).css("zIndex", 100);
        $("#" + child).offset(pos);   // reset position
        $("#" + child).attr("size", len); // open dropdown
        $("#" + child).unbind("focus", down);
        $("#" + child).focus();
    }
    function up() {
        $("#" + child).css("position", "static");
        $("#" + child).attr("size", "1");  // close dropdown
        $("#" + child).unbind("blur", up);
        $("#" + child).focus(); $("#" + child).removeAttr("size");
    }
}
function getPath() {
    var path = "";
    nodes = window.location.pathname.split('/');
    for (var index = 0; index < nodes.length - 3; index++) {
        path += "../";
    }
    return path;
}
function OpenQuickQddPopUp(dvName) {
    var url = $("#" + dvName).data("url");
    $("#" + dvName).load(url);
}
function NavigateToUrl(url) {
    window.location.href = url;
    //window.location.replace(url);
}
function OpenDashBoard(dvName) {
    var url = $("#" + dvName).data("url");//+"?TS="+Date.now();
    $("#" + dvName).load(url);
}
function OpenDashBoardFromHome(obj, dvName, entName) {
    var url = $(obj).attr("dataurl");//+"?TS="+Date.now();
    $("#EntityGraphLabel").html(entName)
    $("#" + dvName).load(url);
}
function OpenPopUpGraph(Popup, dvName, url) {
    $("#" + Popup).modal('show');
    $("#" + Popup).find('.modal-dialog.ui-draggable').attr("style", "width:90%");
    $("#" + dvName).html('');
    $("#" + dvName).load(url);
}
//Refresh index list 
function RefreshGrid(dvName, url) {
    $("#" + dvName).load(url);
}
function RefreshGridFSearch(dvName, url) {
    var host = (getHostingEntityID(url)["AssociatedType"]);
    var IsFilter = (getHostingEntityID(url)["IsFilter"]);
    var IsdrivedTab = (getHostingEntityID(url)["IsdrivedTab"]);
    $.ajax({
        url: url,
        cache: false,
        complete: function (data) {
            $('body').css({ 'cursor': 'default' });
            var filterbtn = $("#" + dvName + "SetFSearchGridbtn");
            if (filterbtn != undefined)
                filterbtn.click();
            // (document.getElementById(dvName+'GridQuery').className = 'in'); todo open box
        },
        success: function (data) {
            if (data != null) {
                if (host != undefined && IsFilter != "True" && $('#' + host).length > 0) {
                    if ($('#' + dvName, $('#' + host)).attr('id') == undefined)
                        $('#' + dvName, $('#dv' + host)).html(data);
                    else
                        $('#' + dvName, $('#' + host)).html(data);
                    if (IsdrivedTab) {
                        $("a[href='" + host + "']").trigger("click");
                    }
                }
                else {
                    try {
                        $('#' + dvName).html(data);
                        if (IsdrivedTab) {
                            $("a[href='" + host + "']").trigger("click");
                        }
                    } catch (ex) { }
                }
            }
        }
    })
    return false;
}
function ClearTabCookie(username) {
    $.cookie(username + "TabCookie", null);
    if ($('#aextratab').length > 0) {
        $('#aextratab').html("More");
    }
}
function LoadTab(dvName, username, url) {
    var inlinegrid = ($("#dvGroup" + dvName).attr("inlinegrid"));
    var pivot = "";
    var pivotlist = "";
    var pivotAncher = $("#dvGroup" + dvName + "Pivot");
    var pivotAncherlist = $("#dvGroup" + dvName + "PivotList");
    if (pivotAncher != undefined) {
        pivot = $(pivotAncher).attr("pivot");
    }
    if (pivotAncherlist != undefined) {
        pivotlist = $(pivotAncherlist).attr("pivot");
    }
    if (dvName.length > 0 && inlinegrid != 'true')
        $.cookie(username + "TabCookie", dvName);
    if (pivot == 'true') {
        $("#" + dvName).html('')
        $("#" + dvName).html('Please wait..');
        $("#" + dvName).load(url);
    }
    else if (pivotlist == 'true') {
        $("#" + dvName).html('')
        $("#" + dvName).html('Please wait..');
        $("#" + dvName).load(url);
    }
    else {
        if ($.trim($("#" + dvName).html()).length == 0) {
            $("#" + dvName).html('Please wait..');
            $("#" + dvName).load(url);
        }
    }

    if ($('#aextratab').length > 0) {
        if ($("#dvGroup" + dvName, $(".responsivetabs-more")).length > 0)
            $('#aextratab').html($('#dvGroup' + dvName).html());
        else $('#aextratab').html("More");
    }
}
function LoadAccordion(dvName, username, url) {
    if ($.trim($("#" + dvName).html()).length == 0) {
        $("#" + dvName).html('Please wait..');
        $("#" + dvName).load(url);
    }
}
function JumpToTab(username) {
    if ($.cookie(username + 'TabCookie') != null) {
        if ($('a[href="#' + $.cookie(username + 'TabCookie') + '"]:visible').length > 0) {
            $('.nav-tabs li:first').removeClass();
            $("#Details").removeClass();
            $("#Details").addClass("tab-pane");
            $('a[href="#' + $.cookie(username + 'TabCookie') + '"]').click();
        }
        else {
            $('.nav-tabs li:first').addClass("active");
            $('a[href="#Details"]').click();
            $("#Details").addClass("tab-pane fade in active");
        }
    }
    else {
        $('.nav-tabs li:first').addClass("active");
        $('a[href="#Details"]').click();
        $("#Details").addClass("tab-pane fade in active");
    }
}
function LoadTabMobile(dvName, url) {
    if ($.trim($("#" + dvName).html()).length == 0) {
        $("#" + dvName).html('Please wait..');
        $("#" + dvName).load(url);
    }
}
function LoadTabTemplate(dvName, url) {
    $("#" + dvName).html('Please wait..');
    $("#" + dvName).load(url);
}
//Refresh index list 
function CancelSearch(dvName, url, UserName) {
    //remove pagination cookies 
    if ($.cookie("pagination" + UserName + dvName) != null)
        $.removeCookie("pagination" + UserName + dvName);
    var host = (getHostingEntityID(url)["AssociatedType"]);
    var IsFilter = (getHostingEntityID(url)["IsFilter"]);
    var IsdrivedTab = (getHostingEntityID(url)["IsdrivedTab"]);
    $.ajax({
        url: url,
        cache: false,
        success: function (data) {
            if (data != null) {
                if (host != undefined && IsFilter != "True" && $('#' + host).length > 0) {
                    if ($('#' + dvName, $('#' + host)).attr('id') == undefined)
                        $('#' + dvName, $('#dv' + host)).html(data);
                    else
                        $('#' + dvName, $('#' + host)).html(data);
                    if (IsdrivedTab) {
                        $("a[href='" + host + "']").trigger("click");
                    }
                }
                else {
                    try {
                        $('#' + dvName).html(data);
                        if (IsdrivedTab) {
                            $("a[href='" + host + "']").trigger("click");
                        }
                    } catch (ex) { }
                }
            }
        }
    })
    return false;
}
function CancelSearchBulk(dvName, url, UserName) {
    //remove pagination cookies 
    if ($.cookie("pagination" + UserName + dvName) != null)
        $.removeCookie("pagination" + UserName + dvName);
    var host = (getHostingEntityID(url)["AssociatedType"]);
    var IsFilter = (getHostingEntityID(url)["IsFilter"]);
    $.ajax({
        url: url,
        cache: false,
        success: function (data) {
            if (data != null) {
                if (host != undefined && IsFilter != "True" && $('#' + host).length > 0) {
                    $($("." + dvName)[1]).html(data);
                    //$('#' + dvName, $('#' + host)).html(data);
                }
                else {
                    try {
                        $('#' + dvName).html(data);
                    } catch (ex) { }
                }
            }
        }
    })
    return false;
}
function OpenPopUpEntityBR(Popup, EntityName, dvName, url) {
    $("#" + Popup + 'Label').html();
    $("#" + Popup + 'Label').html("" + EntityName);
    // $("#" + Popup).modal('show');
    $("#" + dvName).html('Loading..');
    // $("#" + dvName).load(url);
    $("#" + dvName).load(url, function () {
        $("#" + Popup).modal('show');
    });
}
//
//Quick Add From pop window for drop down
function OpenPopUpEntity(Popup, EntityName, dvName, url) {
    // $("#" + Popup + 'Label').html();
    // if ((EntityName.indexOf("Edit") == -1) && (EntityName.indexOf("Delete") == -1)) {
    // $("#" + Popup + 'Label').html("Add " + EntityName);
    // }
    // else {
    // $("#" + Popup + 'Label').html(EntityName);
    // }
    // $("#" + Popup).modal('show');
    // $("#" + dvName).html('Loading..');//uncommented on 7/7/2017
    // $("#" + dvName + "Error").html('');
    // $("#" + dvName).load(url);

    $("#" + Popup + 'Label').html();
    if ((EntityName.indexOf("Edit") == -1) && (EntityName.indexOf("Delete") == -1)) {
        $("#" + Popup + 'Label').html("Add " + EntityName);
    }
    else {
        $("#" + Popup + 'Label').html(EntityName);
    }

    $("#" + dvName).html('Loading..');//uncommented on 7/7/2017
    $("#" + dvName + "Error").html('');

    $("#" + dvName).load(url, function () {
        $("#" + Popup).modal('show');
    });
}
function OpenAlertPopUp(header, msg) {
    $('#PopupBulkOperationLabel').html(header);
    $("#" + "PopupBulkOperation").modal('show');
    $("#" + "PopupBulkOperation").find('.modal-dialog.ui-draggable').attr("style", "width:50%");
    $("#" + "dvPopupBulkOperation").html(msg);
}

function OpenCustomPopUp(header, msg, ok, close) {
    $('#PopupSubmissionLabel').html(header);
    $("#" + "PopupSubmission").modal('show');
    $("#" + "PopupSubmission").find('.modal-dialog.ui-draggable').attr("style", "width:50%");
    if (ok.length > 0)
        $("#okPopupSubmission").html(ok);
    else $("#okPopupSubmission").hide();
    if (close.length > 0)
        $("#closePopupSubmission").html(close);
    else $("#closePopupSubmission").hide();
    $("#" + "dvPopupSubmission").html(msg);
}
function CumstomConfirmDialog(header, msg, ok, close, yesCallback, noCallback) {
    $('#PopupSubmissionLabel').html(header);
    $("#" + "PopupSubmission").modal('show');
    $("#" + "PopupSubmission").find('.modal-dialog.ui-draggable').attr("style", "width:50%");
    if (ok.length > 0)
        $("#okPopupSubmission").html(ok);
    else $("#okPopupSubmission").hide();
    if (close.length > 0)
        $("#closePopupSubmission").html(close);
    else $("#closePopupSubmission").hide();
    $("#" + "dvPopupSubmission").html(msg);

    $('#okPopupSubmission').click(function () {
        //dialog.dialog('close');
        yesCallback();
    });
    $('#closePopupSubmission').click(function () {
        //dialog.dialog('close');
        noCallback();
    });
}
//Open PopUp From List On  QuickEdit
function OpenPopUpEntityQuickEdit(Popup, EntityName, dvName, url) {
    $("#" + Popup + 'Label').html();
    if ((EntityName.indexOf("Edit") == -1) && (EntityName.indexOf("Delete") == -1)) {
        $("#" + Popup + 'Label').html("Add " + EntityName);
    }
    else {
        $("#" + Popup + 'Label').html(EntityName);
    }
    //$("#" + Popup).modal('show');
    $("#" + dvName).html('Loading..');//uncommented on 7/7/2017
    $("#" + dvName + "Error").html('');
    //$("#" + dvName).load(url);
    $("#" + dvName).load(url, function () {
        $("#" + Popup).modal('show');
    });
}
function OpenPopUpEntity1M(obj, Popup, EntityName, dvName, url) {
    $("#" + Popup + 'Label').html();
    var HostingDispVal = ($("#HostingEntityDisplayValue").html());
    var value1 = $(obj).attr("data-original-title");
    $("#" + Popup + 'Label').html(value1 + " " + HostingDispVal);
    //$("#" + Popup).modal('show');
    $("#" + dvName).html('Loading..');
    $("#" + dvName + "Error").html('');
    //$("#" + dvName).load(url);
    $("#" + dvName).load(url, function () {
        $("#" + Popup).modal('show');
    });
}
function OpenPopUpCopyEntity(Popup, EntityName, dvName, url) {
    $("#" + Popup + 'Label').html('');
    $("#" + Popup + 'Label').html("Add " + EntityName);
    //$("#" + Popup).modal('show');
    $("#" + dvName).html('Loading..');
    $("#" + dvName + "Error").html('');
    // $("#" + dvName).load(url);
    $("#" + dvName).load(url, function () {
        $("#" + Popup).modal('show');
    });
}
function OpenPopUpEntityMobile(Popup, EntityName, dvName, ddname, url) {
    $("#" + Popup + 'Label').html();
    $("#" + Popup + 'Label').css({ 'color': "black" });
    $("#" + Popup + 'Label').html("Add " + EntityName);
    $("#" + Popup).modal('show');
    $("#" + dvName).html('Loading..');
    $("#" + dvName + "Error").html('');
    $("#" + dvName).load(url);
    $("#" + dvName).attr('accesskey', ddname);
}
function OpenPopUpBulkOperation(Popup, EntityName, DispName, dvName, url) {
    $("#" + Popup).show();//to allow scrolling of bulkupdate
    $("#" + Popup + 'Label').html();
    $("#" + Popup + 'Label').attr('class', EntityName);
    $("#" + Popup + 'Label').html(DispName);
    //$("#" + Popup).modal('show');
    $("#" + dvName).html('Loading..');
    //$("#" + dvName).load(url);
    $("#" + dvName).load(url, function () {
        $("#" + Popup).modal('show');
    });
}
function OpenPopUpBulkOperationBR(Popup, EntityName, DispName, dvName, url) {
    $("#" + Popup + 'Label').html();
    $("#" + Popup + 'Label').attr('class', "SuggestedPropertyValue");
    $("#" + Popup + 'Label').html(DispName);
    //$("#" + Popup).modal('show');
    $("#" + dvName).html('Loading..');
    // $("#" + dvName).load(url);
    $("#" + dvName).load(url, function () {
        $("#" + Popup).modal('show');
    });
}
function QuickAdd(e, EntityName, bisrule, biscount, ruleType, lblerrormsg, isinline, lstinlineassocname, lstinlineassocdispname, lstinlineentityname, type) {
    $(e.currentTarget).attr('disabled', 'disabled');
    var target;
    if (e.srcElement) target = e.srcElement;
    e = $.event.fix(e);
    if (e.currentTarget) target = e.currentTarget;
    e.preventDefault();
    var form = $(target).closest('form');
    var fd = $(target).closest('form').serialize();
    var url = $(target).closest('form').attr("action");
    //var p = BusineesRule(fd, url, bisrule, biscount, form);
    var p = BusineesRule(fd, url, bisrule, biscount, form, ruleType, EntityName, lblerrormsg, isinline, lstinlineassocname, lstinlineassocdispname, lstinlineentityname, type);
    if (!p)
        $(target).removeAttr("disabled");
    if (!form.valid()) { $(target).removeAttr("disabled"); return; }
    SaveServerTimeQuickAdd(form);
    try {
        var fd = new FormData(form[0]);
        $('input[type="file"]').each(function () {
            var file = $('#' + $(this)[0].id)[0].files;
            if (file.length) {
                fd.append($(this)[0].id, file[0]);
            }
        });
        $.ajax({
            url: url + "?IsAddPop=" + true,
            type: "POST",
            cache: false,
            data: fd,
            processData: false,
            contentType: false,
            success: function (result) {
                $(target).removeAttr("disabled");
                if (result == "FROMPOPUP" || result.result == "FROMPOPUP") {
                    form.find('button[aria-hidden="true"]').click();

                    //auto fill dropdown
                    var caller = $(e.currentTarget).attr("caller");
                    if (result.output > 0 && caller != undefined && $("#" + caller).length > 0) {
                        $("#" + caller).trigger("chosen:open");
                        if ($('#' + caller + ' option[value="' + result.output + '"]').length == 0) {
                            $('#' + caller).append($('<option selected=\'selected\'></option>').val(result.output).html(''));
                            $("#" + caller).trigger('chosen:updated');
                        }
                        $('#' + caller).val(result.output);
                        $("#" + caller).trigger('chosen:updated');
                        $("#" + caller).change();
                        $("#" + caller).trigger("click");
                    }
                    //auto fill dropdown

                } else {
                    // $('#dvPopupError').html(result);
                    $("#divDisplayEntitySave").removeAttr("style"); $("#divDisplayEntitySave").html(getMsgTableEntitySave());
                    document.getElementById("ErrMsgEntitySave").innerHTML = result;
                }
            }
        });
    }
    catch (ex) {
        fd = $(target).closest('form').serialize();
        $.ajax({
            url: url + "?IsAddPop=" + true,
            type: "POST",
            cache: false,
            data: fd,
            success: function (result) {
                $(target).removeAttr("disabled");
                if (result == "FROMPOPUP" || result.result == "FROMPOPUP") {
                    form.find('button[aria-hidden="true"]').click();
                    //auto fill dropdown
                    var caller = $(e.currentTarget).attr("caller");
                    if (result.output > 0 && caller != undefined && $("#" + caller).length > 0) {
                        $("#" + caller).trigger("chosen:open");
                        if ($('#' + caller + ' option[value="' + result.output + '"]').length == 0) {
                            $('#' + caller).append($('<option selected=\'selected\'></option>').val(result.output).html(''));
                            $("#" + caller).trigger('chosen:updated');
                        }
                        $('#' + caller).val(result.output);
                        $("#" + caller).trigger('chosen:updated');
                        $("#" + caller).change();
                        $("#" + caller).trigger("click");
                    }
                    //auto fill dropdown
                } else {
                    //$('#dvPopupError').html(result);
                    $("#divDisplayEntitySave").removeAttr("style"); $("#divDisplayEntitySave").html(getMsgTableEntitySave());
                    document.getElementById("ErrMsgEntitySave").innerHTML = result;
                }
            }
        });
    }
    var btnvalue = $(e.currentTarget).attr("btnval");
    if (btnvalue == "createcontinue") {
        //if (result == "FROMPOPUP" || result.result == "FROMPOPUP")
        $('#add' + EntityName).click();
    }
}
function QuickAddMobile(e, bisrule, biscount) {
    var target;
    if (e.srcElement) target = e.srcElement;
    e = $.event.fix(e);
    if (e.currentTarget) target = e.currentTarget;
    e.preventDefault();
    var form = $(target).closest('form');
    var fd = $(target).closest('form').serialize();
    var url = $(target).closest('form').attr("action");
    BusineesRule(fd, url, bisrule, biscount, form)
    if (!form.valid()) return;
    try {
        var fd = new FormData(form[0]);
        $('input[type="file"]').each(function () {
            var file = $('#' + $(this)[0].id)[0].files;
            if (file.length) {
                fd.append($(this)[0].id, file[0]);
            }
        });
        $.ajax({
            url: url + "?IsAddPop=" + true,
            type: "POST",
            cache: false,
            data: fd,
            processData: false,
            contentType: false,
            success: function (result) {
                if (result == "FROMPOPUP" || result.result == "FROMPOPUP") {
                    form.find('button[aria-hidden="true"]').click();
                    FillDropdownMobile(dropdown);
                }
            }
        });
    }
    catch (ex) {
        fd = $(target).closest('form').serialize();
        $.ajax({
            url: url + "?IsAddPop=" + true,
            type: "POST",
            cache: false,
            data: fd,
            success: function (result) {
                if (result == "FROMPOPUP" || result.result == "FROMPOPUP") {
                    form.find('button[aria-hidden="true"]').click();
                    FillDropdownMobile(dropdown);
                }
            }
        });
    }
    var dropdown = ($('#dvPopup').attr('accesskey'));
    if (dropdown != undefined) {
        FillDropdownMobile(dropdown);
        $("#" + dropdown).change();
    }
}
function QuickAddFromIndex(e, isrefresh, Entity, host, bisrule, biscount, ruleType, lblerrormsg, isinline, lstinlineassocname, lstinlineassocdispname, lstinlineentityname, type) {
    // $(e.currentTarget).attr('disabled', 'disabled')
    //

    //BulkDropDownClt(BulkAddDropDown, "ValueForMultiselect")
    $(':input[type="submit"]').attr('disabled', 'disabled');
    var target;
    if (e.srcElement) target = e.srcElement;
    e = $.event.fix(e);
    if (e.currentTarget) target = e.currentTarget;
    e.preventDefault();
    var form = $(target).closest('form');
    var fd = $(target).closest('form').serialize();
    var url = $(target).closest('form').attr("action");
    var p = BusineesRule(fd, url, bisrule, biscount, form, ruleType, Entity, lblerrormsg, isinline, lstinlineassocname, lstinlineassocdispname, lstinlineentityname, type);
    if (!p) {
        $(':input[type="submit"]').removeAttr("disabled");
        //$(target).removeAttr("disabled");
        return false;
    }
    if (!form.valid()) {
        //$(target).removeAttr("disabled"); return;
        $(':input[type="submit"]').removeAttr("disabled"); return;
    }
    SaveServerTimeQuickAdd(form);
    if (BulkAddDropDown.length > 0) {
        LoadAllItem()
    }
    var iscreatecontinue = false;
    var isredirectedit = false;
    var btnvalue = $(e.currentTarget).attr("btnval");
    if (btnvalue == "createcontinue")
        iscreatecontinue = true;
    var redirectedit = $(e.currentTarget).attr("redirectedit");
    if (redirectedit == 'true') isredirectedit = true;

    try {
        fd = new FormData(form[0]);
        $('input[type="file"]').each(function () {
            var file = $('#' + $(this)[0].id)[0].files;
            if (file.length) {
                //Commented because it adds same file twice
                //fd.append($(this)[0].id, file[0]); 
            }
        });
        $.ajax({
            url: url + "?IsAddPop=" + true + "&BulkAddDropDown=" + BulkAddDropDown,
            type: "POST",
            cache: false,
            data: fd,
            dataType: "json",
            async: !iscreatecontinue,
            processData: false,
            contentType: false,
            success: function (result) {
                //$(target).removeAttr("disabled");
                $(':input[type="submit"]').removeAttr("disabled");
                if (result == "FROMPOPUP" || result.result == "FROMPOPUP") {
                    if (isredirectedit)
                        window.location.replace(result.editurl);
                    if (!iscreatecontinue)
                        form.find('button[aria-hidden="true"]').click();
                    if (isrefresh) {
                        if (host != undefined && host.length > 0 && $('#' + host).length > 0) {
                            BulkAddDropDown = "";
                            $('#' + Entity + 'SearchCancel', $('#' + host)).click();
                            $('#dvcnt_' + host).load(location.href + " #dvcnt_" + host);
                            if ($('#' + Entity + 'SearchCancel', $('#' + host)).length <= 0) {
                                window.location.reload();
                            }
                        }
                        else {
                            $('#' + Entity + 'SearchCancel').click();
                            try {
                                //for kanban
                                var hostid = ($(':hidden#HostingEntityID').val());
                                $('#' + Entity + 'SearchCancel', $('#' + host + hostid)).click();
                            } catch (ex) { }
                        }
                    }
                } else {
                    iscreatecontinue = false; OpenAlertPopUp("Record not Added.", result);
                    //$('#dvPopupError').html(result);
                    //$("#divDisplayEntitySave", '#addPopup').removeAttr("style"); $("#divDisplayEntitySave", '#addPopup').html(getMsgTableEntitySave());
                    //document.getElementById("ErrMsgEntitySave", '#addPopup').innerHTML = result;
                }
            }
        });
    } catch (ex) {
        //$(target).removeAttr("disabled");
        $(':input[type="submit"]').removeAttr("disabled");
        fd = $(target).closest('form').serialize();
        $.ajax({
            url: url + "?IsAddPop=" + true + "&BulkAddDropDown=" + BulkAddDropDown,
            type: "POST",
            cache: false,
            async: !iscreatecontinue,
            data: fd,
            success: function (result) {
                if (result == "FROMPOPUP" || result.result == "FROMPOPUP") {
                    if (isredirectedit)
                        window.location.replace(result.editurl);
                    if (!iscreatecontinue)
                        form.find('button[aria-hidden="true"]').click();
                    if (isrefresh) {
                        if (host != undefined && host.length > 0 && $('#' + host).length > 0) {
                            BulkAddDropDown = "";
                            $('#' + Entity + 'SearchCancel', $('#' + host)).click();
                            $('#dvcnt_' + host).load(location.href + " #dvcnt_" + host);
                            if ($('#' + Entity + 'SearchCancel', $('#' + host)).length <= 0) {
                                window.location.reload();
                            }
                        }
                        else
                            $('#' + Entity + 'SearchCancel').click();
                    }
                } else {
                    iscreatecontinue = false;
                    // $('#dvPopupError').html(result);
                    $("#divDisplayEntitySave", '#addPopup').removeAttr("style"); $("#divDisplayEntitySave", '#addPopup').html(getMsgTableEntitySave());
                    document.getElementById("ErrMsgEntitySave", '#addPopup').innerHTML = result;
                }
            }
        });
    }
    if (Entity.indexOf('Events') > -1) {
        location.reload();
    }

    if (iscreatecontinue) {
        //if (result == "FROMPOPUP" || result.result == "FROMPOPUP")
        $('#add' + Entity).click();
        $("body").addClass("modal-open");
    }

}
function BusineesRule(fd, url, bisrule, bisCount, form, ruleType, entityname, lblerrormsg, isinline, lstinlineassocname, lstinlineassocdispname, lstinlineentityname, type) {
    var BruleUrlMandatory = url;
    var BruleUrlValidate = url;
    var BruleUrl = url;
    if (url.indexOf("CreateQuick") >= 0) {
        BruleUrl = url.replace("CreateQuick", "businessruletype");
        BruleUrl = addParameterToURL(BruleUrl, "ruleType", ruleType);
    }
    else if (url.indexOf("EditQuick") >= 0) {
        BruleUrl = url.replace("EditQuick", "businessruletype");
        BruleUrl = addParameterToURL(BruleUrl, "ruleType", ruleType);
    }
    var flag = true;
    if (bisrule != null && bisCount > 0) {
        flag = ApplyBusinessRuleOnSubmit(BruleUrl, entityname, isinline, lblerrormsg, fd, type, "", "");
    }
    if (lstinlineassocname != "" && lstinlineassocname != null) {
        lstinlineassocname = lstinlineassocname.trim(',');
        var arrassoc = lstinlineassocname.split(',');
        lstinlineassocdispname = lstinlineassocdispname.trim(',');
        var arrassocdispname = lstinlineassocdispname.split(',');
        lstinlineentityname = lstinlineentityname.trim(',');
        var arrassocentityname = lstinlineentityname.split(',');
        $.each(arrassoc, function (index, value) {
            var formdiv = form.find('#dv' + value + 'ID :input').serialize();
            formdiv = formdiv.replaceAll(value.toLowerCase() + ".", "");
            flag = flag && ApplyBusinessRuleOnSubmit(BruleUrl.replace(entityname, arrassocentityname[index]), arrassocentityname[index], true, lblerrormsg, formdiv, type, value, arrassocdispname[index]);
        });
    }
    return flag;
}
function getHostingEntityID(url) {
    var vars = [], hash;
    var hashes = url.slice(url.indexOf('?') + 1).split('&');
    for (var i = 0; i < hashes.length; i++) {
        hash = hashes[i].split('=');
        if ($.inArray(hash[0], vars) > -1) {
            vars[hash[0]] += "," + hash[1];
        }
        else {
            vars.push(hash[0]);
            vars[hash[0]] = hash[1];
        }
    }
    return vars;
}
function EntityFilter(EntityName, url, dataurl, UserName) {
    var _username = UserName;
    UserName = (encodeURI(UserName));
    if ($.cookie("pagination" + _username + EntityName) != null)
        $.removeCookie("pagination" + _username + EntityName);
    var FilterHostingEntityID = (dataurl.indexOf("FirstCall=True") > 0) ? undefined : getHostingEntityID(dataurl)["FilterHostingEntityID"];
    var IsWorkFlow = getHostingEntityID(dataurl)["IsWorkFlow"] == undefined ? false : true;
    var html = "<ul class=\"nav nav-tabs\" style=\"margin-bottom:5px;\">";
    var htmlOtheri = "<i class='fa fa-ellipsis-v' aria-hidden='true' style='padding: 12px;color: black;' title='View More Tabs'></i>";
    var otherhtml = "<li class='nav-item dropdown'>";
    otherhtml += "<a class=\"hidden\" id=\"hiddendatatoggle\" data-toggle=\"tab\"></a>";
    otherhtml += "<a  aria-expanded='false' href='#' data-toggle='dropdown' class='dropbtn'>" + htmlOtheri + "</a>";
    //otherhtml +="<a onclick=\"$('#hiddendatatoggle').click();\" data-original-title=\"Filter-Groupby\" data-toggle=\"dropdown\" href=\"#\" class=\"nav-link\"> <span id=\"filtertabOther\">Other</span></a>";
    otherhtml += "<ul class=\"dropdown-menu\" role=\"menu\" style=\"max-height: 250px;overflow: auto;padding: 1px; top: 0px;left: 0px;\">";
    $.ajax({
        url: url,
        type: "GET",
        cache: false,
        dataType: "json",
        async: false,
        complete: function (result) { ClickFilterTabBtn(); },
        success: function (result) {
            var firstClick = "";
            var isother = false;
            for (i in result) {
                if (result[i].Id == undefined) continue;
                var isactive = false;
                if (FilterHostingEntityID != undefined) {
                    if (result[i].Id == FilterHostingEntityID) {
                        if (i < 8)
                            html += "<li name=\"" + result[i].Id + "\" class=\"nav-item\">";
                        else {
                            otherhtml += "<li name=\"" + result[i].Id + "\" class=\"nav-item\">";
                            isother = true;
                        }
                        isactive = true;
                    }
                    else {
                        if (i < 8)
                            html += "<li name=\"" + result[i].Id + "\" class=\"nav-item\">";
                        else {
                            otherhtml += "<li  name=\"" + result[i].Id + "\" class=\"nav-item\">";
                            isother = true;
                        }
                    }
                } else {
                    if (i == 0) {
                        html += "<li name=\"" + result[i].Id + "\" class=\"nav-item\">";
                        isactive = true;
                    }
                    else {
                        if (i < 8)
                            html += "<li name=\"" + result[i].Id + "\" class=\"nav-item\">";
                        else {
                            otherhtml += "<li name=\"" + result[i].Id + "\" class=\"nav-item\">";
                            isother = true;
                        }
                    }
                }
                if (isother) {
                    if (IsWorkFlow)
                        otherhtml += "<a title='" + result[i].Name + "' style='border-radius: 0; margin: 1px; width: 150px; text-align: left;white-space: nowrap; text-overflow: ellipsis; overflow: hidden;' id=\"flt" + result[i].Id + "\" onclick=\"SetCookieFltTab('flt" + result[i].Id + "');$('#filtertabOther').html('Other-" + result[i].Name + "');CancelSearch('" + EntityName + "','" + dataurl + "&HostingEntityID=" + result[i].Id + "&Wfsearch=" + result[i].Name + "')\" class=\"nav-link \">" + result[i].Name + "</a>";
                    else
                        otherhtml += "<a title='" + result[i].Name + "' style='border-radius: 0; margin: 1px; width: 150px; text-align: left;white-space: nowrap; text-overflow: ellipsis; overflow: hidden;' id=\"flt" + result[i].Id + "\" onclick=\"SetCookieFltTab('flt" + result[i].Id + "');$('#filtertabOther').html('Other-" + result[i].Name + "');CancelSearch('" + EntityName + "','" + dataurl + "&HostingEntityID=" + result[i].Id + "')\" class=\"nav-link \">" + result[i].Name + "</a>";
                }
                else {
                    if (IsWorkFlow) {
                        html += "<a title='" + result[i].Name + "' style='white-space: nowrap; text-overflow: ellipsis; overflow: hidden; width: 115px; text-align: center;' id=\"flt" + result[i].Id + "\" data-toggle=\"tab\"  onclick=\"SetCookieFltTab('flt" + result[i].Id + "');CancelSearch('" + EntityName + "','" + dataurl + "&HostingEntityID=" + result[i].Id + "&Wfsearch=" + result[i].Name + "','" + UserName + "')\" class=\"nav-link \">" + result[i].Name + "</a>";
                    }
                    else
                        html += "<a title='" + result[i].Name + "' style='white-space: nowrap; text-overflow: ellipsis; overflow: hidden; width: 115px; text-align: center;' id=\"flt" + result[i].Id + "\" data-toggle=\"tab\"  onclick=\"SetCookieFltTab('flt" + result[i].Id + "');CancelSearch('" + EntityName + "','" + dataurl + "&HostingEntityID=" + result[i].Id + "','" + UserName + "')\" class=\"nav-link\">" + result[i].Name + "</a>";
                }
                html += "</li>";
                if (isother)
                    otherhtml += "</li>";
                if (isactive) {
                    if (IsWorkFlow) {
                        firstClick = dataurl + "&HostingEntityID=" + result[i].Id + "&Wfsearch=" + result[i].Name;
                    }
                    else
                        firstClick = dataurl + "&HostingEntityID=" + result[i].Id;
                }
            }
            html += "<li class=\"nav-item\" name=\"" + "null" + "\">";
            html += "<a class=\"nav-link\" data-toggle=\"tab\" id='None'  onclick=\"SetCookieFltTab('None');CancelSearch('" + EntityName + "','" + dataurl + "','" + UserName + "')\">" + "None" + "</a>";
            html += "</li>";
            //
            if (isother) {
                otherhtml += "</ul></li>";
                html += "<li>";
                html += otherhtml;
                html += "</li>";
            }
            //
            html += "</ul>";
            $("#dv" + EntityName + "Filter").html($(html));
            if (firstClick.length > 0 && firstClick.indexOf("FirstCall=True") > 0) {
                CancelSearch(EntityName, firstClick.replace("FirstCall=True", ""));
            }
        }
    });
}
//Entity Filter For BizRule
function EntityFilterBizRuleDisable(EntityName, dataurl, UserName) {
    var html = "<ul class=\"nav nav-tabs\">" +
        "<li name='false' class='nav-item'>" +
        "<a data-toggle='tab'  onclick=\"CancelSearchBizRuleDisable('" + EntityName + "','" + dataurl + "&IsDisable=False','" + UserName + "')\" class='nav-link'>Enabled</a></li>" +
        "<li class='nav-item' name='true'>" +
        "<a data-toggle='tab'  onclick=\"CancelSearchBizRuleDisable('" + EntityName + "','" + dataurl + "&IsDisable=True','" + UserName + "')\" class='nav-link'>Disabled</a></li>";
    //"<li name='null'><a data-toggle='tab' onclick=\"CancelSearchBizRuleDisable('" + EntityName + "','" + dataurl + "','" + UserName + "')\">All Record</a></li></ul>";
    $("#dv" + EntityName + "Filter").html($(html));
    var isactive = true;
    if (isactive) {
        firstClick = dataurl + "&IsDisable=false";
    }
    if (firstClick.length > 0 && firstClick.indexOf("FirstCall=True") > 0) {
        CancelSearchBizRuleDisable(EntityName, firstClick.replace("FirstCall=True&", ""));
    }
}
//Rule Action
function EntityFilterBizRuleAction(EntityName, url, dataurl, UserName) {
    var _username = UserName;
    UserName = (encodeURI(UserName));
    if ($.cookie("pagination" + _username + EntityName) != null)
        $.removeCookie("pagination" + _username + EntityName);
    var FilterHostingEntityID = (dataurl.indexOf("FirstCall=True") > 0) ? undefined : getHostingEntityID(dataurl)["FilterHostingEntityID"];
    var html = "<ul class=\"nav nav-tabs\">";
    var otherhtml = "<li class='nav-item'><a class=\"hidden\" id=\"hiddendatatoggle\" data-toggle=\"tab\"></a><a onclick=\"$('#hiddendatatoggle').click();\" data-original-title=\"Filter-Groupby\" data-toggle=\"dropdown\" href=\"#\" class=\"nav-link\"> <span id=\"filtertabOther\">Other</span></a>";
    otherhtml += "<ul class=\"dropdown-menu\" role=\"menu\" style=\"max-height: 400px;overflow-y: auto;\">";
    $.ajax({
        url: url,
        type: "GET",
        cache: false,
        dataType: "json",
        success: function (result) {
            var firstClick = "";
            var isother = false;
            var i = 0;
            $.each(result, function (key, value) {
                var isactive = false;
                if (i == 0) {
                    html += "<li name=\"" + key + "\" class=\"nav-item\">";
                    isactive = true;
                }
                else {
                    if (i < 10)
                        html += "<li name=\"" + key + "\" class=\"nav-item\">";
                    else {
                        otherhtml += "<li name=\"" + key + "\" class=\"nav-item\">";
                        isother = true;
                    }
                }
                if (isother) {
                    otherhtml += "<a onclick=\"$('#filtertabOther').html('Other-" + value + "');CancelSearchBizRule('" + EntityName + "','" + dataurl + "&HostingEntity=RuleAction&HostingEntityID=" + key + "')\" class=\"nav-link\">" + value + "</a>";
                }
                else {
                    html += "<a data-toggle=\"tab\"  onclick=\"CancelSearchBizRule('" + EntityName + "','" + dataurl + "&HostingEntity=RuleAction&HostingEntityID=" + key + "','" + UserName + "')\"class=\"nav-link\">" + value + "</a>";
                }
                html += "</li>";
                if (isother)
                    otherhtml += "</li>";
                if (isactive) {
                    firstClick = dataurl + "&HostingEntity=RuleAction&HostingEntityID=" + key;
                }
                i += 1;
            });
            //html += "<li name=\"" + "null" + "\">";
            //html += "<a data-toggle=\"tab\"  onclick=\"CancelSearchBizRule('" + EntityName + "','" + dataurl + "','" + UserName + "')\">" + "All Record" + "</a>";
            //html += "</li>";
            //
            if (isother) {
                otherhtml += "</ul></li>";
                html += "<li>";
                html += otherhtml;
                html += "</li>";
            }
            //
            html += "</ul>";
            $("#dv" + EntityName + "Filter").html($(html));
            if (firstClick.length > 0 && firstClick.indexOf("FirstCall=True") > 0) {
                CancelSearchBizRule(EntityName, firstClick.replace("FirstCall=true&", ""));
            }
        }
    });
}

function CancelSearchBizRuleDisable(dvName, url, UserName) {
    //remove pagination cookies 
    if ($.cookie("pagination" + UserName + dvName) != null)
        $.removeCookie("pagination" + UserName + dvName);
    var IsFilter = (getHostingEntityID(url)["IsFilter"]);
    $.ajax({
        url: url,
        cache: false,
        success: function (data) {
            if (data != null) {
                try {
                    $('#' + dvName).html(data);
                } catch (ex) { }
            }
        }
    })
    return false;
}
function EntityFilterBizRule(EntityName, url, dataurl, UserName) {
    var _username = UserName;
    UserName = (encodeURI(UserName));
    if ($.cookie("pagination" + _username + EntityName) != null)
        $.removeCookie("pagination" + _username + EntityName);
    var FilterHostingEntityID = (dataurl.indexOf("FirstCall=True") > 0) ? undefined : getHostingEntityID(dataurl)["FilterHostingEntityID"];
    var html = "<ul class=\"nav nav-tabs\">";
    var otherhtml = "<li class='nav-item'><a class=\"hidden\" id=\"hiddendatatoggle\" data-toggle=\"tab\"></a><a onclick=\"$('#hiddendatatoggle').click();\" data-original-title=\"Filter-Groupby\" data-toggle=\"dropdown\" href=\"#\" class=\"nav-link\"> <span id=\"filtertabOther\">Other</span></a>";
    otherhtml += "<ul class=\"dropdown-menu\" role=\"menu\" style=\"max-height: 400px;overflow-y: auto;\">";
    $.ajax({
        url: url,
        type: "GET",
        cache: false,
        dataType: "json",
        success: function (result) {
            var firstClick = "";
            var isother = false;
            var i = 0;
            $.each(result, function (key, value) {
                var isactive = false;
                if (i == 0) {
                    html += "<li name=\"" + key + "\" class=\"nav-item\">";
                    isactive = true;
                }
                else {
                    if (i < 10)
                        html += "<li name=\"" + key + "\" class=\"nav-item\">";
                    else {
                        otherhtml += "<li name=\"" + key + "\" class=\"nav-item\">";
                        isother = true;
                    }
                }
                if (isother) {
                    otherhtml += "<a onclick=\"$('#filtertabOther').html('Other-" + value + "');CancelSearchBizRule('" + EntityName + "','" + dataurl + "&HostingEntityName=" + key + "')\" class=\"nav-link\">" + value + "</a>";
                }
                else {
                    html += "<a data-toggle=\"tab\"  onclick=\"CancelSearchBizRule('" + EntityName + "','" + dataurl + "&HostingEntityName=" + key + "','" + UserName + "')\"class=\"nav-link\">" + value + "</a>";
                }
                html += "</li>";
                if (isother)
                    otherhtml += "</li>";
                if (isactive) {
                    firstClick = dataurl + "&HostingEntityName=" + key;
                }
                i += 1;
            });
            //html += "<li name=\"" + "null" + "\">";
            //html += "<a data-toggle=\"tab\"  onclick=\"CancelSearchBizRule('" + EntityName + "','" + dataurl + "','" + UserName + "')\">" + "All Record" + "</a>";
            //html += "</li>";
            //
            if (isother) {
                otherhtml += "</ul></li>";
                html += "<li>";
                html += otherhtml;
                html += "</li>";
            }
            //
            html += "</ul>";
            $("#dv" + EntityName + "Filter").html($(html));
            if (firstClick.length > 0 && firstClick.indexOf("FirstCall=True") > 0) {
                CancelSearchBizRule(EntityName, firstClick.replace("FirstCall=true&", ""));
            }
        }
    });
}
//Refresh index list 
function CancelSearchBizRule(dvName, url, UserName) {
    //remove pagination cookies 
    if ($.cookie("pagination" + UserName + dvName) != null)
        $.removeCookie("pagination" + UserName + dvName);
    var IsFilter = (getHostingEntityID(url)["IsFilter"]);
    $.ajax({
        url: url,
        cache: false,
        success: function (data) {
            if (data != null) {
                try {
                    $('#' + dvName).html(data);
                } catch (ex) { }
            }
        }
    })
    return false;
}
//
function cancelQuickAdd() {
    $("#CancelQuickAdd").click();
}
function Associate(e, entityName) {
    var id = $(e.target).attr("id");
    var moveRight = "MoveRight" + entityName;
    var idAvailable = "#" + entityName + "IDAvailable";
    var idSelected = "#" + entityName + "IDSelected";
    var selectFrom = id == moveRight ? idAvailable : idSelected;
    var moveTo = id == moveRight ? idSelected : idAvailable;
    var selectedItems = $(selectFrom + " :selected").toArray();
    $(moveTo).append(selectedItems);
    selectedItems.remove;
    $(idSelected + " option").prop("selected", "selected");
    $("#Cnt" + entityName + "IDSelected").html($(idSelected + " :selected").length);
}
function SearchList(id, val, selectCtrl) {
    if (val == "") {
        if (navigator.userAgent.indexOf('Trident') != -1 || navigator.userAgent.indexOf('MSIE') != -1) {
            $("#" + selectCtrl + " option").each(function () {
                if (this.nodeName.toUpperCase() === 'OPTION') {
                    var span = $(this).parent();
                    var opt = this;
                    if ($("#" + selectCtrl + " option[value='" + this.value + "']").parent().is('span')) {
                        $(opt).show();
                        $(span).replaceWith(opt);
                    }
                }
            });
        } else {
            $('#' + selectCtrl).find("option").show();
        }
    } else {
        $("#" + selectCtrl + " option").each(function () {
            if (!(this.text.toLowerCase().match(val.toLowerCase()))) {
                if ($("#" + selectCtrl + " option[value='" + this.value + "']").is('option') && (!$("#" + selectCtrl + " option[value='" + this.value + "']").parent().is('span')))
                    $(this).wrap((navigator.userAgent.indexOf('Trident') != -1 || navigator.userAgent.indexOf('MSIE') != -1) ? '<span>' : null).hide();
            }
            else {
                if (navigator.userAgent.indexOf('Trident') != -1 || navigator.userAgent.indexOf('MSIE') != -1) {
                    if (this.nodeName.toUpperCase() === 'OPTION') {
                        var span = $(this).parent();
                        var opt = this;
                        if ($("#" + selectCtrl + " option[value='" + this.value + "']").parent().is('span')) {
                            $(opt).show();
                            $(span).replaceWith(opt);
                        }
                    }
                } else {
                    $("#" + selectCtrl + " option[value='" + this.value + "']").show(); //all other browsers use standard .show()
                }
            }
        });
    }
}
function SearchListNew(obj, searchstring, hostingentityname, hostingentityid, associatedtype, selectCtrl) {
    var srchstring = $(obj).val();
    var url1 = ($(obj).attr("dataurl"));
    url1 = addParameterToURL(url1, "SearchString", searchstring);
    url1 = addParameterToURL(url1, "HostingEntityName", hostingentityname);
    url1 = addParameterToURL(url1, "HostingEntityID", hostingentityid);
    url1 = addParameterToURL(url1, "AssociatedType", associatedtype);
    $.ajax({
        async: false,
        type: "GET",
        url: url1,
        success: function (data) {
            searchstr = searchstring;
            $('#' + selectCtrl + ' > option[val!=""]').remove();
            $.each(data.data, function (index, value) {
                $('#' + selectCtrl).append('<option value="' + value.Value + '">' + value.Text + '</option>');
            });
            if (data.Results)
                $('#lbl' + selectCtrl).css('display', 'block');
            else
                $('#lbl' + selectCtrl).css('display', 'none');
        },
        error: function (jqXHR, textStatus, errorThrown) {
            //alert(errorThrown + '-' + textStatus);
        }
    });
}
function SearchListNew1(obj, dataurl, searchstring, hostingentityname, hostingentityid, associatedtype, selectCtrl, Asso) {
    var srchstring = $(obj).val();
    var url1 = dataurl;
    url1 = addParameterToURL(url1, "SearchString", searchstring);
    url1 = addParameterToURL(url1, "HostingEntityName", hostingentityname);
    url1 = addParameterToURL(url1, "HostingEntityID", hostingentityid);
    url1 = addParameterToURL(url1, "AssociatedType", associatedtype);
    var association = Asso.split(",");
    var firstparam = 0;
    if (Asso.length > 0) {
        for (i = 0; i < association.length; i++) {
            var vals = "";
            ele = $('select[name=' + association[i] + ']')[0];
            if (ele == undefined)
                ele = document.getElementById(association[i]);
            if (ele != null)
                if (ele.options != undefined) {
                    for (var o = 0; o < ele.options.length; o++) {
                        if (ele.options[o].selected)
                            vals += ele.options[o].value + ",";
                    }
                }
            vals = vals.replace(/^,|,$/g, '');
            if (vals.length > 0) {
                //url1 += "&" + association[i] + "=" + vals;
                url1 = addParameterToURL(url1, association[i], vals);
            }
        }
    }
    $.ajax({
        async: false,
        type: "GET",
        url: url1,
        success: function (data) {
            searchstr = searchstring;
            $('#' + selectCtrl + ' > option[val!=""]').remove();
            if (data.data != null && data.data != undefined) {
                $.each(data.data, function (index, value) {
                    $('#' + selectCtrl).append('<option value="' + value.Value + '">' + value.Text + '</option>');
                });
                if (data.Results)
                    $('#lbl' + selectCtrl).css('display', 'block');
                else
                    $('#lbl' + selectCtrl).css('display', 'none');
            }
        },
        error: function (jqXHR, textStatus, errorThrown) {
            //alert(errorThrown + '-' + textStatus);
        }
    });
}
function check1MThresholdValueQuickAdd(e, entityName) {
    var target;
    if (e.srcElement) target = e.srcElement;
    e = $.event.fix(e);
    if (e.currentTarget) target = e.currentTarget;
    e.preventDefault();
    var form = $(target).closest('form');
    var fd = $(target).closest('form').serialize();
    var url = $(target).closest('form').attr("action");
    url = url.replace("CreateQuick", "Check1MThresholdValue");
    $("#errors").html("");
    $("#errorsMsg").html("");
    var flag = true;
    flag = Check1MThresholdLimit(e, fd, url, entityName, "ErrMsgQuickAdd");
    return flag;
}
function check1MThresholdValue(e, entityName) {
    var target;
    if (e.srcElement) target = e.srcElement;
    e = $.event.fix(e);
    if (e.currentTarget) target = e.currentTarget;
    e.preventDefault();
    var form = $(target).closest('form');
    var fd = $(target).closest('form').serialize();
    var url = $(target).closest('form').attr("action");
    url = url.replace("CreateQuick", "Check1MThresholdValue");
    $("#errors").html("");
    $("#errorsMsg").html("");
    var flag = true;
    flag = Check1MThresholdLimit(e, fd, url, entityName, "ErrMsg");
    return flag;
}
// Radio Button List
function ClearChildRBDD(child, obj) {
    var ctrlId = $(obj).attr("id");
    var ctrlName = $(obj).attr("name").substr(2, $(obj).attr("name").length - 1);
    var childCtrlId = child.split(',');
    var firstChildCtrlId = "";
    var firstChildCtrlName = "";
    var isRequired = "";
    $.each(childCtrlId, function (i, ctrl) {
        if (i == 0) {
            firstChildCtrlId = ctrl.replace('.', '_');
            firstChildCtrlName = ctrl;
        }
        ctrl = ctrl.replace('.', '_');
        if ($('#ul' + ctrl).find('li').length > 0) {
            $('#ul' + ctrl).html("");
        }
    });
    if (firstChildCtrlId != "") {
        var selectedval = $("input:radio[name='" + ctrlName + "']:checked").val();
        isRequired = $("#ul" + firstChildCtrlId).attr("required") == "required";
        var finalUrl = $("#ul" + firstChildCtrlId).attr("dataurl");
        var AssociationName = $("#ul" + firstChildCtrlId).attr("AssoNameWithParent");
        if (AssociationName.length > 0)
            finalUrl = addParameterToURL(finalUrl, 'AssoNameWithParent', AssociationName);
        if (selectedval.length > 0)
            finalUrl = addParameterToURL(finalUrl, 'AssociationID', selectedval);
    }
    $.ajax({
        type: "GET",
        url: finalUrl,
        contentType: "application/json; charset=utf-8",
        global: false,
        async: false,
        cache: false,
        dataType: "json",
        success: function (jsonData) {
            var listItems = "";
            var tagrequired = isRequired ? "required='required'" : "";
            $.each(jsonData, function (i, ctrlValue) {
                listItems += "<li style='list-style-type:none;'><label><input name='" + firstChildCtrlName + "' " + tagrequired + " type='radio' value='" + ctrlValue.Id + "'>";
                listItems += " <span>" + ctrlValue.Name + "</span>";
                listItems += "</label></li>";
            });
            if (listItems == "") {
                listItems += "<input name='" + firstChildCtrlName + "' " + tagrequired + " style='width:0px; height:0px; border:0px solid #fff !important;' type='text'>";
            }
            $("#ul" + firstChildCtrlId).html(listItems);
        }
    });
}
//
// Inline Add Edit Cancel
function quickEdit(e, entityname, url, form) {
    var thelink = url;
    $.ajax({
        async: false,
        type: "GET",
        contentType: "application/json; charset=utf-8",
        url: url,
        success: function (data) {
            var responcedata = data;
            var trEdit = $("#Des_Table").find('table').find('tr:eq(1)');
            $(trEdit).find('td').each(function (index) {
                gettagType($(this), responcedata);
            });
            var formacn = $("#" + form).attr("action").substr(0, $("#" + form).attr("action").lastIndexOf("/") + 1);
            $("#" + form).attr("action", formacn + "EditQuick");
            $("#" + form).append("<input type='hidden' name='Id' value='" + responcedata["Id"] + "' />")
            $("#" + form).append("<input type='hidden' name='ConcurrencyKey' value='" + responcedata["ConcurrencyKeyBase64"] + "' />");
            $("#quickAdd" + entityname).hide();
            $("#quickEdit" + entityname).show();
            $("#quickCancel" + entityname).show();
        },
        error: function (jqXHR, textStatus, errorThrown) {
            alert(errorThrown + '-' + textStatus);
        }
    });
}
function setValueInControl(control, data) {
    if (control.length > 0) {
        var controlname = control.attr("name");
        var tagType = control.get(0).tagName;
        var controlType = control.attr('type');

        switch (tagType) {
            case "TEXT":
                control.val(data);
                break;
            case "SELECT":
                control.val(data);
                control.trigger('chosen:updated');
                break;
            case "RADIO":
                $('input[name=' + controlname + '][value=' + data + ']').attr('checked', 'checked');
                break;
            default:
                if (controlType == 'checkbox') {
                    control.prop('checked', data == 'True' ? true : false);
                    break;
                }
                control.val(data);
                break;
        }

    }
}
function gettagType(tdEdit, responcedata) {
    if ($(tdEdit).html() != "") {
        var tag = $(tdEdit).find('input:text, input:file, input:radio, input:checkbox, select, textarea');
        if (tag.length > 0) {
            var tagType = $(tag)[0].type;
            var control = $($(tag)[0]).prop('name');
            valEdit = responcedata[control];
            switch (tagType) {
                case "text":
                    $("#" + control).val(valEdit);
                    break;
                case "select-one":
                    $("#" + control).val(valEdit);
                    $("#" + control).trigger('chosen:updated');
                    break;
                case "radio":
                    $('input[name=' + control + '][value=' + valEdit + ']').attr('checked', 'checked');
                    break;
            }
        }
    }
}
function QuickAddFromGrid(e, isrefresh, Entity, host) {
    var target;
    if (e.srcElement) target = e.srcElement;
    e = $.event.fix(e);
    if (e.currentTarget) target = e.currentTarget;
    e.preventDefault();
    var form = $(target).closest('form');
    var fd = $(target).closest('form').serialize();
    var url = $(target).closest('form').attr("action");
    if (!form.valid()) return;
    try {
        fd = new FormData(form[0]);
        $('input[type="file"]').each(function () {
            var file = $('#' + $(this)[0].id)[0].files;
            if (file.length) {
                fd.append($(this)[0].id, file[0]);
            }
        });
        $.ajax({
            url: url + "?IsAddPop=" + true,
            type: "POST",
            cache: false,
            data: fd,
            dataType: "json",
            processData: false,
            contentType: false,
            success: function (result) {
                if (result == "FROMPOPUP" || result.result == "FROMPOPUP") {
                    form.find('button[aria-hidden="true"]').click();
                    if (isrefresh) {
                        if (host != undefined && host.length > 0 && $('#' + host).length > 0) {
                            $('#' + Entity + 'SearchCancel', $('#' + host)).click();
                            $('#dvcnt_' + host).load(location.href + " #dvcnt_" + host);
                        }
                        else
                            $('#' + Entity + 'SearchCancel').click();
                    }
                } else {
                    // $('#dvPopupError').html(result);
                    $("#divDisplayEntitySave").removeAttr("style"); $("#divDisplayEntitySave").html(getMsgTableEntitySave());
                    document.getElementById("ErrMsgEntitySave").innerHTML = result;
                }
            }
        });
    } catch (ex) {
        fd = $(target).closest('form').serialize();
        $.ajax({
            url: url + "?IsAddPop=" + true,
            type: "POST",
            cache: false,
            data: fd,
            success: function (result) {
                if (result == "FROMPOPUP" || result.result == "FROMPOPUP") {
                    form.find('button[aria-hidden="true"]').click();
                    if (isrefresh) {
                        if (host != undefined && host.length > 0 && $('#' + host).length > 0) {
                            $('#' + Entity + 'SearchCancel', $('#' + host)).click();
                            $('#dvcnt_' + host).load(location.href + " #dvcnt_" + host);
                        }
                        else
                            $('#' + Entity + 'SearchCancel').click();
                    }
                } else {
                    //$('#dvPopupError').html(result);
                    $("#divDisplayEntitySave").removeAttr("style"); $("#divDisplayEntitySave").html(getMsgTableEntitySave());
                    document.getElementById("ErrMsgEntitySave").innerHTML = result;
                }
            }
        });
    }
}
function QuickEditFromGrid(e, isrefresh, Entity, host, IsWf, bisrule, biscount, ruleType, lblerrormsg, isinline, lstinlineassocname, lstinlineassocdispname, lstinlineentityname, type, QuickEdit) {
    if (IsWf) {
        $('input:hidden[name="hdncommand"]').val(e.currentTarget.value);
    }
    //$("textarea.richtext").each(function () {
    //    $(this).val($(this).code());
    //});
    var target;
    if (e.srcElement) target = e.srcElement;
    e = $.event.fix(e);
    if (e.currentTarget) target = e.currentTarget;
    e.preventDefault();
    var form = $(target).closest('form');
    var fd = $(target).closest('form').serialize();
    var url = $(target).closest('form').attr("action");
    var p = BusineesRule(fd, url, bisrule, biscount, form, ruleType, Entity, lblerrormsg, isinline, lstinlineassocname, lstinlineassocdispname, lstinlineentityname, type);
    if (!p) {
        $(target).removeAttr("disabled");
        return false;
    }
    if (!form.valid()) { $(target).removeAttr("disabled"); return; }
    form.find(':input').removeAttr('disabled');
    fd = form.serialize();
    SaveServerTimeQuickEdit(form);
    try {
        fd = new FormData(form[0]);
        $('input[type="file"]').each(function () {
            var file = $('#' + $(this)[0].id)[0].files;
            if (file.length) {
                fd.append($(this)[0].id, file[0]);
            }
        });
        url = addParameterToURL(url, "IsAddPop", true);
        url = addParameterToURL(url, "RenderPartial", true);

        $.ajax({
            url: url,
            type: "POST",
            cache: false,
            data: fd,
            dataType: "json",
            processData: false,
            contentType: false,
            success: function (result) {
                if (result.Result.trim().toLowerCase() == "succeed") {
                    if (QuickEdit == undefined)
                        form.find('button[aria-hidden="true"]').click();
                    if (isrefresh) {
                        if (host != undefined && host.length > 0 && $('#' + host).length > 0) {
                            // $('#' + Entity + 'SearchCancel', $('#' + host)).click();
                            var buttons = $("button[id=" + Entity + 'Refresh' + "],a[id=" + Entity + 'Refresh' + "]");
                            var btnId = $('#' + Entity + 'Refresh', $('#' + host));
                            var buttonsCount = buttons.length;
                            for (var i = 0; i <= buttonsCount; i++) {
                                var btn = buttons[i];
                                if ($(btn).attr("class") != "extra") {
                                    $(btn).click();
                                    break;
                                }
                            }
                            if ($('#' + Entity).length == 0 && buttonsCount == 0) location.reload();
                            $('#dvcnt_' + host).load(location.href + " #dvcnt_" + host);
                        }
                        else
                            $('#' + Entity).load(result.UrlRefr);
                    }
                } else {
                    // $('#dvPopupError').html(result.UrlRefr);

                    $("#divDisplayEntitySave").removeAttr("style"); $("#divDisplayEntitySave").html(getMsgTableEntitySave());
                    document.getElementById("ErrMsgEntitySave").innerHTML = result.UrlRefr;
                    //form.find('button[aria-hidden="true"]').click();
                    //if (isrefresh) {
                    //    CancelSearch(Entity, url, host);
                    //}
                }
            }
        });
    } catch (ex) {
        fd = $(target).closest('form').serialize();
        $.ajax({
            url: url + "?IsAddPop=" + true,
            type: "POST",
            cache: false,
            data: fd,
            success: function (result) {
                if (result == "FROMPOPUP" || result.result == "FROMPOPUP") {
                    if (QuickEdit == undefined)
                        form.find('button[aria-hidden="true"]').click();
                    if (isrefresh) {
                        if (host != undefined && host.length > 0 && $('#' + host).length > 0) {
                            //$('#' + Entity + 'SearchCancel', $('#' + host)).click();
                            var buttons = $("button[id=" + Entity + 'Refresh' + "],a[id=" + Entity + 'SearchCancel' + "]");
                            var btnId = $('#' + Entity + 'Refresh', $('#' + host));
                            var buttonsCount = buttons.length;
                            for (var i = 0; i <= buttonsCount; i++) {
                                var btn = buttons[i];
                                if ($(btn).attr("class") != "extra") {
                                    $(btn).click();
                                    break;
                                }
                            }
                            if ($('#' + Entity).length == 0 && buttonsCount == 0) location.reload();
                            $('#dvcnt_' + host).load(location.href + " #dvcnt_" + host);
                        }
                        else
                            $('#' + Entity + 'Refresh').click();
                    }
                } else {
                    if (QuickEdit == undefined)
                        form.find('button[aria-hidden="true"]').click();
                    if (isrefresh) {
                        $('#' + Entity + 'Refresh').click();
                    }
                }
            }
        });
    }
    //alert(Entity);
    //if (Entity.indexOf('Events') > -1 || Entity == 'T_Schedule') {
    // location.reload();
    //}
}
function QuickCancelFromGrid(e, isrefresh, Entity, host) {
    $('#' + Entity + 'SearchCancel').click();
}
// End
//Default Entity Page Js...
function SelectPageType(selectedval, SelectUrl, DirectUrl, DefaultUrl, Favorites) {
    DirectUrl.removeProp("required");
    SelectUrl.hide();
    DefaultUrl.hide();
    Favorites.hide();
    DirectUrl.hide();
    DefaultUrl.removeAttr("required");
    Favorites.removeAttr("required");
    DirectUrl.removeAttr("required");
    if (selectedval == "Default") {
        DefaultUrl.show();
        SelectUrl.show();
        DefaultUrl.attr("required", "required");
    }
    else if (selectedval == "Favorite") {
        Favorites.show();
        SelectUrl.show();
        Favorites.attr("required", "required");
    } else if (selectedval == "Url") {
        DirectUrl.show();
        SelectUrl.show();
        DirectUrl.attr("required", "required");
    }
}
function SaveDefaultPage(e, selectedval, DefaultUrl, PageUrl, Other, Favorites, DirectUrl, EntityName, RoleList, Roles, HomePages) {

    var target;
    if (e.srcElement) target = e.srcElement;
    e = $.event.fix(e);
    if (e.currentTarget) target = e.currentTarget;
    e.preventDefault();
    var form = $(target).closest('form');
    if (!form.valid()) return;
    if (RoleList.val() != null) {
        Roles.val(RoleList.val());
        if (selectedval == "Default") {
            if (DefaultUrl.val() == undefined || DefaultUrl.val().length == 0) { alert("Default Value Required.."); return false; }
            var defaultselected = DefaultUrl.val();
            var pageurl = '/_ControllerName/_ActionName';
            if (defaultselected == "Home") {
                PageUrl.val(pageurl.replace("_ActionName", EntityName.val() + "Home").replace("_ControllerName", EntityName.val()));
            }

            else
                PageUrl.val(pageurl.replace("_ActionName", defaultselected).replace("_ControllerName", EntityName.val()));
            Other.val($("option:selected", DefaultUrl).text());
        }
        else if (selectedval == "Favorite") {
            if (Favorites.val() == undefined || Favorites.val().length == 0) { alert("Favorites Value Required.."); return false; }
            PageUrl.val(Favorites.val());
            Other.val($("option:selected", Favorites).text());
        } else if (selectedval == "Url") {
            if (DirectUrl.val() == undefined || DirectUrl.val().length == 0) { alert("Url Value Required.."); return false; }
            PageUrl.val(DirectUrl.val());
            Other.val("Link");
        }
        else if (selectedval == "Home") {
            var defaultselected = DefaultUrl.val();
            var homepageVal = HomePages.val();
            var actionName = "";
            if (homepageVal != "Index")
                actionName = homepageVal;
            var pageurl = '/_ControllerName/_ActionName';
            if (defaultselected = "")
                PageUrl.val(pageurl.replace("_ActionName", HomePages).replace("_ControllerName", HomePages.replace("Home", "")));
            else
                PageUrl.val(pageurl.replace("_ActionName", actionName).replace("_ControllerName", "Home"));
            Other.val($("option:selected", DefaultUrl).text());
        }
        else if (selectedval.toLowerCase() == "layouts") {
            PageUrl.val(HomePages.val());
            Other.val($("option:selected", DefaultUrl).val());
        }
        form.submit();
    }
    else {
        alert("Roles Value Required..");
        return false;
    }
}
function BindViewNames(url, cltId) {
    var cltName = $("#" + cltId);
    var dataurl = url;
    $.ajax({
        url: dataurl,
        cache: false,
        success: function (result) {
            var viewNames = result.LayoutNames;
            var listItems = "";
            $.each(viewNames, function (key, value) {
                listItems += "<option value='" + key + "'>" + value + "</option>";
            });
            cltName.append(listItems);
            //
        }
    })
}
function BindPageNamesofLayouts(thisid, url, cltId) {
    var cltName = $("#" + cltId);
    var dataurl = url + "?viewname=" + $("#" + thisid + " option:selected").text() + "&pagename=" + $("#EntityName").val();
    $.ajax({
        url: dataurl,
        cache: false,
        success: function (result) {
            var viewNames = result.Layoutpages;
            var listItems = "";
            $("#" + cltId).empty();
            $.each(viewNames, function (key, value) {
                listItems += "<option value='" + key + "'>" + value + "</option>";
            });
            cltName.append(listItems);
            //
        }
    })
}
//
function GetCalculationValueOnLoad(frm, url) {
    var form = frm;
    // var url = $(target).closest('form').attr("action");
    try {
        fd = frm.serialize();
        fd.push({ name: "__RequestVerificationToken", value: frm.find('input[name=__RequestVerificationToken]').val() });
        $.ajax({
            url: url,
            type: "POST",
            cache: false,
            data: fd,
            //dataType: "json",
            //processData: false,
            //contentType: false,
            async: false,
            success: function (result) {
                $.each(result, function (key, value) {
                    var format = $("#" + key).attr("format");
                    if (format != undefined) {
                        var keyObj = $("#" + key);
                        if (format == undefined) {
                            value = moment(value).format("MM/DD/YYYY");
                            $("#" + key).val(value);
                        }
                        else {
                            if (format.trim() == "HH:mm".trim() || format.trim() == "hh:mm".trim()) {
                                if (format.trim() == "hh:mm".trim())
                                    value = moment(value).format(format + " A");
                                else
                                    value = moment(value).format(format);
                            }
                            LoadDateTimeByFormat($("#" + key).attr("format"), value, $("#" + key))
                            if (value.length == 0)
                                $("#" + key).val('');
                        }
                    }
                    else {
                        $("#" + key).val(value);
                    }
                })
            }
        });
    } catch (ex) {

    }
}
function GetCalculationValue(e, url) {
    var target;
    if (e.srcElement) target = e.srcElement;
    e = $.event.fix(e);
    if (e.currentTarget) target = e.currentTarget;
    // e.preventDefault();
    var form = $(target).closest('form');
    var fd = $(target).closest('form').serialize();
    // var url = $(target).closest('form').attr("action");
    try {
        fd = new FormData(form[0]);
        $.ajax({
            url: url,
            type: "POST",
            cache: false,
            data: fd,
            dataType: "json",
            processData: false,
            contentType: false,
            async: false,
            success: function (result) {
                $.each(result, function (key, value) {
                    var format = $("#" + key).attr("format");
                    if (format != undefined) {
                        var keyObj = $("#" + key);
                        if (format == undefined) {
                            value = moment(value).format("MM/DD/YYYY");
                            $("#" + key).val(value);
                        }
                        else {
                            if (format.trim() == "HH:mm".trim() || format.trim() == "hh:mm".trim()) {
                                if (format.trim() == "hh:mm".trim())
                                    value = moment(value).format(format + " A");
                                else
                                    value = moment(value).format(format);
                            }
                            LoadDateTimeByFormat($("#" + key).attr("format"), value, $("#" + key))
                            if (value.length == 0)
                                $("#" + key).val('');
                        }
                    }
                    else {
                        $("#" + key).val(value);
                    }
                    //$("#" + key).val(value);
                })
            }
        });
    } catch (ex) {
        fd = $(target).closest('form').serialize();
        $.ajax({
            url: url,
            type: "POST",
            cache: false,
            data: fd,
            dataType: "json",
            processData: false,
            contentType: false,
            success: function (result) {
                $.each(result, function (key, value) {
                    //$("#" + key).val(value);
                    var format = $("#" + key).attr("format");
                    if (format != undefined) {
                        var keyObj = $("#" + key);
                        if (format == undefined) {
                            value = moment(value).format("MM/DD/YYYY");
                            $("#" + key).val(value);
                        }
                        else {
                            if (format.trim() == "HH:mm".trim() || format.trim() == "hh:mm".trim()) {
                                if (format.trim() == "hh:mm".trim())
                                    value = moment(value).format(format + " A");
                                else
                                    value = moment(value).format(format);
                            }
                            LoadDateTimeByFormat($("#" + key).attr("format"), value, $("#" + key))
                            if (value.length == 0)
                                $("#" + key).val('');
                        }
                    }
                    else {
                        $("#" + key).val(value);
                    }
                })
            }
        });
    }
}
function FillDerivedDetailsInline(obj, e, suffix, host) {
    var url = $(obj).attr("derivedurl");
    var hostvalue = $(obj).val();
    var target;
    if (e.srcElement) target = e.srcElement;
    e = $.event.fix(e);
    if (e.currentTarget) target = e.currentTarget;
    //if popup
    if (e.target) target = e.target;
    //
    e.preventDefault();
    var form = $(target).closest('form');
    var fd = $(target).closest('form').serialize();
    url = addParameterToURL(url, "host", host);
    url = addParameterToURL(url, "value", hostvalue);
    try {
        fd = new FormData(form[0]);
        //fd $("#dvT_mainlineitemassociationID").find("select, textarea, input").serialize();
        $.ajax({
            url: url,
            type: "POST",
            cache: false,
            data: fd,
            dataType: "json",
            processData: false,
            contentType: false,
            success: function (result) {
                $.each(result, function (key, value) {
                    if (suffix != undefined && suffix.length > 0) {
                        key = suffix + "_" + key;
                    }
                    if ($("#" + key).is('select')) {
                        $("#" + key).trigger("chosen:open");
                        $("#" + key).val(value);
                        $("#" + key).trigger("chosen:updated");
                        $("#" + key).trigger("click.chosen");
                        $("#" + key).trigger("change");
                    }
                    if ($("#" + key).is(":checkbox")) {
                        if (value.toUpperCase() == "TRUE") {
                            document.getElementById(key).checked = true;
                        }
                        else
                            document.getElementById(key).checked = false;
                        $("#" + key).trigger("change");
                    }
                    else {
                        if ($("#datetimepicker" + key) != undefined && $("#datetimepicker" + key).length > 0 && value != "" && value != undefined) {
                            var format = $("#" + key).attr("format");
                            var keyObj = $("#" + key);
                            if (format == undefined) {
                                value = moment(value).format("MM/DD/YYYY");
                                $("#" + key).val(value);
                            }
                            else {
                                if (format.trim() == "HH:mm".trim() || format.trim() == "hh:mm".trim()) {
                                    if (format.trim() == "hh:mm".trim())
                                        value = moment(value).format(format + " A");
                                    else
                                        value = moment(value).format(format);
                                }
                                LoadDateTimeByFormat($("#" + key).attr("format"), value, $("#" + key))
                            }
                        }
                        else {
                            $("#" + key).val(value);
                        }
                    }
                })
            }
        });
    } catch (ex) {
        fd = $(target).closest('form').serialize();
        $.ajax({
            url: url,
            type: "POST",
            cache: false,
            data: fd,
            dataType: "json",
            processData: false,
            contentType: false,
            success: function (result) {
                $.each(result, function (key, value) {
                    if ($("#" + key).is('select')) {
                        $("#" + key).trigger("chosen:open");
                        $("#" + key).val(value);
                        $("#" + key).trigger("chosen:updated");
                        $("#" + key).trigger("click.chosen");
                        $("#" + key).trigger("change");
                    }
                    if ($("#" + key).is(":checkbox")) {
                        if (value.toUpperCase() == "TRUE") {
                            document.getElementById(key).checked = true;
                        }
                        else
                            document.getElementById(key).checked = false;
                        $("#" + key).trigger("change");
                    }
                    else {
                        if ($("#datetimepicker" + key) != undefined && $("#datetimepicker" + key).length > 0 && value != "" && value != undefined) {
                            var format = $("#" + key).attr("format");
                            var keyObj = $("#" + key);
                            if (format == undefined) {
                                value = moment(value).format("MM/DD/YYYY");
                                $("#" + key).val(value);
                            }
                            else {
                                if (format.trim() == "HH:mm".trim() || format.trim() == "hh:mm".trim()) {
                                    if (format.trim() == "hh:mm".trim())
                                        value = moment(value).format(format + " A");
                                    else
                                        value = moment(value).format(format);
                                }
                                LoadDateTimeByFormat($("#" + key).attr("format"), value, $("#" + key))
                            }
                        }
                        else {
                            $("#" + key).val(value);
                        }
                    }
                })
            }
        });
    }
}
function FillDerivedDetails(obj, e) {
    var url = $(obj).attr("derivedurl");
    var target;
    if (e.srcElement) target = e.srcElement;
    e = $.event.fix(e);
    if (e.currentTarget) target = e.currentTarget;
    //if popup
    if (e.target) target = e.target;
    //
    e.preventDefault();
    var form = $(target).closest('form');
    //alert(form.attr('id'));
    var fd = $(target).closest('form').serialize();
    try {
        fd = new FormData(form[0]);
        $.ajax({
            url: url,
            type: "POST",
            cache: false,
            data: fd,
            dataType: "json",
            async: false,
            processData: false,
            contentType: false,
            success: function (result) {
                $.each(result, function (key, value) {
                    if ($("#" + key, form).is('select')) {
                        $("#" + key, form).removeAttr("lock");
                        $("#" + key, form).trigger("chosen:open");
                        $("#" + key, form).val(value);
                        $("#" + key, form).trigger("chosen:updated");
                        $("#" + key, form).trigger("click.chosen");
                        $("#" + key, form).trigger("change");
                        $("#" + key, form).attr("lock", "true");
                    }
                    else
                        if ($("#" + key, form).is(":checkbox")) {
                            if (value.toUpperCase() == "TRUE") {
                                //document.getElementById(key).checked = true;
                                $("#" + key, form).prop("checked", true);
                            }
                            else {
                                //document.getElementById(key).checked = false;
                                $("#" + key, form).prop("checked", false);
                            }
                            $("#" + key, form).trigger("change");
                        }
                        else {
                            if ($("#datetimepicker" + key).length > 0 && value != "" && value != undefined) {

                                var format = $("#" + key, form).attr("format");
                                var keyObj = $("#" + key, form);
                                if (format == undefined) {
                                    value = moment(value).format("MM/DD/YYYY");
                                    $("#" + key, form).val(value);
                                }
                                else {
                                    if (format.trim() == "HH:mm".trim() || format.trim() == "hh:mm".trim()) {
                                        if (format.trim() == "hh:mm".trim())
                                            value = moment(value).format(format + " A");
                                        else
                                            value = moment(value).format(format);
                                    }
                                    LoadDateTimeByFormat($("#" + key, form).attr("format"), value, $("#" + key, form))
                                    $("#" + key, form).trigger("change");
                                }
                            }
                            else if ($("#" + key, form).is(":file") || $("#File_" + key, form).is(":file")) {
                                var fileClt = $("#File_" + key, form);
                                if (fileClt.length > 0) {
                                    fileClt = $("#File_" + key, form);
                                    var Id = $("#Id", form).val();
                                    downloadClt = $("#adownloadEdit" + key + Id)
                                    if (downloadClt.length == 0) {
                                        Id = "";
                                        downloadClt = $("#adownloadEdit" + key);//.attr('href', downloadlink);
                                    }
                                    var link = fileClt.attr('dataurl') + "?id=" + value;
                                    downloadClt.attr('href', link);
                                    link = link.replace('Download', 'GetDocumentName');
                                    displayDocumentNameForDerived(link, Id, key);
                                    $("#" + key).val(value);
                                }
                                else {
                                    fileClt = $("#" + key, form);
                                    var downloadlink = fileClt.attr('dataurl') + "?id=" + value;
                                    $("#adownloadEdit" + key).attr('href', downloadlink);
                                    downloadlink = downloadlink.replace('Download', 'GetDocumentName');
                                    displayDocumentNameForDerived(downloadlink, '', key);
                                    $("#" + key + "Drv").val(value);

                                }
                            }
                            else {
                                $('#' + key, form).val(value)
                                $('#' + key, form).trigger("change");
                            }
                        }
                })
            }
        });
    } catch (ex) {
        fd = $(target).closest('form').serialize();
        $.ajax({
            url: url,
            type: "POST",
            cache: false,
            data: fd,
            dataType: "json",
            processData: false,
            contentType: false,
            success: function (result) {
                $.each(result, function (key, value) {
                    if ($("#" + key, form).is('select')) {
                        $("#" + key, form).removeAttr("lock");
                        $("#" + key, form).trigger("chosen:open");
                        $("#" + key, form).val(value);
                        $("#" + key, form).trigger("chosen:updated");
                        $("#" + key, form).trigger("click.chosen");
                        $("#" + key, form).trigger("change");
                        $("#" + key, form).attr("lock", "true");
                    }
                    else
                        if ($("#" + key, form).is(":checkbox")) {
                            if (value.toUpperCase() == "TRUE") {
                                //document.getElementById(key).checked = true;
                                $("#" + key, form).prop("checked", true);
                            }
                            else {
                                //document.getElementById(key).checked = false;
                                $("#" + key, form).prop("checked", false);
                            }
                            $("#" + key, form).trigger("change");
                        }
                        else {
                            if ($("#datetimepicker" + key).length > 0 && value != "" && value != undefined) {

                                var format = $("#" + key, form).attr("format");
                                var keyObj = $("#" + key, form);
                                if (format == undefined) {
                                    value = moment(value).format("MM/DD/YYYY");
                                    $("#" + key, form).val(value);
                                }
                                else {
                                    if (format.trim() == "HH:mm".trim() || format.trim() == "hh:mm".trim()) {
                                        if (format.trim() == "hh:mm".trim())
                                            value = moment(value).format(format + " A");
                                        else
                                            value = moment(value).format(format);
                                    }
                                    LoadDateTimeByFormat($("#" + key, form).attr("format"), value, $("#" + key, form))
                                    $("#" + key, form).trigger("change");
                                }
                            }
                            if ($("#" + key, form).is("file")) {
                                alert($("#" + key, form).attr('id'))
                            }
                            else {
                                $('#' + key, form).val(value)
                                $('#' + key, form).trigger("change");
                            }
                        }
                })
            }
        });
    }
}
function displayDocumentNameForDerived(url, Id, propname) {
    if (Id.length == 0)
        Id = '';
    $.ajax({
        url: url,
        type: "GET",
        cache: false,
        dataType: "json",
        async: true,
        success: function (result) {
            var docName;
            var result = result.substr(result.lastIndexOf('\\') + 1)
            var fname = result.substr(0, result.lastIndexOf('.'))
            var ext = result.substr(result.lastIndexOf('.') + 1)
            var len = fname.length;

            if (len > 15) {
                docName = fname.substr(0, 15);
                docName = docName + "..." + ext;
            }
            else
                docName = result;

            if (docName == "NA") {
                if ($("#aDelete" + propname + Id) != undefined)
                    $("#aDelete" + propname + Id).css("display", "none");
            }

            $("#adownloadEdit" + propname + Id).html(docName);
            $("#adownloadEdit" + propname + Id).attr("title", result)
        },
        error: function (jqXHR, textStatus, errorThrown) {
            //alert(errorThrown + '-' + textStatus);
        }
    });
}
function ListBoxPagination(obj, searchstring, hostingentityname, hostingentityid, associatedtype, selectCtrl) {
    var url1 = ($(obj).attr("dataurl"));
    size = $(obj).val();
    url1 = addParameterToURL(url1, "SearchString", searchstring);
    url1 = addParameterToURL(url1, "HostingEntityName", hostingentityname);
    url1 = addParameterToURL(url1, "HostingEntityID", hostingentityid);
    url1 = addParameterToURL(url1, "AssociatedType", associatedtype);
    url1 = addParameterToURL(url1, "Size", size);
    $.ajax({
        async: false,
        type: "GET",
        url: url1,
        success: function (data) {
            searchstr = searchstring;
            $('#' + selectCtrl + ' > option[val!=""]').remove();
            $.each(data.data, function (index, value) {
                $('#' + selectCtrl).append('<option value="' + value.Value + '">' + value.Text + '</option>');
            });
            if (data.Results)
                $('#lbl' + selectCtrl).css('display', 'block');
            else
                $('#lbl' + selectCtrl).css('display', 'none');
        },
        error: function (jqXHR, textStatus, errorThrown) {
            //alert(errorThrown + '-' + textStatus);
        }
    });
}
// Filter Tab Cookies
function ClickFilterBtn() {
    if ($.cookie('fltCookie') != null) {
        var _fltId = $.cookie('fltCookie');
        SetCookieFlt("0");
        $("#" + _fltId + "").click();
    }
}
function ClickFilterTabBtn() {
    if ($.cookie('fltCookieFltTabId') != null) {
        var _fltTabId = $.cookie('fltCookieFltTabId');
        SetCookieFltTab("0");
        $("#" + _fltTabId).click();
    }
}
function SetCookieFlt(fltId) {
    if (fltId != "0") {
        $.cookie("fltCookie", fltId);
    }
    else {
        $.cookie("fltCookie", null)
    }
}
function ClearFilterCookies() {
    $.cookie("fltCookie", null);
    $.cookie("fltCookie", "");
    $.cookie("fltCookieFltTabId", null);
    $.cookie("fltCookieFltTabId", "");
    var cookies = document.cookie.split(";");
    for (var i = 0; i < cookies.length; i++) {
        var equals = cookies[i].indexOf("=");
        var name = equals > -1 ? cookies[i].substr(0, equals) : cookies[i];
        if (name.trim() == 'fltCookie' || name.trim() == 'fltCookieFltTabId')
            document.cookie = name + "=;expires=Thu, 01 Jan 1970 00:00:00 GMT";
    }
    if ($.cookie("fltCookie") != null)
        $.removeCookie("fltCookie");
    if ($.cookie("fltCookieFltTabId") != null)
        $.removeCookie("fltCookieFltTabId");
    $.cookie("fltCookie", null);
    $.cookie("fltCookie", "");
    $.cookie("fltCookieFltTabId", null);
    $.cookie("fltCookieFltTabId", "");
}
function SetCookieFltTab(FltTabId) {
    if (FltTabId != "0") {
        $.cookie("fltCookieFltTabId", FltTabId);
    }
    else {
        $.cookie("fltCookieFltTabId", null)
    }
}
//
function togglesidebar(e, obj, user) {
    e.preventDefault();
    if ($.cookie("sidebartoggle" + user) == "") {
        $('#accordionSidebar').removeClass('toggled')
        $.cookie("sidebartoggle" + user, $("#wrapper").attr("class"));
    }
    else {
        $('#accordionSidebar').addClass('toggled');
        $.cookie("sidebartoggle" + user, "")
    }
}
function LoadReports(rptId, LoadReportsDiv, Entity, Asso, Prop, sortby, isAsc, currentFilter, page) {
    var urlstring = SetReportUrl(rptId, LoadReportsDiv, Entity, Asso, Prop, sortby, isAsc, currentFilter, page);
    $("#ShowReoprts").modal('show');
    $("#ShowReoprts").find('.modal-dialog.ui-draggable').attr("style", "width:90%");
    $("#" + LoadReportsDiv).html('Please wait..');
    $("#" + LoadReportsDiv).load(urlstring);
}
function SetReportUrl(rptId, LoadReportsDiv, Entity, Asso, Prop, sortby, isAsc, currentFilter, page) {
    var urlstring = $("#" + "fSearch" + Entity).attr("dataurl");
    var association = Asso.split(",");
    var property = Prop.split(",");
    var firstparam = 0;
    for (i = 0; i < association.length; i++) {
        var vals = "";
        ele = document.getElementById(association[i]);
        if (ele != null)
            for (var o = 0; o < ele.options.length; o++) {
                if (ele.options[o].selected)
                    vals += ele.options[o].value + ",";
            }
        if (vals.length > 0) {
            if (firstparam == 0)
                urlstring += "?" + association[i] + "=" + vals;
            else
                urlstring += "&" + association[i] + "=" + vals;
            firstparam = 1;
        }
    }
    for (i = 0; i < property.length; i++) {
        ele = document.getElementById(property[i]);
        if (ele != null)
            if (ele.value.length > 0) {
                if (firstparam == 0)
                    urlstring += "?" + property[i] + "=" + ele.value;
                else
                    urlstring += "&" + property[i] + "=" + ele.value;
                firstparam = 1;
            }
    }
    var page_sortstring = "";
    if (sortby != "") {
        page_sortstring = "&sortBy=" + sortby;
        if (isAsc != "")
            page_sortstring += "&isAsc=" + isAsc;
    }
    if (currentFilter != '') {
        if (firstparam == 0)
            urlstring += "?searchString=" + currentFilter + page_sortstring;
        else
            urlstring += "&searchString=" + currentFilter + page_sortstring;
    }
    if (firstparam == 0)
        urlstring += "?search=" + SanitizeURLString(document.getElementById("FSearch").value) + page_sortstring;
    else
        urlstring += "&search=" + SanitizeURLString(document.getElementById("FSearch").value) + page_sortstring;
    urlstring = addParameterToURL(urlstring, "SortOrder", $("#SortOrder").val());
    urlstring = addParameterToURL(urlstring, "GroupByColumn", $("#hdnGroupByColumn").val());
    urlstring = addParameterToURL(urlstring, "HideColumns", $("#HideColumns").val());
    urlstring = addParameterToURL(urlstring, "IsReports", true);
    urlstring = addParameterToURL(urlstring, "FilterCondition", $("#FilterCondition").val());
    if (rptId != "")
        urlstring = addParameterToURL(urlstring, "rptId", rptId);
    if ($("#ViewReport") != undefined)
        $("#ViewReport").val(urlstring);
    return urlstring;
}
function showalldivs() {
    var divs = document.querySelectorAll("[id^=dvGroup]");
    for (var i = 0; i < divs.length; i++) {
        $(divs[i]).removeAttr("style", "block");
    }
}
$(document).on("paste", "input[type=text],textarea", function (e) {
    e.preventDefault();
    //var withoutSpaces = e.originalEvent.clipboardData.getData('Text');
    var withoutSpaces = "";
    if (window.clipboardData && window.clipboardData.getData) { // IE
        withoutSpaces = window.clipboardData.getData('Text');
    }
    else if (e.originalEvent.clipboardData && e.originalEvent.clipboardData.getData) { // other browsers
        withoutSpaces = e.originalEvent.clipboardData.getData('Text');
    }
    withoutSpaces = withoutSpaces.replace(/^\s\s*/, '').replace(/\s\s*$/, '');
    var selectedText = window.getSelection().toString()
    if (selectedText != "") {
        var repVal = $(this).val().replace(selectedText, withoutSpaces);
        $(this).val(repVal)
    }
    else {
        //var val = $(this).val();
        //$(this).val(val + withoutSpaces);
        var caretPos = $(this)[0].selectionStart;
        var textAreaTxt = $(this).val();
        var txtToAdd = withoutSpaces;
        $(this).val(textAreaTxt.substring(0, caretPos) + txtToAdd + textAreaTxt.substring(caretPos));

    }
});
///Entity Help Page
function SavePropertyValueHome(obj, entityName, ObjectId, oldValue, url, EntityName, PropertyDataType, PropertyName, ObjectType) {
    var value = $(obj).val();
    if ($(obj).is(":checkbox")) {
        if ($(obj).prop('checked') === true)
            value = true;
        else value = false;
    }
    if ($(obj).attr("controlname") == 'SectionText') {
        value = encodeURIComponent($(obj).code());
    }
    if (value == undefined)
        value = "";
    $(obj).parent().removeClass("edit");
    if (value != oldValue) {
        var property = $(obj).attr("controlname");
        $(obj).parent().html("<span class='text-warning' id='lblSaving" + ObjectId + "'>Wait..<label>");
        var finalUrl = url + "?id=" + ObjectId + "&property=" + property + "&value=" + value + "&EntityName=" + EntityName + "&PropertyDataType=" + PropertyDataType + "&PropertyName=" + PropertyName + "&ObjectType=" + ObjectType;
        $.ajax({
            type: "GET",
            url: finalUrl,
            asyc: false,
            success: function (jsonObj) {
                if (jsonObj.Result == "duplicate") {
                    alert("Section Name " + jsonObj.data + " already exists");
                }
                if (jsonObj.Result == "Success") {
                    $("#" + entityName + "SearchCancel").click();
                }
                else {
                    // alert(jsonObj.data);
                    $("#" + entityName + "SearchCancel").click();
                }
            },
            error: function (jqXHR, textStatus, errorThrown) {
                $(obj).parent().html("Error in saving data.");
            }
        });
    }
    else {
        if ($(obj).is('select')) {
            $(obj).parent().html(oldValue);
        }
        else {
            $("#" + entityName + "SearchCancel").click();
        }
    }
}
//instruction feature
function ShowInstructionLabel(url, Entity, pageName, topDown, entityDispalyName) {
    var finalUrl = url + "?Entity=" + Entity;
    $.ajax({
        type: "GET",
        url: finalUrl,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        async: true,
        success: function (jsonObj) {
            if (jsonObj.responseJSON != "Success")
                $.each(jsonObj, function (i, item) {
                    $("#dv" + item.PropertyName).html(item.HelpText);
                });
        },
        error: function (jqXHR, textStatus, errorThrown) {
            //alert('failed');
            return false;
        }
    });
}
function ShowHelpIcon(url, Entity, pageName, topDown, entityDispalyName) {
    var finalUrl = url + "?Entity=" + Entity;
    $.ajax({
        type: "GET",
        url: finalUrl,
        contentType: "application/json; charset=utf-8",
        async: true,
        dataType: "json",
        success: function (jsonObj) {
            // if (jsonObj.responseJSON != undefined && jsonObj.responseJSON != "Success")
            if (jsonObj != undefined && jsonObj != "Success")
                ShowICon(jsonObj, Entity, url, pageName, topDown, entityDispalyName)
        },
        error: function (jqXHR, textStatus, errorThrown) {
            //alert('failed');
            return false;
        }
    });
}
function ShowICon(data, Entity, url, pageName, topDown, entityDispalyName) {
    var HasverbIncon = false;
    var HasverbInconDetails = false;
    $.each(data, function (i, item) {
        // if (item.HelpText == null || item.HelpText == "<p><br></p>") {
            // return true;
        // }
        if (item.Tooltip == null && (item.HelpText == null || item.HelpText == "<p><br></p>")) {
            return true;
        }
        if (item.Tooltip == null && (item.HelpText == null || pageName == "editquick" || pageName == "createquick" || item.HelpText == "<p><br></p>")) {
            return true;
        }
        var onhoverstr = "";
        var onclickstrPop = "";
        if (item.HelpText != "" && item.HelpText != null && (pageName == "edit" || pageName == "create" || pageName == "createquick" || pageName == "editquick"))
            onclickstrPop = "onclick=OpenPopUpEntityHelp('addPopupHelp','dvPopupHelp','" + url.replace("ShowHelpIcon", "QuickHelp") + "','" + Entity + "','" + item.PropertyName + "','" + item.ObjectType + "','" + encodeURIComponent(entityDispalyName) + "')"



        if (item.Tooltip != "" && item.Tooltip != null)
            onhoverstr = "onmouseover=ShowHelp(this," + "'" + url.replace("ShowHelpIcon", "ShowPropertyHelp") + "','" + Entity + "','" + item.PropertyName + "') data-trigger='hover' ";
        var openingdiv = "<div class='input-group-append'>"
        var colseingpiv = "</div>";
        var srtIcon = "<a " + onclickstrPop + " " + onhoverstr + " aria-describedby='popover" + item.PropertyName.trim() + " ' id='popover" + item.PropertyName.trim() + "' class='btn btn-white' data-toggle='popover'" +
            "data-placement='auto top' content='Another popover' >" +
            "<i class='fa fa-question' style='font-size: 12px;'></i></a>";

        if (pageName == "details") {
            srtIcon = "<a " + onclickstrPop + " " + onhoverstr + " aria-describedby='popover" + item.PropertyName.trim() + " ' id='popover" + item.PropertyName.trim() + "' class='btn btn-xs' data-toggle='popover'" +
                "data-placement='auto top' content='Another popover' >" +
                "<i class='fa fa-question' style='color:#1d5072; font-size: 12px;'></i></a>";

        }
        if (item.PropertyDataType == "Action" && (pageName == "edit" || pageName == "details")) {
            srtIcon = "<a " + onclickstrPop + " " + onhoverstr + " aria-describedby='popover" + item.PropertyName.trim() + " ' id='popover" + item.PropertyName.trim() + "' style='margin-left:5px;' class='btn btn-white btn-xs pull-right' data-toggle='popover'" +
                "data-placement='auto top' content='Another popover' >" +
                "<i class='fa fa-question' style='font-size: 12px;'></i></a>";
        }
        if (item.ObjectType == "Group") {
            srtIcon = "<a " + onclickstrPop + " " + onhoverstr + " aria-describedby='popover" + item.PropertyName.trim() + " ' id='popover" + item.PropertyName.trim() + "' style='margin-left:5px;' class='btn btn-white btn-xs pull-right' data-toggle='popover'" +
                "data-placement='auto top' content='Another popover' >" +
                "<i class='fa fa-question' style='font-size: 12px;'></i></a>";
        }
        if (item.PropertyDataType == "Association") {
            if (pageName == "create" || pageName == "edit") {
                $("#" + item.PropertyName + "_chosen").after(openingdiv + srtIcon + colseingpiv);
            }
            if (pageName == "createquick" || pageName == "editquick") {
                $("#" + item.PropertyName + "_chosen").after(openingdiv + srtIcon + colseingpiv);
            }
            if (pageName == "details") {
                $("#lbl" + item.PropertyName).after(openingdiv + srtIcon + colseingpiv);
            }
        }
        else {
            var type = $("#" + item.PropertyName).attr('type')
            if (type != 'hidden') {
                if (pageName == "edit" && type != "checkbox")
                    type = $("#File_" + item.PropertyName).attr('type')
                if (type == "file") {
                    if ($("#dv" + item.PropertyName).find("a").attr('class') == "btn btn-primary btnupload") {
                        var fileA = $("#dv" + item.PropertyName).find("a");
                        fileA.after(srtIcon)
                    }
                }
                else if (type == "checkbox") {
                    $("#" + item.PropertyName).after(srtIcon);
                }
                else {
                    if (topDown) {
                        if (pageName == "details") {
                            $("#lbl" + item.PropertyName).wrap("<div class='input-group'></div>");
                            $("#lbl" + item.PropertyName).after(openingdiv + srtIcon + colseingpiv);
                            $("#time" + item.PropertyName).after(openingdiv + srtIcon + colseingpiv);
                            $("#dv" + item.PropertyName).find("p.viewlabelmultiline").after(openingdiv + srtIcon + colseingpiv);
                            $("#dv" + item.PropertyName).find("p.text-primary").after(openingdiv + srtIcon + colseingpiv);
                        }
                        else {
                            $("#" + item.PropertyName).wrap("<div class='input-group'></div>");
                            $("#" + item.PropertyName).after(openingdiv + srtIcon + colseingpiv);
                        }
                    }
                    else {
                        if (pageName == "details") {
                            $("#dv" + item.PropertyName).find("select").after(openingdiv + srtIcon + colseingpiv);
                            $("#lbl" + item.PropertyName).after(openingdiv + srtIcon + colseingpiv);
                            $("#time" + item.PropertyName).after(openingdiv + srtIcon + colseingpiv);
                            $("#dv" + item.PropertyName).find("p.viewlabelmultiline").after(openingdiv + srtIcon + colseingpiv);
                            $("#dv" + item.PropertyName).find("p.text-primary").after(openingdiv + srtIcon + colseingpiv);
                        }
                        else
                            $("#" + item.PropertyName).after(openingdiv + srtIcon + colseingpiv);
                    }
                }
            }
        }
        if (item.ObjectType == "Group") {
            var GroupHeader = $("#dvGroup" + item.GroupInternalName).find(".card-title");
            GroupHeader.append("<div class='pull-right'>" + srtIcon + "</div>");
        }
        if (item.ObjectType == "Verb" && item.PropertyDataType == "Action" && pageName == "edit" && !HasverbIncon) {
            //$(window.document).find('div.btn-group.pull-right').before(srtIcon)
            $(document.getElementById('vrb1' + item.PropertyName)).attr('title', item.Tooltip);
            HasverbIncon = true;
        }
        if (item.ObjectType == "Verb" && item.PropertyDataType == "Action" && pageName == "details" && !HasverbInconDetails) {
            //$(window.document).find('div.btn-group.pull-right').before(srtIcon)
            $(document.getElementById('vrb1' + item.PropertyName)).attr('title', item.Tooltip);
            HasverbInconDetails = true;
        }
    });
}
function ShowHelp(obj, url, Entity, propertyName) {
    var url = url + "?Entity=" + Entity + "&propertyName=" + propertyName;
    var Tooltip = "";
    $.ajax({
        url: url,
        type: "GET",
        cache: true,
        contentType: false,
        data: {},
        async: false,
        success: function (result) {
            Tooltip = result.Tooltip
            $(obj).popover({
                content: Tooltip,
                placement: "top"
            });
            $(obj).popover("toggle");
        },
        error: function (jqXHR, textStatus, errorThrown) {
            return false;
        }
    });
}
function OpenPopUpEntityHelp(Popup, dvName, url, EntityName, propName, ObjectType, entityDispalyName) {
    if ($("#dvPopupHelp") != undefined)
        $("#dvPopupHelp").html('');
    $("#" + Popup + 'Label').html();
    $("#addPopupHelpLabelUni").html("Help: " + decodeURIComponent(entityDispalyName) + " ");
    $("#" + Popup).modal('show');
    $("#" + dvName).html('Loading..');//uncommented on 7/7/2017
    $("#" + dvName + "Error").html('');
    $("#" + dvName).load(url + "?entName=" + EntityName + "&propName=" + propName + "&ObjectType=" + ObjectType);
}
function LoadTabEntityPage(dvName, url) {
    $("#" + dvName).html('Please wait..');
    $("#" + dvName).load(url);
}
function LoadTabEntityBr(dvName, url) {
    $("#" + dvName).html('Please wait..');
    $("#" + dvName).load(url);
}
function QuickAddFromIndexEntityPage(e, isrefresh, Entity, host, bisrule, biscount, ruleType, lblerrormsg, isinline, lstinlineassocname, lstinlineassocdispname, lstinlineentityname) {
    $(e.currentTarget).attr('disabled', 'disabled')
    var target;
    if (e.srcElement) target = e.srcElement;
    e = $.event.fix(e);
    if (e.currentTarget) target = e.currentTarget;
    e.preventDefault();
    var form = $(target).closest('form');
    var fd = $(target).closest('form').serialize();
    var url = $(target).closest('form').attr("action");
    //    $("#SectionText").val($("#SectionText").code());
    var p = BusineesRule(fd, url, bisrule, biscount, form, ruleType, Entity, lblerrormsg, isinline, lstinlineassocname, lstinlineassocdispname, lstinlineentityname);
    if (!p) {
        $(target).removeAttr("disabled");
        return false;
    }
    if (!form.valid()) { $(target).removeAttr("disabled"); return; }
    SaveServerTimeQuickAdd(form);
    try {
        fd = new FormData(form[0]);
        $('input[type="file"]').each(function () {
            var file = $('#' + $(this)[0].id)[0].files;
            if (file.length) {
                fd.append($(this)[0].id, file[0]);
            }
        });
        $.ajax({
            url: url + "?IsAddPop=" + true,
            type: "POST",
            cache: false,
            data: fd,
            dataType: "json",
            processData: false,
            contentType: false,
            success: function (result) {
                $(target).removeAttr("disabled");
                if (result == "FROMPOPUP" || result.result == "FROMPOPUP") {
                    form.find('button[aria-hidden="true"]').click();
                    if (isrefresh) {
                        if (host != undefined && host.length > 0 && $('#' + host).length > 0) {
                            $('#' + Entity + 'SearchCancel', $('#' + host)).click();
                            $('#dvcnt_' + host).load(location.href + " #dvcnt_" + host);
                            if ($('#' + Entity + 'SearchCancel', $('#' + host)).length <= 0) {
                                window.location.reload();
                            }
                        }
                        else
                            $('#' + Entity + 'SearchCancel').click();
                    }
                } else {
                    //$('#dvPopupError').html("Section Name already exists");
                    $("#divDisplayEntitySave").removeAttr("style"); $("#divDisplayEntitySave").html(getMsgTableEntitySave());
                    document.getElementById("ErrMsgEntitySave").innerHTML = "Section Name already exists";
                }
            }
        });
    } catch (ex) {
        $(target).removeAttr("disabled");
        fd = $(target).closest('form').serialize();
        $.ajax({
            url: url + "?IsAddPop=" + true,
            type: "POST",
            cache: false,
            data: fd,
            success: function (result) {
                if (result == "FROMPOPUP") {
                    form.find('button[aria-hidden="true"]').click();
                    if (isrefresh) {
                        if (host != undefined && host.length > 0 && $('#' + host).length > 0) {
                            $('#' + Entity + 'SearchCancel', $('#' + host)).click();
                            $('#dvcnt_' + host).load(location.href + " #dvcnt_" + host);
                            if ($('#' + Entity + 'SearchCancel', $('#' + host)).length <= 0) {
                                window.location.reload();
                            }
                        }
                        else
                            $('#' + Entity + 'SearchCancel').click();
                    }
                } else {
                    //$('#dvPopupError').html("Section Name already exists");
                    $("#divDisplayEntitySave").removeAttr("style"); $("#divDisplayEntitySave").html(getMsgTableEntitySave());
                    document.getElementById("ErrMsgEntitySave").innerHTML = "Section Name already exists";
                }
            }
        });
    }
    if (Entity.indexOf('Events') > -1) {
        location.reload();
    }
    var btnvalue = $(e.currentTarget).attr("btnval");
    if (btnvalue == "createcontinue") {
        //if (result == "FROMPOPUP" || result.result == "FROMPOPUP")
        $('#add' + Entity).click();
    }
}
function OpenGraphFromEntityHome(obj, dvName, entName) {
    var url = $(obj).attr("dataurl");//+"?TS="+Date.now();
    $("#EntityGraphLabel").html(entName)
    $("#" + dvName).load(url);
}
function OpenInlineControl(div, prefix) {
    $("#btn" + prefix).hide();
    $("[name^='" + prefix + ".']").removeProp("readonly");
}
function DisableInlineControl(div, prefix) {
    $("#btn" + prefix).hide();
    $("[name^='" + prefix + ".']").prop("readonly", "readonly");
    $("#btn" + prefix).show();
}
function RefreshDiv(obj, div, prefix) {
    var $this = $(obj);
    var $div = $("#" + div);
    var entityname = $this.attr("HostingName");
    var ddvalue = $this.val();
    var dataurl = $this.attr("dataurl");
    dataurl = dataurl.replace('GetAllValue', 'GetJsonRecordById');
    dataurl = addParameterToURL(dataurl, 'id', ddvalue);
    $.ajax({
        type: "GET",
        url: dataurl,
        contentType: "application/json; charset=utf-8",
        global: false,
        cache: false,
        async: false,
        dataType: "json",
        complete: function (jsonObj) {
            var result = jsonObj.responseJSON;
            var idValue = result['Id'];
            $.each(result, function (key, data) {
                var control = $("#" + prefix + "_" + key);
                if (control.length > 0) {
                    if (key == "ConcurrencyKey")
                        data = base64ArrayToString(data);
                    //control.val(data);
                    setValueInControl(control, data);
                    if (idValue > 0) {
                        control.prop("readonly", "readonly");
                        $("#btn" + prefix).show();
                    }
                    else {
                        control.removeProp("readonly");
                        $("#btn" + prefix).hide();
                    }
                }
            })
            $("body").css('cursor', 'default');
        },
        success: function (jsonObj) { },
        error: function (jqXHR, textStatus, errorThrown) {
            alert(jqXHR + textStatus + errorThrown);
        }
    });
}
// End Entity Home Page
// BulkButton changes
$('#Butbulk').click(function () {
    $('#SelectAll,#SelectCheck,#BulkUpdate,#BulkDelete,.BulkVerb').toggle();
});
// End BulkButton changes
function base64ArrayToString(arrayBuffer) {
    var base64 = ''
    var encodings = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/'
    var bytes = new Uint8Array(arrayBuffer)
    var byteLength = bytes.byteLength
    var byteRemainder = byteLength % 3
    var mainLength = byteLength - byteRemainder
    var a, b, c, d
    var chunk
    for (var i = 0; i < mainLength; i = i + 3) {
        chunk = (bytes[i] << 16) | (bytes[i + 1] << 8) | bytes[i + 2]
        a = (chunk & 16515072) >> 18 // 16515072 = (2^6 - 1) << 18
        b = (chunk & 258048) >> 12 // 258048   = (2^6 - 1) << 12
        c = (chunk & 4032) >> 6 // 4032     = (2^6 - 1) << 6
        d = chunk & 63               // 63       = 2^6 - 1
        // Convert the raw binary segments to the appropriate ASCII encoding
        base64 += encodings[a] + encodings[b] + encodings[c] + encodings[d]
    }
    // Deal with the remaining bytes and padding
    if (byteRemainder == 1) {
        chunk = bytes[mainLength]
        a = (chunk & 252) >> 2 // 252 = (2^6 - 1) << 2
        // Set the 4 least significant bits to zero
        b = (chunk & 3) << 4 // 3   = 2^2 - 1
        base64 += encodings[a] + encodings[b] + '=='
    } else if (byteRemainder == 2) {
        chunk = (bytes[mainLength] << 8) | bytes[mainLength + 1]
        a = (chunk & 64512) >> 10 // 64512 = (2^6 - 1) << 10
        b = (chunk & 1008) >> 4 // 1008  = (2^6 - 1) << 4
        // Set the 2 least significant bits to zero
        c = (chunk & 15) << 2 // 15    = 2^4 - 1
        base64 += encodings[a] + encodings[b] + encodings[c] + '='
    }
    return base64
}
function redirectPost(url, data) {
    var form = document.createElement('form');
    document.body.appendChild(form);
    form.method = 'post';
    form.action = url;
    var input = document.createElement('input');
    input.type = 'hidden';
    input.name = 'token';
    input.value = data;
    form.appendChild(input);
    form.submit();
    document.body.removeChild(form);
}
function loginsso(getjwt, calleeurl) {
    var tokenString = "";
    $.ajax({
        type: "GET",
        url: getjwt,
        asyc: false,
        dataType: "json",
        complete: function (jsonObj) {
            if (jsonObj.responseJSON.length > 0) {
                redirectPost(calleeurl, jsonObj.responseJSON.toString());
            }
            else {
                OpenAlertPopUp("Error", "Token not generated.");
            }
        },
        error: function (jqXHR, textStatus, errorThrown) {
            $("body").css('cursor', 'default');
            $('.fa.fa-spinner.fa-spin').attr('class', '');
            OpenAlertPopUp("Error", "Authentication Failed.");
        }
    });
}
function CopyAddFromIndex(e, Entity) {
    //$(e.currentTarget).attr('disabled', 'true');
    $(e.currentTarget).attr('disabled', 'disabled');
    var target;
    if (e.srcElement) target = e.srcElement;
    e = $.event.fix(e);
    if (e.currentTarget) target = e.currentTarget;
    e.preventDefault();
    var form = $(target).closest('form');
    var fd = new FormData(form[0]);
    var url = $(target).closest('form').attr("action");
    if (!form.valid()) { $(target).removeAttr("disabled"); return; }
    try {
        $.ajax({
            url: url,
            type: "POST",
            cache: false,
            data: fd,
            dataType: "json",
            processData: false,
            contentType: false,
            async: false,
            success: function (result) {
                //$(target).removeAttr("disabled");
                if (result.result.length == 0) {
                    $('#' + Entity + 'SearchCancel').click();
                    e.preventDefault();
                }
                else if (result.result == "Success") {
                    window.location.replace(result.URL);
                }
                else {
                    if ($('#divDisplayEntitySave') != undefined) {
                        $(target).removeAttr("disabled")
                        e.preventDefault();
                        //$('#dvPopupError').html(result.result);
                        $("#divDisplayEntitySave").removeAttr("style"); $("#divDisplayEntitySave").html(getMsgTableEntitySave());
                        document.getElementById("ErrMsgEntitySave").innerHTML = result.result;
                    }
                    else {
                        e.preventDefault();
                        $(target).removeAttr("disabled")
                        $('#divDisplayBRmsgMandatory').html(result.result);
                    }
                }
            }
        });
    } catch (ex) {
        alert('error');
        $(target).removeAttr("disabled");
    }
}
function removeURLQueryParameter(url, queryparameter) {
    var urlparams = url.split('?');
    if (urlparams.length >= 2) {
        var prefix = encodeURIComponent(queryparameter) + '=';
        var parsedurl = urlparams[1].split(/[&;]/g);
        for (var i = parsedurl.length; i-- > 0;) {
            if (parsedurl[i].lastIndexOf(prefix, 0) !== -1) {
                parsedurl.splice(i, 1);
            }
        }
        url = urlparams[0] + '?' + parsedurl.join('&');
        return url;
    } else {
        return url;
    }
}
function OpenInlineEntity(obj, EntityName, url) {
    $("#trInline" + EntityName).show();
    $("#tdInline" + EntityName).load(url);
    $("#theader" + EntityName + " tr:first-child").hide();
}
function OpenPopUpEntityImage(Popup, EntityName, dvName, url) {
    $("#" + Popup + 'Label').html();
    $("#" + Popup + 'Label').html("" + EntityName);
    $("#" + Popup).modal('show');
    //  $("#" + dvName).html('Loading..');
    $("#" + dvName).attr('src', url);
}
function LoadMetricResult(item, dataurl, aggregate, aggregateproperty, fontcolor) {
    dataurl = dataurl.replace("viewtype=IndexPartial", "viewtype=Metric");
    dataurl = dataurl + "&aggregate=" + aggregate;
    if (aggregate != 'Count') {
        dataurl = dataurl + "&aggregateproperty=" + aggregateproperty;
    }
    $.ajax({
        type: "GET",
        url: dataurl,
        contentType: "application/json; charset=utf-8",
        global: false,
        cache: false,
        async: true,
        dataType: "json",
        success: function (jsonObj) {
            //var result = jsonObj.responseJSON.Result;
            var result = jsonObj;
            if (result.type == "groupby") {
                var labels = result.result.map(function (e) {
                    if (result.dataType != undefined && result.dataType.toLowerCase() == "date")
                        return moment(e.X).format(result.displayFormat.toUpperCase());
                    else
                        return e.X;
                });
                $("#" + item).find('#theaderMetric', $("#" + item)).html('');
                $("#" + item).find('#trMetric', $("#" + item)).html('');
                $.each(result.result, function (i, item1) {
                    var html = $("#" + item).find('#dvMetricCrossTab', $("#" + item)).html();
                    var newhtml = "<div class='col py-2 br d-flex align-items-center justify-content-center'>" + "<div>" +
                        "<div class='m-0 text-bold crossheader' style='font-weight:bold'>" + labels[i] + "</div>" +
                        "<div class='text-uppercase crossnumber' style='margin-right:5px;'>" + item1.Y + "</div>" + "</div>" +
                        "</div>";
                    $("#" + item).find('#dvMetricCrossTab', $("#" + item)).html(html + newhtml);
                    // $("#" + item).find('#theaderMetric', $("#" + item)).html($("#" + item).find('#theaderMetric', $("#" + item)).html() + "<td>" + item1.X + "</td>");// "=" + item1.Y + " ");
                    // $("#" + item).find('#trMetric', $("#" + item)).html($("#" + item).find('#trMetric', $("#" + item)).html() + "<td>" + item1.Y + "</td>");
                })
            }
            else
                $("#" + item).find('h3').html(result);
            $("body").css('cursor', 'default');
        },

        error: function (jqXHR, textStatus, errorThrown) {
            $("#" + item).find("a").removeAttr('href');
            $("#" + item).find("h3").html('Error')
            $("#" + item).find("h3").next('span').html("DataMetric:-" + $("#" + item).find("h3").next('span').html());
            //        alert('some error');
        }
    });
}
function LoadGraphMetric(item, dataurl, aggregate, aggregateproperty, type, name) {
    dataurl = dataurl.replace("viewtype=IndexPartial", "viewtype=Metric");
    dataurl = dataurl + "&aggregate=" + aggregate;
    if (aggregate != 'Count') {
        dataurl = dataurl + "&aggregateproperty=" + aggregateproperty;
    }
    $.ajax({
        type: "GET",
        url: dataurl,
        contentType: "application/json; charset=utf-8",
        global: false,
        cache: false,
        async: true,
        dataType: "json",
        success: function (jsonObj) {
            var result = jsonObj;
            if (result.type == "groupby") {
                var labels = result.result.map(function (e) {
                    if (result.dataType != undefined && result.dataType.toLowerCase() == "date")
                        return moment(e.X).format(result.displayFormat.toUpperCase());
                    else
                        return e.X;
                });
                var data = result.result.map(function (e) {
                    return e.Y;
                });
                var ctx = $("#" + item)[0].getContext('2d');
                var coloR = [];
                var dynamicColors = function () {
                    var r = Math.floor(Math.random() * 255);
                    var g = Math.floor(Math.random() * 255);
                    var b = Math.floor(Math.random() * 255);
                    return "rgb(" + r + "," + g + "," + b + ")";
                };
                for (var i in data) {
                    coloR.push(dynamicColors());
                }
                var config = {
                    type: type,
                    data: {
                        labels: labels,
                        maintainAspectRatio: false,
                        responsive: true,
                        datasets: [{
                            label: name,
                            data: data,
                            backgroundColor: coloR,

                        }]
                    },
                    options: {
                        title: {
                            display: (type == 'doughnut' || type == 'bar' || type == 'line') ? true : false,
                            text: name
                        },
                        legend: {
                            display: (type == 'bar' || type == 'line') ? false : true,
                        },
                    }
                };
                var chart = new Chart(ctx, config);
            }
            else
                $("#" + item).find('h3').html(result);
            $("body").css('cursor', 'default');
        },

        error: function (jqXHR, textStatus, errorThrown) {
            var div = $("#" + item).parent();
            $(div).html("<div class='col-sm-6 m-0 rounded-left rounded-right mt-2 p-8 GraphMetrics chartjs-render-monitor' style='float: left; border: 1px solid rgb(207, 219, 226); display: block; height: 335px; width: 670px;' > Error in GraphMetric: - " + name);
            //alert('some error');
        }
    });
}
function ListFromHome(dvName, url, UserName, parentdv, backurl) {
    //remove pagination cookies 
    if ($.cookie("pagination" + UserName + dvName) != null)
        $.removeCookie("pagination" + UserName + dvName);
    var host = (getHostingEntityID(url)["AssociatedType"]);
    host = parentdv;
    var IsFilter = (getHostingEntityID(url)["IsFilter"]);
    var IsdrivedTab = (getHostingEntityID(url)["IsdrivedTab"]);
    url = addParameterToURL(url, 'inlinegrid', true);
    url = addParameterToURL(url, 'backurlhome', backurl);
    $.ajax({
        url: url,
        cache: false,
        async: true,
        success: function (data) {
            if (data != null) {
                if (host != undefined && IsFilter != "True" && $('#' + host).length > 0) {
                    if ($('#' + dvName, $('#' + host)).attr('id') == undefined)
                        $('#' + dvName, $('#dv' + host)).html(data);
                    else
                        $('#' + dvName, $('#' + host)).html(data);
                    if (IsdrivedTab) {
                        $("a[href='" + host + "']").trigger("click");
                    }
                    $("#" + dvName + "GridHeader", $('#' + host)).addClass('collapse in');
                }
                else {
                    try {
                        $('#' + dvName).html(data);
                        if (IsdrivedTab) {
                            $("a[href='" + host + "']").trigger("click");
                        }
                    } catch (ex) { }
                }
            }
        }
    })
    return false;
}
function GenerateDocument(obj, dv, type) {
    var flag = false;
    $("[name='SelectedGenerateDocumentTemplate']", $("#" + dv)).each(function () {
        if ($(this).prop("checked")) {
            flag = true;
            var url = ($(this).attr("dataurl"));
            if (type == 'pdf')
                url = addParameterToURL(url, "forcedOutput", "pdf");
            var isdownload = ($(this).attr("download"))
            if (isdownload == 'true' || isdownload == 'True')
                window.open(url, '_blank');
            else
                $.ajax({
                    url: url,
                    cache: false,
                    async: true,
                    success: function (data) {
                        if (data != null) {
                            if (data == 'Success') {
                                //   $('#lblGenerateDocument').text("Document Generated Successfully.")
                            }
                        }
                    }
                })
        }
    });
    if (flag)
        $('#lblGenerateDocument').text("Document Generated Successfully.");
    else $('#lblGenerateDocument').text("Select at least one document to generate.");
}
function GenerateDocumentFromButton(obj, isdownload) {
    var url = ($(obj).attr("dataurl"));
    if (isdownload == 'true' || isdownload == 'True')
        window.open(url, '_blank');
    else
        alert("Document generated successfully.")
}

function ClickDisabled(obj, url, id) {

}
function LoadStatusDropMenu(associationName, dataurl) {
    var RestrictDropdownVal = new URLSearchParams($("#" + associationName + "ID").attr("dataurl")).get('RestrictDropdownVal');
    var currentVal = $("#" + associationName + "ID").val();
    var obj = $("#Change" + associationName);
    if (RestrictDropdownVal != null && RestrictDropdownVal != undefined)
        dataurl = addParameterToURL(dataurl, "RestrictDropdownVal", RestrictDropdownVal)
    $.ajax({
        type: "GET",
        url: dataurl,
        //contentType: "application/json; charset=utf-8",
        // global: false,
        cache: false,
        async: true,
        // dataType: "json",
        success: function (jsonObj) {
            var result = jsonObj;
            var optionDOM = '';
            for (i = 0; i < result.length; i++) {
                if (result[i].Id == currentVal)
                    optionDOM += '';
                else {
                    var changestatus = 'ChangeStatus("' + result[i].Id + '","' + associationName + '","' + 'test' + '");';
                    optionDOM += '<li><a class="dropdown-item" onclick=' + changestatus + '><span class="fa fa-chevron-right"></span> ' + result[i].Name + '</a></li>';
                }
            }
            obj.html(optionDOM);
            $("body").css('cursor', 'default');
        },
        error: function (jqXHR, textStatus, errorThrown) {

        }
    });
}
function ChangeStatus(id, associationName, text) {
    $("#" + associationName + "ID").append(new Option(text, id))
    $("#" + associationName + "ID").val(id);
    $("#" + associationName + "ID").trigger("change");
    $("#btnsavereturn").click();
}
//for bulk Add
function toggleToMultiselect(ddId, entityName, e, childDd, mainEntity) {
    $("#addPopupLabel").html('Bulk Add ' + mainEntity)
    BulkAddDropDown = ddId;
    $("#" + ddId).html('');
    $("#" + ddId + "_chosen").remove();
    $('select[name=' + ddId + ']').multiselect({
        buttonWidth: '100%',
        nonSelectedText: 'ALL'
    });
    //var parentdd = $("#" + ddId).attr('parentdd');
    //if (parentdd != undefined)
    //{ removeperantDDevents(parentdd) }
    //$("#" + ddId).removeAttr('parentdd');
    //$("#" + ddId).removeAttr('assonamewithparent');
    //removeperantDD(childDd);
    $("#" + ddId).removeAttr('required');
    $("#" + ddId).removeAttr("class");
    $("#" + ddId).attr("dataurl", addParameterToURL($("#" + ddId).attr("dataurl").replace('GetAllValue', 'GetAllMultiSelectValue'), "bulkAdd", "true"));
    $("#" + ddId).attr('multiple', 'multiple')
    $("#" + ddId).removeAttr('onchange');
    $("#" + ddId)[0].setAttribute("onchange", "saveselected('" + ddId + "')")
    $(".iconToggle").remove();
    $(".btnPreviewCreate").show();
    $(".bntcreate").val("Create Bulk");
    $("#bulkIconmsg").hide();

}
function removeperantDD(childDd) {
    var childDdarr = childDd.split(',');
    for (var i = 0; i < childDdarr.length; i++) {
        $("#" + childDdarr[i]).removeAttr('parentdd');
        $("#" + childDdarr[i]).removeAttr('assonamewithparent');
    }
}
function removeperantDDevents(parentdd) {
    var parentDDdarrevent = parentdd.split(',');
    for (var i = 0; i < parentDDdarrevent.length; i++) {
        $("#" + parentDDdarrevent[i]).removeAttr('onchange');
        $("#" + parentDDdarrevent[i]).removeAttr('parentdd');
        $("#" + parentDDdarrevent[i]).removeAttr('assonamewithparent');
    }
}
function saveselected(obj) {
    var selectedval = $('#' + obj).val();
    var selctedtext = $('#' + obj).text()
    var ids = "";
    if (selectedval != undefined) {
        for (var i = 0; i < selectedval.length; i++) {
            ids += selectedval[i] + ",";
        }
        $("#ValueForMultiselect").val(ids.substring(0, ids.length - 1));
    }
}
function saveselectedVerb(obj) {
    var selctedtext = $('#' + obj + ' option:selected').text();
    if (selctedtext != undefined && selctedtext.length > 0) {
        $("#VerbName").val(selctedtext);
    }
}
function ShowBulk() {
    $(".iconToggle").show();
    $(".btnbulkadd").hide();
    $(".btncreatecontinue").hide();
    $("#bulkIconmsg").show();
    $('input[type="submit"][redirectedit="true"]').attr('redirectedit', 'false');

}
function backPreiview(entity) {

    $(".createQuickUI").show();
    $('#PreviewTable').html('');
    $('#PreviewTable').hide();
    $('#btnBack').hide();
    $('#CreateSaveCount').hide();
    $('.btnPreviewCreate').show();
    $("#addPopupLabel").html('Bulk Add ' + entity)
}
function LoadAllItem() {
    if (BulkAddDropDown.length > 0) {
        obj = BulkAddDropDown;
        var rndValue;
        if (($("#" + obj).val() == null || $("#" + obj).val() == '') && $('#' + obj).next().find('button').attr('tempclass') == 'selectmulti' && $('#' + obj).next().find('button').attr('title').toUpperCase() == "ALL") { //$("#" + obj).attr("dataurl", addParameterToURL($("#" + obj).attr("dataurl").replace('GetAllMultiSelectValue', 'GetMultiSelectValueAllSelection'), "bulkAdd", "true"));
            $("#" + obj).attr("dataurl", addParameterToURL($("#" + obj).attr("dataurl").replace('GetAllMultiSelectValue', 'GetMultiSelectValueAllSelection'), "bulkSelection", "All"));
            $('#' + obj).next().find('button').click();
        }
        rndValue = $("#" + obj).val();
        rndtext = $("#" + obj + " option:selected");
        if (($("#" + obj).val() == null || $("#" + obj).val() == '') && $('#' + obj).next().find('button').attr('tempclass') == 'selectmulti' && $('#' + obj).next().find('button').attr('title').toUpperCase() == "ALL") {
            $('#' + obj).next().find('button').click();
            var options = $('#' + obj + ' option');
            var rndValue = $.map(options, function (option) {
                return option.value;
            });
            rndtext = $("#" + obj + " option");
            if ($('#' + obj).attr("IsCustom") == undefined) {
                rndtext.splice(0, 1);
                rndValue.splice(0, 1);
            }
            var ids = "";
            if (rndValue != undefined) {
                for (var i = 0; i < rndValue.length; i++) {
                    ids += rndValue[i] + ",";
                }
                $("#ValueForMultiselect").val(ids.substring(0, ids.length - 1));
            }
            $("#" + obj).val(rndValue);
        }
    }
}
function LoadSetFSearchGridfun(obj, url, entityname) {
    var dvnameforfilterdrid = $("#Load" + entityname + "SetFSearchGrid");
    if (dvnameforfilterdrid != undefined) {
        dvnameforfilterdrid.load(url);
        dvnameforfilterdrid.show();
    }
}

function DDFirstItemSelect(parent, child) {
    if ($("#" + child).attr("required") == "required") {
        if ($("#" + parent).val() > 0) {
            $("#" + child).trigger('chosen:open');
            var totalitem = $("#" + child + " option").length;
            if (totalitem == 2) {
                $("#" + child + " option").each(function () {
                    var item = $(this).val();
                    if (item != "" && item > "0") {
                        $("#" + child + " option:contains(" + $(this).text() + ")").attr('selected', 'selected');
                        $("#" + child).val(item);
                        $("#" + child).trigger('chosen:updated');
                        $("#" + child).trigger('change');
                    }
                });
                $("#" + child).trigger('chosen:close');
            }
        }
    }
}
//find position of an element
jQuery.fn.positioncustom = function () {
    if (!this[0]) {
        return;
    }

    var offsetParent, offset, doc,
        elem = this[0],
        parentOffset = { top: 0, left: 0 };

    if (jQuery.css(elem, "position") === "fixed") {

    } else {
        offset = this.offset();
        doc = elem.ownerDocument;
        offsetParent = elem.offsetParent || doc.documentElement;
        while (offsetParent &&
            jQuery.css(offsetParent, "position") === "static") {
            offsetParent = offsetParent.parentNode;
        }
        if (offsetParent && offsetParent !== elem && offsetParent.nodeType === 1) {
            parentOffset = jQuery(offsetParent).offset();
            parentOffset.top += jQuery.css(offsetParent, "borderTopWidth", true);
            parentOffset.left += jQuery.css(offsetParent, "borderLeftWidth", true);

        }
    }
    return {
        top: offset.top - parentOffset.top - jQuery.css(elem, "marginTop", true),
        left: offset.left - parentOffset.left - jQuery.css(elem, "marginLeft", true)
    };
}

/*FOR HORIZONTAL SCROLL BAR AND FULL SCREEN VIEW*/

$(document).ready(function () {
    /*$(".scroll-x").mCustomScrollbar({
        scrollButtons: { enable: true },
        mouseWheel: { enable: false },
        theme: "dark",
        axis: "x",
    });*/

    $('.view-opt').click(function () {
        $(this).toggleClass('active');
        $('body').toggleClass('full-view');
    });
});

/*FOR HORIZONTAL SCROLL BAR AND FULL SCREEN VIEW*/