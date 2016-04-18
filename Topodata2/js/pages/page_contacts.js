var ContactPage = function() {

    return {

        //Basic Map
        initMap: function() {
            var map;
            $(document).ready(function() {
                map = new GMaps({
                    div: '#map',
                    scrollwheel: false,
                    lat: 18.477197,
                    lng: -69.904489
                });

                var marker = map.addMarker({
                    lat: 18.477197,
                    lng: -69.904489,
                    title: 'Topodata'
                });
            });
        },

        //Panorama Map
        initPanorama: function() {
            var panorama;
            $(document).ready(function() {
                panorama = GMaps.createPanorama({
                    el: '#panorama',
                    lat: 18.477197,
                    lng: -69.904489
                });
            });
        }

    };
}();