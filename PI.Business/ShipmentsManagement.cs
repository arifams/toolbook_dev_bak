using AutoMapper;
using PI.Common;
using PI.Contract.Business;
using PI.Contract.DTOs.Common;
using PI.Contract.DTOs.Division;
using PI.Contract.DTOs.FileUpload;
using PI.Contract.DTOs.RateSheets;
using PI.Contract.DTOs.Report;
using PI.Contract.DTOs.Shipment;
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
using System.Threading.Tasks;

namespace PI.Business
{
    public class ShipmentsManagement : IShipmentManagement
    {
        public ShipmentcostList GetRateSheet(ShipmentDto currentShipment)
        {
            SISIntegrationManager sisManager = new SISIntegrationManager();
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

                using (var context = new PIContext())
                {
                    codeCurrenyString = context.Currencies.Where(c => c.Id == currentShipment.PackageDetails.ValueCurrency).Select(c => c.CurrencyCode).ToList().First();
                }

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
            currentRateSheetDetails.courier_tariff_type = "NLPARUPS:NLPARFED:USPARDHL2:USPARTNT:USPARUPS:USPARFED2:USUPSTNT:USPAREME:USPARPAE";


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
            currentRateSheetDetails.url = " www2.shipitsmarter.com/taleus/";


            return sisManager.GetRateSheetForShipment(currentRateSheetDetails);

        }

        //get the status of inbound outbound rule
        public string GetInboundoutBoundStatus(string userId, string fromCode, string toCode)
        {
            string status = "N";

            using (PIContext context = new PIContext())
            {
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

            }

            return status;
        }
        
