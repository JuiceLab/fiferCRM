var mapExist;
var markers = [];
var igms = false;

$(function () {
    if ($("#css-category").length > 0) {
        $("#css-category").on("change", function () {
            $.get("/SiteTemplate/TemplateCss/GetCategoryCss?siteId=" + $("#siteId").val() + "&categoryId=" + $("#css-category").val(),
                function (result) {
                    $("#availableCss").html(result);
                    bindCssChoice();
                });
        });
    }
    $("#availableCss").on("change", "#css-choice", function () {
        $("#css-variants").attr("href", "/assets/pasteboard/css/" + $("#css-choice").val());
    });
    if ($("#form-submit").length > 0)
    {
        $("#form-submit").on("click", function () {
            if ($("#form-submit").parents("form").validate()) {
                $.post("/Mailer/SendFeedbackMail?siteId=" + $("#siteId").val(),
                $("#form-submit").parents("form").serialize(),
                function () {
                    alert("Выше сообщение успешно отправлено!");
                    $("#form-submit").parents("form").find("input, textarea").val("");
                });
            }
        });
    }
})

function saveCss()
{
    $.get("/PasteBoard/Editor/SaveCss?siteId=" + $("#siteId").val() + "&css=" + $("#css-choice").val(),
        function () {
            location.reload();
        });
}

function bindCssChoice()
{
    if ($("#css-choice").length > 0)
    {
       
    }
}

function updateSiteInfo()
{
    if ($("#info-form").valid())
    {
        $.post($("#info-form").attr("action"),
            $("#info-form").serialize(),
            function () {
                location.reload();
            });
    }
}


function initScripts() {
    if (igms) return;

    var $scripts = [
        "/scripts/maps_google/jquery-ui-map/ui/min/jquery.ui.map.min.js",
        "/scripts/maps_google/jquery-ui-map/ui/jquery.ui.map.extensions.js",
        "/scripts/maps_google/jquery-ui-map/ui/min/jquery.ui.map.services.min.js",
        "/scripts/maps_google/jquery-ui-map/ui/min/jquery.ui.map.microformat.min.js",
        "/scripts/maps_google/jquery-ui-map/addons/markerclustererplus/markerclusterer.min.js"
    ];
    $.each($scripts, function (k, v) {
        if ($('[src="' + v + '"]').length) return true;
        var scriptNode = document.createElement('script');

        scriptNode.src = v;
        $('head').prepend($(scriptNode));
    });

    $.extend($.ui.gmap.prototype, {
        pagination: function (prop) {
            var $el = $("<div id='pagination' class='btn-group btn-group-lg' style='width: 100%'>"
					+ "<a href='#' class='col-md-2 back-btn btn btn-inverse'>&lsaquo;</a>"
					+ "<a class='col-md-8 display btn btn-inverse'></a>"
					+ "<a href='#' class='col-md-2 fwd-btn btn btn-inverse'>&rsaquo;</a>"
				+ "</div>");

            var self = this, i = 0, prop = prop || 'title';
            self.set('pagination', function (a, b) {
                if (a) {
                    i = i + b;
                    $el.find('.display').text(self.get('markers')[i][prop]);
                    self.get('map').panTo(self.get('markers')[i].getPosition());
                }
            });
            self.get('pagination')(true, 0);
            $el.find('.back-btn').click(function (e) {
                e.preventDefault();
                self.get('pagination')((i > 0), -1, this);
            });
            $el.find('.fwd-btn').click(function (e) {
                e.preventDefault();
                self.get('pagination')((i < self.get('markers').length - 1), 1, this);
            });
            self.addControl($el, google.maps.ControlPosition.TOP_LEFT);
        }
    });
    igms = true;
}

function updateGeo()
{
    $.get("/PasteBoard/Editor/UpdateGeo?latitude=" + $("#Latitude").val() + "&longitude=" + $("#Longitude").val() + "&siteId=" + $("#siteId").val(),
        function () {
            location.reload();
        });
}

function initGoogleMaps()
{
    if ($('#google-map').length) {
        initScripts();
        $("#google-map").gmap({
            'zoomControl': true,
            'zoomControlOpt': {
                'style': 'SMALL',
                'position': 'TOP_LEFT'
            },
            'panControl': false,
            'streetViewControl': false,
            'mapTypeControl': false,
            'overviewMapControl': false,
            'scrollwheel': false,
            'mapTypeId': google.maps.MapTypeId.ROADMAP
        }).bind('init', function (event, map) {
            mapExist = map;
            var gmgLatLng = new google.maps.LatLng($("#Latitude").val().replace(",","."),
                                                     $("#Longitude").val().replace(",", "."));
                    $("#google-map").gmap('addMarker', {
                        'position': gmgLatLng,
                        'draggable': true,
                        'bounds': false
                    }, function (map, marker) {
                    }).dragend(function (event) {
                        setLocation($("#ggoogle-map"), event.latLng, this);
                    }).click(function (event) {
                        setLocation($("#google-map"), event.latLng, this);
                    });
                    $("#google-map").gmap('option', 'zoom', 14);
                    google.maps.event.trigger(map, "resize");
                    map.setCenter(gmgLatLng);
                    $(map).click(function (event) {
                        $("#google-map").gmap('addMarker', {
                            'position': event.latLng,
                            'draggable': true,
                            'bounds': false
                        }, function (map, marker) {
                            for (var i = 0; i < markers.length; i++) {
                                markers[i].setMap(null);
                            }
                            markers = [];
                            markers.push(marker);
                            setLocation($("#google-map"), marker.getPosition(), marker);
                        }).dragend(function (event) {
                            setLocation($("#google-map"), event.latLng, this);
                        });
                    });

               
        });
    }
}

function setLocation(curMap, locationMap, marker) {
    if ($("#Latitude").length > 0) {
        $("#Latitude").val(locationMap.k);
        $("#Longitude").val(locationMap.B);
    }
    $(curMap).gmap('search', { 'location': locationMap }, function (results, status) {
        if (status === 'OK') {
            $("#Address").val(results[0].formatted_address);
            marker.setTitle(results[0].formatted_address);
        }
    });
}