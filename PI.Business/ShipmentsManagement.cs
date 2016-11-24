using OfficeOpenXml;
using OfficeOpenXml.Style;
using PI.Common;
using PI.Contract.Business;
using PI.Contract.DTOs.Carrier;
using PI.Contract.DTOs.Common;
using PI.Contract.DTOs.Division;
using PI.Contract.DTOs.FileUpload;
using PI.Contract.DTOs.RateSheets;
using PI.Contract.DTOs.Report;
using PI.Contract.DTOs.Shipment;
using PI.Contract.DTOs.Dashboard;
using PI.Contract.Enums;
using PI.Data;
using PI.Data.Entity;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using PI.Contract.DTOs.CostCenter;
using PI.Contract.DTOs.Address;
using PI.Contract.DTOs.Company;
using PI.Contract;
using AzureMediaManager;
using PI.Contract.DTOs;
using PI.Contract.DTOs.Payment;
using PI.Data.Entity.Identity;
using PI.Contract.DTOs.Postmen;
using System.Data.Entity.Validation;
using Microsoft.ServiceBus.Messaging;
using System.Net;
using System.Collections.Specialized;
using System.Xml.Linq;
using RestSharp.Serializers;
using System.IO;
using AutoMapper;

namespace PI.Business
{
    public class ShipmentsManagement : IShipmentManagement
    {
        private PIContext context;
        ICarrierIntegrationManager sisManager = null;
        ICarrierIntegrationManager stampsManager = new StampsIntegrationManager();
        ICompanyManagement companyManagment;
        private ILogger logger;
        IPaymentManager paymentManager;
        StampsIntegrationManager stampsMenmanager = new StampsIntegrationManager();


        public string SISWebURLUS
        {
            get
            {
                return ConfigurationManager.AppSettings["SISWebURLUS"].ToString();
            }
        }


        public ShipmentsManagement(ILogger logger, ICompanyManagement companyManagment, ICarrierIntegrationManager sisManager, IPaymentManager paymentManager, PIContext _context = null)
        {
            //if (_context==null)
            //{
            //    sisManager = new SISIntegrationManager(logger); // TODO : H - Need to pass from service using autofac
            //}
            //else
            //{
            //    sisManager = new MockSISIntegrationManager(_context);   // TODO : H - Remove this context. and pass mock context
            //}
            // this.postMenmanager = new PostmenIntegrationManager(logger);
            this.sisManager = sisManager;
            context = _context ?? PIContext.Get();
            this.companyManagment = companyManagment;
            this.logger = logger;
            this.paymentManager = paymentManager;
        }

        public ShipmentcostList GetRateSheet(ShipmentDto currentShipment)
        {
            // SISIntegrationManager sisManager = new SISIntegrationManager();
            RateSheetParametersDto currentRateSheetDetails = new RateSheetParametersDto();


            if (currentShipment == null)
            {
                return null;
            }
            if (currentShipment.GeneralInformation != null)
            {
                //  currentRateSheetDetails.type = currentShipment.GeneralInformation.shipmentType;
                // currentRateSheetDetails.
                if (currentShipment.GeneralInformation.ShipmentMode == "Express")
                {
                    currentRateSheetDetails.courier = "UPSDHLFEDTNT";
                }
                else if (currentShipment.GeneralInformation.ShipmentMode == "AirFreight")
                {
                    currentRateSheetDetails.courier_air = "EME";
                }
                else if (currentShipment.GeneralInformation.ShipmentMode == "SeaFreight")
                {
                    currentRateSheetDetails.courier_sea = "";
                }
                else if (currentShipment.GeneralInformation.ShipmentMode == "RoadFreight")
                {
                    currentRateSheetDetails.courier_road = "";
                }
                else
                {
                    //select all shipment modes
                    currentRateSheetDetails.courier = "UPSDHLFEDTNT";
                    currentRateSheetDetails.courier_air = "EME";
                    currentRateSheetDetails.courier_sea = "EME";
                    currentRateSheetDetails.courier_road = "EME";
                }

                currentRateSheetDetails.delivery_condition = currentShipment.GeneralInformation.ShipmentServices;
            }
            if (currentShipment.AddressInformation != null)
            {
                //consigner details
                currentRateSheetDetails.address1 = string.Format("{0} {1}", currentShipment.AddressInformation.Consigner.FirstName, currentShipment.AddressInformation.Consigner.LastName).Replace(' ', '%');
                currentRateSheetDetails.address2 = currentShipment.AddressInformation.Consigner.Address1.Replace(' ', '%');
                currentRateSheetDetails.address3 = currentShipment.AddressInformation.Consigner.Address2 != null ? currentShipment.AddressInformation.Consigner.Address2.Replace(' ', '%') : string.Empty;
                currentRateSheetDetails.address4 = currentShipment.AddressInformation.Consigner.City.Replace(' ', '%');
                currentRateSheetDetails.street_number = currentShipment.AddressInformation.Consigner.Number;
                currentRateSheetDetails.postcode = currentShipment.AddressInformation.Consigner.Postalcode;
                currentRateSheetDetails.country_from = currentShipment.AddressInformation.Consigner.Country;
                currentRateSheetDetails.code_country_from = currentShipment.AddressInformation.Consigner.Country;

                //consignee details
                currentRateSheetDetails.address11 = string.Format("{0} {1}", currentShipment.AddressInformation.Consignee.FirstName, currentShipment.AddressInformation.Consignee.LastName).Replace(' ', '%');
                currentRateSheetDetails.address12 = currentShipment.AddressInformation.Consignee.Address1.Replace(' ', '%');
                currentRateSheetDetails.address13 = currentShipment.AddressInformation.Consignee.Address2 != null ? currentShipment.AddressInformation.Consignee.Address2.Replace(' ', '%') : string.Empty;
                currentRateSheetDetails.address14 = currentShipment.AddressInformation.Consignee.City.Replace(' ', '%');
                currentRateSheetDetails.street_number_delivery = currentShipment.AddressInformation.Consignee.Number;
                currentRateSheetDetails.postcode_delivery = currentShipment.AddressInformation.Consignee.Postalcode;
                currentRateSheetDetails.country_to = currentShipment.AddressInformation.Consignee.Country;
                currentRateSheetDetails.code_country_to = currentShipment.AddressInformation.Consignee.Country;

                //  currentRateSheetDetails.inbound = this.GetInboundoutBoundStatus(currentShipment.UserId, currentShipment.AddressInformation.Consigner.Country, currentShipment.AddressInformation.Consignee.Country);
                currentRateSheetDetails.inbound = "N";

            }
            if (currentShipment.PackageDetails != null)
            {
                decimal maxLength = 0;
                decimal maxWidth = 0;
                decimal maxHeight = 0;
                decimal surface = 0;
                decimal maxdimension = 0;
                decimal pieces = 0;
                decimal volume = 0;
                decimal maxVolume = 0;
                decimal weight = 0;
                decimal maxWeight = 0;
                string package = string.Empty;
                int count = 0;
                string codeCurrenyString = "";

                //using (var context = PIContext.Get())
                //{
                codeCurrenyString = context.Currencies.Where(c => c.Id == currentShipment.PackageDetails.ValueCurrency).Select(c => c.CurrencyCode).ToList().First();
                //}

                foreach (var item in currentShipment.PackageDetails.ProductIngredients)
                {
                    item.Length = currentShipment.PackageDetails.VolumeUnit == "/(cm)" ? Math.Round(item.Length, 2) : Math.Round((item.Length * (decimal)2.54), 2); // currentShipment.PackageDetails.VolumeCMM ? cm : inch;
                    item.Width = currentShipment.PackageDetails.VolumeUnit == "/(cm)" ? Math.Round(item.Width, 2) : Math.Round((item.Width * (decimal)2.54), 2); // currentShipment.PackageDetails.VolumeCMM ? cm : inch;
                    item.Height = currentShipment.PackageDetails.VolumeUnit == "/(cm)" ? Math.Round(item.Height, 2) : Math.Round((item.Height * (decimal)2.54), 2);


                    if (count == 0)
                    {
                        package = item.ProductType;
                    }
                    if (count > 0 && package != item.ProductType)
                    {
                        package = "DIVERSE";
                    }

                    if (item.Length > maxLength)
                    {
                        maxLength = item.Length;
                    }
                    if (item.Width > maxWidth)
                    {
                        maxWidth = item.Width;
                    }
                    if (item.Height > maxHeight)
                    {
                        maxHeight = item.Height;
                    }
                    if (item.Weight > maxWeight)
                    {
                        maxWeight = item.Weight;
                    }

                    surface = surface + (item.Length * item.Width * item.Quantity);
                    pieces = pieces + item.Quantity;
                    volume = volume + item.Length * item.Width * item.Height * item.Quantity;
                    weight = weight + item.Weight * item.Quantity;
                    count++;
                }
                maxdimension = maxLength + (maxWidth * 2) + (maxHeight * 2);
                maxVolume = maxLength * maxWidth * maxHeight;


                currentRateSheetDetails.weight = currentShipment.PackageDetails.WeightUnit == "/(kg)" ? Math.Round(weight, 2).ToString() : Math.Round((weight / (decimal)2.20462), 2).ToString(); // addShipment.PackageDetails.CmLBS ? "kg" : "lbs" ;
                currentRateSheetDetails.width = maxWidth.ToString();
                currentRateSheetDetails.height = maxHeight.ToString();
                currentRateSheetDetails.max_length = maxLength.ToString();
                currentRateSheetDetails.max_actual_length = currentRateSheetDetails.max_length;
                currentRateSheetDetails.max_width = currentRateSheetDetails.width;
                currentRateSheetDetails.max_height = currentRateSheetDetails.height;
                currentRateSheetDetails.max_weight = currentShipment.PackageDetails.WeightUnit == "/(kg)" ? Math.Round(maxWeight, 2).ToString() : Math.Round((maxWeight / (decimal)2.20462), 2).ToString();
                currentRateSheetDetails.pieces = pieces.ToString();
                currentRateSheetDetails.surface = surface.ToString();
                currentRateSheetDetails.max_dimension = maxdimension.ToString();
                currentRateSheetDetails.volume = volume.ToString();
                currentRateSheetDetails.max_volume = maxVolume.ToString();
                currentRateSheetDetails.value = currentShipment.PackageDetails.DeclaredValue.ToString();
                currentRateSheetDetails.package = package;
                currentRateSheetDetails.code_currency = codeCurrenyString;

                if (currentShipment.PackageDetails.IsDG)
                {
                    currentRateSheetDetails.dg = "YES";

                    currentRateSheetDetails.dg_type = currentShipment.PackageDetails.DGType;
                    //set the dg accessibility
                    if (currentShipment.PackageDetails.Accessibility)
                    {
                        currentRateSheetDetails.dg_accessible = "Y";
                    }
                    else
                    {
                        currentRateSheetDetails.dg_accessible = "N";
                    }

                }
                else
                {
                    currentRateSheetDetails.dg = "NO";
                }


                // Pass local time to SIS. Later stage need to change this to pickup address local date.
                currentRateSheetDetails.date_pickup = currentShipment.PackageDetails.PreferredCollectionDate;
                DateTime localDateTime = context.GetLocalTimeByUser(currentShipment.UserId, DateTime.UtcNow);
                currentRateSheetDetails.time_pickup = localDateTime.AddHours(2).ToString("H:mm");   // Get local time add two hours.

                currentRateSheetDetails.UserIdForTimeConvert = currentShipment.UserId;

                if (currentShipment.PackageDetails.IsInsuared == "true")
                {
                    currentRateSheetDetails.insurance_instruction = "Y";
                }
                else
                {
                    currentRateSheetDetails.insurance_instruction = "N";
                }

                //if (currentShipment.PackageDetails.PreferredCollectionDate != null)
                //{
                //    try
                //    {
                //        currentRateSheetDetails.date_pickup = DateTime.ParseExact(currentShipment.PackageDetails.PreferredCollectionDate, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture).ToString("dd-MMM-yyyy HH:mm");
                //    }
                //    catch (Exception)
                //    {
                //        currentRateSheetDetails.date_pickup = "00-Jan-0000 00:00";
                //    }

                //}

                //if (currentShipment.PackageDetails.CmLBS)
                //{
                //    currentRateSheetDetails.weight_unit = "kg";
                //}
                //else
                //{
                //    currentRateSheetDetails.weight_unit = "lbs";
                //}

                //if (currentShipment.PackageDetails.VolumeCMM)
                //{
                //    currentRateSheetDetails.volume_unit = "cm";
                //}
                //else
                //{
                //    currentRateSheetDetails.volume_unit = "inch";
                //}

                currentRateSheetDetails.weight_unit = "kg";
                currentRateSheetDetails.volume_unit = "cm";

            }
            //hardcoded values for now

            //currentRateSheetDetails.userid = "User@Mitrai.com";
            //currentRateSheetDetails.password = "Mitrai462";
            currentRateSheetDetails.output = "XML";
            currentRateSheetDetails.type_xml = "LIST";
            currentRateSheetDetails.vat = "NO";
            currentRateSheetDetails.default_off = "YES";
            currentRateSheetDetails.type = "selectkmnetworkroad";
            currentRateSheetDetails.fieldname4 = "shipment_price";
            currentRateSheetDetails.fieldname1 = "price";
            currentRateSheetDetails.sell_buy = "";
            currentRateSheetDetails.courier_km = "";
            currentRateSheetDetails.courier_tariff_base = "";
            currentRateSheetDetails.courier_date_pickup_transition = "";
            currentRateSheetDetails.language = "EN";
            currentRateSheetDetails.print_button = "";
            currentRateSheetDetails.country_distance = "";
            // currentRateSheetDetails.courier_tariff_type = "NLPARUPS:NLPARFED:USPARDHL2:USPARTNT:USPARUPS:USPARFED2:USUPSTNT:USPAREME:USPARPAE:NLPARTNT2:NLPARDPD:USPARUSP";
            // currentRateSheetDetails.courier_tariff_type = "NLPARUPS:NLPARFED:USPARDHL2:USPARTNT:USPARUPS:USPARFED2:USUPSTNT:USPAREME:USPARPAE:NLPARTNT2:NLPARDPD";
            currentRateSheetDetails.courier_tariff_type = "NLPARUPS:NLPARFED:USPARDHL2:USPARTNT:USPARUPS:USPARFED2:NLPARTNT2:NLPARDPD:USPARUSP";

            // currentRateSheetDetails.date_pickup = "10-Mar-2016 00:00";//preferredCollectionDate
            // currentRateSheetDetails.time_pickup = "12:51";
            // currentRateSheetDetails.date_delivery_request = "25-Mar-2016 00:00";
            //    currentRateSheetDetails.delivery_condition = "DD-DDU-PP";         
            //  currentRateSheetDetails.insurance_instruction = "N";
            currentRateSheetDetails.sort = "PRICE";
            // currentRateSheetDetails.inbound = "N"; 
            // currentRateSheetDetails.dg = "NO";
            // currentRateSheetDetails.dg_type = "";
            currentRateSheetDetails.account = "";
            currentRateSheetDetails.code_customer = "";
            currentRateSheetDetails.ind_delivery_inside = "";
            currentRateSheetDetails.url = " www2.shipitsmarter.com/taleus/";    // As per the instruct, this url is not using in SIS side.


            return sisManager.GetRateSheetForShipment(currentRateSheetDetails);

        }

        //get the status of inbound outbound rule
        public string GetInboundoutBoundStatus(string userId, string fromCode, string toCode)
        {
            string status = "N";

            //using (PIContext context = PIContext.Get())
            //{
            var countryCode = string.Empty;
            var addressId = context.Customers.Where(c => c.UserId == userId).Select(c => c.AddressId).SingleOrDefault();
            if (addressId != 0)
            {
                countryCode = context.Addresses.Where(c => c.Id == addressId).Select(c => c.Country).SingleOrDefault();
            }
            if (countryCode != null && countryCode.Equals(toCode) && !countryCode.Equals(fromCode))
            {
                status = "Y";
            }
            else
            {
                status = "N";
            }

            // }

            return status;
        }

        public ShipmentOperationResult SaveShipment(ShipmentDto addShipment)
        {
            ShipmentOperationResult result = new ShipmentOperationResult();
            OperationResult paymentResult = new OperationResult();

            if (addShipment.GeneralInformation.ShipmentPaymentTypeId == 2)
            {
                // Online payment
                paymentResult = paymentManager.Charge(addShipment.PaymentDto);
            }

            Company currentcompany = context.GetCompanyByUserId(addShipment.UserId);
            long sysDivisionId = 0;
            long sysCostCenterId = 0;

            var packageProductList = new List<PackageProduct>();
            addShipment.PackageDetails.ProductIngredients.ForEach(p => packageProductList.Add(new PackageProduct()
            {
                CreatedBy = addShipment.CreatedBy,
                CreatedDate = DateTime.UtcNow,
                IsActive = true,
                IsDelete = false,
                Description = p.Description,
                Height = p.Height,
                Length = p.Length,
                Weight = p.Weight,
                Width = p.Width,
                Quantity = p.Quantity,
                ProductTypeId = (short)Enum.Parse(typeof(ProductType), p.ProductType)
            }));


            // If division and costcenter Ids are 0, then assign default costcenter and division.
            if (addShipment.GeneralInformation.DivisionId == 0)
            {
                var sysDivision = context.Divisions.Where(d => d.CompanyId == currentcompany.Id
                                                       && d.Type == "SYSTEM").SingleOrDefault();

                sysDivisionId = sysDivision.Id;

            }
            if (addShipment.GeneralInformation.CostCenterId == 0)
            {
                var defaultCostCntr = context.CostCenters.Where(c => c.CompanyId == currentcompany.Id
                                                                            && c.Type == "SYSTEM").SingleOrDefault();
                sysCostCenterId = defaultCostCntr.Id;
            }

            long oldShipmentId = 0;//= Int64.Parse(addShipment.GeneralInformation.ShipmentCode);

            if (addShipment.GeneralInformation.ShipmentCode != "0")
            {
                // If has parent shipment id, then add to previous shipment.
                Data.Entity.Shipment oldShipment = context.Shipments.Where(sh => sh.ShipmentCode == addShipment.GeneralInformation.ShipmentCode).FirstOrDefault();
                oldShipmentId = oldShipment.Id;
                oldShipment.IsParent = true;
                context.SaveChanges();
            }

            //Mapper.CreateMap<GeneralInformationDto, Shipment>();
            Data.Entity.Shipment newShipment = new Data.Entity.Shipment
            {
                ShipmentName = addShipment.GeneralInformation.ShipmentName,
                ShipmentReferenceName = addShipment.GeneralInformation.ShipmentName + "-" + DateTime.UtcNow.ToString("yyyyMMddHHmmssfff"),
                ShipmentCode = null, //addShipmentResponse.CodeShipment,
                DivisionId = addShipment.GeneralInformation.DivisionId == 0 ? sysDivisionId : (long?)addShipment.GeneralInformation.DivisionId,
                CostCenterId = addShipment.GeneralInformation.CostCenterId == 0 ? sysCostCenterId : (long?)addShipment.GeneralInformation.CostCenterId,
                ShipmentMode = (Contract.Enums.CarrierType)Enum.Parse(typeof(Contract.Enums.CarrierType), addShipment.GeneralInformation.ShipmentMode, true),
                ShipmentService = (short)Utility.GetValueFromDescription<ShipmentService>(addShipment.GeneralInformation.ShipmentServices),
                Carrier = context.Carrier.Where(c => c.Name == addShipment.CarrierInformation.CarrierName).FirstOrDefault(),
                TrackingNumber = null, //addShipmentResponse.Awb,
                CreatedBy = addShipment.CreatedBy,
                CreatedDate = DateTime.UtcNow,
                ServiceLevel = addShipment.CarrierInformation.serviceLevel,
                TarriffType = addShipment.CarrierInformation.tarriffType,
                TariffText = addShipment.CarrierInformation.tariffText,
                CarrierDescription = addShipment.CarrierInformation.description,
                ShipmentPaymentTypeId = addShipment.GeneralInformation.ShipmentPaymentTypeId,
                Status = (short)ShipmentStatus.Draft,
                PickUpDate = addShipment.CarrierInformation.PickupDate == null ? null : (DateTime?)addShipment.CarrierInformation.PickupDate.Value.ToUniversalTime(),

                IsActive = true,
                IsParent = false,
                ParentShipmentId = oldShipmentId == 0 ? null : (long?)oldShipmentId,
                ConsigneeAddress = new ShipmentAddress
                {
                    CompanyName = addShipment.AddressInformation.Consignee.CompanyName,
                    FirstName = addShipment.AddressInformation.Consignee.FirstName,
                    LastName = addShipment.AddressInformation.Consignee.LastName,
                    Country = addShipment.AddressInformation.Consignee.Country,
                    ZipCode = addShipment.AddressInformation.Consignee.Postalcode,
                    Number = addShipment.AddressInformation.Consignee.Number,
                    StreetAddress1 = addShipment.AddressInformation.Consignee.Address1,
                    StreetAddress2 = addShipment.AddressInformation.Consignee.Address2,
                    City = addShipment.AddressInformation.Consignee.City,
                    State = addShipment.AddressInformation.Consignee.State,
                    EmailAddress = addShipment.AddressInformation.Consignee.Email,
                    PhoneNumber = addShipment.AddressInformation.Consignee.ContactNumber,
                    ContactName = addShipment.AddressInformation.Consignee.FirstName + " " + addShipment.AddressInformation.Consignee.LastName,
                    IsActive = true,
                    CreatedBy = addShipment.CreatedBy,
                    CreatedDate = DateTime.UtcNow
                },
                ConsignorAddress = new ShipmentAddress
                {
                    CompanyName = addShipment.AddressInformation.Consigner.CompanyName,
                    FirstName = addShipment.AddressInformation.Consigner.FirstName,
                    LastName = addShipment.AddressInformation.Consigner.LastName,
                    Country = addShipment.AddressInformation.Consigner.Country,
                    ZipCode = addShipment.AddressInformation.Consigner.Postalcode,
                    Number = addShipment.AddressInformation.Consigner.Number,
                    StreetAddress1 = addShipment.AddressInformation.Consigner.Address1,
                    StreetAddress2 = addShipment.AddressInformation.Consigner.Address2,
                    City = addShipment.AddressInformation.Consigner.City,
                    State = addShipment.AddressInformation.Consigner.State,
                    EmailAddress = addShipment.AddressInformation.Consigner.Email,
                    PhoneNumber = addShipment.AddressInformation.Consigner.ContactNumber,
                    ContactName = addShipment.AddressInformation.Consigner.FirstName + " " + addShipment.AddressInformation.Consigner.LastName,
                    IsActive = true,
                    CreatedBy = addShipment.CreatedBy,
                    CreatedDate = DateTime.UtcNow
                },
                ShipmentPackage = new ShipmentPackage()
                {
                    PackageDescription = addShipment.PackageDetails.ShipmentDescription,
                    TotalVolume = addShipment.PackageDetails.TotalVolume,
                    TotalWeight = addShipment.PackageDetails.TotalWeight,
                    HSCode = addShipment.PackageDetails.HsCode,
                    CollectionDate = DateTime.Parse(addShipment.PackageDetails.PreferredCollectionDate),
                    CarrierInstruction = addShipment.PackageDetails.Instructions,
                    IsInsured = Convert.ToBoolean(addShipment.PackageDetails.IsInsuared),
                    InsuranceDeclaredValue = addShipment.PackageDetails.DeclaredValue,
                    InsuranceCurrencyType = (short)addShipment.PackageDetails.ValueCurrency,
                    CarrierCost = addShipment.CarrierInformation.Price,
                    InsuranceCost = addShipment.CarrierInformation.Insurance,
                    PaymentTypeId = addShipment.PackageDetails.PaymentTypeId,
                    EarliestPickupDate = addShipment.CarrierInformation.PickupDate == null ? null : (DateTime?)addShipment.CarrierInformation.PickupDate.Value.ToUniversalTime(),
                    EstDeliveryDate = addShipment.CarrierInformation.DeliveryTime ?? null,
                    WeightMetricId = addShipment.PackageDetails.CmLBS ? (short)1 : (short)2,
                    VolumeMetricId = addShipment.PackageDetails.VolumeCMM ? (short)1 : (short)2,
                    IsActive = true,
                    CreatedBy = addShipment.CreatedBy,
                    CreatedDate = DateTime.UtcNow,
                    PackageProducts = packageProductList,
                    IsDG = addShipment.PackageDetails.IsDG,
                    Accessibility = addShipment.PackageDetails.IsDG == true ? addShipment.PackageDetails.Accessibility : false,
                    DGType = addShipment.PackageDetails.IsDG == true ? addShipment.PackageDetails.DGType : null,

                }
            };

            //save consigner details as new address book detail
            if (addShipment.AddressInformation.Consigner.SaveNewAddress)
            {
                AddressBook ConsignerAddressBook = new AddressBook
                {
                    CompanyName = addShipment.AddressInformation.Consigner.CompanyName,
                    FirstName = addShipment.AddressInformation.Consigner.FirstName,
                    LastName = addShipment.AddressInformation.Consigner.LastName,
                    Country = addShipment.AddressInformation.Consigner.Country,
                    ZipCode = addShipment.AddressInformation.Consigner.Postalcode,
                    Number = addShipment.AddressInformation.Consigner.Number,
                    StreetAddress1 = addShipment.AddressInformation.Consigner.Address1,
                    StreetAddress2 = addShipment.AddressInformation.Consigner.Address2,
                    City = addShipment.AddressInformation.Consigner.City,
                    State = addShipment.AddressInformation.Consigner.State,
                    EmailAddress = addShipment.AddressInformation.Consigner.Email,
                    PhoneNumber = addShipment.AddressInformation.Consigner.ContactNumber,
                    IsActive = true,
                    CreatedBy = addShipment.CreatedBy,
                    UserId = addShipment.UserId,
                    CreatedDate = DateTime.UtcNow
                };
                context.AddressBooks.Add(ConsignerAddressBook);

            }

            //save consignee details as new address book detail
            if (addShipment.AddressInformation.Consignee.SaveNewAddress)
            {
                AddressBook ConsignerAddressBook = new AddressBook
                {
                    CompanyName = addShipment.AddressInformation.Consignee.CompanyName,
                    FirstName = addShipment.AddressInformation.Consignee.FirstName,
                    LastName = addShipment.AddressInformation.Consignee.LastName,
                    Country = addShipment.AddressInformation.Consignee.Country,
                    ZipCode = addShipment.AddressInformation.Consignee.Postalcode,
                    Number = addShipment.AddressInformation.Consignee.Number,
                    StreetAddress1 = addShipment.AddressInformation.Consignee.Address1,
                    StreetAddress2 = addShipment.AddressInformation.Consignee.Address2,
                    City = addShipment.AddressInformation.Consignee.City,
                    State = addShipment.AddressInformation.Consignee.State,
                    EmailAddress = addShipment.AddressInformation.Consignee.Email,
                    PhoneNumber = addShipment.AddressInformation.Consignee.ContactNumber,
                    IsActive = true,
                    CreatedBy = addShipment.CreatedBy,
                    UserId = addShipment.UserId,
                    CreatedDate = DateTime.UtcNow
                };
                context.AddressBooks.Add(ConsignerAddressBook);

            }

            try
            {
                context.Shipments.Add(newShipment);
                context.SaveChanges();

                // Save payment information
                if (addShipment.GeneralInformation.ShipmentPaymentTypeId == 2)
                {
                    // Added payment data
                    var paymentEntity = new Payment();
                    paymentEntity.CreatedBy = addShipment.UserId;
                    paymentEntity.CreatedDate = DateTime.UtcNow;
                    paymentEntity.IsActive = true;
                    paymentEntity.PaymentId = paymentResult.FieldList["PaymentKey"];
                    paymentEntity.Status = paymentResult.Status;
                    paymentEntity.PaymentType = PaymentType.Shipment;
                    paymentEntity.ReferenceId = newShipment.Id;
                    paymentEntity.Amount = addShipment.PaymentDto.ChargeAmount;
                    paymentEntity.LocationId = paymentResult.FieldList["LocationId"];
                    paymentEntity.TransactionId = paymentResult.FieldList["TransactionId"];
                    paymentEntity.TenderId = paymentResult.FieldList["TenderId"];

                    if (addShipment.PaymentDto.CurrencyType == "USD")
                    {
                        paymentEntity.CurrencyType = CurrencyType.USD;
                    }

                    if (paymentResult.Status == Status.PaymentError)
                    {
                        // If failed, due to payment gateway error, then record payment error code.
                        paymentEntity.StatusCode = result.FieldList["errorCode"];
                    }

                    context.Payments.Add(paymentEntity);
                    context.SaveChanges();
                }

                result.ShipmentId = newShipment.Id;

                if (addShipment.GeneralInformation.ShipmentPaymentTypeId == 2 && (paymentResult.Status == Status.PaymentError))
                {
                    result.Status = Status.PaymentError;
                }
                else
                {
                    result.Status = Status.Success;
                }

            }
            catch (Exception ex)
            {
                //throw ex;
                result.ShipmentId = 0;
                result.Status = Status.Error;
            }

            //Add Audit Trail Record
            context.AuditTrail.Add(new AuditTrail
            {
                ReferenceId = newShipment.Id.ToString(),
                AppFunctionality = (addShipment.GeneralInformation.ShipmentCode != "0") ?
                                    AppFunctionality.EditShipment : AppFunctionality.AddShipment,
                Result = result.Status.ToString(),
                CreatedBy = "1",
                CreatedDate = DateTime.UtcNow
            });
            context.SaveChanges();

            if (!addShipment.isSaveAsDraft && (result.Status == Status.Success))
            {
                //// set shipment id, bcoz required in sendshipmentdetails method.
                //addShipment.GeneralInformation.ShipmentId = newShipment.Id.ToString();

                //// We required custom shipmentdto, so need to get it back. Later need to change this.
                //ShipmentDto shDto = GetShipmentDtoForSIS(newShipment.Id);

                //var response = sisManager.SendShipmentDetails(shDto);

                //newShipment.Status = (short)ShipmentStatus.Processing;
                //context.SaveChanges();

                SendShipmentDetails(new SendShipmentDetailsDto()
                {
                    ShipmentId = newShipment.Id
                });
            }

            return result;
        }


