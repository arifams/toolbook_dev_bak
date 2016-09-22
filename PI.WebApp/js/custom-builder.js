﻿angular.module('newApp')
.factory('customBuilderFactory', ['applicationService', function (applicationService) {

    var customBuilderFactory = {};

    

    /* Main Color */
    function mainColor() {
       
        $('.theme-color').on('click', function (e) {
            e.preventDefault();
            var main_color = $(this).data('color');
            var main_name = $(this).attr('data-main');
            $('body').removeClass(function (profileInformation, css) {
                return (css.match(/(^|\s)color-\S+/g) || []).join(' ');
            });
            $('body').addClass('color-' + main_name);
            $('.theme-color').removeClass('active');
            $(this).addClass('active');
            
        });
    }

    function setColor(mColor, mName) {
        var main_color = mColor;
        var main_name = mName;
        debugger;
        $('body').removeClass(function (index, css) {
            return (css.match(/(^|\s)color-\S+/g) || []).join(' ');
        });
        $('body').addClass('color-' + main_name);
        $('.theme-color').removeClass('active');
        $(this).addClass('active');
        
    }
	  
    customBuilderFactory = {
        init: function (mColor, mName) {
            "use strict";
            mainColor();

            if (mColor != undefined && mName != undefined){
                setColor(mColor, mName);
            }
            else{
            applicationService.resetStyle();
            }

            if ($('body').hasClass('sidebar-top')) {
                destroySideScroll();
            }
               
           
           

            //if ($('body').hasClass('sidebar-top')) {
            //    destroySideScroll();
            //}
        },
       
        scrollTopackagedetails : function () {

            $("#btnConsignorNext").click(function () {
                $('html, body').animate({
                    scrollTop: 0
                });
                
            });
        },
        scrollToRatesAndCarrierDetails: function () {

            $("#btnPackageDetailsNext").click(function () {
                $('html, body').animate({
                    scrollTop: 0
                });
                
            });
        },
        customFilterToggle: function () {
                console.log("called..");
                if ($('#builder1').hasClass('open'))
                {
                    $('#builder1').removeClass('open');
                }
                else {
                    $('#builder1').addClass('open');
                 
                }
        },

        //loadMap: function () {
       
        //    /**** FLIGHT MAP ****/
        //    // svg path for target icon
        //    var targetSVG = "M9,0C4.029,0,0,4.029,0,9s4.029,9,9,9s9-4.029,9-9S13.971,0,9,0z M9,15.93 c-3.83,0-6.93-3.1-6.93-6.93S5.17,2.07,9,2.07s6.93,3.1,6.93,6.93S12.83,15.93,9,15.93 M12.5,9c0,1.933-1.567,3.5-3.5,3.5S5.5,10.933,5.5,9S7.067,5.5,9,5.5 S12.5,7.067,12.5,9z";
        //    // svg path for plane icon
        //    var planeSVG = "M19.671,8.11l-2.777,2.777l-3.837-0.861c0.362-0.505,0.916-1.683,0.464-2.135c-0.518-0.517-1.979,0.278-2.305,0.604l-0.913,0.913L7.614,8.804l-2.021,2.021l2.232,1.061l-0.082,0.082l1.701,1.701l0.688-0.687l3.164,1.504L9.571,18.21H6.413l-1.137,1.138l3.6,0.948l1.83,1.83l0.947,3.598l1.137-1.137V21.43l3.725-3.725l1.504,3.164l-0.687,0.687l1.702,1.701l0.081-0.081l1.062,2.231l2.02-2.02l-0.604-2.689l0.912-0.912c0.326-0.326,1.121-1.789,0.604-2.306c-0.452-0.452-1.63,0.101-2.135,0.464l-0.861-3.838l2.777-2.777c0.947-0.947,3.599-4.862,2.62-5.839C24.533,4.512,20.618,7.163,19.671,8.11z";

        //    AmCharts.makeChart("flight-map", {
        //        type: "map",
        //        pathToImages: "http://www.amcharts.com/lib/3/images/",
        //        addClassNames: true,
        //        zoomControl: {
        //            "buttonFillColor": "#333333",
        //            top: 60
        //        },
        //        dataProvider: {
        //            map: "worldLow",
        //            getAreasFromMap: true,
        //            linkToObject: "london",
        //            images: [{
        //                id: "london",
        //                color: "#000000",
        //                type: "circle",
        //                title: "London",
        //                latitude: 51.5002,
        //                longitude: -0.1262,
        //                scale: 1.5,
        //                zoomLevel: 2.74,
        //                zoomLongitude: -20.1341,
        //                zoomLatitude: 49.1712,
        //                lines: [{
        //                    latitudes: [51.5002, 50.4422],
        //                    longitudes: [-0.1262, 30.5367]
        //                }, {
        //                    latitudes: [51.5002, 46.9480],
        //                    longitudes: [-0.1262, 7.4481]
        //                }, {
        //                    latitudes: [51.5002, 59.3328],
        //                    longitudes: [-0.1262, 18.0645]
        //                }, {
        //                    latitudes: [51.5002, 40.4167],
        //                    longitudes: [-0.1262, -3.7033]
        //                }, {
        //                    latitudes: [51.5002, 46.0514],
        //                    longitudes: [-0.1262, 14.5060]
        //                }, {
        //                    latitudes: [51.5002, 48.2116],
        //                    longitudes: [-0.1262, 17.1547]
        //                }, {
        //                    latitudes: [51.5002, 44.8048],
        //                    longitudes: [-0.1262, 20.4781]
        //                }, {
        //                    latitudes: [51.5002, 55.7558],
        //                    longitudes: [-0.1262, 37.6176]
        //                }, {
        //                    latitudes: [51.5002, 38.7072],
        //                    longitudes: [-0.1262, -9.1355]
        //                }, {
        //                    latitudes: [51.5002, 54.6896],
        //                    longitudes: [-0.1262, 25.2799]
        //                }, {
        //                    latitudes: [51.5002, 64.1353],
        //                    longitudes: [-0.1262, -21.8952]
        //                }, {
        //                    latitudes: [51.5002, 40.4300],
        //                    longitudes: [-0.1262, -74.0000]
        //                }],
                       
        //            },
        //                {
        //                    id: "vilnius",
        //                    color: "#000000",
        //                    svgPath: targetSVG,
        //                    title: "Vilnius",
        //                    latitude: 54.6896,
        //                    longitude: 25.2799,
        //                    scale: 1.5,
        //                    zoomLevel: 4.92,
        //                    zoomLongitude: 15.4492,
        //                    zoomLatitude: 50.2631,
        //                    lines: [{
        //                        latitudes: [54.6896, 50.8371],
        //                        longitudes: [25.2799, 4.3676]
        //                    }, {
        //                        latitudes: [54.6896, 59.9138],
        //                        longitudes: [25.2799, 10.7387]
        //                    }, {
        //                        latitudes: [54.6896, 40.4167],
        //                        longitudes: [25.2799, -3.7033]
        //                    }, {
        //                        latitudes: [54.6896, 50.0878],
        //                        longitudes: [25.2799, 14.4205]
        //                    }, {
        //                        latitudes: [54.6896, 48.2116],
        //                        longitudes: [25.2799, 17.1547]
        //                    }, {
        //                        latitudes: [54.6896, 44.8048],
        //                        longitudes: [25.2799, 20.4781]
        //                    }, {
        //                        latitudes: [54.6896, 55.7558],
        //                        longitudes: [25.2799, 37.6176]
        //                    }, {
        //                        latitudes: [54.6896, 37.9792],
        //                        longitudes: [25.2799, 23.7166]
        //                    }, {
        //                        latitudes: [54.6896, 54.6896],
        //                        longitudes: [25.2799, 25.2799]
        //                    }, {
        //                        latitudes: [54.6896, 51.5002],
        //                        longitudes: [25.2799, -0.1262]
        //                    }, {
        //                        latitudes: [54.6896, 53.3441],
        //                        longitudes: [25.2799, -6.2675]
        //                    }],

                           
        //                }, {
        //                    svgPath: targetSVG,
        //                    title: "Brussels",
        //                    latitude: 50.8371,
        //                    longitude: 4.3676
        //                }, {
        //                    svgPath: targetSVG,
        //                    title: "Prague",
        //                    latitude: 50.0878,
        //                    longitude: 14.4205
        //                }, {
        //                    svgPath: targetSVG,
        //                    title: "Athens",
        //                    latitude: 37.9792,
        //                    longitude: 23.7166
        //                }, {
        //                    svgPath: targetSVG,
        //                    title: "Reykjavik",
        //                    latitude: 64.1353,
        //                    longitude: -21.8952
        //                }, {
        //                    svgPath: targetSVG,
        //                    title: "Dublin",
        //                    latitude: 53.3441,
        //                    longitude: -6.2675
        //                }, {
        //                    svgPath: targetSVG,
        //                    title: "Oslo",
        //                    latitude: 59.9138,
        //                    longitude: 10.7387
        //                }, {
        //                    svgPath: targetSVG,
        //                    title: "Lisbon",
        //                    latitude: 38.7072,
        //                    longitude: -9.1355
        //                }, {
        //                    svgPath: targetSVG,
        //                    title: "Moscow",
        //                    latitude: 55.7558,
        //                    longitude: 37.6176
        //                }, {
        //                    svgPath: targetSVG,
        //                    title: "Belgrade",
        //                    latitude: 44.8048,
        //                    longitude: 20.4781
        //                }, {
        //                    svgPath: targetSVG,
        //                    title: "Bratislava",
        //                    latitude: 48.2116,
        //                    longitude: 17.1547
        //                }, {
        //                    svgPath: targetSVG,
        //                    title: "Ljubljana",
        //                    latitude: 46.0514,
        //                    longitude: 14.5060
        //                }, {
        //                    svgPath: targetSVG,
        //                    title: "Madrid",
        //                    latitude: 40.4167,
        //                    longitude: -3.7033
        //                }, {
        //                    svgPath: targetSVG,
        //                    title: "Stockholm",
        //                    latitude: 59.3328,
        //                    longitude: 18.0645
        //                }, {
        //                    svgPath: targetSVG,
        //                    title: "Bern",
        //                    latitude: 46.9480,
        //                    longitude: 7.4481
        //                }, {
        //                    svgPath: targetSVG,
        //                    title: "Kiev",
        //                    latitude: 50.4422,
        //                    longitude: 30.5367
        //                }, {
        //                    svgPath: targetSVG,
        //                    title: "Paris",
        //                    latitude: 48.8567,
        //                    longitude: 2.3510
        //                }, {
        //                    svgPath: targetSVG,
        //                    title: "New York",
        //                    latitude: 40.43,
        //                    longitude: -74
        //                }
        //            ]
        //        },
        //        areasSettings: {
        //            unlistedAreasColor: "#66b3ff"
        //        },
        //        imagesSettings: {
        //            color: "#2E2E2E",
        //            rollOverColor: "#2E2E2E",
        //            selectedColor: "#000000"
        //        },
        //        linesSettings: {
        //            color: "#2E2E2E",
        //            alpha: 0.4
        //        },
        //        backgroundZoomsToTop: true,
        //        linesAboveImages: true
        //    });

        //},


       
        orgStructurePopup: function (datascource) {
            
            var secondMenu = '';


            $('#chart-container').orgchart({
                'data': datascource,
                'nodeContent': 'title',
                'nodeID': 'id',
                'createNode': function ($node, data) {

                    if (data.type == "businessowner") {
                        console.log('businessowner');
                        secondMenu = '<div dropdown="" class="second-menu dropdown-toggle" data-toggle="dropdown" aria-haspopup="true"><ul class="orgPopList">'
                        + '<li><a href="javascript:;" ng-click="loadUserManagment(0,\'Manager\')">Manager</a></li><li><a href="javascript:;" ng-click="loadUserManagment(0,\'Supervisor\')">Supervisor</a></li><li><a href="javascript:;" ng-click="loadUserManagment(0,\'Operator\')">Operator</a></li><li><a href="javascript:;" ng-click="loadDivisionManagment(0)">Division</a></li>'
                        + '</ul></div>';
                    }
                    else if (data.type == "manager"){
                        console.log('manager');
                        secondMenu = '<div dropdown="" class="second-menu dropdown-toggle" data-toggle="dropdown" aria-haspopup="true"><ul class="orgPopList">'
                        + '<li><a href="javascript:;" ng-click="loadUserManagment(0,\'Supervisor\')">Supervisor</a></li><li><a href="javascript:;" ng-click="loadUserManagment(0,\'Operator\')">Operator</a></li><li><a href="javascript:;" ng-click="loadDivisionManagment(0)">Division</a></li>'
                        + '</ul></div>';
                    }
                    else if (data.type == "supervisor") {
                        console.log('supervisor');
                        secondMenu = '<div dropdown="" class="second-menu dropdown-toggle" data-toggle="dropdown" aria-haspopup="true"><ul class="orgPopList">'
                        + '<li><a href="javascript:;" ng-click="loadDivisionManagment(0,\'Supervisor\',\'' + data.id + '\')">Division</a></li></ul></div>';
                    }
                    else if (data.type == "division") {
                        secondMenu = '<div dropdown="" class="second-menu dropdown-toggle" data-toggle="dropdown" aria-haspopup="true"><ul class="orgPopList">'
                        + '<li><a href="javascript:;" ng-click="loadUserManagment(0,\'Operator\',\'Division\',\'' + data.id + '\')">Operator</a></li><li><a href="javascript:;" ng-click="loadCostcenterManagement(0,\'Division\',\'' + data.id + '\')">Cost Center</a></li></ul></div>';
                    }
                    var secondMenuIcon = $('<i>', {
                        'class': 'fa fa-plus-circle second-menu-icon',
                        click: function () {
                         //$(this).addClass('fa-plus-circle').removeClass('fa-minus-circle');
                            if ($(this).siblings('.second-menu').is(":visible")) {                         
                                $(this).siblings('.second-menu').hide();
                                $(this).removeClass('fa-minus-circle').addClass('fa-plus-circle');
                            }
                            else {
                                
                               // alert($(this).attr('class'));
                                $(".second-menu").each(function () {
                                    $(this).hide();
                                    
                                });

                                $(".fa-minus-circle").each(function () {
                                    $(this).removeClass('fa-minus-circle').addClass('fa-plus-circle');
                                });
                                $(this).addClass('fa-minus-circle').removeClass('fa-plus-circle');
                                $(this).siblings('.second-menu').show();
                                
                            }
                            
                        }
                    });
                    
                   // var secondMenu = '<div class="second-menu"><div dropdown="" class="btn-group"><button data-toggle="dropdown" class="btn btn-default dropdown-toggle" type="button" aria-haspopup="true" data-toggle="dropdown"><span class="caret"></span></button><span class="dropdown-arrow"></span><ul role="menu" class="dropdown-menu"><li><a href="javascript:;" ng-click="loadUserManagment(0)">add user1</a></li><li><a href="javascript:;" ng-click="loadDivisionManagment(0)">add user2</a></li><li><a href="javascript:;" ng-click="loadCostcenterManagement(0)">add user3</a></li></ul></div></div>';
                    if (data.type != "operator" && data.name != "Division - Inactive" && data.name != "Supervisor - Inactive") {
                        $node.append(secondMenuIcon).append(secondMenu);
                    }
                }
            });
        }
        
        };
    return customBuilderFactory;
}]);
