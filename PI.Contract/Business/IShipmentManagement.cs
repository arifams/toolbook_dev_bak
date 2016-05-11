using PI.Contract.DTOs.Common;
using PI.Contract.DTOs.FileUpload;
using PI.Contract.DTOs.RateSheets;
using PI.Contract.DTOs.Shipment;
using PI.Contract.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Contract.Business
{
    public interface IShipmentManagement
    {
        ShipmentcostList GetRateSheet(ShipmentDto currentShipment);
        string GetInboundoutBoundStatus(string userId, string fromCode, string toCode);
        ShipmentOperationResult SaveShipment(ShipmentDto addShipment);
        PayLaneDto GetHashForPayLane(PayLaneDto payLaneDto);
        ShipmentOperationResult SendShipmentDetails(SendShipmentDetailsDto sendShipmentDetails);
        void InsertShipmentDocument(FileUploadDto fileDetails);
        CommercialInvoiceDto GetshipmentByShipmentCodeForInvoice(string shipmentCode);
        ShipmentOperationResult SaveCommercialInvoice(CommercialInvoiceDto addShipment);
        string RequestForQuote(ShipmentDto addShipment);
        List<FileUploadDto> GetAvailableFilesForShipmentbyTenant(string shipmentCode, string userId);
        void DeleteFileInDB(FileUploadDto fileDetails);
        PagedList GetAllShipmentsbyUser(string status, string userId, DateTime? startDate, DateTime? endDate,
                                              string number, string source, string destination);
        List<ShipmentDto> GetAllshipmentsForManifest(string userId, string date, string carreer, string reference);

        PagedList GetAllPendingShipmentsbyUser(string userId, DateTime? startDate, DateTime? endDate,
                                               string number);
        ShipmentDto GetshipmentById(string shipmentId);
        int DeleteShipment(string shipmentCode, string trackingNumber, string carrierName);
        StatusHistoryResponce GetTrackAndTraceInfo(string carrier, string trackingNumber);
        PagedList GetAllShipmentByCompanyId(string companyId);
        PagedList loadAllShipmentsFromCompanyAndSearch(string companyId, string status = null, DateTime? startDate = null, DateTime? endDate = null,
                                          string number = null, string source = null, string destination = null);

        string ShipmentReport(string userId, string languageId, ReportType reportType, short carrierId = 0, long companyId = 0, DateTime? startDate = null, DateTime? endDate = null);
    }

}
