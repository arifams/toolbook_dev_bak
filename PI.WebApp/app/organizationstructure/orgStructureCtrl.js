'use strict';

(function (app) {

    app.controller('orgStructureCtrl', ['$scope', '$compile', 'ngDialog', 'customBuilderFactory', '$controller',
        function ($scope, $compile, ngDialog, customBuilderFactory, $controller) {
      
        
            $scope.loadUserManagment = function (userId) {

            $scope.userId = userId;

            ngDialog.open({
                scope: $scope,
                template: '/app/userManagement/saveUserManagement.html',
                //controller: $controller('saveUserManagementCtrl', {
                //    $scope: $scope,
                //    name: userId
                //}),
                className: 'ngdialog-theme-plain custom-width',
                closeByDocument: false,
                closeByEscape: false
           
            });
        }

        $scope.loadDivisionManagment = function () {
            ngDialog.open({
                scope: $scope,
                template: '/app/divisions/saveDivision.html',
                className: 'ngdialog-theme-plain custom-width',
                closeByDocument: false,
                closeByEscape: false
            });
        }

        $scope.loadCostcenterManagement = function () {
            ngDialog.open({
                scope: $scope,
                template: '/app/costcenter/saveCostCenter.html',
                className: 'ngdialog-theme-plain custom-width',
                closeByDocument: false,
                closeByEscape: false
            });
        }
        
            $scope.activateView = function (ele) {
                $compile(ele.contents())($scope);
                $scope.$apply();
            };


        angular.element(document).ready(function () {

            var e1 = angular.element(document.getElementById('chart-container'));
            // Compile controller 2 html
            var mController = angular.element(document.getElementById("chart-container"));
            mController.scope().activateView(e1);

        });

        customBuilderFactory.orgStructurePopup();

    }]);

})(angular.module('newApp'));