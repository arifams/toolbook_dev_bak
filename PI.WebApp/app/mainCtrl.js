(function (app) {

    app.factory('userService', function ($http, $window) {

        return {
            getUserName: getUserName,
            getCompanyName: getCompanyName,
            getLogoUrl: getLogoUrl,
            getThemeColour: getThemeColour
        };


        function getCompanyName() {

            return $http.get(serverBaseUrl + '/api/Company/GetCompanyByUserId', {
                params: {
                    loggedInUserId: $window.localStorage.getItem('userGuid')
                }
            });

        }


        function getUserName() {

            return $http.get(serverBaseUrl + '/api/accounts/GetLoggedInUserName', {
                params: {
                    loggedInUserId: $window.localStorage.getItem('userGuid')
                }
            });
        }

        function getLogoUrl() {

            return $http.get(serverBaseUrl + '/api/Company/GetLogoUrl', {
                params: {
                    loggedInUserId: $window.localStorage.getItem('userGuid')
                }
            });
        }

        function getThemeColour() {

            return $http.get(serverBaseUrl + '/api/Customer/GetThemeColour', {
                params: {
                    loggedInUserId: $window.localStorage.getItem('userGuid')
                }
            });
        }

    });


    app.controller('mainCtrl',
        ['$scope', 'applicationService', 'quickViewService', 'builderService', 'pluginsService', 'userService',
            '$location', '$window', '$route','gettextCatalog',
    function ($scope, applicationService, quickViewService, builderService, pluginsService, userService,
        $location, $window, $route, gettextCatalog) {

        $(document).ready(function () {

            applicationService.init();
            quickViewService.init();
            builderService.init();
            pluginsService.init();
            Dropzone.autoDiscover = false;

            builderService.init('#C75757', 'red');

        });

        $scope.$on('$viewContentLoaded', function () {
            pluginsService.init();
            applicationService.customScroll();
            applicationService.handlePanelAction();
            $('.nav.nav-sidebar .nav-active').removeClass('nav-active active');
            $('.nav.nav-sidebar .active:not(.nav-parent)').closest('.nav-parent').addClass('nav-active active');

            if ($location.$$path == '/' || $location.$$path == '/layout-api') {
                $('.nav.nav-sidebar .nav-parent').removeClass('nav-active active');
                $('.nav.nav-sidebar .nav-parent .children').removeClass('nav-active active');
                if ($('body').hasClass('sidebar-collapsed') && !$('body').hasClass('sidebar-hover')) return;
                if ($('body').hasClass('submenu-hover')) return;
                $('.nav.nav-sidebar .nav-parent .children').slideUp(200);
                $('.nav-sidebar .arrow').removeClass('active');
            }
            if ($location.$$path == '/') {
                $('body').addClass('dashboard');
            }
            else {
                $('body').removeClass('dashboard');
            }

        });



        //console.log($cookieStore.get("KEY"));
        $scope.isActive = function (viewLocation) {
            return viewLocation === $location.path();
        };

        // Get user name
        userService.getUserName()
            .then(function successCallback(responce) {
                $scope.userName = responce.data;
            });

        
        userService.getThemeColour()
                  .then(function successCallback(responce) {
                      $scope.userName = responce.data;
                  });

        
        userService.getLogoUrl()
           .then(function successCallback(responce) {
               $scope.logoUrl = responce.data;
           });

        userService.getCompanyName().then(function successCallback(responce) {
            debugger;
            $scope.companyName = responce.data.name;
        });

        $scope.getCurrentLnguage = function (language) {
            if (language == "en") {
                $window.localStorage.setItem('currentLnguage', "")
                gettextCatalog.setCurrentLanguage("");
            }
            else {
                $window.localStorage.setItem('currentLnguage', language);
                gettextCatalog.setCurrentLanguage(language);
            }
            //$route.reload();
        };

        if ($window.localStorage.getItem('userGuid') == '' || $window.localStorage.getItem('userGuid') == undefined) {
            window.location = webBaseUrl + "/app/userLogin/userLogin.html";
        }

        $scope.currentRole = $window.localStorage.getItem('userRole');
        $scope.isCorporate = $window.localStorage.getItem('isCorporateAccount');

    }]);


})(angular.module('newApp'));