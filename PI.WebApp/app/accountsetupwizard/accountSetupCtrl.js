'use strict';

(function (app) {

    app.controller('accountSetupCtrl',
       ['$scope','updateProfilefactory','$window','loadProfilefactory','$rootScope','params',
    function ($scope, updateProfilefactory, $window, loadProfilefactory, $rootScope,params) {
        debugger;
        var vm = this;
        vm.model = {};
        vm.isSubmit1 = false;

        vm.model.customerDetails = {};
        vm.model.customerDetails.customerAddress = {};
        vm.model.customerDetails.customerAddress.country = 'US';
        vm.hidePanel = false;
        vm.errorCode = false;
        vm.hideaddressDetails = false;
        var generalDetailesCompleted = false;

        vm.model.customerDetails.isCorporateAccount = 'false';
        vm.model.customerDetails.userId = $window.localStorage.getItem('userGuid');
        vm.useCorpAddressAsBilling = false;
        vm.toCorporate = false;
        var saveAddressSuccessfully = false;

        vm.changeCountry = function () {
            vm.isRequiredState = vm.model.customerDetails.customerAddress.country == 'US' || vm.model.customerDetails.customerAddress.country == 'CA' || vm.model.customerDetails.customerAddress.country == 'PR' || vm.model.customerDetails.customerAddress.country == 'AU';
        };

        
            debugger;
            //load profile data
            var profileData = params.response;

            if (profileData != null) {

            debugger;
            vm.model = profileData;
            vm.loading = false;

            if (profileData.customerDetails != null) {
                //setting the account type                        
                vm.model.customerDetails = profileData.customerDetails;
                vm.model.companyDetails = profileData.companyDetails;


                if (profileData.customerDetails.salutation == null || profileData.customerDetails.salutation == '') {
                    vm.model.customerDetails.salutation='Mr'
                }

                if (profileData.customerDetails.isCorporateAccount == 'true') {
                    vm.model.customerDetails.isCorporateAccount = "true";
                }
                else {
                    vm.model.customerDetails.isCorporateAccount = "false";
                }

                //show hide the step wizard 
                if (params.level==1) {
                    vm.hideaddressDetails = false;
                }
                else if (params.level == 2) {
                    vm.hideaddressDetails = true;
                }

            }
            }
        
          

        vm.saveGeneralDetails = function () {
            debugger;
            if (vm.model.customerDetails.isCorporateAccount=='true') {
                vm.toCorporate = true;
                $window.localStorage.setItem('isCorporateAccount', true);

            }
            updateProfilefactory.updateProfileGeneral(vm.model)
                                       .success(function (responce) {
                                           if (responce != null) {

                                               vm.hideaddressDetails = true;
                                           }
                                       }).error(function (error) {
                                       });


        };

        vm.saveAddressDetails = function () {

            debugger;
            updateProfilefactory.updateProfileAddress(vm.model)
             .success(function (responce) {
                 if (responce != null){
                     saveAddressSuccessfully = true;
                     if (vm.useCorpAddressAsBilling == true) {

                         updateProfilefactory.UpdateSetupWizardBillingAddress(vm.model)
                                                        .success(function (responce) {
                                                            if (responce != null) {
                                                                $scope.$parent.$parent.userName = vm.model.customerDetails.firstName + ' ' + vm.model.customerDetails.lastName;
                                                                
                                                                if (vm.model.customerDetails.isCorporateAccount == 'true') {
                                                                    $scope.$parent.closePopupAfterSetupWizard();
                                                                } else {
                                                                    $scope.$parent.closePopup();
                                                                }
                                                                //  vm.hideaddressDetails = false;
                                                            }

                                                        }).error(function (error) {

                                                        });

                     } else {
                         debugger;
                         $scope.$parent.$parent.userName = vm.model.customerDetails.firstName + ' ' + vm.model.customerDetails.lastName;
                         if (vm.model.customerDetails.isCorporateAccount == 'true') {
                             $scope.$parent.closePopupAfterSetupWizard();
                         } else {
                             $scope.$parent.closePopup();
                         }
                         //vm.hideaddressDetails = false;
                     }



                 }

             }).error(function (error) {


             });



        };


        //previous button click
        vm.clickPrevious = function () {

            vm.hideaddressDetails = false;

        };

        vm.getAddressInfoByZip = function (zip) {

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
                            var city = addr.city;
                            $scope.$apply(function () {
                                vm.model.customerDetails.customerAddress.city = addr.city;
                                vm.model.customerDetails.customerAddress.state = addr.state;
                                vm.model.customerDetails.customerAddress.country = addr.country;
                                vm.errorCode = false;
                            });

                        } else {
                            $scope.$apply(function () {
                                vm.errorCode = true;
                            });

                        }
                    } else {
                        $scope.$apply(function () {
                                       vm.errorCode = true;
            });

                    }
                });
            } else {
                $scope.$apply(function () {
                    vm.errorCode = true;
                });
            }
        }

        vm.getAddressInformation = function () {

            if (vm.model.customerDetails.customerAddress.zipCode == null || vm.model.customerDetails.customerAddress.zipCode=='') {
                vm.errorCode = true;
            } else {
                vm.getAddressInfoByZip(vm.model.customerDetails.customerAddress.zipCode);
            }


        }






    }]);


})(angular.module('newApp'));