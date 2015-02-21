
function initCropper() {
	if ($("#BirthDate").length > 0)
	    $("#BirthDate").inputmask("d.m.y", { "placeholder": "дд.мм.гггг" });
	if ($("#SocialLinks").length > 0)
	    $("#SocialLinks").tagsInput();

	if ($("#DateIssue").length > 0)
		$("#DateIssue").inputmask("d.m.y", { "placeholder": "дд.мм.гггг" });
	if ($("#Passport_DateIssue").length > 0)
	    $("#Passport_DateIssue").inputmask("d.m.y", { "placeholder": "дд.мм.гггг" });
	$('#image-cropper').cropit();
	
	initMultiSelect();
}
