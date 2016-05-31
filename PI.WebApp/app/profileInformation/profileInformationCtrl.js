'use strict';


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
            updateProfileLoginDetails: function (updatedProfile) {
                return $http.post(serverBaseUrl + '/api/profile/updateProfileLoginDetails', updatedProfile);
            },
            updateProfileAccountSettings: function (updatedProfile) {
                return $http.post(serverBaseUrl + '/api/profile/updateProfileAccountSettings', updatedProfile);
            },
            updateThemeColour: function (updatedProfile) {
                return $http.post(serverBaseUrl + '/api/profile/updateThemeColour', updatedProfile);
            }
        }

    });


    app.factory('loadProfilefactory', function ($http, $window) {
        return {
            loadProfileinfo: function () {
                return $http.get(serverBaseUrl + '/api/profile/GetProfile', {
                    params: {
                        userId: $window.localStorage.getItem('userGuid') //$localStorage.userGuid
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
                    var res = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d.*)(?=.*\W.*)[a-zA-Z0-9\S]{8,20}$/.test(viewValue);
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
        ['loadProfilefactory', 'updateProfilefactory', 'getAllAccountSettings', 'getCustomerAddressDetails', 'builderService', 'applicationService', '$window','$rootScope',
            function (loadProfilefactory, updateProfilefactory, getAllAccountSettings, getCustomerAddressDetails, builderService, applicationService, $window, $rootScope) {

                applicationService.init();
                
                //mainColor();

                // return if user not logged. -- Need to move this to global service.
                if ($window.localStorage.getItem('userGuid') == '' || $window.localStorage.getItem('userGuid') == undefined) {
                    window.location = webBaseUrl + "/app/userLogin/userLogin.html";
                    return;
                }

                var vm = this;

                vm.t_timezones;
                vm.model = {};
                vm.model.accountSettings = {};
                vm.model.doNotUpdateAccountSettings = false;
                vm.isImagetype = false;

                vm.themeColor;

                vm.isImage = function (ext) {
                    debugger;
                    if (ext == "image/jpg" || ext == "image/jpeg" || ext == "image/gif" || ext == "image/png") {

                        return vm.isImagetype = true;
                    } else {

                        vm.isImagetype = false;
                    }
                }

                vm.OnLogoChange = function (logo) {                   
                    if (logo.length>0) {
                        vm.isImage(logo[0].type);
                    }
                   
                }

               

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

                vm.changeCountry = function () {
                    vm.isRequiredState = vm.model.customerDetails.customerAddress.country == 'US' || vm.model.customerDetails.customerAddress.country == 'CA' || vm.model.customerDetails.customerAddress.country == 'PR' || vm.model.customerDetails.customerAddress.country == 'AU';
                };

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
                    builderService.init();
                };

                vm.loadProfile = function () {

                    loadProfilefactory.loadProfileinfo()
                    .success(function (response) {

                        if (response != null) {
                            vm.model = response;

                            if (response.customerDetails != null) {
                                //setting the account type                        
                                vm.model.customerDetails = response.customerDetails;
                                 vm.model.companyDetails = response.companyDetails;

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
                   })
                }


                vm.updateProfile = function () {

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

                    if ((vm.model.newPassword && vm.model.oldPassword) && vm.model.newPassword == vm.model.oldPassword && vm.model.changeLoginData == true) {

                        body.stop().animate({ scrollTop: 0 }, '500', 'swing', function () {
                        });

                        $('#panel-notif').noty({
                            text: '<div class="alert alert-warning media fade in"><p>' + $rootScope.translate('New Password Cannot be same as old Password') + '</p></div>',
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
                        text: '<div class="alert alert-success media fade in"><p>' + $rootScope.translate('Do you want to update the Profile') + '?</p></div>',
                        buttons: [
                                {
                                    addClass: 'btn btn-primary', text: $rootScope.translate('Ok'), onClick: function ($noty) {

                                        $noty.close();
                                        vm.model.customerDetails.templateLink = '<html><head>    <title></title></head><body>    <p><img alt="" src="http://www.parcelinternational.nl/assets/Uploads/_resampled/SetWidth495-id-parcel-big.jpg" style="width: 200px; height: 200px; float: right;" /></p><div>        <h4 style="text-align: justify;">&nbsp;</h4><div style="background:#eee;border:1px solid #ccc;padding:5px 10px;">            <span style="font-family:verdana,geneva,sans-serif;">                <span style="color:#0000CD;">                    <span style="font-size:28px;">Email Verification</span>                </span>            </span>        </div><p style="text-align: justify;">&nbsp;</p><h4 style="text-align: justify;">            &nbsp;        </h4><h4 style="text-align: justify;">            <span style="font-size:12px;">                <span style="font-family:verdana,geneva,sans-serif;">                    Dear <strong>Salutation FirstName LastName, </strong>                </span>            </span>        </h4><h4 style="text-align: justify;">            <br /><span style="font-size:12px;">                <span style="font-family:verdana,geneva,sans-serif;">                    <strong>Welcome to Parcel International, we are looking forward to supporting your shipping needs. &nbsp;&nbsp;</strong>                </span>            </span>        </h4><h4 style="text-align: justify;">            <span style="font-size:12px;">                <span style="font-family:verdana,geneva,sans-serif;">                    <strong>                        Your Username has updated. To activate your account, please click &nbsp;ActivationURL                    </strong>                </span>            </span>        </h4><h4 style="text-align: justify;">            <span style="font-size:12px;">                <span style="font-family:verdana,geneva,sans-serif;"><strong>IMPORTANT! This activation link is valid for 24 hours only. &nbsp;&nbsp;</strong></span>            </span>        </h4><h4 style="text-align: justify;">            <span style="font-size:12px;">                <span style="font-family:verdana,geneva,sans-serif;">                    <strong>                        Should you have any questions or concerns, please contact Parcel International helpdesk for support &nbsp;                    </strong>                </span>            </span>        </h4>        <h4 style="text-align: justify;">            <span style="font-size:12px;">                <span style="font-family:verdana,geneva,sans-serif;">                    <i>                        *** This is an automatically generated email, please do not reply ***                    </i>                </span>            </span>        </h4>        <h4 style="text-align: justify;">&nbsp;</h4><h4 style="text-align: justify;">            <strong>                <span style="font-size:12px;">                    <span style="font-family:verdana,geneva,sans-serif;">Thank You, </span>                </span>            </strong>        </h4><h4 style="text-align: justify;">            <strong>                <span style="font-size:12px;">                    <span style="font-family:verdana,geneva,sans-serif;">Parcel International Team<br/>Phone: +18589144414 <br/>Email: <a href="mailto:helpdesk@parcelinternational.com">helpdesk@parcelinternational.com</a><br/>Website: <a href="http://www.parcelinternational.com">http://www.parcelinternational.com</a></span>                </span>            </strong>        </h4>    </div>   </body></html>'
                                        
                                        updateProfilefactory.updateProfileInfo(vm.model)
                                                        .success(function (responce) {
                                                            if (responce != null) {

                                                                if (responce == 1) {

                                                                    // var body = $("html, body");
                                                                    body.stop().animate({ scrollTop: 0 }, '500', 'swing', function () {
                                                                    });

                                                                    $('#panel-notif').noty({
                                                                        text: '<div class="alert alert-success media fade in"><p>' + $rootScope.translate('Profile Updated Successfully') + '</p></div>',
                                                                        layout: 'bottom-right',
                                                                        theme: 'made',
                                                                        animation: {
                                                                            open: 'animated bounceInLeft',
                                                                            close: 'animated bounceOutLeft'
                                                                        },
                                                                        timeout: 3000,
                                                                    });

                                                                }
                                                                else if (responce == 3) {
                                                                    // vm.model.emailExist = "true";
                                                                    //  var body = $("html, body");
                                                                    body.stop().animate({ scrollTop: 0 }, '500', 'swing', function () {
                                                                    });

                                                                    $('#panel-notif').noty({
                                                                        text: '<div class="alert alert-success media fade in"><p>' + $rootScope.translate('We have send the username change confirmation email. Please confirm before login') + '</p></div>',
                                                                        layout: 'bottom-right',
                                                                        theme: 'made',
                                                                        animation: {
                                                                            open: 'animated bounceInLeft',
                                                                            close: 'animated bounceOutLeft'
                                                                        },
                                                                        timeout: 5000,
                                                                    });
                                                                }
                                                                else if (responce == -2) {
                                                                    // vm.model.emailExist = "true";
                                                                    //  var body = $("html, body");
                                                                    body.stop().animate({ scrollTop: 0 }, '500', 'swing', function () {
                                                                    });

                                                                    $('#panel-notif').noty({
                                                                        text: '<div class="alert alert-warning media fade in"><p>' + $rootScope.translate('Email You Entered Already Exists') + '</p></div>',
                                                                        layout: 'bottom-right',
                                                                        theme: 'made',
                                                                        animation: {
                                                                            open: 'animated bounceInLeft',
                                                                            close: 'animated bounceOutLeft'
                                                                        },
                                                                        timeout: 3000,
                                                                    });
                                                                }
                                                                else if (responce == -3) {
                                                                    //vm.model.oldPasswordWrong = "true";

                                                                    // var body = $("html, body");
                                                                    body.stop().animate({ scrollTop: 0 }, '500', 'swing', function () {
                                                                    });

                                                                    $('#panel-notif').noty({
                                                                        text: '<div class="alert alert-warning media fade in"><p>' + $rootScope.translate('Old password You Entered is Invalid') + '</p></div>',
                                                                        layout: 'bottom-right',
                                                                        theme: 'made',
                                                                        animation: {
                                                                            open: 'animated bounceInLeft',
                                                                            close: 'animated bounceOutLeft'
                                                                        },
                                                                        timeout: 3000,
                                                                    });
                                                                }
                                                                else {
                                                                    //vm.model.unsuccess = "true";

                                                                    //  var body = $("html, body");
                                                                    body.stop().animate({ scrollTop: 0 }, '500', 'swing', function () {
                                                                    });

                                                                    $('#panel-notif').noty({
                                                                        text: '<div class="alert alert-warning media fade in"><p>' + $rootScope.translate('Profile Update Failed') + '</p></div>',
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

                                                        }).error(function (error) {
                                                            // console.log("failed" + error);
                                                            // vm.model.isServerError = "true";

                                                            // var body = $("html, body");
                                                            body.stop().animate({ scrollTop: 0 }, '500', 'swing', function () {
                                                            });

                                                            $('#panel-notif').noty({
                                                                text: '<div class="alert alert-warning media fade in"><p>' + $rootScope.translate('Server Error Occured') + '</p></div>',
                                                                layout: 'bottom-right',
                                                                theme: 'made',
                                                                animation: {
                                                                    open: 'animated bounceInLeft',
                                                                    close: 'animated bounceOutLeft'
                                                                },
                                                                timeout: 3000,
                                                            });
                                                        });


                                    }
                                },
                                {
                                    addClass: 'btn btn-danger', text: $rootScope.translate('Cancel'), onClick: function ($noty) {
                                        
                                        // updateProfile = false;
                                        $noty.close();
                                        return;
                                        // noty({text: 'You clicked "Cancel" button', type: 'error'});
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


                vm.loadAddressInfo = function () {
                    getCustomerAddressDetails.getCustomerAddressDetails(vm.model.customerDetails.addressId, vm.model.companyDetails.id)
                     .then(function successCallback(response) {
                         
                         if (response.data.customerDetails != null) {
                             // vm.model.customerDetails = response.data.customerDetails;
                             vm.model.customerDetails.customerAddress = response.data.customerDetails.customerAddress;
                             // vm.model.companyDetails = response.data.companyDetails;
                             vm.model.customerDetails.customerAddress = response.data.customerDetails.customerAddress;
                             vm.model.companyDetails.costCenter = response.data.companyDetails.costCenter;
                             vm.changeCountry();

                             if (response.data.companyDetails.costCenter != null) {
                                 vm.multipleCostCenters = false;
                             }
                             else {
                                 vm.multipleCostCenters = true;
                             }


                             if (response.data.companyDetails.costCenter != null &&
                                 response.data.companyDetails.costCenter.billingAddress != null) {

                                 vm.model.companyDetails.costCenter.billingAddress = response.data.companyDetails.costCenter.billingAddress;
                                 vm.changeBillingCountry();
                             }
                         }

                     }, function errorCallback(response) {
                         //todo
                     });
                }

                vm.loadAccountSettings = function () {
                    
                    getAllAccountSettings.getAllAccountSettings(vm.model.customerDetails.id)
                     .then(function successCallback(response) {
                         
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
                         vm.model.pickupConfirmation= response.data.pickupConfirmation;
                         vm.model.shipmentDelay= response.data.shipmentDelay;
                         vm.model.shipmentException= response.data.shipmentException;
                         vm.model.notifyNewSolution= response.data.notifyNewSolution;
                         vm.model.notifyDiscountOffer = response.data.notifyDiscountOffer;

                     }, function errorCallback(response) {
                         //todo
                     });
                }


                // Split the User Profile update to seperate sections.

                vm.updateProfileGeneral = function () {

                    vm.model.customerDetails.userId = $window.localStorage.getItem('userGuid');

                    var body = $("html, body");

                    body.stop().animate({ scrollTop: 0 }, '500', 'swing', function () {
                    });

                    $('#panel-notif').noty({
                        text: '<div class="alert alert-success media fade in"><p>' + $rootScope.translate('Are you want to update the Profile') + '?</p></div>',
                        buttons: [
                                {
                                    addClass: 'btn btn-primary', text: $rootScope.translate('Ok'), onClick: function ($noty) {

                                        $noty.close();
                                        vm.model.customerDetails.templateLink = '<html><head>    <title></title></head><body>    <p><img alt="" src="http://www.parcelinternational.nl/assets/Uploads/_resampled/SetWidth495-id-parcel-big.jpg" style="width: 200px; height: 200px; float: right;" /></p><div>        <h4 style="text-align: justify;">&nbsp;</h4><div style="background:#eee;border:1px solid #ccc;padding:5px 10px;">            <span style="font-family:verdana,geneva,sans-serif;">                <span style="color:#0000CD;">                    <span style="font-size:28px;">Email Verification</span>                </span>            </span>        </div><p style="text-align: justify;">&nbsp;</p><h4 style="text-align: justify;">            &nbsp;        </h4><h4 style="text-align: justify;">            <span style="font-size:12px;">                <span style="font-family:verdana,geneva,sans-serif;">                    Dear <strong>Salutation FirstName LastName, </strong>                </span>            </span>        </h4><h4 style="text-align: justify;">            <br /><span style="font-size:12px;">                <span style="font-family:verdana,geneva,sans-serif;">                    <strong>Welcome to Parcel International, we are looking forward to supporting your shipping needs. &nbsp;&nbsp;</strong>                </span>            </span>        </h4><h4 style="text-align: justify;">            <span style="font-size:12px;">                <span style="font-family:verdana,geneva,sans-serif;">                    <strong>                        Your Username has updated. To activate your account, please click &nbsp;ActivationURL                    </strong>                </span>            </span>        </h4><h4 style="text-align: justify;">            <span style="font-size:12px;">                <span style="font-family:verdana,geneva,sans-serif;"><strong>IMPORTANT! This activation link is valid for 24 hours only. &nbsp;&nbsp;</strong></span>            </span>        </h4><h4 style="text-align: justify;">            <span style="font-size:12px;">                <span style="font-family:verdana,geneva,sans-serif;">                    <strong>                        Should you have any questions or concerns, please contact Parcel International helpdesk for support &nbsp;                    </strong>                </span>            </span>        </h4>        <h4 style="text-align: justify;">            <span style="font-size:12px;">                <span style="font-family:verdana,geneva,sans-serif;">                    <i>                        *** This is an automatically generated email, please do not reply ***                    </i>                </span>            </span>        </h4>        <h4 style="text-align: justify;">&nbsp;</h4><h4 style="text-align: justify;">            <strong>                <span style="font-size:12px;">                    <span style="font-family:verdana,geneva,sans-serif;">Thank You, </span>                </span>            </strong>        </h4><h4 style="text-align: justify;">            <strong>                <span style="font-size:12px;">                    <span style="font-family:verdana,geneva,sans-serif;">Parcel International Team<br/>Phone: +18589144414 <br/>Email: <a href="mailto:helpdesk@parcelinternational.com">helpdesk@parcelinternational.com</a><br/>Website: <a href="http://www.parcelinternational.com">http://www.parcelinternational.com</a></span>                </span>            </strong>        </h4>    </div>   </body></html>'

                                        updateProfilefactory.updateProfileGeneral(vm.model)
                                                .success(function (responce) {
                                                    if (responce != null) {
                                                        updateProfileResponse(responce);
                                                    }

                                                }).error(function (error) {

                                                    body.stop().animate({ scrollTop: 0 }, '500', 'swing', function () {
                                                    });

                                                    $('#panel-notif').noty({
                                                        text: '<div class="alert alert-warning media fade in"><p>' + $rootScope.translate('Server Error Occured') + '</p></div>',
                                                        layout: 'bottom-right',
                                                        theme: 'made',
                                                        animation: {
                                                            open: 'animated bounceInLeft',
                                                            close: 'animated bounceOutLeft'
                                                        },
                                                        timeout: 3000,
                                                    });
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
                        text: '<div class="alert alert-success media fade in"><p>' + $rootScope.translate('Are you want to update the Profile') + '?</p></div>',
                        buttons: [
                                {
                                    addClass: 'btn btn-primary', text: $rootScope.translate('Ok'), onClick: function ($noty) {

                                        $noty.close();
                                        
                                        updateProfilefactory.updateProfileAddress(vm.model)
                                                        .success(function (responce) {
                                                            if (responce != null) {
                                                                updateProfileResponse(responce);
                                                            }

                                                        }).error(function (error) {

                                                            body.stop().animate({ scrollTop: 0 }, '500', 'swing', function () {
                                                            });

                                                            $('#panel-notif').noty({
                                                                text: '<div class="alert alert-warning media fade in"><p>' + $rootScope.translate('Server Error Occured') + '</p></div>',
                                                                layout: 'bottom-right',
                                                                theme: 'made',
                                                                animation: {
                                                                    open: 'animated bounceInLeft',
                                                                    close: 'animated bounceOutLeft'
                                                                },
                                                                timeout: 3000,
                                                            });
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
                        text: '<div class="alert alert-success media fade in"><p>' + $rootScope.translate('Are you want to update the Profile') + '?</p></div>',
                        buttons: [
                                {
                                    addClass: 'btn btn-primary', text: $rootScope.translate('Ok'), onClick: function ($noty) {

                                        $noty.close();

                                        updateProfilefactory.updateProfileBillingAddress(vm.model)
                                                        .success(function (responce) {
                                                            if (responce != null) {
                                                                updateProfileResponse(responce);
                                                            }

                                                        }).error(function (error) {

                                                            body.stop().animate({ scrollTop: 0 }, '500', 'swing', function () {
                                                            });

                                                            $('#panel-notif').noty({
                                                                text: '<div class="alert alert-warning media fade in"><p>' + $rootScope.translate('Server Error Occured') + '</p></div>',
                                                                layout: 'bottom-right',
                                                                theme: 'made',
                                                                animation: {
                                                                    open: 'animated bounceInLeft',
                                                                    close: 'animated bounceOutLeft'
                                                                },
                                                                timeout: 3000,
                                                            });
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
                            text: '<div class="alert alert-warning media fade in"><p>' + $rootScope.translate('New Password Cannot be same as old Password') + '</p></div>',
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
                        text: '<div class="alert alert-success media fade in"><p>' + $rootScope.translate('Are you want to update the Profile') + '?</p></div>',
                        buttons: [
                                {
                                    addClass: 'btn btn-primary', text: $rootScope.translate('Ok'), onClick: function ($noty) {

                                        $noty.close();
                                        
                                        updateProfilefactory.updateProfileLoginDetails(vm.model)
                                                        .success(function (responce) {
                                                            if (responce != null) {
                                                                updateProfileResponse(responce);
                                                            }
                                                        }).error(function (error) {

                                                            body.stop().animate({ scrollTop: 0 }, '500', 'swing', function () {
                                                            });

                                                            $('#panel-notif').noty({
                                                                text: '<div class="alert alert-warning media fade in"><p>' + $rootScope.translate('Server Error Occured') + '</p></div>',
                                                                layout: 'bottom-right',
                                                                theme: 'made',
                                                                animation: {
                                                                    open: 'animated bounceInLeft',
                                                                    close: 'animated bounceOutLeft'
                                                                },
                                                                timeout: 3000,
                                                            });
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
                        text: '<div class="alert alert-success media fade in"><p>' + $rootScope.translate('Are you want to update the Profile') + '?</p></div>',
                        buttons: [
                                {
                                    addClass: 'btn btn-primary', text: $rootScope.translate('Ok'), onClick: function ($noty) {

                                        $noty.close();

                                        updateProfilefactory.updateProfileAccountSettings(vm.model)
                                                        .success(function (responce) {
                                                            if (responce != null) {
                                                                updateProfileResponse(responce);
                                                            }
                                                        }).error(function (error) {

                                                            body.stop().animate({ scrollTop: 0 }, '500', 'swing', function () {
                                                            });

                                                            $('#panel-notif').noty({
                                                                text: '<div class="alert alert-warning media fade in"><p>' + $rootScope.translate('Server Error Occured') + '</p></div>',
                                                                layout: 'bottom-right',
                                                                theme: 'made',
                                                                animation: {
                                                                    open: 'animated bounceInLeft',
                                                                    close: 'animated bounceOutLeft'
                                                                },
                                                                timeout: 3000,
                                                            });
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

                vm.updateThemeColour = function(){
                    debugger;
                    vm.model.customerDetails.userId = $window.localStorage.getItem('userGuid');

                    var body = $("html, body");

                    body.stop().animate({ scrollTop: 0 }, '500', 'swing', function () {
                    });

                    $('#panel-notif').noty({
                        text: '<div class="alert alert-success media fade in"><p>' + $rootScope.translate('Are you want to update the Profile') + '?</p></div>',
                        buttons: [
                                {
                                    addClass: 'btn btn-primary', text: $rootScope.translate('Ok'), onClick: function ($noty) {

                                        $noty.close();

                                        updateProfilefactory.updateThemeColour(vm.model)
                                                        .success(function (responce) {
                                                            if (responce != null) {
                                                                updateProfileResponse(responce);
                                                            }
                                                        }).error(function (error) {

                                                            body.stop().animate({ scrollTop: 0 }, '500', 'swing', function () {
                                                            });

                                                            $('#panel-notif').noty({
                                                                text: '<div class="alert alert-warning media fade in"><p>' + $rootScope.translate('Server Error Occured') + '</p></div>',
                                                                layout: 'bottom-right',
                                                                theme: 'made',
                                                                animation: {
                                                                    open: 'animated bounceInLeft',
                                                                    close: 'animated bounceOutLeft'
                                                                },
                                                                timeout: 3000,
                                                            });
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

                // Response of Update Profile.
                function updateProfileResponse(response) {

                    var body = $("html, body");

                    if (response == 1) {
                        body.stop().animate({ scrollTop: 0 }, '500', 'swing', function () {
                        });

                        $('#panel-notif').noty({
                            text: '<div class="alert alert-success media fade in"><p>' + $rootScope.translate('Profile Updated Successfully') + '</p></div>',
                            layout: 'bottom-right',
                            theme: 'made',
                            animation: {
                                open: 'animated bounceInLeft',
                                close: 'animated bounceOutLeft'
                            },
                            timeout: 3000,
                        });

                    }
                    else if (response == 3) {
                        body.stop().animate({ scrollTop: 0 }, '500', 'swing', function () {
                        });

                        $('#panel-notif').noty({
                            text: '<div class="alert alert-success media fade in"><p>' + $rootScope.translate('We have send the username change confirmation email. Please confirm before login') + '</p></div>',
                            layout: 'bottom-right',
                            theme: 'made',
                            animation: {
                                open: 'animated bounceInLeft',
                                close: 'animated bounceOutLeft'
                            },
                            timeout: 5000,
                        });
                    }
                    else if (response == -2) {
                        body.stop().animate({ scrollTop: 0 }, '500', 'swing', function () {
                        });

                        $('#panel-notif').noty({
                            text: '<div class="alert alert-warning media fade in"><p>' + $rootScope.translate('Email You Entered is Already Exists') + '</p></div>',
                            layout: 'bottom-right',
                            theme: 'made',
                            animation: {
                                open: 'animated bounceInLeft',
                                close: 'animated bounceOutLeft'
                            },
                            timeout: 3000,
                        });
                    }
                    else if (response == -3) {
                        body.stop().animate({ scrollTop: 0 }, '500', 'swing', function () {
                        });

                        $('#panel-notif').noty({
                            text: '<div class="alert alert-warning media fade in"><p>' + $rootScope.translate('Old password You Entered is Invalid') + '</p></div>',
                            layout: 'bottom-right',
                            theme: 'made',
                            animation: {
                                open: 'animated bounceInLeft',
                                close: 'animated bounceOutLeft'
                            },
                            timeout: 3000,
                        });
                    }
                    else {
                        body.stop().animate({ scrollTop: 0 }, '500', 'swing', function () {
                        });

                        $('#panel-notif').noty({
                            text: '<div class="alert alert-warning media fade in"><p>' + $rootScope.translate('Profile Update Failed') + '</p></div>',
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

                vm.uploadLogo = function (file) {

                    debugger;
                    file.upload = Upload.upload({
                        url: serverBaseUrl + '/api/Admin/UploadLogo',
                        data: {
                            file: file,
                            userId: $window.localStorage.getItem('userGuid'),
                            documentType: "LOGO"
                            
                        },
                    });

                    file.upload.then(function (response) {
                        debugger;
                        if (response.status==200) {
                            var body = $("html, body");
                            body.stop().animate({ scrollTop: 0 }, '500', 'swing', function () {
                            });

                            $('#panel-notif').noty({
                                text: '<div class="alert alert-success media fade in"><p>' + $rootScope.translate('Company Logo updated, click ok to reload the page') + '?</p></div>',
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

                          //  $window.location.reload();

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


            }]);

})(angular.module('newApp'));

