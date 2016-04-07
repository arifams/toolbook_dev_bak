'use strict';
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
            importAddressBookExcel: function () {
                return $http.get(serverBaseUrl + '/api/AddressBook/GetAddressBookDetailsExcel', {
                    params: {
                        userId: $window.localStorage.getItem('userGuid')
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

    app.controller('loadAddressesCtrl', ['$route', '$scope', '$location', 'loadAddressService', 'addressManagmentService', '$routeParams', '$log', '$window', '$sce', 'importAddressBookFactory', 'exportAddressExcelFactory', function ($route, $scope, $location, loadAddressService, addressManagmentService, $routeParams, $log, $window, $sce, importAddressBookFactory, exportAddressExcelFactory) {
        var vm = this;
        vm.stream = {};

        vm.searchAddresses = function () {

            // Get values from view.
            var userId = $window.localStorage.getItem('userGuid');
            var type = (vm.state == undefined) ? "" : vm.state;
            var searchText = vm.searchText;

            loadAddressService.find(userId, searchText, type)
                .then(function successCallback(responce) {

                    vm.rowCollection = responce.data.content;
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
                });
        };

        vm.Import = function () {
            var importCollection = [];
            if (vm.csv) {
                var addressList = vm.csv.result;

                $.each(addressList, function (index, value) {
                    var address = { "csvContent": value[0] };
                    importCollection.push(address);
                });

                importAddressBookFactory.importAddressBook(importCollection).then(function successCallback(responce) {
                    var body = $("html, body");
                    if (responce.data != -1) {
                        body.stop().animate({ scrollTop: 0 }, '500', 'swing', function () {
                        });

                        $('#panel-notif').noty({
                            text: '<div class="alert alert-success media fade in"><p>' + responce.data + ' Address records added successfully.' + '</p></div>',
                            buttons: [
                                    {
                                        addClass: 'btn btn-primary', text: 'Ok', onClick: function ($noty) {
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
                            text: '<div class="alert alert-warning media fade in"><p> Invalid data import format.</p></div>',
                            buttons: [
                                    {
                                        addClass: 'btn btn-primary', text: 'Ok', onClick: function ($noty) {

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
                }, function errorCallback(response) {
                    //todo
                });;
            } else {
                //  alert("No file uploaded");
                $('#panel-notif').noty({
                    text: '<div class="alert alert-warning media fade in"><p>No File uploaded for import</p></div>',
                    buttons: [
                            {
                                addClass: 'btn btn-primary', text: 'Ok', onClick: function ($noty) {

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
            exportAddressExcelFactory.importAddressBookExcel()
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


        vm.uploadFile = function (file) {
            debugger;
            file.upload = Upload.upload({
                url: serverBaseUrl + '/api/Shipments/upload',
                data: {
                    file: file,
                    userId: $window.localStorage.getItem('userGuid'),
                    documentType: "AddressBook",                   
                },
            });

            file.upload.then(function (response) {
                $timeout(function () {
                    file.result = response.data;
                    deleteFile();
                });
            }, function (response) {
                if (response.status > 0)
                    vm.errorMsg = response.status + ': ' + response.data;
            }, function (evt) {
                // Math.min is to fix IE which reports 200% sometimes
                file.progress = Math.min(100, parseInt(100.0 * evt.loaded / evt.total));
            });
        }

        //detete address detail
        vm.deleteById = function (row) {

            $('#panel-notif').noty({
                text: '<div class="alert alert-success media fade in"><p>Are you want to delete?</p></div>',
                buttons: [
                        {
                            addClass: 'btn btn-primary', text: 'Ok', onClick: function ($noty) {

                                addressManagmentService.deleteAddress({ Id: row.id })
                                .success(function (response) {
                                    if (response == 1) {
                                        var index = vm.rowCollection.indexOf(row);
                                        if (index !== -1) {
                                            vm.rowCollection.splice(index, 1);
                                        }
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

        $scope.renderHtml = function (html_code) {
            return $sce.trustAsHtml(html_code);
        };

        vm.itemsByPage = 25;
        vm.rowCollection = [];
        // Add dumy record, since data loading is async.
        vm.rowCollection.push(1);

    }]);

})(angular.module('newApp'));