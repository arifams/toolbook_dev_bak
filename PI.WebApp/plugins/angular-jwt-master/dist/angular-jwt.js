(function () {


    // Create all modules and define dependencies to make sure they exist
    // and are loaded in the correct order to satisfy dependency injection
    // before all nested files are concatenated by Grunt

    // Modules
    angular.module('angular-jwt',
        [
            'angular-jwt.interceptor',
            'angular-jwt.jwt',
          
        ]);

    angular.module('angular-jwt.interceptor', [])
        
     .provider('jwtInterceptor', function () {

         this.urlParam = null;
         this.authHeader = 'Authorization';
         this.authPrefix = 'Bearer ';
         this.tokenGetter = function () {
             return null;
         }

         var config = this;

         this.$get = ["$q", "$injector", "$rootScope", "$window", function ($q, $injector, $rootScope, $window) {
             return {
                 request: function (request) {
                     if (request.skipAuthorization) {
                         return request;
                     }

                     if (config.urlParam) {
                         request.params = request.params || {};
                         // Already has the token in the url itself
                         if (request.params[config.urlParam]) {
                             return request;
                         }
                     } else {
                         request.headers = request.headers || {};
                         // Already has an Authorization header
                         if (request.headers[config.authHeader]) {
                             return request;
                         }
                     }

                     var tokenPromise = $q.when($injector.invoke(config.tokenGetter, this, {
                         config: request
                     }));

                     return tokenPromise.then(function (token) {
                         if (token) {
                             if (config.urlParam) {
                                 request.params[config.urlParam] = token;
                             } else {
                                 request.headers[config.authHeader] = config.authPrefix + token;
                             }
                         }
                         return request;
                     });
                 },
                 responseError: function (response) {
                    
                     // handle the case where the user is not authenticated
                     if (response.status === 401) {
                         var currentRole = $window.localStorage.getItem('userRole');

                         //redirect to login and clear the local storage
                         if (currentRole != 'Admin' && currentRole != 'BackOffice' && currentRole != 'FrontOffice') {
                             $window.location = webBaseUrl + "/app/userLogin/userLogin.html";
                         }
                         else {
                             $window.location = webBaseUrl + "/app/adminLogin/adminLogin.html";
                         }
                         $window.localStorage.setItem('lastLogin', null);

                         $rootScope.$broadcast('unauthenticated', response);
                     }
                     else if (response.status === 403) {
                         //redirect to login and clear the local storage

                         $window.location = webBaseUrl + "/app/httpError/httpError.html";
                         $window.localStorage.setItem('lastLogin', null);

                         $rootScope.$broadcast('forbidden', response);
                     }
                     return $q.reject(response);
                 },
                 response: function (response) {
                    

                     var currentToken = localStorage.getItem('token');
                     
                     var initInjector = angular.injector(['ng']);
                     var $http = initInjector.get('$http');
                        

                     function getNewToken() {
                         return $http.get(serverBaseUrl + '/api/accounts/GetNewSignedToken', {
                             params: {
                                 currentToken: currentToken
                             }
                         });
                     }
                                         
                     var token = getNewToken().success(function (data) {

                        
                         if (data!=null) {
                             $window.localStorage.setItem('token', data);
                         }
                       
                     })
                      .error(function () {

                           });
                    

                     return response;

              }
             };
         }];
     });

    angular.module('angular-jwt.jwt', [])
     .service('jwtHelper', function () {

         this.urlBase64Decode = function (str) {
             var output = str.replace(/-/g, '+').replace(/_/g, '/');
             switch (output.length % 4) {
                 case 0: { break; }
                 case 2: { output += '=='; break; }
                 case 3: { output += '='; break; }
                 default: {
                     throw 'Illegal base64url string!';
                 }
             }
             return decodeURIComponent(escape(window.atob(output))); //polifyll https://github.com/davidchambers/Base64.js
         }


         this.decodeToken = function (token) {
             var parts = token.split('.');

             if (parts.length !== 3) {
                 throw new Error('JWT must have 3 parts');
             }

             var decoded = this.urlBase64Decode(parts[1]);
             if (!decoded) {
                 throw new Error('Cannot decode the token');
             }

             return JSON.parse(decoded);
         }

         this.getTokenExpirationDate = function (token) {
             var decoded;
             decoded = this.decodeToken(token);

             if (typeof decoded.exp === "undefined") {
                 return null;
             }

             var d = new Date(0); // The 0 here is the key, which sets the date to the epoch
             d.setUTCSeconds(decoded.exp);

             return d;
         };

         this.isTokenExpired = function (token, offsetSeconds) {
             var d = this.getTokenExpirationDate(token);
             offsetSeconds = offsetSeconds || 0;
             if (d === null) {
                 return false;
             }

             // Token expired?
             return !(d.valueOf() > (new Date().valueOf() + (offsetSeconds * 1000)));
         };
     });

}());