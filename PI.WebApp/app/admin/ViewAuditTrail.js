'use strict';
(function (app) {

    app.controller('auditTrailCtrl',
                    ['$location', '$window', 'adminFactory', '$scope', 
           function ($location, $window, adminFactory,  $scope) {
               var vm = this;

               vm.SearchAuditTrail = function () {
                   var status = (status == undefined || status == 'All' || status == null || status == "") ? null : status;

                   adminFactory.GetAuditTrailsForCustomer(invoicenumber)
                        .success(
                               function (responce) {                                   
                                       vm.rowCollection = responce.content;                                   
                               }).error(function (error) {
                                   console.log("Error occurd while retrieving Audit records");
                               });

               }

           }]);


})(angular.module('newApp'));