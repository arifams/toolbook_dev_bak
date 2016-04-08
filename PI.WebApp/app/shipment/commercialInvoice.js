﻿'user strict';

(function (app) {

    app.controller('commercialInvoiceCtrl', ['$window', '$scope', 'Upload', '$http', '$location', 'shipmentFactory', function ($window, $scope, Upload, $http, $location, shipmentFactory) {

        var userId = $window.localStorage.getItem('userGuid');

        var vm = this;

        vm.shipmentCode = $location.search().SHIPMENT_CODE;
        var shipmentId = '';

        vm.shipmentServices = [];
        vm.shipmentServices = [{ "Id": "DD-DDP-PP", "Name": "Door-to-Door, DDP, Prepaid" },
                           { "Id": "DD-DDU-PP", "Name": "Door-to-Door, DDU, Prepaid" },
                           { "Id": "DD-CIP-PP", "Name": "Door-to-Door, CIP, Prepaid" },
                           { "Id": "DP-CIP-PP", "Name": "Door-to-Port, CIP, Prepaid" },
                           { "Id": "DP-CPT-PP", "Name": "Door-to-Port, CPT, Prepaid" },
                           { "Id": "PD-CPT-PP", "Name": "Port-to-Door, CPT, Prepaid" },
                           { "Id": "PD-CIP-PP", "Name": "Port-to-Door, CIP, Prepaid" },
                           { "Id": "PP-CPT-PP", "Name": "Port-to-Port, CPT, Prepaid" },
                           { "Id": "PP-CIP-PP", "Name": "Port-to-Port, CIP, Prepaid" },
                           { "Id": "DP-FCA-CC", "Name": "FCA-Free Carrier" },
                           { "Id": "DF-EXW-CC", "Name": "EXW-Ex Works" },
                           { "Id": "KMSDY", "Name": "Door-to-Door, SDY, Same Day" },
        ];

        vm.valueCurrencyList = [
            { "Id": 1, "Name": "USD" },
            { "Id": 2, "Name": "EUR" },
            { "Id": 3, "Name": "YEN" },
            { "Id": 5, "Name": "GBP" }
        ];


        getshipmentByShipmentCodeForInvoice();

        function getshipmentByShipmentCodeForInvoice() {
            shipmentFactory.getshipmentByShipmentCodeForInvoice(vm.shipmentCode)
            .success(function (data) {
                vm.shipment = data;
                console.info("shipment info in commercial invoice");
                console.info(vm.shipment);
                shipmentId = vm.shipment.generalInformation.shipmentId;
                vm.shipment.termsOfPayment = "FREE OF CHARGE";
                vm.shipment.modeOfTransport = vm.shipment.carrierInformation.carrierName + " " + vm.shipment.carrierInformation.serviceLevel + " " + vm.shipment.generalInformation.trackingNumber;
                vm.shipment.item = {};
                vm.shipment.item.lineItems = [{ no: 1, description: "", qty: 0, pricePerPiece:0.0 }];
                
            })
            .error(function () {
            })
        }

        vm.addEmptyRow = function () {
            vm.shipment.item.lineItems.push({ description: "", qty: 0, pricePerPiece: 0.0 });
            vm.calculateTotal();
        };

        vm.removePackage = function (index) {
            vm.shipment.item.lineItems.splice(index, 1);
            // if array length is 0, then one empty row will insert.
            if (vm.shipment.item.lineItems.length == 0)
                vm.addEmptyRow();
            vm.calculateTotal();
        };

        //calculating the total volume and total weight
        vm.calculateTotal = function () {

            vm.shipment.item.totalPrice = 0.0;

            for (var i = 0; i < vm.shipment.item.lineItems.length; i++) {
                vm.shipment.item.totalPrice = vm.shipment.item.totalPrice + (vm.shipment.item.lineItems[i].qty * vm.shipment.item.lineItems[i].pricePerPiece);
            }
        }

        vm.save = function () {
            
        };
    }])
})(angular.module('newApp'));