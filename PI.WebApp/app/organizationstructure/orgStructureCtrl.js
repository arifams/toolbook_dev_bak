'use strict';

(function (app) {

    app.controller('orgStructureCtrl',[ '$scope',function ($scope) {

                $(document).ready(function () {
                    $("#org").jOrgChart({
                        chartElement: '#chart',
                        dragAndDrop: true
                    });
                });
           }]);


})(angular.module('newApp'));