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
      
            //var datascource = {
            //    'id': '1',
            //    'name': 'Business Owner',
            //    'title': 'Business owner name',
            //    'type': 'user1',
            //    'children': [
            //      {
            //          'id': '2', 'name': 'Manager- Active', 'title': 'manager active name', 'type': 'user1', 'manager': [{ 'id': '8', 'name': 'Manager', 'title': 'manager name', 'type': 'user1' }],
            //          'children': [
            //          {
            //              'id': '3', 'name': 'Supervisor- Active', 'title': 'supervisor active name', 'type': 'user',
            //              'children': [
            //                  {
            //                      'id': '5', 'name': 'Division', 'title': 'division name', 'type': 'division1', 'costcenter': [{ 'id': '9', 'name': 'CostCenter', 'title': 'costcenter name A', 'type': 'costcenter1' }, { 'id': '10', 'name': 'Division B', 'title': 'costcenter name B', 'type': 'costcenter1' }],
            //                      'children': [
            //                          { 'id': '6', 'name': 'Operator- Active', 'title': 'operator active name', 'type': 'user' },
            //                          { 'id': '7', 'name': 'Operator-Inactive', 'title': 'operator inactive name', 'type': 'user' }
            //                      ]
            //                  }
            //              ]
            //          },
            //          {
            //              'id': '4', 'name': 'Division', 'title': 'division name', 'type': 'division', 'costcenter': [{ 'id': '11', 'name': 'CostCenter', 'title': 'costcenter name B', 'type': 'costcenter' }],
            //              'children': [
            //                    { 'id': '6', 'name': 'Operator-Inactive', 'title': 'operator inactive name', 'type': 'user' }
            //              ]
            //          }
            //          ]
            //      },

            //    ]
            //};

            var datascource = {};

            loadOrganizationStructureFactory.loadOrganizationStructure()
            .then(function successCallback(responce) {
                debugger;
                datascource = responce.data;
                customBuilderFactory.orgStructurePopup(datascource);

            }, function errorCallback(response) {
                //todo
            });

            $scope.editNode = function (type, id) {
                if (type == 'user')
                    loadUserManagment(id);
                else if (type == 'division')
                    loadDivisionManagment(id);
                else if (type == 'costcenter')
                    loadCostcenterManagement(id);

            };


            $scope.loadUserManagment = function (userId) {
                debugger;
            $scope.userId = userId;

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

            $scope.loadDivisionManagment = function (id) {
                debugger;
            ngDialog.open({
                scope: $scope,
                template: '/app/divisions/saveDivision.html',
                className: 'ngdialog-theme-plain custom-width',
                closeByDocument: false,
                closeByEscape: false
            });
        }

            $scope.loadCostcenterManagement = function (id) {
                debugger;
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

            var e1 = angular.element(document.getElementById('chart-container'));
            // Compile controller 2 html
            var mController = angular.element(document.getElementById("chart-container"));
            mController.scope().activateView(e1);

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


        

        customBuilderFactory.orgStructurePopup(datascource);

    }]);

})(angular.module('newApp'));