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
                        userId: $window.localStorage.getItem('userGuid') //$localStorage.userGuid
                    }
                });
            }
        }

    });

    app.factory('loadDivisionService', function ($http, $q, $log, $rootScope) {        
              
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
                },
                searchTypes: {
                    costCenter: 0,
                    type:''
                },
                user: {
                    userId:''
                }
            },

            find: function () {
                var params = {
                    costCenter: service.data.searchTypes.costCenter,
                    type: service.data.searchTypes.type,
                    userId:service.data.user.userId,
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


    app.controller('loadDivisionsCtrl', function ($scope, $location, loadAllCostCenters, loadDivisionService, divisionManagmentService, $routeParams, $log, $window) {
    $scope.data = loadDivisionService.data;
    $scope.data.user.userId = $window.localStorage.getItem('userGuid')
    

        loadAllCostCenters.loadCostCenters()
               .then(function successCallback(responce) {
                  
                   $scope.costCenters = responce.data;                   

               }, function errorCallback(response) {
                   //todo
               });


    $scope.$watch('data.sortOptions', function (newVal, oldVal) {
        $log.log("sortOptions changed: " + newVal);
        if (newVal !== oldVal) {
            $scope.data.pagingOptions.currentPage = 1;
            loadDivisionService.find();
        }
    }, true);

    //$scope.$watch('data.filterOptions', function (newVal, oldVal) {
    //    $log.log("filters changed: " + newVal);
    //    if (newVal !== oldVal) {
    //        $scope.data.pagingOptions.currentPage = 1;
    //        loadDivisionService.find();
    //    }
    //}, true);

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
                    { field: 'id', displayName: 'DivisionId', visible: false },
                    { field: 'name', displayName: 'Division' },
                    { field: '', displayName: 'Assigned Cost Center' },
                    { field: '', displayName: 'Number Users' },
                    { field: 'status', displayName: 'Status' },
                    {
                        field: 'editLink', displayName: 'Edit', enableCellEdit: false,
                        cellTemplate: '<a href="#/saveDivision/{{row.entity.id}}" class="edit btn btn-sm btn-default" href="javascript:;"><i class="icon-note"></i></a>', sortable: false
                    },
                    {
                        field: 'deleteLink', displayName: 'Delete', enableCellEdit: false,
                        cellTemplate: '<a ng-click="deleteById(row)" class="delete btn btn-sm btn-danger" href="javascript:;"><i class="icons-office-52"></i></a>', sortable: false
                    }
        ],
        afterSelectionChange: function (selection, event) {
            // $log.log("selection: " + selection.entity.CustomerID);
            // $location.path("comments/" + selection.entity.commentId);
        }
    };

    $scope.searchDivisions = function () {

        $scope.data.filterOptions.filterText = $scope.searchText;
        loadDivisionService.find();
        //loadDivisionService.find($scope.filterOptions.filterText);
    };

    $scope.selectActiveDivisions = function () {

        if (angular.isUndefined($scope.status)) {

            $scope.data.searchTypes.type = '';
        }else {
            $scope.data.searchTypes.type = $scope.status;
        }       
        loadDivisionService.find();
    };

    $scope.selectDivisionforCostCenter = function () {
        if (angular.isUndefined($scope.costcenter)) {

            $scope.data.searchTypes.costCenter = 0;
        } else {
                
            $scope.data.searchTypes.costCenter = $scope.costcenter;
        }
      
        loadDivisionService.find();
    }


    $scope.deleteById = function (row) {

        var r = confirm("Do you want to delete the record?");
        if (r == true) {
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
