'use strict';
(function (app) {

    app.factory('saveAddressBookFactory', function ($http) {
        return {
            saveAddressBook: function (addressDetail) {
                return $http.post(serverBaseUrl + '/api/AddressBook/SaveAddress', addressDetail);
            }
        };
    })

    app.factory('loadAddressBookFactory', function ($http, $routeParams, $window, jwtHelper) {
        return {
            loadAddressInfo: function () {
                var token= $window.localStorage.getItem('token')
                
                var tokenPayload = jwtHelper.decodeToken(token);


                return $http.get(serverBaseUrl + '/api/AddressBook/LoadAddress', {
                    params: {
                        id: $routeParams.id,
                        userId: $window.localStorage.getItem('userGuid')
                    }
                });
            }
        }
    })

    app.controller('saveAddressCtrl', ['saveAddressBookFactory', 'loadAddressBookFactory', '$location', '$window', '$routeParams', '$rootScope', function (saveAddressBookFactory, loadAddressBookFactory, $location, $window, $routeParams, $rootScope) {

        var vm = this;
       


        vm.changeCountry = function () {
            vm.isRequiredState = vm.model.country == 'US' || vm.model.country == 'CA' || vm.model.country == 'PR' || vm.model.country == 'AU';
        };      

        vm.saveAddressDetail = function () {
            vm.model.userId = $window.localStorage.getItem('userGuid');
            vm.model.id = $routeParams.id;
            var body = $("html, body");
            saveAddressBookFactory.saveAddressBook(vm.model)
           .success(function (result) {
               if (result == 1) {
                   body.stop().animate({ scrollTop: 0 }, '500', 'swing', function () {
                   });
                    $('#panel-notif').noty({
                        text: '<div class="alert alert-success media fade in"><p>' + $rootScope.translate('Address Detail saved successfully') + '</p></div>',
                        layout: 'bottom-right',
                        theme: 'made',
                        animation: {
                            open: 'animated bounceInLeft',
                            close: 'animated bounceOutLeft'
                        },
                        timeout: 3000,
                    });
                } 

                })
                .error(function () {
                })
        }
        vm.close = function () {
            $location.path('/loadAddresses');
        }

        var loadDivision = function () {
            loadAddressBookFactory.loadAddressInfo()
            .success(function (data) {
                
                vm.model = data;
                if (data.isActive == true || data.firstName==null) {
                    vm.model.isActive = "true";
                } else {
                    vm.model.isActive = "false";
                }

                //if (vm.model.id == 0) {
                //   // vm.model.status = 1;
                //    vm.model.isActive = "true";
                //}
            })
            .error(function () {
            })
        }

        loadDivision();
       

    }]);

})(angular.module('newApp'));