
'use strict';


(function (app) {

    app.controller('companyListCtrl',
       ['$location', '$window', 'shipmentFactory', '$scope', 'searchList',
    function ($location, $window, shipmentFactory, $scope, searchList) {

        $scope.companyCollection = searchList;


        //set selected address details
        $scope.selectCompany = function (address) {
   

        }

    }]);


})(angular.module('newApp'));