        public ShipmentOperationResult UpdateShipmentReference(ShipmentDto addShipment)
        {

            ShipmentOperationResult result = new ShipmentOperationResult();

            var shipment = context.Shipments.Where(s => s.Id.ToString() == addShipment.GeneralInformation.ShipmentId).SingleOrDefault();


            if (shipment != null)
            {
                try
                {
                    shipment.ShipmentName = addShipment.GeneralInformation.ShipmentName;
                    context.SaveChanges();

                    result.Status = Status.Success;
                }
                catch (Exception ex)
                {
                    //throw ex;
                    result.ShipmentId = 0;
                    result.Status = Status.Error;
                }

            }




            //}

            return result;
        }

        public string GetSquareApplicationId()
        {
            return ConfigurationManager.AppSettings["SquareApplicationId"].ToString();
        }



        public long GetTenantIdByUserId(string user)
        {
            return ContextExtension.GetTenantIdByUserId(context, user);
        }


        private static string Hash(string input)
        {
            using (SHA1Managed sha1 = new SHA1Managed())
            {
                var hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(input));
                var sb = new StringBuilder(hash.Length * 2);

                foreach (byte b in hash)
                {
                    // can be "x2" if you want lowercase
                    sb.Append(b.ToString("x2"));
                }

                return sb.ToString();
            }
        }


        // get all shipment which are not delivered
        public IList<ShipmentDto> GetAllShipmentsForAdmins()
        {
            IList<ShipmentDto> shipmentList = new List<ShipmentDto>();

            var content = (from shipment in context.Shipments
                           where shipment.Status != (short)ShipmentStatus.Delivered
                           && shipment.TrackingNumber != null
                           select shipment).ToList();

            foreach (var item in content)
            {
                shipmentList.Add(new ShipmentDto()
                {
                    GeneralInformation = new GeneralInformationDto
                    {
                        TrackingNumber = item.TrackingNumber,
                        ShipmentCode = item.ShipmentCode,
                        CreatedBy = item.CreatedBy

                    },
                    CarrierInformation = new CarrierInformationDto
                    {
                        CarrierName = item.Carrier.Name
                    },

                });
            }

            return shipmentList;
        }


        //get shipments by User
        public PagedList GetAllShipmentsbyUser(PagedList shipmentSerach)
        {
            IList<DivisionDto> divisions = null;
            IList<int> divisionList = new List<int>();
            List<Data.Entity.Shipment> Shipments = new List<Data.Entity.Shipment>();
            var pagedRecord = new PagedList();

            if (shipmentSerach.UserId == null)
            {
                return null;
            }
            string role = context.GetUserRoleById(shipmentSerach.UserId);
            if (role == "BusinessOwner" || role == "Manager")
            {
                divisions = this.GetAllDivisionsinCompany(shipmentSerach.UserId);
            }
            //else if (role == "Supervisor")
            //{
            //    divisions = companyManagment.GetAssignedDivisions(shipmentSerach.UserId);
            //}
            if (divisions != null && divisions.Count > 0)
            {
                foreach (var item in divisions)
                {
                    Shipments.AddRange(this.GetshipmentsByDivisionId(item.Id));
                }
            }
            else
            {
                Shipments.AddRange(this.GetshipmentsByUserId(shipmentSerach.UserId));
            }

            DateTime? startDate = null, endDate = null;
            if (!string.IsNullOrWhiteSpace(shipmentSerach.DynamicContent.startDate.ToString()))
            {
                // Convert to utc
                startDate = Convert.ToDateTime(shipmentSerach.DynamicContent.startDate.ToString());
                startDate = startDate.Value.ToUniversalTime();

                endDate = Convert.ToDateTime(shipmentSerach.DynamicContent.endDate.ToString());
                endDate = endDate.Value.ToUniversalTime();
            }

            pagedRecord.Content = new List<ShipmentDto>();

            // Get new updated shipment list again.
            var querableShipmentList = (from shipment in Shipments
                                        join package in context.ShipmentPackages on shipment.ShipmentPackageId equals package.Id
                                        where shipment.IsDelete == false &&
                                        ((bool)shipmentSerach.DynamicContent.viaDashboard ? shipment.Status != (short)ShipmentStatus.Delivered &&
                                         shipment.Status != (short)ShipmentStatus.Deleted
                                         && shipment.IsFavourite :
                                         ((string.IsNullOrWhiteSpace(shipmentSerach.DynamicContent.status.ToString()) ||
                                             (shipmentSerach.DynamicContent.status.ToString() == "Error" ? (shipment.Status == (short)ShipmentStatus.Error || shipment.Status == (short)ShipmentStatus.Pending)
                                                         : shipmentSerach.DynamicContent.status.ToString() == "Exception" ? (shipment.Status == (short)ShipmentStatus.Exception || shipment.Status == (short)ShipmentStatus.Claim)
                                                         : shipment.Status == (short)Enum.Parse(typeof(ShipmentStatus), shipmentSerach.DynamicContent.status.ToString()))
                                                        ) &&
                                            (string.IsNullOrWhiteSpace(shipmentSerach.DynamicContent.startDate.ToString()) || (shipment.ShipmentPackage.EarliestPickupDate >= startDate && shipment.ShipmentPackage.EarliestPickupDate <= endDate)) &&
                                            (string.IsNullOrWhiteSpace(shipmentSerach.DynamicContent.number.ToString()) || (!string.IsNullOrEmpty(shipment.TrackingNumber) && shipment.TrackingNumber.Contains(shipmentSerach.DynamicContent.number.ToString())) || (!string.IsNullOrEmpty(shipment.ShipmentCode) && shipment.ShipmentCode.Contains(shipmentSerach.DynamicContent.number.ToString()))) &&
                                            (string.IsNullOrWhiteSpace(shipmentSerach.DynamicContent.source.ToString()) || shipment.ConsignorAddress.Country.Contains(shipmentSerach.DynamicContent.source.ToString()) || shipment.ConsignorAddress.City.Contains(shipmentSerach.DynamicContent.source.ToString())) &&
                                            (string.IsNullOrWhiteSpace(shipmentSerach.DynamicContent.destination.ToString()) || shipment.ConsigneeAddress.Country.Contains(shipmentSerach.DynamicContent.destination.ToString()) || shipment.ConsigneeAddress.City.Contains(shipmentSerach.DynamicContent.destination.ToString()))
                                          )
                                        ) &&
                                        !shipment.IsParent
                                        select shipment);

            var shipmentList = querableShipmentList.OrderByDescending(d => d.CreatedDate).Skip(shipmentSerach.CurrentPage).Take(shipmentSerach.PageSize).ToList();

            foreach (var item in shipmentList)
            {
                item.Status = (item.Status == (short)ShipmentStatus.Pending) ? (short)ShipmentStatus.Error : item.Status;

                pagedRecord.Content.Add(new ShipmentDto
                {
                    AddressInformation = new ConsignerAndConsigneeInformationDto
                    {
                        Consignee = new ConsigneeDto
                        {
                            Address1 = item.ConsigneeAddress.StreetAddress1,
                            Address2 = item.ConsigneeAddress.StreetAddress2,
                            Postalcode = item.ConsigneeAddress.ZipCode,
                            City = item.ConsigneeAddress.City,
                            Country = item.ConsigneeAddress.Country,
                            State = item.ConsigneeAddress.State,
                            FirstName = item.ConsigneeAddress.FirstName,
                            LastName = item.ConsigneeAddress.LastName,
                            ContactName = item.ConsigneeAddress.ContactName,
                            ContactNumber = item.ConsigneeAddress.PhoneNumber,
                            Email = item.ConsigneeAddress.EmailAddress,
                            Number = item.ConsigneeAddress.Number
                        },
                        Consigner = new ConsignerDto
                        {
                            Address1 = item.ConsignorAddress.StreetAddress1,
                            Address2 = item.ConsignorAddress.StreetAddress2,
                            Postalcode = item.ConsignorAddress.ZipCode,
                            City = item.ConsignorAddress.City,
                            Country = item.ConsignorAddress.Country,
                            State = item.ConsignorAddress.State,
                            FirstName = item.ConsignorAddress.FirstName,
                            LastName = item.ConsignorAddress.LastName,
                            ContactName = item.ConsignorAddress.ContactName,
                            ContactNumber = item.ConsignorAddress.PhoneNumber,
                            Email = item.ConsignorAddress.EmailAddress,
                            Number = item.ConsignorAddress.Number
                        }
                    },
                    GeneralInformation = new GeneralInformationDto
                    {
                        CostCenterId = item.CostCenterId.GetValueOrDefault(),
                        DivisionId = item.DivisionId.GetValueOrDefault(),
                        ShipmentCode = item.ShipmentCode,
                        ShipmentId = item.Id.ToString(),
                        ShipmentMode = Enum.GetName(typeof(Contract.Enums.CarrierType), item.ShipmentMode),
                        ShipmentName = item.ShipmentName,
                        ShipmentReferenceName = item.ShipmentReferenceName,
                        ShipmentServices = Utility.GetEnumDescription((ShipmentService)item.ShipmentService),
                        TrackingNumber = item.TrackingNumber,
                        CreatedDate = GetLocalTimeByUser(item.CreatedBy, item.CreatedDate).Value.ToString("dd MMM yyyy"),
                        Status = Utility.GetEnumDescription((ShipmentStatus)item.Status),//((ShipmentStatus)item.Status).ToString(),
                        IsFavourite = item.IsFavourite,
                        //IsEnableEdit = ((ShipmentStatus)item.Status == ShipmentStatus.Error || (ShipmentStatus)item.Status == ShipmentStatus.Pending),
                        IsEnableEdit = (ShipmentStatus)item.Status == ShipmentStatus.Draft,
                        //IsEnableDelete = ((ShipmentStatus)item.Status == ShipmentStatus.Error || (ShipmentStatus)item.Status == ShipmentStatus.Pending || (ShipmentStatus)item.Status == ShipmentStatus.BookingConfirmation)
                        IsEnableDelete = (ShipmentStatus)item.Status == ShipmentStatus.Draft,
                        ShipmentLabelBLOBURL = getLabelforShipmentFromBlobStorage(item.Id, item.Division.Company.TenantId)

                    },
                    PackageDetails = new PackageDetailsDto
                    {
                        CmLBS = Convert.ToBoolean(item.ShipmentPackage.VolumeMetricId),
                        VolumeCMM = Convert.ToBoolean(item.ShipmentPackage.VolumeMetricId),
                        Count = item.ShipmentPackage.PackageProducts.Count,
                        DeclaredValue = item.ShipmentPackage.InsuranceDeclaredValue,
                        HsCode = item.ShipmentPackage.HSCode,
                        Instructions = item.ShipmentPackage.CarrierInstruction,
                        IsInsuared = item.ShipmentPackage.IsInsured.ToString(),
                        TotalVolume = item.ShipmentPackage.TotalVolume,
                        TotalWeight = item.ShipmentPackage.TotalWeight,
                        ValueCurrency = item.ShipmentPackage.Currency == null ? 1 : Convert.ToInt32(item.ShipmentPackage.Currency.Id),
                        PreferredCollectionDate = item.ShipmentPackage.CollectionDate.ToString(),
                        ProductIngredients = this.getPackageDetails(item.ShipmentPackage.PackageProducts),
                        ShipmentDescription = item.ShipmentPackage.PackageDescription

                    },
                    CarrierInformation = new CarrierInformationDto
                    {
                        CarrierName = item.Carrier.Name,
                        serviceLevel = item.ServiceLevel,
                        PickupDate = item.PickUpDate.HasValue ? (DateTime?)context.GetLocalTimeByUser(shipmentSerach.UserId, item.PickUpDate.Value) : null
                    }

                });
            }

            pagedRecord.TotalRecords = querableShipmentList.Count();
            pagedRecord.PageSize = shipmentSerach.PageSize;
            pagedRecord.TotalPages = (int)Math.Ceiling((decimal)pagedRecord.TotalRecords / pagedRecord.PageSize);

            return pagedRecord;
        }


        public IList<Data.Entity.Shipment> GetshipmentsByDivisionId(long divid)
        {
            IList<Data.Entity.Shipment> currentShipments = null;
            //using (PIContext context = PIContext.Get())
            //{
            //currentShipments = (from shipment in context.Shipments
            //                    join shipmentAddress1 in context.ShipmentAddresses on shipment.ConsigneeAddress.Id equals shipmentAddress1.Id
            //                    join shipmentAddress2 in context.ShipmentAddresses on shipment.ConsigneeAddress.Id equals shipmentAddress2.Id
            //                    join shipmentPackages in context.ShipmentPackages on shipment.ShipmentPackageId equals shipmentPackages.Id
            //                    where shipment.DivisionId == divid
            //                    select shipment).ToList();
            currentShipments = context.Shipments.Where(x => x.DivisionId == divid).ToList();

            // }

            return currentShipments;
        }

        //get shipments by user ID
        public IList<Data.Entity.Shipment> GetshipmentsByUserId(string userId)
        {
            IList<Data.Entity.Shipment> currentShipments = null;
            //using (PIContext context = PIContext.Get())
            //{
            //currentShipments = (from shipment in context.Shipments
            //                    join shipmentAddress1 in context.ShipmentAddresses on shipment.ConsigneeAddress.Id equals shipmentAddress1.Id
            //                    join shipmentAddress2 in context.ShipmentAddresses on shipment.ConsigneeAddress.Id equals shipmentAddress2.Id
            //                    join shipmentPackages in context.ShipmentPackages on shipment.ShipmentPackageId equals shipmentPackages.Id
            //                    where shipment.CreatedBy == userId
            //                    select shipment).ToList();

            currentShipments = context.Shipments.Where(x => x.CreatedBy == userId).ToList();

            //  }

            return currentShipments;
        }

        //get shipments by user ID and created date
        private List<Shipment> GetshipmentsByUserIdAndPickupdDate(string userId, DateTime pickupDate, string carreer)
        {
            // Need to convert saved times on shipment entity back to user specific time zone.
            var shipmentIdList = context.Shipments.Where(x =>
                                                         x.CreatedBy == userId &&
                                                         x.Carrier.Name == carreer && !string.IsNullOrEmpty(x.TrackingNumber))
                                                         .Select(s => new
                                                         {
                                                             Id = s.Id,
                                                             PickUpDate = s.PickUpDate
                                                         }).ToList();

            List<Shipment> currentShipments = new List<Shipment>();

            foreach (var shipment in shipmentIdList)
            {
                if (shipment.PickUpDate.HasValue && shipment.PickUpDate.Value.Date == pickupDate.Date)
                {
                    currentShipments.Add(context.Shipments.Where(sh => sh.Id == shipment.Id).First());
                }
            }

            return currentShipments;
        }

        //get shipments by shipment reference
        public List<Data.Entity.Shipment> GetshipmentsByReference(string userId, string reference)
        {
            List<Data.Entity.Shipment> currentShipments = null;
            //using (PIContext context = PIContext.Get())
            //{
            currentShipments = context.Shipments.Where(x => x.CreatedBy == userId && x.ShipmentReferenceName.Contains(reference)
                                                            && !string.IsNullOrEmpty(x.TrackingNumber)).ToList();
            //}
            return currentShipments;
        }


