﻿'use strict';

(function (app) {

    //app.directive('icheck', ['$timeout', '$parse', function ($timeout, $parse) {

    //    return {
    //        require: 'ngModel',
    //        link: function ($scope, element, $attrs, ngModel) {
    //            return $timeout(function () {
    //                var value;
    //                value = $attrs['value'];

    //                $scope.$watch($attrs['ngModel'], function (newValue) {
    //                    $(element).iCheck('update');
    //                })

    //                return $(element).iCheck({
    //                    checkboxClass: 'icheckbox_square-blue', //'icheckbox_flat-aero',
    //                    radioClass: 'iradio_square-blue'

    //                }).on('ifChanged', function (event) {
    //                    if ($(element).attr('type') === 'checkbox' && $attrs['ngModel']) {
    //                        $scope.$apply(function () {
    //                            return ngModel.$setViewValue(event.target.checked);
    //                        });
    //                    }
    //                    if ($(element).attr('type') === 'radio' && $attrs['ngModel']) {
    //                        return $scope.$apply(function () {
    //                            return ngModel.$setViewValue(value);
    //                        });
    //                    }
    //                });
    //            });
    //        }
    //    };

    //}]);

    app.controller('saveUserManagementCtrl', ['$location', '$window', 'userManagementFactory', function ($location, $window, userManagementFactory) {
        var vm = this;
        vm.user = {};

        var loadUser = function () {
            userManagementFactory.getUser()
            .success(function (data) {

                vm.user = data;
                vm.user.assignedDivisionIdList = [];
                if (vm.user.id == 0) {
                    // New user.
                    vm.user.status = 1;
                }
                else {
                    // Exisiting user.
                    //Add selected divisions
                    angular.forEach(vm.user.allDivisions, function (division) {
                        if (division.isAssigned) {
                            vm.user.assignedDivisionIdList.push(division.id);
                        }
                    })
                }
            })
            .error(function () {
                debugger;
            })
        }


        //vm.saveUser = function () {
        //    vm.user.userId = $window.localStorage.getItem('userGuid');
        //    var body = $("html, body");

        //    userManagementFactory.saveUser(vm.user)
        //    .success(function (result) {
        //        debugger;
        //        //if (result == -1) {

        //        //    body.stop().animate({ scrollTop: 0 }, '500', 'swing', function () { });

        //        //    $('#panel-notif').noty({
        //        //        text: '<div class="alert alert-warning media fade in"><p>A cost center with the same name already exists!</p></div>',
        //        //        layout: 'bottom-right',
        //        //        theme: 'made',
        //        //        animation: {
        //        //            open: 'animated bounceInLeft',
        //        //            close: 'animated bounceOutLeft'
        //        //        },
        //        //        timeout: 3000,
        //        //    });
        //        //} else {

        //        //    body.stop().animate({ scrollTop: 0 }, '500', 'swing', function () { });
        //        //    $('#panel-notif').noty({
        //        //        text: '<div class="alert alert-success media fade in"><p>Cost center saved successfully!</p></div>',
        //        //        layout: 'bottom-right',
        //        //        theme: 'made',
        //        //        animation: {
        //        //            open: 'animated bounceInLeft',
        //        //            close: 'animated bounceOutLeft'
        //        //        },
        //        //        timeout: 3000,
        //        //    });
        //        //}

        //    })
        //    .error(function () {
        //    })
        //}

        //vm.close = function () {
        //    $location.path('/loadUserManagement');
        //}

        loadUser();

        //vm.toggleDivisionSelection = function (division) {
        //    var idx = vm.user.assignedDivisionIdList.indexOf(division.id);
        //    // is currently selected
        //    if (idx > -1) {
        //        vm.user.assignedDivisionIdList.splice(idx, 1);
        //    }
        //        // is newly selected
        //    else {
        //        vm.user.assignedDivisionIdList.push(division.id);
        //    }
        //}

    }]);


})(angular.module('newApp'));