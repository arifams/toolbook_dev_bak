'use strict';

(function (app) {

    app.controller('paymentResultCtrl',
       ['$location', '$window', 'shipmentFactory',
           function ($location, $window, shipmentFactory) {
               var vm = this;

               vm.paymentMsgStatus = false;
               vm.paymentMsg = "";
               console.log(window.location.hash);
               if (window.location.hash != "") {

                   var splittedValues = window.location.hash.replace("#/PaymentResult?", "").split('&');
                   
                   // status=PERFORMED&amount=316.37&currency=USD&description=test&hash=c0ad367c5432805481d6ff52091450e6a13634c3&id_sale=8732777

                   var statusKeyValue = splittedValues[0].split('=');
                   var hashKeyValue = splittedValues[4].split('=');
                   var saleIdKeyValue = splittedValues[5].split('=');
                   debugger;
                   var sendShipmentData = {
                       shipmentId: $window.localStorage.getItem('shipmentId'),
                       userId: $window.localStorage.getItem('userGuid'),
                       payLane: {
                           status: statusKeyValue[1],
                           hash: hashKeyValue[1],
                           saleId: saleIdKeyValue[1]
                       }
                   };
                   console.log(sendShipmentData);
                   shipmentFactory.sendShipmentDetails(sendShipmentData).success(
                            function (response) { 
                                debugger; 
                                vm.paymentMsgStatus = true;
                                vm.paymentMsg = response.message;
                                vm.labelUrl = response.labelURL;
                            }).error(function (error) {
                                $('#panel-notif').noty({
                                        text: '<div class="alert alert-danger media fade in"><p>Error occured while adding the Shipment!</p></div>',
                                        layout: 'bottom-right',
                                        theme: 'made',
                                        animation: {
                                                open: 'animated bounceInLeft',
                                                close: 'animated bounceOutLeft'
                                },
                                        timeout: 6000,
                            });
                            });
               }
             
           }]);


})(angular.module('newApp'));