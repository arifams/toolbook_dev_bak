'use strict';

(function (app) {

    app.controller('paymentResultCtrl',
       ['$location', '$window', 'shipmentFactory','$rootScope',
           function ($location, $window, shipmentFactory, $rootScope) {
               var vm = this;

               vm.paymentMsgStatus = false;
               vm.showLabel = false;
               vm.paymentMsg = "";
               console.log(window.location.hash);
               if (window.location.hash != "") {

                   var splittedValues = window.location.hash.replace("#/PaymentResult?", "").split('&');
                   
                   // status=PERFORMED&amount=316.37&currency=USD&description=test&hash=c0ad367c5432805481d6ff52091450e6a13634c3&id_sale=8732777 -

                   var statusKeyValue = splittedValues[0].split('=');
                   var hashKeyValue = splittedValues[4].split('=');
                   var saleIdKeyValue = splittedValues[5].split('=');
                   
                   var sendShipmentData = {
                       shipmentId: $window.localStorage.getItem('shipmentId'),
                       userId: $window.localStorage.getItem('userGuid'),
                       payLane: {
                           status: statusKeyValue[1],
                           hash: hashKeyValue[1],
                           saleId: saleIdKeyValue[1]
                       },
                       templateLink: '<html><head><title></title></head><body style="margin:30px;"><div style="margin-right:40px;margin-left:40px"><div style="margin-top:30px;background-color:#0af;font-size:28px;border:5px solid #d9d9d9;text-align:center;padding:10px;font-family:verdana,geneva,sans-serif;color:#fff">Order Confirmation - Parcel International</div></div><div style="margin-right:40px;margin-left:40px"><div style="float:left;"><img alt="" src="http://www.parcelinternational.nl/assets/Uploads/_resampled/SetWidth495-id-parcel-big.jpg" style="width: 130px; height: 130px;" /></div><div><h3 style="margin-bottom:65px;margin-right:146px;margin-top:0;padding-top:62px;text-align:center;font-size:22px;font-family:verdana,geneva,sans-serif;color:#005c99">Thank you for using Parcel International </h3></div></div><div style="margin-right:40px;margin-left:40px"><div style="padding:10px;font-family:verdana,geneva,sans-serif;color:#fff;border:5px solid #0af;background-color:#005c99;font-size:13px"><p style="font-weight:700;font-style:italic;font-size:16px;">Order Reference  &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<OrderReference></OrderReference></p><p style="font-weight:700;font-style:italic;font-size:16px;">Pickup Date  &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; <PickupDate></PickupDate></p><p style="font-weight:700;font-style:italic;font-size:16px;">Shipment Mode  &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; <ShipmentMode></ShipmentMode></p><p style="font-weight:700;font-style:italic;font-size:16px;">Shipment Type  &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; <ShipmentType></ShipmentType></p><p style="font-weight:700;font-style:italic;font-size:16px;">Carrier   &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<Carrier></Carrier></p><p style="font-weight:700;font-style:italic;font-size:16px;">Shipment Price   &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<ShipmentPrice></ShipmentPrice></p><p style="font-weight:700;font-style:italic;font-size:16px">Payment Type   &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<PaymentType></PaymentType></p></div><br><div style="padding:10px;font-family:verdana,geneva,sans-serif;color:#fff;border:5px solid #0af;background-color:#005c99;font-size:13px"><table><thead><tr><th style="width:290px;color:#fff;font-size:16px;border-bottom:2px solid #fff;">Product Type</th><th style="width:290px;color:#fff;font-size:16px;border-bottom:2px solid #fff;">Quantity</th><th style="width:290px;color:#fff;font-size:16px;border-bottom:2px solid #fff;">Weight</th><th style="width:290px;color:#fff;font-size:16px;border-bottom:2px solid #fff;">Volume</th></tr></thead><tbody><tableRecords></tbody></table></div><p style="font-size:20px;text-align:center;">should you have any questions or concerns, please contact Parcel International helpdesk for support.</p></body></html>'
                   };
                   console.log(sendShipmentData);
                   shipmentFactory.sendShipmentDetails(sendShipmentData).success(
                            function (response) {
                                
                                vm.showLabel = false;
                                console.log('response');
                                console.log(response);
                                
                                vm.paymentMsgStatus = true;
                                vm.paymentMsg = response.message;
                                
                                if (response.status == 2) { // Success
                                    vm.showLabel = true;
                                    vm.labelUrl = response.labelURL;
                                }
                                else if (response.status == 5) { // SIS Error. Redirect to error label page.
                                    var errorUrl = 'http://parcelinternational.pro/errors/' + response.carrierName + '/' + response.shipmentCode;
                                    window.open(errorUrl);
                                }

                                
                            }).error(function (error) {
                                $('#panel-notif').noty({
                                    text: '<div class="alert alert-danger media fade in"><p>' + $rootScope.translate('Error occured while adding the Shipment') + '!</p></div>',
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