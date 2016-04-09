
'use strict';


(function (app) {

    app.controller('shipmentOverviewCtrl',
       ['$location', '$window', 'shipmentFactory','$rootScope',
           function ($location, $window, shipmentFactory, $rootScope) {
               var vm = this;
               var simple_map;
               vm.statusLocationItems = {};
               vm.locationHistory = {};
               var lat = 0;
               var lng = 0;
               vm.Consigneremail = '';

               vm.shipmentCode = $location.search().SHIPMENT_CODE;
               vm.trakingNo = $location.search().TRACKING_NO;
               vm.carrier = $location.search().CARRIER;
               vm.createdOn = $location.search().CREATED_ON;
               var shipmentId = '';
               var loadShipmentStatuses = function () {
                   debugger;
                   shipmentFactory.getLocationHistory(vm.shipment)
                   .success(function (data) {
                       vm.locationHistory = data;

                       for (var i = 0; i < vm.locationHistory.history.items.length; i++) {
                           lat = vm.locationHistory.history.items[i].location.geo.lat;
                           lng = vm.locationHistory.history.items[i].location.geo.lng;

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
               //get the current shipment details
               var loadShipmentInfo = function () {
                   debugger;
                   shipmentFactory.loadShipmentInfo(vm.shipmentCode)
                   .success(function (data) {
                       debugger;
                       vm.shipment = data;
                       shipmentId = vm.shipment.generalInformation.shipmentId;
                       vm.Consigneremail = vm.shipment.addressInformation.consigner.email;

                       vm.awb_URL = SISUrl+ "print_awb.asp?code_shipment=" + vm.shipmentCode + "&email=" + vm.Consigneremail;
                       vm.cmr_URL = SISUrl2 + "print_cmr.asp?code_shipment=" + vm.shipmentCode + "&userid=" + SISUser + "&password=" + SISPassword;
                       vm.shipmentLabel = data.generalInformation.shipmentLabelBLOBURL;
                       console.log(vm.shipmentLabel);
                       loadShipmentStatuses();

                   })
                   .error(function () {
                   })
               }

               loadShipmentInfo();





           }]);


})(angular.module('newApp'));