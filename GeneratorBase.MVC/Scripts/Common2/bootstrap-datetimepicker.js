/*
Version 3.0.0
=========================================================
bootstrap-datetimepicker.js
https://github.com/Eonasdan/bootstrap-datetimepicker
=========================================================
The MIT License (MIT)
Copyright (c) 2014 Jonathan Peterson
Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:
The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.
THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*/
; (function (factory) {
    if (typeof define === 'function' && define.amd) {
        // AMD is used - Register as an anonymous module.
        define(['jquery', 'moment'], factory);
    } else {
        // AMD is not used - Attempt to fetch dependencies from scope.
        if (!jQuery) {
            throw 'bootstrap-datetimepicker requires jQuery to be loaded first';
        } else if (!moment) {
            throw 'bootstrap-datetimepicker requires moment.js to be loaded first';
        } else {
            factory(jQuery, moment);
        }
    }
}
(function ($, moment) {
    if (typeof moment === 'undefined') {
        alert("momentjs is requried");
        throw new Error('momentjs is required');
    };
    var dpgId = 0,
    pMoment = moment,
// ReSharper disable once InconsistentNaming
    DateTimePicker = function (element, options) {
        //datetimepickerLoad($(element), options);
        var defaults = {
            pickDate: true,
            pickTime: true,
            useMinutes: true,
            useSeconds: false,
            useCurrent: false,
            minuteStepping: 1,
            minDate: new pMoment({ y: 1900 }),
            maxDate: new pMoment().add(100, "y"),
            showToday: true,
            collapse: true,
            language: "en",
            defaultDate: "",
            disabledDates: false,
            enabledDates: false,
            icons: {},
            useStrict: false,
            direction: "auto",
            sideBySide: false,
            daysOfWeekDisabled: false
        },
		icons = {
		       time: 'far fa-clock',
		    date: 'far fa-calendar',
		    up: 'fa fa-chevron-up',
		    down: 'fa fa-chevron-down'
		},
        picker = this,
        init = function () {
            var icon = false, i, dDate, longDateFormat;
            picker.options = $.extend({}, defaults, options);
            picker.options.icons = $.extend({}, icons, picker.options.icons);
            picker.element = $(element);
            dataToOptions();
            //if (!(picker.options.pickTime || picker.options.pickDate))
            //    throw new Error('Must choose at least one picker');
            picker.id = dpgId++;
            pMoment.lang(picker.options.language);
            picker.date = pMoment();
            picker.unset = false;
            picker.isInput = picker.element.is('input');
            picker.component = false;
            if (picker.element.hasClass('input-group')) {
                if (picker.element.find('.datepickerbutton').length == 0) {//in case there is more then one 'input-group-addon' Issue #48
                    picker.component = picker.element.find("[class^='input-group-']");
                }
                else {
                    picker.component = picker.element.find('.datepickerbutton');
                }
            }
            picker.format = picker.options.format;
            longDateFormat = pMoment()._locale._longDateFormat;
            if (!picker.format) {
                picker.format = (picker.options.pickDate ? longDateFormat.L : '');
                if (picker.options.pickDate && picker.options.pickTime) picker.format += ' ';
                picker.format += (picker.options.pickTime ? longDateFormat.LT : '');
                if (picker.options.useSeconds) {
                    if (~longDateFormat.LT.indexOf(' A')) {
                        picker.format = picker.format.split(" A")[0] + ":ss A";
                    }
                    else {
                        picker.format += ':ss';
                    }
                }
            }
            picker.use24hours = picker.format.toLowerCase().indexOf("a") < 1;
            if (picker.component) icon = picker.component.find('span');
            if (picker.options.pickTime) {
                if (icon) icon.addClass(picker.options.icons.time);
            }
            if (picker.options.pickDate) {
                if (icon) {
                    icon.removeClass(picker.options.icons.time);
                    icon.addClass(picker.options.icons.date);
                }
            }
            picker.widget = $(getTemplate()).appendTo('body');
            if (picker.options.useSeconds && !picker.use24hours) {
                picker.widget.width(300);
            }
            picker.minViewMode = picker.options.minViewMode || 0;
            if (typeof picker.minViewMode === 'string') {
                switch (picker.minViewMode) {
                    case 'months':
                        picker.minViewMode = 1;
                        break;
                    case 'years':
                        picker.minViewMode = 2;
                        break;
                    default:
                        picker.minViewMode = 0;
                        break;
                }
            }
            picker.viewMode = picker.options.viewMode || 0;
            if (typeof picker.viewMode === 'string') {
                switch (picker.viewMode) {
                    case 'months':
                        picker.viewMode = 1;
                        break;
                    case 'years':
                        picker.viewMode = 2;
                        break;
                    default:
                        picker.viewMode = 0;
                        break;
                }
            }
            picker.options.disabledDates = indexGivenDates(picker.options.disabledDates);
            picker.options.enabledDates = indexGivenDates(picker.options.enabledDates);
            picker.startViewMode = picker.viewMode;
            picker.setMinDate(picker.options.minDate);
            picker.setMaxDate(picker.options.maxDate);
            fillDow();
            fillMonths();
            fillHours();
            fillMinutes();
            fillSeconds();
            update();
            showMode();
            attachDatePickerEvents();
            if (picker.options.defaultDate !== "" && getPickerInput().val() == "") picker.setValue(picker.options.defaultDate);
            if (picker.options.minuteStepping !== 1) {
                var rInterval = picker.options.minuteStepping;
                picker.date.minutes((Math.round(picker.date.minutes() / rInterval) * rInterval) % 60).seconds(0);
            }
        },
        getPickerInput = function () {
            if (picker.isInput) {
                return picker.element;
            } else {
                return dateStr = picker.element.find('input');
            }
        },
        dataToOptions = function () {
            var eData
            if (picker.element.is('input')) {
                eData = picker.element.data();
            }
            else {
                eData = picker.element.data();
            }
            if (eData.dateFormat !== undefined) picker.options.format = eData.dateFormat;
            if (eData.datePickdate !== undefined) picker.options.pickDate = eData.datePickdate;
            if (eData.datePicktime !== undefined) picker.options.pickTime = eData.datePicktime;
            if (eData.dateUseminutes !== undefined) picker.options.useMinutes = eData.dateUseminutes;
            if (eData.dateUseseconds !== undefined) picker.options.useSeconds = eData.dateUseseconds;
            if (eData.dateUsecurrent !== undefined) picker.options.useCurrent = eData.dateUsecurrent;
            if (eData.dateMinutestepping !== undefined) picker.options.minuteStepping = eData.dateMinutestepping;
            if (eData.dateMindate !== undefined) picker.options.minDate = eData.dateMindate;
            if (eData.dateMaxdate !== undefined) picker.options.maxDate = eData.dateMaxdate;
            if (eData.dateShowtoday !== undefined) picker.options.showToday = eData.dateShowtoday;
            if (eData.dateCollapse !== undefined) picker.options.collapse = eData.dateCollapse;
            if (eData.dateLanguage !== undefined) picker.options.language = eData.dateLanguage;
            if (eData.dateDefaultdate !== undefined) picker.options.defaultDate = eData.dateDefaultdate;
            if (eData.dateDisableddates !== undefined) picker.options.disabledDates = eData.dateDisableddates;
            if (eData.dateEnableddates !== undefined) picker.options.enabledDates = eData.dateEnableddates;
            if (eData.dateIcons !== undefined) picker.options.icons = eData.dateIcons;
            if (eData.dateUsestrict !== undefined) picker.options.useStrict = eData.dateUsestrict;
            if (eData.dateDirection !== undefined) picker.options.direction = eData.dateDirection;
            if (eData.dateSidebyside !== undefined) picker.options.sideBySide = eData.dateSidebyside;
        },
        place = function () {
            var position = 'absolute',
            offset = picker.component ? picker.component.offset() : picker.element.offset(), $window = $(window);
            picker.width = picker.component ? picker.component.outerWidth() : picker.element.outerWidth();
            offset.top = offset.top + picker.element.outerHeight();
            var placePosition;
            if (picker.options.direction === 'up') {
                placePosition = 'top'
            } else if (picker.options.direction === 'bottom') {
                placePosition = 'bottom'
            } else if (picker.options.direction === 'auto') {
                if (offset.top + picker.widget.height() > $window.height() + $window.scrollTop() && picker.widget.height() + picker.element.outerHeight() < offset.top) {
                    placePosition = 'top';
                } else {
                    placePosition = 'bottom';
                }
            };
            if (placePosition === 'top') {
                offset.top -= picker.widget.height() + picker.element.outerHeight() + 15;
                picker.widget.addClass('top').removeClass('bottom');
            } else {
                offset.top += 1;
                picker.widget.addClass('bottom').removeClass('top');
            }
            if (picker.options.width !== undefined) {
                picker.widget.width(picker.options.width);
            }
            if (picker.options.orientation === 'left') {
                picker.widget.addClass('left-oriented');
                offset.left = offset.left - picker.widget.width() + 20;
            }
            if (isInFixed()) {
                position = 'fixed';
                offset.top -= $window.scrollTop();
                offset.left -= $window.scrollLeft();
            }
            if ($window.width() < offset.left + picker.widget.outerWidth()) {
                offset.right = $window.width() - offset.left - picker.width;
                offset.left = 'auto';
                picker.widget.addClass('pull-right');
            } else {
                offset.right = 'auto';
                picker.widget.removeClass('pull-right');
            }
            picker.widget.css({
                position: position,
                top: offset.top,
                left: offset.left,
                right: offset.right
            });
        },
        notifyChange = function (oldDate, eventType) {
            if (pMoment(picker.date).isSame(pMoment(oldDate))) return;
            picker.element.trigger({
                type: 'dp.change',
                date: pMoment(picker.date),
                oldDate: pMoment(oldDate)
            });
            if (eventType !== 'change')
                picker.element.change();
        },
		notifyError = function (date) {
		    picker.element.trigger({
		        type: 'dp.error',
		        date: pMoment(date)
		    });
		},
        update = function (newDate) {
            pMoment.lang(picker.options.language);
            var dateStr = newDate;
            if (!dateStr) {
                dateStr = getPickerInput().val()
                if (dateStr) picker.date = pMoment(dateStr, picker.format, picker.options.useStrict);
                if (!picker.date) picker.date = pMoment();
            }
            picker.viewDate = pMoment(picker.date).startOf("month");
            fillDate();
            fillTime();
        },
		fillDow = function () {
		    pMoment.lang(picker.options.language);
		    var html = $('<tr>'), weekdaysMin = pMoment.weekdaysMin(), i;
		    if (pMoment()._locale._week.dow == 0) { // starts on Sunday
		        for (i = 0; i < 7; i++) {
		            html.append('<th class="dow">' + weekdaysMin[i] + '</th>');
		        }
		    } else {
		        for (i = 1; i < 8; i++) {
		            if (i == 7) {
		                html.append('<th class="dow">' + weekdaysMin[0] + '</th>');
		            } else {
		                html.append('<th class="dow">' + weekdaysMin[i] + '</th>');
		            }
		        }
		    }
		    picker.widget.find('.datepicker-days thead').append(html);
		},
        fillMonths = function () {
            pMoment.lang(picker.options.language);
            var html = '', i = 0, monthsShort = pMoment.monthsShort();
            while (i < 12) {
                html += '<span class="month">' + monthsShort[i++] + '</span>';
            }
            picker.widget.find('.datepicker-months td').append(html);
        },
        fillDate = function () {
            pMoment.lang(picker.options.language);
            var year = picker.viewDate.year(),
                month = picker.viewDate.month(),
                startYear = picker.options.minDate.year(),
                startMonth = picker.options.minDate.month(),
                endYear = picker.options.maxDate.year(),
                endMonth = picker.options.maxDate.month(),
                prevMonth, nextMonth, html = [], row, clsName, i, days, yearCont, currentYear, months = pMoment.months();
            picker.widget.find('.datepicker-days').find('.disabled').removeClass('disabled');
            picker.widget.find('.datepicker-months').find('.disabled').removeClass('disabled');
            picker.widget.find('.datepicker-years').find('.disabled').removeClass('disabled');
            picker.widget.find('.datepicker-days th:eq(1)').text(
                months[month] + ' ' + year);
            prevMonth = pMoment(picker.viewDate).subtract("months", 1);
            days = prevMonth.daysInMonth();
            prevMonth.date(days).startOf('week');
            if ((year == startYear && month <= startMonth) || year < startYear) {
                picker.widget.find('.datepicker-days th:eq(0)').addClass('disabled');
            }
            if ((year == endYear && month >= endMonth) || year > endYear) {
                picker.widget.find('.datepicker-days th:eq(2)').addClass('disabled');
            }
            nextMonth = pMoment(prevMonth).add(42, "d");
            while (prevMonth.isBefore(nextMonth)) {
                if (prevMonth.weekday() === pMoment().startOf('week').weekday()) {
                    row = $('<tr>');
                    html.push(row);
                }
                clsName = '';
                if (prevMonth.year() < year || (prevMonth.year() == year && prevMonth.month() < month)) {
                    clsName += ' old';
                } else if (prevMonth.year() > year || (prevMonth.year() == year && prevMonth.month() > month)) {
                    clsName += ' new';
                }
                if (prevMonth.isSame(pMoment({ y: picker.date.year(), M: picker.date.month(), d: picker.date.date() }))) {
                    clsName += ' active';
                }
                if (isInDisableDates(prevMonth) || !isInEnableDates(prevMonth)) {
                    clsName += ' disabled';
                }
                if (picker.options.showToday === true) {
                    if (prevMonth.isSame(pMoment(), 'day')) {
                        clsName += ' today';
                    }
                }
                if (picker.options.daysOfWeekDisabled) {
                    for (i in picker.options.daysOfWeekDisabled) {
                        if (prevMonth.day() == picker.options.daysOfWeekDisabled[i]) {
                            clsName += ' disabled';
                            break;
                        }
                    }
                }
                row.append('<td class="day' + clsName + '">' + prevMonth.date() + '</td>');
                prevMonth.add(1, "d");
            }
            picker.widget.find('.datepicker-days tbody').empty().append(html);
            currentYear = picker.date.year(), months = picker.widget.find('.datepicker-months')
				.find('th:eq(1)').text(year).end().find('span').removeClass('active');
            if (currentYear === year) {
                months.eq(picker.date.month()).addClass('active');
            }
            if (currentYear - 1 < startYear) {
                picker.widget.find('.datepicker-months th:eq(0)').addClass('disabled');
            }
            if (currentYear + 1 > endYear) {
                picker.widget.find('.datepicker-months th:eq(2)').addClass('disabled');
            }
            for (i = 0; i < 12; i++) {
                if ((year == startYear && startMonth > i) || (year < startYear)) {
                    $(months[i]).addClass('disabled');
                } else if ((year == endYear && endMonth < i) || (year > endYear)) {
                    $(months[i]).addClass('disabled');
                }
            }
            html = '';
            year = parseInt(year / 10, 10) * 10;
            yearCont = picker.widget.find('.datepicker-years').find(
                'th:eq(1)').text(year + '-' + (year + 9)).end().find('td');
            picker.widget.find('.datepicker-years').find('th').removeClass('disabled');
            if (startYear > year) {
                picker.widget.find('.datepicker-years').find('th:eq(0)').addClass('disabled');
            }
            if (endYear < year + 9) {
                picker.widget.find('.datepicker-years').find('th:eq(2)').addClass('disabled');
            }
            year -= 1;
            for (i = -1; i < 11; i++) {
                html += '<span class="year' + (i === -1 || i === 10 ? ' old' : '') + (currentYear === year ? ' active' : '') + ((year < startYear || year > endYear) ? ' disabled' : '') + '">' + year + '</span>';
                year += 1;
            }
            yearCont.html(html);
        },
        fillHours = function () {
            pMoment.lang(picker.options.language);
            var table = picker.widget.find('.timepicker .timepicker-hours table'), html = '', current, i, j;
            table.parent().hide();
            if (picker.use24hours) {
                current = 0;
                for (i = 0; i < 6; i += 1) {
                    html += '<tr>';
                    for (j = 0; j < 4; j += 1) {
                        html += '<td class="hour">' + padLeft(current.toString()) + '</td>';
                        current++;
                    }
                    html += '</tr>';
                }
            }
            else {
                current = 1;
                for (i = 0; i < 3; i += 1) {
                    html += '<tr>';
                    for (j = 0; j < 4; j += 1) {
                        html += '<td class="hour">' + padLeft(current.toString()) + '</td>';
                        current++;
                    }
                    html += '</tr>';
                }
            }
            table.html(html);
        },
        fillMinutes = function () {
            var table = picker.widget.find('.timepicker .timepicker-minutes table'), html = '', current = 0, i, j, step = picker.options.minuteStepping;
            table.parent().hide();
            if (step == 1) step = 5;
            for (i = 0; i < Math.ceil(60 / step / 4) ; i++) {
                html += '<tr>';
                for (j = 0; j < 4; j += 1) {
                    if (current < 60) {
                        html += '<td class="minute">' + padLeft(current.toString()) + '</td>';
                        current += step;
                    } else {
                        html += '<td></td>';
                    }
                }
                html += '</tr>';
            }
            table.html(html);
        },
        fillSeconds = function () {
            var table = picker.widget.find('.timepicker .timepicker-seconds table'), html = '', current = 0, i, j;
            table.parent().hide();
            for (i = 0; i < 3; i++) {
                html += '<tr>';
                for (j = 0; j < 4; j += 1) {
                    html += '<td class="second">' + padLeft(current.toString()) + '</td>';
                    current += 5;
                }
                html += '</tr>';
            }
            table.html(html);
        },
        fillTime = function () {
            if (!picker.date) return;
            var timeComponents = picker.widget.find('.timepicker span[data-time-component]'),
            hour = picker.date.hours(),
            period = 'AM';
            if (!picker.use24hours) {
                if (hour >= 12) period = 'PM';
                if (hour === 0) hour = 12;
                else if (hour != 12) hour = hour % 12;
                picker.widget.find('.timepicker [data-action=togglePeriod]').text(period);
            }
            timeComponents.filter('[data-time-component=hours]').text(padLeft(hour));
            timeComponents.filter('[data-time-component=minutes]').text(padLeft(picker.date.minutes()));
            timeComponents.filter('[data-time-component=seconds]').text(padLeft(picker.date.second()));
        },
            keypress = function (e) {
                var dateChanged = false,
                    dir, newDate, newViewDate,
                    focusDate = this.focusDate || this.viewDate;
                switch (e.keyCode) {
                    case 27: // escape
                        picker.hide();
                        e.preventDefault();
                        break;
                        //case 32: // spacebar
                        //    fillDate();
                        //    set();
                        //    picker.hide();
                        //    break;
                        //case 13: // enter
                        //    fillDate();
                        //    set();
                        //    picker.hide();
                        //    e = $.event.fix(e);
                        //    e.preventDefault();
                        //    e.stopPropagation();
                        //    break;
                    case 9: // tab
                        picker.hide();
                        break;
                }
                if (dateChanged) {
                    if (this.dates.length)
                        this._trigger('changeDate');
                    else
                        this._trigger('clearDate');
                    var element;
                    if (this.isInput) {
                        element = this.element;
                    } else if (this.component) {
                        element = this.element.find('input');
                    }
                    if (element) {
                        element.change();
                    }
                }
            },
        click = function (e) {
            e.stopPropagation();
            e.preventDefault();
            picker.unset = false;
            var target = $(e.target).closest('span, td, th'), month, year, step, day, oldDate = pMoment(picker.date);
            if (target.length === 1) {
                if (!target.is('.disabled')) {
                    switch (target[0].nodeName.toLowerCase()) {
                        case 'th':
                            switch (target[0].className) {
                                case 'switch':
                                    showMode(1);
                                    break;
                                case 'prev':
                                case 'next':
                                    step = dpGlobal.modes[picker.viewMode].navStep;
                                    if (target[0].className === 'prev') step = step * -1;
                                    picker.viewDate.add(step, dpGlobal.modes[picker.viewMode].navFnc);
                                    fillDate();
                                    break;
                            }
                            break;
                        case 'span':
                            if (target.is('.month')) {
                                month = target.parent().find('span').index(target);
                                picker.viewDate.month(month);
                            } else {
                                year = parseInt(target.text(), 10) || 0;
                                picker.viewDate.year(year);
                            }
                            if (picker.viewMode === picker.minViewMode) {
                                picker.date = pMoment({
                                    y: picker.viewDate.year(),
                                    M: picker.viewDate.month(),
                                    d: picker.viewDate.date(),
                                    h: picker.date.hours(),
                                    m: picker.date.minutes(),
                                    s: picker.date.seconds()
                                });
                                notifyChange(oldDate, e.type);
                                set();
                            }
                            showMode(-1);
                            fillDate();
                            break;
                        case 'td':
                            if (target.is('.day')) {
                                day = parseInt(target.text(), 10) || 1;
                                month = picker.viewDate.month();
                                year = picker.viewDate.year();
                                if (target.is('.old')) {
                                    if (month === 0) {
                                        month = 11;
                                        year -= 1;
                                    } else {
                                        month -= 1;
                                    }
                                } else if (target.is('.new')) {
                                    if (month == 11) {
                                        month = 0;
                                        year += 1;
                                    } else {
                                        month += 1;
                                    }
                                }
                                picker.date = pMoment({
                                    y: year,
                                    M: month,
                                    d: day,
                                    h: picker.date.hours(),
                                    m: picker.date.minutes(),
                                    s: picker.date.seconds()
                                }
                                );
                                picker.viewDate = pMoment({
                                    y: year, M: month, d: Math.min(28, day)
                                });
                                fillDate();
                                set();
                                notifyChange(oldDate, e.type);
                            }
                            break;
                    }
                }
            }
        },
		actions = {
		    incrementHours: function () {
		        checkDate("add", "hours", 1);
		    },
		    incrementMinutes: function () {
		        checkDate("add", "minutes", picker.options.minuteStepping);
		    },
		    incrementSeconds: function () {
		        checkDate("add", "seconds", 1);
		    },
		    decrementHours: function () {
		        checkDate("subtract", "hours", 1);
		    },
		    decrementMinutes: function () {
		        checkDate("subtract", "minutes", picker.options.minuteStepping);
		    },
		    decrementSeconds: function () {
		        checkDate("subtract", "seconds", 1);
		    },
		    togglePeriod: function () {
		        var hour = picker.date.hours();
		        if (hour >= 12) hour -= 12;
		        else hour += 12;
		        picker.date.hours(hour);
		    },
		    showPicker: function () {
		        picker.widget.find('.timepicker > div:not(.timepicker-picker)').hide();
		        picker.widget.find('.timepicker .timepicker-picker').show();
		    },
		    showHours: function () {
		        picker.widget.find('.timepicker .timepicker-picker').hide();
		        picker.widget.find('.timepicker .timepicker-hours').show();
		    },
		    showMinutes: function () {
		        picker.widget.find('.timepicker .timepicker-picker').hide();
		        picker.widget.find('.timepicker .timepicker-minutes').show();
		    },
		    showSeconds: function () {
		        picker.widget.find('.timepicker .timepicker-picker').hide();
		        picker.widget.find('.timepicker .timepicker-seconds').show();
		    },
		    selectHour: function (e) {
		        var period = picker.widget.find('.timepicker [data-action=togglePeriod]').text(), hour = parseInt($(e.target).text(), 10);
		        if (period == "PM") hour += 12
		        picker.date.hours(hour);
		        actions.showPicker.call(picker);
		    },
		    selectMinute: function (e) {
		        picker.date.minutes(parseInt($(e.target).text(), 10));
		        actions.showPicker.call(picker);
		    },
		    selectSecond: function (e) {
		        picker.date.seconds(parseInt($(e.target).text(), 10));
		        actions.showPicker.call(picker);
		    }
		},
	    doAction = function (e) {
	        var oldDate = pMoment(picker.date), action = $(e.currentTarget).data('action'), rv = actions[action].apply(picker, arguments);
	        stopEvent(e);
	        if (!picker.date) picker.date = pMoment({ y: 1970 });
	        set();
	        fillTime();
	        notifyChange(oldDate, e.type);
	        return rv;
	    },
        stopEvent = function (e) {
            e.stopPropagation();
            e.preventDefault();
        },
        change = function (e) {
            pMoment.lang(picker.options.language);
            var input = $(e.target), oldDate = pMoment(picker.date), newDate = pMoment(input.val(), picker.format, picker.options.useStrict);
            if (newDate.isValid() && !isInDisableDates(newDate) && isInEnableDates(newDate)) {
                update();
                picker.setValue(newDate);
                notifyChange(oldDate, e.type);
                set();
            }
            else {
                picker.viewDate = oldDate;
                notifyChange(oldDate, e.type);
                notifyError(newDate);
                picker.unset = true;
            }
        },
        showMode = function (dir) {
            if (dir) {
                picker.viewMode = Math.max(picker.minViewMode, Math.min(2, picker.viewMode + dir));
            }
            var f = dpGlobal.modes[picker.viewMode].clsName;
            picker.widget.find('.datepicker > div').hide().filter('.datepicker-' + dpGlobal.modes[picker.viewMode].clsName).show();
        },
        attachDatePickerEvents = function () {
            var $this, $parent, expanded, closed, collapseData;
            picker.widget.on('click', '.datepicker *', $.proxy(click, this)); // this handles date picker clicks
            picker.widget.on('click', '[data-action]', $.proxy(doAction, this)); // this handles time picker clicks
            picker.widget.on('mousedown', $.proxy(stopEvent, this));
            if (picker.options.pickDate && picker.options.pickTime) {
                picker.widget.on('click.togglePicker', '.accordion-toggle', function (e) {
                    e.stopPropagation();
                    $this = $(this);
                    $parent = $this.closest('ul');
                    expanded = $parent.find('.show');
                    closed = $parent.find('.collapse:not(.show)');
                    if (expanded && expanded.length) {
                        collapseData = expanded.data('collapse');
                        if (collapseData && collapseData.date - transitioning) return;
                        expanded.collapse('hide');
                        closed.collapse('show');
                        $this.find('span').toggleClass(picker.options.icons.time + ' ' + picker.options.icons.date);
                        picker.element.find('.input-group-addon span').toggleClass(picker.options.icons.time + ' ' + picker.options.icons.date);
                    }
                });
            }
            if (picker.isInput) {
                picker.element.on({
                    'focus': $.proxy(picker.show, this),
                    'change': $.proxy(change, this),
                    'keydown': $.proxy(keypress, this)
                    // To Support IE8 Browser
                    // Modified to allow DateTime pickers to hide on clicking out of the picker
                    //'blur': $.proxy(picker.hide, this)
                });
            } else {
                picker.element.on({
                    'change': $.proxy(change, this)
                }, 'input');
                if (picker.component) {
                    picker.component.on('click', $.proxy(picker.show, this));
                } else {
                    picker.element.on('click', $.proxy(picker.show, this));
                }
            }
        },
        attachDatePickerGlobalEvents = function () {
            $(window).on(
                'resize.datetimepicker' + picker.id, $.proxy(place, this));
            // To Support IE8 Browser
            // Modified to allow DateTime pickers to hide on clicking out of the picker
            // when the picker is configured for both Date and Time
            //if (!picker.isInput) {
            $(document).on(
                'mousedown.datetimepicker' + picker.id, $.proxy(picker.hide, this));
            //}
            //$(document).on(
            //          'keydown.datetimepicker' + picker.id, $.proxy(picker.hide, this));
            //$(document).keydown(function (e) {
            //    if (e.keyCode == 13) {
            //        e.stopPropagation();
            //        e.preventDefault();
            //        return false;
            //    }
            //});
        },
        detachDatePickerEvents = function () {
            picker.widget.off('click', '.datepicker *', picker.click);
            picker.widget.off('click', '[data-action]');
            picker.widget.off('mousedown', picker.stopEvent);
            if (picker.options.pickDate && picker.options.pickTime) {
                picker.widget.off('click.togglePicker');
            }
            if (picker.isInput) {
                picker.element.off({
                    'focus': picker.show,
                    'change': picker.change,
                    'keydown': picker.keypress
                });
            } else {
                picker.element.off({
                    'change': picker.change
                }, 'input');
                if (picker.component) {
                    picker.component.off('click', picker.show);
                } else {
                    picker.element.off('click', picker.show);
                }
            }
        },
        detachDatePickerGlobalEvents = function () {
            $(window).off('resize.datetimepicker' + picker.id);
            if (!picker.isInput) {
                $(document).off('mousedown.datetimepicker' + picker.id);
            }
            //if (!picker.isInput) {
            //    $(window).off('keydown.datetimepicker' + picker.id);
            //}
        },
        isInFixed = function () {
            if (picker.element) {
                var parents = picker.element.parents(), inFixed = false, i;
                for (i = 0; i < parents.length; i++) {
                    if ($(parents[i]).css('position') == 'fixed') {
                        inFixed = true;
                        break;
                    }
                }
                ;
                return inFixed;
            } else {
                return false;
            }
        },
        set = function () {
            pMoment.lang(picker.options.language);
            var formatted = '', input;
            if (!picker.unset) formatted = pMoment(picker.date).format(picker.format);
            getPickerInput().val(formatted);
            picker.element.data('date', formatted);
            if (!picker.options.pickTime) picker.hide();
        },
		checkDate = function (direction, unit, amount) {
		    pMoment.lang(picker.options.language);
		    var newDate;
		    if (direction == "add") {
		        newDate = pMoment(picker.date);
		        if (newDate.hours() == 23) newDate.add(amount, unit);
		        newDate.add(amount, unit);
		    }
		    else {
		        newDate = pMoment(picker.date).subtract(amount, unit);
		    }
		    if (isInDisableDates(pMoment(newDate.subtract(amount, unit))) || isInDisableDates(newDate)) {
		        notifyError(newDate.format(picker.format));
		        return;
		    }
		    if (direction == "add") {
		        picker.date.add(amount, unit);
		    }
		    else {
		        picker.date.subtract(amount, unit);
		    }
		    picker.unset = false;
		},
        isInDisableDates = function (date) {
            pMoment.lang(picker.options.language);
            if (date.isAfter(picker.options.maxDate) || date.isBefore(picker.options.minDate)) return true;
            if (picker.options.disabledDates === false) {
                return false;
            }
            return picker.options.disabledDates[pMoment(date).format("YYYY-MM-DD")] === true;
        },
        isInEnableDates = function (date) {
            pMoment.lang(picker.options.language);
            if (picker.options.enabledDates === false) {
                return true;
            }
            return picker.options.enabledDates[pMoment(date).format("YYYY-MM-DD")] === true;
        },
        indexGivenDates = function (givenDatesArray) {
            // Store given enabledDates and disabledDates as keys.
            // This way we can check their existence in O(1) time instead of looping through whole array.
            // (for example: picker.options.enabledDates['2014-02-27'] === true)
            var givenDatesIndexed = {};
            var givenDatesCount = 0;
            for (i = 0; i < givenDatesArray.length; i++) {
                dDate = pMoment(givenDatesArray[i]);
                if (dDate.isValid()) {
                    givenDatesIndexed[dDate.format("YYYY-MM-DD")] = true;
                    givenDatesCount++;
                }
            }
            if (givenDatesCount > 0) {
                return givenDatesIndexed;
            }
            return false;
        },
        padLeft = function (string) {
            string = string.toString();
            if (string.length >= 2) return string;
            else return '0' + string;
        },
        getTemplate = function () {
            if (picker.options.pickDate && picker.options.pickTime) {
                var ret = '';
                ret = '<div class="bootstrap-datetimepicker-widget' + (picker.options.sideBySide ? ' timepicker-sbs' : '') + ' dropdown-menu" style="z-index:9999 !important;">';
                if (picker.options.sideBySide) {
                    ret += '<div class="row">' +
                       '<div class="col-sm-6 datepicker">' + dpGlobal.template + '</div>' +
                       '<div class="col-sm-6 timepicker">' + tpGlobal.getTemplate() + '</div>' +
                     '</div>';
                } else {
                    ret += '<ul class="list-unstyled">' +
                        '<li' + (picker.options.collapse ? ' class="collapse show"' : '') + '>' +
                            '<div class="datepicker">' + dpGlobal.template + '</div>' +
                        '</li>' +
                        '<li class="picker-switch accordion-toggle"><a class="btn" style="width:100%"><span class="' + picker.options.icons.time + '"></span></a></li>' +
                        '<li' + (picker.options.collapse ? ' class="collapse"' : '') + '>' +
                            '<div class="timepicker">' + tpGlobal.getTemplate() + '</div>' +
                        '</li>' +
                   '</ul>';
                }
                ret += '</div>';
                return ret;
            } else if (picker.options.pickTime) {
                return (
                    '<div class="bootstrap-datetimepicker-widget dropdown-menu">' +
                        '<div class="timepicker">' + tpGlobal.getTemplate() + '</div>' +
                    '</div>'
                );
            } else {
                return (
                    '<div class="bootstrap-datetimepicker-widget dropdown-menu">' +
                        '<div class="datepicker">' + dpGlobal.template + '</div>' +
                    '</div>'
                );
            }
        },
		dpGlobal = {
		    modes: [
                {
                    clsName: 'days',
                    navFnc: 'month',
                    navStep: 1
                },
                {
                    clsName: 'months',
                    navFnc: 'year',
                    navStep: 1
                },
                {
                    clsName: 'years',
                    navFnc: 'year',
                    navStep: 10
                }],
		    headTemplate:
                    '<thead>' +
						'<tr>' +
							'<th class="prev">&lsaquo;</th><th colspan="5" class="switch"></th><th class="next">&rsaquo;</th>' +
						'</tr>' +
                    '</thead>',
		    contTemplate:
        '<tbody><tr><td colspan="7"></td></tr></tbody>'
		},
        tpGlobal = {
            hourTemplate: '<span data-action="showHours"   data-time-component="hours"   class="timepicker-hour"></span>',
            minuteTemplate: '<span data-action="showMinutes" data-time-component="minutes" class="timepicker-minute"></span>',
            secondTemplate: '<span data-action="showSeconds"  data-time-component="seconds" class="timepicker-second"></span>'
        };
        dpGlobal.template =
            '<div class="datepicker-days">' +
                '<table class="table-condensed">' + dpGlobal.headTemplate + '<tbody></tbody></table>' +
            '</div>' +
            '<div class="datepicker-months">' +
                '<table class="table-condensed">' + dpGlobal.headTemplate + dpGlobal.contTemplate + '</table>' +
            '</div>' +
            '<div class="datepicker-years">' +
				'<table class="table-condensed">' + dpGlobal.headTemplate + dpGlobal.contTemplate + '</table>' +
            '</div>';
        tpGlobal.getTemplate = function () {
            return (
                '<div class="timepicker-picker">' +
                    '<table class="table-condensed">' +
						'<tr>' +
							'<td><a href="#" class="btn" data-action="incrementHours"><span class="' + picker.options.icons.up + '"></span></a></td>' +
							'<td class="separator"></td>' +
							'<td>' + (picker.options.useMinutes ? '<a href="#" class="btn" data-action="incrementMinutes"><span class="' + picker.options.icons.up + '"></span></a>' : '') + '</td>' +
                            (picker.options.useSeconds ?
                                '<td class="separator"></td><td><a href="#" class="btn" data-action="incrementSeconds"><span class="' + picker.options.icons.up + '"></span></a></td>' : '') +
							(picker.use24hours ? '' : '<td class="separator"></td>') +
						'</tr>' +
						'<tr>' +
							'<td>' + tpGlobal.hourTemplate + '</td> ' +
							'<td class="separator">:</td>' +
							'<td>' + (picker.options.useMinutes ? tpGlobal.minuteTemplate : '<span class="timepicker-minute">00</span>') + '</td> ' +
                            (picker.options.useSeconds ?
                                '<td class="separator">:</td><td>' + tpGlobal.secondTemplate + '</td>' : '') +
							(picker.use24hours ? '' : '<td class="separator"></td>' +
							'<td><button type="button" class="btn btn-primary" data-action="togglePeriod"></button></td>') +
						'</tr>' +
						'<tr>' +
							'<td><a href="#" class="btn" data-action="decrementHours"><span class="' + picker.options.icons.down + '"></span></a></td>' +
							'<td class="separator"></td>' +
							'<td>' + (picker.options.useMinutes ? '<a href="#" class="btn" data-action="decrementMinutes"><span class="' + picker.options.icons.down + '"></span></a>' : '') + '</td>' +
                            (picker.options.useSeconds ?
                                '<td class="separator"></td><td><a href="#" class="btn" data-action="decrementSeconds"><span class="' + picker.options.icons.down + '"></span></a></td>' : '') +
							(picker.use24hours ? '' : '<td class="separator"></td>') +
						'</tr>' +
                    '</table>' +
                '</div>' +
                '<div class="timepicker-hours" data-action="selectHour">' +
                    '<table class="table-condensed"></table>' +
                '</div>' +
                '<div class="timepicker-minutes" data-action="selectMinute">' +
                    '<table class="table-condensed"></table>' +
                '</div>' +
                (picker.options.useSeconds ?
                    '<div class="timepicker-seconds" data-action="selectSecond"><table class="table-condensed"></table></div>' : '')
            );
        };
        picker.destroy = function () {
            detachDatePickerEvents();
            detachDatePickerGlobalEvents();
            picker.widget.remove();
            picker.element.removeData('DateTimePicker');
            if (picker.component)
                picker.component.removeData('DateTimePicker');
        };
        picker.show = function (e) {
            if (picker.options.useCurrent) {
                if (getPickerInput().val() == '') {
                    if (picker.options.minuteStepping !== 1) {
                        var mDate = pMoment(),
                        rInterval = picker.options.minuteStepping;
                        mDate.minutes((Math.round(mDate.minutes() / rInterval) * rInterval) % 60)
                            .seconds(0);
                        picker.setValue(mDate.format(picker.format))
                    } else {
                        picker.setValue(pMoment().format(picker.format))
                    }
                };
            }
            picker.widget.show();
            picker.height = picker.component ? picker.component.outerHeight() : picker.element.outerHeight();
            place();
            picker.element.trigger({
                type: 'dp.show',
                date: pMoment(picker.date)
            });
            attachDatePickerGlobalEvents();
            if (e) {
                stopEvent(e);
            }
        },
        picker.disable = function () {
            var input = picker.element.find('input');
            if (input.prop('disabled')) return;
            input.prop('disabled', true);
            detachDatePickerEvents();
        },
        picker.enable = function () {
            var input = picker.element.find('input');
            if (!input.prop('disabled')) return;
            input.prop('disabled', false);
            attachDatePickerEvents();
        },
        picker.hide = function (event) {
            if (event && $(event.target).is(picker.element.attr("id")))
                return;
            // Ignore event if in the middle of a picker transition
            var collapse = picker.widget.find('.collapse'), i, collapseData;
            for (i = 0; i < collapse.length; i++) {
                collapseData = collapse.eq(i).data('collapse');
                if (collapseData && collapseData.date - transitioning)
                    return;
            }
            picker.widget.hide();
            picker.viewMode = picker.startViewMode;
            showMode();
            picker.element.trigger({
                type: 'dp.hide',
                date: pMoment(picker.date)
            });
            detachDatePickerGlobalEvents();
        },
        picker.setValue = function (newDate) {
            pMoment.lang(picker.options.language);
            if (!newDate) {
                picker.unset = true;
                set();
            } else {
                picker.unset = false;
            }
            if (!pMoment.isMoment(newDate)) newDate = pMoment(newDate, picker.format);
            if (newDate.isValid()) {
                picker.date = newDate;
                set();
                picker.viewDate = pMoment({ y: picker.date.year(), M: picker.date.month() });
                fillDate();
                fillTime();
            }
            else {
                notifyError(newDate);
            }
        },
        picker.getDate = function () {
            if (picker.unset) return null;
            return picker.date;
        },
        picker.setDate = function (date) {
            var oldDate = pMoment(picker.date);
            if (!date) {
                picker.setValue(null);
            } else {
                picker.setValue(date);
            }
            notifyChange(oldDate, "function");
        },
        picker.setDisabledDates = function (dates) {
            picker.options.disabledDates = indexGivenDates(dates);
            if (picker.viewDate) update();
        },
        picker.setEnabledDates = function (dates) {
            picker.options.enabledDates = indexGivenDates(dates);
            if (picker.viewDate) update();
        },
        picker.setMaxDate = function (date) {
            if (date == undefined) return;
            picker.options.maxDate = pMoment(date);
            if (picker.viewDate) update();
        },
        picker.setMinDate = function (date) {
            if (date == undefined) return;
            picker.options.minDate = pMoment(date);
            if (picker.viewDate) update();
        };
        init();
    };
    $.fn.datetimepicker = function (options) {
        return this.each(function () {
            var $this = $(this), data = $this.data('DateTimePicker');
            if (!data) $this.data('DateTimePicker', new DateTimePicker(this, options));
            //
            if ($(this).val() == "") {
                if ($(this).attr("id").includes("datetimepicker")) {
                    datetimepickerLoad(this, options);
                }
            }
            //
        });
    };
    $.fn.datetimepickerIndex = function (options) {
        //if (options.divid != undefined)
        //    datetimepickerLoadIndex(options)

    };
}));

