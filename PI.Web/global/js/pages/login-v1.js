function copyrightPos() {
    var windowHeight = $(window).height();
    if (windowHeight < 700) {
        $('.account-copyright').css('position', 'relative').css('margin-top', 40);
    } else {
        $('.account-copyright').css('position', '').css('margin-top', '');
    }
}

$(window).resize(function() {
    copyrightPos();
});

$(function() {

    copyrightPos();
    if ($('body').data('page') == 'login') {

        /* Show / Hide Password Recover Form */
        $('#password').on('click', function(e) {
            e.preventDefault();
            $('.form-signin').slideUp(300, function() {
                $('.form-password').slideDown(300);
            });
        });
        $('#login').on('click', function(e) {
            e.preventDefault();
            $('.form-password').slideUp(300, function() {
                $('.form-signin').slideDown(300);
            });
        });

        //var form = $(".form-signin");
        //$('#submit-form').click(function(e) {
        //    form.validate({
        //        rules: {
        //            username: {
        //                required: true,
        //                minlength: 3,
        //            },
        //            password: {
        //                required: true,
        //                minlength: 8,
        //                maxlength: 20
        //            }
        //        },
        //        messages: {
        //            username: {
        //                required: 'Enter your username'
        //            },
        //            password: {
        //                required: 'Write your password'
        //            }
        //        },
        //        errorPlacement: function(error, element) {
        //            error.insertAfter(element);
        //        }
        //    });
        //    //e.preventDefault();
        //    //if (form.valid()) {
        //    //    $(this).addClass('ladda-button');
        //    //    var l = Ladda.create(this);
        //    //    l.start();
        //    //    setTimeout(function() {
        //    //        window.location.href = "dashboard.html";
        //    //    }, 2000);
        //    //} else {
        //    //    $('body').addClass('boxed');
        //    //    // alert('not valid');
        //    //}
        //});


        //$('#submit-form, #submit-password').click(function() {

        //    e.preventDefault();
        //    var l = Ladda.create(this);
        //    l.start();
        //    setTimeout(function() {
        //        window.location.href = "dashboard.html";
        //    }, 2000);
        //});
        $.backstretch(["../../global/images/gallery/login.jpg"], {
            fade: 600,
            duration: 4000
        });


        /***** DEMO CONTENT, CAN BE REMOVED ******/
        $("#account-builder").on('mouseenter', function() {
            TweenMax.to($(this), 0.35, {
                css: {
                    height: 160,
                    width: 500,
                    left: '37%',
                    'border-bottom-left-radius': 0,
                    'border-top-right-radius': 0,
                    '-moz-border-bottom-left-radius': 0,
                    '-moz-border-top-right-radius': 0,
                    '-webkit-border-bottom-left-radius': 0,
                    '-webkit-border-top-right-radius': 0
                },
                ease: Circ.easeInOut
            });
        });
        $("#account-builder").on('mouseleave', function() {
            TweenMax.to($(this), 0.35, {
                css: {
                    height: 44,
                    width: 250,
                    left: '44%',
                    'border-bottom-left-radius': 20,
                    'border-top-right-radius': 20
                },
                ease: Circ.easeInOut
            });
        });
        /* Hide / Show Social Connect */
        $('#social-cb').change(function() {
            if ($(this).is(":checked")) {
                $('.social-btn').slideDown(function() {
                    $('body').removeClass('no-social');
                });
            } else {
                $('.social-btn').slideUp(function() {
                    $('body').addClass('no-social');
                });
            }
        });
        /* Hide / Show Boxed Form */
        $('#boxed-cb').change(function() {
            if ($(this).is(":checked")) {
                TweenMax.to($('.account-wall'), 0.5, {
                    backgroundColor: 'rgba(255, 255, 255,1)',
                    ease: Circ.easeInOut,
                    onComplete: function() {
                        $('body').addClass('boxed');
                    }
                });
            } else {
                TweenMax.to($('.account-wall'), 0.5, {
                    backgroundColor: 'rgba(255, 255, 255,0)',
                    ease: Circ.easeInOut,
                    onComplete: function() {
                        $('body').removeClass('boxed');
                    }
                });
            }
        });
        /* Hide / Show Background Image */
        $('#image-cb').change(function() {
            if ($(this).is(":checked")) {
                $.backstretch(["../../global/images/gallery/login.jpg"], {
                    fade: 600,
                    duration: 4000
                });
                $('#slide-cb').attr('checked', false);
            } else $.backstretch("destroy");
        });
        /* Add / Remove Slide Image */
        $('#slide-cb').change(function() {
            if ($(this).is(":checked")) {
                $.backstretch(["../assets/global/images/gallery/login4.jpg", "../assets/global/images/gallery/login3.jpg", "../assets/global/images/gallery/login2.jpg", "../../global/images/gallery/login.jpg"], {
                    fade: 600,
                    duration: 4000
                });
                $('#image-cb').attr('checked', false);
            } else {
                $.backstretch("destroy");
            }
        });
        /* Separate Inputs */
        $('#input-cb').change(function() {
            if ($(this).is(":checked")) {
                TweenMax.to($('.username'), 0.3, {
                    css: {
                        marginBottom: 8,
                        'border-bottom-left-radius': 2,
                        'border-bottom-right-radius': 2
                    },
                    ease: Circ.easeInOut,
                    onComplete: function() {
                        $('body').addClass('separate-inputs');
                    }
                });
                TweenMax.to($('.password'), 0.3, {
                    css: {
                        'border-top-left-radius': 2,
                        'border-top-right-radius': 2
                    },
                    ease: Circ.easeInOut
                });
            } else {
                TweenMax.to($('.username'), 0.3, {
                    css: {
                        marginBottom: -1,
                        'border-bottom-left-radius': 0,
                        'border-bottom-right-radius': 0
                    },
                    ease: Circ.easeInOut,
                    onComplete: function() {
                        $('body').removeClass('separate-inputs');
                    }
                });
                TweenMax.to($('.password'), 0.3, {
                    css: {
                        'border-top-left-radius': 0,
                        'border-top-right-radius': 0
                    },
                    ease: Circ.easeInOut
                });
            }
        });
        /* Hide / Show User Image */
        $('#user-cb').change(function() {
            if ($(this).is(":checked")) {
                TweenMax.to($('.user-img'), 0.3, {
                    opacity: 0,
                    ease: Circ.easeInOut
                });
            } else {
                TweenMax.to($('.user-img'), 0.3, {
                    opacity: 1,
                    ease: Circ.easeInOut
                });
            }
        });

    }
    if ($('body').data('page') == 'signup') {

        var form = $(".form-signup");
        $.backstretch(["../../global/images/gallery/login.jpg"], {
            fade: 600,
            duration: 4000
        });
        $("#account-builder").on('mouseenter', function() {
            TweenMax.to($(this), 0.35, {
                css: {
                    height: 160,
                    width: 500,
                    left: '37%',
                    'border-bottom-left-radius': 0,
                    'border-top-right-radius': 0,
                    '-moz-border-bottom-left-radius': 0,
                    '-moz-border-top-right-radius': 0,
                    '-webkit-border-bottom-left-radius': 0,
                    '-webkit-border-top-right-radius': 0
                },
                ease: Circ.easeInOut
            });
        });
        $("#account-builder").on('mouseleave', function() {
            TweenMax.to($(this), 0.35, {
                css: {
                    height: 44,
                    width: 250,
                    left: '44%',
                    'border-bottom-left-radius': 20,
                    'border-top-right-radius': 20
                },
                ease: Circ.easeInOut
            });
        });
        /* Hide / Show Social Connect */
        $('#social-cb').change(function() {
            if ($(this).is(":checked")) {
                $('.social-btn').slideDown(function() {
                    $('body').removeClass('no-social');
                });
            } else {
                $('.social-btn').slideUp(function() {
                    $('body').addClass('no-social');
                });
            }
        });
        /* Hide / Show Boxed Form */
        $('#boxed-cb').change(function() {
            if ($(this).is(":checked")) {
                TweenMax.to($('.account-wall'), 0.5, {
                    backgroundColor: 'rgba(255, 255, 255,1)',
                    ease: Circ.easeInOut,
                    onComplete: function() {
                        $('body').addClass('boxed');
                    }
                });
            } else {
                TweenMax.to($('.account-wall'), 0.5, {
                    backgroundColor: 'rgba(255, 255, 255,0)',
                    ease: Circ.easeInOut,
                    onComplete: function() {
                        $('body').removeClass('boxed');
                    }
                });
            }
        });
        /* Hide / Show Background Image */
        $('#image-cb').change(function() {
            if ($(this).is(":checked")) {
                $.backstretch(["../../global/images/gallery/login.jpg"], {
                    fade: 600,
                    duration: 4000
                });
                $('#slide-cb').attr('checked', false);
            } else $.backstretch("destroy");
        });
        /* Add / Remove Slide Image */
        $('#slide-cb').change(function() {
            if ($(this).is(":checked")) {
                $.backstretch(["../assets/global/images/gallery/login4.jpg", "../assets/global/images/gallery/login3.jpg", "../assets/global/images/gallery/login2.jpg", "../../global/images/gallery/login.jpg"], {
                    fade: 600,
                    duration: 4000
                });
                $('#image-cb').attr('checked', false);
            } else {
                $.backstretch("destroy");
            }
        });
        /* Hide / Show Terms Checkbox */
        $('#terms-cb').change(function() {
            if ($(this).is(":checked")) {
                $('.terms').slideDown(function() {
                    $('body').removeClass('no-terms');
                });
            } else {
                $('.terms').slideUp(function() {
                    $('body').addClass('no-terms');
                });
            }
        });
        /* Hide / Show User Image */
        $('#user-cb').change(function() {
            if ($(this).is(":checked")) {
                TweenMax.to($('.user-img'), 0.3, {
                    opacity: 0,
                    ease: Circ.easeInOut
                });
            } else {
                TweenMax.to($('.user-img'), 0.3, {
                    opacity: 1,
                    ease: Circ.easeInOut
                });
            }
        });
          
        //change contact type according to the drop down value
            $("#contacttype").change(function () {
                $(this).find("option:selected").each(function () {
                   
                    if ($(this).attr("value") == "Phone") {
                        $("#phonenumberdiv").show();
                        $("#mobilenumberdiv").hide();
                    }
                    else if ($(this).attr("value") == "Mobile") {
                        $("#phonenumberdiv").hide();
                        $("#mobilenumberdiv").show();
                    }                    
                });
            }).change();
        
        //validate the password
         jQuery.validator.addMethod("ValidPassword", function (value, element) {
            return this.optional(element) || /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d.*)(?=.*\W.*)[a-zA-Z0-9\S]{8,20}$/.test(value);
         }, "Please enter valid password");

         

      //validate the phone number
         jQuery.validator.addMethod("ValidPhoneNumber", function (value, element) {
             return this.optional(element) || /^[0-9()+-]*$/.test(value);
         }, "Please enter valid phone Number");

        //hide the error message and company name on page load
         $('#error-message').hide();
         $('#companynamediv').hide();


         $('input[name=iscorporate]:radio').on('ifChecked', function (event) {
             if (event.target.value == 'True')
                 $('#companynamediv').hide();
             else if (event.target.value == 'False')
                 $('#companynamediv').show();

         });

        $('#submit-form').click(function(e) {
            form.validate({
                rules: {
                    salutation: {
                        required:true
                    },
                    firstname: {
                        required: true,
                        minlength: 3,
                    },
                    lastname: {
                        required: true,
                        minlength: 4,
                    },
                    email: {
                        required: true,
                        email: true
                    },
                    password: {
                        ValidPassword: true,
                        required: true,
                        minlength: 6,
                        maxlength: 16
                    },
                    password_c: {
                        required: true,
                        minlength: 6,
                        maxlength: 16,
                        equalTo: '#password'
                    },
                    mobilenumber: {
                        ValidPhoneNumber:true,
                        required: true,
                        
                    },
                    phonenumber: {
                        ValidPhoneNumber: true,
                        required: true,
                       
                    },
                    companyname: {
                        required: true
                       
                    },
                    postalcode: {
                        required: true
                       
                    },
                    number: {
                        required: true,
                        number:true
                      
                    },
                    street: {
                        required: true
                       
                    },
                    city: {
                        required: true
                       
                    },
                    state: {
                        required: true
                       
                    },
                    terms: {
                        required: true
                    }
                },
                messages: {
                    salutation:{
                    required:'Please select the Salutation'
                    },
                    firstname: {
                        required: 'Enter your first name',
                        minlength: 'Enter at least 3 characters or more'
                    },
                    lastname: {
                        required: 'Enter your last name',
                        minlength: 'Enter at least 3 characters or more'
                    },
                    email: {
                        required: 'Enter email address',
                        email: 'Enter a valid email address'
                    },
                    password: {
                       
                        required: 'Enter your password',
                        minlength: 'Minimum 8 characters',
                        maxlength: 'Maximum 20 characters'
                    },
                    password_c: {
                        required: 'ReEnter your password',
                        minlength: 'Minimum 8 characters',
                        maxlength: 'Maximum 20 characters',
                        equalTo: '2 passwords must be the same'
                    },
                    phonenumber: {
                        required: 'Enter Phone Number',
                        number: 'Enter Valid Phone Number',
                    },
                    
                    mobilenumber: {
                        required: 'Enter Mobile Number',
                        number: 'Enter Valid Mobile Number',
                    },

                    companyname: {
                        required: 'Enter Company name',
                        
                    },
                    postalcode: {
                        required: 'Enter Postal Code',
                       
                    },
                    number: {
                        required: 'Enter Number',
                        number:'Enteter valid Number'
                       
                    },
                    street: {
                        required: 'Enter Street Address 1',
                        
                    },
                    city: {
                        required: 'Enter City',
                       
                    },
                    state: {
                        required: 'Enter State' ,
                       
                    },
                    terms: {
                        required: 'You must agree with terms'
                    }
                },
                errorPlacement: function(error, element) {
                    if (element.is(":radio") || element.is(":checkbox")) {
                        element.closest('.option-group').after(error);
                    } else {
                        error.insertAfter(element);
                    }
                }
            });
            e.preventDefault();
            if (form.valid()) {            

                var result = {};
                $.each($('#form-signup').serializeArray(), function () {
                    result[this.name] = this.value;
                });              

                var newUser = {            
                    Salutation: result.salutation,
                    FirstName: result.firstname,
                    LastName: result.lastname,
                    MiddleName: result.middlename,
                    Email: result.email,
                    PhoneNumber: result.phonenumber,
                    MobileNumber: result.mobilenumber,
                    IsCorporateAccount: result.iscorporate,
                    CompanyName: result.companyname,
                    CustomerAddress: {
                        Country: result.country,
                        ZipCode: result.postalcode,
                        State: result.state,
                        City: result.city,
                        Number: result.number,
                        StreetAddress1: result.street,
                        StreetAddress2: result.additionadetails
                    },
                    Password: result.password
                }
               // var jqxhr = $.post('http://localhost:5555/api/User/createuser', newUser)
               var jqxhr = $.post('https://pibooking.azurewebsites.net/api/User/createuser', newUser)
             .success(function () {
                 var loc = jqxhr.getResponseHeader('Location');
                 var responce = jqxhr.responseJSON;

                 if (responce==1) {
                     setTimeout(function () {
                         window.location.href = "../../app/index.html";
                     }, 2000);
                 }
                 else if (responce == -1) {
                     $('#error-message').show();
                 }
                
             })
             .error(function () {
                 $('#message').html("Error posting the update.");
             });
                
                //$(this).addClass('ladda-button');
                //alert('valide');
                //var l = Ladda.create(this);
                //l.start();
               
            } else {
                // alert('not valid');
            }
        });

    }
});