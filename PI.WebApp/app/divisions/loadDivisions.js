'use strict';

(function (app) {

    app.factory('divisionManagmentService', function ($http) {
        return {
            deleteDivision: function (division) {
                return $http.post(serverBaseUrl + '/api/Company/DeleteDivision', division);
            }
        };
    });

    //get all costSenters for the given user
    app.factory('loadAllCostCenters', function ($http, $window) {
        return {
            loadCostCenters: function () {
                return $http.get(serverBaseUrl + '/api/Company/GetAllCostCenters', {
                    params: {
                        userId: $window.localStorage.getItem('userGuid')
                    }
                });
            }
        }

    });

    app.factory('loadDivisionService', function ($http, $q, $log, $rootScope) {

        var baseUrl = serverBaseUrl + '/api/Company/GetAllDivisionsByFliter';

        return {
            find: function (userId, searchText, costCenter, type) {
                return $http.get(serverBaseUrl + '/api/Company/GetAllDivisionsByFliter', {
                    params: {
                        userId: userId,
                        searchtext: searchText,
                        costCenter: costCenter,
                        type: type
                    }
                });
            }
        }
    });


    app.controller('loadDivisionsCtrl', function ($scope, $location, loadAllCostCenters, loadDivisionService, divisionManagmentService, $routeParams, $log, $window, $sce) {

        // Load all cost centers
        loadAllCostCenters.loadCostCenters()
            .then(function successCallback(responce) {

                $scope.costCenters = responce.data;

            }, function errorCallback(response) {
                //todo
            });

        $scope.itemsByPage = 25; // Set page size    // 25
        $scope.rowCollection = [];
        // Add dumy record, since data loading is async.
        // $scope.rowCollection.push(1);

        $scope.searchDivisions = function () {

            // Get values from view.
            var userId = $window.localStorage.getItem('userGuid');
            var costCenter = ($scope.costcenter == undefined || $scope.costcenter == "") ? 0 : $scope.costcenter;
            var type = ($scope.status == undefined || $scope.status == "") ? 0 : $scope.status;
            var searchText = $scope.searchText;

            loadDivisionService.find(userId, searchText, costCenter, type)
                .then(function successCallback(responce) {

                    $scope.rowCollection = responce.data.content;

                }, function errorCallback(response) {
                    //todo
                });
        };

        // Call search function in page load.
        $scope.searchDivisions();

        $scope.selectActiveDivisions = function () {

            $scope.searchDivisions();
        };

        $scope.selectDivisionforCostCenter = function () {
            $scope.searchDivisions();
        }

        // Delete row.
        $scope.deleteById = function (row) {

            var r = confirm("Do you want to delete the record?");
            if (r == true) {
                divisionManagmentService.deleteDivision({ Id: row.id })
                .then(function (response) {
                    if (response.status == 200) {
                        var index = $scope.rowCollection.indexOf(row);
                        if (index !== -1) {
                            $scope.rowCollection.splice(index, 1);
                        }
                    }
                },
                function (error) {

                    var errorMessage = error.data.message;

                    if (error.data.message == undefined) {
                        errorMessage = 'Error occured while processing your request';
                    }
                    body.stop().animate({ scrollTop: 0 }, '500', 'swing', function () { });
                    $('#panel-notif').noty({
                        text: '<div class="alert alert-warning media fade in"><p>' + $rootScope.translate(errorMessage) + '!</p></div>',
                        layout: 'bottom-right',
                        theme: 'made',
                        animation: {
                            open: 'animated bounceInLeft',
                            close: 'animated bounceOutLeft'
                        },
                        timeout: 3000,
                    });
                });
            };
        };

        $scope.renderHtml = function (html_code) {
            return $sce.trustAsHtml(html_code);
        };

    });

})(angular.module('newApp'));
