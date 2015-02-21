function setViewNotifyItem(notifyId, isLocal)
{
    $.get("/Notify/SetNotifyViewed?notifyItemId=" + notifyId + "&isLocal=" + isLocal,
    function () {
        getLasNotify();
    });
}