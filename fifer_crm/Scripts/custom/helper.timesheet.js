

var hour;
var minute;
var seconds;
var shiftSeconds;
var start;

var mydomstorage = window.localStorage || (window.globalStorage ? globalStorage[location.hostname] : null)

if (mydomstorage) {
    shiftSeconds = (mydomstorage.shiftSeconds) ? parseInt(mydomstorage.shiftSeconds) : 0
}
else {
    document.write("<b>Your browser doesn't support DOM Storage unfortunately.</b>")
}

$(document).ready(function () {
    if (mydomstorage.shiftSeconds && mydomstorage.shiftSeconds > 0) {
        $("#session-state button").removeClass("active");
        $("#session-state button").last().addClass("active");
        sessionTimer();
    }

    $(this).mousemove(function (e) {
        if( $("#isAbsent").val() == "true")
            $("#isAbsent").val("false");
    });

    $(this).keypress(function (e) {
        if ($("#isAbsent").val() == "true")
            $("#isAbsent").val("false");
    });
});


$(window).unload(function () {
    if (window.location.pathname.indexOf("/Account/SignIn") == -1) {
    }
    else {
        mydomstorage.clear();
    }
});

function initDurationAbsent() {
    $("#userId").oneTime("300s", function () {
        if ($("#isAbsent").length > 0) {
            $("#isAbsent").val("true");
            $("#userId").oneTime("300s", function () {
                if ($("#isAbsent").val() == "true") {
                    setTimeBreak("В связи с отсутствием на рабочем месте автоматически установлен перерыв в работе сотрудника"); 
                }
                else {
                    initDurationAbsent();
                }
            });
        }
    });
}

function setTimeBreak(comment)
{
    $.get('/erp/employee/SetTimeBreak?comment=' + comment + "&type=2&isStart=true",
        function () {
            if (!$('#stop-session').hasClass("active")) {
                $("#sessionStop").fadeOut(500);
                $("#sessionStart").fadeIn(500);
                mydomstorage.stateSession = false;
                $("#sessionTime").stopTime("timer");
                mydomstorage.shiftSeconds = 0;
                shiftSeconds = 0;
                $("#sessionTime").html("<i class=\"fa fa-play\"></i>");
            }
        });
}

function sessionTimer() {
    $("#sessionTime").everyTime(1000, "timer", function (i) {
        seconds = (shiftSeconds + i) % 60;
        minute = (((shiftSeconds + i) % 3600) / 60 | 0);
        hour = (((shiftSeconds + i) % 86400) / 3600 | 0);
        mydomstorage.shiftSeconds = (shiftSeconds + i);
        $("#sessionTime").html((hour < 10 ? "0" + hour : hour) + ":" + (minute < 10 ? "0" + minute : minute) + ":" + (seconds < 10 ? "0" + seconds : seconds));
    });
}

function startSession(value, item) {
    if (!$(item).hasClass("active")) {
        $("#stop-session").removeClass("active");
        sessionTimer();
        mydomstorage.stateSession = true;

        $("#sessionStart").fadeOut(500);
        $("#sessionStop").fadeIn(500);
        $.get('/erp/employee/SetTimeBreak?type=2&isStart=false',
          function () {

          });
    }
    $("#session-user").show();
}
function stopSession(item) {
    if (!$(item).hasClass("active")) {
        $("#sessionStop").fadeOut(500);
        $("#sessionStart").fadeIn(500);
        mydomstorage.stateSession = false;
        $("#session-comment").removeClass("hide");
        $("#session-state").addClass("open");
        $("#sessionTime").stopTime("timer");
        mydomstorage.shiftSeconds = 0;
        shiftSeconds = 0;
        $("#sessionTime").html("<i class=\"fa fa-play\"></i>");
    }
}
function setSessionComment() {
    $.get('/erp/employee/SetTimeBreak?comment=' + $("#session-result").val() + "&type=" + $("#TimeBreakType").val() +"&isStart=true",
            function (info) {
                if ($("#TimeBreakType").val() == '3')
                {
                    location.href = '/account/logout';
                }
            },
            "json"
        );
    $("#sessionTime").removeClass("active");
    $("#session-comment").addClass("hide");
    $("#session-state").removeClass("open");

}