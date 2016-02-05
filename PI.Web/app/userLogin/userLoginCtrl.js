'use strict';


(function (app) {    

    app.factory('loginRegisteredUser', function ($http) {
        return {
            loginUser: function (newuser) {
                return $http.post('http://localhost:5555/api/User/LoginUser', newuser);
            }
        };

    });


    app.controller('userLoginCtrl', ['loginRegisteredUser', function (loginRegisteredUser) {
        var vm = this;     

        vm.login = function (user) {
            loginRegisteredUser.loginUser(user)
           .then(function (result) {
               console.log("success");
           },
           function (error) {

               console.log("failed");
           }
           );
           
        };
    }]);


})(angular.module('userLogin', []));

