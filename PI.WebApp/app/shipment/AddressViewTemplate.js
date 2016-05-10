
'use strict';


(function (app) {

    app.controller('addressListCtrl',
       ['$location', '$window', 'shipmentFactory', '$scope', 'searchList','consignor',
    function ($location, $window, shipmentFactory, $scope, searchList,consignor) {
            
               $scope.addressCollection = searchList;
                           
                           
               //set selected address details
               $scope.selectAddress = function (address) {
                   if (consignor) {
                       $scope.shipmentCtrl.shipment.addressInformation.consigner.firstName = address.firstName;
                       $scope.shipmentCtrl.shipment.addressInformation.consigner.lastName = address.lastName;
                       $scope.shipmentCtrl.shipment.addressInformation.consigner.country = address.country;
                       $scope.shipmentCtrl.shipment.addressInformation.consigner.postalcode = address.zipCode;
                       $scope.shipmentCtrl.shipment.addressInformation.consigner.number = address.number;
                       $scope.shipmentCtrl.shipment.addressInformation.consigner.address1 = address.streetAddress1;
                       $scope.shipmentCtrl.shipment.addressInformation.consigner.address2 = address.streetAddress2;
                       $scope.shipmentCtrl.shipment.addressInformation.consigner.city = address.city;
                       $scope.shipmentCtrl.shipment.addressInformation.consigner.state = address.state
                       $scope.shipmentCtrl.shipment.addressInformation.consigner.email = address.emailAddress;
                       $scope.shipmentCtrl.shipment.addressInformation.consigner.contactNumber = address.phoneNumber;
                       $scope.shipmentCtrl.consignorAdded = 'true';
                       $scope.shipmentCtrl.closeWindow();
                       
                   } else {
                       $scope.shipmentCtrl.shipment.addressInformation.consignee.firstName = address.firstName;
                       $scope.shipmentCtrl.shipment.addressInformation.consignee.lastName = address.lastName;
                       $scope.shipmentCtrl.shipment.addressInformation.consignee.country = address.country;
                       $scope.shipmentCtrl.shipment.addressInformation.consignee.postalcode = address.zipCode;
                       $scope.shipmentCtrl.shipment.addressInformation.consignee.number = address.number;
                       $scope.shipmentCtrl.shipment.addressInformation.consignee.address1 = address.streetAddress1;
                       $scope.shipmentCtrl.shipment.addressInformation.consignee.address2 = address.streetAddress2;
                       $scope.shipmentCtrl.shipment.addressInformation.consignee.city = address.city;
                       $scope.shipmentCtrl.shipment.addressInformation.consignee.state = address.state
                       $scope.shipmentCtrl.shipment.addressInformation.consignee.email = address.emailAddress;
                       $scope.shipmentCtrl.shipment.addressInformation.consignee.contactNumber = address.phoneNumber;
                       $scope.shipmentCtrl.consigneeAdded = 'true';
                       $scope.shipmentCtrl.closeWindow();
                     
                   }
                   
                  
               }

           }]);


})(angular.module('newApp'));