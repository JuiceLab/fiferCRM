
function createSupportTicket()
{
    $("#modal-help .close").trigger("click");
    $.post(apihost + "/support/post?idCommand=" + $("#support-cmd").val(),
    $("#supportTicket").serialize(),
        function (result) {
    });
}
function updateSupportTicket(cmd) {
    $("#modal-help .close").trigger("click");
    $.post(apihost + "/support/post?idCommand=" + cmd,
    $("#supportTicketProcess").serialize(),
        function (result) {
        });
}

function getTicketActions()
{
    $.ajax({
        url: apihost + "/support/SetViewed?ticketId=" + $("#modal-help #TicketId").val() + "&userId=" + $("#userId").val(),
        complete:function () {
            $.get("/Support/Actions4Ticket?ticketId=" + $("#modal-help #TicketId").val(),
                function (result) {
                    $("#ticketProcessActions").html(result);
                });
        }
    });
}