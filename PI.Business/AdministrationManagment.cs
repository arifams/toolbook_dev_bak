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

namespace PI.Business
{
    public class AdministrationManagment : IAdministrationManagment
    {
        public OperationResult ImportRateSheetExcel(string URI)
        {
            Workbook MyBook = null;
            Application MyApp = null;

            MyApp = new Application();
            MyApp.Visible = false;

            OperationResult result = new OperationResult();
            try
            {
                MyBook = MyApp.Workbooks.Open(URI);
            }
            catch (Exception ex)
            {
                result.Status = Status.Error;
                result.Message = ex.ToString();
            }

       
            result = InsertCarrierDetails(MyBook);
            if (result.Status != Status.Success)
                return result;

            result = InsertZoneDetails(MyBook);
            if (result.Status != Status.Success)
                return result;

            result = InsertTransmitTimeDetails(MyBook);
            if (result.Status != Status.Success)
                return result;

            result = InsertRateDetails(MyBook);
            
            return result;
        }


        /// <summary>
        /// Insert Carrier Details
        /// </summary>
        /// <param name="MyBook"></param>
        private OperationResult InsertCarrierDetails(Workbook MyBook)
        {
            OperationResult opResult = new OperationResult();

            Worksheet MySheet = null;
            int lastRow = 0;

            MySheet = (Worksheet)MyBook.Sheets[1]; // Explicit cast is not required here
            lastRow = MySheet.Cells.SpecialCells(XlCellType.xlCellTypeLastCell).Row;

            //readin from excel

            using (PIContext context = new PIContext())
            {
                try
                {
                    for (int index = 2; index <= lastRow; index++)
                    {
                        Carrier newCarrier = null;

                        System.Array MyValues = (System.Array)MySheet.get_Range("A" +
                           index.ToString(), "G" + index.ToString()).Cells.Value;

                        if (string.IsNullOrWhiteSpace(MyValues.GetValue(1, 1).ToString()))
                            break;

                        newCarrier = new Carrier
                        {
                            CarrierType = (CarrierType)Enum.Parse(typeof(CarrierType), MyValues.GetValue(1, 2).ToString(), true),
                            ServiceLevel = MyValues.GetValue(1, 3).ToString(),
                            CarrierName = MyValues.GetValue(1, 4).ToString(),
                            CarrierNameLong = MyValues.GetValue(1, 5).ToString(),
                            CarrierCountryCode = MyValues.GetValue(1, 6).ToString(),
                            CarrierAccountNumber = MyValues.GetValue(1, 7).ToString(),
                            CreatedBy = "1",//userId,
                            CreatedDate = DateTime.Now
                        };
                        context.Carrier.Add(newCarrier);
                    }

                    opResult.Status = 0 < context.SaveChanges() ? Status.Success : Status.Error;
                }
                catch (Exception ex)
                {
                    opResult.Status = Status.Error;
                    opResult.Message = "Carrier :" + ex.ToString();
                }
            }

            return opResult;
        }