function datetimepickerLoadIndex(options) {
    var localDateString = "";
    var amPm = "";
    if (options.val != "") {
        if (options.format != undefined) {
            var format = options.format;
            if (format.trim() == "MM/DD/YYYY hh:mm".trim()) {
                var date = new Date(options.val);
                var convertedTime = convertLocalDateToUTCDateOnload(date, false);
                localDateString = moment(convertedTime).format(options.format + " A");
                $("#" + options.divid).html(localDateString);
            }
            else if (format.trim() == "MM/DD/YYYY HH:mm".trim()) {
                var date = new Date(options.val);
                var convertedTime = convertLocalDateToUTCDateOnload(date, false);
                localDateString = moment(convertedTime).format(format);
                $("#" + options.divid).html(localDateString);
            }
            else if (format.trim() == "HH:mm".trim() || format.trim() == "hh:mm".trim()) {
                var time = options.val;
                var convertedTime = ConvertTimeOnlyToDateTime(time, false);
                if (format.trim() == "hh:mm".trim())
                    localDateString = moment(convertedTime).format(format + " A");
                else
                    localDateString = moment(convertedTime).format(format);
                $("#" + options.divid).html(localDateString);
            }
        }
    }
}
function datetimepickerLoad(textBox, options) {

    var localDateString = "";
    var amPm = "";
    if ($(textBox).val() != "") {
        if ($(textBox).attr("format") != undefined) {
            var format = $(textBox).attr("format");
            if (format.trim() == "MM/DD/YYYY hh:mm".trim()) {
                var date = new Date($(textBox).val());
                var convertedTime = "";
                if (options != undefined) {
                    if (options.IsRequired != undefined)
                        convertedTime = new Date();
                    else
                        convertedTime = convertLocalDateToUTCDateOnload(date, false);
                }
                else
                    convertedTime = convertLocalDateToUTCDateOnload(date, false);
                localDateString = moment(convertedTime).format(format + " A");
                $(textBox).val(localDateString);
            }
            else if (format.trim() == "MM/DD/YYYY HH:mm".trim()) {
                var date = new Date($(textBox).val());
                var convertedTime = "";
                if (options.IsRequired != undefined)
                    convertedTime = new Date();
                else
                    convertedTime = convertLocalDateToUTCDateOnload(date, false);
                localDateString = moment(convertedTime).format(format);
                $(textBox).val(localDateString);

            }
            else if (format.trim() == "HH:mm".trim() || format.trim() == "hh:mm".trim()) {
                var time = $(textBox).val();
                var convertedTime = "";
                if (options.IsRequired != undefined) {
                    convertedTime = new Date();
                    if (format.trim() == "hh:mm".trim())
                        localDateString = moment(convertedTime).format(format + " A");
                    else
                        localDateString = moment(convertedTime).format(format);
                }
                else {
                    convertedTime = ConvertTimeOnlyToDateTime(time, false);
                    if (format.trim() == "hh:mm".trim())
                        localDateString = moment(convertedTime).format(format + " A");
                    else
                        localDateString = moment(convertedTime).format(format);
                }
                $(textBox).val(localDateString);
            }

        }
    }
}

