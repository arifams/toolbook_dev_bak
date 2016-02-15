'use strict';


(function (app) {

    app.factory('divisionManagmentService', function ($http) {
        return {
            saveDivision: function (divisionDetail) {
                return $http.post(serverBaseUrl + '/api/Company/SaveDivision', divisionDetail);
            }
        };
    })
    app.controller('saveDivisionCtrl',
       ['divisionManagmentService', function (divisionManagmentService) {
           var vm = this;
           
           vm.saveDivision = function () {
               debugger;

               divisionManagmentService.saveDivision(vm.model)
                 .success(function (result) {
                 })
               .error(function () {
               })
           }
       }]);


})(angular.module('newApp'));