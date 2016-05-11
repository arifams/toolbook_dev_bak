
'use strict';


(function (app) {

    app.controller('companyListCtrl',
       ['$location', '$window', 'shipmentFactory', '$scope', 'searchList','from',
    function ($location, $window, shipmentFactory, $scope, searchList, from) {

        $scope.companyCollection = searchList;


        //set selected address details
        $scope.selectCompany = function (company) {
            debugger;
            $scope.manageShipCtrl.CompanyId = company.id;

            if (from == 'shipReportCtrl') {
                if (responce.content.length > 0) {
                   
                    $scope.shipReportCtrl.closeWindow();                    
                    $scope.shipReportCtrl.CompanyId = company.id;

                } else {
                    $scope.shipReportCtrl.noShipments = true;
                    $scope.shipReportCtrl.closeWindow();
                }
            }


            if (from == 'manageShipCtrl') {
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