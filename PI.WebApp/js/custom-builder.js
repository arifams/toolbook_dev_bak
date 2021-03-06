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

        selectRateRow: function () {
            $("#selectRates tr").click(function () {
                $(this).addClass('rowSelected').siblings().removeClass('rowSelected');
                var value = $(this).find('td:first').html();
                //alert('shjh');
            });
        },

        customScroll : function () {
            if ($.fn.mCustomScrollbar) {
                $('.withScroll').each(function() {
                    $(this).mCustomScrollbar("destroy");
                    var scroll_height = $(this).data('height') ? $(this).data('height') : 'auto';
                    var data_padding = $(this).data('padding') ? $(this).data('padding') : 0;
                    if ($(this).data('height') == '100px') {
                        thisHeight = $(this).height();
                        windowHeight = $(window).height() - data_padding - 50;
                        if (thisHeight < windowHeight) scroll_height = thisHeight;
                        else scroll_height = windowHeight;
                    }
                    $(this).mCustomScrollbar({
                        scrollButtons: {
                            enable: false
                        },
                        autoHideScrollbar: $(this).hasClass('show-scroll') ? false : true,
                        scrollInertia: 150,
                        theme: "light",
                        set_height: scroll_height,
                        advanced: {
                            updateOnContentResize: true
                        }
                    });
                });
            }
        },
       
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
