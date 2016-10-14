using AutoMapper;
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
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using PI.Contract.DTOs.CostCenter;
using PI.Contract.DTOs.Address;
using PI.Contract.DTOs.Company;
using PI.Contract;
using AzureMediaManager;
using System.IO;
using HtmlAgilityPack;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.html.simpleparser;
using PI.Contract.TemplateLoader;
using PI.Contract.DTOs;
using PI.Contract.DTOs.Payment;
using PI.Data.Entity.Identity;

namespace PI.Business
{
    public class ShipmentsManagement : IShipmentManagement
    {
        private PIContext context;
        ICarrierIntegrationManager sisManager = null;
        ICompanyManagement companyManagment;
        private ILogger logger;
        IPaymentManager paymentManager;
        PostmenIntegrationManager postMenmanager;


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
            this.postMenmanager = new PostmenIntegrationManager(logger);
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

                currentRateSheetDetails.length = maxLength.ToString();
                currentRateSheetDetails.width = maxWidth.ToString();
                currentRateSheetDetails.height = maxHeight.ToString();
                currentRateSheetDetails.max_length = maxLength.ToString();
                currentRateSheetDetails.max_actual_length = maxLength.ToString();
                currentRateSheetDetails.max_width = maxWidth.ToString();
                currentRateSheetDetails.max_height = maxHeight.ToString();
                currentRateSheetDetails.max_weight = maxWeight.ToString();
                currentRateSheetDetails.weight = weight.ToString();
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



                currentRateSheetDetails.date_pickup = currentShipment.PackageDetails.PreferredCollectionDate;
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

                if (currentShipment.PackageDetails.CmLBS)
                {
                    currentRateSheetDetails.weight_unit = "kg";
                }
                else
                {
                    currentRateSheetDetails.weight_unit = "lbs";
                }

                if (currentShipment.PackageDetails.VolumeCMM)
                {
                    currentRateSheetDetails.volume_unit = "cm";
                }
                else
                {
                    currentRateSheetDetails.volume_unit = "inch";
                }

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
            currentRateSheetDetails.courier_tariff_type = "NLPARUPS:NLPARFED:USPARDHL2:USPARTNT:USPARUPS:USPARFED2:USUPSTNT:USPAREME:USPARPAE:NLPARTNT2:NLPARDPD";


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
            Company currentcompany = context.GetCompanyByUserId(addShipment.UserId);
            long sysDivisionId = 0;
            long sysCostCenterId = 0;

            //using (PIContext context = PIContext.Get())
            //{
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
                ShipmentPaymentTypeId = addShipment.GeneralInformation.ShipmentPaymentTypeId,
                Status = (short)ShipmentStatus.Pending,
                PickUpDate = addShipment.CarrierInformation.PickupDate,
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
                    EarliestPickupDate = addShipment.CarrierInformation.PickupDate ?? null,
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

                result.ShipmentId = newShipment.Id;
                result.Status = Status.Success;

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

            //}

            return result;
        }

        public string GetSquareApplicationId()
        {
            return ConfigurationManager.AppSettings["SquareApplicationId"].ToString();
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
                           select shipment).ToList();

