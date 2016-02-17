﻿'use strict';


(function (app) {    

    app.factory('userManager', function ($http) {
        return {
            loginUser: function (newuser, url) {
                
                return $http.post(serverBaseUrl + '/' + url, newuser);
            }
        };

    });

    app.controller('userLoginCtrl', ['userManager', '$window',
    function (userManager, $window) {
        var vm = this;     
        //$localStorage.userGuid = '';
        $window.localStorage.setItem('userGuid', '');
        vm.loginInvalid = false;
        vm.invalidToken = false;
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
                user.isConfirmEmail = true;
            }

            userManager.loginUser(user, 'api/accounts/LoginUser')
             .then(function (returnedResult) {
                 debugger;
                 if (returnedResult.data.result == "1" || returnedResult.data.result == "2") {

                     // TODO: To be coverted to a token.
                     $window.localStorage.setItem('userGuid', returnedResult.data.user.id);
                     window.location = webBaseUrl + "/app/index.html";
                 }
                 else if (returnedResult.data.result == "-1") {
                     vm.loginInvalid = true;
                 }
                 else if (returnedResult.data.result == "-2") {
                     vm.invalidToken = true;
                 }
             },
            //.then(function (result) {

            //    if (result.data == "1" || result.data == "2") {

            //        // TODO: To be coverted to a token.


            //        window.location = webBaseUrl + "/app/index.html"; 
            //    }
            //    else if (result.data == "-1") {
            //        vm.loginInvalid = true;
            //    }
            //    else if (result.data == "-2") {
            //        vm.invalidToken = true;
            //    }
            //},
            function (error) {

                console.log("failed");
            });
           
        };

        vm.isConfirmEmail();

    }]);


})(angular.module('userLogin', []));

