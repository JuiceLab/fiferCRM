function initAutocomplete()
{
    $('#Name').keyup(function () { translit(this, '#SiteUrl'); });
    $("#SiteUrl, #Name").on("change", function () {
        if ($("#SiteUrl").val().length > 5) {
            $.get("/WebSite/SiteManage/IsExistUrl?url=" + $("#SiteUrl"),
            function (result) {
                if (result.isExist) {
                    $("#sbmt-btn").prop("disabled", "disabled");
                }
                else {
                    $("#sbmt-btn").removeProp("disabled");
                }
            });
        }
        else {
            $("#sbmt-btn").prop("disabled", "disabled");
        }
    })
}