
'use strict';


(function (app) {

    app.controller('addressBookImportCtrl',
       ['$location', '$window', '$scope','addressManagmentService', '$routeParams', '$log', '$sce', 'importAddressBookFactory', 'Upload', '$timeout', '$rootScope', 'ngDialog', '$controller',
    function ($location, $window, $scope, addressManagmentService, $routeParams, $log, $sce, importAddressBookFactory, Upload, $timeout, $rootScope, ngDialog, $controller) {

        debugger;
     
        $scope.csv = {};
        $scope.csv.accept = '.csv';
        $scope.errorExcelFormat = false;
        $scope.loading = false;

        $scope.validateExcelFormat = function (name) {
            debugger;
            var files = name;
            var fileExtension = name.split('.').pop();

            if (fileExtension != 'xlsx' && fileExtension != 'xls') {
                $scope.document = null;
                $scope.errorExcelFormat = true;
            } else {
                $scope.errorExcelFormat = false;
            }

            var file = $scope.csv;
        }

        $scope.Import = function () {
            $scope.loading = true;
            var importCollection = [];
            if ($scope.csv) {
                var addressList = $scope.csv.result;

                $.each(addressList, function (index, value) {
                    var address = { "csvContent": value[0] };
                    importCollection.push(address);
                });

                importAddressBookFactory.importAddressBook(importCollection).then(function successCallback(responce) {
                    $scope.loading = false;
                    $scope.addressCtrl.closeWindow();
                    debugger;
                    $scope.addressCtrl.csvImportResults(responce);
                    
                }, function errorCallback(response) {
                    $scope.loading = false;
                    //todo
                });;
            } else {
                $scope.loading = false;
                $scope.addressCtrl.showErrorCsv();
                //  alert("No file uploaded");
               
            }



        }
     
        $scope.uploadFile = function (file) {
            debugger;
            $scope.loading = true;
            file.upload = Upload.upload({
                url: serverBaseUrl + '/api/Shipments/UploadAddressBook',
                data: {
                    file: file,
                    userId: $window.localStorage.getItem('userGuid'),
                    documentType: "AddressBook",
                },
                params: {
                    userId: $window.localStorage.getItem('userGuid'),
                }
            });

            file.upload.then(function (response) {
                debugger;
                if (response.statusText = 'OK') {
                    $scope.loading = false;
                    $scope.addressCtrl.closeWindow();
                    $scope.addressCtrl.excelUploadSucces();
                   
                }

                $timeout(function () {
                    file.result = response.data;
                    deleteFile();
                });
            }, function (response) {
                if (response.status > 0)
                    debugger;
                $scope.loading = false;
                $scope.addressCtrl.closeWindow();
                $scope.addressCtrl.showError(response);
                   
            }, function (evt) {
                // Math.min is to fix IE which reports 200% sometimes
                file.progress = Math.min(100, parseInt(100.0 * evt.loaded / evt.total));
            });
        }
    }]);


})(angular.module('newApp'));