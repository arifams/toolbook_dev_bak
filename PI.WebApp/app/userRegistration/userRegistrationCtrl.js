'use strict';


(function(app){

    app.factory('registerUserService', function ($http) {            
         
        return{
            createUser : function (newuser) {
                return $http.post(serverBaseUrl + '/api/accounts/create', newuser);
        }
        };
      
    });

    app.run(function (gettextCatalog, $rootScope, $window) {

        gettextCatalog.setCurrentLanguage($window.localStorage.getItem('currentLnguage'));

        $rootScope.translate = function (str) {
            return gettextCatalog.getString(str);
        };

        //gettextCatalog.debug = true;
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
                    var res = /^(?=.*[a-z])(?=.*[A-Z])[a-zA-Z\S]{7,20}$/.test(viewValue);
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
        ['registerUserService', '$window', '$rootScope', function (registerUserService, $window, $rootScope) {

        var vm = this;
        vm.user = {};
        
        vm.isSubmit = false;
        
        vm.isSentMail = false;
        vm.isEmailNotValid = false;
        vm.isServerError = false;
        vm.user.templateLink = '<html><head><title></title></head><body style="margin:30px;"><div style="margin-right:40px;margin-left:40px"><div style="margin-top:30px;background-color:#0af;font-size:28px;border:5px solid #d9d9d9;text-align:center;padding:10px;font-family:verdana,geneva,sans-serif;color:#fff">Account Activation - Parcel International</div></div><div style="margin-right:40px;margin-left:40px"><div style="float:left;"><img alt="" src="http://www.parcelinternational.nl/assets/Uploads/_resampled/SetWidth495-id-parcel-big.jpg" style="width: 130px; height: 130px;" /></div><div><h3 style="margin-bottom:65px;margin-right:146px;margin-top:0;padding-top:62px;text-align:center;font-size:22px;font-family:verdana,geneva,sans-serif;color:#005c99">Thank you registering with Parcel International</h3></div></div><div style="margin-right:40px;margin-left:40px"><div style="padding:10px;font-family:verdana,geneva,sans-serif;color:#fff;border:5px solid #0af;background-color:#005c99;font-size:13px"><p style="font-weight:700;font-style:italic;font-size:14px">Dear Salutation FirstName LastName,</p><br/><p style="font-weight:700;font-style:italic;font-size:14px">Thank you for creating a Parcel International account.</p><p style="font-weight:700;font-style:italic;font-size:14px">We are looking forward to supporting your shipping needs!.</p><p style="font-weight:700;font-style:italic;font-size:14px">But before we can start shipping, we need to complete your registration, please click <span style="color:#80d4ff;font-size:14px;">ActivationURL</span> to verify your email address.</p><p style="font-weight:700;font-style:italic;font-size:14px">IMPORTANT! Please note that this link is valid for 24 hours only. <p><p style="font-weight:700;font-style:italic;font-size:14px">Should you have any questions or concerns, please contact Parcel International helpdesk for support.</p></br><p style="font-weight:700;font-style:italic;font-size:14px">Thank you,</p><p style="font-weight:700;font-style:italic;font-size:14px">Parcel International Service Team</p><br/><p>Phone: <span style="font-size:14px;color:#80d4ff">+1 858 914 4414</span> </p><p>Email address:<a href="mailto:helpdesk@parcelinternational.com" style="color:#80d4ff">  helpdesk@parcelinternational.com</a></p><p>Website: <a href="http://www.parcelinternational.com" style="color:#80d4ff">www.parcelinternational.com</a></p></div><p><i>*** This is an automatically generated email, please do not reply ***</i></p></div></body></html>';
        
        vm.OpenNewWindow = function () {
            $window.open("termsandconditions.html", "", "width=640, height=480");
        }

        vm.alreadySubmitted = false;

        vm.register = function () {         

            vm.alreadySubmitted = true;
         
            registerUserService.createUser(vm.user)
            .then(function (result)
            {
                if (result.data == "1") {
                    vm.alreadySubmitted = false;
                    vm.isSentMail = true;
                }
                else if (result.data == "-2") {
                    //vm.isEmailNotValid = true;
                    vm.alreadySubmitted = false;
                    $.noty.defaults.killer = true;

                    noty({
                        text: '<p style="font-size:medium">Error! </p>' + $rootScope.translate('Email address is already in use'),
                        layout: 'topRight',
                        type: 'error', //warning
                        animation: {
                            open: 'animated bounceInRight', // Animate.css class names
                            close: { height: 'toggle' }, // Animate.css class names
                            easing: 'swing', // unavailable - no need
                            speed: 200 // unavailable - no need
                        },
                        timeout: 4000
                    });
                }
                else { 
                    // Other issues.
                    //vm.isServerError = true;
                    vm.alreadySubmitted = false;

                    $.noty.defaults.killer = true;

                    noty({
                        text: '<p style="font-size:medium">Error! </p>' + $rootScope.translate('Error occured while processing registration'),
                        layout: 'topRight',
                        type: 'error',
                        animation: {
                            open: 'animated bounceInRight', // Animate.css class names
                            close: { height: 'toggle' }, // Animate.css class names
                            easing: 'swing', // unavailable - no need
                            speed: 200 // unavailable - no need
                        },
                        timeout: 4000
                    });
                }
            },
            function (error) {
                console.log("failed");
            }
            );
        };
    }]);
        

})(angular.module('userRegistration', ['ngMessages', 'gettext']));

