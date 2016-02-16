'use strict';


(function(app){
    
    app.factory('registerUserService', function ($http) {            
         
        return{
            createUser : function (newuser) {
                return $http.post(serverBaseUrl + '/api/accounts/create', newuser);
        }
        };
      
    });

    app.directive('validPasswordC', function () {
        return {
            require: 'ngModel',
            link: function (scope, elm, attrs, ctrl) {
                ctrl.$parsers.unshift(function (viewValue, $scope) {
                    var noMatch = viewValue != scope.formSignup.password.$viewValue;
                    ctrl.$setValidity('noMatch', !noMatch);
                    return viewValue;
                })
            }
        }
    });

    app.directive('validPassword', function () {
        return {
            require: 'ngModel',
            link: function (scope, elm, attrs, ctrl) {
                ctrl.$parsers.unshift(function (viewValue, $scope) {

                    // password validate.
                    var res = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d.*)(?=.*\W.*)[a-zA-Z0-9\S]{8,20}$/.test(viewValue);
                    ctrl.$setValidity('noValidPassword', res);

                    // if change the password when having confirmation password, check match and give error.
                    if (scope.formSignup.password_c.$viewValue != ''){
                        var noMatch = viewValue != scope.formSignup.password_c.$viewValue;
                        scope.formSignup.password_c.$setValidity('noMatch', !noMatch);
                    }

                    return viewValue;
                })
            }
        }
    });

    app.directive('validPhoneNo', function () {
        return {
            require: 'ngModel',
            link: function (scope, elm, attrs, ctrl) {
                ctrl.$parsers.unshift(function (viewValue, $scope) {

                    // should have only +,- and digits.
                    var res1 = /^[0-9()+-]*$/.test(viewValue);
                    // should have at least 8 digits.
                    var res2 = /(?=(.*\d){8})/.test(viewValue);

                    ctrl.$setValidity('notValidPhoneNo', res1 && res2);
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

    app.controller('userRegistrationCtrl', 
        ['registerUserService' ,function (registerUserService) {

        var vm = this;
        vm.user = {};
        vm.user.salutation = "Mr";
        vm.user.iscorporate = "false";
        vm.user.contactType = 'phone';
        vm.isSubmit = false;
        vm.user.customeraddress = {};
        vm.user.customeraddress.country = 'United States';
        vm.isSentMail = false;
        vm.isEmailNotValid = false;
        vm.isServerError = false;
        
        vm.changeCountry = function () {
            vm.isRequiredState = vm.user.customeraddress.country == 'United States' || vm.user.customeraddress.country == 'Canada' || vm.user.customeraddress.country == 'Puerto Rico';
        };
        vm.changeCountry();

        vm.register = function () {         

            registerUserService.createUser(vm.user)
            .then(function (result)
            {
                if (result.data == "1") {
                    vm.isSentMail = true;
                }
                else if (result.data == "-2") {
                    vm.isEmailNotValid = true;
                }
                else { 
                    // Other issues.
                    vm.isServerError = true;
                }
            },
            function (error) {
                console.log("failed");
            }
            );
        };
    }]);
        

})(angular.module('userRegistration', ['ngMessages']));

