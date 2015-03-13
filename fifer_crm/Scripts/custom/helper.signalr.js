$(document).ready(function () {
    // Initialize the connection to the server
    var adminHub = $.connection.adminHub;

    adminHub.client.ticketCreated = function (message) {
        notyfy({
            text: message,
            dismissQueue: true,
            layout: 'top'
        });
    };

    adminHub.client.taskCreated = function (message) {
        notyfy({
            text: message,
            dismissQueue: true,
            layout: 'top'
        });
    };

    adminHub.client.calltaskCreated = function (message) {
        notyfy({
            text: message,
            dismissQueue: true,
            layout: 'top'
        });
    };

    adminHub.client.ticketTableRefresh = function (userId) {
        if ($("#tabTicket").length > 0)
        {
            $.get("/Support/GetTicketTable?userId=" + userId + "&isOwner=" + ($("#isOwner").length > 0),
                function (result) {
                    $("#tabTicket .row").html(result);
                    initTable();
            })
        }
    };

    adminHub.client.taskTableRefresh = function (userId) {
        if ($("#taskTable").length > 0) {
            $.get("/CommonTask/GetTaskTable?userId=" + userId ,
                function (result) {
                    $("#taskTable").html(result);
                    initTable();
                })
        }
    };

    adminHub.client.notifyCreated = function (userId) {
        getLastNotify();
    };

    $.connection.hub.start().done(function () {
        if ($("#userId").length > 0) {
            adminHub.server.connect($("#userId").val());
        }
    });

    $.connection.hub.disconnected(function () {
        setTimeout(function () {
            $.connection.hub.start();
        }, 5000); // Restart connection after 5 seconds.
    });

   
});
