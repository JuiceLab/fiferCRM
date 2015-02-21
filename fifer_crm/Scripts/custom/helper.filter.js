$(function () {
    $("form[id^='filter']").on("submit", function () {
        preventDefault();
		$.post("/Filter/", $(this).attr("id").substr(7),
			$(this).serialaize(),
			function (result) {
				$("#filtred-content").html(result);
			}
		)
    });
    initFilterItems();
});

function initFilterItems()
{
    if ($("#customer-search").length){
        $("#Name, #Services, #Cities, #AssignedBy,#StatusId").select2();
    }
    if ($("#task-search").length) {
        $("#StatusId, #Assigned, #Services, #Cities").select2();
    }
}

function filterUser(companyId) {
    $.get("/CRM/Customer/CustomersList?companyId=" + companyId,
        function (result) {
            $("#customerList").html(result);
        });
}

function filterPayment(companyId) {
    $.get("/Finances/PaymentActs/PaymentsList?companyId=" + companyId,
        function (result) {
            $("#paymentsList").html(result);
        });
}

function filterCustomers(isLegal) {
    $("#IsLegal").val(isLegal);
    $.post("/Workspace/Head/GetCustomers",
        $("#customer-search").serialize(),
        function (result) {
            $("#customerList").html(result);
            setCalendarTasks();
            initDatePicker();
            
            initPhoneMask();
            initWidgetCollapsable();
            initFilterItems();
        });
    $(".modal .close").trigger("click");
}

function filterTasks(isCalendar) {
    $.post("/Task/MainTask/SearchResult",
        $("#task-search").serialize(),
        function (result) {
            $("#task-search-div").html(result);
            initDatePicker();
            initFilterItems();
            if ($("#IsLegal").val() != "true")
                initPhoneMask();
            if (isCalendar)
            {
                $('#option1').trigger("click");
            }
        });
}