        public ShipmentOperationResult SaveShipment(ShipmentDto addShipment)
        {

            ShipmentOperationResult result = new ShipmentOperationResult();
            CompanyManagement companyManagement = new CompanyManagement();
            Company currentcompany = companyManagement.GetCompanyByUserId(addShipment.UserId);
            long sysDivisionId = 0;
            long sysCostCenterId = 0;

            using (PIContext context = new PIContext())
            {
                var packageProductList = new List<PackageProduct>();
                addShipment.PackageDetails.ProductIngredients.ForEach(p => packageProductList.Add(new PackageProduct()
                {
                    CreatedBy = addShipment.UserId,
                    CreatedDate = DateTime.Now,
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
                    Shipment oldShipment = context.Shipments.Where(sh => sh.ShipmentCode == addShipment.GeneralInformation.ShipmentCode).FirstOrDefault();
                    oldShipmentId = oldShipment.Id;
                    oldShipment.IsParent = true;
                    context.SaveChanges();
                }

                //Mapper.CreateMap<GeneralInformationDto, Shipment>();
                Shipment newShipment = new Shipment
                {
                    ShipmentName = addShipment.GeneralInformation.ShipmentName,
                    ShipmentReferenceName = addShipment.GeneralInformation.ShipmentName + "-" + DateTime.Now.ToString("yyyyMMddHHmmssfff"),
                    ShipmentCode = null, //addShipmentResponse.CodeShipment,
                    DivisionId = addShipment.GeneralInformation.DivisionId == 0 ? sysDivisionId : (long?)addShipment.GeneralInformation.DivisionId,
                    CostCenterId = addShipment.GeneralInformation.CostCenterId == 0 ? sysCostCenterId : (long?)addShipment.GeneralInformation.CostCenterId,
                    ShipmentMode = (CarrierType) Enum.Parse(typeof(CarrierType), addShipment.GeneralInformation.ShipmentMode,true),
                    ShipmentService = (short)Utility.GetValueFromDescription<ShipmentService>(addShipment.GeneralInformation.ShipmentServices),
                    Carrier = context.Carrier.Where(c => c.Name == addShipment.CarrierInformation.CarrierName).FirstOrDefault(),
                    TrackingNumber = null, //addShipmentResponse.Awb,
                    CreatedBy = addShipment.UserId,
                    CreatedDate = DateTime.Now,
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
                        ContactName = addShipment.AddressInformation.Consignee.ContactName,
                        IsActive = true,
                        CreatedBy = addShipment.UserId,
                        CreatedDate = DateTime.Now
                    },
                    ConsignorAddress = new ShipmentAddress
                    {
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
                        ContactName = addShipment.AddressInformation.Consigner.ContactName,
                        IsActive = true,
                        CreatedBy = addShipment.UserId,
                        CreatedDate = DateTime.Now
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
                        CreatedBy = addShipment.UserId,
                        CreatedDate = DateTime.Now,
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
                        CreatedBy = addShipment.UserId,
                        UserId = addShipment.UserId,
                        CreatedDate = DateTime.Now
                    };
                    context.AddressBooks.Add(ConsignerAddressBook);

                }

                //save consignee details as new address book detail
                if (addShipment.AddressInformation.Consignee.SaveNewAddress)
                {
                    AddressBook ConsignerAddressBook = new AddressBook
                    {
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
                        CreatedBy = addShipment.UserId,
                        UserId = addShipment.UserId,
                        CreatedDate = DateTime.Now
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
                                        AppFunctionality.EditShipment: AppFunctionality.AddShipment,
                    Result = result.Status.ToString(),
                    CreatedBy = "1",
                    CreatedDate = DateTime.Now
                });
                context.SaveChanges();

            }

            return result;
        }

        public PayLaneDto GetHashForPayLane(PayLaneDto payLaneDto)
        {
            string merchantId = ConfigurationManager.AppSettings["PayLaneMerchantId"].ToString();
            string hashSalt = ConfigurationManager.AppSettings["PayLaneHashSalt"].ToString();
            string description = ConfigurationManager.AppSettings["PayLaneDescription"].ToString();

            //(salt + "|" + description + "|" + amount + "|" + currency + "|" + transaction_type)
            string buildStringForHash = string.Format("{0}|{1}|{2}|{3}|{4}", hashSalt, description, payLaneDto.Amount, payLaneDto.Currency, payLaneDto.TransactionType);
            return new PayLaneDto()
            {
                MerchantId = merchantId,
                Description = description,
                Hash = Hash(buildStringForHash)
            };
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

        //get shipments by User
        public PagedList GetAllShipmentsbyUser(string status, string userId, DateTime? startDate, DateTime? endDate,
                                               string number, string source, string destination)
        {
            int page = 1;
            int pageSize = 10;
            CompanyManagement company = new CompanyManagement();
            IList<DivisionDto> divisions = null;
            IList<int> divisionList = new List<int>();
            List<Shipment> Shipments = new List<Shipment>();
            var pagedRecord = new PagedList();
            if (userId == null)
            {
                return null;
            }
            string role = this.GetUserRoleById(userId);
            if (role == "BusinessOwner" || role == "Manager")
            {
                divisions = this.GetAllDivisionsinCompany(userId);
            }
            else if (role == "Supervisor")
            {
                divisions = company.GetAssignedDivisions(userId);
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
                           (string.IsNullOrEmpty(status) || (status == "Active" ? shipment.Status != (short)ShipmentStatus.Delivered : shipment.Status == (short)ShipmentStatus.Delivered)) &&
                           (startDate == null || (shipment.ShipmentPackage.EarliestPickupDate >= startDate && shipment.ShipmentPackage.EarliestPickupDate <= endDate)) &&
                           (string.IsNullOrEmpty(number) || shipment.TrackingNumber.Contains(number) || shipment.ShipmentCode.Contains(number)) &&
                           (string.IsNullOrEmpty(source) || shipment.ConsignorAddress.Country.Contains(source) || shipment.ConsignorAddress.City.Contains(source)) &&
                           (string.IsNullOrEmpty(destination) || shipment.ConsigneeAddress.Country.Contains(destination) || shipment.ConsigneeAddress.City.Contains(destination)) &&
                           !shipment.IsParent
                           select shipment).ToList();

            // Update retrieve shipment list status from SIS.
            foreach (var shipment in content)
            {
                if (shipment.Status != ((short)ShipmentStatus.Delivered) && !string.IsNullOrWhiteSpace(shipment.TrackingNumber))
                {
                    UpdateLocationHistory(shipment.Carrier.Name, shipment.TrackingNumber, shipment.ShipmentCode, "taleus", shipment.Id);
                }
            }

            // Get new updated shipment list again.
            var updatedtContent = (from shipment in Shipments
                       where shipment.IsDelete == false &&
                       (string.IsNullOrEmpty(status) || (status == "Active" ? shipment.Status != (short)ShipmentStatus.Delivered : shipment.Status == (short)ShipmentStatus.Delivered)) &&
                       (startDate == null || (shipment.ShipmentPackage.EarliestPickupDate >= startDate && shipment.ShipmentPackage.EarliestPickupDate <= endDate)) &&
                       (string.IsNullOrEmpty(number) || shipment.TrackingNumber.Contains(number) || shipment.ShipmentCode.Contains(number)) &&
                       (string.IsNullOrEmpty(source) || shipment.ConsignorAddress.Country.Contains(source) || shipment.ConsignorAddress.City.Contains(source)) &&
                       (string.IsNullOrEmpty(destination) || shipment.ConsigneeAddress.Country.Contains(destination) || shipment.ConsigneeAddress.City.Contains(destination)) &&
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
                            ContactNumber = item.ConsigneeAddress.ContactName,
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
                            ContactNumber = item.ConsignorAddress.ContactName,
                            Email = item.ConsignorAddress.EmailAddress,
                            Number = item.ConsignorAddress.Number
                        }
                    },
                    GeneralInformation = new GeneralInformationDto
                    {
                        CostCenterId = item.CostCenterId.GetValueOrDefault(),
                        DivisionId = item.DivisionId.GetValueOrDefault(),
                        ShipmentCode = item.ShipmentCode,
                        ShipmentMode = Enum.GetName(typeof(CarrierType), item.ShipmentMode),
                        ShipmentName = item.ShipmentName,
                        ShipmentServices = Utility.GetEnumDescription((ShipmentService)item.ShipmentService),
                        TrackingNumber = item.TrackingNumber,
                        CreatedDate = item.CreatedDate.ToString("MM/dd/yyyy"),
                        Status = Utility.GetEnumDescription((ShipmentStatus)item.Status),
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

            pagedRecord.TotalRecords = Shipments.Count();
            pagedRecord.CurrentPage = page;
            pagedRecord.PageSize = pageSize;
            pagedRecord.TotalPages = (int)Math.Ceiling((decimal)pagedRecord.TotalRecords / pagedRecord.PageSize);

            return pagedRecord;
        }

        public string GetUserRoleById(string userId)
        {
            using (PIContext context = new PIContext())
            {
                string roleId = context.Users.Where(u => u.Id == userId).FirstOrDefault().Roles.FirstOrDefault().RoleId;
                string roleName = context.Roles.Where(r => r.Id == roleId).Select(r => r.Name).FirstOrDefault();
                return roleName;
            }

        }

        public IList<Shipment> GetshipmentsByDivisionId(long divid)
        {
            IList<Shipment> currentShipments = null;
            using (PIContext context = new PIContext())
            {
                //currentShipments = (from shipment in context.Shipments
                //                    join shipmentAddress1 in context.ShipmentAddresses on shipment.ConsigneeAddress.Id equals shipmentAddress1.Id
                //                    join shipmentAddress2 in context.ShipmentAddresses on shipment.ConsigneeAddress.Id equals shipmentAddress2.Id
                //                    join shipmentPackages in context.ShipmentPackages on shipment.ShipmentPackageId equals shipmentPackages.Id
                //                    where shipment.DivisionId == divid
                //                    select shipment).ToList();
                currentShipments = context.Shipments.Where(x => x.DivisionId == divid).ToList();

            }

            return currentShipments;
        }

        //get shipments by user ID
        public IList<Shipment> GetshipmentsByUserId(string userId)
        {
            IList<Shipment> currentShipments = null;
            using (PIContext context = new PIContext())
            {
                //currentShipments = (from shipment in context.Shipments
                //                    join shipmentAddress1 in context.ShipmentAddresses on shipment.ConsigneeAddress.Id equals shipmentAddress1.Id
                //                    join shipmentAddress2 in context.ShipmentAddresses on shipment.ConsigneeAddress.Id equals shipmentAddress2.Id
                //                    join shipmentPackages in context.ShipmentPackages on shipment.ShipmentPackageId equals shipmentPackages.Id
                //                    where shipment.CreatedBy == userId
                //                    select shipment).ToList();

                currentShipments = context.Shipments.Where(x => x.CreatedBy == userId).ToList();

            }

            return currentShipments;
        }

        //get shipments by user ID and created date
        public List<Shipment> GetshipmentsByUserIdAndCreatedDate(string userId, DateTime createdDate, string carreer)
        {
            List<Shipment> currentShipments = null;
            using (PIContext context = new PIContext())
            {               
                currentShipments = context.Shipments.Where(x => x.CreatedBy == userId && x.CreatedDate.Year == createdDate.Year && x.CreatedDate.Month == createdDate.Month && x.CreatedDate.Day == createdDate.Day && x.Carrier.Name== carreer && !string.IsNullOrEmpty(x.TrackingNumber)).ToList();
            }
            return currentShipments;
        }

        //get shipments by shipment reference
        public List<Shipment> GetshipmentsByReference(string userId, string reference)
        {
            List<Shipment> currentShipments = null;
            using (PIContext context = new PIContext())
            {
                currentShipments = context.Shipments.Where(x => x.CreatedBy == userId && x.ShipmentReferenceName.Contains(reference) && !string.IsNullOrEmpty(x.TrackingNumber)).ToList();
            }
            return currentShipments;
        }


        //update shipment status manually only by admin
        public int UpdateshipmentStatusManually(string codeShipment, string status)
        {
            using (PIContext context = new PIContext())
            {
                var shipment = (from shipmentinfo in context.Shipments
                                where shipmentinfo.ShipmentCode == codeShipment
                                select shipmentinfo).FirstOrDefault();
                if (shipment != null)
                {
                    shipment.Status = (short)Enum.Parse(typeof(ShipmentStatus), status);
                    shipment.ManualStatusUpdatedDate = DateTime.Now;
                }
                context.SaveChanges();
                return 1;
            }

        }

        public void UpdateShipmentStatus(string codeShipment, short status)
        {
            using (PIContext context = new PIContext())
            {
                var shipment = (from shipmentinfo in context.Shipments
                                where shipmentinfo.ShipmentCode == codeShipment
                                select shipmentinfo).FirstOrDefault();
                if (shipment != null)
                {
                    shipment.Status = status;
                }
                context.SaveChanges();
            }
        }

        public Shipment GetShipmentByShipmentCode(string codeShipment)
        {
            Shipment currentShipment = new Shipment();

            using (PIContext context = new PIContext())
            {
                currentShipment = (from shipment in context.Shipments
                                   where shipment.ShipmentCode == codeShipment
                                   select shipment).FirstOrDefault();
            }

            return currentShipment;
        }

        //get shipments by ID
        public ShipmentDto GetshipmentById(string shipmentId)
        {
            ShipmentDto currentShipmentDto = null;
            Shipment currentShipment = null;
            long tenantId = 0;

            using (PIContext context = new PIContext())
            {
                currentShipment = context.Shipments.Where(x => x.ShipmentCode.ToString() == shipmentId).FirstOrDefault();

                //currentShipment = (from shipment in context.Shipments.Include("Division.Company")
                //                   join shipmentAddress1 in context.ShipmentAddresses on shipment.ConsigneeAddress.Id equals shipmentAddress1.Id
                //                   join shipmentAddress2 in context.ShipmentAddresses on shipment.ConsigneeAddress.Id equals shipmentAddress2.Id
                //                   join shipmentPackages in context.ShipmentPackages on shipment.ShipmentPackageId equals shipmentPackages.Id
                //                   where shipment.ShipmentCode.ToString() == shipmentId
                //                   select shipment).FirstOrDefault();

                tenantId = currentShipment.Division.Company.TenantId;
            }
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
                    ShipmentMode = Enum.GetName(typeof(CarrierType), currentShipment.ShipmentMode),
                    ShipmentName = currentShipment.ShipmentName,
                    ShipmentServices = Utility.GetEnumDescription((ShipmentService)currentShipment.ShipmentService),
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
                    ShipmentDescription = currentShipment.ShipmentPackage.PackageDescription

                },
                CarrierInformation = new CarrierInformationDto
                {
                    CarrierName = currentShipment.Carrier.Name,
                    serviceLevel = currentShipment.ServiceLevel,
                    PickupDate = currentShipment.PickUpDate
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

            using (var context = new PIContext())
            {
                Shipment shipment = context.Shipments.Where(sh => sh.Id == sendShipmentDetails.ShipmentId).FirstOrDefault();

                // Validate the already communicated with SIS (If browser refresh, this method invokes. Using this validate shipment code is already there)
                if (!string.IsNullOrWhiteSpace(shipment.ShipmentCode))
                {
                    result.Status = Status.Error;
                    result.Message = "Shipment is already added";
                    return result;
                }

                if (shipment.ShipmentPaymentTypeId == 2) // Online payment.
                {
                    // Added payment data
                    shipment.ShipmentPayment = new ShipmentPayment()
                    {
                        CreatedBy = sendShipmentDetails.UserId,
                        CreatedDate = DateTime.Now,
                        IsActive = true,
                        SaleId = sendShipmentDetails.PayLane.SaleId,
                        Status = sendShipmentDetails.PayLane.Status // TODO : H- Use enum.
                    };

                    context.SaveChanges();

                    if (sendShipmentDetails.PayLane.Status != "PERFORMED")
                    {
                        result.Status = Status.PaymentError;
                        result.Message = "Error occured when adding payment. Please contact Parcel International";
                        return result;
                    }
                }

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
                        ShipmentServices = Utility.GetEnumDescription((ShipmentService)shipment.ShipmentService)
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
                        PreferredCollectionDate = string.Format("{0}-{1}-{2}", shipment.ShipmentPackage.CollectionDate.Day, shipment.ShipmentPackage.CollectionDate.ToString("MMM", CultureInfo.InvariantCulture), shipment.ShipmentPackage.CollectionDate.Year), //"18-Mar-2016"
                        CmLBS = shipment.ShipmentPackage.WeightMetricId == 1,
                        VolumeCMM = shipment.ShipmentPackage.VolumeMetricId == 1,
                        ProductIngredients = shipmentProductIngredientsList,
                        ShipmentDescription = shipment.ShipmentPackage.PackageDescription,
                        DeclaredValue = shipment.ShipmentPackage.InsuranceDeclaredValue
                    }
                };

                // Add Shipment to SIS.
                response = new SISIntegrationManager().SendShipmentDetails(shipmentDto);

                shipment.ShipmentCode = response.CodeShipment;
                shipment.TrackingNumber = response.Awb;
                result.AddShipmentXML = response.AddShipmentXML;

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

                    // If response.PDF is empty, get from following url.
                    if (string.IsNullOrWhiteSpace(response.PDF))
                    {
                        ICarrierIntegrationManager sisManager = new SISIntegrationManager();
                        result.LabelURL = sisManager.GetLabel(shipment.ShipmentCode);
                    }
                    else
                    {
                        result.LabelURL = response.PDF;
                    }
                    result.ShipmentId = shipment.Id;
                    shipment.Status = (short)ShipmentStatus.BookingConfirmation;

                    #region Send Booking confirmation email, for successful shipments


                    #endregion

                }

                context.SaveChanges();
                return result;
            }
        }

