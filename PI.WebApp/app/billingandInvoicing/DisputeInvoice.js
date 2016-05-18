'use strict';

(function (app) {

    app.controller('disputeInvoiceCtrl', ['$route', '$scope', '$location', '$window', '$timeout', 'invoice', 'customerInvoiceFactory',
        function ($route, $scope, $location, $window, $timeout, invoice, customerInvoiceFactory) {
            var vm = this;

            $scope.cancelDispute = function () {
                $scope.customerinvoiceCtrl.closeWindow();
            }

            $scope.disputeInvoice = function () {

                customerInvoiceFactory.disputeInvoice(invoice.id, disputecomment).success(
                                  
                      $scope.customerinvoiceCtrl.closeWindow()
                                  
                      ).error(function (error) {
                                      console.log("error occurd while dispute invoice");
                                  });

            }
            
         
        }

    ])
})(angular.module('newApp'))