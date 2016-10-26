'use strict';

(function (app) {
    app.controller('loadCompaniesCtrl', ['$scope', '$location', '$window', 'adminFactory', '$rootScope', 'ngDialog', '$controller', 'customBuilderFactory','userManagementFactory',
                  function ($scope, $location, $window, adminFactory, $rootScope, ngDialog, $controller, customBuilderFactory, userManagementFactory) {
                      var vm = this;
                      vm.status = 'All';
                      vm.itemsByPage = 10;
                      vm.rowCollection = [];
                      vm.editUserBtnClick = false; // used for edit btn click function
                      vm.rightPaneLoad = false; // used for change table width
                       
                      vm.closeWindow = function () {
                          ngDialog.close()
                      }

                      //toggle function
                      vm.loadFilterToggle = function () {
                          customBuilderFactory.customFilterToggle();

                      };



                      vm.ViewCompany = function (company) {

                          adminFactory.GetCustomerByCompanyId(company.id).success(
                          function (responce) {
                              if (responce) {

                                  $location.path('/profileInformation/' + responce.userId);
                                  //ngDialog.open({
                                  //    scope: $scope,
                                  //    template: '/app/admin/CustomerDetailsView.html',
                                  //    className: 'ngdialog-theme-plain custom-width-max',
                                  //    controller: $controller('customerDetailsCtrl', {
                                  //        $scope: $scope,
                                  //        company: responce
                                  //    })

                                  //});




                              } else {
                                  console.log("error occurd while retrieving Addresses");

                              }
                          }).error(function (error) {

                              console.log("error occurd while retrieving Addresses");
                          });





                      }

                      var tableStateCopy;

                      vm.searchComapnies = function (status,startRecord, pageRecord, tableState) {
                          
                          // Get values from view.
                          var userId = $window.localStorage.getItem('userGuid');
                          vm.userId = userId;

                          if (startRecord == undefined)
                              startRecord = 0;
                          if (pageRecord == undefined)
                              pageRecord = 10;
                          if (tableState == undefined)
                              tableState = tableStateCopy;

                          var pagedList = {
                              dynamicContent: {
                                  searchText : (vm.searchText == undefined || vm.searchText == "") ? null : vm.searchText,
                                  status : (status == undefined || status == "" || status == "All") ? null : status
                              },
                              pageSize: pageRecord,
                              currentPage: startRecord
                          }

                          adminFactory.getAllComapnies(pagedList)
                              .then(function successCallback(responce) {
                                   
                                  vm.rowCollection = responce.data.content;
                                  tableState.pagination.numberOfPages = responce.data.totalPages;

                              }, function errorCallback(response) {
                                  //todo
                              });
                      };

                      
                      vm.callServerSearch = function (tableState) {

                          tableStateCopy = tableState;

                          var start = tableState.pagination.start;
                          var number = tableState.pagination.number;
                          var numberOfPages = tableState.pagination.numberOfPages;
                           
                          vm.searchComapnies("",start, number, tableState);
                      };


                      vm.loadCompanyByStatus = function (status) {
                           
                          vm.searchComapnies(status, 0, 10, tableStateCopy);
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

                      vm.manageUsers = function () {
                           
                          vm.rightPaneLoad = true;
                          vm.editUserBtnClick = true;
                          vm.user = {};

                          vm.user.isActive = 'true';
                          vm.user.salutation = 'Mr';

                          vm.user.roles = [{
                              id: 'e69a8515-3570-439b-a953-e35422d143fb',
                              roleName: 'BusinessOwner'
                          }];

                          vm.user.assignedRoleName = 'BusinessOwner';
                      }


                      vm.createUser = function () {
                           

                          vm.user.loggedInUserId = $window.localStorage.getItem('userGuid');
                          vm.user.templateLink = '<html><head>    <title></title></head><body>    <p><img alt="" src="http://www.parcelinternational.nl/assets/Uploads/_resampled/SetWidth495-id-parcel-big.jpg" style="width: 200px; height: 200px; float: right;" /></p><div>        <h4 style="text-align: justify;">&nbsp;</h4><div style="background:#eee;border:1px solid #ccc;padding:5px 10px;">            <span style="font-family:verdana,geneva,sans-serif;">                <span style="color:#0000CD;">                    <span style="font-size:28px;">Account Activation</span>                </span>            </span>        </div><p style="text-align: justify;">&nbsp;</p><h4 style="text-align: justify;">            &nbsp;        </h4><h4 style="text-align: justify;">            <span style="font-size:12px;">                <span style="font-family:verdana,geneva,sans-serif;">                    Dear <strong>Salutation FirstName LastName, </strong>                </span>            </span>        </h4><h4 style="text-align: justify;">            <br /><span style="font-size:12px;">                <span style="font-family:verdana,geneva,sans-serif;">                    <strong>Welcome to Parcel International, we are looking forward to supporting your shipping needs. &nbsp;&nbsp;</strong>                </span>            </span>        </h4><h4 style="text-align: justify;">            <span style="font-size:12px;">                <span style="font-family:verdana,geneva,sans-serif;">                    <strong>                        Thank you for registering. To activate your account, please click &nbsp;ActivationURL                    </strong>                </span>            </span>        </h4><h4 style="text-align: justify;">            <span style="font-size:12px;">                <span style="font-family:verdana,geneva,sans-serif;"><strong>IMPORTANT! This activation link is valid for 24 hours only. &nbsp;&nbsp;</strong></span>            </span>        </h4><h4 style="text-align: justify;">            <span style="font-size:12px;">                <span style="font-family:verdana,geneva,sans-serif;">                    <strong>                        Should you have any questions or concerns, please contact Parcel International helpdesk for support &nbsp;                    </strong>                </span>            </span>        </h4>        <h4 style="text-align: justify;">            <span style="font-size:12px;">                <span style="font-family:verdana,geneva,sans-serif;">                    <i>                        *** This is an automatically generated email, please do not reply ***                    </i>                </span>            </span>        </h4>        <h4 style="text-align: justify;">&nbsp;</h4><h4 style="text-align: justify;">            <strong>                <span style="font-size:12px;">                    <span style="font-family:verdana,geneva,sans-serif;">Thank You, </span>                </span>            </strong>        </h4><h4 style="text-align: justify;">            <strong>                <span style="font-size:12px;">                    <span style="font-family:verdana,geneva,sans-serif;">Parcel International Team<br/>Phone: +18589144414 <br/>Email: <a href="mailto:helpdesk@parcelinternational.com">helpdesk@parcelinternational.com</a><br/>Website: <a href="http://www.parcelinternational.com">http://www.parcelinternational.com</a></span>                </span>            </strong>        </h4>    </div>   </body></html>';
                          var body = $("html, body");

                          userManagementFactory.createUser(vm.user)
                          .then(function (result) {
                               
                              if (result.status == 200) {
                                  vm.close();

                                  body.stop().animate({ scrollTop: 0 }, '500', 'swing', function () { });
                                  $('#panel-notif').noty({
                                      text: '<div class="alert alert-success media fade in"><p>' + $rootScope.translate('Customer created successfully') + '!</p></div>',
                                      layout: 'bottom-right',
                                      theme: 'made',
                                      animation: {
                                          open: 'animated bounceInLeft',
                                          close: 'animated bounceOutLeft'
                                      },
                                      timeout: 2000,
                                  });

                                  $timeout(function () {
                                      $route.reload();
                                  }, 2000);
                              }
                          },
                          function (error) {

                              if (error.data.message == "") {
                                  error.data.message = 'Error occured while processing your request';
                              }
                              body.stop().animate({ scrollTop: 0 }, '500', 'swing', function () { });
                              $('#panel-notif').noty({
                                  text: '<div class="alert alert-warning media fade in"><p>' + $rootScope.translate(error.data.message) + '!</p></div>',
                                  layout: 'bottom-right',
                                  theme: 'made',
                                  animation: {
                                      open: 'animated bounceInLeft',
                                      close: 'animated bounceOutLeft'
                                  },
                                  timeout: 3000,
                              });
                          });
                      }

                      vm.close = function () {
                           
                          vm.user = {};
                          //hide right pane
                          vm.rightPaneLoad = false;

                      }

                  }]);

})(angular.module('newApp'));
