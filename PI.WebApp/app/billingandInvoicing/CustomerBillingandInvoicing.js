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

    app.controller('customerinvoiceCtrl', ['$location', '$window', 'customerInvoiceFactory', 'ngDialog', '$controller', '$scope', '$rootScope', 'customBuilderFactory',
                    function ($location, $window, customerInvoiceFactory, ngDialog, $controller, $scope, $rootScope, customBuilderFactory) {
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

                        vm.payInvoice = function (row) {
                            var statusChange = confirm("Are you sure you need to pay this invoice ?");

                            if (statusChange == true) {
                                customerInvoiceFactory.payInvoice({ Id: row.id })
                                    .success(function (response) {

                                        row.invoiceStatus = response;
                                    })
                                    .error(function () {
                                    })
                            }
                        };

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


                        vm.callServerSearch = function (tableState) {
                            debugger;
                            var pagination = 0;//tableState.pagination;

                            var start = pagination.start || 0;     // This is NOT the page number, but the index of item in the list that you want to use to display the table.
                            var number = pagination.number || 10;  // Number of entries showed per page.

                            vm.loadInvoicesBySearch(vm.status);
                        };

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