var zoom;
var centre;
var load = true;

/*** Elements that make up the popup.*/
var container = document.getElementById('popup');
var content = document.getElementById('popup-content');
var closer = document.getElementById('popup-closer');

/** Add a click handler to hide the popup.
 * @return {boolean} Don't follow the href.*/
closer.onclick = function () {
    overlay.setPosition(undefined);
    closer.blur();
    return false;
};

/*** Create an overlay to anchor the popup to the map.*/
var overlay = new ol.Overlay({
    element: container,
    autoPan: true,
    autoPanAnimation: {
        duration: 250
    }
});

function AfficherMarqueurs() {
    if (!load) {
        var dhDeb = new moment($('#dtpDebut')[0].value, 'DD/MM/YYYY HH:mm:SS');
        var dhFin = new moment($('#dtpFin')[0].value, 'DD/MM/YYYY HH:mm:SS');

        if (dhDeb.isValid() && dhFin.isValid() && idCamion !== "0") {
            GeolocCamionsHisto(dhDeb, dhFin);
        } else {
            GeolocCamionsMoment();
        }            
    }
}

var lat = [];
var lon = [];
var taille = 0.5;
var view = null;
var vectorSource = null;
var map = null;
$("#btnRechercher").click(function () {

    var geoBeginDateValue = $("#geoBeginDate").val();
    var startTimeValue = $("#beginTimePickerInput").val(); // Time in hh:mm format
 
    // Combine date and time values into a single string
    var startDateTimeString = geoBeginDateValue + "T" + (startTimeValue.length === 4 ? "0" + startTimeValue : startTimeValue) + ":00"; // Add seconds ":00" if needed

    // Create a JavaScript Date object from the combined string
    var beginDate = new Date(startDateTimeString);

    var geoEndDateValue = $("#geoEndDate").val();

    var endTimeValue = $("#endTimePickerInput").val(); // Time in hh:mm format

    // Combine date and time values into a single string
    var endDateTimeString = geoEndDateValue + "T" + (endTimeValue.length === 4 ? "0" + endTimeValue : endTimeValue) + ":00";

    // Convert the combined string to a Date object
    var endDateTime = new Date(endDateTimeString);

    // Check if the conversion was successful
    if (!isNaN(endDateTime.getTime())) {
        console.log("Converted DateTime:", endDateTime);
    } else {
        console.log("Invalid DateTime Format:", endDateTimeString);
    }

    // Create a JavaScript Date object from the combined string
    var endDate = new Date(endDateTimeString);

    var selectedTerminal = $("#SelectedTerminal").val();
    var selectedSSN = $("#SelectedSSN").val();

    // Create the data object to send in the AJAX request
    var requestData = {
        PointerName: selectedTerminal,
        SSN: selectedSSN,
        StartTime: beginDate,
        EndTime: endDate
    };

    $.ajax({
        type: "POST", // Change to "POST" since you are sending data in the request body
        url: "/api/worktime/Passagelocation", // Replace with your API endpoint URL
        contentType: "application/json", // Set content type to JSON
        data: JSON.stringify(requestData), // Stringify the data object
        success: function (data) {
            // Handle the successful response here
            console.log(data);
            // Update your page with the received data
            for (var i = 0; i < data.length; i++) {

                lat.push(data[i].latitude);
                lon.push(data[i].longitude);
            }

            $("#basicMap .ol-viewport").remove();
            initOSM();
        },
        error: function (error) {
            // Handle any errors that occurred during the AJAX request
            console.error(error);
        }
    });
});


function initOSM() {

    view = new ol.View({
        center: ol.proj.fromLonLat([lon[0], lat[0]]),
        zoom: 16,
        extent: ol.proj.transformExtent([162.2, -24.2, 168.7, -18.1], 'EPSG:4326', 'EPSG:3857')
    });

    map = new ol.Map({
        target: 'basicMap',
        overlays: [overlay],
        layers: [new ol.layer.Tile({source: new ol.source.OSM()})],
        view: view
    });

    var features = [];

    for (var i = 0; i < lon.length; i++) {
        var point = new ol.geom.Point(ol.proj.fromLonLat([lon[i], lat[i]]));
        var pointFeature = new ol.Feature(point);
        var iconStyleFunction = function (resolution) {
            return [new ol.style.Style({
                image: new ol.style.Icon({
                    anchor: [0.5, 1.0],
                    src: '/img/marker.pointeur.32x32.png'
                })
            })];
        };

        pointFeature.setStyle(iconStyleFunction);
        features.push(pointFeature);
    }

    var layer = new ol.layer.Vector({
        source: new ol.source.Vector({
            features: features
        })
    });
    map.addLayer(layer);

    var dragInteraction = new ol.interaction.Modify({
        features: new ol.Collection([pointFeature])
    });
    pointFeature.on('change', function () {
        var lonlat = ol.proj.transform(this.getGeometry().getCoordinates(), 'EPSG:3857', 'EPSG:4326');
        $("#geolocLati")[0].value = lonlat[1];
        $("#geolocLongi")[0].value = lonlat[0];

    }, pointFeature);
    map.addInteraction(dragInteraction);
    map.on('singleclick', function (evt) {
        pointFeature.getGeometry().setCoordinates(evt.coordinate);
    });
}

function marqueursGeo() {
            
    var iconC = function() {
        return [new ol.style.Style({
            image: new ol.style.Icon({
                anchor: [0.5, 1.0],
                src: '/img/marker.pointeur.32x32.png'
            })
        })];
    };

    if (vectorSource !== null) vectorSource.clear();
    vectorSource = new ol.source.Vector();

    var lati = parseFloat($("#geolocLati")[0].value.replace(',', '.'));
    var longi = parseFloat($("#geolocLongi")[0].value.replace(',', '.'));

    var point = new ol.geom.Point(ol.proj.fromLonLat([longi, lati]));
    var pointFeature = new ol.Feature({ geometry: point, name: "Employé" });

    var minMarkerx = 0, minMarkery = 0, maxMarkerx = 0, maxMarkery = 0;

    pointFeature.setStyle(iconC);


    vectorSource.addFeature(pointFeature);

    if (minMarkerx + minMarkery + maxMarkerx + maxMarkery === 0) {
        maxMarkerx = longi;
        minMarkerx = longi;
        maxMarkery = lati;
        minMarkery = lati;
    }
    else {
        if (longi >= maxMarkerx) maxMarkerx = longi;
        if (longi <= minMarkerx) minMarkerx = longi;
        if (lati >= maxMarkery) maxMarkery = lati;
        if (lati <= minMarkery) minMarkery = lati;
    }

    var layer = new ol.layer.Vector({
    source: vectorSource
    });
    map.addLayer(layer);

    var zoomextent = 0.01;
    var extent = ol.proj.transformExtent([minMarkerx - zoomextent, minMarkery - zoomextent, maxMarkerx + zoomextent, maxMarkery + zoomextent], 'EPSG:4326', 'EPSG:3857');
    map.getView().fit(extent, map.getSize());

    
    map.on("moveend", function (e) {
        zoom = map.getView().getZoom();
        centre = map.getView().getCenter();
    });

    if (centre !== undefined) map.getView().setCenter(centre);
    if (zoom !== undefined) map.getView().setZoom(zoom);

    load = false;
}

function wrapLon(value) {
    var worlds = Math.floor((value + 180) / 360);
    return value - worlds * 360;
}

function LongLat(long, lat) {
    return new ol.LonLat(long, lat)
        .transform(
            new ol.Projection("EPSG:4326"), // transform from WGS 1984
            map.getProjectionObject() // to Spherical Mercator Projection
        );
}