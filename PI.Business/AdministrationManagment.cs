using PI.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Office.Interop.Excel;
using PI.Data.Entity.RateEngine;
using PI.Common;
using PI.Contract.Enums;

using PI.Contract.Business;
using PI.Contract.DTOs;
using PI.Data.Entity;
using PI.Contract.DTOs.Invoice;
using PI.Contract.DTOs.Common;
using PI.Contract.DTOs.AuditTrail;
using PI.Contract;

namespace PI.Business
{
    public class AdministrationManagment : IAdministrationManagment
    {

        private PIContext context;
        private ILogger logger;

        public AdministrationManagment(ILogger logger, PIContext _context = null)
        {
            context = _context ?? new PIContext();
            this.logger = logger;
        }

        /// <summary>
        /// Import rate sheets to DB
        /// </summary>
        /// <param name="URI"></param>
        /// <returns></returns>
        public OperationResult ImportRateSheetExcel(string URI)
        {
            OperationResult result = new OperationResult();
            SLExcelReader excelReader = new SLExcelReader();

            result = InsertCarrierDetails(excelReader, URI);
            if (result.Status != Status.Success)
                return result;

            result = InsertZoneDetails(excelReader, URI);
            if (result.Status != Status.Success)
                return result;

            result = InsertTransmitTimeDetails(excelReader, URI);
            if (result.Status != Status.Success)
                return result;

            result = InsertRateDetails(excelReader, URI);

            return result;

        }


        /// <summary>
        /// Insert Carrier Details
        /// </summary>
        /// <param name="MyBook"></param>
        private OperationResult InsertCarrierDetails(SLExcelReader excelReader, string URI)
        {
            OperationResult opResult = new OperationResult();
            List<CarrierService> carrierList = new List<CarrierService>();

            SLExcelData excelData = excelReader.ReadExcel(URI, 1);
            try
            {
                if (excelData.DataRows.Count == 0)
                {
                    opResult.Status = Status.Error;
                }
                foreach (var item in excelData.DataRows)
                {
                    if (item.Count != 0)
                    {
                        var cellArray = item.ToArray();

                        if (cellArray.Length != 0)
                        {
                            carrierList.Add(new CarrierService()
                            {
                                CarrierType = (CarrierType)Enum.Parse(typeof(CarrierType), cellArray[1].ToString(), true),
                                ServiceLevel = cellArray[2].ToString(),
                                Carrier = new Carrier()
                                {
                                    CarrierNameLong = cellArray[4].ToString(),
                                    Name = cellArray[3].ToString(),
                                    CreatedBy = "1",
                                    CreatedDate = DateTime.Now,
                                    IsActive = true
                                },
                                CarrierCountryCode = cellArray[5].ToString(),
                                CarrierAccountNumber = cellArray[6].ToString(),
                                CreatedBy = "1",//userId,
                                CreatedDate = DateTime.Now
                            });
                        }
                    }
                }

                context.CarrierService.AddRange(carrierList);
                opResult.Status = (context.SaveChanges() > 0) ? Status.Success : Status.Error;
            }
            catch (Exception ex)
            {
                opResult.Status = Status.Error;
                opResult.Message = "Carrier :" + ex.ToString();
            }

            return opResult;
        }


