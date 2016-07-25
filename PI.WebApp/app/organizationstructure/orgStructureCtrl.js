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
                className: 'ngdialog-theme-plain custom-width',
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
                className: 'ngdialog-theme-plain custom-width',
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
        //    'name': 'Lao Lao',
        //    'title': 'general manager',
        //    'relationship': { 'children_num': 8 },
        //    'children': [
        //      { 'name': 'Bo Miao', 'title': 'department manager', 'relationship': { 'children_num': 0, 'parent_num': 1, 'sibling_num': 7 } },
        //      {
        //          'name': 'Su Miao', 'title': 'department manager', 'relationship': { 'children_num': 2, 'parent_num': 1, 'sibling_num': 7 },
        //          'children': [
        //            { 'name': 'Tie Hua', 'title': 'senior engineer', 'relationship': { 'children_num': 0, 'parent_num': 1, 'sibling_num': 1 } },
        //            {
        //                'name': 'Hei Hei', 'title': 'senior engineer', 'relationship': { 'children_num': 2, 'parent_num': 1, 'sibling_num': 1 },
        //                'children': [
        //                  { 'name': 'Pang Pang', 'title': 'engineer', 'relationship': { 'children_num': 0, 'parent_num': 1, 'sibling_num': 1 } },
        //                  { 'name': 'Xiang Xiang', 'title': 'UE engineer', 'relationship': { 'children_num': 0, 'parent_num': 1, 'sibling_num': 1 } }
        //                ]
        //            }
        //          ]
        //      },
        //      { 'name': 'Yu Jie', 'title': 'department manager', 'relationship': { 'children_num': 0, 'parent_num': 1, 'sibling_num': 7 } },
        //      { 'name': 'Yu Li', 'title': 'department manager', 'relationship': { 'children_num': 0, 'parent_num': 1, 'sibling_num': 7 } },
        //      { 'name': 'Hong Miao', 'title': 'department manager', 'relationship': { 'children_num': 0, 'parent_num': 1, 'sibling_num': 7 } },
        //      { 'name': 'Yu Wei', 'title': 'department manager', 'relationship': { 'children_num': 0, 'parent_num': 1, 'sibling_num': 7 } },
        //      { 'name': 'Chun Miao', 'title': 'department manager', 'relationship': { 'children_num': 0, 'parent_num': 1, 'sibling_num': 7 } },
        //      { 'name': 'Yu Tie', 'title': 'department manager', 'relationship': { 'children_num': 0, 'parent_num': 1, 'sibling_num': 7 } }
        //    ]
        //};


        var datascource = {
            'id': '1',
            'name': 'Business Owner',
            'title': 'Business owner name',
            'children': [
              {
                  'id': '2', 'name': 'Manager- Active', 'title': 'manager active name',
                  'children': [
                  {
                      'id': '3', 'name': 'Supervisor- Active', 'title': 'supervisor active name',
                      'children': [
                          {
                              'id': '5', 'name': 'Division', 'title': 'division name', 'costcenter': '2',
                              'children': [
                                  { 'id': '6', 'name': 'Operator- Active', 'title': 'operator active name' },
                                  { 'id': '7', 'name': 'Operator-Inactive', 'title': 'operator inactive name' }
                              ]
                          }
                      ]
                  },
                  {
                      'id': '4', 'name': 'Division', 'title': 'division name', 'costcenter': '1',
                      'children': [
                            { 'id': '5', 'name': 'Operator-Inactive', 'title': 'operator inactive name' }
                      ],
                      'costcenter': '1'
                  }
                  ]
              },

            ]
        };

        customBuilderFactory.orgStructurePopup(datascource);

    }]);

})(angular.module('newApp'));