        /// <summary>
        /// Insert Rate Details
        /// </summary>
        /// <param name="MyBook"></param>
        private OperationResult InsertRateDetails(Workbook MyBook)
        {
            OperationResult opResult = new OperationResult();

            Worksheet MySheet = null;
            int lastRow = 0;

            MySheet = (Worksheet)MyBook.Sheets[2]; // Explicit cast is not required here
            lastRow = MySheet.Cells.SpecialCells(XlCellType.xlCellTypeLastCell).Row;

            //readin from excel

            using (PIContext context = new PIContext())
            {

                try
                {
                    for (int index = 2; index <= lastRow; index++)
                    {
                        Rate newRate = null;

                        System.Array MyValues = (System.Array)MySheet.get_Range("A" +
                           index.ToString(), "T" + index.ToString()).Cells.Value;

                        if (string.IsNullOrWhiteSpace(MyValues.GetValue(1, 1).ToString()))
                            break;

                        string service = MyValues.GetValue(1, 2).ToString();
                        string carriername = MyValues.GetValue(1, 3).ToString();
                        string tarrifName = MyValues.GetValue(1, 19).ToString();

                        var rateZoneList = new List<RateZone>();

                        decimal zonePriceUS1 = decimal.Parse(MyValues.GetValue(1, 15).ToString());
                        Zone zone1 = context.Zone.Where(z => z.ZoneName == "US1").FirstOrDefault();
                        rateZoneList.Add(new RateZone() { Zone = zone1, CreatedDate = DateTime.Now, CreatedBy = "1", Price = zonePriceUS1 });

                        decimal zonePriceUS2 = decimal.Parse(MyValues.GetValue(1, 16).ToString());
                        Zone zone2 = context.Zone.Where(z => z.ZoneName == "US2").FirstOrDefault();
                        rateZoneList.Add(new RateZone() { Zone = zone2, CreatedDate = DateTime.Now, CreatedBy = "1", Price = zonePriceUS2 });

                        decimal zonePriceUS3 = decimal.Parse(MyValues.GetValue(1, 17).ToString());
                        Zone zone3 = context.Zone.Where(z => z.ZoneName == "US3").FirstOrDefault();
                        rateZoneList.Add(new RateZone() { Zone = zone3, CreatedDate = DateTime.Now, CreatedBy = "1", Price = zonePriceUS3 });

                        decimal zonePriceUS4 = decimal.Parse(MyValues.GetValue(1, 18).ToString());
                        Zone zone4 = context.Zone.Where(z => z.ZoneName == "US4").FirstOrDefault();
                        rateZoneList.Add(new RateZone() { Zone = zone4, CreatedDate = DateTime.Now, CreatedBy = "1", Price = zonePriceUS4 });

                        newRate = new Rate
                        {
                            Carrier = context.Carrier.Where(c => c.ServiceLevel == service && c.CarrierName == carriername).FirstOrDefault(),
                            CountryFrom = MyValues.GetValue(1, 4).ToString(),
                            IsInbound = string.Equals(MyValues.GetValue(1, 5).ToString(), "Yes", StringComparison.InvariantCultureIgnoreCase),
                            Service = (ProductType)Enum.Parse(typeof(ProductType), MyValues.GetValue(1, 6).ToString(), true),
                            WeightMin = Convert.ToDecimal(MyValues.GetValue(1, 7).ToString()),
                            WeightMax = Convert.ToDecimal(MyValues.GetValue(1, 8).ToString()),
                            Currency = (CurrencyType)Enum.Parse(typeof(CurrencyType), MyValues.GetValue(1, 9).ToString(), true),
                            CalculationMethod = (RatesCalculationMethod)Enum.Parse(typeof(RatesCalculationMethod), MyValues.GetValue(1, 10).ToString(), true),
                            VolumeFactor = Convert.ToInt32(MyValues.GetValue(1, 11).ToString()),
                            MaxLength = Convert.ToDecimal(MyValues.GetValue(1, 12).ToString()),
                            MaxWeightPerPiece = Convert.ToDecimal(MyValues.GetValue(1, 13).ToString()),
                            SellOrBuy = (RatesSell)Enum.Parse(typeof(RatesSell), MyValues.GetValue(1, 14).ToString(), true),
                            TariffType = context.TariffType.Where(t => t.TarrifName == tarrifName).FirstOrDefault(),
                            MaxDimension = Convert.ToDecimal(MyValues.GetValue(1, 20).ToString()),
                            CreatedBy = "1",//userId,
                            CreatedDate = DateTime.Now,
                            RateZoneList = rateZoneList
                        };
                        context.Rate.Add(newRate);
                    }
                    opResult.Status = 0 < context.SaveChanges() ? Status.Success : Status.Error;
                }
                catch (Exception ex)
                {
                    opResult.Status = Status.Error;
                    opResult.Message = "Rate :" + ex.ToString();
                }
            }

            return opResult;
        }


