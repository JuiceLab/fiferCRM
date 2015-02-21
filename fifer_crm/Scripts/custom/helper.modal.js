
function loadModalContentWithCallback(modalId, action, controller, paramStr, callback) {
    $("#" + modalId + " .modal-body").html("<img src=\"/assets/images/ajax-loaders/ajax-loader-6.gif\" title=\"/assets/images/ajax-loaders/ajax-loader-6.gif\" class=\"col-xs-offset-4\" />");
    $("#" + modalId).modal();
    $.get("/" + controller + "/" + action + paramStr, function (result) {
        $("#" + modalId + " .modal-content").html(result);

        var form = $("#" + modalId).find("form")
           .removeData("validator") /* added by the raw jquery.validate plugin */
           .removeData("unobtrusiveValidation");  /* added by the jquery unobtrusive plugin */
        if (form.length) {
            $.validator.unobtrusive.parse(form);
            var validator = $.data(form[0], 'validator');
            validator.settings.ignore = '';
        }
        initDefaultAppendix();
        callback();
       
    });
}

function loadModalContent(modalId, action, controller, paramStr) {

    $("#" + modalId + " .modal-body").html("<img src=\"/assets/images/ajax-loaders/ajax-loader-6.gif\" title=\"/assets/images/ajax-loaders/ajax-loader-6.gif\" class=\"col-xs-offset-4\" />");
    $("#" + modalId).modal();
	$.get("/" + controller + "/" + action + paramStr, function (result) {
	    $("#" + modalId + ' .modal-content').html(result);

	    var form = $("#" + modalId).find("form")
            .removeData("validator") /* added by the raw jquery.validate plugin */
            .removeData("unobtrusiveValidation");  /* added by the jquery unobtrusive plugin */
	    if (form.length) {
	        $.validator.unobtrusive.parse(form);
	        var validator = $.data(form[0], 'validator');
	        validator.settings.ignore = '';
	    }
	    initDefaultAppendix();
	});
}


