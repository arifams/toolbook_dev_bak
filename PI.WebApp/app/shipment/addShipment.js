'use strict';

(function (app) {

    app.controller('addShipmentCtrl', ['$scope', '$location', '$window', 'shipmentFactory', function ($scope,$location, $window, shipmentFactory) {

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
        vm.shipment.CarrierInformation = {};
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
        
        vm.shipment.userId = $window.localStorage.getItem('userGuid');
        vm.hidedivisions = false;
        vm.hidecostcenters = true;
        vm.paylane = {};

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
        vm.shipment.packageDetails.valueCurrency = 1;
        

        shipmentFactory.loadAllCurrencies()
            .success(
               function (responce) {
                   debugger;
                   vm.currencies = responce;
               }).error(function (error) {
                   console.log("error occurd while retrieving currencies");
               });


        //load the division list
        if (vm.currentRole == "BusinessOwner" || vm.currentRole == "Admin") {
            // shipmentFactory.
            shipmentFactory.loadAllDivisions().success(
               function (responce) {
                   if (responce.length>0) {
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
                if (responce.length>0) {
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
                   if (responce.length>0) {
                       vm.costcenterList = responce;
                       vm.hidecostcenters = false;
                   }      
                  
                 

               }).error(function (error) {

                   console.log("error occurd while retrieving cost centers");
               });

            }

        }


        vm.checkGenaralInfo = function (value) {
            if (value == true) {
                vm.collapse1 = true;
                vm.collapse2 = false;
            }
            vm.generalInfoisSubmit = true;


        }

        vm.checkConsignInfo = function (value) {
            if (value == true) {
                vm.collapse2 = true;
                vm.collapse3 = false;
            }
            vm.consignInfoisSubmit = true

        }

        vm.checkPackageDetails = function (value) {

            if (value) {
                vm.collapse3 = true;
                vm.collapse4 = false;
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

        //accordian functionality
        //$(document).ready(function () {
        //    $('#accordion').accordion();
        //    $("#accordion").accordion({ event: false });
        //    $('#accordion button').click(function (e) {
        //        var delta
        //        e.preventDefault();
        //        if ($(this).is('.btn.btn-blue')) {
        //            delta = 1;
        //        }
        //        if ($(this).is('.btn.btn-dark')) {
        //            delta = -1;
        //        }               
        //        $('#accordion').accordion('option', 'active', ($('#accordion').accordion('option', 'active') + delta));
        //    });
        //});


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
                  }
                    
                }).error(function (error) {

                });
        }

        vm.selectCarrier = function (row) {
            var total = 0.0;
            var insurance = 0.0;
            vm.searchRates = false;
            if (row != null) {
                vm.shipment.CarrierInformation.carrierName = row.carrier_name;               
                vm.shipment.CarrierInformation.pickupDate = row.pickup_date;
                vm.shipment.CarrierInformation.deliveryTime = row.delivery_time;
                vm.shipment.CarrierInformation.price = row.price;
                if (vm.shipment.packageDetails.isInsuared=='true') {
                    insurance = (row.price * 1.1).toFixed(2);
                   
                   var currencyCode= vm.getCurrenyCode(vm.shipment.packageDetails.valueCurrency);
                   
                   if (insurance < 10 && currencyCode!=null && currencyCode == 'USD') {
                        insurance = 10;
                    }
                   if (insurance < 5 && currencyCode!=null && currencyCode == 'EUR') {
                        insurance=5;
                    }
                }
                
                vm.shipment.CarrierInformation.insurance = insurance;
                total = parseFloat(row.price) + parseFloat(insurance);
                vm.shipment.CarrierInformation.totalPrice = total.toFixed(2);
                vm.shipment.CarrierInformation.serviceLevel = row.service_level
                vm.shipment.CarrierInformation.tariffText = row.tariff_text
                vm.shipment.CarrierInformation.tarriffType = row.tariff_type
                vm.shipment.CarrierInformation.currency = row.currency

              
                //get the paylane related details
                vm.paylane.currency = vm.shipment.CarrierInformation.currency;
                vm.paylane.amount = vm.shipment.CarrierInformation.totalPrice;
                vm.paylane.transactionType = 'S';

                shipmentFactory.getHashCodesForPaylane(vm.paylane).success(
               function (responce) {                 
                   vm.paylane.description= responce.description;                   
                   vm.paylane.hash = responce.hash;
                   vm.paylane.merchantId = responce.merchantId
               }).error(function (error) {

               });

            }
        }


        //section to set the shipment mode

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
            vm.addingShipment = true;
            var body = $("html, body");
            shipmentFactory.saveShipment(vm.shipment).success(
                            function (response) {
                                vm.addingShipment = false;                                
                                if (response.status == "Success") {
                                    body.stop().animate({ scrollTop: 0 }, '500', 'swing', function () { });

                                    $('#panel-notif').noty({
                                        text: '<div class="alert alert-success media fade in"><p>Shipment saved successfully!</p></div>',
                                        layout: 'bottom-right',
                                        theme: 'made',
                                        animation: {
                                            open: 'animated bounceInLeft',
                                            close: 'animated bounceOutLeft'
                                        },
                                        timeout: 6000,
                                    });

                                    console.info("Add Shipment XML: ");
                                    console.info(response.addShipmentXML);
                                }
                                else {
                                    vm.addingShipment = false;
                                    body.stop().animate({ scrollTop: 0 }, '500', 'swing', function () { });

                                    $('#panel-notif').noty({
                                        text: '<div class="alert alert-danger media fade in"><p>Error occured while saving the Shipment!</p></div>',
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
                                    text: '<div class="alert alert-danger media fade in"><p>Error occured while saving the Shipment!</p></div>',
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


        vm.payOnline = function () {
            //vm.addingShipment = true;
            var body = $("html, body");
            shipmentFactory.saveShipment(vm.shipment).success(
                            function (response) {
                                if (response != 0) {
                                    // Successfully saved in db.
                                    $window.localStorage.setItem('shipmentId', response);
                                    $('#paylane_form').submit();
                                }
                                else {
                                    $('#panel-notif').noty({
                                        text: '<div class="alert alert-danger media fade in"><p>Error occured while saving the Shipment!</p></div>',
                                        layout: 'bottom-right',
                                        theme: 'made',
                                        animation: {
                                            open: 'animated bounceInLeft',
                                            close: 'animated bounceOutLeft'
                                        },
                                        timeout: 6000,
                                    });
                                }
                                    //vm.addingShipment = false;
                                    //if (response.status == "Success") {
                                    //    body.stop().animate({ scrollTop: 0 }, '500', 'swing', function () { });

                                    //    console.info("Add Shipment XML: ");
                                    //    console.info(response.addShipmentXML);

                                    //    $('#paylane_form').submit();
                                    //}
                                    //else {
                                    //    vm.addingShipment = false;
                                    //    body.stop().animate({ scrollTop: 0 }, '500', 'swing', function () { });

                                    //    $('#panel-notif').noty({
                                    //        text: '<div class="alert alert-danger media fade in"><p>Error occured while saving the Shipment!</p></div>',
                                    //        layout: 'bottom-right',
                                    //        theme: 'made',
                                    //        animation: {
                                    //            open: 'animated bounceInLeft',
                                    //            close: 'animated bounceOutLeft'
                                    //        },
                                    //        timeout: 6000,
                                    //    });
                                //}
                            }).error(function (error) {
                                $('#panel-notif').noty({
                                    text: '<div class="alert alert-danger media fade in"><p>Error occured while saving the Shipment!</p></div>',
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
        };

        vm.changeConsigneeCountry = function () {
            vm.isRequiredConsigneeState = vm.shipment.addressInformation.consignee.country == 'US' || vm.shipment.addressInformation.consignee.country == 'CA' || vm.shipment.addressInformation.consignee.country == 'PR' || vm.shipment.addressInformation.consignee.country == 'AU';
        };

        vm.getCurrenyCode=function(key){
            for (var i = 0; i < vm.currencies.length; i++) {
                if (vm.currencies[i].id == key) {
                    var currency = vm.currencies[i].currencyCode;
                    return currency
                }           
            }          
        }
        //clear carrier information if previous button clicked
        vm.previousBtnClicked = function () {

            vm.shipment.CarrierInformation={};
            vm.collapse3=false;
            vm.collapse4=true;
            vm.previousClicked = true;
        }
        // In production remove this.
        vm.textChangeOfName = function () {
           
            if (vm.shipment.generalInformation.shipmentName == "code123") {

                vm.shipment.addressInformation.consigner = {};
                vm.shipment.addressInformation.consigner.name = 'Comp1';
                vm.shipment.addressInformation.consigner.country = 'US';
                vm.shipment.addressInformation.consigner.postalcode = '94404';
                vm.shipment.addressInformation.consigner.number = '901';
                vm.shipment.addressInformation.consigner.address1 = 'Mariners Island Boulevard';
                vm.shipment.addressInformation.consigner.address2 = '';
                vm.shipment.addressInformation.consigner.city = 'San Mateo';
                vm.shipment.addressInformation.consigner.state = 'CA';
                vm.shipment.addressInformation.consigner.email = 'test1@yopmail.com';
                vm.shipment.addressInformation.consigner.contactNumber = '1111111111';

                vm.shipment.addressInformation.consignee = {};
                vm.shipment.addressInformation.consignee.name = 'Comp2';
                vm.shipment.addressInformation.consignee.country = 'US';
                vm.shipment.addressInformation.consignee.postalcode = '94405';
                vm.shipment.addressInformation.consignee.number = '902';
                vm.shipment.addressInformation.consignee.address1 = 'Mariners Island Boulevard';
                vm.shipment.addressInformation.consignee.address2 = '';
                vm.shipment.addressInformation.consignee.city = 'San Mateo';
                vm.shipment.addressInformation.consignee.state = 'CA';
                vm.shipment.addressInformation.consignee.email = 'test2@yopmail.com';
                vm.shipment.addressInformation.consignee.contactNumber = '2111111111';

                vm.shipment.packageDetails.shipmentDescription = "testDesc";
            }
        };

    }]);


})(angular.module('newApp'));