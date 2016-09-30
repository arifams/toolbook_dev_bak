'use strict';
(function (app) {

    app.controller('shipmentManageCtrl', ['$scope', '$location', '$window', 'shipmentFactory', 'ngDialog', '$controller', '$rootScope', 'customBuilderFactory', 'modalService',
                       function ($scope, $location, $window, shipmentFactory, ngDialog, $controller, $rootScope, customBuilderFactory, modalService) {

                           var vm = this;
                           vm.searchText = '';
                           vm.CompanyId = '';
                           vm.rowCollection = [];
                           vm.noShipments = false;
                           vm.statusButton = 'All';
                           vm.datePicker = {};
                           vm.datePicker.date = { startDate: null, endDate: null };

                           //toggle function
                           vm.loadFilterToggle = function () {
                               customBuilderFactory.customFilterToggle();

                           };
                          
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
                                                           //var index = vm.rowCollection.indexOf(row);
                                                           //if (index !== -1) {
                                                           //    vm.rowCollection.splice(index, 1);
                                                           //}
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
                               var from = 'manageShipCtrl'

                               vm.rowCollection = [];

                               shipmentFactory.loadAllcompanies(vm.searchText).success(
                                  function (responce) {
                                      if (responce.content.length > 0) {

                                          vm.noShipments = false;
                                          ngDialog.open({
                                              scope: $scope,
                                              template: '/app/shipment/CompanyViewTemplate.html',
                                              className: 'ngdialog-theme-default',
                                              controller: $controller('companyListCtrl', {
                                                  $scope: $scope,
                                                  searchList: responce.content,
                                                  from: from
                                              })

                                          });


                                      } else {
                                          vm.noShipments = true;
                                          vm.emptySearch = false;
                                      }
                                  }).error(function (error) {

                                      console.log("error occurd while retrieving Addresses");
                                  });

                           }


                           vm.shipmentSyncWithSIS = function () {
                               
                               
                               shipmentFactory.getShipmentForCompanyAndSyncWithSIS(vm.CompanyId).success(
                                  function (responce) {
                                      if (responce.content.length > 0) {
                                          vm.rowCollection = responce.content;
                                      } else {
                                          vm.noShipments = true;
                                          vm.emptySearch = false;
                                      }
                                  }).error(function (error) {
                                          console.log("error occurd while retrieving Addresses");
                                  });

                           }

                           //open modal
                           vm.searchShipments = function () {
                               //console.log('work');
                               $scope.templateUrl = "admin/SearchSpecificShipments.html";
                               modalService.load('modal-searchShipments');
                               
                           }


       
                       }])
})(angular.module('newApp'));
