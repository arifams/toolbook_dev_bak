'user strict';

(function (app) {

    app.controller('commercialInvoiceCtrl', ['$window', '$scope', 'Upload', '$http', '$location', 'shipmentFactory', function ($window, $scope, Upload, $http, $location, shipmentFactory) {

        var userId = $window.localStorage.getItem('userGuid');

        var vm = this;

        vm.shipmentCode = $location.search().SHIPMENT_CODE;
        //vm.trakingNo = $location.search().TRACKING_NO;
        //vm.carrier = $location.search().CARRIER;
        //vm.createdOn = $location.search().CREATED_ON;
        var shipmentId = '';

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
                //vm.shipmentLabel = data.generalInformation.shipmentLabelBLOBURL;
                //console.log(vm.shipmentLabel);
            })
            .error(function () {
            })
        }

    }])
})(angular.module('newApp'));