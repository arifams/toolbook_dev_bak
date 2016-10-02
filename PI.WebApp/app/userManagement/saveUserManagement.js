'use strict';

(function (app) {

    app.directive('validPasswordCSave', function () {
        return {
            require: 'ngModel',
            link: function (scope, elm, attrs, ctrl) {
                ctrl.$parsers.unshift(function (viewValue, $scope) {
                    var noMatch = viewValue != scope.formSaveUser.password.$viewValue;
                    ctrl.$setValidity('noMatch', !noMatch);
                    return viewValue;
                })
            }
        }
    });

    app.directive('validPasswordSave', function () {
        return {
            require: 'ngModel',
            link: function (scope, elm, attrs, ctrl) {
                ctrl.$parsers.unshift(function (viewValue, $scope) {

                    // password validate.
                    var res = /^(?=.*[a-z])(?=.*[A-Z])[a-zA-Z\S]{7,20}$/.test(viewValue);
                    ctrl.$setValidity('noValidPassword', res);

                    // if change the password when having confirmation password, check match and give error.
                    if (scope.formSaveUser.password_c.$viewValue != '') {
                        var noMatch = viewValue != scope.formSaveUser.password_c.$viewValue;
                        scope.formSaveUser.password_c.$setValidity('noMatch', !noMatch);
                    }

                    return viewValue;
                })
            }
        }
    });

    app.controller('saveUserManagementCtrl', ['$location', '$window', 'userManagementFactory', '$rootScope', '$routeParams',
        function ($location, $window, userManagementFactory, $rootScope, $routeParams) {
        var vm = this;
        vm.user = {};

        var loadUser = function () {
            debugger;
            console.log($routeParams.id);
            userManagementFactory.getUser($routeParams.id)
            .success(function (data) {
                debugger;
                vm.user = data;

                vm.user.assignedDivisionIdList = [];
                if (vm.user.id == 0 || vm.user.id == null) {
                    // New user.
                    //vm.user.assignedRoleName = vm.user.roles[0].roleName;

                    vm.user.isActive = 'true';
                    vm.user.salutation = 'Mr';

                    vm.user.assignedRoleName = $scope.userType;

                    if($scope.parentType == "Division"){

                        angular.forEach(vm.user.divisions, function (division) {

                            if (division.id == $scope.parentId) {

                                division.isAssigned = true;
                                vm.user.assignedDivisionIdList.push(division.id);
                            }
                        });
                    }
                }
                else {

                    // Exisiting user.
                    if (vm.user.isActive)
                        vm.user.isActive = 'true';
                    else
                        vm.user.isActive = 'false';

                    //Add selected divisions
                    angular.forEach(vm.user.divisions, function (division) {
                        if (division.isAssigned) {
                            vm.user.assignedDivisionIdList.push(division.id);
                        }
                    })
                }
            })
            .error(function () {
                
            })
        }

        loadUser();

        vm.saveUser = function () {
             
            vm.user.loggedInUserId = $window.localStorage.getItem('userGuid');
            vm.user.templateLink = '<html><head>    <title></title></head><body>    <p><img alt="" src="http://www.parcelinternational.nl/assets/Uploads/_resampled/SetWidth495-id-parcel-big.jpg" style="width: 200px; height: 200px; float: right;" /></p><div>        <h4 style="text-align: justify;">&nbsp;</h4><div style="background:#eee;border:1px solid #ccc;padding:5px 10px;">            <span style="font-family:verdana,geneva,sans-serif;">                <span style="color:#0000CD;">                    <span style="font-size:28px;">Account Activation</span>                </span>            </span>        </div><p style="text-align: justify;">&nbsp;</p><h4 style="text-align: justify;">            &nbsp;        </h4><h4 style="text-align: justify;">            <span style="font-size:12px;">                <span style="font-family:verdana,geneva,sans-serif;">                    Dear <strong>Salutation FirstName LastName, </strong>                </span>            </span>        </h4><h4 style="text-align: justify;">            <br /><span style="font-size:12px;">                <span style="font-family:verdana,geneva,sans-serif;">                    <strong>Welcome to Parcel International, we are looking forward to supporting your shipping needs. &nbsp;&nbsp;</strong>                </span>            </span>        </h4><h4 style="text-align: justify;">            <span style="font-size:12px;">                <span style="font-family:verdana,geneva,sans-serif;">                    <strong>                        Thank you for registering. To activate your account, please click &nbsp;ActivationURL                    </strong>                </span>            </span>        </h4><h4 style="text-align: justify;">            <span style="font-size:12px;">                <span style="font-family:verdana,geneva,sans-serif;"><strong>IMPORTANT! This activation link is valid for 24 hours only. &nbsp;&nbsp;</strong></span>            </span>        </h4><h4 style="text-align: justify;">            <span style="font-size:12px;">                <span style="font-family:verdana,geneva,sans-serif;">                    <strong>                        Should you have any questions or concerns, please contact Parcel International helpdesk for support &nbsp;                    </strong>                </span>            </span>        </h4>        <h4 style="text-align: justify;">            <span style="font-size:12px;">                <span style="font-family:verdana,geneva,sans-serif;">                    <i>                        *** This is an automatically generated email, please do not reply ***                    </i>                </span>            </span>        </h4>        <h4 style="text-align: justify;">&nbsp;</h4><h4 style="text-align: justify;">            <strong>                <span style="font-size:12px;">                    <span style="font-family:verdana,geneva,sans-serif;">Thank You, </span>                </span>            </strong>        </h4><h4 style="text-align: justify;">            <strong>                <span style="font-size:12px;">                    <span style="font-family:verdana,geneva,sans-serif;">Parcel International Team<br/>Phone: +18589144414 <br/>Email: <a href="mailto:helpdesk@parcelinternational.com">helpdesk@parcelinternational.com</a><br/>Website: <a href="http://www.parcelinternational.com">http://www.parcelinternational.com</a></span>                </span>            </strong>        </h4>    </div>   </body></html>';
            var body = $("html, body");

            userManagementFactory.saveUser(vm.user)
            .then(function (result) {
                 
                if (result.status == 200) {
                    body.stop().animate({ scrollTop: 0 }, '500', 'swing', function () { });
                    $('#panel-notif').noty({
                        text: '<div class="alert alert-success media fade in"><p>' + $rootScope.translate('User saved successfully') + '!</p></div>',
                        layout: 'bottom-right',
                        theme: 'made',
                        animation: {
                            open: 'animated bounceInLeft',
                            close: 'animated bounceOutLeft'
                        },
                        timeout: 3000,
                    });
                    vm.close();
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
            $location.path('/adminManageUser');
        }

        vm.toggleDivisionSelection = function (division) {
            
            var idx = vm.user.assignedDivisionIdList.indexOf(division.id);
            // is currently selected
            if (idx > -1) {
                vm.user.assignedDivisionIdList.splice(idx, 1);
            }
                // is newly selected
            else {
                vm.user.assignedDivisionIdList.push(division.id);
            }
        }

    }]);


})(angular.module('newApp'));