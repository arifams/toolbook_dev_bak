angular.module('newApp')
.factory('builderService', ['applicationService', function (applicationService){

    var builderService = {};

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
            if ($(this).data('main') == 'default') {
                $('.theme-left').css('background-color', '#202226');
                $('.theme-sidebar-dark, .theme-right-dark').css('background-color', '#393E44');
                $('.theme-sidebar-light, .theme-right-light').css('background-color', '#fff');
                $('.sltl .theme-left').css('background-color', '#fff');
            }
            if ($(this).data('main') == 'primary') {
                $('.theme-left').css('background-color', '#319DB5');
                $('.theme-sidebar-dark, .theme-right-dark').css('background-color', '#164954');
                $('.theme-sidebar-light, .theme-right-light').css('background-color', '#DDE6E9');
            }
            if ($(this).data('main') == 'red') {
                $('.theme-left').css('background-color', '#C9625F');
                $('.theme-sidebar-dark, .theme-right-dark').css('background-color', '#4E3232');
                $('.theme-sidebar-light, .theme-right-light').css('background-color', '#F8F3F1');
            }
            if ($(this).data('main') == 'green') {
                $('.theme-left').css('background-color', '#18A689');
                $('.theme-sidebar-dark, .theme-right-dark').css('background-color', '#24392E');
                $('.theme-sidebar-light, .theme-right-light').css('background-color', '#F1F8F3');
            }
            if ($(this).data('main') == 'orange') {
                $('.theme-left').css('background-color', '#C58627');
                $('.theme-sidebar-dark, .theme-right-dark').css('background-color', '#50361F');
                $('.theme-sidebar-light, .theme-right-light').css('background-color', '#F8F4F1');
            }
            if ($(this).data('main') == 'purple') {
                $('.theme-left').css('background-color', '#6E62B5');
                $('.theme-sidebar-dark, .theme-right-dark').css('background-color', '#393F51');
                $('.theme-sidebar-light, .theme-right-light').css('background-color', '#F3F2F7');
            }
            if ($(this).data('main') == 'blue') {
                $('.theme-left').css('background-color', '#4A89DC');
                $('.theme-sidebar-dark, .theme-right-dark').css('background-color', '#1E3948');
                $('.theme-sidebar-light, .theme-right-light').css('background-color', '#F2F4F7');
            }
            $.cookie('main-color', main_color, { path: '/' });
            $.cookie('main-name', main_name, { path: '/' });
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
        if ($(this).data('main') == 'default') {
            $('.theme-left').css('background-color', '#202226');
            $('.theme-sidebar-dark, .theme-right-dark').css('background-color', '#393E44');
            $('.theme-sidebar-light, .theme-right-light').css('background-color', '#fff');
            $('.sltl .theme-left').css('background-color', '#fff');
        }
        if ($(this).data('main') == 'primary') {
            $('.theme-left').css('background-color', '#319DB5');
            $('.theme-sidebar-dark, .theme-right-dark').css('background-color', '#164954');
            $('.theme-sidebar-light, .theme-right-light').css('background-color', '#DDE6E9');
        }
        if ($(this).data('main') == 'red') {
            $('.theme-left').css('background-color', '#C9625F');
            $('.theme-sidebar-dark, .theme-right-dark').css('background-color', '#4E3232');
            $('.theme-sidebar-light, .theme-right-light').css('background-color', '#F8F3F1');
        }
        if ($(this).data('main') == 'green') {
            $('.theme-left').css('background-color', '#18A689');
            $('.theme-sidebar-dark, .theme-right-dark').css('background-color', '#24392E');
            $('.theme-sidebar-light, .theme-right-light').css('background-color', '#F1F8F3');
        }
        if ($(this).data('main') == 'orange') {
            $('.theme-left').css('background-color', '#C58627');
            $('.theme-sidebar-dark, .theme-right-dark').css('background-color', '#50361F');
            $('.theme-sidebar-light, .theme-right-light').css('background-color', '#F8F4F1');
        }
        if ($(this).data('main') == 'purple') {
            $('.theme-left').css('background-color', '#6E62B5');
            $('.theme-sidebar-dark, .theme-right-dark').css('background-color', '#393F51');
            $('.theme-sidebar-light, .theme-right-light').css('background-color', '#F3F2F7');
        }
        if ($(this).data('main') == 'blue') {
            $('.theme-left').css('background-color', '#4A89DC');
            $('.theme-sidebar-dark, .theme-right-dark').css('background-color', '#1E3948');
            $('.theme-sidebar-light, .theme-right-light').css('background-color', '#F2F4F7');
        }
        $.cookie('main-color', main_color, { path: '/' });
        $.cookie('main-name', main_name, { path: '/' });
    }
	  
    builderService.init = function (mColor,mName) {
        "use strict";

        // $.removeCookie('main-color');
        // $.removeCookie('topbar-color');
        // $.removeCookie('topbar-color-custom');
        // $.removeCookie('sidebar-color');
        // $.removeCookie('sidebar-color-custom');
        // $.removeCookie('sidebar-hover');
        // $.removeCookie('submenu-hover');

        //toggleBuilder();
        //builderScroll();
        //handleLayout();
        //handleTheme();
        //handleCookie();
        mainColor();
        setColor(mColor, mName);
        //S
        applicationService.resetStyle();

        if ($('body').hasClass('sidebar-top')) {
            destroySideScroll();
        }

    };

    return builderService;
}]);