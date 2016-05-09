'use strict';
(function (app) {

    app.controller('shipmentManageCtrl', ['$scope', '$location', '$window', 'shipmentFactory','ngDialog','$controller',
                       function ($scope, $location, $window, shipmentFactory, ngDialog, $controller) {

                           var vm = this;
                           vm.searchText = '';                          
                     
                           vm.loadAllCompanies = function () {

                               shipmentFactory.loadAllcompanies(vm.searchText).success(
                                  function (responce) {
                                      if (responce.content.length > 0) {

                                          ngDialog.open({
                                              scope: $scope,
                                              template: '/app/shipment/CompanyViewTemplate.html',
                                              className: 'ngdialog-theme-default',
                                              controller: $controller('companyListCtrl', {
                                                  $scope: $scope,
                                                  searchList: responce.content                                             
                                              })

                                          });


                                      } else {
                                          vm.addressDetailsEmpty = true;
                                          vm.emptySearch = false;
                                      }
                                  }).error(function (error) {

                                      console.log("error occurd while retrieving Addresses");
                                  });

                           }




       
                       }])
})(angular.module('newApp'));
