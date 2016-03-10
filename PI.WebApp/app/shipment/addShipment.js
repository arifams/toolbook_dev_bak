'use strict';

(function (app) {

    app.controller('addShipmentCtrl', ['$location', '$window', 'shipmentFactory', 'calculateRatesforShipment', function ($location, $window, shipmentFactory, calculateRatesforShipment) {
                  
       
        var vm = this;
        vm.user = {};
        vm.shipment = {};
        vm.collapse1 = false;
        vm.collapse2 = true;
        vm.collapse3 = true;
        vm.collapse4 = true;
        vm.generalInfoisSubmit = false;
        vm.consignInfoisSubmit = false;
        vm.packageDetailsisSubmit = false;
        vm.shipment.packageDetails = {};
        vm.shipment.packageDetails.productIngredients = [{}];

        vm.checkGenaralInfo = function (value) {
            if (value==true) {
                vm.collapse1 = true;
                vm.collapse2 = false;               
            }
            vm.generalInfoisSubmit = true;

           
        }

        vm.checkConsignInfo = function (value) {
            if (value == true) {               
                vm.collapse2 = true;
                vm.collapse3 = false;   
            }
            vm.consignInfoisSubmit = true

        }
        vm.checkPackageDetails = function (value) {
            if (value) {
                vm.collapse3 = true;
                vm.collapse4 = false;
            }
            vm.packageDetailsisSubmit = true
        }
       
        vm.ClearConsignerAddress = function () {
            vm.AddressInformation.consigner = {};
        }
        vm.ClearConsigneeAddress= function(){
            vm.AddressInformation.consignee = {};
        }
        
        //accordian functionality
        //$(document).ready(function () {
        //    $('#accordion').accordion();
        //    $("#accordion").accordion({ event: false });
        //    $('#accordion button').click(function (e) {
        //        var delta
        //        e.preventDefault();
        //        if ($(this).is('.btn.btn-blue')) {
        //            delta = 1;
        //        }
        //        if ($(this).is('.btn.btn-dark')) {
        //            delta = -1;
        //        }               
        //        $('#accordion').accordion('option', 'active', ($('#accordion').accordion('option', 'active') + delta));
        //    });
        //});

        
        vm.addEmptyRow = function () {
            vm.shipment.packageDetails.productIngredients.push({});
        };

        vm.removePackage = function (index) {
            vm.shipment.packageDetails.productIngredients.splice(index, 1);
            // if array length is 0, then one empty row will insert.
            if (vm.shipment.packageDetails.productIngredients.length == 0)
                vm.addEmptyRow();
        };

        //get the calculated rates
        vm.calculateRates = function () {

            calculateRatesforShipment.calculateRates(vm.shipment).success(
                function (responce) {

                }).error(function (error) {

                });
        }


    }]);


})(angular.module('newApp'));