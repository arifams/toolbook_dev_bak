'use strict';


(function (app) {

    app.factory('costCenterSaveFactory', function ($http) {
        return {
            saveCostCenter: function (costCenterDetail) {
                return $http.post(serverBaseUrl + '/api/Company/SaveCostCenter', costCenterDetail);
            }
        };
    })

    app.factory('costCentrMngtFactory', function ($http, $routeParams, $window) {
        return {
            loadCostcenterInfo: function () {
                return $http.get(serverBaseUrl + '/api/Company/GetCostCentersById', {
                    params: {
                        id: 0,//$routeParams.id,
                        userId: $window.localStorage.getItem('userGuid')
                    }
                });
            }
        }
    })

    app.controller('saveCostCenterCtrl',
       ['costCentrMngtFactory', 'costCenterSaveFactory', '$location', '$window','$rootScope',
           function (costCentrMngtFactory, costCenterSaveFactory, $location, $window, $rootScope) {
               var vm = this;
               
               vm.saveCostCenter = function () {
                   vm.model.userId = $window.localStorage.getItem('userGuid')
                   var body = $("html, body");

                   costCenterSaveFactory.saveCostCenter(vm.model)
                   .success(function (result) {
                       
                       if (result == -1) {

                           body.stop().animate({ scrollTop: 0 }, '500', 'swing', function () {});

                           $('#panel-notif').noty({
                               text: '<div class="alert alert-warning media fade in"><p>' + $rootScope.translate('A cost center with the same name already exists') + '!</p></div>',
                               layout: 'bottom-right',
                               theme: 'made',
                               animation: {
                                   open: 'animated bounceInLeft',
                                   close: 'animated bounceOutLeft'
                               },
                               timeout: 3000,
                           });
                       } else {

                           body.stop().animate({ scrollTop: 0 }, '500', 'swing', function () { });
                           $('#panel-notif').noty({
                               text: '<div class="alert alert-success media fade in"><p>' + $rootScope.translate('Cost center saved successfully') + '!</p></div>',
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
                   $location.path('/loadCostcenters');
               }

               vm.changeCountry = function () {
                   if (vm.model.billingAddress == null) {
                       vm.model.billingAddress = {};
                   }
                   vm.isRequiredState = vm.model.billingAddress.country == 'US' ||
                                        vm.model.billingAddress.country == 'CA' ||
                                        vm.model.billingAddress.country == 'PR';


               };

               var loadCostcenter = function () {
                   costCentrMngtFactory.loadCostcenterInfo()
                   .success(function (data) {
                       
                       vm.model = data;
                       vm.model.assignedDivisionIdList = [];
                       if (vm.model.id == 0) {
                           vm.model.status = 1;
                           vm.model.billingAddress = {
                                       country: 'US'
                                   };
                           vm.isRequiredState = true;
                       }
                       else {
                           vm.changeCountry();
                           
                           //Add selected sites
                           angular.forEach(vm.model.allDivisions, function (availableDivision) {
                               if (availableDivision.isAssigned) {
                                   vm.model.assignedDivisionIdList.push(availableDivision.id);
                               }
                           })
                       }
                   })
                   .error(function () {
                       
                   })
               }

               loadCostcenter();



               vm.toggleDivisionSelection = function (division) {
                   var idx = vm.model.assignedDivisionIdList.indexOf(division.id);
                   // is currently selected
                   if (idx > -1) {
                       vm.model.assignedDivisionIdList.splice(idx, 1);
                   }
                       // is newly selected
                   else {
                       vm.model.assignedDivisionIdList.push(division.id);
                   }
               }

           }]);


})(angular.module('newApp'));