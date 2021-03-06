﻿'use strict';


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
            loadCostcenterInfo: function (costCenterId) {
                return $http.get(serverBaseUrl + '/api/Company/GetCostCentersById', {
                    params: {
                        id: costCenterId,//$routeParams.id,
                        userId: $window.localStorage.getItem('userGuid')
                    }
                });
            }
        }
    })

    app.controller('saveCostCenterCtrl',
       ['costCentrMngtFactory', 'costCenterSaveFactory', '$location', '$window', '$rootScope', '$scope',
           function (costCentrMngtFactory, costCenterSaveFactory, $location, $window, $rootScope, $scope) {
               var vm = this;

               vm.saveCostCenter = function () {

                   vm.model.userId = $window.localStorage.getItem('userGuid');
                   var body = $("html, body");

                   // Validate division has selected.
                   if (vm.model.assignedDivisionIdList.length == 0) {

                       body.stop().animate({ scrollTop: 0 }, '500', 'swing', function () { });

                       $('#panel-notif').noty({
                           text: '<div class="alert alert-warning media fade in"><p>' + $rootScope.translate('Division need to select') + '!</p></div>',
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

                   costCenterSaveFactory.saveCostCenter(vm.model)
                   .then(function (result) {

                       if (result.status == 200) {

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
                   }, function (error) {

                       var errorMessage = error.data.message;

                       if (error.data.message == undefined) {
                           errorMessage = 'Error occured while processing your request';
                       }
                       body.stop().animate({ scrollTop: 0 }, '500', 'swing', function () { });

                       $('#panel-notif').noty({
                           text: '<div class="alert alert-warning media fade in"><p>' + $rootScope.translate(errorMessage) + '!</p></div>',
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

               var loadCostcenter = function (costCenterId) {
                   costCentrMngtFactory.loadCostcenterInfo(costCenterId)
                   .success(function (data) {

                       vm.model = data;
                       vm.model.assignedDivisionIdList = [];
                       if (vm.model.id == 0) {
                           vm.model.status = 1;
                           vm.model.billingAddress = {
                               country: 'US'
                           };
                           vm.isRequiredState = true;

                           if ($scope.parentType == "Division") {

                               angular.forEach(vm.model.allDivisions, function (division) {

                                   if (division.id == $scope.parentId) {

                                       division.isAssigned = true;
                                       vm.model.assignedDivisionIdList.push(division.id);
                                   }
                               });
                           }

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

               loadCostcenter($scope.costCenterId);



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