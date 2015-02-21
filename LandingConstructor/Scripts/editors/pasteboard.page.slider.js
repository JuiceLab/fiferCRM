function showSlideDetails(slideId) {
    $.get("/PasteBoard/Editor/SlideEdit?siteId=" + $("#siteId").val() + "&itemId=" + slideId,
        function (result) {
            $("#slide4Edit").html(result);
            initCropper();
            var form = $("#info-form")
              .removeData("validator") /* added by the raw jquery.validate plugin */
              .removeData("unobtrusiveValidation");  /* added by the jquery unobtrusive plugin */
            $.validator.unobtrusive.parse(form);
        });
}

function initCropper() {
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
        $.post("/PasteBoard/ImageLoader/SaveCroped?pageId=" + $("#pageId").val() + "&siteId=" + $("#siteId").val() + "&typeId=0",
			{ "strStream": data },
			function (result) {
			    var json = $.parseJSON(result);
			    $("#PhotoPath").val(json.url);
			    if (json.url.length > 0)
			        $("#update-form").removeClass("hide");
			});
    });
}

function deleteSlide(itemId)
{
    $.get("/PasteBoard/Editor/DeleteSlide?itemId=" + itemId + "&siteId=" + $("#siteId").val(),
        function (result)
        {
            $("#info-form").parents(".modal-content").html(result);
        });
}

function updateSlideInfo()
{
    if ($("#info-form").valid()) {
        $.post("/PasteBoard/Editor/SlideEdit?siteId=" + $("#siteId").val() + "&pageId=" + $("#pageId").val(),
            $("#info-form").serialize(),
            function (result) {
                $("#info-form").parents(".modal-content").html(result);
            });
    }
}