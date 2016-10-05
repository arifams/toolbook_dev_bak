'use strict';
(function (app) {

    app.controller('adminManageUsersCtrl', function (userManagementFactory, $scope, $location, $routeParams, $log, $window, $sce) {

        var vm = this;
        vm.status = 'All';
        vm.loadingSymbole = true;
        userManagementFactory.loadUserManagement()
            .then(function successCallback(response) {

                vm.divisionList = response.data.divisions;
                vm.roleList = response.data.roles;

            }, function errorCallback(response) {
                //todo
            });


        vm.itemsByPage = 25;
        vm.rowCollection = [];
        // Add dumy record, since data loading is async.
        // vm.rowCollection.push(1);

        vm.searchUsers = function () {
            debugger;
            
            // Get values from view.
            var loggedInuserId = $window.localStorage.getItem('userGuid');
            var role = (vm.role == undefined || vm.role == "") ? 0 : vm.role;
            var searchText = vm.searchText;
            var status = (vm.status == undefined || vm.status == "" || vm.status == "All") ? 0 : vm.status;
            
            userManagementFactory.getUsersByFilter(loggedInuserId, searchText, role, status)
                .then(function successCallback(responce) {
                    debugger;
                    vm.loadingSymbole = false;
                    vm.rowCollection = responce.data.content;

                }, function errorCallback(response) {
                    //todo
                    vm.loadingSymbole = false;
                });
        };

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

        vm.callServerSearch = function (tableState) {
            debugger;
            var pagination = 0;//tableState.pagination;

            var start = pagination.start || 0;     // This is NOT the page number, but the index of item in the list that you want to use to display the table.
            var number = pagination.number || 10;  // Number of entries showed per page.

            vm.searchUsers();
        };

        vm.resetSearch = function (tableState) {
            debugger;
            var pagination = 0;//tableState.pagination;

            var start = pagination.start || 0;     // This is NOT the page number, but the index of item in the list that you want to use to display the table.
            var number = pagination.number || 10;  // Number of entries showed per page.

            vm.userStatus = 'All';
            vm.datePicker.date = { "startDate": null, "endDate": null };
            //vm.datePicker.date.endDate = null;

            vm.searchUsers();

        }

    });

})(angular.module('newApp'));
