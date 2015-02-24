
var mapExist;
var curMarker;
var markers = [];
var igms = false;

if (typeof initGoogleMaps != 'undefined' && typeof google != 'undefined') {
    igms = true;
    initGoogleMaps();
}


function getAddress(addrId)
{
    $.get("/GeoLocation/LegalEntity/LegalAddressEdit?addrId=" + addrId + "&companyId=" + $("#legalCompanyId").val(),
    function(result){ 
        $("#legal-geo").html(result);
        var gmgLatLng = new google.maps.LatLng(parseFloat($("#legal-geo #Latitude").val()), parseFloat($("#legal-geo #Longitude").val()));
        if ($(".legal-form #DistrictId").length > 0) {
            $(".legal-form #DistrictId").on("change", function () {
                $.get("/GeoLocation/District/GetCities?distrId=" + $(".legal-form #DistrictId").val(),
                    function (result) {
                        $(".legal-form  #city-Drop").html(result);
                        if ($("#google-map-legalgeo-add").length > 0) {
                            $(".legal-form #City").on("change", function () {
                                codeAddress("Россия " + $(".legal-form #City option:selected").text(), "google-map-legalgeo-add");
                            })
                        }
                    });
            });
        }
        $("#Phones").tagsInput();

        setLegalLocation($("#google-map-legalgeo-add"), gmgLatLng, curMarker);
    });
}

