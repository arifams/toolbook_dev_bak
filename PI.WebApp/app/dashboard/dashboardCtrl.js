'use strict';

(function (app) {

    app.factory('dashboardfactory', function ($http) {
        return {
            getShipmentStatusCounts: function (status) {
                return $http.get(serverBaseUrl + '/api/shipments/GetShipmentStatusCounts', {
                    params: {
                        status: status,
                        userId: $window.localStorage.getItem('userGuid')
                    }
                })
            }
        });

        app.controller('dashboardCtrl',
           ['$scope', 'builderFactory',
               function ($scope, builderFactory) {



                   $scope.$on('$viewContentLoaded', function () {
                       builderFactory.loadLineChart();
                       builderFactory.loadDougnutChart1();
                       builderFactory.loadDougnutChart2();
                       builderFactory.loadDougnutChart3();
                       builderFactory.loadDougnutChart4();
                   });

                   this.isViaDashboard = true;


               }]);


    })(angular.module('newApp'));