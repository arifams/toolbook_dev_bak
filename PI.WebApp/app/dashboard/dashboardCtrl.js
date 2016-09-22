'use strict';

(function (app) {

    app.factory('dashboardfactory', function ($http, $window) {
        return {
            getShipmentStatusCounts: function (status) {
                return $http.get(serverBaseUrl + '/api/shipments/GetShipmentStatusCounts', {
                    params: {
                        userId: $window.localStorage.getItem('userGuid')
                    }
                })
            }
        }
    });

    app.controller('dashboardCtrl',
       ['$scope', 'customBuilderFactory', 'dashboardfactory', 'modalService','$window','loadProfilefactory','$modal',
    function ($scope, customBuilderFactory, dashboardfactory, modalService, $window,loadProfilefactory,$modal) {
               var vm = this;
               vm.model = {};
               $scope.profile = {};
             
               $scope.closePopup = function () {
                   $scope.modalInstance.close();
                 
               };

               $scope.closePopupAfterSetupWizard = function () {
                   $scope.modalInstance.close();
                   $window.location.reload();
               };


               $scope.getProfile = function () {
                   return $scope.profile;
               };
          
               angular.element(document).ready(function () {

                   loadProfilefactory.loadProfileinfo()
                  .success(function (response) {

                      $scope.profile = response;                      

                      if (response.customerDetails != null) {

                          if ((response.customerDetails.firstName == null || response.customerDetails.firstName == '') ||
                          (response.customerDetails.lastName == null || response.customerDetails.lastName == '') ||
                          (response.customerDetails.salutation == null || response.customerDetails.salutation == '') ||
                          (response.customerDetails.phoneNumber == null || response.customerDetails.phoneNumber == '')) {

                              $scope.modalInstance = $modal.open({
                                  templateUrl: 'accountsetupwizard/accountSetup.html',
                                  animation: true,
                                  controller: 'accountSetupCtrl',
                                  controllerAs: 'vm',
                                  //size: '',
                                  backdrop: 'static',
                                  scope: $scope,
                                  resolve: {
                                      params: function () {
                                          return {
                                              level: 1,
                                              response: response
                                          };
                                      }
                                  }
                              });

                         }
                        else if ((response.customerDetails.customerAddress.zipCode == null || response.customerDetails.customerAddress.zipCode == '') ||
                          (response.customerDetails.customerAddress.streetAddress1 == null || response.customerDetails.customerAddress.streetAddress1 == '') ||
                          (response.customerDetails.customerAddress.number == null || response.customerDetails.customerAddress.number == '') ||
                          (response.customerDetails.customerAddress.city == null || response.customerDetails.customerAddress.city == '') ||
                         (response.customerDetails.customerAddress.country == null || response.customerDetails.customerAddress.country == ''))
                          {
                              $scope.modalInstance = $modal.open({
                                  templateUrl: 'accountsetupwizard/accountSetup.html',
                                  animation: true,
                                  controller: 'accountSetupCtrl',
                                  controllerAs: 'vm',
                                  //size: '',
                                  backdrop: 'static',
                                  scope: $scope,
                                  resolve: {
                                      params: function () {
                                          return {
                                              level: 2,
                                              response: response
                                          };
                                      }
                                  }
                              });
    
                          }                         
                    
                      }                     

                  })
                  .error(function () {

                      vm.model.isServerError = "true";
                      vm.loading = false;
                  })


                  
               });
               

               //$scope.$on('$viewContentLoaded', function () {
               //   /* builderFactory.loadLineChart();
               //    builderFactory.loadDougnutChart1();
               //    builderFactory.loadDougnutChart2();
               //    builderFactory.loadDougnutChart3();
               //    builderFactory.loadDougnutChart4();
               //    builderFactory.loadMap();*/
              
               //});
       
               vm.getShipmentStatusCounts = function () {

                   dashboardfactory.getShipmentStatusCounts()
                   .success(function (response) {

                       if (response != null) {
                           vm.model = response;
                       }
                   })
                  .error(function () {
                      vm.model.isServerError = "true";
                  })
               }

               vm.isViaDashboard = true;

               vm.getShipmentStatusCounts();

           }]);


})(angular.module('newApp'));