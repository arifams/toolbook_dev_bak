'use strict';

(function (app) {

    app.factory('adminFactory', ['$http', '$routeParams', '$window', function ($http, $routeParams, $window) {

        return {
            getAllComapnies: getAllComapnies,
            changeCompanyStatus: changeCompanyStatus
        };


        function getAllComapnies(searchText, status) {
            
            return $http.get(serverBaseUrl + '/api/Company/GetAllComapnies', {
                params: {
                    status: status,
                    searchText: searchText,
                }
            });
        }



        function changeCompanyStatus(companyDetail) {
            
            return $http.post(serverBaseUrl + '/api/Company/ChangeCompanyStatus', companyDetail);
        }

    }]);

})(angular.module('newApp'));