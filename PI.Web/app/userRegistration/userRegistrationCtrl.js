'use strict';


(function(app){
    
    app.factory('registerUserService', function ($http) {            
         
        return{
            createUser : function (newuser) {
            return $http.post('http://localhost:5555/api/User/CreateUser',newuser);
        }
        };
      
        });

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