function ConvertTimeOnlyToDateTimeSaveEdit(actual,time,toUTC)
{  
    var temp = time;
    var TimeOnly = new Date(actual);
    var time = time.match(/(\d+)(?::(\d\d))?\s*(p?)/);
    TimeOnly.setHours(parseInt(time[1]));
    if (temp.match("PM$")) {
        TimeOnly.setHours(0, 0, 0, 0)
        if (!temp.match("^12")) {
            TimeOnly.setHours(parseInt(time[1]) + 12);
        }
        else {
            TimeOnly.setHours(parseInt(time[1]));
        }
    }
    else if (temp.match("AM$")) {
        TimeOnly.setHours(0, 0, 0, 0)
        if (temp.match("^12")) {
            TimeOnly.setHours(parseInt(time[1]) - 12);
        }
        else {
            TimeOnly.setHours(parseInt(time[1]));
        }
    }
    TimeOnly.setMinutes(parseInt(time[2]));
    return new Date(TimeOnly);
}

function ConvertTimeOnlyToDateTimeSave(time, toUTC) {

    var temp = time;
    var TimeOnly = new Date();
    var time = time.match(/(\d+)(?::(\d\d))?\s*(p?)/);
    TimeOnly.setHours(parseInt(time[1]));
    if (temp.match("PM$")) {
        TimeOnly.setHours(0, 0, 0, 0)
        if (!temp.match("^12")) {
            TimeOnly.setHours(parseInt(time[1]) + 12);
        }
        else {
            TimeOnly.setHours(parseInt(time[1]));
        }
    }
    else if (temp.match("AM$")) {
        TimeOnly.setHours(0, 0, 0, 0)
        if (temp.match("^12")) {
            TimeOnly.setHours(parseInt(time[1]) - 12);
        }
        else {
            TimeOnly.setHours(parseInt(time[1]));
        }
    }
    TimeOnly.setMinutes(parseInt(time[2]));
    return new Date(TimeOnly);
}
function ConvertTimeOnlyToDateTime(time, toUTC) {
    var temp = time;
    var TimeOnly = new Date();
    var time = time.match(/(\d+)(?::(\d\d))?\s*(p?)/);
    TimeOnly.setHours(parseInt(time[1]));


    if (temp.match("PM$")) {
        TimeOnly.setHours(0, 0, 0, 0)
        if (!temp.match("^12")) {
            TimeOnly.setHours(parseInt(time[1]) + 12);
        }
        else {
            TimeOnly.setHours(parseInt(time[1]));
        }
    }
    else if (temp.match("AM$")) {
        TimeOnly.setHours(0, 0, 0, 0)
        if (temp.match("^12")) {
            TimeOnly.setHours(parseInt(time[1]) - 12);
        }
        else {
            TimeOnly.setHours(parseInt(time[1]));
        }
    }
    TimeOnly.setMinutes(parseInt(time[2]));
    //Local time converted to UTC
    var localOffset = TimeOnly.getTimezoneOffset() * 60000;
    var localTime = TimeOnly.getTime();
    if (localOffset == 0) {
        TimeOnly = localTime;
    }
    else {
        TimeOnly = localTime - localOffset;
    }
    return new Date(TimeOnly);
}
function convertLocalDateToUTCDateOnloadTimeOnly(date, toUTC) {
    date = new Date(date);
    //Local time converted to UTC
    var localOffset = date.getTimezoneOffset() * 60000;
    //var localOffset =-new Date().getTimezoneOffset() / 60000;
    var localTime = date.getTime();
    if (localOffset == 0) {
        date = localTime;
    }
    else {
        date = localTime - localOffset;
    }
    return date;
}
function convertLocalDateToUTCDateOnload(date, toUTC) {
    date = new Date(date);
    //Local time converted to UTC
    var localOffset = date.getTimezoneOffset() * 60000;
    //var localOffset = new Date().getTimezoneOffset() / 60000;
    var localTime = date.getTime();
    if (localOffset == 0) {
        date = localTime;
    }
    else {
        date = localTime - localOffset;
    }
    date = new Date(date);
    return date;

}
function SaveServerTime(thisform, isbr) {
    
}

