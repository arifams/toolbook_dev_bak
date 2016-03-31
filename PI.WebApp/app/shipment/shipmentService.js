'use strict';

(function (app) {

    app.factory('shipmentFactory', ['$http', '$routeParams', '$window', function ($http, $routeParams, $window) {

        return {
            calculateRates: calculateRates,
            loadAllDivisions: loadAllDivisions,
            loadAssignedDivisions: loadAssignedDivisions,
            loadAssignedCostCenters: loadAssignedCostCenters,
            submitShipment: submitShipment,
            loadAllCurrencies: loadAllCurrencies,
            getHashCodesForPaylane: getHashCodesForPaylane,
            loadAllShipments: loadAllShipments,
            loadShipmentInfo: loadShipmentInfo,
            loadShipmentStatusList: loadShipmentStatusList,
            loadAddressBookDetails:loadAddressBookDetails
        };

        //get paylane relted Details
        function getHashCodesForPaylane(paylane) {
            return $http.post(serverBaseUrl + '/api/shipments/GetHashForPayLane', paylane)
        }

        function loadAddressBookDetails(searchText)
        {
            return $http.get(serverBaseUrl + '/api/AddressBook/GetSerchedAddressList', {
                params: {
                    userId: $window.localStorage.getItem('userGuid'),
                    searchText: searchText
                }
            });
        }
        //loading language dropdown     
        function loadAllCurrencies() {

            return $http.get(serverBaseUrl + '/api/shipments/GetAllCurrencies');
        }

        // Implement the functions.
        function submitShipment(shipmentDetail) {

            return $http.post(serverBaseUrl + '/api/shipments/SubmitShipment', shipmentDetail);
        }

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

        function loadAssignedCostCenters(divisionid) {

           // GetCostCentersbyDivision
            return $http.get(serverBaseUrl + '/api/Company/GetCostCentersbyDivision', {
                params: {
                    divisionId: divisionid
                }
            });
        }

        function loadShipmentInfo(shipmentid) {
            return $http.get(serverBaseUrl + '/api/shipments/GetShipmentbyId', {
                params: {
                    shipmentId: shipmentid
                }
            });
        }

        
        function loadShipmentStatusList(shipmentid) {
            return $http.get(serverBaseUrl + '/api/shipments/GetShipmentStatusListbyId', {
                params: {
                    shipmentId: shipmentid
                }
            });
        }

        function loadAllShipments(status, startDate,endDate, number, source, destination)
        {
            return $http.get(serverBaseUrl + '/api/shipments/GetAllShipments', {               
                params: {
                userId: $window.localStorage.getItem('userGuid'),
                status: status,
                startDate: startDate,
                endDate:endDate,
                number: number,
                source: source,
                destination: destination
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