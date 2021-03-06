﻿'use strict';

(function (app) {

    app.directive('validPasswordorg', function () {
        return {
            require: 'ngModel',
            link: function (scope, elm, attrs, ctrl) {
                ctrl.$parsers.unshift(function (viewValue, $scope) {
                    
                    // password validate.
                    var res = /^(?=.*[a-z])(?=.*[A-Z])[a-zA-Z\S]{7,20}$/.test(viewValue);
                    ctrl.$setValidity('noValidPassword', res);
                    
                    // if change the password when having confirmation password, check match and give error.
                    if (scope.formSaveUser.password_c.$viewValue != '') {
                        var noMatch = viewValue != scope.formSaveUser.password_c.$viewValue;
                        scope.formSaveUser.password_c.$setValidity('noMatch', !noMatch);
                    }

                    return viewValue;
                })
            }
        }
    });

    //app.directive('validPasswordCorg', function () {
    //    return {
    //        require: 'ngModel',
    //        link: function (scope, elm, attrs, ctrl) {
    //            ctrl.$parsers.unshift(function (viewValue, $scope) {
    //                
    //                var noMatch = viewValue != scope.formSaveUser.password.$viewValue;
    //                ctrl.$setValidity('noMatch', !noMatch);
    //                return viewValue;
    //            })
    //        }
    //    }
    //});

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

            $scope.editUserBtnClick = false; // used for edit btn click function
            $scope.rightPaneLoad = false; // used for change table width

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


            $scope.loadOrganizationStructure();

            $scope.manageUsers = function () {
                
                $scope.rightPaneLoad = true;
                $scope.editUserBtnClick = true;
            }

        }]);

})(angular.module('newApp'));