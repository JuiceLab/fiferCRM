var authHost = "http://emprana-dev2.azurewebsites.net";
//var authHost = "http://localhost:64505";

$(document).ready(function () {
    if ($("#auth-form").length > 0) {
        $.getJSON(authHost + "/SSOAuth/Index?callback=?",
        function (data) {
            $('div#auth-form').html(data.body);
            if ($("#UserId").length > 0) {
                GetCookie(data.user.UserId);
            }
        });
    }

    if ($("#change-pass-div").length > 0) {
        $.getJSON(authHost + "/SSOAuth/ChangePassIndex?callback=?",
        function (data) {
            $('div#change-pass-div').html(data.body);
        });
    }

    $("#change-pass-btn").live("click", function () {
        $.post("/Access/ChangeUserPass",
            $("#change-pass-form").serialize(),
       function (data) {
           $('div#auth-form').html(data.body);
           $('#change-pass-div').html(data.body);
           if ($("#UserId").length > 0) {
               history.back();
           }
       });
    });

    $("#login-enter").live("click", function () {
        $.post("/Access/LogOn",
             $("#standart_form").serialize(),
        function (data) {
            $('div#auth-form').html(data.body);
            if ($("#UserId").length > 0 || $("#Login").length > 0) {
                GetCookie(data.user.UserId);
            }
        });
    });

    $('body').keyup(function (e) {
        if (e.which == 13) {
            $("#login-enter").trigger("click");
        }
    });
})

function ssoLogout(userId) {
    $.getJSON(authHost + "/SSOAuth/PAuthExpired?callback=?", function (data) { });
    $.get("/Access/LogOut?userId=" + userId + "&callback=?",
        function (data) {
            location.href = "/Access/Login"
        });
}


function GetCookie(userId) {
    $.getJSON(authHost + "/SSOAuth/PLoginSuccessfull?userId=" + userId + "&callback=?",
       function (data) {
           if ($("#Login").length == 0) {
               var url = window.location.pathname;
               var append = window.location.search.indexOf("?ReturnUrl") >= 0 ? "&" : "?";
               location.href = url + window.location.search + append + "userId=" + userId;
           }
       });
}