        public List<DivisionDto> GetAllDivisionsinCompany(string userId)
        {
            List<DivisionDto> divisionList = new List<DivisionDto>();
            CompanyManagement companyManagement = new CompanyManagement();
            Company currentcompany = companyManagement.GetCompanyByUserId(userId);

            if (currentcompany == null)
            {
                return null;
            }

            using (var context = new PIContext())//PIContext.Get())
            {
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
            }

            return divisionList;
        }

        //Delete shipment
        public int DeleteShipment(string shipmentCode, string trackingNumber, string carrierName, bool isAdmin)
        {

            SISIntegrationManager sisManager = new SISIntegrationManager();
            string URL = "http://parcelinternational.pro/status/" + carrierName + "/" + trackingNumber;
            if (isAdmin)
            {
                sisManager.DeleteShipment(shipmentCode);
                return 1;
            }
            else
            {
            if (sisManager.GetShipmentStatus(URL, shipmentCode) == "")
            {
                sisManager.DeleteShipment(shipmentCode);
                return 1;
            }
            else
            {
                return 2;
            }

        }

        }

        //get the location history list 
        public StatusHistoryResponce GetLocationHistoryInfoForShipment(string carrier, string trackingNumber, string codeShipment, string environment)
        {
            StatusHistoryResponce locationHistory = new StatusHistoryResponce();
            ShipmentDto currentShipmet = this.GetshipmentById(codeShipment);
            info info = new info();

            if (currentShipmet.GeneralInformation.Status == ((short)ShipmentStatus.Delivered).ToString())
            {
                locationHistory = this.getUpdatedShipmentHistoryFromDB(codeShipment);
                Shipment currentShipment = GetShipmentByShipmentCode(codeShipment);
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
            SISIntegrationManager sisManager = new SISIntegrationManager();
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
                Shipment currentShipment = GetShipmentByShipmentCode(codeShipment);
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
        public StatusHistoryResponce GetTrackAndTraceInfo(string carrier, string trackingNumber)
        {
            string environment = "taleus";
            StatusHistoryResponce trackingInfo = new StatusHistoryResponce();
            Shipment currentShipment = this.GetShipmentByTrackingNo(trackingNumber);
            SISIntegrationManager sisManager = new SISIntegrationManager();
            if (currentShipment != null)
            {
                trackingInfo = sisManager.GetUpdatedShipmentStatusehistory(carrier, trackingNumber, currentShipment.ShipmentCode, environment);
            }
            //  trackingInfo = sisManager.GetUpdatedShipmentStatusehistory(carrier, "8925859014", "38649998", environment);
            return trackingInfo;
        }


        //get shipment details by tracking number
        public Shipment GetShipmentByTrackingNo(string trackingNo)
        {
            using (PIContext context = new PIContext())
            {
                var currentShipment = (from shipment in context.Shipments
                                       where shipment.TrackingNumber == trackingNo
                                       select shipment).SingleOrDefault();

                return currentShipment;
            }


        }

        //update status hisory with latest statuses and locations
        public void UpdateStatusHistories(StatusHistoryResponce statusHistory, long ShipmntId)
        {

            using (PIContext context = new PIContext())
            {
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
                    locationHistory.CreatedDate = DateTime.Now;
                    context.ShipmentLocationHistories.Add(locationHistory);
                    context.SaveChanges();
                }
                List<ShipmentLocationHistory> histories = this.GetShipmentLocationHistoryByShipmentId(ShipmntId);
                foreach (var item in histories)
                {
                    foreach (var his in statusHistory.history.Items)
                    {
                        if ((his.location.geo != null && item.Longitude.ToString() == his.location.geo.lng && item.Latitude.ToString() == his.location.geo.lat) || (string.IsNullOrEmpty(his.location.city) && item.City.Equals(his.location.city)))
                        {
                            foreach (var activityItems in his.activity.Items)
                            {
                                LocationActivity activity = new LocationActivity();
                                activity.ShipmentLocationHistoryId = item.Id;
                                activity.Status = activityItems.status;
                                activity.Time = Convert.ToDateTime(activityItems.timestamp.time);
                                activity.Date = Convert.ToDateTime(activityItems.timestamp.date);
                                activity.CreatedDate = DateTime.Now;
                                context.LocationActivities.Add(activity);
                                context.SaveChanges();
                            }
                        }

                    }
                }
            }



        }

        //get updated status history from DB
        public StatusHistoryResponce getUpdatedShipmentHistoryFromDB(string codeShipment)
        {
            StatusHistoryResponce statusHistory = new StatusHistoryResponce();
            Shipment currentShipment = this.GetShipmentByCodeShipment(codeShipment);

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
                        date = activ.Date.ToString(),
                        time = activ.Time.ToString(),

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
            using (PIContext context = new PIContext())
            {
                List<LocationActivity> activities = (from activity in context.LocationActivities
                                                     where activity.ShipmentLocationHistoryId == historyId
                                                     select activity).ToList();

                context.LocationActivities.RemoveRange(activities);
                context.SaveChanges();

            }
        }

        //get shipmentLocation from database
        public void DeleteShipmentLocationHistoryByShipmentId(long shipmentId)
        {

            using (PIContext context = new PIContext())
            {
                List<ShipmentLocationHistory> histories = (from history in context.ShipmentLocationHistories
                                                           where history.ShipmentId == shipmentId
                                                           select history).ToList();

                context.ShipmentLocationHistories.RemoveRange(histories);
                context.SaveChanges();
            }
        }


        public List<LocationActivity> GetLocationActivityByLocationHistoryId(long historyId)
        {
            using (PIContext context = new PIContext())
            {
                List<LocationActivity> histories = (from activity in context.LocationActivities
                                                    where activity.ShipmentLocationHistoryId == historyId
                                                    select activity).ToList();

                return histories;
            }
        }

        //get shipmentLocation from database
        public List<ShipmentLocationHistory> GetShipmentLocationHistoryByShipmentId(long shipmentId)
        {

            using (PIContext context = new PIContext())
            {
                List<ShipmentLocationHistory> histories = (from history in context.ShipmentLocationHistories
                                                           where history.ShipmentId == shipmentId
                                                           select history).ToList();

                return histories;
            }
        }

        //get the shipment by code shipment
        public Shipment GetShipmentByCodeShipment(string codeShipment)
        {
            using (PIContext context = new PIContext())
            {
                Shipment shipmentContent = (from shipment in context.Shipments
                                            where shipment.ShipmentCode == codeShipment
                                            select shipment).FirstOrDefault();

                return shipmentContent;
            }

        }


        /// <summary>
        /// Insert shipment record
        /// </summary>
        /// <param name="fileDetails"></param>
        public void InsertShipmentDocument(FileUploadDto fileDetails)
        {
            using (var context = new PIContext())
            {
                var shipement = context.Shipments.Where(x => x.ShipmentCode == fileDetails.CodeReference).SingleOrDefault();

                context.ShipmentDocument.Add(new ShipmentDocument
                {
                    TenantId = fileDetails.TenantId,
                    ShipmentId = shipement.Id,
                    ClientFileName = fileDetails.ClientFileName,
                    DocumentType = (int)fileDetails.DocumentType,
                    UploadedFileName = fileDetails.UploadedFileName,
                    IsActive = true,
                    CreatedDate = DateTime.Now,
                    CreatedBy = "1"
                });

                context.SaveChanges();
            }
        }


        public List<FileUploadDto> GetAvailableFilesForShipmentbyTenant(string shipmentCode, string userId)
        {
            List<FileUploadDto> returnList = new List<FileUploadDto>();
            // Make absolute link
            string baseUrl = ConfigurationManager.AppSettings["PIBlobStorage"];

            CompanyManagement companyManagement = new CompanyManagement();
            var tenantId = companyManagement.GettenantIdByUserId(userId);

            using (var context = new PIContext())
            {
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
            }
            return returnList;
        }



        //get shipments by User
        public PagedList GetAllPendingShipmentsbyUser(string userId, DateTime? startDate, DateTime? endDate,
                                               string number)
        {
            int page = 1;
            int pageSize = 10;
            CompanyManagement company = new CompanyManagement();
            IList<DivisionDto> divisions = null;
            IList<int> divisionList = new List<int>();
            List<Shipment> Shipments = new List<Shipment>();
            var pagedRecord = new PagedList();
            if (userId == null)
            {
                return null;
            }
            string role = this.GetUserRoleById(userId);
            if (role == "BusinessOwner" || role == "Manager")
            {
                divisions = this.GetAllDivisionsinCompany(userId);
            }
            else if (role == "Supervisor")
            {
                divisions = company.GetAssignedDivisions(userId);
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
                        ShipmentMode = Enum.GetName(typeof(CarrierType), item.ShipmentMode),
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
            List<Shipment> shipmentList = new List<Shipment>();
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
                        ShipmentMode = Enum.GetName(typeof(CarrierType), item.ShipmentMode),
                        ShipmentName = item.ShipmentName,
                        ShipmentReferenceName = item.ShipmentReferenceName,
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

        //    using (var context = new PIContext())
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
        //                CreatedDate = DateTime.Now
        //            });
        //            context.SaveChanges();

        //        }

        //    }

        //}

        public CommercialInvoiceDto GetshipmentByShipmentCodeForInvoice(string shipmentCode)
        {

            Shipment currentShipment = null;
            long tenantId = 0;
            CommercialInvoiceDto invocieDto = null;

            using (PIContext context = new PIContext())
            {
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
                        Quantity = p.Quantity
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
                            ShipmentDescription = currentShipment.ShipmentPackage.PackageDescription

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
                        HSCode = currentShipment.CommercialInvoice.HSCode
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
                            ShipmentDescription = currentShipment.ShipmentPackage.PackageDescription

                        },
                        Item = new InvoiceItemDto() { LineItems = new List<InvoiceItemLineDto>() },
                        VatNo = currentShipment.Division.Company.VATNumber,
                        HSCode = currentShipment.ShipmentPackage.HSCode
                    };
                }
            }





            return invocieDto;
        }


