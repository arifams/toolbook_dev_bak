'use strict';
(function (app) {

    app.controller('loadShipmentsCtrl', ['$scope', '$location', '$window', 'shipmentFactory', '$rootScope', '$route','$routeParams',
                       function ($scope, $location, $window, shipmentFactory, $rootScope, $route, $routeParams) {

                           var vm = this;
                           var statusValue = null;
                           vm.viaDashboard = false;
                           vm.viaDashboard = $scope.dashCtrl == undefined ? false : $scope.dashCtrl.isViaDashboard;

                           vm.statusButton = 'All';
                           vm.datePicker = {};
                           vm.datePicker.date = { startDate: null, endDate: null };
                           vm.itemsByPage = 25; // Set page size    // 25
                           vm.rowCollection = [];
                           vm.loadingSymbole = true;

                           vm.loadAllShipments = function (status) {

                               vm.loadingSymbole = true;

                               var status = (status == undefined || status == 'All' || status == null || status == "") ? null : status;
                               var startDate = (vm.datePicker.date.startDate == null) ? null : vm.datePicker.date.startDate.toDate();
                               var endDate = (vm.datePicker.date.endDate == null) ? null : vm.datePicker.date.endDate.toDate();
                               var number = (vm.shipmentNumber == undefined) ? null : vm.shipmentNumber;
                               var source = (vm.originCityCountry == undefined) ? null : vm.originCityCountry;
                               var destination = (vm.desCityCountry == undefined) ? null : vm.desCityCountry;

                               statusValue = status;

                               shipmentFactory.loadAllShipments(status, startDate, endDate, number, source, destination, vm.viaDashboard)
                                    .success(
                                           function (responce) {
                                               vm.loadingSymbole = false;
                                               vm.rowCollection = responce.content;

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



                                           }).error(function (error) {
                                               vm.loadingSymbole = false;
                                               console.log("error occurd while retrieving shiments");
                                           });

                           }

                           vm.loadShipmentsByStatus = function (status) {

                               vm.statusButton = status;
                               vm.loadAllShipments(status);
                           }

                           vm.ExportExcel = function () {

                               debugger;
                               var status = statusValue;
                               var startDate = (vm.datePicker.date.startDate == null) ? null : vm.datePicker.date.startDate.toDate();
                               var endDate = (vm.datePicker.date.endDate == null) ? null : vm.datePicker.date.endDate.toDate();
                               var number = (vm.shipmentNumber == undefined) ? null : vm.shipmentNumber;
                               var source = (vm.originCityCountry == undefined) ? null : vm.originCityCountry;
                               var destination = (vm.desCityCountry == undefined) ? null : vm.desCityCountry;

                               shipmentFactory.getFilteredShipmentsExcel(status, startDate, endDate, number, source, destination, vm.viaDashboard)
                               .success(function (data, status, headers) {

                                   var octetStreamMime = 'application/octet-stream';
                                   var success = false;

                                   // Get the headers
                                   headers = headers();

                                   // Get the filename from the x-filename header or default to "download.bin"
                                   var filename = headers['x-filename'] || 'AddressBook.xlsx';

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
                           vm.deleteById = function (row) {

                               $('#panel-notif').noty({
                                   text: '<div class="alert alert-success media fade in"><p>' + $rootScope.translate('Are you sure you want to delete') + '?</p></div>',
                                   buttons: [
                                           {
                                               addClass: 'btn btn-primary', text: $rootScope.translate('Ok'), onClick: function ($noty) {

                                                   shipmentFactory.deleteShipment(row)
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

                           
                           if($routeParams.status != undefined && $routeParams.status != null)
                           {
                               vm.loadAllShipments($routeParams.status);
                           }
                           else {
                               vm.loadAllShipments();
                           }

                       }])
})(angular.module('newApp'));
