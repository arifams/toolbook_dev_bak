﻿'use strict';


(function (app) {

    app.factory('costCenterSaveFactory', function ($http) {
        return {
            saveCostCenter: function (costCenterDetail) {
                return $http.post(serverBaseUrl + '/api/Company/SaveCostCenter', costCenterDetail);
            }
        };
    })
    
    app.factory('costCentrMngtFactory', function ($http, $routeParams, $window) {
        return {
            loadCostcenterInfo: function () {
                return $http.get(serverBaseUrl + '/api/Company/GetCostCentersById', {
                    params: {
                        id: $routeParams.id,
                        userId:$window.localStorage.getItem('userGuid')
                    }
                });
            }
        }
    })

 
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

    app.controller('saveCostCenterCtrl',
       ['costCentrMngtFactory', 'costCenterSaveFactory', '$location', '$window',
           function (costCentrMngtFactory, costCenterSaveFactory, $location, $window) {
           var vm = this;

           vm.saveCostCenter = function () {
               vm.model.userId = $window.localStorage.getItem('userGuid')
               
               costCenterSaveFactory.saveCostCenter(vm.model)
               .success(function (result) {
                   debugger;
                   if (result == -1) {

                       $('#panel-notif').noty({
                           text: '<div class="alert alert-warning media fade in"><p>A cost center with the same name already exists!</p></div>',
                           layout: 'bottom-right',
                           theme: 'made',
                           animation: {
                               open: 'animated bounceInLeft',
                               close: 'animated bounceOutLeft'
                           },
                           timeout: 3000,
                       });
                   } else {

                       $('#panel-notif').noty({
                           text: '<div class="alert alert-success media fade in"><p>"Cost center saved successfully"</p></div>',
                           layout: 'bottom-right',
                           theme: 'made',
                           animation: {
                               open: 'animated bounceInLeft',
                               close: 'animated bounceOutLeft'
                           },
                           timeout: 3000,
                       });
                   }

               })
               .error(function () {
               })
           }

           vm.close = function () {
               $location.path('/loadCostcenters');
           }

           vm.changeCountry = function () {
               vm.isRequiredState = vm.model.billingAddress.country == 'US' ||
                                    vm.model.billingAddress.country == 'CA' ||
                                    vm.model.billingAddress.country == 'PR';
           };

           var loadCostcenter = function () {
               costCentrMngtFactory.loadCostcenterInfo()
               .success(function (data) {
                   vm.model = data;
                   debugger;
                   if (vm.model.id == 0) {
                       vm.model.status = 1;
                   }
               })
               .error(function () {
               })
           }

           loadCostcenter();
           
           //    [
           //    { id: 1, name: 'Div1', isSelected: false },
           //    { id: 2, name: 'Div2', isSelected: true },
           //    { id: 3, name: 'Div3', isSelected: false },
           //    { id: 4, name: 'Div4', isSelected: true }
           //]; // vm.model.AllDivisions;

           
           
           vm.toggleDivisionSelection = function (division) {
               var idx = vm.model.assignedDivisionIdList.indexOf(division.id);
               // is currently selected
               if (idx > -1) {
                   vm.model.assignedDivisionIdList.splice(idx, 1);
               }
                   // is newly selected
               else {
                   vm.model.assignedDivisionIdList.push(division.id);
               }
           }

       }]);


})(angular.module('newApp'));