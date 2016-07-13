'use strict';

(function (app) {

    app.controller('accountSetupCtrl',
       ['$scope',
           function ($scope) {
               var vm = this;

               vm.hideaddressDetails = false;

               vm.func = function () {
                   vm.hideaddressDetails = true;
               };
               vm.func2 = function () {
                   vm.hideaddressDetails = false;
               };
           }]);


})(angular.module('newApp'));