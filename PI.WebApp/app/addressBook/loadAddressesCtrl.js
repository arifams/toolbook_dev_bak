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
      
    app.controller('loadAddressesCtrl', ['$scope', '$location', 'loadAddressService', 'addressManagmentService', '$routeParams', '$log', '$window', '$sce', 'importAddressBookFactory', function ($scope, $location, loadAddressService, addressManagmentService, $routeParams, $log, $window, $sce, importAddressBookFactory) {
       var vm = this;
        
        vm.searchAddresses = function () {

            // Get values from view.
            var userId = $window.localStorage.getItem('userGuid');            
            var type = (vm.state == undefined) ? "" : vm.state;
            var searchText = vm.searchText;

            loadAddressService.find(userId,searchText,type)
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
                        debugger;
                        var t = responce.data.content[index].fullName;
                        debugger;
                        if (!responce.data.content[index].fullName || !responce.data.content[index].fullAddress)
                            vm.exportcollection.push(value);
                    });
                    //loop through the address collection to remove the fullname and fulladdress properties
                    //$.each(vm.exportcollection, function (index, value) {
                    //    delete vm.exportcollection[index].fullName;
                    //    delete vm.exportcollection[index].fullAddress;
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
                    var address = {"csvContent": value[0] };
                    importCollection.push(address);
                });
             
                importAddressBookFactory.importAddressBook(importCollection).then(function successCallback(responce) {
                    var body = $("html, body");
                    if (responce) {

                        body.stop().animate({ scrollTop: 0 }, '500', 'swing', function () {
                        });
                        $('#panel-notif').noty({
                            text: '<div class="alert alert-success media fade in"><p>' + responce.data+' Address records added.' + '</p></div>',
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
                alert("No file uploaded");
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
  
        //detete address detail
        vm.deleteById = function (row) {

            var r = confirm("Do you want to delete the record?");
            if (r == true) {
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
            }
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