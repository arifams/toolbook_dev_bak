
'use strict';


(function (app) {   

    app.controller('printManifestCtrl',
       ['$location', '$window', 'shipmentFactory',
           function ($location, $window, shipmentFactory) {
               var vm = this;
               var SISURL = 'http://book.12send.com/taleus/admin-manifest.asp?'
               var userid = 'user@Mitrai.com';
               var password = 'Mitrai462';
               vm.isPackageEnable = "false";
               vm.isSubmit = false;
              
               vm.GenerateManifest = function () {
                   vm.isSubmit = true;
                   var dateArray = vm.Date.split('-');
                   var  details='N';
                   var career=vm.carrier;

                   if (dateArray != null) {
                       var date = dateArray[0];
                       var month = dateArray[1];
                       var year = dateArray[2];
                   }                  

                   if (vm.isPackageEnable==true) {
                       details='Y';
                   }
                   var completeURL = SISURL + 'select_date_day=' + date + '&userid=' + userid + '&password=' + password + '&select_date_month=' + month + '&select_date_year=' + year + '&select_date=&select_date_time=00%3A00.00&select_details=' + details + '&total_copies=2&select_courier=' + career + '&submit=+Show+manifest+';
                   window.open(completeURL);
               }

           }]);


})(angular.module('newApp'));