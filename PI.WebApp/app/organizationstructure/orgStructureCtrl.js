'use strict';

(function (app) {

    app.controller('orgStructureCtrl', ['$scope', '$compile', 'ngDialog', 'customBuilderFactory', function ($scope, $compile, ngDialog, customBuilderFactory) {
      
        $scope.loadUserManagment = function () {
            ngDialog.open({
                scope: $scope,
                template: '/app/userManagement/saveUserManagement.html',
                className: 'ngdialog-theme-plain custom-width',
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