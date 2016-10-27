'use strict';

(function (app) {

    app.factory('adminFactory', ['$http', '$routeParams', '$window', function ($http, $routeParams, $window) {

        return {
            getAllComapnies: getAllComapnies,
            changeCompanyStatus: changeCompanyStatus,
            manageInvoicePaymentSetting: manageInvoicePaymentSetting,
            loadAllInvoices: loadAllInvoices,
            exportInvoiceDetailsReport: exportInvoiceDetailsReport,
            updateInvoiceStatus: updateInvoiceStatus,
            GetCustomerByCompanyId: GetCustomerByCompanyId,
            GetAuditTrailsForCustomer: GetAuditTrailsForCustomer
        };
        

        function loadAllInvoices(status, startDate, endDate, searchValue) {
           
            return $http.get(serverBaseUrl + '/api/Admin/GetAllInvoices', {
                params: {
                    status: status,
                    userId: $window.localStorage.getItem('userGuid'),
                    startDate: startDate,
                    endDate: endDate,
                    searchValue: searchValue
                }
            });
        }

        function exportInvoiceDetailsReport(status, startDate, endDate, searchValue) {
            

            return $http.get(serverBaseUrl + '/api/Admin/ExportInvoiceReport', {
                params: {
                    status: status,
                    userId: $window.localStorage.getItem('userGuid'),
                    startDate: startDate,
                    endDate: endDate,
                    searchValue: searchValue
                },
                responseType: 'arraybuffer'
            });
        }

        function getAllComapnies(pageList) {
            return $http.post(serverBaseUrl + '/api/Admin/GetAllComapnies', pageList);
        }
        
                
        function GetCustomerByCompanyId(companyId) {

            return $http.get(serverBaseUrl + '/api/Admin/GetCustomerByCompanyId', {
                params: {
                    companyid: companyId                  
                }
            });
        }


        function changeCompanyStatus(companyDetail) {
            
            return $http.post(serverBaseUrl + '/api/Admin/ChangeCompanyStatus', companyDetail);
        }


        function manageInvoicePaymentSetting(companyDetail) {

            return $http.post(serverBaseUrl + '/api/Admin/ManageInvoicePaymentSetting', companyDetail);
        }

        function updateInvoiceStatus(invoice) {

            return $http.post(serverBaseUrl + '/api/Admin/UpdateInvoiceStatus', invoice);
        }


        function GetAuditTrailsForCustomer() {

            return $http.get(serverBaseUrl + '/api/Admin/GetAuditTrailsForCustomer', {
                params: {
                    companyid: companyId
                }
            });
        }

    }]);

})(angular.module('newApp'));