        //update shipment status manually only by admin
        public bool UpdateshipmentStatusManually(ShipmentDto shipmentDetails)
        {

            long id = long.Parse(shipmentDetails.GeneralInformation.ShipmentId);

            var shipment = (from shipmentinfo in context.Shipments
                            where shipmentinfo.Id == id
                            select shipmentinfo).FirstOrDefault();
            if (shipment == null)
            {
                return false;
            }

            try
            {
                shipment.Status = (short)Enum.Parse(typeof(ShipmentStatus), shipmentDetails.GeneralInformation.Status);
                shipment.ManualStatusUpdatedDate = DateTime.UtcNow;
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }


        public void UpdateShipmentStatus(string trackingNo, short status)
        {
            //using (PIContext context = PIContext.Get())
            //{
            var shipment = (from shipmentinfo in context.Shipments
                            where shipmentinfo.TrackingNumber == trackingNo
                            select shipmentinfo).FirstOrDefault();
            if (shipment != null)
            {
                shipment.Status = status;
            }
            context.SaveChanges();
            // }
        }

        public void UpdateShipmentStatusByTrackingNo(string trackingNo, short status)
        {
            var shipment = (from shipmentinfo in context.Shipments
                            where shipmentinfo.TrackingNumber == trackingNo
                            select shipmentinfo).FirstOrDefault();
            if (shipment != null)
            {
                shipment.Status = status;
            }
            context.SaveChanges();
        }

        public Data.Entity.Shipment GetShipmentByShipmentCode(string codeShipment)
        {
            Data.Entity.Shipment currentShipment = new Data.Entity.Shipment();

            //using (PIContext context = PIContext.Get())
            //{
            currentShipment = (from shipment in context.Shipments
                               where shipment.ShipmentCode == codeShipment
                               select shipment).FirstOrDefault();
            // }

            return currentShipment;
        }

        //get shipments by ID
        public ShipmentDto GetshipmentById(string shipmentCode, long shipmentId = 0)
        {
            ShipmentDto currentShipmentDto = null;
            Data.Entity.Shipment currentShipment = null;
            long tenantId = 0;
            string countryCodeFromTarrifText = string.Empty;

            //using (PIContext context = PIContext.Get())
            //{
            if (!string.IsNullOrWhiteSpace(shipmentCode))
                currentShipment = context.Shipments.Where(x => x.ShipmentCode.ToString() == shipmentCode).FirstOrDefault();
            else
                currentShipment = context.Shipments.Where(x => x.Id == shipmentId).FirstOrDefault();

            //currentShipment = (from shipment in context.Shipments.Include("Division.Company")
            //                   join shipmentAddress1 in context.ShipmentAddresses on shipment.ConsigneeAddress.Id equals shipmentAddress1.Id
            //                   join shipmentAddress2 in context.ShipmentAddresses on shipment.ConsigneeAddress.Id equals shipmentAddress2.Id
            //                   join shipmentPackages in context.ShipmentPackages on shipment.ShipmentPackageId equals shipmentPackages.Id
            //                   where shipment.ShipmentCode.ToString() == shipmentId
            //                   select shipment).FirstOrDefault();

            tenantId = currentShipment.Division.Company.TenantId;

            var tarrifTextCode = context.TarrifTextCodes.Where(t => t.TarrifText == currentShipment.TariffText && t.IsActive && !t.IsDelete).FirstOrDefault();

            if (tarrifTextCode != null)
                countryCodeFromTarrifText = tarrifTextCode.CountryCode;
            else
                countryCodeFromTarrifText = "US";
            //}
            if (currentShipment == null)
            {
                return null;
            }

            AccountSettings currentAccountSettings = context.AccountSettings.SingleOrDefault(s => s.Customer.UserId == currentShipment.CreatedBy);

            currentShipmentDto = new ShipmentDto
            {
                AddressInformation = new ConsignerAndConsigneeInformationDto
                {
                    Consignee = new ConsigneeDto
                    {
                        Address1 = currentShipment.ConsigneeAddress.StreetAddress1,
                        Address2 = currentShipment.ConsigneeAddress.StreetAddress2,
                        Postalcode = currentShipment.ConsigneeAddress.ZipCode,
                        City = currentShipment.ConsigneeAddress.City,
                        Country = currentShipment.ConsigneeAddress.Country,
                        State = currentShipment.ConsigneeAddress.State,
                        FirstName = currentShipment.ConsigneeAddress.FirstName,
                        LastName = currentShipment.ConsigneeAddress.LastName,
                        ContactName = currentShipment.ConsigneeAddress.ContactName,
                        ContactNumber = currentShipment.ConsigneeAddress.PhoneNumber,
                        Email = currentShipment.ConsigneeAddress.EmailAddress,
                        Number = currentShipment.ConsigneeAddress.Number
                    },
                    Consigner = new ConsignerDto
                    {
                        Address1 = currentShipment.ConsignorAddress.StreetAddress1,
                        Address2 = currentShipment.ConsignorAddress.StreetAddress2,
                        Postalcode = currentShipment.ConsignorAddress.ZipCode,
                        City = currentShipment.ConsignorAddress.City,
                        Country = currentShipment.ConsignorAddress.Country,
                        State = currentShipment.ConsignorAddress.State,
                        FirstName = currentShipment.ConsignorAddress.FirstName,
                        LastName = currentShipment.ConsignorAddress.LastName,
                        ContactName = currentShipment.ConsignorAddress.ContactName,
                        ContactNumber = currentShipment.ConsignorAddress.PhoneNumber,
                        Email = currentShipment.ConsignorAddress.EmailAddress,
                        Number = currentShipment.ConsignorAddress.Number
                    }
                },
                GeneralInformation = new GeneralInformationDto
                {
                    ShipmentId = currentShipment.Id.ToString(),
                    CostCenterId = currentShipment.CostCenterId.GetValueOrDefault(),
                    DivisionId = currentShipment.DivisionId.GetValueOrDefault(),
                    ShipmentCode = currentShipment.ShipmentCode,
                    ShipmentMode = Enum.GetName(typeof(Contract.Enums.CarrierType), currentShipment.ShipmentMode),
                    ShipmentName = currentShipment.ShipmentName,
                    ShipmentServices = Utility.GetEnumDescription((ShipmentService)currentShipment.ShipmentService),
                    CreatedUser = currentShipment.CreatedBy,
                    //ShipmentTermCode = currentShipment.ShipmentTermCode,
                    //ShipmentTypeCode = currentShipment.ShipmentTypeCode,
                    TrackingNumber = currentShipment.TrackingNumber,
                    CreatedDate = GetLocalTimeByUser(currentShipment.CreatedBy, currentShipment.CreatedDate).Value.ToString("dd MMM yyyy"),
                    Status = currentShipment.Status.ToString(),
                    ShipmentLabelBLOBURL = getLabelforShipmentFromBlobStorage(currentShipment.Id, tenantId)
                },
                PackageDetails = new PackageDetailsDto
                {
                    VolumeMetricId = currentShipment.ShipmentPackage.VolumeMetricId,
                    WeightMetricId = currentShipment.ShipmentPackage.WeightMetricId,
                    Count = currentShipment.ShipmentPackage.PackageProducts.Count,
                    DeclaredValue = currentShipment.ShipmentPackage.InsuranceDeclaredValue,
                    HsCode = currentShipment.ShipmentPackage.HSCode,
                    Instructions = currentShipment.ShipmentPackage.CarrierInstruction,
                    IsInsuared = currentShipment.ShipmentPackage.IsInsured.ToString(),
                    TotalVolume = currentShipment.ShipmentPackage.TotalVolume,
                    TotalWeight = currentShipment.ShipmentPackage.TotalWeight,
                    ValueCurrency = currentShipment.ShipmentPackage.InsuranceCurrencyType,
                    PreferredCollectionDate = currentShipment.ShipmentPackage.CollectionDate.ToString(),
                    EstDeliveryDate = DateTime.Parse(currentShipment.ShipmentPackage.EstDeliveryDate.ToString()).ToShortDateString(),
                    ProductIngredients = this.getPackageDetails(currentShipment.ShipmentPackage.PackageProducts, currentAccountSettings, currentShipment.ShipmentPackage),
                    ShipmentDescription = currentShipment.ShipmentPackage.PackageDescription,
                    CarrierCost = currentShipment.ShipmentPackage.CarrierCost.ToString(),
                    IsDG = currentShipment.ShipmentPackage.IsDG

                },
                CarrierInformation = new CarrierInformationDto
                {
                    CarrierName = currentShipment.Carrier.Name,
                    serviceLevel = currentShipment.ServiceLevel,
                    PickupDate = currentShipment.PickUpDate.HasValue ? (DateTime?)context.GetLocalTimeByUser(currentShipment.CreatedBy, currentShipment.PickUpDate.Value) : null,
                    CountryCodeByTarrifText = countryCodeFromTarrifText
                }
            };

            //var payment = context.Payments.Where(p => p.ReferenceId == currentShipment.Id).FirstOrDefault();
            //if(payment != null)
            //{
            //    currentShipmentDto.PaymentDto = new PaymentDto()
            //    {
            //        Amount = payment.Amount.ToString(),
            //        CurrencyType = payment.CurrencyType.ToString(),
            //        LocationId = payment.LocationId,
            //        TransactionId = payment.TransactionId,
            //        TenderId = payment.TenderId
            //    };
            //}

            return currentShipmentDto;
        }


        //get the product ingrediants List
        public List<ProductIngredientsDto> getPackageDetails(IList<PackageProduct> products, AccountSettings accountSettings = null,
                                                             ShipmentPackage shipmentPackage = null)
        {
            List<ProductIngredientsDto> ingrediantList = new List<ProductIngredientsDto>();


            foreach (var ingrediant in products)
            {

                if (shipmentPackage != null)
                {
                    //if (shipmentPackage.VolumeMetricId == accountSettings.VolumeMetricId)
                    //{
                    //    height = ingrediant.Height;
                    //    length = ingrediant.Length;
                    //    width = ingrediant.Width;
                    //}
                    //else
                    //{
                    //    height = accountSettings.DefaultVolumeMetric.Name == "cm" ? (ingrediant.Height * (decimal)2.54) : (ingrediant.Height / (decimal)2.54);
                    //    length = accountSettings.DefaultVolumeMetric.Name == "cm" ? (ingrediant.Length * (decimal)2.54) : (ingrediant.Length / (decimal)2.54);
                    //    width = accountSettings.DefaultVolumeMetric.Name == "cm" ? (ingrediant.Width * (decimal)2.54) : (ingrediant.Width / (decimal)2.54);
                    //}

                    //if (shipmentPackage.WeightMetricId == accountSettings.WeightMetricId)
                    //{
                    //    weight = ingrediant.Weight;
                    //}
                    //else
                    //{
                    //    weight = accountSettings.DefaultWeightMetric.Name == "lbs" ? (ingrediant.Weight * (decimal)2.20462) : (ingrediant.Weight / (decimal)2.20462);
                    //}

                    ingrediantList.Add(
                        new ProductIngredientsDto
                        {
                            Height = Math.Round(ingrediant.Height, 2),
                            Length = Math.Round(ingrediant.Length, 2),
                            ProductType = Utility.GetEnumDescription((ProductType)ingrediant.ProductTypeId),
                            Quantity = ingrediant.Quantity,
                            Weight = Math.Round(ingrediant.Weight, 2),
                            Width = Math.Round(ingrediant.Width, 2),
                            Description = ingrediant.Description
                        });

                }
            }
            return ingrediantList;
        }

        private ShipmentDto GetShipmentDtoForSIS(long shipmentId)
        {
            ShipmentDto shipmentDto;
            AddShipmentResponse response;
            ShipmentOperationResult result = new ShipmentOperationResult();

            Data.Entity.Shipment shipment = context.Shipments.Where(sh => sh.Id == shipmentId).FirstOrDefault();

            var shipmentProductIngredientsList = new List<ProductIngredientsDto>();

            shipment.ShipmentPackage.PackageProducts.ToList().ForEach(p => shipmentProductIngredientsList.Add(new ProductIngredientsDto()
            {
                Description = p.Description,
                Height = p.Height,
                Length = p.Length,
                Weight = p.Weight,
                Width = p.Width,
                Quantity = p.Quantity,
                ProductType = Utility.GetEnumDescription((ProductType)p.ProductTypeId)
            }));

            shipmentDto = new ShipmentDto()
            {
                GeneralInformation = new GeneralInformationDto()
                {
                    ShipmentId = shipment.Id.ToString(),
                    ShipmentName = shipment.ShipmentName,
                    ShipmentReferenceName = shipment.ShipmentReferenceName,
                    ShipmentServices = Utility.GetEnumDescription((ShipmentService)shipment.ShipmentService),
                    shipmentModeName = Utility.GetEnumDescription(shipment.ShipmentMode)//,
                    //UserId = shipment.user
                },
                CarrierInformation = new CarrierInformationDto()
                {
                    CarrierName = shipment.Carrier.Name,
                    serviceLevel = shipment.ServiceLevel,
                    Price = shipment.ShipmentPackage.CarrierCost,
                    Insurance = shipment.ShipmentPackage.InsuranceCost,
                    tarriffType = shipment.TarriffType,
                    tariffText = shipment.TariffText,
                    description = shipment.CarrierDescription

                },
                AddressInformation = new ConsignerAndConsigneeInformationDto()
                {
                    Consignee = new ConsigneeDto()
                    {
                        FirstName = shipment.ConsigneeAddress.FirstName,
                        LastName = shipment.ConsigneeAddress.LastName,
                        Country = shipment.ConsigneeAddress.Country,
                        Postalcode = shipment.ConsigneeAddress.ZipCode,
                        Number = shipment.ConsigneeAddress.Number,
                        Address1 = shipment.ConsigneeAddress.StreetAddress1,
                        Address2 = shipment.ConsigneeAddress.StreetAddress2,
                        City = shipment.ConsigneeAddress.City,
                        State = shipment.ConsigneeAddress.State,
                        Email = shipment.ConsigneeAddress.EmailAddress,
                        ContactNumber = shipment.ConsigneeAddress.PhoneNumber,
                        ContactName = shipment.ConsigneeAddress.ContactName
                    },
                    Consigner = new ConsignerDto()
                    {
                        FirstName = shipment.ConsignorAddress.FirstName,
                        LastName = shipment.ConsignorAddress.LastName,
                        Country = shipment.ConsignorAddress.Country,
                        Postalcode = shipment.ConsignorAddress.ZipCode,
                        Number = shipment.ConsignorAddress.Number,
                        Address1 = shipment.ConsignorAddress.StreetAddress1,
                        Address2 = shipment.ConsignorAddress.StreetAddress2,
                        City = shipment.ConsignorAddress.City,
                        State = shipment.ConsignorAddress.State,
                        Email = shipment.ConsignorAddress.EmailAddress,
                        ContactNumber = shipment.ConsignorAddress.PhoneNumber,
                        ContactName = shipment.ConsignorAddress.ContactName
                    }
                },
                PackageDetails = new PackageDetailsDto()
                {
                    IsInsuared = shipment.ShipmentPackage.IsInsured.ToString().ToLower(),
                    ValueCurrency = shipment.ShipmentPackage.InsuranceCurrencyType,
                    ValueCurrencyString = Utility.GetEnumDescription((CurrencyType)shipment.ShipmentPackage.InsuranceCurrencyType),
                    PreferredCollectionDate = string.Format("{0}-{1}-{2}", shipment.ShipmentPackage.CollectionDate.Day, shipment.ShipmentPackage.CollectionDate.ToString("MMM", CultureInfo.InvariantCulture), shipment.ShipmentPackage.CollectionDate.Year), //"18-Mar-2016"
                    CmLBS = shipment.ShipmentPackage.WeightMetricId == 1,
                    VolumeCMM = shipment.ShipmentPackage.VolumeMetricId == 1,
                    ProductIngredients = shipmentProductIngredientsList,
                    ShipmentDescription = shipment.ShipmentPackage.PackageDescription,
                    DeclaredValue = shipment.ShipmentPackage.InsuranceDeclaredValue
                }
            };

            return shipmentDto;
        }

        public ShipmentOperationResult SendShipmentDetails(SendShipmentDetailsDto sendShipmentDetails)
        {
            // Get data from database and fill dto.
            ShipmentDto shipmentDto;
            AddShipmentResponse response;
            ShipmentOperationResult result = new ShipmentOperationResult();

            Data.Entity.Shipment shipment = context.Shipments.Where(sh => sh.Id == sendShipmentDetails.ShipmentId).FirstOrDefault();

            var shipmentProductIngredientsList = new List<ProductIngredientsDto>();

            shipment.ShipmentPackage.PackageProducts.ToList().ForEach(p => shipmentProductIngredientsList.Add(new ProductIngredientsDto()
            {
                Description = p.Description,
                Height = p.Height,
                Length = p.Length,
                Weight = p.Weight,
                Width = p.Width,
                Quantity = p.Quantity,
                ProductType = Utility.GetEnumDescription((ProductType)p.ProductTypeId)
            }));

            shipmentDto = new ShipmentDto()
            {
                GeneralInformation = new GeneralInformationDto()
                {
                    ShipmentId = shipment.Id.ToString(),
                    ShipmentName = shipment.ShipmentName,
                    ShipmentReferenceName = shipment.ShipmentReferenceName,
                    ShipmentServices = Utility.GetEnumDescription((ShipmentService)shipment.ShipmentService),
                    shipmentModeName = Utility.GetEnumDescription(shipment.ShipmentMode)
                },
                CarrierInformation = new CarrierInformationDto()
                {
                    CarrierName = shipment.Carrier.Name,
                    serviceLevel = shipment.ServiceLevel,
                    Price = shipment.ShipmentPackage.CarrierCost,
                    Insurance = shipment.ShipmentPackage.InsuranceCost,
                    tarriffType = shipment.TarriffType,
                    tariffText = shipment.TariffText,
                    description = shipment.CarrierDescription

                },
                AddressInformation = new ConsignerAndConsigneeInformationDto()
                {
                    Consignee = new ConsigneeDto()
                    {
                        FirstName = shipment.ConsigneeAddress.FirstName,
                        LastName = shipment.ConsigneeAddress.LastName,
                        Country = shipment.ConsigneeAddress.Country,
                        Postalcode = shipment.ConsigneeAddress.ZipCode,
                        Number = shipment.ConsigneeAddress.Number,
                        Address1 = shipment.ConsigneeAddress.StreetAddress1,
                        Address2 = shipment.ConsigneeAddress.StreetAddress2,
                        City = shipment.ConsigneeAddress.City,
                        State = shipment.ConsigneeAddress.State,
                        Email = shipment.ConsigneeAddress.EmailAddress,
                        ContactNumber = shipment.ConsigneeAddress.PhoneNumber,
                        ContactName = shipment.ConsigneeAddress.ContactName
                    },
                    Consigner = new ConsignerDto()
                    {
                        FirstName = shipment.ConsignorAddress.FirstName,
                        LastName = shipment.ConsignorAddress.LastName,
                        Country = shipment.ConsignorAddress.Country,
                        Postalcode = shipment.ConsignorAddress.ZipCode,
                        Number = shipment.ConsignorAddress.Number,
                        Address1 = shipment.ConsignorAddress.StreetAddress1,
                        Address2 = shipment.ConsignorAddress.StreetAddress2,
                        City = shipment.ConsignorAddress.City,
                        State = shipment.ConsignorAddress.State,
                        Email = shipment.ConsignorAddress.EmailAddress,
                        ContactNumber = shipment.ConsignorAddress.PhoneNumber,
                        ContactName = shipment.ConsignorAddress.ContactName
                    }
                },
                PackageDetails = new PackageDetailsDto()
                {
                    IsInsuared = shipment.ShipmentPackage.IsInsured.ToString().ToLower(),
                    ValueCurrency = shipment.ShipmentPackage.InsuranceCurrencyType,
                    ValueCurrencyString = Utility.GetEnumDescription((CurrencyType)shipment.ShipmentPackage.InsuranceCurrencyType),
                    PreferredCollectionDate = string.Format("{0}-{1}-{2}", shipment.ShipmentPackage.CollectionDate.Day, shipment.ShipmentPackage.CollectionDate.ToString("MMM", CultureInfo.InvariantCulture), shipment.ShipmentPackage.CollectionDate.Year), //"18-Mar-2016"
                    CmLBS = shipment.ShipmentPackage.WeightMetricId == 1,
                    VolumeCMM = shipment.ShipmentPackage.VolumeMetricId == 1,
                    ProductIngredients = shipmentProductIngredientsList,
                    ShipmentDescription = shipment.ShipmentPackage.PackageDescription,
                    DeclaredValue = shipment.ShipmentPackage.InsuranceDeclaredValue
                }
            };

            AddShipmentResponsePM responsePM = new AddShipmentResponsePM();
            bool isPostmen = false;

            if (shipment.Carrier.Name == "USP")
            {
                response = stampsMenmanager.SendShipmentDetails(shipmentDto);
            }
            else
            {
                response = sisManager.SendShipmentDetails(shipmentDto);
            }


            shipment.Status = (short)ShipmentStatus.Processing;
            result.Status = Status.Processing;
            shipment.ShipmentCode = response.CodeShipment;
            shipment.TrackingNumber = response.Awb;
            result.AddShipmentXML = response.AddShipmentXML;

            // SIS will return the pacific time zone. So need to convert it to user time zone
            DateTime utcPickupDate = GetUTCTimeFromSISTaleUS(Convert.ToDateTime(response.DatePickup));
            shipmentDto.CarrierInformation.PickupDate = context.GetLocalTimeByUser(shipment.CreatedBy, utcPickupDate);

            shipmentDto.GeneralInformation.ShipmentPaymentTypeId = shipment.ShipmentPaymentTypeId;
            shipmentDto.GeneralInformation.ShipmentPaymentTypeName = Utility.GetEnumDescription((ShipmentPaymentType)shipment.ShipmentPaymentTypeId);

            ShipmentError shipmentError = null;

            if (string.IsNullOrWhiteSpace(response.Awb))
            {
                result.Status = Status.SISError;
                result.Message = "Error occured when adding shipment";
                result.CarrierName = shipmentDto.CarrierInformation.CarrierName;
                result.ShipmentCode = response.CodeShipment;
                result.ShipmentReference = shipment.ShipmentReferenceName;
                shipment.Provider = "Ship It Smarter";
                shipment.Status = (short)ShipmentStatus.Error;
            }

            else
            {
                result.Status = Status.Success;
                result.Message = "Shipment added successfully";
                result.ShipmentDto = shipmentDto;

                // If response.PDF is empty, get from following url.
                if (string.IsNullOrWhiteSpace(response.PDF))
                {
                    // ICarrierIntegrationManager sisManager = new SISIntegrationManager();
                    result.LabelURL = sisManager.GetLabel(shipment.ShipmentCode);
                    // shipment.BlobUrl = result.LabelURL;
                }
                else
                {
                    if (shipment.Carrier.Name == "TNT")
                    {
                        result.LabelURL = sisManager.GetLabel(shipment.ShipmentCode);
                    }
                    else
                    {
                        result.LabelURL = response.PDF;
                    }



                    // shipment.BlobUrl = response.PDF;
                }
                result.ShipmentId = shipment.Id;
                shipment.Status = (short)ShipmentStatus.BookingConfirmation;
                shipment.Provider = "Ship It Smarter";
                //adding the shipment label to azure
                // For now replace userid from created by
                sendShipmentDetails.UserId = shipment.CreatedBy;
                this.AddShipmentLabeltoAzure(result, sendShipmentDetails);

                var tenantId = context.GetTenantIdByUserId(shipment.CreatedBy);
                var Url = getLabelforShipmentFromBlobStorage(shipment.Id, tenantId);
                result.LabelURL = Url;
            }

            if (shipmentError != null)
                context.ShipmentErrors.Add(shipmentError);

            context.SaveChanges();
            return result;

        }

        //check the shipment after communicated with the SIS to get the label
        public ShipmentOperationResult CheckTheShipmentStatusToViewLabel(SendShipmentDetailsDto sendShipmentDetails)
        {
            // Get data from database and fill dto.                      
            ShipmentOperationResult result = new ShipmentOperationResult();

            Data.Entity.Shipment shipment = context.Shipments.Where(sh => sh.Id == sendShipmentDetails.ShipmentId).FirstOrDefault();

            if (shipment.Status == (short)ShipmentStatus.BookingConfirmation)
            {
                result.Status = Status.Success;
            }
            else if (shipment.Status == (short)ShipmentStatus.Error)
            {
                result.Status = Status.Error;
            }
            else if (shipment.Status == (short)ShipmentStatus.Processing)
            {
                result.Status = Status.Processing;
            }

            return result;

        }

        public bool HandleSISRequest(string addShipmentXml, string shipmentReference)
        {
            AddShipmentResponse addShipmentResponse = null;
            long shipmentId = Convert.ToInt16(shipmentReference);
            using (var wb = new WebClient())
            {
                var data = new NameValueCollection();
                data["data_xml"] = addShipmentXml;

                var response = wb.UploadValues(SISWebURLUS + "insert_shipment.asp", "POST", data);
                var responseString = Encoding.Default.GetString(response);

                XDocument doc = XDocument.Parse(responseString);

                System.Xml.Serialization.XmlSerializer mySerializer = new System.Xml.Serialization.XmlSerializer(typeof(AddShipmentResponse));
                addShipmentResponse = (AddShipmentResponse)mySerializer.Deserialize(new StringReader(responseString));
            }
            if (addShipmentResponse != null)
            {

                var shipment = this.SaveCommunicatedShipment(addShipmentResponse, shipmentId);
                if (shipment.Status == (short)ShipmentStatus.Error)
                {
                    if (shipment.ShipmentPaymentTypeId == 2)
                    {
                        RefundCharge(shipmentId);
                    }

                    return false;
                }
                else
                {
                    return true;
                }

            }
            else
            {
                return false;
            }

        }


        private Shipment SaveCommunicatedShipment(AddShipmentResponse response, long shipmentId)
        {

            Data.Entity.Shipment shipment = context.Shipments.Where(sh => sh.Id == shipmentId).FirstOrDefault();
            SendShipmentDetailsDto sendShipmentDetails = new SendShipmentDetailsDto();
            ShipmentOperationResult result = new ShipmentOperationResult();


            sendShipmentDetails.UserId = shipment.CreatedBy;
            shipment.ShipmentCode = response.CodeShipment;
            shipment.TrackingNumber = response.Awb;

            ShipmentError shipmentError = null;

            if (string.IsNullOrWhiteSpace(response.Awb))
            {
                shipment.Provider = "Ship It Smarter";
                shipment.Status = (short)ShipmentStatus.Error;

            }

            else
            {


                // If response.PDF is empty, get from following url.
                if (string.IsNullOrWhiteSpace(response.PDF))
                {
                    result.LabelURL = sisManager.GetLabel(shipment.ShipmentCode);
                    // shipment.BlobUrl = result.LabelURL;
                }
                else
                {
                    if (shipment.Carrier.Name == "TNT")
                    {
                        result.LabelURL = sisManager.GetLabel(shipment.ShipmentCode);
                    }
                    else
                    {
                        result.LabelURL = response.PDF;
                    }

                }
                result.ShipmentId = shipment.Id;
                shipment.Provider = "Ship It Smarter";
                shipment.Status = (short)ShipmentStatus.BookingConfirmation;

                //adding the shipment label to azure
                this.AddShipmentLabeltoAzure(result, sendShipmentDetails);

                var tenantId = context.GetTenantIdByUserId(shipment.CreatedBy);
                var Url = getLabelforShipmentFromBlobStorage(shipment.Id, tenantId);
                result.LabelURL = Url;
            }

            if (shipmentError != null)
                context.ShipmentErrors.Add(shipmentError);

            context.SaveChanges();
            return shipment;
        }


        private bool AddShipmentLabeltoAzure(ShipmentOperationResult operationResult, SendShipmentDetailsDto sendShipmentDetails)
        {
            AzureFileManager media = new AzureFileManager();
            long tenantId = companyManagment.GetTenantIdByUserId(sendShipmentDetails.UserId);
            media.InitializeStorage(tenantId.ToString(), Utility.GetEnumDescription(DocumentType.ShipmentLabel));
            var result = media.UploadFromFileURL(operationResult.LabelURL, operationResult.ShipmentId.ToString() + ".pdf");
            return true;

        }


        public List<DivisionDto> GetAllDivisionsinCompany(string userId)
        {
            List<DivisionDto> divisionList = new List<DivisionDto>();
            Company currentcompany = context.GetCompanyByUserId(userId);

            if (currentcompany == null)
            {
                return null;
            }

            //using (var context = PIContext.Get())//PIContext.Get())
            //{
            var divisions = context.Divisions.Where(c => c.CompanyId == currentcompany.Id &&
                                                        c.IsDelete == false).ToList();

            foreach (var item in divisions)
            {
                divisionList.Add(new DivisionDto
                {
                    Id = item.Id,
                    Name = item.Name
                });
            }
            //}

            return divisionList;
        }

        //Delete shipment
        public int DeleteShipment(string shipmentCode, string trackingNumber, string carrierName, bool isAdmin, long shipmentId)
        {

            // SISIntegrationManager sisManager = new SISIntegrationManager();
            string URL = "http://parcelinternational.pro/status/" + carrierName + "/" + trackingNumber;
            //using (PIContext context = PIContext.Get())
            //{
            var currentShipment = (from shipment in context.Shipments
                                   where shipment.Id == shipmentId
                                   select shipment).SingleOrDefault();

            if (currentShipment.Carrier.Name!="USP")
            {
                 if (isAdmin)
            {
                if (!string.IsNullOrWhiteSpace(shipmentCode))
                    sisManager.DeleteShipment(shipmentCode);

                currentShipment.Status = (short)ShipmentStatus.Deleted;
                context.SaveChanges();

                return 1;
            }
            else
            {
                if (string.IsNullOrWhiteSpace(trackingNumber))
                {
                    // Shipment hasn't tracking no. So no need to get update of status. Delete the shipment.
                    if (!string.IsNullOrWhiteSpace(shipmentCode))
                        sisManager.DeleteShipment(shipmentCode);

                    currentShipment.Status = (short)ShipmentStatus.Deleted;
                    context.SaveChanges();

                    return 1;
                }
                else if (currentShipment.Status != ((short)ShipmentStatus.Delivered))
                {
                    string env = GetEnvironmentByTarrif(currentShipment.TariffText);

                    UpdateLocationHistory(currentShipment.Carrier.Name, currentShipment.TrackingNumber, currentShipment.ShipmentCode, env, currentShipment.Id);

                    var updatedShipment = (from shipment in context.Shipments
                                           where shipment.ShipmentCode == shipmentCode
                                           select shipment).SingleOrDefault();

                    if (updatedShipment.Status != ((short)ShipmentStatus.Delivered))
                    {
                        sisManager.DeleteShipment(shipmentCode);
                        updatedShipment.Status = (short)ShipmentStatus.Deleted;
                        context.SaveChanges();
                        return 1;
                    }
                    else
                    {
                        return 2;
                    }

                }
                else
                {
                    return 2;
                }

            }

            }
            else
            {
                if (isAdmin)
                {
                    if (!string.IsNullOrWhiteSpace(shipmentCode))
                        stampsManager.DeleteShipment(shipmentCode);

                    currentShipment.Status = (short)ShipmentStatus.Deleted;
                    context.SaveChanges();

                    return 1;
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(trackingNumber))
                    {
                        // Shipment hasn't tracking no. So no need to get update of status. Delete the shipment.
                        if (!string.IsNullOrWhiteSpace(shipmentCode))
                            stampsManager.DeleteShipment(shipmentCode);

                        currentShipment.Status = (short)ShipmentStatus.Deleted;
                        context.SaveChanges();

                        return 1;
                    }
                    else if (currentShipment.Status != ((short)ShipmentStatus.Delivered))
                    {
                        string env = GetEnvironmentByTarrif(currentShipment.TariffText);

                        UpdateLocationHistory(currentShipment.Carrier.Name, currentShipment.TrackingNumber, currentShipment.ShipmentCode, env, currentShipment.Id);

                        var updatedShipment = (from shipment in context.Shipments
                                               where shipment.ShipmentCode == shipmentCode
                                               select shipment).SingleOrDefault();

                        if (updatedShipment.Status != ((short)ShipmentStatus.Delivered))
                        {
                            sisManager.DeleteShipment(shipmentCode);
                            updatedShipment.Status = (short)ShipmentStatus.Deleted;
                            context.SaveChanges();
                            return 1;
                        }
                        else
                        {
                            return 2;
                        }

                    }
                    else
                    {
                        return 2;
                    }

                }

            }

           
            //}

        }

        //get the location history list 
        public StatusHistoryResponce GetLocationHistoryInfoForShipment(string carrier, string trackingNumber, string codeShipment, string environment)
        {
            StatusHistoryResponce locationHistory = new StatusHistoryResponce();
            ShipmentDto currentShipmet = this.GetshipmentById(codeShipment);
            info info = new info();

            if (currentShipmet.GeneralInformation.Status == ((short)ShipmentStatus.Delivered).ToString())
            {
                // locationHistory = this.getUpdatedShipmentHistoryFromDB(codeShipment);
                Data.Entity.Shipment currentShipment = GetShipmentByShipmentCode(codeShipment);
                info.status = currentShipment.Status.ToString();

            }
            else
            {
                info = UpdateLocationHistory(carrier, trackingNumber, codeShipment, environment, Convert.ToInt64(currentShipmet.GeneralInformation.ShipmentId));
                locationHistory = this.getUpdatedShipmentHistoryFromDB(codeShipment);
            }
            locationHistory.info = info;
            return locationHistory;

        }


        private info UpdateLocationHistory(string carrier, string trackingNumber, string codeShipment, string environment, long currentShipmetId)
        {
            // SISIntegrationManager sisManager = new SISIntegrationManager();
            info info = new info();
            var currentSisLocationHistory = sisManager.GetUpdatedShipmentStatusehistory(carrier, trackingNumber, codeShipment, environment);

            if (currentSisLocationHistory != null)
            {
                if (!string.IsNullOrWhiteSpace(currentSisLocationHistory.info.status))
                {
                    short status = (short)Enum.Parse(typeof(ShipmentStatus), currentSisLocationHistory.info.status);
                    this.UpdateShipmentStatus(trackingNumber, status);
                }

                //this.UpdateShipmentStatus(codeShipment, (short)ShipmentStatus.Delivered);
                Data.Entity.Shipment currentShipment = GetShipmentByShipmentCode(codeShipment);
                info.status = currentShipment.Status.ToString();
                info.system = currentSisLocationHistory.info.system;

                List<ShipmentLocationHistory> historyList = this.GetShipmentLocationHistoryByShipmentId(currentShipment.Id);

                foreach (var item in historyList)
                {
                    this.DeleteLocationActivityByLocationHistoryId(item.Id);
                }
                this.DeleteShipmentLocationHistoryByShipmentId(currentShipment.Id);

                if (currentSisLocationHistory.history != null)
                {
                    this.UpdateStatusHistories(currentSisLocationHistory, currentShipmetId);
                }

            }

            return info;
        }


        //get track and trace information
        //get track and trace information
        public StatusHistoryResponce GetTrackAndTraceInfo(string carrier, string trackingNumber)
        {
            string environment = "";
            //using (PIContext context = PIContext.Get())
            //{
            var shipment = context.Shipments.Where(s => s.TrackingNumber == trackingNumber).FirstOrDefault();

            if (shipment != null)
                environment = GetEnvironmentByTarrif(shipment.TariffText);
            else
                environment = "taleus";
            //  }

            StatusHistoryResponce trackingInfo = new StatusHistoryResponce();
            Data.Entity.Shipment currentShipment = this.GetShipmentByTrackingNo(trackingNumber);
            // SISIntegrationManager sisManager = new SISIntegrationManager();
            if (currentShipment != null)
            {
                trackingInfo = sisManager.GetUpdatedShipmentStatusehistory(carrier, trackingNumber, currentShipment.ShipmentCode, environment);
            }
            //  trackingInfo = sisManager.GetUpdatedShipmentStatusehistory(carrier, "8925859014", "38649998", environment);
            return trackingInfo;
        }


        //get shipment details by tracking number
        public Data.Entity.Shipment GetShipmentByTrackingNo(string trackingNo)
        {
            var currentShipment = (from shipment in context.Shipments
                                   where shipment.TrackingNumber == trackingNo
                                   select shipment).SingleOrDefault();

            return currentShipment;

        }

        //get shipment details by tracking number
        public ShipmentDto GetShipmentDetailsByTrackingNo(string trackingNo)
        {
            ShipmentDto shipmentdetails;
            Data.Entity.Shipment currentShipment = new Data.Entity.Shipment();

            currentShipment = (from shipment in context.Shipments
                               where shipment.TrackingNumber == trackingNo
                               select shipment).SingleOrDefault();

            long tenantId = currentShipment.Division.Company.TenantId;

            shipmentdetails = new ShipmentDto
            {
                AddressInformation = new ConsignerAndConsigneeInformationDto
                {
                    Consignee = new ConsigneeDto
                    {
                        Address1 = currentShipment.ConsigneeAddress.StreetAddress1,
                        Address2 = currentShipment.ConsigneeAddress.StreetAddress2,
                        Postalcode = currentShipment.ConsigneeAddress.ZipCode,
                        City = currentShipment.ConsigneeAddress.City,
                        Country = currentShipment.ConsigneeAddress.Country,
                        State = currentShipment.ConsigneeAddress.State,
                        FirstName = currentShipment.ConsigneeAddress.FirstName,
                        LastName = currentShipment.ConsigneeAddress.LastName,
                        ContactName = currentShipment.ConsigneeAddress.ContactName,
                        ContactNumber = currentShipment.ConsigneeAddress.PhoneNumber,
                        Email = currentShipment.ConsigneeAddress.EmailAddress,
                        Number = currentShipment.ConsigneeAddress.Number
                    },
                    Consigner = new ConsignerDto
                    {
                        Address1 = currentShipment.ConsignorAddress.StreetAddress1,
                        Address2 = currentShipment.ConsignorAddress.StreetAddress2,
                        Postalcode = currentShipment.ConsignorAddress.ZipCode,
                        City = currentShipment.ConsignorAddress.City,
                        Country = currentShipment.ConsignorAddress.Country,
                        State = currentShipment.ConsignorAddress.State,
                        FirstName = currentShipment.ConsignorAddress.FirstName,
                        LastName = currentShipment.ConsignorAddress.LastName,
                        ContactName = currentShipment.ConsignorAddress.ContactName,
                        ContactNumber = currentShipment.ConsignorAddress.PhoneNumber,
                        Email = currentShipment.ConsignorAddress.EmailAddress,
                        Number = currentShipment.ConsignorAddress.Number
                    }
                },
                GeneralInformation = new GeneralInformationDto
                {
                    ShipmentId = currentShipment.Id.ToString(),
                    CostCenterId = currentShipment.CostCenterId.GetValueOrDefault(),
                    DivisionId = currentShipment.DivisionId.GetValueOrDefault(),
                    ShipmentCode = currentShipment.ShipmentCode,
                    ShipmentMode = Enum.GetName(typeof(Contract.Enums.CarrierType), currentShipment.ShipmentMode),
                    ShipmentName = currentShipment.ShipmentName,
                    ShipmentServices = Utility.GetEnumDescription((ShipmentService)currentShipment.ShipmentService),
                    //ShipmentTermCode = currentShipment.ShipmentTermCode,
                    //ShipmentTypeCode = currentShipment.ShipmentTypeCode,
                    TrackingNumber = currentShipment.TrackingNumber,
                    CreatedDate = GetLocalTimeByUser(currentShipment.CreatedBy, currentShipment.CreatedDate).Value.ToString("dd MMM yyyy"),
                    Status = currentShipment.Status.ToString(),
                    ShipmentLabelBLOBURL = getLabelforShipmentFromBlobStorage(currentShipment.Id, tenantId),
                    CreatedBy = currentShipment.CreatedBy
                },
                PackageDetails = new PackageDetailsDto
                {
                    CmLBS = Convert.ToBoolean(currentShipment.ShipmentPackage.VolumeMetricId),
                    VolumeCMM = Convert.ToBoolean(currentShipment.ShipmentPackage.VolumeMetricId),
                    Count = currentShipment.ShipmentPackage.PackageProducts.Count,
                    DeclaredValue = currentShipment.ShipmentPackage.InsuranceDeclaredValue,
                    HsCode = currentShipment.ShipmentPackage.HSCode,
                    Instructions = currentShipment.ShipmentPackage.CarrierInstruction,
                    IsInsuared = currentShipment.ShipmentPackage.IsInsured.ToString(),
                    TotalVolume = currentShipment.ShipmentPackage.TotalVolume,
                    TotalWeight = currentShipment.ShipmentPackage.TotalWeight,
                    ValueCurrency = currentShipment.ShipmentPackage.InsuranceCurrencyType,
                    PreferredCollectionDate = currentShipment.ShipmentPackage.CollectionDate.ToString(),
                    ProductIngredients = this.getPackageDetails(currentShipment.ShipmentPackage.PackageProducts),
                    ShipmentDescription = currentShipment.ShipmentPackage.PackageDescription

                },
                CarrierInformation = new CarrierInformationDto
                {
                    CarrierName = currentShipment.Carrier.Name,
                    serviceLevel = currentShipment.ServiceLevel,
                    PickupDate = context.GetLocalTimeByUser(currentShipment.CreatedBy, Convert.ToDateTime(currentShipment.PickUpDate).ToUniversalTime()),
                    //CountryCodeByTarrifText = countryCodeFromTarrifText
                }

            };




            return shipmentdetails;


        }

