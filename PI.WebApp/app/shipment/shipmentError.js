
'use strict';


(function (app) {

    app.controller('shipmentErrorCtrl',
       ['$location', '$window',
           function ($location, $window) {
               var vm = this;
              
               
               vm.errorMessage = $location.search().message;

           }]);

   app.config(function($locationProvider) {
       $locationProvider.html5Mode({
           enabled: true,
           requireBase: false
       });
      
    });
})(angular.module('shipmentError',[]));