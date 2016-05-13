
'use strict';


(function (app) {

    app.controller('BillingandInvoicingCtrl',
       ['$location', '$window', 'shipmentFactory', 'ngDialog', '$controller','$scope',
           function ($location, $window, shipmentFactory, ngDialog, $controller, $scope) {
               var vm = this;
               vm.datePicker = {};
               vm.datePicker.date = { startDate: null, endDate: null };
           

               vm.uploadInvoice = function () {

                   ngDialog.open({
                       scope: $scope,
                       template: '/app/admin/uploadInvoice.html',
                       className: 'ngdialog-theme-plain custom-width',
                       controller: $controller('uploadInvoiceCtrl', {
                           $scope: $scope,
                           
                       })                      

                   });
               }


           }]);


})(angular.module('newApp'));