        //update status hisory with latest statuses and locations
        public void UpdateStatusHistories(StatusHistoryResponce statusHistory, long ShipmntId)
        {

            //using (PIContext context = PIContext.Get())
            //{
            foreach (var item in statusHistory.history.Items)
            {
                ShipmentLocationHistory locationHistory = new ShipmentLocationHistory();
                if (item.location != null)
                {
                    locationHistory.City = string.IsNullOrEmpty(item.location.city) ? string.Empty : item.location.city;
                    locationHistory.Country = string.IsNullOrEmpty(item.location.country) ? string.Empty : item.location.country;
                    if (item.location.geo != null)
                    {
                        locationHistory.Longitude = Convert.ToDouble(item.location.geo.lng);
                        locationHistory.Latitude = Convert.ToDouble(item.location.geo.lat);
                    }
                }

                TimeSpan time = TimeSpan.Parse(item.activity.Items.FirstOrDefault().timestamp.time);
                locationHistory.DateTime = Convert.ToDateTime(item.activity.Items.FirstOrDefault().timestamp.date).Add(time);
                locationHistory.ShipmentId = ShipmntId;
                locationHistory.CreatedDate = DateTime.UtcNow;
                context.ShipmentLocationHistories.Add(locationHistory);
                context.SaveChanges();
            }
            List<ShipmentLocationHistory> histories = this.GetShipmentLocationHistoryByShipmentId(ShipmntId);
            foreach (var item in histories)
            {
                foreach (var his in statusHistory.history.Items)
                {
                    if ((his.location.geo != null && item.Longitude.ToString() == his.location.geo.lng && item.Latitude.ToString() == his.location.geo.lat) || (!string.IsNullOrEmpty(his.location.city) && item.City.Equals(his.location.city)))
                    {
                        foreach (var activityItems in his.activity.Items)
                        {
                            LocationActivity activity = new LocationActivity();
                            activity.ShipmentLocationHistoryId = item.Id;
                            activity.Status = activityItems.status;
                            activity.Time = Convert.ToDateTime(activityItems.timestamp.time);
                            activity.Date = Convert.ToDateTime(activityItems.timestamp.date);
                            activity.CreatedDate = DateTime.UtcNow;


                            context.LocationActivities.Add(activity);
                            context.SaveChanges();

                        }
                    }
                }
            }

            //  }

        }


        //get updated tracking history history from DB
        public StatusHistoryResponce getUpdatedShipmentHistoryFromDB(string codeShipment)
        {
            StatusHistoryResponce statusHistory = new StatusHistoryResponce();
            ShipmentDto currentShipment = this.GetShipmentByCodeShipment(codeShipment);

            List<ShipmentLocationHistory> historyList = GetShipmentLocationHistoryByShipmentId(currentShipment.Id);
            history historynew = new history();
            List<items> itemList = new List<items>();
            historynew.Items = itemList;


            foreach (var item in historyList)
            {
                items items = new items();
                location location = new location();
                geo geo = new geo();

                location.city = item.City;
                location.country = item.Country;

                geo.lat = item.Latitude.ToString();
                geo.lng = item.Longitude.ToString();
                location.geo = geo;
                items.location = location;

                List<LocationActivity> locationActivities = this.GetLocationActivityByLocationHistoryId(item.Id);
                activity activity = new activity();
                foreach (var activ in locationActivities)
                {

                    timestamp time = new timestamp()
                    {
                        date = activ.Date.ToShortDateString().ToString(),
                        time = activ.Time.TimeOfDay.ToString(),

                    };
                    activity.Items.Add(
                        new item
                        {
                            status = activ.Status,
                            timestamp = time
                        });

                    //adding location activity histories                

                }
                items.activity = activity;
                historynew.Items.Add(items);
                statusHistory.history = historynew;
            }
            return statusHistory;

        }

        public void DeleteLocationActivityByLocationHistoryId(long historyId)
        {
            //using (PIContext context = PIContext.Get())
            //{
            List<LocationActivity> activities = (from activity in context.LocationActivities
                                                 where activity.ShipmentLocationHistoryId == historyId
                                                 select activity).ToList();

            context.LocationActivities.RemoveRange(activities);
            context.SaveChanges();

            //  }
        }

        //get shipmentLocation from database
        public void DeleteShipmentLocationHistoryByShipmentId(long shipmentId)
        {

            //using (PIContext context = PIContext.Get())
            //{
            List<ShipmentLocationHistory> histories = (from history in context.ShipmentLocationHistories
                                                       where history.ShipmentId == shipmentId
                                                       select history).ToList();

            context.ShipmentLocationHistories.RemoveRange(histories);
            context.SaveChanges();
            // }
        }


        public List<LocationActivity> GetLocationActivityByLocationHistoryId(long historyId)
        {
            //using (PIContext context = PIContext.Get())
            //{
            List<LocationActivity> histories = (from activity in context.LocationActivities
                                                where activity.ShipmentLocationHistoryId == historyId
                                                select activity).ToList();

            return histories;
            //}
        }

        //get shipmentLocation from database
        public List<ShipmentLocationHistory> GetShipmentLocationHistoryByShipmentId(long shipmentId)
        {

            //using (PIContext context = PIContext.Get())
            //{
            List<ShipmentLocationHistory> histories = (from history in context.ShipmentLocationHistories
                                                       where history.ShipmentId == shipmentId
                                                       select history).ToList();

            return histories;
            //}
        }

        //get the shipment by code shipment
        public ShipmentDto GetShipmentByCodeShipment(string codeShipment)
        {
            Data.Entity.Shipment shipmentContent = (from shipment in context.Shipments
                                                    where shipment.ShipmentCode == codeShipment
                                                    select shipment).FirstOrDefault();

            ShipmentDto shipmentDto = new ShipmentDto()
            {
                Id = shipmentContent.Id,
                Division = new DivisionDto()
                {
                    Company = new CompanyDto()
                    {
                        TenantId = shipmentContent.Division.Company.TenantId
                    }
                }
            };

            return shipmentDto;
        }


        /// <summary>
        /// Insert shipment record
        /// </summary>
        /// <param name="fileDetails"></param>
        public void InsertShipmentDocument(FileUploadDto fileDetails)
        {
            //using (var context = PIContext.Get())
            //{
            var shipement = context.Shipments.Where(x => x.ShipmentCode == fileDetails.CodeReference).SingleOrDefault();

            context.ShipmentDocument.Add(new ShipmentDocument
            {
                TenantId = fileDetails.TenantId,
                ShipmentId = shipement.Id,
                ClientFileName = fileDetails.ClientFileName,
                DocumentType = (int)fileDetails.DocumentType,
                UploadedFileName = fileDetails.UploadedFileName,
                IsActive = true,
                CreatedDate = DateTime.UtcNow,
                CreatedBy = "1"
            });

            context.SaveChanges();
            // }
        }


        public List<FileUploadDto> GetAvailableFilesForShipmentbyTenant(string shipmentCode, string userId)
        {
            List<FileUploadDto> returnList = new List<FileUploadDto>();
            // Make absolute link
            string baseUrl = ConfigurationManager.AppSettings["PIBlobStorage"];
            var tenantId = context.GetTenantIdByUserId(userId);

            //using (var context = PIContext.Get())
            //{
            var docList = context.ShipmentDocument.Where(x => x.TenantId == tenantId
                                                && x.Shipment.ShipmentCode == shipmentCode).
                                                OrderByDescending(x => x.CreatedDate).ToList();

            docList.ForEach(x => returnList.Add(new FileUploadDto
            {
                Id = x.Id,
                TenantId = x.TenantId,
                ReferenceId = x.ShipmentId,
                ClientFileName = x.ClientFileName,
                UploadedFileName = x.UploadedFileName
            }));

            returnList.ForEach(e =>
                e.FileAbsoluteURL = baseUrl + "TENANT_" + e.TenantId + "/" + Utility.GetEnumDescription(DocumentType.Shipment) + "/" + e.UploadedFileName
            );
            // }
            return returnList;
        }



        //get shipments by User
        public PagedList GetAllPendingShipmentsbyUser(PagedList pageList)
        {
            int page = 1;
            int pageSize = 10;
            IList<DivisionDto> divisions = null;
            IList<int> divisionList = new List<int>();
            List<Data.Entity.Shipment> Shipments = new List<Data.Entity.Shipment>();
            var pagedRecord = new PagedList();
            if (pageList.UserId == null)
            {
                return null;
            }
            string role = context.GetUserRoleById(pageList.UserId);
            if (role == "BusinessOwner" || role == "Manager")
            {
                divisions = this.GetAllDivisionsinCompany(pageList.UserId);
            }
            else if (role == "Supervisor")
            {
                divisions = companyManagment.GetAssignedDivisions(pageList.UserId);
            }
            if (divisions.Count > 0)
            {
                foreach (var item in divisions)
                {
                    Shipments.AddRange(this.GetshipmentsByDivisionId(item.Id));
                }
            }
            else
            {
                Shipments.AddRange(this.GetshipmentsByUserId(pageList.UserId));
            }

            pageList.DynamicContent = pageList.filterContent;

            DateTime? startDate = null, endDate = null;
            if (!string.IsNullOrWhiteSpace(pageList.DynamicContent.startDate.ToString()))
            {
                // Convert to utc
                startDate = Convert.ToDateTime(pageList.DynamicContent.startDate.ToString());
                startDate = startDate.Value.ToUniversalTime();

                endDate = Convert.ToDateTime(pageList.DynamicContent.endDate.ToString());
                endDate = endDate.Value.ToUniversalTime();
            }

            string number = pageList.DynamicContent.shipmentNumber.ToString();

            pagedRecord.Content = new List<ShipmentDto>();

            var contentQuerable = (from shipment in Shipments
                                   where shipment.IsDelete == false &&
                                   shipment.Status == (short)ShipmentStatus.BookingConfirmation &&
                                   (startDate == null || (shipment.ShipmentPackage.EarliestPickupDate >= startDate && shipment.ShipmentPackage.EarliestPickupDate <= endDate)) &&
                                   (string.IsNullOrEmpty(number) || shipment.TrackingNumber.Contains(number) || shipment.ShipmentCode.Contains(number))
                                   select shipment);

            var content = contentQuerable.OrderBy(d => d.CreatedDate).Skip(pageList.CurrentPage).Take(pageList.PageSize).ToList();

            foreach (var item in content)
            {
                pagedRecord.Content.Add(new ShipmentDto
                {
                    AddressInformation = new ConsignerAndConsigneeInformationDto
                    {
                        Consignee = new ConsigneeDto
                        {
                            Address1 = item.ConsigneeAddress.StreetAddress1,
                            Address2 = item.ConsigneeAddress.StreetAddress2,
                            Postalcode = item.ConsigneeAddress.ZipCode,
                            City = item.ConsigneeAddress.City,
                            Country = item.ConsigneeAddress.Country,
                            State = item.ConsigneeAddress.State,
                            FirstName = item.ConsigneeAddress.FirstName,
                            LastName = item.ConsigneeAddress.LastName,
                            ContactName = item.ConsigneeAddress.ContactName,
                            ContactNumber = item.ConsigneeAddress.PhoneNumber,
                            Email = item.ConsigneeAddress.EmailAddress,
                            Number = item.ConsigneeAddress.Number
                        },
                        Consigner = new ConsignerDto
                        {
                            Address1 = item.ConsignorAddress.StreetAddress1,
                            Address2 = item.ConsignorAddress.StreetAddress2,
                            Postalcode = item.ConsignorAddress.ZipCode,
                            City = item.ConsignorAddress.City,
                            Country = item.ConsignorAddress.Country,
                            State = item.ConsignorAddress.State,
                            FirstName = item.ConsignorAddress.FirstName,
                            LastName = item.ConsignorAddress.LastName,
                            ContactName = item.ConsignorAddress.ContactName,
                            ContactNumber = item.ConsignorAddress.PhoneNumber,
                            Email = item.ConsignorAddress.EmailAddress,
                            Number = item.ConsignorAddress.Number
                        }
                    },
                    GeneralInformation = new GeneralInformationDto
                    {
                        CostCenterId = item.CostCenterId.GetValueOrDefault(),
                        DivisionId = item.DivisionId.GetValueOrDefault(),
                        ShipmentCode = item.ShipmentCode,
                        ShipmentMode = Enum.GetName(typeof(Contract.Enums.CarrierType), item.ShipmentMode),
                        ShipmentName = item.ShipmentName,
                        //ShipmentTermCode = item.ShipmentTermCode,
                        //ShipmentTypeCode = item.ShipmentTypeCode,


                        TrackingNumber = item.TrackingNumber,
                        CreatedDate = GetLocalTimeByUser(item.CreatedBy, item.CreatedDate).Value.ToString("dd MMM yyyy"),
                        Status = Utility.GetEnumDescription((ShipmentStatus)item.Status),
                        ShipmentLabelBLOBURL = getLabelforShipmentFromBlobStorage(item.Id, item.Division.Company.TenantId)
                    },
                    PackageDetails = new PackageDetailsDto
                    {
                        CmLBS = Convert.ToBoolean(item.ShipmentPackage.VolumeMetricId),
                        VolumeCMM = Convert.ToBoolean(item.ShipmentPackage.VolumeMetricId),
                        Count = item.ShipmentPackage.PackageProducts.Count,
                        DeclaredValue = item.ShipmentPackage.InsuranceDeclaredValue,
                        HsCode = item.ShipmentPackage.HSCode,
                        Instructions = item.ShipmentPackage.CarrierInstruction,
                        IsInsuared = item.ShipmentPackage.IsInsured.ToString(),
                        TotalVolume = item.ShipmentPackage.TotalVolume,
                        TotalWeight = item.ShipmentPackage.TotalWeight,
                        ValueCurrency = item.ShipmentPackage.Currency == null ? 1 : Convert.ToInt32(item.ShipmentPackage.Currency.Id),
                        PreferredCollectionDate = item.ShipmentPackage.CollectionDate.ToString(),
                        ProductIngredients = this.getPackageDetails(item.ShipmentPackage.PackageProducts),
                        ShipmentDescription = item.ShipmentPackage.PackageDescription

                    },
                    CarrierInformation = new CarrierInformationDto
                    {
                        CarrierName = item.Carrier.Name,
                        serviceLevel = item.ServiceLevel,
                        PickupDate = item.PickUpDate.HasValue ? (DateTime?)context.GetLocalTimeByUser(item.CreatedBy, item.PickUpDate.Value) : null
                    }

                });
            }

            pagedRecord.TotalRecords = contentQuerable.Count();
            pagedRecord.PageSize = pageList.PageSize;
            pagedRecord.TotalPages = (int)Math.Ceiling((decimal)pagedRecord.TotalRecords / pagedRecord.PageSize);

            return pagedRecord;
        }


        public List<ShipmentDto> GetAllshipmentsForManifest(string userId, string date, string carreer, string reference)
        {
            List<Data.Entity.Shipment> shipmentList = new List<Data.Entity.Shipment>();
            if (string.IsNullOrEmpty(reference))
            {
                // Have saved pickup date in user time zone (local time). So no need to convert.
                DateTime datetimeFromString = Convert.ToDateTime(date);
                shipmentList = this.GetshipmentsByUserIdAndPickupdDate(userId, datetimeFromString, carreer);
            }
            else
            {
                shipmentList = this.GetshipmentsByReference(userId, reference);
            }

            List<ShipmentDto> shipments = new List<ShipmentDto>();

            foreach (var item in shipmentList)
            {
                shipments.Add(new ShipmentDto
                {
                    AddressInformation = new ConsignerAndConsigneeInformationDto
                    {
                        Consignee = new ConsigneeDto
                        {
                            Address1 = item.ConsigneeAddress.StreetAddress1,
                            Address2 = item.ConsigneeAddress.StreetAddress2,
                            Postalcode = item.ConsigneeAddress.ZipCode,
                            City = item.ConsigneeAddress.City,
                            Country = item.ConsigneeAddress.Country,
                            State = item.ConsigneeAddress.State,
                            FirstName = item.ConsigneeAddress.FirstName,
                            LastName = item.ConsigneeAddress.LastName,
                            ContactName = item.ConsigneeAddress.ContactName,
                            ContactNumber = item.ConsigneeAddress.PhoneNumber,
                            Email = item.ConsigneeAddress.EmailAddress,
                            Number = item.ConsigneeAddress.Number
                        },
                        Consigner = new ConsignerDto
                        {
                            Address1 = item.ConsignorAddress.StreetAddress1,
                            Address2 = item.ConsignorAddress.StreetAddress2,
                            Postalcode = item.ConsignorAddress.ZipCode,
                            City = item.ConsignorAddress.City,
                            Country = item.ConsignorAddress.Country,
                            State = item.ConsignorAddress.State,
                            FirstName = item.ConsignorAddress.FirstName,
                            LastName = item.ConsignorAddress.LastName,
                            ContactName = item.ConsignorAddress.ContactName,
                            ContactNumber = item.ConsignorAddress.PhoneNumber,
                            Email = item.ConsignorAddress.EmailAddress,
                            Number = item.ConsignorAddress.Number
                        }
                    },
                    GeneralInformation = new GeneralInformationDto
                    {
                        CostCenterId = item.CostCenterId.GetValueOrDefault(),
                        DivisionId = item.DivisionId.GetValueOrDefault(),
                        ShipmentCode = item.ShipmentCode,
                        ShipmentMode = Enum.GetName(typeof(Contract.Enums.CarrierType), item.ShipmentMode),
                        ShipmentName = item.ShipmentName,
                        ShipmentReferenceName = this.sep(item.ShipmentReferenceName),
                        //ShipmentTermCode = item.ShipmentTermCode,
                        //ShipmentTypeCode = item.ShipmentTypeCode,
                        TrackingNumber = item.TrackingNumber,
                        CreatedDate = GetLocalTimeByUser(item.CreatedBy, item.CreatedDate).Value.ToString("dd MMM yyyy"),
                        Status = Utility.GetEnumDescription((ShipmentStatus)item.Status)
                    },
                    PackageDetails = new PackageDetailsDto
                    {
                        CmLBS = Convert.ToBoolean(item.ShipmentPackage.VolumeMetricId),
                        VolumeCMM = Convert.ToBoolean(item.ShipmentPackage.VolumeMetricId),
                        Count = item.ShipmentPackage.PackageProducts.Count,
                        DeclaredValue = item.ShipmentPackage.InsuranceDeclaredValue,
                        HsCode = item.ShipmentPackage.HSCode,
                        Instructions = item.ShipmentPackage.CarrierInstruction,
                        IsInsuared = item.ShipmentPackage.IsInsured.ToString(),
                        TotalVolume = item.ShipmentPackage.TotalVolume,
                        TotalWeight = item.ShipmentPackage.TotalWeight,
                        ValueCurrency = item.ShipmentPackage.Currency == null ? 1 : Convert.ToInt32(item.ShipmentPackage.Currency.Id),
                        PreferredCollectionDate = item.ShipmentPackage.CollectionDate.ToString(),
                        ProductIngredients = this.getPackageDetails(item.ShipmentPackage.PackageProducts),
                        ShipmentDescription = item.ShipmentPackage.PackageDescription

                    },
                    CarrierInformation = new CarrierInformationDto
                    {
                        CarrierName = item.Carrier.Name,
                        serviceLevel = item.ServiceLevel,
                        PickupDate = item.PickUpDate.HasValue ? (DateTime?)context.GetLocalTimeByUser(item.CreatedBy, item.PickUpDate.Value) : null
                    }

                });
            }

            return shipments;
        }


        private string getLabelforShipmentFromBlobStorage(long shipmentId, long tenantId)
        {
            // Make absolute link
            string baseUrl = ConfigurationManager.AppSettings["PIBlobStorage"];

            string fileAbsoluteURL = baseUrl + "TENANT_" + tenantId + "/" + Utility.GetEnumDescription(DocumentType.ShipmentLabel)
                                                                          + "/" + (shipmentId.ToString() + ".pdf");
            return fileAbsoluteURL;
        }


        //Update shipment status
        //public int ShipmentStatusBulkUpdate(string shipmentCode, string trackingNumber, string carrierName, string userId)
        //{

        //    SISIntegrationManager sisManager = new SISIntegrationManager();
        //    string URL = "http://parcelinternational.pro/status/" + carrierName + "/" + trackingNumber;

        //    using (var context = PIContext.Get())
        //    {
        //        var shipmentList = context.ShipmentStatusHistory.Where(x=> x.NewStatus != "Completed")
        //                           .Select(x => x.Shipment).ToList();

        //        foreach (var shipment in shipmentList)
        //        {
        //            var newStatus = sisManager.GetShipmentStatus(URL, shipmentCode);

        //            context.ShipmentStatusHistory.Add(new ShipmentStatusHistory
        //            {
        //                ShipmentId = shipment.Id,
        //               // NewStatus = ,
        //                CreatedBy = userId,
        //                CreatedDate = DateTime.UtcNow
        //            });
        //            context.SaveChanges();

        //        }

        //    }

        //}

        public CommercialInvoiceDto GetshipmentByShipmentCodeForInvoice(string shipmentCode)
        {

            Data.Entity.Shipment currentShipment = null;
            long tenantId = 0;
            CommercialInvoiceDto invocieDto = null;

            //using (PIContext context = PIContext.Get())
            //{
            currentShipment = context.Shipments.Where(x => x.ShipmentCode.ToString() == shipmentCode).FirstOrDefault();

            tenantId = currentShipment.Division.Company.TenantId;

            if (currentShipment.CommercialInvoice != null)
            {
                // Load invoice from CommercialInvoice
                var invoiceItemLineList = new List<InvoiceItemLineDto>();
                currentShipment.CommercialInvoice.InvoiceItem.InvoiceItemLines.ToList().ForEach(p => invoiceItemLineList.Add(new InvoiceItemLineDto()
                {
                    Description = p.Description,
                    PricePerPiece = p.PricePerPiece,
                    Quantity = p.Quantity,
                    HSCode = p.HSCode,

                }));

                try
                {

                }
                catch (Exception e)
                {
                    var message = e.Message;
                }


                invocieDto = new CommercialInvoiceDto()
                {
                    ShipmentId = currentShipment.Id,
                    ShipmentReferenceName = currentShipment.CommercialInvoice.ShipmentReferenceName,
                    AddressInformation = new ConsignerAndConsigneeInformationDto
                    {
                        Consignee = new ConsigneeDto
                        {
                            Address1 = currentShipment.ConsigneeAddress.StreetAddress1,
                            Address2 = currentShipment.ConsigneeAddress.StreetAddress2,
                            Postalcode = currentShipment.ConsigneeAddress.ZipCode,
                            City = currentShipment.ConsigneeAddress.City,
                            Country = currentShipment.ConsigneeAddress.Country,
                            State = currentShipment.ConsigneeAddress.State,
                            FirstName = currentShipment.ConsigneeAddress.FirstName,
                            LastName = currentShipment.ConsigneeAddress.LastName,
                            ContactName = currentShipment.ConsigneeAddress.ContactName,
                            ContactNumber = currentShipment.ConsigneeAddress.PhoneNumber,
                            Email = currentShipment.ConsigneeAddress.EmailAddress,
                            Number = currentShipment.ConsigneeAddress.Number
                        },
                        Consigner = new ConsignerDto
                        {
                            Address1 = currentShipment.ConsignorAddress.StreetAddress1,
                            Address2 = currentShipment.ConsignorAddress.StreetAddress2,
                            Postalcode = currentShipment.ConsignorAddress.ZipCode,
                            City = currentShipment.ConsignorAddress.City,
                            Country = currentShipment.ConsignorAddress.Country,
                            State = currentShipment.ConsignorAddress.State,
                            FirstName = currentShipment.ConsignorAddress.FirstName,
                            LastName = currentShipment.ConsignorAddress.LastName,
                            ContactName = currentShipment.ConsignorAddress.ContactName,
                            ContactNumber = currentShipment.ConsignorAddress.PhoneNumber,
                            Email = currentShipment.ConsignorAddress.EmailAddress,
                            Number = currentShipment.ConsignorAddress.Number
                        }
                    },
                    PackageDetails = new PackageDetailsDto
                    {
                        CmLBS = Convert.ToBoolean(currentShipment.ShipmentPackage.VolumeMetricId),
                        VolumeCMM = Convert.ToBoolean(currentShipment.ShipmentPackage.VolumeMetricId),
                        Count = currentShipment.ShipmentPackage.PackageProducts.Count,
                        DeclaredValue = currentShipment.ShipmentPackage.InsuranceDeclaredValue,
                        HsCode = currentShipment.ShipmentPackage.HSCode,
                        Instructions = currentShipment.ShipmentPackage.CarrierInstruction,
                        IsInsuared = currentShipment.ShipmentPackage.IsInsured.ToString(),
                        TotalVolume = currentShipment.ShipmentPackage.TotalVolume,
                        TotalWeight = currentShipment.ShipmentPackage.TotalWeight,
                        ValueCurrency = currentShipment.ShipmentPackage.InsuranceCurrencyType,
                        PreferredCollectionDate = currentShipment.ShipmentPackage.CollectionDate.ToString(),
                        ProductIngredients = this.getPackageDetails(currentShipment.ShipmentPackage.PackageProducts),
                        ShipmentDescription = currentShipment.ShipmentPackage.PackageDescription,
                        CarrierCost = currentShipment.ShipmentPackage.CarrierCost.ToString()

                    },
                    CreatedDate = GetLocalTimeByUser(currentShipment.CommercialInvoice.CreatedBy, currentShipment.CommercialInvoice.CreatedDate).Value.ToString("dd-MMM-yyyy"),
                    InvoiceNo = currentShipment.CommercialInvoice.InvoiceNo,
                    ShipTo = currentShipment.CommercialInvoice.ShipTo,
                    VatNo = currentShipment.CommercialInvoice.VatNo,
                    CustomerNo = currentShipment.CommercialInvoice.CustomerNo,
                    InvoiceTo = currentShipment.CommercialInvoice.InvoiceTo,
                    ShipmentServices = Utility.GetEnumDescription((ShipmentService)currentShipment.CommercialInvoice.ShipmentService),
                    TermsOfPayment = currentShipment.CommercialInvoice.TermsOfPayment, //string.IsNullOrWhiteSpace(currentShipment.CommercialInvoice.TermsOfPayment) ? "FREE OF CHARGE" : currentShipment.CommercialInvoice.TermsOfPayment,
                    CountryOfOrigin = currentShipment.CommercialInvoice.CountryOfOrigin,
                    CountryOfDestination = currentShipment.CommercialInvoice.CountryOfDestination,
                    ModeOfTransport = currentShipment.CommercialInvoice.ModeOfTransport,
                    ImportBroker = currentShipment.CommercialInvoice.ImportBroker,
                    Note = currentShipment.CommercialInvoice.Note,
                    ValueCurrency = currentShipment.CommercialInvoice.ValueCurrency,
                    Item = new InvoiceItemDto() { LineItems = invoiceItemLineList },
                    //  HSCode = currentShipment.CommercialInvoice.HSCode
                };
            }
            else
            {
                // Load invoice from Shipment

                invocieDto = new CommercialInvoiceDto()
                {
                    ShipmentId = currentShipment.Id,
                    ShipTo = string.Format("{0} {1} \n {2} {3} \n {4} {5} {6} \n {7}",
                        currentShipment.ConsigneeAddress.FirstName, currentShipment.ConsigneeAddress.LastName, currentShipment.ConsigneeAddress.StreetAddress1, currentShipment.ConsigneeAddress.Number,
                        currentShipment.ConsigneeAddress.ZipCode, currentShipment.ConsigneeAddress.City, currentShipment.ConsigneeAddress.State, currentShipment.ConsigneeAddress.Country),
                    ShipmentReferenceName = currentShipment.ShipmentReferenceName,
                    AddressInformation = new ConsignerAndConsigneeInformationDto
                    {
                        Consignee = new ConsigneeDto
                        {
                            Address1 = currentShipment.ConsigneeAddress.StreetAddress1,
                            Address2 = currentShipment.ConsigneeAddress.StreetAddress2,
                            Postalcode = currentShipment.ConsigneeAddress.ZipCode,
                            City = currentShipment.ConsigneeAddress.City,
                            Country = currentShipment.ConsigneeAddress.Country,
                            State = currentShipment.ConsigneeAddress.State,
                            FirstName = currentShipment.ConsigneeAddress.FirstName,
                            LastName = currentShipment.ConsigneeAddress.LastName,
                            ContactName = currentShipment.ConsigneeAddress.ContactName,
                            ContactNumber = currentShipment.ConsigneeAddress.PhoneNumber,
                            Email = currentShipment.ConsigneeAddress.EmailAddress,
                            Number = currentShipment.ConsigneeAddress.Number
                        },
                        Consigner = new ConsignerDto
                        {
                            Address1 = currentShipment.ConsignorAddress.StreetAddress1,
                            Address2 = currentShipment.ConsignorAddress.StreetAddress2,
                            Postalcode = currentShipment.ConsignorAddress.ZipCode,
                            City = currentShipment.ConsignorAddress.City,
                            Country = currentShipment.ConsignorAddress.Country,
                            State = currentShipment.ConsignorAddress.State,
                            FirstName = currentShipment.ConsignorAddress.FirstName,
                            LastName = currentShipment.ConsignorAddress.LastName,
                            ContactName = currentShipment.ConsignorAddress.ContactName,
                            ContactNumber = currentShipment.ConsignorAddress.PhoneNumber,
                            Email = currentShipment.ConsignorAddress.EmailAddress,
                            Number = currentShipment.ConsignorAddress.Number
                        }
                    },
                    CreatedDate = GetLocalTimeByUser(currentShipment.CreatedBy, currentShipment.CreatedDate).Value.ToString("dd-MMM-yyyy"),
                    InvoiceTo = string.Format("{0} {1} \n {2} {3} \n {4} {5} {6} \n {7}",
                        currentShipment.ConsigneeAddress.FirstName, currentShipment.ConsigneeAddress.LastName, currentShipment.ConsigneeAddress.StreetAddress1, currentShipment.ConsigneeAddress.Number,
                        currentShipment.ConsigneeAddress.ZipCode, currentShipment.ConsigneeAddress.City, currentShipment.ConsigneeAddress.State, currentShipment.ConsigneeAddress.Country),

                    InvoiceNo = currentShipment.ShipmentCode,

                    ShipmentServices = Utility.GetEnumDescription((ShipmentService)currentShipment.ShipmentService),
                    TermsOfPayment = "FREE OF CHARGE",

                    CountryOfOrigin = currentShipment.ConsignorAddress.Country,
                    CountryOfDestination = currentShipment.ConsigneeAddress.Country,
                    ModeOfTransport = currentShipment.Carrier.Name + " " + currentShipment.ServiceLevel + " " + currentShipment.TrackingNumber,
                    ValueCurrency = currentShipment.ShipmentPackage.InsuranceCurrencyType,
                    PackageDetails = new PackageDetailsDto
                    {
                        CmLBS = Convert.ToBoolean(currentShipment.ShipmentPackage.VolumeMetricId),
                        VolumeCMM = Convert.ToBoolean(currentShipment.ShipmentPackage.VolumeMetricId),
                        Count = currentShipment.ShipmentPackage.PackageProducts.Count,
                        DeclaredValue = currentShipment.ShipmentPackage.InsuranceDeclaredValue,
                        HsCode = currentShipment.ShipmentPackage.HSCode,
                        Instructions = currentShipment.ShipmentPackage.CarrierInstruction,
                        IsInsuared = currentShipment.ShipmentPackage.IsInsured.ToString(),
                        TotalVolume = currentShipment.ShipmentPackage.TotalVolume,
                        TotalWeight = currentShipment.ShipmentPackage.TotalWeight,
                        ValueCurrency = currentShipment.ShipmentPackage.InsuranceCurrencyType,
                        PreferredCollectionDate = currentShipment.ShipmentPackage.CollectionDate.ToString(),
                        ProductIngredients = this.getPackageDetails(currentShipment.ShipmentPackage.PackageProducts),
                        ShipmentDescription = currentShipment.ShipmentPackage.PackageDescription,
                        CarrierCost = currentShipment.ShipmentPackage.CarrierCost.ToString()

                    },
                    Item = new InvoiceItemDto() { LineItems = new List<InvoiceItemLineDto>() },
                    VatNo = currentShipment.Division.Company.VATNumber,
                    // HSCode = currentShipment.ShipmentPackage.HSCode
                };
            }
            //  }





            return invocieDto;
        }

