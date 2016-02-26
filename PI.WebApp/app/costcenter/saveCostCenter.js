'use strict';


(function (app) {

    app.factory('costCenterSaveFactory', function ($http) {
        return {
            saveCostCenter: function (costCenterDetail) {
                return $http.post(serverBaseUrl + '/api/Company/SaveCostCenter', costCenterDetail);
            }
        };
    })

    app.factory('costCentrMngtFactory', function ($http, $routeParams, $window) {
        return {
            loadCostcenterInfo: function () {
                return $http.get(serverBaseUrl + '/api/Company/GetCostCentersById', {
                    params: {
                        id: $routeParams.id,
                        userId: $window.localStorage.getItem('userGuid')
                    }
                });
            }
        }
    })


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

    app.controller('saveCostCenterCtrl',
       ['costCentrMngtFactory', 'costCenterSaveFactory', '$location', '$window',
           function (costCentrMngtFactory, costCenterSaveFactory, $location, $window) {
               var vm = this;

               vm.saveCostCenter = function () {
                   vm.model.userId = $window.localStorage.getItem('userGuid')

                   costCenterSaveFactory.saveCostCenter(vm.model)
                   .success(function (result) {
                       debugger;
                       if (result == -1) {

                           $('#panel-notif').noty({
                               text: '<div class="alert alert-warning media fade in"><p>A cost center with the same name already exists!</p></div>',
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
                               text: '<div class="alert alert-success media fade in"><p>"Cost center saved successfully"</p></div>',
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
                   $location.path('/loadCostcenters');
               }

               vm.changeCountry = function () {
                   if (vm.model.billingAddress == null) {
                       vm.model.billingAddress = {};
                   }
                   vm.isRequiredState = vm.model.billingAddress.country == 'US' ||
                                        vm.model.billingAddress.country == 'CA' ||
                                        vm.model.billingAddress.country == 'PR';


               };

               var loadCostcenter = function () {
                   costCentrMngtFactory.loadCostcenterInfo()
                   .success(function (data) {
                       vm.model = data;
                       debugger;
                       if (vm.model.id == 0) {
                           vm.model.status = 1;

                           vm.model = {

                               billingAddress: {
                                   country: 'US'
                               }

                           };
                           vm.isRequiredState = true;
                       }
                       else {
                           vm.changeCountry();
                       }
                   })
                   .error(function () {
                   })
               }

               loadCostcenter();



               vm.toggleDivisionSelection = function (division) {
                   var idx = vm.model.assignedDivisionIdList.indexOf(division.id);
                   // is currently selected
                   if (idx > -1) {
                       vm.model.assignedDivisionIdList.splice(idx, 1);
                   }
                       // is newly selected
                   else {
                       vm.model.assignedDivisionIdList.push(division.id);
                   }
               }

           }]);


})(angular.module('newApp'));