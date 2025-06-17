(function () {
    window.initMap = function () {
        var mapDiv = document.getElementById('map');
        if (!mapDiv) return;
        var latInput = document.getElementById('Latitude');
        var lonInput = document.getElementById('Longitude');
        var lat = parseFloat(latInput.value) || 52.2297;
        var lon = parseFloat(lonInput.value) || 21.0122;
        var map = new google.maps.Map(mapDiv, {
            center: { lat: lat, lng: lon },
            zoom: 12
        });
        var marker = new google.maps.Marker({
            position: { lat: lat, lng: lon },
            map: map,
            draggable: true
        });
        google.maps.event.addListener(map, 'click', function (e) {
            marker.setPosition(e.latLng);
            latInput.value = e.latLng.lat();
            lonInput.value = e.latLng.lng();
        });
        google.maps.event.addListener(marker, 'dragend', function (e) {
            latInput.value = e.latLng.lat();
            lonInput.value = e.latLng.lng();
        });
    };
})();