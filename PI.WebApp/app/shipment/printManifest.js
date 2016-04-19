﻿
'use strict';


(function (app) {   

    app.controller('printManifestCtrl',
       ['$location', '$window', 'shipmentFactory',
           function ($location, $window, shipmentFactory) {
               var vm = this;
               var SISURL = 'http://book.12send.com/taleus/admin-manifest.asp?'
               var userid = 'user@Mitrai.com';
               var password = 'Mitrai462';
               vm.isPackageEnable = 'false';
               vm.isSubmit = false;
               vm.shipmentInfo = {};
               vm.consignerfname = "";
               vm.consignerlname = "";
               vm.consignerAddress1 = "";
               vm.consignerAddress2 = "";
               vm.consignernumber = "";
               vm.consignerCity = "";
               vm.consignerContact = "";
               vm.showManifest = false;
               vm.showEdit = true;
               vm.reference = "";
               vm.specific = false;
               vm.referencedropdown = 'reference';


               vm.BacktoSearch = function () {
                   vm.showEdit = true;
                   vm.showManifest = false;
               }

               vm.printManifest = function (divName) {
                   var printContents = document.getElementById(divName).innerHTML;
                   var popupWin = window.open('', '_blank', 'width=800,height=800');    
                   popupWin.document.open();
                   popupWin.document.write('<html><head><link rel="stylesheet" type="text/css" href="style.css"/>   <link href="../global/css/style.css" rel="stylesheet">   <link href="../global/css/theme.css" rel="stylesheet"> <link href="../global/css/ui.css" rel="stylesheet"> </head><body onload="window.print()">' + printContents + '</body></html>');
                   popupWin.document.close();
               }

               vm.clickGeneral = function () {
                   vm.specific = false;
                   vm.reference = '';
               }

               vm.clickSpecific = function () {
                   vm.specific = true;
                   vm.Date = '';
                   career = null;
               }



               vm.GenerateManifest = function () {
                   vm.showEdit = false;
                   vm.isSubmit = true;                   
                   var career=vm.carrier;

                   if (vm.Date == null && career==null) {
                       vm.Date = '';
                       career = '';
                   }

                   if (vm.reference==null) {
                       vm.reference = '';
                   }

                   shipmentFactory.getAllshipmentsForManifest(vm.Date, career, vm.reference).success(
               function (responce) {
                   if (responce.length > 0) {                      

                    vm.shipmentInfo = responce;

                    vm.consignerfname=   vm.shipmentInfo[0].addressInformation.consigner.firstName;
                    vm.consignerlname=   vm.shipmentInfo[0].addressInformation.consigner.lastName;
                    vm.consignerAddress1=   vm.shipmentInfo[0].addressInformation.consigner.address1;
                    vm.consignerAddress2=   vm.shipmentInfo[0].addressInformation.consigner.address2;
                    vm.consignernumber=   vm.shipmentInfo[0].addressInformation.consigner.number;
                    vm.consignerCity=   vm.shipmentInfo[0].addressInformation.consigner.city;
                    vm.consignerContact = vm.shipmentInfo[0].addressInformation.consigner.contactNumber;

                    vm.showManifest = true;
                       
                   }
                 
               }).error(function (error) {
                   console.log("error occurd while retrieving customer details");
               });
               }

           }]);


})(angular.module('newApp'));