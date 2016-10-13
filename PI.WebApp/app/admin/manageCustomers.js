'use strict';

(function (app) {
    app.controller('adminManageCustomersCtrl', ['customBuilderFactory',
        function (customBuilderFactory) {

            var vm = this;
            vm.editUserBtnClick = false; // used for edit btn click function
            vm.rightPaneLoad = false; // used for change table width


            //toggle function
        vm.loadFilterToggle = function () {
            customBuilderFactory.customFilterToggle();

        };

        vm.manageUsers = function () {
            debugger;
            vm.rightPaneLoad = true;
            vm.editUserBtnClick = true;
        }

        
    }]);

})(angular.module('newApp'));
