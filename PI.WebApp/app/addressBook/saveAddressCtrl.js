﻿'use strict';
(function (app) {

    app.directive('validPhoneNo', function () {
        return {
            require: 'ngModel',
            link: function (scope, elm, attrs, ctrl) {
                ctrl.$parsers.unshift(function (viewValue, $scope) {

                    // should have only +,- and digits.
                    var res1 = /^[0-9()+-]*$/.test(viewValue);
                    // should have at least 8 digits.
                    var res2 = /(?=(.*\d){8})/.test(viewValue);
                    if (viewValue != '')
                        ctrl.$setValidity('notValidPhoneNo', res1 && res2);
                    else
                        ctrl.$setValidity('notValidPhoneNo', true);

                    return viewValue;
                })
            }
        }
    });
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

    app.controller('saveAddressCtrl', ['saveAddressBookFactory', 'loadAddressBookFactory', '$location', '$window', '$routeParams', function (saveAddressBookFactory, loadAddressBookFactory, $location, $window, $routeParams) {

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
                        text: '<div class="alert alert-success media fade in"><p>Address Detail saved successfully</p></div>',
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
                if (data.isActive==true) {
                    vm.model.isActive = "true";
                } else {
                    vm.model.isActive = "false";
                }

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