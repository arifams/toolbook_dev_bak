﻿'use strict';


(function (app) {

    app.factory('userManager', function ($http) {
        return {
            loginUser: function (newuser, url) {

                return $http.post(serverBaseUrl + '/' + url, newuser);
            }
        };

    });

    app.controller('adminLoginCtrl', ['userManager', '$window', '$cookieStore', '$scope', 'gettextCatalog',
    function (userManager, $window, $cookieStore, $scope, gettextCatalog) {
        var vm = this;
        //$localStorage.userGuid = '';
        $window.localStorage.setItem('userGuid', '');
        $window.localStorage.setItem('currentLnguage', '');
        gettextCatalog.setCurrentLanguage($window.localStorage.getItem('currentLnguage'));

        vm.loginInvalid = false;
        vm.invalidToken = false;
        vm.isEmailConfirm = false;

        vm.isSentPasswordResetMail = false;
        vm.passwordResetError = false;

        vm.signInLodingSymbole = false;

        var loggedusername = $cookieStore.get('username');
        var loggedpassword = $cookieStore.get('password');
        $scope.user = {};
        // restore credentials for logged user
        if (loggedusername && loggedpassword) {
            $scope.user.username = loggedusername;
            $scope.user.password = loggedpassword;
        }

        vm.isConfirmEmail = function () {

            if (window.location.search != "") {
                // Show email confirm message.
                vm.emailConfirmationMessage = "To confirm email address, please login using your username and password";
                vm.isEmailConfirm = true;

            }

        };

        vm.login = function (user) {

            vm.signInLodingSymbole = true;
            if (vm.rememberme == true) {
                $cookieStore.put('username', user.username);
                $cookieStore.put('password', user.password);
            }


            if (window.location.search != "") {

                var splittedValues = window.location.search.replace("?", "").split('&');

                if (splittedValues.length != 2 || splittedValues[0].split('=').length != 2 || splittedValues[1].split('=').length != 2) {
                    vm.emailConfirmationMessage = "Confirmation URL link is not properly formatted. Please resend the confirmation URL";
                    return;
                }

                var userIdKeyValue = splittedValues[0].split('=');
                var codeKeyValue = splittedValues[1].split('=');

                if (userIdKeyValue[0] != 'userId') {
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

            userManager.loginUser(user, 'api/accounts/LoginAdmin')
             .then(function (returnedResult) {
                 vm.signInLodingSymbole = true;

                 if (returnedResult.data.result == "1") {
                     vm.signInLodingSymbole = true;
                     // TODO: To be coverted to a token.
                     $window.localStorage.setItem('userGuid', returnedResult.data.id);
                     $window.localStorage.setItem('userRole', returnedResult.data.role);
                     $window.localStorage.setItem('token', returnedResult.data.token);

                     window.location = webBaseUrl + "/app/index.html";
                 }
                 else if (returnedResult.data.result == "-1") {
                     vm.signInLodingSymbole = false;
                     vm.loginInvalid = true;
                     vm.loginInvalidMessage = "Incorrect UserName/Password";
                     $cookieStore.remove('username');
                     $cookieStore.remove('password');
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

            vm.pwdReset.templateLink = '<html><head>    <title></title></head><body>    <p>        <img alt="" src="http://www.12send.com/template/logo_12send.png"style="width: 193px; height: 100px; float: right;" />    </p><div>        <h4 style="text-align: justify;">            &nbsp;        </h4><div style="background:#eee;border:1px solid #ccc;padding:5px 10px;">            <span style="font-family:verdana,geneva,sans-serif;">                <span style="color:#0000CD;">                    <span style="font-size:28px;">                        Password Reset                    </span>                </span>            </span>        </div><p style="text-align: justify;">&nbsp;</p><h4 style="text-align: justify;">            &nbsp;        </h4><h4 style="text-align: justify;">            <span style="font-size:12px;">                <span style="font-family:verdana,geneva,sans-serif;">                    Dear <strong>Salutation.  FirstName &nbsp;LastName, &nbsp;</strong>                </span>            </span>        </h4><h4 style="text-align: justify;">            <br /><span style="font-size:12px;">                <span style="font-family:verdana,geneva,sans-serif;">                    <strong>                        We received a request to reset the password associated with this email address.                        If you made this request, please follow the instructions below.                        &nbsp;&nbsp;                    </strong>                </span>            </span>        </h4><h4 style="text-align: justify;">            <span style="font-size:12px;">                <span style="font-family:verdana,geneva,sans-serif;">                    <strong>If you did not request to have your password reset, you can safetly ignore this email.</strong>                </span>            </span>        </h4><h4 style="text-align: justify;">            <span style="font-size:12px;">                <span style="font-family:verdana,geneva,sans-serif;">                    <strong>Please click ActivationURL to reset the password.</strong>                </span>            </span>        </h4>        <h4 style="text-align: justify;">            <span style="font-size:12px;">                <span style="font-family:verdana,geneva,sans-serif;">                    <strong>IMPORTANT! This link is valid for 24 hours only. &nbsp;&nbsp;</strong>                </span>            </span>        </h4>        <h4 style="text-align: justify;">            <span style="font-size:12px;">                <span style="font-family:verdana,geneva,sans-serif;">                    <strong>Should you have any questions or concerns, please contact Parcel International helpdesk for support &nbsp;</strong>                </span>            </span>        </h4>        <h4 style="text-align: justify;">            <span style="font-size:12px;">                <span style="font-family:verdana,geneva,sans-serif;">                    <i>                        *** This is an automatically generated email, please do not reply ***                    </i>                </span>            </span>        </h4>        <h4 style="text-align: justify;">&nbsp;</h4><h4 style="text-align: justify;">            <strong>                <span style="font-size:12px;">                    <span style="font-family:verdana,geneva,sans-serif;">Thank You, </span>                </span>            </strong>        </h4><h4 style="text-align: justify;">            <strong>                <span style="font-size:12px;">                    <span style="font-family:verdana,geneva,sans-serif;">Parcel International Team<br/>Phone: +18589144414 <br/>Email: <a href="mailto:helpdesk@parcelinternational.com">helpdesk@parcelinternational.com</a><br/>Website: <a href="http://www.parcelinternational.com">http://www.parcelinternational.com</a></span>                </span>            </strong>        </h4>    </div></body></html>';

            userManager.loginUser(vm.pwdReset, 'api/accounts/ResetForgetPassword')
             .then(function (returnedResult) {

                 if (returnedResult.status == 200) {
                     vm.passwordResetError = false;
                     vm.isSentPasswordResetMail = true;
                 }
             },
            function (error) {
                vm.passwordResetError = true;

                vm.passwordResetErrorMsg = $rootScope.translate(error.data.message);

                if (error.data == "" || error.data.message == "") {
                    vm.passwordResetErrorMsg = $rootScope.translate('Error occured while processing your request.');
                }
            });
        };

    }]);


})(angular.module('adminLogin', ['ngMessages', 'ngCookies', 'gettext']));

