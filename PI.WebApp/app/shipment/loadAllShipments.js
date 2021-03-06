﻿'use strict';
(function (app) {

    app.controller('loadShipmentsCtrl', ['$scope', '$location', '$window', 'shipmentFactory', '$rootScope', '$route', '$routeParams', 'customBuilderFactory',
                       function ($scope, $location, $window, shipmentFactory, $rootScope, $route, $routeParams, customBuilderFactory) {

                           var vm = this;
                           var statusValue = null;
                           vm.viaDashboard = false;
                           var tableStateCopy;

                           vm.viaDashboard = $scope.dashCtrl == undefined ? false : $scope.dashCtrl.isViaDashboard;

                           //toggle function
                           vm.loadFilterToggle = function () {
                               customBuilderFactory.customFilterToggle();

                           };

                           vm.openLabel = function (url) {
                               debugger;
                               window.open(url);
                           }

                           vm.openLabelList = function (url) {
                               debugger;
                               for (var i = 0; i < url.length; i++) {
                                   window.open(url[i]);
                               }
                           }

                           vm.status = 'All';
                           vm.datePicker = {};
                           vm.datePicker.date = { startDate: null, endDate: null };
                           vm.rowCollection = [];
                           vm.loadingSymbole = true;

                           vm.loadAllShipments = function (status, startRecord, pageRecord, tableState) {

                               vm.loadingSymbole = true;

                               var pagedList = {
                                   filterContent: {
                                       status: (status == undefined || status == 'All' || status == null || status == "") ? null : status,
                                       startDate: (vm.datePicker.date.startDate == null) ? null : vm.datePicker.date.startDate.toDate(),
                                       endDate: (vm.datePicker.date.endDate == null) ? null : vm.datePicker.date.endDate.toDate(),
                                       number: (vm.shipmentNumber == undefined) ? null : vm.shipmentNumber,
                                       source: (vm.originCityCountry == undefined) ? null : vm.originCityCountry,
                                       destination: (vm.desCityCountry == undefined) ? null : vm.desCityCountry,
                                       viaDashboard: vm.viaDashboard
                                   },
                                   pageSize: pageRecord,
                                   currentPage: startRecord
                               }

                               statusValue = status;

                               shipmentFactory.loadAllShipments(pagedList)
                                    .success(
                                           function (responce) {
                                               debugger;
                                               vm.loadingSymbole = false;
                                               vm.rowCollection = responce.content;

                                               tableState.pagination.numberOfPages = responce.totalPages;

                                               vm.setCSVData(responce);

                                           }).error(function (error) {
                                               vm.loadingSymbole = false;
                                               console.log("error occurd while retrieving shiments");
                                           });

                           }

                           vm.loadShipmentsByStatus = function (status) {

                               vm.statusButton = status;
                               vm.loadAllShipments(status);
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

                               $.each(responce.content, function (index, value) {
                                    
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

                           vm.ExportExcel = function (tableState) {

                               vm.loadingSymbole = true;
                               
                               if (tableState != undefined) {
                                   tableStateCopy = tableState;
                               }
                               else {
                                   tableState = tableStateCopy;
                               }

                               var start = tableState.pagination.start;
                               var number = tableState.pagination.number;
                               var numberOfPages = tableState.pagination.numberOfPages;

                               var pagedList = {
                                   filterContent: {
                                       status: (vm.status == undefined || vm.status == 'All' || vm.status == null || vm.status == "") ? null : vm.status,
                                       startDate: (vm.datePicker.date.startDate == null) ? null : vm.datePicker.date.startDate.toDate(),
                                       endDate: (vm.datePicker.date.endDate == null) ? null : vm.datePicker.date.endDate.toDate(),
                                       number: (vm.shipmentNumber == undefined) ? null : vm.shipmentNumber,
                                       source: (vm.originCityCountry == undefined) ? null : vm.originCityCountry,
                                       destination: (vm.desCityCountry == undefined) ? null : vm.desCityCountry,
                                       viaDashboard: false
                                   },
                                   pageSize: number,
                                   currentPage: start
                               }

                               shipmentFactory.getFilteredShipmentsExcel(pagedList)
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

                           };

                           //delete shipment
                           vm.deleteById = function (row, source) {

                               $('#panel-notif').noty({
                                   text: '<div class="alert alert-success media fade in"><p>' + $rootScope.translate('Are you sure you want to delete') + '?</p></div>',
                                   buttons: [
                                           {
                                               addClass: 'btn btn-primary', text: $rootScope.translate('Ok'), onClick: function ($noty) {

                                                   shipmentFactory.deleteShipment(row, source)
                                                   .success(function (response) {
                                                       if (response == 1) {


                                                           $('#panel-notif').noty({
                                                               text: '<div class="alert alert-success media fade in"><p>' + $rootScope.translate('Shipment Deleted Successfully') + '!</p></div>',
                                                               buttons: [
                                                                       {
                                                                           addClass: 'btn btn-primary', text: $rootScope.translate('Ok'), onClick: function ($noty) {
                                                                               $noty.close();
                                                                               row.generalInformation.status = 'Deleted';
                                                                               row.generalInformation.isEnableEdit = false;
                                                                               row.generalInformation.isEnableDelete = false;

                                                                               debugger;
                                                                               if (source == 'delete-copy') {
                                                                                   $location.path('/addShipment/0').search({
                                                                                       PARAM_SOURCE: source,
                                                                                       PARAM_SOURCEID: row.generalInformation.shipmentId
                                                                                   });
                                                                               }
                                                                           }
                                                                       }

                                                               ],
                                                               layout: 'bottom-right',
                                                               theme: 'made',
                                                               animation: {
                                                                   open: 'animated bounceInLeft',
                                                                   close: 'animated bounceOutLeft'
                                                               },
                                                               timeout: 500,
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
                           
                           //delete shipment
                           vm.saveById = function (row) {

                               $('#panel-notif').noty({
                                   text: '<div class="alert alert-success media fade in"><p>' + $rootScope.translate('Are you sure you want to update reference') + '?</p></div>',
                                   buttons: [
                                           {
                                               addClass: 'btn btn-primary', text: $rootScope.translate('Ok'), onClick: function ($noty) {

                                                   shipmentFactory.UpdateShipmentReference(row)
                                                   .success(function (response) {
                                                       
                                                       if (response.status == 2) {


                                                           $('#panel-notif').noty({
                                                               text: '<div class="alert alert-success media fade in"><p>' + $rootScope.translate('Shipment Deleted Successfully') + '!</p></div>',
                                                               buttons: [
                                                                       {
                                                                           addClass: 'btn btn-primary', text: $rootScope.translate('Ok'), onClick: function ($noty) {
                                                                               $noty.close();
                                                                              
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
                                                    
                           vm.toggleFavourite = function (row) {
                               var count = 0;
                               angular.forEach(vm.rowCollection, function (item, key) {
                                   count = (item.generalInformation.isFavourite) ? count + 1 : count;
                               });


                               if (count == 10 && row.generalInformation.isFavourite == false) {
                                   var body = $("html, body");

                                   $('#panel-notif').noty({
                                       text: '<div class="alert alert-warning media fade in"><p> ' + $rootScope.translate('Please select only 10 favourite shipments') + '.</p></div>',
                                       buttons: [
                                               {
                                                   addClass: 'btn btn-primary', text: $rootScope.translate('Ok'), onClick: function ($noty) {
                                                       $noty.close();

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
                               else {
                                   shipmentFactory.toggleFavourite(row)
                                                  .success(function (response) {
                                                      row.generalInformation.isFavourite = response;

                                                  });
                               }
                           }

                           vm.copyAsNewShipment = function (shipmentId) {

                               $location.path('/addShipment/0').search({
                                   PARAM_SOURCE: 'copy',
                                   PARAM_SOURCEID: shipmentId
                               })
                           }

                           vm.deleteAndCopyShipment = function (shipment) {
                               debugger;
                               // call delete shipment
                               vm.deleteById(shipment, 'delete-copy');
                           }

                           vm.createReturnShipment = function (shipmentId) {

                                $location.path('/addShipment/0').search({
                                   PARAM_SOURCE: 'return-copy',
                                   PARAM_SOURCEID: shipmentId
                               });
                           }
                           

                           vm.OpenTab = function (row, source) {
                               debugger;
                               if (row.generalInformation.status === 'Draft')
                               {
                                   row.generalInformation.shipmentCode = '';
                                   row.generalInformation.trackingNumber = '';
                                   row.generalInformation.trackingNumber = '';                                   
                               }
                               $location.path('/ShipmentOverview').search({
                                   SHIPMENT_CODE: row.generalInformation.shipmentCode,
                                   SHIPMENT_ID: row.generalInformation.shipmentId,
                                   TRACKING_NO: row.generalInformation.trackingNumber,
                                   CARRIER: row.carrierInformation.carrierName,
                                   CREATED_ON: row.generalInformation.createdDate,
                                   SOURCE: source

                               });
                           }

                           vm.callServerSearch = function (tableState) {

                               
                               if (tableState != undefined) {
                                   tableStateCopy = tableState;
                               }
                               else {
                                   tableState = tableStateCopy;
                               }

                               var start = tableState.pagination.start;
                               var number = tableState.pagination.number;
                               var numberOfPages = tableState.pagination.numberOfPages;

                               if ($routeParams.status != undefined && $routeParams.status != null) {
                                   vm.loadAllShipments($routeParams.status, start, number, tableState);
                               }
                               else {
                                   console.log(vm.status);
                                   vm.loadAllShipments(vm.status, start, number, tableState);
                               }
                           };

                          // vm.callServerSearch();

                           vm.resetSearch = function (tableState) {
                                
                               var pagination = 0;//tableState.pagination;

                               var start = pagination.start || 0;     // This is NOT the page number, but the index of item in the list that you want to use to display the table.
                               var number = pagination.number || 10;  // Number of entries showed per page.

                               vm.status = 'All';
                               vm.datePicker.date = { "startDate": null, "endDate": null };
                               //vm.datePicker.date.endDate = null;
                               console.log(vm.status);

                               vm.loadAllShipments(vm.status, start, number, tableState);

                           }

                       }])
})(angular.module('newApp'));
