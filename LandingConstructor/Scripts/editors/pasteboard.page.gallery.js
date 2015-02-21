
function showImageDetails(imageId) {
    $.get("/PasteBoard/Editor/ImageEdit?siteId=" + $("#siteId").val() + "&itemId=" + imageId,
        function (result) {
            $("#image4Edit").html(result);
            var form = $("#info-form")
             .removeData("validator") /* added by the raw jquery.validate plugin */
             .removeData("unobtrusiveValidation");  /* added by the jquery unobtrusive plugin */
            $.validator.unobtrusive.parse(form);
            initImageCropper();
        });
}

function initNoticeImageCropper()
{
    $('#image-cropper').cropit();
    $('.select-image-btn').on("click", function () {
        $('.cropit-image-input').click();
    });

    $("#crop-save-submit").on("click", function () {
        var data = $('#image-cropper').cropit('export', {
            type: 'image/jpeg',
            quality: .9,
            originalSize: true
        });
        $.post("/PasteBoard/ImageLoader/SaveCroped?pageId=" + $("#CurPageId").val() + "&siteId=" + $("#siteId").val() + "&typeId=2",
			{ "strStream": data },
			function (result) {
			    var json = $.parseJSON(result);
			    $("#NoticePath").val(json.url);
			    if (json.url.length > 0)
			        $.post("/PasteBoard/Editor/NoticeImage?pageId=" + $("#CurPageId").val() + "&siteId=" + $("#siteId").val() + "&photoPath=" + $("#NoticePath").val(),
                        null,
                        function () {
                            location.reload();
                        })
			});
    });
}

function initImageCropper() {
    $('#image-cropper').cropit();
    $('.select-image-btn').on("click", function () {
        $('.cropit-image-input').click();
    });

    $("#crop-save").on("click", function () {
        var data = $('#image-cropper').cropit('export', {
            type: 'image/jpeg',
            quality: .9,
            originalSize: true
        });
        $.post("/PasteBoard/ImageLoader/SaveCroped?pageId=" + $("#pageId").val() + "&siteId=" + $("#siteId").val() + "&typeId=1",
			{ "strStream": data },
			function (result) {
			    var json = $.parseJSON(result);
			    $("#Path").val(json.url);
			    if (json.url.length > 0)
			        $("#update-form").removeClass("hide");
			});
    });
}

function deleteImage(itemId) {
    $.get("/PasteBoard/Editor/DeleteImage?itemId=" + itemId + "&siteId=" + $("#siteId").val(),
        function (result) {
            $("#info-form").parents(".modal-content").html(result);
        });
}

function updateImageInfo() {
    if ($("#info-form").valid()) {
        $.post("/PasteBoard/Editor/ImageEdit?siteId=" + $("#siteId").val() + "&pageId=" + $("#pageId").val(),
            $("#info-form").serialize(),
            function (result) {
                $("#info-form").parents(".modal-content").html(result);
            })
    }
}

function removeGallery(pageId) {
    $.get("/PasteBoard/Editor/RemoveGallery?pageId=" + pageId + "&siteId=" + $("#siteId").val(),
        function (result) {
            location.reload();
        });
}