'use strict';

(function (app) {
    app.controller('adminManageCustomersCtrl', ['customBuilderFactory', function (customBuilderFactory) {

        var vm = this;
        //toggle function
        vm.loadFilterToggle = function () {
            customBuilderFactory.customFilterToggle();

        };
        
    }]);

})(angular.module('newApp'));
