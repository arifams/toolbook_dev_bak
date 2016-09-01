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
       ['$scope', 'customBuilderFactory', 'dashboardfactory', 'modalService','$window','loadProfilefactory',
    function ($scope, customBuilderFactory, dashboardfactory, modalService, $window,loadProfilefactory) {
               var vm = this;
               vm.model = {};
               $scope.profile = {};
               debugger;
               $scope.modalWizard = "accountsetupwizard/accountSetup.html";

               $scope.closePopup = function () {
                   modalService.close('editrole_popup');
                 
               };

               $scope.closePopupAfterSetupWizard = function () {
                   modalService.close('editrole_popup');
                   $window.location.reload();
               };


               $scope.getProfile = function () {
                   return $scope.profile;
               };

               $scope.templateModal = {
                   id: "editrole_popup",
                   header: "Edit Confirmation",
                   closeCaption: "No",
                   saveCaption: "Yes",
                   close: function () {
                       $scope.closePopup();
                   },

               };

                         
               angular.element(document).ready(function () {


                   debugger;
                   loadProfilefactory.loadProfileinfo()
                  .success(function (response) {


                      $scope.profile = response;
                      debugger;                    

                      if (response.customerDetails != null) {

                          debugger;
                          if ((response.customerDetails.firstName == null || response.customerDetails.firstName == '') ||
                          (response.customerDetails.lastName == null || response.customerDetails.lastName == '') ||
                          (response.customerDetails.salutation == null || response.customerDetails.salutation == '') ||
                          (response.customerDetails.phoneNumber == null || response.customerDetails.phoneNumber == '') ||                         
                          (response.customerDetails.customerAddress.zipCode == null || response.customerDetails.customerAddress.zipCode == '') ||
                          (response.customerDetails.customerAddress.streetAddress1 == null || response.customerDetails.customerAddress.streetAddress1 == '') ||
                          (response.customerDetails.customerAddress.number == null || response.customerDetails.customerAddress.number == '') ||
                          (response.customerDetails.customerAddress.city == null || response.customerDetails.customerAddress.city == '') ||
                         (response.customerDetails.customerAddress.country == null || response.customerDetails.customerAddress.country == ''))                            
                          {
                              debugger;
                              modalService.load('editrole_popup');
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