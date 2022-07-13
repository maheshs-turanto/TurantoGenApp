function FillMultiSelectDropDown(result, element, elementName) {
    var isHaveNullSelect = false;
    var countoptions = 0;
    if (element != null && element != undefined) {
        for (var o = 0; o < element.options.length; o++) {
            if (result.indexOf(element.options[o].value) != -1)
                 $(element.options[o]).prop('selected', true); //element.options[o].selected = true;
            else if (result.indexOf("NULL") != -1)
                isHaveNullSelect = true;
            countoptions++;
        }
        var opt = document.createElement('option');
        opt.value = "NULL";
        opt.innerHTML = "None";
        if (isHaveNullSelect) {
            opt.selected = true;
            element.insertBefore(opt, element.firstChild);
        }
        if (!isHaveNullSelect) {
            element.insertBefore(opt, element.firstChild);
        }
        $("#" + elementName).multiselect("rebuild");
        if (countoptions >= 10) {
            var hostingentity = elementName;
            var urlGetAll = $('#' + hostingentity).attr("dataurl").replace("GetAllMultiSelectValue", "Index") + "?BulkOperation=multiple";
            var dispName = ($("label[for=\"" + hostingentity + "\"]").text());
            var link = "<a class='nohref' title='View All' onclick=\"" + "OpenPopUpBulkOperation('PopupBulkOperation','" + hostingentity + "','" + dispName + "','dvPopupBulkOperation','" + urlGetAll + "')\">View All</a>";
            var getall = "<li class='disabled-result disabled-result' style='font-style:Italic;text-decoration:underline;' >" + link + "</li>";
            var $ul = $("ul", "#dv" + elementName);
            $ul.append($(getall));
        }
    }
}
function LoadDivInsideTab(dvName, username, url) {
    if (dvName.length > 0)
        $.cookie(username + "TabCookie", dvName);
    $("#" + dvName).empty();
    if ($.trim($("#" + dvName).html()).length == 0) {
        $("#" + dvName).html('Please wait..');
        $("#" + dvName).load(url);
    }
}
$('.table tr').click(function (e) {
    $('.table tr').removeClass('highlighted');
    $(this).addClass('highlighted');
});
function ApplyAreYouSure() {
    var userAgent = navigator.userAgent.toLowerCase();
    // Figure out what browser is being used
    var browser = {
        version: (userAgent.match(/.+(?:rv|it|ra|ie)[\/: ]([\d.]+)/) || [])[1],
        safari: /webkit/.test(userAgent),
        opera: /opera/.test(userAgent),
        msie: /msie/.test(userAgent) && !/opera/.test(userAgent),
        mozilla: /mozilla/.test(userAgent) && !/(compatible|webkit)/.test(userAgent)
    };
    if (!browser.msie) {
        $('form').areYouSure();
    }
    else if (browser.version > 8.0) {
        $('form').areYouSure();
    }
}
function NextPrev(entityname) {
    var RecIdEdit = $("#frm" + entityname).find("input:hidden[name='Id']").val();
    $("#Entity" + entityname + "DisplayValueEdit").val(RecIdEdit);
    var textedit = $("option:selected", $("#Entity" + entityname + "DisplayValueEdit")).text();
    $("#Entity" + entityname + "DisplayValueEdit").attr('data-toggle', 'tooltip')
    $("#Entity" + entityname + "DisplayValueEdit").attr('title', textedit);
    var lastOptionValEdit = $('#Entity' + entityname + 'DisplayValueEdit option:last-child').val();
    var fristOptionValEdit = $('#Entity' + entityname + 'DisplayValueEdit option:first-child').val();
    if (lastOptionValEdit == RecIdEdit) {
        $('#nextEdit').attr({ "disabled": "true", "style": "background-color:#eeeeee !important; color:#969696 !important; border-color:#ccc !important" });
    }
    if (fristOptionValEdit == RecIdEdit)
        $('#prevEdit').attr({ "disabled": "true", "style": "background-color:#eeeeee !important; color:#969696 !important; border-color:#ccc !important" });
}
function SetFSearchParameters(SortOrder, GroupByColumn, HideColumns, viewtype) {
    if (SortOrder != null && SortOrder.length > 0) {
        var value = SortOrder;
        var indexlist = value.split(',')
        $("#SortOrder").val('');
        for (var i = 0; i < indexlist.length; i++) {
            if (indexlist[i].length > 0) {
                $("#SortOrder1").val(indexlist[i]);
                SetSortOrder();
            }
        }
        $("#SortOrder").val(value);
        $("#btnsortorder").show();
        $("#lblOrderDepth").html('Then Order By');
        $("#lblsortorder").attr("style", "border:1px solid #428bca;margin-left: 15px; background: #edf5fa; padding-left: 5px !important; border-radius: 4px;")
    }
    if (GroupByColumn != null && GroupByColumn.length > 0) {
        var value = GroupByColumn;
        var indexlist = value.split(',')
        $("#hdnGroupByColumn").val('');
        for (var i = 0; i < indexlist.length; i++) {
            if (indexlist[i].length > 0) {
                $("#GroupByColumn").val(indexlist[i]);
                SetGroupByColumn();
            }
        }
        $("#hdnGroupByColumn").val(value);
        $("#btngroupbycolumn").show();
        $("#lblGroupDepth").html('Then Group By');
        $("#lblgroupbycolumn").attr("style", "border:1px solid #428bca;margin-left: 15px; background: #edf5fa; padding-left: 5px !important; border-radius: 4px;")
    }
    if (HideColumns != null && HideColumns.length > 0) {
        var resHideColumns = HideColumns.split(",");
        var eleHideColumns = document.getElementById("HideColumns");
        for (i = 0; i < resHideColumns.length; i++) {
            for (var o = 0; o < eleHideColumns.options.length; o++) {
                if (eleHideColumns.options[o].value == resHideColumns[i])
                    eleHideColumns.options[o].selected = true;
            }
        }
        $("#HideColumns").multiselect("rebuild");
    }
    if (viewtype != null && viewtype.length > 0) {
        $("#DisplayLayout").val(viewtype);
    }
}
function SetFSearchDropdown(dropdown, id) {
    if (dropdown != null && dropdown.length > 0) {
        var resT_EmployeeCustomerAssociation = dropdown.split(",");
        var eleT_EmployeeCustomerAssociation = document.getElementById(id);
        FillMultiSelectDropDown(resT_EmployeeCustomerAssociation, eleT_EmployeeCustomerAssociation, id);
    }
    else {
        var resT_EmployeeCustomerAssociation = dropdown.split(",");
        var eleT_EmployeeCustomerAssociation = document.getElementById(id);
        FillMultiSelectDropDown(resT_EmployeeCustomerAssociation, eleT_EmployeeCustomerAssociation, id);
    }
}
function SetFSearchFilterCondition(entityurl, FilterCondition, entityName) {
    if (FilterCondition != null && FilterCondition.length > 0) {
        var value = FilterCondition.split("?");
        var Isass = false;
        var gridval1 = "";
        for (i = 0; i < value.length - 1; i++) {
            if (value != "" && value != "undefined") {
                var str = "<tr>";
                var param = value[i].toString().split(",");
                var val1Text = "";
                if (param[0].indexOf("[") > -1) {

                    var assocNm = param[0].substring(1, param[0].lastIndexOf("."));
                    var assocPrp = param[0].substring(param[0].lastIndexOf(".") + 1, param[0].length - 1);
                    var assocEnt = $("#PropertyList option[value='" + assocNm + "']").text();
                    var assocEntVal = $("#PropertyList option[value='" + assocNm + "']").val();
                    var assocEntPrp = "";
                    if (assocPrp != "") {
                        $.ajax({
                            url: entityurl + '?entityName=' + entityName + '&propertyName=' + param[0],
                            type: "GET",
                            cache: false,
                            async: false,
                            success: function (result) {
                                if (result != "") {
                                    Isass = true;
                                    gridval1 = result;
                                    assocEntPrp = result;
                                }
                            },
                            error: function (jqXHR, textStatus, errorThrown) {
                            }
                        });
                    }
                    val1Text = param[0].replace(assocNm, assocEnt).replace(assocPrp, assocEntPrp);
                }
                else { val1Text = $("#PropertyList option[value='" + param[0] + "']").text(); }
                if (Isass)
                    val1Text = gridval1;
                var val1 = param[0];
                var val2 = checkOperator(param[1]);
                var val3 = param[2];
                var val4 = param[3];
                var funcval = val1 + "," + val2 + "," + val3 + "," + val4 + "?";
                if (Isass) {
                    funcval = val1 + "," + val2 + "," + val3 + "," + val4 + "," + param[4] + "?";
                }
                var val = $("#FilterCondition").val();
                val += val1 + "," + val2 + "," + val3 + "," + val4 + "?";
                str += "<td>" + val1Text + "</td>";
                str += "<td>" + val2 + "</td>";
                str += "<td>" + val3 + "</td>";
                str += "<td>" + val4 + "</td>";
                str += "<td><i name=\"FilterCondition\" onclick=\"deleteRowCriteria(this,'" + funcval + "');\" class=\"fa fa-trash\"></i></td>";
                $("#FilterCondition").val(val);
                if (Isass)
                    $("#FilterCondition").val(funcval);
                $('#tblConditionList').show();
                $('#tblConditionList').append(str);
                $("#collapseSearchCriteria").show();
            }
        }
    }
}
function checkOperator(operator) {
    var operand = operator;
    switch (operator) {
        case "&gt;":
            {
                operand = ">";
                break;
            }
        case "&lt;":
            {
                operand = "<";
                break;
            }
        case "&gt;=":
            {
                operand = ">=";
                break;
            }
        case "&lt;=":
            {
                operand = "<=";
                break;
            }
    }
    return operand;
}
function FillSugestedDynamicValuesForCriteria() {
    var text = $("#" + "PropertyList option:selected").text();
    if ($("#" + "AssociationPropertyList").val() != null && $("#" + "AssociationPropertyList").val().length > 0) {
        var value = "[" + $("#" + "PropertyList").val() + "." + $("#" + "AssociationPropertyList").val() + "]";
    }
    else {
        var value = "[" + $("#" + "PropertyList").val() + "]";
    }
}
function FillCriteriaValueFunction(obj, entityName, dataurl) {
    var propertyName = $("#" + obj.id).val();
    $("#" + "OperatorList").html('');
    $.ajax({
        url: dataurl + '?entityName=' + entityName + '&propertyName=' + propertyName,
        type: "GET",
        cache: false,
        async: false,
        success: function (result) {
            var optionDOM = '<option value="0">-- Select --</option>';
            optionDOM += '<option value="=">Equals to</option>';
            if (result == "String") {
                optionDOM += '<option value="Contains">Contains</option>';
            }
            else if (result != "Boolean") {
                optionDOM += '<option value=">">Greater than</option>';
                optionDOM += '<option value="<">Less than</option>';
                optionDOM += '<option value="<=">Less than Or Equals to</option>';
                optionDOM += '<option value=">=">Greater than Or Equals to</option>';
            }
            //
            if (result == "DateTime") {
                $("#ConditionValue").val("Today");
                $("#dvDurationInCondition").show();
            }
            else {
                $("#ConditionValue").val('');
                $("#dvDurationInCondition").hide();
            }
            //
            optionDOM += '<option value="!=">Not Equals to</option>';
            $("#" + "OperatorList").html(optionDOM);
        },
        error: function (jqXHR, textStatus, errorThrown) {
        }
    });
}
function GetSecondLevelAttributeForCriteriaFunction(SelectedEntity, dataurl) {
    var SelectedProperty = $("#" + "PropertyList").val();
    $("#" + "AssociationPropertyList").hide();
    $("#" + "AssociationPropertyList").html('');
    $.ajax({
        url: dataurl + '?Entity=' + SelectedEntity + '&AttributeName=' + SelectedProperty,
        type: "GET",
        cache: false,
        success: function (result) {
            var optionDOM = '<option value="SelectProperty">-- Select Association Property --</option>';
            for (i = 0; i < result.length; i++) {
                optionDOM += '<option class="' + result[i].DataType + '" value="' + result[i].Name + '">' + result[i].DisplayName + '</option>';
            }
            if (result.length == 0) {
                $("#" + "AssociationPropertyList").html('');
                $("#" + "AssociationPropertyList").hide();
            }
            else {
                $("#" + "AssociationPropertyList").show();
                $("#" + "AssociationPropertyList").html(optionDOM);
            }
            FillSugestedDynamicValuesForCriteria();
        },
        error: function (jqXHR, textStatus, errorThrown) {
            alert("error");
        }
    });
}
function GetCriteriaSuggestedValuesFunction(obj, SelectedEntity, dataurl1, dataurl2, dataurl3) {
    $("#dvRule7DynamicValueInCondition").hide();
    if ($("#" + "ConditionValue").val().toLowerCase() != 'today') {
        $("#" + "ConditionValue").val('');
    }
    if ($("#ValueTypeList option:selected").text() == "Pick From List") {
        var SelectedProperty = $("#" + "PropertyList").val();
        if (SelectedEntity.length == 0) {
            alert("Please select an Entity first !");
            return false;
        }
        if (SelectedProperty.length == 0) {
            alert("Please select a Property !");
            return false;
        }
        var assocProp = $("#AssociationPropertyList").val();
        if (assocProp == "SelectProperty") {
            alert("Please select Association Property.");
            return false;
        }
        var hostingEntity = "";
        var propType = "Property";
        $.ajax({
            url: dataurl1 + '?Entity=' + SelectedEntity + '&Property=' + SelectedProperty,
            type: "GET",
            cache: false,
            success: function (result) {
                if (result != "Failure") {
                    $("#" + "dvSuggestedPropertyValue").show();
                    $("#" + "ConditionValue").hide();
                    //
                    var selectedoptionclass = $('option:selected', $('#AssociationPropertyList')).attr('class');
                    if (selectedoptionclass != undefined && selectedoptionclass == "Int64") {
                        $.ajax({
                            url: dataurl2 + '?Entity=' + result + '&AttributeName=' + assocProp,
                            type: "GET",
                            cache: false,
                            async: false,
                            success: function (data) {
                                result = data.Name;
                                assocProp = "DisplayValue";
                                propType = "Association";
                            }
                        });
                    }
                    //
                    hostingEntity = result;
                    $("#SuggestedPropertyValue").attr("dataurl", dataurl3.replace("_Entity", result) + '?propNameBR=' + assocProp);
                    if (hostingEntity != "") {
                        var dataurl = dataurl3.replace("_Entity", result) + '?propNameBR=' + assocProp;
                        $.ajax({
                            url: dataurl,
                            type: "GET",
                            cache: false,
                            async: false,
                            success: function (result) {
                                var countItems = 0
                                var optionDOM = "";
                                for (i = 0; i < result.length; i++) {
                                    if (result[i] != null)
                                        optionDOM += '<option class="' + propType + '" value="' + result[i].Id + '">' + result[i].value + '</option>';
                                    countItems++;
                                }
                                $("#SuggestedPropertyValue").html(optionDOM);
                                $("#SuggestedPropertyValue").multiselect('rebuild');
                            },
                            error: function (jqXHR, textStatus, errorThrown) {
                                alert("error");
                            }
                        });
                    }
                }
                else {
                    alert("Supports only association values.");
                    $("#ValueTypeList :nth(0)").prop("selected", "selected").change();
                    hostingEntity = "";
                    $("#" + "dvSuggestedPropertyValue").hide();
                    $("#" + "ConditionValue").show();
                    $("#" + "SuggestedPropertyValue").removeClass = "chosen-select form-control";
                    $("#" + "SuggestedPropertyValue").removeAttr("HostingName");
                    $("#" + "SuggestedPropertyValue").removeAttr("dataurl");
                }
            },
            error: function (jqXHR, textStatus, errorThrown) {
                alert("error");
            }
        });
    }
    else if ($("#ValueTypeList option:selected").text() == "Dynamic") {
        $("#dvSuggestedPropertyValue").hide();
        $("#ConditionValue").val('');
        $("#ConditionValue").hide();
        GetSecondLevelAttributeInCriteria();
        $("#dvRule7DynamicValueInCondition").show();
    }
    else { $("#" + "dvSuggestedPropertyValue").hide(); $("#" + "ConditionValue").show(); $("#" + "ConditionValue").prop("disabled", false); }
}
function FillCriteriaForAssociationFunction(entityName, dataurl) {
    var assocName = $("#PropertyList").val();
    var propertyName = $("#AssociationPropertyList").val();
    $("#" + "OperatorList").html('');
    $.ajax({
        url: dataurl + '?entityName=' + entityName + '&assocName=' + assocName + '&propertyName=' + propertyName,
        type: "GET",
        cache: false,
        async: false,
        success: function (result) {
            var optionDOM = '<option value="0">--- Select ---</option>';
            optionDOM += '<option value="=">Equals to</option>';
            if (result == "String") {
                optionDOM += '<option value="Contains">Contains</option>';
            }
            else if (result != "Boolean") {
                optionDOM += '<option value=">">Greater than</option>';
                optionDOM += '<option value="<">Less than</option>';
                optionDOM += '<option value="<=">Less than Or Equals to</option>';
                optionDOM += '<option value=">=">Greater than Or Equals to</option>';
            }
            optionDOM += '<option value="!=">Not Equals to</option>';
            $("#" + "OperatorList").html(optionDOM);
        },
        error: function (jqXHR, textStatus, errorThrown) {
        }
    });
}
function SetCriteriaValueType(obj) {
    if (obj.id == "OperatorList") {
        $("#ValueTypeList").val('Constant').change();
    }
}
function GetSecondLevelAttributeInCriteriaFunction(SelectedEntity, dataurl) {
    var SelectedProperty = $("#" + "SuggestedDynamicValueInCondition7").val();
    $("#" + "SuggestedDynamicValueInCondition71").hide();
    $("#" + "SuggestedDynamicValueInCondition71").html('');
    $.ajax({
        url: dataurl + '?Entity=' + SelectedEntity + '&AttributeName=' + SelectedProperty,
        type: "GET",
        cache: false,
        success: function (result) {
            var optionDOM = '<option value="SelectProperty">--Select Association Property--</option>';
            for (i = 0; i < result.length; i++) {
                optionDOM += '<option class="' + result[i].DataType + '" value="' + result[i].Name + '">' + result[i].DisplayName + '</option>';
            }
            if (result.length == 0) {
                $("#" + "SuggestedDynamicValueInCondition71").html('');
                $("#" + "SuggestedDynamicValue71").hide();
            }
            else {
                $("#" + "SuggestedDynamicValueInCondition71").show();
                $("#" + "SuggestedDynamicValueInCondition71").html(optionDOM);
            }
        },
        error: function (jqXHR, textStatus, errorThrown) {
            alert("error");
        }
    });
}
function AddCriteriaInGridFunction(entityName) {
    if ($("#PropertyList option:selected").val() == "SelectProperty" || $("#PropertyList option:selected").val() == "") {
        alert("Please select Property for condition.");
        return false;
    }
    if ($("#OperatorList").val() == "" || $("#OperatorList").val() == "0") {
        alert("Please select Operator for condition.");
        return false;
    }
    if ($("#ConditionValue").val().trim() == "" && !($("#ValueTypeList").val() == "Pick From List" || $("#ValueTypeList").val() == "Dynamic")) {
        alert("Please enter Property Value for condition.");
        return false;
    }
    if ($("#AssociationPropertyList").val() != null && $("#AssociationPropertyList").val() != "") {
        if ($("#AssociationPropertyList option:selected").val() == "SelectProperty") {
            alert("Please select Association Property.");
            return false;
        }
        var option_all = $("#SuggestedPropertyValue option:selected").map(function () {
            // return $(this).text();
            if ($(this).attr("class") == "Association")
                return $(this).val();
            else
                return $(this).text();
        }).get().join();
        if (option_all != "")
            $("#ConditionValue").val(option_all);
        $("#SuggestedPropertyValue option:selected").removeAttr("selected");
        $("#SuggestedPropertyValue").multiselect('rebuild');
    }
    //Dynamic
    var dynamicPropertyDispValue = "";
    if ($("#ValueTypeList option:selected").text() == "Dynamic") {
        if ($("#" + "SuggestedDynamicValueInCondition71").val() != null && $("#" + "SuggestedDynamicValueInCondition71").val().length > 0) {
            var value = "[" + $("#" + "SuggestedDynamicValueInCondition7").val() + "." + $("#" + "SuggestedDynamicValueInCondition71").val() + "]";
            dynamicPropertyDispValue = "[" + $("#" + "SuggestedDynamicValueInCondition7 option:selected").text() + "." + $("#" + "SuggestedDynamicValueInCondition71 option:selected").text() + "]";
            $("#" + "ConditionValue").val(value);
        }
        else {
            var value = "[" + $("#" + "SuggestedDynamicValueInCondition7").val() + "]";
            dynamicPropertyDispValue = "[" + $("#" + "SuggestedDynamicValueInCondition7 option:selected").text() + "]";
            $("#" + "ConditionValue").val(value);
        }
    }
    //Dynamic
    var condValue = $("#ConditionValue").val();
    if (condValue.toLowerCase() == "today") {
        var isnumber = isNaN($("#DurationCount").val())
        if ($("#DurationCount").val().length > 0 && !isnumber) {
            condValue = condValue + " " + $("#DateOperatorList").val() + " " + $("#DurationCount").val() + " " + $("#DurationList").val();
        }
        $("#ConditionValue").val(condValue)
    }
    var str = "<tr>";
    var val1Text = $("#PropertyList option:selected").text();
    var val1 = $("#PropertyList").val();
    var val2 = $("#OperatorList").val();
    var val3 = $("#ConditionValue").val();
    var val4 = $("#LogicalConnectorList option:selected").text();
    var val5 = $("#ValueTypeList").val();
    if ($("#AssociationPropertyList").val() != null && $("#AssociationPropertyList").val() != "") {
        val1 = "[" + val1 + "." + $("#AssociationPropertyList").val() + "]";
        val1Text = "[" + val1Text + "." + $("#AssociationPropertyList option:selected").text() + "]";
        CreateCriteriaForAssociation(entityName, condValue, val1, val1Text, val5, dynamicPropertyDispValue);
        return true;
    }
    var funcval = val1 + "," + val2 + "," + val3 + "," + val4 + "?";
    var val = $("#FilterCondition").val();
    val += val1 + "," + val2 + "," + val3 + "," + val4 + "?";
    if ($("#ValueTypeList option:selected").text() == "Dynamic" && dynamicPropertyDispValue != "") {
        val3 = dynamicPropertyDispValue;
    }
    str += "<td>" + val1Text + "</td>";
    str += "<td>" + val2 + "</td>";
    str += "<td>" + val3 + "</td>";
    str += "<td>" + val4 + "</td>";
    str += "<td><i name=\"FilterCondition\" onclick=\"deleteRowCriteria(this,'" + funcval + "');\" class=\"fa fa-trash\"></i></td>";
    var exMsg = VerifyPropertyAndValueDataType(entityName, val1, $("#ConditionValue").val(), val5, 'condition');
    if (exMsg != "" && exMsg != null) {
        $("#ConditionValue").val('');
        alert('Please enter correct Property Value. ' + exMsg);
        return false;
    }
    $("#FilterCondition").val(val);
    $('#tblConditionList').show();
    $('#tblConditionList').append(str);
    $("#DurationCount").val('');
    $("#ConditionValue").val('');
}
function CreateCriteriaForAssociationFunction(entityName, param, assocName, assocDispName, valueType, dynamicDispName) {
    var arrCond = param.split(",");
    var val = $("#FilterCondition").val();
    var operator = $("#OperatorList").val();
    var exMsg = VerifyPropertyAndValueDataType(entityName, assocName, param, valueType, 'condition');
    if (valueType != "Pick From List") {
        if (exMsg != "" && exMsg != null) {
            $("#ConditionValue").val('');
            alert('Data types of selected properties did not match. Please select compatible properties. ' + exMsg);
            return false;
        }
    }
    var logicalConnector = $("#LogicalConnectorList").val();
    if (valueType == "Pick From List") {
        logicalConnector = "OR";
    }
    for (i = 0; i < arrCond.length; i++) {
        if (i == arrCond.length - 1) {
            logicalConnector = $("#LogicalConnectorList").val();
        }
        if (valueType != "Dynamic")
            dynamicDispName = arrCond[i];
        val += assocName + ',' + operator + ',' + arrCond[i] + ',' + logicalConnector + "," + valueType + '?';
        var str = "<tr><td>" + assocDispName + "</td><td>" + operator + "</td><td>" + dynamicDispName + "</td><td>" + logicalConnector + "</td>";
        str += "<td><i name=\"FilterCondition\" onclick=\"deleteRowCriteria(this,'" + assocName + ',' + operator + ',' + dynamicDispName + ',' + logicalConnector + '' + "');\" class=\"fa fa-trash\"></i></td>";
        $("#FilterCondition").val(val);
        $('#tblConditionList').show();
        $('#tblConditionList').append(str);
        $("#ConditionValue").val('');
    }
}
function deleteRowCriteria(obj, val) {
    var obj_tr = $(obj).closest("tr");
    var newobj = $("#" + $(obj).attr("name")).val().replace(val, "");
    $("#" + $(obj).attr("name")).val(newobj);
    obj_tr.remove();
}
function FSearchConditionFunction() {
    $(':input').each(function () {
        var valid = $(this).attr("id");
        if (valid != 'LogicalConnectorList' && valid != 'ValueTypeList' && valid != 'DisplayLayout' && valid != 'inbuiltSearchCriteria') {
            if ($(this).val() != null && $(this).val().length > 0)
                $(this).css("border-color", "orange")
        }
    });
    $('.fsearch .btn').each(function () {
        if ($(this).attr("title") != null && $(this).attr("title") != 'ALL' && $(this).attr("title") != 'None selected') {
            $(this).attr("style", "border-color:orange !important;")
        }
    });
}
function VerifyPropertyAndValueDataTypeFunction(entityName, propertyName, conditionValue, valueType, actionType, dataurl) {
    var propDataType = "";
    $.ajax({
        url: dataurl + '?entityName=' + entityName + '&propertyName=' + propertyName + "&conditionValue=" + conditionValue + "&valueType=" + valueType + "&actionType=" + actionType,
        type: "GET",
        cache: false,
        async: false,
        success: function (result) {
            propDataType = result;
        }
    });
    return propDataType;
}
function DoubleClickRow(RecordID, Url) {
    var url = Url;//"@Url.Action("Details", "Order", new { id = "_Id", UrlReferrer = Request.Url, AssociatedType=ViewData["AssociatedType"], HostingEntityName = @Convert.ToString(ViewData["HostingEntity"]), HostingEntityID = @Convert.ToString(ViewData["HostingEntityID"]) }, null)".replace("_Id", RecordID);
    //window.location.replace(url);
    window.location.href = url;
}
function OpenNotes(url, fieldName, ctrl, ev) {
    if (ev.which == 3) {
        var Url = encodeURI(url.replace("_FieldName", fieldName).replace("_UIControlName", ctrl));
        OpenPopUpEntity('addPopup', 'Feedback', 'dvPopup', Url);
    }
}
function FillFromTimeSlot(obj, e) {
    var object = $(obj);
    var value = object.find('option:selected').text();
    var vals = value.split("-")
    $("[valuetype='T_StartTime']").val(vals[0]);
    $("[valuetype='T_EndTime']").val(vals[1]);
    if (object.val() != undefined && object.val() != '0' && object.val().length > 0) {
        $("[valuetype='T_StartTime']").attr("readonly", "readonly").attr("disabled", "disabled");
        $("[valuetype='T_EndTime']").attr("readonly", "readonly").attr("disabled", "disabled");
        $("[valuetype='T_EndTime']").next("span").css("pointer-events", "none");
        $("[valuetype='T_StartTime']").next("span").css("pointer-events", "none");
    }
    else {
        $("[valuetype='T_StartTime']").removeAttr("readonly").removeAttr("disabled");
        $("[valuetype='T_EndTime']").removeAttr("readonly").removeAttr("disabled");
        $("[valuetype='T_EndTime']").next("span").css("pointer-events", "auto");
        $("[valuetype='T_StartTime']").next("span").css("pointer-events", "auto");
    }
    return false;
}
function pagesizelistChange(e, EntityName, UserName, IsReports, fromview, rptId) {
    //remove pagination cookies 
    if ($.cookie("pagination" + UserName + EntityName) != null) {
        $.removeCookie("pagination" + UserName + EntityName);
        $.cookie("pagination" + UserName + EntityName, null, { path: '/' });
    }
    //
    var target;
    if (e.srcElement) target = e.srcElement;
    e = $.event.fix(e);
    if (e.currentTarget) target = e.currentTarget;
    var thelink = $(target).attr("Url");
    if (IsReports)
        thelink = addParameterToURL(thelink, "IsReports", IsReports)
    if (fromview)
        thelink = addParameterToURL(thelink, "FromViewReport", fromview)
    if (rptId)
        thelink = addParameterToURL(thelink, "rptId", rptId)


    var pagesizeCookie = "pageSize" + UserName + EntityName;
    if ($.cookie(pagesizeCookie) != null) {
        $.removeCookie(pagesizeCookie);
        $.cookie(pagesizeCookie, null, { path: '/' });
        $.removeCookie(pagesizeCookie, { path: thelink });
        $.removeCookie("pagination" + UserName + EntityName, { path: thelink });
    }
    var pageSizeValue = $(target).val();
    if (pageSizeValue > 10 || pageSizeValue == -1) {
        //   $.cookie(pagesizeCookie, pageSizeValue);
        $.cookie(pagesizeCookie, pageSizeValue, { path: '/' });
    }
    if ($.cookie(pagesizeCookie) != null)
        pageSizeValue = $.cookie(pagesizeCookie)
    $.ajax({
        url: thelink,
        cache: false,
        data: { searchString: $(target).closest('#SearchString' + EntityName).val(), itemsPerPage: pageSizeValue },
        success: function (data) {
            if (data != null) {
                try {
                    $(target).closest("#" + EntityName).html(data);
                    var filterbtn = $("#" + EntityName + "SetFSearchGridbtn");
                    if (filterbtn != undefined && GridQuery.length > 0)
                        filterbtn.click();
                } catch (ex) { }
            }
        }
    })
    return false;
}
function SortLinkClick(e, EntityName, IsReports, fromview, rptId) {
    var target;
    if (e.srcElement) target = e.srcElement;
    e = $.event.fix(e);
    if (e.currentTarget) target = e.currentTarget;
    var thelink = e.target.href;
    if (thelink == undefined)
        thelink = $("#" + target.id).attr("href");
    if (IsReports)
        thelink = addParameterToURL(thelink, "IsReports", IsReports)
    if (fromview)
        thelink = addParameterToURL(thelink, "FromViewReport", fromview)
    if (rptId)
        thelink = addParameterToURL(thelink, "rptId", rptId)

    eval("query = {" + thelink.split("?")[1].replace(/&/ig, "\",").replace(/=/ig, ":\"") + "\"};");
    e.preventDefault();
    e.stopPropagation();
    $.ajax({
        url: thelink,
        cache: false,
        data: { itemsPerPage: $("#pagesizelist" + EntityName, $(target).closest("#" + EntityName)).val() },
        success: function (data) {
            if (data != null) {
                try {
                    $(target).closest("#" + EntityName).html(data);
                    var filterbtn = $("#" + EntityName + "SetFSearchGridbtn");
                    if (filterbtn != undefined && GridQuery.length > 0)
                        filterbtn.click();
                } catch (ex) { }
                thelink = "";
            }
        }
    })
    return false;
}
function SearchClick(e, EntityName, Url, UserName) {
    //remove pagination cookies
    if ($.cookie("pagination" + UserName + EntityName) != null)
        $.removeCookie("pagination" + UserName + EntityName);
    //
    var target;
    if (e.srcElement) target = e.srcElement;
    e = $.event.fix(e);
    if (e.currentTarget) target = e.currentTarget;
    var thelink = Url;

    var searchval = $("#SearchString" + EntityName, $(target).closest("#" + EntityName)).val()
    searchval = SanitizeURLString(searchval);
    thelink = thelink.replace("_SearchString", searchval);

    $.ajax({
        url: thelink,
        cache: false,
        success: function (data) {
            if (data != null) {
                try {
                    $(target).closest("#" + EntityName).html(data);
                } catch (ex) { }
            }
        }
    })
    return false;
}
function PaginationClick(e, EntityName, UserName, IsReports, fromview, rptId) {
    var target;
    if (e.srcElement) target = e.srcElement;
    e = $.event.fix(e);
    if (e.currentTarget) target = e.currentTarget;
    var thelink = e.target.href;
    if (IsReports)
        thelink = addParameterToURL(thelink, "IsReports", IsReports)
    if (fromview)
        thelink = addParameterToURL(thelink, "FromViewReport", fromview)
    if (rptId)
        thelink = addParameterToURL(thelink, "rptId", rptId)
    if (thelink != '') {
        var queryStr = eval("query = {" + thelink.split("?")[1].replace(/&/ig, "\",").replace(/=/ig, ":\"") + "\"};");
        paginationcookies(e, EntityName, UserName, queryStr.page)
        e.preventDefault();
        e.stopPropagation();
        $.ajax({
            url: thelink,
            cache: false,
            data: { itemsPerPage: $("#pagesizelist" + EntityName, $(target).closest("#" + EntityName)).val() },
            success: function (data) {
                if (data != null) {
                    try {
                        $(target).closest("#" + EntityName).html(data);
                        var filterbtn = $("#" + EntityName + "SetFSearchGridbtn");
                        if (filterbtn != undefined && GridQuery.length > 0)
                            filterbtn.click();
                    } catch (ex) { }
                }
            }
        })
    }
    return false;
}
function paginationcookies(e, EntityName, UserName, page) {
    var target;
    if (e.srcElement) target = e.srcElement;
    e = $.event.fix(e);
    if (e.currentTarget) target = e.currentTarget;
    var thelink = $(target).attr("Url");
    var paginationCookie = "pagination" + UserName + EntityName;

    if ($.cookie(paginationCookie) != null) {
        $.removeCookie(paginationCookie);
        $.cookie(paginationCookie, null, { path: '/' });
        $.removeCookie(paginationCookie, { path: thelink });
    }
    var paginationValue = page;
    //$.cookie(paginationCookie, paginationValue);
    $.cookie(paginationCookie, paginationValue, { path: '/' });
    if ($.cookie(paginationCookie) != null)
        paginationValue = page;
}
function showhideColumns(e, EntityName, url) {
    var dvnameforshowhide = $("#dvShowHide" + EntityName);
    if (dvnameforshowhide != undefined) {
        dvnameforshowhide.load(url);
        dvnameforshowhide.show();
    //var target;
    //if (e.srcElement) target = e.srcElement;
    //e = $.event.fix(e);
    //if (e.currentTarget) target = e.currentTarget;
    //var div = $("#ColumnShowHide" + EntityName, $(target).closest("#" + EntityName));
    //var lbl = $("#lbl" + EntityName, $(target).closest("#" + EntityName));
    //if (div.hasClass('collapse')) {
    //    div.toggleClass('in');
    //    lbl.css('display', 'none');
    //}
    //else {
    //    div.toggleClass('collapse');
    //    lbl.css('display', 'block');
    }
}
function showhideSaveCookies(e, EntityName, UserName, HostingEntity) {
    var target;
    if (e.srcElement) target = e.srcElement;
    e = $.event.fix(e);
    if (e.currentTarget) target = e.currentTarget;
    var myCookie = UserName + EntityName + HostingEntity;
    if ($.cookie(myCookie) != null) {
        $.removeCookie(myCookie);
    }
    var selected = [];
    var uncheckedcolcnt = 0;
    $('#ColumnShowHide' + EntityName + ' input[type=checkbox]', $(target).closest("#" + EntityName)).each(function () {
        if ($(this).prop('checked') == false) {
            selected.push($(this).attr('name'));
            uncheckedcolcnt++;
        }
    });
    var allcollength = $('#ColumnShowHide' + EntityName + ' input[type=checkbox]', $(target).closest("#" + EntityName)).length - 1;
    if (uncheckedcolcnt == allcollength) {
        $('#lblWarning' + EntityName, $(target).closest("#" + EntityName)).css('display', 'block');
        $('#lbl' + EntityName, $(target).closest("#" + EntityName)).css('display', 'none');
        return;
    }
    if (selected != "") {
        $.cookie(myCookie, selected);
        $('#lbl' + EntityName, $(target).closest("#" + EntityName)).css('display', 'block');
        $('#lblWarning' + EntityName, $(target).closest("#" + EntityName)).css('display', 'none');
    }
    if (selected == "" && uncheckedcolcnt == 0) {
        $('#lblWarning' + EntityName, $(target).closest("#" + EntityName)).css('display', 'none');
        $('#lbl' + EntityName, $(target).closest("#" + EntityName)).css('display', 'none');
    }
}
function ColumnClick(e, EntityName) {
    var target;
    if (e.srcElement) target = e.srcElement;
    e = $.event.fix(e);
    if (e.currentTarget) target = e.currentTarget;
    var index = $(target).attr('name').substr(3);
    //index--;
    $('table tr', $(target).closest("#" + EntityName)).each(function () {
        $('td:eq(' + index + ')', this).toggle();
    });
    $('th.' + $(target).attr('name'), $(target).closest("#" + EntityName)).toggle();
    var divarr = $('div', $(target).closest("#" + EntityName))
    if (divarr != undefined) {
        divarr.each(function () {
            var innerDiv = $('div.' + 'col' + index, $("#" + EntityName))
            innerDiv.each(function () {
                if (this.style.display == 'none') {
                    this.style.display = "block";
                }
                else {
                    this.style.display = "none";
                }

            });
        });
    }
}
function FSearchColumnsShowHide(indexes, EntityName) {
    if (indexes.length > 0) {
        var indexlist = indexes.split(',')
        for (var i = 0; i < indexlist.length; i++) {
            $('table tbody tr', $("#" + EntityName)).each(function () {
                $('td:eq(' + indexlist[i] + ')', this).toggle();
            });
            $('th.' + 'col' + indexlist[i], $("#" + EntityName)).toggle();
        }
    }
}


