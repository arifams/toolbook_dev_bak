'user strict';
(function (app) {

    app.controller('shipmentDocumentCtrl',
        ['$window', '$scope' ,'Upload', '$http', '$timeout',
    function ( $window, $scope, Upload, $http, $timeout) {

        $scope.uploadFile = function (file) {
            debugger;
                   file.upload = Upload.upload({
                       url: serverBaseUrl + '/api/Shipments/upload',
                       data: {
                           file: file,
                           referenceId: $scope.overviewShipCtrl.shipment.generalInformation.shipmentId,
                           userId: $window.localStorage.getItem('userGuid')
                       },
                   });

                   file.upload.then(function (response) {
                       $timeout(function () {
                           file.result = response.data;
                       });
                   }, function (response) {
                       if (response.status > 0)
                           $scope.errorMsg = response.status + ': ' + response.data;
                   }, function (evt) {
                       // Math.min is to fix IE which reports 200% sometimes
                       file.progress = Math.min(100, parseInt(100.0 * evt.loaded / evt.total));
                   });
               }

           }
        ])
})(angular.module('newApp'));