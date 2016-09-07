
'use strict';


(function (app) {

    app.controller('customerDetailsCtrl',
       ['$location', '$window', 'adminFactory', '$scope', 'company',
    function ($location, $window, adminFactory, $scope, company) {
        
        $scope.company = company

    }]);


})(angular.module('newApp'));