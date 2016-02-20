'use strict';


(function (app) {

    app.factory('divisionManagmentFactory', function ($http) {
        return {
            saveDivision: function (divisionDetail) {
                return $http.post(serverBaseUrl + '/api/Company/SaveDivision', divisionDetail);
            }
        };
    })

    app.factory('divisionService', function ($http, $routeParams, $window) {
        return {
            loadDivisioninfo: function () {
                return $http.get(serverBaseUrl + '/api/Company/GetDivisionById', {
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


    app.controller('saveDivisionCtrl',
       ['divisionManagmentFactory', 'divisionService', '$location', '$window',
           function (divisionManagmentFactory, divisionService, $location, $window) {
               var vm = this;
           
               vm.saveDivision = function () {
                   vm.model.userId = $window.localStorage.getItem('userGuid')

                   divisionManagmentFactory.saveDivision(vm.model)
                    .success(function (result) {
                        if (result == -1) {

                            $('#panel-notif').noty({
                                text: '<div class="alert alert-warning media fade in"><p>A division with the same name already exists!</p></div>',
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
                                text: '<div class="alert alert-success media fade in"><p>"Division added successfully"</p></div>',
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
               $location.path('/loadDivisions');
           }

    var loadDivision = function () {
        divisionService.loadDivisioninfo()
        .success(function (data) {
            vm.model = data
        })
        .error(function () {
        })
    }

    loadDivision();
}]);


})(angular.module('newApp'));