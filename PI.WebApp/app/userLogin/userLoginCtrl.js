'use strict';


(function (app) {

    app.factory('userManager', function ($http) {
        return {
            loginUser: function (newuser, url) {

                return $http.post(serverBaseUrl + '/' + url, newuser);
            }
        };

    });

    app.factory('registerExternalUser', function ($http) {

        return {
            createUser: function (newuser) {
                debugger;
                return $http.post(serverBaseUrl + '/api/accounts/create', newuser);
            }
        };

    });

    //var serviceBase = 'https://service.transportal.it/';
    //var serviceBase = 'https://localhost:44339/';
    var serviceBase = serverBaseUrl + '/';

    app.constant('ngAuthSettings', {
        apiServiceBaseUri: serviceBase,
        clientId: 'ngAuthApp'
    });

    app.run(function (gettextCatalog, $rootScope, $window) {

        
        gettextCatalog.setCurrentLanguage($window.localStorage.getItem('currentLnguage'));


        $rootScope.translate = function (str) {
            return gettextCatalog.getString(str);
        };

        //gettextCatalog.debug = true;
    });

    app.controller('userLoginCtrl', ['userManager', '$window', '$cookieStore', '$scope', '$rootScope', 'gettextCatalog',
        '$location', 'authService', 'registerExternalUser','ngAuthSettings',
    function (userManager, $window, $cookieStore, $scope, $rootScope, gettextCatalog, $location, authService, 
registerExternalUser,ngAuthSettings) {
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
        vm.UserModel = {};

        var loggedusername = $cookieStore.get('username');
        var loggedpassword = $cookieStore.get('password');
        $scope.user = {};
        // restore credentials for logged user
        if (loggedusername && loggedpassword) {
            $scope.user.username = loggedusername;
            $scope.user.password = loggedpassword;
        }

       // $auth.authenticate('facebook');

        vm.isConfirmEmail = function () {

            if (window.location.search != "") {
                // Show email confirm message.
                vm.emailConfirmationMessage = "To confirm your email address, please login using your username and password";
                vm.isEmailConfirm = true;

            }

        };

        vm.login = function (user) {
            debugger;
            if (!user.viaExternalLogin) {
                if (vm.rememberme == true) {
                    $cookieStore.put('username', user.username);
                    $cookieStore.put('password', user.password);
                }


                if (window.location.search != "") {

                    var splittedValues = window.location.search.replace("?", "").split('&');

                    if (splittedValues.length != 2 || splittedValues[0].split('=').length != 2 || splittedValues[1].split('=').length != 2) {
                        vm.emailConfirmationMessage = "Confirmation URL link is not properly formatted. Please resend the confirmation URL!";
                        return;
                    }

                    var userIdKeyValue = splittedValues[0].split('=');
                    var codeKeyValue = splittedValues[1].split('=');

                    if (userIdKeyValue[0] != 'userId') {
                        vm.emailConfirmationMessage = "Confirmation URL link is not properly formatted. Please resend the confirmation URL!";
                        return;
                    }
                    if (codeKeyValue[0] != 'code') {
                        vm.emailConfirmationMessage = "Confirmation URL link is not properly formatted. Please resend the confirmation URL!";
                        return;
                    }

                    user.userId = userIdKeyValue[1];
                    user.code = codeKeyValue[1];
                    user.isConfirmEmail = true;
                }

            }
            userManager.loginUser(user, 'api/accounts/LoginUser')
             .then(function (returnedResult) {

                 if (returnedResult.data.result == "1" || returnedResult.data.result == "2") {
                     debugger;
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
                     vm.loginInvalidMessage = $rootScope.translate("You must have a confirmed email to log in!");
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

            vm.pwdReset.templateLink = '<html><head><title></title></head><body style="margin:30px;"><div style="margin-right:40px;margin-left:40px"><div style="margin-top:30px;background-color:#0af;font-size:28px;border:5px solid #d9d9d9;text-align:center;padding:10px;font-family:verdana,geneva,sans-serif;color:#fff">Password Reset - Parcel International</div></div><div style="margin-right:40px;margin-left:40px"><div style="float:left;"><img alt="" src="http://www.parcelinternational.nl/assets/Uploads/_resampled/SetWidth495-id-parcel-big.jpg" style="width: 130px; height: 130px;" /></div><div><h3 style="margin-bottom:65px;margin-right:146px;margin-top:0;padding-top:62px;text-align:center;font-size:22px;font-family:verdana,geneva,sans-serif;color:#005c99">Password reset instructions</h3></div></div><div style="margin-right:40px;margin-left:40px"><div style="padding:10px;font-family:verdana,geneva,sans-serif;color:#fff;border:5px solid #0af;background-color:#005c99;font-size:13px"><p style="font-weight:700;font-style:italic;font-size:14px">Dear Customer,</p><br/><p style="font-weight:700;font-style:italic;font-size:14px">We received a request to reset the password associated with this email address. If you made this request,please follow the instructions below.</p><p style="font-weight:700;font-style:italic;font-size:14px">If you did not request to have your password reset, you can safely ignore this email.</p><p style="font-weight:700;font-style:italic;font-size:14px">Please click <span style="color:#80d4ff;font-size:14px;">ActivationURL</span> to reset the password.</p><p style="font-weight:700;font-style:italic;font-size:14px">IMPORTANT! Please note that this link is valid for 24 hours only. <p><p style="font-weight:700;font-style:italic;font-size:14px">Should you have any questions or concerns, please contact Parcel International helpdesk for support.</p></br><p style="font-weight:700;font-style:italic;font-size:14px">Thank you,</p><p style="font-weight:700;font-style:italic;font-size:14px">Parcel International Service Team</p><br/><p>Phone: <span style="font-size:14px;color:#80d4ff">+1 858 914 4414</span> </p><p>Email address:<a href="mailto:helpdesk@parcelinternational.com" style="color:#80d4ff">  helpdesk@parcelinternational.com</a></p><p>Website: <a href="http://www.parcelinternational.com" style="color:#80d4ff">www.parcelinternational.com</a></p></div><p><i>*** This is an automatically generated email, please do not reply ***</i></p></div></body></html>';

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
                 //else if (returnedResult.data == "-11") {
                 //    //No account find by this email.
                 //    vm.passwordResetError = true;
                 //    vm.passwordResetErrorMsg = $rootScope.translate("You must have a confirmed email to log in!");
                 //}
             },
            function (error) {
                vm.passwordResetError = true;
                vm.passwordResetErrorMsg = $rootScope.translate("Server error occured while reseting password");
            });
        };


        //$scope.authExternalProvider = function (provider) {
        //    debugger;
        //    //var redirectUri = location.protocol + '//' + location.host + '/app/index.html';
        //    var redirectUri = 'http://localhost:49995/app/authComplete.html';
        //    var externalProviderUrl = 'https://localhost:44339/' + "api/accounts/ExternalLogin?provider=" + provider
        //                                                                + "&response_type=token&client_id=" + 'ngAuthApp'
        //                                                                + "&redirect_uri=" + redirectUri;
        //    window.$windowScope = $scope;

        //    var oauthWindow = window.open(externalProviderUrl, "Authenticate Account", "location=0,status=0,width=600,height=750");
        //};

        $scope.authExternalProvider = function (provider) {
            debugger;

            var redirectUri = location.protocol + '//' + location.host + '/app/authComplete.html';

            // var externalProviderUrl = 'https://localhost:44339/'

            var externalProviderUrl  = "";
            if (provider == 'Microsoft')
            {
                console.log('mi');
                //externalProviderUrl = "https://login.microsoftonline.com/common/oauth2/v2.0/authorize?client_id=70a1a68c-0c5e-445f-8724-5e433fe463e1&scope=openid+profile&response_type=id_token&redirect_uri=https://locahost&nonce=111111111123";
                //externalProviderUrl = "https://login.microsoftonline.com/common/oauth2/v2.0/authorize?client_id=70a1a68c-0c5e-445f-8724-5e433fe463e1&scope=openid+profile&response_type=id_token&redirect_uri=https://service.transportal.it&nonce=111111111123";
                //externalProviderUrl = "https://login.live.com/oauth20_authorize.srf?client_id=70a1a68c-0c5e-445f-8724-5e433fe463e1&scope=SCOPES&response_type=code&redirect_uri=https://web.transportal.it/app/authComplete.html";

                externalProviderUrl = ngAuthSettings.apiServiceBaseUri + "api/accounts/ExternalLogin?provider=" +provider
                                                                        + "&response_type=token&client_id=" + 'ngAuthApp'
                                                                        + "&redirect_uri=" + redirectUri;
            }
            else{
            externalProviderUrl = ngAuthSettings.apiServiceBaseUri + "api/accounts/ExternalLogin?provider=" + provider
                                                                        + "&response_type=token&client_id=" + 'ngAuthApp'
                                                                        + "&redirect_uri=" + redirectUri;
            }
            window.$windowScope = $scope;

            var oauthWindow = window.open(externalProviderUrl, "Authenticate Account", "location=0,status=0,width=600,height=750");
        };


        $scope.authCompletedCB = function (fragment) {
            debugger;
            $scope.$apply(function () {

                if (fragment.haslocalaccount == 'False') {

                    authService.logOut();
                    
                    authService.externalAuthData = {
                        provider: fragment.provider,
                        userName: fragment.external_user_name,
                        firstName: fragment.external_first_name,
                        lastName: fragment.external_last_name,
                        externalAccessToken: fragment.external_access_token
                    };

                    vm.UserModel.Email = fragment.external_user_name;
                    vm.UserModel.viaExternalLogin = true;
                    vm.UserModel.firstName = fragment.external_first_name;
                    vm.UserModel.lastName = fragment.external_last_name;
                    debugger;
                    
                    // register external user
                    registerExternalUser.createUser(vm.UserModel)

                    .then(function (result) {
                        debugger;
                       var userDetails = {
                            username: fragment.external_user_name,
                            viaExternalLogin: true
                       };
                       debugger;
                       vm.login(userDetails);
                    },
                    function (error) {
                        console.log("failed");
                    }
                    );

                    // end of register external user
                    
                    debugger;
                }
                else {
                    //Obtain access token and redirect to orders
                    var externalData = { provider: fragment.provider, externalAccessToken: fragment.external_access_token };
                    authService.obtainAccessToken(externalData).then(function (response) {
                        debugger;
                        
                        var userDetails = {
                            username: fragment.external_user_name,
                            viaExternalLogin: true
                        };
                        debugger;
                        vm.login(userDetails);
                    },
                 function (err) {
                     debugger;
                     $scope.message = err.error_description;
                 });
                }

            });
        }

    }]);


})(angular.module('userLogin', ['ngMessages', 'ngCookies', 'gettext', 'LocalStorageModule']));

