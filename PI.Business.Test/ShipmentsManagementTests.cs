using Microsoft.VisualStudio.TestTools.UnitTesting;
using PI.Business;
using PI.Contract.DTOs.RateSheets;
using PI.Contract.DTOs.Shipment;
using PI.Contract.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Business.Tests
{
    [TestClass()]
    public class ShipmentsManagementTests
    {
        ShipmentsManagement shipment = null;

        [TestInitialize]
        public void Initialize()
        {
            shipment = new ShipmentsManagement();

        }

        //[TestMethod()]
        //public void SaveShipmentTest()
        //{
        //    ShipmentDto addShipment = new ShipmentDto()
        //    {
        //        GeneralInformation= new GeneralInformationDto
        //        {
        //            ShipmentName = "",
        //            DivisionId = 0,
        //            CostCenterId = 0,
        //            ShipmentMode= CarrierType.Express.ToString(),
        //            ShipmentServices= "DD-DDP-PP",
        //            ShipmentPaymentTypeId=1,

        //        },
        //        CarrierInformation= new CarrierInformationDto
        //        {
        //           CarrierName="UPS",
        //            serviceLevel="",
        //            tarriffType="",
        //            tariffText="",
        //            PickupDate=DateTime.Now,
        //        },               
                           
        //         CreatedBy="1",

        //        AddressInformation =new ConsignerAndConsigneeInformationDto{
        //            Consignee= new ConsigneeDto
        //            {
        //                CompanyName= "CompanyName",
        //                FirstName= "FirstName",
        //                LastName="",


        //            },
        //        },
        //         ConsigneeAddress = new ShipmentAddress
        //         {
        //                CompanyName = addShipment.AddressInformation.Consignee.CompanyName,
        //                FirstName = addShipment.AddressInformation.Consignee.FirstName,
        //                LastName = addShipment.AddressInformation.Consignee.LastName,
        //                Country = addShipment.AddressInformation.Consignee.Country,
        //                ZipCode = addShipment.AddressInformation.Consignee.Postalcode,
        //                Number = addShipment.AddressInformation.Consignee.Number,
        //                StreetAddress1 = addShipment.AddressInformation.Consignee.Address1,
        //                StreetAddress2 = addShipment.AddressInformation.Consignee.Address2,
        //                City = addShipment.AddressInformation.Consignee.City,
        //                State = addShipment.AddressInformation.Consignee.State,
        //                EmailAddress = addShipment.AddressInformation.Consignee.Email,
        //                PhoneNumber = addShipment.AddressInformation.Consignee.ContactNumber,
        //                ContactName = addShipment.AddressInformation.Consignee.FirstName + " " + addShipment.AddressInformation.Consignee.LastName,
        //                IsActive = true,
        //                CreatedBy = addShipment.CreatedBy,
        //                CreatedDate = DateTime.Now
        //            },
        //            ConsignorAddress = new ShipmentAddress
        //            {
        //                CompanyName = addShipment.AddressInformation.Consigner.CompanyName,
        //                FirstName = addShipment.AddressInformation.Consigner.FirstName,
        //                LastName = addShipment.AddressInformation.Consigner.LastName,
        //                Country = addShipment.AddressInformation.Consigner.Country,
        //                ZipCode = addShipment.AddressInformation.Consigner.Postalcode,
        //                Number = addShipment.AddressInformation.Consigner.Number,
        //                StreetAddress1 = addShipment.AddressInformation.Consigner.Address1,
        //                StreetAddress2 = addShipment.AddressInformation.Consigner.Address2,
        //                City = addShipment.AddressInformation.Consigner.City,
        //                State = addShipment.AddressInformation.Consigner.State,
        //                EmailAddress = addShipment.AddressInformation.Consigner.Email,
        //                PhoneNumber = addShipment.AddressInformation.Consigner.ContactNumber,
        //                ContactName = addShipment.AddressInformation.Consigner.FirstName + " " + addShipment.AddressInformation.Consigner.LastName,
        //                IsActive = true,
        //                CreatedBy = addShipment.CreatedBy,
        //                CreatedDate = DateTime.Now
        //            },
        //            ShipmentPackage = new ShipmentPackage()
        //            {
        //                PackageDescription = addShipment.PackageDetails.ShipmentDescription,
        //                TotalVolume = addShipment.PackageDetails.TotalVolume,
        //                TotalWeight = addShipment.PackageDetails.TotalWeight,
        //                HSCode = addShipment.PackageDetails.HsCode,
        //                CollectionDate = DateTime.Parse(addShipment.PackageDetails.PreferredCollectionDate),
        //                CarrierInstruction = addShipment.PackageDetails.Instructions,
        //                IsInsured = Convert.ToBoolean(addShipment.PackageDetails.IsInsuared),
        //                InsuranceDeclaredValue = addShipment.PackageDetails.DeclaredValue,
        //                InsuranceCurrencyType = (short)addShipment.PackageDetails.ValueCurrency,
        //                CarrierCost = addShipment.CarrierInformation.Price,
        //                InsuranceCost = addShipment.CarrierInformation.Insurance,
        //                PaymentTypeId = addShipment.PackageDetails.PaymentTypeId,
        //                EarliestPickupDate = addShipment.CarrierInformation.PickupDate ?? null,
        //                EstDeliveryDate = addShipment.CarrierInformation.DeliveryTime ?? null,
        //                WeightMetricId = addShipment.PackageDetails.CmLBS ? (short)1 : (short)2,
        //                VolumeMetricId = addShipment.PackageDetails.VolumeCMM ? (short)1 : (short)2,
        //                IsActive = true,
        //                CreatedBy = addShipment.CreatedBy,
        //                CreatedDate = DateTime.Now,
        //                PackageProducts = packageProductList,
        //                IsDG = addShipment.PackageDetails.IsDG,
        //                Accessibility = addShipment.PackageDetails.IsDG == true ? addShipment.PackageDetails.Accessibility : false,
        //                DGType = addShipment.PackageDetails.IsDG == true ? addShipment.PackageDetails.DGType : null,

        //            };
                

        //    save consigner details as new address book detail
        //    if (addShipment.AddressInformation.Consigner.SaveNewAddress)
        //    {
        //        AddressBook ConsignerAddressBook = new AddressBook
        //        {
        //            CompanyName = addShipment.AddressInformation.Consigner.CompanyName,
        //            FirstName = addShipment.AddressInformation.Consigner.FirstName,
        //            LastName = addShipment.AddressInformation.Consigner.LastName,
        //            Country = addShipment.AddressInformation.Consigner.Country,
        //            ZipCode = addShipment.AddressInformation.Consigner.Postalcode,
        //            Number = addShipment.AddressInformation.Consigner.Number,
        //            StreetAddress1 = addShipment.AddressInformation.Consigner.Address1,
        //            StreetAddress2 = addShipment.AddressInformation.Consigner.Address2,
        //            City = addShipment.AddressInformation.Consigner.City,
        //            State = addShipment.AddressInformation.Consigner.State,
        //            EmailAddress = addShipment.AddressInformation.Consigner.Email,
        //            PhoneNumber = addShipment.AddressInformation.Consigner.ContactNumber,
        //            IsActive = true,
        //            CreatedBy = addShipment.CreatedBy,
        //            UserId = addShipment.UserId,
        //            CreatedDate = DateTime.Now
        //        };

        //    };
        //    ShipmentOperationResult response= shipment.SaveShipment(addShipment);  
        //}

        [TestMethod()]
        public void GetRateSheetTest()
        {
            ShipmentDto currentShipment = new ShipmentDto()
            {

            };
            ShipmentcostList response = shipment.GetRateSheet(currentShipment);
            Assert.AreNotEqual(response, null);
            
        }

        [TestMethod()]
        public void GetInboundoutBoundStatusTest()
        {
            string userId = "";
            string fromCode ="";
            string toCode = "";

            string response=shipment.GetInboundoutBoundStatus(userId, fromCode, toCode);
            Assert.AreNotEqual(response, "N");

        }
       

        [TestMethod()]
        public void GetHashForPayLaneTest()
        {
            
        }

        [TestMethod()]
        public void GetAllShipmentsbyUserTest()
        {
            
        }

        [TestMethod()]
        public void GetshipmentsByDivisionIdTest()
        {
            
        }

        [TestMethod()]
        public void GetshipmentsByUserIdTest()
        {
            new NotImplementedException();
        }

        [TestMethod()]
        public void GetshipmentsByUserIdAndCreatedDateTest()
        {
            new NotImplementedException();
        }

        [TestMethod()]
        public void GetshipmentsByReferenceTest()
        {
            new NotImplementedException();
        }

        [TestMethod()]
        public void UpdateshipmentStatusManuallyTest()
        {
            new NotImplementedException();
        }

        [TestMethod()]
        public void UpdateShipmentStatusTest()
        {
            new NotImplementedException();
        }

        [TestMethod()]
        public void GetShipmentByShipmentCodeTest()
        {
            
        }

        [TestMethod()]
        public void GetshipmentByIdTest()
        {
            
        }

        [TestMethod()]
        public void getPackageDetailsTest()
        {
            
        }

        [TestMethod()]
        public void SendShipmentDetailsTest()
        {
            
        }

        [TestMethod()]
        public void GetAllDivisionsinCompanyTest()
        {
            
        }

        [TestMethod()]
        public void DeleteShipmentTest()
        {
            
        }

        [TestMethod()]
        public void GetLocationHistoryInfoForShipmentTest()
        {
            
        }

        [TestMethod()]
        public void GetTrackAndTraceInfoTest()
        {
            
        }

        [TestMethod()]
        public void GetShipmentByTrackingNoTest()
        {
            
        }

        [TestMethod()]
        public void UpdateStatusHistoriesTest()
        {
            
        }

        [TestMethod()]
        public void getUpdatedShipmentHistoryFromDBTest()
        {
            
        }

        [TestMethod()]
        public void DeleteLocationActivityByLocationHistoryIdTest()
        {
            
        }

        [TestMethod()]
        public void DeleteShipmentLocationHistoryByShipmentIdTest()
        {
            
        }

        [TestMethod()]
        public void GetLocationActivityByLocationHistoryIdTest()
        {
            
        }

        [TestMethod()]
        public void GetShipmentLocationHistoryByShipmentIdTest()
        {
            
        }

        [TestMethod()]
        public void GetShipmentByCodeShipmentTest()
        {
            
        }

        [TestMethod()]
        public void InsertShipmentDocumentTest()
        {
            
        }

        [TestMethod()]
        public void GetAvailableFilesForShipmentbyTenantTest()
        {
            
        }

        [TestMethod()]
        public void GetAllPendingShipmentsbyUserTest()
        {
            
        }

        [TestMethod()]
        public void GetAllshipmentsForManifestTest()
        {
            
        }

        [TestMethod()]
        public void GetshipmentByShipmentCodeForInvoiceTest()
        {
            
        }

        [TestMethod()]
        public void DeleteFileInDBTest()
        {
            
        }

        [TestMethod()]
        public void SaveCommercialInvoiceTest()
        {
            
        }

        [TestMethod()]
        public void RequestForQuoteTest()
        {
            
        }

        [TestMethod()]
        public void GetShipmentForCompanyAndSyncWithSISTest()
        {
            
        }

        [TestMethod()]
        public void GetAllShipmentByCompanyIdTest()
        {
            
        }

        [TestMethod()]
        public void loadAllShipmentsFromCompanyAndSearchTest()
        {
            
        }

        [TestMethod()]
        public void ShipmentReportTest()
        {
            
        }

        [TestMethod()]
        public void ShipmentReportForExcelTest()
        {
            
        }

        [TestMethod()]
        public void LoadAllCarriersTest()
        {
            
        }

        [TestMethod()]
        public void ToggleShipmentFavouritesTest()
        {
            
        }

        [TestMethod()]
        public void GetShipmentStatusCountsTest()
        {
            
        }

        [TestMethod()]
        public void SearchShipmentsByIdTest()
        {
            
        }
    }
}