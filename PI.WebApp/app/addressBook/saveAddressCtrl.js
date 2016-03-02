'use strict';
(function (app) {

    app.directive('icheck', ['$timeout', '$parse', function ($timeout, $parse) {

        return {
            require: 'ngModel',
            link: function ($scope, element, $attrs, ngModel) {
                return $timeout(function () {
                    var value;
                    value = $attrs['value'];

                    $scope.$watch($attrs['ngModel'], function (newValue) {
                        $(element).iCheck('update');
                    })

                    return $(element).iCheck({
                        checkboxClass: 'icheckbox_square-blue', //'icheckbox_flat-aero',
                        radioClass: 'iradio_square-blue'

                    }).on('ifChanged', function (event) {
                        if ($(element).attr('type') === 'checkbox' && $attrs['ngModel']) {
                            $scope.$apply(function () {
                                return ngModel.$setViewValue(event.target.checked);
                            });
                        }
                        if ($(element).attr('type') === 'radio' && $attrs['ngModel']) {
                            return $scope.$apply(function () {
                                return ngModel.$setViewValue(value);
                            });
                        }
                    });
                });
            }
        };

    }]);

    app.factory('saveAddressBookFactory', function ($http) {
        return {
            saveAddressBook: function (addressDetail) {
                return $http.post(serverBaseUrl + '/api/AddressBook/SaveAddress', addressDetail);
            }
        };
    })

    app.factory('loadAddressBookFactory', function ($http, $routeParams, $window) {
        return {
            loadAddressInfo: function () {
                return $http.get(serverBaseUrl + '/api/AddressBook/LoadAddress', {
                    params: {
                        id: $routeParams.id,
                        userId: $window.localStorage.getItem('userGuid')
                    }
                });
            }
        }
    })

    app.controller('saveAddressCtrl', ['saveAddressBookFactory', 'loadAddressBookFactory', '$location', '$window', function (saveAddressBookFactory, loadAddressBookFactory, $location, $window) {

        var vm = this;

        vm.saveAddressDetail = function () {
            vm.model.userId = $window.localStorage.getItem('userGuid');
            
            saveAddressBookFactory.saveAddressBook(vm.model)
           .success(function (result) {
                if (result == 1) {              
                    $('#panel-notif').noty({
                        text: '<div class="alert alert-success media fade in"><p>"Division saved successfully"</p></div>',
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

                if (vm.model.id == 0) {
                    vm.model.status = 1;
                }
            })
            .error(function () {
            })
        }

        loadDivision();
       

    }]);

})(angular.module('newApp'));