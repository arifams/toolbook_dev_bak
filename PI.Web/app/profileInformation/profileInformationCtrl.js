'use strict';


(function (app) {

    app.factory('updateProfilefactory', function ($http) {
        return {
                updateProfileInfo: function (updatedProfile) {

                $http.post('api/profile/UpdateProfile', updatedProfile).then(function successCallback(response) {
                    if (response.data != null) {
                        return response;
                    }

                }, function errorCallback(response) {
                    return null;
                })
            }
        }

    })

  
    app.factory('loadProfilefactory', function ($http) {
        return {           
            loadProfileinfo: function () {
                $http.get('http://localhost:5555/api/profile/GetProfile', {
                    params: {
                        username: 'dilshan@amarasinghe'
                    }
                });
            }
        }

    });

    //app.directive('validPasswordC', function () {
    //    return {
    //        require: 'ngModel',
    //        link: function (scope, elm, attrs, ctrl) {
    //            ctrl.$parsers.unshift(function (viewValue, $scope) {
    //                var noMatch = viewValue != scope.newUserRegisterForm.password.$viewValue
    //                ctrl.$setValidity('noMatch', !noMatch)
    //            })
    //        }
    //    }
    //})

    app.controller('profileInformationCtrl',
        ['loadProfilefactory', 'updateProfilefactory', function (loadProfilefactory, updateProfilefactory) {
            var vm = this;

            vm.loadProfile = function () {

                loadProfilefactory.loadProfileinfo()
                    .then(function (result) {
                        if (result.data != null) {

                            var profile = result.data;
                            return profile;
                        }                       

                    }, function (error) {
                        console.log("failed");
                    });
            }

            vm.updateProfile = function () {
                updateProfilefactory.updateProfileInfo()
                .then(function (result) {
                    if (result.data != null) {

                        //TO DO
                    }
                    
                }, function (error) {
                    console.log("failed");
                });
            }
        }]);


})(angular.module('newApp'));

