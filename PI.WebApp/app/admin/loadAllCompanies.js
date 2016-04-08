'use strict';

(function (app) {
    app.controller('loadCompaniesCtrl', ['$scope', '$location', '$window', 'adminFactory',
                  function ($scope, $location, $window, adminFactory) {
                      var vm = this;
                      vm.status = 'All';
                      vm.itemsByPage = 25;
                      vm.rowCollection = [];


                      vm.searchComapnies = function () {

                          // Get values from view.
                          var userId = $window.localStorage.getItem('userGuid');
                          vm.userId = userId;
                          var searchText = (vm.searchText == undefined || vm.searchText == "") ? null : vm.searchText;
                          var status = (status == undefined || status == "" || status == "All") ? null : status;

                          debugger;
                          adminFactory.getAllComapnies(searchText, status)
                              .then(function successCallback(responce) {

                                  vm.rowCollection = responce.data.content;
                                  debugger;
                              }, function errorCallback(response) {
                                  //todo
                              });
                      };


                      vm.searchComapnies = function (status) {

                          // Get values from view.
                          var userId = $window.localStorage.getItem('userGuid');
                          vm.userId = userId;
                          var searchText = (vm.searchText == undefined || vm.searchText == "") ? null : vm.searchText;
                          var status = (status == undefined || status == "" || status == "All") ? null : status;

                          adminFactory.getAllComapnies(searchText, status)
                              .then(function successCallback(responce) {

                                  vm.rowCollection = responce.data.content;
                              }, function errorCallback(response) {
                                  //todo
                              });
                      };

                      // Call search function in page load.
                      vm.searchComapnies();

                      vm.loadCompanyByStatus = function (status) {
                          debugger;

                          vm.searchComapnies(status);
                      };

                      vm.changeCompanyStatus = function (row) {

                          var r = confirm("Are you sure you need to change the status?");
                          if (r == true) {
                              debugger;
                              adminFactory.changeCompanyStatus({ Id: row.id })
                                  .success(function (response) {
                                      debugger;
                                      row.status = response;
                                      //vm.buttonName = (response) ? 'Deactivate' : 'Activate';
                                  })
                                  .error(function () {
                                  })
                          }
                      };

                  }]);

})(angular.module('newApp'));
