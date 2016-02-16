'use strict';


(function(app){
    
    app.factory('registerUserService', function ($http) {            
         
        return{
            createUser : function (newuser) {
                return $http.post(serverBaseUrl + '/api/accounts/create', newuser);
        }
        };
      
    });

    app.directive('validPasswordC', function () {
        return {
            require: 'ngModel',
            link: function (scope, elm, attrs, ctrl) {
                ctrl.$parsers.unshift(function (viewValue, $scope) {
                    var noMatch = viewValue != scope.formSignup.password.$viewValue;
                    ctrl.$setValidity('noMatch', !noMatch);
                    return viewValue;
                })
            }
        }
    });

    app.directive('validPassword', function () {
        return {
            require: 'ngModel',
            link: function (scope, elm, attrs, ctrl) {
                ctrl.$parsers.unshift(function (viewValue, $scope) {

                    // password validate.
                    var res = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d.*)(?=.*\W.*)[a-zA-Z0-9\S]{8,20}$/.test(viewValue);
                    ctrl.$setValidity('noValidPassword', res);

                    // if change the password when having confirmation password, check match and give error.
                    if (scope.formSignup.password_c.$viewValue != ''){
                        var noMatch = viewValue != scope.formSignup.password_c.$viewValue;
                        scope.formSignup.password_c.$setValidity('noMatch', !noMatch);
                    }

                    return viewValue;
                })
            }
        }
    });

    app.directive('validPhoneNo', function () {
        return {
            require: 'ngModel',
            link: function (scope, elm, attrs, ctrl) {
                ctrl.$parsers.unshift(function (viewValue, $scope) {

                    // should have only +,- and digits.
                    var res1 = /^[0-9()+-]*$/.test(viewValue);
                    // should have at least 8 digits.
                    var res2 = /(?=(.*\d){8})/.test(viewValue);

                    ctrl.$setValidity('notValidPhoneNo', res1 && res2);
                    return viewValue;
                })
            }
        }
    });

    app.directive('icheck', ['$timeout', '$parse', function ($timeout, $parse) {

        return {
            require: 'ngModel',
            link: function ($scope, element, $attrs, ngModel) {
                return $timeout(function () {
                    var value;
                    value = $attrs['value'];

                    $scope.$watch($attrs['ngModel'], function (newValue) {
                        $(element).iCheck('update');
                    })

                    return $(element).iCheck({
                        checkboxClass: 'icheckbox_square-blue', //'icheckbox_flat-aero',
                        radioClass: 'iradio_square-blue'

                    }).on('ifChanged', function (event) {
                        if ($(element).attr('type') === 'checkbox' && $attrs['ngModel']) {
                            $scope.$apply(function () {
                                return ngModel.$setViewValue(event.target.checked);
                            });
                        }
                        if ($(element).attr('type') === 'radio' && $attrs['ngModel']) {
                            return $scope.$apply(function () {
                                return ngModel.$setViewValue(value);
                            });
                        }
                    });
                });
            }
        };

    }]);

    app.controller('userRegistrationCtrl', 
        ['registerUserService' ,function (registerUserService) {

        var vm = this;
        vm.user = {};
        vm.user.salutation = "Mr";
        vm.user.iscorporate = "false";
        vm.user.contactType = 'phone';
        vm.isSubmit = false;
        vm.user.customeraddress = {};
        vm.user.customeraddress.country = 'United States';
        vm.isSentMail = false;
        vm.isEmailNotValid = false;
        vm.isServerError = false;
        vm.user.templateLink= '<html><head>	<title></title></head><body><p><img alt="" src="http://www.parcelinternational.nl/assets/Uploads/_resampled/SetWidth495-id-parcel-big.jpg" style="width: 200px; height: 200px; float: right;" /></p><div><h4 style="text-align: justify;">&nbsp;</h4><div style="background:#eee;border:1px solid #ccc;padding:5px 10px;"><span style="font-family:verdana,geneva,sans-serif;"><span style="color:#0000CD;"><span style="font-size:28px;">Account Activation</span></span></span></div><p style="text-align: justify;">&nbsp;</p><h4 style="text-align: justify;">&nbsp;</h4><h4 style="text-align: justify;"><span style="font-size:12px;"><span style="font-family:verdana,geneva,sans-serif;">Dear <strong>FirstName &nbsp;LastName, &nbsp;</strong></span></span></h4><h4 style="text-align: justify;"><br /><span style="font-size:12px;"><span style="font-family:verdana,geneva,sans-serif;"><strong>Welcome to Parcel International, we are looking forward to supporting your shipping needs. &nbsp;&nbsp;</strong></span></span></h4><h4 style="text-align: justify;"><span style="font-size:12px;"><span style="font-family:verdana,geneva,sans-serif;"><strong>Thank you for registering. To activate your account, please click &nbsp;ActivationURL</strong></span></span></h4><h4 style="text-align: justify;"><span style="font-size:12px;"><span style="font-family:verdana,geneva,sans-serif;"><strong>IMPORTANT! This activation link is valid for 24 hours only. &nbsp;&nbsp;</strong></span></span></h4><h4 style="text-align: justify;"><span style="font-size:12px;"><span style="font-family:verdana,geneva,sans-serif;"><strong>Should you have any questions or concerns, please contact Parcel International helpdesk for support &nbsp;</strong></span></span></h4><h4 style="text-align: justify;">&nbsp;</h4><h4 style="text-align: justify;"><strong><span style="font-size:12px;"><span style="font-family:verdana,geneva,sans-serif;">Thank You, </span></span></strong></h4><h4 style="text-align: justify;"><strong><span style="font-size:12px;"><span style="font-family:verdana,geneva,sans-serif;">Parcel International Team</span></span></strong></h4></div></body></html>';
        
        vm.changeCountry = function () {
            vm.isRequiredState = vm.user.customeraddress.country == 'United States' || vm.user.customeraddress.country == 'Canada' || vm.user.customeraddress.country == 'Puerto Rico';
        };
        vm.changeCountry();

        vm.register = function () {         

            registerUserService.createUser(vm.user)
            .then(function (result)
            {
                if (result.data == "1") {
                    vm.isSentMail = true;
                }
                else if (result.data == "-2") {
                    vm.isEmailNotValid = true;
                }
                else { 
                    // Other issues.
                    vm.isServerError = true;
                }
            },
            function (error) {
                console.log("failed");
            }
            );
        };
    }]);
        

})(angular.module('userRegistration', ['ngMessages']));

