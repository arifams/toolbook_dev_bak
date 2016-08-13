'use strict';

(function (app) {

    app.factory('divisionManagmentFactory', function ($http) {
        return {
            saveDivision: function (divisionDetail) {
                return $http.post(serverBaseUrl + '/api/Company/SaveDivision', divisionDetail);
            }
        };
    })

    app.factory('divisionService', function ($http, $routeParams, $window) {

        return {
            loadDivisioninfo: function (divisionId) {
                debugger;
                return $http.get(serverBaseUrl + '/api/Company/GetDivisionById', {
                    params: {
                        id: divisionId, //$routeParams.id
                        userId: $window.localStorage.getItem('userGuid')
                    }
                });
            }
        }
    })

    app.controller('saveDivisionCtrl',
       ['divisionManagmentFactory', 'divisionService', '$location', '$window', '$rootScope', '$scope',
           function (divisionManagmentFactory, divisionService, $location, $window, $rootScope, $scope) {
               var vm = this;

               vm.saveDivision = function () {
                   debugger;
                   vm.model.userId = $window.localStorage.getItem('userGuid')

                   if ($scope.parentType = "Supervisor")
                   {
                       vm.model.assignedSupervisorId =  $scope.parentId;
                   }                   
                   divisionManagmentFactory.saveDivision(vm.model)
                    .then(function (result) {
                        debugger;
                        if (result.status == 200) {
                            $('#panel-notif').noty({
                                text: '<div class="alert alert-success media fade in"><p>' + $rootScope.translate('Division saved successfully!') + '</p></div>',
                                layout: 'bottom-right',
                                theme: 'made',
                                animation: {
                                    open: 'animated bounceInLeft',
                                    close: 'animated bounceOutLeft'
                                },
                                timeout: 3000,
                            });
                        }
                    },
                    function (error) {

                        var errorMessage = error.data.message;

                        if (error.data.message == "" || error.data.message == undefined) {
                            errorMessage = "Error occured while processing your request";
                        }
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
               }

               vm.close = function () {
                   $location.path('/loadDivisions');
               }

               var loadDivision = function (divisionId) {
                   divisionService.loadDivisioninfo(divisionId)
                    .success(function (data) {
                        debugger;
                        vm.model = data;

                        if (vm.model.id == 0) {
                            vm.model.status = 1;
                        }
                    })
                    .error(function () {
                    })
               }
               
               loadDivision($scope.divisionId);
           }]);


})(angular.module('newApp'));