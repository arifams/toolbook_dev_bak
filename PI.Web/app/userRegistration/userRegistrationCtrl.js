﻿'use strict';


(function(app){
    
    app.factory('registerUserService', function ($http) {            
         
        return{
            createUser : function (newuser) {
                return $http.post(serverBaseUrl + '/api/User/CreateUser', newuser);
        }
        };
      
    });

    //password validation
    app.directive('validPasswordC', function () {
        return {
            require: 'ngModel',
            link: function (scope, elm, attrs, ctrl) {
                ctrl.$parsers.unshift(function (viewValue, $scope) {
                    var noMatch = viewValue != scope.newUserRegisterForm.password.$viewValue
                    ctrl.$setValidity('noMatch', !noMatch)
                })
            }
        }
    });


 



    app.controller('userRegistrationCtrl', 
        ['registerUserService' ,function (registerUserService) {
        var vm = this;      
        vm.contacttype = 'Phone';
        vm.user = {};
        vm.user.Salutation = "MR";
        vm.user.isCorporateAccount = "True";
        vm.showEmailError = false;

        vm.register = function (user) {         

            registerUserService.createUser(user)
            .then(function (result)
            {
               
                if (result.data == "-1") {
                    vm.showEmailError = true;
                }
                else {
                    window.location = webBaseUrl + "/app/index.html";
                }
                console.log("success");
               

            },
            function (error) {
                console.log("failed");
            }
            );
        };
    }]);
        

})(angular.module('userRegistration', []));

