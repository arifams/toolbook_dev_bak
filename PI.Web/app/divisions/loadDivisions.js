app.factory('CustomerService', function ($http, $q, $log, $rootScope) {

    var baseUrl = '/api/customer';

    var service = {
        data: {
            currentcustomer: {},
            customers: [],
            selected: [],
            totalPages: 0,

            filterOptions: {
                filterText: '',
                externalFilter: 'searchText',
                useExternalFilter: true
            },
            sortOptions: {
                fields: ["CustomerID"],
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
                service.data.customers = data;
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


app.controller('CustomerController', function ($scope, CustomerService, $routeParams, $log) {
    $scope.data = CustomerService.data;

    $scope.$watch('data.sortOptions', function (newVal, oldVal) {
        $log.log("sortOptions changed: " + newVal);
        if (newVal !== oldVal) {
            $scope.data.pagingOptions.currentPage = 1;
            CustomerService.find();
        }
    }, true);

    $scope.$watch('data.filterOptions', function (newVal, oldVal) {
        $log.log("filters changed: " + newVal);
        if (newVal !== oldVal) {
            $scope.data.pagingOptions.currentPage = 1;
            CustomerService.find();
        }
    }, true);

    $scope.$watch('data.pagingOptions', function (newVal, oldVal) {
        $log.log("page changed: " + newVal);
        if (newVal !== oldVal) {
            CustomerService.find();
        }
    }, true);

    $scope.gridOptions = {
        data: 'data.customers.content',
        showFilter: false,
        multiSelect: false,
        selectedItems: $scope.data.selected,
        enablePaging: true,
        showFooter: true,
        totalServerItems: 'data.customers.totalRecords',
        pagingOptions: $scope.data.pagingOptions,
        filterOptions: $scope.data.filterOptions,
        useExternalSorting: true,
        sortInfo: $scope.data.sortOptions,
        plugins: [new ngGridFlexibleHeightPlugin()],
        columnDefs: [
                    { field: 'customerID', displayName: 'ID' },
                    { field: 'contactName', displayName: 'Contact Name' },
                    { field: 'contactTitle', displayName: 'Contact Title' },
                    { field: 'address', displayName: 'Address' },
                    { field: 'city', displayName: 'City' },
                    { field: 'postalCode', displayName: 'Postal Code' },
                    { field: 'country', displayName: 'Country' }
        ],
        afterSelectionChange: function (selection, event) {
            $log.log("selection: " + selection.entity.CustomerID);
            // $location.path("comments/" + selection.entity.commentId);
        }
    };

});
