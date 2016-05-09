
'use strict';


(function (app) {

    app.controller('companyListCtrl',
       ['$location', '$window', 'shipmentFactory', '$scope', 'searchList',
    function ($location, $window, shipmentFactory, $scope, searchList) {

        $scope.companyCollection = searchList;


        //set selected address details
        $scope.selectCompany = function (company) {
            debugger;
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
    }]);


})(angular.module('newApp'));