'use strict';


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
       
        vm.isSentPasswordResetMail = false;
        vm.passwordResetError = false;

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
                     vm.loginInvalidMessage = "Incorrect UserName/Password";
                 }
                 else if (returnedResult.data.result == "-2") {
                     vm.invalidToken = true;
                 }
                 else if (returnedResult.data.result == "-11") {
                     vm.loginInvalid = true;
                     vm.loginInvalidMessage = "You must have a confirmed email to log in";
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

        vm.resetForgetPassword = function () {

            vm.pwdReset.templateLink = '<html><head><title></title></head><body><p><img alt="" src="http://www.parcelinternational.nl/assets/Uploads/_resampled/SetWidth495-id-parcel-big.jpg" style="width: 200px; height: 200px; float: right;" /></p><div><h4 style="text-align: justify;">&nbsp;</h4><div style="background:#eee;border:1px solid #ccc;padding:5px 10px;"><span style="font-family:verdana,geneva,sans-serif;"><span style="color:#0000CD;"><span style="font-size:28px;">Password Reset</span></span></span></div><p style="text-align: justify;">&nbsp;</p><h4 style="text-align: justify;">&nbsp;</h4><h4 style="text-align: justify;"><span style="font-size:12px;"><span style="font-family:verdana,geneva,sans-serif;">Dear <strong>FirstName &nbsp;LastName, &nbsp;</strong></span></span></h4><h4 style="text-align: justify;"><br /><span style="font-size:12px;"><span style="font-family:verdana,geneva,sans-serif;"><strong>We received a request to reset the password associated with this email address. If you made this request, please follow the instructions below. &nbsp;&nbsp;</strong></span></span></h4><h4 style="text-align: justify;"><span style="font-size:12px;"><span style="font-family:verdana,geneva,sans-serif;"><strong>If you did not request to have your password reset, you can safetly ignore this email.</strong></span></span></h4><h4 style="text-align: justify;"><span style="font-size:12px;"><span style="font-family:verdana,geneva,sans-serif;"><strong>Please click this &nbsp;ActivationURL&nbsp;</strong></span></span></h4><h4 style="text-align: justify;"><span style="font-size:12px;"><span style="font-family:verdana,geneva,sans-serif;"><strong>Should you have any questions or concerns, please contact Parcel International helpdesk for support &nbsp;ng></span></span></h4><h4 style="text-align: justify;">&nbsp;</h4><h4 style="text-align: justify;"><strong><span style="font-size:12px;"><span style="font-family:verdana,geneva,sans-serif;">Thank You, </span></span></strong></h4><h4 style="text-align: justify;"><strong><span style="font-size:12px;"><span style="font-family:verdana,geneva,sans-serif;">Parcel International Team</span></span></strong></h4></div></body></html>';

            userManager.loginUser(vm.pwdReset, 'api/accounts/ResetForgetPassword')
             .then(function (returnedResult) {
                 debugger;
                 
                 if (returnedResult.data == "1") {
                     vm.passwordResetError = false;
                     vm.isSentPasswordResetMail = true;
                 }
                 else if (returnedResult.data == "-1") {
                     //No account find by this email.
                     vm.passwordResetError = true;
                     vm.passwordResetErrorMsg = "No account found by this email. Please enter registered Email";
                 }
                 else if (returnedResult.data == "-11") {
                     //No account find by this email.
                     vm.passwordResetError = true;
                     vm.passwordResetErrorMsg = "You must have a confirmed email to log in.";
                 }
             },
            function (error) {
                vm.passwordResetError = true;
                vm.passwordResetErrorMsg = "Server error occured while reseting password";
            });
        };

    }]);


})(angular.module('userLogin', ['ngMessages']));

