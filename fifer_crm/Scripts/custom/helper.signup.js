$(function () {
    $("#Phone").inputmask("mask", { "mask": "+7(999) 999-99-99" });
    if ($("#DistrictId").length > 0)
    {
        $("#DistrictId").on("change", function () {
            $.get("/GeoLocation/District/GetCities?distrId=" + $("#DistrictId").val(),
                function (result)
                {
                    $("#city-Drop").html(result);
                });
        });
    }
})
function resetPass()
{
    $.get("/Account/ResetPass?login=" + $("#Email").val(),
    function () {
        alert("Письмо c новым паролем отправлено на почту");
    });
}