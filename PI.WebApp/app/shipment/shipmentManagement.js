'use strict';
(function (app) {

    app.controller('shipmentManageCtrl', ['$scope', '$location', '$window', 'shipmentFactory', 'ngDialog', '$controller', '$rootScope', 'customBuilderFactory', 'modalService', '$route',
                       function ($scope, $location, $window, shipmentFactory, ngDialog, $controller, $rootScope, customBuilderFactory, modalService, $route) {

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

                           vm.ExportExcel = function () {

                               vm.loadingSymbole = true;

                               var status = (vm.status == undefined || vm.status == 'All' || vm.status == null || vm.status == "") ? null : vm.status;
                               var startDate = (vm.datePicker.date.startDate == null) ? null : vm.datePicker.date.startDate.toDate();
                               var endDate = (vm.datePicker.date.endDate == null) ? null : vm.datePicker.date.endDate.toDate();
                               var number = (vm.shipmentNumber == undefined) ? null : vm.shipmentNumber;
                               var source = (vm.originCityCountry == undefined) ? null : vm.originCityCountry;
                               var destination = (vm.desCityCountry == undefined) ? null : vm.desCityCountry;

                               shipmentFactory.loadAllShipmentsForAdminExcelExport(vm.CompanyId, status, startDate, endDate, number, source, destination)
                              .success(function (data, status, headers) {
                                  vm.loadingSymbole = false;
                                  var octetStreamMime = 'application/octet-stream';
                                  var success = false;

                                  // Get the headers    
                                  headers = headers();

                                  // Get the filename from the x-filename header or default to "download.bin"
                                  var filename = headers['x-filename'] || 'ShipmentDetails.xlsx';

                                  // Determine the content type from the header or default to "application/octet-stream"
                                  var contentType = headers['content-type'] || octetStreamMime;

                                  try {
                                      // Try using msSaveBlob if supported
                                      console.log("Trying saveBlob method ...");
                                      var blob = new Blob([data], { type: contentType });
                                      if (navigator.msSaveBlob)
                                          navigator.msSaveBlob(blob, filename);
                                      else {
                                          // Try using other saveBlob implementations, if available
                                          var saveBlob = navigator.webkitSaveBlob || navigator.mozSaveBlob || navigator.saveBlob;
                                          if (saveBlob === undefined) throw "Not supported";
                                          saveBlob(blob, filename);
                                      }
                                      console.log("saveBlob succeeded");
                                      success = true;
                                  } catch (ex) {
                                      console.log("saveBlob method failed with the following exception:");
                                      console.log(ex);
                                  }

                                  if (!success) {
                                      // Get the blob url creator
                                      var urlCreator = window.URL || window.webkitURL || window.mozURL || window.msURL;
                                      if (urlCreator) {
                                          // Try to use a download link
                                          var link = document.createElement('a');
                                          if ('download' in link) {
                                              // Try to simulate a click
                                              try {
                                                  // Prepare a blob URL
                                                  console.log("Trying download link method with simulated click ...");
                                                  var blob = new Blob([data], { type: contentType });
                                                  var url = urlCreator.createObjectURL(blob);
                                                  link.setAttribute('href', url);

                                                  // Set the download attribute (Supported in Chrome 14+ / Firefox 20+)
                                                  link.setAttribute("download", filename);

                                                  // Simulate clicking the download link
                                                  var event = document.createEvent('MouseEvents');
                                                  event.initMouseEvent('click', true, true, window, 1, 0, 0, 0, 0, false, false, false, false, 0, null);
                                                  link.dispatchEvent(event);
                                                  console.log("Download link method with simulated click succeeded");
                                                  success = true;

                                              } catch (ex) {
                                                  console.log("Download link method with simulated click failed with the following exception:");
                                                  console.log(ex);
                                              }
                                          }

                                          if (!success) {
                                              // Fallback to window.location method
                                              try {
                                                  // Prepare a blob URL
                                                  // Use application/octet-stream when using window.location to force download
                                                  console.log("Trying download link method with window.location ...");
                                                  var blob = new Blob([data], { type: octetStreamMime });
                                                  var url = urlCreator.createObjectURL(blob);
                                                  window.location = url;
                                                  console.log("Download link method with window.location succeeded");
                                                  success = true;
                                              } catch (ex) {
                                                  console.log("Download link method with window.location failed with the following exception:");
                                                  console.log(ex);
                                              }
                                          }

                                      }
                                  }

                                  if (!success) {
                                      // Fallback to window.open method
                                      console.log("No methods worked for saving the arraybuffer, using last resort window.open");
                                      window.open(httpPath, '_blank', '');
                                  }
                              })
                             .error(function (data, status) {
                                 console.log("Request failed with status: " + status);

                                 // Optionally write the error out to scope
                                 $scope.errorDetails = "Request failed with status: " + status;
                             });


                           }

                           vm.loadShipmentsBySearch = function (status, startRecord, pageRecord, tableState) {

                               vm.loadingSymbole = true;

                               var status = (status == undefined || status == 'All' || status == null || status == "") ? null : status;
                               var startDate = (vm.datePicker.date.startDate == null) ? null : vm.datePicker.date.startDate.toDate();
                               var endDate = (vm.datePicker.date.endDate == null) ? null : vm.datePicker.date.endDate.toDate();
                               var searchValue = (vm.searchValue == undefined || vm.searchValue == null || vm.searchValue == "") ? null : vm.searchValue;

                               shipmentFactory.loadAllShipmentsForAdmin(status, startDate, endDate, vm.searchValue, startRecord, pageRecord)
                               .then(function (responce) {


                                   vm.loadingSymbole = false;
                                   if (responce.data.content != null) {
                                       vm.rowCollection = responce.data.content;

                                       tableState.pagination.numberOfPages = responce.data.totalPages;

                                       vm.noShipments = false;


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
                                                           location.reload();
                                                           //vm.loadShipmentsBySearch();
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

                           vm.deleteById = function (row, source) {

                               $('#panel-notif').noty({
                                   text: '<div class="alert alert-success media fade in"><p>' + $rootScope.translate('Are you sure you want to delete') + '?</p></div>',
                                   buttons: [
                                           {
                                               addClass: 'btn btn-primary', text: $rootScope.translate('Ok'), onClick: function ($noty) {

                                                   shipmentFactory.deleteShipmentbyAdmin(row)
                                                   .success(function (response) {
                                                       if (response == 1) {

                                                           row.generalInformation.status = 'Deleted';
                                                           row.generalInformation.isEnableEdit = false;
                                                           row.generalInformation.isEnableDelete = false;

                                                           if (source == 'delete-copy') {
                                                               $location.path('/addShipment/0');
                                                           }
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
                               vm.loadingSymbole = true;

                               vm.rowCollection = [];

                               shipmentFactory.loadAllcompanies(vm.searchText).success(
                                  function (responce) {
                                      if (responce.content.length > 0) {

                                          vm.loadingSymbole = false;
                                          vm.noShipments = false;
                                          ngDialog.open({
                                              scope: $scope,
                                              template: '/app/shipment/CompanyViewTemplate.html',
                                              className: 'ngdialog-theme-plain custom-width',
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

                           //search specific customers
                           vm.searchShipments = function () {
                               ngDialog.open({
                                   scope: $scope,
                                   template: '/app/admin/SearchSpecificShipments.html',
                                   className: 'ngdialog-theme-plain custom-width-max',
                                   controller: $controller('shipmentSearchCtrl', {
                                       $scope: $scope,

                                   })
                               });

                           }

                           vm.getShipmentStatusCounts = function () {

                               shipmentFactory.GetAllShipmentCounts()
                               .then(function (response) {

                                   if (response.data != null) {
                                       vm.counts = response.data;
                                   }
                               },
                               function (error) {
                                   vm.model.isServerError = "true";
                               })
                           }

                           var tableStateCopy;

                           vm.callServerSearch = function (tableState) {

                               if (tableState != undefined) {
                                   tableStateCopy = tableState;
                               }
                               else {
                                   tableState = tableStateCopy;
                                   // tablestate undefined mean, this is come from search btn. Not from click on pagination. Reset everything.
                                   tableState.pagination.start = 0;
                               }

                               var start = tableState.pagination.start;
                               var number = tableState.pagination.number;
                               var numberOfPages = tableState.pagination.numberOfPages;

                               vm.loadShipmentsBySearch(vm.status, start, number, tableState);
                           };

                           vm.shipmentSearch = function () {
                               vm.status = 'All';
                               vm.datePicker.date = { "startDate": null, "endDate": null };
                               vm.callServerSearch();
                           };

                           vm.applySearch = function () {
                               vm.searchValue = '';
                               vm.callServerSearch();
                           };

                           vm.statusSearch = function () {
                               vm.datePicker.date = { "startDate": null, "endDate": null };
                               vm.searchValue = '';
                               vm.callServerSearch();
                           };

                           vm.resetSearch = function (tableState) {

                               var pagination = 0;//tableState.pagination;

                               var start = pagination.start || 0;     // This is NOT the page number, but the index of item in the list that you want to use to display the table.
                               var number = pagination.number || 10;  // Number of entries showed per page.

                               vm.status = 'All';
                               vm.datePicker.date = { "startDate": null, "endDate": null };

                               vm.loadShipmentsBySearch(vm.status, start, number, tableState);
                           }

                           vm.copyAsNewShipment = function (shipmentId) {
                               
                               $window.localStorage.setItem('paramSource', null);
                               $window.localStorage.setItem('paramSource', 'copy');
                               $window.localStorage.setItem('paramSourceId', null);
                               $window.localStorage.setItem('paramSourceId', shipmentId);
                               $location.path('/addShipment/0');
                           }

                           vm.deleteAndCopyShipment = function (shipment) {
                               debugger;
                               // call delete shipment
                      
                               $window.localStorage.setItem('paramSource', null);
                               $window.localStorage.setItem('paramSource', 'delete-copy');
                               $window.localStorage.setItem('paramSourceId', null);
                               $window.localStorage.setItem('paramSourceId', shipment.generalInformation.shipmentId);

                               vm.deleteById(shipment, 'delete-copy');                      
                           }
                           
                           vm.createReturnShipment = function (shipmentId) {

                               $window.localStorage.setItem('paramSource', null);
                               $window.localStorage.setItem('paramSource', 'return-copy');
                               $window.localStorage.setItem('paramSourceId', null);
                               $window.localStorage.setItem('paramSourceId', shipmentId);

                               $location.path('/addShipment/0');
                           }


                           vm.getShipmentStatusCounts();

                       }])

})(angular.module('newApp'));
