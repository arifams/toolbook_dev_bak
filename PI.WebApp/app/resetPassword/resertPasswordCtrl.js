'use strict';


(function (app) {

    app.run(function (gettextCatalog, $rootScope, $window) {

        gettextCatalog.setCurrentLanguage($window.localStorage.getItem('currentLnguage'));

        $rootScope.translate = function (str) {
            return gettextCatalog.getString(str);
        };

        //gettextCatalog.debug = true;
    });

    app.factory('userManager', function ($http) {
        return {
            loginUser: function (newuser, url) {

                return $http.post(serverBaseUrl + '/' + url, newuser);
            }
        };

    });

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
                    var res = /^(?=.*[a-z])(?=.*[A-Z])[a-zA-Z\S]{7,20}$/.test(viewValue);
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

    app.controller('resetPasswordCtrl', ['userManager', '$timeout', 'gettextCatalog','$window',
    function (userManager, $timeout, gettextCatalog, $window) {
        var vm = this;
       
        vm.invalidToken = false;
        vm.successReset = false;
        vm.erroMessage = "";
        gettextCatalog.setCurrentLanguage($window.localStorage.getItem('currentLnguage'));


        vm.resetOldPassword = function (user) {
            
            if (window.location.search == "") {
                vm.invalidToken = true;
                vm.erroMessage = "Invalid token. Please resend the Password reset URL";
                return;
            }

            var urlString = window.location.search;

            var userId = urlString.substr(1, urlString.indexOf('&') - 1);
            var code = urlString.substr(urlString.indexOf('&') + 1);
            var userIdName, userIdValue, codeName,codeValue;

            if (userId == undefined && code == undefined) {
                vm.invalidToken = true;
                vm.erroMessage = "Invalid token. Please resend the Password reset URL";
                return;
            }
                
            userIdName = userId.substr(0, userId.indexOf('='));
            userIdValue = userId.substr(userId.indexOf('=') + 1);

            codeName = code.substr(0, code.indexOf('='));
            codeValue = code.substr(code.indexOf('=') + 1);
                
            if (userIdName != 'userId' || codeName != 'code' || userIdValue == '' || userIdValue == undefined || codeValue == '' || codeValue == undefined) {
                vm.invalidToken = true;
                vm.erroMessage = "Invalid token. Please resend the Password reset URL";
                return;
            }

            user.userId = userIdValue;
            user.code = codeValue;
            
            userManager.loginUser(user, 'api/accounts/resetForgetPasswordConfirm')
             .then(function (returnedResult) {

                 if (returnedResult.status == 200) {
                     vm.successReset = true;
                     $timeout(function () {
                         window.location = webBaseUrl + "/app/userLogin/userLogin.html";
                     }, 4000);
                 }
                
                 else if (returnedResult.data == "-1") {
                     vm.invalidToken = true;
                     vm.erroMessage = "Valid token and password required";
                 }
                 else if (returnedResult.data == "-2") {
                     vm.invalidToken = true;
                     vm.erroMessage = "Invalid token. Please resend the password reset URL";
                 }
             },
            function (error) {
                console.log("failed");
                vm.invalidToken = true;
                vm.erroMessage = error.data.message;

                if (error.data == "" || error.data.message == "") {
                    vm.erroMessage = 'Error occured while processing your request.';
                }                
            });

        };

    }]);


})(angular.module('resetPassword', ['ngMessages', 'gettext']));

