
'use strict';


(function (app) {

    app.controller('shipmentOverviewCtrl',
       ['$location', '$window', 'shipmentFactory',
           function ($location, $window, shipmentFactory) {
               var vm = this;
               var simple_map;
               vm.statusHistories = {};

               if ($("#simple-map").length) {
                   simple_map = new GMaps({
                       el: '#simple-map',
                       lat: -12.043333,
                       lng: -77.028333,
                       zoomControl: true,
                       zoomControlOpt: {
                           style: 'SMALL',
                           position: 'TOP_LEFT'
                       },
                       panControl: false,
                       streetViewControl: false,
                       mapTypeControl: false,
                       overviewMapControl: false
                   });
                   simple_map.addMarker({
                       lat: -12.042,
                       lng: -77.028333,
                       title: 'Marker with InfoWindow',
                       infoWindow: {
                           content: '<p>Here we are!</p>'
                       }
                   });
               }


           }]);


})(angular.module('newApp'));