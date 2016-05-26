﻿
'use strict';


(function (app) {

    app.controller('BillingandInvoicingCtrl',
       ['$location', '$window', 'adminFactory', 'ngDialog', '$controller', '$scope', '$rootScope',
           function ($location, $window, adminFactory, ngDialog, $controller, $scope, $rootScope) {
               var vm = this;
               vm.datePicker = {};
               vm.datePicker.date = { startDate: null, endDate: null };
           
               vm.closeWindow = function () {
                   ngDialog.close()
               }

               vm.uploadInvoice = function (fromMethod, invoiceId) {
                   debugger;
                   ngDialog.open({
                       scope: $scope,
                       template: '/app/admin/uploadInvoice.html',
                       className: 'ngdialog-theme-plain custom-width',
                       controller: $controller('uploadInvoiceCtrl', {
                           $scope: $scope,
                           fromMethod: fromMethod,
                           invoiceId: invoiceId
                       })                     
                   });
               }


               vm.loadAllInvoices = function (status, from) {
                   var status = (status == undefined || status == 'All' || status == null || status == "") ? null : status;
                   var startDate = (vm.datePicker.date.startDate == null) ? null : vm.datePicker.date.startDate.toDate();
                   var endDate = (vm.datePicker.date.endDate == null) ? null : vm.datePicker.date.endDate.toDate();
                   var shipmentnumber = (vm.shipmentNumber == undefined) ? null : vm.shipmentNumber;
                   var businessowner = (vm.businessOwner == undefined) ? null : vm.businessOwner;
                   var invoicenumber = (vm.invoiceNumber == undefined) ? null : vm.invoiceNumber;

                   adminFactory.loadAllInvoices(status, startDate, endDate, shipmentnumber, businessowner, invoicenumber)
                        .success(
                               function (responce) {
                                   if (from == 'fromDisputed')
                                   {
                                       vm.rowCollectionDisputed  = responce.content;
                                   }
                                   else {
                                       vm.rowCollection = responce.content;
                                   }
                               }).error(function (error) {
                                   console.log("error occurd while retrieving shiments");
                               });

               }

               vm.loadDisputedInvoices = function () {
                   vm.loadAllInvoices('Disputed', 'fromDisputed');
               }

               vm.loadInvoicesByStatus = function (status) {

                   vm.statusButton = status;
                   vm.loadAllInvoices(status);
               }

               vm.exportInvoiceDetailsReport = function () {

                   adminFactory.exportInvoiceDetailsReport(vm.rowCollection)
                                  .success(function (data, status, headers) {
                                      var octetStreamMime = 'application/octet-stream';
                                      var success = false;

                                      // Get the headers
                                      headers = headers();

                                      // Get the filename from the x-filename header or default to "download.bin"
                                      var filename = headers['x-filename'] || 'InvoiceReport.xlsx';

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

               vm.updateInvoiceStatus = function (row) {
                  
                   var body = $("html, body");
                   body.stop().animate({ scrollTop: 0 }, '500', 'swing', function () {
                   });

                   $('#panel-notif').noty({
                       text: '<div class="alert alert-success media fade in"><p>' + $rootScope.translate('Are you want to update the invoice status') + '?</p></div>',
                       buttons: [
                               {
                                   addClass: 'btn btn-primary', text: $rootScope.translate('Ok'), onClick: function ($noty) {

                                       $noty.close();
                                       
                                       adminFactory.updateInvoiceStatus(row)
                                                  .success(
                                                         function (responce) {
                                                             row.invoiceStatus = responce;
                                                             $('#panel-notif').noty({
                                                                 text: '<div class="alert alert-success media fade in"><p> ' + $rootScope.translate('Invoice updated successfully') + '.</p></div>',
                                                                 buttons: [
                                                                         {
                                                                             addClass: 'btn btn-primary', text: $rootScope.translate('Yes'), onClick: function ($noty) {                                                      
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
                                                         }).error(function (error) {
                                                             console.log("error occurd while retrieving shiments");
                                                         });
            
                                   }
                               },
                               {
                                   addClass: 'btn btn-danger', text: $rootScope.translate('No'), onClick: function ($noty) {
                                       
                                       $noty.close();
                                       return;
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

              
               vm.loadAllInvoices();


           }]);


})(angular.module('newApp'));