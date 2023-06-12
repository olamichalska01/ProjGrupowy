export function load_map(raw) {
/*    console.log(JSON.parse(String(raw)));
*/    let map = L.map('map');
    map.setZoom(8);
    map.setView({ lon: 10, lat: 10 });
    L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', { maxZoom: 19 }).addTo(map);
    var geojson_layer = L.geoJSON().addTo(map);
    var geojson_data = JSON.parse(String(raw));
    for (var geojson_item of geojson_data) {
        map.setView({ lon: geojson_item.geometry.coordinates[0], lat: geojson_item.geometry.coordinates[1] }, 16);
        geojson_layer.addData(geojson_item);
        var marker = new L.marker(
            [geojson_item.geometry.coordinates[1],
            geojson_item.geometry.coordinates[0]],
            { opacity: 0.01 }
        );
        marker.bindTooltip(geojson_item.properties.name,
            {
                permanent: true,
                className: "my-label",
                offset: [0, 0]
            }
        );
        marker.addTo(map);
    }

    return "";
}

export function initAddEventMapView(dotnetHelper) {
    const map = L.map('map', { center: [52.237, 21.017], zoom: 19 });

    const locateOptions = {
        watch: false,
        setView: true,
        maxZoom: 19
    };

    map.setZoom(8);
    L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', { maxZoom: 19 }).addTo(map);
    map.locate(locateOptions)
        .on('locationfound', (e) => {
            map.flyTo(e.latlng, map.getZoom());
        })
        .on('locationerror', (e) => {
            console.log('Error while loading user location.');
        })

    var marker = null;
    map.on('click', (e) => {
        if (!marker) {
            marker = L.marker(e.latlng).addTo(map);
        } else {
            marker.setLatLng(e.latlng);
        }
        dotnetHelper.invokeMethodAsync("EventPositionChanged", e.latlng.lat, e.latlng.lng);
    })
}
