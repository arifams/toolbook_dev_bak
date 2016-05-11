
'use strict';


(function (app) {

    app.controller('companyListCtrl',
       ['$location', '$window', 'shipmentFactory', '$scope', 'searchList','from',
    function ($location, $window, shipmentFactory, $scope, searchList, from) {

        $scope.companyCollection = searchList;


        //set selected address details
        $scope.selectCompany = function (company) {
            debugger;         

            if (from == 'shipReportCtrl') {                
                   
                $scope.shipCtrl.closeWindow();
                $scope.shipCtrl.CompanyId = company.id;
                
            }


            if (from == 'manageShipCtrl') {
                $scope.manageShipCtrl.CompanyId = company.id;
                shipmentFactory.loadAllshipmentsForCompany(company.id).success(

                                      function (responce) {
                                          if (responce.content.length > 0) {
                                              $scope.manageShipCtrl.rowCollection = responce.content;
                                              $scope.manageShipCtrl.closeWindow();
                                              $scope.manageShipCtrl.noShipments = false;
                                              $scope.manageShipCtrl.CompanyId = company.id;

                                          } else {
                                              $scope.manageShipCtrl.noShipments = true;
                                              $scope.manageShipCtrl.closeWindow();
                                          }


                                      }).error(function (error) {

                                          console.log("error occurd while retrieving Addresses");
                                      });
            }
        }
    }]);


})(angular.module('newApp'));