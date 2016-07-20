'use strict';

(function (app) {

    app.controller('orgStructureCtrl',[ '$scope',function ($scope) {

                //$(document).ready(function () {
                //    $("#org").jOrgChart({
                //        chartElement: '#chart',
                //        dragAndDrop: true
                //    });
        //});


        jQuery(function () {

            var datascource = {
                'id': '1',
                'name': 'Business Owner',
                'title': 'Business owner name',
                'children': [
                  {
                      'id': '2', 'name': 'Manager- Active', 'title': 'manager active name',
                      'children': [
                      {
                          'id': '3', 'name': 'Supervisor- Active', 'title': 'supervisor active name',
                          'children': [
                              {
                                  'id': '5', 'name': 'Division', 'title': 'division name',
                                  'children': [
                                      { 'id': '6', 'name': 'Operator- Active', 'title': 'operator active name' },
                                      { 'id': '7', 'name': 'Operator-Inactive', 'title': 'operator inactive name' }
                                  ], 'costcenter': '2'
                              }
                          ]
                      },
                      {
                          'id': '4', 'name': 'Division', 'title': 'division name',
                          'children': [
                                { 'id': '5', 'name': 'Operator-Inactive', 'title': 'operator inactive name' }
                          ],
                          'costcenter': '1'
                      }
                      ]
                  },

                ]
            };

            $('#chart-container').orgchart({
                'data': datascource,
                
                'nodeContent': 'title',
                'nodeID': 'id',
                'createNode': function ($node, data) {
                    var secondMenuIcon = $('<i>', {
                        'class': 'fa fa-plus-circle second-menu-icon',
                        click: function () {
                            $(this).siblings('.second-menu').toggle();
                        }
                    });
                    var secondMenu = '<div class="second-menu><div dropdown="" class="btn-group"><button data-toggle="dropdown" class="btn btn-default dropdown-toggle" type="button" aria-haspopup="true" data-toggle="dropdown"><span class="caret"></span></button><span class="dropdown-arrow"></span><ul role="menu" class="dropdown-menu"><li><a href="#">add user</a></li><li><a href="#" >another action</a></li><li><a href="#">something else here</a></li></ul></div></div>';
                    $node.append(secondMenuIcon);
                    //$node.append(secondMenuIcon).append(secondMenu);
                }
            });

        });

           }]);


})(angular.module('newApp'));