'use strict';

(function (app) {

    app.controller('addShipmentCtrl', ['$location', '$window', 'shipmentFactory', function ($location, $window, shipmentFactory) {
                  
       
        var vm = this;
        vm.user = {};
        vm.shipment = {};
        vm.collapse1 = false;
        vm.collapse2 = true;
        vm.collapse3 = true;
        vm.collapse4 = true;
        vm.generalInfoisSubmit = false;
        vm.consignInfoisSubmit = false;
        vm.packageDetails = {};
        vm.packageDetails.productIngredients = [{}];

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
            vm.packageDetails.productIngredients.push({});
        };

        vm.removePackage = function (index) {
            vm.packageDetails.productIngredients.splice(index, 1);
            // if array length is 0, then one empty row will insert.
            if (vm.packageDetails.productIngredients.length == 0)
                vm.addEmptyRow();
        };

    }]);


})(angular.module('newApp'));