        public void DeleteFileInDB(FileUploadDto fileDetails)
        {
            using (var context = new PIContext())
            {
                var document = context.ShipmentDocument.Where(x => x.Id == fileDetails.Id).SingleOrDefault();

                context.ShipmentDocument.Remove(document);
                context.SaveChanges();
            }
        }

        public ShipmentOperationResult SaveCommercialInvoice(CommercialInvoiceDto addInvoice)
        {

            ShipmentOperationResult result = new ShipmentOperationResult();
            //CompanyManagement companyManagement = new CompanyManagement();
            //Company currentcompany = companyManagement.GetCompanyByUserId(addShipment.UserId);
            //long sysDivisionId = 0;
            //long sysCostCenterId = 0;

            using (PIContext context = new PIContext())
            {
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
                    CreatedDate = DateTime.Now,
                    IsActive = true
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
                        CreatedDate = DateTime.Now,
                        IsActive = true,
                        InvoiceItemLines = invoiceItemLineList
                    }
                };

                context.CommercialInvoices.Add(invoice);
                context.SaveChanges();
            }
            return result;
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


        public PagedList GetAllShipmentByCompanyId(string companyId)
        {
            int page = 1;
            int pageSize = 10;
            var pagedRecord = new PagedList();

            pagedRecord.Content = new List<ShipmentDto>();

            using (var context = new PIContext())
            {
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

                pagedRecord.TotalRecords = content.Count();
                pagedRecord.CurrentPage = page;
                pagedRecord.PageSize = pageSize;
                pagedRecord.TotalPages = (int)Math.Ceiling((decimal)pagedRecord.TotalRecords / pagedRecord.PageSize);

                return pagedRecord;
            }
        }

        public PagedList loadAllShipmentsFromCompanyAndSearch(string companyId, string status = null, DateTime? startDate = null, DateTime? endDate = null,
                                         string number = null, string source = null, string destination = null)
        {
            int page = 1;
            int pageSize = 10;
            var pagedRecord = new PagedList();

            pagedRecord.Content = new List<ShipmentDto>();

            using (var context = new PIContext())
            {
                var content = (from shipment in context.Shipments                              
                               where  shipment.Division.CompanyId.ToString() == companyId &&
                               (string.IsNullOrEmpty(status) || (status == "Active" ? shipment.Status != (short)ShipmentStatus.Delivered : shipment.Status == (short)ShipmentStatus.Delivered)) &&
                               (startDate == null || (shipment.ShipmentPackage.EarliestPickupDate >= startDate && shipment.ShipmentPackage.EarliestPickupDate <= endDate)) &&
                               (string.IsNullOrEmpty(number) || shipment.TrackingNumber.Contains(number) || shipment.ShipmentCode.Contains(number)) &&
                               (string.IsNullOrEmpty(source) || shipment.ConsignorAddress.Country.Contains(source) || shipment.ConsignorAddress.City.Contains(source)) &&
                               (string.IsNullOrEmpty(destination) || shipment.ConsigneeAddress.Country.Contains(destination) || shipment.ConsigneeAddress.City.Contains(destination)) 
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

                pagedRecord.TotalRecords = content.Count();
                pagedRecord.CurrentPage = page;
                pagedRecord.PageSize = pageSize;
                pagedRecord.TotalPages = (int)Math.Ceiling((decimal)pagedRecord.TotalRecords / pagedRecord.PageSize);

                return pagedRecord;
            }
        }


        public string ShipmentReport(string userId, string languageId, ReportType reportType, 
                                     short carrierId = 0, long companyId = 0, DateTime? startDate = null, 
                                     DateTime? endDate = null)
        {
            using (PIContext context = new PIContext())
            {
                var roleId = context.Users.Where(u => u.Id == userId).FirstOrDefault().Roles.FirstOrDefault().RoleId;
                
                var roleName = context.Roles.Where(r => r.Id == roleId).FirstOrDefault().Name;
                
                IList<Shipment> shipmentList = null;

                if (roleName == "Admin" || roleName == "BusinessOwner" )
                {
                    if (roleName == "BusinessOwner")
                    {
                        CompanyManagement companyManagement = new CompanyManagement();
                        companyId = companyManagement.GetCompanyByUserId(userId).Id;
                    }

                    shipmentList =
                        context.Shipments.Where(s => s.Division.CompanyId == companyId ||
                        (carrierId != 0 && s.CarrierId == carrierId) ||
                        (startDate != null && startDate <= s.PickUpDate) ||
                        (endDate != null && s.PickUpDate <= endDate)
                    ).ToList();
                }
                else if (roleName == "Manager")
                {
                    shipmentList =
                        context.Shipments.Where(s => s.Division.UserInDivisions.Any(u => u.UserId == userId) ||
                        (carrierId != 0 && s.CarrierId == carrierId) ||
                        (startDate != null && startDate <= s.PickUpDate) ||
                        (endDate != null && s.PickUpDate <= endDate)
                    ).ToList();
                }

                // If empty list, return empty list by message result is empty.
                if (shipmentList == null)
                    return null;

                // Update retrieve shipment list status from SIS.
                foreach (var shipment in shipmentList)
                {
                    if (shipment.Status != ((short)ShipmentStatus.Delivered) && !string.IsNullOrWhiteSpace(shipment.TrackingNumber))
                    {
                        UpdateLocationHistory(shipment.Carrier.Name, shipment.TrackingNumber, shipment.ShipmentCode, "taleus", shipment.Id);
                    }
                }

                // Get updated list again.
                var UpdatedShipmentList = context.Shipments.Where(sh => shipmentList.Any(s => s.Id == sh.Id)).ToList();

                // Get shipment data, delivery date, carrier details, customer data, address details, cost center details and division details.
                IList<ShipmentReportDto> reportList = new List<ShipmentReportDto>();

                foreach (var item in UpdatedShipmentList)
                {
                    reportList.Add(new ShipmentReportDto
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
                                ContactNumber = item.ConsigneeAddress.ContactName,
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
                                ContactNumber = item.ConsignorAddress.ContactName,
                                Email = item.ConsignorAddress.EmailAddress,
                                Number = item.ConsignorAddress.Number
                            }
                        },
                        GeneralInformation = new GeneralInformationDto
                        {
                            CostCenterId = item.CostCenterId.GetValueOrDefault(),
                            DivisionId = item.DivisionId.GetValueOrDefault(),
                            ShipmentCode = item.ShipmentCode,
                            ShipmentMode = Enum.GetName(typeof(CarrierType), item.ShipmentMode),
                            ShipmentName = item.ShipmentName,
                            ShipmentServices = Utility.GetEnumDescription((ShipmentService)item.ShipmentService),
                            TrackingNumber = item.TrackingNumber,
                            CreatedDate = item.CreatedDate.ToString("MM/dd/yyyy"),
                            Status = Utility.GetEnumDescription((ShipmentStatus)item.Status),
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


            }

            return "";
        }
    }


}
