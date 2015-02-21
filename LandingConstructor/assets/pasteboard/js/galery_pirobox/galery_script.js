

jQuery(document).ready(function() {
	jQuery().piroBox({
			my_speed: 400, //скорость анимации
			bg_alpha: 0.6, //прозрачность фона
			slideShow : true, // true == slideshow on, false == slideshow off
			slideSpeed : 4, //slideshow duration in seconds(3 to 6 Recommended)
			close_all : '.piro_close,.piro_overlay'// add class .piro_overlay(with comma)if you want overlay click close piroBox

	});
});
