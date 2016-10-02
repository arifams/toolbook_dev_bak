'use strict';

(function (app) {

    app.factory('userManagementFactory', ['$http', '$routeParams', '$window', function ($http, $routeParams, $window) {

        return {
            getAllDivisionsByCompany: getAllDivisionsByCompany,
            deleteUser: deleteUser,
            getAllRolesByUser: getAllRolesByUser,
            getUsersByFilter: getUsersByFilter,
            getUser: getUser,
            saveUser: saveUser,
            loadUserManagement: loadUserManagement
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

        function getUsersByFilter(userId, searchText, role, status) {
            debugger;
            return $http.get(serverBaseUrl + '/api/accounts/GetUsersByFilter', {
                params: {
                    loggedInuserId: userId,
                    searchtext: searchText,
                    role: role,
                    status: status
                }
            });
        }

        function getUser(userId) {
            
            var userId = (userId == 0) ? "" : userId;

            return $http.get(serverBaseUrl + '/api/accounts/GetUserByUserId', {
                params: {
                    userId: userId,
                    loggedInUser: $window.localStorage.getItem('userGuid')
                }
            });
        }

        function saveUser(user) {
            return $http.post(serverBaseUrl + '/api/accounts/SaveUser', user);
        }

        function loadUserManagement() {
            return $http.get(serverBaseUrl + '/api/accounts/LoadUserManagement', {
                params: {
                    loggedInUser: $window.localStorage.getItem('userGuid')
                }
            });
        }

    }]);

})(angular.module('newApp'));