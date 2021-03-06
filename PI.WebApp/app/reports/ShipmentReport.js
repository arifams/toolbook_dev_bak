﻿'use strict';
(function (app) {


    app.factory('ShipmentReportFactory', function ($http, $window) {
        return {
            exportShipmentReport: function (carrierId, companyId, startDate, endDate, status, countryOfOrigin, countryOfDestination, product, packageType) {
                
                return $http.get(serverBaseUrl + '/api/shipments/GetShipmentDetails', {
                    params: {
                        userId: $window.localStorage.getItem('userGuid'),
                        carrierId: carrierId,
                        companyId: companyId,
                        startDate: startDate,
                        endDate: endDate,
                        status: status,
                        countryOfOrigin: countryOfOrigin,
                        countryOfDestination: countryOfDestination,
                        product: product,
                        packageType: packageType
                    },
                    responseType: 'arraybuffer'
                });
            }
        };
    });


    app.factory('ShipmentReportCSVFactory', function ($http, $window) {
        return {
            exportShipmentReportcsv: function (carrierId, companyId, startDate, endDate) {
                return $http.get(serverBaseUrl + '/api/shipments/GetShipmentDetailsForCSV', {
                    params: {
                        userId: $window.localStorage.getItem('userGuid'),
                        carrierId: carrierId,
                        companyId: companyId,
                        startDate: startDate,
                        endDate: endDate
                    }
                });
            }
        };
    });

    app.factory('CarrierFactory', function ($http, $window) {
        return {
            loadAllCarriers: function () {
                return $http.get(serverBaseUrl + '/api/shipments/LoadAllCarriers');
            }
        };
    });


    app.controller('shipReportCtrl', ['$scope', '$location', 'ShipmentReportFactory', '$window', '$sce', 'shipmentFactory','$rootScope',
                                     'ngDialog', '$controller', 'ShipmentReportCSVFactory', 'CarrierFactory',
    function ($scope, $location, ShipmentReportFactory, $window, $sce, shipmentFactory, $rootScope, ngDialog, $controller,
        ShipmentReportCSVFactory, CarrierFactory) {

        var vm = this;
        vm.stream = {};
        vm.selectedCompanyId = '';
        vm.searchText = '';
        vm.emptySearch = false;

        vm.isAdmin = ($window.localStorage.getItem('userRole') == "Admin") ? true : false;
        vm.loadingSymbole = false;


       

        var loadAllCarriers = function () {
            CarrierFactory.loadAllCarriers().success(
            function (response) {
                vm.carrierList = response;
                
            });
        }

        vm.closeWindow = function () {
            ngDialog.close()
        }

        vm.loadAllCompanies = function (search) {
            var from = 'shipReportCtrl'

            shipmentFactory.loadAllcompanies(search).success(
               function (responce) {
                   
                   if (responce.content.length > 0) {

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
                       vm.emptySearch = true;

                       
                       $('#panel-notif').noty({
                           text: '<div class="alert alert-danger media fade in"><p>' + $rootScope.translate('Customers not found for this search') + '!</p></div>',
                           layout: 'bottom-right',
                           theme: 'made',
                           animation: {
                               open: 'animated bounceInLeft',
                               close: 'animated bounceOutLeft'
                           },
                           timeout: 2000,
                       });

                   }
               }).error(function (error) {

                   console.log("error occurd while retrieving Addresses");
               });

        }

        vm.statusList = [
            { Id: 0, Name: "All" },
            { Id: 1, Name: "Error" },
            { Id: 2, Name: "Pending" },
            { Id: 3, Name: "Booking confirmation" },
            { Id: 4, Name: "Pickup" },
            { Id: 5, Name: "Transit" },
            { Id: 6, Name: "Out for delivery" },
            { Id: 7, Name: "Delivered" },
            { Id: 8, Name: "Deleted" },
            { Id: 9, Name: "Exception" },
            { Id: 10, Name: "Claim" }
        ];
        vm.status = 0;

        vm.productList = [
            { Id: 0, Name: "All" },
            { Id: 1, Name: "Express" },
            { Id: 2, Name: "Air Freight" },
            { Id: 3, Name: "Sea Freight" },
            { Id: 4, Name: "Road Freight" }
        ];
        vm.product = 0;

        vm.packageTypeList = [
            { Id: 0, Name: "All" },
            { Id: 1, Name: "Box" },
            { Id: 2, Name: "Document" },
            { Id: 3, Name: "Pallet" },
            { Id: 4, Name: "Euro Pallet" },
            { Id: 5, Name: "Diverse" }
        ];
        vm.packageType = 0;

        vm.exportExcel = function () {

            debugger;
            var from = Date.parse(vm.dateFrom);
            var to = Date.parse(vm.dateTo);


            if (from > to) {

                $('#panel-notif').noty({
                    text: '<div class="alert alert-danger media fade in"><p>' + $rootScope.translate('Invalid Date Range') + '!</p></div>',
                    layout: 'bottom-right',
                    theme: 'made',
                    animation: {
                        open: 'animated bounceInLeft',
                        close: 'animated bounceOutLeft'
                    },
                    timeout: 2000,
                });

                return;
            }
            
            if (vm.isNeedSearchCustomer) {
                var n = noty({
                    text: '<p style="font-size:medium;">' + 'Please click search button to select a customer or clear customer fileds to search on all customers' + '! </p>',
                    layout: 'center',
                    type: 'error',
                    animation: {
                        open: { height: 'toggle' },
                        close: { height: 'toggle' }, // jQuery animate function property object
                        easing: 'swing', // easing
                        speed: 500 // opening & closing animation speed
                    }
                });

                return;
            }

            var carrierId = vm.carrierId;
            var companyId = vm.selectedCompanyId == "" ? null : vm.selectedCompanyId;
            var startDate = vm.dateFrom;
            var endDate = vm.dateTo;

            vm.loadingSymbole = true;

            ShipmentReportFactory.exportShipmentReport(carrierId, companyId, startDate, endDate, vm.status, vm.countryOfOrigin, vm.countryOfDestination, vm.product, vm.packageType)
            .success(function (data, status, headers) {

                vm.loadingSymbole = false;

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

              vm.loadingSymbole = false;

              console.log("Request failed with status: " + status);
              
              // Optionally write the error out to scope
              $scope.errorDetails = "Request failed with status: " + status;
          });
        }

        $scope.renderHtml = function (html_code) {
            return $sce.trustAsHtml(html_code);
        };

        loadAllCarriers();     
      
        vm.isNeedSearchCustomer = false;

        vm.searchTextChange = function () {
            if (vm.searchText == '' || vm.searchText == undefined) {
                vm.isNeedSearchCustomer = false;
                vm.selectedCompanyId = '';
            }
            else {
                vm.isNeedSearchCustomer = true;
            }
        };

    }]);

})(angular.module('newApp'));