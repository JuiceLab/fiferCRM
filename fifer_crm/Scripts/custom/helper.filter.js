$(function () {
    loadFilterData();
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




function resetFilter()
{
    $.post("/Workspace/Head/GetCustomers?IsLegal=" + $("#IsLegal").val(),
        null,
        function (result) {
            $("#customerList").html(result);
            initFilterItems();
            setCalendarTasks();
            initPhoneMask();
            initScroll();
            initWidgetCollapsable();
            initDatePicker();
            saveFilterData();
        });
    $(".modal .close").trigger("click");

}

function loadFilterData() {
    if ($("#customer-search").length) {
        $("#customer-search").html(localStorage.getItem("customer-search"));
        $("#s2id_Services").remove();
        $("#s2id_AssignedBy").remove();
        $("#s2id_StatusId").remove();
        $("#s2id_Cities").remove();
        $("#s2id_Name").remove();
        filterCustomers($("#IsLegal").val());
    }
}

function saveFilterData() {
    if ($("#customer-search").length) {
        localStorage.setItem("customer-search", $("#customer-search").html());
    }
}

function initFilterItems()
{
    if ($("#customer-search").length){
        $("#Name, #Services, #Cities, #AssignedBy,#StatusId").select2();
    }
    if ($("#task-search").length) {
        $("#Statuses, #Assigned, #Services, #Cities").select2();
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
    if (isLegal !="true") {
        $("#option1").parents("label").addClass("active");
        $("#option2").parents("label").removeClass("active");
    } else {
        $("#option2").parents("label").addClass("active");
        $("#option1").parents("label").removeClass("active");
    }
    showProgress();
    $.post("/Workspace/Head/GetCustomers",
        $("#customer-search").serialize(),
        function (result) {
            $("#customerList").html(result);
            initFilterItems();
            initPhoneMask();
            initScroll();
            initWidgetCollapsable();
            initDatePicker();
            saveFilterData();
            hideProgress();
        });
    $(".modal .close").trigger("click");
}

function filterTasks(isCalendar) {
    $.post("/Task/MainTask/SearchResult",
        $("#task-search").serialize(),
        function (result) {
            $("#task-search-div").html(result);
            initFilterItems();
            if ($("#IsLegal").val() != "true")
                initPhoneMask();
            if (isCalendar)
            {
                $('#option1').trigger("click");
            }
            initDatePicker();

        });
}