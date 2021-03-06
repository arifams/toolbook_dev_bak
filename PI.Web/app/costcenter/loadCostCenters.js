﻿'use strict';

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
                        userId: $window.localStorage.getItem('userGuid') //$localStorage.userGuid
                    }
                });
            }
        }

    });

    app.factory('loadCostCenterService', function ($http, $q, $log, $rootScope) {

        var baseUrl = serverBaseUrl + '/api/Company/GetAllCostCentersByFliter';

        var service = {
            data: {
                currentCostCenter: {},
                costcenters: [],
                selected: [],
                totalPages: 0,

                filterOptions: {
                    filterText: '',
                    externalFilter: 'searchText',
                    useExternalFilter: true
                },
                sortOptions: {
                    fields: ["Id"],
                    directions: ["desc"]
                },
                pagingOptions: {
                    pageSizes: [10, 20, 50, 100],
                    pageSize: 10,
                    currentPage: 1
                },
                searchTypes: {
                    division: 0,
                    type: ''
                },
                user: {
                    userId: ''
                }
            },

            find: function () {
                var params = {
                    division: service.data.searchTypes.division,
                    type: service.data.searchTypes.type,
                    userId: service.data.user.userId,
                    searchtext: service.data.filterOptions.filterText,
                    page: service.data.pagingOptions.currentPage,
                    pageSize: service.data.pagingOptions.pageSize,
                    sortBy: service.data.sortOptions.fields[0],
                    sortDirection: service.data.sortOptions.directions[0]

                };

                var deferred = $q.defer();
                $http.get(baseUrl, { params: params })
                .success(function (data) {                 
                    service.data.costcenters = data;
                    deferred.resolve(data);
                }).error(function () {
                    deferred.reject();
                });
                return deferred.promise;
            }
        }

        service.find();
        return service;
    });


    app.controller('loadCostCentersCtrl', function ($scope, $location, loadAllDivisions, loadCostCenterService,
                                        costCenterManagmentService, $routeParams, $log, $window) {

        $scope.data = loadCostCenterService.data;
        $scope.data.user.userId = $window.localStorage.getItem('userGuid');

        loadAllDivisions.loadAllDivisions()
               .then(function successCallback(responce) {

                   $scope.divisionList = responce.data;

               }, function errorCallback(response) {
                   //todo
               });


        $scope.$watch('data.sortOptions', function (newVal, oldVal) {
            $log.log("sortOptions changed: " + newVal);
            if (newVal !== oldVal) {
                $scope.data.pagingOptions.currentPage = 1;
                loadCostCenterService.find();
            }
        }, true);

        //$scope.$watch('data.filterOptions', function (newVal, oldVal) {
        //    $log.log("filters changed: " + newVal);
        //    if (newVal !== oldVal) {
        //        $scope.data.pagingOptions.currentPage = 1;
        //        loadCostCenterService.find();
        //    }
        //}, true);

        $scope.$watch('data.pagingOptions', function (newVal, oldVal) {
            $log.log("page changed: " + newVal);
            if (newVal !== oldVal) {
                loadCostCenterService.find();
            }
        }, true);


        $scope.gridOptions = {
            data: 'data.costcenters.content',
            showFilter: false,
            multiSelect: false,
            selectedItems: $scope.data.selected,
            enablePaging: true,
            showFooter: true,
            totalServerItems: 'data.costcenters.totalRecords',
            pagingOptions: $scope.data.pagingOptions,
            filterOptions: $scope.data.filterOptions,
            useExternalSorting: true,
            sortInfo: $scope.data.sortOptions,
            plugins: [new ngGridFlexibleHeightPlugin()],
            columnDefs: [
                        { field: 'id', displayName: 'CostCenterId', visible: false },
                        { field: 'name', displayName: 'Cost Center' },
                        { field: 'assignedDivisions', displayName: 'Assigned Divisions', sortable: false },
                        { field: 'fullBillingAddress', displayName: 'Billing Address', sortable: false },
                        { field: 'status', displayName: 'Status' },
                        {
                            field: '', displayName: 'Edit', enableCellEdit: false,
                            cellTemplate: '<a href="#/saveCostcenter/{{row.entity.id}}" class="edit btn btn-sm btn-default" href="javascript:;"><i class="icon-note"></i></a>', sortable: false
                        },
                        {
                            field: '', displayName: 'Delete', enableCellEdit: false,
                            cellTemplate: '<a ng-click="deleteById(row)" class="delete btn btn-sm btn-danger" href="javascript:;"><i class="icons-office-52"></i></a>', sortable: false
                        }
            ],
            afterSelectionChange: function (selection, event) {
                // $log.log("selection: " + selection.entity.CustomerID);
                // $location.path("comments/" + selection.entity.commentId);
            }
        };

        $scope.searchCostCenters = function () {
            $scope.data.filterOptions.filterText = $scope.searchText;
            loadCostCenterService.find();
            //loadCostCenterService.find($scope.filterOptions.filterText);
        };

        $scope.selectActiveCostCenters = function () {

            if (angular.isUndefined($scope.status)) {

                $scope.data.searchTypes.type = '';
            } else {
                $scope.data.searchTypes.type = $scope.status;
            }
            loadCostCenterService.find();
        };


        $scope.selectDivisionforCostCenter = function () {
            debugger;
            if (angular.isUndefined($scope.selectedDivision)) {
                $scope.data.searchTypes.division = 0;
            } else {
                $scope.data.searchTypes.division = $scope.selectedDivision;
            }

            loadCostCenterService.find();
        }


        $scope.deleteById = function (row) {

            var r = confirm("Do you want to delete the record?");
            if (r == true) {
                costCenterManagmentService.deleteCostCenter({ Id: row.entity.id })
                            .success(function (response) {
                                debugger;
                                if (response == 1) {
                                    debugger;
                                    // remove($scope.gridOptions.data, 'id', row.entity.id);

                                    angular.forEach($scope.data.costcenters.content, function (index, result) {
                                        if (index.id  == row.entity.id) {
                                            $scope.data.costcenters.content.splice(index, 1);
                                        }
                                    })
                                }
                            })
                           .error(function () {
                           })
            }
        };

        // parse the gridData array to find the object with Id
        function remove(array, property, value) {
            angular.forEach(array, function (index, result) {
                if (result[property] == value) {
                    array.splice(index, 1);
                }
            });
        };

    });

})(angular.module('newApp'));
