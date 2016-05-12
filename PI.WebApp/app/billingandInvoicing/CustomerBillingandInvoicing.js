
'use strict';


(function (app) {

    app.controller('CustomerBillingandInvoicingCtrl',
       ['$location', '$window', 
           function ($location, $window) {
               var vm = this;
               vm.datePicker = {};
               vm.datePicker.date = { startDate: null, endDate: null };


           }]);


})(angular.module('newApp'));