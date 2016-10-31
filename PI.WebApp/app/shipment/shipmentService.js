'use strict';

(function (app) {

    app.factory('shipmentFactory', ['$http', '$routeParams', '$window', function ($http, $routeParams, $window) {

        var userId = '';
        var createdBy = '';


        function setLoginUserID() {
            debugger;
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
            getSquareApplicationId: getSquareApplicationId,
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
            loadAllShipmentsForAdmin: loadAllShipmentsForAdmin,
            deleteShipmentbyAdmin: deleteShipmentbyAdmin,
            UpdateshipmentStatusManually: UpdateshipmentStatusManually,
            GetBusinessOwneridbyCompanyId: GetBusinessOwneridbyCompanyId,
            getShipmentForCompanyAndSyncWithSIS: GetShipmentForCompanyAndSyncWithSIS,
            toggleFavourite: toggleFavourite,
            searchShipmentsById: searchShipmentsById,
            GetshipmentByShipmentCodeForAirwayBill: GetshipmentByShipmentCodeForAirwayBill,
            loadDefaultCostCenterId: loadDefaultCostCenterId,
            getFilteredShipmentsExcel: getFilteredShipmentsExcel,
            PaymentCharge: PaymentCharge,
            loadAllShipmentsForAdminExcelExport: loadAllShipmentsForAdminExcelExport,
            PaymentCharge: PaymentCharge,
            GetAllShipmentCounts: GetAllShipmentCounts,
            saveAwbNo: saveAwbNo,
            UpdateShipmentReference: UpdateShipmentReference
        };

        function saveAwbNo(awbDto) {
            return $http.post(serverBaseUrl + '/api/shipments/UpdateTrackingNo', awbDto);
        }

        function PaymentCharge(paymentDto) {
            return $http.post(serverBaseUrl + '/api/shipments/PaymentCharge', paymentDto);
        }

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
        function getSquareApplicationId(paylane) {
            return $http.get(serverBaseUrl + '/api/shipments/GetSquareApplicationId')
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

        function UpdateShipmentReference(shipmentDetail) {

            return $http.post(serverBaseUrl + '/api/shipments/UpdateShipmentReference', shipmentDetail);
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


        function loadDefaultCostCenterId(divisionid) {

            // GetCostCentersbyDivision
            return $http.get(serverBaseUrl + '/api/Company/GetDefaultCostCentersbyDivision', {
                params: {
                    divisionId: divisionid
                }
            });
        }

        function loadShipmentInfo(shipmentcode, shipmentid) {
            return $http.get(serverBaseUrl + '/api/shipments/GetShipmentbyId', {
                params: {
                    shipmentCode: shipmentcode,
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
            return $http.get(serverBaseUrl + '/api/Admin/GetAllComapniesForAdminSearch', {
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

        
        function loadAllShipmentsForAdmin(status, startDate, endDate, searchValue, startRecord, pageRecord) {

            return $http.get(serverBaseUrl + '/api/Admin/loadAllShipmentsForAdmin', {
                params: {
                    status: status,
                    startDate: startDate,
                    endDate: endDate,
                    searchValue: searchValue,
                    currentPage: startRecord,
                    pageSize: pageRecord
                }
            });
        }

        
        function loadAllShipmentsForAdminExcelExport(companyId, status, startDate, endDate, number, source, destination) {
            debugger;
            return $http.get(serverBaseUrl + '/api/Admin/loadAllShipmentsForAdminExcelExport', {
                params: {
                    companyId: companyId,
                    status: status,
                    startDate: startDate,
                    endDate: endDate,
                    number: number,
                    source: source,
                    destination: destination
                },
                responseType: 'arraybuffer'
            });
        }


        function getFilteredShipmentsExcel(pagedList) {
             
            setLoginUserID();
            pagedList.userId = userId;

            return $http.get(serverBaseUrl + '/api/shipments/GetFilteredShipmentsExcel', {
                params: {
                    pagedList: pagedList
                },
                responseType: 'arraybuffer'
            });
        }


        function loadAllShipments(pagedList) {
            setLoginUserID();
            pagedList.userId = userId;
            return $http.post(serverBaseUrl + '/api/shipments/GetAllShipments', pagedList);
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

        function GetshipmentByShipmentCodeForAirwayBill(shipmentcode) {
            return $http.get(serverBaseUrl + '/api/shipments/GetshipmentByShipmentCodeForAirwayBill', {
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

        function searchShipmentsById(number) {

            return $http.get(serverBaseUrl + '/api/shipments/SearchShipmentsById', {
                params: {
                    number: number
                }
            });
        }

        function GetAllShipmentCounts() {
            return $http.get(serverBaseUrl + '/api/shipments/GetShipmentStatusCounts', {
                params: {
                    userId: null
                }
            })
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