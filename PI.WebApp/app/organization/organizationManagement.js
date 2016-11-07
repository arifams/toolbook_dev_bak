'use strict';
(function (app) {

    app.controller('organizationUsersCtrl', function (userManagementFactory, $scope, $location, $routeParams, $timeout,
        $log, $window, $sce, $route, $rootScope) {

        var vm = this;
        vm.status = 'All';
        vm.loadingSymbole = true;
        vm.editUserBtnClick = false; // used for edit btn click function
        vm.rightPaneLoad = false; // used for change table width
        vm.user = {};

        userManagementFactory.loadUserManagement()
            .then(function successCallback(response) {

                vm.divisionList = response.data.divisions;
                vm.roleList = response.data.roles;
                vm.loadingSymbole = false;

            }, function errorCallback(response) {
                //todo
            });


        vm.itemsByPage = 25;
        vm.rowCollection = [];
        // Add dumy record, since data loading is async.
        // vm.rowCollection.push(1);

        vm.searchUsers = function () {
            

            // Get values from view.
            var loggedInuserId = $window.localStorage.getItem('userGuid');
            var role = (vm.role == undefined || vm.role == "") ? 0 : vm.role;
            var searchText = vm.searchText;
            var status = (vm.status == undefined || vm.status == "" || vm.status == "All") ? 0 : vm.status;

            vm.loadingSymbole = true;

            userManagementFactory.getUsersByFilter(loggedInuserId, searchText, role, status)
                .then(function successCallback(responce) {
                    vm.loadingSymbole = false;
                    vm.rowCollection = responce.data.content;

                }, function errorCallback(response) {
                    //todo
                    vm.loadingSymbole = false;
                });
        };

        vm.selectRole = function () {
            vm.searchUsers();
        };

        vm.deleteById = function (row) {

            var r = confirm("Do you want to delete the record?");
            if (r == true) {
                userManagementFactory.deleteUser({ Id: row.id })
                    .success(function (response) {
                        if (response == 1) {
                            var index = vm.rowCollection.indexOf(row);
                            if (index !== -1) {
                                vm.rowCollection.splice(index, 1);
                            }
                        }
                    })
                    .error(function () {
                    })
            }
        };

        vm.callServerSearch = function (tableState) {
            
            var pagination = 0;//tableState.pagination;

            var start = pagination.start || 0;     // This is NOT the page number, but the index of item in the list that you want to use to display the table.
            var number = pagination.number || 10;  // Number of entries showed per page.
            vm.loadingSymbole = true;
            vm.searchUsers();
        };

        vm.callServerSearch();

        vm.resetSearch = function (tableState) {
            
            var pagination = 0;//tableState.pagination;

            var start = pagination.start || 0;     // This is NOT the page number, but the index of item in the list that you want to use to display the table.
            var number = pagination.number || 10;  // Number of entries showed per page.

            vm.userStatus = 'All';
            vm.datePicker.date = { "startDate": null, "endDate": null };
            //vm.datePicker.date.endDate = null;

            vm.searchUsers();

        };


        vm.manageUsers = function (userObj) {
            
            vm.rightPaneLoad = true;
            vm.editUserBtnClick = true;

            vm.user = angular.copy(userObj);

            if (userObj == null) {
                vm.user = {};
                vm.user.id = 0;
            }

            if (vm.user.id == 0 || vm.user.id == null) {

                vm.user.isActive = 'true';
                vm.user.salutation = 'Mr';

                vm.user.roles = [{
                    id: '1',
                    roleName: 'BusinessOwner'
                }
                , {
                    id: '2',
                    roleName: 'Manager'
                }];


                vm.user.assignedRoleName = 'Manager';
            }
            else {
                
                // Exisiting user.
                if (vm.user.status == 'Active')
                    vm.user.isActive = 'true';
                else
                    vm.user.isActive = 'false';

                vm.user.roles = [{
                    id: '1',
                    roleName: 'BusinessOwner'
                },
               {
                   id: '2',
                   roleName: 'Manager'
               }];

                vm.user.assignedRoleName = vm.user.roleName;
            }
        }

        vm.saveUser = function () {
            
            vm.user.loggedInUserId = $window.localStorage.getItem('userGuid');
            vm.user.templateLink = '<html><head>    <title></title></head><body>    <p><img alt="" src="http://www.parcelinternational.nl/assets/Uploads/_resampled/SetWidth495-id-parcel-big.jpg" style="width: 200px; height: 200px; float: right;" /></p><div>        <h4 style="text-align: justify;">&nbsp;</h4><div style="background:#eee;border:1px solid #ccc;padding:5px 10px;">            <span style="font-family:verdana,geneva,sans-serif;">                <span style="color:#0000CD;">                    <span style="font-size:28px;">Account Activation</span>                </span>            </span>        </div><p style="text-align: justify;">&nbsp;</p><h4 style="text-align: justify;">            &nbsp;        </h4><h4 style="text-align: justify;">            <span style="font-size:12px;">                <span style="font-family:verdana,geneva,sans-serif;">                    Dear <strong>Salutation FirstName LastName, </strong>                </span>            </span>        </h4><h4 style="text-align: justify;">            <br /><span style="font-size:12px;">                <span style="font-family:verdana,geneva,sans-serif;">                    <strong>Welcome to Parcel International, we are looking forward to supporting your shipping needs. &nbsp;&nbsp;</strong>                </span>            </span>        </h4><h4 style="text-align: justify;">            <span style="font-size:12px;">                <span style="font-family:verdana,geneva,sans-serif;">                    <strong>                        Thank you for registering. To activate your account, please click &nbsp;ActivationURL                    </strong>                </span>            </span>        </h4><h4 style="text-align: justify;">            <span style="font-size:12px;">                <span style="font-family:verdana,geneva,sans-serif;"><strong>IMPORTANT! This activation link is valid for 24 hours only. &nbsp;&nbsp;</strong></span>            </span>        </h4><h4 style="text-align: justify;">            <span style="font-size:12px;">                <span style="font-family:verdana,geneva,sans-serif;">                    <strong>                        Should you have any questions or concerns, please contact Parcel International helpdesk for support &nbsp;                    </strong>                </span>            </span>        </h4>        <h4 style="text-align: justify;">            <span style="font-size:12px;">                <span style="font-family:verdana,geneva,sans-serif;">                    <i>                        *** This is an automatically generated email, please do not reply ***                    </i>                </span>            </span>        </h4>        <h4 style="text-align: justify;">&nbsp;</h4><h4 style="text-align: justify;">            <strong>                <span style="font-size:12px;">                    <span style="font-family:verdana,geneva,sans-serif;">Thank You, </span>                </span>            </strong>        </h4><h4 style="text-align: justify;">            <strong>                <span style="font-size:12px;">                    <span style="font-family:verdana,geneva,sans-serif;">Parcel International Team<br/>Phone: +18589144414 <br/>Email: <a href="mailto:helpdesk@parcelinternational.com">helpdesk@parcelinternational.com</a><br/>Website: <a href="http://www.parcelinternational.com">http://www.parcelinternational.com</a></span>                </span>            </strong>        </h4>    </div>   </body></html>';
            var body = $("html, body");

            userManagementFactory.saveUser(vm.user)
            .then(function (result) {
                
                if (result.status == 200) {
                    vm.close();

                    body.stop().animate({ scrollTop: 0 }, '500', 'swing', function () { });
                    $('#panel-notif').noty({
                        text: '<div class="alert alert-success media fade in"><p>' + $rootScope.translate('User saved successfully') + '!</p></div>',
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

    });

})(angular.module('newApp'));
