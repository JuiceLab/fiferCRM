//var apihost = "http://localhost:2271/api";
 var apihost = "http://wf.bizinvit.ru/api";
$(document).ajaxError(function myErrorHandler(event, xhr, ajaxOptions, thrownError) {
    hideProgress();
});

$(function () {
    
    jQuery.extend(jQuery.validator.methods, {
        date: function (value, element) {
            return value.match(/^\d\d?\.\d\d?\.\d\d\d\d$/);
        }
    });

    initDefaultAppendix();

    $('#image-cropper').cropit();

    if ($("#lastNotify").length > 0)
    {
        getLastNotify();
    }

    if ($("#todayTasks").length)
    {
        getTodayTasks();
    }

    initScroll();

    initWidgetCollapsable();


    setTimeout(function () {
        if (typeof $.fn.bdatepicker == 'undefined')
            $.fn.bdatepicker = $.fn.datepicker;

        $.datepicker.setDefaults($.datepicker.regional["ru"]);

        $.fn.datepicker.defaults.language = 'ru';
        
        initDatePicker();
       
        if ($('#datepicker-events').length) {
            var array = $("#event-dates").val().split(',', 100);
            $('#datepicker-events').bdatepicker({
                inline: false, showOtherMonths: true,
                onSelect: function (date) {
                    $.get("/Workspace/Ordinary/EventsTimeline?date=" + date + "&shift=0", function (result) {
                        $("#cur-timeline").replaceWith(result);
                    });
                },
                beforeShowDay: function (date) {
                    var highlighted = $.inArray(date.toLocaleDateString("ru-RU"), array);
                    if (highlighted != -1) {
                        return [true, 'highlighted', 'Встречи'];
                    }
                    else {
                        return [true, '', ''];
                    }
                }
            });
        };
    }, 1000);

    $(document.body).on('submit', "form", function () {
        $(".modal .close").trigger("click");
    });

    $(document.body).on("click", '.select-image-btn', function () {
        if ($('#crop-uploader').parents(".tab-pane").find(".edit-on").length)
            $('#crop-uploader').parents(".tab-pane").find(".edit-on").trigger("click");
        $('#crop-uploader').trigger("click");
    });

    $(document.body).on('click', "#crop-save", function () {
        var data = $('#image-cropper').cropit('export', {
            type: 'image/jpeg',
            quality: .9,
            originalSize: true
        });
        $.post("/image/SaveCroped?companyId=" + $("#company_id").val() + "&type=" + $("#photoType").val(),
			{ "strStream": data },
			function (result) {
			    var json = $.parseJSON(result);
			    $("#employee_img").attr('src', json.url);
			    $(".photo_hidden").val(json.url);
			});
    });
    $(document.body).on('keydown', "input[id^='tags']", function (e) {
        if ($(this).parents(".tagsinput").siblings("input[name='Phone']").length || $(this).parents(".tagsinput").siblings("input[name='Phones']").length) {
            // Allow: backspace, delete, tab, escape, enter and .
            if ($.inArray(e.keyCode, [46, 8, 9, 27, 13, 110, 190]) !== -1 ||
                // Allow: Ctrl+A
                (e.keyCode == 65 && e.ctrlKey === true) ||
                // Allow: home, end, left, right, down, up
                (e.keyCode >= 35 && e.keyCode <= 40)) {
                // let it happen, don't do anything
                return;
            }
            // Ensure that it is a number and stop the keypress
            if ((e.shiftKey || (e.keyCode < 48 || e.keyCode > 57)) && (e.keyCode < 96 || e.keyCode > 105)) {
                e.preventDefault();
            }
        }
    });
    $(document.body).on('keydown', "input[id$='tag']", function (e) {
        if ($(this).parents(".tagsinput").siblings("input[name='Phone']").length || $(this).parents(".tagsinput").siblings("input[name='Phones']").length) {
            // Allow: backspace, delete, tab, escape, enter and .
            if ($.inArray(e.keyCode, [46, 8, 9, 27, 13, 110, 190]) !== -1 ||
                // Allow: Ctrl+A
                (e.keyCode == 65 && e.ctrlKey === true) ||
                // Allow: home, end, left, right, down, up
                (e.keyCode >= 35 && e.keyCode <= 40)) {
                // let it happen, don't do anything
                return;
            }
            // Ensure that it is a number and stop the keypress
            if ((e.shiftKey || (e.keyCode < 48 || e.keyCode > 57)) && (e.keyCode < 96 || e.keyCode > 105)) {
                e.preventDefault();
            }
        }
    });

    $(document.body).on('click', "#crop-multisave", function (e) {
        var data = $('#image-cropper').cropit('export', {
            type: 'image/jpeg',
            quality: .9,
            originalSize: true
        });
        $.post("/image/SaveCroped?companyId=" + $("#company_id").val() + "&type=" + $("#photoType").val(),
			{ "strStream": data },
			function (result) {
			    $("#img-items").append(result);
			});
    });
})

