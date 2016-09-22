'use strict';

(function (app) {
    app.controller('customerFinanceCtrl', ['$scope', 'customBuilderFactory', function ($scope, customBuilderFactory) {

        var vm = this;

        vm.loadFilterToggle = function () {
            customBuilderFactory.customFilterToggle();

        };


        $scope.currentPage = 1;
        $scope.numPerPage = 9;
        $scope.maxSize = 3;
        $scope.length = 20;


    }]);

})(angular.module('newApp'));
