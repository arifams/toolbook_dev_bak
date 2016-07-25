angular.module('newApp')
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

        $('body').removeClass(function (profileInformation, css) {
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

            if (mColor != undefined && mName != undefined)
                setColor(mColor, mName);
           
            applicationService.resetStyle();

            if ($('body').hasClass('sidebar-top')) {
                destroySideScroll();
            }
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

       
        orgStructurePopup: function (datascource) {
            
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
                    var secondMenu = '<div class="second-menu"><div dropdown="" class="btn-group"><button data-toggle="dropdown" class="btn btn-default dropdown-toggle" type="button" aria-haspopup="true" data-toggle="dropdown"><span class="caret"></span></button><span class="dropdown-arrow"></span><ul role="menu" class="dropdown-menu"><li><a href="javascript:;" ng-click="loadUserManagment(0)">add user1</a></li><li><a href="javascript:;" ng-click="loadDivisionManagment(0)">add user2</a></li><li><a href="javascript:;" ng-click="loadCostcenterManagement(0)">add user3</a></li></ul></div></div>';
                    $node.append(secondMenuIcon).append(secondMenu);
                }
            });
        }
        
        };
    return customBuilderFactory;
}]);
