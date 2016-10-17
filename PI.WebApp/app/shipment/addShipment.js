'use strict';

(function (app) {

    app.controller('addShipmentCtrl', ['$scope', '$location', '$window', 'shipmentFactory', 'ngDialog', '$controller', '$routeParams', '$rootScope', 'customBuilderFactory', function ($scope, $location, $window, shipmentFactory, ngDialog, $controller, $routeParams, $rootScope, customBuilderFactory) {

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
        vm.rateTable = false;

        vm.closeWindow = function () {
            ngDialog.close()
        }

        vm.loadConsignerInfo = function () {

            shipmentFactory.getProfileInfo().success(
               function (responce) {
                   if (responce != null) {

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
                           vm.isInvoicePaymentEnabled = responce.isInvoicePaymentEnabled;
                       }
                   }

               }).error(function (error) {
                   console.log("error occurd while retrieving customer details");
               });

        }

        vm.loadConsignerInfo();

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
            vm.shipmentServices = [{ "Id": "DD-DDP-PP", "Name": "Door-to-Door, DDP, Prepaid" },
                               { "Id": "DD-DDU-PP", "Name": "Door-to-Door, DDU, Prepaid" },
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
                         { "Id": "DD-DDP-PP", "Name": "Door-to-Door, DDP, Prepaid" },
                         { "Id": "DD-DDU-PP", "Name": "Door-to-Door, DDU, Prepaid" },
                         { "Id": "DD-CIP-PP", "Name": "Door-to-Door, CIP, Prepaid" },
                         { "Id": "KMSDY", "Name": "Door-to-Door, SDY, Same Day" },
                        ];

        };

        // Select default values.
        vm.shipment.generalInformation.shipmentServices = "DD-DDU-PP";
        vm.shipment.packageDetails.cmLBS = "true";
        vm.shipment.packageDetails.volumeCMM = "true";
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
                count = count + (Pieces);

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
            vm.rateTable = true;

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
                if (vm.shipment.packageDetails.isInsuared == 'true') {
                    insurance = (row.price * 0.011).toFixed(2);

                    var currencyCode = vm.getCurrenyCode(vm.shipment.packageDetails.valueCurrency);

                    if (insurance < 10 && currencyCode != null && currencyCode == 'USD') {
                        insurance = 10;
                    }
                    if (insurance < 5 && currencyCode != null && currencyCode == 'EUR') {
                        insurance = 5;
                    }
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
                               debugger;
                               if (errors) {
                                   console.log("Encountered errors:");

                                   // This logs all errors encountered during nonce generation to the
                                   // Javascript console.
                                   errors.forEach(function (error) {
                                       console.log('  ' + error.message);
                                   });

                                   // No errors occurred. Extract the card nonce.
                               } else {

                                   debugger;
                                   var body = $("html, body");

                                   // Show payment page.

                                   var paymentDto = {
                                       ChargeAmount: vm.shipment.carrierInformation.totalPrice,
                                       CurrencyType: vm.shipment.carrierInformation.currency,
                                       CardNonce: nonce,
                                       shipmentId: $window.localStorage.getItem('shipmentId'),
                                       userId: $window.localStorage.getItem('userGuid'),
                                       templateLink: '<html><head><title></title></head><body style="margin:30px;"><div style="margin-right:40px;margin-left:40px"><div style="margin-top:30px;background-color:#0af;font-size:28px;border:5px solid #d9d9d9;text-align:center;padding:10px;font-family:verdana,geneva,sans-serif;color:#fff">Order Confirmation - Parcel International</div></div><div style="margin-right:40px;margin-left:40px"><div style="float:left;"><img alt="" src="http://www.parcelinternational.nl/assets/Uploads/_resampled/SetWidth495-id-parcel-big.jpg" style="width: 130px; height: 130px;" /></div><div><h3 style="margin-bottom:65px;margin-right:146px;margin-top:0;padding-top:62px;text-align:center;font-size:22px;font-family:verdana,geneva,sans-serif;color:#005c99">Thank you for using Parcel International </h3></div></div><div style="margin-right:40px;margin-left:40px"><div style="padding:10px;font-family:verdana,geneva,sans-serif;color:#fff;border:5px solid #0af;background-color:#005c99;font-size:13px"><p style="font-weight:700;font-style:italic;font-size:16px;">Order Reference  &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<OrderReference></OrderReference></p><p style="font-weight:700;font-style:italic;font-size:16px;">Pickup Date  &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; <PickupDate></PickupDate></p><p style="font-weight:700;font-style:italic;font-size:16px;">Shipment Mode  &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; <ShipmentMode></ShipmentMode></p><p style="font-weight:700;font-style:italic;font-size:16px;">Shipment Type  &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; <ShipmentType></ShipmentType></p><p style="font-weight:700;font-style:italic;font-size:16px;">Carrier   &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<Carrier></Carrier></p><p style="font-weight:700;font-style:italic;font-size:16px;">Shipment Price   &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<ShipmentPrice></ShipmentPrice></p><p style="font-weight:700;font-style:italic;font-size:16px">Payment Type   &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<PaymentType></PaymentType></p></div><br><div style="padding:10px;font-family:verdana,geneva,sans-serif;color:#fff;border:5px solid #0af;background-color:#005c99;font-size:13px"><table><thead><tr><th style="width:290px;color:#fff;font-size:16px;border-bottom:2px solid #fff;">Product Type</th><th style="width:290px;color:#fff;font-size:16px;border-bottom:2px solid #fff;">Quantity</th><th style="width:290px;color:#fff;font-size:16px;border-bottom:2px solid #fff;">Weight</th><th style="width:290px;color:#fff;font-size:16px;border-bottom:2px solid #fff;">Volume</th></tr></thead><tbody><tableRecords></tbody></table></div><p style="font-size:20px;text-align:center;">should you have any questions or concerns, please contact Parcel International helpdesk for support.</p></body></html>'
                                   };

                                   shipmentFactory.PaymentCharge(paymentDto).success(
                                                   function (response) {
                                                       
                                                       addShipmentResponse(response);

                                                   }).error(function (error) {

                                                       $('#panel-notif').noty({
                                                           text: '<div class="alert alert-danger media fade in"><p>' + $rootScope.translate('Error occured while processing payment') + '!</p></div>',
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
        
        //section to set the shipment mode
        function addShipmentResponse(response) {
            debugger;
            vm.loadingSymbole = false;
            vm.shipmentStatusMsg = response.message;
            vm.isShowResponse = true;

            if (response.status == 2) {
                // Success both payment and shipment.
                
                vm.isShowPaymentForm = false;                
                vm.labelUrl = response.labelURL;
             
                vm.isShowLabel = true;
                if (response.invoiceURL != '') {
                    vm.isShowInvoice = true;
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
            else if (response.status == 5) {
                // SISError.
                vm.isShowPaymentForm = false;
                vm.errorUrl = 'http://parcelinternational.pro/errors/' + response.carrierName + '/' + response.shipmentCode;
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

        vm.submitShipment = function () {
            
            vm.shipment.generalInformation.shipmentPaymentTypeId = 1; // Payment type is Invoice.

            vm.carrierselected = false;
            vm.isPrevDisabled = true;

            saveShipment();
        }

        function saveShipment() {

            vm.loadingSymbole = true;

            vm.addingShipment = true;
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

                                    $window.localStorage.setItem('shipmentId', response.shipmentId);

                                    if (vm.shipment.generalInformation.shipmentPaymentTypeId == 1) {
                                        //window.location = webBaseUrl + "/app/index.html#/PaymentResult?status=0&amount=0&currency=USD&description=0&hash=0&id_sale=0";

                                        // Send shipment to SIS.

                                        var sendShipmentData = {
                                            shipmentId: $window.localStorage.getItem('shipmentId'),
                                            userId: $window.localStorage.getItem('userGuid'),
                                            templateLink: '<html><head><title></title></head><body style="margin:30px;"><div style="margin-right:40px;margin-left:40px"><div style="margin-top:30px;background-color:#0af;font-size:28px;border:5px solid #d9d9d9;text-align:center;padding:10px;font-family:verdana,geneva,sans-serif;color:#fff">Order Confirmation - Parcel International</div></div><div style="margin-right:40px;margin-left:40px"><div style="float:left;"><img alt="" src="http://www.parcelinternational.nl/assets/Uploads/_resampled/SetWidth495-id-parcel-big.jpg" style="width: 130px; height: 130px;" /></div><div><h3 style="margin-bottom:65px;margin-right:146px;margin-top:0;padding-top:62px;text-align:center;font-size:22px;font-family:verdana,geneva,sans-serif;color:#005c99">Thank you for using Parcel International </h3></div></div><div style="margin-right:40px;margin-left:40px"><div style="padding:10px;font-family:verdana,geneva,sans-serif;color:#fff;border:5px solid #0af;background-color:#005c99;font-size:13px"><p style="font-weight:700;font-style:italic;font-size:16px;">Order Reference  &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<OrderReference></OrderReference></p><p style="font-weight:700;font-style:italic;font-size:16px;">Pickup Date  &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; <PickupDate></PickupDate></p><p style="font-weight:700;font-style:italic;font-size:16px;">Shipment Mode  &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; <ShipmentMode></ShipmentMode></p><p style="font-weight:700;font-style:italic;font-size:16px;">Shipment Type  &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; <ShipmentType></ShipmentType></p><p style="font-weight:700;font-style:italic;font-size:16px;">Carrier   &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<Carrier></Carrier></p><p style="font-weight:700;font-style:italic;font-size:16px;">Shipment Price   &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<ShipmentPrice></ShipmentPrice></p><p style="font-weight:700;font-style:italic;font-size:16px">Payment Type   &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<PaymentType></PaymentType></p></div><br><div style="padding:10px;font-family:verdana,geneva,sans-serif;color:#fff;border:5px solid #0af;background-color:#005c99;font-size:13px"><table><thead><tr><th style="width:290px;color:#fff;font-size:16px;border-bottom:2px solid #fff;">Product Type</th><th style="width:290px;color:#fff;font-size:16px;border-bottom:2px solid #fff;">Quantity</th><th style="width:290px;color:#fff;font-size:16px;border-bottom:2px solid #fff;">Weight</th><th style="width:290px;color:#fff;font-size:16px;border-bottom:2px solid #fff;">Volume</th></tr></thead><tbody><tableRecords></tbody></table></div><p style="font-size:20px;text-align:center;">should you have any questions or concerns, please contact Parcel International helpdesk for support.</p></body></html>'
                                        };

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

        vm.isShowPaymentForm = false;
        
        vm.payOnline = function () {
            vm.shipment.generalInformation.shipmentPaymentTypeId = 2; // Payment type is Online.

            vm.carrierselected = false;
            vm.isPrevDisabled = true;

            saveShipment();

            //vm.isShowPaymentForm = true;
            //paymentForm.build();
        };

        vm.chargeFromCard = function () {

            vm.loadingSymbole = true;
            paymentForm.requestCardNonce();

        }

        function saveShipment_NotUsing() {
            vm.shipment.generalInformation.shipmentPaymentTypeId = 2; // Payment type is Online.
            shipmentFactory.saveShipment(vm.shipment).success(
                            function (response) {

                                if (response.status == 2) {
                                    // Successfully saved in db.
                                    $window.localStorage.setItem('shipmentId', response.shipmentId);
                                }
                                else {
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

        var loadShipmentInfo = function (code, id) {
            shipmentFactory.loadShipmentInfo(code, id)
            .success(function (data) {
                vm.shipment = data;

                if (vm.shipment.packageDetails.cmlbs == true) {
                    vm.shipment.packageDetails.cmLBS = "true";
                }
                else {
                    vm.shipment.packageDetails.cmLBS = "false";
                }

                if (vm.shipment.packageDetails.volumeCMM == true) {
                    vm.shipment.packageDetails.volumeCMM = "true";
                }
                else {
                    vm.shipment.packageDetails.volumeCMM = "false";
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

        vm.shipment.generalInformation.shipmentCode = "0";
        if ($routeParams.id != "0") {
            vm.editShipmentCode = $routeParams.id;
            // (ShipmentCode, ShipmentId)
            loadShipmentInfo('', $routeParams.id);
        }

        // In production remove this.
        vm.textChangeOfName = function () {

            if (vm.shipment.generalInformation.shipmentName == "code123") {

                vm.shipment.addressInformation.consigner = {};
                vm.shipment.addressInformation.consigner.firstName = 'Comp1';
                vm.shipment.addressInformation.consigner.lastName = 'Comp11';
                vm.shipment.addressInformation.consigner.country = 'US';
                vm.shipment.addressInformation.consigner.postalcode = '94404';
                vm.shipment.addressInformation.consigner.number = '901';
                vm.shipment.addressInformation.consigner.address1 = 'Mariners Island Boulevard';
                vm.shipment.addressInformation.consigner.address2 = '';
                vm.shipment.addressInformation.consigner.city = 'San Mateo';
                vm.shipment.addressInformation.consigner.state = 'CA';
                vm.shipment.addressInformation.consigner.email = 'test1@yopmail.com';
                vm.shipment.addressInformation.consigner.contactNumber = '1111111111';
                vm.shipment.addressInformation.consigner.contactName = "contact name A";

                vm.shipment.addressInformation.consignee = {};
                vm.shipment.addressInformation.consignee.firstName = 'Comp2';
                vm.shipment.addressInformation.consignee.lastName = 'Comp22';
                vm.shipment.addressInformation.consignee.country = 'US';
                vm.shipment.addressInformation.consignee.postalcode = '94405';
                vm.shipment.addressInformation.consignee.number = '902';
                vm.shipment.addressInformation.consignee.address1 = 'Mariners Island Boulevard';
                vm.shipment.addressInformation.consignee.address2 = '';
                vm.shipment.addressInformation.consignee.city = 'San Mateo';
                vm.shipment.addressInformation.consignee.state = 'CA';
                vm.shipment.addressInformation.consignee.email = 'test2@yopmail.com';
                vm.shipment.addressInformation.consignee.contactNumber = '2111111111';
                vm.shipment.addressInformation.consignee.contactName = "contact name B";

                vm.shipment.packageDetails.shipmentDescription = "testDesc";
                vm.shipment.packageDetails.declaredValue = 500;
            }
            if (vm.shipment.generalInformation.shipmentName == "code123h") {
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
                            addr.success = true;
                            $scope.$apply(function () {
                                vm.shipment.addressInformation.consigner.city = addr.city;
                                vm.shipment.addressInformation.consigner.state = addr.state;
                                vm.shipment.addressInformation.consigner.country = addr.country;
                                vm.errorCodeConsignor = false;
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
                            addr.success = true;
                            //assign retrieved address details
                            $scope.$apply(function () {
                                vm.shipment.addressInformation.consignee.city = addr.city;
                                vm.shipment.addressInformation.consignee.state = addr.state;
                                vm.shipment.addressInformation.consignee.country = addr.country;
                                vm.errorCodeConsignee = false;
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

        //get the address details via google API
        vm.getAddressInformationConsignor = function () {

            if (vm.shipment.addressInformation.consigner.postalcode == null || vm.shipment.addressInformation.consigner.postalcode == '') {
                vm.errorCode = true;
            } else {
                vm.getAddressInfoByZipConsignor(vm.shipment.addressInformation.consigner.postalcode);
            }

        }

        //get the address details via google API
        vm.getAddressInformationConsignee = function () {

            if (vm.shipment.addressInformation.consignee.postalcode == null || vm.shipment.addressInformation.consignee.postalcode == '') {
                vm.errorCode = true;
            } else {
                vm.getAddressInfoByZipConsignee(vm.shipment.addressInformation.consignee.postalcode);
            }
        }

    }]);


})(angular.module('newApp'));