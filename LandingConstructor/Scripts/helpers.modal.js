
function loadModalContentWithCallback(modalId, action, controller, paramStr, callback) {
    $("#" + modalId + " .modal-body").html("<img  style='width:360px' src=\"/content/ajax_loader_blue_512.gif\" title=\"/content/ajax_loader_blue_512.gif\" class=\"col-xs-offset-4\" />");
    $("#" + modalId).modal();
    $.get("/" + controller + "/" + action + paramStr, function (result) {
        $("#" + modalId + " .modal-content").html(result);
        var form = $("#" + modalId).find("form")
           .removeData("validator") /* added by the raw jquery.validate plugin */
           .removeData("unobtrusiveValidation");  /* added by the jquery unobtrusive plugin */
        $.validator.unobtrusive.parse(form);
        callback();
    });
}

function loadModalContent(modalId, action, controller, paramStr) {

    $("#" + modalId + " .modal-body").html("<img style='width:360px' src=\"/content/ajax_loader_blue_512.gif\" title=\"/content/ajax_loader_blue_512.gif\" class=\"col-xs-offset-4\" />");
    $("#" + modalId).modal();
    $.get("/" + controller + "/" + action + paramStr, function (result) {
        $("#" + modalId + ' .modal-content').html(result);

        var form = $("#" + modalId).find("form")
            .removeData("validator") /* added by the raw jquery.validate plugin */
            .removeData("unobtrusiveValidation");  /* added by the jquery unobtrusive plugin */

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

