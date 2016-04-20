'use strict';
(function (app) {

    app.controller('loadShipmentsCtrl', ['$scope', '$location', '$window', 'shipmentFactory',
                       function ($scope, $location, $window, shipmentFactory) {

                           var vm = this;
                           vm.statusButton = 'All';
                           vm.datePicker = {};
                           vm.datePicker.date = { startDate: null, endDate: null };
                           vm.itemsByPage = 25; // Set page size    // 25
                           vm.rowCollection = [];

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
                                               vm.rowCollection = responce.content;
                                           }).error(function (error) {
                                               console.log("error occurd while retrieving shiments");
                                           });

                           }

                           vm.loadShipmentsByStatus = function (status) {

                               vm.statusButton = status;
                               vm.loadAllShipments(status);
                           }
                           
                           // Delete row.
                           vm.deleteById = function (row) {

                               var r = confirm("Are you sure you want to delete this shipment?");
                               if (r == true) {
                                   shipmentFactory.deleteShipment(row)
                                   .success(function (response) {

                                       //if (response == 1) {
                                       //    //var index = vm.rowCollection.indexOf(row);
                                       //    //if (index !== -1) {
                                       //    //    vm.rowCollection.splice(index, 1);
                                       //    //}
                                       //}
                                   })
                                   .error(function () {
                                       debugger;
                                   });
                               }
                           };

                           vm.loadAllShipments();



                       }])
})(angular.module('newApp'));
