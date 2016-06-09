
'use strict';


(function (app) {

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