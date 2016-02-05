'use strict';


(function(app){
    
    app.factory('registerUserService', function ($http) {            
         
        return{
            createUser : function (newuser) {
            return $http.post('http://localhost:5555/api/User/CreateUser',newuser);
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

    app.controller('userRegistrationCtrl', ['registerUserService', function (registerUserService) {
        var vm = this;      
        vm.contacttype = 'Phone';
        // vm.user.isCorporateAccount.value = 'False';

        vm.register = function (user) {         

            registerUserService.createUser(user)
            .then(function (result)
            {
                console.log("success");
            },
            function (error) {
                console.log("failed");
            }
            );
        };
    }]);
        

})(angular.module('userRegistration', []));