            foreach (var item in content)
            {
                shipmentList.Add(new ShipmentDto()
                {
                    GeneralInformation = new GeneralInformationDto
                    {
                        TrackingNumber = item.TrackingNumber,
                        ShipmentCode = item.ShipmentCode
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
            int page = 1;
            int pageSize = 10;
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
            else if (role == "Supervisor")
            {
                divisions = companyManagment.GetAssignedDivisions(shipmentSerach.UserId);
            }
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


            pagedRecord.Content = new List<ShipmentDto>();


            var content = (from shipment in Shipments
                           where shipment.IsDelete == false && !shipment.IsParent &&
                           ((bool)shipmentSerach.DynamicContent.viaDashboard ?
                            shipment.Status != (short)ShipmentStatus.Delivered && shipment.Status != (short)ShipmentStatus.Deleted
                               && shipment.IsFavourite :
                               ((string.IsNullOrWhiteSpace(shipmentSerach.DynamicContent.status.ToString()) ||
                                  (shipmentSerach.DynamicContent.status.ToString() == "Error" ? (shipment.Status == (short)ShipmentStatus.Error || shipment.Status == (short)ShipmentStatus.Pending)
                                                    : shipmentSerach.DynamicContent.status.ToString() == "Transit" ? (shipment.Status == (short)ShipmentStatus.Pickup || shipment.Status == (short)ShipmentStatus.Transit || shipment.Status == (short)ShipmentStatus.OutForDelivery)
                                                    : shipmentSerach.DynamicContent.status.ToString() == "Exception" ? (shipment.Status == (short)ShipmentStatus.Exception || shipment.Status == (short)ShipmentStatus.Claim)
                                                    : (shipmentSerach.DynamicContent.status.ToString() == "Delayed" || shipment.Status == (short)Enum.Parse(typeof(ShipmentStatus), shipmentSerach.DynamicContent.status.ToString())))
                                                   )
                               //(startDate == null || (shipment.ShipmentPackage.EarliestPickupDate >= startDate && shipment.ShipmentPackage.EarliestPickupDate <= endDate)) &&
                               //(string.IsNullOrEmpty(number) || shipment.TrackingNumber.Contains(number) || shipment.ShipmentCode.Contains(number)) &&
                               //(string.IsNullOrEmpty(source) || shipment.ConsignorAddress.Country.Contains(source) || shipment.ConsignorAddress.City.Contains(source)) &&
                               //(string.IsNullOrEmpty(destination) || shipment.ConsigneeAddress.Country.Contains(destination) || shipment.ConsigneeAddress.City.Contains(destination))
                               )
                           ) &&
                           !shipment.IsParent
                           select shipment).ToList();

            // Update retrieve shipment list status from SIS.
            //string environment = "";
            //foreach (var shipment in content)
            //{
            //    if (shipment.Status != ((short)ShipmentStatus.Delivered) && !string.IsNullOrWhiteSpace(shipment.TrackingNumber))
            //    {
            //        environment = GetEnvironmentByTarrif(shipment.TariffText);
            //        UpdateLocationHistory(shipment.Carrier.Name, shipment.TrackingNumber, shipment.ShipmentCode, environment, shipment.Id);
            //    }
            //}

            //using (PIContext context = PIContext.Get())
            //{
            var latestStatusHistory = context.ShipmentLocationHistories.OrderByDescending(x => x.CreatedDate).FirstOrDefault();
            //latestStatusHistory.CreatedDate 

            // Get new updated shipment list again.
            var updatedtContent = (from shipment in Shipments
                                   join package in context.ShipmentPackages on shipment.ShipmentPackageId equals package.Id
                                   where shipment.IsDelete == false &&
                                   ((bool)shipmentSerach.DynamicContent.viaDashboard ? shipment.Status != (short)ShipmentStatus.Delivered &&
                                    shipment.Status != (short)ShipmentStatus.Deleted
                                    && shipment.IsFavourite :
                                    ((string.IsNullOrWhiteSpace(shipmentSerach.DynamicContent.status.ToString()) ||
                                        (shipmentSerach.DynamicContent.status.ToString() == "Error" ? (shipment.Status == (short)ShipmentStatus.Error || shipment.Status == (short)ShipmentStatus.Pending)
                                                    : shipmentSerach.DynamicContent.status.ToString() == "Transit" ? (shipment.Status == (short)ShipmentStatus.Pickup || shipment.Status == (short)ShipmentStatus.Transit || shipment.Status == (short)ShipmentStatus.OutForDelivery)
                                                    : shipmentSerach.DynamicContent.status.ToString() == "Exception" ? (shipment.Status == (short)ShipmentStatus.Exception || shipment.Status == (short)ShipmentStatus.Claim)
                                                    : (shipmentSerach.DynamicContent.status.ToString() == "Delayed" || shipment.Status == (short)Enum.Parse(typeof(ShipmentStatus), shipmentSerach.DynamicContent.status.ToString())))
                                                   ) &&
                                       (string.IsNullOrWhiteSpace(shipmentSerach.DynamicContent.startDate.ToString()) || (shipment.ShipmentPackage.EarliestPickupDate >= DateTime.Parse(shipmentSerach.DynamicContent.startDate.ToString()) && shipment.ShipmentPackage.EarliestPickupDate <= DateTime.Parse(shipmentSerach.DynamicContent.endDate.ToString()))) &&
                                       (string.IsNullOrWhiteSpace(shipmentSerach.DynamicContent.number.ToString()) || (!string.IsNullOrEmpty(shipment.TrackingNumber) && shipment.TrackingNumber.Contains(shipmentSerach.DynamicContent.number.ToString())) || (!string.IsNullOrEmpty(shipment.ShipmentCode) && shipment.ShipmentCode.Contains(shipmentSerach.DynamicContent.number.ToString()))) &&
                                       (string.IsNullOrWhiteSpace(shipmentSerach.DynamicContent.source.ToString()) || shipment.ConsignorAddress.Country.Contains(shipmentSerach.DynamicContent.source.ToString()) || shipment.ConsignorAddress.City.Contains(shipmentSerach.DynamicContent.source.ToString())) &&
                                       (string.IsNullOrWhiteSpace(shipmentSerach.DynamicContent.destination.ToString()) || shipment.ConsigneeAddress.Country.Contains(shipmentSerach.DynamicContent.destination.ToString()) || shipment.ConsigneeAddress.City.Contains(shipmentSerach.DynamicContent.destination.ToString()))
                                     )
                                   ) &&
                                   !shipment.IsParent
                                   select shipment).ToList();

            foreach (var item in updatedtContent)
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
                        ShipmentId = item.Id.ToString(),
                        ShipmentMode = Enum.GetName(typeof(Contract.Enums.CarrierType), item.ShipmentMode),
                        ShipmentName = item.ShipmentName,
                        ShipmentReferenceName = item.ShipmentReferenceName,
                        ShipmentServices = Utility.GetEnumDescription((ShipmentService)item.ShipmentService),
                        TrackingNumber = item.TrackingNumber,
                        CreatedDate = item.CreatedDate.ToString("MM/dd/yyyy"),
                        Status = Utility.GetEnumDescription((ShipmentStatus)item.Status),
                        IsFavourite = item.IsFavourite,
                        IsEnableEdit = ((ShipmentStatus)item.Status == ShipmentStatus.Error || (ShipmentStatus)item.Status == ShipmentStatus.Pending),
                        IsEnableDelete = ((ShipmentStatus)item.Status == ShipmentStatus.Error || (ShipmentStatus)item.Status == ShipmentStatus.Pending || (ShipmentStatus)item.Status == ShipmentStatus.BookingConfirmation)
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
                        ValueCurrency = Convert.ToInt32(item.ShipmentPackage.Currency),
                        PreferredCollectionDate = item.ShipmentPackage.CollectionDate.ToString(),
                        ProductIngredients = this.getPackageDetails(item.ShipmentPackage.PackageProducts),
                        ShipmentDescription = item.ShipmentPackage.PackageDescription

                    },
                    CarrierInformation = new CarrierInformationDto
                    {
                        CarrierName = item.Carrier.Name,
                        serviceLevel = item.ServiceLevel,
                        PickupDate = item.PickUpDate
                    }

                });
            }
            // }
            pagedRecord.TotalRecords = Shipments.Count();
            pagedRecord.CurrentPage = page;
            pagedRecord.PageSize = pageSize;
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
        public List<Data.Entity.Shipment> GetshipmentsByUserIdAndCreatedDate(string userId, DateTime createdDate, string carreer)
        {
            List<Data.Entity.Shipment> currentShipments = null;
            //using (PIContext context = PIContext.Get())
            //{
            currentShipments = context.Shipments.Where(x => x.CreatedBy == userId && x.CreatedDate.Year == createdDate.Year && x.CreatedDate.Month == createdDate.Month && x.CreatedDate.Day == createdDate.Day && x.Carrier.Name == carreer && !string.IsNullOrEmpty(x.TrackingNumber)).ToList();
            //}
            return currentShipments;
        }

        //get shipments by shipment reference
        public List<Data.Entity.Shipment> GetshipmentsByReference(string userId, string reference)
        {
            List<Data.Entity.Shipment> currentShipments = null;
            //using (PIContext context = PIContext.Get())
            //{
            currentShipments = context.Shipments.Where(x => x.CreatedBy == userId && x.ShipmentReferenceName.Contains(reference) && !string.IsNullOrEmpty(x.TrackingNumber)).ToList();
            //}
            return currentShipments;
        }


