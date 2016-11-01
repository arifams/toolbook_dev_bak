
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

               vm.loadAllShipments = function (start, number, tableState) {
                   
                   vm.loadingSymbole = true;
                   var startDate = (vm.datePicker.date.startDate == null) ? null : vm.datePicker.date.startDate.toDate();
                   var endDate = (vm.datePicker.date.endDate == null) ? null : vm.datePicker.date.endDate.toDate();
                   var shipmentNumber = (vm.shipmentNumber == undefined) ? null : vm.shipmentNumber;

                   var pageList = {
                       filterContent: {
                           startDate: startDate,
                           endDate: endDate,
                           shipmentNumber: shipmentNumber
                       },
                       currentPage: start,
                       pageSize: number
                   }

                   shipmentFactory.loadAllPendingShipments(pageList)
                        .success(
                               function (responce) {
                                   vm.loadingSymbole = false;
                                   vm.rowCollection = responce.content;
                                   tableState.pagination.numberOfPages = responce.totalPages;

                               }).error(function (error) {
                                   vm.loadingSymbole = false;
                                   console.log("error occurd while retrieving shiments");
                               });

               }
               //vm.loadAllShipments();


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

               var tableStateCopy;

               vm.callServerSearch = function (tableState) {
                   
                   if (tableState != undefined) {
                       tableStateCopy = tableState;
                   }
                   else {
                       tableState = tableStateCopy;
                       // tableState undefined mean, this come from directly button click event. so set pagination to zero.
                       tableState.pagination.start = 0;
                       tableState.pagination.numberOfPages = undefined;
                   }

                   var start = tableState.pagination.start;
                   var number = tableState.pagination.number;
                   var numberOfPages = tableState.pagination.numberOfPages;

                   vm.loadAllShipments(start, number, tableState);
               };

               vm.resetSearch = function (tableState) {

                   tableState = tableStateCopy;

                   // reset
                   tableState.pagination.start = 0;
                   tableState.pagination.numberOfPages = undefined;

                   var start = tableState.pagination.start;
                   var number = tableState.pagination.number;
                   var numberOfPages = tableState.pagination.numberOfPages;

                   vm.datePicker.date = { "startDate": null, "endDate": null };

                   vm.loadAllShipments(start, number, tableState);
               };
         
           }]);


})(angular.module('newApp'));