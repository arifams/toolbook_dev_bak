
'use strict';


(function (app) {

    app.controller('shipmentOverviewCtrl',
       ['$location', '$window', 'shipmentFactory', '$rootScope',
           function ($location, $window, shipmentFactory, $rootScope) {
               var vm = this;
               var simple_map;
               vm.statusLocationItems = {};
               vm.locationHistory = {};
               var lat = 0;
               var lng = 0;
               vm.Consigneremail = '';
               vm.awb_URL = '';
               var totalPrintlength = 0;
               var totalLenght = 0;

               vm.currentRole = $window.localStorage.getItem('userRole');
               //vm.step = 3; //To Do - change this number with logic

               vm.openLabel = function (url) {
                   window.open(url);
               }

               vm.openLabelList = function (url) {
                   for (var i = 0; i < url.length; i++) {
                       window.open(url[i]);
                   }
               }

               vm.shipmentCode = $location.search().SHIPMENT_CODE; 
               vm.sourceShipmentId = $location.search().SHIPMENT_ID;
               vm.trakingNo = $location.search().TRACKING_NO;
               vm.carrier = $location.search().CARRIER;
               vm.createdOn = $location.search().CREATED_ON;
               vm.source = $location.search().SOURCE;

               if (vm.source == "Tracking") {
                   vm.isAirwayBillActive = false;
                   vm.isComInvoiceActive = false;
                   vm.isShipDocumentsActive = false;
               }
               else if (vm.source == "AWB") {
                   vm.isAirwayBillActive = true;
                   vm.isComInvoiceActive = false
                   vm.isShipDocumentsActive = false;
               }
               else if (vm.source == "ComInvoice") {
                   vm.isAirwayBillActive = false;
                   vm.isComInvoiceActive = true;
                   vm.isShipDocumentsActive = false;
               }
               else if (vm.source == "Documents") {
                   vm.isAirwayBillActive = false;
                   vm.isComInvoiceActive = false
                   vm.isShipDocumentsActive = true;
               }

               var shipmentId = '';

               var loadShipmentStatuses = function () {

                   shipmentFactory.getLocationHistory(vm.shipment)
                   .success(function (data) {

                       vm.locationHistory = data;

                       if (vm.locationHistory.info != null) {

                           vm.step = vm.locationHistory.info.status;
                           //  vm.step = 2;
                       }
                       if (vm.locationHistory.history != null && vm.locationHistory.history.items.length > 0) {
                           for (var i = 0; i < vm.locationHistory.history.items.length; i++) {
                               lat = vm.locationHistory.history.items[i].location.geo.lat;
                               lng = vm.locationHistory.history.items[i].location.geo.lng;

                           }
                       }
                       else {
                           if (vm.locationHistory.info.system.consignor.geo != null) {
                               lat = vm.locationHistory.info.system.consignor.geo.lat;
                               lng = vm.locationHistory.info.system.consignor.geo.lng;
                           }

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

               //
               vm.print = function (divId) {

                   var printContents = document.getElementById(divId).innerHTML;
                   var popupWin = window.open('', '_blank', 'width=800,height=800');
                   popupWin.document.open();
                   popupWin.document.write('<html><head></head><body onload="window.print()">' + printContents + '</body></html>');
                   popupWin.document.close();


               }

               //get the current shipment details
               var loadShipmentInfo = function () {

                   shipmentFactory.loadShipmentInfo(vm.shipmentCode, vm.sourceShipmentId)
                   .success(function (data) {

                       vm.shipment = data;
                       shipmentId = vm.shipment.generalInformation.shipmentId;
                       vm.shipmentLabel = data.generalInformation.shipmentLabelBLOBURLList;

                       vm.Consigneremail = vm.shipment.addressInformation.consigner.email;

                       var length = Calclengthfunction(vm.shipment.packageDetails.productIngredients);
                       totalLenght = length + 900;



                       var sisUrl = "";
                       if (vm.shipment.carrierInformation.countryCodeByTarrifText == "NL")
                           sisUrl = SISUrlNL;
                       else
                           sisUrl = SISUrlUS;

                       vm.awb_URL = sisUrl + "print_awb.asp?code_shipment=" + vm.shipmentCode + "&email=" + vm.Consigneremail;
                       vm.cmr_URL = sisUrl + "print_cmr.asp?code_shipment=" + vm.shipmentCode + "&userid=" + SISUser + "&password=" + SISPassword;


                       $('<iframe src="' + vm.awb_URL + '" frameborder="0" scrolling="no" id="myFrame" height="' + totalLenght + '" width="700"></iframe>').appendTo('.awb');
                       $('<iframe src="' + vm.cmr_URL + '" frameborder="0" scrolling="no" id="myFrame" height="4500" width="800"></iframe>').appendTo('.cmr');
                       //  console.log(vm.shipmentLabel);
                       loadShipmentStatuses();


                   })
                   .error(function () {
                   })
               }

               var Calclengthfunction = function (products) {
                   var totalRows = 0;

                   for (var i = 0; i < products.length; i++) {

                       totalRows = totalRows + products[i].quantity;
                   }

                   return totalRows * 12;
               }


               loadShipmentInfo();

           }]);


})(angular.module('newApp'));