        /// <summary>
        /// Insert Rate Details
        /// </summary>
        /// <param name="MyBook"></param>
        private OperationResult InsertRateDetails(SLExcelReader excelReader, string URI)
        {

            OperationResult opResult = new OperationResult();
            List<Rate> rateList = new List<Rate>();

            SLExcelData excelData = excelReader.ReadExcel(URI, 2);
            try
            {

                if (excelData.DataRows.Count == 0)
                {
                    opResult.Status = Status.Error;
                }
                foreach (var item in excelData.DataRows)
                {
                    if (item.Count != 0)
                    {
                        var cellArray = item.ToArray();

                        if (cellArray.Length != 0)
                        {
                            string service = cellArray[1].ToString();
                            string carriername = cellArray[2].ToString();
                            string tarrifName = cellArray[18].ToString();

                            var rateZoneList = new List<RateZone>();

                            decimal zonePriceUS1 = decimal.Parse(cellArray[14].ToString());
                            Zone zone1 = context.Zone.Where(z => z.ZoneName == "US1").FirstOrDefault();
                            rateZoneList.Add(new RateZone() { Zone = zone1, CreatedDate = DateTime.Now, CreatedBy = "1", Price = zonePriceUS1 });

                            decimal zonePriceUS2 = decimal.Parse(cellArray[15].ToString());
                            Zone zone2 = context.Zone.Where(z => z.ZoneName == "US2").FirstOrDefault();
                            rateZoneList.Add(new RateZone() { Zone = zone2, CreatedDate = DateTime.Now, CreatedBy = "1", Price = zonePriceUS2 });

                            decimal zonePriceUS3 = decimal.Parse(cellArray[16].ToString());
                            Zone zone3 = context.Zone.Where(z => z.ZoneName == "US3").FirstOrDefault();
                            rateZoneList.Add(new RateZone() { Zone = zone3, CreatedDate = DateTime.Now, CreatedBy = "1", Price = zonePriceUS3 });

                            decimal zonePriceUS4 = decimal.Parse(cellArray[17].ToString());
                            Zone zone4 = context.Zone.Where(z => z.ZoneName == "US4").FirstOrDefault();
                            rateZoneList.Add(new RateZone() { Zone = zone4, CreatedDate = DateTime.Now, CreatedBy = "1", Price = zonePriceUS4 });

                            rateList.Add(new Rate
                            {
                                Carrier = context.CarrierService.Where(c => c.ServiceLevel == service && c.Carrier.Name == carriername).FirstOrDefault(),
                                CountryFrom = cellArray[3].ToString(),
                                IsInbound = string.Equals(cellArray[4].ToString(), "Yes", StringComparison.InvariantCultureIgnoreCase),
                                Service = (ProductType)Enum.Parse(typeof(ProductType), cellArray[5].ToString(), true),
                                WeightMin = Convert.ToDecimal(cellArray[6].ToString()),
                                WeightMax = Convert.ToDecimal(cellArray[7].ToString()),
                                Currency = (CurrencyType)Enum.Parse(typeof(CurrencyType), cellArray[8].ToString(), true),
                                CalculationMethod = (RatesCalculationMethod)Enum.Parse(typeof(RatesCalculationMethod), cellArray[9].ToString(), true),
                                VolumeFactor = Convert.ToInt32(cellArray[10].ToString()),
                                MaxLength = Convert.ToDecimal(cellArray[11].ToString()),
                                MaxWeightPerPiece = Convert.ToDecimal(cellArray[12].ToString()),
                                SellOrBuy = (RatesSell)Enum.Parse(typeof(RatesSell), cellArray[13].ToString(), true),
                                TariffType = context.TariffType.Where(t => t.TarrifName == tarrifName).FirstOrDefault(),
                                MaxDimension = Convert.ToDecimal(cellArray[19].ToString()),
                                CreatedBy = "1",//userId,
                                CreatedDate = DateTime.Now,
                                RateZoneList = rateZoneList
                            });
                        }
                    }

                }

                context.Rate.AddRange(rateList);
                opResult.Status = (context.SaveChanges() > 0) ? Status.Success : Status.Error;
            }
            catch (Exception ex)
            {
                opResult.Status = Status.Error;
                opResult.Message = "Rate :" + ex.ToString();
            }

            return opResult;
        }


        /// <summary>
        /// Insert Zone Details
        /// </summary>
        /// <param name="MyBook"></param>
        private OperationResult InsertZoneDetails(SLExcelReader excelReader, string URI)
        {
            OperationResult opResult = new OperationResult();
            List<Zone> zoneList = new List<Zone>();

            SLExcelData excelData = excelReader.ReadExcel(URI, 3);
            try
            {

                if (excelData.DataRows.Count == 0)
                {
                    opResult.Status = Status.Error;
                }

                foreach (var item in excelData.DataRows)
                {

                    if (item.Count != 0)
                    {
                        var cellArray = item.ToArray();

                        if (cellArray.Length != 0)
                        {
                            string service = cellArray[1].ToString();
                            string carriername = cellArray[2].ToString();
                            string tarrifName = cellArray[8].ToString();

                            zoneList.Add(new Zone
                            {
                                Carrier = context.CarrierService.Where(c => c.ServiceLevel == service && c.Carrier.Name == carriername).FirstOrDefault(),
                                CountryFrom = cellArray[3].ToString(),
                                CountryTo = cellArray[4].ToString(),
                                ZoneName = cellArray[5].ToString(),
                                LocationFrom = cellArray[6].ToString(),
                                LocationTo = cellArray[7].ToString(),
                                TariffType = context.TariffType.Where(t => t.TarrifName == tarrifName).FirstOrDefault(),
                                IsInbound = string.Equals(cellArray[9].ToString(), "Yes", StringComparison.InvariantCultureIgnoreCase),
                                CreatedBy = "1",//userId,
                                CreatedDate = DateTime.Now
                            });
                        }
                    }

                }

                context.Zone.AddRange(zoneList);
                opResult.Status = (context.SaveChanges() > 0) ? Status.Success : Status.Error;

            }
            catch (Exception ex)
            {
                opResult.Status = Status.Error;
                opResult.Message = "Zone :" + ex.ToString();
            }

            return opResult;
        }