        /// <summary>
        /// Insert Zone Details
        /// </summary>
        /// <param name="MyBook"></param>
        private OperationResult InsertZoneDetails(Workbook MyBook)
        {
            OperationResult opResult = new OperationResult();

            Worksheet MySheet = null;
            int lastRow = 0;

            MySheet = (Worksheet)MyBook.Sheets[3]; // Explicit cast is not required here
            lastRow = MySheet.Cells.SpecialCells(XlCellType.xlCellTypeLastCell).Row;

            //readin from excel

            using (PIContext context = new PIContext())
            {
                try
                {
                    for (int index = 2; index <= lastRow; index++)
                    {
                        Zone newZone = null;

                        System.Array MyValues = (System.Array)MySheet.get_Range("A" +
                           index.ToString(), "J" + index.ToString()).Cells.Value;

                        if (string.IsNullOrWhiteSpace(MyValues.GetValue(1, 1).ToString()))
                            break;

                        string service = MyValues.GetValue(1, 2).ToString();
                        string carriername = MyValues.GetValue(1, 3).ToString();
                        string tarrifName = MyValues.GetValue(1, 9).ToString();

                        newZone = new Zone
                        {
                            Carrier = context.Carrier.Where(c => c.ServiceLevel == service && c.CarrierName == carriername).FirstOrDefault(),
                            CountryFrom = MyValues.GetValue(1, 4).ToString(),
                            CountryTo = MyValues.GetValue(1, 5).ToString(),
                            ZoneName = MyValues.GetValue(1, 6).ToString(),
                            LocationFrom = MyValues.GetValue(1, 7).ToString(),
                            LocationTo = MyValues.GetValue(1, 8).ToString(),
                            TariffType = context.TariffType.Where(t => t.TarrifName == tarrifName).FirstOrDefault(),
                            IsInbound = string.Equals(MyValues.GetValue(1, 10).ToString(), "Yes", StringComparison.InvariantCultureIgnoreCase),
                            CreatedBy = "1",//userId,
                            CreatedDate = DateTime.Now
                        };
                        context.Zone.Add(newZone);
                    }
                    opResult.Status = 0 < context.SaveChanges() ? Status.Success : Status.Error;
                }
                catch (Exception ex)
                {
                    opResult.Status = Status.Error;
                    opResult.Message = "Zone :" + ex.ToString();
                }
            }

            return opResult;
        }


        /// <summary>
        /// Insert Transmit Time Details
        /// </summary>
        /// <param name="MyBook"></param>
        private OperationResult InsertTransmitTimeDetails(Workbook MyBook)
        {
            OperationResult opResult = new OperationResult();

            Worksheet MySheet = null;
            int lastRow = 0;

            MySheet = (Worksheet)MyBook.Sheets[4]; // Explicit cast is not required here
            lastRow = MySheet.Cells.SpecialCells(XlCellType.xlCellTypeLastCell).Row;

            IList<TransitTimeProduct> transitTimeProdList;

            //readin from excel

            using (PIContext context = new PIContext())
            {
                try
                {
                    for (int index = 2; index <= lastRow; index++)
                    {
                        TransmitTime newTransmitTime = null;

                        System.Array MyValues = (System.Array)MySheet.get_Range("A" +
                           index.ToString(), "H" + index.ToString()).Cells.Value;

                        if (string.IsNullOrWhiteSpace(MyValues.GetValue(1, 1).ToString()))
                            break;

                        string service = MyValues.GetValue(1, 2).ToString();
                        string carriername = MyValues.GetValue(1, 3).ToString();
                        string zoneName = MyValues.GetValue(1, 6).ToString();

                        transitTimeProdList = new List<TransitTimeProduct>();

                        if (MyValues.GetValue(1, 7) != null && !string.IsNullOrWhiteSpace(MyValues.GetValue(1, 7).ToString()))
                            transitTimeProdList.Add(new TransitTimeProduct() { ProductType = ProductType.Document, Days = Convert.ToInt16(MyValues.GetValue(1, 7).ToString()), CreatedDate = DateTime.Now });

                        if (MyValues.GetValue(1, 8).ToString() != null && !string.IsNullOrWhiteSpace(MyValues.GetValue(1, 8).ToString()))
                            transitTimeProdList.Add(new TransitTimeProduct() { ProductType = ProductType.Box, Days = Convert.ToInt16(MyValues.GetValue(1, 8).ToString()), CreatedDate = DateTime.Now });

                        newTransmitTime = new TransmitTime
                        {
                            Carrier = context.Carrier.Where(c => c.ServiceLevel == service && c.CarrierName == carriername).FirstOrDefault(),
                            CountryFrom = MyValues.GetValue(1, 4).ToString(),
                            CountryTo = MyValues.GetValue(1, 5).ToString(),
                            Zone = context.Zone.Where(z => z.ZoneName == zoneName).FirstOrDefault(),
                            TransitTimeProductList = transitTimeProdList,
                            CreatedBy = "1",//userId,
                            CreatedDate = DateTime.Now
                        };
                        context.TransmitTime.Add(newTransmitTime);
                    }

                    opResult.Status = 0 < context.SaveChanges() ? Status.Success : Status.Error;
                }
                catch (Exception ex)
                {
                    opResult.Status = Status.Error;
                    opResult.Message = "Transmit :" + ex.ToString();
                }
            }

            return opResult;
        }

    }
}
