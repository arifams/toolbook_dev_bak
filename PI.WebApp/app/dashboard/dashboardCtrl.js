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
       ['$scope', 'builderFactory','dashboardfactory',
           function ($scope, builderFactory, dashboardfactory) {
               var vm = this;
               vm.model = {};
               $scope.$on('$viewContentLoaded', function () {
                   builderFactory.loadLineChart();
                   builderFactory.loadDougnutChart1();
                   builderFactory.loadDougnutChart2();
                   builderFactory.loadDougnutChart3();
                   builderFactory.loadDougnutChart4();
                   builderFactory.loadMap();
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