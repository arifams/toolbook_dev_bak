'use strict';


(function (app) {

    app.factory('costCenterManagmentService', function ($http) {
        return {
            saveCostCenter: function (costCenterDetail) {
                return $http.post(serverBaseUrl + '/api/Company/SaveCostCenter', costCenterDetail);
            }
        };
    })

    app.controller('saveCostCenterCtrl',
       ['costCenterManagmentService', function (costCenterManagmentService) {
           var vm = this;

           vm.saveCostCenter = function () {
               debugger;

               costCenterManagmentService.saveCostCenter(vm.model)
                 .success(function (result) {
                 })
               .error(function () {
               })
           }
       }]);


})(angular.module('newApp'));