﻿'use strict';

(function (app) {

    app.factory('adminFactory', ['$http', '$routeParams', '$window', function ($http, $routeParams, $window) {

        return {
            getAllComapnies: getAllComapnies,
            changeCompanyStatus: changeCompanyStatus,
            manageInvoicePaymentSetting: manageInvoicePaymentSetting,
            loadAllInvoices: loadAllInvoices,
            exportInvoiceDetailsReport: exportInvoiceDetailsReport,
            updateInvoiceStatus: updateInvoiceStatus
        };




        function loadAllInvoices(status, startDate, endDate, shipmentnumber, businessowner, invoicenumber) {
           
            return $http.get(serverBaseUrl + '/api/admin/GetAllInvoices', {
                params: {
                    status: status,
                    userId: $window.localStorage.getItem('userGuid'),
                    startDate: startDate,
                    endDate: endDate,
                    shipmentnumber: shipmentnumber,
                    businessowner: businessowner,
                    invoicenumber: invoicenumber
                }
            });
        }

        function exportInvoiceDetailsReport(invoiceList) {
            return $http({
                url: serverBaseUrl + '/api/admin/ExportInvoiceReport',
                data: invoiceList,
                method: "POST",
                responseType: 'arraybuffer'
            });
        }


        function getAllComapnies(searchText, status) {
            
            return $http.get(serverBaseUrl + '/api/Company/GetAllComapnies', {
                params: {
                    status: status,
                    searchText: searchText,
                }
            });
        }
        

        function changeCompanyStatus(companyDetail) {
            
            return $http.post(serverBaseUrl + '/api/Company/ChangeCompanyStatus', companyDetail);
        }


        function manageInvoicePaymentSetting(companyDetail) {

            return $http.post(serverBaseUrl + '/api/Admin/ManageInvoicePaymentSetting', companyDetail);
        }

        function updateInvoiceStatus(invoice) {

            return $http.post(serverBaseUrl + '/api/Admin/UpdateInvoiceStatus', invoice);
        }

    }]);

})(angular.module('newApp'));