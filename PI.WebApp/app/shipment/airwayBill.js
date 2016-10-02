'user strict';

(function (app) {

    app.controller('airwayBillCtrl', ['$window', '$scope', 'Upload', '$http', '$location', 'shipmentFactory', '$rootScope', function ($window, $scope, Upload, $http, $location, shipmentFactory, $rootScope) {

        var userId = $window.localStorage.getItem('userGuid');

        var vm = this;
        vm.shipmnetCurrencyLabel = '';
        vm.valueCurrencyList = [
           { "Id": 1, "Name": "USD" },
           { "Id": 2, "Name": "EUR" },
           { "Id": 3, "Name": "YEN" },
           { "Id": 4, "Name": "GBP" }
        ];

        vm.currentRole = $window.localStorage.getItem('userRole');
        vm.isEditableAwb = vm.currentRole == 'Admin';

        vm.saveAwbNo = function () {

            var awbDto = {
                shipmentId: vm.shipment.shipmentId,
                trackingNumber: vm.shipment.trackingNumber
            };

            shipmentFactory.saveAwbNo(awbDto)
                .then(function (response) {

                    if (response.data.status == 2) {
                        $('#panel-notif').noty({
                            text: '<div class="alert alert-success media fade in"><p>' + $rootScope.translate('Tracking number updated successfully') + '</p></div>',
                            layout: 'bottom-right',
                            theme: 'made',
                            animation: {
                                open: 'animated bounceInLeft',
                                close: 'animated bounceOutLeft'
                            },
                            timeout: 3000,
                        });
                    }
                },
                function (error) {
                    if (response.data.status == 1) {
                        $('#panel-notif').noty({
                            text: '<div class="alert alert-error media fade in"><p>' + $rootScope.translate('There is an error when updating tracking number') + '</p></div>',
                            layout: 'bottom-right',
                            theme: 'made',
                            animation: {
                                open: 'animated bounceInLeft',
                                close: 'animated bounceOutLeft'
                            },
                            timeout: 3000,
                        });
                    }
                });
        }

        vm.print = function (divId) {

            var cont = document.getElementById(divId);
            var printContents = document.getElementById(divId).innerHTML;
            var popupWin = window.open('', '_blank', 'width=800,height=800');
            popupWin.document.open();
            popupWin.document.write('<html><head></head><body onload="window.print()">' + printContents + '</body></html>');
            popupWin.document.close();
        }

        vm.GetCurrencyLabel = function (id) {

            angular.forEach(vm.valueCurrencyList, function (item, key) {
                if (item.Id == id) {
                    vm.shipmnetCurrencyLabel = item.Name;
                }
            });
        }

        vm.shipmentCode = $location.search().SHIPMENT_CODE;   

        GetshipmentByShipmentCodeForAirwayBill();

        function GetshipmentByShipmentCodeForAirwayBill() {
            shipmentFactory.GetshipmentByShipmentCodeForAirwayBill(vm.shipmentCode)
            .success(function (data) {

                vm.shipment = data;          
               
                vm.GetCurrencyLabel(vm.shipment.valueCurrency);
            })
            .error(function () {
            })
        }
        
    }])
})(angular.module('newApp'));