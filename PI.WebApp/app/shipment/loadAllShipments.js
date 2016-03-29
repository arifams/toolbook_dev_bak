'use strict';
(function (app) {

    app.controller('loadShipmentsCtrl', ['$scope', '$location', '$window', 'shipmentFactory',
                       function ($scope, $location, $window, shipmentFactory) {

                           var vm = this;
                           vm.datePicker = {};
                           vm.datePicker.date = { startDate: null, endDate: null };

                           vm.loadAllShipments = function (status) {
                               debugger;
                               var status = (status == undefined || status == 'All' || status == null || status == "") ? null : status;
                               var startDate = (vm.datePicker.date.startDate == null) ? null : vm.datePicker.date.startDate.toDate();
                               var endDate = (vm.datePicker.date.endDate == null) ? null : vm.datePicker.date.endDate.toDate();
                               var number = (vm.shipmentNumber == undefined) ? null : vm.shipmentNumber;
                               var source = (vm.originCityCountry == undefined) ? null : vm.originCityCountry;
                               var destination = (vm.desCityCountry == undefined) ? null : vm.desCityCountry;

                               shipmentFactory.loadAllShipments(status, startDate, endDate, number, source, destination)
                                    .success(
                                           function (responce) {
                                               debugger;
                                               vm.shipmentList = responce.content;
                                           }).error(function (error) {
                                               console.log("error occurd while retrieving shiments");
                                           });

                           }

                           vm.loadShipmentsByStatus = function (status) {
                               vm.loadAllShipments(status);
                           }

                           vm.loadAllShipments();



                       }])
})(angular.module('newApp'));
