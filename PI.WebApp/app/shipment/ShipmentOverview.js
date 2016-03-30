
'use strict';


(function (app) {

    app.controller('shipmentOverviewCtrl',
       ['$location', '$window', 'shipmentFactory',
           function ($location, $window, shipmentFactory) {
               var vm = this;
               var simple_map;
               vm.statusHistories = {};
               vm.shipmentCode = $location.search().SHIPMENT_CODE;
               vm.trakingNo = $location.search().TRACKING_NO;
               vm.carrier = $location.search().CARRIER;
               vm.createdOn = $location.search().CREATED_ON;
               var shipmentId = '';
               var loadShipmentStatuses = function () {
                   debugger;
                   shipmentFactory.loadShipmentStatusList(shipmentId)
                   .success(function (data) {
                       vm.statusHistories = data;
                   })
                   .error(function () {
                   })
               }
               //get the current shipment details
               var loadShipmentInfo = function () {
                   debugger;
                   shipmentFactory.loadShipmentInfo(vm.shipmentCode)
                   .success(function (data) {
                       vm.shipment = data;
                       shipmentId = vm.shipment.generalInformation.shipmentId;
                       loadShipmentStatuses();
                      
                   })
                   .error(function () {
                   })
               }
                
               loadShipmentInfo();
                   
               

              

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