'use strict';

(function (app) {



    app.directive('validCustomerPasswordC', function () {
        return {
            require: 'ngModel',
            link: function (scope, elm, attrs, ctrl) {
                ctrl.$parsers.unshift(function (viewValue, $scope) {
                    var noMatch = viewValue != scope.cont.formSaveCustomer.password.$viewValue;
                    ctrl.$setValidity('noMatch', !noMatch);
                    return viewValue;
                })
            }
        }
    });


    app.directive('validCustomerPassword', function () {
        return {
            require: 'ngModel',
            link: function (scope, elm, attrs, ctrl) {
                ctrl.$parsers.unshift(function (viewValue, $scope) {

                    // password validate.
                    var res = /^(?=.*[a-z])(?=.*[A-Z])[a-zA-Z\S]{7,20}$/.test(viewValue);
                    ctrl.$setValidity('noValidPassword', res);

                    // if change the password when having confirmation password, check match and give error.
                    if (scope.cont.formSaveCustomer.passwordConf.$viewValue != '') {
                        var noMatch = viewValue != scope.cont.formSaveCustomer.passwordConf.$viewValue;
                        scope.cont.formSaveCustomer.passwordConf.$setValidity('noMatch', !noMatch);
                    }

                    return viewValue;
                })
            }
        }
    });

    app.controller('adminManageCustomersCtrl', ['customBuilderFactory',
        function (customBuilderFactory) {

            var vm = this;
          
            vm.editUserBtnClick = false; // used for edit btn click function
            vm.rightPaneLoad = false; // used for change table width


            //toggle function
        vm.loadFilterToggle = function () {
            customBuilderFactory.customFilterToggle();

        };

        vm.manageUsers = function () {
            
            vm.rightPaneLoad = true;
            vm.editUserBtnClick = true;
        }

        
    }]);

})(angular.module('newApp'));