        public OperationResult UpdateTrackingNo(AirwayBillDto awbDto)
        {
            OperationResult result = new OperationResult();

            var shipment = context.Shipments.Where(s => s.Id == awbDto.ShipmentId).First();
            shipment.TrackingNumber = awbDto.TrackingNumber;
            int saveResult = context.SaveChanges();

            if (saveResult == 1)
            {
                result.Status = Status.Success;
                result.Message = "Successfully updated the Tracking Number";
            }
            else
            {
                result.Status = Status.Error;
                result.Message = "There was an error when updating the Tracking Number";
            }

            return result;
        }

        //get the shipment from shipment code for Airway Bill generation
        public AirwayBillDto GetshipmentByShipmentCodeForAirwayBill(string shipmentCode)
        {

            Data.Entity.Shipment currentShipment = null;
            long tenantId = 0;
            AirwayBillDto awbill = null;

            //using (PIContext context = PIContext.Get())
            //{
            currentShipment = context.Shipments.Where(x => x.ShipmentCode.ToString() == shipmentCode).FirstOrDefault();

            tenantId = currentShipment.Division.Company.TenantId;

            // Load invoice from Shipment

            awbill = new AirwayBillDto()
            {
                ShipmentId = currentShipment.Id,
                ShipTo = string.Format("{0} {1} \n {2} {3} \n {4} {5} {6} \n {7}",
                    currentShipment.ConsigneeAddress.FirstName, currentShipment.ConsigneeAddress.LastName, currentShipment.ConsigneeAddress.StreetAddress1, currentShipment.ConsigneeAddress.Number,
                    currentShipment.ConsigneeAddress.ZipCode, currentShipment.ConsigneeAddress.City, currentShipment.ConsigneeAddress.State, currentShipment.ConsigneeAddress.Country),
                ShipmentReferenceName = currentShipment.ShipmentReferenceName,
                AddressInformation = new ConsignerAndConsigneeInformationDto
                {
                    Consignee = new ConsigneeDto
                    {
                        Address1 = currentShipment.ConsigneeAddress.StreetAddress1,
                        Address2 = currentShipment.ConsigneeAddress.StreetAddress2,
                        Postalcode = currentShipment.ConsigneeAddress.ZipCode,
                        City = currentShipment.ConsigneeAddress.City,
                        Country = currentShipment.ConsigneeAddress.Country,
                        State = currentShipment.ConsigneeAddress.State,
                        FirstName = currentShipment.ConsigneeAddress.FirstName,
                        LastName = currentShipment.ConsigneeAddress.LastName,
                        ContactName = currentShipment.ConsigneeAddress.ContactName,
                        ContactNumber = currentShipment.ConsigneeAddress.PhoneNumber,
                        Email = currentShipment.ConsigneeAddress.EmailAddress,
                        Number = currentShipment.ConsigneeAddress.Number
                    },
                    Consigner = new ConsignerDto
                    {
                        Address1 = currentShipment.ConsignorAddress.StreetAddress1,
                        Address2 = currentShipment.ConsignorAddress.StreetAddress2,
                        Postalcode = currentShipment.ConsignorAddress.ZipCode,
                        City = currentShipment.ConsignorAddress.City,
                        Country = currentShipment.ConsignorAddress.Country,
                        State = currentShipment.ConsignorAddress.State,
                        FirstName = currentShipment.ConsignorAddress.FirstName,
                        LastName = currentShipment.ConsignorAddress.LastName,
                        ContactName = currentShipment.ConsignorAddress.ContactName,
                        ContactNumber = currentShipment.ConsignorAddress.PhoneNumber,
                        Email = currentShipment.ConsignorAddress.EmailAddress,
                        Number = currentShipment.ConsignorAddress.Number
                    }
                },
                CareerDetails = new CarrierDto
                {
                    Name = currentShipment.Carrier.Name,

                },
                CostCenter = new CostCenterDto
                {
                    Id = currentShipment.CostCenter.Id,
                    Name = currentShipment.CostCenter.Name,
                    BillingAddress = new AddressDto
                    {
                        Number = currentShipment.CostCenter.BillingAddress.Number,
                        StreetAddress1 = currentShipment.CostCenter.BillingAddress.StreetAddress1,
                        StreetAddress2 = currentShipment.CostCenter.BillingAddress.StreetAddress2,
                        City = currentShipment.CostCenter.BillingAddress.City,
                        ZipCode = currentShipment.CostCenter.BillingAddress.ZipCode,
                        State = currentShipment.CostCenter.BillingAddress.State,
                        Country = currentShipment.CostCenter.BillingAddress.Country
                    }
                },
                CreatedDate = GetLocalTimeByUser(currentShipment.CreatedBy, currentShipment.CreatedDate).Value.ToString("dd-MMM-yyyy"),
                InvoiceTo = string.Format("{0} {1} \n {2} {3} \n {4} {5} {6} \n {7}",
                    currentShipment.ConsigneeAddress.FirstName, currentShipment.ConsigneeAddress.LastName, currentShipment.ConsigneeAddress.StreetAddress1, currentShipment.ConsigneeAddress.Number,
                    currentShipment.ConsigneeAddress.ZipCode, currentShipment.ConsigneeAddress.City, currentShipment.ConsigneeAddress.State, currentShipment.ConsigneeAddress.Country),

                InvoiceNo = currentShipment.ShipmentCode,

                ShipmentServices = Utility.GetEnumDescription((ShipmentService)currentShipment.ShipmentService),
                TermsOfPayment = "FREE OF CHARGE",

                CountryOfOrigin = currentShipment.ConsignorAddress.Country,
                CountryOfDestination = currentShipment.ConsigneeAddress.Country,
                ModeOfTransport = currentShipment.Carrier.Name + " " + currentShipment.ServiceLevel + " " + currentShipment.TrackingNumber,
                ServiceLevel = currentShipment.ServiceLevel,
                TrackingNumber = currentShipment.TrackingNumber,

                ValueCurrency = currentShipment.ShipmentPackage.InsuranceCurrencyType,
                PackageDetails = new PackageDetailsDto
                {
                    CmLBS = Convert.ToBoolean(currentShipment.ShipmentPackage.VolumeMetricId),
                    VolumeCMM = Convert.ToBoolean(currentShipment.ShipmentPackage.VolumeMetricId),
                    Count = currentShipment.ShipmentPackage.PackageProducts.Count,
                    DeclaredValue = currentShipment.ShipmentPackage.InsuranceDeclaredValue,
                    HsCode = currentShipment.ShipmentPackage.HSCode,
                    Instructions = currentShipment.ShipmentPackage.CarrierInstruction,
                    IsInsuared = currentShipment.ShipmentPackage.IsInsured.ToString(),
                    TotalVolume = currentShipment.ShipmentPackage.TotalVolume,
                    TotalWeight = currentShipment.ShipmentPackage.TotalWeight,
                    ValueCurrency = currentShipment.ShipmentPackage.InsuranceCurrencyType,
                    PreferredCollectionDate = currentShipment.ShipmentPackage.CollectionDate.ToString(),
                    ProductIngredients = this.getPackageDetails(currentShipment.ShipmentPackage.PackageProducts),
                    ShipmentDescription = currentShipment.ShipmentPackage.PackageDescription,
                    CarrierCost = currentShipment.ShipmentPackage.CarrierCost.ToString(),
                    EarliestPickupDate = currentShipment.ShipmentPackage.EarliestPickupDate.HasValue ?
                                                context.GetLocalTimeByUser(currentShipment.CreatedBy, currentShipment.ShipmentPackage.EarliestPickupDate.Value).ToString() : null,
                    EstDeliveryDate = currentShipment.ShipmentPackage.EstDeliveryDate.HasValue ?
                                                context.GetLocalTimeByUser(currentShipment.CreatedBy, currentShipment.ShipmentPackage.EstDeliveryDate.Value).ToString() : null,
                },

                VatNo = currentShipment.Division.Company.VATNumber
                // HSCode = currentShipment.ShipmentPackage.HSCode
            };


            return awbill;
        }

        public void DeleteFileInDB(FileUploadDto fileDetails)
        {
            var document = context.ShipmentDocument.Where(x => x.Id == fileDetails.Id).SingleOrDefault();

            context.ShipmentDocument.Remove(document);
            context.SaveChanges();
        }


        public ShipmentOperationResult SaveCommercialInvoice(CommercialInvoiceDto addInvoice)
        {

            ShipmentOperationResult result = new ShipmentOperationResult();
            //CompanyManagement companyManagement = new CompanyManagement();
            //Company currentcompany = companyManagement.GetCompanyByUserId(addShipment.UserId);
            //long sysDivisionId = 0;
            //long sysCostCenterId = 0;

            Shipment shipment = context.Shipments.Where(s => s.Id == addInvoice.ShipmentId).SingleOrDefault();

            //using (PIContext context = PIContext.Get())
            //{
            // If has invoice from shipment id, delete
            var inv = context.CommercialInvoices.Where(e => e.ShipmentId == addInvoice.ShipmentId).FirstOrDefault();
            if (inv != null)
            {
                context.CommercialInvoices.Remove(inv);
            }

            var invoiceItemLineList = new List<InvoiceItemLine>();
            addInvoice.Item.LineItems.ToList().ForEach(p => invoiceItemLineList.Add(new InvoiceItemLine()
            {
                Description = p.Description,
                PricePerPiece = p.PricePerPiece,
                Quantity = p.Quantity,
                CreatedBy = shipment.CreatedBy,
                CreatedDate = DateTime.UtcNow,
                IsActive = true,
                HSCode = p.HSCode,
            }));

            CommercialInvoice invoice = new CommercialInvoice()
            {
                ShipmentId = addInvoice.ShipmentId,
                ShipmentReferenceName = addInvoice.ShipmentReferenceName,
                CreatedBy = shipment.CreatedBy,
                CreatedDate = Convert.ToDateTime(addInvoice.CreatedDate).ToUniversalTime(),
                IsActive = true,
                ShipTo = addInvoice.ShipTo,
                InvoiceNo = addInvoice.InvoiceNo,
                VatNo = addInvoice.VatNo,
                CustomerNo = addInvoice.CustomerNo,
                InvoiceTo = addInvoice.InvoiceTo,
                ShipmentService = (short)Utility.GetValueFromDescription<ShipmentService>(addInvoice.ShipmentServices),
                TermsOfPayment = addInvoice.TermsOfPayment,
                CountryOfOrigin = addInvoice.CountryOfOrigin,
                CountryOfDestination = addInvoice.CountryOfDestination,
                ModeOfTransport = addInvoice.ModeOfTransport,
                ImportBroker = addInvoice.ImportBroker,
                Note = addInvoice.Note,
                ValueCurrency = addInvoice.ValueCurrency,
                InvoiceItem = new InvoiceItem()
                {
                    CreatedBy = shipment.CreatedBy,
                    CreatedDate = DateTime.UtcNow,
                    IsActive = true,
                    InvoiceItemLines = invoiceItemLineList
                }
            };

            context.CommercialInvoices.Add(invoice);
            context.SaveChanges();

            return result;
        }

        private string sep(string s)
        {
            int l = s.IndexOf("-");
            if (l > 0)
            {
                return s.Substring(0, l);
            }
            return "";

        }

        public string RequestForQuote(ShipmentDto addShipment)
        {
            StringBuilder strTemplate = new StringBuilder();
            string keyValueHtmlTemplate = "<span> <span class='name'>{0}:</span> <span class='value'>{1}</span> </span> <br>";

            // Start the document
            strTemplate.Append("<!DOCTYPE html><html><head><title></title><meta charset='utf-8' /> <style> .name{ width:200px;display:inline-block;font-weight:600;font-size:medium } .value{ font-style:italic; } table { border-collapse: collapse; width: 100%; } th, td { text-align: left; padding: 8px; } tr:nth-child(even){background-color: #f2f2f2} th {background-color: lightblue;color: white;} </style></head><body>");

            // build the template

            // General 
            strTemplate.Append("<h1>Request For Quote</h1>");
            strTemplate.Append("<h3>Shipment General Information</h3>");
            strTemplate.AppendFormat(keyValueHtmlTemplate, "Reference", addShipment.GeneralInformation.ShipmentName);
            strTemplate.AppendFormat(keyValueHtmlTemplate, "Product", addShipment.GeneralInformation.ShipmentMode);
            strTemplate.AppendFormat(keyValueHtmlTemplate, "Condition", addShipment.GeneralInformation.ShipmentServices);
            strTemplate.Append("<br>");

            // Consigner Address
            strTemplate.Append("<h3>Consigner Address Information</h3>");
            strTemplate.AppendFormat(keyValueHtmlTemplate, "First Name", addShipment.AddressInformation.Consigner.FirstName);
            strTemplate.AppendFormat(keyValueHtmlTemplate, "Last Name", addShipment.AddressInformation.Consigner.LastName);
            strTemplate.AppendFormat(keyValueHtmlTemplate, "Country", addShipment.AddressInformation.Consigner.Country);
            strTemplate.AppendFormat(keyValueHtmlTemplate, "Postal/ZipCode", addShipment.AddressInformation.Consigner.Postalcode);
            strTemplate.AppendFormat(keyValueHtmlTemplate, "Number", addShipment.AddressInformation.Consigner.Number);
            strTemplate.AppendFormat(keyValueHtmlTemplate, "Street Name", addShipment.AddressInformation.Consigner.Address1);
            strTemplate.AppendFormat(keyValueHtmlTemplate, "Street Name 2", addShipment.AddressInformation.Consigner.Address2);
            strTemplate.AppendFormat(keyValueHtmlTemplate, "City", addShipment.AddressInformation.Consigner.City);
            strTemplate.AppendFormat(keyValueHtmlTemplate, "State Code", addShipment.AddressInformation.Consigner.State);
            strTemplate.AppendFormat(keyValueHtmlTemplate, "Email", addShipment.AddressInformation.Consigner.Email);
            strTemplate.AppendFormat(keyValueHtmlTemplate, "Contact Number", addShipment.AddressInformation.Consigner.ContactNumber);
            strTemplate.AppendFormat(keyValueHtmlTemplate, "Contact Name", addShipment.AddressInformation.Consigner.ContactName);
            strTemplate.Append("<br>");

            // Consignee Address
            strTemplate.Append("<h3>Consignee Address Information</h3>");
            strTemplate.AppendFormat(keyValueHtmlTemplate, "First Name", addShipment.AddressInformation.Consignee.FirstName);
            strTemplate.AppendFormat(keyValueHtmlTemplate, "Last Name", addShipment.AddressInformation.Consignee.LastName);
            strTemplate.AppendFormat(keyValueHtmlTemplate, "Country", addShipment.AddressInformation.Consignee.Country);
            strTemplate.AppendFormat(keyValueHtmlTemplate, "Postal/ZipCode", addShipment.AddressInformation.Consignee.Postalcode);
            strTemplate.AppendFormat(keyValueHtmlTemplate, "Number", addShipment.AddressInformation.Consignee.Number);
            strTemplate.AppendFormat(keyValueHtmlTemplate, "Street Name", addShipment.AddressInformation.Consignee.Address1);
            strTemplate.AppendFormat(keyValueHtmlTemplate, "Street Name 2", addShipment.AddressInformation.Consignee.Address2);
            strTemplate.AppendFormat(keyValueHtmlTemplate, "City", addShipment.AddressInformation.Consignee.City);
            strTemplate.AppendFormat(keyValueHtmlTemplate, "State Code", addShipment.AddressInformation.Consignee.State);
            strTemplate.AppendFormat(keyValueHtmlTemplate, "Email", addShipment.AddressInformation.Consignee.Email);
            strTemplate.AppendFormat(keyValueHtmlTemplate, "Contact Number", addShipment.AddressInformation.Consignee.ContactNumber);
            strTemplate.AppendFormat(keyValueHtmlTemplate, "Contact Name", addShipment.AddressInformation.Consignee.ContactName);
            strTemplate.Append("<br>");

            // Package details
            strTemplate.Append("<h3>Shipment Package Details</h3>");
            strTemplate.AppendFormat(keyValueHtmlTemplate, "Shipment Description", addShipment.PackageDetails.ShipmentDescription);

            strTemplate.Append("<h4>Product Details</h4>");
            strTemplate.Append("<table>");
            strTemplate.Append("<tr> <th>PRODUCT TYPE</th> <th>QUANTITY</th> <th>DESCRIPTION</th> <th>WEIGHT</th> <th>HEIGHT</th> <th>LENGTH</th> <th>WIDTH</th> </tr>");

            foreach (var item in addShipment.PackageDetails.ProductIngredients)
            {
                strTemplate.AppendFormat("<tr> <td>{0}</td> <td>{1}</td> <td>{2}</td> <td>{3}</td> <td>{4}</td> <td>{5}</td> <td>{6}</td> </tr>", item.ProductType, item.Quantity, item.Description,
                item.Weight, item.Height, item.Length, item.Width);
            }
            strTemplate.Append("</table><br>");

            strTemplate.AppendFormat(keyValueHtmlTemplate, "HS Code", addShipment.PackageDetails.HsCode);
            strTemplate.AppendFormat(keyValueHtmlTemplate, "Dangerous Good", addShipment.PackageDetails.IsDG ? "Yes" : "No");
            if (addShipment.PackageDetails.IsDG)
            {
                strTemplate.AppendFormat(keyValueHtmlTemplate, "Dangerous Good type", addShipment.PackageDetails.DGType);
                strTemplate.AppendFormat(keyValueHtmlTemplate, "Dangerous Good accessible", addShipment.PackageDetails.Accessibility ? "Yes" : "No");
            }
            strTemplate.AppendFormat(keyValueHtmlTemplate, "Preferred collection date", addShipment.PackageDetails.PreferredCollectionDate);
            strTemplate.AppendFormat(keyValueHtmlTemplate, "Carrier Instructions", addShipment.PackageDetails.Instructions);
            strTemplate.AppendFormat(keyValueHtmlTemplate, "Insurance", addShipment.PackageDetails.IsInsuared == "true" ? "Yes" : "No");
            strTemplate.AppendFormat(keyValueHtmlTemplate, "Declared Value", addShipment.PackageDetails.DeclaredValue);
            strTemplate.AppendFormat(keyValueHtmlTemplate, "Currency format", ((CurrencyType)addShipment.PackageDetails.ValueCurrency).ToString());

            strTemplate.Append("<br>");


            strTemplate.Append("</body></html>");
            // End the document

            return strTemplate.ToString();
        }

        public PagedList GetShipmentForCompanyAndSyncWithSIS(long companyId)
        {
            int page = 1;
            int pageSize = 10;
            var pagedRecord = new PagedList();

            pagedRecord.Content = new List<ShipmentDto>();

            //using (var context = PIContext.Get())
            //{
            var content = (from shipment in context.Shipments
                           where shipment.Division.CompanyId == companyId
                           select shipment).ToList();

            // Update retrieve shipment list status from SIS.
            string environment = "";
            foreach (var shipment in content)
            {
                if (shipment.Status != ((short)ShipmentStatus.Delivered) && !string.IsNullOrWhiteSpace(shipment.TrackingNumber))
                {
                    environment = GetEnvironmentByTarrif(shipment.TariffText);

                    UpdateLocationHistory(shipment.Carrier.Name, shipment.TrackingNumber, shipment.ShipmentCode, environment, shipment.Id);
                }
            }

            foreach (var item in content)
            {
                pagedRecord.Content.Add(new ShipmentDto
                {
                    AddressInformation = new ConsignerAndConsigneeInformationDto
                    {
                        Consignee = new ConsigneeDto
                        {
                            Address1 = item.ConsigneeAddress.StreetAddress1,
                            Address2 = item.ConsigneeAddress.StreetAddress2,
                            Postalcode = item.ConsigneeAddress.ZipCode,
                            City = item.ConsigneeAddress.City,
                            Country = item.ConsigneeAddress.Country,
                            State = item.ConsigneeAddress.State,
                            FirstName = item.ConsigneeAddress.FirstName,
                            LastName = item.ConsigneeAddress.LastName,
                            ContactName = item.ConsigneeAddress.ContactName,
                            ContactNumber = item.ConsigneeAddress.PhoneNumber,
                            Email = item.ConsigneeAddress.EmailAddress,
                            Number = item.ConsigneeAddress.Number
                        },
                        Consigner = new ConsignerDto
                        {
                            Address1 = item.ConsignorAddress.StreetAddress1,
                            Address2 = item.ConsignorAddress.StreetAddress2,
                            Postalcode = item.ConsignorAddress.ZipCode,
                            City = item.ConsignorAddress.City,
                            Country = item.ConsignorAddress.Country,
                            State = item.ConsignorAddress.State,
                            FirstName = item.ConsignorAddress.FirstName,
                            LastName = item.ConsignorAddress.LastName,
                            ContactName = item.ConsignorAddress.ContactName,
                            ContactNumber = item.ConsignorAddress.PhoneNumber,
                            Email = item.ConsignorAddress.EmailAddress,
                            Number = item.ConsignorAddress.Number
                        }
                    },
                    GeneralInformation = new GeneralInformationDto
                    {
                        CostCenterId = item.CostCenterId.GetValueOrDefault(),
                        DivisionId = item.DivisionId.GetValueOrDefault(),
                        ShipmentCode = item.ShipmentCode,
                        ShipmentMode = item.ShipmentMode.ToString(),
                        ShipmentName = item.ShipmentName,
                        //ShipmentTermCode = item.ShipmentTermCode,
                        //ShipmentTypeCode = item.ShipmentTypeCode,


                        TrackingNumber = item.TrackingNumber,
                        CreatedDate = GetLocalTimeByUser(item.CreatedBy, item.CreatedDate).Value.ToString("dd MMM yyyy"),
                        Status = Utility.GetEnumDescription((ShipmentStatus)item.Status),
                        ShipmentLabelBLOBURL = getLabelforShipmentFromBlobStorage(item.Id, item.Division.Company.TenantId)
                    },
                    PackageDetails = new PackageDetailsDto
                    {
                        CmLBS = Convert.ToBoolean(item.ShipmentPackage.VolumeMetricId),
                        VolumeCMM = Convert.ToBoolean(item.ShipmentPackage.VolumeMetricId),
                        Count = item.ShipmentPackage.PackageProducts.Count,
                        DeclaredValue = item.ShipmentPackage.InsuranceDeclaredValue,
                        HsCode = item.ShipmentPackage.HSCode,
                        Instructions = item.ShipmentPackage.CarrierInstruction,
                        IsInsuared = item.ShipmentPackage.IsInsured.ToString(),
                        TotalVolume = item.ShipmentPackage.TotalVolume,
                        TotalWeight = item.ShipmentPackage.TotalWeight,
                        ValueCurrency = item.ShipmentPackage.Currency == null ? 1 : Convert.ToInt32(item.ShipmentPackage.Currency.Id),
                        PreferredCollectionDate = item.ShipmentPackage.CollectionDate.ToString(),
                        ProductIngredients = this.getPackageDetails(item.ShipmentPackage.PackageProducts),
                        ShipmentDescription = item.ShipmentPackage.PackageDescription

                    },
                    CarrierInformation = new CarrierInformationDto
                    {
                        CarrierName = item.Carrier.Name,
                        serviceLevel = item.ServiceLevel,
                        PickupDate = item.PickUpDate.HasValue ? (DateTime?)context.GetLocalTimeByUser(item.CreatedBy, item.PickUpDate.Value) : null
                    }

                });
            }

            pagedRecord.TotalRecords = content.Count();
            pagedRecord.CurrentPage = page;
            pagedRecord.PageSize = pageSize;
            pagedRecord.TotalPages = (int)Math.Ceiling((decimal)pagedRecord.TotalRecords / pagedRecord.PageSize);

            return pagedRecord;
            // }
        }


        public PagedList GetAllShipmentByCompanyId(string companyId)
        {
            int page = 1;
            int pageSize = 10;
            var pagedRecord = new PagedList();

            pagedRecord.Content = new List<ShipmentDto>();

            //using (var context = PIContext.Get())
            //{
            var content = (from shipment in context.Shipments
                           where shipment.Division.CompanyId.ToString() == companyId
                           select shipment).ToList();

            foreach (var item in content)
            {
                pagedRecord.Content.Add(new ShipmentDto
                {
                    AddressInformation = new ConsignerAndConsigneeInformationDto
                    {
                        Consignee = new ConsigneeDto
                        {
                            Address1 = item.ConsigneeAddress.StreetAddress1,
                            Address2 = item.ConsigneeAddress.StreetAddress2,
                            Postalcode = item.ConsigneeAddress.ZipCode,
                            City = item.ConsigneeAddress.City,
                            Country = item.ConsigneeAddress.Country,
                            State = item.ConsigneeAddress.State,
                            FirstName = item.ConsigneeAddress.FirstName,
                            LastName = item.ConsigneeAddress.LastName,
                            ContactName = item.ConsigneeAddress.ContactName,
                            ContactNumber = item.ConsigneeAddress.PhoneNumber,
                            Email = item.ConsigneeAddress.EmailAddress,
                            Number = item.ConsigneeAddress.Number
                        },
                        Consigner = new ConsignerDto
                        {
                            Address1 = item.ConsignorAddress.StreetAddress1,
                            Address2 = item.ConsignorAddress.StreetAddress2,
                            Postalcode = item.ConsignorAddress.ZipCode,
                            City = item.ConsignorAddress.City,
                            Country = item.ConsignorAddress.Country,
                            State = item.ConsignorAddress.State,
                            FirstName = item.ConsignorAddress.FirstName,
                            LastName = item.ConsignorAddress.LastName,
                            ContactName = item.ConsignorAddress.ContactName,
                            ContactNumber = item.ConsignorAddress.PhoneNumber,
                            Email = item.ConsignorAddress.EmailAddress,
                            Number = item.ConsignorAddress.Number
                        }
                    },
                    GeneralInformation = new GeneralInformationDto
                    {
                        CostCenterId = item.CostCenterId.GetValueOrDefault(),
                        DivisionId = item.DivisionId.GetValueOrDefault(),
                        ShipmentCode = item.ShipmentCode,
                        ShipmentMode = item.ShipmentMode.ToString(),
                        ShipmentName = item.ShipmentName,
                        //ShipmentTermCode = item.ShipmentTermCode,
                        //ShipmentTypeCode = item.ShipmentTypeCode,
                        ShipmentId = item.Id.ToString(),
                        TrackingNumber = item.TrackingNumber,
                        CreatedDate = GetLocalTimeByUser(item.CreatedBy, item.CreatedDate).Value.ToString("dd MMM yyyy"), //item.CreatedDate.ToString("MM/dd/yyyy"),
                        Status = Utility.GetEnumDescription((ShipmentStatus)item.Status),
                        //IsEnableEdit = ((ShipmentStatus)item.Status == ShipmentStatus.Error || (ShipmentStatus)item.Status == ShipmentStatus.Pending),
                        IsEnableEdit = true,
                        //IsEnableDelete = ((ShipmentStatus)item.Status == ShipmentStatus.Error || (ShipmentStatus)item.Status == ShipmentStatus.Pending || (ShipmentStatus)item.Status == ShipmentStatus.BookingConfirmation)
                        IsEnableDelete = true,
                        ShipmentLabelBLOBURL = getLabelforShipmentFromBlobStorage(item.Id, item.Division.Company.TenantId)
                    },
                    PackageDetails = new PackageDetailsDto
                    {
                        CmLBS = Convert.ToBoolean(item.ShipmentPackage.VolumeMetricId),
                        VolumeCMM = Convert.ToBoolean(item.ShipmentPackage.VolumeMetricId),
                        Count = item.ShipmentPackage.PackageProducts.Count,
                        DeclaredValue = item.ShipmentPackage.InsuranceDeclaredValue,
                        HsCode = item.ShipmentPackage.HSCode,
                        Instructions = item.ShipmentPackage.CarrierInstruction,
                        IsInsuared = item.ShipmentPackage.IsInsured.ToString(),
                        TotalVolume = item.ShipmentPackage.TotalVolume,
                        TotalWeight = item.ShipmentPackage.TotalWeight,
                        ValueCurrency = item.ShipmentPackage.Currency == null ? 1 : Convert.ToInt32(item.ShipmentPackage.Currency.Id),
                        PreferredCollectionDate = item.ShipmentPackage.CollectionDate.ToString(),
                        ProductIngredients = this.getPackageDetails(item.ShipmentPackage.PackageProducts),
                        ShipmentDescription = item.ShipmentPackage.PackageDescription

                    },
                    CarrierInformation = new CarrierInformationDto
                    {
                        CarrierName = item.Carrier.Name,
                        serviceLevel = item.ServiceLevel,
                        PickupDate = item.PickUpDate.HasValue ? (DateTime?)context.GetLocalTimeByUser(item.CreatedBy, item.PickUpDate.Value) : null
                    }

                });
            }

            pagedRecord.TotalRecords = content.Count();
            pagedRecord.CurrentPage = page;
            pagedRecord.PageSize = pageSize;
            pagedRecord.TotalPages = (int)Math.Ceiling((decimal)pagedRecord.TotalRecords / pagedRecord.PageSize);

            return pagedRecord;
            //}
        }


