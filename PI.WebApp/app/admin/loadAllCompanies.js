'use strict';

(function (app) {
    app.controller('loadCompaniesCtrl', ['$scope', '$location', '$window', 'adminFactory','$rootScope',
                  function ($scope, $location, $window, adminFactory, $rootScope) {
                      var vm = this;
                      vm.status = 'All';
                      vm.itemsByPage = 25;
                      vm.rowCollection = [];


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
                              text: '<div class="alert alert-success media fade in"><p>' + $rootScope.translate('Are you sure you need to change the status?') + '?</p></div>',
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
                              text: '<div class="alert alert-success media fade in"><p>' + $rootScope.translate('Are you sure you need to update invoice payment status?') + '?</p></div>',
                              buttons: [
                                      {
                                          addClass: 'btn btn-primary', text: $rootScope.translate('Ok'), onClick: function ($noty) {
                                              $noty.close();
                                              adminFactory.manageInvoicePaymentSetting({ Id: row.id })
                                             .success(function (response) {
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
