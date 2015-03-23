
var mapAdd;
var mapAddGeo;
var mapLegalGeo;

var curMarker;
var markers = [];
var igms = false;

var geocoder ;

if (typeof initGoogleMaps != 'undefined' && typeof google != 'undefined') {
    igms = true;
    initGoogleMaps();
}


function getAddress(addrId)
{
    $.get("/GeoLocation/LegalEntity/LegalAddressEdit?addrId=" + addrId + "&companyId=" + $("#legalCompanyId").val(),
    function(result){ 
        $("#legal-geo").html(result);
        $(".legal-form").removeClass('hide');
        var gmgLatLng = new google.maps.LatLng(parseFloat($("#legal-geo #Latitude").val()), parseFloat($("#legal-geo #Longitude").val()));

        if ($(".legal-form #DistrictId").length > 0) {
            $(".legal-form #DistrictId").on("change", function () {
                $.get("/GeoLocation/District/GetCities?distrId=" + $(".legal-form #DistrictId").val(),
                    function (result) {
                        $(".legal-form  #city-Drop").html(result);
                        if ($("#google-map-legalgeo-add").length > 0) {
                            $(".legal-form #City").on("change", function () {
                                codeAddress("Россия " + $(".legal-form #City option:selected").text(), true);
                            })
                        }
                    });
            });
        }
        $("#Phones").tagsInput();
        if (addrId == undefined)
            clearMarkers();
        else
            setLegalLocation(gmgLatLng, curMarker);
    });
}

