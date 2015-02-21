$(document).ready(function () {
	// Initialize the connection to the server
	var adminHub = $.connection.adminHub;

	//adminHub.client.sendMessageNotify = function (name, message, phone, type) {
	//	alert(message);
	//};

	adminHub.client.sendLogMsg = function (message, progress) {
		if ($("#SyncLog").length > 0) {
			$("#SyncLog").val($("#SyncLog").val() + '\r\n' + message);
			$("#SyncProgress").text(progress + "%").attr("aria-valuenow", progress).css("width", progress + "%");
			$('#SyncLog').animate({
			    scrollTop: $("#SyncLog").get(0).scrollHeight
			}, 0);
		}
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
