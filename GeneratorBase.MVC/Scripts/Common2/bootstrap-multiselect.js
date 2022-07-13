/**
* bootstrap-multiselect.js
* https://github.com/davidstutz/bootstrap-multiselect
*
* Copyright 2012 - 2014 David Stutz
*
* Dual licensed under the BSD-3-Clause and the Apache License, Version 2.0.
*/
!function ($) {
    "use strict";// jshint ;_;

    if (typeof ko !== 'undefined' && ko.bindingHandlers && !ko.bindingHandlers.multiselect) {
        ko.bindingHandlers.multiselect = {
            init: function (element, valueAccessor, allBindingsAccessor, viewModel, bindingContext) {
                var listOfSelectedItems = allBindingsAccessor().selectedOptions,
                    config = ko.utils.unwrapObservable(valueAccessor());
                $(element).multiselect(config);
                if (isObservableArray(listOfSelectedItems)) {
                    // Subscribe to the selectedOptions: ko.observableArray
                    listOfSelectedItems.subscribe(function (changes) {
                        var addedArray = [], deletedArray = [];
                        changes.forEach(function (change) {
                            switch (change.status) {
                                case 'added':
                                    addedArray.push(change.value);
                                    break;
                                case 'deleted':
                                    deletedArray.push(change.value);
                                    break;
                            }
                        });
                        if (addedArray.length > 0) {
                            $(element).multiselect('select', addedArray);
                        };
                        if (deletedArray.length > 0) {
                            $(element).multiselect('deselect', deletedArray);
                        };
                    }, null, "arrayChange");
                }
            },
            update: function (element, valueAccessor, allBindingsAccessor, viewModel, bindingContext) {
                var listOfItems = allBindingsAccessor().options,
                    ms = $(element).data('multiselect'),
                    config = ko.utils.unwrapObservable(valueAccessor());
                if (isObservableArray(listOfItems)) {
                    // Subscribe to the options: ko.observableArray incase it changes later
                    listOfItems.subscribe(function (theArray) {
                        $(element).multiselect('rebuild');
                    });
                }
                if (!ms) {
                    $(element).multiselect(config);
                }
                else {
                    ms.updateOriginalOptions();
                }
            }
        };
    }
    function isObservableArray(obj) {
        return ko.isObservable(obj) && !(obj.destroyAll === undefined);
    }
    /**
     * Constructor to create a new multiselect using the given select.
     * 
     * @param {jQuery} select
     * @param {Object} options
     * @returns {Multiselect}
     */
    function Multiselect(select, options, host, editoptHtml, optCount) {
        this.options = this.mergeOptions(options);
        this.$select = $(select);
        // Initialization.
        // We have to clone to create a new reference.
        this.originalOptions = this.$select.clone()[0].options;
        this.query = '';
        this.editoptHtml = "";
        this.optCount = 0;
        this.searchTimeout = null;
        this.options.multiple = this.$select.attr('multiple') === "multiple";
        this.options.onChange = $.proxy(this.options.onChange, this);
        this.options.onDropdownShow = $.proxy(this.options.onDropdownShow, this);
        this.options.onDropdownHide = $.proxy(this.options.onDropdownHide, this);
        // Build select all if enabled.
        this.buildContainer();
        this.buildButton();
        this.buildSelectAll();
        this.buildDropdown();
        this.buildDropdownOptions();
        this.buildFilter();
        this.updateButtonText();
        this.updateSelectAll();
        this.$select.hide().after(this.$container);
    };
    Multiselect.prototype = {
        defaults: {
            /**
             * Default text function will either print 'None selected' in case no
             * option is selected or a list of the selected options up to a length of 3 selected options.
             * 
             * @param {jQuery} options
             * @param {jQuery} select
             * @returns {String}
             */
            buttonText: function (options, select) {
                if (options.length === 0) {
                    return this.nonSelectedText + ' <b class="caret"></b>';
                }
                else {
                    if (options.length > this.numberDisplayed) {
                        return options.length + ' ' + this.nSelectedText + ' <b class="caret"></b>';
                    }
                    else {
                        var selected = '';
                        options.each(function () {
                            var label = ($(this).attr('label') !== undefined) ? $(this).attr('label') : $(this).html();
                            selected += label + ', ';
                        });
                        return selected.substr(0, selected.length - 2) + ' <b class="caret"></b>';
                    }
                }
            },
            /**
             * Updates the title of the button similar to the buttonText function.
             * @param {jQuery} options
             * @param {jQuery} select
             * @returns {@exp;selected@call;substr}
             */
            buttonTitle: function (options, select) {
                if (options.length === 0) {
                    return this.nonSelectedText;
                }
                else {
                    var selected = '';
                    options.each(function () {
                        selected += $(this).text() + ', ';
                    });
                    return selected.substr(0, selected.length - 2);
                }
            },
            /**
             * Create a label.
             * 
             * @param {jQuery} element
             * @returns {String}
             */
            label: function (element) {
                return $(element).attr('label') || $(element).html();
            },
            /**
             * Triggered on change of the multiselect.
             * Not triggered when selecting/deselecting options manually.
             * 
             * @param {jQuery} option
             * @param {Boolean} checked
             */
            onChange: function (option, checked) {
            },
            /**
             * Triggered when the dropdown is shown.
             * 
             * @param {jQuery} event
             */

            onDropdownShow: function (event) {

                this.optCount = this.$select.find('option').length;
                var selobj = this.$select;
                this.editoptHtml = "";
                if (this.optCount > 0)
                    this.editoptHtml = $(selobj).html();
                if (this.$select.attr("HostingName") != undefined) {
                    var hostingid = this.$select.attr("id");
                    var parentDDid = $('#' + hostingid).attr('ParentDD');
                    var AssociationNames = $('#' + hostingid).attr('AssoNameWithParent');
                    var associationParam = "";
                    if (parentDDid != null || parentDDid != undefined) {
						
						if ($('#' + parentDDid).attr("required") == "required" && ($('#' + parentDDid).val() == '' || $('#' + parentDDid).val() == '0'))
                        {
                            var label = $("label[for=" + parentDDid + "]");
                            alert("Please select " + label.text() + " first.");
                            $('#' + hostingid).html('');
                            $('#' + hostingid).multiselect('rebuild');
                            return;
                        }
										
                        var Parents = parentDDid.split(",");
                        var AssociationNameWithParent = "";
                        var selectedParentVal = "";
                        var ele = document.getElementById(Parents);
                        if (ele != null) {
                            for (var o = 0; o < ele.options.length; o++) {
                                if (ele.options[o].selected) {
                                    AssociationNameWithParent = AssociationNames.split(",")[0];
                                    selectedParentVal += ele.options[o].value + ",";
                                }
                            }
                        }
                        associationParam = "AssoNameWithParent=" + AssociationNameWithParent + "&AssociationID=" + selectedParentVal;
                    }
                    // alert($('#' + hostingid).attr("dataurl"));
                    var callingurl = $('#' + hostingid).attr("dataurl").indexOf("?") > -1 ? $('#' + hostingid).attr("dataurl") + "&" + associationParam : $('#' + hostingid).attr("dataurl") + "?" + associationParam;
                    $.ajax({
                        type: "GET",
                        url: callingurl,
                        contentType: "application/json; charset=utf-8",
                        global: false,
                        async: false,
                        cache: false,
                        dataType: "json",
                        success: function (jsonObj) {
                            if ($('#' + hostingid).attr("IsCustom") == undefined) {
                                var object1 = {
                                    Id: "NULL",
                                    Name: "None",
                                };
                                jsonObj.insert("0", object1)
                            }
                            $('#' + hostingid).html('');
                            if (this.optCount == 0)
                                $('#' + hostingid).multiselect('dataproviderOnShowDropDown', jsonObj, hostingid);
                            else {
                                $('#' + hostingid).multiselect('dataproviderOnShowDropDown', jsonObj, hostingid);
                            }
                        }
                    });
                }
                else {
                    var ullen = this.$ul.find('li').length;
                    if (ullen >= 10) {
                        for (var i = ullen; i < this.$ul.find('li').length; i++) {
                            if (this.$ul.find("li:eq(" + i + ")").attr('class') != "active")
                                this.$ul.find("li:eq(" + i + ")").css('display', 'none');
                        }
                    }
                    //var limore = "<li class='disabled-result disabled-result' style='font-style:Italic;' >Search for more...</li>";
                    //this.$ul.append($(limore));
                }
            },
            /**
             * Triggered when the dropdown is hidden.
             * 
             * @param {jQuery} event
             */
            onDropdownHide: function (event) {
            },
            buttonClass: 'btn btn-xs btn-default',
            dropRight: false,
            selectedClass: 'active',
            buttonWidth: 'auto',
            buttonMinWidth: 'auto',
            buttonContainer: '<div class="btn-group" style="width:100%" />',
            // Maximum height of the dropdown menu.
            // If maximum height is exceeded a scrollbar will be displayed.
            maxHeight: 250,
            checkboxName: 'multiselect',
            includeSelectAllOption: false,
            includeSelectAllIfMoreThan: 0,
            selectAllText: ' Select all',
            selectAllValue: 'multiselect-all',
            enableFiltering: true,
            enableCaseInsensitiveFiltering: true,
            filterPlaceholder: 'Search',
            // possible options: 'text', 'value', 'both'
            filterBehavior: 'text',
            preventInputChangeEvent: false,
            nonSelectedText: 'None selected',
            nSelectedText: 'selected',
            numberDisplayed: 3,
            templates: {
                button: '<button type="button" tempclass="selectmulti" onclick="HideShowul(this)" class="multiselect dropdown-toggle multiselectFix" data-toggle="dropdown"></button>',
                ul: '<ul class="multiselect-container  dropdown-menu" id="popul"></ul>',
                filter: '<div class="input-group"><input class="form-control multiselect-search" type="text"><div class="input-group-btn"><a style="height:34px;" class="btn btn-default btn-sm pull-left" onclick=closeSel(this);>Close</a></div></div>',
                li: '<li><a href="javascript:void(0);"><label></label></a></li>',
                divider: '<li class="divider"></li>',
                liGroup: '<li><label class="multiselect-group"></label></li>'
            }
        },
        constructor: Multiselect,
        /**
         * Builds the container of the multiselect.
         */
        buildContainer: function () {

            this.$container = $(this.options.buttonContainer);
            this.$container.on('show.bs.dropdown', this.options.onDropdownShow);
            this.$container.on('hide.bs.dropdown', this.options.onDropdownHide);
        },
        /**
         * Builds the button of the multiselect.
         */
        buildButton: function () {
            this.$button = $(this.options.templates.button).addClass(this.options.buttonClass);
            // Adopt active state.
            if (this.$select.prop('disabled')) {
                this.disable();
            }
            else {
                this.enable();
            }
            // Manually add button width if set.
            if (this.options.buttonWidth && this.options.buttonWidth !== 'auto') {
                this.$button.css({
                    'width': this.options.buttonWidth
                });
            }
            if (this.options.buttonMinWidth && this.options.buttonMinWidth !== 'auto') {
                this.$button.css({
                    'min-width': this.options.buttonMinWidth
                });
            }
            // Keep the tab index from the select.
            var tabindex = this.$select.attr('tabindex');
            if (tabindex) {
                this.$button.attr('tabindex', tabindex);
            }
            this.$container.prepend(this.$button);
        },
        /**
         * Builds the ul representing the dropdown menu.
         */
        buildDropdown: function () {
            $('.input-group')
            // Build ul.
            this.$ul = $(this.options.templates.ul);
            if (this.options.dropRight) {
                this.$ul.addClass('pull-right');
            }
            // Set max height of dropdown menu to activate auto scrollbar.
            if (this.options.maxHeight) {
                // TODO: Add a class for this option to move the css declarations.
                this.$ul.css({
                    'max-height': this.options.maxHeight + 'px',
                    'overflow-y': 'auto',
                    'overflow-x': 'hidden'
                });
            }
            this.$container.append(this.$ul);
        },
        /**
         * Build the dropdown options and binds all nessecary events.
         * Uses createDivider and createOptionValue to create the necessary options.
         */
        buildDropdownOptions: function () {
            this.$select.children().each($.proxy(function (index, element) {
                // Support optgroups and options without a group simultaneously.
                var tag = $(element).prop('tagName')
                    .toLowerCase();
                if (tag === 'optgroup') {
                    this.createOptgroup(element);
                }
                else if (tag === 'option') {
                    if ($(element).data('role') === 'divider') {
                        this.createDivider();
                    }
                    else {
                        this.createOptionValue(element);
                    }
                }
                // Other illegal tags will be ignored.
            }, this));
            // Bind the change event on the dropdown elements.
            $('li input', this.$ul).on('change', $.proxy(function (event) {

                var $target = $(event.target);
                var checked = $target.prop('checked') || false;
                var isSelectAllOption = $target.val() === this.options.selectAllValue;
                // Apply or unapply the configured selected class.
                if (this.options.selectedClass) {
                    if (checked) {
                        $target.parents('li')
                            .addClass(this.options.selectedClass);
                    }
                    else {
                        $target.parents('li')
                            .removeClass(this.options.selectedClass);
                    }
                }
                // Get the corresponding option.
                var value = $target.val();
                var $option = this.getOptionByValue(value);
                var $optionsNotThis = $('option', this.$select).not($option);
                var $checkboxesNotThis = $('input', this.$container).not($target);
                if (isSelectAllOption) {
                    var values = [];
                    // Select the visible checkboxes except the "select-all" and possible divider.
                    var availableInputs = $('li input[value!="' + this.options.selectAllValue + '"][data-role!="divider"]', this.$ul).filter(':visible');
                    for (var i = 0, j = availableInputs.length; i < j; i++) {
                        values.push(availableInputs[i].value);
                    }
                    if (checked) {
                        this.select(values);
                    }
                    else {
                        this.deselect(values);
                    }
                }
                if (checked) {
                    $option.prop('selected', true);
                    if (this.options.multiple) {
                        // Simply select additional option.
                        $option.prop('selected', true);
                    }
                    else {
                        // Unselect all other options and corresponding checkboxes.
                        if (this.options.selectedClass) {
                            $($checkboxesNotThis).parents('li').removeClass(this.options.selectedClass);
                        }
                        $($checkboxesNotThis).prop('checked', false);
                        $optionsNotThis.prop('selected', false);
                        // It's a single selection, so close.
                        this.$button.click();
                    }
                    if (this.options.selectedClass === "active") {
                        $optionsNotThis.parents("a").css("outline", "");
                    }
                }
                else {
                    // Unselect option.
                    $option.prop('selected', false);
                    $option.removeAttr("selected");
                }
                this.$select.change();
                this.options.onChange($option, checked);
                this.updateButtonText();
                this.updateSelectAll();
                if (this.options.preventInputChangeEvent) {
                    return false;
                }
            }, this));
            $('li a', this.$ul).on('touchstart click', function (event) {
                event.stopPropagation();
                var $target = $(event.target);
                if (event.shiftKey) {
                    var checked = $target.prop('checked') || false;
                    if (checked) {
                        var prev = $target.parents('li:last')
                            .siblings('li[class="active"]:first');
                        var currentIdx = $target.parents('li')
                            .index();
                        var prevIdx = prev.index();
                        if (currentIdx > prevIdx) {
                            $target.parents("li:last").prevUntil(prev).each(
                                function () {
                                    $(this).find("input:first").prop("checked", true)
                                        .trigger("change");
                                }
                            );
                        }
                        else {
                            $target.parents("li:last").nextUntil(prev).each(
                                function () {
                                    $(this).find("input:first").prop("checked", true)
                                        .trigger("change");
                                }
                            );
                        }
                    }
                }
                $target.blur();
            });
            // Keyboard support.
            this.$container.on('keydown', $.proxy(function (event) {
                if ($('input[type="text"]', this.$container).is(':focus')) {
                    return;
                }
                if ((event.keyCode === 9 || event.keyCode === 27)
                        && this.$container.hasClass('open')) {
                    // Close on tab or escape.
                    this.$button.click();
                }
                else {
                    var $items = $(this.$container).find("li:not(.divider):visible a");
                    if (!$items.length) {
                        return;
                    }
                    var index = $items.index($items.filter(':focus'));
                    // Navigation up.
                    if (event.keyCode === 38 && index > 0) {
                        index--;
                    }
                        // Navigate down.
                    else if (event.keyCode === 40 && index < $items.length - 1) {
                        index++;
                    }
                    else if (!~index) {
                        index = 0;
                    }
                    var $current = $items.eq(index);
                    $current.focus();
                    if (event.keyCode === 32 || event.keyCode === 13) {
                        var $checkbox = $current.find('input');
                        $checkbox.prop("checked", !$checkbox.prop("checked"));
                        $checkbox.change();
                    }
                    event.stopPropagation();
                    event.preventDefault();
                }
            }, this));
        },
        /**
         * Create an option using the given select option.
         * 
         * @param {jQuery} element
         */
        createOptionValue: function (element) {
            if ($(element).is(':selected')) {
                $(element).prop('selected', true);
            }
            // Support the label attribute on options.
            var label = this.options.label(element);
            var value = $(element).val();
            var inputType = this.options.multiple ? "checkbox" : "radio";
            var $li = $(this.options.templates.li);
            $('label', $li).addClass(inputType);
            $('label', $li).append('<input type="' + inputType + '" name="' + this.options.checkboxName + '" />');
            var selected = $(element).prop('selected') || false;
            var $checkbox = $('input', $li);
            $checkbox.val(value);
            if (value === this.options.selectAllValue) {
                $checkbox.parent().parent()
                    .addClass('multiselect-all');
            }
            $('label', $li).append(" " + label);
            this.$ul.append($li);
            if ($(element).is(':disabled')) {
                $checkbox.attr('disabled', 'disabled')
                    .prop('disabled', true)
                    .parents('li')
                    .addClass('disabled');
            }
            $checkbox.prop('checked', selected);
            if (selected && this.options.selectedClass) {
                $checkbox.parents('li')
                    .addClass(this.options.selectedClass);
            }
        },
        /**
         * Creates a divider using the given select option.
         * 
         * @param {jQuery} element
         */
        createDivider: function (element) {
            var $divider = $(this.options.templates.divider);
            this.$ul.append($divider);
        },
        /**
         * Creates an optgroup.
         * 
         * @param {jQuery} group
         */
        createOptgroup: function (group) {
            var groupName = $(group).prop('label');
            // Add a header for the group.
            var $li = $(this.options.templates.liGroup);
            $('label', $li).text(groupName);
            this.$ul.append($li);
            if ($(group).is(':disabled')) {
                $li.addClass('disabled');
            }
            // Add the options of the group.
            $('option', group).each($.proxy(function (index, element) {
                this.createOptionValue(element);
            }, this));
        },
        /**
         * Build the selct all.
         * Checks if a select all ahs already been created.
         */
        buildSelectAll: function () {
            var alreadyHasSelectAll = this.hasSelectAll();
            if (!alreadyHasSelectAll && this.options.includeSelectAllOption && this.options.multiple
                    && $('option[data-role!="divider"]', this.$select).length > this.options.includeSelectAllIfMoreThan) {
                // Check whether to add a divider after the select all.
                if (this.options.includeSelectAllDivider) {
                    this.$select.prepend('<option value="" disabled="disabled" data-role="divider">');
                }
                this.$select.prepend('<option value="' + this.options.selectAllValue + '">' + this.options.selectAllText + '</option>');
            }
        },
        /**
         * Builds the filter.
         */
        buildFilter: function () {
            // Build filter if filtering OR case insensitive filtering is enabled and the number of options exceeds (or equals) enableFilterLength.
            if (this.options.enableFiltering || this.options.enableCaseInsensitiveFiltering) {
                var enableFilterLength = Math.max(this.options.enableFiltering, this.options.enableCaseInsensitiveFiltering);
                if (this.$select.find('option').length >= enableFilterLength) {
                    this.$filter = $(this.options.templates.filter);
                    $('input', this.$filter).attr('placeholder', this.options.filterPlaceholder);
                    this.$ul.prepend(this.$filter);
                    this.$filter.val(this.query).on('click', function (event) {
                        event.stopPropagation();
                    }).on('input keyup', $.debounce(400, $.proxy(function (event) {
                        if (this.$select.attr("HostingName") != undefined)
                            this.getfiltervalues(this.$select, event);

                        clearTimeout(this.searchTimeout);
                        this.searchTimeout = this.asyncFunction($.proxy(function () {
                            if (this.query !== event.target.value) {
                                this.query = event.target.value;
                                // alert(this.query);
                                $.each($('li', this.$ul), $.proxy(function (index, element) {
                                    var value = $('input', element).val();
                                    var text = $('label', element).text();
                                    var filterCandidate = '';
                                    if ((this.options.filterBehavior === 'text')) {
                                        filterCandidate = text;
                                    }
                                    else if ((this.options.filterBehavior === 'value')) {
                                        filterCandidate = value;
                                    }
                                    else if (this.options.filterBehavior === 'both') {
                                        filterCandidate = text + '\n' + value;
                                    }
                                    if (value !== this.options.selectAllValue && text) {
                                        // by default lets assume that element is not
                                        // interesting for this search
                                        var showElement = false;
                                        if (this.options.enableCaseInsensitiveFiltering && filterCandidate.toLowerCase().indexOf(this.query.toLowerCase()) > -1) {
                                            showElement = true;
                                        }
                                        else if (filterCandidate.indexOf(this.query) > -1) {
                                            showElement = true;
                                        }
                                        if (showElement) {
                                            $(element).show();
                                        }
                                        else {
                                            $(element).hide();
                                        }
                                    }
                                }, this));
                            }
                            // TODO: check whether select all option needs to be updated.
                        }, this), 300, this);
                    }, this)));
                    //

                }

            }
        },
        /**
         * Unbinds the whole plugin.
         */
        destroy: function () {
            this.$container.remove();
            this.$select.show();
            this.$select.data('multiselect', null);
        },
        /**
         * Refreshs the multiselect based on the selected options of the select.
         */
        refresh: function () {
            $('option', this.$select).each($.proxy(function (index, element) {
                var $input = $('li input', this.$ul).filter(function () {
                    return $(this).val() === $(element).val();
                });
                if ($(element).is(':selected')) {
                    $input.prop('checked', true);
                    if (this.options.selectedClass) {
                        $input.parents('li')
                            .addClass(this.options.selectedClass);
                    }
                }
                else {
                    $input.prop('checked', false);
                    if (this.options.selectedClass) {
                        $input.parents('li')
                            .removeClass(this.options.selectedClass);
                    }
                }
                if ($(element).is(":disabled")) {
                    $input.attr('disabled', 'disabled')
                        .prop('disabled', true)
                        .parents('li')
                        .addClass('disabled');
                }
                else {
                    $input.prop('disabled', false)
                        .parents('li')
                        .removeClass('disabled');
                }
            }, this));
            this.updateButtonText();
            this.updateSelectAll();
        },
        /**
         * Select all options of the given values.
         * 
         * @param {Array} selectValues
         */
        select: function (selectValues) {
            if (!$.isArray(selectValues)) {
                selectValues = [selectValues];
            }
            for (var i = 0; i < selectValues.length; i++) {
                var value = selectValues[i];
                var $option = this.getOptionByValue(value);
                var $checkbox = this.getInputByValue(value);
                if (this.options.selectedClass) {
                    $checkbox.parents('li')
                        .addClass(this.options.selectedClass);
                }
                $checkbox.prop('checked', true);
                $option.prop('selected', true);
            }
            this.updateButtonText();
        },
        /**
         * Clears all selected items
         * 
         */
        clearSelection: function () {
            var selected = this.getSelected();
            if (selected.length) {
                var arry = [];
                for (var i = 0; i < selected.length; i = i + 1) {
                    arry.push(selected[i].value);
                }
                this.deselect(arry);
                this.$select.change();
            }
        },
        /**
         * Deselects all options of the given values.
         * 
         * @param {Array} deselectValues
         */
        deselect: function (deselectValues) {
            if (!$.isArray(deselectValues)) {
                deselectValues = [deselectValues];
            }
            for (var i = 0; i < deselectValues.length; i++) {
                var value = deselectValues[i];
                var $option = this.getOptionByValue(value);
                var $checkbox = this.getInputByValue(value);
                if (this.options.selectedClass) {
                    $checkbox.parents('li')
                        .removeClass(this.options.selectedClass);
                }
                $checkbox.prop('checked', false);
                $option.prop('selected', false);
            }
            this.updateButtonText();
        },
        /**
         * Rebuild the plugin.
         * Rebuilds the dropdown, the filter and the select all option.
         */
        rebuild: function () {
            this.$ul.html('');
            // Remove select all option in select.
            $('option[value="' + this.options.selectAllValue + '"]', this.$select).remove();
            // Important to distinguish between radios and checkboxes.
            this.options.multiple = this.$select.attr('multiple') === "multiple";
            this.buildSelectAll();
            this.buildDropdownOptions();
            this.buildFilter();
            this.updateButtonText();
            this.updateSelectAll();
        },
        /**
         * The provided data will be used to build the dropdown.
         * 
         * @param {Array} dataprovider
         */
        dataprovider: function (dataprovider) {
            var optionDOM = "";
            for (var i = 0; i < dataprovider.length; i++) {
                optionDOM += '<option value="' + dataprovider[i].Id + '">' + dataprovider[i].Name + '</option>';
            }
            //dataprovider.forEach(function (option) {
            //    optionDOM += '<option value="' + option.value + '">' + option.label + '</option>';
            //});
            this.$select.html(optionDOM);
            this.rebuild();
        },
        /**
         * Enable the multiselect.
         */
        enable: function () {
            this.$select.prop('disabled', false);
            this.$button.prop('disabled', false)
                .removeClass('disabled');
        },
        /**
         * Disable the multiselect.
         */
        disable: function () {
            this.$select.prop('disabled', true);
            this.$button.prop('disabled', true)
                .addClass('disabled');
        },
        /**
         * Set the options.
         * 
         * @param {Array} options
         */
        setOptions: function (options) {
            this.options = this.mergeOptions(options);
        },
        /**
         * Merges the given options with the default options.
         * 
         * @param {Array} options
         * @returns {Array}
         */
        mergeOptions: function (options) {
            return $.extend(true, {}, this.defaults, options);
        },
        /**
         * Checks whether a select all option is present.
         * 
         * @returns {Boolean}
         */
        hasSelectAll: function () {
            return $('option[value="' + this.options.selectAllValue + '"]', this.$select).length > 0;
        },
        /**
         * Updates the select all option based on the currently selected options.
         */
        updateSelectAll: function () {
            if (this.hasSelectAll()) {
                var selected = this.getSelected();
                if (selected.length === $('option:not([data-role=divider])', this.$select).length - 1) {
                    this.select(this.options.selectAllValue);
                }
                else {
                    this.deselect(this.options.selectAllValue);
                }
            }
        },
        /**
         * Update the button text and its title based on the currently selected options.
         */
        updateButtonText: function () {
            var options = this.getSelected();
            // First update the displayed button text.
            $('button', this.$container).html(this.options.buttonText(options, this.$select));
            // Now update the title attribute of the button.
            $('button', this.$container).attr('title', this.options.buttonTitle(options, this.$select));
        },
        /**
         * Get all selected options.
         * 
         * @returns {jQUery}
         */
        getSelected: function () {
            return $('option[value!="' + this.options.selectAllValue + '"]:selected', this.$select).filter(function () {
                return $(this).attr('selected', 'selected');
                //return $(this).prop('selected');
            });
        },
        /**
         * Gets a select option by its value.
         * 
         * @param {String} value
         * @returns {jQuery}
         */
        getOptionByValue: function (value) {
            var options = $('option', this.$select);
            var valueToCompare = value.toString();
            for (var i = 0; i < options.length; i = i + 1) {
                var option = options[i];
                if (option.value === valueToCompare) {
                    return $(option);
                }
            }
        },
        /**
         * Get the input (radio/checkbox) by its value.
         * 
         * @param {String} value
         * @returns {jQuery}
         */
        getInputByValue: function (value) {
            var checkboxes = $('li input', this.$ul);
            var valueToCompare = value.toString();
            for (var i = 0; i < checkboxes.length; i = i + 1) {
                var checkbox = checkboxes[i];
                if (checkbox.value === valueToCompare) {
                    return $(checkbox);
                }
            }
        },
        /**
         * Used for knockout integration.
         */
        updateOriginalOptions: function () {
            this.originalOptions = this.$select.clone()[0].options;
        },
        // filter functions
        //call on keyup of search text box in multiselect dropdown when for getting filter value.
        //
        getfiltervalues: function (obj, e) {
            var oldvalues = "";
            var ele = document.getElementById(obj.attr("id"));
            if (ele != null) {
                for (var o = 0; o < ele.options.length; o++) {
                    if (ele.options[o].selected) {
                        oldvalues += ele.options[o].value + ",";
                    }
                }
            }

            if (oldvalues != "")
                oldvalues = oldvalues.trimEnd(',');

            var searchtxt = e.target.value;
            var hostingid = obj.attr("id");
            var parentDDid = obj.attr('ParentDD');
            var AssociationNames = obj.attr('AssoNameWithParent');
            var associationParam = "";
            if (parentDDid != null || parentDDid != undefined) {
                var Parents = parentDDid.split(",");
                var AssociationNameWithParent = "";
                var selectedParentVal = "";
                var ele = document.getElementById(Parents);
                if (ele != null) {
                    for (var o = 0; o < ele.options.length; o++) {
                        if (ele.options[o].selected) {
                            AssociationNameWithParent = AssociationNames.split(",")[0];
                            selectedParentVal += ele.options[o].value + ",";
                        }
                    }
                }
                associationParam = "AssoNameWithParent=" + AssociationNameWithParent + "&AssociationID=" + selectedParentVal;
            }
            var callingurl = obj.attr("dataurl");
            if (callingurl.indexOf("?") > -1) {
                if (searchtxt != undefined && searchtxt.length > 0)
                    callingurl = callingurl + "&key=" + searchtxt;
                if (associationParam.length > 0)
                    callingurl = callingurl + "&" + associationParam;
            }
            else {
                callingurl = callingurl + (searchtxt.length > 0 ? "?key=" + searchtxt + "&" + associationParam : "?" + associationParam);
            }
            if (oldvalues.length > 0)
                callingurl = addParameterToURL(callingurl, "ExtraVal", oldvalues);
            $.ajax({
                type: "GET",
                url: callingurl,
                contentType: "application/json; charset=utf-8",
                global: false,
                async: false,
                cache: false,
                dataType: "json",
                success: function (data) {
                    $('#' + hostingid).multiselect('dataproviderOnFilter', data, obj)
                },
            });
        },
        //function call on show drop down
        dataproviderOnShowDropDown: function (dataprovider, url) {
            
            var urlGetAll = $('#' + url).attr("dataurl").replace("GetAllMultiSelectValue", "Index");
            urlGetAll = addParameterToURL(urlGetAll, 'BulkOperation', 'multiple');
            var parentDDid = $("#" + url).attr("ParentDD");
            var hostingname = $("#" + url).attr("hostingname");
            var AssociationNames = $("#" + url).attr("AssoNameWithParent");
            if (parentDDid != null || parentDDid != undefined) {
                var Parents = parentDDid.split(",");
                var AssociationNameWithParent = "";
                var selectedParentVal = "";
                var parentdd = "";
                for (var i = 0; i < Parents.length; i++) {
                    var type = $("#" + Parents[i]).attr('type');
                    if (type == "hidden") {
                        AssociationNameWithParent = AssociationNames.split(",")[i];
                        selectedParentVal = $("#" + Parents[i]).val();
                        parentdd = Parents[i];
                    }
                    else {

                        if ($("option:selected", $("select#" + Parents[i])).val() != undefined && $("option:selected", $("select#" + Parents[i])).val().length > 0) {
                            AssociationNameWithParent = AssociationNames.split(",")[i];
                            selectedParentVal = $("option:selected", $("select#" + Parents[i])).val();
                            parentdd = Parents[i];
                        }
                    }
                }
                if (parentdd.length > 0)
                    urlGetAll = addParameterToURL(urlGetAll, 'HostingEntity', $("#" + parentdd).attr("hostingname"));

                if (AssociationNameWithParent.length > 0) {
                    var associatedtype = AssociationNameWithParent.substring(0, AssociationNameWithParent.length - 2);
                    if (associatedtype.indexOf("?") != -1) {
                        associatedtype = associatedtype.split("?")[1];
                    }
                    urlGetAll = addParameterToURL(urlGetAll, 'AssociatedType', associatedtype);
                }

                if (selectedParentVal.length > 0)
                    urlGetAll = addParameterToURL(urlGetAll, 'HostingEntityID', selectedParentVal);
            }
            var optionDOM = "";
            var optionDOMedit = "";
            if (this.editoptHtml.length > 0 && this.optCount != 0) {
                optionDOMedit = this.editoptHtml;
            }
            for (var i = 0; i < dataprovider.length; i++)
            {
                if (optionDOMedit.indexOf("value=\"" + dataprovider[i].Id + "\"") == -1)
                {
                    optionDOM += '<option value="' + dataprovider[i].Id + '">' + dataprovider[i].Name + '</option>';
                }
            }
            
            this.$select.html(optionDOM);
            this.$select.append(optionDOMedit);
            var selectedListLis = "";
            this.$ul.find('li').each(function () {
                if ($(this).attr('class') == 'active') {
                    selectedListLis += this.outerHTML;
                }
            })
            this.rebuild();
            this.buindlis(selectedListLis);
            if (dataprovider.length + this.optCount >= 10) {
                var hostingentity = url;
                var dispName = ($("label[for=\"" + hostingentity + "\"]").text());
                var link = "<a onclick=\"" + "OpenPopUpBulkOperation('PopupBulkOperation','" + hostingentity + "','" + dispName + "','dvPopupBulkOperation','" + urlGetAll + "')\">View All</a>";
                var getall = "<li class='disabled-result disabled-result' style='font-style:Italic;text-decoration:underline;' >" + link + "</li>";
                this.$ul.append($(getall));
            }
        },

        dataproviderOnFilter: function (dataprovider, obj) {
            
            var urlGetAll = obj.attr("dataurl").replace("GetAllMultiSelectValue", "Index");
            urlGetAll = addParameterToURL(urlGetAll, 'BulkOperation', 'multiple');
            var optionDOM = "";
            var optionDOMedit = "";
            if (this.editoptHtml.length > 0 && this.optCount != 0) {
                optionDOMedit = this.editoptHtml;
            }
            var oldvalues = "";
            var ele = document.getElementById(obj.attr("id"));
            if (ele != null) {
                for (var o = 0; o < ele.options.length; o++) {
                    if (ele.options[o].selected) {
                        oldvalues += ele.options[o].value + ",";
                    }
                }
            }
            if (oldvalues != "")
                oldvalues = oldvalues.trimEnd(',');

            this.$select.html('');

            var valNew = [];
            if (oldvalues != "") {
                valNew = oldvalues.split(",").filter(function (v) { return v !== '' });
            }

            for (var i = 0; i < dataprovider.length; i++) {
                if (this.$select.find("option[value='" + dataprovider[i].Id + "']").length == 0 && this.$select.find("option[value='" + dataprovider[i].Id + "']").attr('selected') == undefined) {
                    if (optionDOMedit.indexOf("value=\"" + dataprovider[i].Id + "\"") == -1)
                        if (jQuery.inArray(dataprovider[i].Id.toString(), valNew) !== -1) {
                            optionDOM += '<option value="' + dataprovider[i].Id + '" selected=selected>' + dataprovider[i].Name + '</option>';
                        }
                        else {
                            optionDOM += '<option value="' + dataprovider[i].Id + '">' + dataprovider[i].Name + '</option>';
                        }
                }
            }
            this.$select.append(optionDOM);
            this.$select.append(optionDOMedit);
            var len = $('li input', this.$ul).length;
            for (var i = 0; i < len; i++) {
                if ($('li input', this.$ul)[i].value.length > 0) {
                    this.$select.find("option[value='" + $('li input', this.$ul)[i].value + "']").css('display', 'none');
                }
            }

            this.buildDropdownOptions();
            var str = this.$ul.find('div').find('input').val();
            var selectedListLis = "";
            this.$ul.find('li').each(function () {
                if ($(this).attr('class') == 'active') {
                    selectedListLis += this.outerHTML;
                }
            })
            this.rebuild();
            this.buindlis(selectedListLis);
            if (this.$ul.find('input')[0] != undefined) {
                this.$ul.find('input')[0].value = str;
                SetFocusAtEnd(this.$ul.find('input')[0]);
            }
            //this.$ul.find('input')[0].focus();
            if (dataprovider.length + this.optCount >= 10) {
                var hostingentity = obj.attr("id");
                var dispName = ($("label[for=\"" + hostingentity + "\"]").text());
                var link = "<a onclick=\"" + "OpenPopUpBulkOperation('PopupBulkOperation','" + hostingentity + "','" + dispName + "','dvPopupBulkOperation','" + urlGetAll + "')\">View All</a>";
                var getall = "<li class='disabled-result disabled-result' style='font-style:Italic;text-decoration:underline;' >" + link + "</li>";
                this.$ul.append($(getall));
            }
            if (dataprovider.length == 0) {
                var hostingentity = obj.attr("id");
                var getall = "<li class='no-results style'font-style: Italic;margin: -10px 2px 0px 2px;border: 1px solid; >No results match<span>\"" + str + "\"</span> </li>";
                this.$ul.append(selectedListLis);
                this.$ul.append($(getall));
                this.buindlis(selectedListLis)
            }
        },
        buindlis: function (selectedListLis) {
            for (var j = 0; j < this.$ul.find('li').length; j++) {
                var litxt = $(this.$ul.find('li')[j]).text();
                var liobj1 = this.$ul.find('li');
                $(selectedListLis).each(function () {
                    if ($(liobj1[j]).text() == $(this).text() && $(this).attr('class') == 'active') {
                        $(liobj1[j]).attr('class', 'active')
                        $(liobj1[j]).find('input').prop('checked', true)
                    }
                });
            }
            var btntext = "";
            this.$ul.find('li').each(function () {
                if ($(this).attr('class') == 'active') {
                    btntext += $(this).text() + ",";
                }
            });
            if (btntext.length > 0) {

                btntext = btntext.substring(0, btntext.length - 1);
                // First update the displayed button text.
                $('button', this.$container).html(btntext);
                // Now update the title attribute of the button.
                $('button', this.$container).attr('title', btntext);
            }
        },
        //
        asyncFunction: function (callback, timeout, self) {
            var args = Array.prototype.slice.call(arguments, 3);
            return setTimeout(function () {
                callback.apply(self || window, args);
            }, timeout);
        }
    };
    $.fn.multiselect = function (option, parameter, url) {
        // alert(option);
        return this.each(function () {
            var data = $(this).data('multiselect');
            var options = typeof option === 'object' && option;
            // Initialize the multiselect.
            if (!data) {
                data = new Multiselect(this, options);
                $(this).data('multiselect', data);
            }
            // Call multiselect method.
            if (typeof option === 'string') {
                data[option](parameter, url);
                if (option === 'destroy') {
                    $(this).data('multiselect', false);
                }
            }
        });
    };
    $.fn.multiselect.Constructor = Multiselect;
    $(function () {
        $("select[data-role=multiselect]").multiselect();
    });
}(window.jQuery);
Array.prototype.insert = function (index, item) {
    this.splice(index, 0, item);
};

function SetFocusAtEnd(elem) {
    var elemLen = elem.value.length;
    // For IE Only
    if (document.selection) {
        // Set focus
        elem.focus();
        // Use IE Ranges
        var oSel = document.selection.createRange();
        // Reset position to 0 & then set at end
        oSel.moveStart('character', -elemLen);
        oSel.moveStart('character', elemLen);
        oSel.moveEnd('character', 0);
        oSel.select();
    }
    else if (elem.selectionStart || elem.selectionStart == '0') {
        // Firefox/Chrome
        elem.selectionStart = elemLen;
        elem.selectionEnd = elemLen;
        elem.focus();
    } // if
}
function HideShowul(obj) {
    $(document).click();
}
function closeSel(obj) {
    $(document).click();
}