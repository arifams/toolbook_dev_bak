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
            find: function (userId, searchText, division, type) {
                return $http.get(serverBaseUrl + '/api/Company/GetAllCostCentersByFliter', {
                    params: {
                        userId: userId,
                        searchtext: searchText,
                        division: division,
                        type: type
                    }
                });
            }
        }

    });


    app.controller('loadCostCentersCtrl', function ($scope, $location, loadAllDivisions, loadCostCenterService, $rootScope,
                                        costCenterManagmentService, $routeParams, $log, $window, $sce) {

        loadAllDivisions.loadAllDivisions()
            .then(function successCallback(responce) {

                $scope.divisionList = responce.data;

            }, function errorCallback(response) {
                //todo
            });

        $scope.itemsByPage = 25;
        $scope.rowCollection = [];
        // Add dumy record, since data loading is async.
        //$scope.rowCollection.push();

        $scope.searchCostCenters = function () {

            // Get values from view.
            var userId = $window.localStorage.getItem('userGuid');
            var division = ($scope.selectedDivision == undefined || $scope.selectedDivision == "") ? 0 : $scope.selectedDivision;
            var type = ($scope.status == undefined || $scope.status == "") ? 0 : $scope.status;
            var searchText = $scope.searchText;

            loadCostCenterService.find(userId, searchText, division, type)
                .then(function successCallback(responce) {

                    $scope.rowCollection = responce.data.content;

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

            var body = $("html, body");

            body.stop().animate({ scrollTop: 0 }, '500', 'swing', function () {
            });

            $('#panel-notif').noty({
                text: '<div class="alert alert-success media fade in"><p>' + $rootScope.translate('Do you want to delete the record') + '?</p></div>',
                buttons: [
                        {
                            addClass: 'btn btn-primary', text: $rootScope.translate('Ok'), onClick: function ($noty) {

                                $noty.close();

                                costCenterManagmentService.deleteCostCenter({ Id: row.id })
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

                                    body.stop().animate({ scrollTop: 0 }, '500', 'swing', function () {
                                    });

                                    $('#panel-notif').noty({
                                        text: '<div class="alert alert-warning media fade in"><p>' + $rootScope.translate(errorMessage) + '</p></div>',
                                        layout: 'bottom-right',
                                        theme: 'made',
                                        animation: {
                                            open: 'animated bounceInLeft',
                                            close: 'animated bounceOutLeft'
                                        },
                                        timeout: 3000,
                                    });
                            });
                            }
                        },
                        {
                            addClass: 'btn btn-danger', text: $rootScope.translate('Cancel'), onClick: function ($noty) {
                                $noty.close();
                                return;
                            }
                        }
                ],
                layout: 'bottom-right',
                theme: 'made',
                animation: {
                    open: 'animated bounceInLeft',
                    close: 'animated bounceOutLeft'
                },
                timeout: 3000,
            });

        };

        $scope.renderHtml = function (html_code) {
            return $sce.trustAsHtml(html_code);
        };

    });


})(angular.module('newApp'));
