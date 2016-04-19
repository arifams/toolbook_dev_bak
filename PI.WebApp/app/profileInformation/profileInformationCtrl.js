'use strict';


(function (app) {

    app.factory('updateProfilefactory', function ($http) {
        return {
            updateProfileInfo: function (updatedProfile) {
                return $http.post(serverBaseUrl + '/api/profile/UpdateProfile', updatedProfile);
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
                    var noMatch = viewValue != scope.formUpdateProfile.newpassword.$viewValue;
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
                    if (scope.formUpdateProfile.newpassword_c.$viewValue != '') {
                        var noMatch = viewValue != scope.formUpdateProfile.newpassword_c.$viewValue;
                        scope.formUpdateProfile.newpassword_c.$setValidity('noMatch', !noMatch);
                    }

                    return viewValue;
                })
            }
        }
    });

    app.controller('profileInformationCtrl',
        ['loadProfilefactory', 'updateProfilefactory', 'getAllAccountSettings', 'getCustomerAddressDetails', '$window',
            function (loadProfilefactory, updateProfilefactory, getAllAccountSettings, getCustomerAddressDetails, $window) {

                // return if user not logged. -- Need to move this to global service.
                if ($window.localStorage.getItem('userGuid') == '' || $window.localStorage.getItem('userGuid') == undefined) {
                    window.location = webBaseUrl + "/app/userLogin/userLogin.html";
                    return;
                }

                var vm = this;
                //vm.t_currencies;
                //vm.t_languages;
                vm.t_timezones;
                vm.model = {};
                vm.model.accountSettings = {};
                vm.model.doNotUpdateAccountSettings = false;

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

                //vm.changeCountry();

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

                vm.loadProfile = function () {

                    loadProfilefactory.loadProfileinfo()
                    .success(function (response) {

                        if (response != null) {
                            vm.model = response;
                            debugger;

                            //if (response.defaultCurrencyId != 0)
                            //    vm.defaultCurrency = { id: response.defaultCurrencyId };
                            //if (response.defaultLanguageId != 0)
                            //    vm.defaultLanguage = { id: response.defaultLanguageId };
                            //if (response.defaultTimeZoneId != 0)
                            //    vm.defaultTimezone = { id: response.defaultTimeZoneId };

                            if (response.customerDetails != null) {
                                debugger;
                                //setting the account type
                                // vm.iscorporate = response.data.customerDetails.isCorporateAccount;                            
                                vm.model.customerDetails = response.customerDetails;
                                //vm.model.customerDetails.customerAddress = response.customerDetails.customerAddress;
                                vm.model.companyDetails = response.companyDetails;
                                //vm.model.companyDetails.costCenter = response.companyDetails.costCenter;
                                //vm.changeCountry();

                                //if (response.companyDetails.costCenter != null) {
                                //    vm.multipleCostCenters = false;
                                //}
                                //else {
                                //    vm.multipleCostCenters = true;
                                //}


                                //if (response.companyDetails.costCenter != null &&
                                //    response.companyDetails.costCenter.billingAddress != null) {

                                //    vm.model.companyDetails.costCenter.billingAddress = response.companyDetails.costCenter.billingAddress;
                                //    vm.changeBillingCountry();
                                //}



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
                    debugger;
                    vm.model.customerDetails.userId = $window.localStorage.getItem('userGuid');

                    if (vm.defaultLanguage != undefined) {
                        vm.model.defaultLanguageId = vm.defaultLanguage;
                        vm.model.defaultCurrencyId = vm.defaultCurrency;
                        vm.model.defaultTimeZoneId = vm.defaultTimezone;
                    }
                    else {
                        vm.model.doNotUpdateAccountSettings = true;
                    }

                    var body = $("html, body");

                    if ((vm.model.newPassword && vm.model.oldPassword) && vm.model.newPassword == vm.model.oldPassword && vm.model.changeLoginData == true) {

                        body.stop().animate({ scrollTop: 0 }, '500', 'swing', function () {
                        });

                        $('#panel-notif').noty({
                            text: '<div class="alert alert-warning media fade in"><p>New Password Cannot be same as old Password</p></div>',
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
                        text: '<div class="alert alert-success media fade in"><p>Are you want to update the Profile?</p></div>',
                        buttons: [
                                {
                                    addClass: 'btn btn-primary', text: 'Ok', onClick: function ($noty) {

                                        $noty.close();
                                        vm.model.customerDetails.templateLink = '<html><head>    <title></title></head><body>    <p><img alt="" src="http://www.parcelinternational.nl/assets/Uploads/_resampled/SetWidth495-id-parcel-big.jpg" style="width: 200px; height: 200px; float: right;" /></p><div>        <h4 style="text-align: justify;">&nbsp;</h4><div style="background:#eee;border:1px solid #ccc;padding:5px 10px;">            <span style="font-family:verdana,geneva,sans-serif;">                <span style="color:#0000CD;">                    <span style="font-size:28px;">Email Verification</span>                </span>            </span>        </div><p style="text-align: justify;">&nbsp;</p><h4 style="text-align: justify;">            &nbsp;        </h4><h4 style="text-align: justify;">            <span style="font-size:12px;">                <span style="font-family:verdana,geneva,sans-serif;">                    Dear <strong>Salutation FirstName LastName, </strong>                </span>            </span>        </h4><h4 style="text-align: justify;">            <br /><span style="font-size:12px;">                <span style="font-family:verdana,geneva,sans-serif;">                    <strong>Welcome to Parcel International, we are looking forward to supporting your shipping needs. &nbsp;&nbsp;</strong>                </span>            </span>        </h4><h4 style="text-align: justify;">            <span style="font-size:12px;">                <span style="font-family:verdana,geneva,sans-serif;">                    <strong>                        Your Username has updated. To activate your account, please click &nbsp;ActivationURL                    </strong>                </span>            </span>        </h4><h4 style="text-align: justify;">            <span style="font-size:12px;">                <span style="font-family:verdana,geneva,sans-serif;"><strong>IMPORTANT! This activation link is valid for 24 hours only. &nbsp;&nbsp;</strong></span>            </span>        </h4><h4 style="text-align: justify;">            <span style="font-size:12px;">                <span style="font-family:verdana,geneva,sans-serif;">                    <strong>                        Should you have any questions or concerns, please contact Parcel International helpdesk for support &nbsp;                    </strong>                </span>            </span>        </h4>        <h4 style="text-align: justify;">            <span style="font-size:12px;">                <span style="font-family:verdana,geneva,sans-serif;">                    <strong>                        *** This is an automatically generated email, please do not reply ***                    </strong>                </span>            </span>        </h4>        <h4 style="text-align: justify;">&nbsp;</h4><h4 style="text-align: justify;">            <strong>                <span style="font-size:12px;">                    <span style="font-family:verdana,geneva,sans-serif;">Thank You, </span>                </span>            </strong>        </h4><h4 style="text-align: justify;">            <strong>                <span style="font-size:12px;">                    <span style="font-family:verdana,geneva,sans-serif;">Parcel International Team</span>                </span>            </strong>        </h4>    </div>   </body></html>'
                                        debugger;
                                        updateProfilefactory.updateProfileInfo(vm.model)
                                                        .success(function (responce) {
                                                            if (responce != null) {

                                                                if (responce == 1) {

                                                                    // var body = $("html, body");
                                                                    body.stop().animate({ scrollTop: 0 }, '500', 'swing', function () {
                                                                    });

                                                                    $('#panel-notif').noty({
                                                                        text: '<div class="alert alert-success media fade in"><p>Profile Updated Successfully</p></div>',
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
                                                                        text: '<div class="alert alert-success media fade in"><p>We have send the username change confirmation email. Please confirm before login</p></div>',
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
                                                                        text: '<div class="alert alert-warning media fade in"><p>Email You Entered is Already Exist</p></div>',
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
                                                                        text: '<div class="alert alert-warning media fade in"><p>Old password You Entered is Invalid</p></div>',
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
                                                                        text: '<div class="alert alert-warning media fade in"><p>Profile Update Failed</p></div>',
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
                                                                text: '<div class="alert alert-warning media fade in"><p>Server Error Occured</p></div>',
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
                                    addClass: 'btn btn-danger', text: 'Cancel', onClick: function ($noty) {
                                        debugger;
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
                    debugger;
                    getAllAccountSettings.getAllAccountSettings(vm.model.customerDetails.id)
                     .then(function successCallback(response) {

                         vm.languageList = response.data.accountSettings.languages;
                         vm.defaultLanguage = response.data.accountSettings.languages[0];

                         vm.currencyList = response.data.accountSettings.currencies;
                         vm.defaultCurrency = response.data.accountSettings.currencies[0];

                         vm.timezoneList = response.data.accountSettings.timeZones;
                         vm.defaultTimezone = response.data.accountSettings.timeZones[0];

                         if (response.data.defaultCurrencyId != 0)
                             vm.defaultCurrency = { id: response.defaultCurrencyId };
                         if (response.data.defaultLanguageId != 0)
                             vm.defaultLanguage = { id: response.defaultLanguageId };
                         if (response.data.defaultTimeZoneId != 0)
                             vm.defaultTimezone = { id: response.defaultTimeZoneId };

                     }, function errorCallback(response) {
                         //todo
                     });
                }


            }]);


})(angular.module('newApp'));

