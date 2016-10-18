
'use strict';


(function (app) {

    app.controller('companyListCtrl',
       ['$location', '$window', 'shipmentFactory', '$scope', 'searchList','from',
    function ($location, $window, shipmentFactory, $scope, searchList, from) {

        $scope.companyCollection = searchList;


        //set selected address details
        $scope.selectCompany = function (company) {
                     

            if (from == 'shipReportCtrl') {                
                $scope.vm.selectedCompanyId = company.id;
                $scope.vm.isNeedSearchCustomer = false;
                $scope.vm.closeWindow();                
            }


            if (from == 'manageShipCtrl') {
               
                //$scope.manageShipCtrl.CompanyId = company.id;
                $scope.manageShipCtrl.closeWindow();
                $location.path('/addShipment/0');
                //shipmentFactory.loadAllshipmentsForCompany(company.id).success(

                //                      function (responce) {
                //                          if (responce.content.length > 0) {
                //                              $scope.manageShipCtrl.rowCollection = responce.content;
                //                              $scope.manageShipCtrl.closeWindow();
                //                              $scope.manageShipCtrl.noShipments = false;
                //                              $scope.manageShipCtrl.CompanyId = company.id;
                                              
                //                              shipmentFactory.GetBusinessOwneridbyCompanyId(company.id).success(
                //                               function (responce) {
                //                                   $window.localStorage.setItem('businessOwnerId', responce);
                                              
                //                               }).error(
                //                               function (error) {
                                              
                //                                   console.log("error occurd while retrieving business owner Id");
                //                               });

                //                          } else {
                //                              $scope.manageShipCtrl.noShipments = true;
                //                              $scope.manageShipCtrl.closeWindow();
                //                              shipmentFactory.GetBusinessOwneridbyCompanyId(company.id).success(
                //                              function (responce) {
                //                                  $window.localStorage.setItem('businessOwnerId', responce);

                //                              }).error(
                //                              function (error) {

                //                                  console.log("error occurd while retrieving business owner Id");
                //                              });
                //                          }


                //                      }).error(function (error) {

                //                          console.log("error occurd while retrieving Addresses");
                //                      });
            }
        }
    }]);


})(angular.module('newApp'));