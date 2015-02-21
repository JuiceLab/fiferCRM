function loadModalContentWithCallback(modalId, action, controller, paramStr, callback) {
    $("#" + modalId + " .modal-body").html("<img src=\"/content/img/ajax-loaders/ajax-loader-6.gif\" title=\"/content/img/ajax-loaders/ajax-loader-6.gif\"/>");
    $("#" + modalId).modal();
    $.get("/" + controller + "/" + action + paramStr, function (result) {
        $("#" + modalId).html(result);
        callback();
    });
}

function loadModalContent(modalId, action, controller, paramStr) {
    $("#" + modalId + " .modal-body").html("<img src=\"/content/img/ajax-loaders/ajax-loader-6.gif\" title=\"/content/img/ajax-loaders/ajax-loader-6.gif\"/>");
    $("#" + modalId).modal();
	$.get("/" + controller + "/" + action + paramStr, function (result) {
	    $("#" + modalId).html(result);
	    var form = $("#" + modalId + " .modal-body form");
	    form.removeData('validator');
	    form.removeData('unobtrusiveValidation');
	    $.validator.unobtrusive.parse(form);

	});
}

function initFullSync() {
    $.post("/dashboard/sync/fullupdate",
        $("#SyncWeb").serialize(),
        function (result) {
            location.reload();
        });
}

function addTag()
{
    $.get("/dashboard/tag/addTag?text=" + $("#TagText").val(), function (result) {
        $(".k-grid .k-i-refresh").trigger('click');
        $("#myModal .close").trigger('click');
    });
}

function addGroupTag() {
    $.get("/dashboard/tag/addGroupTag?text=" + $("#TagText").val(), function (result) {
        $(".k-grid .k-i-refresh").trigger('click');
        $("#myModal .close").trigger('click');
    });
} 

function initActivityPositionSync()
{
    $.get("/dashboard/sync/positionupdate", function (result) {
        location.reload();
    });
}

function updateTagRelations()
{
    $(".ms-selected").each(function (index, item) {
        var text = $(this).find("span").text();
        $("#ChoicedValues option:contains('" + text + "')").prop("selected", "selected");

    });
    $.post("/dashboard/tag/updatetagrelation", $("#tagRelationForm").serialize(), function () {
        $(".k-grid .k-i-refresh").trigger('click');
        $("#myModal .close").trigger('click');

    });
}

function initTagMultiselect()
{
    $('#ChoicedValues').multiSelect({
        selectableHeader: "<div class='custom-header'>Без привязки к тэгу</div>",
        selectionHeader: "<div class='custom-header'>Привязаны к тэгу</div>"
    });
    $("#ms-ChoicedValues").css("padding-left", "10px");
}

function editRegionPageBlock()
{
    window.open("/dashboard/page/PageContentBlockEdit?pageId=" + $("#PageId").val() + "&pos=" + $("#ContentBlockPos").val(),
          "Региональные блоки контента",
          "target=_blank,scrollbars=1,width=1005,height=800");
}

function editRegionPageMetaTags() {
    window.open("/dashboard/seo/ItemRegionEdit?pageId=" + $("#PageId").val(),
          "Региональные метатэги",
          "target=_blank,scrollbars=1,width=1005,height=800");
}

function syncProduct(productId) {
    $.get("/dashboard/sync/SyncProduct?productId=" + productId,
        function (result) {
            location.reload();
        });
}

function saveSlider() {
    $.post("/dashboard/slider/updateslider",
        $("#SlidetemUpdate").serialize(),
        function () {
            $(".k-grid .k-i-refresh").trigger('click');
            $("#myModal .close").trigger('click');
        });
}

