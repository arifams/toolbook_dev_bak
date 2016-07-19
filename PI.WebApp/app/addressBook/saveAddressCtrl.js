'use strict';
(function (app) {

    app.factory('saveAddressBookFactory', function ($http) {
        return {
            saveAddressBook: function (addressDetail) {
                return $http.post(serverBaseUrl + '/api/AddressBook/SaveAddress', addressDetail);
            }
        };
    })

    app.factory('loadAddressBookFactory', function ($http, $routeParams, $window, jwtHelper) {
        return {
            loadAddressInfo: function () {
                var token= $window.localStorage.getItem('token')
                
                var tokenPayload = jwtHelper.decodeToken(token);


                return $http.get(serverBaseUrl + '/api/AddressBook/LoadAddress', {
                    params: {
                        id: $routeParams.id,
                        userId: $window.localStorage.getItem('userGuid')
                    }
                });
            }
        }
    })

    app.controller('saveAddressCtrl', ['saveAddressBookFactory', 'loadAddressBookFactory', '$location', '$window', '$routeParams', '$rootScope', '$scope', function (saveAddressBookFactory, loadAddressBookFactory, $location, $window, $routeParams, $rootScope, $scope) {

        var vm = this;
       
        vm.errorCode = false;

        vm.changeCountry = function () {
            vm.isRequiredState = vm.model.country == 'US' || vm.model.country == 'CA' || vm.model.country == 'PR' || vm.model.country == 'AU';
        };      

        vm.saveAddressDetail = function () {
            vm.model.userId = $window.localStorage.getItem('userGuid');
            vm.model.id = $routeParams.id;
            var body = $("html, body");
            saveAddressBookFactory.saveAddressBook(vm.model)
           .success(function (result) {
               if (result == 1) {
                   body.stop().animate({ scrollTop: 0 }, '500', 'swing', function () {
                   });
                    $('#panel-notif').noty({
                        text: '<div class="alert alert-success media fade in"><p>' + $rootScope.translate('Address Detail saved successfully') + '</p></div>',
                        layout: 'bottom-right',
                        theme: 'made',
                        animation: {
                            open: 'animated bounceInLeft',
                            close: 'animated bounceOutLeft'
                        },
                        timeout: 3000,
                    });
                } 

                })
                .error(function () {
                })
        }
        vm.close = function () {
            $location.path('/loadAddresses');
        }

        var loadDivision = function () {
            loadAddressBookFactory.loadAddressInfo()
            .success(function (data) {
                
                vm.model = data;
                if (data.isActive == true || data.firstName==null) {
                    vm.model.isActive = "true";
                } else {
                    vm.model.isActive = "false";
                }

                //if (vm.model.id == 0) {
                //   // vm.model.status = 1;
                //    vm.model.isActive = "true";
                //}
            })
            .error(function () {
            })
        }

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
                                vm.model.city = addr.city;
                                vm.model.state = addr.state;
                                vm.model.country = addr.country;
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
            
            if (vm.model.zipCode == null || vm.model.zipCode =='') {
                vm.errorCode = true;
            } else {
                vm.getAddressInfoByZip(vm.model.zipCode);
                
            }


        }


        loadDivision();
       

    }]);

})(angular.module('newApp'));