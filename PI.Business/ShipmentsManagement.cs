using AutoMapper;
using PI.Contract.Business;
using PI.Contract.DTOs.Common;
using PI.Contract.DTOs.Division;
using PI.Contract.DTOs.RateSheets;
using PI.Contract.DTOs.Shipment;
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
                if (currentShipment.GeneralInformation.ShipmentMode=="Express")
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
                currentRateSheetDetails.address1 = currentShipment.AddressInformation.Consigner.Name.Replace(' ', '%');
                currentRateSheetDetails.address2 = currentShipment.AddressInformation.Consigner.Address1.Replace(' ','%');
                currentRateSheetDetails.address3 = currentShipment.AddressInformation.Consigner.Address2!=null? currentShipment.AddressInformation.Consigner.Address2.Replace(' ', '%'):string.Empty;
                currentRateSheetDetails.address4 = currentShipment.AddressInformation.Consigner.City.Replace(' ', '%');
                currentRateSheetDetails.street_number = currentShipment.AddressInformation.Consigner.Number;
                currentRateSheetDetails.postcode = currentShipment.AddressInformation.Consigner.Postalcode;
                currentRateSheetDetails.country_from = currentShipment.AddressInformation.Consigner.Country;
                currentRateSheetDetails.code_country_from = currentShipment.AddressInformation.Consigner.Country;
           
                //consignee details
                currentRateSheetDetails.address11 = currentShipment.AddressInformation.Consignee.Name.Replace(' ', '%');
                currentRateSheetDetails.address12 = currentShipment.AddressInformation.Consignee.Address1.Replace(' ', '%');
                currentRateSheetDetails.address13 = currentShipment.AddressInformation.Consignee.Address2!=null? currentShipment.AddressInformation.Consignee.Address2.Replace(' ', '%'): string.Empty;
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
                double maxLength = 0;
                double maxWidth = 0;
                double maxHeight = 0;
                double surface = 0;
                double maxdimension = 0;
                double pieces = 0;
                double volume = 0;
                double maxVolume = 0;
                double weight = 0;
                double maxWeight = 0;
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
                if (currentShipment.PackageDetails.IsInsuared=="true")
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

        //get the status of inbound outbound rule
        public string GetInboundoutBoundStatus(string userId, string fromCode,string toCode)
        {
            string status = "N";

            using (PIContext context=new PIContext())
            {
                var countryCode = string.Empty;
                var addressId = context.Customers.Where(c => c.UserId == userId).Select(c => c.AddressId).SingleOrDefault();
                if (addressId!=0)
                {
                    countryCode= context.Addresses.Where(c => c.Id == addressId).Select(c => c.Country).SingleOrDefault();
                }
                if (countryCode!=null&& countryCode.Equals(toCode) && !countryCode.Equals(fromCode))
                {
                    status = "Y";
                }
                else
                {
                    status="N";
                }

            }

            return status;
        }

        public ShipmentOperationResult SubmitShipment(ShipmentDto addShipment)
        {
            ICarrierIntegrationManager sisManager = new SISIntegrationManager();

            AddShipmentResponse addShipmentResponse = sisManager.SubmitShipment(addShipment);

            //If response is successfull save the shipment in DB.
            using (PIContext context = new PIContext())
            {
                //Mapper.CreateMap<GeneralInformationDto, Shipment>();
                Shipment newShipment = new Shipment
                {
                    ShipmentName = addShipment.GeneralInformation.ShipmentName,
                    ShipmentCode = addShipmentResponse.CodeShipment,
                    DivisionId = addShipment.GeneralInformation.DivisionId == 0 ? null : (long?)addShipment.GeneralInformation.DivisionId,
                    CostCenterId = addShipment.GeneralInformation.CostCenterId == 0 ? null : (long?)addShipment.GeneralInformation.CostCenterId,
                    ShipmentMode = addShipment.GeneralInformation.shipmentModeName,
                    ShipmentTypeCode = addShipment.GeneralInformation.ShipmentTypeCode,
                    ShipmentTermCode = addShipment.GeneralInformation.ShipmentTermCode,
                    CarrierName = addShipment.CarrierInformation.CarrierName,
                    TrackingNumber = addShipmentResponse.Awb,
                    //Status = addShipmentResponse.Status,
                    CreatedBy = "1",
                    CreatedDate = DateTime.Now,

                    ConsigneeAddress = new ShipmentAddress
                    {
                        Country = addShipment.AddressInformation.Consignee.Country,
                        ZipCode = addShipment.AddressInformation.Consignee.Postalcode,
                        Number = addShipment.AddressInformation.Consignee.Number,
                        StreetAddress1 = addShipment.AddressInformation.Consignee.Address1,
                        StreetAddress2 = addShipment.AddressInformation.Consignee.Address2,
                        City = addShipment.AddressInformation.Consignee.City,
                        State = addShipment.AddressInformation.Consignee.State,
                        EmailAddress = addShipment.AddressInformation.Consignee.Email,
                        PhoneNumber = addShipment.AddressInformation.Consignee.ContactNumber,
                        ContactName=addShipment.AddressInformation.Consignee.ContactName,
                        IsActive = true,
                        CreatedBy = addShipment.UserId,
                        CreatedDate = DateTime.Now
                    },
                    ConsignorAddress = new ShipmentAddress
                    {
                        FirstName = addShipment.AddressInformation.Consigner.Name,
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
                        CreatedDate = DateTime.Now
                    }
                };
               
                try
                {
                    context.Shipments.Add(newShipment);
                    context.SaveChanges();

                    context.ShipmentStatusHistory.Add(new ShipmentStatusHistory { ShipmentId = newShipment.Id, NewStatus = "NEW", CreatedBy = addShipment.UserId, CreatedDate= DateTime.Now });
                    context.SaveChanges();
                }
                catch (Exception ex) { throw ex; }
            }

            ShipmentOperationResult shipmentResult = new ShipmentOperationResult();

            if (addShipmentResponse == null || string.IsNullOrWhiteSpace(addShipmentResponse.Awb))
                shipmentResult.Status = "Error";
            else
            {
                shipmentResult.Status = "Success";
                shipmentResult.AddShipmentXML = addShipmentResponse.AddShipmentXML;
            }

            return shipmentResult;
        }

        public PayLaneDto GetHashForPayLane(PayLaneDto payLaneDto)
        {
            string merchantId = ConfigurationManager.AppSettings["PayLaneMerchantId"].ToString();
            string hashSalt = ConfigurationManager.AppSettings["PayLaneHashSalt"].ToString();
            string description = ConfigurationManager.AppSettings["PayLaneDescription"].ToString();

            //(salt + "|" + description + "|" + amount + "|" + currency + "|" + transaction_type)
            string buildStringForHash = string.Format("{0}|{1}|{2}|{3}|{4}",hashSalt, description, payLaneDto.Amount,payLaneDto.Currency,payLaneDto.TransactionType);
            return new PayLaneDto() {
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
            CompanyManagement company =new CompanyManagement();
            IList<DivisionDto> divisions = null;
            IList<int> divisionList = new List<int>();
            List<Shipment> Shipments=new List<Shipment>();
            var pagedRecord = new PagedList();
            if (userId==null)
            {
                return null;
            }
            string role = this.GetUserRoleById(userId);
            if (role == "BusinessOwner")
            {
                divisions = company.GetAllDivisionsForCompany(userId);
            }              
            else
            {
                divisions = company.GetAssignedDivisions(userId);
            }
            foreach (var item in divisions)
            {
                Shipments.AddRange(this.GetshipmentsByDivisionId(item.Id));
            }

            pagedRecord.Content = new List<ShipmentDto>();

             var content = (from shipment in Shipments
                           where shipment.IsDelete == false &&
                           (string.IsNullOrEmpty(status) || shipment.ShipmentStatuses.Any(x=> x.NewStatus == status)) &&
                           (startDate == null || (shipment.ShipmentPackage.EarliestPickupDate >= startDate && shipment.ShipmentPackage.EarliestPickupDate <= endDate)) &&                           
                           (string.IsNullOrEmpty(number)|| shipment.TrackingNumber.Contains(number) ||shipment.ShipmentCode.Contains(number) ) &&
                           (string.IsNullOrEmpty(source)||shipment.ConsignorAddress.Country.Contains(source) || shipment.ConsignorAddress.City.Contains(source) )&&
                           (string.IsNullOrEmpty(destination)||shipment.ConsigneeAddress.Country.Contains(destination) || shipment.ConsigneeAddress.City.Contains(destination))
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
                            Name = item.ConsigneeAddress.FirstName + " " + item.ConsigneeAddress.LastName,
                            ContactName = item.ConsigneeAddress.ContactName,
                            ContactNumber = item.ConsigneeAddress.ContactName,
                            Email = item.ConsigneeAddress.EmailAddress,
                            Number = item.ConsigneeAddress.Number
                        },
                        Consigner = new ConsignerDto {
                            Address1 = item.ConsignorAddress.StreetAddress1,
                            Address2 = item.ConsignorAddress.StreetAddress2,
                            Postalcode = item.ConsignorAddress.ZipCode,
                            City = item.ConsignorAddress.City,
                            Country = item.ConsignorAddress.Country,
                            State = item.ConsignorAddress.State,
                            Name = item.ConsignorAddress.FirstName + " " + item.ConsignorAddress.LastName,
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
                        ShipmentTermCode = item.ShipmentTermCode,
                        ShipmentTypeCode = item.ShipmentTypeCode,
                        TrackingNumber = item.TrackingNumber,
                        CreatedDate = item.CreatedDate.ToString("MM/dd/yyyy"),
                        Status = (item.ShipmentStatuses.Count() == 0)? null : 
                                 item.ShipmentStatuses.OrderByDescending(x => x.CreatedDate).FirstOrDefault().NewStatus
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
            using (PIContext context=new PIContext())
            {
                string roleId = context.Users.Where(u => u.Id == userId).FirstOrDefault().Roles.FirstOrDefault().RoleId;
                string roleName = context.Roles.Where(r => r.Id == roleId).Select(r => r.Name).FirstOrDefault();
                return roleName;                
            }

        }

        public IList<Shipment> GetshipmentsByDivisionId(long divid)
        {
            IList<Shipment> currentShipments = null;
            using (PIContext context=new PIContext())
            {
                currentShipments = (from shipment in context.Shipments
                                    join shipmentAddress1 in context.ShipmentAddresses on shipment.ConsigneeAddress.Id equals shipmentAddress1.Id
                                    join shipmentAddress2 in context.ShipmentAddresses on shipment.ConsigneeAddress.Id equals shipmentAddress2.Id
                                    join shipmentPackages in context.ShipmentPackages on shipment.ShipmentPackageId equals shipmentPackages.Id
                                    where shipment.DivisionId == divid  select shipment).ToList();
               
            }
            
            return currentShipments;
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
                        Height = (double)ingrediant.Height,
                        Length = (double)ingrediant.Length,
                        ProductType = ingrediant.ProductTypeId.ToString(),
                        Quantity = ingrediant.Quantity,
                        Weight = (double)ingrediant.Weight,
                        Width = (double)ingrediant.Width,
                        Description = ingrediant.Description
                    });
                
            }
          return ingrediantList;
        }
    }
}