function getTodayTasks() {
    $.get("/Notify/GetTodayTasks", function (result) {
        $("#todayTasks").html(result);
        $('#todayTasks li.notif').on({
            "shown.bs.dropdown": function () { this.closable = false; },
            "click": function () { this.closable = true; },
            "hide.bs.dropdown": function () { return this.closable; }
        });
       
    });
}


function getLastNotify()
{
    $.get("/Notify/GetLastNotify", function (result) {
        $("#lastNotify").html(result);
        $('#lastNotify li.notif').on({
            "shown.bs.dropdown": function() { this.closable = false; },
            "click":             function() { this.closable = true; },
            "hide.bs.dropdown":  function() { return this.closable; }
        });
        
        $("#notify-hover").on("hover", function () {
            if (!$("#notify-hover").hasClass("updated")) {
                $("#notify-hover").addClass("updated");
                $.get("/Notify/SetLastNotifyViewed",
                     function (result) {
                    });
            }
        });
    });
}

function initWidgetCollapsable()
{
    $('.widget[data-toggle="collapse-widget"] .widget-body')
		.on('show.bs.collapse', function () {
		    $(this).parents('.widget:first').attr('data-collapse-closed', "false");
		})
		.on('shown.bs.collapse', function () {
		    setTimeout(function () { $(window).resize(); }, 500);
		})
		.on('hidden.bs.collapse', function () {
		    $(this).parents('.widget:first').attr('data-collapse-closed', "true");
		});

    $('.widget[data-toggle="collapse-widget"]').each(function () {
        // append toggle button
        if (!$(this).find('.widget-head > .collapse-toggle').length)
            $('<span class="collapse-toggle"></span>').appendTo($(this).find('.widget-head'));

        // make the widget body collapsible
        $(this).find('.widget-body').not('.collapse').addClass('collapse');

        // verify if the widget should be opened
        if ($(this).attr('data-collapse-closed') !== "true")
            $(this).find('.widget-body').addClass('in');

        // bind the toggle button
        $(this).find('.collapse-toggle').on('click', function () {
            $(this).parents('.widget:first').find('.widget-body').collapse('toggle');
        });
    });
}

function initPaymentCallback()
{
    initDatePicker();
}

function initOnlyCRM()
{
    $("#onlyCRM").on("click", function () {
        $.get("/Admin/God/CRMTable?onlyCRM=" + $("#onlyCRM").val(),
            function (result) {
                $("#tabCompany .row").html(result);
                initTable();
                initOnlyCRM();
            });
    });
}

function initMultiSelect() {
    $('#Groups, #WorkGroups').multiSelect({
        selectableHeader: "<div class='custom-header'>Доступные рабочие группы</div>",
        selectionHeader: "<div class='custom-header'>Выбранные рабочие группы</div>"
    });
}

function initDropDown()
{
    $("#CompanyId").on("change", function () {
        addPaymentTableItem();
    });
    initDateMask();
}

function initMultiselectAndPhone()
{
    initMultiSelect();
    initPhoneMask();
}

function intiCropAndPhone()
{
    initCropper();
    initPhoneMask();
    initDatePicker();
}

