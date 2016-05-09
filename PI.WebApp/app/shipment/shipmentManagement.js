'use strict';
(function (app) {

    app.controller('shipmentManageCtrl', ['$scope', '$location', '$window', 'shipmentFactory','ngDialog','$controller',
                       function ($scope, $location, $window, shipmentFactory, ngDialog, $controller) {

                           var vm = this;
                           vm.searchText = '';
                           vm.CompanyId = '';
                           vm.rowCollection = [];
                           vm.noShipments = false;
                           vm.statusButton = 'All';
                           vm.datePicker = {};
                           vm.datePicker.date = { startDate: null, endDate: null };
                          
                           vm.loadShipmentsBySearch = function (status) {

                               var status = (status == undefined || status == 'All' || status == null || status == "") ? null : status;
                               var startDate = (vm.datePicker.date.startDate == null) ? null : vm.datePicker.date.startDate.toDate();
                               var endDate = (vm.datePicker.date.endDate == null) ? null : vm.datePicker.date.endDate.toDate();
                               var number = (vm.shipmentNumber == undefined) ? null : vm.shipmentNumber;
                               var source = (vm.originCityCountry == undefined) ? null : vm.originCityCountry;
                               var destination = (vm.desCityCountry == undefined) ? null : vm.desCityCountry;


                               shipmentFactory.loadAllShipmentsFromCompanyAndSearch(vm.CompanyId, status, startDate, endDate, number, source, destination).success(
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

                           vm.loadShipmentsByStatus = function (status) {

                               vm.statusButton = status;
                               vm.loadShipmentsBySearch(status);
                           }

                           vm.closeWindow = function () {
                               ngDialog.close()
                           }
                     
                           vm.loadAllCompanies = function () {

                               shipmentFactory.loadAllcompanies(vm.searchText).success(
                                  function (responce) {
                                      if (responce.content.length > 0) {

                                          ngDialog.open({
                                              scope: $scope,
                                              template: '/app/shipment/CompanyViewTemplate.html',
                                              className: 'ngdialog-theme-default',
                                              controller: $controller('companyListCtrl', {
                                                  $scope: $scope,
                                                  searchList: responce.content                                             
                                              })

                                          });


                                      } else {
                                          vm.addressDetailsEmpty = true;
                                          vm.emptySearch = false;
                                      }
                                  }).error(function (error) {

                                      console.log("error occurd while retrieving Addresses");
                                  });

                           }




       
                       }])
})(angular.module('newApp'));
