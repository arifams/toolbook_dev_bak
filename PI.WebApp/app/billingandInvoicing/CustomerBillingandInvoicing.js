'use strict';

(function (app) {

    app.factory('customerInvoiceFactory', ['$http', '$routeParams', '$window', function ($http, $routeParams, $window) {

        return {
            getAllInvoicesByCustomer: getAllInvoicesByCustomer,
            payInvoice: payInvoice,
            disputeInvoice: disputeInvoice,
            exportInvoiceReport: exportInvoiceReport
        };

        function getAllInvoicesByCustomer(status, startDate, endDate, shipmentNumber, invoiceNumber) {

            return $http.get(serverBaseUrl + '/api/Customer/GetAllInvoicesByCustomer', {
                params: {
                    status: status,
                    userId: $window.localStorage.getItem('userGuid'),
                    startDate: startDate,
                    endDate: endDate,
                    shipmentNumber: shipmentNumber,
                    invoiceNumber: invoiceNumber
                }
            });
        }


        function payInvoice(invoiceDetail) {

            return $http.post(serverBaseUrl + '/api/Customer/PayInvoice', invoiceDetail);
        }


        function disputeInvoice(invoiceDetail) {
            invoiceDetail.createdBy = $window.localStorage.getItem('userGuid');
            return $http.post(serverBaseUrl + '/api/Customer/DisputeInvoice', invoiceDetail);
        }

        function exportInvoiceReport(invoiceList) {
            return $http({
                url: serverBaseUrl + '/api/Customer/ExportInvoiceReport',
                data: invoiceList,
                method: "POST",             
                responseType: 'arraybuffer'
            });
        }

    }]);

    app.controller('customerinvoiceCtrl', ['$location', '$window', 'customerInvoiceFactory', 'ngDialog', '$controller', '$scope', '$rootScope', 'customBuilderFactory','shipmentFactory',
                    function ($location, $window, customerInvoiceFactory, ngDialog, $controller, $scope, $rootScope, customBuilderFactory, shipmentFactory) {
                        var vm = this;
                        vm.datePicker = {};
                        vm.datePicker.date = { startDate: null, endDate: null };

                        //toggle function
                        vm.loadFilterToggle = function () {
                            customBuilderFactory.customFilterToggle();

                        };

                        vm.closeWindow = function () {
                            ngDialog.close()
                        }



                        vm.loadInvoicesBySearch = function (status) {
                            
                            var status = (status == undefined || status == 'All' || status == null || status == "") ? null : status;
                            var startDate = (vm.datePicker.date.startDate == null) ? null : vm.datePicker.date.startDate.toDate();
                            var endDate = (vm.datePicker.date.endDate == null) ? null : vm.datePicker.date.endDate.toDate();
                            var shipmentNumber = (vm.shipmentNumber == undefined || vm.shipmentNumber == "") ? null : vm.shipmentNumber;
                            var invoiceNumber = (vm.invoiceNumber == undefined || vm.invoiceNumber == "") ? null : vm.invoiceNumber;

                            customerInvoiceFactory.getAllInvoicesByCustomer(status, startDate, endDate, shipmentNumber, invoiceNumber)
                                .then(function successCallback(responce) {

                                    vm.rowCollection = responce.data.content;
                                    vm.exportcollection = [];

                                    //adding headers for export csv file
                                    var headers = {};
                                    headers.orderSubmitted = "Invoice Number";
                                    headers.trackingNumber = "Invoice Date";
                                    headers.shipmentId = "Shipment Reference";
                                    headers.carrier = "Invoice Value";
                                    headers.originCity = "Invoice Status";                                   
                                    vm.exportcollection.push(headers);

                                    $.each(responce.data.content, function (index, value) {                                      
                                        var invoiceObj = {}
                                        invoiceObj.orderSubmitted = value.invoiceDate;
                                        invoiceObj.trackingNumber = value.shipmentReference;
                                        invoiceObj.shipmentId = value.invoiceNumber;
                                        invoiceObj.carrier = value.invoiceValue;
                                        invoiceObj.originCity = value.invoiceStatus;                                   

                                        vm.exportcollection.push(invoiceObj);
                                    });







                                }, function errorCallback(response) {
                                    //todo
                                });
                        };

                        vm.loadInvoicesByStatus = function (status) {
                            vm.loadInvoicesBySearch(status);
                        };


                        vm.loadInvoicesBySearch();

                        vm.exportInvoiceReport = function () {
                            
                            customerInvoiceFactory.exportInvoiceReport(vm.rowCollection)
                                      .success(function (data, status, headers) {
                                          var octetStreamMime = 'application/octet-stream';
                                          var success = false;

                                          // Get the headers
                                          headers = headers();

                                          // Get the filename from the x-filename header or default to "download.bin"
                                          var filename = headers['x-filename'] || 'MyInvoiceReport.xlsx';

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

                        var paymentForm;
                        initializePaymentForm();

                        function initializePaymentForm() {
                            
                            shipmentFactory.getSquareApplicationId().success(
                               function (responce) {

                                   paymentForm = new SqPaymentForm({
                                       applicationId: responce,
                                       inputClass: 'sq-input',
                                       inputStyles: [
                                         {
                                             fontSize: '15px'
                                         }
                                       ],
                                       cardNumber: {
                                           elementId: 'sq-card-number',
                                           placeholder: '•••• •••• •••• ••••'
                                       },
                                       cvv: {
                                           elementId: 'sq-cvv',
                                           placeholder: 'CVV'
                                       },
                                       expirationDate: {
                                           elementId: 'sq-expiration-date',
                                           placeholder: 'MM/YY'
                                       },
                                       postalCode: {
                                           elementId: 'sq-postal-code'
                                       },
                                       callbacks: {

                                           // Called when the SqPaymentForm completes a request to generate a card
                                           // nonce, even if the request failed because of an error.
                                           cardNonceResponseReceived: function (errors, nonce, cardData) {
                                               debugger;
                                               if (errors) {
                                                   console.log("Encountered errors:");

                                                   // This logs all errors encountered during nonce generation to the
                                                   // Javascript console.
                                                   errors.forEach(function (error) {
                                                       console.log('  ' + error.message);
                                                   });

                                                   // No errors occurred. Extract the card nonce.
                                               } else {

                                                   debugger;
                                                   var body = $("html, body");

                                                   customerInvoiceFactory.payInvoice({ Id: vm.invoiceId, CardNonce: nonce, UserId: $window.localStorage.getItem('userGuid') })
                                                    .success(function (response) {
                                                        debugger;
                                                        if (response.result == 2) {
                                                            // Payment is success
                                                            currentRow.invoiceStatus = "Paid";
                                                        }
                                                        else {
                                                            // Payment error
                                                            $('#panel-notif').noty({
                                                                text: '<div class="alert alert-danger media fade in"><p>' + response.message + '!</p></div>',
                                                                layout: 'bottom-right',
                                                                theme: 'made',
                                                                animation: {
                                                                    open: 'animated bounceInLeft',
                                                                    close: 'animated bounceOutLeft'
                                                                },
                                                                timeout: 6000,
                                                            });
                                                        
                                                        }
                                                        
                                                    })
                                                    .error(function () {
                                                    })
                                               }
                                           },

                                           unsupportedBrowserDetected: function () {
                                               // Fill in this callback to alert buyers when their browser is not supported.
                                           },

                                           // Fill in these cases to respond to various events that can occur while a
                                           // buyer is using the payment form.
                                           inputEventReceived: function (inputEvent) {
                                               switch (inputEvent.eventType) {
                                                   case 'focusClassAdded':
                                                       // Handle as desired
                                                       break;
                                                   case 'focusClassRemoved':
                                                       // Handle as desired
                                                       break;
                                                   case 'errorClassAdded':
                                                       // Handle as desired
                                                       break;
                                                   case 'errorClassRemoved':
                                                       // Handle as desired
                                                       break;
                                                   case 'cardBrandChanged':
                                                       // Handle as desired
                                                       break;
                                                   case 'postalCodeChanged':
                                                       // Handle as desired
                                                       break;
                                               }
                                           },

                                           paymentFormLoaded: function () {
                                               // Fill in this callback to perform actions after the payment form is
                                               // done loading (such as setting the postal code field programmatically).
                                               
                                           }
                                       }
                                   });

                               }).error(function (error) {

                               });

                        }

                        vm.isShowPaymentForm = false;
                        vm.invoiceId;
                        var currentRow;
                        vm.payInvoice = function (row) {
                            //var statusChange = confirm("Are you sure you need to pay this invoice ?");
                            paymentForm.build();
                            vm.isShowPaymentForm = true;
                            vm.invoiceId = row.id;
                            currentRow = row;
                            //if (statusChange == true) {
                                
                            //}
                        };

                        vm.chargeFromCard = function () {

                            //vm.loadingSymbole = true;
                            paymentForm.requestCardNonce();

                        }

                        vm.disputeInvoice = function (row) {

                            // var statusChange = confirm("Are you sure you need to dispute this invoice ?");

                            $('#panel-notif').noty({
                                text: '<div class="alert alert-success media fade in"><p>' + $rootScope.translate('Are you want to Dispute the Invoice ') + row.invoiceNumber + '?</p></div>',
                                buttons: [
                                        {
                                            addClass: 'btn btn-primary', text: $rootScope.translate('Ok'), onClick: function ($noty) {
                                                row.disputeComment = '';

                                                ngDialog.open({
                                                    scope: $scope,
                                                    template: '/app/billingandInvoicing/DisputeInvoice.html',
                                                    className: 'ngdialog-theme-plain custom-width',
                                                    controller: $controller('disputeInvoiceCtrl', {
                                                        $scope: $scope,
                                                        invoice: row
                                                    })

                                                });


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



                            //if (statusChange == true) {
                            //    customerInvoiceFactory.disputeInvoice({ Id: row.id })
                            //        .success(function (response) {

                            //            row.invoiceStatus = response;
                            //        })
                            //        .error(function () {
                            //        })
                            //}
                        };

                    }]);


})(angular.module('newApp'));