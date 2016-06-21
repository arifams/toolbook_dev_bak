'use strict';
(function (app) {

    app.controller('shipmentSearchCtrl', ['$scope', '$location', '$window', 'shipmentFactory', 'ngDialog', '$controller', '$rootScope',
                       function ($scope, $location, $window, shipmentFactory, ngDialog, $controller, $rootScope) {

                           var vm = this;
                           vm.searchText = '';
                           vm.CompanyId = '';
                           vm.rowCollection = [];
                           vm.noShipments = false;


                           vm.updateShipmentStatus = function (row) {

                               row.generalInformation.manualStatusUpdatedDate = Date();
                               $('#panel-notif').noty({
                                   text: '<div class="alert alert-success media fade in"><p>' + $rootScope.translate('Are you sure you want to update') + '?</p></div>',
                                   buttons: [
                                           {
                                               addClass: 'btn btn-primary', text: $rootScope.translate('Ok'), onClick: function ($noty) {

                                                   shipmentFactory.UpdateshipmentStatusManually(row)
                                                   .success(function (response) {
                                                       if (response == 1) {
                                                           // location.reload();
                                                           vm.loadShipmentsBySearch();
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

                           }


                           vm.deleteById = function (row) {

                               $('#panel-notif').noty({
                                   text: '<div class="alert alert-success media fade in"><p>' + $rootScope.translate('Are you sure you want to delete') + '?</p></div>',
                                   buttons: [
                                           {
                                               addClass: 'btn btn-primary', text: $rootScope.translate('Ok'), onClick: function ($noty) {

                                                   shipmentFactory.deleteShipmentbyAdmin(row)
                                                   .success(function (response) {
                                                       if (response == 1) {
                                                           row.generalInformation.status = 'Deleted';
                                                       }
                                                   })
                                       .error(function () {
                                       })
                                                   $noty.close();
                                               }
                                           },
                                           {
                                               addClass: 'btn btn-danger', text: 'Cancel', onClick: function ($noty) {

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

                           vm.loadShipmentsBySearch = function () {
                               var number = (vm.shipmentNumber == undefined) ? null : vm.shipmentNumber;

                               shipmentFactory.searchShipmentsById(number).success(
                                function (responce) {
                                    if (responce.content.length > 0) {
                                        vm.rowCollection = responce.content;
                                        vm.noShipments = false;

                                    } else {

                                        vm.noShipments = true;
                                        vm.rowCollection = [];
                                    }
                                }).error(function (error) {

                                    console.log("error occurd while retrieving Addresses");
                                });

                           }                           

                        }])
})(angular.module('newApp'));