function SaveServerTimeFsearch(docs, isbr) {
    $(docs).find("input").each(function () {
        if ($(this).attr("id") != undefined && $(this).attr("format") != undefined) {
            var inputbox = $(this).attr("id");
            if (inputbox != undefined && $("#" + inputbox).val() != "") {
                if ($("#" + inputbox).attr("readonly") == undefined && $("#" + inputbox).attr("format") != undefined)
                    convertLocalDateToUTCServerTime($("#" + inputbox), false);
            }
        }
    });

}

function LoadDateTimeByFormat(format, val, clt) {
    var localDateString = "";
    var amPm = "";
    if (val != "") {
        if (format != undefined) {
            if (format.trim() == "MM/DD/YYYY hh:mm".trim()) {
                var date = new Date(val);
                var convertedTime = convertLocalDateToUTCDateOnload(date, false);
                localDateString = moment(convertedTime).format(format + " A");
                clt.val(localDateString);
            }
            else if (format.trim() == "MM/DD/YYYY HH:mm".trim()) {
                var date = new Date(val);
                var convertedTime = convertLocalDateToUTCDateOnload(date, false);
                localDateString = moment(convertedTime).format(format);
                clt.val(localDateString);
            }
            else if (format.trim() == "MM/DD/YYYY HH:mm:ss".trim()) {
                var date = new Date(val);
                var convertedTime = convertLocalDateToUTCDateOnload(date, false);
                localDateString = moment(convertedTime).format(format);
                clt.val(localDateString);
            }
            else if (format.trim() == "MM/DD/YYYY hh:mm:ss tt".trim()) {
                var date = new Date(val);
                var convertedTime = convertLocalDateToUTCDateOnload(date, false);
                localDateString = moment(convertedTime).format(format + " A");
                clt.val(localDateString);
            }
            else if (format.trim() == "MM/DD/YYYY hh:mm:ss A".trim()) {
                var date = new Date(val);
                var convertedTime = convertLocalDateToUTCDateOnload(date, false);
                localDateString = moment(convertedTime).format(format);
                clt.val(localDateString);
            }
            else if (format.trim() == "HH:mm".trim() || format.trim() == "hh:mm".trim()) {
                var time = val;
                var convertedTime = ConvertTimeOnlyToDateTime(time, false);
                if (format.trim() == "hh:mm".trim())
                    localDateString = moment(convertedTime).format(format + " A");
                else
                    localDateString = moment(convertedTime).format(format);
                clt.val(localDateString);
            }
			else if (format.trim() == "MM/DD/YYYY".trim()) {
				var date = new Date(val);
                //var convertedTime = convertLocalDateToUTCDateOnload(date, false);
                localDateString = moment(date).format(format);
                clt.val(localDateString);
			}
            else
            {
                var date = new Date(val);
                var convertedTime = convertLocalDateToUTCDateOnload(date, false);
                localDateString = moment(date).format(format);
                clt.val(localDateString);
            }

        }
    }
}
function SaveServerTimeQuickAdd(thisform) {

}
function SaveServerTimeQuickEdit(thisform) {
   
}
function convertLocalDateToUTCServerTime(dateval, toUTC) {
    var offset = new Date().getTimezoneOffset();
    var format = dateval.attr("format")
    var localDateString = "";
    if (format.trim() == "MM/DD/YYYY HH:mm".trim()) {
        var date = new Date(dateval.val());
        localDateString = toUTCDate(date)
        dateval.val(localDateString);
    }
    else if (format == "MM/DD/YYYY hh:mm") {
        var date = new Date(dateval.val());
        localDateString = toUTCDate(date)
        dateval.val(localDateString);
    }
    else if (format == "HH:mm" || format == "hh:mm") {       
        var actual = dateval.attr("actualvalue");
        var time = dateval.val();

        if (actual != undefined && actual.length > 0) {
            var shortdate = ConvertTimeOnlyToDateTimeSaveEdit(actual,time, false)
            localDateString = toUTCDate(shortdate)
        }
        else {
            var shortdate = ConvertTimeOnlyToDateTimeSave(time, false)
            localDateString = toUTCDate(shortdate)
        }
        dateval.val(localDateString);
    }
}
function toUTCDate(datetime) {
    var month = datetime.getUTCMonth() + 1;
    var day = datetime.getUTCDate();
    var year = datetime.getUTCFullYear();
    var hours = datetime.getUTCHours(); //returns 0-23
    var minutes = datetime.getUTCMinutes(); //returns 0-59
    var seconds = datetime.getUTCSeconds();
    return (month + "/" + day + "/" + year + " " + hours + ":" + minutes);

}
function SaveDateTimeBR(startdate) {
    var dateval = $("#" + startdate);
	 var format = dateval.attr("format");
    var datetime = new Date(dateval.val());
    var month = datetime.getUTCMonth() + 1;
    var day = datetime.getUTCDate();
    var year = datetime.getUTCFullYear();
    var hours = datetime.getUTCHours(); //returns 0-23
    var minutes = datetime.getUTCMinutes(); //returns 0-59
    var seconds = datetime.getUTCSeconds();
    //dateval.val(month + "/" + day + "/" + year + " " + hours + ":" + minutes);
	 var localDateString = month + "/" + day + "/" + year + " " + hours + ":" + minutes;
    localDateString = moment(localDateString).format(format);
    dateval.val(localDateString);
}

