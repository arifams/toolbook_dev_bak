'use strict';

(function (app) {

    app.controller('loadUserManagementCtrl', function (userManagementFactory, $scope, $location, $routeParams, $log, $window, $sce) {

        var vm = this;

        userManagementFactory.loadUserManagement()
            .then(function successCallback(response) {
                debugger;
                vm.divisionList = response.data.divisions;
                //vm.roleList = response.data.roles;

            }, function errorCallback(response) {
                //todo
            });

        userManagementFactory.getAllDivisionsByCompany()
            .then(function successCallback(response) {

                vm.divisionList = response.data;

            }, function errorCallback(response) {
                //todo
            });

        //userManagementFactory.getAllRolesByUser()
        //            .then(function successCallback(response) {

        //                vm.roleList = response.data;

        //            }, function errorCallback(response) {
        //                //todo
        //            });

        vm.itemsByPage = 25;
        vm.rowCollection = [];
        // Add dumy record, since data loading is async.
        vm.rowCollection.push(1);

        vm.searchUsers = function () {

            // Get values from view.
            var userId = $window.localStorage.getItem('userGuid');
            var division = (vm.selectedDivision == undefined || vm.selectedDivision == "") ? 0 : vm.selectedDivision;
            var role = (vm.status == undefined || vm.status == "") ? 0 : vm.status;
            var searchText = vm.searchText;
            var status = vm.status;

            userManagementFactory.getUsersByFilter(userId, searchText, division, role, status)
                .then(function successCallback(responce) {

                    vm.rowCollection = responce.data.content;
                    debugger;
                }, function errorCallback(response) {
                    //todo
                });
        };

        // Call search function in page load.
        vm.searchUsers();

        vm.selectActiveUsers = function () {
            vm.searchUsers();
        };

        vm.selectDivision = function () {
            vm.searchUsers();
        }

        vm.selectRole = function () {
            vm.searchUsers();
        };

        vm.deleteById = function (row) {

            var r = confirm("Do you want to delete the record?");
            if (r == true) {
                userManagementFactory.deleteUser({ Id: row.id })
                    .success(function (response) {
                        if (response == 1) {
                            var index = vm.rowCollection.indexOf(row);
                            if (index !== -1) {
                                vm.rowCollection.splice(index, 1);
                            }
                        }
                    })
                    .error(function () {
                    })
            }
        };

        vm.renderHtml = function (html_code) {
            return $sce.trustAsHtml(html_code);
        };

    });

})(angular.module('newApp'));
