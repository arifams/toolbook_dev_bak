using PI.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Office.Interop.Excel;
using PI.Data.Entity.RateEngine;
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
                        CarrierType = MyValues.GetValue(1, 2).ToString(),
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

                for (int index = 2; index <= lastRow; index++)
                {
                    Rate newRate = null;

                    System.Array MyValues = (System.Array)MySheet.get_Range("A" +
                       index.ToString(), "T" + index.ToString()).Cells.Value;

                    newRate = new Rate
                    {
                        ServiceLevel = MyValues.GetValue(1, 2).ToString(),
                        Carrier = MyValues.GetValue(1, 3).ToString(),
                        CountryFrom = MyValues.GetValue(1, 4).ToString(),
                        Inbound = MyValues.GetValue(1, 5).ToString(),
                        ServiceType = MyValues.GetValue(1, 6).ToString(),
                        WeightMin = Convert.ToDouble(MyValues.GetValue(1, 7).ToString()),
                        WeightMax = Convert.ToDouble(MyValues.GetValue(1, 8).ToString()),
                        Currency = MyValues.GetValue(1, 9).ToString(),
                        CalculationMethod = MyValues.GetValue(1, 10).ToString(),
                        VolumeFactor = Convert.ToInt32(MyValues.GetValue(1, 11).ToString()),
                        MaxLength = Convert.ToDouble(MyValues.GetValue(1, 12).ToString()),
                        MaxWeightPerPiece = Convert.ToDouble(MyValues.GetValue(1, 13).ToString()),
                        SellOrBuy = MyValues.GetValue(1, 14).ToString(),
                        TariffType = MyValues.GetValue(1, 19).ToString(),
                        MaxDimension = Convert.ToDouble(MyValues.GetValue(1, 20).ToString()),
                        CreatedBy = "1",//userId,
                        CreatedDate = DateTime.Now
                    };
                    context.Rate.Add(newRate);
                }
                context.SaveChanges();
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

                for (int index = 2; index <= lastRow; index++)
                {
                    Zone newZone = null;

                    System.Array MyValues = (System.Array)MySheet.get_Range("A" +
                       index.ToString(), "J" + index.ToString()).Cells.Value;

                    newZone = new Zone
                    {
                        ServiceLevel = MyValues.GetValue(1, 2).ToString(),
                        CarrierName = MyValues.GetValue(1, 3).ToString(),
                        CountryFrom = MyValues.GetValue(1, 4).ToString(),
                        CountryTo = MyValues.GetValue(1, 5).ToString(),
                        ZoneName = MyValues.GetValue(1, 6).ToString(),
                        LocationFrom = MyValues.GetValue(1, 7).ToString(),
                        LocationTo = MyValues.GetValue(1, 8).ToString(),
                        TariffType = MyValues.GetValue(1, 9).ToString(),
                        Inbound = MyValues.GetValue(1, 10).ToString(),
                        CreatedBy = "1",//userId,
                        CreatedDate = DateTime.Now
                    };
                    context.Zone.Add(newZone);
                }
                context.SaveChanges();
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

            //readin from excel

            using (PIContext context = new PIContext())
            {

                for (int index = 2; index <= lastRow; index++)
                {
                    TransmitTime newTransmitTime = null;

                    System.Array MyValues = (System.Array)MySheet.get_Range("A" +
                       index.ToString(), "H" + index.ToString()).Cells.Value;

                    newTransmitTime = new TransmitTime
                    {
                        ServiceLevel = MyValues.GetValue(1, 2).ToString(),
                        CarrierName = MyValues.GetValue(1, 3).ToString(),
                        CountryFrom = MyValues.GetValue(1, 4).ToString(),
                        CountryTo = MyValues.GetValue(1, 5).ToString(),
                        ZoneName = MyValues.GetValue(1, 6).ToString(),                      
                        CreatedBy = "1",//userId,
                        CreatedDate = DateTime.Now
                    };
                    context.TransmitTime.Add(newTransmitTime);
                }
                context.SaveChanges();
            }
        }

    }
}