function initDatePicker() {
    if ($.fn.datepicker.dates != undefined)
    $.fn.datepicker.dates['ru'] = {
        days: ["Воскресенье", "Понедельник", "Вторник", "Среда", "Четверг", "Пятница", "Суббота", "Воскресенье"],
        daysShort: ["Вск", "Пнд", "Втр", "Срд", "Чтв", "Птн", "Суб", "Вск"],
        daysMin: ["Вс", "Пн", "Вт", "Ср", "Чт", "Пт", "Сб", "Вс"],
        months: ["Январь", "Февраль", "Март", "Апрель", "Май", "Июнь", "Июль", "Август", "Сентябрь", "Октябрь", "Ноябрь", "Декабрь"],
        monthsShort: ["Янв", "Фев", "Мар", "Апр", "Май", "Июн", "Июл", "Авг", "Сен", "Окт", "Ноя", "Дек"],
        today: "Сегодня",
        clear: "Очистить",
        format: "dd.mm.yyyy",
        weekStart: 1
    };

    if ($('#Date').length) $("#Date").bdatepicker({
        format: 'dd.mm.yyyy',
        startDate: $("#now-date").val()
    });

    if ($('#BirthDate').length) $("#BirthDate").bdatepicker({
        format: 'dd.mm.yyyy',
        endDate: '-1d'
    });

    if ($('#DateIssue').length) $("#DateIssue").bdatepicker({
        format: 'dd.mm.yyyy',
        endDate: '-1d'
    });

    if ($("input[name^='DateRangeInvariant'], #PayBeforeInvariant").length)
        $("input[name^='DateRangeInvariant'], #PayBeforeInvariant").bdatepicker({
        format: 'dd.mm.yyyy'
    });
    if ($("#DateRangeInvariant_0_").length) {
        $("#DateRangeInvariant_0_,#DateRangeInvariant_1_").inputmask("d.m.y", { "placeholder": "дд.мм.гггг" })
          .bdatepicker({
              format: 'dd.mm.yyyy',
              startDate: $("#now-date").val()
          });
    }

    $(window).on('load', function () {
        if ($('.menu-right-hidden').length) {
            $('.btn-navbar-right').parent().removeClass('visible-xs');

            if (typeof sidebarKisInit !== 'undefined')
                return;

            $('#menu_kis').width(70);

            setTimeout(function () {

                $('#menu_kis').animate({
                    right: '-70px'
                }, function () {
                    $(this).removeAttr('style');
                });

            }, 1000);
        }

        window.sidebarKisInit = true;
    });
}

function intiPassportForm()
{
    initCropper();
    initDatePicker();
    $("#customer_passport #DistrictId").on("change", function () {
        $.get("/GeoLocation/District/GetCities?distrId=" + $("#customer_passport #DistrictId").val(),
            function (result) {
                $("#customer_passport #city-Drop").html(result);
            });
    });
}

function initTaskPeriods() {
    initScroll();
    initDateMask();
}

function initDefaultAppendix()
{
    $.each($(".need-append-input"), function (index, item) {
        $("<input class='appendix' type='text' /><span onclick=\"appendText('#" + $(item).attr('id') + "');\" class='btn btn-default'>+ Добавить</span>").insertAfter(item);
    });
}

function appendText(selector)
{
    if($(selector).siblings(".appendix").val().length >0){
        if ($(selector).val().length == 0)
            $(selector).val($(selector).siblings(".appendix").val());
        else
            $(selector).val($(selector).val() + "," + $(selector).siblings(".appendix").val());
    }
    $(selector).siblings(".appendix").val("");
}

function initScroll() {
    if ($("#scrolled-div").length > 0) {
        $("#scrolled-div .scroll-body").niceScroll("#scrolled-div .scroll-body ul", { cursorcolor: "#00F" });
    }


    if ($("#tabEmployeeActivity").length > 0) {
        $("#tabEmployeeActivity .widget-body").niceScroll("#tabEmployeeActivity .widget-body .innerAll", { cursorcolor: "#00F" });
    }

    if ($("#tabExpense").length > 0) {
        $("#tabExpense .widget-body, #tabService .widget-body").niceScroll("#tabEmployeeActivity .widget-body .innerAll", { cursorcolor: "#00F" });
    }
}

