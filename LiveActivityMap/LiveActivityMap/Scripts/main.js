var _ge;
var _existingLabels = {};

var Types =
{
    Order: 0,
    Shipment: 1,
    View: 2
};

function initEarth() {
    google.earth.createInstance('map3d', initSuccess, initFail);
}

function initSuccess(ge) {
    _ge = ge;
    _ge.getWindow().setVisibility(true);
    _ge.getOptions().setFlyToSpeed(_flyToSpeed);

    initSignalR();

    addPoweredByIndicator();
}

function initFail(errorCode) {
}

function addPoweredByIndicator() {
	var geometry = _ge.createPoint("BASpoint");
	geometry.setLatLngAlt(30.39600, -97.7081, 0.0);

	var icon = _ge.createIcon("BASicon");
	//icon.setHref("http://maps.google.com/mapfiles/kml/shapes/capital_big.png");
	icon.setHref("");

	var style = _ge.createStyle("BASstyle");
	style.getIconStyle().setIcon(icon);
	style.getIconStyle().setScale(2);

	var mark = _ge.createPlacemark("BAS");
	mark.setGeometry(geometry);
	mark.setStyleSelector(style);
	mark.setVisibility(true);

	_ge.getFeatures().appendChild(mark);

	var balloon = _ge.createHtmlStringBalloon('');
	balloon.setFeature(mark);
	balloon.setContentString('<img src="/images/PoweredByTheLab.png" />');
	balloon.setBackgroundColor('#000000');
	_ge.setBalloon(balloon);
	
	// Move the camera.
	var la = _ge.createLookAt('');
	la.set(30.396, -97.7081, 10000, _ge.ALTITUDE_RELATIVE_TO_GROUND, 0, 0, 0);
	_ge.getView().setAbstractView(la);
}

function addTemporaryIndicator(event) {
    var id;

    id = 'point' + event.id;
    var geometry = _ge.getElementById(id);
    if (geometry == null) {
        geometry = _ge.createPoint(id);
    }

    geometry.setLatLngAlt(event.latitude, event.longitude, 0.0);

    id = 'icon' + event.id;
    var icon = _ge.getElementById(id);
    if (icon == null) {
        icon = _ge.createIcon(id);
    }

    if (event.iconUrl) {
        icon.setHref(event.iconUrl);
    } else {
        icon.setHref(getIconUrlForEventType(event.type));
    }
    
    var scale = getScaleForEventSize(event.size);

    id = 'style' + event.id;
    var style = _ge.getElementById(id);
    if (style == null) {
        style = _ge.createStyle(id);
    }

    style.getIconStyle().setIcon(icon);
    style.getIconStyle().setScale(scale);

    id = 'placemark' + event.id;
    var placemark = _ge.getElementById(id);
    if (placemark == null) {
        placemark = _ge.createPlacemark(id);
    }



    //don't place duplicate labels
    var label = '' + event.latitude + '|' + event.longitude + '|' + event.name;
    if (!_existingLabels[label])
        placemark.setName(event.name);
    _existingLabels[label] = (new Date()).getTime();
    
    placemark.setGeometry(geometry);

    placemark.setStyleSelector(style);
    placemark.setVisibility(true);
    placemark.setOpacity(1.0);
    _ge.getFeatures().appendChild(placemark);

    var audioElement = getAudioElementForEventType(event.type);
    audioElement.play();
    
    setTimeout(function () { removePlacemark(id, label); }, _removeTimer);
}

function removePlacemark(id, label) {
	var placemark = _ge.getElementById(id);
	_ge.getFeatures().removeChild(placemark);
    
	if (_existingLabels[label]) {
	    var now = (new Date()).getTime();
	    var then = new Date(_existingLabels[label]);
	    if (now - then >= _removeTimer) //it's been a while since we last showed this label, we should start showing it again 
	        _existingLabels[label] = 0;
    }
}

function getIconUrlForEventType(eventType) {
    var filename = '';
    switch(eventType) {
        case Types.Order:
            filename = "order.png";
            break;
        case Types.Shipment:
            filename = "shipment.png";
            break;
        case Types.View:
            filename = "view.png";
            break;
        default:
            filename = "";
    }

    return siteUrl + "images/" + filename;
}

// TODO: Get the rest of the sounds
function getAudioElementForEventType(eventType) {
    var elem;
    switch(eventType) {
        case Types.Order:
            elem = document.getElementById("orderAudio");
            break;
        case Types.Shipment:
            elem = document.getElementById("shipmentAudio");
            break;
        case Types.View:
            elem = document.getElementById("viewAudio");
            break;
        default:
            elem = null;
            break;
    }
    return elem;
}

function getEventTypeByName(name) {
    switch(name) {
        case "sale":
            return Types.Order;
        case "ship":
            return Types.Shipment;
    }
    return Types.View;
}

function getScaleForEventSize(eventSize) {
    return eventSize / 50;
}

function panAndZoom(event) {
	var lookAt = _ge.getView().copyAsLookAt(_ge.ALTITUDE_RELATIVE_TO_GROUND);
	lookAt.setHeading(0);
    zoomOut(lookAt);
    setTimeout(function () { panTo(event.latitude, event.longitude, lookAt); }, 2000);
    setTimeout(function () { addTemporaryIndicator(event); }, 3500);
    setTimeout(function () { zoomIn(lookAt); }, 4000);
}

function zoomOut(lookAt) {
    lookAt.setRange(_zoomOutRange);
    _ge.getView().setAbstractView(lookAt);
}

function zoomIn(lookAt) {
    lookAt.setRange(_zoomInRange);
    _ge.getView().setAbstractView(lookAt);
}

function panTo(latitude, longitude, lookAt) {
    lookAt.setLatitude(latitude);
    lookAt.setLongitude(longitude);

    _ge.getView().setAbstractView(lookAt);
}

function initSignalR() {
    var liveActivity = $.connection.liveActivity;

    liveActivity.client.send = function (id, lat, lng, label, iconUrl, size, type) {
        $('#activity').prepend('<li>' + id + ' ' + lat + ' ' + lng + ' ' +  label + ' ' + iconUrl + ' ' + size + ' ' + type + '</li>');
        
        var event = {
            id: id,
            latitude: lat,
            longitude: lng,
            name: label,
            iconUrl: iconUrl,
            size: size,
            type: getEventTypeByName(type)
        };
        panAndZoom(event);
        
        while ($('#activity li').length > 10) {
            $('#activity li').last().remove();
        }
    };

    $.connection.hub.start();

    $.connection.hub.disconnected(function () {
		setTimeout(function() {
			$.connection.hub.start();
		}, 5000);
	});

}


function init() {
    google.load("earth", "1", { "other_params": "sensor=false" });
    google.setOnLoadCallback(initEarth);
}

init();