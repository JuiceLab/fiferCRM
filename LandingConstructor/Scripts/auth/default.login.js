
function doLogin()
{
    $.post("/account/signin?siteId=" + $("#siteId").val(),
      $("#auth-from").serialize(),
      function () {
          location.reload();
      });
}

function logout() {
    $.get("/account/Logout",
        function () {
            localtion.reload();
        });
}

