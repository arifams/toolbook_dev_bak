'use strict';

(function (app) {

    app.controller('uploadInvoiceCtrl', ['$route', '$scope', '$location', '$window', 'Upload', '$timeout', 'fromMethod','invoiceId',
    function ($route, $scope, $location, $window, Upload, $timeout, fromMethod,invoiceId) {
            var vm = this;
            $scope.from = fromMethod;
            debugger;
            $scope.uploadInvoice = function (file) {
                debugger;
                if (fromMethod == 'creditNote') {

                    file.upload = Upload.upload({
                        url: serverBaseUrl + '/api/Admin/UploadInvoice',
                        data: {
                            userId: $window.localStorage.getItem('userGuid'),
                            documentType: "CREDIT_NOTE",
                            invoiceId : invoiceId,
                            file: file,
                        },
                        params: {
                            userId: $window.localStorage.getItem('userGuid'),
                        }
                    });
                }
                else {
                    debugger;
                    file.upload = Upload.upload({
                        url: serverBaseUrl + '/api/Admin/UploadInvoice',
                        data: {
                            userId: $window.localStorage.getItem('userGuid'),
                            documentType: "INVOICE",
                            file: file,
                        },
                        params: {
                            userId: $window.localStorage.getItem('userGuid'),
                        }
                    });
                }


                file.upload.then(function (response) {

                    debugger;
                    var body = $("html, body");
                    if (response.statusText = 'OK') {

                        $scope.invoiceCtrl.closeWindow();

                        body.stop().animate({ scrollTop: 0 }, '500', 'swing', function () {
                        });

                        $('#panel-notif').noty({
                            text: '<div class="alert alert-success media fade in"><p>' + ' File(s) added successfully.' + '</p></div>',
                            buttons: [
                                    {
                                        addClass: 'btn btn-primary', text: 'Ok', onClick: function ($noty) {                                     
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

                    $timeout(function () {
                        file.result = response.data;
                        console.log('$timeout ok');
                        deleteFile();
                    });
                }, function (response) {
                    console.log('Response of rate sheet: ');
                    console.log(response);
                    if (response.status > 0) {
                        var body = $("html, body");
                        vm.errorMsg = response.status + ': ' + response.data;
                        body.stop().animate({ scrollTop: 0 }, '500', 'swing', function () {
                        });

                        $('#panel-notif').noty({
                            text: '<div class="alert alert-error media fade in"><p>' + ' Error occured.' + '</p></div>',
                            buttons: [
                                    {
                                        addClass: 'btn btn-primary', text: 'Ok', onClick: function ($noty) {
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
                }, function (evt) {
                    // Math.min is to fix IE which reports 200% sometimes
                    file.progress = Math.min(100, parseInt(100.0 * evt.loaded / evt.total));
                });
            }
        }

    ])
})(angular.module('newApp'))