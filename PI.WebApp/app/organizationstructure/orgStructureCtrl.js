'use strict';

(function (app) {

    app.factory('loadOrganizationStructureFactory', function ($http, $window) {
        return {
            loadOrganizationStructure: function () {
                return $http.get(serverBaseUrl + '/api/Company/GetOrganizationStructure', {
                    params: {
                        userId: $window.localStorage.getItem('userGuid')
                    }
                });
            }
        }

    });

    app.controller('orgStructureCtrl', ['$scope', '$compile', 'ngDialog', 'customBuilderFactory', '$controller','loadOrganizationStructureFactory',
        function ($scope, $compile, ngDialog, customBuilderFactory, $controller, loadOrganizationStructureFactory) {

       

            var datascource = {};

            $scope.loadOrganizationStructure = function () {
                loadOrganizationStructureFactory.loadOrganizationStructure()
                .then(function successCallback(responce) {

                    datascource = responce.data;
                    customBuilderFactory.orgStructurePopup(datascource);
                    //console.log(datascource);
                    // Compile
                    var e1 = angular.element(document.getElementById('chart-container'));
                    // Compile controller 2 html
                    var mController = angular.element(document.getElementById("chart-container"));
                    mController.scope().activateView(e1);

                }, function errorCallback(response) {
                    //todo
                });
            }

            $scope.editNode = function (type, id) {

                if (type == 'businessowner' || type == 'manager' || type == 'supervisor' || type == 'operator')
                    $scope.loadUserManagment(id);
                else if (type == 'division')
                    $scope.loadDivisionManagment(id);
                else if (type == 'costcenter')
                    $scope.loadCostcenterManagement(id);

            };


            $scope.loadUserManagment = function (userId, userType,parentType,parentId) {
                
                $scope.userId = userId;
                $scope.userType = userType;
                $scope.parentType = parentType;
                $scope.parentId = parentId;

                ngDialog.open({
                    scope: $scope,
                    template: '/app/userManagement/saveUserManagement.html',
                    //controller: $controller('saveUserManagementCtrl', {
                    //    $scope: $scope,
                    //    name: userId
                    //}),
                    className: 'ngdialog-theme-plain custom-width-max',
                    closeByDocument: false,
                    closeByEscape: false

                });
            }

            $scope.loadDivisionManagment = function (id, parentType, parentId) {
                
                $scope.divisionId = id;
                $scope.parentType = parentType;
                $scope.parentId = parentId;

                ngDialog.open({
                    scope: $scope,
                    template: '/app/divisions/saveDivision.html',
                    className: 'ngdialog-theme-plain custom-width',
                    closeByDocument: false,
                    closeByEscape: false
                });
            }

            $scope.loadCostcenterManagement = function (id, parentType, parentId) {
                
                $scope.costCenterId = id;
                $scope.parentType = parentType;
                $scope.parentId = parentId;

                ngDialog.open({
                    scope: $scope,
                    template: '/app/costcenter/saveCostCenter.html',
                    className: 'ngdialog-theme-plain custom-width-max',
                    closeByDocument: false,
                    closeByEscape: false
                });
            }

            $scope.activateView = function (ele) {
                $compile(ele.contents())($scope);
                $scope.$apply();
            };


            angular.element(document).ready(function () {

                //var e1 = angular.element(document.getElementById('chart-container'));
                //// Compile controller 2 html
                //var mController = angular.element(document.getElementById("chart-container"));
                //mController.scope().activateView(e1);

            });

            //var datascource = {
            //    'name': 'Business Manager',
            //    'title': 'Lao Lao1',
            //    'relationship': { 'children_num': 8 },
            //    'children': [
            //      {
            //          'name': 'Manager', 'title': 'Bo Miao',
            //          'children': [
            //            {
            //                'name': 'Supervisor', 'title': 'Tie Hua',
            //                'children': [
            //                  { 'name': 'Division', 'title': 'Division A' },
            //                  { 'name': 'Division', 'title': 'Division B' }
            //                ]
            //            }
            //          ]
            //      },
            //      {
            //          'name': 'Manager', 'title': 'Su Miao',
            //          'children': [
            //            {
            //                'name': 'Supervisor', 'title': 'Tie Hua B',
            //                'children': [
            //                  { 'name': 'Division', 'title': 'Division B' },
            //                  { 'name': 'Division', 'title': 'Division C' }
            //                ]    
            //            },
            //            { 'name': 'Supervisor', 'title': 'Tie Hua A' }
            //          ]
            //      }
            //    ]
            //};


            $scope.loadOrganizationStructure();

        }]);

})(angular.module('newApp'));