function clearMarkers()
{
    for (var i = 0; i < markers.length; i++) {
        markers[i].setMap(null);
    }
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
            mapAddGeo = map;
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

function setAddAddr() {
    $("#legalEditForm #Address").val($("#legalEditForm #DistrictId option:selected").text() + " " + $("#legalEditForm #City option:selected").text() + " " + $("#address-data #Street").val() + " " + $("#address-data #App").val() + " " + $("#address-data #AddApp").val());
    $("#legalEditForm #addr").val($("#legalEditForm #Address").val());
    codeAddAddress($("#legalEditForm #Address").val());
}
function setAddLegalAddr()
{
    $("#Address").val($("#City option:selected").text() + " " + $("#Street").val() + " " + $("#App").val() + " " + $("#AddApp").val());
    codeAddress($("#Address").val(), true);

}
function setLegalAddr()
{
    $("#Address").val($("#City option:selected").text() + " " + $("#Street").val() + " " + $("#App").val() + " " + $("#AddApp").val());
    codeAddress($("#Address").val(), true);
}

function codeAddress(address, isLegal) {
    clearMarkers();
    geocoder = new google.maps.Geocoder();
    geocoder.geocode({
        'address': address
    }, function (results, status) {
        if (status == google.maps.GeocoderStatus.OK) {
            var marker = new google.maps.Marker({
                map: isLegal? mapLegalGeo : mapAdd,
                position: results[0].geometry.location
            });
            markers.push(marker);
            if (isLegal) {
                mapLegalGeo.setCenter(results[0].geometry.location);
                setLegalLocation(mapLegalGeo.center, marker);
            } else {
                mapAdd.setCenter(results[0].geometry.location);
                setLocation(mapAdd.center, marker);
            }
        }
    });
}


function codeAddAddress(address) {
    clearMarkers();
    geocoder = new google.maps.Geocoder();
    geocoder.geocode({
        'address': address
    }, function (results, status) {
        if (status == google.maps.GeocoderStatus.OK) {
            var marker = new google.maps.Marker({
                map: mapAddGeo,
                position: results[0].geometry.location
            });
            markers.push(marker);
            mapAddGeo.setCenter(results[0].geometry.location);
            setAddLocation(mapAddGeo.center, marker);
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
            mapLegalGeo = map;
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
                    $("#google-map-legalgeo-add").gmap('option', 'zoom', 14);
                    google.maps.event.trigger(map, "resize");
                    map.setCenter(gmgLatLng);
                    resizingMap(mapLegalGeo);
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
                            setLegalLocation(marker.getPosition(), marker);
                        }).dragend(function (event) {
                            setLegalLocation(event.latLng, this);
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
                mapAdd = map;
                if (navigator.geolocation) {
                    navigator.geolocation.getCurrentPosition(function (position) {
                        var gmgLatLng = new google.maps.LatLng(position.coords.latitude,
                                                         position.coords.longitude);
                        var marker = new google.maps.Marker({
                            position: event.latLng,
                            map: mapAdd,
                            title: '',
                            dragend: function (event) {
                                setLocation($("#google-map-add"), event.latLng, this)
                            },
                            click: function (event) {
                                setLocation($("#google-map-add"), event.latLng, this);
                            }
                        });
                        markers.push(marker);

                        mapAdd.setZoom(14);
                        google.maps.event.trigger(mapAdd, "resize");
                        mapAdd.setCenter(gmgLatLng);
                        $(map).click(function (event) {
                            var marker = new google.maps.Marker({
                                position: event.latLng,
                                map: mapAdd,
                                title: ''
                            });
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
                }
            });
        }
    }

    function loadMapWithGeo(curLat, curLong, addr) {
        initScripts();
        $("form").removeClass("cur-form");
        $("#modal-map").modal();

        if (curLat.length > 0 && curLong.length > 0) {
            var gmgLatLng = new google.maps.LatLng(parseFloat(curLat), parseFloat(curLong));
            mapAdd.setCenter(gmgLatLng);
            mapAdd.setZoom(14);
            geocoder = new google.maps.Geocoder();
            geocoder.geocode({ 'latLng': gmgLatLng }, function (results, status) {
                if (status == google.maps.GeocoderStatus.OK) {

                    if (results[0]) {
                        var marker = new google.maps.Marker({
                            position: gmgLatLng,
                            map: mapAdd
                        });
                        markers.push(marker);

                        google.maps.event.addListener(marker, 'click', function () {
                            infowindow.open(mapAdd, marker);
                        });
                        var infowindow = new google.maps.InfoWindow({
                            content: '<p>' + results[0].formatted_address + '</p>'
                        });
                        infowindow.open(mapAdd, marker)
                    }
                } else {
                    alert("Geocoder failed due to: " + status);
                }
            });
        }
        setTimeout(function () {
            resizingMap(mapAdd);
        }, 200);

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
            mapAdd.setCenter(gmgLatLng);
            mapAdd.setZoom(14);
            geocoder = new google.maps.Geocoder();
            geocoder.geocode({ 'latLng': gmgLatLng }, function (results, status) {
                if (status == google.maps.GeocoderStatus.OK) {

                    if (results[0]) {
                        var marker = new google.maps.Marker({
                            position: gmgLatLng,
                            map: mapAdd
                        });
                        markers.push(marker);

                        google.maps.event.addListener(marker, 'click', function () {
                            infowindow.open(mapAdd, marker);
                        });
                        var infowindow = new google.maps.InfoWindow({
                            content: '<p>' + results[0].formatted_address + '</p>'
                        });
                        infowindow.open(mapAdd, marker)
                    }
                } else {
                    alert("Geocoder failed due to: " + status);
                }
            });
        }
        setTimeout(function () {
            resizingMap(mapAdd);
        }, 200);
    }

    function resizingMap(mapExist) {
        var center = mapExist.getCenter();
        google.maps.event.trigger(mapExist, "resize");
        mapExist.setCenter(center);
      
    }

    function setLegalLocation(locationMap, marker) {
        if ($(".legal-form #Latitude").length > 0) {
            $(".legal-form #Latitude").val(locationMap.k);
            $(".legal-form #Longitude").val(locationMap.D);
            resizingMap(mapLegalGeo);
        }

        geocoder = new google.maps.Geocoder();
        geocoder.geocode({ 'latLng': locationMap }, function (results, status) {
            if (status == google.maps.GeocoderStatus.OK) {
                if ($("#addr").length) {
                    $("#addr").text(results[0].formatted_address);
                }
                $(".legal-form #Address").val(results[0].formatted_address);
                if (results[0]) {
                    var marker = new google.maps.Marker({
                        position: locationMap,
                        map: mapLegalGeo
                    });
                    markers.push(marker);
                    $(".legal-form #Street").val(results[0].address_components[1].short_name);
                    $(".legal-form #Number").val(results[0].address_components[0].short_name);
                    google.maps.event.addListener(marker, 'click', function () {
                        infowindow.open(mapLegalGeo, marker);
                    });
                    var infowindow = new google.maps.InfoWindow({
                        content: '<p>' + results[0].formatted_address + '</p>'
                    });
                    infowindow.open(mapLegalGeo, marker)
                }
            } else {
                alert("Geocoder failed due to: " + status);
            }
        });
    }

    function setLocation(locationMap, marker) {
        if ($(".cur-form #Latitude").length > 0) {
            $(".cur-form #Latitude").val(locationMap.k);
            $(".cur-form #Longitude").val(locationMap.D);
        }
        else {
            $(".cur-form #GeoAddr_Latitude").val(locationMap.k);
            $(".cur-form #GeoAddr_Longitude").val(locationMap.D);
        }
        geocoder = new google.maps.Geocoder();
        geocoder.geocode({ 'latLng': locationMap }, function (results, status) {
            if (status == google.maps.GeocoderStatus.OK)
            {
                if ($("#addr").length) {
                    $("#addr").text(results[0].formatted_address);
                }
                $(".cur-form #Address").val(results[0].formatted_address);
                if (results[0]) {
                    var marker = new google.maps.Marker({
                        position: gmgLatLng,
                        map: mapAdd
                    });
                    markers.push(marker);

                    google.maps.event.addListener(marker, 'click', function () {
                        infowindow.open(mapAdd, marker);
                    });
                    var infowindow = new google.maps.InfoWindow({
                        content: '<p>' + results[0].formatted_address + '</p>'
                    });
                    infowindow.open(mapAdd, marker)
                }
            }
            else
            {
                alert("Geocoder failed due to: " + status);
            }
        });
    }

    function setAddLocation(locationMap, marker) {
        if ($(".cur-form #Latitude").length > 0) {
            $(".cur-form #Latitude").val(locationMap.k);
            $(".cur-form #Longitude").val(locationMap.D);
        }
        else {
            $(".cur-form #GeoAddr_Latitude").val(locationMap.k);
            $(".cur-form #GeoAddr_Longitude").val(locationMap.D);
        }
        geocoder = new google.maps.Geocoder();
        geocoder.geocode({ 'latLng': locationMap }, function (results, status) {
            if (status == google.maps.GeocoderStatus.OK) {
                if ($("#addr").length) {
                    $("#addr").text(results[0].formatted_address);
                }
                $(".cur-form #Address").val(results[0].formatted_address);
                if (results[0]) {
                    var marker = new google.maps.Marker({
                        position: locationMap,
                        map: mapAddGeo
                    });
                    markers.push(marker);

                    google.maps.event.addListener(marker, 'click', function () {
                        infowindow.open(mapAddGeo, marker);
                    });
                    var infowindow = new google.maps.InfoWindow({
                        content: '<p>' + results[0].formatted_address + '</p>'
                    });
                    infowindow.open(mapAddGeo, marker)
                }
            }
            else {
                alert("Geocoder failed due to: " + status);
            }
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