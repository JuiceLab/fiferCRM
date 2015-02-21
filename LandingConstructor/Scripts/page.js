var description = ""
var keyWord = "";
$(function () {
    var pageId = $(".pageId").val();
    //$.post("/dashboard/seo/RefreshSeoBlockItem?pageId=" + pageId,
    //   function (result) {
    //       description = result.MetaDescription;
    //       keyWord = result.MetaKeyWords;
    //   });
})


function savePage() {
    var isValid = true; 
        if ($("#Anchor").val() == "" || $("#Title").val() == "") {
            $(".error").text("Поле обязательны для заполнения").addClass("label label-danger");
            isValid = false;
            $("#pageBlock").show();
            $("#contentBlock4Page").hide();          
        }    
    
    if(isValid==true) {
        $.post("/dashboard/page/UpdatePageAdd",
            $("#PageItemUpdate").serialize(),
            function () {
                $(".k-grid .k-i-refresh").trigger('click');
                $("#myModal .close").trigger('click');
            });
    }
}

function LoadContentBlock(isContentBlock) {
    if (isContentBlock == true) {
        $("#pageBlock").hide();      
        $("#contentBlock4Page").show();
    }
    else {       
        $("#contentBlock4Page").hide();
        $("#pageBlock").show();     
    }

}

function refreshSeoBlock(name) {
    if (name == "description") {
        $(".description").val(description);
    }
    if (name == "keyWord") {
        $(".keyWord").val(keyWord);
    }
     
}

function refreshSeoBlockItem(seoItemId,  name) {
        
           if (name == "description") {             
               var curCl = "#description" + seoItemId
               var cur = $("#regionSeo").find(curCl)
               cur.val(description);
           }
           if (name == "keyWord") {
               var curCl = "#keyWord" + seoItemId
               var cur = $("#regionSeo").find(curCl)
               cur.val(keyWord);
           }        
}

