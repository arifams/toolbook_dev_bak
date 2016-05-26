'use strict';


(function(app){
    
    var indexOf = [].indexOf || function (item) { for (var i = 0, l = this.length; i < l; i++) { if (i in this && this[i] === item) return i; } return -1; };

    //language list directive
    app.directive('countrySelect', function () {
        var allCountries;
        allCountries = [
          {
              code: 'AF',
              name: 'Afghanistan'
          }, {
              code: 'AL',
              name: 'Albania'
          }, {
              code: 'DZ',
              name: 'Algeria'
          }, {
              code: 'AS',
              name: 'American Samoa'
          }, {
              code: 'AD',
              name: 'Andorre'
          }, {
              code: 'AO',
              name: 'Angola'
          }, {
              code: 'AI',
              name: 'Anguilla'
          }, {
              code: 'AQ',
              name: 'Antarctica'
          }, {
              code: 'AG',
              name: 'Antigua and Barbuda'
          }, {
              code: 'AR',
              name: 'Argentina'
          }, {
              code: 'AM',
              name: 'Armenia'
          }, {
              code: 'AW',
              name: 'Aruba'
          }, {
              code: 'AU',
              name: 'Australia'
          }, {
              code: 'AT',
              name: 'Austria'
          }, {
              code: 'AZ',
              name: 'Azerbaijan'
          }, {
              code: 'BS',
              name: 'Bahamas'
          }, {
              code: 'BH',
              name: 'Bahrain'
          }, {
              code: 'BD',
              name: 'Bangladesh'
          }, {
              code: 'BB',
              name: 'Barbade'
          }, {
              code: 'BY',
              name: 'Belarus'
          }, {
              code: 'BE',
              name: 'Belgium'
          }, {
              code: 'BZ',
              name: 'Belize'
          }, {
              code: 'BJ',
              name: 'Benin'
          }, {
              code: 'BM',
              name: 'Bermuda'
          }, {
              code: 'BT',
              name: 'Bhutan'
          }, {
              code: 'BO',
              name: 'Bolivia'
          }, {
              code: 'BQ',
              name: 'Bonaire, Sint Eustatius and Saba'
          }, {
              code: 'BA',
              name: 'Bosnia and Herzegovina'
          }, {
              code: 'BW',
              name: 'Botswana'
          }, {
              code: 'BV',
              name: 'Bouvet Island'
          }, {
              code: 'BR',
              name: 'Brazil'
          }, {
              code: 'IO',
              name: 'British Indian Ocean Territory'
          }, {
              code: 'VG',
              name: 'British Virgin Islands'
          }, {
              code: 'BN',
              name: 'Brunei'
          }, {
              code: 'BG',
              name: 'Bulgaria'
          }, {
              code: 'BF',
              name: 'Burkina Faso'
          }, {
              code: 'BI',
              name: 'Burundi'
          }, {
              code: 'KH',
              name: 'Cambodia'
          }, {
              code: 'CM',
              name: 'Cameroon'
          }, {
              code: 'CA',
              name: 'Canada'
          }, {
              code: 'CV',
              name: 'Cape Verde'
          }, {
              code: 'KY',
              name: 'Cayman Islands'
          }, {
              code: 'CF',
              name: 'Central African Republic'
          }, {
              code: 'TD',
              name: 'Chad'
          }, {
              code: 'CL',
              name: 'Chile'
          }, {
              code: 'CN',
              name: 'China'
          }, {
              code: 'CX',
              name: 'Christmas Island'
          }, {
              code: 'CC',
              name: 'Cocos (Keeling) Islands'
          }, {
              code: 'CO',
              name: 'Colombia'
          }, {
              code: 'KM',
              name: 'Comoros'
          }, {
              code: 'CG',
              name: 'Congo'
          }, {
              code: 'CD',
              name: 'Congo (Dem. Rep.)'
          }, {
              code: 'CK',
              name: 'Cook Islands'
          }, {
              code: 'CR',
              name: 'Costa Rica'
          }, {
              code: 'ME',
              name: 'Crna Gora'
          }, {
              code: 'HR',
              name: 'Croatia'
          }, {
              code: 'CU',
              name: 'Cuba'
          }, {
              code: 'CW',
              name: 'Curaçao'
          }, {
              code: 'CY',
              name: 'Cyprus'
          }, {
              code: 'CZ',
              name: 'Czech Republic'
          }, {
              code: 'CI',
              name: "Côte D'Ivoire"
          }, {
              code: 'DK',
              name: 'Denmark'
          }, {
              code: 'DJ',
              name: 'Djibouti'
          }, {
              code: 'DM',
              name: 'Dominica'
          }, {
              code: 'DO',
              name: 'Dominican Republic'
          }, {
              code: 'TL',
              name: 'East Timor'
          }, {
              code: 'EC',
              name: 'Ecuador'
          }, {
              code: 'EG',
              name: 'Egypt'
          }, {
              code: 'SV',
              name: 'El Salvador'
          }, {
              code: 'GQ',
              name: 'Equatorial Guinea'
          }, {
              code: 'ER',
              name: 'Eritrea'
          }, {
              code: 'EE',
              name: 'Estonia'
          }, {
              code: 'ET',
              name: 'Ethiopia'
          }, {
              code: 'FK',
              name: 'Falkland Islands'
          }, {
              code: 'FO',
              name: 'Faroe Islands'
          }, {
              code: 'FJ',
              name: 'Fiji'
          }, {
              code: 'FI',
              name: 'Finland'
          }, {
              code: 'FR',
              name: 'France'
          }, {
              code: 'GF',
              name: 'French Guiana'
          }, {
              code: 'PF',
              name: 'French Polynesia'
          }, {
              code: 'TF',
              name: 'French Southern Territories'
          }, {
              code: 'GA',
              name: 'Gabon'
          }, {
              code: 'GM',
              name: 'Gambia'
          }, {
              code: 'GE',
              name: 'Georgia'
          }, {
              code: 'DE',
              name: 'Germany'
          }, {
              code: 'GH',
              name: 'Ghana'
          }, {
              code: 'GI',
              name: 'Gibraltar'
          }, {
              code: 'GR',
              name: 'Greece'
          }, {
              code: 'GL',
              name: 'Greenland'
          }, {
              code: 'GD',
              name: 'Grenada'
          }, {
              code: 'GP',
              name: 'Guadeloupe'
          }, {
              code: 'GU',
              name: 'Guam'
          }, {
              code: 'GT',
              name: 'Guatemala'
          }, {
              code: 'GG',
              name: 'Guernsey and Alderney'
          }, {
              code: 'GN',
              name: 'Guinea'
          }, {
              code: 'GW',
              name: 'Guinea-Bissau'
          }, {
              code: 'GY',
              name: 'Guyana'
          }, {
              code: 'HT',
              name: 'Haiti'
          }, {
              code: 'HM',
              name: 'Heard and McDonald Islands'
          }, {
              code: 'HN',
              name: 'Honduras'
          }, {
              code: 'HK',
              name: 'Hong Kong'
          }, {
              code: 'HU',
              name: 'Hungary'
          }, {
              code: 'IS',
              name: 'Iceland'
          }, {
              code: 'IN',
              name: 'India'
          }, {
              code: 'ID',
              name: 'Indonesia'
          }, {
              code: 'IR',
              name: 'Iran'
          }, {
              code: 'IQ',
              name: 'Iraq'
          }, {
              code: 'IE',
              name: 'Ireland'
          }, {
              code: 'IM',
              name: 'Isle of Man'
          }, {
              code: 'IL',
              name: 'Israel'
          }, {
              code: 'IT',
              name: 'Italy'
          }, {
              code: 'JM',
              name: 'Jamaica'
          }, {
              code: 'JP',
              name: 'Japan'
          }, {
              code: 'JE',
              name: 'Jersey'
          }, {
              code: 'JO',
              name: 'Jordan'
          }, {
              code: 'KZ',
              name: 'Kazakhstan'
          }, {
              code: 'KE',
              name: 'Kenya'
          }, {
              code: 'KI',
              name: 'Kiribati'
          }, {
              code: 'KP',
              name: 'Korea (North)'
          }, {
              code: 'KR',
              name: 'Korea (South)'
          }, {
              code: 'KW',
              name: 'Kuwait'
          }, {
              code: 'KG',
              name: 'Kyrgyzstan'
          }, {
              code: 'LA',
              name: 'Laos'
          }, {
              code: 'LV',
              name: 'Latvia'
          }, {
              code: 'LB',
              name: 'Lebanon'
          }, {
              code: 'LS',
              name: 'Lesotho'
          }, {
              code: 'LR',
              name: 'Liberia'
          }, {
              code: 'LY',
              name: 'Libya'
          }, {
              code: 'LI',
              name: 'Liechtenstein'
          }, {
              code: 'LT',
              name: 'Lithuania'
          }, {
              code: 'LU',
              name: 'Luxembourg'
          }, {
              code: 'MO',
              name: 'Macao'
          }, {
              code: 'MK',
              name: 'Macedonia'
          }, {
              code: 'MG',
              name: 'Madagascar'
          }, {
              code: 'MW',
              name: 'Malawi'
          }, {
              code: 'MY',
              name: 'Malaysia'
          }, {
              code: 'MV',
              name: 'Maldives'
          }, {
              code: 'ML',
              name: 'Mali'
          }, {
              code: 'MT',
              name: 'Malta'
          }, {
              code: 'MH',
              name: 'Marshall Islands'
          }, {
              code: 'MQ',
              name: 'Martinique'
          }, {
              code: 'MR',
              name: 'Mauritania'
          }, {
              code: 'MU',
              name: 'Mauritius'
          }, {
              code: 'YT',
              name: 'Mayotte'
          }, {
              code: 'MX',
              name: 'Mexico'
          }, {
              code: 'FM',
              name: 'Micronesia'
          }, {
              code: 'MD',
              name: 'Moldova'
          }, {
              code: 'MC',
              name: 'Monaco'
          }, {
              code: 'MN',
              name: 'Mongolia'
          }, {
              code: 'MS',
              name: 'Montserrat'
          }, {
              code: 'MA',
              name: 'Morocco'
          }, {
              code: 'MZ',
              name: 'Mozambique'
          }, {
              code: 'MM',
              name: 'Myanmar'
          }, {
              code: 'NA',
              name: 'Namibia'
          }, {
              code: 'NR',
              name: 'Nauru'
          }, {
              code: 'NP',
              name: 'Nepal'
          }, {
              code: 'NL',
              name: 'Netherlands'
          }, {
              code: 'AN',
              name: 'Netherlands Antilles'
          }, {
              code: 'NC',
              name: 'New Caledonia'
          }, {
              code: 'NZ',
              name: 'New Zealand'
          }, {
              code: 'NI',
              name: 'Nicaragua'
          }, {
              code: 'NE',
              name: 'Niger'
          }, {
              code: 'NG',
              name: 'Nigeria'
          }, {
              code: 'NU',
              name: 'Niue'
          }, {
              code: 'NF',
              name: 'Norfolk Island'
          }, {
              code: 'MP',
              name: 'Northern Mariana Islands'
          }, {
              code: 'NO',
              name: 'Norway'
          }, {
              code: 'OM',
              name: 'Oman'
          }, {
              code: 'PK',
              name: 'Pakistan'
          }, {
              code: 'PW',
              name: 'Palau'
          }, {
              code: 'PS',
              name: 'Palestine'
          }, {
              code: 'PA',
              name: 'Panama'
          }, {
              code: 'PG',
              name: 'Papua New Guinea'
          }, {
              code: 'PY',
              name: 'Paraguay'
          }, {
              code: 'PE',
              name: 'Peru'
          }, {
              code: 'PH',
              name: 'Philippines'
          }, {
              code: 'PN',
              name: 'Pitcairn'
          }, {
              code: 'PL',
              name: 'Poland'
          }, {
              code: 'PT',
              name: 'Portugal'
          }, {
              code: 'PR',
              name: 'Puerto Rico'
          }, {
              code: 'QA',
              name: 'Qatar'
          }, {
              code: 'RO',
              name: 'Romania'
          }, {
              code: 'RU',
              name: 'Russia'
          }, {
              code: 'RW',
              name: 'Rwanda'
          }, {
              code: 'RE',
              name: 'Réunion'
          }, {
              code: 'BL',
              name: 'Saint Barthélemy'
          }, {
              code: 'SH',
              name: 'Saint Helena'
          }, {
              code: 'KN',
              name: 'Saint Kitts and Nevis'
          }, {
              code: 'LC',
              name: 'Saint Lucia'
          }, {
              code: 'MF',
              name: 'Saint Martin'
          }, {
              code: 'PM',
              name: 'Saint Pierre and Miquelon'
          }, {
              code: 'VC',
              name: 'Saint Vincent and the Grenadines'
          }, {
              code: 'WS',
              name: 'Samoa'
          }, {
              code: 'SM',
              name: 'San Marino'
          }, {
              code: 'SA',
              name: 'Saudi Arabia'
          }, {
              code: 'SN',
              name: 'Senegal'
          }, {
              code: 'RS',
              name: 'Serbia'
          }, {
              code: 'SC',
              name: 'Seychelles'
          }, {
              code: 'SL',
              name: 'Sierra Leone'
          }, {
              code: 'SG',
              name: 'Singapore'
          }, {
              code: 'SX',
              name: 'Sint Maarten'
          }, {
              code: 'SK',
              name: 'Slovakia'
          }, {
              code: 'SI',
              name: 'Slovenia'
          }, {
              code: 'SB',
              name: 'Solomon Islands'
          }, {
              code: 'SO',
              name: 'Somalia'
          }, {
              code: 'ZA',
              name: 'South Africa'
          }, {
              code: 'GS',
              name: 'South Georgia and the South Sandwich Islands'
          }, {
              code: 'SS',
              name: 'South Sudan'
          }, {
              code: 'ES',
              name: 'Spain'
          }, {
              code: 'LK',
              name: 'Sri Lanka'
          }, {
              code: 'SD',
              name: 'Sudan'
          }, {
              code: 'SR',
              name: 'Suriname'
          }, {
              code: 'SJ',
              name: 'Svalbard and Jan Mayen'
          }, {
              code: 'SZ',
              name: 'Swaziland'
          }, {
              code: 'SE',
              name: 'Sweden'
          }, {
              code: 'CH',
              name: 'Switzerland'
          }, {
              code: 'SY',
              name: 'Syria'
          }, {
              code: 'ST',
              name: 'São Tomé and Príncipe'
          }, {
              code: 'TW',
              name: 'Taiwan'
          }, {
              code: 'TJ',
              name: 'Tajikistan'
          }, {
              code: 'TZ',
              name: 'Tanzania'
          }, {
              code: 'TH',
              name: 'Thailand'
          }, {
              code: 'TG',
              name: 'Togo'
          }, {
              code: 'TK',
              name: 'Tokelau'
          }, {
              code: 'TO',
              name: 'Tonga'
          }, {
              code: 'TT',
              name: 'Trinidad and Tobago'
          }, {
              code: 'TN',
              name: 'Tunisia'
          }, {
              code: 'TR',
              name: 'Turkey'
          }, {
              code: 'TM',
              name: 'Turkmenistan'
          }, {
              code: 'TC',
              name: 'Turks and Caicos Islands'
          }, {
              code: 'TV',
              name: 'Tuvalu'
          }, {
              code: 'UG',
              name: 'Uganda'
          }, {
              code: 'UA',
              name: 'Ukraine'
          }, {
              code: 'AE',
              name: 'United Arab Emirates'
          }, {
              code: 'GB',
              name: 'United Kingdom'
          }, {
              code: 'UM',
              name: 'United States Minor Outlying Islands'
          }, {
              code: 'US',
              name: 'United States of America'
          }, {
              code: 'UY',
              name: 'Uruguay'
          }, {
              code: 'UZ',
              name: 'Uzbekistan'
          }, {
              code: 'VU',
              name: 'Vanuatu'
          }, {
              code: 'VA',
              name: 'Vatican City'
          }, {
              code: 'VE',
              name: 'Venezuela'
          }, {
              code: 'VN',
              name: 'Vietnam'
          }, {
              code: 'VI',
              name: 'Virgin Islands of the United States'
          }, {
              code: 'WF',
              name: 'Wallis and Futuna'
          }, {
              code: 'EH',
              name: 'Western Sahara'
          }, {
              code: 'YE',
              name: 'Yemen'
          }, {
              code: 'ZM',
              name: 'Zambia'
          }, {
              code: 'ZW',
              name: 'Zimbabwe'
          }, {
              code: 'AX',
              name: 'Åland Islands'
          }
        ];
        return {
            restrict: 'AE',
            replace: true,
            scope: {
                priorities: '@csPriorities',
                only: '@csOnly',
                except: '@csExcept'
            },
            template: '<select ng-options="country.code as country.name for country in countries"> <option value="" ng-if="isSelectionOptional"></option> </select>',
            controller: [
              '$scope', '$attrs', function ($scope, $attrs) {
                  var countryCodesIn, findCountriesIn, includeOnlyRequestedCountries, removeCountry, removeExcludedCountries, separator, updateWithPriorityCountries;
                  separator = {
                      code: '-',
                      name: '────────────────────',
                      disabled: true
                  };
                  countryCodesIn = function (codesString) {
                      var codes;
                      codes = codesString ? codesString.split(',') : [];
                      return codes.map(function (code) {
                          return code.trim();
                      });
                  };
                  findCountriesIn = (function (_this) {
                      return function (codesString) {
                          var country, countryCodes, i, len, ref, ref1, results;
                          countryCodes = countryCodesIn(codesString);
                          ref = _this.countries;
                          results = [];
                          for (i = 0, len = ref.length; i < len; i++) {
                              country = ref[i];
                              if (ref1 = country.code, indexOf.call(countryCodes, ref1) >= 0) {
                                  results.push(country);
                              }
                          }
                          return results;
                      };
                  })(this);
                  removeCountry = (function (_this) {
                      return function (country) {
                          return _this.countries.splice(_this.countries.indexOf(country), 1);
                      };
                  })(this);
                  includeOnlyRequestedCountries = (function (_this) {
                      return function () {
                          if (!$scope.only) {
                              return;
                          }
                          return _this.countries = findCountriesIn($scope.only);
                      };
                  })(this);
                  removeExcludedCountries = function () {
                      var country, i, len, ref, results;
                      if (!$scope.except) {
                          return;
                      }
                      ref = findCountriesIn($scope.except);
                      results = [];
                      for (i = 0, len = ref.length; i < len; i++) {
                          country = ref[i];
                          results.push(removeCountry(country));
                      }
                      return results;
                  };
                  updateWithPriorityCountries = (function (_this) {
                      return function () {
                          var i, len, priorityCountries, priorityCountry, ref, results;
                          priorityCountries = findCountriesIn($scope.priorities);
                          if (priorityCountries.length === 0) {
                              return;
                          }
                          _this.countries.unshift(separator);
                          ref = priorityCountries.reverse();
                          results = [];
                          for (i = 0, len = ref.length; i < len; i++) {
                              priorityCountry = ref[i];
                              removeCountry(priorityCountry);
                              results.push(_this.countries.unshift(priorityCountry));
                          }
                          return results;
                      };
                  })(this);
                  this.countries = allCountries.slice();
                  includeOnlyRequestedCountries();
                  removeExcludedCountries();
                  updateWithPriorityCountries();
                  $scope.countries = this.countries;
                  return $scope.isSelectionOptional = $attrs.csRequired === void 0;
              }
            ]
        };
    });

    app.factory('registerUserService', function ($http) {            
         
        return{
            createUser : function (newuser) {
                return $http.post(serverBaseUrl + '/api/accounts/create', newuser);
        }
        };
      
    });

    app.run(function(gettextCatalog, $rootScope) {

        gettextCatalog.setCurrentLanguage('nl');

        $rootScope.translate = function (str) {
            return gettextCatalog.getString(str);
        };

        gettextCatalog.debug = true;
    });

    app.directive('validPasswordC', function () {
        return {
            require: 'ngModel',
            link: function (scope, elm, attrs, ctrl) {
                ctrl.$parsers.unshift(function (viewValue, $scope) {
                    var noMatch = viewValue != scope.formSignup.password.$viewValue;
                    ctrl.$setValidity('noMatch', !noMatch);
                    return viewValue;
                })
            }
        }
    });

    app.directive('validPassword', function () {
        return {
            require: 'ngModel',
            link: function (scope, elm, attrs, ctrl) {
                ctrl.$parsers.unshift(function (viewValue, $scope) {

                    // password validate.
                    var res = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d.*)(?=.*\W.*)[a-zA-Z0-9\S]{8,20}$/.test(viewValue);
                    ctrl.$setValidity('noValidPassword', res);

                    // if change the password when having confirmation password, check match and give error.
                    if (scope.formSignup.password_c.$viewValue != ''){
                        var noMatch = viewValue != scope.formSignup.password_c.$viewValue;
                        scope.formSignup.password_c.$setValidity('noMatch', !noMatch);
                    }

                    return viewValue;
                })
            }
        }
    });

    app.directive('validPhoneNo', function () {
        return {
            require: 'ngModel',
            link: function (scope, elm, attrs, ctrl) {
                ctrl.$parsers.unshift(function (viewValue, $scope) {

                    // should have only +,- and digits.
                    var res1 = /^[0-9()+-]*$/.test(viewValue);
                    // should have at least 8 digits.
                    var res2 = /(?=(.*\d){8})/.test(viewValue);

                    ctrl.$setValidity('notValidPhoneNo', res1 && res2);
                    return viewValue;
                })
            }
        }
    });

    app.directive('icheck', ['$timeout', '$parse', function ($timeout, $parse) {

        return {
            require: 'ngModel',
            link: function ($scope, element, $attrs, ngModel) {
                return $timeout(function () {
                    var value;
                    value = $attrs['value'];

                    $scope.$watch($attrs['ngModel'], function (newValue) {
                        $(element).iCheck('update');
                    })

                    return $(element).iCheck({
                        checkboxClass: 'icheckbox_square-blue', //'icheckbox_flat-aero',
                        radioClass: 'iradio_square-blue'

                    }).on('ifChanged', function (event) {
                        if ($(element).attr('type') === 'checkbox' && $attrs['ngModel']) {
                            $scope.$apply(function () {
                                return ngModel.$setViewValue(event.target.checked);
                            });
                        }
                        if ($(element).attr('type') === 'radio' && $attrs['ngModel']) {
                            return $scope.$apply(function () {
                                return ngModel.$setViewValue(value);
                            });
                        }
                    });
                });
            }
        };

    }]);

    app.controller('userRegistrationCtrl', 
        ['registerUserService', '$window', function (registerUserService, $window) {

        var vm = this;
        vm.user = {};
        vm.user.salutation = "Mr";
        vm.user.isCorporateAccount = "false"; //IsCorporateAccount
        vm.user.contactType = 'phone';
        vm.isSubmit = false;
        vm.user.customeraddress = {};
        vm.user.customeraddress.country = 'US';
        vm.isSentMail = false;
        vm.isEmailNotValid = false;
        vm.isServerError = false;
        vm.user.templateLink = '<html><head>    <title></title></head><body>    <p><img alt="" src="http://www.parcelinternational.nl/assets/Uploads/_resampled/SetWidth495-id-parcel-big.jpg" style="width: 200px; height: 200px; float: right;" /></p><div>        <h4 style="text-align: justify;">&nbsp;</h4><div style="background:#eee;border:1px solid #ccc;padding:5px 10px;">            <span style="font-family:verdana,geneva,sans-serif;">                <span style="color:#0000CD;">                    <span style="font-size:28px;"> Account Activation - Parcel International</span>                </span>            </span>        </div><p style="text-align: justify;">&nbsp;</p><h4 style="text-align: justify;">            &nbsp;        </h4><h4 style="text-align: justify;">            <span style="font-size:12px;">                <span style="font-family:verdana,geneva,sans-serif;">                    Dear <strong>Salutation FirstName LastName, </strong>                </span>            </span>        </h4><h4 style="text-align: justify;">            <br /><span style="font-size:12px;">                <span style="font-family:verdana,geneva,sans-serif;">                    <strong>Welcome to Parcel International, we are looking forward to supporting your shipping needs. &nbsp;&nbsp;</strong>                </span>            </span>        </h4><h4 style="text-align: justify;">            <span style="font-size:12px;">                <span style="font-family:verdana,geneva,sans-serif;">                    <strong>                        Thank you for registering. To activate your account, please click &nbsp;ActivationURL                    </strong>                </span>            </span>        </h4><h4 style="text-align: justify;">            <span style="font-size:12px;">                <span style="font-family:verdana,geneva,sans-serif;"><strong>IMPORTANT! This activation link is valid for 24 hours only. &nbsp;&nbsp;</strong></span>            </span>        </h4><h4 style="text-align: justify;">            <span style="font-size:12px;">                <span style="font-family:verdana,geneva,sans-serif;">                    <strong>                        Should you have any questions or concerns, please contact Parcel International helpdesk for support &nbsp;                    </strong>                </span>            </span>        </h4>        <h4 style="text-align: justify;">            <span style="font-size:12px;">                <span style="font-family:verdana,geneva,sans-serif;">                    <i>                        *** This is an automatically generated email, please do not reply ***                    </i>                </span>            </span>        </h4>        <h4 style="text-align: justify;">&nbsp;</h4><h4 style="text-align: justify;">            <strong>                <span style="font-size:12px;">                    <span style="font-family:verdana,geneva,sans-serif;">Thank You, </span>                </span>            </strong>        </h4><h4 style="text-align: justify;">            <strong>                <span style="font-size:12px;">                    <span style="font-family:verdana,geneva,sans-serif;">Parcel International Team<br/>Phone: +18589144414 <br/>Email: <a href="mailto:helpdesk@parcelinternational.com">helpdesk@parcelinternational.com</a><br/>Website: <a href="http://www.parcelinternational.com">http://www.parcelinternational.com</a></span>  </span>            </strong>        </h4>    </div>   </body></html>';

        vm.changeCountry = function () {
            vm.isRequiredState = vm.user.customeraddress.country == 'US' || vm.user.customeraddress.country == 'CA' || vm.user.customeraddress.country == 'PR' || vm.user.customeraddress.country == 'AU';
        };
        vm.changeCountry();

        vm.OpenNewWindow = function () {
            $window.open("termsandconditions.html", "", "width=640, height=480");
        }

        vm.alreadySubmitted = false;

        vm.register = function () {         

            vm.alreadySubmitted = true;
         
            registerUserService.createUser(vm.user)
            .then(function (result)
            {
                if (result.data == "1") {
                    vm.alreadySubmitted = false;
                    vm.isSentMail = true;
                }
                else if (result.data == "-2") {
                    //vm.isEmailNotValid = true;
                    vm.alreadySubmitted = false;
                    $.noty.defaults.killer = true;

                    noty({
                        text: '<p style="font-size:medium">Error! </p>' + 'Email address is already in use.',
                        layout: 'topRight',
                        type: 'error', //warning
                        animation: {
                            open: 'animated bounceInRight', // Animate.css class names
                            close: { height: 'toggle' }, // Animate.css class names
                            easing: 'swing', // unavailable - no need
                            speed: 200 // unavailable - no need
                        },
                        timeout: 4000
                    });
                }
                else { 
                    // Other issues.
                    //vm.isServerError = true;
                    vm.alreadySubmitted = false;

                    $.noty.defaults.killer = true;

                    noty({
                        text: '<p style="font-size:medium">Error! </p>' + 'Error occured while processing registration',
                        layout: 'topRight',
                        type: 'error',
                        animation: {
                            open: 'animated bounceInRight', // Animate.css class names
                            close: { height: 'toggle' }, // Animate.css class names
                            easing: 'swing', // unavailable - no need
                            speed: 200 // unavailable - no need
                        },
                        timeout: 4000
                    });
                }
            },
            function (error) {
                console.log("failed");
            }
            );
        };
    }]);
        

})(angular.module('userRegistration', ['ngMessages', 'gettext']));

