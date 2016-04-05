using AutoMapper;
using PI.Common;
using PI.Contract.Business;
using PI.Contract.DTOs.Common;
using PI.Contract.DTOs.Division;
using PI.Contract.DTOs.FileUpload;
using PI.Contract.DTOs.RateSheets;
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
            currentRateSheetDetails.dg = "NO";
            currentRateSheetDetails.dg_type = "";
            currentRateSheetDetails.account = "";
            currentRateSheetDetails.code_customer = "";
            currentRateSheetDetails.ind_delivery_inside = "";
            currentRateSheetDetails.url = " www2.shipitsmarter.com/taleus/";


            return sisManager.GetRateSheetForShipment(currentRateSheetDetails);

        }

        //get the shipment status histories for shipment overview 
        public List<ShipmentStatusHistoryDto> GetShipmentStatusListByShipmentId(string shipmentId)
        {
            //List<ShipmentStatusHistoryDto> statusHistoryList = new List<ShipmentStatusHistoryDto>();

            //using (PIContext context= new PIContext())
            //{
            // var statusList = (from statusHistory in context.ShipmentStatusHistory
            //                         where statusHistory.ShipmentId.ToString() == shipmentId
            //                         select statusHistory).ToList();

            //    foreach (var item in statusList)
            //    {
            //        statusHistoryList.Add(new ShipmentStatusHistoryDto {
            //            NewStatus=item.NewStatus,
            //            OldStatus=item.OldStatus,
            //            ShipmentId=item.ShipmentId,
            //            CreatedDate=item.CreatedDate
            //        });
            //    }
            //}

            //return statusHistoryList;

            return null;
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

                //Mapper.CreateMap<GeneralInformationDto, Shipment>();
                Shipment newShipment = new Shipment
                {
                    ShipmentName = addShipment.GeneralInformation.ShipmentName,
                    ShipmentCode = null, //addShipmentResponse.CodeShipment,
                    DivisionId = addShipment.GeneralInformation.DivisionId == 0 ? sysDivisionId : (long?)addShipment.GeneralInformation.DivisionId,
                    CostCenterId = addShipment.GeneralInformation.CostCenterId == 0 ? sysCostCenterId : (long?)addShipment.GeneralInformation.CostCenterId,
                    ShipmentMode = addShipment.GeneralInformation.ShipmentMode,
                    ShipmentService = (short)Utility.GetValueFromDescription<ShipmentService>(addShipment.GeneralInformation.ShipmentServices),
                    //ShipmentTypeCode = addShipment.GeneralInformation.ShipmentTypeCode,
                    //ShipmentTermCode = addShipment.GeneralInformation.ShipmentTermCode,
                    CarrierName = addShipment.CarrierInformation.CarrierName,
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
                        PackageProducts = packageProductList
                    }
                };

                try
                {
                    context.Shipments.Add(newShipment);
                    context.SaveChanges();

                    //context.ShipmentStatusHistory.Add(new ShipmentStatusHistory { ShipmentId = newShipment.Id, Status = "Pending", CreatedBy = addShipment.UserId, CreatedDate= DateTime.Now,IsActive = true });
                    //context.SaveChanges();

                    result.ShipmentId = newShipment.Id;
                    result.Status = Status.Success;

                    // If success fully saved and is invoice, then add to SIS.
                    //if (addShipment.GeneralInformation.ShipmentPaymentTypeId == 1)
                    //{
                    //    AddShipmentResponse addShipmentResponse = new SISIntegrationManager().SendShipmentDetails(addShipment);
                    //    result.AddShipmentXML = addShipmentResponse.AddShipmentXML;

                    //    if (string.IsNullOrWhiteSpace(addShipmentResponse.Awb))
                    //    { 
                    //        result.Status = Status.Error;
                    //        result.Message = "Error occured while adding shipment";
                    //    }
                    //    else
                    //    {
                    //        result.Status = Status.Success;
                    //        result.Message = "Shipment added successfully";

                    //        result.LabelURL = addShipmentResponse.PDF;

                    //        // Update shipment entity.
                    //        newShipment.ShipmentCode = addShipmentResponse.CodeShipment;
                    //        newShipment.TrackingNumber = addShipmentResponse.Awb;
                    //        newShipment.Status = (short)ShipmentStatus.BookingConfirmation;
                    //        context.SaveChanges();
                    //    }
                    //}
                }
                catch (Exception ex)
                {
                    //throw ex;
                    result.ShipmentId = 0;
                    result.Status = Status.Error;
                }
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
                               //(string.IsNullOrEmpty(status) || shipment.ShipmentStatuses.Any(x => (status == "Active" ? x.NewStatus != "Delivered" : x.NewStatus == "Delivered"))) &&
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
                        ShipmentMode = item.ShipmentMode,
                        ShipmentName = item.ShipmentName,
                        //ShipmentTermCode = item.ShipmentTermCode,
                        //ShipmentTypeCode = item.ShipmentTypeCode,


                        TrackingNumber = item.TrackingNumber,
                        CreatedDate = item.CreatedDate.ToString("MM/dd/yyyy"),
                        //Status = (item.ShipmentStatuses.Count() == 0) ? null :
                        //         item.ShipmentStatuses.OrderByDescending(x => x.CreatedDate).FirstOrDefault().NewStatus
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
                        CarrierName = item.CarrierName,
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
                currentShipments = (from shipment in context.Shipments
                                    join shipmentAddress1 in context.ShipmentAddresses on shipment.ConsigneeAddress.Id equals shipmentAddress1.Id
                                    join shipmentAddress2 in context.ShipmentAddresses on shipment.ConsigneeAddress.Id equals shipmentAddress2.Id
                                    join shipmentPackages in context.ShipmentPackages on shipment.ShipmentPackageId equals shipmentPackages.Id
                                    where shipment.DivisionId == divid
                                    select shipment).ToList();

            }

            return currentShipments;
        }

        //get shipments by user ID
        public IList<Shipment> GetshipmentsByUserId(string userId)
        {
            IList<Shipment> currentShipments = null;
            using (PIContext context = new PIContext())
            {
                currentShipments = (from shipment in context.Shipments
                                    join shipmentAddress1 in context.ShipmentAddresses on shipment.ConsigneeAddress.Id equals shipmentAddress1.Id
                                    join shipmentAddress2 in context.ShipmentAddresses on shipment.ConsigneeAddress.Id equals shipmentAddress2.Id
                                    join shipmentPackages in context.ShipmentPackages on shipment.ShipmentPackageId equals shipmentPackages.Id
                                    where shipment.CreatedBy == userId
                                    select shipment).ToList();

            }

            return currentShipments;
        }

        //get shipments by ID
        public ShipmentDto GetshipmentById(string shipmentId)
        {
            ShipmentDto currentShipmentDto = null;

            Shipment currentShipment = null;
            using (PIContext context = new PIContext())
            {
                currentShipment = (from shipment in context.Shipments
                                   join shipmentAddress1 in context.ShipmentAddresses on shipment.ConsigneeAddress.Id equals shipmentAddress1.Id
                                   join shipmentAddress2 in context.ShipmentAddresses on shipment.ConsigneeAddress.Id equals shipmentAddress2.Id
                                   join shipmentPackages in context.ShipmentPackages on shipment.ShipmentPackageId equals shipmentPackages.Id
                                   where shipment.ShipmentCode.ToString() == shipmentId
                                   select shipment).FirstOrDefault();

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
                        ContactNumber = currentShipment.ConsigneeAddress.ContactName,
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
                        ContactNumber = currentShipment.ConsignorAddress.ContactName,
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
                    ShipmentMode = currentShipment.ShipmentMode,
                    ShipmentName = currentShipment.ShipmentName,
                    ShipmentServices = Utility.GetEnumDescription((ShipmentService)currentShipment.ShipmentService),
                    //ShipmentTermCode = currentShipment.ShipmentTermCode,
                    //ShipmentTypeCode = currentShipment.ShipmentTypeCode,
                    TrackingNumber = currentShipment.TrackingNumber,
                    CreatedDate = currentShipment.CreatedDate.ToString("MM/dd/yyyy")
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
                    ValueCurrency = Convert.ToInt32(currentShipment.ShipmentPackage.Currency),
                    PreferredCollectionDate = currentShipment.ShipmentPackage.CollectionDate.ToString(),
                    ProductIngredients = this.getPackageDetails(currentShipment.ShipmentPackage.PackageProducts),
                    ShipmentDescription = currentShipment.ShipmentPackage.PackageDescription

                },
                CarrierInformation = new CarrierInformationDto
                {
                    CarrierName = currentShipment.CarrierName,
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
                        ProductType = ingrediant.ProductTypeId.ToString(),
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
                        result.Status = Status.Error;
                        result.Message = "Error occured when adding payment";
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
                        ShipmentName = shipment.ShipmentName,
                        ShipmentServices = Utility.GetEnumDescription((ShipmentService)shipment.ShipmentService)
                    },
                    CarrierInformation = new CarrierInformationDto()
                    {
                        CarrierName = shipment.CarrierName,
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
                    result.Status = Status.Error;
                    result.Message = "Error occured when adding shipment";
                }
                else
                {
                    result.Status = Status.Success;
                    result.Message = "Shipment added successfully";
                    result.LabelURL = response.PDF;

                    shipment.Status = (short)ShipmentStatus.BookingConfirmation;
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
        public int DeleteShipment(string shipmentCode, string trackingNumber, string carrierName)
        {

            SISIntegrationManager sisManager = new SISIntegrationManager();
            string URL = "http://parcelinternational.pro/status/" + carrierName + "/" + trackingNumber;

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


        /// <summary>
        /// Insert shipment record
        /// </summary>
        /// <param name="fileDetails"></param>
        public void InsertShipmentDocument(FileUploadDto fileDetails)
        {
            using (var context = new PIContext())
            {
                CompanyManagement companyManagement = new CompanyManagement();
                var tenantId = companyManagement.GettenantIdByUserId(fileDetails.UserId);

                context.ShipmentDocument.Add(new ShipmentDocument
                {
                    TenantId = tenantId,
                    ShipmentId = fileDetails.ReferenceId,
                    ClientFileName = fileDetails.ClientFileName,
                    UploadedFileName = fileDetails.UploadedFileName
                });

                context.SaveChanges();
            }
        }


        public List<FileUploadDto> GetAvailableFilesForShipmentbyTenant(int shipmentId, string userId)
        {
            List<FileUploadDto> returnList = new List<FileUploadDto>();
            // Make absolute link
            string baseUrl = @"https://pidocuments.blob.core.windows.net:443/piblobstorage/";

            CompanyManagement companyManagement = new CompanyManagement();
            var tenantId = companyManagement.GettenantIdByUserId(userId);

            using (var context = new PIContext())
            {
                var docList = context.ShipmentDocument.Where(x => x.TenantId == tenantId
                                                    && x.ShipmentId == shipmentId).
                                                    OrderByDescending(x => x.CreatedDate).ToList();

                docList.ForEach(x => returnList.Add(new FileUploadDto
                {
                    TenantId = x.TenantId,
                    ReferenceId = x.ShipmentId,
                    ClientFileName = x.ClientFileName,
                    UploadedFileName = x.UploadedFileName
                }));

                returnList.ForEach(e =>
                    e.FileAbsoluteURL = baseUrl + "TENANT_"+ e.TenantId + "/" + "SHIPMENT_DOCUMENTS" + "/" + e.UploadedFileName
                );
            }
            return returnList;
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

    }


}
