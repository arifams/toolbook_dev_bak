'user strict';
(function (app) {

    app.controller('shipmentDocumentCtrl',
        ['$location', '$window', 'shipmentFactory', 'akFileUploaderService',
           function ($location, $window, shipmentFactory, akFileUploaderService) {
               var vm = this;

               vm.fileUpload = function () {
                   debugger;
                   var fileUpload = {
                       ReferenceId:2, 
                       Attachment: vm.model.document
                   };

                   akFileUploaderService.saveModel(fileUpload, serverBaseUrl + '/api/shipments/UploadDocumentsForShipment')
                   .then(function (data) {
                   })
               };

           }
        ])
})(angular.module('newApp'));