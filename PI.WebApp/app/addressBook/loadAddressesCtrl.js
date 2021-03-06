﻿'use strict';
(function (app) {

    app.factory('addressManagmentService', function ($http) {
        return {
            deleteAddress: function (address) {
                return $http.post(serverBaseUrl + '/api/AddressBook/DeleteAddress', address);
            }
        };
    });

    app.factory('importAddressBookFactory', function ($http, $window) {
        return {
            importAddressBook: function (addressDetails) {
                return $http.post(serverBaseUrl + '/api/AddressBook/ImportAddresses', addressDetails, {
                    params: {
                        userId: $window.localStorage.getItem('userGuid')
                    }
                });
            }
        };
    });

    app.factory('exportAddressExcelFactory', function ($http, $window) {
        return {
            importAddressBookExcel: function (userId, searchText, type) {
                return $http.get(serverBaseUrl + '/api/AddressBook/GetAddressBookDetailsExcel', {
                    params: {
                        userId: userId,
                        searchtext: searchText,
                        type: type
                    },
                    responseType: 'arraybuffer'
                });
            }
        };
    })

    app.factory('loadAddressService', function ($http, $q, $log, $rootScope) {

        var baseUrl = serverBaseUrl + '/api/AddressBook/GetAllAddressBookDetailsByFilter';

        return {
            find: function (userId, searchText, type) {
                return $http.get(serverBaseUrl + '/api/AddressBook/GetAllAddressBookDetailsByFilter', {
                    params: {
                        userId: userId,
                        searchtext: searchText,
                        type: type
                    }
                });
            }
        }
    });

    app.controller('loadAddressesCtrl', ['$route', '$scope', '$location', 'loadAddressService', 'addressManagmentService', '$routeParams', '$log', '$window', '$sce', 'importAddressBookFactory', 'exportAddressExcelFactory', 'Upload', '$timeout', '$rootScope', 'ngDialog', '$controller', 'customBuilderFactory',
        function ($route, $scope, $location, loadAddressService, addressManagmentService, $routeParams, $log, $window, $sce, importAddressBookFactory, exportAddressExcelFactory, Upload, $timeout, $rootScope, ngDialog, $controller, customBuilderFactory) {
            var vm = this;
            vm.stream = {};
            vm.noAvailableAddressDetails = false;
            vm.loading = false;
            vm.loadingSymbole = true;

            //toggle function
            vm.loadFilterToggle = function () {
                customBuilderFactory.customFilterToggle();

            };

            vm.searchAddresses = function () {
                vm.loading = true;
                vm.loadingSymbole = true;
                // Get values from view.
                var userId = $window.localStorage.getItem('userGuid');
                var type = (vm.state == undefined) ? "" : vm.state;
                var searchText = vm.searchText;

                loadAddressService.find(userId, searchText, type)
                    .then(function successCallback(responce) {
                        vm.loading = false;
                        vm.loadingSymbole = false;
                        vm.rowCollection = responce.data.content;
                        if (vm.rowCollection.length == 0) {
                            vm.noAvailableAddressDetails = true;
                        } else {
                            vm.noAvailableAddressDetails = false;
                        }
                        vm.exportcollection = [];

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

                        vm.exportcollection.push(headers);

                        $.each(responce.data.content, function (index, value) {

                            vm.exportcollection.push(value);
                        });
                        //loop through the address collection to remove the fullname and fulladdress properties
                        //$.each(vm.exportcollection, function (index, value) {

                        //     vm.exportcollection[index].pop("fullName");
                        //     vm.exportcollection[index].pop("fullAddress");
                        //});

                    }, function errorCallback(response) {
                        //todo
                        vm.loading = false;
                    });
            };

            vm.searchAddressesfor = function () {

                // Get values from view.
                var userId = $window.localStorage.getItem('userGuid');
                var type = (vm.state == undefined) ? "" : vm.state;
                var searchText = vm.searchText;

                loadAddressService.find(userId, searchText, type)
                    .then(function successCallback(responce) {

                        vm.rowCollection = responce.data.content;

                    }, function errorCallback(response) {
                        //todo
                    });
            };

            // Call search function in page load.
            vm.searchAddresses();

            vm.ExportExcel = function () {

                var userId = $window.localStorage.getItem('userGuid');
                var type = (vm.state == undefined) ? "" : vm.state;
                var searchText = vm.searchText;

                exportAddressExcelFactory.importAddressBookExcel(userId, searchText, type)
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
            }

            vm.importAddressBook = function () {

                ngDialog.open({
                    scope: $scope,
                    template: '/app/addressBook/AddressBookImport.html',
                    className: 'ngdialog-theme-plain custom-width',
                    controller: $controller('addressBookImportCtrl', {
                        $scope: $scope,
                        parent: $scope

                    })

                });

            }


            //detete address detail
            vm.deleteById = function (row) {

                $('#panel-notif').noty({
                    text: '<div class="alert alert-success media fade in"><p>' + $rootScope.translate('Are you sure you want to delete this address') + ' ?</p></div>',
                    buttons: [
                            {
                                addClass: 'btn btn-primary', text: $rootScope.translate('Ok'), onClick: function ($noty) {

                                    addressManagmentService.deleteAddress({ Id: row.id })
                                    .then(function (response) {
                                        
                                        if (response.status == 200) {
                                            var index = vm.rowCollection.indexOf(row);
                                            vm.rowCollection.splice(index, 1);
                                        }
                                    },
                                        function (error) {
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


            };

            $scope.renderHtml = function (html_code) {
                return $sce.trustAsHtml(html_code);
            };

            vm.closeWindow = function () {
                
                ngDialog.close()
            }

            vm.excelUploadSucces = function () {

                var body = $("html, body");
                body.stop().animate({ scrollTop: 0 }, '500', 'swing', function () {
                });
                
                $('#panel-notif').noty({
                    text: '<div class="alert alert-success media fade in"><p>' + ' ' + $rootScope.translate('Address records added successfully') + '.</p></div>',
                    buttons: [
                            {
                                addClass: 'btn btn-primary', text: $rootScope.translate('Ok'), onClick: function ($noty) {

                                    $route.reload();
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

            vm.showError = function (response) {
                vm.errorMsg = response.status + ': ' + response.data;
                $('#panel-notif').noty({
                    text: '<div class="alert alert-warning media fade in"><p>' + $rootScope.translate('Upload Failed') + '</p></div>',
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

            vm.csvImportResults = function (responce) {

                var body = $("html, body");
                if (responce.data != -1) {
                    body.stop().animate({ scrollTop: 0 }, '500', 'swing', function () {
                    });

                    $('#panel-notif').noty({
                        text: '<div class="alert alert-success media fade in"><p>' + responce.data + ' ' + $rootScope.translate('Address records added successfully') + '.</p></div>',
                        buttons: [
                                {
                                    addClass: 'btn btn-primary', text: $rootScope.translate('Ok'), onClick: function ($noty) {
                                        $route.reload();
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

                } else {
                    $('#panel-notif').noty({
                        text: '<div class="alert alert-warning media fade in"><p> ' + $rootScope.translate('Invalid data import format') + '.</p></div>',
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
            }
            vm.itemsByPage = 25;
            vm.rowCollection = [];
            // Add dumy record, since data loading is async.
            //vm.rowCollection.push(1);
            vm.showErrorCsv = function () {

                $('#panel-notif').noty({
                    text: '<div class="alert alert-warning media fade in"><p>' + $rootScope.translate('No File uploaded for import') + '</p></div>',
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



        }]);

})(angular.module('newApp'));