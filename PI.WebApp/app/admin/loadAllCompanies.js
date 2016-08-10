﻿'use strict';

(function (app) {
    app.controller('loadCompaniesCtrl', ['$scope', '$location', '$window', 'adminFactory','$rootScope','ngDialog', '$controller',
                  function ($scope, $location, $window, adminFactory, $rootScope, ngDialog, $controller) {
                      var vm = this;
                      vm.status = 'All';
                      vm.itemsByPage = 25;
                      vm.rowCollection = [];

                      vm.closeWindow = function () {
                          ngDialog.close()
                      }


                      
                  vm.ViewCompany = function (company) {

                  adminFactory.GetCustomerByCompanyId(company.id).success(
                  function (responce) {
                  if (responce) {

                      ngDialog.open({
                          scope: $scope,
                          template: '/app/admin/CustomerDetailsView.html',
                          className: 'ngdialog-theme-plain custom-width-max',
                          controller: $controller('customerDetailsCtrl', {
                              $scope: $scope,
                              company: responce
                          })

                      });
                    
                    
                  }   else {
                      console.log("error occurd while retrieving Addresses");
                            
                         }
                     }).error(function (error) {
                    
                         console.log("error occurd while retrieving Addresses");
                     });

                        
                          


                      }


                      vm.searchComapnies = function () {

                          // Get values from view.
                          var userId = $window.localStorage.getItem('userGuid');
                          vm.userId = userId;
                          var searchText = (vm.searchText == undefined || vm.searchText == "") ? null : vm.searchText;
                          var status = (status == undefined || status == "" || status == "All") ? null : status;


                          adminFactory.getAllComapnies(searchText, status)
                              .then(function successCallback(responce) {

                                  vm.rowCollection = responce.data.content;

                              }, function errorCallback(response) {
                                  //todo
                              });
                      };


                      vm.searchComapnies = function (status) {

                          // Get values from view.
                          var userId = $window.localStorage.getItem('userGuid');
                          vm.userId = userId;
                          var searchText = (vm.searchText == undefined || vm.searchText == "") ? null : vm.searchText;
                          var status = (status == undefined || status == "" || status == "All") ? null : status;

                          adminFactory.getAllComapnies(searchText, status)
                              .then(function successCallback(responce) {

                                  vm.rowCollection = responce.data.content;
                              }, function errorCallback(response) {
                                  //todo
                              });
                      };

                      // Call search function in page load.
                      vm.searchComapnies();

                      vm.loadCompanyByStatus = function (status) {


                          vm.searchComapnies(status);
                      };

                      vm.changeCompanyStatus = function (row) {                         

                          var body = $("html, body");

                          body.stop().animate({ scrollTop: 0 }, '500', 'swing', function () {
                          });

                          $('#panel-notif').noty({
                              text: '<div class="alert alert-success media fade in"><p>' + $rootScope.translate('Do you want to save the changes?') + '?</p></div>',
                              buttons: [
                                      {
                                          addClass: 'btn btn-primary', text: $rootScope.translate('Ok'), onClick: function ($noty) {

                                              $noty.close();
                                              adminFactory.changeCompanyStatus({ Id: row.id })
                                             .success(function (response) { 

                                                 row.status = response;
                                                 //vm.buttonName = (response) ? 'Deactivate' : 'Activate';

                                                 $('#panel-notif').noty({
                                                     text: '<div class="alert alert-success media fade in"><p>' + $rootScope.translate('Customer status changed succesfully') + '</p></div>',
                                                     layout: 'bottom-right',
                                                     theme: 'made',
                                                     animation: {
                                                         open: 'animated bounceInLeft',
                                                         close: 'animated bounceOutLeft'
                                                     },
                                                     timeout: 3000,
                                                 });
                                                })
                                                .error(function () {
                                               
                                                    body.stop().animate({ scrollTop: 0 }, '500', 'swing', function () {
                                                    });
                                               
                                                    $('#panel-notif').noty({
                                                        text: '<div class="alert alert-warning media fade in"><p>' + $rootScope.translate('Server Error Occured') + '</p></div>',
                                                        layout: 'bottom-right',
                                                        theme: 'made',
                                                        animation: {
                                                            open: 'animated bounceInLeft',
                                                            close: 'animated bounceOutLeft'
                                                        },
                                                        timeout: 3000,
                                                    });
                                               
                                                })
                                                            
                                          }
                                      },
                                      {
                                          addClass: 'btn btn-danger', text: $rootScope.translate('Cancel'), onClick: function ($noty) {
                                              $noty.close();
                                              return;
                                          }
                                      }
                              ],
                              layout: 'bottom-right',
                              theme: 'made',
                              animation: {
                                  open: 'animated bounceInLeft',
                                  close: 'animated bounceOutLeft'
                              },
                              timeout: 3000,
                          });




                      };

                      vm.changeInvoiceSetting = function (row) {

                          
                          var body = $("html, body");

                          body.stop().animate({ scrollTop: 0 }, '500', 'swing', function () {
                          });

                          $('#panel-notif').noty({
                              text: '<div class="alert alert-success media fade in"><p>' + $rootScope.translate('Are you sure you want to update invoice status?') + '?</p></div>',
                              buttons: [
                                      {
                                          addClass: 'btn btn-primary', text: $rootScope.translate('Ok'), onClick: function ($noty) {
                                              $noty.close();
                                              adminFactory.manageInvoicePaymentSetting({ Id: row.id })
                                             .success(function (response) {
                                                 debugger;
                                                 row.isInvoiceEnabled = response;
                                                 $('#panel-notif').noty({
                                                     text: '<div class="alert alert-success media fade in"><p>' + $rootScope.translate('Invoice payment status updated succesfully') + '</p></div>',
                                                     layout: 'bottom-right',
                                                     theme: 'made',
                                                     animation: {
                                                         open: 'animated bounceInLeft',
                                                         close: 'animated bounceOutLeft'
                                                     },
                                                     timeout: 3000,
                                                 });


                                             })
                                            .error(function () {

                                                body.stop().animate({ scrollTop: 0 }, '500', 'swing', function () {
                                                });

                                                $('#panel-notif').noty({
                                                    text: '<div class="alert alert-warning media fade in"><p>' + $rootScope.translate('Server Error Occured') + '</p></div>',
                                                    layout: 'bottom-right',
                                                    theme: 'made',
                                                    animation: {
                                                        open: 'animated bounceInLeft',
                                                        close: 'animated bounceOutLeft'
                                                    },
                                                    timeout: 3000,
                                                });
                                           })

                                          }
                                      },
                                      {
                                          addClass: 'btn btn-danger', text: $rootScope.translate('Cancel'), onClick: function ($noty) {
                                              $noty.close();
                                              return;
                                          }
                                      }
                              ],
                              layout: 'bottom-right',
                              theme: 'made',
                              animation: {
                                  open: 'animated bounceInLeft',
                                  close: 'animated bounceOutLeft'
                              },
                              timeout: 3000,
                          });


                      };

                  }]);

})(angular.module('newApp'));
