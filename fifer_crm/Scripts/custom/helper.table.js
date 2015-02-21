function initTable()
{
    if ($('.dynamicTable').size() > 0) {
        $('.dynamicTable').each(function () {
            // DataTables with TableTools
            if ($(this).is('.tableTools')) {
                $(this).dataTable({
                    "oLanguage": {
                        "sProcessing": "Подождите...",
                        "sLengthMenu": "Показать _MENU_ записей",
                        "sZeroRecords": "Записи отсутствуют.",
                        "sInfo": "Записи с _START_ до _END_ из _TOTAL_ записей",
                        "sInfoEmpty": "Записи с 0 до 0 из 0 записей",
                        "sInfoFiltered": "(отфильтровано из _MAX_ записей)",
                        "sInfoPostFix": "",
                        "sSearch": "Поиск:",
                        "sUrl": "",
                        "oPaginate": {
                            "sFirst": "Первая",
                            "sPrevious": "Предыдущая",
                            "sNext": "Следующая",
                            "sLast": "Последняя"
                        },
                        "oAria": {
                            "sSortAscending": ": активировать для сортировки столбца по возрастанию",
                            "sSortDescending": ": активировать для сортировки столбцов по убыванию"
                        }
                    },
                    "sPaginationType": "bootstrap",
                    "sDom": "<'row separator bottom'<'col-md-5'T><'col-md-3'l><'col-md-4'f>r>t<'row'<'col-md-6'i><'col-md-6'p>>",
                    "oLanguage": {
                        "sLengthMenu": "_MENU_ Show"
                    },
                    "oTableTools": {
                        "sSwfPath": componentsPath + "../plugins/tables_datatables/extras/TableTools/media/swf/copy_csv_xls_pdf.swf"
                    },
                    "aoColumnDefs": [
                      { 'bSortable': false, 'aTargets': [0] }
                    ],
                    "sScrollX": "100%",
                    "sScrollXInner": "100%",
                    "bScrollCollapse": true,
                    "fnInitComplete": function () {
                        fnInitCompleteCallback(this);
                    }
                });
            }
                // colVis extras initialization
            else if ($(this).is('.colVis')) {
                $(this).dataTable({
                    "oLanguage": {
                        "sProcessing": "Подождите...",
                        "sLengthMenu": "Показать _MENU_ записей",
                        "sZeroRecords": "Записи отсутствуют.",
                        "sInfo": "Записи с _START_ до _END_ из _TOTAL_ записей",
                        "sInfoEmpty": "Записи с 0 до 0 из 0 записей",
                        "sInfoFiltered": "(отфильтровано из _MAX_ записей)",
                        "sInfoPostFix": "",
                        "sSearch": "Поиск:",
                        "sUrl": "",
                        "oPaginate": {
                            "sFirst": "Первая",
                            "sPrevious": "Предыдущая",
                            "sNext": "Следующая",
                            "sLast": "Последняя"
                        },
                        "oAria": {
                            "sSortAscending": ": активировать для сортировки столбца по возрастанию",
                            "sSortDescending": ": активировать для сортировки столбцов по убыванию"
                        }
                    },
                    "sPaginationType": "bootstrap",
                    "sDom": "<'row separator bottom'<'col-md-3'f><'col-md-3'l><'col-md-6'C>r>t<'row'<'col-md-6'i><'col-md-6'p>>",
                    "oLanguage": {
                        "sLengthMenu": "_MENU_ per page"
                    },
                    "oColVis": {
                        "buttonText": "Show / Hide Columns",
                        "sAlign": "right"
                    },
                    "sScrollX": "100%",
                    "sScrollXInner": "100%",
                    "bScrollCollapse": true,
                    "fnInitComplete": function () {
                        fnInitCompleteCallback(this);
                    }
                });
            }
            else if ($(this).is('.scrollVertical')) {
                $(this).dataTable({
                    "oLanguage": {
                        "sProcessing": "Подождите...",
                        "sLengthMenu": "Показать _MENU_ записей",
                        "sZeroRecords": "Записи отсутствуют.",
                        "sInfo": "Записи с _START_ до _END_ из _TOTAL_ записей",
                        "sInfoEmpty": "Записи с 0 до 0 из 0 записей",
                        "sInfoFiltered": "(отфильтровано из _MAX_ записей)",
                        "sInfoPostFix": "",
                        "sSearch": "Поиск:",
                        "sUrl": "",
                        "oPaginate": {
                            "sFirst": "Первая",
                            "sPrevious": "Предыдущая",
                            "sNext": "Следующая",
                            "sLast": "Последняя"
                        },
                        "oAria": {
                            "sSortAscending": ": активировать для сортировки столбца по возрастанию",
                            "sSortDescending": ": активировать для сортировки столбцов по убыванию"
                        }
                    },
                    "bPaginate": false,
                    "sScrollY": $("#sidebar-fusion-wrapper").height() > 350 ? $("#sidebar-fusion-wrapper").height() - 350 : 200,
                    "sScrollX": "100%",
                    "sScrollXInner": "100%",
                    "bScrollCollapse": true,
                    "sDom": "<'row separator bottom'<'col-md-12'f>r>t<'row'<'col-md-6'i><'col-md-6'p>>",
                    "fnInitComplete": function () {
                        fnInitCompleteCallback(this);
                    }
                });
            }
            else if ($(this).is('.ajax')) {
                $(this).dataTable({
                    "oLanguage": {
                        "sProcessing": "Подождите...",
                        "sLengthMenu": "Показать _MENU_ записей",
                        "sZeroRecords": "Записи отсутствуют.",
                        "sInfo": "Записи с _START_ до _END_ из _TOTAL_ записей",
                        "sInfoEmpty": "Записи с 0 до 0 из 0 записей",
                        "sInfoFiltered": "(отфильтровано из _MAX_ записей)",
                        "sInfoPostFix": "",
                        "sSearch": "Поиск:",
                        "sUrl": "",
                        "oPaginate": {
                            "sFirst": "Первая",
                            "sPrevious": "Предыдущая",
                            "sNext": "Следующая",
                            "sLast": "Последняя"
                        },
                        "oAria": {
                            "sSortAscending": ": активировать для сортировки столбца по возрастанию",
                            "sSortDescending": ": активировать для сортировки столбцов по убыванию"
                        }
                    },
                    "sPaginationType": "bootstrap",
                    "bProcessing": true,
                    "sAjaxSource": rootPath + 'admin/ajax/DataTables.json',
                    "sDom": "<'row separator bottom'<'col-md-12'f>r>t<'row'<'col-md-6'i><'col-md-6'p>>",
                    "sScrollX": "100%",
                    "sScrollXInner": "100%",
                    "bScrollCollapse": true,
                    "fnInitComplete": function () {
                        fnInitCompleteCallback(this);
                    }
                });
            }
            else if ($(this).is('.fixedHeaderColReorder')) {
                $(this).dataTable({
                    "oLanguage": {
                        "sProcessing": "Подождите...",
                        "sLengthMenu": "Показать _MENU_ записей",
                        "sZeroRecords": "Записи отсутствуют.",
                        "sInfo": "Записи с _START_ до _END_ из _TOTAL_ записей",
                        "sInfoEmpty": "Записи с 0 до 0 из 0 записей",
                        "sInfoFiltered": "(отфильтровано из _MAX_ записей)",
                        "sInfoPostFix": "",
                        "sSearch": "Поиск:",
                        "sUrl": "",
                        "oPaginate": {
                            "sFirst": "Первая",
                            "sPrevious": "Предыдущая",
                            "sNext": "Следующая",
                            "sLast": "Последняя"
                        },
                        "oAria": {
                            "sSortAscending": ": активировать для сортировки столбца по возрастанию",
                            "sSortDescending": ": активировать для сортировки столбцов по убыванию"
                        }
                    },
                    "sPaginationType": "bootstrap",
                    "sDom": "R<'clear'><'row separator bottom'<'col-md-12'f>r>t<'row'<'col-md-6'i><'col-md-6'p>>",
                    "sScrollX": "100%",
                    "sScrollXInner": "100%",
                    "bScrollCollapse": true,
                    "fnInitComplete": function () {
                        fnInitCompleteCallback(this);
                        var t = this;
                        setTimeout(function () {
                            new FixedHeader(t);
                        }, 1000);
                    }
                });
            }
                // default initialization
            else {
                $(this).dataTable({
                    "sPaginationType": "bootstrap",
                    "sDom": "<'row separator bottom'<'col-md-5'T><'col-md-3'l><'col-md-4'f>r>t<'row'<'col-md-6'i><'col-md-6'p>>",
                    "sScrollX": "100%",
                    "sScrollXInner": "100%",
                    "bScrollCollapse": true,
                    "oLanguage": {
                        "sProcessing": "Подождите...",
                        "sLengthMenu": "Показать _MENU_ записей",
                        "sZeroRecords": "Записи отсутствуют.",
                        "sInfo": "Записи с _START_ до _END_ из _TOTAL_ записей",
                        "sInfoEmpty": "Записи с 0 до 0 из 0 записей",
                        "sInfoFiltered": "(отфильтровано из _MAX_ записей)",
                        "sInfoPostFix": "",
                        "sSearch": "Поиск:",
                        "sUrl": "",
                        "oPaginate": {
                            "sFirst": "Первая",
                            "sPrevious": "Предыдущая",
                            "sNext": "Следующая",
                            "sLast": "Последняя"
                        },
                        "oAria": {
                            "sSortAscending": ": активировать для сортировки столбца по возрастанию",
                            "sSortDescending": ": активировать для сортировки столбцов по убыванию"
                        }
                    },
                    "fnInitComplete": function () {
                        fnInitCompleteCallback(this);
                    }
                });
            }
        });
    }
}

function addTableItem()
{
    $.post("/Finances/Payment/PaymentDetailsTable?serviceId="+ $("#service4Payment").val() + "&qty=" + $("#qty").val(),
    $("#paymentForm").serialize(),
    function (result) {
        $("#payment-details-div").html(result);
    });
}

function addPaymentTableItem() {
    $.get("/Finances/PaymentActs/PaymentActDetailsTable?payments=" + $("#StrPayments").val() +($("#payment4Act").val() !=null? "," + $("#payment4Act").val() : "")+ "&companyGuid=" + $("#CompanyId").val(),
    function (result) {
        $("#payment-details-div").html(result);
    });
}