function initScripts() {
    if (igms) return;

    var $scripts = [
        "/assets/plugins/maps_google/jquery-ui-map/ui/min/jquery.ui.map.min.js",
        "/assets/plugins/maps_google/jquery-ui-map/ui/jquery.ui.map.extensions.js",
        "/assets/plugins/maps_google/jquery-ui-map/ui/min/jquery.ui.map.services.min.js",
        "/assets/plugins/maps_google/jquery-ui-map/ui/min/jquery.ui.map.microformat.min.js",
        "/assets/plugins/maps_google/jquery-ui-map/addons/markerclustererplus/markerclusterer.min.js"
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

function resizeGMap()
{
    initGeoLocationMap();
    $('#google-map-geo-add').trigger("resize");
}

function initGeoLocationMap() {
    initScripts();
    if ($('#google-map-geo-add').length) {
        $("#google-map-geo-add").parents("form").addClass("cur-form");
        $("#google-map-geo-add").gmap({
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
            if (navigator.geolocation) {
                navigator.geolocation.getCurrentPosition(function (position) {
                    var gmgLatLng = new google.maps.LatLng(position.coords.latitude,
                                                     position.coords.longitude);
                    if ($(".cur-form #GeoAddr_Latitude").val().length > 0) {
                        gmgLatLng  = new google.maps.LatLng(parseFloat($(".cur-form #GeoAddr_Latitude").val()), parseFloat($(".cur-form #GeoAddr_Longitude").val()))
                    }
                    else {
                        $(".cur-form #GeoAddr_Latitude").val(position.coords.latitude);
                        $(".cur-form #GeoAddr_Longitude").val(position.coords.longitude);
                    }

                    $("#google-map-geo-add").gmap('addMarker', {
                        'position': gmgLatLng,
                        'draggable': true,
                        'bounds': false
                    }, function (map, marker) {
                    }).dragend(function (event) {
                        setLocation($("#google-map-geo-add"), event.latLng, this);
                    }).click(function (event) {
                        setLocation($("#google-map-geo-add"), event.latLng, this);
                    });
                    $("#google-map-geo-add").gmap('option', 'zoom', 14);
                    google.maps.event.trigger(map, "resize");
                    map.setCenter(gmgLatLng);
                    $(map).click(function (event) {
                        $("#google-map-geo-add").gmap('addMarker', {
                            'position': event.latLng,
                            'draggable': true,
                            'bounds': false
                        }, function (map, marker) {
                            for (var i = 0; i < markers.length; i++) {
                                markers[i].setMap(null);
                            }
                            markers = [];
                            markers.push(marker);
                            setLocation($("#google-map-geo-add"), marker.getPosition(), marker);
                        }).dragend(function (event) {
                            setLocation($("#google-map-geo-add"), event.latLng, this);
                        });
                    });

                });
            }
        });
    }
   
}

function setAddLegalAddr() {
    $(".legal-form #Address").val($(".legal-form #DistrictId option:selected").text()  + " "+$(".legal-form #City option:selected").text() + " " + $(".legal-form #Street").val() + " " + $(".legal-form #Number").val() + " " + $(".legal-form #AddNumber").val());
    $(".legal-form #addr").val($(".legal-form #Address").val());
    codeAddress($(".legal-form #Address").val(), "google-map-legalgeo-add");
}

function setLegalAddr()
{
    $("#Address").val($("#City option:selected").text() + " " + $("#Street").val() + " " + $("#App").val() + " " + $("#AddApp").val());
    codeAddress($("#Address").val(), "google-map-geo-add");
}

function codeAddress(address, map_id) {
    geocoder = new google.maps.Geocoder();
    geocoder.geocode({
        'address': address
    }, function (results, status) {
        if (status == google.maps.GeocoderStatus.OK) {
            var myOptions = {
                zoom: 14,
                center: results[0].geometry.location,
                mapTypeId: google.maps.MapTypeId.ROADMAP
            }
            map = new google.maps.Map(document.getElementById(map_id), myOptions);
            map.setCenter(results[0].geometry.location);

            var marker = new google.maps.Marker({
                map: map,
                position: results[0].geometry.location
            });
            if (map_id == 'google-map-add')
                setLocation($("#google-map-add"), map.center, marker);
            else
                setLegalLocation($("#google-map-legalgeo-add"), map.center, marker);

        }
    });
}

function initLegalGeo() {
    if ($('#google-map-legalgeo-add').length) {
        $("#google-map-legalgeo-add").gmap({
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
            $('#google-map-legalgeo-add').css("overflow", "visible");
            mapExist = map;
            if (navigator.geolocation) {
                navigator.geolocation.getCurrentPosition(function (position) {
                    var gmgLatLng = new google.maps.LatLng(position.coords.latitude,
                                                     position.coords.longitude);
                    $("#google-map-legalgeo-add").gmap('addMarker', {
                        'position': gmgLatLng,
                        'draggable': true,
                        'bounds': false
                    }, function (map, marker) {
                        curMarker = marker;
                    }).dragend(function (event) {
                        setLegalLocation($("#google-map-legalgeo-add"), event.latLng, this);
                    }).click(function (event) {
                        setLegalLocation($("#google-map-legalgeo-add"), event.latLng, this);
                    });
                    $("#google-map-legalgeo-add").gmap('option', 'zoom', 12);
                    google.maps.event.trigger(map, "resize");
                    map.setCenter(gmgLatLng);
                    $(map).click(function (event) {
                        $("#google-map-legalgeo-add").gmap('addMarker', {
                            'position': event.latLng,
                            'draggable': true,
                            'bounds': false
                        }, function (map, marker) {
                            for (var i = 0; i < markers.length; i++) {
                                markers[i].setMap(null);
                            }
                            curMarker = marker;
                            markers = [];
                            markers.push(marker);
                            setLegalLocation($("#google-map-legalgeo-add"), marker.getPosition(), marker);
                        }).dragend(function (event) {
                            setLegalLocation($("#google-map-legalgeo-add"), event.latLng, this);
                        });
                    });
                });
            }
        });
    }
}
    function initGoogleMaps() {
        initScripts();

        if ($('#google-map-add').length) {
            $("#google-map-add").gmap({
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
                if (navigator.geolocation) {
                    navigator.geolocation.getCurrentPosition(function (position) {
                        var gmgLatLng = new google.maps.LatLng(position.coords.latitude,
                                                         position.coords.longitude);
                        $("#google-map-add").gmap('addMarker', {
                            'position': gmgLatLng,
                            'draggable': true,
                            'bounds': false
                        }, function (map, marker) {
                        }).dragend(function (event) {
                            setLocation($("#google-map-add"), event.latLng, this);
                        }).click(function (event) {
                            setLocation($("#google-map-add"), event.latLng, this);
                        });
                        $("#google-map-add").gmap('option', 'zoom', 12);
                        google.maps.event.trigger(map, "resize");
                        map.setCenter(gmgLatLng);
                        $(map).click(function (event) {
                            $("#google-map-add").gmap('addMarker', {
                                'position': event.latLng,
                                'draggable': true,
                                'bounds': false
                            }, function (map, marker) {
                                for (var i = 0; i < markers.length; i++) {
                                    markers[i].setMap(null);
                                }
                                markers = [];
                                markers.push(marker);
                                setLocation($("#google-map-add"), marker.getPosition(), marker);
                            }).dragend(function (event) {
                                setLocation($("#google-map-add"), event.latLng, this);
                            });
                        });

                    });
                }
            });
        }
    }

    function loadMapWithGeo(curLat, curLong) {
        initScripts();
        $("form").removeClass("cur-form");
        $("#modal-map").modal();

        if (curLat.length > 0 && curLong.length > 0) {
            var gmgLatLng = new google.maps.LatLng(parseFloat(curLat), parseFloat(curLong));
            $("#google-map-add").gmap('option', 'center', gmgLatLng);
            $("#google-map-add").gmap('option', 'zoom', 14);
            $("#google-map-add").gmap('addMarker', {
                'position': gmgLatLng,
                'draggable': true,
                'bounds': false
            });
        }
        setTimeout(function () { resizingMap(); }, 400);

    }

    function initFormDialogMap(curItem) {
        initScripts();
        $("form").removeClass("cur-form");
        $("#modal-map").modal();
        var form = $(curItem).parents("form").addClass("cur-form");

        var curLat = $(form).find("#Latitude").val();
        var curLong = $(form).find("#Longitude").val();
        if (curLat.length > 0 && curLong.length > 0) {
            var gmgLatLng = new google.maps.LatLng(parseFloat(curLat), parseFloat(curLong));
            $("#google-map-add").gmap('option', 'center', gmgLatLng)
        }
        setTimeout(function () { resizingMap(); }, 400);

    }

    function resizingMap() {
        if (typeof mapExist == "undefined") return;
        var center = mapExist.getCenter();
        google.maps.event.trigger(mapExist, "resize");
        mapExist.setCenter(center);
    }

    function setLegalLocation(curMap, locationMap, marker) {
        if ($(".legal-form #Latitude").length > 0) {
            $(".legal-form #Latitude").val(locationMap.k);
            $(".legal-form #Longitude").val(locationMap.B);
            $("#google-map-legalgeo-add").gmap('option', 'center', locationMap);

            $('#google-map-legalgeo-add').trigger("resize");
        }
        $(curMap).gmap('search', { 'location': locationMap }, function (results, status) {
            if (status === 'OK') {
                if ($("#addr").length) {
                    $("#addr").text(results[0].formatted_address);
                }
                $(".legal-form #Address").val(results[0].formatted_address);
                //marker.setTitle(results[0].formatted_address);
            }
        });
    }

    function setLocation(curMap, locationMap, marker) {
        if ($(".cur-form #Latitude").length > 0) {
            $(".cur-form #Latitude").val(locationMap.k);
            $(".cur-form #Longitude").val(locationMap.D);

        }
        else {
            $(".cur-form #GeoAddr_Latitude").val(locationMap.k);
            $(".cur-form #GeoAddr_Longitude").val(locationMap.D);
        }
        $(curMap).gmap('search', { 'location': locationMap }, function (results, status) {
            if (status === 'OK') {
                if ($("#addr").length) {
                    $("#addr").text(results[0].formatted_address);
                }
                $(".cur-form #Address").val(results[0].formatted_address);
                //marker.setTitle(results[0].formatted_address);
            }
        });
    }
