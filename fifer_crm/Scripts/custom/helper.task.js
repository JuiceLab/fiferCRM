
function initMeetingTask() {
    initDateMask();
    if ($("#availableActions").length) {
        $.ajax({
            url: apihost + "/Meeting/SetViewed?ticketId=" + $("#taskTicket #TicketId").val() + "&userId=" + $("#userId").val(),
            complete: function () {
                $.get("/Task/Meeting/Actions4MeetingTask?taskId=" + $("#taskTicket #TicketId").val(),
                     function (result) {
                         $("#availableActions").html(result);
                     });
            }
        });
    }
}

function initCallTask()
{
    initDateMask();
    if ($("#availableActions").length) {
        $.ajax({
            url: apihost + "/calltask/SetViewed?ticketId=" + $("#taskTicket #TicketId").val() + "&userId=" + $("#userId").val(),
            complete: function () {
                $.get("/Task/Call/Actions4CallTask?taskId=" + $("#taskTicket #TicketId").val(),
                     function (result) {
                         $("#availableActions").html(result);
                     });
            }
        });
    }
}

function setCalendarTasks() {
    $.post("/Task/MainTask/GetTasks",
            $("#task-search").serialize(),
                function (result) {
                    var array = Array();
                    $.each(result.data, function (index, item) {
                        array.push($.parseJSON(item));
                    })

                    $('#calendar').fullCalendar({
                        header: {
                            left: 'prev,next today',
                            center: 'title',
                            right: 'month,agendaWeek,agendaDay'
                        },
                        editable: true,
                        droppable: true,
                        eventLimit: true,
                        events: array,
                        lang: 'ru',
                        eventRender: function (event, element) {
                            $(element).attr("data-toggle", "popover")
                                .attr("data-trigger", "click")
                                .attr("title", event.number + ": " + event.title + ". ")
                                .attr("data-content", event.description)
                                .find(".fc-content").append("<span style='padding:1px; background-color:white; border-radius:1px' class='pull-right'>" + getActions(event.taskid,event.number) + "</span>");
                        },
                        eventDrop: function (event, delta, revertFunc) {
                            transferTask(event.taskid, event.start.format());
                        },
                        drop: function (date, allDay) {
                            // retrieve the dropped element's stored Event Object
                            var originalEventObject = $(this).data('eventObject');

                            // we need to copy it, so that multiple events don't have a reference to the same object
                            var copiedEventObject = $.extend({}, originalEventObject);

                            // assign it the date that was reported
                            copiedEventObject.start = date;
                            copiedEventObject.allDay = allDay;

                            // render the event on the calendar
                            // the last `true` argument determines if the event "sticks" (http://arshaw.com/fullcalendar/docs/event_rendering/renderEvent/)
                            $('#calendar').fullCalendar('renderEvent', copiedEventObject, true);

                            // is the "remove after drop" checkbox checked?
                            if ($('#drop-remove').is(':checked')) {
                                // if so, remove the element from the "Draggable Events" list
                                $(this).remove();
                            }
                        }
                    });
                    $('[data-toggle="popover"]').popover();
                })
}

function getActions(ticketId, number)
{
    return "<span class='btn btn-default padding-none' onclick=\"loadModalContentWithCallback('modal-help','TaskProcess','Task/GroupTask', '?taskId=" + ticketId + "', getTaskActions); return false;\">&nbsp;<i class='fa fa-edit'></i>&nbsp;</span>&nbsp;" +
     "<span class='btn btn-success padding-none' onclick=\"loadModalContentWithCallback('modal-help','TaskAssign','Task/GroupTask', '?taskId=" + ticketId + "', getTaskActions); return false;\">&nbsp;<i class='fa fa-user'></i>&nbsp;</span>&nbsp;" +
     "<span class='btn btn-warning padding-none'onclick=\"loadModalContentWithCallback('modal-help','TaskDateTransfer','Task/GroupTask', '?taskId=" + ticketId + "', getTaskActions); return false;\">&nbsp;<i class='fa fa-refresh'></i>&nbsp;</span>&nbsp;" +
    "<span class='btn btn-info padding-none' onclick=\"loadModalContent('modal-help','TaskHistory','Task/GroupTask', '?taskId=" + ticketId + "&taskNumber=" + number + "')\">&nbsp;<i class='icon icon-comment-1' ></i>&nbsp;</span>";
}

