﻿'user strict';
(function (app) {

    app.controller('shipmentDocumentCtrl',
        ['$location', '$window', 'shipmentFactory', 'akFileUploaderService', '$http', '$timeout', 'Upload',
    function ($location, $window, shipmentFactory, akFileUploaderService, $http, $timeout, Upload) {
               var vm = this;

               vm.upload = [];
               vm.fileUploadObj = { testString1: "Test string 1", testString2: "Test string 2" };

               vm.onFileSelect = function ($files) {
                   debugger;
                   //$files: an array of files selected, each file has name, size, and type.
                   for (var i = 0; i < $files.length; i++) {
                       var $file = $files[i];
                       (function (index) {
                           vm.upload[index] = Upload.upload({
                               url: "/api/shipments/UploadDocumentsForShipment", // webapi url
                               method: "POST",
                               data: { fileUploadObj: $scope.fileUploadObj },
                               file: $file
                           }).progress(function (evt) {
                               // get upload percentage
                               console.log('percent: ' + parseInt(100.0 * evt.loaded / evt.total));
                           }).success(function (data, status, headers, config) {
                               // file is uploaded successfully
                               console.log(data);
                           }).error(function (data, status, headers, config) {
                               // file failed to upload
                               console.log(data);
                           });
                       })(i);
                   }
               }

               vm.abortUpload = function (index) {
                   vm.upload[index].abort();
               }


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