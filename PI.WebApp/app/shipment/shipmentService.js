'use strict';

(function (app) {

    app.factory('shipmentFactory', ['$http', '$routeParams', '$window', function ($http, $routeParams, $window) {

        return {
            
        }

        // Implement the functions.

       

    }]);
    
    app.factory('calculateRatesforShipment', function ($http) {
        return {
            calculateRates: function (shipmentDetail) {

                return $http.post(serverBaseUrl + '/api/shipments/GetRatesforShipment', shipmentDetail);
            }
        }

    });

  

})(angular.module('newApp'));