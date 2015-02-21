
function addGallery(siteId, pageId, isSlider) {
    $.get("/PasteBoard/Editor/AddGallery?siteId=" + siteId + "&pageId=" + pageId + "&isSlider=" + isSlider,
        function () {
            location.reload();
    });
}

function initCkEditor()
{
    if (CKEDITOR.instances["Text"] != undefined)
        CKEDITOR.instances["Text"].destroy();
    $('#Text').ckeditor();
    $('#Title').keyup(function () { translit(this, '#Anchor'); });
}

function updatePage()
{
    $.get("/PasteBoard/Editor/IsExistAnchor?siteId=" + $("#ProjectId").val() + "&anchor=" + $("#Anchor").val() +"&pageId=" + $("#PageId").val(),
        function (result) {
            if (!result.isExist)
            {
                if ($("#page-form").valid()) {
                    $.post($("#page-form").attr("action"),
                        $("#page-form").serialize(),
                        function (result) {
                            location.href=result.url;
                        });
                }
            }
        });
    
}

function removePage(pageId) {
    $.get("/PasteBoard/Editor/RemovePage?pageId=" + pageId + "&siteId=" + $("#siteId").val(),
        function (result) {
            location.reload();
        });
}