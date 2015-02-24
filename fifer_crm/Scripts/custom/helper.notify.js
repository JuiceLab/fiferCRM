function setViewNotifyItem(notifyId, isLocal)
{
    $.get("/Notify/SetNotifyViewed?notifyItemId=" + notifyId + "&isLocal=" + isLocal,
    function () {
        getLasNotify();
    });
}

function notifyLegal(id)
{
    $.get('/CRM/LegalEntity/NotifyAssigned?companyId=' + id + "&msg=" + $('#msg').val(),
    function () {
        alert('уведомление отправлено');
        $("#modal-help .close").trigger("click");
    });
}


function notifyNotLegal(id) {
    $.get('/CRM/LegalEntity/NotifyNotLegalAssigned?customerId=' + id + "&msg=" + $('#msg').val(),
    function () {
        alert('уведомление отправлено');
        $("#modal-help .close").trigger("click");
    });
}