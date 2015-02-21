function changeBlock(userId, isBlock) {
    $.get("/User/BlockUser?userId=" + userId + "&isBlock=" + isBlock,
        function () {
            location.reload();
        });
}


function kickUser(userId, isRandom) {
    $.get("/User/KickUser?userId=" + userId,
      function () {
          location.reload();
      });
}

function resetPass(userId, isRandom) {
    var pass = !isRandom ? $("#novelty-pass").val() : "";
    $.get("/User/ResetPass?userId=" + userId + "&pass=" + pass,
      function () {
          location.reload();
      });
}