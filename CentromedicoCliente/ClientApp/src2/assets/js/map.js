var locations = [
    ['La Romana', 18.4296925, -68.9656023, 17],
    ['Santo Domingo', 18.4634403, -69.9499557, 15]
];
const image =
    "./assets/icons/hospital.svg";
var map, marker, i;

function initMap() {
console.log(map )    
    if (!map) {
      alert("google")

        map = new google.maps.Map(document.getElementById('map'), {
            zoom: 10,
            center: new google.maps.LatLng(18.5208959, -69.5155716),
            mapTypeId: google.maps.MapTypeId.ROADMAP,
            fullscreenControl: false,
        });

        var infowindow = new google.maps.InfoWindow();

        for (i = 0; i < locations.length; i++) {
            marker = new google.maps.Marker({
                position: new google.maps.LatLng(locations[i][1], locations[i][2]),
                map: map,
                icon: image

            });

            google.maps.event.addListener(marker, 'click', (function (marker, i) {
                return function () {
                    infowindow.setContent(locations[i][0]);
                    infowindow.open(map, marker);
                }
            })(marker, i));
        }
    }
}


function setSD() {
    var myPlace = { lat: 18.4634403, lng: -69.9499557 };
    map.setZoom(15);
    map.setCenter(myPlace);
}

function setRomana() {
    var myPlace = { lat: 18.4296925, lng: -68.9656023 };
    map.setZoom(15);
    map.setCenter(myPlace);
}

function centerMap() {
    var myPlace = { lat: 18.5208959, lng: -69.5155716 };
    map.setZoom(10);
    map.setCenter(myPlace);
}