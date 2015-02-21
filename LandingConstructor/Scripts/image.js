function saveImage() {
    $.post("/dashboard/image/UpdateImage",
        $("#ImageEditForm").serialize(),
        function () {
            $(".k-grid .k-i-refresh").trigger('click');
            $("#myModal .close").trigger('click');
        });

}