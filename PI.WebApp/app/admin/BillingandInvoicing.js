
'use strict';


(function (app) {

    app.controller('BillingandInvoicingCtrl',
       ['$location', '$window', 'adminFactory', 'ngDialog', '$controller', '$scope',
           function ($location, $window, adminFactory, ngDialog, $controller, $scope) {
               var vm = this;
               vm.datePicker = {};
               vm.datePicker.date = { startDate: null, endDate: null };
           
               vm.closeWindow = function () {
                   ngDialog.close()
               }

               vm.uploadInvoice = function (fromMethod, invoiceId) {
                   debugger;
                   ngDialog.open({
                       scope: $scope,
                       template: '/app/admin/uploadInvoice.html',
                       className: 'ngdialog-theme-plain custom-width',
                       controller: $controller('uploadInvoiceCtrl', {
                           $scope: $scope,
                           fromMethod: fromMethod,
                           invoiceId: invoiceId
                       })                     
                   });
               }


               vm.loadAllInvoices = function (status, from) {
                   debugger;
                   var status = (status == undefined || status == 'All' || status == null || status == "") ? null : status;
                   var startDate = (vm.datePicker.date.startDate == null) ? null : vm.datePicker.date.startDate.toDate();
                   var endDate = (vm.datePicker.date.endDate == null) ? null : vm.datePicker.date.endDate.toDate();
                   var shipmentnumber = (vm.shipmentNumber == undefined) ? null : vm.shipmentNumber;
                   var businessowner = (vm.businessOwner == undefined) ? null : vm.businessOwner;
                   var invoicenumber = (vm.invoiceNumber == undefined) ? null : vm.invoiceNumber;

                   adminFactory.loadAllInvoices(status, startDate, endDate, shipmentnumber, businessowner, invoicenumber)
                        .success(
                               function (responce) {
                                   debugger;                                   
                                   if (from == 'fromDisputed')
                                   {
                                       vm.rowCollectionDisputed  = responce.content;
                                   }
                                   else {
                                       vm.rowCollection = responce.content;
                                   }
                               }).error(function (error) {
                                   console.log("error occurd while retrieving shiments");
                               });

               }

               vm.loadDisputedInvoices = function () {
                   vm.loadAllInvoices('Disputed', 'fromDisputed');
               }

               vm.loadInvoicesByStatus = function (status) {

                   vm.statusButton = status;
                   vm.loadAllInvoices(status);
               }

               vm.loadAllInvoices();


           }]);


})(angular.module('newApp'));