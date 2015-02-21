$(function () {
    $("#ContentBlockPos").live("change", function () {
        loadModalContent('myModal', 'PageEdit', 'dashboard/page', '?pageId=' + $("#PageItemUpdate #PageId").val() + "&pos=" + $("#ContentBlockPos").val());
    })
})

function saveSeo() {
    $.post("/dashboard/seo/itemsave",
        $("#SeoItemUpdate").serialize(),
        function () {
            $(".k-grid .k-i-refresh").trigger('click');
            $("#myModal .close").trigger('click');
        });
}

function updateSeo()
{
    $.get("/dashboard/seo/syncseo", function (result) { location.reload(); })
}