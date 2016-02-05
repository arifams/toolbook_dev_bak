'use strict';


(function(app){
    
    app.factory('registerUserService', function ($http) {            
         
        return{
            createUser : function (newuser) {
               // return $http.post('http://pibooking.azurewebsites.net/api/User/CreateUser', newuser);
                return $http.post('http://localhost:5555/api/User/CreateUser', newuser);

        }
        };
      
    });

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
    })

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
                debugger;
                if (result.data == "-1") {
                    vm.showEmailError = true;
                }
                else {
                    // window.location = "http://pibookingservice.azurewebsites.net/app/index.html";
                    window.location = "http://localhost:63874/app/index.html";
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

