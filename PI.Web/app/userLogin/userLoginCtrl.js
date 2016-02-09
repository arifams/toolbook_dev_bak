'use strict';


(function (app) {    

    app.factory('userRegister', function ($http) {
        return {
            loginUser: function (newuser, url) {
                //return $http.post('http://localhost:5555/api/User/LoginUser', newuser);
                return $http.post(window.location.host + '/' + url, newuser);
            }
        };

    });

    app.controller('userLoginCtrl', ['userRegister', function (userRegister) {
        var vm = this;     

        vm.loginInvalid = false;
        vm.isEmailConfirm = false;

        vm.isConfirmEmail = function () {

            if (window.location.search != "") {
                // Show email confirm message.
                vm.isEmailConfirm = true;
                var splittedValues = window.location.search.replace("?", "").split('&');

                // Check userid valid and token isn't expired

            }

        };

        vm.login = function (user) {
            userRegister.loginUser(user, 'api/User/LoginUser')
           .then(function (result) {
               console.log("success" + result);
               
               if (result.data == "1") {
                   //window.location = "http://localhost:63874/app/index.html";
                   window.location = "http://pibookingservice.azurewebsites.net/app/index.html";
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

        vm.isConfirmEmail();

    }]);


})(angular.module('userLogin', []));

