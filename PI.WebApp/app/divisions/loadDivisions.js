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
    app.factory('loadCostCenters', function ($http, $window) {
        return {
            loadCostCenters: function (userId,searchText,costCenter,type) {
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

    app.controller('loadDivisionsCtrl', function ($scope, $location, loadCostCenters, divisionManagmentService, $routeParams, $log, $window) {

        var userId = $window.localStorage.getItem('userGuid');
        var costCenter = $scope.costcenter == undefined ? 0 : $scope.costcenter;
        var type = $scope.status == undefined ? 0 : $scope.status;
        // Passed filterby and status as 0. Need to change this

        loadCostCenters.loadCostCenters(userId, $scope.searchText, costCenter, type)
        .then(function successCallback(responce) {
                  
            $scope.rowCollection = responce.data.content;

            //scope.rowCollection = [
            //        { firstName: 'Laurent', lastName: 'Renard', birthDate: new Date('1987-05-21'), balance: 102, email: 'whatever@gmail.com' },
            //        { firstName: 'Blandine', lastName: 'Faivre', birthDate: new Date('1987-04-25'), balance: -2323.22, email: 'oufblandou@gmail.com' },
            //        { firstName: 'Francoise', lastName: 'Frere', birthDate: new Date('1955-08-27'), balance: 42343, email: 'raymondef@gmail.com' }
            //];

            debugger;
        }, function errorCallback(response) {
            //todo
        });
   

    //$scope.gridOptions = {
    //    data: 'data.divisions.content',
    //    showFilter: false,
    //    multiSelect: false,
    //    selectedItems: $scope.data.selected,
    //    enablePaging: true,
    //    showFooter: true,
    //    totalServerItems: 'data.divisions.totalRecords',
    //    pagingOptions: $scope.data.pagingOptions,
    //    filterOptions: $scope.data.filterOptions,
    //    useExternalSorting: true,
    //    sortInfo: $scope.data.sortOptions,
    //    plugins: [new ngGridFlexibleHeightPlugin()],
    //    columnDefs: [
    //                { field: 'id', displayName: 'DivisionId', visible: false },
    //                { field: 'name', displayName: 'Division' },
    //                { field: 'assignedDivisionsForGrid', displayName: 'Assigned Cost Centers', sortable: false },
    //                { field: 'numberOfUsers', displayName: 'Number Users', sortable: false },
    //                { field: 'status', displayName: 'Status' },
    //                {
    //                    field: 'editLink', displayName: 'Edit', enableCellEdit: false,
    //                    cellTemplate: '<a href="#/saveDivision/{{row.entity.id}}" class="edit btn btn-sm btn-default" href="javascript:;"><i class="icon-note"></i></a>', sortable: false
    //                },
    //                {
    //                    field: 'deleteLink', displayName: 'Delete', enableCellEdit: false,
    //                    cellTemplate: '<a ng-click="deleteById(row)" class="delete btn btn-sm btn-danger" href="javascript:;"><i class="icons-office-52"></i></a>', sortable: false
    //                }
    //    ],
    //    afterSelectionChange: function (selection, event) {
    //        // $log.log("selection: " + selection.entity.CustomerID);
    //        // $location.path("comments/" + selection.entity.commentId);
    //    }
    //};

    //$scope.searchDivisions = function () {

    //    $scope.data.filterOptions.filterText = $scope.searchText;
    //    //loadDivisionService.find();
    //};

    //$scope.selectActiveDivisions = function () {

    //    if (angular.isUndefined($scope.status)) {

    //        $scope.data.searchTypes.type = '';
    //    }else {
    //        $scope.data.searchTypes.type = $scope.status;
    //    }       
    //    //loadDivisionService.find();
    //};

    //$scope.selectDivisionforCostCenter = function () {
    //    if (angular.isUndefined($scope.costcenter)) {

    //        $scope.data.searchTypes.costCenter = 0;
    //    } else {
                
    //        $scope.data.searchTypes.costCenter = $scope.costcenter;
    //    }
      
    //    //loadDivisionService.find();
    //}

    //$scope.deleteById = function (row) {
       
    //    var r = confirm("Do you want to delete the record?");
    //    if (r == true) {
    //        divisionManagmentService.deleteDivision({ Id: row.entity.id })
    //                    .success(function (response) {
    //                        debugger;
    //                        if (response == 1) {
    //                            debugger;
    //                            // remove($scope.gridOptions.data, 'id', row.entity.id);

    //                            angular.forEach($scope.data.divisions.content, function (index, result) {
    //                                if (index.id == row.entity.id) {
    //                                    $scope.data.divisions.content.splice(index, 1);
    //                                }
    //                            })

    //                        }
    //                    })
    //                   .error(function () {
    //                   })
    //    }
    //};


});

})(angular.module('newApp'));
