'use strict';


(function (app) {

    app.factory('userManager', function ($http) {
        return {
            loginUser: function (newuser, url) {

                return $http.post(serverBaseUrl + '/' + url, newuser);
            }
        };

    });

    app.run(function (gettextCatalog, $rootScope, $window) {

        
        gettextCatalog.setCurrentLanguage($window.localStorage.getItem('currentLnguage'));


        $rootScope.translate = function (str) {
            return gettextCatalog.getString(str);
        };

        //gettextCatalog.debug = true;
    });

    app.controller('userLoginCtrl', ['userManager', '$window', '$cookieStore', '$scope', '$rootScope','gettextCatalog',
    function (userManager, $window, $cookieStore, $scope, $rootScope, gettextCatalog) {
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

            userManager.loginUser(user, 'api/accounts/LoginUser')
             .then(function (returnedResult) {

                 if (returnedResult.data.result == "1" || returnedResult.data.result == "2") {

                     // TODO: To be coverted to a token.
                     $window.localStorage.setItem('userGuid', returnedResult.data.id); 
                     $window.localStorage.setItem('userRole', returnedResult.data.role);
                     $window.localStorage.setItem('isCorporateAccount', returnedResult.data.isCorporateAccount);
                     $window.localStorage.setItem('token', returnedResult.data.token);                   

                     window.location = webBaseUrl + "/app/index.html";
                 }
                 else if (returnedResult.data.result == "-1") {
                     //vm.loginInvalid = true;
                     //vm.loginInvalidMessage = "Incorrect UserName/Password";
                     $cookieStore.remove('username');
                     $cookieStore.remove('password');

                     $.noty.defaults.killer = true;

                     noty({
                         text: '<p style="font-size:medium">' + $rootScope.translate('Error') + '! </p>' + returnedResult.data.message,
                         layout: 'topRight',
                         type: 'warning',
                         animation: {
                             open: 'animated bounceInRight', // Animate.css class names
                             //close: 'animated bounceInLeft', // Animate.css class names
                             easing: 'swing', // unavailable - no need
                             speed: 200 // unavailable - no need
                         },
                         closeWith: ['click']
                     });

                 }
                 else if (returnedResult.data.result == "-2") {
                     vm.invalidToken = true;
                     $cookieStore.remove('username');
                     $cookieStore.remove('password');
                 }
                 else if (returnedResult.data.result == "-11") {
                     vm.loginInvalid = true;
                     vm.loginInvalidMessage = $rootScope.translate("You must have a confirmed email to log in");
                     $cookieStore.remove('username');
                     $cookieStore.remove('password');
                 }
             },
            function (error) {

                console.log("failed");
            });

        };

        vm.isConfirmEmail();

        vm.resetForgetPassword = function () {

            vm.pwdReset.templateLink = '<html><head><title></title><style>body{margin:30px;}.email-content,.email-header{padding:10px;font-family:verdana,geneva,sans-serif;color:#fff}.email-header,.row>h3,.title>h3{text-align:center}<html><head><title></title><style>body{margin:30px}.email-header{background-color:#0af;font-size:28px;border:5px solid #d9d9d9}.logo-header{float:left;z-index:1}.row{margin-right:40px;margin-left:40px}.row>h3{font-weight:700}.email-content{border:5px solid #0af;background-color:#005c99;font-size:13px}.email-content>p{font-weight:700;font-style:italic;font-size:14px}.title>h3{margin-bottom:65px;margin-right:146px;margin-top:0;padding-top:62px}a{color:#80d4ff}a:focus,a:hover{color:#fff}</style></head><body><div class="row"><div class="email-header" style="margin-top:30px;">Password Reset - Parcel International</div></div><div class="row"><div class="logo-header"><img alt="" src="http://www.parcelinternational.nl/assets/Uploads/_resampled/SetWidth495-id-parcel-big.jpg" style="width: 150px; height: 150px;" /></div><div class="title"><h3>Password reset instructions</h3></div></div><div class="row"><div class="email-content"><p>Dear Salutation FirstName LastName,</p><br/><p>We received a request to reset the password associated with this email address. If you made this request, please follow the instructions below.</p>If you did not request to have your password reset, you can safetly ignore this email.</p><p>Please click this &nbsp;ActivationURL&nbsp;</p><p>IMPORTANT! Please note that this link is valid for 24 hours only. <p><p>Should you have any questions or concerns, please contact Parcel International helpdesk for support.</p></br><p>Thank you,</p><p>Parcel International Service Team</p></br>Phone: +1 858 914 4414 </br>Email address:<a href="mailto:helpdesk@parcelinternational.com">  helpdesk@parcelinternational.com</a></br>Website: <a href="http://www.parcelinternational.com">www.parcelinternational.com</a></div><p><i>*** This is an automatically generated email, please do not reply ***</i></p></div><div></div></body></html>';

            userManager.loginUser(vm.pwdReset, 'api/accounts/ResetForgetPassword')
             .then(function (returnedResult) {

                 if (returnedResult.data == "1") {
                     vm.passwordResetError = false;
                     vm.isSentPasswordResetMail = true;
                 }
                 else if (returnedResult.data == "-1") {
                     //No account find by this email.
                     vm.passwordResetError = true;
                     vm.passwordResetErrorMsg = $rootScope.translate("No account found by this email. Please enter registered Email");
                 }
                 else if (returnedResult.data == "-11") {
                     //No account find by this email.
                     vm.passwordResetError = true;
                     vm.passwordResetErrorMsg = $rootScope.translate("You must have a confirmed email to log in");
                 }
             },
            function (error) {
                vm.passwordResetError = true;
                vm.passwordResetErrorMsg = $rootScope.translate("Server error occured while reseting password");
            });
        };

    }]);


})(angular.module('userLogin', ['ngMessages', 'ngCookies', 'gettext']));

