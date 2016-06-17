'use strict';
(function (app) {

    app.controller('auditTrailCtrl',
                    ['$location', '$window', 'adminFactory', '$scope', 
           function ($location, $window, adminFactory,  $scope) {
               var vm = this;
               vm.datePicker = {};
               vm.datePicker.date = { startDate: null, endDate: null };

               vm.closeWindow = function () {
                   ngDialog.close()
               }

              
               vm.SearchAuditTrail = function () {
                   var status = (status == undefined || status == 'All' || status == null || status == "") ? null : status;

                   adminFactory.GetAuditTrailsForCustomer(invoicenumber)
                        .success(
                               function (responce) {
                                   if (from == 'fromDisputed') {
                                       vm.rowCollectionDisputed = responce.content;
                                   }
                                   else {
                                       vm.rowCollection = responce.content;
                                   }
                               }).error(function (error) {
                                   console.log("error occurd while retrieving shiments");
                               });

               }

               vm.loadAllInvoices();


           }]);


})(angular.module('newApp'));