mapboxgl.accessToken = 'pk.eyJ1IjoibGVvbmFyZHoxIiwiYSI6ImNsazM0bDdvYTBua3IzZHAybm02azNwM3gifQ.tiaCEyPec5_jLghxMVQk2g';

// Initialize the map
var map = new mapboxgl.Map({
    container: 'map',
    style: 'mapbox://styles/mapbox/streets-v11',
    center: [20.7138486, 42.6221658], //Default latitude & longitude
    zoom: 9
});

// Add a custom marker image
map.loadImage('/images/MapMarker/custom_marker.png', function (error, image) {
    if (error) throw error;

    map.addImage('custom-marker', image);

    // Add a marker on map click and update the fields
    map.on('click', function (e) {
        // Clear existing marker, if any
        if (map.getLayer('marker')) {
            map.removeLayer('marker');
            map.removeSource('marker');
        }

        // Add a marker at the clicked location
        map.addLayer({
            id: 'marker',
            type: 'symbol',
            source: {
                type: 'geojson',
                data: {
                    type: 'Feature',
                    geometry: {
                        type: 'Point',
                        coordinates: [e.lngLat.lng, e.lngLat.lat]
                    }
                }
            },
            layout: {
                'icon-image': 'custom-marker',
                'icon-size': 0.8
            }
        });

        // Update the latitude, longitude, rounds the decimal number to (6)
        document.getElementById('Latitude').value = e.lngLat.lat.toFixed(6);
        document.getElementById('Longitude').value = e.lngLat.lng.toFixed(6);

        // Reverse geocode the coordinates to get the street name, city, country
        reverseGeocode(e.lngLat.lng, e.lngLat.lat)
            .then(function (result) {
                document.getElementById('StreetName').value = result.streetName;
                document.getElementById('City').value = result.city;
                document.getElementById('State').value = result.country;
            });
    });
});

function reverseGeocode(longitude, latitude) {
    return new Promise(function (resolve, reject) {
        var url = 'https://api.mapbox.com/geocoding/v5/mapbox.places/' + longitude + ',' + latitude + '.json?access_token=' + mapboxgl.accessToken;

        fetch(url)
            .then(function (response) {
                return response.json();
            })
            .then(function (data) {
                var features = data.features;
                var address = {
                    streetName: '',
                    city: '',
                    country: ''
                };
                // Find the street name, city, country in address
                for (var i = 0; i < features.length; i++) {
                    //console.log(features[i]);
                    var componentTypes = features[i].place_type;

                    if (componentTypes.includes('address')) {
                        address.streetName = features[i].text;
                    } else if (componentTypes.includes('place')) {
                        if (!address.streetName) {
                            address.streetName = null;
                        }
                        address.city = features[i].text;
                    } else if (componentTypes.includes('country')) {
                        address.country = features[i].text;
                    }
                }

                resolve(address);
            })
            .catch(function (error) {
                reject(error);
            });
    });
}

function ValidateMap() {
    if (!map.getLayer('marker')) {
        Swal.fire({
            icon: 'error',
            title: 'Oops...',
            text: 'Please select a location on the map!',
        });
        return false;
    }

    return true;
}
