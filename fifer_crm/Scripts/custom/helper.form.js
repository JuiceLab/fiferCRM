$(function () {
    if ($("#editable-forms").length > 0)
    {
        $("form input").attr("disabled", "disabled");
        $("form button").hide();
        $(".edit-on").on("click", function () {
            $(this).parents("form").find("input").removeProp("disabled");
            $(this).parents("form").find("button").show();
            $(this).parents("form").find(".edit-on").hide();
        })
    }
})

function searchCompanies()
{
    $.post($("#companySearch").attr("action"),
       $("#companySearch").serialize(),
       function (result) {
           $("#search-list").html(result);
       });

}

function createTask(form, action, controller)
{
    if (!$(form).valid()) {
        $(form).validate();
    }
    else {
        $.post($(form).attr('action'),
            $(form).serialize(),
            function (result)
            {
                $(form).parents(".modal").find(".close").trigger("click");
                filterCustomers($('#IsLegal').val());
            });
    }
}

function openAndAddNoveltyCompany() {
    $("#modal-add").modal();
    addNoveltyCompany();
}

function editLegalCompany(legalId)
{
    $("#modal-add").modal();
    $.get("/CRM/LegalEntity/Edit?legalEnitityId=" + legalId,
      function (result) {
          $("#modal-add .modal-content").html(result);
          
          $("#legalEditForm #Address").on("keyup", function () {
              if ($("#Address").val().length > 0)
                  codeAddress($("#Address").val(), "google-map-geo-add");
          });
          initFormWizzard(".wizard", false);

          var form = $("#modal-add").find("form")
             .removeData("validator") /* added by the raw jquery.validate plugin */
             .removeData("unobtrusiveValidation");  /* added by the jquery unobtrusive plugin */
          $.validator.unobtrusive.parse(form);
          var validator = $.data(form[0], 'validator');
          validator.settings.ignore = '';
          initDefaultAppendix();
          initLegalEnitityForm();
          initCropper();
      });
}

function addNoveltyCompany()
{
    $.post("/CRM/LegalEntity/NoveltyLegalEdit",
          $("#companySearch").serialize(),
       function (result) {
           $("#modal-add .modal-content").html(result);
           $("#City").on("change", function () {
               codeAddress($("#City").text(), "google-map-geo-add");
           })
           $("#Address").on("keyup", function () {
               if ($("#Address").val().length > 0)
                   codeAddress($("#Address").val(), "google-map-geo-add");
           });
           initFormWizzard(".wizard", false);
         
           var form = $("#modal-add").find("form")
              .removeData("validator") /* added by the raw jquery.validate plugin */
              .removeData("unobtrusiveValidation");  /* added by the jquery unobtrusive plugin */
               $.validator.unobtrusive.parse(form);
               initDefaultAppendix();
               initLegalEnitityForm();
               onlyDigitsInput("Phones_tag");
               onlyDigitsInput("tags1424088929140_tag");
               initCropper();
       });
}

function updateSettings() {
    $.post("/Notify/NotifySettingsEdit",
        $("#settings-update").serialize(),
        function () {
            location.reload();
        });
}

function copyCRMCompany(companyId)
{
    $.get("/CRM/LegalEntity/CopyLocal?companyId=" + companyId, function (result) {
        location.reload();
    });
}

function updateLegalGeo()
{
    $.post($(".legal-form").attr("action"),
        $(".legal-form").serialize(),
        function (result) {
            $("#modal-add .modal-content").html(result);
            var form = $(".legal-form")
              .removeData("validator") /* added by the raw jquery.validate plugin */
              .removeData("unobtrusiveValidation");  /* added by the jquery unobtrusive plugin */
                if (form.length) {
                    $.validator.unobtrusive.parse(form);
                    var validator = $.data(form[0], 'validator');
                    validator.settings.ignore = '';
                }
            initDefaultAppendix();
            initLegalGeo();
        }
    );
}

function initLegalEnitityForm()
{
    initDateMask();
    $("#Phones").tagsInput();
    $("#Sites").tagsInput();
    $("#Mails").tagsInput();
  
    
    if ($('#companySearch #Services').length > 0)
    {
        $('#companySearch #Services').select2();
        $("#companySearch #DistrictId").on("change", function () {
            $.get("/GeoLocation/District/GetCities?distrId=" + $("#companySearch #DistrictId").val(),
                function (result) {
                    $("#companySearch #city-Drop").html(result);
                    if ($("#google-map-geo-add").length > 0) {
                        $("#companySearch #City").on("change", function () {
                            codeAddress($("#companySearch #City option:selected").text(), "google-map-geo-add");
                        })
                    }
                });
        });
    }

    if ($('#legalEditForm #Activities').length > 0)
    {
      
        $("#legalEditForm #DistrictId").on("change", function () {
            $.get("/GeoLocation/District/GetCities?distrId=" + $("#legalEditForm #DistrictId").val(),
                function (result) {
                    $("#legalEditForm #city-Drop").html(result);
                    if ($("#google-map-geo-add").length > 0) {
                        $("#City").on("change", function () {
                            $("#legalEditForm #City").on("change", function () {
                                codeAddress($("#legalEditForm #City").text(), "google-map-geo-add");
                            })
                            codeAddress($("#legalEditForm #City option:selected").text(), "google-map-geo-add");
                        })
                    }
                });
        });
    }
  

    if ($('#legalEditForm #Services').length > 0) {
        $('#legalEditForm #Services').select2();
        $("#legalEditForm #DistrictId").on("change", function () {
            $.get("/GeoLocation/District/GetCities?distrId=" + $("#DistrictId").val(),
                function (result) {
                    $("#legalEditForm #city-Drop").html(result);
                    if ($("#google-map-geo-add").length > 0) {
                        $("#City").on("change", function () {
                            codeAddress($("#City option:selected").text(), "google-map-geo-add");
                        })
                    }
                });
        });
    }
}

