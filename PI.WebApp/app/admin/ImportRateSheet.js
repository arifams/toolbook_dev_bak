﻿'use strict';

(function (app) {

    app.controller('loadRateSheetCtrl', ['$route', '$scope', '$location', '$window', 'Upload', '$timeout','$rootScope',
        function ($route, $scope, $location, $window, Upload, $timeout, $rootScope) {
            var vm = this;

            vm.uploadRateSheet = function (file) {
                file.upload = Upload.upload({
                    url: serverBaseUrl + '/api/Admin/UploadRateSheet',
                    data: {
                        file: file,
                        userId: $window.localStorage.getItem('userGuid'),
                        documentType: "RATE_SHEET",
                    },
                    params: {
                        userId: $window.localStorage.getItem('userGuid'),
                    }
                });

                file.upload.then(function (response) {

                    var body = $("html, body");
                    if (response.statusText = 'OK') {

                        body.stop().animate({ scrollTop: 0 }, '500', 'swing', function () {
                        });

                        $('#panel-notif').noty({
                            text: '<div class="alert alert-success media fade in"><p> ' + $rootScope.translate('Rates records added successfully') + '.</p></div>',
                            buttons: [
                                    {
                                        addClass: 'btn btn-primary', text: $rootScope.translate('Ok'), onClick: function ($noty) {
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
                            text: '<div class="alert alert-error media fade in"><p> ' + $rootScope.translate('Error occured') + '.</p></div>',
                            buttons: [
                                    {
                                        addClass: 'btn btn-primary', text: $rootScope.translate('Ok'), onClick: function ($noty) {
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