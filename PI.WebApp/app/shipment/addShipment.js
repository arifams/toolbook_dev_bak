﻿'use strict';

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
        vm.shipment.addressInformation = {};
        vm.shipment.addressInformation.consigner = {};
        vm.shipment.addressInformation.consignee = {};
        vm.shipment.CarrierInformation = {};
        vm.searchRates = false;
        vm.loadingRates = false;
        vm.divisionList = {};
        vm.costcenterList = {};
        vm.Expressclass = "btn btn-success";
        vm.Airclass = "btn btn-success";
        vm.Seaclass = "btn btn-success";
        vm.Roadclass = "btn btn-success";
        vm.allclass = "btn btn-success";





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
        vm.shipmentTypes = [{ "Id": "DD-PP", "Name": "Door to Door, Prepaid (DD-PP)" },
                                { "Id": "DP-PP", "Name": "Door to Port, Prepaid (DP-PP)" },
                                { "Id": "PD-PP", "Name": "Port to Door, Prepaid (PD-PP)" },
                                { "Id": "PP-PP", "Name": "Port to Port, Prepaid (PP-PP)" },
                                { "Id": "FCA", "Name": "Free Carrier (FCA)" }
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
            var divisionId = vm.shipment.generalInformation.divisionId;
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
            vm.AddressInformation.consigner = {};
        }

        vm.ClearConsigneeAddress = function () {
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

                var Pieces=packages[i].quantity != undefined ? packages[i].quantity : 0;
                count = count + (Pieces);

                totWeight = totWeight + ((packages[i].weight != undefined ? packages[i].weight : 0)*Pieces);

                if (packages[i].height != undefined && packages[i].length != undefined && packages[i].width != undefined) {
                    totVolume = totVolume +( (packages[i].height * packages[i].length * packages[i].width)*Pieces);
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
            if (row != null) {
                vm.shipment.CarrierInformation.carrierName = row.carrier_name;
                vm.shipment.CarrierInformation.pickupDate = row.pickup_date;
                vm.shipment.CarrierInformation.deliveryTime = row.delivery_time;
                vm.shipment.CarrierInformation.price = row.price;
                vm.shipment.CarrierInformation.insurance = row.price * 1.1;
                vm.shipment.CarrierInformation.totalPrice =parseInt(row.price) + (row.price * 1.1);

                vm.shipment.CarrierInformation.serviceLevel = row.service_level
                vm.shipment.CarrierInformation.tariffText = row.tariff_text
                vm.shipment.CarrierInformation.tarriffType = row.tariff_type
                vm.shipment.CarrierInformation.currency = row.currency

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

        }
        vm.selectAir = function () {
            vm.Expressclass = "btn btn-success";
            vm.Airclass = "btn btn-dark";
            vm.Seaclass = "btn btn-success";
            vm.Roadclass = "btn btn-success";
            vm.allclass = "btn btn-success";
            vm.shipment.generalInformation.shipmentMode = 'AirFreight';
        }
        vm.selectSea = function () {

            vm.Expressclass = "btn btn-success";
            vm.Airclass = "btn btn-success";
            vm.Seaclass = "btn btn-dark";
            vm.Roadclass = "btn btn-success";
            vm.allclass = "btn btn-success";
            vm.shipment.generalInformation.shipmentMode = 'SeaFreight';
        }
        vm.selectRoad = function () {
            vm.Expressclass = "btn btn-success";
            vm.Airclass = "btn btn-success";
            vm.Seaclass = "btn btn-success";
            vm.Roadclass = "btn btn-dark";
            vm.allclass = "btn btn-success";
            vm.shipment.generalInformation.shipmentMode = 'RoadFreight';

        }
        vm.selectall = function () {

            vm.Expressclass = "btn btn-success";
            vm.Airclass = "btn btn-success";
            vm.Seaclass = "btn btn-success";
            vm.Roadclass = "btn btn-success";
            vm.allclass = "btn btn-dark";
            vm.shipment.generalInformation.shipmentMode = 'All';

        }


        vm.submitShipment = function () {
            debugger;
            var body = $("html, body");
            shipmentFactory.submitShipment(vm.shipment).success(
                            function (responce) {
                                debugger;
                                if (response == "success") {
                                    body.stop().animate({ scrollTop: 0 }, '500', 'swing', function () { });

                                    $('#panel-notif').noty({
                                        text: '<div class="alert alert-success media fade in"><p>Shipment saved successfully!</p></div>',
                                        layout: 'bottom-right',
                                        theme: 'made',
                                        animation: {
                                            open: 'animated bounceInLeft',
                                            close: 'animated bounceOutLeft'
                                        },
                                        timeout: 3000,
                                    });
                                }
                            }).error(function (error) {

                            });
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

            }
        };

    }]);


})(angular.module('newApp'));