function updateAssign()
{
    $.post("/CRM/LegalEntity/Assign",
        $("#assign-legal-form").serialize(),
        function () {
            filterCustomers( $("#IsLegal").val());
            // todo set signalr partial updated
    });
}

function updateComment()
{
    $.post("/CRM/LegalEntity/EditComment",
      $("#comment-legal-form").serialize(),
      function () {
          filterCustomers( $("#IsLegal").val());
          // todo set signalr partial updated
      });
}

function updateStatus() {
    $.post("/CRM/LegalEntity/ChangeStatus",
      $("#status-legal-form").serialize(),
      function () {
          filterCustomers( $("#IsLegal").val());
          // todo set signalr partial updated
      });
}


function updateCustomerAssign() {
    $.post("/CRM/Customer/Assign",
        $("#assign-legal-form").serialize(),
        function () {
            filterCustomers( $("#IsLegal").val());
            // todo set signalr partial updated
        });
}

function updateCustomerComment() {
    $.post("/CRM/Customer/EditComment",
      $("#comment-legal-form").serialize(),
      function () {
          filterCustomers( $("#IsLegal").val());
          // todo set signalr partial updated
      });
}

function updateCustomerStatus() {
    $.post("/CRM/Customer/ChangeStatus",
      $("#status-legal-form").serialize(),
      function () {
          filterCustomers( $("#IsLegal").val());
          // todo set signalr partial updated
      });
}

function initCustomerNonLegalForm()
{
    initCropper();
    initCustomerForm();
    if ($("#nonLegalForm #DistrictId").length > 0) {
        $("#nonLegalForm #DistrictId").on("change", function () {
            $.get("/GeoLocation/District/GetCities?distrId=" + $("#nonLegalForm #DistrictId").val(),
                function (result) {
                    $("#city-Drop").html(result);
                    if ($("#google-map-geo-add").length > 0) {
                        $("#City").on("change", function () {
                            codeAddress($("#City option:selected").text(), "google-map-geo-add");
                        })
                    }
                });
        });
    }
}

function updateCustomerPassport()
{
    $.post("/CRM/Customer/EditPassportCustomer",
       $("#customer_passport").serialize(),
       function () {
           filterCustomers( $("#IsLegal").val());
           // todo set signalr partial updated
       });
}

function initCustomerForm()
{
    if ($("form #Phone").length) {
        $("form #Phone").inputmask("mask", { "mask": "+7(999) 999-99-99" });
    }
    $("#SocialLinks").tagsInput();
   
}

function getUser(customerId)
{
    $.get("/CRM/Customer/CustomerEditForm?customerId=" + customerId + "&companyId=" + $("#cur-legal-guid").val(), function (result) {
        $("#customer-editor").html(result);
        initCustomerForm();
    });

}

function updateUserProfile()
{
    if ($("#legal-customer-id").valid()) {
        $.post($("#legal-customer-id").attr("action"),
            $("#legal-customer-id").serialize(),
             function (result) {
                 loadModalContent('modal-fullscreen', 'CustomersList', 'CRM/Customer', '?companyId=' + $("#cur-legal-guid").val());
             });
    }
}
function editCustomerNonLegal()
{
    $("#nonLegalForm").validate({ ignore: "" });
    if ($("#nonLegalForm").valid()) {
        $.post($("#nonLegalForm").attr("action"),
            $("#nonLegalForm").serialize(),
             function (result) {
                 filterCustomers($('#IsLegal').val());
             });
    }
}

function editMeeting()
{
    if ($("#taskTicket").valid()) {
        $.post($("#taskTicket").attr("action"),
            $("#taskTicket").serialize(),
             function (result) {
                location.reload();
             });
    }
}
function editCall()
{
    if ($("#taskTicket").valid()) {
        $.post($("#taskTicket").attr("action"),
            $("#taskTicket").serialize(),
             function (result) {
                 location.reload();
             });
    }
}


function createPayment() {
    if ($("#paymentForm").valid()) {
        $.post($("#paymentForm").attr("action"),
            $("#paymentForm").serialize(),
             function (result) {
                 location.reload();
             });
    }
}

function showCurDateShift(shift)
{
    $.get("/Workspace/Ordinary/EventsTimeline?date=" + $("#date4timeline").val() + "&shift=" + shift, function (result) {
        $("#cur-timeline").replaceWith(result);
        $("#datepicker-events").text($("#date4timeline").val());
    });
}

function changePaymentStatus(statusId)
{
    $.post("/Finances/Payment/PaymentUpdateStatus?statusId=" + statusId,
        $("#paymentForm").serialize(),
        function () {
            location.reload();
        }
    )
}


