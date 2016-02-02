'use strict';


(function(app){

    app.controller('userRegistrationCtrl', [function ($q,registerUserService) {
        var vm = this;
        vm.register = function (user) {
            //window.alert("test");
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

    app.factory('registerUserService', function ($http, $q) {

        var deferred = $q.defer();

        this.createUser = function (user)
        {
             return $http.post('api/createConttroller', user);             

        }
    });

})(angular.module('userRegistration', []));

