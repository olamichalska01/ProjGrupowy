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