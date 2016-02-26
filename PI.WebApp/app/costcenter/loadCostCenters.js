'use strict';

(function (app) {

    app.factory('costCenterManagmentService', function ($http) {
        return {
            deleteCostCenter: function (division) {
                return $http.post(serverBaseUrl + '/api/Company/DeleteCostCenter', division);
            }
        };
    });

    //get all divisions for the given user
    app.factory('loadAllDivisions', function ($http, $window) {
        return {
            loadAllDivisions: function () {
                return $http.get(serverBaseUrl + '/api/Company/GetAllDivisions', {
                    params: {
                        userId: $window.localStorage.getItem('userGuid')
                    }
                });
            }
        }

    });

    app.factory('loadCostCenterService', function ($http, $q, $log, $rootScope) {

        return {
            find: function (userId, searchText, costCenter, type) {
                return $http.get(serverBaseUrl + '/api/Company/GetAllCostCentersByFliter', {
                    params: {
                        userId: userId,
                        searchtext: searchText,
                        division: costCenter,
                        type: type
                    }
                });
            }
        }

    });


    app.controller('loadCostCentersCtrl', function ($scope, $location, loadAllDivisions, loadCostCenterService,
                                        costCenterManagmentService, $routeParams, $log, $window) {

        loadAllDivisions.loadAllDivisions()
            .then(function successCallback(responce) {

                $scope.divisionList = responce.data;

            }, function errorCallback(response) {
                //todo
            });

        $scope.itemsByPage = 25;
        $scope.rowCollection = [];
        // Add dumy record, since data loading is async.
        $scope.rowCollection.push(1);

        $scope.searchCostCenters = function () {
            
            // Get values from view.
            var userId = $window.localStorage.getItem('userGuid');
            var costCenter = ($scope.costcenter == undefined || $scope.costcenter == "") ? 0 : $scope.costcenter;
            var type = ($scope.status == undefined || $scope.status == "") ? 0 : $scope.status;
            var searchText = $scope.searchText;

            loadCostCenterService.find(userId, searchText, costCenter, type)
                .then(function successCallback(responce) {

                    $scope.rowCollection = responce.data.content;
                    debugger;
                }, function errorCallback(response) {
                    //todo
                });
        };

        // Call search function in page load.
        $scope.searchCostCenters();

        $scope.selectActiveCostCenters = function () {
            $scope.searchCostCenters();
        };

        $scope.selectDivisionforCostCenter = function () {
            $scope.searchCostCenters();
        }

        $scope.deleteById = function (row) {

            var r = confirm("Do you want to delete the record?");
            if (r == true) {
                costCenterManagmentService.deleteCostCenter({ Id: row.id })
                    .success(function (response) {
                        if (response == 1) {
                            var index = $scope.rowCollection.indexOf(row);
                            if (index !== -1) {
                                $scope.rowCollection.splice(index, 1);
                            }
                        }
                    })
                    .error(function () {
                    })
            }
        };

    });

})(angular.module('newApp'));
