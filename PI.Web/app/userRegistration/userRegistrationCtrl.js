'use strict';


(function(app){

    app.controller('userRegistrationCtrl', ['$http',function () {
        var vm = this;
        vm.register = function (user) {

            window.alert("test");
        };
    }]);

})(angular.module('userRegistration', []));

