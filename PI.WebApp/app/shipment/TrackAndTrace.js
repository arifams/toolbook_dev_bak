
'use strict';


(function (app) {

    app.controller('trackAndTraceCtrl',
       ['$location', '$window', 'shipmentFactory',
           function ($location, $window, shipmentFactory) {
               var vm = this;
               var simple_map;        
              
               var lat = 0;
               var lng = 0;


               vm.LoadTrackAndTracecDate = function () {
                   var shipmentMode = vm.shipmentMode;
                   var carrier = vm.carrier;
                   var trackingNo = vm.trackingNumber;
                   
                   shipmentFactory.getTeackAndTraceDetails(carrier, trackingNo).success(function (data) {
                       vm.trackingData = data;

                       for (var i = 0; i < vm.trackingData.history.items.length; i++) {
                           lat = vm.trackingData.history.items[i].location.geo.lat;
                           lng = vm.trackingData.history.items[i].location.geo.lng;

                       }


                       if ($("#simple-map").length) {
                           simple_map = new GMaps({
                               el: '#simple-map',
                               lat: lat,
                               lng: lng,
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
                               lat: lat,
                               lng: lng,
                               title: 'Marker with InfoWindow',
                               infoWindow: {
                                   content: '<p>Here we are!</p>'
                               }
                           });
                       }
                      
                   })
                   .error(function () {
                   })
               }              


              


           }]);


})(angular.module('newApp'));