'use strict';


(function (app) {

    app.factory('updateProfilefactory', function ($http) {
        return {
                updateProfileInfo: function (updatedProfile) {

                $http.post('api/profile/UpdateProfile', updatedProfile).then(function successCallback(response) {
                    if (response.data != null) {                        
                        return  response.data;
                    }

                }, function errorCallback(response) {
                    return null;
                })
            }
        }

    })

  
    app.factory('loadProfilefactory', function ($http) {
        return {           
            loadProfileinfo: function () {
                return $http.get('http://localhost:5555/api/profile/GetProfile', {
                    params: {
                        username: 'dilshan@amarasinghe'
                    }
                });
            }
        }

    });

    //loading language dropdown
    app.factory('loadAllLanguages', function ($http) {
        return {
            loadLanguages: function () {
                return $http.get('http://localhost:5555/api/profile/GetAllLanguages');
                
            }
        }

    });

    //loading language dropdown
    app.factory('loadAllCurrencies', function ($http) {
        return {
            loadCurrencies: function () {
                return $http.get('http://localhost:5555/api/profile/GetAllCurrencies');

            }
        }

    });

    //loading language dropdown
    app.factory('loadAllTimeZones', function ($http) {
        return {
            loadTimeZones: function () {
                return $http.get('http://localhost:5555/api/profile/GetAllTimezones');

            }
        }

    });

    //app.directive('validPasswordC', function () {
    //    return {
    //        require: 'ngModel',
    //        link: function (scope, elm, attrs, ctrl) {
    //            ctrl.$parsers.unshift(function (viewValue, $scope) {
    //                var noMatch = viewValue != scope.newUserRegisterForm.password.$viewValue
    //                ctrl.$setValidity('noMatch', !noMatch)
    //            })
    //        }
    //    }
    //})

    app.controller('profileInformationCtrl',
        ['loadProfilefactory', 'updateProfilefactory', 'loadAllLanguages', 'loadAllCurrencies', 'loadAllTimeZones', function (loadProfilefactory, updateProfilefactory,loadAllLanguages, loadAllCurrencies, loadAllTimeZones) {
            var vm = this;
            var t_currencies;
            var t_languages;
            var t_timezones;

            vm.loadProfile = function () {

                //loading languages to dropdown
                loadAllLanguages.loadLanguages()
                    .then(function successCallback(responce) {

                        t_languages = responce.data;  //[{ id: 5, languageCode: "ENG", languageName: "English" }];
                        //JSON.stringify(responce.data);
                       

                    }, function errorCallback(response) {
                       //todo
                    });
                
                //loading currencies to dropdown
                loadAllCurrencies.loadCurrencies()
                .then(function successCallback(responce) {

                    // vm.currencies = responce.data;
                    t_currencies = responce.data;
                }, function errorCallback(response) {
                    //todo
                });

                //loading timezones to dropdown
                loadAllTimeZones.loadTimeZones()
                .then(function successCallback(responce) {
                    t_timezones = responce.data;

                }, function errorCallback(response) {
                    //todo
                });
                

                 loadProfilefactory.loadProfileinfo().
                    then(function successCallback(response) {
                        if (response.data != null) {

                            //setting the account type
                            if (customerDetails.isCorporateAccount) {
                                vm.corporate = true;
                            }
                            else {
                                vm.individual = true;
                            }                          

                            vm.firstname = response.data.customerDetails.firstName;
                            vm.salutation = response.data.customerDetails.salutation;                           
                            vm.middlename = response.data.customerDetails.middleName;
                            vm.lastname = response.data.customerDetails.lastName;
                            vm.country = response.data.customerDetails.customerAddress.country;
                            vm.postalcode = response.data.customerDetails.customerAddress.zipCode;
                            vm.number = response.data.customerDetails.customerAddress.number;
                            vm.streetaddress1 = response.data.customerDetails.customerAddress.streetAddress1;
                            vm.streetaddress2 = response.data.customerDetails.customerAddress.streetAddress2;
                            vm.city = response.data.customerDetails.customerAddress.city;
                            vm.state = response.data.customerDetails.customerAddress.state;
                            vm.phonenumber = response.data.customerDetails.phoneNumber;
                            vm.mobilenumber = response.data.customerDetails.mobileNumber;
                            vm.emailaddress = response.data.customerDetails.email;
                            vm.cocnumber
                            vm.vatnumber
                            //return response.data;                             
                            
                        
                            vm.currencies = t_currencies
                            vm.languages = t_languages;
                            vm.timezones = t_timezones;
                            vm.defaultlanguage = response.data.defaultLanguageId;
                            vm.defaultcurrency = response.data.defaultCurrencyId;
                            vm.defaulttimezone = response.data.defaultTimeZoneId;

                            vm.booking_confirmation= response.data.bookingConfirmation;                           
                            vm.pickup_confirmation = response.data.pickupConfirmation;
                            vm.shipment_delays = response.data.shipmentDelay;
                            vm.shipment_exceptions = response.data.shipmentException;
                            vm.notenewsolution = response.data.notifyNewSolution;
                            vm.notediscount = response.data.notifyDiscountOffer;
                        }

                    }, function errorCallback(response) {
                        return null;
                    });
               
                
                
                  
            }

            vm.updateProfile = function () {

                var UpdatedProfile = {

                    BookingConfirmation: vm.booking_confirmation,
                    PickupConfirmation : vm.pickup_confirmation,
                    ShipmentDelay :vm.shipment_delays,
                    ShipmentException: vm.shipment_exceptions ,
                    NotifyNewSolution :vm.notenewsolution,
                    NotifyDiscountOffer:vm.notediscount ,                   
                  
                    DefaultLanguageId :vm.defaultlanguage,
                    DefaultCurrencyId :vm.defaultcurrency,
                    DefaultTimeZoneId :vm.defaulttimezone,                   
                
                    NewPassword : vm.newpassword,
                    OldPassword :vm.oldpassword,                   
                    
                    CustomerDetails: {
                        Salutation: vm.salutation,
                        FirstName: vm.firstname,
                        LastName: vm.lastname,
                        MiddleName: vm.middlename,
                        Email: vm.email,
                        PhoneNumber: vm.contact,                       
                        ConfirmPassword: vm.confirmpassword,
                        IsCorporateAccount: vm.iscorporate,
                        CompanyName: vm.companyname,
                        CustomerAddress:
                          {
                             Country: user.country,
                             ZipCode: user.postalcode,
                             StreetAddress1: user.street,
                             StreetAddress2: user.additionaldetails,
                             City: user.city,
                             State: user.state
                          },
                    }
                }

                updateProfilefactory.updateProfileInfo(UpdatedProfile)
                .then(function (result) {
                    if (result.data != null) {

                        //
                    }
                    
                }, function (error) {
                    console.log("failed");
                });
            }
        }]);


})(angular.module('newApp'));

