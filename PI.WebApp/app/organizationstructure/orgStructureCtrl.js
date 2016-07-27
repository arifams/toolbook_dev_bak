'use strict';

(function (app) {

    app.controller('orgStructureCtrl', ['$scope', '$compile', 'ngDialog', 'customBuilderFactory', '$controller',
        function ($scope, $compile, ngDialog, customBuilderFactory, $controller) {
      
        
            $scope.loadUserManagment = function (userId) {

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

        $scope.loadDivisionManagment = function () {
            ngDialog.open({
                scope: $scope,
                template: '/app/divisions/saveDivision.html',
                className: 'ngdialog-theme-plain custom-width',
                closeByDocument: false,
                closeByEscape: false
            });
        }

        $scope.loadCostcenterManagement = function () {
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


        var datascource = {
            'id': '1',
            'name': 'Business Owner',
            'title': 'Business owner name',
            'children': [
              {
                  'id': '2', 'name': 'Manager- Active', 'title': 'manager active name', 'manager': [{ 'id': '8', 'name': 'Manager', 'title': 'manager name' }],
                  'children': [
                  {
                      'id': '3', 'name': 'Supervisor- Active', 'title': 'supervisor active name',
                      'children': [
                          {
                              'id': '5', 'name': 'Division', 'title': 'division name', 'costcenter': [{ 'id': '8', 'name': 'CostCenter', 'title': 'costcenter name A' }, { 'id': '9', 'name': 'Division B', 'title': 'costcenter name B' }],
                              'children': [
                                  { 'id': '6', 'name': 'Operator- Active', 'title': 'operator active name' },
                                  { 'id': '7', 'name': 'Operator-Inactive', 'title': 'operator inactive name' }
                              ]
                          }
                      ]
                  },
                  {
                      'id': '4', 'name': 'Division', 'title': 'division name', 'costcenter': [{ 'id': '8', 'name': 'CostCenter', 'title': 'costcenter name B' }],
                      'children': [
                            { 'id': '5', 'name': 'Operator-Inactive', 'title': 'operator inactive name' }
                      ]
                  }
                  ]
              },

            ]
        };

        customBuilderFactory.orgStructurePopup(datascource);

    }]);

})(angular.module('newApp'));