'use strict';

(function (app) {

    app.factory('divisionManagmentService', function ($http) {
        return {
            deleteDivision: function (division) {
                return $http.post(serverBaseUrl + '/api/Company/DeleteDivision', division);
            }
        };
    })

    app.factory('loadDivisionService', function ($http, $q, $log, $rootScope) {
        debugger;
        var baseUrl = serverBaseUrl + '/api/Company/GetAllDivisionsByFliter';

        var service = {
            data: {
                currentDivision: {},
                divisions: [],
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
                }
            },

            find: function () {
                var params = {
                    searchtext: service.data.filterOptions.filterText,
                    page: service.data.pagingOptions.currentPage,
                    pageSize: service.data.pagingOptions.pageSize,
                    sortBy: service.data.sortOptions.fields[0],
                    sortDirection: service.data.sortOptions.directions[0]
                };

                var deferred = $q.defer();
                $http.get(baseUrl, { params: params })
                .success(function (data) {
                    service.data.divisions = data;
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


    app.controller('loadDivisionsCtrl', function ($scope, $location, loadDivisionService, divisionManagmentService, $routeParams, $log) {
    $scope.data = loadDivisionService.data;

    $scope.$watch('data.sortOptions', function (newVal, oldVal) {
        $log.log("sortOptions changed: " + newVal);
        if (newVal !== oldVal) {
            $scope.data.pagingOptions.currentPage = 1;
            loadDivisionService.find();
        }
    }, true);

    $scope.$watch('data.filterOptions', function (newVal, oldVal) {
        $log.log("filters changed: " + newVal);
        if (newVal !== oldVal) {
            $scope.data.pagingOptions.currentPage = 1;
            loadDivisionService.find();
        }
    }, true);

    $scope.$watch('data.pagingOptions', function (newVal, oldVal) {
        $log.log("page changed: " + newVal);
        if (newVal !== oldVal) {
            loadDivisionService.find();
        }
    }, true);

    $scope.gridOptions = {
        data: 'data.divisions.content',
        showFilter: false,
        multiSelect: false,
        selectedItems: $scope.data.selected,
        enablePaging: true,
        showFooter: true,
        totalServerItems: 'data.divisions.totalRecords',
        pagingOptions: $scope.data.pagingOptions,
        filterOptions: $scope.data.filterOptions,
        useExternalSorting: true,
        sortInfo: $scope.data.sortOptions,
        plugins: [new ngGridFlexibleHeightPlugin()],
        columnDefs: [
                    { field: 'name', displayName: 'Division Name' },
                    { field: 'id', displayName: 'Division Name' },
                    { field: 'status', displayName: 'Status' },
                    {
                        field: 'editLink', displayName: 'Edit', enableCellEdit: false,
                        cellTemplate: '<a href="#/saveDivision/{{row.entity.id}}" class="edit btn btn-sm btn-default" href="javascript:;"><i class="icon-note"></i></a>'
                    },
                    {
                        field: 'deleteLink', displayName: 'Delete', enableCellEdit: false,
                        cellTemplate: '<a ng-click="deleteById(row)" class="delete btn btn-sm btn-danger" href="javascript:;"><i class="icons-office-52"></i></a>'
                    }
        ],
        afterSelectionChange: function (selection, event) {
            // $log.log("selection: " + selection.entity.CustomerID);
            // $location.path("comments/" + selection.entity.commentId);
        }
    };

    $scope.deleteById = function (row) {
        divisionManagmentService.deleteDivision({ Id: row.entity.id })
            .success(function (response) {
                debugger;
                if (response == 1) {
                    debugger;
                    // remove($scope.gridOptions.data, 'id', row.entity.id);

                    angular.forEach($scope.data.divisions.content, function (index, result) {
                        if (result['id'] == row.entity.id) {
                            $scope.data.divisions.content.splice(index, 1);
                        }
                    })
                    
                }
            })
           .error(function () {
           })

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
