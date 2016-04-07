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
                           userId: $window.localStorage.getItem('userGuid'),
                           documentType: "SHIPMENT_LABEL",
                           referenceId: $scope.overviewShipCtrl.shipment.generalInformation.shipmentId,
                       },
                   });

                   file.upload.then(function (response) {
                       $timeout(function () {
                           file.result = response.data;
                           deleteFile();
                       });
                   }, function (response) {
                       if (response.status > 0)
                           $scope.errorMsg = response.status + ': ' + response.data;
                   }, function (evt) {
                       // Math.min is to fix IE which reports 200% sometimes
                       file.progress = Math.min(100, parseInt(100.0 * evt.loaded / evt.total));
                   });
        }


        
        akFileUploaderService.saveModel(fileUpload, "/Media/FileUpload")
            .then(function (data) {
            debugger;
            vm.imageFiles.push({
                fileAbsoluteURL: data[data.length - 1].FileAbsoluteURL,
                fileNameWithExtention: data[data.length - 1].FileNameWithExtention,
                categoryId: vm.model.categoryId,
                id: data[data.length - 1].Id,
                documentType: vm.model.documentType,
                contentDescription: vm.model.contentDescription,
                classification: vm.model.classification,
                searchAttributes: vm.model.searchAttributes,
                tag: vm.model.tag,
                tenantId: abp.session.tenantId
            });
            clearUplodFields();
        });


        $scope.deleteFile = function (file) {
            debugger;
            $http({
                url: serverBaseUrl + '/api/Shipments/upload',
                method: "POST",
                data: file
            }).success(function (result) {

                for (var i = 0; i < $scope.fileList.length; i++)
                    if ($scope.fileList[i].id === file.id) {
                        $scope.fileList.splice(i, 1);
                        break;
                    }
            }).error(function (result, status) {

            });
        }


        var clearUplodFields = function () {

            $scope.document = null,
            angular.element("input[name='document']").val(null);
        }      

      }
        ])
})(angular.module('newApp'));