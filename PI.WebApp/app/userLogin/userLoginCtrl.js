'use strict';


(function (app) {

    app.factory('userManager', function ($http) {
        return {
            loginUser: function (newuser, url) {

                return $http.post(serverBaseUrl + '/' + url, newuser);
            },
            isPhoneNumberVerified: function (email) {
                return $http.get(serverBaseUrl + '/api/accounts/IsPhoneNumberVerified', {
                    params: {
                        email: email //$localStorage.userGuid
                    }
                });
            },
            stampHealthCheck: function () {

                return $http.get(serverBaseUrl + '/api/accounts/stampHealthCheck');
            }
        };

    });

    app.factory('registerExternalUser', function ($http) {

        return {
            createUser: function (newuser) {
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
        '$location', 'authService', 'registerExternalUser', 'ngAuthSettings',
    function (userManager, $window, $cookieStore, $scope, $rootScope, gettextCatalog, $location, authService,
registerExternalUser, ngAuthSettings) {
        var vm = this;
        //$localStorage.userGuid = '';
        $window.localStorage.setItem('userGuid', '');
        $window.localStorage.setItem('currentLnguage', '');
        $window.localStorage.setItem('paramSource', '');
        $window.localStorage.setItem('paramSourceId','');
        gettextCatalog.setCurrentLanguage($window.localStorage.getItem('currentLnguage'));

        vm.loginInvalid = false;
        vm.invalidToken = false;
        vm.isEmailConfirm = false;

        vm.isSentPasswordResetMail = false;
        vm.passwordResetError = false;
        vm.UserModel = {};

        vm.signInLodingSymbole = false;

        var loggedusername = $cookieStore.get('username');
        var loggedpassword = $cookieStore.get('password');
        $scope.user = {};
        // restore credentials for logged user
        if (loggedusername && loggedpassword) {
            $scope.user.username = loggedusername;
            $scope.user.password = loggedpassword;
        }

        // $auth.authenticate('facebook');

        vm.stampHealthCheck = function () {
            debugger;
            userManager.stampHealthCheck().then(function (response) {

                console.log('stampHealthCheck success');
                console.log(response);

            }, function (error) {

                console.log('stampHealthCheck error');
                console.log(error);

            });

        };

        vm.isConfirmEmail = function () {

            if (window.location.search != "") {
                // Show email confirm message.
                vm.emailConfirmationMessage = "To confirm your email address, please login using your username and password";
                vm.isEmailConfirm = true;

            }

        };

        vm.login = function (user) {
            //vm.signInLodingSymbole = true;
            if (!user.viaExternalLogin) {
                vm.signInLodingSymbole = true;
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
                 vm.signInLodingSymbole = true;
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
                     vm.signInLodingSymbole = false;
                     $cookieStore.remove('username');
                     $cookieStore.remove('password');

                     $.noty.defaults.killer = true;

                     noty({
                         text: '<p style="font-size:medium">' + $rootScope.translate('Error') + '! </p>' + returnedResult.data.message,
                         layout: 'topRight',
                         type: 'error',
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
                     //vm.signInLodingSymbole = false;
                     $cookieStore.remove('username');
                     $cookieStore.remove('password');
                 }
                 else if (returnedResult.data.result == "-11") {
                     vm.loginInvalid = true;
                     //vm.signInLodingSymbole = false;
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

        
        vm.resetForgetPassword = function (option) {
             
            if (option == "false") {
                vm.resetForgetPasswordUsingEmail();
            }
            else {
                vm.resetForgetPasswordUsingSMS();
            }
        }


        vm.resetForgetPasswordUsingEmail = function () {
            debugger;
            vm.pwdReset.templateLink = '<html><head><title></title></head><body style="margin-left:30px;margin-right:30px;margin-top:30px"><div style="margin-right:40px;margin-left:40px"><div style="margin-top:30px;background-color:#0af;font-size:24px;text-align:center;padding:10px;font-family:verdana,geneva,sans-serif;color:#fff">Password Reset - 12Send</div></div><div style="margin-right:40px;margin-left:40px"><div style="float:left;"><img alt="" src="http://www.12send.com/template/logo_12send.png" style="width: 193px; height: 100px;" /></div><div><h3 style="margin-bottom:10px;margin-right:146px;margin-top:0;padding-top:62px;text-align:center;font-family:verdana,geneva,sans-serif;color:#000">Password reset instructions</h3></div></div><div style="margin-right:40px;margin-left:40px"><div style="padding:10px;font-family:verdana,geneva,sans-serif;color:#000;font-size:13px"><p style="font-style:italic;font-size:13px">Dear FirstName LastName,</p><br/><p style="font-style:italic;font-size:13px">We received a request to reset the password associated with this email address. If you made this request, please go to the following page and choose a new password <span style="color:#005c99;font-size:13px;">ActivationURL</span></p><p style="font-style:italic;font-size:13px">If you did not make this request, please disregard this email.</p><p style="font-style:italic;font-size:13px">Thank you for using One2send, we look forward to providing a shipment for you soon. If you need assistance, you can reach out to our One2send Service Team.</p><br/><p style="color:#000">Phone: <span style="font-size:14px;color:#005c99">+1 858 914 4414</span> </p><p style="color:#000">Email address: <a href="support@12send.com" style="color:#005c99">support@12send.com</a></p><p style="color:#000">Website: <a href="http://www.12send.com" style="color:#005c99">www.12send.com </a></p></div><p><i>*** This is an automatically generated email, please do not reply ***</i></p></div></body></html>';

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


        vm.resetForgetPasswordUsingSMS = function () {
             
            userManager.loginUser(vm.pwdReset, 'api/accounts/SendOPTCodeForPhoneValidation')
             .then(function (returnedResult) {
                 if (returnedResult.status == 200) {
                     vm.passwordResetError = false;
                     vm.isSentSecurityCode = true;
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


        vm.submitSecurityCode = function () {
             
            userManager.loginUser(vm.pwdReset, 'api/accounts/VerifyPhoneCode')
             .then(function (returnedResult) {
                 if (returnedResult.status == 200) {
                      
                     if(returnedResult.data)
                     {
                         $window.localStorage.setItem('isViaPhone', true);
                         $window.localStorage.setItem('email', vm.pwdReset.email);
                         // Redirect to Reset password screen
                         window.location = webBaseUrl + "/app/resetPassword/resetPassword.html";
                     }
                     else
                     {
                         vm.passwordResetError = true;
                         vm.passwordResetErrorMsg = $rootScope.translate('Invalid security code.');
                     }
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

        vm.isEmailPhoneNumberVerified = function ($event) {
             
            userManager.isPhoneNumberVerified(vm.pwdReset.email)
             .then(function (returnedResult) {
                 if (returnedResult.status == 200) {
                      
                     if (returnedResult.data.result == 1) {
                         vm.showNext = true;
                         vm.isEnabled = true;
                         vm.passwordResetError = false;
                         vm.isEmailNotFound = false;
                     }
                     else if (returnedResult.data.result == 0) {
                         // vm.showError = true;
                         vm.showNext = true;
                         vm.isEnabled = false;
                         vm.passwordResetError = false;
                         vm.isEmailNotFound = false;
                         //vm.errorMessage = $rootScope.translate('Phone Number is not verified!');
                     }
                     else if (returnedResult.data.result == -1) {
                         vm.showNext = false;
                         vm.passwordResetError = true;
                         vm.isEmailNotFound = true;
                         vm.passwordResetErrorMsg = $rootScope.translate('No account found for this email!');
                     }
                 }
             },
            function (error) {
                vm.isDisabled = false;
            });
        };

        //$scope.authExternalProvider = function (provider) {
        //    //var redirectUri = location.protocol + '//' + location.host + '/app/index.html';
        //    var redirectUri = 'http://localhost:49995/app/authComplete.html';
        //    var externalProviderUrl = 'https://localhost:44339/' + "api/accounts/ExternalLogin?provider=" + provider
        //                                                                + "&response_type=token&client_id=" + 'ngAuthApp'
        //                                                                + "&redirect_uri=" + redirectUri;
        //    window.$windowScope = $scope;

        //    var oauthWindow = window.open(externalProviderUrl, "Authenticate Account", "location=0,status=0,width=600,height=750");
        //};

        $scope.authExternalProvider = function (provider) {

            var redirectUri = location.protocol + '//' + location.host + '/app/authComplete.html';

            // var externalProviderUrl = 'https://localhost:44339/'

            var externalProviderUrl = "";
            if (provider == 'Microsoft') {
                console.log('mi');
                //externalProviderUrl = "https://login.microsoftonline.com/common/oauth2/v2.0/authorize?client_id=70a1a68c-0c5e-445f-8724-5e433fe463e1&scope=openid+profile&response_type=id_token&redirect_uri=https://locahost&nonce=111111111123";
                //externalProviderUrl = "https://login.microsoftonline.com/common/oauth2/v2.0/authorize?client_id=70a1a68c-0c5e-445f-8724-5e433fe463e1&scope=openid+profile&response_type=id_token&redirect_uri=https://service.transportal.it&nonce=111111111123";
                //externalProviderUrl = "https://login.live.com/oauth20_authorize.srf?client_id=70a1a68c-0c5e-445f-8724-5e433fe463e1&scope=SCOPES&response_type=code&redirect_uri=https://web.transportal.it/app/authComplete.html";

                externalProviderUrl = ngAuthSettings.apiServiceBaseUri + "api/accounts/ExternalLogin?provider=" + provider
                                                                        + "&response_type=token&client_id=" + 'ngAuthApp'
                                                                        + "&redirect_uri=" + redirectUri;
            }
            else {
                externalProviderUrl = ngAuthSettings.apiServiceBaseUri + "api/accounts/ExternalLogin?provider=" + provider
                                                                            + "&response_type=token&client_id=" + 'ngAuthApp'
                                                                            + "&redirect_uri=" + redirectUri;
            }
            window.$windowScope = $scope;

            var oauthWindow = window.open(externalProviderUrl, "Authenticate Account", "location=0,status=0,width=600,height=750");
        };


        $scope.authCompletedCB = function (fragment) {

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

                    // register external user
                    registerExternalUser.createUser(vm.UserModel)

                    .then(function (result) {
                        var userDetails = {
                            username: fragment.external_user_name,
                            viaExternalLogin: true
                        };

                        vm.login(userDetails);
                    },
                    function (error) {
                        console.log("failed");
                    }
                    );

                    // end of register external user
                }
                else {
                    //Obtain access token and redirect to orders
                    var externalData = { provider: fragment.provider, externalAccessToken: fragment.external_access_token };
                    authService.obtainAccessToken(externalData).then(function (response) {

                        var userDetails = {
                            username: fragment.external_user_name,
                            viaExternalLogin: true
                        };

                        vm.login(userDetails);
                    },
                 function (err) {

                     $scope.message = err.error_description;
                 });
                }

            });
        }

    }]);


})(angular.module('userLogin', ['ngMessages', 'ngCookies', 'gettext', 'LocalStorageModule']));

