'use strict';

(function (app) {

    app.controller('addShipmentCtrl', ['$location', '$window', 'shipmentFactory', function ($location, $window, shipmentFactory) {
        
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
        vm.shipment.CarrierInformation = {};
        vm.searchRates = false;
        vm.loadingRates = false;
        vm.divisionList = {};
        vm.costcenterList = {};

       

        //get the user and corporate status
        vm.currentRole = $window.localStorage.getItem('userRole');
        vm.isCorporate = $window.localStorage.getItem('isCorporateAccount');

        vm.productTypes = [{ "Id": 1, "Name": "Document" },
                                        { "Id": 2, "Name": "Pallet" },
                                        { "Id": 3, "Name": "Euro Pallet" },
                                        { "Id": 4, "Name": "Diverse" },
                                        { "Id": 5, "Name": "Box" }
        ];

        vm.shipmentTerms = [{ "Id": 1, "Name": "Delivered Duty Unpaid (DDU)" },
                                { "Id": 2, "Name": "Delivered Duty Paid (DDP)" },
                                { "Id": 3, "Name": "Carriage and Insurance Paid (CIP)" },
                                { "Id": 4, "Name": "Carriage Paid To (CPT)" },
                                { "Id": 5, "Name": "Ex Works (EXW)" }
        ];
        vm.shipmentTypes = [{ "Id": 1, "Name": "Door to Door, Prepaid (DDP)" },
                                { "Id": 2, "Name": "Door to Port, Prepaid (DPP)" },
                                { "Id": 3, "Name": "Port to Door, Prepaid (PDP)" },
                                { "Id": 4, "Name": "Port to Port, Prepaid (PPP)" },
                                { "Id": 5, "Name": "Free Carrier (FCA)" }
        ];


        //load the division list
        if (vm.currentRole == "BusinessOwner" || vm.currentRole == "Admin") {
            // shipmentFactory.
            shipmentFactory.loadAllDivisions().success(
               function (responce) {

                   vm.divisionList = responce;
                   
               }).error(function (error) {

               });
            
        } 
        else {

            shipmentFactory.loadAssignedDivisions().success(
            function (responce) {

                vm.divisionList = responce;
                   
            }).error(function (error) {
                console.log("error occurd while retrieving divisions");
            });
           
        }

        //get the cost centers according to the division
        vm.selectDivision = function () {
            var divisionId = vm.shipment.generalInformation.selectedDivision;
            vm.costcenterList = {};
            //  loadAssignedCostCenters
            if (divisionId != '') {
                shipmentFactory.loadAssignedCostCenters(divisionId).success(
               function (responce) {

                   vm.costcenterList = responce;

               }).error(function (error) {

                   console.log("error occurd while retrieving cost centers");
               });

            }
            
        }


        vm.checkGenaralInfo = function (value) {
            if (value==true) {
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
            vm.AddressInformation.consigner = {};
        }
        vm.ClearConsigneeAddress= function(){
            vm.AddressInformation.consignee = {};
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

                count = count + (packages[i].quantity != undefined ? packages[i].quantity : 0);

                totWeight = totWeight + (packages[i].weight != undefined ? packages[i].weight : 0);

                if (packages[i].height != undefined && packages[i].length != undefined && packages[i].width != undefined) {
                    totVolume = totVolume + (packages[i].height * packages[i].length * packages[i].width);
                }
               

            }
            vm.shipment.packageDetails.totalWeight = totWeight;
            vm.shipment.packageDetails.totalVolume = totVolume;
            vm.shipment.packageDetails.count = count;
        }

        //get the calculated rates
        vm.calculateRates = function () {
            vm.loadingRates = true;
            shipmentFactory.calculateRates(vm.shipment).success(
                function (responce) {
                    vm.displayedCollection = responce.items;
                    vm.loadingRates = false;
                    vm.searchRates = true;
                }).error(function (error) {

                });
        }

        vm.selectCarrier = function (row) {

            vm.searchRates = false;
            if (row!=null) {
                vm.shipment.CarrierInformation.carrierName=row.carrier_name;
                vm.shipment.CarrierInformation.pickupDate=row.pickup_date;
                vm.shipment.CarrierInformation.deliveryTime=row.delivery_time;
                vm.shipment.CarrierInformation.price = row.price;
                vm.shipment.CarrierInformation.insurance = row.price * 1.1;

                vm.shipment.CarrierInformation.serviceLevel= row.service_level
                vm.shipment.CarrierInformation.tariffText  = row.tariff_text
                vm.shipment.CarrierInformation.tarriffType = row.tariff_type
                vm.shipment.CarrierInformation.currency = row.currency
                
            }
        }


        //section to set the shipment mode

        vm.selectExpress = function () {

            vm.shipment.generalInformation.express = true;
            vm.shipment.generalInformation.airFreight = false;
            vm.shipment.generalInformation.seaFreight = false;
            vm.shipment.generalInformation.roadFreight = false;
            vm.shipment.generalInformation.all = false;
            $("#express").toggleClass("text-primary");
            $("#airFreight").addClass("text-danger");
            $("#seaFreight").addClass("text-danger");
            $("#roadFreight").addClass("text-danger");
            $("#all").addClass("text-danger");
        }
        vm.selectAir = function () {
            vm.shipment.generalInformation.express = false;
            vm.shipment.generalInformation.airFreight = true;
            vm.shipment.generalInformation.seaFreight = false;
            vm.shipment.generalInformation.roadFreight = false;
            vm.shipment.generalInformation.all = false;

            $("#express").addClass("text-danger");
            $("#airFreight").toggleClass("text-primary");
            $("#seaFreight").addClass("text-danger");
            $("#roadFreight").addClass("text-danger");
            $("#all").addClass("text-danger");
        }
        vm.selectSea = function () {
            vm.shipment.generalInformation.express = false;
            vm.shipment.generalInformation.airFreight = false;
            vm.shipment.generalInformation.seaFreight = true;
            vm.shipment.generalInformation.roadFreight = false;
            vm.shipment.generalInformation.all = false;

            $("#express").addClass("text-danger");
            $("#airFreight").addClass("text-danger");
            $("#seaFreight").toggleClass("text-primary");
            $("#roadFreight").addClass("text-danger");
            $("#all").addClass("text-danger");
        }
        vm.selectRoad = function () {
            vm.shipment.generalInformation.express = false;
            vm.shipment.generalInformation.airFreight = false;
            vm.shipment.generalInformation.seaFreight = false;
            vm.shipment.generalInformation.roadFreight = true;
            vm.shipment.generalInformation.all = false;

            $("#express").addClass("text-danger");
            $("#airFreight").addClass("text-danger");
            $("#seaFreight").addClass("text-danger");
            $("#roadFreight").toggleClass("text-primary");
            $("#all").addClass("text-danger");
        }
        vm.selectall = function () {
            vm.shipment.generalInformation.express = false;
            vm.shipment.generalInformation.airFreight = false;
            vm.shipment.generalInformation.seaFreight = false;
            vm.shipment.generalInformation.roadFreight = false;
            vm.shipment.generalInformation.all = true;

            $("#express").addClass("text-danger");
            $("#airFreight").addClass("text-danger");
            $("#seaFreight").addClass("text-danger");
            $("#roadFreight").addClass("text-danger");
            $("#all").toggleClass("text-primary");
        }


    }]);


})(angular.module('newApp'));