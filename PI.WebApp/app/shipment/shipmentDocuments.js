'user strict';
(function (app) {

    app.controller('shipmentDocumentCtrl',
        ['$window', '$scope', '$rootScope', 'Upload', '$http', '$timeout', 'shipmentFactory',
        function ($window, $scope, $rootScope,Upload, $http, $timeout, shipmentFactory) {


            var userId = $window.localStorage.getItem('userGuid');
            

            $scope.uploadFile = function (file) {
                
                file.upload = Upload.upload({
                    url: serverBaseUrl + '/api/Shipments/upload',
                    data: {
                        file: file,
                        userId: userId,
                        documentType: "SHIPMENT_DOCUMENTS",
                        referenceId: $scope.shipmentId
                    },
                });

                file.upload.then(function (response) {
                    $timeout(function () {
                        
                        file.result = response.data;
                        //deleteFile();
                        $scope.document = null;
                         $scope.loadAllUploadedFiles();
                    });
                }, function (response) {
                    if (response.status > 0)
                        $scope.errorMsg = response.status + ': ' + response.data;
                }, function (evt) {
                    // Math.min is to fix IE which reports 200% sometimes
                    file.progress = Math.min(100, parseInt(100.0 * evt.loaded / evt.total));
                });
            }

        
            $scope.deleteFile = function (file) {
                
                shipmentFactory.deleteFile(file)
                .success(function (result) {

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


            $scope.loadAllUploadedFiles = function () {
                
                $scope.shipmentId = $scope.overviewShipCtrl.shipmentCode;
                shipmentFactory.getAvailableFilesForShipment($scope.shipmentId, userId)
                                .success(
                                        function (responce) {
                                            
                                            $scope.fileList = [];
                                            $scope.fileList = responce;
                                        }).error(function (error) {
                                            console.log("error occurd while retrieving shiment documents");
                                        });
            }

            $scope.loadAllUploadedFiles();

        }
        ])
})(angular.module('newApp'));