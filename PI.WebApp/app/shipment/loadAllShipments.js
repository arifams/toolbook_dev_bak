'use strict';
(function (app) {

    app.controller('loadShipmentsCtrl', ['$scope', '$location', '$window', 'shipmentFactory','$rootScope','$route',
                       function ($scope, $location, $window, shipmentFactory, $rootScope, $route) {

                           var vm = this;
                           vm.statusButton = 'All';
                           vm.datePicker = {};
                           vm.datePicker.date = { startDate: null, endDate: null };
                           vm.itemsByPage = 25; // Set page size    // 25
                           vm.rowCollection = [];

                           vm.loadAllShipments = function (status) {
                               
                               var status = (status == undefined || status == 'All' || status == null || status == "") ? null : status;
                               var startDate = (vm.datePicker.date.startDate == null) ? null : vm.datePicker.date.startDate.toDate();
                               var endDate = (vm.datePicker.date.endDate == null) ? null : vm.datePicker.date.endDate.toDate();
                               var number = (vm.shipmentNumber == undefined) ? null : vm.shipmentNumber;
                               var source = (vm.originCityCountry == undefined) ? null : vm.originCityCountry;
                               var destination = (vm.desCityCountry == undefined) ? null : vm.desCityCountry;

                               shipmentFactory.loadAllShipments(status, startDate, endDate, number, source, destination)
                                    .success(
                                           function (responce) {
                                               
                                               vm.rowCollection = responce.content;
                                           }).error(function (error) {
                                               console.log("error occurd while retrieving shiments");
                                           });

                           }

                           vm.loadShipmentsByStatus = function (status) {

                               vm.statusButton = status;
                               vm.loadAllShipments(status);
                           }
                           
                          //delete shipment
                           vm.deleteById = function (row) {

                               $('#panel-notif').noty({
                                   text: '<div class="alert alert-success media fade in"><p>' + $rootScope.translate('Are you want to delete') + '?</p></div>',
                                   buttons: [
                                           {
                                               addClass: 'btn btn-primary', text: $rootScope.translate('Ok'), onClick: function ($noty) {

                                                   shipmentFactory.deleteShipment(row)
                                                   .success(function (response) {
                                                       if (response == 1) {
                                                           debugger;

                                                           $('#panel-notif').noty({
                                                               text: '<div class="alert alert-success media fade in"><p>' + $rootScope.translate('Shipment Deleted Successfully, click ok to reload the shipment list') + '?</p></div>',
                                                               buttons: [
                                                                       {
                                                                           addClass: 'btn btn-primary', text: $rootScope.translate('Ok'), onClick: function ($noty) {
                                                                               $noty.close();                                                                            
                                                                               $route.reload();

                                                                           }
                                                                       }

                                                               ],
                                                               layout: 'bottom-right',
                                                               theme: 'made',
                                                               animation: {
                                                                   open: 'animated bounceInLeft',
                                                                   close: 'animated bounceOutLeft'
                                                               },
                                                               timeout: 3000,
                                                           });

                                                           
                                                       }
                                                   })
                                       .error(function () {
                                       })

                                                   $noty.close();


                                               }
                                           },
                                           {
                                               addClass: 'btn btn-danger', text: $rootScope.translate('Cancel'), onClick: function ($noty) {

                                                   // updateProfile = false;
                                                   $noty.close();
                                                   return;
                                                   // noty({text: 'You clicked "Cancel" button', type: 'error'});
                                               }
                                           }
                                   ],
                                   layout: 'bottom-right',
                                   theme: 'made',
                                   animation: {
                                       open: 'animated bounceInLeft',
                                       close: 'animated bounceOutLeft'
                                   },
                                   timeout: 3000,
                               });


                           };

                           vm.loadAllShipments();



                       }])
})(angular.module('newApp'));
