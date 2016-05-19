'use strict';

(function (app) {

    app.controller('disputeInvoiceCtrl', ['$route', '$scope', '$location', '$window', '$timeout', 'invoice', 'customerInvoiceFactory',
        function ($route, $scope, $location, $window, $timeout, invoice, customerInvoiceFactory) {
            var vm = this;
            $scope.isSubmit = false;

            $scope.cancelDispute = function () {
                $scope.customerinvoiceCtrl.closeWindow();
            }

            $scope.disputeInvoice = function () {
                debugger;
                invoice.disputeComment = $scope.disputecomment;
                customerInvoiceFactory.disputeInvoice(invoice).success(                               
                    function (responce) {
                        if (responce == 3) {                         
                                
                            $('#panel-notif').noty({
                                text: '<div class="alert alert-success media fade in"><p>' + ' Rates records added successfully.' + '</p></div>',
                                buttons: [
                                        {
                                            addClass: 'btn btn-primary', text: 'Ok', onClick: function ($noty) {
                                                $scope.customerinvoiceCtrl.closeWindow();
                                                $route.reload();
                                                $noty.close();


                                            }
                                        }

                                ],
                                layout: 'bottom-right',
                                theme: 'made',
                                animation: {
                                    open: 'animated bounceInLeft',
                                    close: 'animated bounceOutLeft'
                                },
                                timeout: 3000,
                            });
                            
                        }
                        $scope.customerinvoiceCtrl.closeWindow();

                    }
                     
                                  
                      ).error(function (error) {
                                      console.log("error occurd while dispute invoice");
                                  });

            }
            
         
        }

    ])
})(angular.module('newApp'))