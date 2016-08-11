using NUnit.Framework;
using PI.Business;
using PI.Business.Test;
using PI.Contract.DTOs.Carrier;
using PI.Contract.DTOs.Common;
using PI.Contract.DTOs.Dashboard;
using PI.Contract.DTOs.Division;
using PI.Contract.DTOs.FileUpload;
using PI.Contract.DTOs.RateSheets;
using PI.Contract.DTOs.Report;
using PI.Contract.DTOs.Shipment;
using PI.Contract.Enums;
using PI.Data;
using PI.Data.Entity;
using PI.Data.Entity.Identity;
using PI.Data.Entity.RateEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Business.Tests
{
    [TestFixture]
    public class ShipmentsManagementTests
    {
        ShipmentsManagement shipmentManagement = null;
        ShipmentDto shipmentDto = null;

        public ShipmentsManagementTests()
        {
            #region Dto Section 

            shipmentDto = new ShipmentDto()
            {
                UserId="1",
                SISCompanyCode="123",
                GeneralInformation = new GeneralInformationDto
                {
                    ShipmentName = "ship1",
                    DivisionId = 1,
                    CostCenterId = 1,
                    ShipmentMode = "Express",
                    ShipmentServices = "DD-DDP-PP",
                    ShipmentPaymentTypeId = 1,
                    IsFavourite = true,
                    ShipmentCode= "ship123"
                },
                CarrierInformation = new CarrierInformationDto
                {
                    CarrierName = "UPS",
                    serviceLevel = "1",
                    tarriffType = "1",
                    tariffText = "test",
                    PickupDate = DateTime.Now,
                    Price = 100,
                    Insurance = 10,
                    DeliveryTime = DateTime.Now,

                },

                CreatedBy = "1",

                AddressInformation = new ConsignerAndConsigneeInformationDto
                {
                    Consigner = new ConsignerDto
                    {
                        CompanyName = "CompanyName",
                        FirstName = "FirstName",
                        LastName = "",
                        Country = "US",
                        Postalcode = "1234",
                        Number = "1234",
                        Address1 = "Address1",
                        Address2 = "Address2",
                        City = "City",
                        State = "State",
                        Email = "Email@email",
                        ContactNumber = "2342342344",
                        ContactName = "ContactName",

                    },

                    Consignee = new ConsigneeDto
                    {
                        CompanyName = "CompanyName",
                        FirstName = "FirstName",
                        LastName = "",
                        Country = "US",
                        Postalcode = "1234",
                        Number = "1234",
                        Address1 = "Address1",
                        Address2 = "Address2",
                        City = "City",
                        State = "State",
                        Email = "Email@email",
                        ContactNumber = "2342342344",
                        ContactName = "ContactName",

                    },


                },

                PackageDetails = new PackageDetailsDto
                {
                    ShipmentDescription = "",
                    TotalVolume = 10,
                    TotalWeight = 10,
                    HsCode = "123Code",
                    PreferredCollectionDate = "2017-12-12",
                    Instructions = "instruction",
                    IsInsuared = "True",
                    DeclaredValue = 10,
                    ValueCurrency = 1,
                    PaymentTypeId = 1,
                    CmLBS = true,
                    VolumeCMM = false,
                    IsDG = true,
                    ProductIngredients= new List<ProductIngredientsDto>
                    {
                        new ProductIngredientsDto
                        {
                             Height=10,
                             Length=10,
                             Weight=10,
                             Width=10,
                             Quantity=10,
                             ProductType=ProductType.Box.ToString()
                        },
                        new ProductIngredientsDto
                        {
                             Height=20,
                             Length=20,
                             Weight=20,
                             Width=20,
                             Quantity=20,
                             ProductType=ProductType.Document.ToString()
                        }
                    }
                }
            };
            #endregion

            #region mocking Data
            List<ApplicationUser> users = new List<ApplicationUser>()
            {
                new ApplicationUser()
                {
                        Id="1",
                        UserName="user1",
                        Email="user1@parcel.com",
                        FirstName="fname",
                        LastName="lname",
                        IsActive=true,
                        IsDeleted=false,
                        Salutation="Mr",
                        TenantId=1,
                        PhoneNumber="1231231233",


                }
            };

            List<DivisionCostCenter> divisionCostCenters = new List<DivisionCostCenter>()
            {
                new DivisionCostCenter
                {
                    CostCenterId=1,
                    DivisionId=1,
                    Id=1,
                    IsAssigned=true,
                    IsActive=true,
                    IsDelete=false,
                    CostCenters=new CostCenter(),
                    Divisions= new Division()

                }
            };

            List<CostCenter> costCenters = new List<CostCenter>()
            {
               new CostCenter
                {
                    Id = 1,
                    BillingAddressId = 1,
                    CompanyId = 1,
                    Description = "costcenter",
                    Name = "cc_finance",
                    IsActive = true,
                    IsDelete = false,
                    PhoneNumber = "12312312312",
                    Type="SYSTEM"
                   // DivisionCostCenters= divisionCostCenters
,
                },
            };

            List<Company> companies = new List<Company>()
            {
                new Company
                {
                    Id=1,
                     Name="comp1",
                     IsActive=true,
                     IsDelete=false,
                     CompanyCode="comp1",
                     LogoUrl="http://parcelinternational.com",
                     TenantId=1,
                     COCNumber="1234",
                     VATNumber="vat123",
                     IsInvoiceEnabled=true,
                }
            };



            List<Tenant> tenants = new List<Tenant>()
            {
              new Tenant
              {
                    Id = 1,
                    TenancyName = "tenant1",
                    IsCorporateAccount = true,
                    IsActive = true,
                    IsDelete = false
              }
            };


            List<UserInDivision> userindivisions = new List<UserInDivision>()
            {
                new UserInDivision
                {
                    UserId="1",
                    DivisionId=1,
                    IsDelete=false,
                    IsActive=true,
                    User= new ApplicationUser
                    {
                        Id="1",
                        UserName="user1",
                        Email="user1@parcel.com",
                        FirstName="fname",
                        LastName="lname",
                        IsActive=true,
                        IsDeleted=false,
                        Salutation="Mr",
                        TenantId=1,
                        PhoneNumber="1231231233",
                    },
                    Divisions= new Division
                    {

                        Id=1,
                        CompanyId=1,
                        IsActive=true,
                        IsDelete=false,
                        Name="div1",
                        Description="finance_div",
                        DefaultCostCenterId=1,
                        Type="USER"
                    }


                }
            };

            List<Division> divisions = new List<Division>
            {

             new Division {

                 Id=1,
                 CompanyId=1,
                 IsActive=true,
                 IsDelete=false,
                 Name="div1",
                 Description="Desc",
                 DefaultCostCenterId=1,
                 UserInDivisions=userindivisions,
                 Type="SYSTEM",


                 CostCenter= new CostCenter
                 {
                     Id=1,
                     BillingAddressId=1,
                     CompanyId=1,
                     Description="costcenter",
                     Name="cc_finance",
                     IsActive=true,
                     IsDelete=false,
                     PhoneNumber="12312312312",
                     Type="USER"
,                 },

                 Company= new Company
                 {
                     Id=1,
                     Name="comp1",
                     IsActive=true,
                     IsDelete=false,
                     CompanyCode="comp1",
                     LogoUrl="http://parcelinternational.com",
                     TenantId=1,
                     COCNumber="1234",
                     VATNumber="vat123",
                     IsInvoiceEnabled=true,


                     Tenant= new Tenant
                     {
                         Id=1,
                         TenancyName="tenant1",
                         IsCorporateAccount=true,
                         IsActive=true,
                         IsDelete=false
                     }
                 }
             },

            };

            List<Customer> customers = new List<Customer>()
            {
                new Customer
                {
                    UserId="1",
                    AddressId=1,
                    Email="user1@parcel.com",
                    FirstName="fname",
                    JobCapacity="jobcapacity",
                    LastName="lname",
                    Id=1

                }
            };

            List<AuditTrail> auditTrails = new List<AuditTrail>()
            {
                new AuditTrail
                {
                    Id=1,
                    CreatedBy="1",
                    IsActive=true,
                    IsDelete=false
                }
            };

            List<Carrier> careers = new List<Carrier>()
            {
                new Carrier
                {
                    Id=1,
                    Name="DHL",
                    IsActive=true,
                    CarrierNameLong="DHL Express",
                    IsDelete= false
                }
            };

            List<Shipment> shipments = new List<Shipment>()
            {
                new Shipment
                {
                  Id=2,
                  ShipmentName ="ship1",
                  ShipmentReferenceName="ref1234",
                  ShipmentCode="",
                  DivisionId =1,
                  CostCenterId =1,
                  ShipmentMode=CarrierType.Express,  
                  ShipmentService= 1,                       
                  CreatedBy="1",
                  CreatedDate=DateTime.Now,
                  TarriffType="DHL_EXP",
                  TariffText="DHL_EXP",
                  ServiceLevel="",
                  PickUpDate=DateTime.Now,
                  Status=4,
                  TrackingNumber="43453535335356",        
                  ConsignorId =1,
                  ConsigneeId =1,        
                  ShipmentPackageId=1,                 
                  ParentShipmentId=1,
                  IsParent=false,
                  ShipmentPaymentTypeId =1,
                  ManualStatusUpdatedDate =DateTime.Now,
                  IsFavourite =true,
                  CarrierId=1,
                  Division = new Division
                  {
                      Id=1,
                      CompanyId=1,
                      IsActive=true,
                      IsDelete=false,
                      Name="div1",
                      Description="Desc",
                      DefaultCostCenterId=1,
                      UserInDivisions=userindivisions,
                      Type="SYSTEM",


                 CostCenter= new CostCenter
                 {
                     Id=1,
                     BillingAddressId=1,
                     CompanyId=1,
                     Description="costcenter",
                     Name="cc_finance",
                     IsActive=true,
                     IsDelete=false,
                     PhoneNumber="12312312312",
                     Type="SYSTEM"
,                 },

                 Company= new Company
                 {
                     Id=1,
                     Name="comp1",
                     IsActive=true,
                     IsDelete=false,
                     CompanyCode="comp1",
                     LogoUrl="http://parcelinternational.com",
                     TenantId=1,
                     COCNumber="1234",
                     VATNumber="vat123",
                     IsInvoiceEnabled=true,


                     Tenant= new Tenant
                     {
                         Id=1,
                         TenancyName="tenant1",
                         IsCorporateAccount=true,
                         IsActive=true,
                         IsDelete=false
                     }
                  }
                  },     
                  CostCenter = new  CostCenter
                  {
                      Id = 1,
                      BillingAddressId = 1,
                      CompanyId = 1,
                      Description = "costcenter",
                      Name = "cc_finance",
                      IsActive = true,
                      IsDelete = false,
                      PhoneNumber = "12312312312",
                      Type="SYSTEM"

                  },
                  ConsignorAddress = new ShipmentAddress
                  {
                      Id=1,
                      FirstName="fname",
                      LastName="lname",
                      City="city",
                      CompanyName="comp1",
                      EmailAddress="test@shipment.com",
                      Country="US",
                      ContactName="cont_name",
                      IsActive=true,
                      IsDelete=false,
                      Number="123123123",
                      State="State",
                      PhoneNumber="12312311",
                      StreetAddress1="stadd1",
                      StreetAddress2="stadd2",
                      ZipCode="123ABC"
                      

                  },
                  ConsigneeAddress= new ShipmentAddress
                  {
                      Id=2,
                      FirstName="fname",
                      LastName="lname",
                      City="city",
                      CompanyName="comp1",
                      EmailAddress="test2@shipment.com",
                      Country="US",
                      ContactName="cont_name",
                      IsActive=true,
                      IsDelete=false,
                      Number="123123123",
                      State="State",
                      PhoneNumber="12312311",
                      StreetAddress1="stadd1",
                      StreetAddress2="stadd2",
                      ZipCode="123ABC"

                  },
                  ShipmentPackage = new ShipmentPackage
                  {
                      Id=1,
                      InsuranceCurrencyType=1,
                      PackageProducts=new List<PackageProduct>
                      {
                          new PackageProduct
                          {
                              Id=1,
                              Height=10,
                              Length=10,
                              Width=10,
                              Quantity=2,
                              Description="desc",
                              IsActive=true,
                              IsDelete=false,
                              Weight=10,
                              ProductTypeId=1,
                              CreatedDate=DateTime.Now,
                              ShipmentPackageId=1
                          }
                      },

                      IsInsured=true,
                      Currency=new Currency
                      {
                          Id=1,
                          CurrencyCode="USD",
                          CurrencyName="US Dollars",
                          IsActive=true,
                          IsDelete=false,
                          CreatedDate=DateTime.Now,
                          CreatedBy="1"
                      }
                  },
                  ShipmentPayment = new ShipmentPayment
                  {
                      ShipmentId=1,
                      SaleId=1,
                      Status="4",
                      IsActive=true                     

                  },
                  //CommercialInvoice= new CommercialInvoice
                  //{

                  //},           
                
                  Carrier= new Carrier
                  {
                     IsActive=true,
                     Id=1,
                     Name="DHL",
                     CarrierNameLong="DHL Express",
                     IsDelete=false                     
                  }                

               }, 
                 new Shipment
                 {

                  Id=1,
                  ShipmentName ="ship1",
                  ShipmentReferenceName="ref123",
                  ShipmentCode="ship123",
                  DivisionId =1,
                  CostCenterId =1,
                  ShipmentMode=CarrierType.Express,
                  ShipmentService= 1,
                  CreatedBy="1",
                  CreatedDate=DateTime.Now,
                  TarriffType="DHL_EXP",
                  TariffText="DHL_EXP",
                  ServiceLevel="",
                  PickUpDate=DateTime.Now,
                  Status=4,
                  TrackingNumber="4345353533535",
                  ConsignorId =1,
                  ConsigneeId =1,
                  ShipmentPackageId=1,
                  ParentShipmentId=1,
                  IsParent=false,
                  ShipmentPaymentTypeId =1,
                  ManualStatusUpdatedDate =DateTime.Now,
                  IsFavourite =true,
                  CarrierId=1,
                  Division = new Division
                  {
                      Id=1,
                      CompanyId=1,
                      IsActive=true,
                      IsDelete=false,
                      Name="div1",
                      Description="Desc",
                      DefaultCostCenterId=1,
                      UserInDivisions=userindivisions,
                      Type="SYSTEM",


                 CostCenter= new CostCenter
                 {
                     Id=1,
                     BillingAddressId=1,
                     CompanyId=1,
                     Description="costcenter",
                     Name="cc_finance",
                     IsActive=true,
                     IsDelete=false,
                     PhoneNumber="12312312312",
                     Type="SYSTEM"
,                 },

                 Company= new Company
                 {
                     Id=1,
                     Name="comp1",
                     IsActive=true,
                     IsDelete=false,
                     CompanyCode="comp1",
                     LogoUrl="http://parcelinternational.com",
                     TenantId=1,
                     COCNumber="1234",
                     VATNumber="vat123",
                     IsInvoiceEnabled=true,


                     Tenant= new Tenant
                     {
                         Id=1,
                         TenancyName="tenant1",
                         IsCorporateAccount=true,
                         IsActive=true,
                         IsDelete=false
                     }
                  }
                  },
                  CostCenter = new  CostCenter
                  {
                      Id = 1,
                      BillingAddressId = 1,
                      CompanyId = 1,
                      Description = "costcenter",
                      Name = "cc_finance",
                      IsActive = true,
                      IsDelete = false,
                      PhoneNumber = "12312312312",
                      Type="SYSTEM"

                  },
                  ConsignorAddress = new ShipmentAddress
                  {
                      Id=1,
                      FirstName="fname",
                      LastName="lname",
                      City="city",
                      CompanyName="comp1",
                      EmailAddress="test@shipment.com",
                      Country="US",
                      ContactName="cont_name",
                      IsActive=true,
                      IsDelete=false,
                      Number="123123123",
                      State="State",
                      PhoneNumber="12312311",
                      StreetAddress1="stadd1",
                      StreetAddress2="stadd2",
                      ZipCode="123ABC"


                  },
                  ConsigneeAddress= new ShipmentAddress
                  {
                      Id=2,
                      FirstName="fname",
                      LastName="lname",
                      City="city",
                      CompanyName="comp1",
                      EmailAddress="test2@shipment.com",
                      Country="US",
                      ContactName="cont_name",
                      IsActive=true,
                      IsDelete=false,
                      Number="123123123",
                      State="State",
                      PhoneNumber="12312311",
                      StreetAddress1="stadd1",
                      StreetAddress2="stadd2",
                      ZipCode="123ABC"

                  },
                  ShipmentPackage = new ShipmentPackage
                  {
                      Id=1,
                      PackageProducts=new List<PackageProduct>
                      {
                          new PackageProduct
                          {
                              Id=1,
                              Height=10,
                              Length=10,
                              Width=10,
                              Quantity=2,
                              Description="desc",
                              IsActive=true,
                              IsDelete=false,
                              Weight=10,
                              ProductTypeId=1,
                              CreatedDate=DateTime.Now,
                              ShipmentPackageId=1
                          }
                      },

                      IsInsured=true,
                      Currency=new Currency
                      {
                          Id=1,
                          CurrencyCode="USD",
                          CurrencyName="US Dollars",
                          IsActive=true,
                          IsDelete=false,
                          CreatedDate=DateTime.Now,
                          CreatedBy="1"
                      }
                  },
                  ShipmentPayment = new ShipmentPayment
                  {
                      ShipmentId=1,
                      SaleId=1,
                      Status="4",
                      IsActive=true

                  },
                  //CommercialInvoice= new CommercialInvoice
                  //{

                  //},           
                
                  Carrier= new Carrier
                  {
                     IsActive=true,
                     Id=1,
                     Name="DHL",
                     CarrierNameLong="DHL Express",
                     IsDelete=false
                  }               
                 },


                 new Shipment { 
                  Id=3,
                  ShipmentName ="ship1",
                  ShipmentReferenceName="ref12345",
                  ShipmentCode="ship12345",
                  DivisionId =1,
                  CostCenterId =1,
                  ShipmentMode=CarrierType.Express,
                  ShipmentService= 1,
                  CreatedBy="1",
                  CreatedDate=DateTime.Now,
                  TarriffType="DHL_EXP",
                  TariffText="DHL_EXP",
                  ServiceLevel="",
                  PickUpDate=DateTime.Now,
                  Status=4,
                  TrackingNumber="434535353353567",
                  ConsignorId =1,
                  ConsigneeId =1,
                  ShipmentPackageId=1,
                  ParentShipmentId=1,
                  IsParent=false,
                  ShipmentPaymentTypeId =1,
                  ManualStatusUpdatedDate =DateTime.Now,
                  IsFavourite =true,
                  CarrierId=1,
                  Division = new Division
                  {
                      Id=1,
                      CompanyId=1,
                      IsActive=true,
                      IsDelete=false,
                      Name="div1",
                      Description="Desc",
                      DefaultCostCenterId=1,
                      UserInDivisions=userindivisions,
                      Type="SYSTEM",


                 CostCenter= new CostCenter
                 {
                     Id=1,
                     BillingAddressId=1,
                     CompanyId=1,
                     Description="costcenter",
                     Name="cc_finance",
                     IsActive=true,
                     IsDelete=false,
                     PhoneNumber="12312312312",
                     Type="SYSTEM"
,                 },

                 Company= new Company
                 {
                     Id=1,
                     Name="comp1",
                     IsActive=true,
                     IsDelete=false,
                     CompanyCode="comp1",
                     LogoUrl="http://parcelinternational.com",
                     TenantId=1,
                     COCNumber="1234",
                     VATNumber="vat123",
                     IsInvoiceEnabled=true,


                     Tenant= new Tenant
                     {
                         Id=1,
                         TenancyName="tenant1",
                         IsCorporateAccount=true,
                         IsActive=true,
                         IsDelete=false
                     }
                  }
                  },
                  CostCenter = new  CostCenter
                  {
                      Id = 1,
                      BillingAddressId = 1,
                      CompanyId = 1,
                      Description = "costcenter",
                      Name = "cc_finance",
                      IsActive = true,
                      IsDelete = false,
                      PhoneNumber = "12312312312",
                      Type="SYSTEM"

                  },
                  ConsignorAddress = new ShipmentAddress
                  {
                      Id=1,
                      FirstName="fname",
                      LastName="lname",
                      City="city",
                      CompanyName="comp1",
                      EmailAddress="test@shipment.com",
                      Country="US",
                      ContactName="cont_name",
                      IsActive=true,
                      IsDelete=false,
                      Number="123123123",
                      State="State",
                      PhoneNumber="12312311",
                      StreetAddress1="stadd1",
                      StreetAddress2="stadd2",
                      ZipCode="123ABC"


                  },
                  ConsigneeAddress= new ShipmentAddress
                  {
                      Id=2,
                      FirstName="fname",
                      LastName="lname",
                      City="city",
                      CompanyName="comp1",
                      EmailAddress="test2@shipment.com",
                      Country="US",
                      ContactName="cont_name",
                      IsActive=true,
                      IsDelete=false,
                      Number="123123123",
                      State="State",
                      PhoneNumber="12312311",
                      StreetAddress1="stadd1",
                      StreetAddress2="stadd2",
                      ZipCode="123ABC"

                  },
                  ShipmentPackage = new ShipmentPackage
                  {
                      Id=1,
                      PackageProducts=new List<PackageProduct>
                      {
                          new PackageProduct
                          {
                              Id=1,
                              Height=10,
                              Length=10,
                              Width=10,
                              Quantity=2,
                              Description="desc",
                              IsActive=true,
                              IsDelete=false,
                              Weight=10,
                              ProductTypeId=1,
                              CreatedDate=DateTime.Now,
                              ShipmentPackageId=1
                          }
                      },

                      IsInsured=true,
                      Currency=new Currency
                      {
                          Id=1,
                          CurrencyCode="USD",
                          CurrencyName="US Dollars",
                          IsActive=true,
                          IsDelete=false,
                          CreatedDate=DateTime.Now,
                          CreatedBy="1"
                      }

                  },
                  ShipmentPayment = new ShipmentPayment
                  {
                      ShipmentId=1,
                      SaleId=1,
                      Status="4",
                      IsActive=true

                  },
                  //CommercialInvoice= new CommercialInvoice
                  //{

                  //},           
                
                  Carrier= new Carrier
                  {
                     IsActive=true,
                     Id=1,
                     Name="DHL",
                     CarrierNameLong="DHL Express",
                     IsDelete=false
                  }
                 }
            };

            List<AddressBook> addressBooks = new List<AddressBook>()
            {
                new AddressBook
                {
                    Id=1,
                    AccountNumber="123",
                    FirstName="fname",
                    LastName="lname",
                    Salutation="Mr", 
                    UserId="1",
                    StreetAddress1="staddres1",
                    StreetAddress2="staddres2",
                    ZipCode="1234",
                    Country="US",
                    State="state",
                    City="city",
                    CompanyName="comp1",
                    IsActive=true,
                    IsDelete=false,
                    Number="123",
                    EmailAddress="test@email.com",
                    PhoneNumber="1212312312",
                    CreatedBy="1",
                    CreatedDate= DateTime.Now
                    
                    
                }
            };

            List<Address> addresses = new List<Address>()
            {
                new Address
                {
                    Id=1,                   
                    StreetAddress1="staddres1",
                    StreetAddress2="staddres2",
                    ZipCode="1234",
                    Country="US",
                    State="state",
                    City="city",                  
                    IsActive=true,
                    IsDelete=false,
                    Number="123",                    
                    CreatedBy="1",
                    CreatedDate= DateTime.Now,                    
                    
                }
            };

            List<TarrifTextCode> tarrifTextCodes = new List<TarrifTextCode>()
            {
                new TarrifTextCode
                {
                    Id=1,
                    CountryCode="US",
                    TarrifText="DHL_EXP",
                    IsActive=true,
                    IsDelete=false
                }
            };

            List<Rate> rates = new List<Rate>()
            {
                 new Rate
                 {
                     Id=1,
                     IsInbound=false,
                     CarrierId=1,
                     MaxDimension=1000,
                     MaxLength=100,
                     MaxWeightPerPiece=1000,
                     VolumeFactor=1,
                     WeightMin=1,
                     WeightMax=1000,
                     Service=ProductType.Box,
                     Currency=CurrencyType.USD,
                     SellOrBuy=RatesSell.Sell,
                     TariffType=new TariffType
                     {
                         Id=1,
                         TarrifName="Express",
                         IsActive=true,
                     },
                     IsActive=true,
                     CountryFrom="US",
                     RateZoneList= new List<RateZone>
                     {
                         new RateZone
                         {
                             Id=1,
                             ZoneId=1,
                             Price=1000,
                             RateId=1,
                             Zone= new Zone
                             {
                                 Id=1, 
                                 CountryFrom="US",
                                 CountryTo="US",
                                 LocationFrom="1234-1234",
                                 LocationTo="1234-1234",
                                 TransmitTimeList= new List<TransmitTime>()
                                 {
                                     new TransmitTime
                                     {
                                         Id=1,
                                         CarrierId=1,
                                         
                                         TransitTimeProductList= new List<TransitTimeProduct>()
                                         {
                                             
                                             new TransitTimeProduct
                                             {
                                                 Id=1,                                                
                                                 ProductType= ProductType.Box,
                                                 Days=10
                                             }
                                         }
                                     }
                                 }
                             },
                             
                         }
                     },
                     Carrier= new CarrierService
                     {
                         Id=1,
                         CarrierAccountNumber="123",
                         CarrierId=1,
                         ServiceLevel="Express",
                         CarrierType=CarrierType.Express,
                         CarrierCountryCode="US",
                         IsActive=true,
                         IsDelete=false,
                         CreatedBy="1",
                         CreatedDate=DateTime.Now,

                         Carrier=  new Carrier
                         {
                             Id=1,
                             CarrierNameLong="DHL Express",
                             Name="DHL",
                             IsActive=true,
                             IsDelete=false,
                             CreatedBy="1",
                             CreatedDate=DateTime.Now
                         }
                        

                     }
                     
                 }
            };


            List<ShipmentDocument> documents = new List<ShipmentDocument>()
            {
                new ShipmentDocument
                {
                    Id=1,
                    DocumentType=4,
                    IsActive=true,
                    ShipmentId=1,
                    TenantId=1,
                    UploadedFileName="shipment_AWB",
                    Shipment= new Shipment
                    {
                        ShipmentCode="ship123"
                    }
                }
            };

            List<Currency> currencies = new List<Currency>()
            {
                 new Currency
                 {
                     Id=1,
                     CurrencyName="USD",
                     CurrencyCode="USD",
                     IsActive=true
                 }
            };

            List<ShipmentLocationHistory> locationHistories = new List<ShipmentLocationHistory>()
            {
                 new ShipmentLocationHistory
                 {
                     Id=1,
                     ShipmentId=1,
                     Latitude=21.2,
                     Longitude=22.2,
                     City="city",
                     Country="US", 
                     CreatedDate=DateTime.Now,
                     CreatedBy="1",
                     IsActive=true,
                     IsDelete=false,
                     LocaionActivities= new List<LocationActivity>{

                         new LocationActivity
                         {
                             ShipmentLocationHistoryId=1,
                             Id=1,
                             Status="pending",
                             Time=DateTime.Now,
                             Date=DateTime.Now.Date,
                             IsActive=true,
                             IsDelete=false,
                             CreatedBy="1",
                             CreatedDate=DateTime.Now                             
                         }
                     }

                     
                 }
            };


            List<LocationActivity> locationActivities = new List<LocationActivity>()
            {
                 new LocationActivity
                 {
                     ShipmentLocationHistoryId=1,
                     Id=1,
                     Status="pending",
                     Time=DateTime.Now,
                     Date=DateTime.Now.Date,
                     IsActive=true,
                     IsDelete=false,
                     CreatedBy="1",
                     CreatedDate=DateTime.Now

                 }
            };

            List<CommercialInvoice> invoices = new List<CommercialInvoice>()
            {
                new CommercialInvoice
                {
                    CountryOfDestination="US",
                    CountryOfOrigin="GB",
                    CreatedBy="1",
                    CreatedDate=DateTime.Now,
                    CustomerNo="cus1234",
                    HSCode="HScode123",
                    ImportBroker="broker",
                    InvoiceItemId=1,
                    InvoiceNo="1",
                    InvoiceTo="to",
                    IsActive=true,
                    IsDelete=false,
                    ModeOfTransport="SHIP",
                    Note="note",
                    ShipmentId=1,
                    TermsOfPayment="top",
                    ValueCurrency=1,
                    ShipTo="US",
                    VatNo="vatno",
                    ShipmentReferenceName="ship123",
                    ShipmentService=1,
                   
                    InvoiceItem= new InvoiceItem
                    {
                        Id=1,
                        CreatedBy="1",
                        CreatedDate=DateTime.Now,
                        IsActive=true,
                        IsDelete=false,
                        InvoiceItemLines= new List<InvoiceItemLine>
                        {
                            new InvoiceItemLine
                            {
                                  Description="desc",
                                  Quantity =2,
                                  PricePerPiece=10,
                                  InvoiceItemId=1,
                                  HSCode="HScode1234"
                            }
                            }
                           
                    }
                }
            };



            var mockSetDivisions = MoqHelper.CreateMockForDbSet<Division>()
                                                .SetupForQueryOn(divisions)
                                                .WithAdd(divisions);
            var mockSetUserindivisions = MoqHelper.CreateMockForDbSet<UserInDivision>()
                                               .SetupForQueryOn(userindivisions)
                                               .WithAdd(userindivisions);
            var mockSetCostCenters = MoqHelper.CreateMockForDbSet<CostCenter>()
                                             .SetupForQueryOn(costCenters)
                                             .WithAdd(costCenters);
            var mockSetCompanies = MoqHelper.CreateMockForDbSet<Company>()
                                             .SetupForQueryOn(companies)
                                             .WithAdd(companies);
            var mockSetTenants = MoqHelper.CreateMockForDbSet<Tenant>()
                                             .SetupForQueryOn(tenants)
                                             .WithAdd(tenants);
            var mockSetDivisionCostCenters = MoqHelper.CreateMockForDbSet<DivisionCostCenter>()
                                             .SetupForQueryOn(divisionCostCenters)
                                             .WithAdd(divisionCostCenters);
            var mockSetusers = MoqHelper.CreateMockForDbSet<ApplicationUser>()
                                            .SetupForQueryOn(users)
                                            .WithAdd(users);
            var mockSetcustomers = MoqHelper.CreateMockForDbSet<Customer>()
                                           .SetupForQueryOn(customers)
                                           .WithAdd(customers);

            var mockSetAuditTrails = MoqHelper.CreateMockForDbSet<AuditTrail>()
                                                .SetupForQueryOn(auditTrails)
                                                .WithAdd(auditTrails);
            var mockSetShipments = MoqHelper.CreateMockForDbSet<Shipment>()
                                          .SetupForQueryOn(shipments)
                                          .WithAdd(shipments);

            var mockSetCareer = MoqHelper.CreateMockForDbSet<Carrier>()
                                          .SetupForQueryOn(careers)
                                          .WithAdd(careers);

            var mockSetAddresses = MoqHelper.CreateMockForDbSet<Address>()
                                        .SetupForQueryOn(addresses)
                                        .WithAdd(addresses);

            var mockSetAddressBooks = MoqHelper.CreateMockForDbSet<AddressBook>()
                                        .SetupForQueryOn(addressBooks)
                                        .WithAdd(addressBooks);

            var mockSetTarriftexts = MoqHelper.CreateMockForDbSet<TarrifTextCode>()
                                                .SetupForQueryOn(tarrifTextCodes)
                                                .WithAdd(tarrifTextCodes);

            var mockSetRates = MoqHelper.CreateMockForDbSet<Rate>()
                                                .SetupForQueryOn(rates)
                                                .WithAdd(rates);

            var mockSetCurrencies = MoqHelper.CreateMockForDbSet<Currency>()
                                                .SetupForQueryOn(currencies)
                                                .WithAdd(currencies);

            var mockSetDocuments = MoqHelper.CreateMockForDbSet<ShipmentDocument>()
                                               .SetupForQueryOn(documents)
                                               .WithAdd(documents);

            var mockSetLocationHistories = MoqHelper.CreateMockForDbSet<ShipmentLocationHistory>()
                                             .SetupForQueryOn(locationHistories)
                                             .WithAdd(locationHistories);

            var mockSetLocationActivities = MoqHelper.CreateMockForDbSet<LocationActivity>()
                                            .SetupForQueryOn(locationActivities)
                                            .WithAdd(locationActivities);

            var mockSetInvoices = MoqHelper.CreateMockForDbSet<CommercialInvoice>()
                                            .SetupForQueryOn(invoices)
                                            .WithAdd(invoices);


            var mockContext = MoqHelper.CreateMockForDbContext<PIContext, Division>(mockSetDivisions);


            mockContext.Setup(c => c.Divisions).Returns(mockSetDivisions.Object);
            mockContext.Setup(c => c.UsersInDivisions).Returns(mockSetUserindivisions.Object);
            mockContext.Setup(c => c.CostCenters).Returns(mockSetCostCenters.Object);
            mockContext.Setup(c => c.Companies).Returns(mockSetCompanies.Object);
            mockContext.Setup(c => c.Tenants).Returns(mockSetTenants.Object);
            mockContext.Setup(c => c.DivisionCostCenters).Returns(mockSetDivisionCostCenters.Object);
            mockContext.Setup(c => c.Users).Returns(mockSetusers.Object);
            mockContext.Setup(c => c.AuditTrail).Returns(mockSetAuditTrails.Object);
            mockContext.Setup(c => c.Customers).Returns(mockSetcustomers.Object);
            mockContext.Setup(c=>c.Shipments).Returns(mockSetShipments.Object);
            mockContext.Setup(c => c.Carrier).Returns(mockSetCareer.Object);
            mockContext.Setup(c => c.Addresses).Returns(mockSetAddresses.Object);
            mockContext.Setup(c => c.AddressBooks).Returns(mockSetAddressBooks.Object);
            mockContext.Setup(c => c.TarrifTextCodes).Returns(mockSetTarriftexts.Object);
            mockContext.Setup(c => c.Rate).Returns(mockSetRates.Object);
            mockContext.Setup(c => c.Currencies).Returns(mockSetCurrencies.Object);
            mockContext.Setup(c => c.ShipmentDocument).Returns(mockSetDocuments.Object);
            mockContext.Setup(c => c.ShipmentLocationHistories).Returns(mockSetLocationHistories.Object);
            mockContext.Setup(c => c.LocationActivities).Returns(mockSetLocationActivities.Object);
            mockContext.Setup(c => c.CommercialInvoices).Returns(mockSetInvoices.Object);



            shipmentManagement = new ShipmentsManagement(mockContext.Object);
            #endregion
        }


        [Test]
        public void SaveShipmentTest()
        {           
            ShipmentOperationResult response = shipmentManagement.SaveShipment(shipmentDto);
            Assert.AreEqual(response.Status, Status.Success);
        }

        //todo
        [Test]
        public void GetRateSheetTest()
        {            
            ShipmentcostList response = shipmentManagement.GetRateSheet(shipmentDto);
            Assert.AreNotEqual(response, null);            
        }

        [TestCase("GB","US")]
        [TestCase("US", "GB")]
        public void GetInboundoutBoundStatusTest(string _fromCode, string _toCode)
        {
            string userId = "1";
            string fromCode = _fromCode;
            string toCode = _toCode;

            string response=shipmentManagement.GetInboundoutBoundStatus(userId, fromCode, toCode);
            if (_fromCode=="GB")
            {
                Assert.AreEqual(response, "Y");
            }
            else if (_fromCode == "US")
            {
                Assert.AreEqual(response, "N");
            }
           
        }
       

        //this method is not currently in use
        //for app settings are not available
        [Test]
        public void GetHashForPayLaneTest()
        {
            PayLaneDto paylaneDto = new PayLaneDto()
            {
                Description="",
                Currency="1",
                Amount=100,
                TransactionType="Card",
                Hash="",
                MerchantId ="",
                Status="",
                SaleId=1  
            };
            PayLaneDto response = shipmentManagement.GetHashForPayLane(paylaneDto);
            Assert.AreNotEqual(response.MerchantId, null);
        }

        //testing blocked by mocking Role issue
        [Test]
        public void GetAllShipmentsbyUserTest()
        {
            string status = "";
            string userId = "1";
            DateTime? startDate = DateTime.Now;
            DateTime? endDate = DateTime.Now;
            string number = "";
            string source = "";
            string destination = "";
            bool viaDashboard = true;

            PagedList response=shipmentManagement.GetAllShipmentsbyUser(status, userId, startDate, endDate,
                                               number, source, destination, viaDashboard);
            Assert.AreNotEqual(response.TotalRecords, 0);
        }

        [TestCase(1)]
        [TestCase(10)]
        public void GetshipmentsByDivisionIdTest(long _divId)
        {
            long divid = _divId;
            IList<Shipment> response = shipmentManagement.GetshipmentsByDivisionId(divid);
            if (_divId==1)
            {
                Assert.AreEqual(response.Count(), 3);
            }
            else
            {
                Assert.AreEqual(response.Count(), 0);
            }
           
        }

        [TestCase("1")]
        [TestCase("10")]
        public void GetshipmentsByUserIdTest(string _userId)
        {
            string userId = _userId;
            IList<Shipment> response=shipmentManagement.GetshipmentsByUserId(userId);
            if (_userId=="1")
            {
                Assert.AreEqual(response.Count, 3);
            }
            else
            {
                Assert.AreEqual(response.Count,0);
            }
           
        }

        [TestCase("1")]
        [TestCase("10")]
        public void GetshipmentsByUserIdAndCreatedDateTest(string _userId)
        {
            string userId= _userId;
            DateTime createdDate=DateTime.Now;
            string carreer = "DHL";
            List<Shipment> reponse = shipmentManagement.GetshipmentsByUserIdAndCreatedDate(userId, createdDate, carreer);

            if (_userId=="1")
            {
                Assert.AreEqual(reponse.Count, 3);
            }
            else
            {
                Assert.AreEqual(reponse.Count, 0);
            }
            
        }

        [TestCase("1")]
        [TestCase("10")]        
        public void GetshipmentsByReferenceTest(string _userId)
        {
            string userId= _userId;
            string reference = "ref123";
            List<Shipment> response = shipmentManagement.GetshipmentsByReference(userId, reference);
           
            if (_userId=="2")
            {
                Assert.AreEqual(response.Count, 1);
            }
            else
            {
                Assert.AreEqual(response.Count, 0);
            }

        }

        [TestCase("ship123")]
        [TestCase("ship123456")]
        public void UpdateshipmentStatusManuallyTest(string _codeShipment)
        {
            string codeShipment= _codeShipment;
            string status= ShipmentStatus.Pickup.ToString();
            int response = shipmentManagement.UpdateshipmentStatusManually(codeShipment, status);

            if (_codeShipment== "ship123")
            {
                Assert.AreEqual(response, 1);
            }
            else
            {
                Assert.AreEqual(response, 0);
            }
           
        }

        [Test]
        public void UpdateShipmentStatusTest()
        {
            new NotImplementedException();
        }

        [TestCase("ship123")]
        [TestCase("ship12345")]
        public void GetShipmentByShipmentCodeTest(string _codeShipment)
        {
            string codeShipment = _codeShipment;
            Shipment response = shipmentManagement.GetShipmentByShipmentCode(codeShipment);
           
            if (_codeShipment == "ship123")
            {
                Assert.AreEqual(response.Id, 1);
            }
            else
            {
                Assert.AreEqual(response, null);
            }
        }

        [Test]
        public void GetshipmentByIdTest()
        {
            string shipmentCode= "ship123";
            long shipmentId = 1;
            ShipmentDto response= shipmentManagement.GetshipmentById(shipmentCode, shipmentId);
            Assert.AreNotEqual(response, null);
        }

        [Test]
        public void getPackageDetailsTest()
        {
            IList<PackageProduct> products = new List<PackageProduct>();
            products.Add(new PackageProduct()
            {
                Height=10,
                Length=10,
                ProductTypeId=1,
                Quantity=1,
                Weight=10,
                Width=10,
                Description="Description"
            });
            List<ProductIngredientsDto> response = shipmentManagement.getPackageDetails(products);
            Assert.AreNotEqual(response.Count, 0);
        }

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        public void SendShipmentDetailsTest(long _shipmentId)
        {
            SendShipmentDetailsDto sendShipmentDetails = new SendShipmentDetailsDto()
            {
                ShipmentId = _shipmentId,
                PayLane = new PayLaneDto
                {
                    SaleId=2,
                    Status= "Status",                  
                },
                UserId="1",
                TemplateLink=""
             };
            ShipmentOperationResult response = shipmentManagement.SendShipmentDetails(sendShipmentDetails);

            if (_shipmentId == 1)
            {
                Assert.AreEqual(response.Status, Status.Error);
            }
            else if (_shipmentId == 2)
            {
                Assert.AreEqual(response.Status, Status.Success);
            }
            else if (_shipmentId == 1)
            {
                Assert.AreEqual(response.Status, Status.SISError);
            }           

        }

        [TestCase("1")]       
        public void GetAllDivisionsinCompanyTest(string _userId)
        {
            string userId = _userId;
            List<DivisionDto> response = shipmentManagement.GetAllDivisionsinCompany(userId);
            Assert.AreEqual(response.Count, 1);               
        }
               

        [Test]
        public void GetLocationHistoryInfoForShipmentTest()
        {
            string carrier = "";
            string trackingNumber = "";
            string codeShipment = "";
            string environment = "taleus";
            StatusHistoryResponce response = shipmentManagement.GetLocationHistoryInfoForShipment(carrier, trackingNumber, codeShipment, environment);
            Assert.AreNotEqual(response.info, null);
        }

        [Test]
        public void GetTrackAndTraceInfoTest()
        {
            string carrier="DHL";
            string trackingNumber= "4345353533535";
            StatusHistoryResponce response = shipmentManagement.GetTrackAndTraceInfo(carrier, trackingNumber);
            Assert.AreEqual(response.info.carrier, "DHL");
        }

        [Test]
        public void GetShipmentByTrackingNoTest()
        {
            string trackingNo = "4345353533535";
            Shipment response = shipmentManagement.GetShipmentByTrackingNo(trackingNo);
            Assert.AreNotEqual(response, null);
        }

        [Test]
        public void UpdateStatusHistoriesTest()
        {
            
        }

        [Test]
        public void getUpdatedShipmentHistoryFromDBTest()
        {
            string codeShipment = "ship123";
            StatusHistoryResponce response = shipmentManagement.getUpdatedShipmentHistoryFromDB(codeShipment);
            Assert.AreNotEqual(response, null);
        }

        [Test]
        public void DeleteLocationActivityByLocationHistoryIdTest()
        {
            
        }

        [Test]
        public void DeleteShipmentLocationHistoryByShipmentIdTest()
        {
            
        }

        [Test]
        public void GetLocationActivityByLocationHistoryIdTest()
        {
            long historyId = 1;
            List<LocationActivity> response = shipmentManagement.GetLocationActivityByLocationHistoryId(historyId);
            Assert.AreNotEqual(response.Count, 0);
        }

        [Test]
        public void GetShipmentLocationHistoryByShipmentIdTest()
        {
            long shipmentId = 1;
            List<ShipmentLocationHistory> response = shipmentManagement.GetShipmentLocationHistoryByShipmentId(shipmentId);
            Assert.AreNotEqual(response.Count, 0);
        }

        [Test]
        public void GetShipmentByCodeShipmentTest()
        {
            string codeShipment = "ship123";
            Shipment response = shipmentManagement.GetShipmentByCodeShipment(codeShipment);
            Assert.AreNotEqual(response, null);
        }

        [Test]
        public void InsertShipmentDocumentTest()
        {
            
        }

        [Test]
        public void GetAvailableFilesForShipmentbyTenantTest()
        {
            string shipmentCode = "ship123";
            string userId = "1";
            List<FileUploadDto> response = shipmentManagement.GetAvailableFilesForShipmentbyTenant(shipmentCode, userId);
            Assert.AreNotEqual(response.Count, 0);
        }

        //blocked test by mocking Role
        [Test]
        public void GetAllPendingShipmentsbyUserTest()
        {
            string userId="1";
            DateTime startDate = DateTime.Now;
            DateTime? endDate = DateTime.Now.AddDays(1);
            string number = "";

            PagedList response= shipmentManagement.GetAllPendingShipmentsbyUser(userId, startDate, endDate, number);
            Assert.AreNotEqual(response.TotalRecords, 0);

        }

        [TestCase("1","DHL","")]
        [TestCase("1", "DHL", "ref123")]
        public void GetAllshipmentsForManifestTest(string _userId, string _career, string _reference)
        {
            string userId = _userId;
            string date= DateTime.Now.ToString();
            string carreer = _career;
            string reference = _reference;

            List<ShipmentDto> response= shipmentManagement.GetAllshipmentsForManifest(userId, date, carreer, reference);
            Assert.AreEqual(response.Count, 3);
        }

        [Test]
        public void GetshipmentByShipmentCodeForInvoiceTest()
        {
            string shipmentCode = "ship123";
            CommercialInvoiceDto response = shipmentManagement.GetshipmentByShipmentCodeForInvoice(shipmentCode);
            Assert.AreNotEqual(response, null);
        }

        [Test]
        public void DeleteFileInDBTest()
        {
            FileUploadDto fileDetails = new FileUploadDto
            {
                Id = 1
            };

            shipmentManagement.DeleteFileInDB(fileDetails);
        }

        [Test]
        public void SaveCommercialInvoiceTest()
        {
            CommercialInvoiceDto addInvoice = new CommercialInvoiceDto()
            {
                AddressInformation= new ConsignerAndConsigneeInformationDto
                {
                    Consignee= new ConsigneeDto
                    {
                        FirstName= "FirstName",
                        LastName= "LastName",
                        Number= "Number",
                        Address1= "Address1",
                        Address2= "Address2",
                        Postalcode="123",
                        City= "City",
                        CompanyName= "CompanyName",
                        ContactName= "ContactName",
                        ContactNumber= "ContactNumber",
                        Country="US",
                        Details= "Details",
                        Email= "Email",
                        State= "State"
                    },
                    Consigner= new ConsignerDto
                    {
                        FirstName = "FirstName",
                        LastName = "LastName",
                        Number = "Number",
                        Address1 = "Address1",
                        Address2 = "Address2",
                        Postalcode = "123",
                        City = "City",
                        CompanyName = "CompanyName",
                        ContactName = "ContactName",
                        ContactNumber = "ContactNumber",
                        Country = "US",
                        Details = "Details",
                        Email = "Email",
                        State = "State"
                    }
                    
                },
                CountryOfDestination="",
                CountryOfOrigin="",
                CustomerNo="1234",
                ImportBroker="broker",
                InvoiceNo="1",
                InvoiceTo="to",
                Item= new InvoiceItemDto
                {
                    LineItems= new List<InvoiceItemLineDto>
                    {
                        new InvoiceItemLineDto
                        {
                            Description="desc",
                            HSCode="hscode",
                            PricePerPiece=10,
                            Quantity=2
                        }
                    }
                },
                ModeOfTransport="SHIP",
                PackageDetails= new PackageDetailsDto
                {
                    Accessibility=true,
                    CarrierCost="100",
                    CmLBS=true,
                    VolumeCMM=true,
                    Count=2,
                    DeclaredValue=100,
                    DGType="N",
                    EarliestPickupDate="2016-10-10",
                    EstDeliveryDate= "2016-10-10",
                    HsCode="123hs",
                    Instructions="instruction",
                    IsDG=true,
                    IsInsuared="false",
                    PaymentTypeId=1,
                    PreferredCollectionDate= "2016-10-10",
                    ProductIngredients= new List<ProductIngredientsDto>
                    {
                        new ProductIngredientsDto
                        {
                            Description="desc",
                            Height=1,
                            Length=1,
                            Weight=1,
                            Quantity=1,
                            Width=1,
                            ProductType="Box"
                        }
                    }                  

                },
                Note="note",
                ShipmentId=1,
                ShipmentReferenceName="ship123",
                ShipTo="to",
                TermsOfPayment="top",
                ValueCurrency=1,
                VatNo="vatno",
                ShipmentServices= "DD-DDP-PP",
                CreatedDate="2016-10-10"

            };
            ShipmentOperationResult response = shipmentManagement.SaveCommercialInvoice(addInvoice);
            Assert.AreNotEqual(response, null);
        }

        [Test]
        public void RequestForQuoteTest()
        {
            string response = shipmentManagement.RequestForQuote(shipmentDto);
            Assert.AreNotEqual(response, string.Empty);
        }

        [Test]
        public void GetShipmentForCompanyAndSyncWithSISTest()
        {
            long companyId = 1;
            PagedList response = shipmentManagement.GetShipmentForCompanyAndSyncWithSIS(companyId);
            Assert.AreNotEqual(response.TotalRecords, 0);
        }

        [Test]
        public void GetAllShipmentByCompanyIdTest()
        {
            string companyId = "1";
            PagedList response=shipmentManagement.GetAllShipmentByCompanyId(companyId);
            Assert.AreNotEqual(response.TotalRecords, 0);
        }

        [Test]
        public void loadAllShipmentsFromCompanyAndSearchTest()
        {
            string companyId = "1";
            string status = "";
            DateTime? startDate=null;
            DateTime? endDate=null;
            string number = "";
            string source = "";
            string destination = "";

            PagedList response = shipmentManagement.loadAllShipmentsFromCompanyAndSearch(companyId, status, startDate, endDate,
                                           number, source, destination);

            Assert.AreEqual(response.TotalRecords,3);
        }


        //blocked because of mocking role
        [Test]
        public void ShipmentReportTest()
        {
            string userId = "1";
            short carrierId = 1;
            long companyId = 1;
            DateTime startDate = DateTime.Now.AddDays(1);
            DateTime endDate = DateTime.Now.AddDays(10);
            short status = 1;
            string countryOfOrigin = "US";
            string countryOfDestination = null;
            short product = 0;
            short packageType = 0;
            List<ShipmentReportDto> response=shipmentManagement.ShipmentReport(userId, carrierId, companyId, startDate,
                                                     endDate, status, countryOfOrigin, countryOfDestination, product, packageType);
            Assert.AreNotEqual(response.Count, 0);
        }


        //blocked by mocking role
        [Test]
        public void ShipmentReportForExcelTest()
        {
            string userId="1";
            short carrierId = 0;
            long companyId = 0;
            DateTime startDate = DateTime.Now;
            DateTime endDate = DateTime.Now.AddDays(2);
            short status = 1;
            string countryOfOrigin = "US";
            string countryOfDestination = "GB";
            short product = 1;
            short packageType = 1;

            byte[] responce = shipmentManagement.ShipmentReportForExcel(userId, carrierId, companyId, startDate,
                                       endDate, status, countryOfOrigin, countryOfDestination, product, packageType);

            Assert.AreNotEqual(responce.Length, 0);
        }

        [Test]
        public void LoadAllCarriersTest()
        {
            List<CarrierDto> response = shipmentManagement.LoadAllCarriers();
            Assert.AreNotEqual(response.Count, 0);
                
        }

        [Test]
        public void ToggleShipmentFavouritesTest()
        {
            bool response = shipmentManagement.ToggleShipmentFavourites(shipmentDto);
            Assert.AreEqual(response, false );
        }


        //blocked bymocking user role
        [Test]
        public void GetShipmentStatusCountsTest()
        {
            string userId = "1";
            DashboardShipments response = shipmentManagement.GetShipmentStatusCounts(userId);
            Assert.AreEqual(response.PendingStatusCount, 1);
        }

        //SIS integration
        [Test]
        public void SearchShipmentsByIdTest()
        {
            string trackingNumber = "4345353533535";
            PagedList response = shipmentManagement.SearchShipmentsById(trackingNumber);
            Assert.AreNotEqual(response.TotalRecords, 0);
        }


        [Order(45)]
        [TestCase(true, "", "ship123")]
        [TestCase(false, "", "ship1234")]        
        public void DeleteShipmentTest(bool _isadmin, string trackingNo, string _shipmentCode)
        {
            string shipmentCode = _shipmentCode;
            string trackingNumber = trackingNo;
            string carrierName = "DHL";
            bool isAdmin = _isadmin;
            long shipmentId = 1;

            int response = shipmentManagement.DeleteShipment(shipmentCode, trackingNumber, carrierName, isAdmin, shipmentId);
            Assert.AreEqual(response, 1);
        }

    }
}