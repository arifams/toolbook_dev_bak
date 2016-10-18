
'use strict';


(function (app) {

    app.controller('printLabelCtrl',
       ['$location', '$window', 'shipmentFactory', 'ngDialog', '$controller', '$scope', 'customBuilderFactory',
           function ($location, $window, shipmentFactory, ngDialog, $controller, $scope, customBuilderFactory) {
               var vm = this;
               vm.datePicker = {};
               vm.datePicker.date = { startDate: null, endDate: null };
               vm.loadingSymbole = true;

               vm.loadFilterToggle = function () {
                   customBuilderFactory.customFilterToggle();

               };

               vm.loadAllShipments = function (status) {
                   
                   vm.loadingSymbole = true;
                   var startDate = (vm.datePicker.date.startDate == null) ? null : vm.datePicker.date.startDate.toDate();
                   var endDate = (vm.datePicker.date.endDate == null) ? null : vm.datePicker.date.endDate.toDate();
                   var number = (vm.shipmentNumber == undefined) ? null : vm.shipmentNumber;

                   shipmentFactory.loadAllPendingShipments(startDate, endDate, number)
                        .success(
                               function (responce) {
                                   vm.loadingSymbole = false;
                                   vm.shipmentList = responce.content;
                               }).error(function (error) {
                                   vm.loadingSymbole = false;
                                   console.log("error occurd while retrieving shiments");
                               });

               }
               vm.loadAllShipments();


               vm.previewLabel = function (rowdetails) {

                   ngDialog.open({
                       scope: $scope,
                       template: '/app/shipment/PreviewLabelTemplate.html',
                       className: 'ngdialog-theme-default',
                       controller: $controller('previewLabelCtrl', {
                           $scope: $scope,
                           row: rowdetails
                       })

                   })

               }
         
           }]);


})(angular.module('newApp'));