        //update shipment status manually only by admin
        public int UpdateshipmentStatusManually(string codeShipment, string status)
        {
            //using (PIContext context = PIContext.Get())
            //{
            var shipment = (from shipmentinfo in context.Shipments
                            where shipmentinfo.ShipmentCode == codeShipment
                            select shipmentinfo).FirstOrDefault();
            if (shipment == null)
            {
                return 0;
            }

            shipment.Status = (short)Enum.Parse(typeof(ShipmentStatus), status);
            shipment.ManualStatusUpdatedDate = DateTime.UtcNow;
            context.SaveChanges();
            return 1;
            //}

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
                    CreatedDate = currentShipment.CreatedDate.ToString("MM/dd/yyyy"),
                    Status = currentShipment.Status.ToString(),
                    ShipmentLabelBLOBURL = getLabelforShipmentFromBlobStorage(currentShipment.Id, tenantId)
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
                CarrierInformation = new CarrierInformationDto
                {
                    CarrierName = currentShipment.Carrier.Name,
                    serviceLevel = currentShipment.ServiceLevel,
                    PickupDate = currentShipment.PickUpDate,
                    CountryCodeByTarrifText = countryCodeFromTarrifText
                }

            };

            return currentShipmentDto;
        }
        //get the product ingrediants List
        public List<ProductIngredientsDto> getPackageDetails(IList<PackageProduct> products)
        {
            List<ProductIngredientsDto> ingrediantList = new List<ProductIngredientsDto>();

            foreach (var ingrediant in products)
            {
                ingrediantList.Add(
                    new ProductIngredientsDto
                    {
                        Height = ingrediant.Height,
                        Length = ingrediant.Length,
                        ProductType = Utility.GetEnumDescription((ProductType)ingrediant.ProductTypeId),
                        Quantity = ingrediant.Quantity,
                        Weight = ingrediant.Weight,
                        Width = ingrediant.Width,
                        Description = ingrediant.Description
                    });

            }
            return ingrediantList;
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
                    tariffText = shipment.TariffText
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

            // Add Shipment to SIS.
            if (shipment.Carrier.Name == "USPS")
            {
                response = postMenmanager.SendShipmentDetails(shipmentDto);
            }
            else
            {
                response = sisManager.SendShipmentDetails(shipmentDto);
            }


            shipment.ShipmentCode = response.CodeShipment;
            shipment.TrackingNumber = response.Awb;
            result.AddShipmentXML = response.AddShipmentXML;

            shipmentDto.CarrierInformation.PickupDate = Convert.ToDateTime(response.DatePickup);
            shipmentDto.GeneralInformation.ShipmentPaymentTypeId = shipment.ShipmentPaymentTypeId;
            shipmentDto.GeneralInformation.ShipmentPaymentTypeName = Utility.GetEnumDescription((ShipmentPaymentType)shipment.ShipmentPaymentTypeId);

            if (string.IsNullOrWhiteSpace(response.Awb))
            {
                result.Status = Status.SISError;
                result.Message = "Error occured when adding shipment";
                result.CarrierName = shipmentDto.CarrierInformation.CarrierName;
                result.ShipmentCode = response.CodeShipment;
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
                }
                else
                {
                    result.LabelURL = response.PDF;
                }
                result.ShipmentId = shipment.Id;
                shipment.Status = (short)ShipmentStatus.BookingConfirmation;

                //adding the shipment label to azure
                this.AddShipmentLabeltoAzure(result, sendShipmentDetails);

                //create the invoice and upload to the blob
                //    result.InvoiceURL =  this.GenerateUSInvoice(shipment);

            }

            context.SaveChanges();
            return result;
            // }
        }

        public long GetTenantIdByUserId(string userid)
        {
            ApplicationUser currentuser = null;

            currentuser = context.Users.SingleOrDefault(u => u.Id == userid);

            if (currentuser == null)
            {
                return 0;
            }
            return currentuser.TenantId;
        }


        //method to generate US invoices
        public string GenerateUSInvoice(Data.Entity.Shipment shipmentDetails)
        {
            string baseUrl = ConfigurationManager.AppSettings["PIBlobStorage"];

            Random generator = new Random();
            string code = generator.Next(1000000, 9999999).ToString("D7");
            string invoiceNumber = "PI_" + DateTime.UtcNow.Year.ToString() + "_" + code;

            //initializing azure storage
            AzureFileManager media = new AzureFileManager();

            var tenantId = context.GetTenantIdByUserId(shipmentDetails.CreatedBy);

            var invoicePdf = new Document(PageSize.B5);
            //getting the server path to create temp pdf file
            string wanted_path = System.Web.HttpContext.Current.Server.MapPath("\\Pdf\\invoice.pdf");

            PdfWriter.GetInstance(invoicePdf, new FileStream(wanted_path, FileMode.Create));
            HTMLWorker htmlWorker = new HTMLWorker(invoicePdf);

            string htmlTemplate = "";
            TemplateLoader templateLoader = new TemplateLoader();

            StringBuilder packageDetails = new StringBuilder();

            packageDetails.Append("<tr> <td> <label>" + shipmentDetails.Carrier.Name + "</label><br/>");
            packageDetails.Append("<label>AWB#:</label><p>" + shipmentDetails.TrackingNumber + "</p><br/>");
            packageDetails.Append("<label>Reference:</label><p>" + shipmentDetails.ShipmentPackage.PackageDescription + "</p><br/>");
            packageDetails.Append("<label>Origin:</label><p>" + shipmentDetails.ConsignorAddress.City + " " + shipmentDetails.ConsignorAddress.Country + "</p><br/>");
            packageDetails.Append("<label>Destination:</label><p>" + shipmentDetails.ConsigneeAddress.City + " " + shipmentDetails.ConsigneeAddress.Country + "</p><br/>");
            packageDetails.Append("<label>Weight:</label><p>" + shipmentDetails.ShipmentPackage.TotalWeight + "</p><br/>");
            packageDetails.Append("<label>Date:</label><p>" + shipmentDetails.CreatedDate + "</p><br/>");
            packageDetails.Append("</td>");
            packageDetails.Append("<td>" + shipmentDetails.ShipmentPackage.PackageProducts.Count() + "</td>");
            packageDetails.Append("<td>$" + shipmentDetails.ShipmentPackage.CarrierCost + "</td>");
            packageDetails.Append("<td>$" + shipmentDetails.ShipmentPackage.CarrierCost + "</td> </tr>");
            packageDetails.Append("<tr><td> <label>Services</label><br/> <p>Paypal fee(4.5%)</p></td>");
            packageDetails.Append("<td>" + shipmentDetails.ShipmentPackage.PackageProducts.Count() + "</td>");
            packageDetails.Append("<td>" + "" + "</td> </tr>");
            packageDetails.Append("<td>" + "" + "</td> </tr>");


            //get the email template for invoice
            HtmlDocument template = templateLoader.getHtmlTemplatebyName("invoiceUS");
            htmlTemplate = template.DocumentNode.InnerHtml;


            //replacing values from shipment
            var replacedString = htmlTemplate.Replace("{BillingName}", shipmentDetails.ConsignorAddress.FirstName + " " + shipmentDetails.ConsignorAddress.LastName)
            .Replace("{BillingAddress1}", shipmentDetails.ConsignorAddress.StreetAddress1)
            .Replace("{BillingAddress2}", shipmentDetails.ConsignorAddress.StreetAddress1)
            .Replace("{BillingCity}", shipmentDetails.ConsignorAddress.City)
            .Replace("{BillingState}", shipmentDetails.ConsignorAddress.State)
            .Replace("{BillingZip}", shipmentDetails.ConsignorAddress.ZipCode)
            .Replace("{BillingCountry}", shipmentDetails.ConsignorAddress.Country)
            .Replace("{invoicenumber}", "2016-260")
            .Replace("{invoicedate}", DateTime.Now.ToString("dd/MM/yyyy"))
            .Replace("{duedate}", DateTime.Now.AddDays(10).ToString("dd/MM/yyyy"))
            .Replace("{terms}", "Net 10")
            .Replace("{totalvalue}", shipmentDetails.ShipmentPackage.CarrierCost.ToString() + "$")
            .Replace("{tableBody}", packageDetails.ToString());


            TextReader txtReader = new StringReader(replacedString);
            invoicePdf.Open();
            htmlWorker.StartDocument();
            htmlWorker.Parse(txtReader);

            htmlWorker.EndDocument();
            htmlWorker.Close();
            //closing the doc
            invoicePdf.Close();


            var invoicename = "";
            using (Stream savedPdf = File.OpenRead(wanted_path))
            {
                invoicename = string.Format("{0}_{1}", System.Guid.NewGuid().ToString(), invoiceNumber + ".pdf");

                media.InitializeStorage(tenantId.ToString(), Utility.GetEnumDescription(DocumentType.Invoice));

                // var opResult = media.Upload(savedPdf, invoicename);
                Task.Run(async () => await media.Upload(savedPdf, invoicename));
            }

            //get the saved pdf url
            var returnData = baseUrl + "TENANT_" + tenantId + "/" + Utility.GetEnumDescription(DocumentType.Invoice)
                                     + "/" + invoicename;

            return returnData;

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
                // locationHistory = this.getUpdatedShipmentHistoryFromDB(codeShipment);
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
                    short status = (short)Utility.GetValueFromDescription<ShipmentStatus>(currentSisLocationHistory.info.status);
                    this.UpdateShipmentStatus(codeShipment, status);
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
                    CreatedDate = currentShipment.CreatedDate.ToString("MM/dd/yyyy"),
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
                    PickupDate = currentShipment.PickUpDate,
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
        public TrackerDto getUpdatedShipmentHistoryFromDB(string shipmentId)
        {
            StatusHistoryResponce statusHistory = new StatusHistoryResponce();
            TrackerDto tracker = new TrackerDto();
            tracker.TrackingDetails = new List<TrackingDetails>();

            List<ShipmentLocationHistory> historyList = GetShipmentLocationHistoryByShipmentId(Convert.ToInt16(shipmentId));

            foreach (var item in historyList)
            {
                tracker.TrackingDetails.Add(new TrackingDetails()
                {

                    Status = item.Status,
                    DateTime = item.DateTime.ToString(),
                    City = item.City,
                    Country = item.Country,
                    Message = item.Message,
                    State = item.State,
                    Zip = item.Zip
                });
            }
            tracker.Status = tracker.TrackingDetails.Last().Status;
            //   tracker.Status = historyList.Last().Status;
            return tracker;

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
        public PagedList GetAllPendingShipmentsbyUser(string userId, DateTime? startDate, DateTime? endDate,
                                               string number)
        {
            int page = 1;
            int pageSize = 10;
            IList<DivisionDto> divisions = null;
            IList<int> divisionList = new List<int>();
            List<Data.Entity.Shipment> Shipments = new List<Data.Entity.Shipment>();
            var pagedRecord = new PagedList();
            if (userId == null)
            {
                return null;
            }
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


            pagedRecord.Content = new List<ShipmentDto>();

            var content = (from shipment in Shipments
                           where shipment.IsDelete == false &&
                           shipment.Status == (short)ShipmentStatus.BookingConfirmation &&
                           (startDate == null || (shipment.ShipmentPackage.EarliestPickupDate >= startDate && shipment.ShipmentPackage.EarliestPickupDate <= endDate)) &&
                           (string.IsNullOrEmpty(number) || shipment.TrackingNumber.Contains(number) || shipment.ShipmentCode.Contains(number))
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
                        ShipmentMode = Enum.GetName(typeof(Contract.Enums.CarrierType), item.ShipmentMode),
                        ShipmentName = item.ShipmentName,
                        //ShipmentTermCode = item.ShipmentTermCode,
                        //ShipmentTypeCode = item.ShipmentTypeCode,


                        TrackingNumber = item.TrackingNumber,
                        CreatedDate = item.CreatedDate.ToString("MM/dd/yyyy"),
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
                        ValueCurrency = Convert.ToInt32(item.ShipmentPackage.Currency),
                        PreferredCollectionDate = item.ShipmentPackage.CollectionDate.ToString(),
                        ProductIngredients = this.getPackageDetails(item.ShipmentPackage.PackageProducts),
                        ShipmentDescription = item.ShipmentPackage.PackageDescription

                    },
                    CarrierInformation = new CarrierInformationDto
                    {
                        CarrierName = item.Carrier.Name,
                        serviceLevel = item.ServiceLevel,
                        PickupDate = item.PickUpDate
                    }

                });
            }

            pagedRecord.TotalRecords = Shipments.Count();
            pagedRecord.CurrentPage = page;
            pagedRecord.PageSize = pageSize;
            pagedRecord.TotalPages = (int)Math.Ceiling((decimal)pagedRecord.TotalRecords / pagedRecord.PageSize);

            return pagedRecord;
        }


        public List<ShipmentDto> GetAllshipmentsForManifest(string userId, string date, string carreer, string reference)
        {
            List<Data.Entity.Shipment> shipmentList = new List<Data.Entity.Shipment>();
            if (string.IsNullOrEmpty(reference))
            {
                shipmentList = this.GetshipmentsByUserIdAndCreatedDate(userId, Convert.ToDateTime(date), carreer);
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
                        CreatedDate = item.CreatedDate.ToString("MM/dd/yyyy"),
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
                        ValueCurrency = item.ShipmentPackage.Currency == null ? 0 : Convert.ToInt32(item.ShipmentPackage.Currency.Id),
                        PreferredCollectionDate = item.ShipmentPackage.CollectionDate.ToString(),
                        ProductIngredients = this.getPackageDetails(item.ShipmentPackage.PackageProducts),
                        ShipmentDescription = item.ShipmentPackage.PackageDescription

                    },
                    CarrierInformation = new CarrierInformationDto
                    {
                        CarrierName = item.Carrier.Name,
                        serviceLevel = item.ServiceLevel,
                        PickupDate = item.PickUpDate
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
                    CreatedDate = currentShipment.CommercialInvoice.CreatedDate.ToString("dd-MMM-yyyy"),
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
                    CreatedDate = currentShipment.CreatedDate.ToString("dd-MMM-yyyy"),
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
                CreatedDate = currentShipment.CreatedDate.ToString("dd-MMM-yyyy"),
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
                    EarliestPickupDate = currentShipment.ShipmentPackage.EarliestPickupDate.ToString(),
                    EstDeliveryDate = currentShipment.ShipmentPackage.EstDeliveryDate.ToString()

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
                CreatedBy = "1",
                CreatedDate = DateTime.UtcNow,
                IsActive = true,
                HSCode = p.HSCode,
            }));

            CommercialInvoice invoice = new CommercialInvoice()
            {
                ShipmentId = addInvoice.ShipmentId,
                ShipmentReferenceName = addInvoice.ShipmentReferenceName,
                CreatedBy = "1",
                CreatedDate = Convert.ToDateTime(addInvoice.CreatedDate),
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
                    CreatedBy = "1",
                    CreatedDate = DateTime.UtcNow,
                    IsActive = true,
                    InvoiceItemLines = invoiceItemLineList
                }
            };

            context.CommercialInvoices.Add(invoice);
            context.SaveChanges();
            // }
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
                        CreatedDate = item.CreatedDate.ToString("MM/dd/yyyy"),
                        Status = ((ShipmentStatus)item.Status).ToString(),
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
                        ValueCurrency = item.ShipmentPackage.Currency == null ? 0 : Convert.ToInt32(item.ShipmentPackage.Currency.Id),
                        PreferredCollectionDate = item.ShipmentPackage.CollectionDate.ToString(),
                        ProductIngredients = this.getPackageDetails(item.ShipmentPackage.PackageProducts),
                        ShipmentDescription = item.ShipmentPackage.PackageDescription

                    },
                    CarrierInformation = new CarrierInformationDto
                    {
                        CarrierName = item.Carrier.Name,
                        serviceLevel = item.ServiceLevel,
                        PickupDate = item.PickUpDate
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
                        CreatedDate = item.CreatedDate.ToString("MM/dd/yyyy"),
                        Status = ((ShipmentStatus)item.Status).ToString(),
                        IsEnableEdit = ((ShipmentStatus)item.Status == ShipmentStatus.Error || (ShipmentStatus)item.Status == ShipmentStatus.Pending),
                        IsEnableDelete = ((ShipmentStatus)item.Status == ShipmentStatus.Error || (ShipmentStatus)item.Status == ShipmentStatus.Pending || (ShipmentStatus)item.Status == ShipmentStatus.BookingConfirmation),
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
                        ValueCurrency = item.ShipmentPackage.Currency == null ? 0 : Convert.ToInt32(item.ShipmentPackage.Currency.Id),
                        PreferredCollectionDate = item.ShipmentPackage.CollectionDate.ToString(),
                        ProductIngredients = this.getPackageDetails(item.ShipmentPackage.PackageProducts),
                        ShipmentDescription = item.ShipmentPackage.PackageDescription

                    },
                    CarrierInformation = new CarrierInformationDto
                    {
                        CarrierName = item.Carrier.Name,
                        serviceLevel = item.ServiceLevel,
                        PickupDate = item.PickUpDate
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


        public PagedList loadAllShipmentsForAdmin(string status = null, DateTime? startDate = null, DateTime? endDate = null, string searchValue = null)
        {
            int page = 1;
            int pageSize = 10;
            var pagedRecord = new PagedList();
            short enumStatus = string.IsNullOrEmpty(status) || status == "Delayed" ? (short)0 : (short)Enum.Parse(typeof(ShipmentStatus), status);

            pagedRecord.Content = new List<ShipmentDto>();

            //using (var context = PIContext.Get())
            //{
            var content = (from shipment in context.Shipments
                           where shipment.IsDelete == false &&
                           //shipment.
                           (string.IsNullOrEmpty(status) ||
                              (status == "Error" ? (shipment.Status == (short)ShipmentStatus.Error || shipment.Status == (short)ShipmentStatus.Pending)
                                                : status == "Transit" ? (shipment.Status == (short)ShipmentStatus.Pickup || shipment.Status == (short)ShipmentStatus.Transit || shipment.Status == (short)ShipmentStatus.OutForDelivery)
                                                : status == "Exception" ? (shipment.Status == (short)ShipmentStatus.Exception || shipment.Status == (short)ShipmentStatus.Claim)
                                                : (status == "Delayed" || shipment.Status == enumStatus)
                              )
                            ) &&
                           (startDate == null || (shipment.ShipmentPackage.EarliestPickupDate >= startDate && shipment.ShipmentPackage.EarliestPickupDate <= endDate)) &&
                           (searchValue == null ||
                           (shipment.TrackingNumber.Contains(searchValue)) || (shipment.Division.Company.Name.Contains(searchValue)) ||
                           (shipment.ConsignorAddress.Country.Contains(searchValue) || shipment.ConsignorAddress.City.Contains(searchValue)) ||
                           (shipment.ConsigneeAddress.Country.Contains(searchValue) || shipment.ConsigneeAddress.City.Contains(searchValue)))
                           select shipment).ToList();


            foreach (var item in content)
            {
                var owner = context.Users.Where(u => u.Id == item.CreatedBy).SingleOrDefault();

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
                        Owner = owner.FirstName + " " + owner.LastName,
                        //ShipmentTermCode = item.ShipmentTermCode,
                        //ShipmentTypeCode = item.ShipmentTypeCode,
                        ShipmentId = item.Id.ToString(),
                        TrackingNumber = item.TrackingNumber,
                        CreatedDate = item.CreatedDate.ToString("MM/dd/yyyy"),
                        Status = ((ShipmentStatus)item.Status).ToString(),
                        IsEnableEdit = ((ShipmentStatus)item.Status == ShipmentStatus.Error || (ShipmentStatus)item.Status == ShipmentStatus.Pending),
                        IsEnableDelete = ((ShipmentStatus)item.Status == ShipmentStatus.Error || (ShipmentStatus)item.Status == ShipmentStatus.Pending || (ShipmentStatus)item.Status == ShipmentStatus.BookingConfirmation),
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
                        ValueCurrency = item.ShipmentPackage.Currency == null ? 0 : Convert.ToInt32(item.ShipmentPackage.Currency.Id),
                        PreferredCollectionDate = item.ShipmentPackage.CollectionDate.ToString(),
                        ProductIngredients = this.getPackageDetails(item.ShipmentPackage.PackageProducts),
                        ShipmentDescription = item.ShipmentPackage.PackageDescription

                    },
                    CarrierInformation = new CarrierInformationDto
                    {
                        CarrierName = item.Carrier.Name,
                        serviceLevel = item.ServiceLevel,
                        PickupDate = item.PickUpDate
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



        public byte[] loadAllShipmentsForAdminExcelExport(string status = null, DateTime? startDate = null, DateTime? endDate = null,
                                        string number = null, string source = null, string destination = null)
        {
            int page = 1;
            int pageSize = 10;
            short enumStatus = string.IsNullOrEmpty(status) || status == "Delayed" ? (short)0 : (short)Enum.Parse(typeof(ShipmentStatus), status);

            var Content = new List<ShipmentDto>();

            //using (var context = PIContext.Get())
            //{
            var content = (from shipment in context.Shipments
                           where shipment.IsDelete == false &&
                           (string.IsNullOrEmpty(status) ||
                              (status == "Error" ? (shipment.Status == (short)ShipmentStatus.Error || shipment.Status == (short)ShipmentStatus.Pending)
                                                : status == "Transit" ? (shipment.Status == (short)ShipmentStatus.Pickup || shipment.Status == (short)ShipmentStatus.Transit || shipment.Status == (short)ShipmentStatus.OutForDelivery)
                                                : status == "Exception" ? (shipment.Status == (short)ShipmentStatus.Exception || shipment.Status == (short)ShipmentStatus.Claim)
                                                : (status == "Delayed" || shipment.Status == enumStatus)
                              )
                            ) &&
                           /*shipment.Division.CompanyId.ToString() == companyId &&*/
                           //(string.IsNullOrEmpty(status) || (status == "Active" ? shipment.Status != (short)ShipmentStatus.Delivered : shipment.Status == (short)ShipmentStatus.Delivered)) &&
                           (startDate == null || (shipment.ShipmentPackage.EarliestPickupDate >= startDate && shipment.ShipmentPackage.EarliestPickupDate <= endDate)) &&
                           (number == null || shipment.TrackingNumber.Contains(number) || shipment.ShipmentCode.Contains(number)) &&
                           (source == null || shipment.ConsignorAddress.Country.Contains(source) || shipment.ConsignorAddress.City.Contains(source)) &&
                           (destination == null || shipment.ConsigneeAddress.Country.Contains(destination) || shipment.ConsigneeAddress.City.Contains(destination))
                           select shipment).ToList();


            foreach (var item in content)
            {
                Content.Add(new ShipmentDto
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
                        CreatedDate = item.CreatedDate.ToString("MM/dd/yyyy"),
                        Status = ((ShipmentStatus)item.Status).ToString(),
                        IsEnableEdit = ((ShipmentStatus)item.Status == ShipmentStatus.Error || (ShipmentStatus)item.Status == ShipmentStatus.Pending),
                        IsEnableDelete = ((ShipmentStatus)item.Status == ShipmentStatus.Error || (ShipmentStatus)item.Status == ShipmentStatus.Pending || (ShipmentStatus)item.Status == ShipmentStatus.BookingConfirmation),
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
                        ValueCurrency = item.ShipmentPackage.Currency == null ? 0 : Convert.ToInt32(item.ShipmentPackage.Currency.Id),
                        PreferredCollectionDate = item.ShipmentPackage.CollectionDate.ToString(),
                        ProductIngredients = this.getPackageDetails(item.ShipmentPackage.PackageProducts),
                        ShipmentDescription = item.ShipmentPackage.PackageDescription

                    },
                    CarrierInformation = new CarrierInformationDto
                    {
                        CarrierName = item.Carrier.Name,
                        serviceLevel = item.ServiceLevel,
                        PickupDate = item.PickUpDate
                    }

                });
            }

            return this.GenerateExcelSheetForShipmentExportFunction(Content);

            // }
        }


        public byte[] loadAllShipmentsForExcel(string status, string userId, DateTime? startDate, DateTime? endDate,
                                              string number, string source, string destination, bool viaDashboard)
        {
            int page = 1;
            int pageSize = 10;
            IList<DivisionDto> divisions = null;
            IList<int> divisionList = new List<int>();
            List<Data.Entity.Shipment> Shipments = new List<Data.Entity.Shipment>();

            if (userId == null)
            {
                return null;
            }
            string role = context.GetUserRoleById(userId);
            if (role == "BusinessOwner" || role == "Manager")
            {
                divisions = this.GetAllDivisionsinCompany(userId);
            }
            else if (role == "Supervisor")
            {
                divisions = companyManagment.GetAssignedDivisions(userId);
            }
            if (divisions != null && divisions.Count > 0)
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


            var shipments = new List<ShipmentDto>();

            var content = (from shipment in Shipments
                           where shipment.IsDelete == false && !shipment.IsParent &&
                           (viaDashboard ? shipment.Status != (short)ShipmentStatus.Delivered && shipment.Status != (short)ShipmentStatus.Deleted
                               && shipment.IsFavourite :
                               ((string.IsNullOrEmpty(status) ||
                                  (status == "Error" ? (shipment.Status == (short)ShipmentStatus.Error || shipment.Status == (short)ShipmentStatus.Pending)
                                                    : status == "Transit" ? (shipment.Status == (short)ShipmentStatus.Pickup || shipment.Status == (short)ShipmentStatus.Transit || shipment.Status == (short)ShipmentStatus.OutForDelivery)
                                                    : status == "Exception" ? (shipment.Status == (short)ShipmentStatus.Exception || shipment.Status == (short)ShipmentStatus.Claim)
                                                    : (status == "Delayed" || shipment.Status == (short)Enum.Parse(typeof(ShipmentStatus), status)))
                                                   )
                               //(startDate == null || (shipment.ShipmentPackage.EarliestPickupDate >= startDate && shipment.ShipmentPackage.EarliestPickupDate <= endDate)) &&
                               //(string.IsNullOrEmpty(number) || shipment.TrackingNumber.Contains(number) || shipment.ShipmentCode.Contains(number)) &&
                               //(string.IsNullOrEmpty(source) || shipment.ConsignorAddress.Country.Contains(source) || shipment.ConsignorAddress.City.Contains(source)) &&
                               //(string.IsNullOrEmpty(destination) || shipment.ConsigneeAddress.Country.Contains(destination) || shipment.ConsigneeAddress.City.Contains(destination))
                               )
                           ) &&
                           !shipment.IsParent
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

            //using (PIContext context = PIContext.Get())
            //{
            var latestStatusHistory = context.ShipmentLocationHistories.OrderByDescending(x => x.CreatedDate).FirstOrDefault();
            //latestStatusHistory.CreatedDate 

            // Get new updated shipment list again.
            var updatedtContent = (from shipment in Shipments
                                   join package in context.ShipmentPackages on shipment.ShipmentPackageId equals package.Id
                                   where shipment.IsDelete == false &&
                                   (viaDashboard ? shipment.Status != (short)ShipmentStatus.Delivered && shipment.Status != (short)ShipmentStatus.Deleted
                                    && shipment.IsFavourite :
                                       ((string.IsNullOrEmpty(status) ||
                                        (status == "Error" ? (shipment.Status == (short)ShipmentStatus.Error || shipment.Status == (short)ShipmentStatus.Pending)
                                                         : status == "Transit" ? (shipment.Status == (short)ShipmentStatus.Pickup || shipment.Status == (short)ShipmentStatus.Transit || shipment.Status == (short)ShipmentStatus.OutForDelivery)
                                                         : status == "Exception" ? (shipment.Status == (short)ShipmentStatus.Exception || shipment.Status == (short)ShipmentStatus.Claim)
                                                         : status == "Delayed" ? (shipment.Status != (short)ShipmentStatus.Delivered && latestStatusHistory != null && latestStatusHistory.CreatedDate > package.EstDeliveryDate.Value)
                                                         : shipment.Status == (short)Enum.Parse(typeof(ShipmentStatus), status))) &&
                                       //((string.IsNullOrEmpty(status) || (status == "Active" ? shipment.Status != (short)ShipmentStatus.Delivered : shipment.Status == (short)ShipmentStatus.Delivered)) &&
                                       (startDate == null || (shipment.ShipmentPackage.EarliestPickupDate >= startDate && shipment.ShipmentPackage.EarliestPickupDate <= endDate)) &&
                                         (string.IsNullOrEmpty(number) || (!string.IsNullOrEmpty(shipment.TrackingNumber) && shipment.TrackingNumber.Contains(number)) || (!string.IsNullOrEmpty(shipment.ShipmentCode) && shipment.ShipmentCode.Contains(number))) &&
                                       (string.IsNullOrEmpty(source) || shipment.ConsignorAddress.Country.Contains(source) || shipment.ConsignorAddress.City.Contains(source)) &&
                                       (string.IsNullOrEmpty(destination) || shipment.ConsigneeAddress.Country.Contains(destination) || shipment.ConsigneeAddress.City.Contains(destination))
                                     )
                                   ) &&
                                   !shipment.IsParent
                                   select shipment).ToList();

            foreach (var item in updatedtContent)
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
                        ShipmentId = item.Id.ToString(),
                        ShipmentMode = Enum.GetName(typeof(Contract.Enums.CarrierType), item.ShipmentMode),
                        ShipmentName = item.ShipmentName,
                        ShipmentServices = Utility.GetEnumDescription((ShipmentService)item.ShipmentService),
                        TrackingNumber = item.TrackingNumber,
                        CreatedDate = item.CreatedDate.ToString("MM/dd/yyyy"),
                        Status = Utility.GetEnumDescription((ShipmentStatus)item.Status),
                        IsFavourite = item.IsFavourite,
                        IsEnableEdit = ((ShipmentStatus)item.Status == ShipmentStatus.Error || (ShipmentStatus)item.Status == ShipmentStatus.Pending),
                        IsEnableDelete = ((ShipmentStatus)item.Status == ShipmentStatus.Error || (ShipmentStatus)item.Status == ShipmentStatus.Pending || (ShipmentStatus)item.Status == ShipmentStatus.BookingConfirmation)
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
                        ValueCurrency = Convert.ToInt32(item.ShipmentPackage.Currency),
                        PreferredCollectionDate = item.ShipmentPackage.CollectionDate.ToString(),
                        ProductIngredients = this.getPackageDetails(item.ShipmentPackage.PackageProducts),
                        ShipmentDescription = item.ShipmentPackage.PackageDescription

                    },
                    CarrierInformation = new CarrierInformationDto
                    {
                        CarrierName = item.Carrier.Name,
                        serviceLevel = item.ServiceLevel,
                        PickupDate = item.PickUpDate
                    }

                });
            }

            return this.GenerateExcelSheetForShipmentExportFunction(shipments); ;
        }



        private byte[] GenerateExcelSheetForShipmentExportFunction(List<ShipmentDto> shipments)
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
                ws.Cells["A6"].Value = "Order Submitted";
                ws.Cells["B6"].Value = "Tracking Number";
                ws.Cells["C6"].Value = "Shipment ID";
                ws.Cells["D6"].Value = "Carrier";
                ws.Cells["E6"].Value = "Orgin City";
                ws.Cells["F6"].Value = "Orgin Country";
                ws.Cells["G6"].Value = "Consignor Name";
                ws.Cells["H6"].Value = "Consignor Number";
                ws.Cells["I6"].Value = "Consignor Email";

                ws.Cells["J6"].Value = "Destination City";
                ws.Cells["K6"].Value = "Destination Country";
                ws.Cells["L6"].Value = "Consignee Name";
                ws.Cells["M6"].Value = "Consignee Number";
                ws.Cells["N6"].Value = "Consignee Email";

                ws.Cells["O6"].Value = "Status";
                ws.Cells["P6"].Value = "Shipment Mode";
                ws.Cells["Q6"].Value = "Pickup date";
                ws.Cells["R6"].Value = "Service Level";

                //Format the header for columns.
                using (ExcelRange rng = ws.Cells["A6:U6"])
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

                    var cell = ws.Cells[rowIndex, 1];
                    cell.Value = shipment.GeneralInformation.CreatedDate;

                    cell = ws.Cells[rowIndex, 2];
                    cell.Value = shipment.GeneralInformation.TrackingNumber;

                    cell = ws.Cells[rowIndex, 3];
                    cell.Value = shipment.GeneralInformation.ShipmentCode;

                    cell = ws.Cells[rowIndex, 4];
                    cell.Value = shipment.CarrierInformation.CarrierName;

                    cell = ws.Cells[rowIndex, 5];
                    cell.Value = shipment.AddressInformation.Consigner.City;

                    cell = ws.Cells[rowIndex, 6];
                    cell.Value = shipment.AddressInformation.Consigner.Country;

                    cell = ws.Cells[rowIndex, 7];
                    cell.Value = shipment.AddressInformation.Consigner.ContactName;

                    cell = ws.Cells[rowIndex, 8];
                    cell.Value = shipment.AddressInformation.Consigner.ContactNumber;

                    cell = ws.Cells[rowIndex, 9];
                    cell.Value = shipment.AddressInformation.Consigner.Email;

                    cell = ws.Cells[rowIndex, 10];
                    cell.Value = shipment.AddressInformation.Consignee.City;

                    cell = ws.Cells[rowIndex, 11];
                    cell.Value = shipment.AddressInformation.Consignee.Country;

                    cell = ws.Cells[rowIndex, 12];
                    cell.Value = shipment.AddressInformation.Consignee.ContactName;

                    cell = ws.Cells[rowIndex, 13];
                    cell.Value = shipment.AddressInformation.Consignee.ContactNumber;

                    cell = ws.Cells[rowIndex, 14];
                    cell.Value = shipment.AddressInformation.Consignee.Email;

                    cell = ws.Cells[rowIndex, 15];
                    cell.Value = shipment.GeneralInformation.Status;

                    cell = ws.Cells[rowIndex, 16];
                    cell.Value = shipment.GeneralInformation.ShipmentMode;

                    cell = ws.Cells[rowIndex, 17];
                    cell.Value = shipment.CarrierInformation.PickupDate != null ? shipment.CarrierInformation.PickupDate.Value.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture) : "";

                    cell = ws.Cells[rowIndex, 18];
                    cell.Value = shipment.CarrierInformation.serviceLevel;

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
                using (ExcelRange rng = ws.Cells["A6:U6"])
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
                    cell.Value = shipment.PickupDate;

                    cell = ws.Cells[rowIndex, 9];
                    cell.Value = shipment.DeliveryTime; ////

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
                    CreatedDate = item.CreatedDate.ToString("MM/dd/yyyy"),
                    Status = Utility.GetEnumDescription((ShipmentStatus)item.Status),
                    IsEnableEdit = ((ShipmentStatus)item.Status == ShipmentStatus.Error || (ShipmentStatus)item.Status == ShipmentStatus.Pending),
                    IsEnableDelete = ((ShipmentStatus)item.Status == ShipmentStatus.Error || (ShipmentStatus)item.Status == ShipmentStatus.Pending || (ShipmentStatus)item.Status == ShipmentStatus.BookingConfirmation),

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
                    ValueCurrency = Convert.ToInt32(item.ShipmentPackage.Currency),
                    PreferredCollectionDate = item.ShipmentPackage.CollectionDate.ToString(),
                    ProductIngredients = this.getPackageDetails(item.ShipmentPackage.PackageProducts),
                    ShipmentDescription = item.ShipmentPackage.PackageDescription,

                    //CarrierInformation                       
                    CarrierName = item.Carrier.Name,
                    serviceLevel = item.ServiceLevel,
                    PickupDate = item.PickUpDate == null ? null : DateTime.Parse(item.PickUpDate.ToString()).ToString("dd/MM/yyyy"),
                    DeliveryTime = item.ShipmentPackage.EstDeliveryDate == null ? null : DateTime.Parse(item.ShipmentPackage.EstDeliveryDate.ToString()).ToString("dd/MM/yyyy")

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

            shipmentCounts.PendingStatusCount = allShipments.Where(x => x.Status == (short)ShipmentStatus.Pending || x.Status == (short)ShipmentStatus.Error).Count();
            shipmentCounts.DeliveredStatusCount = allShipments.Where(x => x.Status == (short)ShipmentStatus.Delivered).Count();
            shipmentCounts.InTransitStatusCount = allShipments.Where(x => x.Status == (short)ShipmentStatus.Transit || x.Status == (short)ShipmentStatus.Pickup || x.Status == (short)ShipmentStatus.OutForDelivery).Count();
            shipmentCounts.ExceptionStatusCount = allShipments.Where(x => x.Status == (short)ShipmentStatus.Exception || x.Status == (short)ShipmentStatus.Claim).Count();
            shipmentCounts.BookingConfStatusCount = allShipments.Where(x => x.Status == (short)ShipmentStatus.BookingConfirmation).Count();
            shipmentCounts.allStatusCount = allShipments.Count();


            var delayed = (from shipment in allShipments
                           join package in context.ShipmentPackages on shipment.ShipmentPackageId equals package.Id
                           join history in context.ShipmentLocationHistories on shipment.Id equals history.ShipmentId
                           where shipment.Status != (short)ShipmentStatus.Delivered &&
                           history.CreatedDate > package.EstDeliveryDate.Value &&
                           !shipment.IsParent
                           select shipment).Count();

            shipmentCounts.DelayedStatusCount = delayed;

            return shipmentCounts;
            //}
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
                        CreatedDate = item.CreatedDate.ToString("MM/dd/yyyy"),
                        Status = ((ShipmentStatus)item.Status).ToString(),
                        IsEnableEdit = ((ShipmentStatus)item.Status == ShipmentStatus.Error || (ShipmentStatus)item.Status == ShipmentStatus.Pending),
                        IsEnableDelete = ((ShipmentStatus)item.Status == ShipmentStatus.Error || (ShipmentStatus)item.Status == ShipmentStatus.Pending || (ShipmentStatus)item.Status == ShipmentStatus.BookingConfirmation),
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
                        ValueCurrency = item.ShipmentPackage.Currency == null ? 0 : Convert.ToInt32(item.ShipmentPackage.Currency.Id),
                        PreferredCollectionDate = item.ShipmentPackage.CollectionDate.ToString(),
                        ProductIngredients = this.getPackageDetails(item.ShipmentPackage.PackageProducts),
                        ShipmentDescription = item.ShipmentPackage.PackageDescription

                    },
                    CarrierInformation = new CarrierInformationDto
                    {
                        CarrierName = item.Carrier.Name,
                        serviceLevel = item.ServiceLevel,
                        PickupDate = item.PickUpDate
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
                return SendShipmentDetails(new SendShipmentDetailsDto()
                {
                    ShipmentId = payment.ShipmentId,
                    PaymentResult = result,
                    UserId = payment.UserId
                });
            }

        }
    }


}
