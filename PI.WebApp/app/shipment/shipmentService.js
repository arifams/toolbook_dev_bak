'use strict';

(function (app) {

    app.factory('shipmentFactory', ['$http', '$routeParams', '$window', function ($http, $routeParams, $window) {

        return {
            calculateRates: calculateRates,
            loadAllDivisions: loadAllDivisions,
            loadAssignedDivisions: loadAssignedDivisions
        };

       

        // Implement the functions.
        function calculateRates(shipmentDetail) {

            return $http.post(serverBaseUrl + '/api/shipments/GetRatesforShipment', shipmentDetail);
        }
       
        //get all divisions for business owners
        function loadAllDivisions() {
         
             return $http.get(serverBaseUrl + '/api/Company/GetAllDivisions', {
                    params: {
                        userId: $window.localStorage.getItem('userGuid')
                    }
             });
            
        }

        //get the assigned divisions for other users
        function loadAssignedDivisions() {

            return $http.get(serverBaseUrl + '/api/Company/GetAssignedDivisions', {
                params: {
                    userId: $window.localStorage.getItem('userGuid')
                }
            });

        }


    }]);
    
    //app.factory('calculateRatesforShipment', function ($http) {
    //    return {
    //        calculateRates: function (shipmentDetail) {

    //            return $http.post(serverBaseUrl + '/api/shipments/GetRatesforShipment', shipmentDetail);
    //        }
    //    }

    //});

  

})(angular.module('newApp'));