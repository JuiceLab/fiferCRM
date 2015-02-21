function getMatrix()
{
    $.get("/dashboard/sync/getmatrix", function (result) { location.reload();});
}

function WWWGridDataBind() {
    if ($("#WWWGridProduct").length > 0) {
        var grid = $("#WWWGridProduct").data("kendoGrid");
        var data = grid.dataSource.data();
        $.each(data, function (i, row) {
            var status = row.IsErrorResponseCode;

            if (status == true) {
                //Change the background color of QtyReceived here
                $('tr[data-uid="' + row.uid + '"] td').first().css("color", "red");
            }
        });
    }
}

function WWWGridCollectionDataBind() {
    if ($("#WWWGridCollection").length > 0) {
        var grid = $("#WWWGridCollection").data("kendoGrid");
        var data = grid.dataSource.data();
        $.each(data, function (i, row) {
            var status = row.IsErrorResponseCode;

            if (status == true) {
                //Change the background color of QtyReceived here
                $('tr[data-uid="' + row.uid + '"] td').first().css("color", "red");
            }
        });
    }
}

function WWWGridPageDataBind() {
    if ($("#WWWGridPageGroup").length > 0) {
        var grid = $("#WWWGridPageGroup").data("kendoGrid");
        var data = grid.dataSource.data();
        $.each(data, function (i, row) {
            var status = row.IsErrorResponseCode;

            if (status == true) {
                //Change the background color of QtyReceived here
                $('tr[data-uid="' + row.uid + '"] td').first().css("color", "red");
            }
        });
    }
}

function WWWGridActivityDataBind() {
    if ($("#WWWGridActivity").length > 0) {
        var grid = $("#WWWGridActivity").data("kendoGrid");
        var data = grid.dataSource.data();
        $.each(data, function (i, row) {
            var status = row.IsErrorResponseCode;

            if (status == true) {
                //Change the background color of QtyReceived here
                $('tr[data-uid="' + row.uid + '"] td').first().css("color", "red");
            }
        });
    }
}


function WWWGridSliderBind() {
    if ($("#WWWGridSlider").length > 0) {
        var grid = $("#WWWGridSlider").data("kendoGrid");
        var data = grid.dataSource.data();
        $.each(data, function (i, row) {
            var status = row.IsErrorResponseCode;

            if (status == true) {
                //Change the background color of QtyReceived here
                $('tr[data-uid="' + row.uid + '"] td').first().css("color", "red");
            }
        });
    }
}