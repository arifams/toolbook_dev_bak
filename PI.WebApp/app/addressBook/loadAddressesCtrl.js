'use strict';
(function (app) {

    app.factory('addressManagmentService', function ($http) {
        return {
            deleteAddress: function (address) {
                return $http.post(serverBaseUrl + '/api/AddressBook/DeleteAddress', address);
            }
        };
    });

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
      
    app.controller('loadAddressesCtrl', ['$scope','$location', 'loadAddressService', 'addressManagmentService', '$routeParams', '$log', '$window', '$sce', function ($scope, $location, loadAddressService, addressManagmentService, $routeParams, $log, $window, $sce) {
       var vm = this;
        
        vm.searchAddresses = function () {

            // Get values from view.
            var userId = $window.localStorage.getItem('userGuid');            
            var type = (vm.state == undefined) ? "" : vm.state;
            var searchText = vm.searchText;

            loadAddressService.find(userId,searchText,type)
                .then(function successCallback(responce) {

                    vm.rowCollection = responce.data.content;
                }, function errorCallback(response) {
                    //todo
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