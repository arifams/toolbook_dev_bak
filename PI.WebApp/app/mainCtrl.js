(function (app) {

    app.factory('userService', function ($http, $window) {
    return {
        getUserName: function () {
            
            return $http.get(serverBaseUrl + '/api/accounts/GetLoggedInUserName', {
                params: {
                    loggedInUserId: $window.localStorage.getItem('userGuid')
                }
            });
        }
    }

});


app.controller('mainCtrl',
    ['$scope', 'applicationService', 'quickViewService', 'builderService', 'pluginsService', 'userService', '$location', '$window',
function ($scope, applicationService, quickViewService, builderService, pluginsService, userService, $location, $window) {
        
    $(document).ready(function () {
        
        applicationService.init();
        quickViewService.init();
        builderService.init();
        pluginsService.init();
        Dropzone.autoDiscover = false;
    });

    $scope.$on('$viewContentLoaded', function () {
        pluginsService.init();
        applicationService.customScroll();
        applicationService.handlePanelAction();
        $('.nav.nav-sidebar .nav-active').removeClass('nav-active active');
        $('.nav.nav-sidebar .active:not(.nav-parent)').closest('.nav-parent').addClass('nav-active active');

        if($location.$$path == '/' || $location.$$path == '/layout-api'){
            $('.nav.nav-sidebar .nav-parent').removeClass('nav-active active');
            $('.nav.nav-sidebar .nav-parent .children').removeClass('nav-active active');
            if ($('body').hasClass('sidebar-collapsed') && !$('body').hasClass('sidebar-hover')) return;
            if ($('body').hasClass('submenu-hover')) return;
            $('.nav.nav-sidebar .nav-parent .children').slideUp(200);
            $('.nav-sidebar .arrow').removeClass('active');
        }
        if($location.$$path == '/'){
            $('body').addClass('dashboard');
        }
        else{
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


    if ($window.localStorage.getItem('userGuid') == '' || $window.localStorage.getItem('userGuid') == undefined) {
        window.location = webBaseUrl + "/app/userLogin/userLogin.html";
    }

    $scope.currentRole = $window.localStorage.getItem('userRole');
    $scope.isCorporate = $window.localStorage.getItem('isCorporateAccount');

}]);


})(angular.module('newApp'));