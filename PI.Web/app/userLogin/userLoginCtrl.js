﻿'use strict';


(function (app) {    

    app.factory('userRegister', function ($http) {
        return {
            loginUser: function (newuser, url) {
                return $http.post(serverBaseUrl + '/' + url, newuser);
            }
        };

    });

    app.controller('userLoginCtrl', ['userRegister', function (userRegister) {
        var vm = this;     

        vm.loginInvalid = false;
        vm.isEmailConfirm = false;
       
        vm.isConfirmEmail = function () {

            if (window.location.search != "") {
                // Show email confirm message.
                vm.emailConfirmationMessage = "To confirm email address, please login using your username and password";
                vm.isEmailConfirm = true;
                
            }

        };

        vm.login = function (user) {

            debugger;
            if (window.location.search != "") {

                var splittedValues = window.location.search.replace("?", "").split('&');

                if (splittedValues.length != 2 || splittedValues[0].split('=').length != 2 || splittedValues[1].split('=').length != 2) {
                    vm.emailConfirmationMessage = "Confirmation URL link is not properly formatted. Please resend the confirmation URL";
                    return;
                }

                var userIdKeyValue = splittedValues[0].split('=');
                var codeKeyValue = splittedValues[1].split('=');

                if (userIdKeyValue[0] != 'userId'){
                    vm.emailConfirmationMessage = "Confirmation URL link is not properly formatted. Please resend the confirmation URL";
                    return;
                }
                if (codeKeyValue[0] != 'code') {
                    vm.emailConfirmationMessage = "Confirmation URL link is not properly formatted. Please resend the confirmation URL";
                    return;
                }

                user.userId = userIdKeyValue[1];
                user.code = codeKeyValue[1];
            }

            userRegister.loginUser(user, 'api/User/LoginUser')
            .then(function (result) {
                console.log("success" + result);

                if (result.data == "1") {
                    //window.location = "http://localhost:63874/app/index.html";
                    window.location = window.location.host + "/app/index.html";
                }
                else {
                    vm.loginInvalid = true;
                }
            },
            function (error) {

                console.log("failed");
            });
           
        };

        vm.isConfirmEmail();

    }]);


})(angular.module('userLogin', []));

