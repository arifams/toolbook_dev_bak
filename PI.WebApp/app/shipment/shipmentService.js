'use strict';

(function (app) {

    app.factory('shipmentFactory', ['$http', '$routeParams', '$window', function ($http, $routeParams, $window) {

        var userId = '';
        var createdBy = '';


        function setLoginUserID() {
            
            if ($window.localStorage.getItem('userRole') == 'Admin') {
                
                createdBy = $window.localStorage.getItem('userGuid');
                userId = $window.localStorage.getItem('businessOwnerId');
            } else {
                createdBy = $window.localStorage.getItem('userGuid');
                userId = $window.localStorage.getItem('userGuid');
            }
        }
        

        return {
            calculateRates: calculateRates,
            loadAllDivisions: loadAllDivisions,
            loadAssignedDivisions: loadAssignedDivisions,
            loadAssignedCostCenters: loadAssignedCostCenters,
            saveShipment: saveShipment,
            loadAllCurrencies: loadAllCurrencies,
            getHashCodesForPaylane: getHashCodesForPaylane,
            loadAllShipments: loadAllShipments,
            loadShipmentInfo: loadShipmentInfo,
            loadShipmentStatusList: loadShipmentStatusList,
            loadAddressBookDetails: loadAddressBookDetails,
            sendShipmentDetails: sendShipmentDetails,
            getLocationHistory: getLocationHistory,
            getTeackAndTraceDetails: getTeackAndTraceDetails,
            loadAllPendingShipments: loadAllPendingShipments,
            getshipmentByShipmentCodeForInvoice: getshipmentByShipmentCodeForInvoice,
            getAvailableFilesForShipment: getAvailableFilesForShipment,
            deleteFile: deleteFile,
            saveCommercialInvoice: saveCommercialInvoice,
            getProfileInfo: getProfileInfo,
            getAllshipmentsForManifest: getAllshipmentsForManifest,
            requestForQuote: requestForQuote,
            deleteShipment: deleteShipment,
            loadAllcompanies: loadAllcompanies,
            loadAllshipmentsForCompany: loadAllshipmentsForCompany,
            loadAllShipmentsFromCompanyAndSearch: loadAllShipmentsFromCompanyAndSearch,
            deleteShipmentbyAdmin: deleteShipmentbyAdmin,
            UpdateshipmentStatusManually: UpdateshipmentStatusManually,
            GetBusinessOwneridbyCompanyId: GetBusinessOwneridbyCompanyId,
            getShipmentForCompanyAndSyncWithSIS: GetShipmentForCompanyAndSyncWithSIS,
            toggleFavourite: toggleFavourite,
        };

        function getProfileInfo() {
            
            setLoginUserID();
            return $http.get(serverBaseUrl + '/api/profile/GetProfileForShipment', {
                params: {
                    // userId: $window.localStorage.getItem('userGuid'),
                    userId: userId,
                }
            });
        }


        function getAllshipmentsForManifest(date, carreer, reference) {
            setLoginUserID();
            return $http.get(serverBaseUrl + '/api/shipments/GetAllshipmentsForManifest', {
                params: {
                   // userId: $window.localStorage.getItem('userGuid'),
                    userId: userId,
                    createdDate: date,
                    carreer: carreer,
                    reference: reference
                }
            });
        }

        function getTeackAndTraceDetails(carrier, trackingNo) {
            return $http.get(serverBaseUrl + '/api/shipments/GetTrackAndTraceInfo', {
                params: {
                    career: carrier,
                    trackingNumber: trackingNo
                        }
             });
            
        }
        //get paylane relted Details
        function getHashCodesForPaylane(paylane) {
            return $http.post(serverBaseUrl + '/api/shipments/GetHashForPayLane', paylane)
        }

        function loadAddressBookDetails(searchText) {
            setLoginUserID();
            return $http.get(serverBaseUrl + '/api/AddressBook/GetSerchedAddressList', {
                params: {
                    // userId: $window.localStorage.getItem('userGuid'),
                    userId: userId,
                    searchText: searchText
                }
            });
        }
        //loading language dropdown     
        function loadAllCurrencies() {

            return $http.get(serverBaseUrl + '/api/shipments/GetAllCurrencies');
        }

        function saveShipment(shipmentDetail) {

            return $http.post(serverBaseUrl + '/api/shipments/SaveShipment', shipmentDetail);
        }

        function UpdateshipmentStatusManually(shipmentDetail) {

            return $http.post(serverBaseUrl + '/api/shipments/UpdateshipmentStatusManually', shipmentDetail);
        }        

        function getLocationHistory(shipmentDetail) {

            return $http.post(serverBaseUrl + '/api/shipments/GetLocationHistoryforShipment', shipmentDetail);
        }
                
        function calculateRates(shipmentDetail) {

            return $http.post(serverBaseUrl + '/api/shipments/GetRatesforShipment', shipmentDetail);
        }
       
        //get all divisions for business owners
        function loadAllDivisions() {
            setLoginUserID();
             return $http.get(serverBaseUrl + '/api/Company/GetAllDivisions', {
                    params: {
                        // userId: $window.localStorage.getItem('userGuid')
                        userId: userId,
                    }
             });
            
        }

        //get the assigned divisions for other users
        function loadAssignedDivisions() {
            setLoginUserID();
            return $http.get(serverBaseUrl + '/api/Company/GetAssignedDivisions', {
                params: {
                    //userId: $window.localStorage.getItem('userGuid')
                    userId: userId,
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
        function sendShipmentDetails(shipmentId) {

            return $http.post(serverBaseUrl + '/api/shipments/SendShipmentDetails', shipmentId);
        }

        
        function loadShipmentStatusList(shipmentid) {
            return $http.get(serverBaseUrl + '/api/shipments/GetShipmentStatusListbyId', {
                params: {
                    shipmentId: shipmentid
                }
            });
        }
      
        function loadAllcompanies(searchtext) {
            return $http.get(serverBaseUrl + '/api/Company/GetAllComapniesForAdminSearch', {
                params: {
                    searchText: searchtext
                }
            });

        }


        function loadAllshipmentsForCompany(companyID) {
            
            return $http.get(serverBaseUrl + '/api/shipments/GetAllShipmentByCompanyId', {
                params: {
                    companyId: companyID
                }
            });
        }


        function GetBusinessOwneridbyCompanyId(companyId) {

            return $http.get(serverBaseUrl + '/api/shipments/GetBusinessOwneridbyCompanyId', {
                params: {
                    companyId: companyId
                }
            });
        }


        function GetShipmentForCompanyAndSyncWithSIS(companyId) {

            return $http.get(serverBaseUrl + '/api/shipments/GetShipmentForCompanyAndSyncWithSIS', {
                params: {
                    companyId: companyId
                }
            });
        }


        function loadAllShipmentsFromCompanyAndSearch(companyId, status, startDate, endDate, number, source, destination) {
            
            return $http.get(serverBaseUrl + '/api/shipments/loadAllShipmentsFromCompanyAndSearch', {
                params: {
                    companyId: companyId,
                    status: status,
                    startDate: startDate,
                    endDate: endDate,
                    number: number,
                    source: source,
                    destination: destination
                }
            });
        }


        function loadAllShipments(status, startDate, endDate, number, source, destination, viaDashboard) {
            setLoginUserID();
            return $http.get(serverBaseUrl + '/api/shipments/GetAllShipments', {               
                params: {
                 // userId: $window.localStorage.getItem('userGuid'),
                userId: userId,
                status: status,
                startDate: startDate,
                    endDate: endDate,
                number: number,
                source: source,
                destination: destination,
                viaDashboard: viaDashboard
            }
            });
        }

        function loadAllPendingShipments(startDate, endDate, number) {
            setLoginUserID();
            return $http.get(serverBaseUrl + '/api/shipments/GetAllPendingShipments', {
                params: {
                   // userId: $window.localStorage.getItem('userGuid'),
                    userId: userId,
                    startDate: startDate,
                    endDate: endDate,
                    number: number,                   
                }
            });
        }

        function getAvailableFilesForShipment(shipmentId, userId) {
            setLoginUserID();
            return $http.get(serverBaseUrl + '/api/shipments/GetAvailableFilesForShipment', {
                params: {
                    userId: userId,
                    shipmentCode: shipmentId
                }
            });
        }

        function getshipmentByShipmentCodeForInvoice(shipmentcode) {
            return $http.get(serverBaseUrl + '/api/shipments/GetshipmentByShipmentCodeForInvoice', {
                params: {
                    shipmentCode: shipmentcode
                }
            });
        }

        function deleteFile(fileDetails) {
            return $http.post(serverBaseUrl + '/api/shipments/DeleteFile', fileDetails)
        }

        function saveCommercialInvoice(shipmentDetail) {

            return $http.post(serverBaseUrl + '/api/shipments/SaveCommercialInvoice', shipmentDetail);
        }

        function requestForQuote(shipmentDetail) {

            return $http.post(serverBaseUrl + '/api/shipments/RequestForQuote', shipmentDetail);
        }

        
        function deleteShipment(shipmentDetail) {
            
            var dataToPass = {
                trackingNumber: shipmentDetail.generalInformation.trackingNumber != undefined ? shipmentDetail.generalInformation.trackingNumber : "",
                shipmentCode: shipmentDetail.generalInformation.shipmentCode != undefined ? shipmentDetail.generalInformation.shipmentCode : "",
                carrierName: shipmentDetail.carrierInformation.carrierName,
                isAdmin: false,
                shipmentId: shipmentDetail.generalInformation.shipmentId
            };
            
            return $http({
                url: serverBaseUrl + '/api/shipments/DeleteShipment',
                method: "GET",
                params: dataToPass
            })
        }


        function deleteShipmentbyAdmin(shipmentDetail) {

            var dataToPass = {
                trackingNumber: shipmentDetail.generalInformation.trackingNumber != undefined ? shipmentDetail.generalInformation.trackingNumber : "",
                shipmentCode: shipmentDetail.generalInformation.shipmentCode != undefined ? shipmentDetail.generalInformation.shipmentCode : "",
                carrierName: shipmentDetail.carrierInformation.carrierName,
                isAdmin: true,
                shipmentId: shipmentDetail.generalInformation.shipmentId
            };
            return $http({
                url: serverBaseUrl + '/api/shipments/DeleteShipment',
                method: "GET",
                params: dataToPass
            })
        }
        

        function toggleFavourite(shipment) {
            return $http.post(serverBaseUrl + '/api/shipments/ToggleShipmentFavourites', shipment)
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