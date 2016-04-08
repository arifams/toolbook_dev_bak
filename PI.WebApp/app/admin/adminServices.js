'use strict';

(function (app) {

    app.factory('adminFactory', ['$http', '$routeParams', '$window', function ($http, $routeParams, $window) {

        return {
            getAllComapnies: getAllComapnies,
            changeCompanyStatus: changeCompanyStatus
        };


        function getAllComapnies(searchText, status) {
            debugger;
            return $http.get(serverBaseUrl + '/api/Company/GetAllComapnies', {
                params: {
                    status: status,
                    searchText: searchText,
                }
            });
        }



        function changeCompanyStatus(companyDetail) {
            debugger;
            return $http.post(serverBaseUrl + '/api/Company/ChangeCompanyStatus', companyDetail);
        }

    }]);

})(angular.module('newApp'));