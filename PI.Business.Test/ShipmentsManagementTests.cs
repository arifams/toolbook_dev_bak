using NUnit.Framework;
using PI.Business;
using PI.Contract.DTOs.Carrier;
using PI.Contract.DTOs.Common;
using PI.Contract.DTOs.Dashboard;
using PI.Contract.DTOs.Division;
using PI.Contract.DTOs.FileUpload;
using PI.Contract.DTOs.RateSheets;
using PI.Contract.DTOs.Report;
using PI.Contract.DTOs.Shipment;
using PI.Contract.Enums;
using PI.Data.Entity;
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
        ShipmentsManagement shipment = null;
        ShipmentDto shipmentDto = null;

        public ShipmentsManagementTests()
        {
            shipment = new ShipmentsManagement();
            shipmentDto = new ShipmentDto()
            {
                GeneralInformation = new GeneralInformationDto
                {
                    ShipmentName = "",
                    DivisionId = 0,
                    CostCenterId = 0,
                    ShipmentMode = CarrierType.Express.ToString(),
                    ShipmentServices = "DD-DDP-PP",
                    ShipmentPaymentTypeId = 1,
                    IsFavourite = true
                },
                CarrierInformation = new CarrierInformationDto
                {
                    CarrierName = "UPS",
                    serviceLevel = "",
                    tarriffType = "",
                    tariffText = "",
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
                    IsDG = true
                }
            };
         }
                

        [Test]
        public void SaveShipmentTest()
        {           
            ShipmentOperationResult response = shipment.SaveShipment(shipmentDto);
            Assert.AreEqual(response, null);
        }

        [Test]
        public void GetRateSheetTest()
        {            
            ShipmentcostList response = shipment.GetRateSheet(shipmentDto);
            Assert.AreNotEqual(response, null);            
        }

        [Test]
        public void GetInboundoutBoundStatusTest()
        {
            string userId = "";
            string fromCode ="";
            string toCode = "";

            string response=shipment.GetInboundoutBoundStatus(userId, fromCode, toCode);
            Assert.AreNotEqual(response, "N");
        }
       

        [Test]
        public void GetHashForPayLaneTest()
        {
            PayLaneDto paylaneDto = new PayLaneDto()
            {
                Description="",
                Currency="1",
                Amount=100,
                TransactionType="",
                Hash="",
                MerchantId ="",
                Status="",
                SaleId=1  
            };
            PayLaneDto response = shipment.GetHashForPayLane(paylaneDto);
            Assert.AreNotEqual(response.MerchantId, null);
        }

        [Test]
        public void GetAllShipmentsbyUserTest()
        {
            string status = "";
            string userId = "";
            DateTime? startDate = DateTime.Now;
            DateTime? endDate = DateTime.Now;
            string number = "12345";
            string source = "source";
            string destination = "destination";
            bool viaDashboard = true;

            PagedList response=shipment.GetAllShipmentsbyUser(status, userId, startDate, endDate,
                                               number, source, destination, viaDashboard);
            Assert.AreNotEqual(response.TotalRecords, 0);
        }

        [Test]
        public void GetshipmentsByDivisionIdTest()
        {
            long divid = 1;
            IList<Shipment> response = shipment.GetshipmentsByDivisionId(divid);
            Assert.AreNotEqual(response, null);
        }

        [Test]
        public void GetshipmentsByUserIdTest()
        {
            string userId ="1";
            IList<Shipment> response=shipment.GetshipmentsByUserId(userId);
            Assert.AreNotEqual(response.Count, 0);
        }

        [Test]
        public void GetshipmentsByUserIdAndCreatedDateTest()
        {
            string userId="";
            DateTime createdDate=DateTime.Now;
            string carreer = "";
            List<Shipment> reponse = shipment.GetshipmentsByUserIdAndCreatedDate(userId, createdDate, carreer);
            Assert.AreNotEqual(reponse, null);
        }

        [Test]
        public void GetshipmentsByReferenceTest()
        {
            string userId="";
            string reference = "";
            List<Shipment> response = shipment.GetshipmentsByReference(userId, reference);
            Assert.AreNotEqual(response, null);
        }

        [Test]
        public void UpdateshipmentStatusManuallyTest()
        {
            string codeShipment="";
            string status="";
            int response = shipment.UpdateshipmentStatusManually(codeShipment, status);
            Assert.AreEqual(response, 1);
        }

        [Test]
        public void UpdateShipmentStatusTest()
        {
            new NotImplementedException();
        }

        [Test]
        public void GetShipmentByShipmentCodeTest()
        {
            string codeShipment = "";
            Shipment response = shipment.GetShipmentByShipmentCode(codeShipment);
            Assert.AreNotEqual(response.Id, null);
        }

        [Test]
        public void GetshipmentByIdTest()
        {
            string shipmentCode="";
            long shipmentId = 0;
            ShipmentDto response= shipment.GetshipmentById(shipmentCode, shipmentId);
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
            List<ProductIngredientsDto> response = shipment.getPackageDetails(products);
            Assert.AreNotEqual(response.Count, 0);
        }

        [Test]
        public void SendShipmentDetailsTest()
        {
            SendShipmentDetailsDto sendShipmentDetails = new SendShipmentDetailsDto()
            {
                ShipmentId =2,
                PayLane = new PayLaneDto
                {
                    SaleId=2,
                    Status= "Status",                  
                },
                UserId="",
                TemplateLink=""
             };
            ShipmentOperationResult response = shipment.SendShipmentDetails(sendShipmentDetails);
           Assert.AreNotEqual(response.Status, Status.Error);
        }

        [Test]
        public void GetAllDivisionsinCompanyTest()
        {
            string userId = "";
            List<DivisionDto> response = shipment.GetAllDivisionsinCompany(userId);
            Assert.AreNotEqual(response.Count, 0);

        }

        [Test]
        public void DeleteShipmentTest()
        {
            string shipmentCode = "";
            string trackingNumber = "";
            string carrierName = "";
            bool isAdmin = true;
            long shipmentId = 1;

            int response = shipment.DeleteShipment(shipmentCode, trackingNumber, carrierName, isAdmin, shipmentId);
            Assert.AreEqual(response, 1);
        }
        

        [Test]
        public void GetLocationHistoryInfoForShipmentTest()
        {
            string carrier = "";
            string trackingNumber = "";
            string codeShipment = "";
            string environment = "taleus";
            StatusHistoryResponce response = shipment.GetLocationHistoryInfoForShipment(carrier, trackingNumber, codeShipment, environment);
            Assert.AreNotEqual(response.info, null);
        }

        [Test]
        public void GetTrackAndTraceInfoTest()
        {
            string carrier="";
            string trackingNumber="";
            StatusHistoryResponce response = shipment.GetTrackAndTraceInfo(carrier, trackingNumber);
            Assert.AreNotEqual(response.info, null);
        }

        [Test]
        public void GetShipmentByTrackingNoTest()
        {
            string trackingNo = "";
            Shipment response = shipment.GetShipmentByTrackingNo(trackingNo);
            Assert.AreNotEqual(response, null);
        }

        [Test]
        public void UpdateStatusHistoriesTest()
        {
            
        }

        [Test]
        public void getUpdatedShipmentHistoryFromDBTest()
        {
            string codeShipment = "";
            StatusHistoryResponce response = shipment.getUpdatedShipmentHistoryFromDB(codeShipment);
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
            List<LocationActivity> response = shipment.GetLocationActivityByLocationHistoryId(historyId);
            Assert.AreNotEqual(response.Count, 0);
        }

        [Test]
        public void GetShipmentLocationHistoryByShipmentIdTest()
        {
            long shipmentId = 1;
            List<ShipmentLocationHistory> response = shipment.GetShipmentLocationHistoryByShipmentId(shipmentId);
            Assert.AreNotEqual(response.Count, 0);
        }

        [Test]
        public void GetShipmentByCodeShipmentTest()
        {
            string codeShipment = "";
            Shipment response = shipment.GetShipmentByCodeShipment(codeShipment);
            Assert.AreNotEqual(response, null);
        }

        [Test]
        public void InsertShipmentDocumentTest()
        {
            
        }

        [Test]
        public void GetAvailableFilesForShipmentbyTenantTest()
        {
            string shipmentCode = "";
            string userId = "";
            List<FileUploadDto> response = shipment.GetAvailableFilesForShipmentbyTenant(shipmentCode, userId);
            Assert.AreNotEqual(response.Count, 0);
        }

        [Test]
        public void GetAllPendingShipmentsbyUserTest()
        {
            string userId="";
            DateTime startDate = DateTime.Now;
            DateTime? endDate = DateTime.Now.AddDays(1);
            string number = "";

            PagedList response= shipment.GetAllPendingShipmentsbyUser(userId, startDate, endDate, number);
            Assert.AreNotEqual(response.TotalRecords, 0);

        }

        [Test]
        public void GetAllshipmentsForManifestTest()
        {
            string userId = "";
            string date= "";
            string carreer = "";
            string reference = "";

            List<ShipmentDto> response= shipment.GetAllshipmentsForManifest(userId, date, carreer, reference);
            Assert.AreNotEqual(response.Count, 0);
        }

        [Test]
        public void GetshipmentByShipmentCodeForInvoiceTest()
        {
            string shipmentCode = "";
            CommercialInvoiceDto response = shipment.GetshipmentByShipmentCodeForInvoice(shipmentCode);
            Assert.AreNotEqual(response, null);
        }

        [Test]
        public void DeleteFileInDBTest()
        {
            
        }

        [Test]
        public void SaveCommercialInvoiceTest()
        {
            CommercialInvoiceDto addInvoice = new CommercialInvoiceDto()
            {

            };
            ShipmentOperationResult response = shipment.SaveCommercialInvoice(addInvoice);
        }

        [Test]
        public void RequestForQuoteTest()
        {
            string response = shipment.RequestForQuote(shipmentDto);
            Assert.AreNotEqual(response, string.Empty);
        }

        [Test]
        public void GetShipmentForCompanyAndSyncWithSISTest()
        {
            long companyId = 1;
            PagedList response = shipment.GetShipmentForCompanyAndSyncWithSIS(companyId);
            Assert.AreNotEqual(response.TotalRecords, 0);
        }

        [Test]
        public void GetAllShipmentByCompanyIdTest()
        {
            string companyId = "";
            PagedList response=shipment.GetAllShipmentByCompanyId(companyId);
            Assert.AreNotEqual(response.TotalRecords, 0);
        }

        [Test]
        public void loadAllShipmentsFromCompanyAndSearchTest()
        {
            string companyId = "";
            string status = "";
            DateTime startDate = DateTime.Now.AddDays(1);
            DateTime endDate = DateTime.Now.AddDays(10);
            string number = "";
            string source = "";
            string destination = "";

            PagedList response = shipment.loadAllShipmentsFromCompanyAndSearch(companyId, status, startDate, endDate,
                                           number, source, destination);

            Assert.AreNotEqual(response.TotalRecords, 0);
        }

        [Test]
        public void ShipmentReportTest()
        {
            string userId = "";
            short carrierId = 0;
            long companyId = 0;
            DateTime startDate = DateTime.Now.AddDays(1);
            DateTime endDate = DateTime.Now.AddDays(10);
            short status = 0;
            string countryOfOrigin = null;
            string countryOfDestination = null;
            short product = 0;
            short packageType = 0;
            List<ShipmentReportDto> response=shipment.ShipmentReport(userId, carrierId, companyId, startDate,
                                                     endDate, status, countryOfOrigin, countryOfDestination, product, packageType);
            Assert.AreNotEqual(response.Count, 0);
        }

        [Test]
        public void ShipmentReportForExcelTest()
        {
            string userId="";
            short carrierId = 0;
            long companyId = 0;
            DateTime startDate = DateTime.Now;
            DateTime endDate = DateTime.Now.AddDays(2);
            short status = 1;
            string countryOfOrigin = "US";
            string countryOfDestination = "GB";
            short product = 1;
            short packageType = 1;

            byte[] responce = shipment.ShipmentReportForExcel(userId, carrierId, companyId, startDate,
                                       endDate, status, countryOfOrigin, countryOfDestination, product, packageType);

            Assert.AreNotEqual(responce.Length, 0);
        }

        [Test]
        public void LoadAllCarriersTest()
        {
            List<CarrierDto> response = shipment.LoadAllCarriers();
            Assert.AreNotEqual(response.Count, 0);
                
        }

        [Test]
        public void ToggleShipmentFavouritesTest()
        {
            bool response = shipment.ToggleShipmentFavourites(shipmentDto);
            Assert.AreEqual(response, true );
        }

        [Test]
        public void GetShipmentStatusCountsTest()
        {
            string userId = "";
            DashboardShipments response = shipment.GetShipmentStatusCounts(userId);
            Assert.AreEqual(response.PendingStatusCount, 1);
        }

        [Test]
        public void SearchShipmentsByIdTest()
        {
            string trackingNumber = "";
            PagedList response = shipment.SearchShipmentsById(trackingNumber);
            Assert.AreNotEqual(response.TotalRecords, 0);
        }
    }
}