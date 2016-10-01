'use strict';

(function (app) {

    app.controller('disputeInvoiceCtrl', ['$route', '$scope', '$location', '$window', '$timeout', 'invoice', 'customerInvoiceFactory','$rootScope',
        function ($route, $scope, $location, $window, $timeout, invoice, customerInvoiceFactory, $rootScope) {
            var vm = this;
            $scope.isSubmit = false;

            $scope.cancelDispute = function () {
                $scope.disputecomment = "";
                $scope.customerinvoiceCtrl.closeWindow();
            }

            $scope.disputeInvoice = function () {
                
                invoice.disputeComment = $scope.disputecomment;
                customerInvoiceFactory.disputeInvoice(invoice).success(                               
                    function (responce) {
                        if (responce == 3) {                         
                                
                            $('#panel-notif').noty({
                                text: '<div class="alert alert-success media fade in"><p> ' + $rootScope.translate('Invoice disputed successfully') + '.</p></div>',
                                buttons: [
                                        {
                                            addClass: 'btn btn-primary', text: $rootScope.translate('Ok'), onClick: function ($noty) {
                                                $scope.customerinvoiceCtrl.closeWindow();
                                                $noty.close();
                                                $route.reload();
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