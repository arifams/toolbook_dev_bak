'use strict';

(function (app) {

    app.factory('adminFactory', ['$http', '$routeParams', '$window', function ($http, $routeParams, $window) {

        return {
            getAllComapnies: getAllComapnies,
            changeCompanyStatus: changeCompanyStatus,
            manageInvoicePaymentSetting: manageInvoicePaymentSetting,
            loadAllInvoices:loadAllInvoices
        };




        function loadAllInvoices(status, startDate, endDate, shipmentnumber, businessowner, invoicenumber) {
           
            return $http.get(serverBaseUrl + '/api/admin/GetAllInvoices', {
                params: {
                    
                   // userId: userId,
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

    }]);

})(angular.module('newApp'));