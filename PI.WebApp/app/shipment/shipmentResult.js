'use strict';

(function (app) {

    app.controller('shipmentResultCtrl', ['$window', function ($window) {

           var vm = this;

           vm.labelUrl = $window.localStorage.getItem('labelUrl');
           vm.invoiceUrl = $window.localStorage.getItem('invoiceURL');
           vm.shipmentReferenceName = $window.localStorage.getItem('sisErrorReferenceName');

           console.log('vm.labelUrl');
           console.log(vm.labelUrl);

           console.log('vm.invoiceUrl');
           console.log(vm.invoiceUrl);

           console.log('vm.shipmentReferenceName');
           console.log(vm.shipmentReferenceName);

           vm.openLabel = function (url) {
               window.open(url);
           }

       }]);

})(angular.module('newApp'));