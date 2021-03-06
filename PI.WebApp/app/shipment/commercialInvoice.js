﻿'user strict';

(function (app) {

    app.controller('commercialInvoiceCtrl', ['$window', '$scope', 'Upload', '$http', '$location', 'shipmentFactory', '$rootScope', function ($window, $scope, Upload, $http, $location, shipmentFactory, $rootScope) {
        
        var userId = $window.localStorage.getItem('userGuid');

        var vm = this;

        vm.print = function (divId) {
            
            var cont = document.getElementById(divId);
            var printContents = document.getElementById(divId).innerHTML;
            var popupWin = window.open('', '_blank', 'width=800,height=800');
            popupWin.document.open();
            popupWin.document.write('<html><head></head><body onload="window.print()">' + printContents + '</body></html>');
            popupWin.document.close();
        }


        vm.shipmentCode = $location.search().SHIPMENT_CODE;
        vm.shipmnetServiceLabel = '';
        vm.shipmnetCurrencyLabel = '';


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
            { "Id": 1, "Name": "USD" }//,
            //{ "Id": 2, "Name": "EUR" },
            //{ "Id": 3, "Name": "YEN" },
            //{ "Id": 4, "Name": "GBP" }
        ];


        vm.GetServiceLabel = function (id) {
           
            angular.forEach(vm.shipmentServices, function (item, key) {
                if (item.Id==id) {
                    vm.shipmnetServiceLabel = item.Name;
                }               
            });
        }

        vm.GetCurrencyLabel = function (id) {            

            angular.forEach(vm.valueCurrencyList, function (item, key) {
                if (item.Id == id) {
                    vm.shipmnetCurrencyLabel = item.Name;
                }
            });
        }

        //calculating the total volume and total weight
        vm.calculateTotal = function () {
            
            vm.shipment.item.totalPrice = 0.0;

            for (var i = 0; i < vm.shipment.item.lineItems.length; i++) {
                vm.shipment.item.totalPrice = vm.shipment.item.totalPrice + (vm.shipment.item.lineItems[i].quantity * vm.shipment.item.lineItems[i].pricePerPiece);
            }
        }

        vm.addEmptyRow = function () {
            vm.shipment.item.lineItems.push({ description: "", quantity: 0, pricePerPiece: 0.0 });
            vm.calculateTotal();
        };

        vm.removePackage = function (index) {
            vm.shipment.item.lineItems.splice(index, 1);
            // if array length is 0, then one empty row will insert.
            if (vm.shipment.item.lineItems.length == 0)
                vm.addEmptyRow();
            vm.calculateTotal();
        };

        getshipmentByShipmentCodeForInvoice();

        function getshipmentByShipmentCodeForInvoice() {
            shipmentFactory.getshipmentByShipmentCodeForInvoice(vm.shipmentCode)
            .success(function (data) {
                vm.shipment = data;
                console.info("shipment info in commercial invoice");
                console.info(vm.shipment);
                vm.shipment.termsOfPayment = "FREE OF CHARGE";
                
                vm.GetServiceLabel(vm.shipment.shipmentServices);
                vm.GetCurrencyLabel(vm.shipment.valueCurrency);
                //vm.shipment.modeOfTransport = vm.shipment.carrierInformation.carrierName + " " + vm.shipment.carrierInformation.serviceLevel + " " + vm.shipment.generalInformation.trackingNumber;
                //vm.shipment.item = {};
                if (vm.shipment.item.lineItems.length == 0) {
                    vm.addEmptyRow();
                }
                else{
                    vm.calculateTotal();
                }
            })
            .error(function () {
            })
        }

        vm.save = function () {
            shipmentFactory.saveCommercialInvoice(vm.shipment)
            .success(function (data) {
                console.log('saved success');
                $('#panel-notif1').noty({
                    text: '<div class="alert alert-success media fade in"><p>' + $rootScope.translate('Commercial invoice saved successfully') + '</p></div>',
                    layout: 'bottom-right',
                    theme: 'made',
                    animation: {
                        open: 'animated bounceInLeft',
                        close: 'animated bounceOutLeft'
                    },
                    timeout: 3000,
                });
            })
            .error(function () {
            })
        };
    }])
})(angular.module('newApp'));