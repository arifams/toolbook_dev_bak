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
       ['$scope', 'customBuilderFactory', 'dashboardfactory', 'modalService', '$window',
           function ($scope, customBuilderFactory, dashboardfactory, modalService, $window) {
               var vm = this;
               vm.model = {};
               debugger;
               $scope.modalWizard = "accountsetupwizard/accountSetup.html";

               $scope.closePopup = function () {
                   modalService.close('editrole_popup');
                 
               };

               $scope.closePopupAfterSetupWizard = function () {
                   modalService.close('editrole_popup');
                   $window.location.reload();
               };

               $scope.templateModal = {
                   id: "editrole_popup",
                   header: "Edit Confirmation",
                   closeCaption: "No",
                   saveCaption: "Yes",
                   close: function () {
                       $scope.closePopup();
                   }
               };             
               angular.element(document).ready(function () {
                   modalService.load('editrole_popup');
               });
               
              
               $scope.$on('$viewContentLoaded', function () {
                   customBuilderFactory.loadMap();
                  /* builderFactory.loadLineChart();
                   builderFactory.loadDougnutChart1();
                   builderFactory.loadDougnutChart2();
                   builderFactory.loadDougnutChart3();
                   builderFactory.loadDougnutChart4();
                   builderFactory.loadMap();*/
              
               });
       
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