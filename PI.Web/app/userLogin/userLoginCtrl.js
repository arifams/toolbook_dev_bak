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

        vm.loginInvalid = false;

        vm.login = function (user) {
            loginRegisteredUser.loginUser(user)
           .then(function (result) {
               console.log("success" + result);
               debugger;
               if (result.data == "1") {
                   window.location = "http://localhost:63874/app/index.html";
               }
               else {
                   vm.loginInvalid = true;
               }
           },
           function (error) {

               console.log("failed");
           }
           );
           
        };
    }]);


})(angular.module('userLogin', []));

