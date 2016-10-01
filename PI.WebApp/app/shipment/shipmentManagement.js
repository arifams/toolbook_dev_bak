'use strict';
(function (app) {

    app.controller('shipmentManageCtrl', ['$scope', '$location', '$window', 'shipmentFactory', 'ngDialog', '$controller', '$rootScope', 'customBuilderFactory', 'modalService',
                       function ($scope, $location, $window, shipmentFactory, ngDialog, $controller, $rootScope, customBuilderFactory, modalService) {

                           var vm = this;
                           vm.searchText = '';
                           vm.CompanyId = '';
                           vm.rowCollection = [];
                           vm.noShipments = false;
                           vm.status = 'All';
                           vm.datePicker = {};
                           vm.datePicker.date = { startDate: null, endDate: null };
                           vm.loadingSymbole = true;
                           vm.status = 'BookingConfirmation';

                           //toggle function
                           vm.loadFilterToggle = function () {
                               customBuilderFactory.customFilterToggle();

                           };

                           vm.loadShipmentsBySearch = function (status, pageStart, pageNumber, tableState) {
                               debugger;
                               var status = (status == undefined || status == 'All' || status == null || status == "") ? null : status;
                               var startDate = (vm.datePicker.date.startDate == null) ? null : vm.datePicker.date.startDate.toDate();
                               var endDate = (vm.datePicker.date.endDate == null) ? null : vm.datePicker.date.endDate.toDate();
                               var number = (vm.shipmentNumber == undefined) ? null : vm.shipmentNumber;
                               var source = (vm.originCityCountry == undefined) ? null : vm.originCityCountry;
                               var destination = (vm.desCityCountry == undefined) ? null : vm.desCityCountry;


                               shipmentFactory.loadAllShipmentsFromCompanyAndSearch(vm.CompanyId, status, startDate, endDate, number, source, destination)
                               .then(function (responce) {
                                    debugger;
                                    if (responce.data.content != null) {
                                        vm.rowCollection = responce.data.content;
                                        vm.noShipments = false;
                                        vm.loadingSymbole = false;

                                        vm.setCSVData(responce);

                                    } else {

                                        vm.noShipments = true;
                                        vm.rowCollection = [];
                                    }
                               }, function errorCallback(error) {
                                   vm.loadingSymbole = false;
                                   console.log("error occurd while retrieving Addresses");
                               });                              

                           }

                           vm.setCSVData = function (responce) {

                               vm.exportcollection = [];

                               //adding headers for export csv file
                               var headers = {};
                               headers.orderSubmitted = "Order Submitted";
                               headers.trackingNumber = "Tracking Number";
                               headers.shipmentId = "ShipmentId";
                               headers.carrier = "Carrier";
                               headers.originCity = "Origin City";
                               headers.originCountry = "Origin Country";
                               headers.consignorName = "Consignor Name";
                               headers.consignorNumber = "Consignor Number";
                               headers.consignorEmail = "Consignor Email";
                               headers.destinationCity = "Destination City";
                               headers.destinationCountry = "Destination Country";
                               headers.consigneeName = "Consignee Name";
                               headers.consigneeNumber = "Consignee Number";
                               headers.consigneeEmail = "Consignee Email";
                               headers.status = "Status";
                               headers.shipmentMode = "Shipment Mode";
                               headers.pickupDate = "Pickup Date";
                               headers.serviceLevel = "Service Level";


                               vm.exportcollection.push(headers);

                               $.each(responce.data.content, function (index, value) {
                                   debugger;
                                   var shipmentObj = {}
                                   shipmentObj.orderSubmitted = value.generalInformation.createdDate;
                                   shipmentObj.trackingNumber = value.generalInformation.trackingNumber;
                                   shipmentObj.shipmentId = value.generalInformation.shipmentCode;
                                   shipmentObj.carrier = value.carrierInformation.carrierName;
                                   shipmentObj.originCity = value.addressInformation.consigner.city;
                                   shipmentObj.originCountry = value.addressInformation.consigner.country;
                                   shipmentObj.consignorName = value.addressInformation.consigner.contactName;
                                   shipmentObj.consignorNumber = value.addressInformation.consigner.contactNumber;
                                   shipmentObj.consignorEmail = value.addressInformation.consigner.email;
                                   shipmentObj.destinationCity = value.addressInformation.consignee.city;
                                   shipmentObj.destinationCountry = value.addressInformation.consignee.country;
                                   shipmentObj.consigneeName = value.addressInformation.consignee.contactName;
                                   shipmentObj.consigneeNumber = value.addressInformation.consignee.contactNumber;
                                   shipmentObj.consigneeEmail = value.addressInformation.consignee.email;
                                   shipmentObj.status = value.generalInformation.status;
                                   shipmentObj.shipmentMode = value.generalInformation.shipmentMode;
                                   shipmentObj.pickupDate = value.carrierInformation.pickupDate;
                                   shipmentObj.serviceLevel = value.carrierInformation.serviceLevel;
                                   vm.exportcollection.push(shipmentObj);
                               });
                           }


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


                           vm.callServerSearch = function (tableState) {
                               debugger;
                               var pagination = 0;//tableState.pagination;

                               var start = pagination.start || 0;     // This is NOT the page number, but the index of item in the list that you want to use to display the table.
                               var number = pagination.number || 10;  // Number of entries showed per page.

                            
                               vm.loadShipmentsBySearch(vm.status, start, number, tableState);

                           };


                           vm.resetSearch = function (tableState) {
                               debugger;
                               var pagination = 0;//tableState.pagination;

                               var start = pagination.start || 0;     // This is NOT the page number, but the index of item in the list that you want to use to display the table.
                               var number = pagination.number || 10;  // Number of entries showed per page.

                               vm.status = 'All';
                               vm.datePicker.date = { "startDate": null, "endDate": null };
                               //vm.datePicker.date.endDate = null;
                               console.log(vm.status);

                               vm.loadShipmentsBySearch(vm.status, start, number, tableState);
                           }

                       }])
})(angular.module('newApp'));
