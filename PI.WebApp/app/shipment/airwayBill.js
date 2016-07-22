'user strict';

(function (app) {

    app.controller('airwayBillCtrl', ['$window', '$scope', 'Upload', '$http', '$location', 'shipmentFactory', '$rootScope', function ($window, $scope, Upload, $http, $location, shipmentFactory, $rootScope) {

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


     
        getshipmentByShipmentCodeForInvoice();

        function getshipmentByShipmentCodeForInvoice() {
            shipmentFactory.getshipmentByShipmentCodeForInvoice(vm.shipmentCode)
            .success(function (data) {

                debugger;
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
                else {
                    vm.calculateTotal();
                }
            })
            .error(function () {
            })
        }
        
    }])
})(angular.module('newApp'));