        /// <summary>
        /// Insert Transmit Time Details
        /// </summary>
        /// <param name="MyBook"></param>
        private OperationResult InsertTransmitTimeDetails(SLExcelReader excelReader, string URI)
        {

            OperationResult opResult = new OperationResult();
            List<TransmitTime> transmitTimeList = new List<TransmitTime>();
            IList<TransitTimeProduct> transitTimeProdList;

            SLExcelData excelData = excelReader.ReadExcel(URI, 4);
            try
            {

                if (excelData.DataRows.Count == 0)
                {
                    opResult.Status = Status.Error;
                }

                foreach (var item in excelData.DataRows)
                {
                    if (item.Count != 0)
                    {
                        var cellArray = item.ToArray();

                        if (cellArray.Length != 0)
                        {
                            string service = cellArray[1].ToString();
                            string carriername = cellArray[2].ToString();
                            string zoneName = cellArray[5].ToString();

                            transitTimeProdList = new List<TransitTimeProduct>();

                            if (cellArray[6] != null && !string.IsNullOrWhiteSpace(cellArray[6].ToString()))
                                transitTimeProdList.Add(new TransitTimeProduct() { ProductType = ProductType.Document, Days = Convert.ToInt16(cellArray[6].ToString()), CreatedDate = DateTime.Now });

                            if (cellArray[7].ToString() != null && !string.IsNullOrWhiteSpace(cellArray[7].ToString()))
                                transitTimeProdList.Add(new TransitTimeProduct() { ProductType = ProductType.Box, Days = Convert.ToInt16(cellArray[7].ToString()), CreatedDate = DateTime.Now });

                            transmitTimeList.Add(new TransmitTime()
                            {
                                Carrier = context.CarrierService.Where(c => c.ServiceLevel == service && c.Carrier.Name == carriername).FirstOrDefault(),
                                CountryFrom = cellArray[3].ToString(),
                                CountryTo = cellArray[4].ToString(),
                                Zone = context.Zone.Where(z => z.ZoneName == zoneName).FirstOrDefault(),
                                TransitTimeProductList = transitTimeProdList,
                                CreatedBy = "1",//userId,
                                CreatedDate = DateTime.Now
                            });
                        }
                    }
                }

                context.TransmitTime.AddRange(transmitTimeList);
                opResult.Status = (context.SaveChanges() > 0) ? Status.Success : Status.Error;

            }
            catch (Exception ex)
            {
                opResult.Status = Status.Error;
                opResult.Message = "Transmit :" + ex.ToString();
            }

            return opResult;
        }


        /// <summary>
        /// Manage invoice payment setting
        /// </summary>
        /// <param name="comapnyId"></param>
        /// <returns></returns>
        public bool ManageInvoicePaymentSetting(long comapnyId)
        {
            var comapny = context.Companies.Where(x => x.Id == comapnyId).SingleOrDefault();

            if (comapny != null)
            {
                comapny.IsInvoiceEnabled = !comapny.IsInvoiceEnabled;
                context.SaveChanges();
            }

            return comapny.IsInvoiceEnabled;        
        }


        /// <summary>
        /// Get Audit Trails For Customer
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<AuditTrailDto> GetAuditTrailsForCustomer(string userId)
        {
            List<AuditTrailDto> customerAuditRecords = new List<AuditTrailDto>();
       
            var auditRecords = context.AuditTrail.Where(x => x.CreatedBy == userId).ToList();

            foreach (var record in auditRecords)
            {
                customerAuditRecords.Add(new AuditTrailDto
                {
                    AppFunctionality = record.AppFunctionality,
                    Comments = record.Comments,
                    Result = record.Result,
                });
            }

            return customerAuditRecords;
        }

    }
}
