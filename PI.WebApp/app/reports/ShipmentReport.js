'use strict';
(function (app) {


    app.factory('ShipmentReportFactory', function ($http, $window) {
        return {
            exportShipmentReport: function () {
                return $http.get(serverBaseUrl + '/api/Admin/GetShipmentDetails', {
                    params: {
                        userId: $window.localStorage.getItem('userGuid')
                    },
                    responseType: 'arraybuffer'
                });
            }
        };
    });


    app.controller('shipReportCtrl', ['$scope', '$location', 'ShipmentReportFactory', '$window', '$sce',
                  function ($scope, $location, ShipmentReportFactory, $window, $sce) {
                      var vm = this;
                      vm.stream = {};


                      vm.exportCSV = function () {
                          debugger;
                          // Get values from view.
                          var userId = $window.localStorage.getItem('userGuid');
                          var type = (vm.state == undefined) ? "" : vm.state;
                          var searchText = vm.searchText;

                          ShipmentReportFactory.exportShipmentReport(userId, searchText, type)
                              .then(function successCallback(responce) {

                                  //adding headers for export csv file
                                  var headers = {};
                                  headers.id = "Id";
                                  headers.companyName = "companyName";
                                  headers.userId = "userId";
                                  headers.salutation = "salutation";
                                  headers.firstName = "firstName";
                                  headers.lastName = "lastName";
                                  headers.emailAddress = "emailAddress";
                                  headers.phoneNumber = "phoneNumber";
                                  headers.accountNumber = "accountNumber";
                                  headers.fullName = "fullName";
                                  headers.fullAddress = "fullAddress";

                                  headers.country = "country";
                                  headers.zipCode = "zipCode";
                                  headers.number = "number";
                                  headers.streetAddress1 = "streetAddress1";
                                  headers.streetAddress2 = "streetAddress2";
                                  headers.city = "city";
                                  headers.state = "state";
                                  headers.isActive = "isActive";

                                  vm.exportcollection = [];
                                  vm.exportcollection.push(headers);

                                  $.each(responce.data.content, function (index, value) {
                                      vm.exportcollection.push(value);
                                  });
                              },
                              function errorCallback(response) {
                                  //todo
                              });
                      };


                      vm.exportExcel = function () {
                          debugger;
                          ShipmentReportFactory.exportShipmentReport()
                          .success(function (data, status, headers) {

                              var octetStreamMime = 'application/octet-stream';
                              var success = false;

                              // Get the headers
                              headers = headers();

                              // Get the filename from the x-filename header or default to "download.bin"
                              var filename = headers['x-filename'] || 'ShipmentDetailsReport.xlsx';

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


                      $scope.renderHtml = function (html_code) {
                          return $sce.trustAsHtml(html_code);
                      };


                  }]);

})(angular.module('newApp'));