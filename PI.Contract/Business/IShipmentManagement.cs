using PI.Contract.DTOs.Carrier;
using PI.Contract.DTOs.Common;
using PI.Contract.DTOs.FileUpload;
using PI.Contract.DTOs.RateSheets;
using PI.Contract.DTOs.Report;
using PI.Contract.DTOs.Shipment;
using PI.Contract.DTOs.Dashboard;
using PI.Contract.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PI.Contract.DTOs;
using PI.Contract.DTOs.Payment;

namespace PI.Contract.Business
{
    public interface IShipmentManagement
    {
        ShipmentcostList GetRateSheet(ShipmentDto currentShipment);
        string GetInboundoutBoundStatus(string userId, string fromCode, string toCode);
        ShipmentOperationResult SaveShipment(ShipmentDto addShipment);
        string GetSquareApplicationId();
        ShipmentOperationResult SendShipmentDetails(SendShipmentDetailsDto sendShipmentDetails);
        void InsertShipmentDocument(FileUploadDto fileDetails);
        CommercialInvoiceDto GetshipmentByShipmentCodeForInvoice(string shipmentCode);
        ShipmentOperationResult SaveCommercialInvoice(CommercialInvoiceDto addShipment);
        string RequestForQuote(ShipmentDto addShipment);
        List<FileUploadDto> GetAvailableFilesForShipmentbyTenant(string shipmentCode, string userId);
        void DeleteFileInDB(FileUploadDto fileDetails);
        PagedList GetAllShipmentsbyUser(PagedList shipmentSerach);
        List<ShipmentDto> GetAllshipmentsForManifest(string userId, string date, string carreer, string reference);

        PagedList GetAllPendingShipmentsbyUser(string userId, DateTime? startDate, DateTime? endDate,
                                               string number);
        ShipmentDto GetshipmentById(string shipmentCode, long shipmentId = 0);
        int DeleteShipment(string shipmentCode, string trackingNumber, string carrierName, bool isAdmin, long shipmentId);
        StatusHistoryResponce GetTrackAndTraceInfo(string carrier, string trackingNumber);
        PagedList GetAllShipmentByCompanyId(string companyId);
        PagedList loadAllShipmentsForAdmin(string status = null, DateTime? startDate = null, DateTime? endDate = null, string searchValue = null, int currentPage = 0, int pageSize = 10);

        bool UpdateshipmentStatusManually(string codeShipment, string status);

        StatusHistoryResponce GetLocationHistoryInfoForShipment(string carrier, string trackingNumber, string codeShipment,
                                                                                                       string environment);

        byte[] ShipmentReportForExcel(string userId, short carrierId = 0, long companyId = 0, DateTime? startDate = null,
                               DateTime? endDate = null, short status = 0, string countryOfOrigin = null, string countryOfDestination = null, short product = 0, short packageType = 0);


        List<ShipmentReportDto> ShipmentReport(string userId, short carrierId = 0, long companyId = 0, DateTime? startDate = null,
                                  DateTime? endDate = null, short status = 0, string countryOfOrigin = null, string countryOfDestination = null, short product = 0, short packageType = 0);

        List<CarrierDto> LoadAllCarriers();


        PagedList GetShipmentForCompanyAndSyncWithSIS(long companyId);

        /// <summary>
        /// Toggle Shipment Favourites
        /// </summary>
        /// <param name="shipment"></param>
        /// <returns></returns>
        bool ToggleShipmentFavourites(ShipmentDto shipment);

        /// <summary>
        /// Toggle Shipment Favourites
        /// </summary>
        /// <param name="shipment"></param>
        /// <returns></returns>
        DashboardShipments GetShipmentStatusCounts(string userId);


        /// <summary>
        /// Search shipments by Tracking number/ shipment Id
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        PagedList SearchShipmentsById(string number);

        /// <summary>
        /// Get shipmentBy ShipmentCodeForAirwayBill
        /// </summary>
        /// <param name="shipmentCode"></param>
        /// <returns></returns>
        AirwayBillDto GetshipmentByShipmentCodeForAirwayBill(string shipmentCode);

        /// <summary>
        ///Get All Shipments For Admins
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        IList<ShipmentDto> GetAllShipmentsForAdmins();

        /// <summary>
        /// Get shipment by CodeShipment
        /// </summary>
        /// <param name="codeShipment">codeShipment</param>
        /// <returns></returns>
        ShipmentDto GetShipmentByCodeShipment(string codeShipment);

        /// <summary>
        /// load All Shipments ForExcel generation
        /// </summary>
        /// <param name="codeShipmen
        byte[] loadAllShipmentsForExcel(PagedList shipmentSerach);


        /// <summary>
        ///Get Shipment Details By TrackingNo
        /// </summary>
        /// <param name="trackingNo">trackingNo</param>
        /// <returns></returns>
        ShipmentDto GetShipmentDetailsByTrackingNo(string trackingNo);


        /// <summary>
        /// Charge from customer by using credit card
        /// </summary>
        /// <param name="payment">payment</param>
        /// <returns></returns>
        ShipmentOperationResult PaymentCharge(PaymentDto payment);

      
        /// <summary>
        /// load All Shipments For Admin Excel Export
        /// </summary>
        /// <param name="">payment</param>
        /// <returns></returns>
        byte[] loadAllShipmentsForAdminExcelExport(string status = null, DateTime? startDate = null, DateTime? endDate = null,
                                        string number = null, string source = null, string destination = null);

        OperationResult UpdateTrackingNo(AirwayBillDto awbDto);


        long GetTenantIdByUserId(string user);


        /// <summary>
        /// Get Payment by Reference
        /// </summary>
        /// <param name="">reference</param>
        /// <returns></returns>
        PaymentDto GetPaymentbyReference(long reference);

        DateTime? GetLocalTimeByUser(string loggedUserId, DateTime utcDatetime);

        ShipmentOperationResult UpdateShipmentReference(ShipmentDto addShipment);
    }

}
