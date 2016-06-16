
'use strict';


(function (app) {

    app.controller('customerDetailsCtrl',
       ['$location', '$window', 'adminFactory', '$scope', 'company',
    function ($location, $window, adminFactory, $scope, company) {
        debugger;
        $scope.company = company

    }]);


})(angular.module('newApp'));