        public PagedList loadAllShipmentsForAdmin(string status = null, DateTime? startDate = null, DateTime? endDate = null, string searchValue = null, int currentPage = 0, int pageSize = 10)
        {
            var pagedRecord = new PagedList();
            short enumStatus = status == null ? (short)0 : (short)Enum.Parse(typeof(ShipmentStatus), status);
            string baseWebUrl = ConfigurationManager.AppSettings["BaseWebURL"];

            pagedRecord.Content = new List<ShipmentDto>();

            if (startDate.HasValue)
                startDate = startDate.Value.ToUniversalTime();
            if (endDate.HasValue)
                endDate = endDate.Value.ToUniversalTime();

            var querableContent = (from shipment in context.Shipments
                                   where shipment.IsDelete == false &&
                                   !shipment.IsParent &&
                                    ((status == null ||
                                     (status == "Error" ? (shipment.Status == (short)ShipmentStatus.Error || shipment.Status == (short)ShipmentStatus.Pending)
                                   : status == "Exception" ? (shipment.Status == (short)ShipmentStatus.Exception || shipment.Status == (short)ShipmentStatus.Claim)
                                   : status == "Out for delivery" ? shipment.Status == (short)ShipmentStatus.OutForDelivery
                                   : shipment.Status == enumStatus
                                   )
                                  )) &&
                                   (startDate == null || (shipment.ShipmentPackage.EarliestPickupDate >= startDate && shipment.ShipmentPackage.EarliestPickupDate <= endDate)) &&
                                   (searchValue == null ||
                                   (shipment.TrackingNumber.Contains(searchValue)) || (shipment.Division.Company.Name.Contains(searchValue)) ||
                                   (shipment.ConsignorAddress.Country.Contains(searchValue) || shipment.ConsignorAddress.City.Contains(searchValue)) ||
                                   (shipment.ConsigneeAddress.Country.Contains(searchValue) || shipment.ConsigneeAddress.City.Contains(searchValue)))
                                   select shipment);

            var content = querableContent.OrderByDescending(d => d.CreatedDate).Skip(currentPage).Take(pageSize).ToList();


            foreach (var item in content)
            {
                string errorUrl = "";

                var owner = context.Users.Where(u => u.Id == item.CreatedBy).SingleOrDefault();

                //if shipment is in pending status get the error message
                if ((ShipmentStatus)item.Status == ShipmentStatus.Pending)
                {
                    ShipmentError error = context.ShipmentErrors.Where(i => i.ShipmentId == item.Id).FirstOrDefault();

                    if (error == null)
                    {
                        errorUrl = "http://parcelinternational.pro/errors/" + item.Carrier.Name + "/" + item.ShipmentCode;
                    }
                    else
                    {
                        errorUrl = baseWebUrl + "app/shipment/shipmenterror.html?message=" + error.ErrorMessage;

                    }

                }

                item.Status = (item.Status == (short)ShipmentStatus.Pending) ? (short)ShipmentStatus.Error : item.Status;


                pagedRecord.Content.Add(new ShipmentDto
                {
                    AddressInformation = new ConsignerAndConsigneeInformationDto
                    {
                        Consignee = new ConsigneeDto
                        {
                            Address1 = item.ConsigneeAddress.StreetAddress1,
                            Address2 = item.ConsigneeAddress.StreetAddress2,
                            Postalcode = item.ConsigneeAddress.ZipCode,
                            City = item.ConsigneeAddress.City,
                            Country = item.ConsigneeAddress.Country,
                            State = item.ConsigneeAddress.State,
                            FirstName = item.ConsigneeAddress.FirstName,
                            LastName = item.ConsigneeAddress.LastName,
                            ContactName = item.ConsigneeAddress.ContactName,
                            ContactNumber = item.ConsigneeAddress.PhoneNumber,
                            Email = item.ConsigneeAddress.EmailAddress,
                            Number = item.ConsigneeAddress.Number
                        },
                        Consigner = new ConsignerDto
                        {
                            Address1 = item.ConsignorAddress.StreetAddress1,
                            Address2 = item.ConsignorAddress.StreetAddress2,
                            Postalcode = item.ConsignorAddress.ZipCode,
                            City = item.ConsignorAddress.City,
                            Country = item.ConsignorAddress.Country,
                            State = item.ConsignorAddress.State,
                            FirstName = item.ConsignorAddress.FirstName,
                            LastName = item.ConsignorAddress.LastName,
                            ContactName = item.ConsignorAddress.ContactName,
                            ContactNumber = item.ConsignorAddress.PhoneNumber,
                            Email = item.ConsignorAddress.EmailAddress,
                            Number = item.ConsignorAddress.Number
                        }
                    },
                    GeneralInformation = new GeneralInformationDto
                    {
                        CostCenterId = item.CostCenterId.GetValueOrDefault(),
                        DivisionId = item.DivisionId.GetValueOrDefault(),
                        ShipmentCode = item.ShipmentCode,
                        ShipmentMode = item.ShipmentMode.ToString(),
                        ShipmentName = item.ShipmentName,
                        CompanyName = item.Division.Company.Name,
                        Owner = owner != null ? (owner.FirstName + " " + owner.LastName) : "",
                        //ShipmentTermCode = item.ShipmentTermCode,
                        //ShipmentTypeCode = item.ShipmentTypeCode,
                        ShipmentId = item.Id.ToString(),
                        TrackingNumber = item.TrackingNumber,
                        CreatedDate = GetLocalTimeByUser(item.CreatedBy, item.CreatedDate).Value.ToString("dd MMM yyyy"),
                        Status = ((ShipmentStatus)item.Status).ToString(),
                        IsEnableEdit = true, // Any status is ediitable for admins/support staff
                        IsEnableDelete = ((ShipmentStatus)item.Status == ShipmentStatus.Deleted || (ShipmentStatus)item.Status == ShipmentStatus.Processing) ? false : true, // Any status is deletable for admins/support staff
                        ShipmentLabelBLOBURL = getLabelforShipmentFromBlobStorage(item.Id, item.Division.Company.TenantId),
                        ErrorUrl = errorUrl,
                        CreatedBy = item.CreatedBy

                    },
                    PackageDetails = new PackageDetailsDto
                    {
                        CmLBS = Convert.ToBoolean(item.ShipmentPackage.VolumeMetricId),
                        VolumeCMM = Convert.ToBoolean(item.ShipmentPackage.VolumeMetricId),
                        Count = item.ShipmentPackage.PackageProducts.Count,
                        DeclaredValue = item.ShipmentPackage.InsuranceDeclaredValue,
                        HsCode = item.ShipmentPackage.HSCode,
                        Instructions = item.ShipmentPackage.CarrierInstruction,
                        IsInsuared = item.ShipmentPackage.IsInsured.ToString(),
                        TotalVolume = item.ShipmentPackage.TotalVolume,
                        TotalWeight = item.ShipmentPackage.TotalWeight,
                        ValueCurrency = item.ShipmentPackage.Currency == null ? 1 : Convert.ToInt32(item.ShipmentPackage.Currency.Id),
                        PreferredCollectionDate = item.ShipmentPackage.CollectionDate.ToString(),
                        ProductIngredients = this.getPackageDetails(item.ShipmentPackage.PackageProducts),
                        ShipmentDescription = item.ShipmentPackage.PackageDescription

                    },
                    CarrierInformation = new CarrierInformationDto
                    {
                        CarrierName = item.Carrier.Name,
                        serviceLevel = item.ServiceLevel,
                        PickupDate = item.PickUpDate.HasValue ? ((DateTime?)context.GetLocalTimeByUser(item.CreatedBy, item.PickUpDate.Value)) : null,
                        Provider = item.Provider
                    }

                });
            }

            pagedRecord.TotalRecords = querableContent.Count();
            pagedRecord.PageSize = pageSize;
            pagedRecord.TotalPages = (int)Math.Ceiling((decimal)pagedRecord.TotalRecords / pagedRecord.PageSize);

            return pagedRecord;
        }



        public byte[] loadAllShipmentsForAdminExcelExport(string status = null, DateTime? startDate = null, DateTime? endDate = null,
                                        string number = null, string source = null, string destination = null)
        {
            var pagedRecord = new PagedList();
            pagedRecord.Content = new List<ShipmentDto>();

            pagedRecord.Content = loadAllShipmentsForAdmin().Content;

            return this.GenerateExcelSheetForShipmentExportFunction((List<ShipmentDto>)pagedRecord.Content);

            // }
        }


        public byte[] loadAllShipmentsForExcel(PagedList shipmentSerach)
        {
            var pagedRecord = new PagedList();
            pagedRecord.Content = new List<ShipmentDto>();

            var retunedResult = GetAllShipmentsbyUser(shipmentSerach);
            pagedRecord.Content = retunedResult == null ? null : retunedResult.Content;

            return this.GenerateExcelSheetForShipmentExportFunction((List<ShipmentDto>)pagedRecord.Content); ;
        }