String.prototype.includes = function (search, start) {
    'use strict';
    if (typeof start !== 'number') {
        start = 0;
    }

    if (start + search.length > this.length) {
        return false;
    } else {
        return this.indexOf(search, start) !== -1;
    }
};
function SetCalendarStartDate(value1, value2) {
    $('#' + value1).attr("format", "MM/DD/YYYY hh:mm");
    var date = new Date($('#' + value1).val());
    var mm = date.getMonth() + 1;
    var dd = date.getDate();
    var yy = date.getFullYear();
    var time = $('#' + value2).val();
    var finaldate = mm + "/" + dd + "/" + yy + " " + time;
    //$('#' + value1).val(finaldate);
    LoadDateTimeByFormat("MM/DD/YYYY", finaldate, $('#' + value1))
}
function SetCalendarStartDate(value1, value2, isedit) {
    $('#' + value1).attr("format", "MM/DD/YYYY");
    var date = new Date($('#' + value1).val());
    var mm = date.getMonth() + 1;
    var dd = date.getDate();
    var yy = date.getFullYear();
    var finaldate = "";
    if (isedit == 1) {
        var time = new Date(ConvertTimeOnlyToDateTime($('#' + value2).val()));
        var hh = time.getHours();
        var mmm = time.getMinutes();
        finaldate = mm + "/" + dd + "/" + yy + " " + hh + ":" + mmm;
    }
    else {
        var time = $('#' + value2).val();
        finaldate = mm + "/" + dd + "/" + yy + " " + time;
    }
    LoadDateTimeByFormat("MM/DD/YYYY", finaldate, $('#' + value1))
    //$('#' + value1).val(finaldate);
}