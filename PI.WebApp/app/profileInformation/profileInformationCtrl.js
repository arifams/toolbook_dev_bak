﻿'use strict';


(function (app) {

    app.factory('updateProfilefactory', function ($http) {
        return {
            updateProfileInfo: function (updatedProfile) {
                return $http.post(serverBaseUrl + '/api/profile/UpdateProfile', updatedProfile);
            },
            updateProfileGeneral: function (updatedProfile) {
                return $http.post(serverBaseUrl + '/api/profile/UpdateProfileGeneral', updatedProfile);
            },
            updateProfileAddress: function (updatedProfile) {
                return $http.post(serverBaseUrl + '/api/profile/UpdateProfileAddress', updatedProfile);
            },
            updateProfileBillingAddress: function (updatedProfile) {
                return $http.post(serverBaseUrl + '/api/profile/UpdateProfileBillingAddress', updatedProfile);
            },

            UpdateSetupWizardBillingAddress: function (updatedProfile) {
                return $http.post(serverBaseUrl + '/api/profile/UpdateSetupWizardBillingAddress', updatedProfile);
            },

            updateProfileLoginDetails: function (updatedProfile) {
                return $http.post(serverBaseUrl + '/api/profile/updateProfileLoginDetails', updatedProfile);
            },
            updateProfileAccountSettings: function (updatedProfile) {
                return $http.post(serverBaseUrl + '/api/profile/updateProfileAccountSettings', updatedProfile);
            },
            updateThemeColour: function (updatedProfile) {
                return $http.post(serverBaseUrl + '/api/profile/updateThemeColour', updatedProfile);
            },
            SendOPTCodeForPhoneValidation: function (updatedProfile) {
                return $http.post(serverBaseUrl + '/api/accounts/SendOPTCodeForPhoneValidation', updatedProfile);
            },
            VerifyPhoneCode: function (updatedProfile) {
                return $http.post(serverBaseUrl + '/api/accounts/VerifyPhoneCode', updatedProfile);
            },
            isPhoneNumberVerified: function (email) {
                return $http.get(serverBaseUrl + '/api/accounts/IsPhoneNumberVerified', {
                    params: {
                        email: email
                    }
                })
            }
        }

    });


    app.factory('loadProfilefactory', function ($http, $window) {
        return {
            loadProfileinfo: function (userId) {
               
                return $http.get(serverBaseUrl + '/api/profile/GetProfile', {
                    params: {
                        userId: userId //$localStorage.userGuid
                    }
                });
            }
        }

    });


    //loading All account setting dropdowns.
    app.factory('getAllAccountSettings', function ($http) {
        return {
            getAllAccountSettings: function (customerId) {
                return $http.get(serverBaseUrl + '/api/profile/GetAllAccountSettings', {
                    params: {
                        customerId: customerId //$localStorage.userGuid
                    }
                });
            }
        }

    });


    app.factory('getCustomerAddressDetails', function ($http) {
        return {
            getCustomerAddressDetails: function (cusomerAddressId, companyId) {
                return $http.get(serverBaseUrl + '/api/profile/GetCustomerAddressDetails', {
                    params: {
                        cusomerAddressId: cusomerAddressId,
                        companyId: companyId
                    }
                });
            }
        }

    });


    app.directive('validPasswordC', function () {
        return {
            require: 'ngModel',
            link: function (scope, elm, attrs, ctrl) {
                ctrl.$parsers.unshift(function (viewValue, $scope) {
                    var noMatch = viewValue != scope.formUpdateProfileLogin.newpassword.$viewValue;
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
                    if (scope.formUpdateProfileLogin.newpassword_c.$viewValue != '') {
                        var noMatch = viewValue != scope.formUpdateProfileLogin.newpassword_c.$viewValue;
                        scope.formUpdateProfileLogin.newpassword_c.$setValidity('noMatch', !noMatch);
                    }

                    return viewValue;
                })
            }
        }
    });

    app.controller('profileInformationCtrl',
        ['loadProfilefactory', 'updateProfilefactory', 'getAllAccountSettings', 'getCustomerAddressDetails', 'customBuilderFactory', '$window', '$rootScope', 'Upload', 'gettextCatalog', '$scope','$routeParams',
    function (loadProfilefactory, updateProfilefactory, getAllAccountSettings, getCustomerAddressDetails, customBuilderFactory, $window, $rootScope, Upload, gettextCatalog, $scope, $routeParams) {


        //applicationService.init();

        //mainColor();

        // return if user not logged. -- Need to move this to global service.
        if ($window.localStorage.getItem('userGuid') == '' || $window.localStorage.getItem('userGuid') == undefined) {

            window.location = webBaseUrl + "/app/userLogin/userLogin.html";
            return;
        }

        var vm = this;

        vm.loading = false;

        vm.t_timezones;
        vm.model = {};
        vm.model.accountSettings = {};
        vm.model.doNotUpdateAccountSettings = false;
        vm.isImagetype = false;
        vm.emailCopy = '';
        vm.errorCodeCustomer = false;
        vm.errorCodeBilling = false;
        vm.isverifyClicked = false;
        vm.showCustomerName = false;


        vm.isImage = function (ext) {

            if (ext == "image/jpg" || ext == "image/jpeg" || ext == "image/gif" || ext == "image/png") {

                return vm.isImagetype = true;
            } else {

                vm.isImagetype = false;
            }
        }

        vm.OnLogoChange = function (logo) {
            if (logo.length > 0) {
                vm.isImage(logo[0].type);
            }

        }

        //auto update the default language bofore the accept
        vm.getCurrentLnguage = function (language) {

            if (language.languageCode == "en") {
                $window.localStorage.setItem('currentLnguage', "")
                gettextCatalog.setCurrentLanguage("");
            }
            else {
                $window.localStorage.setItem('currentLnguage', language.languageCode);
                gettextCatalog.setCurrentLanguage(language.languageCode);
            }
            //$route.reload();
        };

        vm.CheckMail = function () {
            if (vm.model.customerDetails.email == vm.model.customerDetails.secondaryEmail) {
                vm.invalidEmail = true;
            }
            else {
                vm.invalidEmail = false;
            }

        }
        //clear password data 
        vm.clearPassword = function () {

            if (vm.model.changeLoginData == false) {

                vm.model.newPassword = "";
                vm.model.confirmPassword = "";
                vm.model.oldPassword = "";
            }
        };

        //vm.changeCountry = function () {
        //    vm.isRequiredState = vm.model.customerDetails.customerAddress.country == 'US' || vm.model.customerDetails.customerAddress.country == 'CA' || vm.model.customerDetails.customerAddress.country == 'PR' || vm.model.customerDetails.customerAddress.country == 'AU';
        //};

        vm.changeBillingCountry = function () {
            vm.isRequiredBillingState = vm.model.companyDetails.costCenter.billingAddress.country == 'US' || vm.model.companyDetails.costCenter.billingAddress.country == 'CA' || vm.model.companyDetails.costCenter.billingAddress.country == 'PR' || vm.model.companyDetails.costCenter.billingAddress.country == 'AU';
        };

        vm.useCorpAddressAsBilling = function () {

            if (vm.model.customerDetails.isCorpAddressUseAsBusinessAddress == true) {
                vm.model.companyDetails.costCenter = {};
                vm.model.companyDetails.costCenter.billingAddress = vm.model.customerDetails.customerAddress;
                if (vm.model.customerDetails.phoneNumber) {

                    vm.model.companyDetails.costCenter.phoneNumber = vm.model.customerDetails.phoneNumber;

                } else if (vm.model.customerDetails.mobileNumber) {

                    vm.model.companyDetails.costCenter.phoneNumber = vm.model.customerDetails.mobileNumber;
                }
            }

            if (vm.model.customerDetails.isCorpAddressUseAsBusinessAddress == false) {
                vm.model.companyDetails.costCenter = {};
                vm.model.companyDetails.costCenter.billingAddress = {};
                vm.model.companyDetails.costCenter.phoneNumber = "";
            }
        };

        vm.loadCustomize = function () {
            customBuilderFactory.init();
        };


        vm.loadAddressInfo = function () {
            vm.loading = true;
            getCustomerAddressDetails.getCustomerAddressDetails(vm.model.customerDetails.addressId, vm.model.companyDetails.id)
             .then(function successCallback(response) {
                 vm.loading = false;
                 
                 if (response.data.customerDetails != null) {

                     vm.isPhoneNumberVerified();
                     vm.originalPhone = vm.model.customerDetails.mobileNumber;
                     vm.originalVerifiedStatus = vm.isVerified;

                     // vm.model.customerDetails = response.data.customerDetails;
                     vm.model.customerDetails.customerAddress = response.data.customerDetails.customerAddress;
                     // vm.model.companyDetails = response.data.companyDetails;
                     vm.model.customerDetails.customerAddress = response.data.customerDetails.customerAddress;
                     vm.model.companyDetails.costCenter = response.data.companyDetails.costCenter;
                     //vm.changeCountry();

                     if (response.data.companyDetails.costCenter != null) {
                         vm.multipleCostCenters = true;
                     }
                     else {
                         vm.multipleCostCenters = false;
                     }


                     if (response.data.companyDetails.costCenter != null &&
                         response.data.companyDetails.costCenter.billingAddress != null) {

                         vm.model.companyDetails.costCenter.billingAddress = response.data.companyDetails.costCenter.billingAddress;
                         vm.changeBillingCountry();
                     }
                     else {
                         vm.model.companyDetails.costCenter = { billingAddress: { country: 'US' } };
                         vm.changeBillingCountry();
                     }                    
                 }

             }, function errorCallback(response) {
                 vm.loading = false;
                 //todo
             });
        }

        vm.loadProfile = function (userId) {
            vm.loading = true;
            
            loadProfilefactory.loadProfileinfo(userId)
            .success(function (response) {

                if (response != null) {
                    vm.model = response;
                    vm.loading = false;

                    if (response.customerDetails != null) {
                        //setting the account type    

                        vm.model.customerDetails = response.customerDetails;
                        vm.model.companyDetails = response.companyDetails;
                        vm.emailCopy = response.customerDetails.email;
                        vm.loadAddressInfo();

                        if (response.customerDetails.isCorporateAccount) {
                            vm.model.customerDetails.isCorporateAccount = "true";
                            vm.corpAccount = "true";
                        }
                        else {
                            vm.model.customerDetails.isCorporateAccount = "false";
                        }


                    }
                }
            })
           .error(function () {
               vm.model.isServerError = "true";
               vm.loading = false;
           })
        }

        //loading the profile according to the logged in user
        if ($routeParams.id != "0") {

            vm.Loggeduser = $routeParams.id;
            // (ShipmentCode, ShipmentId)
            vm.showCustomerName = true;
            vm.loadProfile(vm.Loggeduser);
        } else {
            vm.loadProfile($window.localStorage.getItem('userGuid'));
        }

        vm.loadAccountSettings = function () {
            vm.loading = true;

            getAllAccountSettings.getAllAccountSettings(vm.model.customerDetails.id)
             .then(function successCallback(response) {
                 
                 vm.loading = false;

                 vm.languageList = response.data.accountSettings.languages;
                 vm.defaultLanguage = response.data.accountSettings.languages[0];

                 vm.currencyList = response.data.accountSettings.currencies;
                 vm.defaultCurrency = response.data.accountSettings.currencies[0];

                 vm.timezoneList = response.data.accountSettings.timeZones;
                 vm.defaultTimezone = response.data.accountSettings.timeZones[0];

                 if (response.data.accountSettings.defaultCurrencyId != 0)
                     vm.defaultCurrency = { id: response.data.accountSettings.defaultCurrencyId };
                 if (response.data.accountSettings.defaultLanguageId != 0)
                     vm.defaultLanguage = { id: response.data.accountSettings.defaultLanguageId };
                 if (response.data.accountSettings.defaultTimeZoneId != 0)
                     vm.defaultTimezone = { id: response.data.accountSettings.defaultTimeZoneId };

                 vm.model.bookingConfirmation = response.data.bookingConfirmation;
                 vm.model.pickupConfirmation = response.data.pickupConfirmation;
                 vm.model.shipmentDelay = response.data.shipmentDelay;
                 vm.model.shipmentException = response.data.shipmentException;
                 vm.model.notifyNewSolution = response.data.notifyNewSolution;
                 vm.model.notifyDiscountOffer = response.data.notifyDiscountOffer;
                 
                 vm.model.defaultVolumeMetricId = response.data.accountSettings.defaultVolumeMetricId;
                 vm.model.defaultWeightMetricId = response.data.accountSettings.defaultWeightMetricId;

             }, function errorCallback(response) {
                 vm.loading = false;
                 //todo
             });
        }


        function getSuccessMessage(body, responce) {

            body.stop().animate({ scrollTop: 0 }, '500', 'swing', function () {
            });

            $('#panel-notif').noty({
                text: '<div class="alert alert-success media fade in"><p>' + $rootScope.translate('Profile updated successfully!') + '</p></div>',
                layout: 'bottom-right',
                theme: 'made',
                animation: {
                    open: 'animated bounceInLeft',
                    close: 'animated bounceOutLeft'
                },
                timeout: 3000,
            });
        }

        function getErrorMessage(body, error) {

            var errorMessage = error.data.message;

            if (error.data.message == "" || error.data.message == undefined) {
                errorMessage = "Error occured while processing your request";
            }

            body.stop().animate({ scrollTop: 0 }, '500', 'swing', function () {
            });

            $('#panel-notif').noty({
                text: '<div class="alert alert-warning media fade in"><p>' + $rootScope.translate(errorMessage) + '</p></div>',
                layout: 'bottom-right',
                theme: 'made',
                animation: {
                    open: 'animated bounceInLeft',
                    close: 'animated bounceOutLeft'
                },
                timeout: 3000,
            });
        }

        // Split the User Profile update to seperate sections.

        vm.updateProfileGeneral = function () {

            //vm.model.customerDetails.userId = $window.localStorage.getItem('userGuid');
            vm.model.loggedUserId = $window.localStorage.getItem('userGuid');

            var body = $("html, body");

            body.stop().animate({ scrollTop: 0 }, '500', 'swing', function () {
            });

            $('#panel-notif').noty({
                text: '<div class="alert alert-success media fade in"><p>' + $rootScope.translate('Are you sure you want to update your profile') + '?</p></div>',
                buttons: [
                        {
                            addClass: 'btn btn-primary', text: $rootScope.translate('Ok'), onClick: function ($noty) {

                                $noty.close();
                                vm.model.customerDetails.templateLink = '<html><head><title></title></head><body style="margin-left:40px;margin-right:40p;margin-top:30px"><div style="margin-right:40px;margin-left:40px"><div style="margin-top:30px;background-color:#0af;font-size:24px;text-align:center;padding:10px;font-family:verdana,geneva,sans-serif;color:#fff">Email Verification-12Send</div></div><div style="margin-right:40px;margin-left:40px"><div style="float:left;"><img alt="" src="http://www.12send.com/template/logo_12send.png" style="width: 193px; height: 100px;" /></div><h3 style="margin-bottom:65px;margin-right:146px;margin-top:0;padding-top:62px;text-align:center;font-size:20px;font-family:verdana,geneva,sans-serif;color:#000">Email verification required</h3></div><div style="margin-right:40px;margin-left:40px"><div style="padding:10px;font-family:verdana,geneva,sans-serif;color:#000;font-size:13px"><p style="font-style:italic">Dear Salutation FirstName LastName,</p><br/><p style="font-style:italic">Welcome to One2send, we are looking forward to supporting your shipping needs.Your email has been updated, but before you can start shipping, please click <span style="color:#005c99;font-size:13px;">ActivationURL</span> to verify your email address.</p><p style="font-style:italic">The link will be valid for 24 hours.<p><p style="font-style:italic">In need of assistance? Reach out to our support team.</p></br>Phone: <span style="font-size:13px;color:#005c99">+1 858 914 4414</span> </br>Email address:<a href="support@12send.com" style="color:#005c99">support@12send.com</a></br>Website: <a href="http://www.12send.com" style="color:#005c99">www.12send.com</a></div><p><i>*** This is an automatically generated email, please do not reply ***</i></p></div></body></html>';

                                var updatedtoCorporate = false;

                                if (vm.model.companyDetails.name != null && $window.localStorage.getItem('isCorporateAccount') == 'false') {

                                    $window.localStorage.setItem('isCorporateAccount', true);
                                    updatedtoCorporate = true;
                                }

                                updateProfilefactory.updateProfileGeneral(vm.model)
                                        .then(function (responce) {

                                            if (responce.status == 200) {

                                                getSuccessMessage(body, responce);

                                                if (updatedtoCorporate == true) {
                                                    $window.location.reload();
                                                }
                                            }

                                        },
                                        function (error) {
                                            getErrorMessage(body, error);
                                        });
                            }
                        },
                        {
                            addClass: 'btn btn-danger', text: $rootScope.translate('Cancel'), onClick: function ($noty) {

                                $noty.close();
                                return;
                            }
                        }
                ],
                layout: 'bottom-right',
                theme: 'made',
                animation: {
                    open: 'animated bounceInLeft',
                    close: 'animated bounceOutLeft'
                },
                timeout: 3000,
            });

        }

        vm.updateProfileAddress = function () {

            vm.model.customerDetails.userId = $window.localStorage.getItem('userGuid');

            var body = $("html, body");
            body.stop().animate({ scrollTop: 0 }, '500', 'swing', function () {
            });

            $('#panel-notif').noty({
                text: '<div class="alert alert-success media fade in"><p>' + $rootScope.translate('Are you sure you want to update the Profile') + '?</p></div>',
                buttons: [
                        {
                            addClass: 'btn btn-primary', text: $rootScope.translate('Ok'), onClick: function ($noty) {

                                $noty.close();

                                updateProfilefactory.updateProfileAddress(vm.model)
                                                .then(function (responce) {
                                                    if (responce.status == 200) {
                                                        getSuccessMessage(body, responce);
                                                    }

                                                },
                                                function (error) {
                                                    getErrorMessage(body, error);
                                                });
                            }
                        },
                        {
                            addClass: 'btn btn-danger', text: $rootScope.translate('Cancel'), onClick: function ($noty) {
                                $noty.close();
                                return;
                            }
                        }
                ],
                layout: 'bottom-right',
                theme: 'made',
                animation: {
                    open: 'animated bounceInLeft',
                    close: 'animated bounceOutLeft'
                },
                timeout: 3000,
            });

        }

        vm.updateProfileBillingAddress = function () {

            vm.model.customerDetails.userId = $window.localStorage.getItem('userGuid');

            var body = $("html, body");

            body.stop().animate({ scrollTop: 0 }, '500', 'swing', function () {
            });

            $('#panel-notif').noty({
                text: '<div class="alert alert-success media fade in"><p>' + $rootScope.translate('Are you sure you want to update the profile') + '?</p></div>',
                buttons: [
                        {
                            addClass: 'btn btn-primary', text: $rootScope.translate('Ok'), onClick: function ($noty) {

                                $noty.close();

                                updateProfilefactory.updateProfileBillingAddress(vm.model)
                                                .then(function (responce) {

                                                    if (responce.status == 200) {
                                                        getSuccessMessage(body, responce);
                                                    }
                                                },
                                                function (error) {
                                                    getErrorMessage(body, error);
                                                });
                            }
                        },
                        {
                            addClass: 'btn btn-danger', text: $rootScope.translate('Cancel'), onClick: function ($noty) {
                                $noty.close();
                                return;
                            }
                        }
                ],
                layout: 'bottom-right',
                theme: 'made',
                animation: {
                    open: 'animated bounceInLeft',
                    close: 'animated bounceOutLeft'
                },
                timeout: 3000,
            });

        }

        vm.updateProfileLoginDetails = function () {

            vm.model.customerDetails.userId = $window.localStorage.getItem('userGuid');

            var body = $("html, body");

            if ((vm.model.newPassword && vm.model.oldPassword) && vm.model.newPassword == vm.model.oldPassword && vm.model.changeLoginData == true) {

                body.stop().animate({ scrollTop: 0 }, '500', 'swing', function () {
                });

                $('#panel-notif').noty({
                    text: '<div class="alert alert-warning media fade in"><p>' + $rootScope.translate('New password cannot be same as old password') + '</p></div>',
                    layout: 'bottom-right',
                    theme: 'made',
                    animation: {
                        open: 'animated bounceInLeft',
                        close: 'animated bounceOutLeft'
                    },
                    timeout: 3000,
                });

                return;
            }

            body.stop().animate({ scrollTop: 0 }, '500', 'swing', function () {
            });

            $('#panel-notif').noty({
                text: '<div class="alert alert-success media fade in"><p>' + $rootScope.translate('Are you sure you want to update the profile') + '?</p></div>',
                buttons: [
                        {
                            addClass: 'btn btn-primary', text: $rootScope.translate('Ok'), onClick: function ($noty) {

                                $noty.close();

                                updateProfilefactory.updateProfileLoginDetails(vm.model)
                                                .then(function (responce) {
                                                    if (responce.status == 200) {
                                                        getSuccessMessage(body, responce);
                                                    }
                                                },
                                                function (error) {

                                                    getErrorMessage(body, error);
                                                });
                            }
                        },
                        {
                            addClass: 'btn btn-danger', text: $rootScope.translate('Cancel'), onClick: function ($noty) {
                                $noty.close();
                                return;
                            }
                        }
                ],
                layout: 'bottom-right',
                theme: 'made',
                animation: {
                    open: 'animated bounceInLeft',
                    close: 'animated bounceOutLeft'
                },
                timeout: 3000,
            });
        }

        vm.updateProfileAccountSettings = function () {

            vm.model.customerDetails.userId = $window.localStorage.getItem('userGuid');

            if (vm.defaultLanguage != undefined) {
                vm.model.defaultLanguageId = vm.defaultLanguage.id;
                vm.model.defaultCurrencyId = vm.defaultCurrency.id;
                vm.model.defaultTimeZoneId = vm.defaultTimezone.id;
            }
            else {
                vm.model.doNotUpdateAccountSettings = true;
            }

            var body = $("html, body");

            body.stop().animate({ scrollTop: 0 }, '500', 'swing', function () {
            });

            $('#panel-notif').noty({
                text: '<div class="alert alert-success media fade in"><p>' + $rootScope.translate('Are you sure you want to update the profile') + '?</p></div>',
                buttons: [
                        {
                            addClass: 'btn btn-primary', text: $rootScope.translate('Ok'), onClick: function ($noty) {

                                $noty.close();

                                updateProfilefactory.updateProfileAccountSettings(vm.model)
                                                .then(function (responce) {
                                                    if (responce.status == 200) {
                                                        getSuccessMessage(body, responce);
                                                    }
                                                },
                                                function (error) {
                                                    getErrorMessage(body, error);
                                                });
                            }
                        },
                        {
                            addClass: 'btn btn-danger', text: $rootScope.translate('Cancel'), onClick: function ($noty) {
                                $noty.close();
                                return;
                            }
                        }
                ],
                layout: 'bottom-right',
                theme: 'made',
                animation: {
                    open: 'animated bounceInLeft',
                    close: 'animated bounceOutLeft'
                },
                timeout: 3000,
            });
        }

        vm.updateThemeColour = function () {

            vm.model.customerDetails.userId = $window.localStorage.getItem('userGuid');

            var body = $("html, body");

            body.stop().animate({ scrollTop: 0 }, '500', 'swing', function () {
            });

            $('#panel-notif').noty({
                text: '<div class="alert alert-success media fade in"><p>' + $rootScope.translate('Are you want to update the theme colour') + '?</p></div>',
                buttons: [
                        {
                            addClass: 'btn btn-primary', text: $rootScope.translate('Ok'), onClick: function ($noty) {

                                $noty.close();

                                updateProfilefactory.updateThemeColour(vm.model)
                                              .then(function (responce) {
                                                  if (responce.status == 200) {
                                                      getSuccessMessage(body, responce);
                                                  }
                                              },
                                                function (error) {
                                                    getErrorMessage(body, error);
                                                });
                            }
                        },
                        {
                            addClass: 'btn btn-danger', text: $rootScope.translate('Cancel'), onClick: function ($noty) {
                                $noty.close();
                                return;
                            }
                        }
                ],
                layout: 'bottom-right',
                theme: 'made',
                animation: {
                    open: 'animated bounceInLeft',
                    close: 'animated bounceOutLeft'
                },
                timeout: 3000,
            });
        }

        vm.uploadLogo = function (file) {
            
            file.upload = Upload.upload({
                url: serverBaseUrl + '/api/Company/UploadLogo',
                data: {
                    file: file,
                    userId: $window.localStorage.getItem('userGuid'),
                    documentType: "LOGO",
                    customerId: vm.model.customerDetails.id
                },
            });

            file.upload.then(function (response) {

                if (response.status == 200) {
                    var body = $("html, body");
                    body.stop().animate({ scrollTop: 0 }, '500', 'swing', function () {
                    });

                    if (vm.showCustomerName) {
                        // mean admin change data behalf of user.
                        getSuccessMessage(body, response);
                    }
                    else {
                        $('#panel-notif').noty({
                            text: '<div class="alert alert-success media fade in"><p>' + $rootScope.translate('Company logo updated, click ok to reload the page') + '?</p></div>',
                            buttons: [
                                    {
                                        addClass: 'btn btn-primary', text: $rootScope.translate('Ok'), onClick: function ($noty) {
                                            $noty.close();
                                            $window.location.reload();

                                        }
                                    }

                            ],
                            layout: 'bottom-right',
                            theme: 'made',
                            animation: {
                                open: 'animated bounceInLeft',
                                close: 'animated bounceOutLeft'
                            },
                            timeout: 3000,
                        });
                    }

                }
                //$timeout(function () {

                //    file.result = response.data;
                //    //deleteFile();
                //    $scope.document = null;
                //    //$scope.loadAllUploadedFiles();
                //});
            }, function (response) {
                if (response.status > 0)
                    $scope.errorMsg = response.status + ': ' + response.data;
            }, function (evt) {
                // Math.min is to fix IE which reports 200% sometimes
                file.progress = Math.min(100, parseInt(100.0 * evt.loaded / evt.total));
            });
        }

        vm.getAddressInfoByZipCustomer = function (zip) {

            if (zip.length >= 5 && typeof google != 'undefined') {
                var addr = {};
                var geocoder = new google.maps.Geocoder();
                geocoder.geocode({ 'address': zip }, function (results, status) {
                    if (status == google.maps.GeocoderStatus.OK) {
                        if (results.length >= 1) {
                            var street_number = '';
                            var route = '';
                            var street = '';
                            var city = '';
                            var state = '';
                            var zipcode = '';
                            var country = '';
                            var formatted_address = '';

                            for (var ii = 0; ii < results[0].address_components.length; ii++) {

                                var types = results[0].address_components[ii].types.join(",");
                                if (types == "street_number") {
                                    addr.street_number = results[0].address_components[ii].long_name;
                                }
                                if (types == "route" || types == "point_of_interest,establishment") {
                                    addr.route = results[0].address_components[ii].long_name;
                                }
                                if (types == "sublocality,political" || types == "locality,political" || types == "neighborhood,political" || types == "administrative_area_level_3,political") {
                                    addr.city = (city == '' || types == "locality,political") ? results[0].address_components[ii].long_name : city;
                                }
                                if (types == "administrative_area_level_1,political") {
                                    addr.state = results[0].address_components[ii].short_name;
                                }
                                if (types == "postal_code" || types == "postal_code_prefix,postal_code") {
                                    addr.zipcode = results[0].address_components[ii].long_name;
                                }
                                if (types == "country,political") {
                                    addr.country = results[0].address_components[ii].short_name;
                                }
                            }
                            addr.success = true;
                            //assign retrieved address details
                            $scope.$apply(function () {
                                vm.model.customerDetails.customerAddress.city = addr.city;
                                vm.model.customerDetails.customerAddress.state = addr.state;
                                //vm.model.customerDetails.customerAddress.country = addr.country;
                                vm.errorCodeCustomer = false;
                            });


                        } else {
                            $scope.$apply(function () {
                                vm.errorCodeCustomer = true;
                            });

                        }
                    } else {
                        $scope.$apply(function () {
                            vm.errorCodeCustomer = true;
                        });

                    }
                });
            } else {
                $scope.$apply(function () {
                    vm.errorCodeCustomer = true;
                });
            }
        }

        vm.getAddressInfoByZipBilling = function (zip) {

            if (zip.length >= 5 && typeof google != 'undefined') {
                var addr = {};
                var geocoder = new google.maps.Geocoder();
                geocoder.geocode({ 'address': zip }, function (results, status) {
                    if (status == google.maps.GeocoderStatus.OK) {
                        if (results.length >= 1) {
                            var street_number = '';
                            var route = '';
                            var street = '';
                            var city = '';
                            var state = '';
                            var zipcode = '';
                            var country = '';
                            var formatted_address = '';

                            for (var ii = 0; ii < results[0].address_components.length; ii++) {

                                var types = results[0].address_components[ii].types.join(",");
                                if (types == "street_number") {
                                    addr.street_number = results[0].address_components[ii].long_name;
                                }
                                if (types == "route" || types == "point_of_interest,establishment") {
                                    addr.route = results[0].address_components[ii].long_name;
                                }
                                if (types == "sublocality,political" || types == "locality,political" || types == "neighborhood,political" || types == "administrative_area_level_3,political") {
                                    addr.city = (city == '' || types == "locality,political") ? results[0].address_components[ii].long_name : city;
                                }
                                if (types == "administrative_area_level_1,political") {
                                    addr.state = results[0].address_components[ii].short_name;
                                }
                                if (types == "postal_code" || types == "postal_code_prefix,postal_code") {
                                    addr.zipcode = results[0].address_components[ii].long_name;
                                }
                                if (types == "country,political") {
                                    addr.country = results[0].address_components[ii].short_name;
                                }
                            }
                            addr.success = true;
                            //assign retrieved address details
                            $scope.$apply(function () {
                                vm.model.companyDetails.costCenter.billingAddress.city = addr.city;
                                vm.model.companyDetails.costCenter.billingAddress.state = addr.state;
                                vm.model.companyDetails.costCenter.billingAddress.country = addr.country;
                                vm.errorCodeBilling = false;
                            });




                        } else {
                            $scope.$apply(function () {
                                vm.errorCodeBilling = true;
                            });

                        }
                    } else {
                        $scope.$apply(function () {
                            vm.errorCodeBilling = true;
                        });

                    }
                });
            } else {
                $scope.$apply(function () {
                    vm.errorCodeBilling = true;
                });
            }
        }

        //get the address details via google API
        vm.getAddressInformationCustomer = function () {

            if (vm.model.customerDetails.customerAddress.zipCode == null || vm.model.customerDetails.customerAddress.zipCode == '') {
                vm.errorCode = true;
            } else {
                vm.getAddressInfoByZipCustomer(vm.model.customerDetails.customerAddress.zipCode);
            }


        }

        //get the address details via google API
        vm.getAddressInformationBilling = function () {

            if (vm.model.companyDetails.costCenter.billingAddress.zipCode == null || vm.model.companyDetails.costCenter.billingAddress.zipCode == '') {
                vm.errorCode = true;
            } else {
                vm.getAddressInfoByZipBilling(vm.model.companyDetails.costCenter.billingAddress.zipCode);
            }


        }

        vm.textPhoneCode = function () {
            

            var userDetails = {
                email: vm.model.customerDetails.email,
                mobileNumber: vm.model.customerDetails.mobileNumber,
                isViaProfileSettings: true
            };
            updateProfilefactory.SendOPTCodeForPhoneValidation(userDetails)
             .then(function (returnedResult) {
                 
                 if (returnedResult.status == 200) {
                     vm.showError = false;
                     vm.isSentSecurityCode = true;
                 }
             },
            function (error) {
                
                vm.showError = true;
                vm.errorMessage = $rootScope.translate(error.data.message);

                if (error.data == "" || error.data.message == "") {
                    vm.errorMessage = $rootScope.translate('Error occured while processing your request.');
                }
            });
        };


        vm.submitSecurityCode = function () {
            
            var userDetails = {
                email: vm.model.customerDetails.email,          
                mobileVerificationCode: vm.model.customerDetails.mobileVerificationCode,
                isViaProfileSettings: true
            };
            updateProfilefactory.VerifyPhoneCode(userDetails)
             .then(function (returnedResult) {
                 if (returnedResult.status == 200) {
                     
                     if (returnedResult.data) {
                         vm.isVerified = true;
                     }
                     else {
                         vm.showError = true;
                         vm.errorMessage = $rootScope.translate('Invalid security code.');
                     }
                 }
             },
            function (error) {
                vm.showError = true;
                vm.errorMessage = $rootScope.translate(error.data.message);

                if (error.data == "" || error.data.message == "") {
                    vm.errorMessage = $rootScope.translate('Error occured while processing your request.');
                }
            });
        };

        vm.isVerified = false;
        vm.isPhoneNumberVerified = function () {
            
            updateProfilefactory.isPhoneNumberVerified(vm.model.customerDetails.email)
             .then(function (returnedResult) {
                 if (returnedResult.status == 200) {
                     
                     if (returnedResult.data.result == 1) {
                         vm.isVerified = true;
                     }
                     else if (returnedResult.data.result == 0) {
                         vm.isVerified = false;
                     }                     
                 }
             },
            function (error) {
                vm.isDisabled = false;
            });
        };

       
        vm.changePhone = function () {
            
            vm.isVerified = (vm.originalPhone == vm.model.customerDetails.mobileNumber) && vm.originalVerifiedStatus;
        };


    }]);

})(angular.module('newApp'));

