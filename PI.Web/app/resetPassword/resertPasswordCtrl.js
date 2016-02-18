'use strict';


(function (app) {

    app.directive('validPasswordC', function () {
        return {
            require: 'ngModel',
            link: function (scope, elm, attrs, ctrl) {
                ctrl.$parsers.unshift(function (viewValue, $scope) {
                    var noMatch = viewValue != scope.resetPasswordForm.password.$viewValue;
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
                    if (scope.resetPasswordForm.password_c.$viewValue != '') {
                        var noMatch = viewValue != scope.resetPasswordForm.password_c.$viewValue;
                        scope.resetPasswordForm.password_c.$setValidity('noMatch', !noMatch);
                    }

                    return viewValue;
                })
            }
        }
    });

    app.controller('resetPasswordCtrl', [
    function () {
        var vm = this;
       

    }]);


})(angular.module('resetPassword', ['ngMessages']));

