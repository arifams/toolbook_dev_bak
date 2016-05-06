'use strict';
(function (app) {

    app.controller('shipmentManageCtrl', ['$scope', '$location', '$window', 'shipmentFactory',
                       function ($scope, $location, $window, shipmentFactory) {

                           var vm = this;
                           vm.searchText = '';                          

                           vm.loadAllCompanies = function () {                             
                               
                              var searchtext= vm.searchText;
                               shipmentFactory.loadAllcompanies(searchtext)
                                   .success(                                   
                                          function (responce) {
                                             // vm.rowCollection = responce.content;
                                          }).error(function (error) {
                                              console.log("error occurd while retrieving shiments");
                                          });
                           }
       
                       }])
})(angular.module('newApp'));