function FSearchColumnsShowHideGalaryList(indexes, EntityName) {
    if (indexes.length > 0) {
        var indexlist = indexes.split(',')
        for (var i = 0; i < indexlist.length; i++) {
            var divarr = $('div.' + 'col' + indexlist[i], $("#" + EntityName))
            divarr.each(function () {
                this.style.display = "none";
            });
        }
    }
}
function FillDropdownMobile(hostingentity) {
    //hostingentity = hostingentity.id;
    var selectedval = $("option:selected", $("#" + hostingentity)).val();
    var finalUrl = $("#" + hostingentity).attr("dataurl");
    var urlGetAll = $("#" + hostingentity).attr("dataurl").replace("GetAllValueMobile", "Index");
    urlGetAll = addParameterToURL(urlGetAll, 'BulkOperation', "single");
    var parentDDid = $("#" + hostingentity).attr("ParentDD");
    var hostingname = $("#" + hostingentity).attr("hostingname");
    var AssociationNames = $("#" + hostingentity).attr("AssoNameWithParent");
    var associationParam = "";
    if (parentDDid != null || parentDDid != undefined) {
        var Parents = parentDDid.split(",");
        var AssociationNameWithParent = "";
        var selectedParentVal = "";
        var parent = "";
        for (var i = 0; i < Parents.length; i++) {
            if ($("option:selected", $("#" + Parents[i])).val().length > 0) {
                AssociationNameWithParent = AssociationNames.split(",")[i];
                selectedParentVal = $("option:selected", $("#" + Parents[i])).val();
                parent = Parents[i];
            }
        }
        var IsReverse = $("#" + hostingentity).attr("IsReverse");
        if (IsReverse == "true" || IsReverse == "True") {
            if (selectedParentVal.length > 0) {
                var parent1 = $("#" + parent).attr("HostingName");
                var parentUrl = $("#" + parent).attr("dataurl").replace("GetAllValueMobile", "GetPropertyValueByEntityId");
                parentUrl = addParameterToURL(parentUrl, "Id", selectedParentVal);
                parentUrl = addParameterToURL(parentUrl, "PropName", AssociationNameWithParent);
                $.ajax({
                    type: "GET",
                    async: false,
                    url: parentUrl,
                    success: function (jsonObj) {
                        if (selectedParentVal.length > 0)
                            finalUrl = addParameterToURL(finalUrl, 'AssociationID', jsonObj);
                        if (AssociationNameWithParent.length > 0)
                            finalUrl = addParameterToURL(finalUrl, 'AssoNameWithParent', "Id");
                    }
                });
            }
        }
        else {
            if (AssociationNameWithParent.length > 0)
                finalUrl = addParameterToURL(finalUrl, 'AssoNameWithParent', AssociationNameWithParent);
            if (selectedParentVal.length > 0)
                finalUrl = addParameterToURL(finalUrl, 'AssociationID', selectedParentVal);
        }
    }
    if (parentDDid != null || parentDDid != undefined) {
        var Parents = parentDDid.split(",");
        var AssociationNameWithParent = "";
        var selectedParentVal = "";
        var hostingnameofparent = "";
        var parentdd = "";
        for (var i = 0; i < Parents.length; i++) {
            if ($("option:selected", $("#" + Parents[i])).val().length > 0) {
                AssociationNameWithParent = AssociationNames.split(",")[i];
                selectedParentVal = $("option:selected", $("#" + Parents[i])).val()
                parentdd = Parents[i];
            }
        }
        if (parentdd.length > 0)
            urlGetAll = addParameterToURL(urlGetAll, 'HostingEntity', $("#" + parentdd).attr("hostingname"));
        if (AssociationNameWithParent.length > 0)
            urlGetAll = addParameterToURL(urlGetAll, 'AssociatedType', AssociationNameWithParent.substring(0, AssociationNameWithParent.length - 2));
        if (selectedParentVal.length > 0)
            urlGetAll = addParameterToURL(urlGetAll, 'HostingEntityID', selectedParentVal);
    }
    var dispName = ($("label[for=\"" + hostingentity + "\"]").text());
    var bulkurl = "value=\"'PopupBulkOperation','" + hostingentity + "','" + dispName + "','dvPopupBulkOperation','" + urlGetAll + "'\"";
    $.ajax({
        type: "GET",
        url: finalUrl,
        contentType: "application/json; charset=utf-8",
        global: false,
        async: false,
        cache: false,
        dataType: "json",
        success: function (jsonObj) {
            var listItems = "";
            $("#" + hostingentity).empty();
            if (selectedval != '')
                listItems += "<option value=''>--None--</option>";
            else
                listItems += "<option selected='selected' value=''>--None--</option>";
            for (i in jsonObj) {
                if (jsonObj[i].Id != undefined && jsonObj[i].Name != undefined) {
                    if (selectedval == jsonObj[i].Id)
                        listItems += "<option selected='selected' value='" + jsonObj[i].Id + "'>" + jsonObj[i].Name + "</option>";
                    else
                        listItems += "<option value='" + jsonObj[i].Id + "'>" + jsonObj[i].Name + "</option>";
                }
            }
            if (jsonObj.length >= 10)
                listItems += "<option " + bulkurl + ">ViewAll</option>"
            $("#" + hostingentity).html(listItems);
        }
    });
}
function openbulk(ele) {
    var value = $(ele).val();
    var selectedtext = $(ele).find('option:selected').text();
    if (selectedtext.toLowerCase() == "viewall") {
        var split = value.split(",");
        var arg1 = split[0].replace(/'/g, '');
        var arg2 = split[1].replace(/'/g, '');
        var arg3 = split[2].replace(/'/g, '');
        var arg4 = split[3].replace(/'/g, '');
        var arg5 = split[4].replace(/'/g, '');
        OpenPopUpBulkOperation(arg1, arg2, arg3, arg4, arg5);
        $("#" + ele.id + " option:contains(--None--)").prop({ selected: true });
    }
}
$(document).ready(function () {
    $(".tip-top").tooltip({
        placement: 'top'
    });
    $(".tip-right").tooltip({
        placement: 'right'
    });
    $(".tip-bottom").tooltip({
        placement: 'bottom'
    });
    $(".tip-left").tooltip({
        placement: 'left'
    });
});
//open image for mobile......
function OpenPopUpImage(e, pop, pic) {
    e.preventDefault();
    var maxHeight = $(window).height() + "px";
    $("#" + pop).css("max-height", maxHeight);
    $("#" + pop + " img").css("max-height", maxHeight);
    $("#" + pic).modal('show');
    var maxWidth = $("#" + pop).width() + "px";
    $("#" + pop + " img").css("width", maxWidth);
}
function OpenPopUpImageByte(e, pop, pic, crlimg, docid, rowId) {
    var mypop = "<div class='modal fade' style='cursor:default;' onclick=ClosePopUpImage(event,'Picture_pop_" + rowId + "')" +
        " id='" + pic + "' tabindex='-1' role='dialog' aria-hidden='true'><br />" +
        " <div class='modal-dialog'>" +
        " <div class='modal-content'>" +
        " <div class='modal-header'>" +
        " <button type='button' id='close_Picture_" + rowId + "' onclick=ClosePopUpImage(event,'Picture_pop_" + rowId + "')" +
        " data-dismiss='modal' class='close' aria-hidden='true'>&times;" +
        " </button>" +
        " <div id='" + pop + "'>" +
        " <img id='" + crlimg + "' style='min-width:100%' /> </div></div></div></div></div>";
    $("#popupDiv").html(mypop);

    e.preventDefault();
    var loc = window.location;
    var pathName = loc.pathname.substring(0, loc.pathname.lastIndexOf('/') + 1);
    $("#" + crlimg).attr("src", pathName + "Document/DisplayImageAfterClick/" + docid);
    var maxHeight = $(window).height() + "px";
    $("#" + pop).css("max-height", maxHeight);
    $("#" + pop + " img").css("max-height", maxHeight);
    $("#" + pic).modal('show');
    var maxWidth = $("#" + pop).width() + "px";
    $("#" + pic + " img").css("width", maxWidth);
}

function ClosePopUpImage(e, pic) {
    e.preventDefault();
    $("#" + pic).modal('hide');
}
function hideShowmore(e) {
    e.preventDefault();
}
function SelectAllRows(obj, entityname) {
    var checked = false;
    var table = $(obj).closest("#Des_Table");
    if ($(obj).is(":checked"))
        checked = true;
    else {
        $("#SelectedItems", table).val('');
    }
    $('input[type=checkbox]', table.find("tr").find("td:first")).each(function () {
        if (this != obj && !($(this).is(':disabled'))) {
            $(this).prop("checked", checked);
            SelectForBulkOperation(this, $(this).attr("id"), entityname);
        }
    });
}
function SelectForBulkOperation(source, id, entityName) {
    var table = $(source).closest("#Des_Table");
    var selectedobj = $("#SelectedItems", table);
    var value = selectedobj.val();
    if (value.length < 1 || value == undefined)
        value = ",";
    if ($(source).is(":checked"))
        value += id + ",";
    else
        value = value.replace("," + id + ",", ",");
    selectedobj.val(value);
}

function CommonSelectAllRows(obj, type) {
    var checked = false;
    var table = $(obj).closest("div").next("#" + type);
    if ($(obj).is(":checked"))
        checked = true;
    else {
        $("#SelectedItems", table).val('');
    }
    $('input[type=checkbox]', table.find(".tagline.tagline-promo-40")).each(function () {
        if (this != obj && !($(this).is(':disabled'))) {
            $(this).prop("checked", checked);
            CommonSelectForBulkOperation(this, $(this).attr("id"), type);
        }
    });
}
function CommonSelectForBulkOperation(source, id, type) {
    var table = $(source).closest("#" + type);
    var selectedobj = $("#SelectedItems", table);
    var value = selectedobj.val();
    if (value.length < 1 || value == undefined)
        value = ",";
    if ($(source).is(":checked"))
        value += id + ",";
    else
        value = value.replace("," + id + ",", ",");
    selectedobj.val(value);
}
function SetSingle(source, id, DisplayValue) {
    var dropdown = ($('#PopupBulkOperationLabel').attr('class'));
    if ($('#' + dropdown + ' option[value="' + id + '"]').length == 0) {
        $('#' + dropdown).append($('<option selected=\'selected\'></option>').val(id).html(DisplayValue));
        $("#" + dropdown).trigger('chosen:updated');
    }
    $("#" + dropdown).val(id);
    $("#" + dropdown).trigger('chosen:updated');
    $("#" + dropdown).change();
    $("#closePopupBulkOperation").click();
}
function Update(source, id, DisplayValue) {
    val1 = $("#idvalues").val();
    if (source.checked) {
        $("#idvalues").val(val1 + "," + id);
    }
    else {
        $("#idvalues").val(val1.replace("," + id, ""));
    }
}
function UpdateRecordsFunction(selectedvalues, selectedvalues, url1, entity, AssociatedType, HostingEntity, HostingEntityID) {
    var host = AssociatedType;
    $.ajax({
        type: "POST",
        data: { ids: selectedvalues, AssociatedType: AssociatedType, HostingEntity: HostingEntity, HostingEntityID: HostingEntityID },
        url: url1,
        complete: function (msg) {
            $("#closePopupBulkOperation").click();
        },
        success: function (msg) {
            if (host != undefined && host.length > 0 && $('#' + host).length > 0) {
                $('#' + entity + 'SearchCancel', $('#' + host)).click();
                $('#dvcnt_' + host).load(location.href + " #dvcnt_" + host);
            }
        }
    });
}
function LockHostDropdownOnCreate(hostingEntityName) {
    try {
        if (hostingEntityName != null && hostingEntityName.length > 0) {
            $('#' + hostingEntityName + 'ID').attr("lock", "true");
            $('#' + hostingEntityName + 'ID').trigger("change");
        }
    }
    catch (ex) { }
}
function LockHostDropdownOnEdit(hostingEntityName) {
    try {
        if (hostingEntityName != null && hostingEntityName.length > 0) {
            $('#' + hostingEntityName + 'ID').attr("lock", "true");
            $("input[type='radio'][name='" + hostingEntityName + "ID']").each(function () {
                if (!this.checked)
                    this.closest("li").style.display = "none";
            });
        }
    }
    catch (ex) { }
}
function LockHostDropdownOnEditQuick(hostingEntityName, IsFilter) {
    if (hostingEntityName != null && hostingEntityName.length > 0) {
        //if (IsFilter != "False")
            $('#' + hostingEntityName + 'ID').attr("lock", "true");
        $('#' + hostingEntityName + 'ID').trigger("change");
        $("input[type='radio'][name='" + hostingEntityName + "ID']").each(function () {
            if (!this.checked)
                this.closest("li").style.display = "none";
        });
    }
}
function LockHostDropdownOnCreateQuick(hostingEntityName, hostingValue) {
    try {
        if (hostingEntityName != null && hostingEntityName.length > 0) {
            $('#' + hostingEntityName + 'ID').attr("lock", "true");
            var label = $('#addPopupLabel').html() + hostingValue;
            $('#addPopupLabel').html(label);
        }
    }
    catch (ex) { }
}
function RecycleActionSelected(obj, entity, action, url1) {
    var val = $("#" + entity).find("#SelectedItems").val().substr(1).split(",");
    if ($.trim(val).length == 0) {
        alert("Please select at least one item."); return true;
    }
    var r = confirm("Do you want to really execute " + action + "!");
    if (r == true) {
        $.ajax({
            type: "POST",
            data: { ids: val },
            url: url1,
            success: function (msg) {
                //alert(msg);
                OpenAlertPopUp(action, msg);
                $("#" + entity + "SearchCancel").click();
            }
        });
    }
    else {
        return true;
    }
}
function RecycleActions(obj, entity, action, url1) {
    var r = confirm("Do you want to really execute " + action + "!");
    if (r == true) {
        $.ajax({
            type: "POST",
            url: url1,
            success: function (msg) {
                OpenAlertPopUp(action, msg);;
                $("#" + entity + "SearchCancel").click();
            }
        });
    }
    else {
        return true;
    }
}
function PerformBulkOperation(obj, entity, action, url1, host) {
    debugger;
    var val = $("#" + entity).find("#SelectedItems").val().substr(1).split(",");
    if ($.trim(val).length == 0) {
        alert('Please select at least one record!');
        return true;
    }
    var r = confirm("Do you really want to execute " + action + "?");
    if (r == true) {
        $.ajax({
            type: "POST",
            data: { ids: val },
            url: url1,
            success: function (msg) {
                if (msg.result != undefined && msg.result != "Success" && msg.message != undefined) {
                    alert(msg.message);
					//$("#" + entity + "SearchCancel").click();
                    //return true;
                    location.reload();
                }
				
				  if (msg.isRedirect) {
                    if (msg.newtab) {
                        if (msg.result == "Success")
                            alert(msg.message);
                        window.open(msg.redirectUrl, '_blank');
                        if (msg.result == "Success")
                            location.reload();

                    }
                    else
                        window.location.href = msg.redirectUrl;
                    return false;
                }

                if (host != undefined && host.length > 0)
                    $('#dvcnt_' + host).load(location.href + " #dvcnt_" + host);
                $("#" + entity + "SearchCancel").click();
                if (msg.msg != undefined && msg.msg == "File") {
                    window.location = "" + entity + "\\Download?FileName=" + msg.filename;
                }
            }
        });
    }
    else {
        return true;
    }
}
function PerformBulkOperationDownLoadDissolve(obj, entity, action, url1, Isflat) {
    var val = $("#" + entity).find("#SelectedItems").val().substr(1).split(",");
    if ($.trim(val).length == 0) {
        alert('Please select at least one record!');
        return true;
    }
    //url1 = url1 + "?Ids=" + val + "&Isflat=" + Isflat;
    $("#BulkDownLoad" + entity).hide();
    $.ajax({
        type: "POST",
        data: { Ids: val, Isflat: Isflat },
        url: url1,
        success: function (result) {
            debugger;
            if (result.result != "fail") {
                $("#BulkDownLoad" + entity).hide();
                $("#" + entity + "SearchCancel").click();
                var objTarget = $("#BulkDownLoad" + entity);
                var bulkdataurl = objTarget.attr('href');
                objTarget.attr('href', url1 + "?Ids=" + val + "&Isflat=" + Isflat);
                (objTarget[0]).click();
            }
            else {
                alert('Please select at least one record!');
                $("#BulkDownLoad" + entity).removeAttr('href');
                $("#BulkDownLoad" + entity).show();
                $("#" + entity + "SearchCancel").click();
                return true;
            }
        }
    });
}
function PerformBulkOperationDownLoad(obj, entity, action, url1, Isflat) {
    
    var val = $("#" + entity).find("#SelectedItems").val().substr(1).split(",");
    if ($.trim(val).length == 0) {
        alert('Please select at least one record!');
        return true;
    }
    $("#BulkDownLoad" + entity).hide();
    $("#" + entity + "SearchCancel").click();
    var objTarget = $("#BulkDownLoad" + entity);
    var bulkdataurl = objTarget.attr('href');
    objTarget.attr('href', url1 + "?Ids=" + val + "&Isflat=" + Isflat);
    (objTarget[0]).click();
}
function PerformBulkOperationExportData(obj, entity, action, url1) {

    var val = $("#" + entity).find("#SelectedItems").val().substr(1).split(",");
    if ($.trim(val).length == 0) {
        alert('Please select at least one record!');
        return true;
    }
    url1 = url1.replace("IDSLST", val);
    OpenPopUpEntity('addPopup', 'Note', 'dvPopup', url1);
}
function OpenPopUpBulKUpdate(Popup, EntityName, dvName, url, Entity) {
    $("#" + Popup + 'Label').html();
    if ((EntityName.indexOf("Update") == -1) && (EntityName.indexOf("Delete") == -1)) {
        $("#" + Popup + 'Label').html("Add " + EntityName);
    }
    else {
        $("#" + Popup + 'Label').html(EntityName);
    }
    var val = $("#" + Entity).find("#SelectedItems").val().substr(1).split(",");
    url = url + "&ids=" + val;
    $("#" + Popup).modal('show');
    $("#" + dvName).html('');
    $("#" + dvName + "Error").html('');
    $("#" + dvName).load(url);
}

function ExcuteSingleVerb(EntityName, obj) {
    var url1 = $(obj).attr("dataurl");
    $(obj).addClass("disabled");
    $.ajax({
        url: url1,
        cache: false,
        success: function (data) {
            if (data.isRedirect) 
			{
				$(obj).removeClass("disabled");
                if (data.newtab) {
                    if (data.result == "Success")
                        alert(data.message);
                    window.open(data.redirectUrl, '_blank');
                    if (data.result == "Success")
                        location.reload();

                }
                else
				{
				    window.location.href = data.redirectUrl;
				}
                return false;
            }
            if (data.result == "Success") {
                alert(data.message);
				$(obj).removeClass("disabled");
                location.reload();
            }
            else if (data.result == "Failure") {
                $(obj).removeClass("disabled");
                alert(data.message);
            }
            else if (data == "Success") {
                alert('Action executed sucessfully.');
				$(obj).removeClass("disabled");
                location.reload();
            } else {
                $(obj).removeClass("disabled");
                alert('Some error in executing action!');
            }
        },
        error: function (jqXHR, textStatus, errorThrown) {
            $(obj).removeClass("disabled");
            alert(jqXHR);
        }
    });
}

function ExcuteSyncData(EntityName, obj) {
    var url1 = $(obj).attr("dataurl");
    $(obj).addClass("disabled");

    $('#syncDataMessage').css("display", "block");
    $('#liSyncData').attr("title", "Sync running");
    //$(obj).closest("li").attr("title", "Sync running");
    $(obj).find('em').removeClass("fa fa-cloud-download-alt");
    $(obj).find('em').addClass("fa fa-spinner fa-spin");

    $.ajax({
        url: url1,
        cache: false,
        success: function (data) {
            if (data.isRedirect) {
                if (data.newtab) {
                    if (data.result == "Success")
                        alert(data.message);
                    window.open(data.redirectUrl, '_blank');
                    if (data.result == "Success")
                        location.reload();

                }
                else {
                    window.location.href = data.redirectUrl;
                }
                return false;
            }
            if (data.result == "Success") {
                alert(data.message);
                location.reload();
            }
            else if (data.result == "Failure") {
                $(obj).removeClass("disabled");
                $(obj).find('em').removeClass("fa fa-spinner fa-spin");
                $(obj).find('em').addClass("fa fa-cloud-download-alt");
                $('#syncDataMessage').css("display", "none");
                $('#liSyncData').removeAttr("title");

                alert(data.message);
            }
            else if (data == "Success") {
                alert('Action executed sucessfully.');
                location.reload();
            } else {
                alert('Some error in executing action!')
            }
        },
        error: function (jqXHR, textStatus, errorThrown) {
            $(obj).removeClass("disabled");
            $(obj).find('em').removeClass("fa fa-spinner fa-spin");
            $(obj).find('em').addClass("fa fa-cloud-download-alt");
            $('#syncDataMessage').css("display", "none");
            $('#liSyncData').removeAttr("title");

            alert(jqXHR);

        }
    });
}

//open Quickedit on click of idex records
var IsClicked = false;
function OpenQuickEdit(entityName, RecId, e) {
    if (e.target.tagName != "TD") {
        //e.preventDefault();
        return false;
    }
    if (!IsClicked) {
        e.preventDefault();
        IsClicked = true;
        if ($("#aBtnQuickEdit" + entityName + "_" + RecId).length != 0)
            $("#aBtnQuickEdit" + entityName + "_" + RecId).click();
    }
    IsClicked = false;
    return false;
}
//
//
function bindpages(obj, viewTemplates, listTemplates, editTemplates, searchTemplates, createTemplates, HomePageTemplates) {
    var url1 = $(obj).attr("dataurl");
    url1 = url1 + "?EntityName=" + $(obj).val();
    $.ajax({
        url: url1,
        cache: false,
        success: function (result) {
            var indexpage = result.IndexPages;
            var detailspage = result.DetailsPage;
            var editpage = result.EditPage;
            var searchpage = result.SearchPage;
            var createpage = result.CreatePage;
            var homepage = result.HomePage;
            //
            var viewItems = "";
            var listItems = "";
            var editItems = "";
            var searchItems = "";
            var createItems = "";
            var homeItems = "";
            $("#" + viewTemplates).empty();
            $("#" + listTemplates).empty();
            $("#" + editTemplates).empty();
            $("#" + searchTemplates).empty();
            $("#" + createTemplates).empty();
            $("#" + HomePageTemplates).empty();

            $("#" + viewTemplates).html("<option value=''>--Select Details--</option>");
            $("#" + listTemplates).html("<option value=''>--Select List--</option>");
            $("#" + editTemplates).html("<option value=''>--Select Edit--</option>");
            $("#" + searchTemplates).html("<option value=''>--Select Search--</option>");
            $("#" + createTemplates).html("<option value=''>--Select Create--</option>");
            $("#" + HomePageTemplates).html("<option value=''>--Select Home--</option>");

            //
            //List page
            $.each(indexpage, function (dispayvalue, value) {
                if (dispayvalue == "Table(Default)")
                    listItems += "<option selected='selected' value='" + value + "'>" + dispayvalue + "</option>";
                else
                    listItems += "<option value='" + value + "'>" + dispayvalue + "</option>";

            });
            $("#" + listTemplates).append(listItems);
            //
            //Details page
            $.each(detailspage, function (dispayvalue, value) {
                if (dispayvalue == "(Detail)Default")
                    viewItems += "<option selected='selected' value='" + value + "'>" + dispayvalue + "</option>";
                else {
                    viewItems += "<option value='" + value + "'>" + dispayvalue + "</option>";
                }

            });
            $("#" + viewTemplates).append(viewItems);
            //
            //edit page
            $.each(editpage, function (dispayvalue, value) {
                if (dispayvalue == "(Edit)Default")
                    editItems += "<option selected='selected' value='" + value + "'>" + dispayvalue + "</option>";
                else
                    editItems += "<option value='" + value + "'>" + dispayvalue + "</option>";

            });
            $("#" + editTemplates).append(editItems);
            //
            //search page
            $.each(searchpage, function (dispayvalue, value) {
                if (dispayvalue == "Faceted Search(Default)")
                    searchItems += "<option selected='selected' value='" + value + "'>" + dispayvalue + "</option>";
                else
                    searchItems += "<option value='" + value + "'>" + dispayvalue + "</option>";

            });
            $("#" + searchTemplates).append(searchItems);
            //
            //create page
            $.each(createpage, function (dispayvalue, value) {
                if (dispayvalue == "(Create)Default")
                    createItems += "<option selected='selected' value='" + value + "'>" + dispayvalue + "</option>";
                else
                    createItems += "<option value='" + value + "'>" + dispayvalue + "</option>";

            });
            $("#" + createTemplates).append(createItems);
            //
            //home page
            $.each(homepage, function (dispayvalue, value) {
                if (dispayvalue == "(Home)Default")
                    homeItems += "<option selected='selected' value='" + value + "'>" + dispayvalue + "</option>";
                else
                    homeItems += "<option value='" + value + "'>" + dispayvalue + "</option>";

            });
            $("#" + HomePageTemplates).append(homeItems);
            //
        }
    })
}
function bindpagesFromEdit(obj, viewTemplates, listTemplates, editTemplates, searchTemplates, createTemplates, homeTemplates, details, list, edit, search, create, home) {
    var url1 = $(obj).attr("dataurl");
    url1 = url1 + "?EntityName=" + $(obj).val();
    $.ajax({
        url: url1,
        cache: false,
        success: function (result) {
            var indexpage = result.IndexPages;
            var detailspage = result.DetailsPage;
            var editpage = result.EditPage;
            var searchpage = result.SearchPage;
            var createpage = result.CreatePage;
            var HomePage = result.HomePage;
            //
            var viewItems = "";
            var listItems = "";
            var editItems = "";
            var searchItems = "";
            var createItems = "";
            var homeItems = "";
            $("#" + viewTemplates).empty();
            $("#" + listTemplates).empty();
            $("#" + editTemplates).empty();
            $("#" + searchTemplates).empty();
            $("#" + createTemplates).empty();
            $("#" + homeTemplates).empty();
            if (details == "")
                $("#" + viewTemplates).html("<option value=''>--Select Details--</option>");
            if (list == "")
                $("#" + listTemplates).html("<option value=''>--Select List--</option>");
            if (edit == "")
                $("#" + editTemplates).html("<option value=''>--Select Edit--</option>");
            if (search == "")
                $("#" + searchTemplates).html("<option value=''>--Select Search--</option>");
            if (search == "")
                $("#" + createTemplates).html("<option value=''>--Select Create--</option>");
            if (search == "home")
                $("#" + HomePageTemplates).html("<option value=''>--Select Home--</option>");
            //
            //List page
            $.each(indexpage, function (dispayvalue, value) {
                listItems += "<option value='" + value + "'>" + dispayvalue + "</option>";

            });
            $("#" + listTemplates).append(listItems);
            $("#" + listTemplates + " option[value='" + list + "']").prop('selected', true);
            //
            //Details page
            $.each(detailspage, function (dispayvalue, value) {
                viewItems += "<option value='" + value + "'>" + dispayvalue + "</option>";

            });
            $("#" + viewTemplates).append(viewItems);
            $("#" + viewTemplates + " option[value='" + details + "']").prop('selected', true);
            //
            //edit page
            $.each(editpage, function (dispayvalue, value) {
                editItems += "<option value='" + value + "'>" + dispayvalue + "</option>";

            });
            $("#" + editTemplates).append(editItems);
            $("#" + editTemplates + " option[value='" + edit + "']").prop('selected', true);
            //
            //search page
            $.each(searchpage, function (dispayvalue, value) {
                searchItems += "<option value='" + value + "'>" + dispayvalue + "</option>";

            });
            $("#" + searchTemplates).append(searchItems);
            $("#" + searchTemplates + " option[value='" + search + "']").prop('selected', true);
            //
            //create page
            $.each(createpage, function (dispayvalue, value) {
                createItems += "<option value='" + value + "'>" + dispayvalue + "</option>";

            });
            $("#" + createTemplates).append(createItems);
            $("#" + createTemplates + " option[value='" + create + "']").prop('selected', true);
            //
            //Home page
            $.each(HomePage, function (dispayvalue, value) {
                homeItems += "<option value='" + value + "'>" + dispayvalue + "</option>";
            });
            $("#" + homeTemplates).append(homeItems);
            $("#" + homeTemplates + " option[value='" + home + "']").prop('selected', true);
            //
        }
    })
}
//
function displayDocumentName(url, Id, propname) {
    debugger;
    $.get(url, {}, function (result) {
        $("#adownload" + propname + Id).html(result);
    }, "json");
}
function displayDocumentNameEdit(url, Id, propname) {
    $.get(url, {}, function (result) {
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

        if (docName == "NA")
        {
            $("#aDelete" + propname + Id).css("display", "none");
        }

        $("#adownloadEdit" + propname + Id).html(docName);
        $("#adownloadEdit" + propname + Id).attr("title", result)
    }, "json");
}

function DocumentDeassociate(url, Id, propname) {
    $.ajax({
        url: url + "&Id=" + Id + "&propname=" + propname,
        cache: false,
        success: function (data) {
            if (data != null) {
                if (data == "Success") {
                    $("#" + propname).val("");
                    $("#aDelete" + propname + Id).css("display", "none");
                    $("#adownloadEdit" + propname + Id).html("");
                    //$("#" + propname).val('');
                    location.reload();

                }
            }
        }
    })
}

function displayDocumentNameDetail(url, Id, propname) {
    $.get(url, {}, function (result) {
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

        $("#adownloadDetail" + propname + Id).html(docName);
        $("#adownloadDetail" + propname + Id).attr("title", result)
    }, "json");
}
function displayDocumentNameEditImageGalary(url, Id, propname) {
    $.get(url, {}, function (result) {
        var result = result.substr(result.lastIndexOf('\\') + 1)
        $("#adownloadGalary" + propname + Id).prepend(result);
        $("#adownloadGalary" + propname + Id).attr("title", result)
    }, "json");
}
function displayDocumentName(url, Id, propname) {
    $.get(url, {}, function (result) {
        var result = result.substr(result.lastIndexOf('\\') + 1)
        $("#gallcap" + Id).html(result);
    }, "json");
}
//

function uploadedFileName(dispalyobjid, result) {
    var docName;
    var result = result.substr(result.lastIndexOf('\\') + 1)
    var fname = result.substr(0, result.lastIndexOf('.'))
    var ext = result.substr(result.lastIndexOf('.') + 1)
    var len = fname.length;

    if (len > 15) {
        docName = fname.substr(0, 15);
        docName = docName + ".." + ext;
    }
    else
        docName = result;

    $("#" + dispalyobjid).html(docName);
    $("#" + dispalyobjid).attr("title", result)
}
function SanitizeURLString(searchString) {
    var Results = "";
    if (searchString == undefined) {
        searchString=""
    }
    Results = searchString;
    Results = Results.replace("%", "%25")
    Results = Results.replace("<", "%3C")
    Results = Results.replace(">", "%3E")
    Results = Results.replace("#", "%23")
    Results = Results.replace("{", "%7B")
    Results = Results.replace("}", "%7D")
    Results = Results.replace("|", "%7C")
    Results = Results.replace("\\", "%5C")
    Results = Results.replace("^", "%5E")
    Results = Results.replace("~", "%7E")
    Results = Results.replace("[", "%5B")
    Results = Results.replace("]", "%5D")
    Results = Results.replace("`", "%60")
    Results = Results.replace(";", "%3B")
    Results = Results.replace("/", "%2F")
    Results = Results.replace("?", "%3F")
    Results = Results.replace(":", "%3A")
    Results = Results.replace("@", "%40")
    Results = Results.replace("=", "%3D")
    Results = Results.replace("&", "%26")
    Results = Results.replace("$", "%24")
    return Results
}
function AntiSanitizeURLString(searchString) {
    var Results = "";

    Results = searchString;
    Results = Results.replaceAll("%2C", ",")
    Results = Results.replace("%25", "%")
    Results = Results.replace("%3C", "<")
    Results = Results.replace("%3E", ">")
    Results = Results.replace("%23", "#")
    Results = Results.replace("%7B", "{")
    Results = Results.replace("%7D", "}")
    Results = Results.replace("%7C", "|")
    Results = Results.replace("%5C", "\\")
    Results = Results.replace("%5E", "^")
    Results = Results.replace("%7E", "~")
    Results = Results.replace("%5B", "[")
    Results = Results.replace("%5D", "]")
    Results = Results.replace("%60", "`")
    Results = Results.replace("%3B", ";")
    Results = Results.replace("%2F", "/")
    Results = Results.replace("%3F", "?")
    Results = Results.replace("%3A", ":")
    Results = Results.replace("%40", "@")
    Results = Results.replace("%3D", "=")
    Results = Results.replace("&amp;", "&")
    Results = Results.replace("%24", "$")
    return Results
}
(function ($) {
    $.fn.localTimeFromUTCEdit = function (format) {
        return this.each(function () {
            var tagText = $(this).val();
            if (tagText != "" && tagText != undefined) {
                var givenDate = new Date(tagText);

                var amPm = "";
                var convertedTime = convertLocalDateToUTCDate(givenDate, false);
                if (!format.includes('HH'))
                    amPm = convertedTime.getHours() >= 12 ? "PM" : "AM";
                // format the date
                var localDateString = moment(convertedTime).format(format) + amPm;
                $(this).val(localDateString);
            }
        });
    };
    $.fn.localTimeFromUTCIndex = function (tdid, format, textDate, userName) {
        if (textDate != "" && textDate != undefined) {
            // textDate = new Date();

            var tagText = textDate;
            var givenDate = new Date(tagText);

            var amPm = "";
            var convertedTime = convertLocalDateToUTCDate(givenDate, false);
            if (!format.includes('HH'))
                amPm = convertedTime.getHours() >= 12 ? "PM" : "AM";

            // format the date
            var localDateString = moment(convertedTime).format(format) + amPm;
            $("#" + tdid).html(localDateString + " " + userName);
        }
    };
    $.fn.localTimeFromUTC = function (format) {
        return this.each(function () {
            var tagText = $(this).val();
            if (tagText != "" && tagText != undefined) {
                var givenDate = new Date(tagText);
                var amPm = "";
                var convertedTime = convertLocalDateToUTCDate(givenDate, false);
                if (!format.includes('HH'))
                    amPm = convertedTime.getHours() >= 12 ? "PM" : "AM";
                // format the date
                var localDateString = moment(convertedTime).format(format) + amPm;
                $(this).val(localDateString);
            }
        });
    };
})(jQuery);

function convertLocalDateToUTCDate(date, toUTC) {
    date = new Date(date);
    //Local time converted to UTC
    var localOffset = date.getTimezoneOffset() * 60000;
    //var localOffset = new Date().getTimezoneOffset() / 60000;
    var localTime = date.getTime();
    if (toUTC) {
        date = localTime + localOffset;
    } else {
        date = localTime - localOffset;
    }
    date = new Date(date);
    return date;
} function Set(source, id, DisplayValue) {
    var dropdown = ($('#PopupBulkOperationLabel').attr('class'));
    if (source.checked) {
        if ($('#' + dropdown).attr('multiple') == 'multiple' && $('#' + dropdown).attr('multipleText') != "multipleText") {
            var obj = document.getElementById(dropdown);
            var found = false;
            for (var o = 0; o < obj.options.length; o++) {
                if (obj.options[o].value == id) {
                    found = true;
                    obj.options[o].setAttribute('selected', "selected");
                }
            }
            if (!found) {
                $('#' + dropdown).append($('<option selected=\'selected\'></option>').val(id).html(DisplayValue));
            }
			$('#' + dropdown).select().trigger('change');
            $('#' + dropdown).multiselect('rebuild');
            var urlGetAll = $('#' + dropdown).attr("dataurl").replace("GetAllMultiSelectValue", "Index");
            urlGetAll = addParameterToURL(urlGetAll, 'BulkOperation', 'multiple');
            if (obj.options.length > 10) {
                var hostingentity = $('#' + dropdown).attr('id');
                var dispName = ($("label[for=\"" + hostingentity + "\"]").text());
                var link = "<a onclick=\"" + "OpenPopUpBulkOperation('PopupBulkOperation','" + hostingentity + "','" + dispName + "','dvPopupBulkOperation','" + urlGetAll + "')\">View All</a>";
                var getall = "<li class='disabled-result disabled-result' style='font-style:Italic;text-decoration:underline;' >" + link + "</li>";
                $('#' + dropdown).next().find('ul').append($(getall));
            }
        

        }
        if ($('#' + dropdown).attr('multipleText') == "multipleText") {

            var obj = document.getElementById(dropdown);
            var found = false;
            for (var o = 0; o < obj.options.length; o++) {
                if (obj.options[o].value == id) {
                    found = true;
                    obj.options[o].selected = true;
                }
            }
            if (!found) {
                $('#' + dropdown).append($('<option selected=\'selected\'></option>').val(id).html(DisplayValue));
            }
            $('#' + dropdown).select().trigger('change');
        }
    }
    else {
        if ($('#' + dropdown).attr('multiple') == 'multiple' && $('#' + dropdown).attr('multipleText') != "multipleText") {
            var obj = document.getElementById(dropdown);
            for (var o = 0; o < obj.options.length; o++) {
                if (obj.options[o].value == id) {
                    obj.options[o].removeAttribute("selected");
                    $('#' + dropdown).multiselect('deselect', id);

                }
            }
        }
        if ($('#' + dropdown).attr('multipleText') == "multipleText") {
            var obj = document.getElementById(dropdown);
            for (var o = 0; o < obj.options.length; o++) {
                if (obj.options[o].value == id) {
                    obj.options[o].selected = false;
                }
            }
            $('#' + dropdown).select().trigger('change');
        }
    }
}
//time out popup alert
var sess_intervalID;
var sess_lastActivity;
function initSession() {
    sess_lastActivity = new Date();
    sessSetInterval();
    $(document).bind('keypress.session', function (ed, e) {
        sessKeyPressed(ed, e);
    });
}
function sessSetInterval() {
    sess_intervalID = setInterval('sessInterval()', sess_pollInterval);
}
function sessClearInterval() {
    clearInterval(sess_intervalID);

}
function sessKeyPressed(ed, e) {
    sess_lastActivity = new Date();
}
function sessLogOut() {
    ClearFilterCookies();
    $("#logoutForm").submit();
}
function sessInterval() {
    var now = new Date();
    //get milliseconds of differneces
    var diff = now - sess_lastActivity;
    //get minutes between differences
    var diffMins = (diff / 1000 / 60);
    if (diffMins >= sess_warningMinutes) {
        //warn before expiring
        //stop the timer
        sessClearInterval();
        var min = 5;
        if (sess_warningMinutes <= 5)
            min = 1;
        //prompt for attention
        var active = OkClick(min)
        if (active == 1) {
            now = new Date();
            diff = now - sess_lastActivity;
            diffMins = (diff / 1000 / 60);
            if (diffMins > sess_expirationMinutes) {
                sessLogOut();
            }
            else {
                initSession();
                sessSetInterval();
                sess_lastActivity = new Date();
            }
        }
        //else {
        //    sessLogOut();
        //}
    }
}
function OkClick(min) {
    alert('Your session will expire in ' + min + ' minutes. Click Ok to continue working');
    return 1;
}

function ViewReports(url, entityName) {
    $("#ShowReoprtsLabel").html("Report of " + entityName);
    $("#ShowReoprts").modal('show');
    $("#ShowReoprts").find('.modal-dialog.ui-draggable').attr("style", "width:90%");
    $("#LoadReportsDiv").html('Please wait..');
    $("#LoadReportsDiv").load(url);
}
function focusOnControl(formId) {
    var cltIds = $("#" + formId).find('input[type=text]:not([class=hidden]):not([readonly]),textarea:not([readonly])');
    var cltId = "";
    $(cltIds).each(function () {
        if ($(this).attr("id") == undefined)
            return
        var dvhidden = $("#dv" + $(this).attr("id"));
        var dvDate = $("#datetimepicker" + $(this).attr("id")).attr("id");
        if (!(dvhidden.css('display') == 'none') && dvDate == undefined) {
            cltId = $(this);
            return false;
        }
    });
    if (cltId != "" && cltId != undefined)
        setTimeout(function () { $(cltId).focus(); }, 500)
    var ctrlReadonly = $("#" + formId).find('input[type=text][readonly],textarea[readonly]');
    $(ctrlReadonly).each(function () {
        $(ctrlReadonly).attr("tabindex", "-1");
    });
}
function focusOnControlWizardStep(formId) {
    var cltIds = $(formId).find('input[type=text]:not([class=hidden]):not([readonly]),textarea:not([readonly])');
    var cltId = "";
    $(cltIds).each(function () {
        if ($(this).attr("id") == undefined)
            return
        var dvhidden = $("#dv" + $(this).attr("id"));
        var dvDate = $("#datetimepicker" + $(this).attr("id")).attr("id");
        if (!(dvhidden.css('display') == 'none') && dvDate == undefined) {
            cltId = $(this);
            return false;
        }
    });
    if (cltId != "" && cltId != undefined)
        setTimeout(function () { $(cltId).focus(); }, 500)
    var ctrlReadonly = $(formId).find('input[type=text][readonly],textarea[readonly]');
    $(ctrlReadonly).each(function () {
        $(ctrlReadonly).attr("tabindex", "-1");
    });
}
function FillDisplayValueQEdit(entityName) {
    var selectedval = $("option:selected", $("select#" + entityName + "DD")).val();
    var selectedtext = $("option:selected", $("select#" + entityName + "DD")).text();
    $("#aBtnQuickEdit" + entityName + "_" + selectedval).click();
}
function nextFun(entityName) {
    $("option:selected", $("select#" + entityName + "DD")).next().prop('selected', true);
    var selectedval = $("option:selected", $("select#" + entityName + "DD")).val();
    var selectedtext = $("option:selected", $("select#" + entityName + "DD")).text();
    var lastOptionVal = $('#' + entityName + "DD" + ' option:last-child').val();
    $('#sevranBtn').click();
    $("#aBtnQuickEdit" + entityName + "_" + selectedval).click();
    return false;

}
function prevFun(entityName) {
    $("option:selected", $("select#" + entityName + "DD")).prev().prop('selected', true);
    var selectedval = $("option:selected", $("select#" + entityName + "DD")).val();
    var selectedtext = $("option:selected", $("select#" + entityName + "DD")).text();
    var fristOptionVal = $('#' + entityName + "DD" + ' option:first-child').val();
    $('#sevranBtn').click();
    $("#aBtnQuickEdit" + entityName + "_" + selectedval).click();
    return false;
}
function SaveAndContinueEdit(entityName, e) {
    e.preventDefault()
    var lastOptionVal = $('#' + entityName + "DD" + ' option:last-child').val();
    var fristOptionVal = $('#' + entityName + "DD" + ' option:first-child').val();


    var selectedval = $("option:selected", $("select#" + entityName + "DD")).val();
    var selectedtext = $("option:selected", $("select#" + entityName + "DD")).text();
    if (lastOptionVal != selectedval) {
        $("option:selected", $("select#" + entityName + "DD")).next().prop('selected', true);

    }
    selectedval = $("option:selected", $("select#" + entityName + "DD")).val();
    if (lastOptionVal == selectedval) {
        $('#next').hide();
    }

    if (fristOptionVal == selectedval) {
        $('#prev').hide();
    }

    $('#sevranBtn').click();
    $("#aBtnQuickEdit" + entityName + "_" + selectedval).click();
}

function nextFunEdit(entityName, event, hdnNextPrevId) {
    event.preventDefault();
    $("option:selected", $("select#Entity" + entityName + "DisplayValueEdit")).next().prop('selected', true);
    var selectedval = $("option:selected", $("select#Entity" + entityName + "DisplayValueEdit")).val();
    var selectedtext = $("option:selected", $("select#Entity" + entityName + "DisplayValueEdit")).text();
    var lastOptionVal = $("#Entity" + entityName + "DisplayValueEdit" + ' option:last-child').val();
    $('input:hidden[name="' + hdnNextPrevId + '"]').val(selectedval);
    $('#sevranBtnEdit').click();
    return false;

}
function prevFunEdit(entityName, event, hdnNextPrevId) {
    event.preventDefault();
    $("option:selected", $("select#Entity" + entityName + "DisplayValueEdit")).prev().prop('selected', true);
    var selectedval = $("option:selected", $("select#Entity" + entityName + "DisplayValueEdit")).val();
    var selectedtext = $("option:selected", $("select#Entity" + entityName + "DisplayValueEdit")).text();
    var fristOptionVal = $("#Entity" + entityName + "DisplayValueEdit" + ' option:first-child').val();
    $('input:hidden[name="' + hdnNextPrevId + '"]').val(selectedval);
    $('#sevranBtnEdit').click();
    return false;
}
function FillDisplayValueEditPage(entityName, frm, id, event) {
    event.preventDefault();
    var selectedval = $("option:selected", $("select#Entity" + entityName + "DisplayValueEdit")).val();
    var selectedtext = $("option:selected", $("select#Entity" + entityName + "DisplayValueEdit")).text();
    var url = $("#" + frm).attr("action");
    var url = url.replace("Edit/" + id, "Edit/" + selectedval);
    window.location.replace(url);
}
function DisableTabOnReadonlyProperty(formId) {
    var ctrlReadonly = $("#" + formId).find('input[type=text][readonly],textarea[readonly]');
    $(ctrlReadonly).each(function () {
        $(ctrlReadonly).attr("tabindex", "-1");
    });
}
function goBack(baseurl) {
    var response = '';
    var url = document.referrer;
    var UrlMain = document.URL;
    if (url == UrlMain) {
        window.location.href = baseurl;
    }
    else {
        var response = $.ajax({
            type: "GET",
            url: url,
            async: false,
            cache: false
        });
        if (response.status == 404) {
            window.location.href = baseurl;
        }
        else if (response.status == 500) {
            window.location.href = baseurl;
        }
        else {

            var result = window.location.href.split('/');
            var Param1 = result[result.length - 1];
            var Param2 = result[result.length - 2];

            var result1 = document.referrer.split('/');
            var Param3 = result1[result1.length - 1];
            var Param4 = result1[result1.length - 2];

            var res = Param3.split('?');
            var Param5 = res[res.length - 1];
            var Param6 = res[res.length - 2];

            if ((Param2 == "Create" && Param6 == "Create") || (Param2 == "Edit" && (Param6 == "Edit" || Param6 == "Create")) || (Param2 == "Edit" && Param5 == "Edit") || (Param2 == "Create" && Param6 == "Create"))
            {
                window.location.href = baseurl;
            }
            else {
                window.location.replace(document.referrer);
                //window.history.back();
            }
        }
    }
}
function IgnoreReadOnlyCtrlFocus() {
    try {
        document.getElementsByTagName("body")[0].focus();
        $("#addPopup").removeAttr("tabindex");
        var cltcoll = $("#dvPopup").find('input[type=text]:not([class=hidden]):not([readonly]),textarea:not([readonly])');
        var cltid = "";
        $(cltcoll).each(function () {
            if ($(this).attr("id") == undefined)
                return
            var dvhidden = $("#dv" + $(this).attr("id"));
            var dvDate = $("#datetimepicker" + $(this).attr("id")).attr("id");
            if (!(dvhidden.css('display') == 'none') && dvDate == undefined) {
                cltid = $(this);
                return false;
            }
        });
        if (cltid != "" && cltid != undefined)
            setTimeout(function () { $(cltid).focus(); }, 500);
        var ctrlReadonly = $("#dvPopup").find('input[type=text][readonly],textarea[readonly]');
        $(ctrlReadonly).each(function () {
            $(ctrlReadonly).attr("tabindex", "-1");
        });
    }
    catch (ex) { }
}
function UpdateImageGallary(url, divid, imgId) {
    var r = confirm("Do you want to delete all image ");
    if (r == true) {
        url = url + "&IDs=" + $('input:text[name="' + imgId + '"]').val();
        $.ajax({
            url: url,
            cache: false,
            success: function (data) {
                if (data != null) {
                    if (data.result == "POP") {
                        $("#itemNo" + imgId + divid).remove();
                        if ($("#carousel" + imgId).find("#FindNull" + imgId).html() == undefined) {
                            $('input:text[name="' + imgId + '"]').val('');
                            $("#ConcurrencyKey").val(base64ArrayToString(data.ConcurrencyKey));
                            $("#viewImg" + imgId).remove();
                            if ($("#btnAll" + imgId).html() != undefined) {
                                $("#btnAll" + imgId).remove();
                            }
                            $("#" + imgId.toLowerCase() + "popup").click()
                        }
                        else {
                            if ((data.output).split(',').length == 1) {
                                if ($("#btnAll" + imgId).html() != undefined) {
                                    $("#btnAll" + imgId).remove();
                                }
                            }
                            $('input:text[name="' + imgId + '"]').val(data.output);
                            $("#ConcurrencyKey").val(base64ArrayToString(data.ConcurrencyKey));
                            $("#carousel" + imgId).html($("#carousel" + imgId).html());
                        }

                    }
                }
            }
        })
    }
    else {
        return true;
    }
}
function PopupLinkToCard(obj, url, id) {
    $('.DisplayCardOpenClose').remove();
    var winHeight = window.outerHeight;
    var winWidth = window.outerWidth;
    var offsetX = $(obj).positioncustom().left;
    var offsetY = $(obj).positioncustom().top;
    var remHeight = winHeight - offsetY;
    var remWidth = winWidth - offsetX;
    var marLeft = 0;
    var marTop = 0;
    $(obj).append('<div class="DisplayCardOpenClose"></div>');
    $('.DisplayCardOpenClose').load(url);
    $('.DisplayCardOpenClose').hide();
    var cName = $(obj).parents();
    var tdRow = cName[0].closest("tr");
    if (tdRow != null) {
        if (remWidth < 500) {
            marLeft = -270;
        }
    }
    else {
        marLeft = -270;
    }
    if (remHeight < 400) {
        if (tdRow != null) {
            marTop = -200;
        }
        else {
            marTop = -500;
        }
    }
    $('.DisplayCardOpenClose').attr('style', 'position:absolute!important;top:' + offsetY + 'px!important; left:' + offsetX + 'px!important; margin-left:' + marLeft + 'px!important; margin-top:' + marTop + 'px!important;');
}
function ClosePopupCard(e) {
    e.preventDefault();
    $('.DisplayCardOpenClose').remove();
}
function HideVerbFromBR(verdlist) {
    var array = verdlist.split(',');
    for (var i = 0; i < array.length; i++) {
        var verbId1 = "vrb" + array[i].split(' ').join('');
        if ($("#" + verbId1).html() != undefined) {
            $("#" + verbId1).remove();
        }
        var verbId = "vrb1" + array[i].split(' ').join('');
        if ($("#" + verbId).html() != undefined) {
            $("#" + verbId).remove();
        }
        else {
            if ($('input[name ="SelectedGenerateDocumentTemplate"]') != null) {
                $('input[name ="SelectedGenerateDocumentTemplate"]').each(function () {
                    if ($(this).attr('verbName') == array[i].split(' ').join('')) {
                        $(this).parent().remove();
                    }
                });
            }
        }
    }
}
function GetValueOfProperty(id, url, AssociationIDName, propertyofAssociation) {
    var propertyValueOfAssociation = "";
    $.ajax({
        url: url,
        type: "GET",
        cache: false,
        async: false,
        data: { Id: id, PropName: AssociationIDName, propertyofAssociation: propertyofAssociation },
        complete: function (result) {
            propertyValueOfAssociation = result.responseJSON;
        },
        success: function (result) {
        },
        error: function (jqXHR, textStatus, errorThrown) {
        }
    });
    return propertyValueOfAssociation;
}
function AddAssocationMap(obj, entityName, dataurl, entitydisplayName, entityAssocationName, IsRequird, MainEntity) {
    entitydisplayName = decodeURIComponent(entitydisplayName);
    var color = getColorCode()
    var leftboder = "border-left:solid 3px " + color + ";"
    var topboder = " border-top:solid 3px " + color + ";"
    var bottomboder = "border-bottom:solid 3px " + color + ";"
    
    if (IsRequird) {
        var tdNext = $('#tr' + entityAssocationName + ' td')[1];
        var collist = $(tdNext).find('select#colList');
        $(collist).removeAttr('required');
    }
    var trfull = "";
    var urlentity = $(obj).attr("id").replace("a_", "");
    $.ajax({
        async: false,
        type: "POST",
        url: dataurl + "?entNametarget=" + entityName,
        success: function (data) {
            var targetEntarr = "";
            var preAssoc = "";
            var preAssocdisp = "";
            var dispayfor = "";
            for (var i = 0; i < data.Properties.length; i++) {
                if (data.Properties[i].Name == "DisplayValue" || data.Properties[i].Name == "TenantId" || data.Properties[i].Name == "IsDeleted" || data.Properties[i].Name == "DeleteDateTime")
                    continue;
                var trcontant = ""
                var targetEnt = "";
                var trId = "";
                var assoName = "";
                var isRequired = false;
                var strstyle = "";
                var _entitydisplayName = "";
                for (var j = 0; j < data.Associations.length; j++) {
                    if (data.Associations[j].AssociationProperty == data.Properties[i].Name) {
                        targetEnt = data.Associations[j].Target;
                        isRequired = data.Associations[j].IsRequired;
                        _entitydisplayName = data.Associations[j].DisplayName;
                        assoName = data.Associations[j].Name;
                        trId = " id=tr" + assoName;
                    }
                }
                if (i == 0)
                    trcontant += "<tr style='" + leftboder + "' " + trId + " class=cls" + entityName + ">";
                else if (i == data.Properties.length - 1)
                    trcontant += "<tr style='" + leftboder + bottomboder + "' " + trId + " class=cls" + entityName + ">";
                else
                    trcontant += "<tr style='" + leftboder + "' " + trId + " class=cls" + entityName + ">";

                trcontant += "<td>";
                preAssoc = $("#AssocInternal" + entityAssocationName).val() + "$";
                preAssocdisp = $("#AssocDisplay" + entityAssocationName).val() + "$";
                dispayfor = $("#AssocDisplay" + entityAssocationName).val().replace("$", " ");
                trcontant += "<label for='" + preAssoc.replace("$", "") + data.Properties[i].Name.replace("$", " ") + "'>" + " " + data.Properties[i].DisplayName.replace(".", " ").replace("$", " ") + "</label>";
                if (data.Properties[i].IsRequired || isRequired) {
                    trcontant += "<span class='text-danger-reg'>*</span>";
                }
                if (targetEnt.length > 0) {
                    trcontant += "<a id='a_" + targetEnt + "' onclick = AddAssocationMap(this,'" + targetEnt + "','" + dataurl + "','" + encodeURIComponent(_entitydisplayName) + "','" + assoName + "'," + isRequired + ",'" + MainEntity + "')><i class='fa fa-plus-circle'></i></a>";
                }

                for (var p = 0; p < data.Associations.length; p++) {
                    if (data.Associations[p].AssociationProperty == data.Properties[i].Name) {
                        trcontant += "<input id='AssocInternal" + data.Associations[p].Name + "' name='AssocInternal' type='hidden' value='" + preAssoc + data.Associations[p].Name + "'>";
                        trcontant += "<input id='AssocDisplay" + data.Associations[p].Name + "' name='AssocDisplay' type='hidden' value='" + preAssocdisp + data.Associations[p].DisplayName + "'>";
                    }
                }
                trcontant += "<input id='lblColumnDisplayName' name='lblColumnDisplayName' type='hidden' value='" + entityName + "$" + preAssocdisp + data.Properties[i].DisplayName.replace(".", "") + "'>";
                //change MainEntity to entityName for assosated entity 
                trcontant += "<input id='lblColumn' name='lblColumn' type='hidden' value='" + entityName + "$" + preAssoc + "" + data.Properties[i].Name + "'>"
                trcontant += "</td>";
                trcontant += "<td>";
                if (data.Properties[i].IsRequired || isRequired) {
                    trcontant += "<select id='colList' name='colList' required = 'required'>";
                }
                else {
                    trcontant += "<select id='colList' name='colList' " + strstyle + " >";
                }
                var optionStr = "";
                var select = $('#colList').html();
                for (var m = 0; m < $(select).length; m++) {
                    var option = $(select)[m];
                    $(option).removeAttr('selected')
                    if (option.text != undefined && option.text == entitydisplayName + " " + data.Properties[i].DisplayName) {
                        $(option).prop('selected', true)
                    }
                    if (option.text != undefined)
                        optionStr += option.outerHTML
                }
                trcontant += optionStr;
                trcontant += "</select>";
                var assPropOption = "";
                for (var k = 0; k < data.Associations.length; k++) {
                    if (data.Associations[k].AssociationProperty == data.Properties[i].Name) {
                        if (data.Associations[k])
                            assPropOption += "<select id='colAssocPropList' name='colAssocPropList' " + strstyle + ">";
                    }
                }
                for (var k = 0; k < data.Associations.length; k++) {
                    if (data.Associations[k].AssociationProperty == data.Properties[i].Name) {
                        assPropOption += "<option value='DisplayValue-" + data.Associations[k].AssociationProperty + "'>DisplayValue</option>";
                        var tar = data.Associations[k].Target;
                        var Properties = [];
                        Properties = AssocationProperties(tar, dataurl);
                        if (Properties != undefined)
                            for (var l = 0; l < Properties.length; l++) {
                                assPropOption += "<option value='" + Properties[l].Name + "-" + data.Associations[k].AssociationProperty + "'>" + Properties[l].DisplayName + "</option>";
                            }
                    }
                }
                for (var k = 0; k < data.Associations.length; k++) {
                    if (data.Associations[k].AssociationProperty == data.Properties[i].Name) {
                        assPropOption += "</select>";
                    }
                }
                trcontant += assPropOption;
                trcontant += "</td>";
                trcontant += "<td >";
                trcontant += "<input datavalue='" + data.Properties[i].Name + "' id='chkUpdate' name='chkUpdate' type='checkbox' value='true'  " + strstyle + ">";
                trcontant += "<input name='chkUpdate' type='hidden' value='false'>";
                trcontant += "<label for='" + data.Properties[i].Name + "'  " + strstyle + ">If " + entitydisplayName + " " + data.Properties[i].DisplayName + " matches</label>";
                trcontant += "</td>";
                trcontant += "</tr>";
                if (i == data.Properties.length - 1)
                    trcontant += "<tr class='cls" + entityName + "'><td></td><td></td><td></td></tr>";
                //$("#tr" + entityName).attr("style", backcoloer);
                trfull += trcontant;
                if (targetEnt.length > 0)
                    targetEntarr += targetEnt + ",";
                else
                    targetEntarr += entityName + ",";
            }

            $("#tr" + entityAssocationName).after(trfull);
            $("#tr" + entityAssocationName).attr("style", topboder + leftboder);
            //$("#tr" + entityName).after( trfull);
            $(obj).removeAttr('onclick')
            $(obj).attr("onclick", "removeAsspcation(this,'" + entityName + "','" + dataurl + "','" + entitydisplayName + "','" + entityAssocationName + "','" + targetEntarr + "'," + IsRequird + ",'" + MainEntity + "')")
            $(obj).html("<i class='fa fa-minus-circle'></i>")

        },
        error: function (jqXHR, textStatus, errorThrown) {
        }
    });
    //alert(entityName)
}
function AssocationProperties(entityName, dataurl) {
    var Properties = [];
    $.ajax({
        async: false,
        type: "POST",
        url: dataurl + "?entNametarget=" + entityName,
        success: function (data) {
            if (data.Properties != undefined) { }
            for (var i = 0; i < data.Properties.length; i++) {
                if (data.Properties[i].Name == "DisplayValue" || data.Properties[i].Name == "TenantId" || data.Properties[i].Name == "IsDeleted" || data.Properties[i].Name == "DeleteDateTime" || data.Properties[i].Name == "T_AutoNo")
                    continue;
                Properties.push(data.Properties[i]);
            }
            //callback.call(Properties);
        },
        error: function (jqXHR, textStatus, errorThrown) {
        }
    });
    return Properties;
}
function removeAsspcation(obj, entityName, dataurl, entitydisplayName, entityAssocationName, targetEntarr, IsRequird, MainEntity) {
    targetEntarr = targetEntarr.substring(0, targetEntarr.length - 1);
    var value = targetEntarr.split(',');
    for (i = 0; i < value.length; i++) {
        if (value[i] != "" && value[i] != "undefined") {
            $(".cls" + value[i]).remove();
        }
    }
    if (IsRequird) {
        var tdNext = $('#tr' + entityAssocationName + ' td')[1];
        var collist = $(tdNext).find('select#colList');
        $(collist).attr('required', 'required');
    }
    $("#tr" + entityAssocationName).removeAttr('style');
    $(".cls" + entityName).remove();
    $(obj).removeAttr('onclick');
    $(obj).attr("onclick", "AddAssocationMap(this,'" + entityName + "','" + dataurl + "','" + entitydisplayName + "','" + entityAssocationName + "'," + IsRequird + ",'" + MainEntity + "')")
    $(obj).html("<i class='fa fa-plus-circle'></i>")
}
function getColorCode() {
    var makeColorCode = '0123456789ABCDEF';
    var code = '#';
    for (var count = 0; count < 6; count++) {
        code = code + makeColorCode[Math.floor(Math.random() * 16)];
    }
    return code;
}
function DashboardHomeShowhide(visibleDefaultDashboard, Url, linkobj) {
    $.ajax({
        type: "Get",
        url: Url,
        success: function (res) {
            if (res.IsDefaultDashboard == true && !visibleDefaultDashboard)
                $("#" + linkobj).show();
            if (res.IsDefaultDashboard == false && visibleDefaultDashboard)
                $("#" + linkobj).hide();
        }
    });
}