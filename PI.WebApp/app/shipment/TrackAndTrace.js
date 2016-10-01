
'use strict';


(function (app) {

    app.controller('trackAndTraceCtrl',
       ['$location', '$window', 'shipmentFactory',
           function ($location, $window, shipmentFactory) {
               var vm = this;
               var simple_map;        
              
               vm.missingFields = false;
               var lat = 0;
               var lng = 0;
               vm.shipmentMode = 'EXPRESS';
               vm.searching = false;
               vm.trackingNotFound = true;
               vm.onload = true;
             

               vm.LoadTrackAndTracecDate = function () {
                   
                   vm.searching = true;
                   var shipmentMode = vm.shipmentMode;
                   var carrier = vm.carrier;
                   var trackingNo = vm.trackingNumber;
                   if (shipmentMode == null || carrier == null || trackingNo==null) {
                       vm.missingFields = true;
                       vm.searching = false;
                      
                   } else {
                       vm.missingFields = false;
                       shipmentFactory.getTeackAndTraceDetails(carrier, trackingNo).success(function (data) {
                           vm.trackingData = data;
                           if (vm.trackingData != null && vm.trackingData.history!=null) {

                               //if (vm.trackingData.history.items.length > 0) {
                               //    vm.trackingNotFound = false;
                               //    document.getElementById("simple-map").style.visibility = "visible";
                               //    vm.onload = false;
                               //    for (var i = 0; i < vm.trackingData.history.items.length; i++) {
                               //        lat = vm.trackingData.history.items[i].location.geo.lat;
                               //        lng = vm.trackingData.history.items[i].location.geo.lng;

                               //    }
                                 

                                  
                               //} else {
                               //    if (vm.trackingData.info.system.consignor.geo != null) {
                               //        document.getElementById("simple-map").style.visibility = "visible";
                               //        vm.trackingNotFound = false;
                               //        lat = vm.trackingData.info.system.consignor.geo.lat;
                               //        lng = vm.trackingData.info.system.consignor.geo.lng;
                                       
                               //    } else {
                               //        document.getElementById("simple-map").style.visibility = "hidden";
                               //        vm.trackingNotFound = true;
                               //        vm.onload = false;
                               //    }

                               
                               //}

                               vm.searching = false;
                           } else {
                               document.getElementById("simple-map").style.visibility = "hidden";
                               vm.trackingNotFound = true;
                               vm.searching = false;
                               vm.onload = false;
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
                   
                   
               }              


              


           }]);


})(angular.module('newApp'));