function createTaskTicket() {
    if ($("#taskTicket").valid()) {
        $("#modal-help .close").trigger("click");
        $.post(apihost + "/task/post?idCommand=" + $("#task-cmd").val(),
        $("#taskTicket").serialize(),
            function (result) { });
    }
}


function editMeeting(cmd) {
    if ($("#hide" + cmd).length && $("#hide" + cmd).hasClass("hide")) {
        $("div[id^='hide']").addClass("hide");
        $("#hide" + cmd).removeClass("hide");
    }
    else if ($("#taskTicket").valid()) {
        $("#modal-help .close").trigger("click");
        $.ajax({
            url: apihost + "/meeting/post?idCommand=" + cmd,
            data: $("#taskTicket").serialize(),
            method: "POST",
            complete: function () {
                if (cmd == 2) {
                    loadModalContentWithCallback('modal-help', 'Edit', 'Task/Call', '?meetingId=' + $("#taskTicket #TicketId").val(), initCallTask);
                }
                if (cmd == 4) {
                    loadModalContentWithCallback('modal-help', 'Edit', 'Task/Meeting', '', initMeetingTask);
                }

            }
        });
    }
}

function updateCallTask(cmd) {
    if ($("#hide" + cmd).length && $("#hide" + cmd).hasClass("hide")) {
        $("div[id^='hide']").addClass("hide");
        $("#hide" + cmd).removeClass("hide");
    }
    else if ($("#taskTicket").valid()) {
        $("#modal-help .close").trigger("click");
        $.ajax({
            url: apihost + "/calltask/post?idCommand=" + cmd,
            data: $("#taskTicket").serialize(),
            method: "POST",
            complete: function () {
                if (cmd == 10) {
                    loadModalContentWithCallback('modal-help', 'Edit', 'Task/Call', '?prevCallId=' + $("#taskTicket #TicketId").val(), initCallTask);
                }
                if (cmd == 8) {
                    loadModalContentWithCallback('modal-help', 'Edit', 'Task/Meeting', '', initMeetingTask);
                }
            }
        });
    }
}

function updateTaskTicket(cmd) {
    if ($("#taskTicketProcess").valid()) {
        $("#modal-help .close").trigger("click");
        $.post(apihost + "/Task/post?idCommand=" + cmd,
        $("#taskTicketProcess").serialize(),
            function (result) { });
    }
}

function updateGroupTaskTicket()
{
    $("#modal-help .close").trigger("click");
    $.post("/Task/GroupTask/TaskGroup",
      $("#taskGroupEdit").serialize(),
          function (result) {

          });
}

function getPeriod(periodId)
{
    $.get("/Task/GroupTask/TaskPeriod?periodId=" + periodId,
          function (result) {
              $("#period-editor").html(result);
              initDateMask();
          });
}

function updatePeriodTaskTicket() {
    $.post("/Task/GroupTask/TaskPeriod",
      $("#taskPeriodEdit").serialize(),
          function (result) {
          });
}


function transferTask(taskId, date) {
    $.get(apihost + "/Task/transfer?taskId=" + taskId + "&date=" + date + "&userId=" + $("#userId").val(),
        function (result) { });
}

function getTaskActions() {
    if ($("#taskProcessActions").length) {
        $.ajax({
            url: apihost + "/Task/SetViewed?taskId=" + $("#modal-help #TicketId").val() + "&userId=" + $("#userId").val(),
            complete: function () {
                $.get("/CommonTask/Actions4Task?taskId=" + $("#modal-help #TicketId").val(),
                    function (result) {
                        $("#taskProcessActions").html(result);
                    });
            }
        });
    }
    initDateMask();
}

function changeTable(isTable)
{
    $('#isCalendar').val(isTable);
    if (isTable) {
        $("#option2").parent("label").removeClass("active");
        $("#option1").parent("label").addClass("active");

        $("#calendar-div").removeClass("hide");
        $("#table-div").addClass("hide");
    }
    else {
        $("#option2").parent("label").addClass("active");
        $("#option1").parent("label").removeClass("active");

        $("#calendar-div").addClass("hide");
        $("#table-div").removeClass("hide");
        
    }
}

function initGroupTask()
{
    initDateMask();
    $("#Group_AssignedDepartments, #Group_AssignedGroups, #Group_AssignedUsers").select2();
}