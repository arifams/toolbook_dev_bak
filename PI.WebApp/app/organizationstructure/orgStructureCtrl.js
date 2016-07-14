'use strict';

(function (app) {

    app.controller('orgStructureCtrl', [function ($scope) {

                    $("#org").jOrgChart({
                        chartElement: '#chart',
                        dragAndDrop: true
                    });

           }]);


})(angular.module('newApp'));