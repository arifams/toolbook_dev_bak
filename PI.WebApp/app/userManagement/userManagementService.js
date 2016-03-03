'use strict';

(function (app) {

    app.factory('userManagementFactory', ['$http', '$routeParams', '$window', function ($http, $routeParams, $window) {

        return {
            getAllDivisionsByCompany: getAllDivisionsByCompany,
            deleteUser: deleteUser,
            getAllRolesByUser: getAllRolesByUser,
            getUsersByFilter: getUsersByFilter,
            getUser: getUser,
            saveUser: saveUser            
        }

        // Implement the functions.

        function getAllDivisionsByCompany() {
            return $http.get(serverBaseUrl + '/api/Company/GetAllDivisions', {
                params: {
                    userId: $window.localStorage.getItem('userGuid')
                }
            });
        }

        function deleteUser(user) {
            return $http.post(serverBaseUrl + '/api/accounts/DeleteUser', user);
        }

        function getAllRolesByUser() {
            return $http.get(serverBaseUrl + '/api/accounts/GetAllRolesByUser', {
                params: {
                    userId: $window.localStorage.getItem('userGuid')
                }
            });
        }

        function getUsersByFilter(userId, searchText, division, role, status) {
            return $http.get(serverBaseUrl + '/api/accounts/GetUsersByFilter', {
                params: {
                    userId: userId,
                    searchtext: searchText,
                    division: division,
                    role: role,
                    status: status
                }
            });
        }

        function getUser() {
            return $http.get(serverBaseUrl + '/api/accounts/GetUserByUserId', {
                params: {
                    id: $routeParams.id,
                    userId: $window.localStorage.getItem('userGuid')
                }
            });
        }

        function saveUser(user) {
            return $http.post(serverBaseUrl + '/api/accounts/SaveUser', user);
        }

    }]);

})(angular.module('newApp'));