'use strict';

(function (app) {

    app.factory('customerInvoiceFactory', ['$http', '$routeParams', '$window', function ($http, $routeParams, $window) {

        return {
            getAllInvoicesByCustomer: getAllInvoicesByCustomer,
            payInvoice: payInvoice,
            disputeInvoice: disputeInvoice,
            exportInvoiceReport: exportInvoiceReport
        };

        function getAllInvoicesByCustomer(status, startDate, endDate, searchValue) {

            return $http.get(serverBaseUrl + '/api/Customer/GetAllInvoicesByCustomer', {
                params: {
                    status: status,
                    userId: $window.localStorage.getItem('userGuid'),
                    startDate: startDate,
                    endDate: endDate,
                    searchValue: searchValue
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

        function exportInvoiceReport(status, startDate, endDate, searchValue) {

            return $http.get(serverBaseUrl + '/api/Customer/ExportInvoiceReport', {
                params: {
                    status: status,
                    userId: $window.localStorage.getItem('userGuid'),
                    startDate: startDate,
                    endDate: endDate,
                    searchValue: searchValue
                },
                responseType: 'arraybuffer'
            });
            
           
        }

    }]);

    app.controller('customerinvoiceCtrl', ['$location', '$window', 'customerInvoiceFactory', 'ngDialog', '$controller', '$scope', '$rootScope', 'customBuilderFactory','shipmentFactory',
                    function ($location, $window, customerInvoiceFactory, ngDialog, $controller, $scope, $rootScope, customBuilderFactory, shipmentFactory) {
                        var vm = this;
                        vm.datePicker = {};
                        vm.datePicker.date = { startDate: null, endDate: null };
                        vm.status = 'All';
                        vm.loadingSymbole = true;

                        //toggle function
                        vm.loadFilterToggle = function () {
                            customBuilderFactory.customFilterToggle();

                        };

                        vm.closeWindow = function () {
                            ngDialog.close()
                        }
                        

                        vm.loadInvoicesBySearch = function (status) {
                            vm.loadingSymbole = true;
                            var status = (status == undefined || status == 'All' || status == null || status == "") ? null : status;
                            var startDate = (vm.datePicker.date.startDate == null) ? null : vm.datePicker.date.startDate.toDate();
                            var endDate = (vm.datePicker.date.endDate == null) ? null : vm.datePicker.date.endDate.toDate();
                            var searchValue = (vm.searchValue == undefined || vm.searchValue == null || vm.searchValue == "") ? null : vm.searchValue;
                            vm.statusSelected = status;

                            customerInvoiceFactory.getAllInvoicesByCustomer(status, startDate, endDate, searchValue)
                                .then(function successCallback(responce) {

                                    debugger;
                                    vm.loadingSymbole = false;
                                    vm.rowCollection = responce.data.content;
                                    vm.exportcollection = [];

                                    //adding headers for export csv file
                                    var headers = {};
                                    headers.orderSubmitted = "Invoice Date";
                                    headers.invoiceNumber = "Invoice Number";
                                    headers.shipmentId = "Shipment ID";
                                    headers.value = "Invoice Value";
                                    headers.sum = "Sum";
                                    headers.invoiceStatus = "Invoice Status";
                                    headers.creditedValue = "credited value";
                                    headers.url = "URL";
                                   
                                    vm.exportcollection.push(headers);

                                    $.each(responce.data.content, function (index, value) {                                      
                                        var invoiceObj = {}
                                        invoiceObj.orderSubmitted = value.invoiceDate;
                                        invoiceObj.invoiceNumber = value.invoiceNumber;
                                        invoiceObj.shipmentId = value.shipmentReference;
                                        invoiceObj.value = value.invoiceValue;
                                        invoiceObj.sum = value.sum;                                        
                                        invoiceObj.invoiceStatus = value.invoiceStatus;
                                        invoiceObj.creditedValue = value.creditedValue;
                                        invoiceObj.url = value.url;
                                       

                                        vm.exportcollection.push(invoiceObj);
                                    });                                    

                                }, function errorCallback(response) {
                                    vm.loadingSymbole = false;
                                    //todo
                                });
                        };
                        
                        vm.exportInvoiceReport = function () {
                            
                            var status = (vm.statusSelected == undefined || vm.statusSelected == 'All' || vm.statusSelected == null || vm.statusSelected == "") ? null : vm.statusSelected;
                            var startDate = (vm.datePicker.date.startDate == null) ? null : vm.datePicker.date.startDate.toDate();
                            var endDate = (vm.datePicker.date.endDate == null) ? null : vm.datePicker.date.endDate.toDate();
                            var searchValue = (vm.searchValue == undefined || vm.searchValue == null || vm.searchValue == "") ? null : vm.searchValue;


                            customerInvoiceFactory.exportInvoiceReport(status, startDate, endDate, searchValue)
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
                                                        
                                                        

                                                        if (response.status == 2) {
                                                            vm.isShowPaymentForm = false;
                                                            // Payment is success
                                                            currentRow.invoiceStatus = "Paid";
                                                            $('#panel-notif').noty({
                                                                text: '<div class="alert alert-success media fade in"><p>' + 'Transaction is completed' + '!</p></div>',
                                                                layout: 'bottom-right',
                                                                theme: 'made',
                                                                animation: {
                                                                    open: 'animated bounceInLeft',
                                                                    close: 'animated bounceOutLeft'
                                                                },
                                                                timeout: 6000,
                                                            });
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
                                text: '<div class="alert alert-success media fade in"><p>' + $rootScope.translate('Are you sure you want to dispute invoice: ') + row.invoiceNumber + '?</p></div>',
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



                            //if (statusChange == true) {
                            //    customerInvoiceFactory.disputeInvoice({ Id: row.id })
                            //        .success(function (response) {

                            //            row.invoiceStatus = response;
                            //        })
                            //        .error(function () {
                            //        })
                            //}
                        };


                        vm.callServerSearch = function (tableState) {
                            debugger;
                            var pagination = 0;//tableState.pagination;

                            var start = pagination.start || 0;     // This is NOT the page number, but the index of item in the list that you want to use to display the table.
                            var number = pagination.number || 10;  // Number of entries showed per page.

                            vm.loadInvoicesBySearch(vm.status);
                        };

                        vm.callServerSearch();

                        vm.resetSearch = function (tableState) {
                            debugger;
                            var pagination = 0;//tableState.pagination;

                            var start = pagination.start || 0;     // This is NOT the page number, but the index of item in the list that you want to use to display the table.
                            var number = pagination.number || 10;  // Number of entries showed per page.

                            vm.status = 'All';
                            vm.datePicker.date = { "startDate": null, "endDate": null };
                            //vm.datePicker.date.endDate = null;

                            vm.loadInvoicesBySearch();
                        }

                    }]);


})(angular.module('newApp'));