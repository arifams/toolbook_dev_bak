﻿'use strict';

(function (app) {

    app.controller('addShipmentCtrl', ['$scope', '$location', '$window', 'shipmentFactory', 'ngDialog', '$controller', '$routeParams', '$rootScope', 'customBuilderFactory', '$timeout',
                    function ($scope, $location, $window, shipmentFactory, ngDialog, $controller, $routeParams, $rootScope, customBuilderFactory, $timeout) {

                        var vm = this;
                        vm.user = {};
                        vm.shipment = {};
                        vm.collapse1 = false;
                        vm.collapse2 = true;
                        vm.collapse3 = true;
                        vm.collapse4 = true;
                        vm.generalInfoisSubmit = false;
                        vm.consignInfoisSubmit = false;
                        vm.packageDetailsisSubmit = false;
                        vm.shipment.generalInformation = {};
                        vm.shipment.packageDetails = {};
                        vm.shipment.packageDetails.productIngredients = [{}];
                        vm.shipment.addressInformation = {};
                        vm.shipment.addressInformation.consigner = {};
                        vm.shipment.addressInformation.consignee = {};
                        vm.shipment.carrierInformation = {};
                        vm.searchRates = false;
                        vm.loadingRates = false;
                        vm.addingShipment = false;
                        vm.divisionList = {};
                        vm.costcenterList = {};
                        vm.Expressclass = "btn btn-success";
                        vm.Airclass = "btn btn-success";
                        vm.Seaclass = "btn btn-success";
                        vm.Roadclass = "btn btn-success";
                        vm.allclass = "btn btn-success";
                        vm.currencies = [];
                        vm.ratesNotAvailable = false;
                        vm.clearAll = false;
                        vm.carrierselected = false;
                        vm.displayedAddressCollection = {};
                        vm.AddList = {};
                        vm.shipment.userId = $window.localStorage.getItem('userGuid');
                        vm.hidedivisions = false;
                        vm.hidecostcenters = true;
                        vm.paylane = {};
                        vm.consignor = '';
                        vm.addressDetailsEmpty = false;
                        vm.searchText = '';
                        vm.emptySearch = false;
                        vm.shipment.packageDetails.dGType = 'EQ';
                        vm.shipment.packageDetails.accessibility = 'true';
                        vm.consigneeAdded = false;
                        vm.consignorAdded = false;
                        vm.isClickCalculateRate = false;
                        vm.addingRequestForQuote = false;
                        vm.errorCodeConsignee = false;
                        vm.errorCodeConsignor = false;
                        vm.isShowInvoice = false;
                        vm.loadingSymbole = false;
                        vm.savedShipmentId = '';
                        vm.shipmentReferenceName = ''
                        vm.isViaInvoicePayment = true;
                        vm.isBacktoRatesDisabled = false;
                        vm.savePayShipment = false;
                        vm.payementProgress = false;
                        vm.shipmentReferenceName = '';
                        vm.hideRateSummary = false;                        
                        vm.shipmentChanged = false;
                        vm.shipmentStatusMsg = '';
                        vm.labelArray = [];

                        debugger;
                        vm.paramSource = $location.search().PARAM_SOURCE;
                        vm.paramSourceId = $location.search().PARAM_SOURCEID;
                        vm.paramOwnerId = $location.search().PARAM_OWNERID;

                        //var paramSource = $window.localStorage.getItem('paramSource');
                        //var paramSourceId = $window.localStorage.getItem('paramSourceId');

                        vm.closeWindow = function () {
                            ngDialog.close()
                        }

                        vm.loadConsignerInfo = function () {
                            debugger;
                            shipmentFactory.getProfileInfo(vm.paramOwnerId).success(
                               function (responce) {
                                   debugger;
                                   if (responce != null) {

                                       debugger;
                                       if (responce.defaultVolumeMetricId == 1) {

                                           vm.shipment.packageDetails.volumeUnit = "/(cm)";
                                       } else {

                                           vm.shipment.packageDetails.volumeUnit = "/(inch)";
                                       }

                                       if (responce.defaultWeightMetricId == 1) {

                                           vm.shipment.packageDetails.weightUnit = "/(kg)";
                                       } else {
                                           vm.shipment.packageDetails.weightUnit = "/(lbs)";
                                       }

                                       if (responce.customerDetails != null && responce.customerDetails.customerAddress != null) {
                                           //assigning customer address info to consigner details
                                           vm.shipment.addressInformation.consigner.firstName = responce.customerDetails.firstName;
                                           vm.shipment.addressInformation.consigner.lastName = responce.customerDetails.lastName;
                                           vm.customerFirstName = responce.customerDetails.firstName;
                                           vm.customerLastName = responce.customerDetails.lastName;

                                           vm.shipment.addressInformation.consigner.companyName = responce.companyDetails.name;
                                           //  vm.shipment.addressInformation.consigner.contactName = responce.customerDetails.firstName + ' ' + responce.customerDetails.lastName;

                                           vm.shipment.addressInformation.consigner.country = responce.customerDetails.customerAddress.country;
                                           vm.shipment.addressInformation.consigner.postalcode = responce.customerDetails.customerAddress.zipCode;
                                           vm.shipment.addressInformation.consigner.number = responce.customerDetails.customerAddress.number
                                           vm.shipment.addressInformation.consigner.address1 = responce.customerDetails.customerAddress.streetAddress1
                                           vm.shipment.addressInformation.consigner.address2 = responce.customerDetails.customerAddress.streetAddress2
                                           vm.shipment.addressInformation.consigner.city = responce.customerDetails.customerAddress.city;
                                           vm.shipment.addressInformation.consigner.state = responce.customerDetails.customerAddress.state;
                                           vm.shipment.addressInformation.consigner.email = responce.customerDetails.email;
                                           vm.shipment.addressInformation.consigner.contactNumber = responce.customerDetails.phoneNumber;

                                           vm.shipment.addressInformation.consignee.country = responce.customerDetails.customerAddress.country;
                                           vm.isInvoicePaymentEnabled = responce.isInvoicePaymentEnabled;
                                       }
                                   }

                               }).error(function (error) {
                                   console.log("error occurd while retrieving customer details");
                               });

                        }

                        // Set current date as collection date. - dd-MMM-yyyy --- dd-MMM-yyyy HH:mm
                        var monthNamesShort = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'];
                        // This preferredCollectionDateLocal is use only in view. Not passing to server through the dto. So when editing shipment, need to explicilty load to preferredCollectionDateLocal from preferredCollectionDate.
                        vm.shipment.packageDetails.preferredCollectionDateLocal = ("0" + new Date().getDate()).slice(-2) + "-" + monthNamesShort[new Date().getUTCMonth()] + "-" + new Date().getFullYear();

                        //get the user and corporate status
                        vm.currentRole = $window.localStorage.getItem('userRole');
                        vm.isCorporate = $window.localStorage.getItem('isCorporateAccount');

                        vm.productTypes = [{ "Id": "Document", "Name": "Document" },
                                                        { "Id": "Pallet", "Name": "Pallet" },
                                                        { "Id": "Euro Pallet", "Name": "Euro Pallet" },
                                                        { "Id": "Diverse", "Name": "Diverse" },
                                                        { "Id": "Box", "Name": "Box" }
                        ];

                        vm.shipmentTerms = [{ "Id": "DDU", "Name": "Delivered Duty Unpaid (DDU)" },
                                                { "Id": "DDP", "Name": "Delivered Duty Paid (DDP)" },
                                                { "Id": "CIP", "Name": "Carriage and Insurance Paid (CIP)" },
                                                { "Id": "CPT", "Name": "Carriage Paid To (CPT)" },
                                                { "Id": "EXW", "Name": "Ex Works (EXW)" }
                        ];

                        vm.loadAllShipmentServices = function () {

                            vm.shipmentServices = [];
                            vm.shipmentServices = [{ "Id": "DD-DDU-PP", "Name": "Door-to-Door, DDU, Prepaid (standard)" },
                                               { "Id": "DD-DDP-PP", "Name": "Door-to-Door, DDP, Prepaid" },
                                               { "Id": "DD-CIP-PP", "Name": "Door-to-Door, CIP, Prepaid" },
                                               { "Id": "DP-CIP-PP", "Name": "Door-to-Port, CIP, Prepaid" },
                                               { "Id": "DP-CPT-PP", "Name": "Door-to-Port, CPT, Prepaid" },
                                               { "Id": "PD-CPT-PP", "Name": "Port-to-Door, CPT, Prepaid" },
                                               { "Id": "PD-CIP-PP", "Name": "Port-to-Door, CIP, Prepaid" },
                                               { "Id": "PP-CPT-PP", "Name": "Port-to-Port, CPT, Prepaid" },
                                               { "Id": "PP-CIP-PP", "Name": "Port-to-Port, CIP, Prepaid" },
                                               { "Id": "DP-FCA-CC", "Name": "FCA-Free Carrier" },
                                               { "Id": "DF-EXW-CC", "Name": "EXW-Ex Works" },
                                               { "Id": "KMSDY", "Name": "Door-to-Door, SDY, Same Day" },
                            ];
                        };

                        vm.loadDoorToDoorShipmentServices = function () {

                            // Allow only doo-to-door
                            vm.shipmentServices = [];
                            vm.shipmentServices =
                                        [
                                         { "Id": "DD-DDU-PP", "Name": "Door-to-Door, DDU, Prepaid (standard)" },
                                         { "Id": "DD-DDP-PP", "Name": "Door-to-Door, DDP, Prepaid" },
                                         { "Id": "DD-CIP-PP", "Name": "Door-to-Door, CIP, Prepaid" },
                                         { "Id": "KMSDY", "Name": "Door-to-Door, SDY, Same Day" },
                                        ];

                        };

                        // Select default values.
                        vm.shipment.generalInformation.shipmentServices = "DD-DDU-PP";
                        //vm.shipment.packageDetails.weightUnit = true;
                        //vm.shipment.packageDetails.volumeUnit = "true";
                        vm.shipment.packageDetails.isInsuared = "false";
                        vm.shipment.packageDetails.isDG = "false";
                        vm.shipment.packageDetails.valueCurrency = 1;

                        shipmentFactory.loadAllCurrencies()
                            .success(
                               function (responce) {
                                   vm.currencies = responce;
                               }).error(function (error) {
                                   console.log("error occurd while retrieving currencies");
                               });


                        //load the division list
                        if (vm.currentRole == "BusinessOwner") {
                            // shipmentFactory.
                            shipmentFactory.loadAllDivisions().success(
                               function (responce) {
                                   if (responce.length > 0) {
                                       vm.divisionList = responce;
                                   } else {
                                       vm.hidedivisions = true;
                                   }
                               }).error(function (error) {

                               });
                        }
                        else {

                            shipmentFactory.loadAssignedDivisions().success(
                            function (responce) {
                                if (responce.length > 0) {
                                    vm.divisionList = responce;
                                } else {
                                    vm.hidedivisions = true;
                                }


                            }).error(function (error) {
                                console.log("error occurd while retrieving divisions");
                            });

                        }

                        //get the cost centers according to the division
                        vm.selectDivision = function () {
                            var divisionId = vm.shipment.generalInformation.divisionId;
                            vm.costcenterList = {};

                            //  loadAssignedCostCenters
                            if (divisionId != '') {

                                shipmentFactory.loadAssignedCostCenters(divisionId).success(
                                function (responce) {

                                    if (responce.length > 0) {
                                        vm.costcenterList = responce;
                                        vm.hidecostcenters = false;
                                        //loading default cost center Id to bind to cost center dropdown
                                        shipmentFactory.loadDefaultCostCenterId(divisionId).success(

                                       function (responce) {

                                           if (responce != 0 || responce != null) {

                                               vm.shipment.generalInformation.costCenterId = responce;

                                           }

                                       }).error(function (error) {

                                           console.log("error occurd while retrieving defaultcost centers");
                                       });


                                    } else {
                                        vm.hidecostcenters = true;
                                    }



                                }).error(function (error) {

                                    console.log("error occurd while retrieving cost centers");
                                });

                            }

                        }

                        vm.consignorSearchChange = function () {
                            vm.addressDetailsEmpty = false;

                            // For testing purpose only. Auto Fill data.
                            testShipmentDataFill();
                        }
                        vm.consigneeSearchChange = function () {
                            vm.addressDetailsEmpty = false;
                        }

                        vm.getAddressBookDetails = function () {


                            shipmentFactory.loadAddressBookDetails(vm.searchText).success(
                               function (responce) {
                                   if (responce.content.length > 0) {


                                       ngDialog.open({
                                           scope: $scope,
                                           template: '/app/shipment/AddressViewTemplate.html',
                                           className: 'ngdialog-theme-plain custom-width',
                                           controller: $controller('addressListCtrl', {
                                               $scope: $scope,
                                               searchList: responce.content,
                                               consignor: vm.consignor
                                           })

                                       });


                                   } else {
                                       vm.addressDetailsEmpty = true;
                                       vm.emptySearch = false;
                                   }
                               }).error(function (error) {

                                   console.log("error occurd while retrieving Addresses");
                               });

                        }

                        //showing consigner addressBook search details
                        vm.searchAddressesConsignor = function () {
                            vm.consignor = true;
                            if (vm.consignorSearchText) {
                                vm.searchText = vm.consignorSearchText;
                                vm.getAddressBookDetails();
                                vm.emptySearch = false;
                            } else {
                                vm.emptySearch = true;
                            }

                        }

                        vm.consignorEdited = function () {

                            vm.consignorAdded = false;
                        }

                        vm.consigneeEdited = function () {

                            vm.consigneeAdded = false;

                        }

                        vm.searchAddressesConsignee = function () {
                            vm.consignor = false;
                            if (vm.consigneeSearchText) {
                                vm.searchText = vm.consigneeSearchText;
                                vm.getAddressBookDetails();
                                vm.emptySearch = false;
                            } else {
                                vm.emptySearch = true;
                            }

                        }

                        //vm.checkGenaralInfo = function (value) {
                        //    if (value == true) {
                        //        vm.collapse1 = true;
                        //        vm.collapse2 = false;
                        //    }
                        //    vm.generalInfoisSubmit = true;

                        //    customBuilderFactory.scrollTopackagedetails();

                        //}

                        vm.checkConsignInfo = function (value) {
                            if (value == true) {
                                vm.collapse1 = true;
                                vm.collapse2 = false;

                            }
                            vm.consignInfoisSubmit = true

                        }
                        customBuilderFactory.scrollToRatesAndCarrierDetails();

                        vm.checkPackageDetails = function (value) {

                            if (value) {
                                vm.collapse2 = true;
                                vm.collapse3 = false;
                                vm.collapse4 = true;

                            }
                            vm.packageDetailsisSubmit = true
                        }

                        vm.ClearConsignerAddress = function () {
                            $scope.consignerConsigneeInfoForm.$setPristine();
                            vm.shipment.addressInformation.consigner = {};

                        }

                        vm.ClearConsigneeAddress = function () {
                            $scope.consignerConsigneeInfoForm.$setPristine();
                            vm.shipment.addressInformation.consignee = {};
                        }

                        vm.addEmptyRow = function () {
                            vm.shipment.packageDetails.productIngredients.push({});
                            vm.CalctotalWeightVolume();
                        };

                        vm.removePackage = function (index) {
                            vm.shipment.packageDetails.productIngredients.splice(index, 1);
                            // if array length is 0, then one empty row will insert.
                            if (vm.shipment.packageDetails.productIngredients.length == 0)
                                vm.addEmptyRow();
                            vm.CalctotalWeightVolume();
                        };

                        //calculating the total volume and total weight
                        vm.CalctotalWeightVolume = function () {



                            var packages = vm.shipment.packageDetails.productIngredients;
                            var count = 0;
                            var totWeight = 0;
                            var totVolume = 0;

                            for (var i = 0; i < packages.length; i++) {

                                var Pieces = packages[i].quantity != undefined ? packages[i].quantity : 0;
                                count = count + parseInt(Pieces);

                                totWeight = totWeight + ((packages[i].weight != undefined ? packages[i].weight : 0) * Pieces);

                                if (packages[i].height != undefined && packages[i].length != undefined && packages[i].width != undefined) {
                                    totVolume = totVolume + ((packages[i].height * packages[i].length * packages[i].width) * Pieces);
                                }


                            }
                            vm.shipment.packageDetails.totalWeight = totWeight;
                            vm.shipment.packageDetails.totalVolume = totVolume;
                            vm.shipment.packageDetails.count = count;
                        }

                        //get the calculated rates
                        vm.calculateRates = function () {
                            vm.loadingRates = true;
                            vm.ratesNotAvailable = false;
                            vm.searchRates = false;
                            vm.previousClicked = false;
                            vm.rateTable = false;
                            vm.shipmentChanged = false;


                            vm.shipment.packageDetails.preferredCollectionDate = vm.shipment.packageDetails.preferredCollectionDateLocal + " " + new Date().getHours() + ":" + ("0" + new Date().getMinutes()).slice(-2);

                            shipmentFactory.calculateRates(vm.shipment).success(
                                function (responce) {
                                    if (responce.items.length > 0) {
                                        vm.displayedCollection = responce.items;
                                        vm.loadingRates = false;
                                        vm.searchRates = true;

                                        console.info("Rate calculate url: ");
                                        console.info(responce.rateCalculateURL);

                                    } else {
                                        vm.loadingRates = false;
                                        vm.ratesNotAvailable = true;
                                        vm.isClickCalculateRate = true;
                                    }

                                }).error(function (error) {

                                });
                        }

                        var paymentForm;

                        vm.selectCarrier = function (row) {

                            vm.collapse3 = true;
                            vm.collapse4 = false;

                            vm.rateTable = true;
                            customBuilderFactory.selectRateRow();

                            var total = 0.0;
                            var insurance = 0.0;
                            vm.searchRates = false;
                            if (row != null) {


                                vm.carrierselected = true;
                                vm.shipment.carrierInformation.carrierName = row.carrier_name;
                                vm.shipment.carrierInformation.pickupDate = row.pickup_date;
                                vm.shipment.carrierInformation.deliveryTime = row.delivery_date;
                                vm.shipment.carrierInformation.price = parseFloat(row.price).toFixed(2);
                                vm.shipment.carrierInformation.description = row.price_detail.description;

                                var declaredVal = vm.shipment.packageDetails.declaredValue;

                                if (vm.shipment.packageDetails.isInsuared == 'true') {

                                    insurance = (declaredVal * 0.011).toFixed(2);

                                    var currencyCode = vm.getCurrenyCode(vm.shipment.packageDetails.valueCurrency);

                                    if (insurance < 5.5 && currencyCode != null && currencyCode == 'USD') {
                                        insurance = 5.5;
                                    }
                                    //if (insurance < 5 && currencyCode != null && currencyCode == 'EUR') {
                                    //    insurance = 5;
                                    //}
                                }


                                vm.shipment.carrierInformation.insurance = insurance;
                                total = parseFloat(row.price) + parseFloat(insurance);
                                vm.shipment.carrierInformation.totalPrice = total.toFixed(2);
                                vm.shipment.carrierInformation.serviceLevel = row.service_level
                                vm.shipment.carrierInformation.tariffText = row.tariff_text
                                vm.shipment.carrierInformation.tarriffType = row.tariff_type
                                vm.shipment.carrierInformation.currency = row.currency

                                initializePaymentForm();

                            }
                        }

                        vm.backToRates = function () {
                            vm.collapse3 = false;
                            vm.collapse4 = true;
                            vm.isShowPaymentForm = false;
                            vm.isShowResponse = false;
                        }


                        vm.isShowLabel = false;
                        vm.isPrevDisabled = false;

                        vm.isShowResponse = false;

                        function initializePaymentForm() {

                            shipmentFactory.getSquareApplicationId().success(
                               function (responce) {

                                   paymentForm = new SqPaymentForm({
                                       applicationId: responce,
                                       inputClass: 'sq-input',
                                       inputStyles: [
                                         {
                                             fontSize: '15px'
                                         }
                                       ],
                                       cardNumber: {
                                           elementId: 'sq-card-number',
                                           placeholder: '•••• •••• •••• ••••'
                                       },
                                       cvv: {
                                           elementId: 'sq-cvv',
                                           placeholder: 'CVV'
                                       },
                                       expirationDate: {
                                           elementId: 'sq-expiration-date',
                                           placeholder: 'MM/YY'
                                       },
                                       postalCode: {
                                           elementId: 'sq-postal-code'
                                       },
                                       callbacks: {

                                           // Called when the SqPaymentForm completes a request to generate a card
                                           // nonce, even if the request failed because of an error.
                                           cardNonceResponseReceived: function (errors, nonce, cardData) {

                                               vm.isViaInvoicePayment = false;
                                               if (errors) {
                                                   // This logs all errors encountered during nonce generation to the
                                                   // Javascript console.
                                                   var errorList = '';
                                                   errors.forEach(function (error) {
                                                       errorList = errorList + error.message + '  ';
                                                   });

                                                   $scope.$apply(function () {
                                                       vm.shipmentStatusMsg = errorList;
                                                       vm.loadingSymbole = false;
                                                       vm.payementProgress = false;
                                                   });
                                                   // No errors occurred. Extract the card nonce.
                                               } else {


                                                   var body = $("html, body");

                                                   // Show payment page.
                                                   vm.shipment.paymentDto = {
                                                       ChargeAmount: vm.shipment.carrierInformation.totalPrice,
                                                       CurrencyType: vm.shipment.carrierInformation.currency,
                                                       CardNonce: nonce//,
                                                       //shipmentId: $window.localStorage.getItem('shipmentId'),
                                                       //userId: $window.localStorage.getItem('userGuid'),
                                                       //templateLink: '<html><head><title></title></head><body style="margin-left:40px;margin-right:40px;margin-top:30px"><div style="margin-right:40px;margin-left:40px"><div style="margin-top:30px;background-color:#0af;font-size:24px;text-align:center;padding:10px;font-family:verdana,geneva,sans-serif;color:#fff">Order Confirmation - One2send</div></div><div style="margin-right:40px;margin-left:40px"><div style="float:left;"><img alt="" src="http://www.12send.com/template/logo_12send.png" style="width: 193px; height: 100px;" /></div><h3 style="margin-bottom:65px;margin-right:146px;margin-top:0;padding-top:62px;text-align:center;font-family:verdana,geneva,sans-serif;color:#000">Thank you for using One2send </h3></div><div style="margin-right:40px;margin-left:40px"><div style="padding:10px;font-family:verdana,geneva,sans-serif;color:#000font-size:13px"><p style="font-style:italic;">Order Reference  &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<OrderReference></OrderReference></p><p style="font-style:italic;">Pickup Date  &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; <PickupDate></PickupDate></p><p style="font-style:italic;">Shipment Mode  &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; <ShipmentMode></ShipmentMode></p><p style="font-style:italic;">Shipment Type  &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; <ShipmentType></ShipmentType></p><p style="font-style:italic;">Carrier   &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<Carrier></Carrier></p><p style="font-style:italic;">Shipment Price   &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<ShipmentPrice></ShipmentPrice></p><p style="font-style:italic;">Payment Type   &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<PaymentType></PaymentType></p></div><br><div style="padding:10px;font-family:verdana,geneva,sans-serif;color:#fff;font-size:13px"><table><thead><tr><th style="width:290px;color:#000;font-size:13px;border-bottom:2px solid #000;">Product Type</th><th style="width:290px;color:#000;font-size:13px;border-bottom:2px solid #000;">Quantity</th><th style="width:290px;color:#000;font-size:13px;border-bottom:2px solid #000;">Weight</th><th style="width:290px;color:#000;font-size:13px;border-bottom:2px solid #000;">Volume</th></tr></thead><tbody><tableRecords></tbody></table></div><p style="font-family:verdana,geneva,sans-serif;font-size:14px;text-align:center;">should you have any questions or concerns, please contact One2send helpdesk for support.</p></body></html>'
                                                   };

                                                   vm.shipment.generalInformation.shipmentPaymentTypeId = 2;

                                                   saveShipment();

                                                   //shipmentFactory.PaymentCharge(paymentDto).success(
                                                   //                function (response) {

                                                   //                    //addShipmentResponse(response);

                                                   //                }).error(function (error) {

                                                   //                    $('#panel-notif').noty({
                                                   //                        text: '<div class="alert alert-danger media fade in"><p>' + $rootScope.translate('Error occured while processing payment') + '!</p></div>',
                                                   //                        layout: 'bottom-right',
                                                   //                        theme: 'made',
                                                   //                        animation: {
                                                   //                            open: 'animated bounceInLeft',
                                                   //                            close: 'animated bounceOutLeft'
                                                   //                        },
                                                   //                        timeout: 6000,
                                                   //                    });
                                                   //                });
                                               }
                                           },

                                           unsupportedBrowserDetected: function () {
                                               // Fill in this callback to alert buyers when their browser is not supported.
                                           },

                                           // Fill in these cases to respond to various events that can occur while a
                                           // buyer is using the payment form.
                                           inputEventReceived: function (inputEvent) {
                                               switch (inputEvent.eventType) {
                                                   case 'focusClassAdded':
                                                       // Handle as desired
                                                       break;
                                                   case 'focusClassRemoved':
                                                       // Handle as desired
                                                       break;
                                                   case 'errorClassAdded':
                                                       // Handle as desired
                                                       break;
                                                   case 'errorClassRemoved':
                                                       // Handle as desired
                                                       break;
                                                   case 'cardBrandChanged':
                                                       // Handle as desired
                                                       break;
                                                   case 'postalCodeChanged':
                                                       // Handle as desired
                                                       break;
                                               }
                                           },

                                           paymentFormLoaded: function () {
                                               // Fill in this callback to perform actions after the payment form is
                                               // done loading (such as setting the postal code field programmatically).
                                               paymentForm.setPostalCode(vm.shipment.addressInformation.consigner.postalcode);
                                           }
                                       }
                                   });

                               }).error(function (error) {

                               });

                        }

                        vm.addingShipment = false;
                        //section to set the shipment mode
                        function addShipmentResponse(responseArray) {
                            
                            //vm.loadingSymbole = false;
                            //vm.shipmentStatusMsg = response.message;
                            //vm.isShowResponse = true;
                            vm.isBooking = false;
                            var response = responseArray[0];

                            vm.isShowResponse = true;
                            debugger;
                            if (response.status == 2) {
                                // Success both payment and shipment.
                                debugger;
                                //vm.isBookingInCarrier = true;

                                //vm.addingShipment = true;
                                vm.isShowPaymentForm = false;
                                vm.hideRateSummary = true;
                                //vm.payementProgress = false;
                                //vm.savePayShipment = false;

                               // vm.labelUrl = response.labelURL;
                                for (var i = 0; i < responseArray.length; i++) {

                                    vm.labelArray.push(responseArray[i].labelURL);
                                }
                                debugger;
                                if (response.shipmentDto.carrierInformation.carrierName == 'TNT') {
                                    debugger;
                                    // If carrier tnt, then add manifest to label to download by user.
                                    vm.labelArray.push(response.tntManifest);
                                }

                                vm.shipmentCode = response.shipmentDto.generalInformation.shipmentCode;
                                vm.trackingNumber = response.shipmentDto.generalInformation.trackingNumber;
                                vm.carrierName = response.shipmentDto.carrierInformation.carrierName;
                                vm.createdDate = response.shipmentDto.generalInformation.createdDate;
                                debugger;
                                vm.isShowLabel = true;
                                if (response.invoiceURL != '') {
                                    vm.isShowInvoice = true;
                                    vm.payementProgress = false;
                                    vm.savePayShipment = false;
                                    vm.invoiceUrl = response.invoiceURL;
                                }
                            }
                            else if (response.status == 4) {
                                // PaymentError.
                                //$('#panel-notif').noty({
                                //    text: '<div class="alert alert-danger media fade in"><p>' + $rootScope.translate(response.message) + '!</p></div>',
                                //    layout: 'bottom-right',
                                //    theme: 'made',
                                //    animation: {
                                //        open: 'animated bounceInLeft',
                                //        close: 'animated bounceOutLeft'
                                //    },
                                //    timeout: 6000,
                                //});
                            }
                            else if (response.status == 5 || response.status == 6) {
                                // SISError.
                                debugger;
                                //$window.localStorage.setItem('sisErrorReferenceName', response.shipmentReference);
                                //$window.localStorage.setItem('labelUrl', '');
                                //$window.localStorage.setItem('invoiceURL', '');
                                //$location.path('/shipmentResult');
                                
                                vm.addingShipment = true;
                                vm.shipmentReferenceName = response.shipmentReferenceName; //response.shipmentReference;
                                vm.isShowPaymentForm = false;
                                vm.payementProgress = false;
                                //vm.savePayShipment = false;
                                vm.isShowLabel = false;
                                vm.errorUrl = response.errorUrl;
                                vm.hideRateSummary = true;
                                //window.open(errorUrl);
                            }
                        }


                        vm.selectExpress = function () {
                            vm.Expressclass = "btn btn-dark";
                            vm.Airclass = "btn btn-success";
                            vm.Seaclass = "btn btn-success";
                            vm.Roadclass = "btn btn-success";
                            vm.allclass = "btn btn-success";
                            vm.shipment.generalInformation.shipmentMode = 'Express';

                            vm.loadDoorToDoorShipmentServices();

                        }

                        vm.selectAir = function () {
                            vm.Expressclass = "btn btn-success";
                            vm.Airclass = "btn btn-dark";
                            vm.Seaclass = "btn btn-success";
                            vm.Roadclass = "btn btn-success";
                            vm.allclass = "btn btn-success";
                            vm.shipment.generalInformation.shipmentMode = 'AirFreight';

                            vm.loadAllShipmentServices();
                        }

                        vm.selectSea = function () {

                            vm.Expressclass = "btn btn-success";
                            vm.Airclass = "btn btn-success";
                            vm.Seaclass = "btn btn-dark";
                            vm.Roadclass = "btn btn-success";
                            vm.allclass = "btn btn-success";
                            vm.shipment.generalInformation.shipmentMode = 'SeaFreight';

                            vm.loadAllShipmentServices();
                        }

                        vm.selectRoad = function () {
                            vm.Expressclass = "btn btn-success";
                            vm.Airclass = "btn btn-success";
                            vm.Seaclass = "btn btn-success";
                            vm.Roadclass = "btn btn-dark";
                            vm.allclass = "btn btn-success";
                            vm.shipment.generalInformation.shipmentMode = 'RoadFreight';

                            vm.loadDoorToDoorShipmentServices();
                        }

                        vm.selectall = function () {

                            vm.Expressclass = "btn btn-success";
                            vm.Airclass = "btn btn-success";
                            vm.Seaclass = "btn btn-success";
                            vm.Roadclass = "btn btn-success";
                            vm.allclass = "btn btn-dark";
                            vm.shipment.generalInformation.shipmentMode = 'All';

                            vm.loadAllShipmentServices();
                        }

                        vm.selectExpress();
                        //vm.selectall();

                        vm.submitShipment = function (source) {

                            vm.shipment.generalInformation.shipmentPaymentTypeId = 1; // Payment type is Invoice.

                            vm.carrierselected = false;
                            vm.isPrevDisabled = true;

                            if (source == 'invoice') {
                                vm.isViaInvoicePayment = true;
                            }

                            vm.shipment.templateLink = '<html><head><title></title></head><body style="margin-top:30px;margin-left:40px;margin-right:40px"><div style="margin-right:40px;margin-left:40px"><div style="margin-top:30px;background-color:#0af;font-size:24px;text-align:center;padding:10px;font-family:verdana,geneva,sans-serif;color:#fff">Order Confirmation - 12Send</div></div><div style="margin-right:40px;margin-left:40px"><div style="float:left;"><img alt="" src="http://www.12send.com/template/logo_12send.png" style="width: 193px; height: 100px;" /></div><h3 style="margin-bottom:65px;margin-right:146px;margin-top:0;padding-top:62px;text-align:center;font-size:20px;font-family:verdana,geneva,sans-serif;color:#000">Thank you for using One2send </h3></div><div style="margin-right:40px;margin-left:40px"><div style="padding:10px;font-family:verdana,geneva,sans-serif;color:#000;font-size:13px"><p style="font-style:italic;font-size:16px">Order Reference  &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<OrderReference></OrderReference></p><p style="font-style:italic;font-size:16px">Pickup Date  &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; <PickupDate></PickupDate></p><p style="font-style:italic;font-size:16px">Shipment Mode  &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; <ShipmentMode></ShipmentMode></p><p style="font-style:italic;font-size:16px">Shipment Type  &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; <ShipmentType></ShipmentType></p><p style="font-style:italic;font-size:16px">Carrier   &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<Carrier></Carrier></p><p style="font-style:italic;font-size:16px">Shipment Price   &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<ShipmentPrice></ShipmentPrice></p><p style="font-style:italic;font-size:16px">Payment Type   &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<PaymentType></PaymentType></p></div><br><div style="padding:10px;font-family:verdana,geneva,sans-serif;color:#000;font-size:13px"><table><thead><tr><th style="width:290px;color:#000;font-size:13px;border-bottom:2px solid #000;">Product Type</th><th style="width:290px;color:#000;font-size:13px;border-bottom:2px solid #000;">Quantity</th><th style="width:290px;color:#000;font-size:13px;border-bottom:2px solid #000;">Weight</th><th style="width:290px;color:#000;font-size:13px;border-bottom:2px solid #000;">Volume</th></tr></thead><tbody><tableRecords></tbody></table></div><p style="text-align:center;">should you have any questions or concerns, please contact One2send helpdesk for support.</p></body></html>';
                            vm.shipment.isSaveAsDraft = false;
                            saveShipment();
                        }

                        vm.shipmentReferenceName = '';

                        function saveShipment() {

                            // Freeze screen
                            vm.loadingSymbole = true;
                            vm.shipment.createdBy = $window.localStorage.getItem('userGuid');

                            if ($window.localStorage.getItem('userRole') == 'Admin') {
                                vm.shipment.userId = $window.localStorage.getItem('businessOwnerId');
                            } else {
                                vm.shipment.userId = $window.localStorage.getItem('userGuid');
                            }
                            
                            var body = $("html, body");

                            //if (vm.shipment.generalInformation.shipmentPaymentTypeId == 1) {
                            //    vm.loadingSymbole = false;
                            //    vm.isShowPaymentForm = true;
                            //    paymentForm.build();

                            //    return;
                            //}

                            // Save and send shipment
                            shipmentFactory.saveShipmentV1(vm.shipment).success(
                                            function (response) {

                                                //vm.addingShipment = false;
                                                vm.loadingSymbole = false;
                                                
                                                //vm.savePayShipment = true;

                                                if (response.status == 2) {
                                                    debugger;
                                                    // Save record in db Or payment + db save is Success
                                                    vm.shipment.generalInformation.shipmentId = response.shipmentId;
                                                    vm.shipmentReferenceName = response.shipmentReferenceName;
                                                    //vm.savePayShipment = true;
                                                    //vm.isShowPaymentForm = false;
                                                    //vm.isShowResponse = true;

                                                    vm.isBooking = true;
                                                    vm.hideRateSummary = true;
                                                    vm.isShowPaymentForm = false;

                                                    var sendShipmentDetails = {
                                                        shipmentId: response.shipmentId
                                                    };

                                                    // save in SIS
                                                    shipmentFactory.sendShipmentDetailsV1(sendShipmentDetails).success(
                                                    function (response) {
                                                       
                                                        addShipmentResponse(response);

                                                    }).error(function (error) {
                                                        var response = {
                                                            status: 2
                                                        };
                                                        addShipmentResponse(response);
                                                    });
                                                    //vm.savePayShipment = true;

                                                    //$timeout(function () {

                                                    //    if(vm.isBooking)
                                                    //        GetAddShipmentResponseV1(response.shipmentId);

                                                    //}, 5000);

                                                }
                                                else if (response.status == 4) {
                                                    // payment error
                                                    vm.shipmentStatusMsg = "There is issue with the charge from credit card. Please try again";
                                                }
                                                else if (response.status == 1) {
                                                    
                                                    $('#panel-notif').noty({
                                                        text: '<div class="alert alert-danger media fade in"><p>' + $rootScope.translate('Error occured while saving the Shipment') + '!</p></div>',
                                                        layout: 'bottom-right',
                                                        theme: 'made',
                                                        animation: {
                                                            open: 'animated bounceInLeft',
                                                            close: 'animated bounceOutLeft'
                                                        },
                                                        timeout: 6000,
                                                    });
                                                }
                                            }).error(function (error) {
                                                vm.loadingSymbole = false;
                                                $('#panel-notif').noty({
                                                    text: '<div class="alert alert-danger media fade in"><p>' + $rootScope.translate('Error occured while saving the Shipment') + '!</p></div>',
                                                    layout: 'bottom-right',
                                                    theme: 'made',
                                                    animation: {
                                                        open: 'animated bounceInLeft',
                                                        close: 'animated bounceOutLeft'
                                                    },
                                                    timeout: 6000,
                                                });
                                            });
                        }


                        function saveShipmentOld() {

                            vm.loadingSymbole = true;

                            //vm.addingShipment = true;
                            var body = $("html, body");

                            vm.shipment.createdBy = $window.localStorage.getItem('userGuid');

                            if ($window.localStorage.getItem('userRole') == 'Admin') {
                                vm.shipment.userId = $window.localStorage.getItem('businessOwnerId');
                            } else {
                                vm.shipment.userId = $window.localStorage.getItem('userGuid');
                            }

                            // Save shipment in database.
                            shipmentFactory.saveShipment(vm.shipment).success(
                                            function (response) {
                                                vm.addingShipment = false;

                                                if (response.status == 2) {

                                                    if (vm.shipment.generalInformation.shipmentPaymentTypeId == 1) {

                                                        shipmentFactory.sendShipmentDetails(sendShipmentData).success(
                                                                 function (response) {

                                                                     addShipmentResponse(response);

                                                                 }).error(function (error) {
                                                                     $('#panel-notif').noty({
                                                                         text: '<div class="alert alert-danger media fade in"><p>' + $rootScope.translate('Error occured while adding the Shipment') + '!</p></div>',
                                                                         layout: 'bottom-right',
                                                                         theme: 'made',
                                                                         animation: {
                                                                             open: 'animated bounceInLeft',
                                                                             close: 'animated bounceOutLeft'
                                                                         },
                                                                         timeout: 6000,
                                                                     });
                                                                 });
                                                    }
                                                    else {
                                                        vm.loadingSymbole = false;
                                                        vm.isShowPaymentForm = true;
                                                        paymentForm.build();
                                                    }
                                                }
                                                else {
                                                    vm.addingShipment = false;
                                                    body.stop().animate({ scrollTop: 0 }, '500', 'swing', function () { });
                                                    $('#panel-notif').noty({
                                                        text: '<div class="alert alert-danger media fade in"><p>' + $rootScope.translate("Error occured while saving the Shipment") + '!</p></div>',
                                                        layout: 'bottom-right',
                                                        theme: 'made',
                                                        animation: {
                                                            open: 'animated bounceInLeft',
                                                            close: 'animated bounceOutLeft'
                                                        },
                                                        timeout: 6000,
                                                    });
                                                }
                                            }).error(function (error) {
                                                vm.loadingSymbole = false;
                                                $('#panel-notif').noty({
                                                    text: '<div class="alert alert-danger media fade in"><p>' + $rootScope.translate('Error occured while saving the Shipment') + '!</p></div>',
                                                    layout: 'bottom-right',
                                                    theme: 'made',
                                                    animation: {
                                                        open: 'animated bounceInLeft',
                                                        close: 'animated bounceOutLeft'
                                                    },
                                                    timeout: 6000,
                                                });
                                            });
                        }


                        function saveShipmentAsDraft() {

                            vm.loadingSymbole = true;

                            vm.addingShipment = true;
                            var body = $("html, body");

                            vm.shipment.createdBy = $window.localStorage.getItem('userGuid');

                            if ($window.localStorage.getItem('userRole') == 'Admin') {
                                vm.shipment.userId = $window.localStorage.getItem('businessOwnerId');
                            } else {
                                vm.shipment.userId = $window.localStorage.getItem('userGuid');
                            }
                            vm.shipment.isSaveAsDraft = true;
                            // Save shipment in database.
                            shipmentFactory.saveShipment(vm.shipment).success(
                                        function (response) {
                                            vm.addingShipment = false;
                                            vm.savePayShipment = false;
                                            if (response.status == 2) {

                                                vm.loadingSymbole = false;
                                                vm.savePayShipment = false;
                                                body.stop().animate({
                                                    scrollTop: 0
                                                }, '500', 'swing', function () { });
                                                $('#panel-notif').noty({
                                                    text: '<div class="alert alert-success media fade in"><p>' + $rootScope.translate("saved as Draft") + '!</p></div>',
                                                    layout: 'bottom-right',
                                                    theme: 'made',
                                                    animation: {
                                                        open: 'animated bounceInLeft',
                                                        close: 'animated bounceOutLeft'
                                                    },
                                                    timeout: 6000,
                                                });


                                            }
                                            else {
                                                vm.addingShipment = false;
                                                vm.savePayShipment = false;
                                                body.stop().animate({
                                                    scrollTop: 0
                                                }, '500', 'swing', function () { });
                                                $('#panel-notif').noty({
                                                    text: '<div class="alert alert-danger media fade in"><p>' + $rootScope.translate("Error occured while saving the Shipment") + '!</p></div>',
                                                    layout: 'bottom-right',
                                                    theme: 'made',
                                                    animation: {
                                                        open: 'animated bounceInLeft',
                                                        close: 'animated bounceOutLeft'
                                                    },
                                                    timeout: 6000,
                                                });
                                            }
                                        }).error(function (error) {
                                            vm.loadingSymbole = false;
                                            vm.savePayShipment = false;
                                            $('#panel-notif').noty({
                                                text: '<div class="alert alert-danger media fade in"><p>' + $rootScope.translate('Error occured while saving the Shipment') + '!</p></div>',
                                                layout: 'bottom-right',
                                                theme: 'made',
                                                animation: {
                                                    open: 'animated bounceInLeft',
                                                    close: 'animated bounceOutLeft'
                                                },
                                                timeout: 6000,
                                            });
                                        });
                        }

                        function GetAddShipmentResponse(shipmentId) {
                            vm.loadingSymbole = false;
                            shipmentFactory.GetAddShipmentResponse(shipmentId).then(function (response) {
                                console.log('rec');
                                console.log(response);
                                console.log(response.data.hasShipmentAdded);
                                debugger;

                                if (response.data.hasShipmentAdded == false && $location.path().includes("addShipment")) {
                                    $timeout(function () {

                                        GetAddShipmentResponse(shipmentId);

                                    }, 10000);
                                }
                                else if (response.data.hasShipmentAdded) {

                                    vm.isShowPaymentForm = false;
                                    vm.isShowResponse = true;
                                    vm.savePayShipment = false;
                                    vm.payementProgress = false;

                                    vm.isShowLabel = true;
                                    vm.labelUrl = response.data.labelUrl;

                                    if (response.data.invoiceUrl != '') {
                                        vm.isShowInvoice = true;
                                        vm.invoiceUrl = response.data.invoiceUrl;
                                    }

                                }

                            });
                        }

                        function GetAddShipmentResponseV1(shipmentId) {
                            
                            shipmentFactory.GetAddShipmentResponse(shipmentId).then(function (response) {
                                console.log('rec');
                                console.log(response);
                                console.log(response.data.hasShipmentAdded);
                                debugger;

                                if (response.data.hasShipmentAdded == false) {

                                    response.status == 5;

                                    addShipmentResponse(response);
                                }
                                else if (response.data.hasShipmentAdded) {

                                    vm.isShowPaymentForm = false;
                                    vm.isShowResponse = true;
                                    vm.savePayShipment = false;
                                    vm.payementProgress = false;

                                    vm.isShowLabel = true;
                                    vm.labelUrl = response.data.labelUrl;

                                    if (response.data.invoiceUrl != '') {
                                        vm.isShowInvoice = true;
                                        vm.invoiceUrl = response.data.invoiceUrl;
                                    }

                                }

                            });
                        }


                        vm.isShowPaymentForm = false;
                        vm.openLabel = function (url) {
                            //window.open(url);
                            for (var i = 0; i < url.length; i++) {
                                window.open(url[i]);
                            }
                        }

                        vm.payOnline = function () {
                            vm.shipment.generalInformation.shipmentPaymentTypeId = 2; // Payment type is Online.

                            vm.carrierselected = false;
                            vm.isPrevDisabled = true;
                            vm.isBacktoRatesDisabled = true;
                            vm.hideRateSummary = true;

                            // Build form
                            vm.isShowPaymentForm = true;
                            paymentForm.build();
                        };

                        vm.saveAsDraft = function myfunction() {

                            saveShipmentAsDraft();
                        }

                        vm.chargeFromCard = function () {
                            //vm.hideSummary = true;
                            //vm.savePayShipment = true;
                            vm.shipmentStatusMsg = '';
                            vm.payementProgress = true;
                            paymentForm.requestCardNonce();

                        }

                        function saveShipment_NotUsing() {
                            vm.shipment.generalInformation.shipmentPaymentTypeId = 2; // Payment type is Online.
                            shipmentFactory.saveShipment(vm.shipment).success(
                                            function (response) {

                                                if (response.status == 2) {
                                                    // Successfully saved in db.
                                                    vm.payementProgress = false;
                                                    vm.savePayShipment = false;
                                                    $window.localStorage.setItem('shipmentId', response.shipmentId);
                                                }
                                                else {
                                                    vm.payementProgress = false;
                                                    vm.savePayShipment = false;
                                                    $('#panel-notif').noty({
                                                        text: '<div class="alert alert-danger media fade in"><p>' + $rootScope.translate('Error occured while saving the Shipment') + '!</p></div>',
                                                        layout: 'bottom-right',
                                                        theme: 'made',
                                                        animation: {
                                                            open: 'animated bounceInLeft',
                                                            close: 'animated bounceOutLeft'
                                                        },
                                                        timeout: 6000,
                                                    });
                                                }

                                            }).error(function (error) {
                                                vm.payementProgress = false;
                                                vm.savePayShipment = false;

                                                $('#panel-notif').noty({
                                                    text: '<div class="alert alert-danger media fade in"><p>' + $rootScope.translate('Error occured while saving the Shipment') + '!</p></div>',
                                                    layout: 'bottom-right',
                                                    theme: 'made',
                                                    animation: {
                                                        open: 'animated bounceInLeft',
                                                        close: 'animated bounceOutLeft'
                                                    },
                                                    timeout: 6000,
                                                });
                                            });
                        }


                        //change state required according to the country code
                        vm.changeConsignerCountry = function () {
                            vm.isRequiredConsignerState = vm.shipment.addressInformation.consigner.country == 'US' || vm.shipment.addressInformation.consigner.country == 'CA' || vm.shipment.addressInformation.consigner.country == 'PR' || vm.shipment.addressInformation.consigner.country == 'AU';
                            vm.consignorAdded = false;
                        };


                        vm.changeConsigneeCountry = function () {
                            vm.isRequiredConsigneeState = vm.shipment.addressInformation.consignee.country == 'US' || vm.shipment.addressInformation.consignee.country == 'CA' || vm.shipment.addressInformation.consignee.country == 'PR' || vm.shipment.addressInformation.consignee.country == 'AU';
                            vm.consigneeAdded = false;
                        };


                        vm.getCurrenyCode = function (key) {
                            for (var i = 0; i < vm.currencies.length; i++) {
                                if (vm.currencies[i].id == key) {
                                    var currency = vm.currencies[i].currencyCode;
                                    return currency
                                }
                            }
                        }


                        //clear carrier information if previous button clicked
                        vm.previousBtnClicked = function () {
                            vm.carrierselected = false;
                            vm.shipment.carrierInformation = {};
                            vm.collapse2 = false;
                            vm.collapse3 = true;
                            vm.previousClicked = true;
                        }


                        vm.selectShipmentType = function () {

                            vm.shipmentChanged = true;
                        }


                        vm.loadConsignerInfo();


                        var loadShipmentInfo = function (code, id) {
                            shipmentFactory.loadShipmentInfo(code, id)
                            .success(function (data) {
                                debugger;

                                vm.shipment = data;

                                if (data.packageDetails.volumeMetricId == 1) {

                                    vm.shipment.packageDetails.volumeUnit = "/(cm)";
                                } else {

                                    vm.shipment.packageDetails.volumeUnit = "/(inch)";
                                }

                                if (data.packageDetails.weightMetricId == 1) {

                                    vm.shipment.packageDetails.weightUnit = "/(kg)";
                                } else {
                                    vm.shipment.packageDetails.weightUnit = "/(lbs)";
                                }

                                debugger;
                                if (vm.paramSource == 'copy' || vm.paramSource == 'delete-copy' || vm.paramSource == 'return-copy') {
                                    vm.shipment.generalInformation.shipmentId = "0";
                                    vm.shipment.generalInformation.shipmentCode = "0";

                                    debugger;
                                    if (vm.paramSource == 'return-copy') {
                                        var consigneeDetails = angular.copy(vm.shipment.addressInformation.consignee);
                                        var consignerDetails = angular.copy(vm.shipment.addressInformation.consigner);

                                        vm.shipment.addressInformation.consignee = consignerDetails;
                                        vm.shipment.addressInformation.consigner = consigneeDetails;
                                    }
                                }

                                if (vm.shipment.packageDetails.isDG == true) {
                                    vm.shipment.packageDetails.isDG = "true"
                                } else {
                                    vm.shipment.packageDetails.isDG = "false"
                                }


                                if (vm.shipment.packageDetails.isInsuared == "True") {
                                    vm.shipment.packageDetails.isInsuared = "true";
                                }
                                else {
                                    vm.shipment.packageDetails.isInsuared = "false";
                                }

                                vm.shipment.packageDetails.preferredCollectionDateLocal = ("0" + new Date(vm.shipment.packageDetails.preferredCollectionDate).getDate()).slice(-2) + "-" + monthNamesShort[new Date(vm.shipment.packageDetails.preferredCollectionDate).getUTCMonth()] + "-" + new Date(vm.shipment.packageDetails.preferredCollectionDate).getFullYear();

                                //console.log(vm.shipment);
                                vm.shipment.carrierInformation = {};
                            })
                            .error(function () {
                            })
                        }


                        $scope.safeApply = function (fn) {
                            var phase = this.$root.$$phase;
                            if (phase == '$apply' || phase == '$digest') {
                                if (fn && (typeof (fn) === 'function')) {
                                    fn();
                                }
                            } else {
                                this.$apply(fn);
                            }
                        };


                        vm.shipment.generalInformation.shipmentCode = "0";

                        debugger;
                        if ($routeParams.id != "0" || (vm.paramSourceId != "" && vm.paramSourceId != null)) {
                            vm.editShipmentCode = $routeParams.id;

                            if (vm.paramSource != null && vm.paramSource != '') {
                                vm.editShipmentCode = vm.paramSourceId;
                            }
                            // (ShipmentCode, ShipmentId)
                            debugger;
                            loadShipmentInfo('', vm.editShipmentCode);
                        }


                        // In production remove this.
                        function testShipmentDataFill() {

                            if (vm.consignorSearchText == "code123") {

                                vm.shipment.addressInformation.consigner = {};
                                vm.shipment.generalInformation.shipmentName = 'code123';
                                vm.shipment.addressInformation.consigner.firstName = 'CNerFName';
                                vm.shipment.addressInformation.consigner.lastName = 'CNerLName';
                                vm.shipment.addressInformation.consigner.country = 'US';
                                vm.shipment.addressInformation.consigner.postalcode = '91803';
                                vm.shipment.addressInformation.consigner.number = '901';
                                vm.shipment.addressInformation.consigner.address1 = '500 S Marengo Avenue';
                                vm.shipment.addressInformation.consigner.address2 = '';
                                vm.shipment.addressInformation.consigner.city = 'Alhambra';
                                vm.shipment.addressInformation.consigner.state = 'CA';
                                vm.shipment.addressInformation.consigner.email = 'test1@yopmail.com';
                                vm.shipment.addressInformation.consigner.contactNumber = '6264589800';
                                vm.shipment.addressInformation.consigner.contactName = "CNer contact name";

                                vm.shipment.addressInformation.consignee = {};
                                vm.shipment.addressInformation.consignee.firstName = 'CNeeFName';
                                vm.shipment.addressInformation.consignee.lastName = 'CNeeLName';
                                vm.shipment.addressInformation.consignee.country = 'US';
                                vm.shipment.addressInformation.consignee.postalcode = '90241';
                                vm.shipment.addressInformation.consignee.number = '902';
                                vm.shipment.addressInformation.consignee.address1 = '7121 Firestone Blvd';
                                vm.shipment.addressInformation.consignee.address2 = '';
                                vm.shipment.addressInformation.consignee.city = 'Downey';
                                vm.shipment.addressInformation.consignee.state = 'CA';
                                vm.shipment.addressInformation.consignee.email = 'test2@yopmail.com';
                                vm.shipment.addressInformation.consignee.contactNumber = '5627762200';
                                vm.shipment.addressInformation.consignee.contactName = "CNee contact name";

                                vm.shipment.packageDetails.productIngredients = [{ productType: 'Box', quantity: 1, description: 'desc', weight: 1, height: 1, length: 1 }];

                                vm.shipment.packageDetails.shipmentDescription = "testDesc";
                                vm.shipment.packageDetails.declaredValue = 500;
                            }
                            if (vm.consignorSearchText == "code1234") {
                                // Added UK country
                                vm.shipment.addressInformation.consignee.country = 'GB';
                                vm.shipment.addressInformation.consignee.postalcode = 'W1J 8NE';
                                vm.shipment.addressInformation.consignee.number = '3';
                                vm.shipment.addressInformation.consignee.address1 = 'Berkeley Street';
                                vm.shipment.addressInformation.consignee.address2 = '';
                                vm.shipment.addressInformation.consignee.city = 'London';
                                vm.shipment.addressInformation.consignee.state = 'GL';
                            }
                        };

                        vm.requestForQuote = function () {

                            vm.addingRequestForQuote = true;
                            var body = $("html, body");

                            shipmentFactory.requestForQuote(vm.shipment).success(
                                            function (response) {
                                                vm.addingRequestForQuote = false;

                                                if (response.status == 2) {
                                                    $('#panel-notif').noty({
                                                        text: '<div class="alert alert-success media fade in"><p>' + $rootScope.translate('Successfully request the quote') + '!</p></div>',
                                                        layout: 'bottom-right',
                                                        theme: 'made',
                                                        animation: {
                                                            open: 'animated bounceInLeft',
                                                            close: 'animated bounceOutLeft'
                                                        },
                                                        timeout: 6000,
                                                    });
                                                }
                                                else {
                                                    vm.addingRequestForQuote = false;
                                                    body.stop().animate({ scrollTop: 0 }, '500', 'swing', function () { });

                                                    $('#panel-notif').noty({
                                                        text: '<div class="alert alert-danger media fade in"><p>' + $rootScope.translate('Error occured while requesting the quote') + '!</p></div>',
                                                        layout: 'bottom-right',
                                                        theme: 'made',
                                                        animation: {
                                                            open: 'animated bounceInLeft',
                                                            close: 'animated bounceOutLeft'
                                                        },
                                                        timeout: 6000,
                                                    });
                                                }
                                            }).error(function (error) {
                                                $('#panel-notif').noty({
                                                    text: '<div class="alert alert-danger media fade in"><p>' + $rootScope.translate('Error occured while requesting the quote') + '!</p></div>',
                                                    layout: 'bottom-right',
                                                    theme: 'made',
                                                    animation: {
                                                        open: 'animated bounceInLeft',
                                                        close: 'animated bounceOutLeft'
                                                    },
                                                    timeout: 6000,
                                                });
                                            });


                        };

                        vm.getAddressInfoByZipConsignor = function (zip) {

                            if (zip.length >= 5 && typeof google != 'undefined') {
                                var addr = {};
                                var geocoder = new google.maps.Geocoder();
                                geocoder.geocode({ 'address': zip }, function (results, status) {
                                    if (status == google.maps.GeocoderStatus.OK) {
                                        if (results.length >= 1) {
                                            var street_number = '';
                                            var route = '';
                                            var street = '';
                                            var city = '';
                                            var state = '';
                                            var zipcode = '';
                                            var country = '';
                                            var formatted_address = '';

                                            for (var ii = 0; ii < results[0].address_components.length; ii++) {

                                                var types = results[0].address_components[ii].types.join(",");
                                                if (types == "street_number") {
                                                    addr.street_number = results[0].address_components[ii].long_name;
                                                }
                                                if (types == "route" || types == "point_of_interest,establishment") {
                                                    addr.route = results[0].address_components[ii].long_name;
                                                }
                                                if (types == "sublocality,political" || types == "locality,political" || types == "neighborhood,political" || types == "administrative_area_level_3,political") {
                                                    addr.city = (city == '' || types == "locality,political") ? results[0].address_components[ii].long_name : city;
                                                }
                                                if (types == "administrative_area_level_1,political") {
                                                    addr.state = results[0].address_components[ii].short_name;
                                                }
                                                if (types == "postal_code" || types == "postal_code_prefix,postal_code") {
                                                    addr.zipcode = results[0].address_components[ii].long_name;
                                                }
                                                if (types == "country,political") {
                                                    addr.country = results[0].address_components[ii].short_name;
                                                }
                                            }
                                            
                                            $scope.$apply(function () {
                                                if (addr.country == vm.shipment.addressInformation.consigner.country) {
                                                    // Check search zip code return the selected country
                                                    vm.shipment.addressInformation.consigner.city = addr.city;
                                                    vm.shipment.addressInformation.consigner.state = addr.state;
                                                    vm.errorCodeConsignor = false;
                                                    addr.success = true;
                                                }
                                                else {
                                                    vm.errorCodeConsignor = true;
                                                }
                                            });

                                        } else {
                                            $scope.$apply(function () {
                                                vm.errorCodeConsignor = true;
                                            });

                                        }
                                    } else {
                                        $scope.$apply(function () {
                                            vm.errorCodeConsignor = true;
                                        });

                                    }
                                });
                            } else {
                                $scope.$apply(function () {
                                    vm.errorCodeConsignor = true;
                                });
                            }
                        }

                        vm.getAddressInfoByZipConsignee = function (zip) {

                            if (zip.length >= 5 && typeof google != 'undefined') {
                                var addr = {};
                                var geocoder = new google.maps.Geocoder();
                                geocoder.geocode({ 'address': zip }, function (results, status) {
                                    if (status == google.maps.GeocoderStatus.OK) {
                                        if (results.length >= 1) {
                                            var street_number = '';
                                            var route = '';
                                            var street = '';
                                            var city = '';
                                            var state = '';
                                            var zipcode = '';
                                            var country = '';
                                            var formatted_address = '';

                                            for (var ii = 0; ii < results[0].address_components.length; ii++) {

                                                var types = results[0].address_components[ii].types.join(",");
                                                if (types == "street_number") {
                                                    addr.street_number = results[0].address_components[ii].long_name;
                                                }
                                                if (types == "route" || types == "point_of_interest,establishment") {
                                                    addr.route = results[0].address_components[ii].long_name;
                                                }
                                                if (types == "sublocality,political" || types == "locality,political" || types == "neighborhood,political" || types == "administrative_area_level_3,political") {
                                                    addr.city = (city == '' || types == "locality,political") ? results[0].address_components[ii].long_name : city;
                                                }
                                                if (types == "administrative_area_level_1,political") {
                                                    addr.state = results[0].address_components[ii].short_name;
                                                }
                                                if (types == "postal_code" || types == "postal_code_prefix,postal_code") {
                                                    addr.zipcode = results[0].address_components[ii].long_name;
                                                }
                                                if (types == "country,political") {
                                                    addr.country = results[0].address_components[ii].short_name;
                                                }
                                            }
                                            
                                            //assign retrieved address details
                                            $scope.$apply(function () {

                                                if (  addr.country == vm.shipment.addressInformation.consignee.country) {
                                                    // Check search zip code return the selected country
                                                    vm.shipment.addressInformation.consignee.city = addr.city;
                                                    vm.shipment.addressInformation.consignee.state = addr.state;
                                                    vm.errorCodeConsignee = false;
                                                    addr.success = true;
                                                }
                                                else {
                                                    vm.errorCodeConsignee = true;
                                                }
                                                
                                            });


                                        } else {
                                            $scope.$apply(function () {
                                                vm.errorCodeConsignee = true;
                                            });

                                        }
                                    } else {
                                        $scope.$apply(function () {

                                            vm.errorCodeConsignee = true;
                                        });

                                    }
                                });
                            } else {
                                $scope.$apply(function () {
                                    vm.errorCodeConsignee = true;
                                });
                            }
                        }

                        vm.requiredConsignorCountry = false;

                        //get the address details via google API
                        vm.getAddressInformationConsignor = function () {

                            vm.requiredConsignorCountry = false;

                            if (vm.shipment.addressInformation.consigner.country == undefined) {
                                vm.errorCode = true;
                            }
                            else if (vm.shipment.addressInformation.consigner.postalcode == null || vm.shipment.addressInformation.consigner.postalcode == '') {
                                vm.errorCode = true;
                            }
                            else {
                                vm.getAddressInfoByZipConsignor(vm.shipment.addressInformation.consigner.postalcode + ' ' + vm.shipment.addressInformation.consigner.country);
                            }

                        }

                        vm.requiredConsigneeCountry = false;
                        //get the address details via google API
                        vm.getAddressInformationConsignee = function () {

                            vm.requiredConsigneeCountry = false;

                            if (vm.shipment.addressInformation.consignee.country == undefined) {
                                vm.requiredConsigneeCountry = true;
                            }
                            else if (vm.shipment.addressInformation.consignee.postalcode == null || vm.shipment.addressInformation.consignee.postalcode == '') {
                                vm.errorCode = true;
                            }
                            else {
                                vm.getAddressInfoByZipConsignee(vm.shipment.addressInformation.consignee.postalcode + ' ' + vm.shipment.addressInformation.consignee.country);
                            }
                        }

                    }]);


})(angular.module('newApp'));