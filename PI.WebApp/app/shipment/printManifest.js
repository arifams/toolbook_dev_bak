
'use strict';


(function (app) {   

    app.controller('printManifestCtrl',
       ['$location', '$window', 'shipmentFactory',
           function ($location, $window, shipmentFactory) {
               var vm = this;
               var SISURL = 'http://book.12send.com/taleus/admin-manifest.asp?'
               var userid = 'user@Mitrai.com';
               var password = 'Mitrai462';
               var career = null;
               vm.isPackageEnable = 'false';
               vm.isGeneralSubmit = false;
               vm.isSpecificSubmit = false;
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
               vm.showError = false;


               vm.BacktoSearch = function () {
                   vm.showEdit = true;
                   vm.showManifest = false;
               }

               vm.printManifest = function (divName) {
                   var printContents = document.getElementById(divName).innerHTML;
                   var popupWin = window.open('', '_blank', 'width=800,height=800');    
                   popupWin.document.open();
                   //popupWin.document.write('<html><head><link rel="stylesheet" type="text/css" href="style.css"/>   <link href="../global/css/style.css" rel="stylesheet">   <link href="../global/css/theme.css" rel="stylesheet"> <link href="../global/css/ui.css" rel="stylesheet"> </head><body onload="window.print()">' + printContents + '</body></html>');
                   popupWin.document.write('<html><head></head><body onload="window.print()">' + printContents + '</body></html>');
                   popupWin.document.close();
               }

               vm.clickGeneral = function () {
                   vm.specific = false;
                   vm.reference = '';
                   // Clear the errors.
                   vm.showError = false;
                   vm.showManifest = false;
                   vm.BacktoSearch();
               }

               vm.clickSpecific = function () {
                   vm.specific = true;
                   vm.Date = '';
                   // Clear the errors.
                   vm.showError = false;
                   vm.showManifest = false;
                   vm.BacktoSearch();
               }

               vm.GenerateManifest = function () {
                   vm.showError = false;
                   vm.showEdit = false;              
                   career=vm.carrier;
                   
                   if (career == undefined || career == null) {
                       career = '';
                   }
                   if (vm.Date == undefined || vm.Date == null) {
                       vm.Date = '';
                   }

                   if (vm.reference==null) {
                       vm.reference = '';
                   }

                   shipmentFactory.getAllshipmentsForManifest(vm.Date, career, vm.reference).success(
               function (responce) {
                   if (responce.length > 0) {                      

                    vm.shipmentInfo = responce;

                    vm.consignerfname=   vm.shipmentInfo[0].addressInformation.consigner.firstName;
                    vm.consignerlname = vm.shipmentInfo[0].addressInformation.consigner.lastName;
                    vm.consignerzipCode = vm.shipmentInfo[0].addressInformation.consigner.postalcode;
                    vm.consignerAddress1=   vm.shipmentInfo[0].addressInformation.consigner.address1;
                    vm.consignerAddress2 = vm.shipmentInfo[0].addressInformation.consigner.address2;                    
                    vm.consignernumber=   vm.shipmentInfo[0].addressInformation.consigner.number;
                    vm.consignerCity=   vm.shipmentInfo[0].addressInformation.consigner.city;
                    vm.consignerContact = vm.shipmentInfo[0].addressInformation.consigner.contactNumber;

                    vm.showManifest = true;
                       
                   }
                   else {
                       vm.showEdit = true;
                       vm.showError = true;
                   }
                 
               }).error(function (error) {
                   vm.showEdit = true;
                   vm.showError = true;

                   console.log("error occurd while retrieving customer details");
               });
               }

               vm.GenerateManifestFromGeneral = function () {
                   vm.isGeneralSubmit = true;
                   vm.GenerateManifest();
               }

               vm.GenerateManifestFromSpecific = function () {
                   vm.isSpecificSubmit = true;
                   vm.GenerateManifest();
               }

           }]);


})(angular.module('newApp'));