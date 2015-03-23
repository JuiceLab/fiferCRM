$(function () {
    loadFilterData();
    initFilterItems();
});



function resetFilterTasks() {
    $.post("/Task/MainTask/SearchResult",
      null,
      function (result) {
          $("#task-search-div").html(result);
          if ($('#isCalendar').val()) {
              $('#option1').trigger("click");
          }
          initFilterItems();
          initPhoneMask();
          initScroll();
          initWidgetCollapsable();
          initDatePicker();
          saveFilterData();
      });
}

function resetFilter()
{
    $.post("/Workspace/Head/GetCustomers?IsLegal=" + $("#IsLegal").val(),
        null,
        function (result) {
            $("#customerList").html(result);
            initFilterItems();
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
    if ($("#task-search").length) {
        $("#task-search").html(localStorage.getItem("task-search"));
        $("#s2id_Services").remove();
        $("#s2id_Assigned").remove();
        $("#s2id_Statuses").remove();
        $("#s2id_Cities").remove();
        $("#s2id_Name").remove();

    

        filterTasks($('#isCalendar').val());
      
    }
}

function saveFilterData() {
    if ($("#customer-search").length) {
        localStorage.setItem("customer-search", $("#customer-search").html());
    }

    if ($("#task-search").length) {
        localStorage.setItem("task-search", $("#task-search").html());
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
    showProgress();
    $.post("/Task/MainTask/SearchResult",
        $("#task-search").serialize(),
        function (result) {
            hideProgress();
            $("#task-search-div").html(result);
            initFilterItems();
            initPhoneMask();
            initScroll();
            initWidgetCollapsable();
            initDatePicker();
            setCalendarTasks();
            saveFilterData();
            if (isCalendar) {
                $("#isCalendar").val(isCalendar);
                $('#option1').trigger("click");
            }
            else {
                $('#option2').trigger("click");
            }
        });
}