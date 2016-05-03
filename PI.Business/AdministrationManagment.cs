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

namespace PI.Business
{
    public class AdministrationManagment : IAdministrationManagment
    {

        public bool ImportRateSheetExcel(string URI)
        {
            Workbook MyBook = null;
            Application MyApp = null;

            MyApp = new Application();
            MyApp.Visible = false;
            MyBook = MyApp.Workbooks.Open(URI);

            InsertCarrierDetails(MyBook);

            InsertRateDetails(MyBook);

            InsertZoneDetails(MyBook);

            InsertTransmitTimeDetails(MyBook);

            return true;

        }


        /// <summary>
        /// Insert Carrier Details
        /// </summary>
        /// <param name="MyBook"></param>
        private void InsertCarrierDetails(Workbook MyBook)
        {
            Worksheet MySheet = null;
            int lastRow = 0;

            MySheet = (Worksheet)MyBook.Sheets[1]; // Explicit cast is not required here
            lastRow = MySheet.Cells.SpecialCells(XlCellType.xlCellTypeLastCell).Row;

            //readin from excel

            using (PIContext context = new PIContext())
            {

                for (int index = 2; index <= lastRow; index++)
                {
                    Carrier newCarrier = null;

                    System.Array MyValues = (System.Array)MySheet.get_Range("A" +
                       index.ToString(), "G" + index.ToString()).Cells.Value;

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
                context.SaveChanges();
            }
        }


        /// <summary>
        /// Insert Rate Details
        /// </summary>
        /// <param name="MyBook"></param>
        private void InsertRateDetails(Workbook MyBook)
        {
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
                            CreatedDate = DateTime.Now
                        };
                        context.Rate.Add(newRate);
                    }
                    context.SaveChanges();
                }
                catch (Exception ex)
                {

                }
            }
        }


        /// <summary>
        /// Insert Zone Details
        /// </summary>
        /// <param name="MyBook"></param>
        private void InsertZoneDetails(Workbook MyBook)
        {
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
                    context.SaveChanges();
                }
                catch (Exception ex)
                {

                }
            }
        }


        /// <summary>
        /// Insert Transmit Time Details
        /// </summary>
        /// <param name="MyBook"></param>
        private void InsertTransmitTimeDetails(Workbook MyBook)
        {
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
                    context.SaveChanges();
                }
                catch (Exception ex)
                {

                }
            }
        }

    }
}
