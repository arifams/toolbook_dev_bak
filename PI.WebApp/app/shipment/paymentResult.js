'use strict';

(function (app) {

    app.controller('paymentResultCtrl',
       ['$location', '$window', 'shipmentFactory',
           function ($location, $window, shipmentFactory) {
               var vm = this;

               vm.paymentMsgStatus = false;
               vm.paymentMsg = "";

               if (window.location.search != "") {

                   var splittedValues = window.location.search.replace("?", "").split('&');
                   debugger;

                   // If success.
                   shipmentFactory.SendShipmentDetails($window.localStorage.getItem('shipmentId')).success(
                            function (response) {
                               
                            }).error(function (error) {
                                $('#panel-notif').noty({
                                    text: '<div class="alert alert-danger media fade in"><p>Error occured while saving the Shipment!</p></div>',
                                    layout: 'bottom-right',
                                    theme: 'made',
                                    animation: {
                                        open: 'animated bounceInLeft',
                                        close: 'animated bounceOutLeft'
                                    },
                                    timeout: 6000,
                                });
                            });


                   //if (splittedValues.length != 2 || splittedValues[0].split('=').length != 2 || splittedValues[1].split('=').length != 2) {
                   //    vm.emailConfirmationMessage = "Confirmation URL link is not properly formatted. Please resend the confirmation URL";
                   //    return;
                   //}

                   //var userIdKeyValue = splittedValues[0].split('=');
                   //var codeKeyValue = splittedValues[1].split('=');

                   //if (userIdKeyValue[0] != 'userId') {
                   //    vm.emailConfirmationMessage = "Confirmation URL link is not properly formatted. Please resend the confirmation URL";
                   //    return;
                   //}
                   //if (codeKeyValue[0] != 'code') {
                   //    vm.emailConfirmationMessage = "Confirmation URL link is not properly formatted. Please resend the confirmation URL";
                   //    return;
                   //}

                   //user.userId = userIdKeyValue[1];
                   //user.code = codeKeyValue[1];
                   //user.isConfirmEmail = true;
               }
             
           }]);


})(angular.module('newApp'));