        private byte[] GenerateExcelSheetForShipmentExportFunction(IList<ShipmentDto> shipments)
        {
            using (ExcelPackage excel = new ExcelPackage())
            {
                //Create the worksheet
                ExcelWorksheet ws = excel.Workbook.Worksheets.Add("Shipments");

                ws.Cells["A1"].Value = "Report:";
                ws.Cells["A1"].Style.Font.Size = 16;
                ws.Cells["A1"].Style.Font.Bold = true;
                ws.Cells["A1"].Style.Font.Name = "Calibri";
                ws.Cells["A1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                ws.Cells["B1"].Value = "Shipment Details";
                ws.Cells["B1"].Style.Font.Size = 16;
                ws.Cells["B1"].Style.Font.Bold = true;
                ws.Cells["B1"].Style.Font.Name = "Calibri";
                ws.Cells["B1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                ws.Cells["A2"].Value = "Generated on:";
                ws.Cells["A2"].Style.Font.Size = 12;
                ws.Cells["A2"].Style.Font.Bold = true;
                ws.Cells["A2"].Style.Font.Name = "Calibri";
                ws.Cells["A2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                ws.Cells["B2"].Value = DateTime.Now;
                ws.Cells["B2"].Style.Numberformat.Format = "dd-MMM-yyyy hh:mm";
                ws.Cells["B2"].Style.Font.Size = 12;
                ws.Cells["B2"].Style.Font.Bold = true;
                ws.Cells["B2"].Style.Font.Name = "Calibri";
                ws.Cells["B2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                // Set headings.              

                ws.Cells["A6"].Value = "Shipment Reference";
                ws.Cells["B6"].Value = "Carrier";
                ws.Cells["C6"].Value = "Product";
                ws.Cells["D6"].Value = "Shipment Status";
                ws.Cells["E6"].Value = "Tracking Number";
                ws.Cells["F6"].Value = "Price";
                ws.Cells["G6"].Value = "Total Weight";
                ws.Cells["H6"].Value = "Weight Metric Unit";
                ws.Cells["I6"].Value = "Shipment Condition";
                ws.Cells["J6"].Value = "Package Count";
                ws.Cells["K6"].Value = "Sender Company Name";
                ws.Cells["L6"].Value = "Sender First Name";
                ws.Cells["M6"].Value = "Sender Last Name";
                ws.Cells["N6"].Value = "Sender Phone Number";
                ws.Cells["O6"].Value = "Sender Email";
                ws.Cells["P6"].Value = "Sender Address";
                ws.Cells["Q6"].Value = "Sender Postal Code";
                ws.Cells["R6"].Value = "Sender City";
                ws.Cells["S6"].Value = "Sender State";
                ws.Cells["T6"].Value = "Sender Country Code";

                ws.Cells["U6"].Value = "Created Date";
                ws.Cells["V6"].Value = "Pickup Date";
                ws.Cells["W6"].Value = "Delivery Date";

                ws.Cells["X6"].Value = "Recipient Company Name";
                ws.Cells["Y6"].Value = "Recipient First Name";
                ws.Cells["Z6"].Value = "Recipient Last Name";
                ws.Cells["AA6"].Value = "Recipient Phone Number";
                ws.Cells["AB6"].Value = "Recipient Email";
                ws.Cells["AC6"].Value = "Recipient Address";
                ws.Cells["AD6"].Value = "Recipient Postal Code";
                ws.Cells["AE6"].Value = "Recipient City";
                ws.Cells["AF6"].Value = "Recipient State";
                ws.Cells["AG6"].Value = "Recipient Country Code";

                ws.Cells["AH6"].Value = "Shipment ID";



                //Format the header for columns.
                using (ExcelRange rng = ws.Cells["A6:AH6"])
                {
                    rng.Style.Font.Bold = true;
                    rng.Style.Fill.PatternType = ExcelFillStyle.Solid;                      //Set Pattern for the background to Solid
                    rng.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(79, 129, 189));  //Set color to dark blue
                    rng.Style.Font.Color.SetColor(System.Drawing.Color.White);
                }

                //ws.Cells["A6:H6"].AutoFitColumns();

                // Set data.
                int rowIndex = 6;
                foreach (var shipment in shipments) // Adding Data into rows
                {
                    rowIndex++;
                    var WeightMetric = "";
                    if (shipment.PackageDetails.CmLBS)
                    {
                        WeightMetric = "Kg";
                    }
                    else
                    {
                        WeightMetric = "Lbs";
                    }

                    var cell = ws.Cells[rowIndex, 1];
                    cell.Value = shipment.GeneralInformation.ShipmentName;

                    cell = ws.Cells[rowIndex, 2];
                    cell.Value = shipment.CarrierInformation.CarrierName;

                    cell = ws.Cells[rowIndex, 3];
                    cell.Value = shipment.GeneralInformation.ShipmentMode;

                    cell = ws.Cells[rowIndex, 4];
                    cell.Value = shipment.GeneralInformation.Status;

                    cell = ws.Cells[rowIndex, 5];
                    cell.Value = shipment.GeneralInformation.TrackingNumber;

                    cell = ws.Cells[rowIndex, 6];
                    cell.Style.Numberformat.Format = "$0.00";
                    cell.Value = shipment.CarrierInformation.Price;

                    cell = ws.Cells[rowIndex, 7];
                    cell.Style.Numberformat.Format = "0.00";
                    cell.Value = shipment.PackageDetails.TotalWeight;

                    cell = ws.Cells[rowIndex, 8];
                    cell.Value = WeightMetric;

                    cell = ws.Cells[rowIndex, 9];
                    cell.Value = shipment.GeneralInformation.ShipmentServices;

                    cell = ws.Cells[rowIndex, 10];
                    cell.Value = shipment.PackageDetails.Count;

                    cell = ws.Cells[rowIndex, 11];
                    cell.Value = shipment.AddressInformation.Consigner.CompanyName;

                    cell = ws.Cells[rowIndex, 12];
                    cell.Value = shipment.AddressInformation.Consigner.FirstName;

                    cell = ws.Cells[rowIndex, 13];
                    cell.Value = shipment.AddressInformation.Consigner.LastName;

                    cell = ws.Cells[rowIndex, 14];
                    cell.Value = shipment.AddressInformation.Consigner.ContactNumber;

                    cell = ws.Cells[rowIndex, 15];
                    cell.Value = shipment.AddressInformation.Consigner.Email;

                    cell = ws.Cells[rowIndex, 16];
                    cell.Value = shipment.AddressInformation.Consigner.Address1 + shipment.AddressInformation.Consigner.Address2;

                    cell = ws.Cells[rowIndex, 17];
                    cell.Value = shipment.AddressInformation.Consigner.Postalcode;

                    cell = ws.Cells[rowIndex, 18];
                    cell.Value = shipment.AddressInformation.Consigner.City;

                    cell = ws.Cells[rowIndex, 19];
                    cell.Value = shipment.AddressInformation.Consigner.State;

                    cell = ws.Cells[rowIndex, 20];
                    cell.Value = shipment.AddressInformation.Consigner.Country;

                    cell = ws.Cells[rowIndex, 21];
                    cell.Style.Numberformat.Format = "dd MMM yyyy";
                    cell.Value = shipment.GeneralInformation.CreatedDate;

                    cell = ws.Cells[rowIndex, 22];
                    cell.Style.Numberformat.Format = "dd MMM yyyy";
                    cell.Value = shipment.CarrierInformation.PickupDate.HasValue ? (DateTime?)context.GetLocalTimeByUser(shipment.CreatedBy, shipment.CarrierInformation.PickupDate.Value) : null;

                    cell = ws.Cells[rowIndex, 23];
                    cell.Style.Numberformat.Format = "dd MMM yyyy";
                    cell.Value = shipment.CarrierInformation.DeliveryTime;


                    cell = ws.Cells[rowIndex, 24];
                    cell.Value = shipment.AddressInformation.Consignee.CompanyName;

                    cell = ws.Cells[rowIndex, 25];
                    cell.Value = shipment.AddressInformation.Consignee.FirstName;

                    cell = ws.Cells[rowIndex, 26];
                    cell.Value = shipment.AddressInformation.Consignee.LastName;

                    cell = ws.Cells[rowIndex, 27];
                    cell.Value = shipment.AddressInformation.Consignee.ContactNumber;

                    cell = ws.Cells[rowIndex, 28];
                    cell.Value = shipment.AddressInformation.Consignee.Email;

                    cell = ws.Cells[rowIndex, 29];
                    cell.Value = shipment.AddressInformation.Consignee.Address1 + shipment.AddressInformation.Consignee.Address2;

                    cell = ws.Cells[rowIndex, 30];
                    cell.Value = shipment.AddressInformation.Consignee.Postalcode;

                    cell = ws.Cells[rowIndex, 31];
                    cell.Value = shipment.AddressInformation.Consignee.City;

                    cell = ws.Cells[rowIndex, 32];
                    cell.Value = shipment.AddressInformation.Consignee.State;

                    cell = ws.Cells[rowIndex, 33];
                    cell.Value = shipment.AddressInformation.Consignee.Country;

                    cell = ws.Cells[rowIndex, 34];
                    cell.Value = shipment.GeneralInformation.ShipmentCode;


                    ws.Row(rowIndex).Height = 25;
                }

                // Set width
                for (int i = 1; i < 35; i++)
                {
                    ws.Column(i).Width = 34;
                }

                return excel.GetAsByteArray();
            }
        }


        private byte[] GenerateExcelSheetForShipmentReport(List<ShipmentReportDto> shipmentReport)
        {
            using (ExcelPackage excel = new ExcelPackage())
            {
                //Create the worksheet
                ExcelWorksheet ws = excel.Workbook.Worksheets.Add("Shipments");

                //Merging cells and create a center heading for out table
                ws.Cells[2, 1].Value = "Shipment Details";
                ws.Cells[2, 1, 2, 8].Merge = true;
                ws.Cells[2, 1, 2, 8].Style.Font.Bold = true;
                ws.Cells[2, 1, 2, 8].Style.Font.Size = 15;
                ws.Cells[2, 1, 2, 8].Style.Font.Name = "Calibri";
                ws.Cells[2, 1, 2, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                // Set headings.
                ws.Cells["A6"].Value = "Shipment Code";
                ws.Cells["B6"].Value = "Shipment Mode";
                ws.Cells["C6"].Value = "Consignor FirstName";
                ws.Cells["D6"].Value = "Consignor LastName";
                ws.Cells["E6"].Value = "Consignor Postalcode";
                ws.Cells["F6"].Value = "Consignor Country";
                ws.Cells["G6"].Value = "Created Date";
                ws.Cells["H6"].Value = "Pickup Date";
                ws.Cells["I6"].Value = "Delivery Date";
                ws.Cells["J6"].Value = "Shipment Description";
                ws.Cells["K6"].Value = "Status";
                ws.Cells["L6"].Value = "Tracking Number";
                ws.Cells["M6"].Value = "CarrierName";
                ws.Cells["N6"].Value = "Consignee Country";
                ws.Cells["O6"].Value = "Consignee Postalcode";
                ws.Cells["P6"].Value = "Price";
                ws.Cells["Q6"].Value = "Currency";
                ws.Cells["R6"].Value = "Total Weight";
                ws.Cells["S6"].Value = "Total Volume";
                ws.Cells["T6"].Value = "Shipment TermCode";
                ws.Cells["U6"].Value = "Package Count";

                //Format the header for columns.
                using (ExcelRange rng = ws.Cells["A6:Z6"])
                {
                    rng.Style.Font.Bold = true;
                    rng.Style.Fill.PatternType = ExcelFillStyle.Solid;                      //Set Pattern for the background to Solid
                    rng.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(79, 129, 189));  //Set color to dark blue
                    rng.Style.Font.Color.SetColor(System.Drawing.Color.White);
                }

                //ws.Cells["A6:H6"].AutoFitColumns();

                // Set data.
                int rowIndex = 6;
                foreach (var shipment in shipmentReport) // Adding Data into rows
                {
                    rowIndex++;

                    var cell = ws.Cells[rowIndex, 1];
                    cell.Value = shipment.ShipmentCode;

                    cell = ws.Cells[rowIndex, 2];
                    cell.Value = shipment.ShipmentMode;

                    cell = ws.Cells[rowIndex, 3];
                    cell.Value = shipment.ConsignorFirstName;

                    cell = ws.Cells[rowIndex, 4];
                    cell.Value = shipment.ConsignorLastName;

                    cell = ws.Cells[rowIndex, 5];
                    cell.Value = shipment.ConsignorPostalcode;

                    cell = ws.Cells[rowIndex, 6];
                    cell.Value = shipment.ConsignorCountry;

                    cell = ws.Cells[rowIndex, 7];
                    cell.Value = shipment.CreatedDate;

                    cell = ws.Cells[rowIndex, 8];
                    cell.Value = context.GetLocalTimeByUser(shipment.UserId, Convert.ToDateTime(shipment.PickupDate));

                    cell = ws.Cells[rowIndex, 9];
                    cell.Value = shipment.DeliveryTime;

                    cell = ws.Cells[rowIndex, 10];
                    cell.Value = shipment.ShipmentDescription;

                    cell = ws.Cells[rowIndex, 11];
                    cell.Value = shipment.Status;

                    cell = ws.Cells[rowIndex, 12];
                    cell.Value = shipment.TrackingNumber;

                    cell = ws.Cells[rowIndex, 13];
                    cell.Value = shipment.CarrierName;

                    cell = ws.Cells[rowIndex, 14];
                    cell.Value = shipment.ConsigneeCountry;

                    cell = ws.Cells[rowIndex, 15];
                    cell.Value = shipment.ConsigneePostalcode;

                    cell = ws.Cells[rowIndex, 16];
                    cell.Value = shipment.Price;

                    cell = ws.Cells[rowIndex, 17];
                    cell.Value = shipment.ValueCurrency;

                    cell = ws.Cells[rowIndex, 18];
                    cell.Value = shipment.TotalVolume;

                    cell = ws.Cells[rowIndex, 19];
                    cell.Value = shipment.TotalWeight;

                    cell = ws.Cells[rowIndex, 20];
                    cell.Value = shipment.ShipmentTermCode;

                    cell = ws.Cells[rowIndex, 21];
                    cell.Value = shipment.Count;

                    ws.Row(rowIndex).Height = 25;
                }

                // Set width
                for (int i = 1; i < 22; i++)
                {
                    ws.Column(i).Width = 25;
                }

                return excel.GetAsByteArray();
            }
        }


        public List<ShipmentReportDto> ShipmentReport(string userId, short carrierId = 0, long companyId = 0, DateTime? startDate = null,
                                                      DateTime? endDate = null, short status = 0, string countryOfOrigin = null, string countryOfDestination = null, short product = 0, short packageType = 0)
        {

            List<ShipmentReportDto> reportList = new List<ShipmentReportDto>();

            //using (PIContext context = PIContext.Get())
            //{
            var roleId = context.Users.Where(u => u.Id == userId).FirstOrDefault().Roles.FirstOrDefault().RoleId;

            var roleName = context.Roles.Where(r => r.Id == roleId).FirstOrDefault().Name;

            IList<Data.Entity.Shipment> shipmentList = null;

            if (startDate.HasValue)
                startDate = startDate.Value.ToUniversalTime();
            if (endDate.HasValue)
                endDate = endDate.Value.ToUniversalTime();

            if (roleName == "Admin" || roleName == "BusinessOwner")
            {
                if (roleName == "BusinessOwner")
                {
                    companyId = context.GetCompanyByUserId(userId).Id;
                }

                shipmentList =
                    context.Shipments.Where(s =>
                    (companyId == 0 || s.Division.CompanyId == companyId) &&
                    (carrierId == 0 || s.CarrierId == carrierId) &&
                    (startDate == null || startDate <= s.PickUpDate) &&
                    (endDate == null || s.PickUpDate <= endDate) &&
                    (countryOfOrigin == null || s.ConsignorAddress.Country == countryOfOrigin) &&
                    (countryOfDestination == null || s.ConsigneeAddress.Country == countryOfDestination) &&
                    (product == 0 || s.ShipmentMode == (Contract.Enums.CarrierType)product) &&
                    (packageType == 0 || s.ShipmentPackage.PackageProducts.Any(p => p.ProductTypeId == packageType))
                ).ToList();
            }
            else if (roleName == "Manager")
            {
                shipmentList =
                    context.Shipments.Where(s => s.Division.UserInDivisions.Any(u => u.UserId == userId) &&
                    (carrierId == 0 || s.CarrierId == carrierId) &&
                    (startDate == null || startDate <= s.PickUpDate) &&
                    (endDate == null || s.PickUpDate <= endDate) &&
                    (countryOfOrigin == null || s.ConsignorAddress.Country == countryOfOrigin) &&
                    (countryOfDestination == null || s.ConsigneeAddress.Country == countryOfDestination) &&
                    (product == 0 || s.ShipmentMode == (Contract.Enums.CarrierType)product) &&
                    (packageType == 0 || s.ShipmentPackage.PackageProducts.Any(p => p.ProductTypeId == packageType))
                ).ToList();
            }

            // If empty list, return empty list by message result is empty.
            if (shipmentList == null || shipmentList.Count == 0)
                return reportList;

            // Update retrieved shipment list status from SIS.
            string environment = "";
            foreach (var shipment in shipmentList)
            {
                if (shipment.Status != ((short)ShipmentStatus.Delivered) && !string.IsNullOrWhiteSpace(shipment.TrackingNumber))
                {
                    environment = GetEnvironmentByTarrif(shipment.TariffText);

                    UpdateLocationHistory(shipment.Carrier.Name, shipment.TrackingNumber, shipment.ShipmentCode, environment, shipment.Id);
                }
            }

            var selectedShipmentId = shipmentList.Select(s => s.Id).ToList();
            // Get updated list again with filter status.
            var UpdatedShipmentList = context.Shipments.Where(x =>
                                    selectedShipmentId.Any(s => s == x.Id) &&
                                    (status == 0 || x.Status == status)
                                    ).ToList();

            // Get shipment data, delivery date, carrier details, customer data, address details, cost center details and division details.

            foreach (var item in UpdatedShipmentList)
            {
                reportList.Add(new ShipmentReportDto
                {


                    ConsignorAddress1 = item.ConsignorAddress.StreetAddress1,
                    ConsignorAddress2 = item.ConsignorAddress.StreetAddress2,
                    ConsignorPostalcode = item.ConsignorAddress.ZipCode,
                    ConsignorCity = item.ConsignorAddress.City,
                    ConsignorCountry = item.ConsignorAddress.Country,
                    ConsignorState = item.ConsignorAddress.State,
                    ConsignorFirstName = item.ConsignorAddress.FirstName,
                    ConsignorLastName = item.ConsignorAddress.LastName,
                    ConsignorContactName = item.ConsignorAddress.ContactName,
                    ConsignorContactNumber = item.ConsignorAddress.ContactName,
                    ConsignorEmail = item.ConsignorAddress.EmailAddress,
                    ConsignorNumber = item.ConsignorAddress.Number,

                    ConsigneeAddress1 = item.ConsigneeAddress.StreetAddress1,
                    ConsigneeAddress2 = item.ConsigneeAddress.StreetAddress2,
                    ConsigneePostalcode = item.ConsigneeAddress.ZipCode,
                    ConsigneeCity = item.ConsigneeAddress.City,
                    ConsigneeCountry = item.ConsigneeAddress.Country,
                    ConsigneeState = item.ConsigneeAddress.State,
                    ConsigneeFirstName = item.ConsigneeAddress.FirstName,
                    ConsigneeLastName = item.ConsigneeAddress.LastName,
                    ConsigneeContactName = item.ConsigneeAddress.ContactName,
                    ConsigneeContactNumber = item.ConsigneeAddress.ContactName,
                    ConsigneeEmail = item.ConsigneeAddress.EmailAddress,
                    ConsigneeNumber = item.ConsigneeAddress.Number,

                    //GeneralInformation 
                    CostCenterId = item.CostCenterId.GetValueOrDefault(),
                    DivisionId = item.DivisionId.GetValueOrDefault(),
                    ShipmentCode = item.ShipmentCode,
                    ShipmentMode = Enum.GetName(typeof(Contract.Enums.CarrierType), item.ShipmentMode),
                    ShipmentName = item.ShipmentName,
                    ShipmentServices = Utility.GetEnumDescription((ShipmentService)item.ShipmentService),
                    TrackingNumber = item.TrackingNumber,
                    CreatedDate = item.CreatedDate.ToString("dd MMM yyyy"),
                    Status = Utility.GetEnumDescription((ShipmentStatus)item.Status),

                    //Package Details
                    CmLBS = Convert.ToBoolean(item.ShipmentPackage.VolumeMetricId),
                    VolumeCMM = Convert.ToBoolean(item.ShipmentPackage.VolumeMetricId),
                    Count = item.ShipmentPackage.PackageProducts.Count,
                    DeclaredValue = item.ShipmentPackage.InsuranceDeclaredValue,
                    HsCode = item.ShipmentPackage.HSCode,
                    Instructions = item.ShipmentPackage.CarrierInstruction,
                    IsInsuared = item.ShipmentPackage.IsInsured.ToString(),
                    TotalVolume = item.ShipmentPackage.TotalVolume,
                    TotalWeight = item.ShipmentPackage.TotalWeight,
                    ValueCurrency = item.ShipmentPackage.Currency == null ? 1 : Convert.ToInt32(item.ShipmentPackage.Currency.Id),
                    PreferredCollectionDate = item.ShipmentPackage.CollectionDate.ToString(),
                    ProductIngredients = this.getPackageDetails(item.ShipmentPackage.PackageProducts),
                    ShipmentDescription = item.ShipmentPackage.PackageDescription,

                    //CarrierInformation                       
                    CarrierName = item.Carrier.Name,
                    serviceLevel = item.ServiceLevel,
                    PickupDate = item.PickUpDate.HasValue ? context.GetLocalTimeByUser(item.CreatedBy, item.PickUpDate.Value).ToString("dd MMM yyyy") : string.Empty,
                    DeliveryTime = item.ShipmentPackage.EstDeliveryDate == null ? null : DateTime.Parse(item.ShipmentPackage.EstDeliveryDate.ToString()).ToString("dd MMM yyyy")

                });
            }


            //}

            return reportList;
        }

        public byte[] ShipmentReportForExcel(string userId, short carrierId = 0, long companyId = 0, DateTime? startDate = null,
                                     DateTime? endDate = null, short status = 0, string countryOfOrigin = null, string countryOfDestination = null, short product = 0, short packageType = 0)
        {

            var shipments = ShipmentReport(userId, carrierId, companyId, startDate, endDate, status, countryOfOrigin, countryOfDestination, product, packageType);

            byte[] stream = this.GenerateExcelSheetForShipmentReport(shipments);
            return stream;

        }


        public List<CarrierDto> LoadAllCarriers()
        {
            List<CarrierDto> carriers = new List<CarrierDto>();

            //using (PIContext context = PIContext.Get())
            //{
            var carrierList = context.Carrier.ToList();

            carrierList.ForEach(x => carriers.Add(new CarrierDto { Id = x.Id, Name = x.Name }));
            //}

            return carriers;
        }


        /// <summary>
        /// Toggle Shipment Favourites
        /// </summary>
        /// <param name="shipment"></param>
        /// <returns></returns>
        public bool ToggleShipmentFavourites(ShipmentDto shipment)
        {
            //using (PIContext context = PIContext.Get())
            //{
            var existingShipment = context.Shipments
                               .Where(x => x.ShipmentCode == shipment.GeneralInformation.ShipmentCode).SingleOrDefault();

            existingShipment.IsFavourite = !shipment.GeneralInformation.IsFavourite;
            context.SaveChanges();

            return existingShipment.IsFavourite;
            //}
        }


        /// <summary>
        /// Toggle Shipment Favourites
        /// </summary>
        /// <param name="shipment"></param>
        /// <returns></returns>
        public DashboardShipments GetShipmentStatusCounts(string userId)
        {

            IList<DivisionDto> divisions = null;
            List<Data.Entity.Shipment> Shipments = new List<Data.Entity.Shipment>();
            DashboardShipments shipmentCounts = new DashboardShipments();

            // For Admins
            if (userId == null)
            {
                Shipments.AddRange(context.Shipments.ToList());
            }
            else
            {
                // For Customers
                string role = context.GetUserRoleById(userId);
                if (role == "BusinessOwner" || role == "Manager")
                {
                    divisions = this.GetAllDivisionsinCompany(userId);
                }
                else if (role == "Supervisor")
                {
                    divisions = companyManagment.GetAssignedDivisions(userId);
                }
                if (divisions.Count > 0)
                {
                    foreach (var item in divisions)
                    {
                        Shipments.AddRange(this.GetshipmentsByDivisionId(item.Id));
                    }
                }
                else
                {
                    Shipments.AddRange(this.GetshipmentsByUserId(userId));
                }
            }

            var allShipments = Shipments.Where(s => s.IsParent == false).ToList();

            shipmentCounts.DraftStatusCount = allShipments.Where(x => x.Status == (short)ShipmentStatus.Draft).Count();
            shipmentCounts.BookingConfStatusCount = allShipments.Where(x => x.Status == (short)ShipmentStatus.BookingConfirmation).Count();
            shipmentCounts.PickeduptatusCount = allShipments.Where(x => x.Status == (short)ShipmentStatus.Pickup).Count();
            shipmentCounts.InTransitStatusCount = allShipments.Where(x => x.Status == (short)ShipmentStatus.Transit).Count();
            shipmentCounts.OutForDeliveryStatusCount = allShipments.Where(x => x.Status == (short)ShipmentStatus.OutForDelivery).Count();
            shipmentCounts.ExceptionStatusCount = allShipments.Where(x => x.Status == (short)ShipmentStatus.Exception).Count();
            shipmentCounts.DeliveredStatusCount = allShipments.Where(x => x.Status == (short)ShipmentStatus.Delivered).Count();
            shipmentCounts.ErrorStatusCount = allShipments.Where(x => x.Status == (short)ShipmentStatus.Pending || x.Status == (short)ShipmentStatus.Error).Count();
            shipmentCounts.DeletedStatusCount = allShipments.Where(x => x.Status == (short)ShipmentStatus.Deleted).Count();
            shipmentCounts.allStatusCount = allShipments.Count();


            //var delayed = (from shipment in allShipments
            //               join package in context.ShipmentPackages on shipment.ShipmentPackageId equals package.Id
            //               join history in context.ShipmentLocationHistories on shipment.Id equals history.ShipmentId
            //               where shipment.Status != (short)ShipmentStatus.Delivered &&
            //               history.CreatedDate > package.EstDeliveryDate.Value &&
            //               !shipment.IsParent
            //               select shipment).Count();

            //shipmentCounts.DelayedStatusCount = delayed;

            return shipmentCounts;

        }


        /// <summary>
        /// Search shipments by Tracking number/ shipment Id
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public PagedList SearchShipmentsById(string number)
        {
            int page = 1;
            int pageSize = 10;
            var pagedRecord = new PagedList();

            pagedRecord.Content = new List<ShipmentDto>();

            //using (var context = PIContext.Get())
            //{
            var content = (from shipment in context.Shipments
                           where shipment.TrackingNumber == number || shipment.ShipmentCode == number
                           select shipment).ToList();

            // Update retrieve shipment list status from SIS.
            string environment = "";
            foreach (var shipment in content)
            {
                if (shipment.Status != ((short)ShipmentStatus.Delivered) && !string.IsNullOrWhiteSpace(shipment.TrackingNumber))
                {
                    environment = GetEnvironmentByTarrif(shipment.TariffText);

                    UpdateLocationHistory(shipment.Carrier.Name, shipment.TrackingNumber, shipment.ShipmentCode, environment, shipment.Id);
                }
            }

            foreach (var item in content)
            {
                pagedRecord.Content.Add(new ShipmentDto
                {
                    AddressInformation = new ConsignerAndConsigneeInformationDto
                    {
                        Consignee = new ConsigneeDto
                        {
                            Address1 = item.ConsigneeAddress.StreetAddress1,
                            Address2 = item.ConsigneeAddress.StreetAddress2,
                            Postalcode = item.ConsigneeAddress.ZipCode,
                            City = item.ConsigneeAddress.City,
                            Country = item.ConsigneeAddress.Country,
                            State = item.ConsigneeAddress.State,
                            FirstName = item.ConsigneeAddress.FirstName,
                            LastName = item.ConsigneeAddress.LastName,
                            ContactName = item.ConsigneeAddress.ContactName,
                            ContactNumber = item.ConsigneeAddress.PhoneNumber,
                            Email = item.ConsigneeAddress.EmailAddress,
                            Number = item.ConsigneeAddress.Number
                        },
                        Consigner = new ConsignerDto
                        {
                            Address1 = item.ConsignorAddress.StreetAddress1,
                            Address2 = item.ConsignorAddress.StreetAddress2,
                            Postalcode = item.ConsignorAddress.ZipCode,
                            City = item.ConsignorAddress.City,
                            Country = item.ConsignorAddress.Country,
                            State = item.ConsignorAddress.State,
                            FirstName = item.ConsignorAddress.FirstName,
                            LastName = item.ConsignorAddress.LastName,
                            ContactName = item.ConsignorAddress.ContactName,
                            ContactNumber = item.ConsignorAddress.PhoneNumber,
                            Email = item.ConsignorAddress.EmailAddress,
                            Number = item.ConsignorAddress.Number
                        }
                    },
                    GeneralInformation = new GeneralInformationDto
                    {
                        CostCenterId = item.CostCenterId.GetValueOrDefault(),
                        DivisionId = item.DivisionId.GetValueOrDefault(),
                        ShipmentCode = item.ShipmentCode,
                        ShipmentMode = item.ShipmentMode.ToString(),
                        ShipmentName = item.ShipmentName,
                        //ShipmentTermCode = item.ShipmentTermCode,
                        //ShipmentTypeCode = item.ShipmentTypeCode,
                        ShipmentId = item.Id.ToString(),
                        TrackingNumber = item.TrackingNumber,
                        CreatedDate = GetLocalTimeByUser(item.CreatedBy, item.CreatedDate).Value.ToString("dd MMM yyyy"),
                        Status = Utility.GetEnumDescription((ShipmentStatus)item.Status),
                        IsEnableEdit = true,
                        IsEnableDelete = true,
                        ShipmentLabelBLOBURL = getLabelforShipmentFromBlobStorage(item.Id, item.Division.Company.TenantId)
                    },
                    PackageDetails = new PackageDetailsDto
                    {
                        CmLBS = Convert.ToBoolean(item.ShipmentPackage.VolumeMetricId),
                        VolumeCMM = Convert.ToBoolean(item.ShipmentPackage.VolumeMetricId),
                        Count = item.ShipmentPackage.PackageProducts.Count,
                        DeclaredValue = item.ShipmentPackage.InsuranceDeclaredValue,
                        HsCode = item.ShipmentPackage.HSCode,
                        Instructions = item.ShipmentPackage.CarrierInstruction,
                        IsInsuared = item.ShipmentPackage.IsInsured.ToString(),
                        TotalVolume = item.ShipmentPackage.TotalVolume,
                        TotalWeight = item.ShipmentPackage.TotalWeight,
                        ValueCurrency = item.ShipmentPackage.Currency == null ? 1 : Convert.ToInt32(item.ShipmentPackage.Currency.Id),
                        PreferredCollectionDate = item.ShipmentPackage.CollectionDate.ToString(),
                        ProductIngredients = this.getPackageDetails(item.ShipmentPackage.PackageProducts),
                        ShipmentDescription = item.ShipmentPackage.PackageDescription

                    },
                    CarrierInformation = new CarrierInformationDto
                    {
                        CarrierName = item.Carrier.Name,
                        serviceLevel = item.ServiceLevel,
                        PickupDate = item.PickUpDate.HasValue ? (DateTime?)context.GetLocalTimeByUser(item.CreatedBy, item.PickUpDate.Value) : null
                    }

                });
            }

            pagedRecord.TotalRecords = content.Count();
            pagedRecord.CurrentPage = page;
            pagedRecord.PageSize = pageSize;
            pagedRecord.TotalPages = (int)Math.Ceiling((decimal)pagedRecord.TotalRecords / pagedRecord.PageSize);

            return pagedRecord;
            //}
        }


        private string GetEnvironmentByTarrif(string tarrifText)
        {
            string environment = string.Empty;

            //using (PIContext context = PIContext.Get())
            //{
            var tarrifTextCode = context.TarrifTextCodes.Where(t => t.TarrifText == tarrifText && t.IsActive && !t.IsDelete).FirstOrDefault();

            if (tarrifTextCode != null && tarrifTextCode.CountryCode == "NL")
                environment = "tale";
            else
                environment = "taleus";
            //   }

            return environment;
        }

        //get payment details by reference
        public PaymentDto GetPaymentbyReference(long reference)
        {
            PaymentDto paymentDetails = new PaymentDto();

            var payment = context.Payments.Where(t => t.ReferenceId == reference).FirstOrDefault();

            if (payment != null)
            {
                paymentDetails.Amount = payment.Amount.ToString();
            }

            return paymentDetails;
        }


        public ShipmentOperationResult PaymentCharge(PaymentDto payment)
        {
            OperationResult result;

            result = paymentManager.Charge(payment);

            // Added payment data
            var paymentEntity = new Payment();
            paymentEntity.CreatedBy = payment.UserId;
            paymentEntity.CreatedDate = DateTime.UtcNow;
            paymentEntity.IsActive = true;
            paymentEntity.PaymentId = result.FieldList["PaymentKey"];
            paymentEntity.Status = result.Status;
            paymentEntity.PaymentType = PaymentType.Shipment;
            paymentEntity.ReferenceId = payment.ShipmentId;
            paymentEntity.Amount = payment.ChargeAmount;
            paymentEntity.LocationId = result.FieldList["LocationId"];
            paymentEntity.TransactionId = result.FieldList["TransactionId"];
            paymentEntity.TenderId = result.FieldList["TenderId"];

            if (payment.CurrencyType == "USD")
            {
                paymentEntity.CurrencyType = CurrencyType.USD;
            }

            if (result.Status == Status.PaymentError)
            {
                // If failed, due to payment gateway error, then record payment error code.
                paymentEntity.StatusCode = result.FieldList["errorCode"];
            }

            context.Payments.Add(paymentEntity);
            context.SaveChanges();

            if (result.Status == Status.PaymentError)
            {
                return new ShipmentOperationResult()
                {
                    Status = Status.PaymentError,
                    Message = "Error occured when adding payment." + result.Message
                };
            }
            else
            {
                //ShipmentOperationResult shipmentResult = SendShipmentDetails(new SendShipmentDetailsDto()
                //{
                //    ShipmentId = payment.ShipmentId,
                //    PaymentResult = result,
                //    UserId = payment.UserId
                //});

                return new ShipmentOperationResult()
                {
                    Status = Status.Success,
                    Message = "Payment is succcess"
                };
            }

        }

        public OperationResult RefundCharge(long shipmentId)
        {
            Payment payment = context.Payments.Where(p => p.ReferenceId == shipmentId).FirstOrDefault();

            if (payment == null)
                return new OperationResult()
                {
                    Status = Status.PaymentError,
                    Message = "Couldn't find the Payment"
                };

            PaymentDto dto = new PaymentDto();
            dto.ChargeAmount = payment.Amount;
            dto.CurrencyType = payment.CurrencyType == CurrencyType.USD ? "USD" : "";
            dto.TenderId = payment.TenderId;
            dto.LocationId = payment.LocationId;
            dto.TransactionId = payment.TransactionId;

            OperationResult result = paymentManager.Refund(dto);

            if (result.Status == Status.Success)
            {
                payment.Status = Status.Refund;
                context.SaveChanges();
            }

            return result;
        }

        public DateTime? GetLocalTimeByUser(string loggedUserId, DateTime utcDatetime)
        {
            return context.GetLocalTimeByUser(loggedUserId, utcDatetime);
        }


        public DateTime GetSISTaleUSTime(DateTime datetime)
        {
            TimeZoneInfo cstZone = TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time");
            DateTime sisTime = TimeZoneInfo.ConvertTimeFromUtc(datetime, cstZone);

            return sisTime;
        }


        public DateTime GetUTCTimeFromSISTaleUS(DateTime datetime)
        {
            return TimeZoneInfo.ConvertTime(datetime, TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time"), TimeZoneInfo.Utc);
        }


        public ShipmentDto GetShipmentResult(long shipmentId)
        {
            ShipmentDto shipmentDto = new ShipmentDto();
            Shipment shipment = context.Shipments.Where(s => s.Id == shipmentId).FirstOrDefault();

            if (shipment.Status == (short)ShipmentStatus.Processing || shipment.Status == (short)ShipmentStatus.Draft)
            {
                shipmentDto.HasShipmentAdded = false;
                return shipmentDto;
            }

            shipmentDto.HasShipmentAdded = true;

            // get label
            ApplicationUser user = context.Users.Where(u => u.Id == shipment.CreatedBy).FirstOrDefault();
            if (user != null)
            {
                shipmentDto.LabelUrl = getLabelforShipmentFromBlobStorage(shipmentId, user.TenantId);
            }
            else
            {
                shipmentDto.LabelUrl = sisManager.GetLabel(shipment.ShipmentCode);
            }

            var invoice = context.Invoices.Where(s => s.Id == shipmentId).FirstOrDefault();
            shipmentDto.InvoiceUrl = invoice != null ? invoice.URL : "";

            return shipmentDto;
        }

        #region Temp solution

        public ShipmentOperationResult SaveShipmentV1(ShipmentDto addShipment)
        {
            ShipmentOperationResult result = new ShipmentOperationResult();
            OperationResult paymentResult = new OperationResult();

            if (addShipment.GeneralInformation.ShipmentPaymentTypeId == 2)
            {
                // Make online payment
                paymentResult = paymentManager.Charge(addShipment.PaymentDto);

                if (paymentResult.Status == Status.PaymentError)
                {
                    // Payment failt. No need to save shipment details on db.
                    result.Status = Status.PaymentError;
                    result.Message = "Payment failed. " + result.FieldList["errorCode"];

                    return result;
                }
            }


            if (addShipment.CarrierInformation.CarrierName!="USP")
            {

                Company currentcompany = context.GetCompanyByUserId(addShipment.UserId);
                long sysDivisionId = 0;
                long sysCostCenterId = 0;

                var packageProductList = new List<PackageProduct>();
                addShipment.PackageDetails.ProductIngredients.ForEach(p => packageProductList.Add(new PackageProduct()
                {
                    CreatedBy = addShipment.CreatedBy,
                    CreatedDate = DateTime.UtcNow,
                    IsActive = true,
                    IsDelete = false,
                    Description = p.Description,
                    Height = p.Height,
                    Length = p.Length,
                    Weight = p.Weight,
                    Width = p.Width,
                    Quantity = p.Quantity,
                    ProductTypeId = (short)Enum.Parse(typeof(ProductType), p.ProductType)
                }));


                // If division and costcenter Ids are 0, then assign default costcenter and division.
                if (addShipment.GeneralInformation.DivisionId == 0)
                {
                    var sysDivision = context.Divisions.Where(d => d.CompanyId == currentcompany.Id
                                                           && d.Type == "SYSTEM").SingleOrDefault();

                    sysDivisionId = sysDivision.Id;

                }
                if (addShipment.GeneralInformation.CostCenterId == 0)
                {
                    var defaultCostCntr = context.CostCenters.Where(c => c.CompanyId == currentcompany.Id
                                                                                && c.Type == "SYSTEM").SingleOrDefault();
                    sysCostCenterId = defaultCostCntr.Id;
                }

                long oldShipmentId = 0;//= Int64.Parse(addShipment.GeneralInformation.ShipmentCode);

                if (addShipment.GeneralInformation.ShipmentCode != "0")
                {
                    // If has parent shipment id, then add to previous shipment.
                    Data.Entity.Shipment oldShipment = context.Shipments.Where(sh => sh.ShipmentCode == addShipment.GeneralInformation.ShipmentCode).FirstOrDefault();
                    oldShipmentId = oldShipment.Id;
                    oldShipment.IsParent = true;
                    context.SaveChanges();
                }

                //Mapper.CreateMap<GeneralInformationDto, Shipment>();
                Shipment newShipment = new Shipment
                {
                    ShipmentName = addShipment.GeneralInformation.ShipmentName,
                    ShipmentReferenceName = addShipment.GeneralInformation.ShipmentName + "-" + DateTime.UtcNow.ToString("yyyyMMddHHmmssfff"),
                    ShipmentCode = null, //addShipmentResponse.CodeShipment,
                    DivisionId = addShipment.GeneralInformation.DivisionId == 0 ? sysDivisionId : (long?)addShipment.GeneralInformation.DivisionId,
                    CostCenterId = addShipment.GeneralInformation.CostCenterId == 0 ? sysCostCenterId : (long?)addShipment.GeneralInformation.CostCenterId,
                    ShipmentMode = (Contract.Enums.CarrierType)Enum.Parse(typeof(Contract.Enums.CarrierType), addShipment.GeneralInformation.ShipmentMode, true),
                    ShipmentService = (short)Utility.GetValueFromDescription<ShipmentService>(addShipment.GeneralInformation.ShipmentServices),
                    Carrier = context.Carrier.Where(c => c.Name == addShipment.CarrierInformation.CarrierName).FirstOrDefault(),
                    TrackingNumber = null, //addShipmentResponse.Awb,
                    CreatedBy = addShipment.CreatedBy,
                    CreatedDate = DateTime.UtcNow,
                    ServiceLevel = addShipment.CarrierInformation.serviceLevel,
                    TarriffType = addShipment.CarrierInformation.tarriffType,
                    TariffText = addShipment.CarrierInformation.tariffText,
                    CarrierDescription = addShipment.CarrierInformation.description,
                    ShipmentPaymentTypeId = addShipment.GeneralInformation.ShipmentPaymentTypeId,
                    Status = (short)ShipmentStatus.Draft,   // When initial save, set Draft.If user close the browser, shipment will remain as Draft mode.
                    PickUpDate = addShipment.CarrierInformation.PickupDate == null ? null : (DateTime?)addShipment.CarrierInformation.PickupDate.Value.ToUniversalTime(),

                    IsActive = true,
                    IsParent = false,
                    ParentShipmentId = oldShipmentId == 0 ? null : (long?)oldShipmentId,
                    ConsigneeAddress = new ShipmentAddress
                    {
                        CompanyName = addShipment.AddressInformation.Consignee.CompanyName,
                        FirstName = addShipment.AddressInformation.Consignee.FirstName,
                        LastName = addShipment.AddressInformation.Consignee.LastName,
                        Country = addShipment.AddressInformation.Consignee.Country,
                        ZipCode = addShipment.AddressInformation.Consignee.Postalcode,
                        Number = addShipment.AddressInformation.Consignee.Number,
                        StreetAddress1 = addShipment.AddressInformation.Consignee.Address1,
                        StreetAddress2 = addShipment.AddressInformation.Consignee.Address2,
                        City = addShipment.AddressInformation.Consignee.City,
                        State = addShipment.AddressInformation.Consignee.State,
                        EmailAddress = addShipment.AddressInformation.Consignee.Email,
                        PhoneNumber = addShipment.AddressInformation.Consignee.ContactNumber,
                        ContactName = addShipment.AddressInformation.Consignee.FirstName + " " + addShipment.AddressInformation.Consignee.LastName,
                        IsActive = true,
                        CreatedBy = addShipment.CreatedBy,
                        CreatedDate = DateTime.UtcNow
                    },
                    ConsignorAddress = new ShipmentAddress
                    {
                        CompanyName = addShipment.AddressInformation.Consigner.CompanyName,
                        FirstName = addShipment.AddressInformation.Consigner.FirstName,
                        LastName = addShipment.AddressInformation.Consigner.LastName,
                        Country = addShipment.AddressInformation.Consigner.Country,
                        ZipCode = addShipment.AddressInformation.Consigner.Postalcode,
                        Number = addShipment.AddressInformation.Consigner.Number,
                        StreetAddress1 = addShipment.AddressInformation.Consigner.Address1,
                        StreetAddress2 = addShipment.AddressInformation.Consigner.Address2,
                        City = addShipment.AddressInformation.Consigner.City,
                        State = addShipment.AddressInformation.Consigner.State,
                        EmailAddress = addShipment.AddressInformation.Consigner.Email,
                        PhoneNumber = addShipment.AddressInformation.Consigner.ContactNumber,
                        ContactName = addShipment.AddressInformation.Consigner.FirstName + " " + addShipment.AddressInformation.Consigner.LastName,
                        IsActive = true,
                        CreatedBy = addShipment.CreatedBy,
                        CreatedDate = DateTime.UtcNow
                    },
                    ShipmentPackage = new ShipmentPackage()
                    {
                        PackageDescription = addShipment.PackageDetails.ShipmentDescription,
                        TotalVolume = addShipment.PackageDetails.TotalVolume,
                        TotalWeight = addShipment.PackageDetails.TotalWeight,
                        HSCode = addShipment.PackageDetails.HsCode,
                        CollectionDate = DateTime.Parse(addShipment.PackageDetails.PreferredCollectionDate),
                        CarrierInstruction = addShipment.PackageDetails.Instructions,
                        IsInsured = Convert.ToBoolean(addShipment.PackageDetails.IsInsuared),
                        InsuranceDeclaredValue = addShipment.PackageDetails.DeclaredValue,
                        InsuranceCurrencyType = (short)addShipment.PackageDetails.ValueCurrency,
                        CarrierCost = addShipment.CarrierInformation.Price,
                        InsuranceCost = addShipment.CarrierInformation.Insurance,
                        PaymentTypeId = addShipment.PackageDetails.PaymentTypeId,
                        EarliestPickupDate = addShipment.CarrierInformation.PickupDate == null ? null : (DateTime?)addShipment.CarrierInformation.PickupDate.Value.ToUniversalTime(),
                        EstDeliveryDate = addShipment.CarrierInformation.DeliveryTime ?? null,
                        WeightMetricId = addShipment.PackageDetails.CmLBS ? (short)1 : (short)2,
                        VolumeMetricId = addShipment.PackageDetails.VolumeCMM ? (short)1 : (short)2,
                        IsActive = true,
                        CreatedBy = addShipment.CreatedBy,
                        CreatedDate = DateTime.UtcNow,
                        PackageProducts = packageProductList,
                        IsDG = addShipment.PackageDetails.IsDG,
                        Accessibility = addShipment.PackageDetails.IsDG == true ? addShipment.PackageDetails.Accessibility : false,
                        DGType = addShipment.PackageDetails.IsDG == true ? addShipment.PackageDetails.DGType : null,

                    }
                };

                //save consigner details as new address book detail
                if (addShipment.AddressInformation.Consigner.SaveNewAddress)
                {
                    AddressBook ConsignerAddressBook = new AddressBook
                    {
                        CompanyName = addShipment.AddressInformation.Consigner.CompanyName,
                        FirstName = addShipment.AddressInformation.Consigner.FirstName,
                        LastName = addShipment.AddressInformation.Consigner.LastName,
                        Country = addShipment.AddressInformation.Consigner.Country,
                        ZipCode = addShipment.AddressInformation.Consigner.Postalcode,
                        Number = addShipment.AddressInformation.Consigner.Number,
                        StreetAddress1 = addShipment.AddressInformation.Consigner.Address1,
                        StreetAddress2 = addShipment.AddressInformation.Consigner.Address2,
                        City = addShipment.AddressInformation.Consigner.City,
                        State = addShipment.AddressInformation.Consigner.State,
                        EmailAddress = addShipment.AddressInformation.Consigner.Email,
                        PhoneNumber = addShipment.AddressInformation.Consigner.ContactNumber,
                        IsActive = true,
                        CreatedBy = addShipment.CreatedBy,
                        UserId = addShipment.UserId,
                        CreatedDate = DateTime.UtcNow
                    };
                    context.AddressBooks.Add(ConsignerAddressBook);

                }

                //save consignee details as new address book detail
                if (addShipment.AddressInformation.Consignee.SaveNewAddress)
                {
                    AddressBook ConsignerAddressBook = new AddressBook
                    {
                        CompanyName = addShipment.AddressInformation.Consignee.CompanyName,
                        FirstName = addShipment.AddressInformation.Consignee.FirstName,
                        LastName = addShipment.AddressInformation.Consignee.LastName,
                        Country = addShipment.AddressInformation.Consignee.Country,
                        ZipCode = addShipment.AddressInformation.Consignee.Postalcode,
                        Number = addShipment.AddressInformation.Consignee.Number,
                        StreetAddress1 = addShipment.AddressInformation.Consignee.Address1,
                        StreetAddress2 = addShipment.AddressInformation.Consignee.Address2,
                        City = addShipment.AddressInformation.Consignee.City,
                        State = addShipment.AddressInformation.Consignee.State,
                        EmailAddress = addShipment.AddressInformation.Consignee.Email,
                        PhoneNumber = addShipment.AddressInformation.Consignee.ContactNumber,
                        IsActive = true,
                        CreatedBy = addShipment.CreatedBy,
                        UserId = addShipment.UserId,
                        CreatedDate = DateTime.UtcNow
                    };
                    context.AddressBooks.Add(ConsignerAddressBook);

                }

                // Save shipment
                context.Shipments.Add(newShipment);
                context.SaveChanges();

                // Save payment. If come so far, mean payment is success.
                if (addShipment.GeneralInformation.ShipmentPaymentTypeId == 2)
                {
                    var paymentEntity = new Payment();
                    paymentEntity.CreatedBy = addShipment.UserId;
                    paymentEntity.CreatedDate = DateTime.UtcNow;
                    paymentEntity.IsActive = true;
                    paymentEntity.PaymentId = paymentResult.FieldList["PaymentKey"];
                    paymentEntity.Status = paymentResult.Status;
                    paymentEntity.PaymentType = PaymentType.Shipment;
                    paymentEntity.ReferenceId = newShipment.Id;
                    paymentEntity.Amount = addShipment.PaymentDto.ChargeAmount;
                    paymentEntity.LocationId = paymentResult.FieldList["LocationId"];
                    paymentEntity.TransactionId = paymentResult.FieldList["TransactionId"];
                    paymentEntity.TenderId = paymentResult.FieldList["TenderId"];

                    if (addShipment.PaymentDto.CurrencyType == "USD")
                    {
                        paymentEntity.CurrencyType = CurrencyType.USD;
                    }

                    context.Payments.Add(paymentEntity);
                    context.SaveChanges();
                }

                result.ShipmentId = newShipment.Id;
                result.Status = Status.Success;

                //Add Audit Trail Record
                context.AuditTrail.Add(new AuditTrail
                {
                    ReferenceId = newShipment.Id.ToString(),
                    AppFunctionality = (addShipment.GeneralInformation.ShipmentCode != "0") ?
                                        AppFunctionality.EditShipment : AppFunctionality.AddShipment,
                    Result = result.Status.ToString(),
                    CreatedBy = "1",
                    CreatedDate = DateTime.UtcNow
                });
                context.SaveChanges();

                //if (!addShipment.isSaveAsDraft && (result.Status == Status.Success))
                //{
                //    //// set shipment id, bcoz required in sendshipmentdetails method.
                //    //addShipment.GeneralInformation.ShipmentId = newShipment.Id.ToString();

                //    //// We required custom shipmentdto, so need to get it back. Later need to change this.
                //    //ShipmentDto shDto = GetShipmentDtoForSIS(newShipment.Id);

                //    //var response = sisManager.SendShipmentDetails(shDto);

                //    //newShipment.Status = (short)ShipmentStatus.Processing;
                //    //context.SaveChanges();

                //    SendShipmentDetails(new SendShipmentDetailsDto()
                //    {
                //        ShipmentId = newShipment.Id
                //    });
                //}

                // Need to confirm user, if did payment if it is not success or if happen any issue when save in db. So return the status of those and if those success, 
                // then browser will call service method auto without the interaction of user.
                return result;

            }
            else
            {

                Company currentcompany = context.GetCompanyByUserId(addShipment.UserId);
                long sysDivisionId = 0;
                long sysCostCenterId = 0;
                List<PackageProduct> packageProductList = null;
                long mainShipmentId = 0;


                int packageCount = 0;

                foreach (var package in addShipment.PackageDetails.ProductIngredients)
                {
                    packageProductList = new List<PackageProduct>();
                    packageProductList.Add(new PackageProduct()
                    {
                        CreatedBy = addShipment.CreatedBy,
                        CreatedDate = DateTime.UtcNow,
                        IsActive = true,
                        IsDelete = false,
                        Description = package.Description,
                        Height = package.Height,
                        Length = package.Length,
                        Weight = package.Weight,
                        Width = package.Width,
                        Quantity = package.Quantity,
                        ProductTypeId = (short)Enum.Parse(typeof(ProductType), package.ProductType)
                    });


                    // If division and costcenter Ids are 0, then assign default costcenter and division.
                    if (addShipment.GeneralInformation.DivisionId == 0)
                    {
                        var sysDivision = context.Divisions.Where(d => d.CompanyId == currentcompany.Id
                                                               && d.Type == "SYSTEM").SingleOrDefault();

                        sysDivisionId = sysDivision.Id;

                    }
                    if (addShipment.GeneralInformation.CostCenterId == 0)
                    {
                        var defaultCostCntr = context.CostCenters.Where(c => c.CompanyId == currentcompany.Id
                                                                                    && c.Type == "SYSTEM").SingleOrDefault();
                        sysCostCenterId = defaultCostCntr.Id;
                    }

                    long oldShipmentId = 0;//= Int64.Parse(addShipment.GeneralInformation.ShipmentCode);

                    if (addShipment.GeneralInformation.ShipmentCode != "0")
                    {
                        // If has parent shipment id, then add to previous shipment.
                        Data.Entity.Shipment oldShipment = context.Shipments.Where(sh => sh.ShipmentCode == addShipment.GeneralInformation.ShipmentCode).FirstOrDefault();
                        oldShipmentId = oldShipment.Id;
                        oldShipment.IsParent = true;
                        context.SaveChanges();
                    }

                    Shipment newShipment = new Shipment
                    {
                        ShipmentName = addShipment.GeneralInformation.ShipmentName,
                        ShipmentReferenceName = addShipment.GeneralInformation.ShipmentName + "-" + DateTime.UtcNow.ToString("yyyyMMddHHmmssfff"),
                        ShipmentCode = null, //addShipmentResponse.CodeShipment,
                        DivisionId = addShipment.GeneralInformation.DivisionId == 0 ? sysDivisionId : (long?)addShipment.GeneralInformation.DivisionId,
                        CostCenterId = addShipment.GeneralInformation.CostCenterId == 0 ? sysCostCenterId : (long?)addShipment.GeneralInformation.CostCenterId,
                        ShipmentMode = (Contract.Enums.CarrierType)Enum.Parse(typeof(Contract.Enums.CarrierType), addShipment.GeneralInformation.ShipmentMode, true),
                        ShipmentService = (short)Utility.GetValueFromDescription<ShipmentService>(addShipment.GeneralInformation.ShipmentServices),
                        Carrier = context.Carrier.Where(c => c.Name == addShipment.CarrierInformation.CarrierName).FirstOrDefault(),
                        TrackingNumber = null, //addShipmentResponse.Awb,
                        CreatedBy = addShipment.CreatedBy,
                        CreatedDate = DateTime.UtcNow,
                        ServiceLevel = addShipment.CarrierInformation.serviceLevel,
                        TarriffType = addShipment.CarrierInformation.tarriffType,
                        TariffText = addShipment.CarrierInformation.tariffText,
                        CarrierDescription = addShipment.CarrierInformation.description,
                        ShipmentPaymentTypeId = addShipment.GeneralInformation.ShipmentPaymentTypeId,
                        Status = (short)ShipmentStatus.Draft,   // When initial save, set Draft.If user close the browser, shipment will remain as Draft mode.
                        PickUpDate = addShipment.CarrierInformation.PickupDate == null ? null : (DateTime?)addShipment.CarrierInformation.PickupDate.Value.ToUniversalTime(),
                        MainShipment=mainShipmentId,
                        IsActive = true,
                        IsParent = false,
                        ParentShipmentId = oldShipmentId == 0 ? null : (long?)oldShipmentId,
                        ConsigneeAddress = new ShipmentAddress
                        {
                            CompanyName = addShipment.AddressInformation.Consignee.CompanyName,
                            FirstName = addShipment.AddressInformation.Consignee.FirstName,
                            LastName = addShipment.AddressInformation.Consignee.LastName,
                            Country = addShipment.AddressInformation.Consignee.Country,
                            ZipCode = addShipment.AddressInformation.Consignee.Postalcode,
                            Number = addShipment.AddressInformation.Consignee.Number,
                            StreetAddress1 = addShipment.AddressInformation.Consignee.Address1,
                            StreetAddress2 = addShipment.AddressInformation.Consignee.Address2,
                            City = addShipment.AddressInformation.Consignee.City,
                            State = addShipment.AddressInformation.Consignee.State,
                            EmailAddress = addShipment.AddressInformation.Consignee.Email,
                            PhoneNumber = addShipment.AddressInformation.Consignee.ContactNumber,
                            ContactName = addShipment.AddressInformation.Consignee.FirstName + " " + addShipment.AddressInformation.Consignee.LastName,
                            IsActive = true,
                            CreatedBy = addShipment.CreatedBy,
                            CreatedDate = DateTime.UtcNow
                        },
                        ConsignorAddress = new ShipmentAddress
                        {
                            CompanyName = addShipment.AddressInformation.Consigner.CompanyName,
                            FirstName = addShipment.AddressInformation.Consigner.FirstName,
                            LastName = addShipment.AddressInformation.Consigner.LastName,
                            Country = addShipment.AddressInformation.Consigner.Country,
                            ZipCode = addShipment.AddressInformation.Consigner.Postalcode,
                            Number = addShipment.AddressInformation.Consigner.Number,
                            StreetAddress1 = addShipment.AddressInformation.Consigner.Address1,
                            StreetAddress2 = addShipment.AddressInformation.Consigner.Address2,
                            City = addShipment.AddressInformation.Consigner.City,
                            State = addShipment.AddressInformation.Consigner.State,
                            EmailAddress = addShipment.AddressInformation.Consigner.Email,
                            PhoneNumber = addShipment.AddressInformation.Consigner.ContactNumber,
                            ContactName = addShipment.AddressInformation.Consigner.FirstName + " " + addShipment.AddressInformation.Consigner.LastName,
                            IsActive = true,
                            CreatedBy = addShipment.CreatedBy,
                            CreatedDate = DateTime.UtcNow
                        },
                        ShipmentPackage = new ShipmentPackage()
                        {
                            PackageDescription = addShipment.PackageDetails.ShipmentDescription,
                            TotalVolume = addShipment.PackageDetails.TotalVolume,
                            TotalWeight = addShipment.PackageDetails.TotalWeight,
                            HSCode = addShipment.PackageDetails.HsCode,
                            CollectionDate = DateTime.Parse(addShipment.PackageDetails.PreferredCollectionDate),
                            CarrierInstruction = addShipment.PackageDetails.Instructions,
                            IsInsured = Convert.ToBoolean(addShipment.PackageDetails.IsInsuared),
                            InsuranceDeclaredValue = addShipment.PackageDetails.DeclaredValue,
                            InsuranceCurrencyType = (short)addShipment.PackageDetails.ValueCurrency,
                            CarrierCost = addShipment.CarrierInformation.Price,
                            InsuranceCost = addShipment.CarrierInformation.Insurance,
                            PaymentTypeId = addShipment.PackageDetails.PaymentTypeId,
                            EarliestPickupDate = addShipment.CarrierInformation.PickupDate == null ? null : (DateTime?)addShipment.CarrierInformation.PickupDate.Value.ToUniversalTime(),
                            EstDeliveryDate = addShipment.CarrierInformation.DeliveryTime ?? null,
                            WeightMetricId = addShipment.PackageDetails.CmLBS ? (short)1 : (short)2,
                            VolumeMetricId = addShipment.PackageDetails.VolumeCMM ? (short)1 : (short)2,
                            IsActive = true,
                            CreatedBy = addShipment.CreatedBy,
                            CreatedDate = DateTime.UtcNow,
                            PackageProducts = packageProductList,
                            IsDG = addShipment.PackageDetails.IsDG,
                            Accessibility = addShipment.PackageDetails.IsDG == true ? addShipment.PackageDetails.Accessibility : false,
                            DGType = addShipment.PackageDetails.IsDG == true ? addShipment.PackageDetails.DGType : null,

                        }
                    };
                   
                    context.Shipments.Add(newShipment);
                    context.SaveChanges();
                    //check the package count to track the first shipment to save as Main shipment     
                    if (packageCount == 0)
                 {
                        
                    if (addShipment.AddressInformation.Consigner.SaveNewAddress)
                    {
                        AddressBook ConsignerAddressBook = new AddressBook
                        {
                            CompanyName = addShipment.AddressInformation.Consigner.CompanyName,
                            FirstName = addShipment.AddressInformation.Consigner.FirstName,
                            LastName = addShipment.AddressInformation.Consigner.LastName,
                            Country = addShipment.AddressInformation.Consigner.Country,
                            ZipCode = addShipment.AddressInformation.Consigner.Postalcode,
                            Number = addShipment.AddressInformation.Consigner.Number,
                            StreetAddress1 = addShipment.AddressInformation.Consigner.Address1,
                            StreetAddress2 = addShipment.AddressInformation.Consigner.Address2,
                            City = addShipment.AddressInformation.Consigner.City,
                            State = addShipment.AddressInformation.Consigner.State,
                            EmailAddress = addShipment.AddressInformation.Consigner.Email,
                            PhoneNumber = addShipment.AddressInformation.Consigner.ContactNumber,
                            IsActive = true,
                            CreatedBy = addShipment.CreatedBy,
                            UserId = addShipment.UserId,
                            CreatedDate = DateTime.UtcNow
                        };
                        context.AddressBooks.Add(ConsignerAddressBook);

                    }

                    //save consignee details as new address book detail
                    if (addShipment.AddressInformation.Consignee.SaveNewAddress)
                    {
                        AddressBook ConsignerAddressBook = new AddressBook
                        {
                            CompanyName = addShipment.AddressInformation.Consignee.CompanyName,
                            FirstName = addShipment.AddressInformation.Consignee.FirstName,
                            LastName = addShipment.AddressInformation.Consignee.LastName,
                            Country = addShipment.AddressInformation.Consignee.Country,
                            ZipCode = addShipment.AddressInformation.Consignee.Postalcode,
                            Number = addShipment.AddressInformation.Consignee.Number,
                            StreetAddress1 = addShipment.AddressInformation.Consignee.Address1,
                            StreetAddress2 = addShipment.AddressInformation.Consignee.Address2,
                            City = addShipment.AddressInformation.Consignee.City,
                            State = addShipment.AddressInformation.Consignee.State,
                            EmailAddress = addShipment.AddressInformation.Consignee.Email,
                            PhoneNumber = addShipment.AddressInformation.Consignee.ContactNumber,
                            IsActive = true,
                            CreatedBy = addShipment.CreatedBy,
                            UserId = addShipment.UserId,
                            CreatedDate = DateTime.UtcNow
                        };
                        context.AddressBooks.Add(ConsignerAddressBook);

                    }                 

                    
                        // Save payment. If come so far, mean payment is success.
                        if (addShipment.GeneralInformation.ShipmentPaymentTypeId == 2)
                        {
                            var paymentEntity = new Payment();
                            paymentEntity.CreatedBy = addShipment.UserId;
                            paymentEntity.CreatedDate = DateTime.UtcNow;
                            paymentEntity.IsActive = true;
                            paymentEntity.PaymentId = paymentResult.FieldList["PaymentKey"];
                            paymentEntity.Status = paymentResult.Status;
                            paymentEntity.PaymentType = PaymentType.Shipment;
                            paymentEntity.ReferenceId = newShipment.Id;
                            paymentEntity.Amount = addShipment.PaymentDto.ChargeAmount;
                            paymentEntity.LocationId = paymentResult.FieldList["LocationId"];
                            paymentEntity.TransactionId = paymentResult.FieldList["TransactionId"];
                            paymentEntity.TenderId = paymentResult.FieldList["TenderId"];

                            if (addShipment.PaymentDto.CurrencyType == "USD")
                            {
                                paymentEntity.CurrencyType = CurrencyType.USD;
                            }

                            context.Payments.Add(paymentEntity);
                            context.SaveChanges();
                        }
                        context.SaveChanges(); // get shipment id.
                        mainShipmentId =newShipment.Id;
                    }


                    // result.ShipmentId = newShipment.Id;

                    result.ShipmentId = mainShipmentId;
                    result.Status = Status.Success;

                    //Add Audit Trail Record
                    context.AuditTrail.Add(new AuditTrail
                    {
                        ReferenceId = newShipment.Id.ToString(),
                        AppFunctionality = (addShipment.GeneralInformation.ShipmentCode != "0") ?
                                            AppFunctionality.EditShipment : AppFunctionality.AddShipment,
                        Result = result.Status.ToString(),
                        CreatedBy = "1",
                        CreatedDate = DateTime.UtcNow
                    });
                   // context.SaveChanges();
                    packageCount++;
                    //end of USPS Add shipment
                    context.SaveChanges();
                }
                context.SaveChanges();

                //Mapper.CreateMap<GeneralInformationDto, Shipment>();


                //save consigner details as new address book detail              

                //if (!addShipment.isSaveAsDraft && (result.Status == Status.Success))
                //{
                //    //// set shipment id, bcoz required in sendshipmentdetails method.
                //    //addShipment.GeneralInformation.ShipmentId = newShipment.Id.ToString();

                //    //// We required custom shipmentdto, so need to get it back. Later need to change this.
                //    //ShipmentDto shDto = GetShipmentDtoForSIS(newShipment.Id);

                //    //var response = sisManager.SendShipmentDetails(shDto);

                //    //newShipment.Status = (short)ShipmentStatus.Processing;
                //    //context.SaveChanges();

                //    SendShipmentDetails(new SendShipmentDetailsDto()
                //    {
                //        ShipmentId = newShipment.Id
                //    });
                //}

                // Need to confirm user, if did payment if it is not success or if happen any issue when save in db. So return the status of those and if those success, 
                // then browser will call service method auto without the interaction of user.
                return result;

            }
            
        }

        public List<ShipmentOperationResult> SendShipmentDetailsV1(SendShipmentDetailsDto sendShipmentDetails)
        {
            // Get data from database.
            List<ShipmentOperationResult> operationList = new List<ShipmentOperationResult>();
            Shipment shipment = context.Shipments.Where(sh => sh.Id == sendShipmentDetails.ShipmentId).FirstOrDefault();
            List<Shipment> subShipmentList = new List<Shipment>();

            //adding main shipment to the list
            subShipmentList.Add(shipment);

            //adding sub shipments
            var subShipments = context.Shipments.Where(sb => sb.MainShipment == shipment.Id).ToList();
            if (subShipments!=null)
            {
                subShipmentList.AddRange(subShipments);
            }
           
            foreach (var shipmentItem in subShipmentList)
            {
                //get shipment again to avoid context update error
                Shipment currentShipment = context.Shipments.Where(sh => sh.Id == shipmentItem.Id).SingleOrDefault();


                var shipmentProductIngredientsList = new List<ProductIngredientsDto>();
                currentShipment.ShipmentPackage.PackageProducts.ToList().ForEach(p => shipmentProductIngredientsList.Add(new ProductIngredientsDto()
                {
                    Description = p.Description,
                    Height = p.Height,
                    Length = p.Length,
                    Weight = p.Weight,
                    Width = p.Width,
                    Quantity = p.Quantity,
                    ProductType = Utility.GetEnumDescription((ProductType)p.ProductTypeId)
                }));

                //Build Shipment Dto
                #region Build ShipmentDto

                ShipmentDto shipmentDto = new ShipmentDto()
                {
                    GeneralInformation = new GeneralInformationDto()
                    {
                        ShipmentId = currentShipment.Id.ToString(),
                        ShipmentName = currentShipment.ShipmentName,
                        ShipmentReferenceName = currentShipment.ShipmentReferenceName,
                        ShipmentServices = Utility.GetEnumDescription((ShipmentService)currentShipment.ShipmentService),
                        shipmentModeName = Utility.GetEnumDescription(currentShipment.ShipmentMode),
                        ShipmentPaymentTypeId = currentShipment.ShipmentPaymentTypeId,
                        CreatedUser = currentShipment.CreatedBy,
                        CreatedBy = currentShipment.CreatedBy,
                        CreatedDate = currentShipment.CreatedDate.ToString("MM/dd/yyyy")
                    },
                    CarrierInformation = new CarrierInformationDto()
                    {
                        CarrierName = currentShipment.Carrier.Name,
                        serviceLevel = currentShipment.ServiceLevel,
                        Price = currentShipment.ShipmentPackage.CarrierCost,
                        Insurance = currentShipment.ShipmentPackage.InsuranceCost,
                        tarriffType = currentShipment.TarriffType,
                        tariffText = currentShipment.TariffText,
                        description = currentShipment.CarrierDescription

                    },
                    AddressInformation = new ConsignerAndConsigneeInformationDto()
                    {
                        Consignee = new ConsigneeDto()
                        {
                            FirstName = currentShipment.ConsigneeAddress.FirstName,
                            LastName = currentShipment.ConsigneeAddress.LastName,
                            Country = currentShipment.ConsigneeAddress.Country,
                            Postalcode = currentShipment.ConsigneeAddress.ZipCode,
                            Number = currentShipment.ConsigneeAddress.Number,
                            Address1 = currentShipment.ConsigneeAddress.StreetAddress1,
                            Address2 = currentShipment.ConsigneeAddress.StreetAddress2,
                            City = currentShipment.ConsigneeAddress.City,
                            State = currentShipment.ConsigneeAddress.State,
                            Email = currentShipment.ConsigneeAddress.EmailAddress,
                            ContactNumber = currentShipment.ConsigneeAddress.PhoneNumber,
                            ContactName = currentShipment.ConsigneeAddress.ContactName
                        },
                        Consigner = new ConsignerDto()
                        {
                            FirstName = currentShipment.ConsignorAddress.FirstName,
                            LastName = currentShipment.ConsignorAddress.LastName,
                            Country = currentShipment.ConsignorAddress.Country,
                            Postalcode = currentShipment.ConsignorAddress.ZipCode,
                            Number = currentShipment.ConsignorAddress.Number,
                            Address1 = currentShipment.ConsignorAddress.StreetAddress1,
                            Address2 = currentShipment.ConsignorAddress.StreetAddress2,
                            City = currentShipment.ConsignorAddress.City,
                            State = currentShipment.ConsignorAddress.State,
                            Email = currentShipment.ConsignorAddress.EmailAddress,
                            ContactNumber = currentShipment.ConsignorAddress.PhoneNumber,
                            ContactName = currentShipment.ConsignorAddress.ContactName
                        }
                    },
                    PackageDetails = new PackageDetailsDto()
                    {
                        IsInsuared = currentShipment.ShipmentPackage.IsInsured.ToString().ToLower(),
                        ValueCurrency = currentShipment.ShipmentPackage.InsuranceCurrencyType,
                        ValueCurrencyString = Utility.GetEnumDescription((CurrencyType)currentShipment.ShipmentPackage.InsuranceCurrencyType),
                        PreferredCollectionDate = string.Format("{0}-{1}-{2}", currentShipment.ShipmentPackage.CollectionDate.Day, currentShipment.ShipmentPackage.CollectionDate.ToString("MMM", CultureInfo.InvariantCulture), shipment.ShipmentPackage.CollectionDate.Year), //"18-Mar-2016"
                        CmLBS = currentShipment.ShipmentPackage.WeightMetricId == 1,
                        VolumeCMM = currentShipment.ShipmentPackage.VolumeMetricId == 1,
                        ProductIngredients = shipmentProductIngredientsList,
                        ShipmentDescription = currentShipment.ShipmentPackage.PackageDescription,
                        DeclaredValue = currentShipment.ShipmentPackage.InsuranceDeclaredValue,
                        CarrierCost = currentShipment.ShipmentPackage.CarrierCost.ToString(),
                        Count = 1,
                        TotalWeight = currentShipment.ShipmentPackage.TotalWeight,
                    }
                };

                #endregion

                AddShipmentResponse response = new AddShipmentResponse();

                // Call to SIS.
                if (shipment.Carrier.Name == "USP")
                {
                    response = stampsMenmanager.SendShipmentDetails(shipmentDto);
                }
                else
                {
                    response = sisManager.SendShipmentDetails(shipmentDto);
                }

                // Update the Shipment entity based on the result of SIS.
                currentShipment.ShipmentCode = response.CodeShipment;
                currentShipment.TrackingNumber = response.Awb;

                // SIS will return the pacific time zone. So need to convert it to user time zone
                DateTime utcPickupDate = GetUTCTimeFromSISTaleUS(Convert.ToDateTime(response.DatePickup));
                shipmentDto.CarrierInformation.PickupDate = context.GetLocalTimeByUser(currentShipment.CreatedBy, utcPickupDate);

                shipmentDto.GeneralInformation.ShipmentPaymentTypeId = currentShipment.ShipmentPaymentTypeId;
                shipmentDto.GeneralInformation.ShipmentPaymentTypeName = Utility.GetEnumDescription((ShipmentPaymentType)currentShipment.ShipmentPaymentTypeId);

                ShipmentOperationResult result = new ShipmentOperationResult();


                if (string.IsNullOrWhiteSpace(response.Awb) && shipment.Carrier.Name != "USP")
                {
                    // Update Shipment entity
                    currentShipment.Provider = "Ship It Smarter";
                    currentShipment.Status = (short)ShipmentStatus.Error;
                    context.SaveChanges();

                    // This is SIS error.
                    // If payment done by online, do the refund.
                    if (currentShipment.ShipmentPaymentTypeId == 2)
                    {
                        RefundCharge(currentShipment.Id);
                    }

                    // build response result
                    result.Status = Status.SISError;
                    result.Message = "Error occured when adding shipment";
                    result.CarrierName = shipmentDto.CarrierInformation.CarrierName;
                    result.ShipmentCode = response.CodeShipment;
                    result.ShipmentReference = currentShipment.ShipmentReferenceName;
                }
                else if (string.IsNullOrWhiteSpace(response.Awb) && shipment.Carrier.Name == "USP")
                {
                    // Update Shipment entity
                    currentShipment.Provider = "Stamps.com";
                    currentShipment.Status = (short)ShipmentStatus.Error;
                    context.SaveChanges();

                    // This is SIS error.
                    // If payment done by online, do the refund.
                    if (currentShipment.ShipmentPaymentTypeId == 2)
                    {
                        RefundCharge(currentShipment.Id);
                    }

                    // build response result
                    result.Status = Status.SISError;
                    result.Message = "Error occured when adding shipment";
                    result.CarrierName = shipmentDto.CarrierInformation.CarrierName;
                    result.ShipmentCode = response.CodeShipment;
                    result.ShipmentReference = currentShipment.ShipmentReferenceName;

                }
                else
                {
                    // Update Shipment entity
                    currentShipment.Status = (short)ShipmentStatus.BookingConfirmation;
                    context.SaveChanges();

                    // If payment done by online, need to generate and save invoice from controller.

                    // build response result
                    result.Status = Status.Success;
                    result.Message = "Shipment added successfully";
                    result.ShipmentDto = shipmentDto;
                    result.ShipmentDto.GeneralInformation.TrackingNumber = currentShipment.TrackingNumber;

                    // If response.PDF is empty, get from following url.
                    if (string.IsNullOrWhiteSpace(response.PDF))
                    {
                        result.LabelURL = sisManager.GetLabel(shipment.ShipmentCode);
                    }
                    else
                    {
                        if (shipment.Carrier.Name == "TNT")
                        {
                            result.LabelURL = sisManager.GetLabel(shipment.ShipmentCode);
                        }
                        else
                        {
                            result.LabelURL = response.PDF;
                        }
                    }


                    result.ShipmentId = shipment.Id;

                    //adding the shipment label to azure
                    // For now replace userid from created by
                    sendShipmentDetails.UserId = shipment.CreatedBy;
                    AddShipmentLabeltoAzure(result, sendShipmentDetails);

                    var tenantId = context.GetTenantIdByUserId(shipment.CreatedBy);
                    var Url = getLabelforShipmentFromBlobStorage(shipment.Id, tenantId);
                    result.LabelURL = Url;
                }

                operationList.Add(result);

            }

            return operationList;
        }

        #endregion
    }


}
