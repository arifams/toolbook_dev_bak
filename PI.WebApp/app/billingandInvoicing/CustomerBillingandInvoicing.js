'use strict';

(function (app) {

    app.factory('customerInvoiceFactory', ['$http', '$routeParams', '$window', function ($http, $routeParams, $window) {

        return {
            getAllInvoicesByCustomer: getAllInvoicesByCustomer,
            payInvoice: payInvoice,
            disputeInvoice: disputeInvoice,
            disputeInvoice, disputeInvoice
        };



        function disputeInvoice(invoice) {

        }

        function getAllInvoicesByCustomer(status, startDate, endDate, shipmentNumber, invoiceNumber) {

            return $http.get(serverBaseUrl + '/api/Customer/GetAllInvoicesByCustomer', {
                params: {
                    status: status,
                    userId: $window.localStorage.getItem('userGuid'),
                    startDate: startDate,
                    endDate: endDate,
                    shipmentNumber: shipmentNumber,
                    invoiceNumber: invoiceNumber
                }
            });
        }


        function payInvoice(invoiceDetail) {

            return $http.post(serverBaseUrl + '/api/Customer/PayInvoice', invoiceDetail);
        }


        function disputeInvoice(invoiceDetail) {

            return $http.post(serverBaseUrl + '/api/Customer/DisputeInvoice', invoiceDetail);
        }

    }]);

    app.controller('customerinvoiceCtrl', ['$location', '$window', 'customerInvoiceFactory', 'ngDialog', '$controller', '$scope',
                    function ($location, $window, customerInvoiceFactory, ngDialog, $controller,$scope) {
                        var vm = this;
                        vm.datePicker = {};
                        vm.datePicker.date = { startDate: null, endDate: null };


                        vm.closeWindow = function () {
                            ngDialog.close()
                        }



                        vm.loadInvoicesBySearch = function (status) {
                            debugger;
                            var status = (status == undefined || status == 'All' || status == null || status == "") ? null : status;
                            var startDate = (vm.datePicker.date.startDate == null) ? null : vm.datePicker.date.startDate.toDate();
                            var endDate = (vm.datePicker.date.endDate == null) ? null : vm.datePicker.date.endDate.toDate();
                            var shipmentNumber = (vm.shipmentNumber == undefined || vm.shipmentNumber == "") ? null : vm.shipmentNumber;
                            var invoiceNumber = (vm.invoiceNumber == undefined || vm.invoiceNumber == "") ? null : vm.invoiceNumber;

                            customerInvoiceFactory.getAllInvoicesByCustomer(status, startDate, endDate, shipmentNumber, invoiceNumber)
                                .then(function successCallback(responce) {
                                  
                                    vm.rowCollection = responce.data.content;

                                }, function errorCallback(response) {
                                    //todo
                                });
                        };

                        vm.loadInvoicesByStatus = function (status) {
                            vm.loadInvoicesBySearch(status);
                        };
                        

                        vm.loadInvoicesBySearch();

                        vm.payInvoice = function (row) {
                            var statusChange = confirm("Are you sure you need to pay this invoice ?");

                            if (statusChange == true) {
                                customerInvoiceFactory.payInvoice({ Id: row.id })
                                    .success(function (response) {
                                    
                                        row.invoiceStatus = response;
                                    })
                                    .error(function () {
                                    })
                            }
                        };


                        vm.disputeInvoice = function (row) {

                           // var statusChange = confirm("Are you sure you need to dispute this invoice ?");

                            $('#panel-notif').noty({
                                text: '<div class="alert alert-success media fade in"><p>Are you want to Dispute the Invoice:' + row.invoiceNumber + '?</p></div>',
                                buttons: [
                                        {
                                            addClass: 'btn btn-primary', text: 'Ok', onClick: function ($noty) {

                                                ngDialog.open({
                                                    scope: $scope,
                                                    template: '/app/billingandInvoicing/DisputeInvoice.html',
                                                    className: 'ngdialog-theme-plain custom-width',
                                                    controller: $controller('disputeInvoiceCtrl', {
                                                        $scope: $scope,
                                                        invoice: row                                                        
                                                    })

                                                });
                                          

                                                $noty.close();


                                            }
                                        },
                                        {
                                            addClass: 'btn btn-danger', text: 'Cancel', onClick: function ($noty) {

                                                // updateProfile = false;
                                                $noty.close();
                                                return;
                                                // noty({text: 'You clicked "Cancel" button', type: 'error'});
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



                            //if (statusChange == true) {
                            //    customerInvoiceFactory.disputeInvoice({ Id: row.id })
                            //        .success(function (response) {
                                        
                            //            row.invoiceStatus = response;
                            //        })
                            //        .error(function () {
                            //        })
                            //}
                        };

                    }]);


})(angular.module('newApp'));