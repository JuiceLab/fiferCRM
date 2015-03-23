$(function () {
    if ($("input[name='Phones']").length) {
        $("input[name='Phones']").each(function (i, item) {
            $(item).tagsInput();
        });
        onlyDigitsInput("Phones_tag");
    }

    if ($("#SiteUrl").length) {
        $("#SiteUrl").tagsInput();
    } 
    if ($("input[name='Mails']").length) {
        $("input[name='Mails']").each(function (i, item) {
            $(item).tagsInput();
        });
    }
    if ($("#Mail").length && $("#tabAll").length ==0) {
        $("#Mail").tagsInput();
    }
    if ($("#onlyCRM").length > 0) {
        initOnlyCRM();
    }
    if ($('#Services').length > 0)
        $('#Services').select2();

    initPhoneMask();

})

function intiIpTags() {
    $("#LockIps").tagsInput();
}

function initPhoneMask() {
    if ($("#Phone").length) {
        $("#Phone").inputmask("mask", { "mask": "+7(999) 999-99-99" });
    }

    if ($("#Fax").length) {
        $("#Fax").inputmask("mask", { "mask": "+7(999) 999-99-99" });
    }
}

var spinnerVisible = false;
function showProgress() {
    if (!spinnerVisible) {
        $("div#spinner").fadeIn("fast");
        spinnerVisible = true;
    }
};
function hideProgress() {
    if (spinnerVisible) {
        var spinner = $("div#spinner");
        spinner.stop();
        spinner.fadeOut("fast");
        spinnerVisible = false;
    }
};

function initDateMask() {
    initMultiSelectActivity();
    if ($("#DateStartedStr").length) {
        $("#DateStartedStr").inputmask("d.m.y", { "placeholder": "дд.мм.гггг" })
            .bdatepicker({
                format: 'dd.mm.yyyy',
                startDate: $("#now-date").val()
            });

        $("#TimeStartedStr").timepicker({
            showMeridian: false
        });

        if ($("#NotifyBeforeStr").length) {
            $("#NotifyBeforeStr").timepicker({
                showMeridian: false
            });
        }
    }

    if ($("#Periods_0__DateBeforeStr").length) {
        $("#Periods_0__DateBeforeStr, #Periods_0__DateStartedStr").inputmask("d.m.y", { "placeholder": "дд.мм.гггг" })
            .bdatepicker({
                format: 'dd.mm.yyyy',
                startDate: $("#now-date").val()
            });
        $("#TimeStartedStr, #Periods_0__TimeStartedStr").timepicker({
            showMeridian: false
        });
    }

    if ($("#DealDate").length > 0) {
        $("#DealDate").inputmask("d.m.y", { "placeholder": "дд.мм.гггг" })
            .bdatepicker({
                format: 'dd.mm.yyyy',
            });
    }
    initDatePicker();
}


function addTagsInputItem(curItem) {
    var focused = $(curItem).parents(".form-group").find("input");
    $(focused).trigger("focusout");
    $(focused).trigger("focus");
}

function addMails() {
    $("#Mails").trigger("focusout");
    $("#Mails").trigger("focus");
}

function addMail() {
    $("#Mail").trigger("focusout");
    $("#Mail").trigger("focus");
}

function addPhone() {
    $("#Phones").trigger("focusout");
    $("#Phones").trigger("focus");
}

function initMultiSelectActivity() {
    if ($('#Activities, #Activities').length > 0) {
        $('#Activities, #Activities').select2();
    }
}

function onlyDigitsInput(itemId) {
    $(document.body).on('keydown', "#" + itemId, function (e) {
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
    });
}