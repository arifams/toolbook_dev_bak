'use strict';


(function(app){
    
    app.factory('registerUserService', function ($http) {            
         
        return{
            createUser : function (newuser) {
            return $http.post('http://localhost:5555/api/User/CreateUser',newuser);
        }
        };
      
        });

    app.controller('userRegistrationCtrl', ['registerUserService', function (registerUserService) {
        var vm = this;
        vm.iscorporate={
            name:'False'
        }


        vm.register = function (user) {          

            var newUser = {               
                Salutation:user.salutation,
                FirstName: user.firstname,
                LastName: user.lastname,
                MiddleName: user.middlename,
                Email: user.email,
                PhoneNumber: user.contact,
                Password: user.password,
                ConfirmPassword: user.confirmpassword,
                IsCorporateAccount: user.iscorporate,
                CompanyName: user.companyname,
                CustomerAddress:
                {
                   Country : user.country,
                   ZipCode :user.postalcode,
                   StreetAddress1 :user.street,
                   StreetAddress2 :user.additionaldetails,
                   City :user.city,
                   State:user.state
                }
            }
          registerUserService.createUser(newUser)
            .then(function (result)
            {
                console.log("success");
            },
            function (error) {
                console.log("failed");
            }
            );
        };
    }]);
        

})(angular.module('userRegistration', []));

