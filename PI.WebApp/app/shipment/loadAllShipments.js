'use strict';
(function (app) {

    app.controller('loadShipmentsCtrl', ['$scope', '$location', '$window', 'shipmentFactory',
                       function ($scope, $location, $window, shipmentFactory) {

                           var vm = this;

                           var loadAllShipments = function () {
                               shipmentFactory.loadAllShipments()
                                    .success(
                                           function (responce) {
                                               vm.shipmentList = responce;
                                           }).error(function (error) {
                                               console.log("error occurd while retrieving shiments");
                                           });

                           }

                          loadAllShipments();


                       }])
})(angular.module('newApp'));
