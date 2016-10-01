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
           
            return $http.get(serverBaseUrl + '/api/admin/GetAllInvoices', {
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
            

            return $http.get(serverBaseUrl + '/api/admin/ExportInvoiceReport', {
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


        function getAllComapnies(searchText, status) {
            
            return $http.get(serverBaseUrl + '/api/Company/GetAllComapnies', {
                params: {
                    status: status,
                    searchText: searchText,
                }
            });
        }
        
                
        function GetCustomerByCompanyId(companyId) {

            return $http.get(serverBaseUrl + '/api/Customer/GetCustomerByCompanyId', {
                params: {
                    companyid: companyId                  
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


        function GetAuditTrailsForCustomer() {

            return $http.get(serverBaseUrl + '/api/Admin/GetAuditTrailsForCustomer', {
                params: {
                    companyid: companyId
                }
            });
        }

    }]);

})(angular.module('newApp'));