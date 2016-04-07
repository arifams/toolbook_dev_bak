using OfficeOpenXml;
using OfficeOpenXml.Style;
using PI.Contract.Business;
using PI.Contract.DTOs.AddressBook;
using PI.Contract.DTOs.Common;
using PI.Contract.DTOs.ImportAddress;
using PI.Data;
using PI.Data.Entity;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Excel = Microsoft.Office.Interop.Excel;

namespace PI.Business
{
    public class AddressBookManagement: IAddressBookManagement
    {

        public PagedList GetAllAddresses(string type, string userId, string searchtext, int page = 1, int pageSize = 25)
                                         
        {
            var pagedRecord = new PagedList();

            pagedRecord.Content = new List<AddressBookDto>();
            using (PIContext context = new PIContext())
            {
                var content = (from a in context.AddressBooks
                              where a.IsDelete == false &&
                              a.UserId==userId &&
                              (string.IsNullOrEmpty(searchtext) || a.CompanyName.Contains(searchtext) || a.FirstName.Contains(searchtext) || a.LastName.Contains(searchtext)) &&
                              (type == null || a.IsActive.ToString() == type)  
                              orderby a.CreatedDate ascending                             
                              select a)                            
                              .ToList();              

                foreach (var item in content)
                {
                    pagedRecord.Content.Add(new AddressBookDto
                    {
                        Id = item.Id,
                        CompanyName = item.CompanyName,
                        FullAddress = item.Number + "/ " + item.StreetAddress1 + "/ " + item.StreetAddress2,
                        FullName = item.FirstName+ " "+ item.LastName,
                        IsActive = item.IsActive,
                        FirstName=item.FirstName,
                        LastName=item.LastName,
                        UserId =item.UserId,
                        Salutation =item.Salutation,                    
                        EmailAddress=item.EmailAddress,
                        PhoneNumber=item.PhoneNumber,
                        AccountNumber=item.AccountNumber,

                        Country=item.Country,
                        ZipCode=item.ZipCode,                        
                        Number =item.Number,
                        StreetAddress1=item.StreetAddress1,
                        StreetAddress2=item.StreetAddress2,
                        City =item.City,                           
                        State=item.State                                 
 
                    });
                }

                // Count
                pagedRecord.TotalRecords = content.Count();

                pagedRecord.CurrentPage = page;
                pagedRecord.PageSize = pageSize;

                return pagedRecord;


            }

        }

        public int DeleteAddress(long id)
        {
            using (var context = new PIContext())
            {
                var AddressDetail = context.AddressBooks.SingleOrDefault(d => d.Id == id);

                if (AddressDetail == null)
                {
                    return -1;
                }
                else
                {
                    AddressDetail.IsActive = false;
                    AddressDetail.IsDelete = true;
                    context.SaveChanges();
                    return 1;
                }
            }

        }

        public int ImportAddressBook(IList<ImportAddressDto> addressDetails,string userId)
        {
           // IList<AddressBook> currentAddress =null;
            AddressBook currentAddress = new AddressBook();
            int addressCount = 0;

            var headerContent = addressDetails[0].CsvContent.ToString();
            string[] headerArray = headerContent.Split(',');

            //remove the header details
            addressDetails.Remove(addressDetails[0]);
            //var list = new Dictionary<KeyValuePair<string, string>>();
            Dictionary<string, string> list =new Dictionary<string, string>();

            foreach (var address in addressDetails)
            {
                string[] dataArray = address.CsvContent.ToString().Split(',');
                for (int i = 0; i < dataArray.Length; i++)
                {
                    try
                    {
                        list.Add(headerArray[i], dataArray[i]);
                    }
                    catch (Exception)
                    {
                        return -1;
                    }
                               
                }
                using (PIContext context=new PIContext())
                {
                    if (list.ContainsKey("companyName") && list.ContainsKey("salutation") && list.ContainsKey("firstName") && list.ContainsKey("lastName") && list.ContainsKey("emailAddress") && list.ContainsKey("phoneNumber") && list.ContainsKey("accountNumber") &&
                        list.ContainsKey("country") && list.ContainsKey("zipCode") && list.ContainsKey("number") && list.ContainsKey("streetAddress1") && list.ContainsKey("streetAddress2") && list.ContainsKey("city") &&
                        list.ContainsKey("state") && list.ContainsKey("isActive")  &&
                        list["companyName"]!= string.Empty&& list["salutation"]!=string.Empty && list["firstName"]!=string.Empty && list["lastName"] != string.Empty && list["emailAddress"] != string.Empty && list["phoneNumber"] != string.Empty && list["accountNumber"] != string.Empty 
                        && list["country"] != string.Empty && list["zipCode"] != string.Empty && list["number"] != string.Empty && list["streetAddress1"] != string.Empty && list["streetAddress2"] != string.Empty && list["city"] != string.Empty && list["state"] != string.Empty && list["isActive"] != string.Empty)
                    {
                        currentAddress.CompanyName = list["companyName"];
                        currentAddress.UserId = userId;
                        currentAddress.CreatedBy = userId;
                        currentAddress.Salutation = list["salutation"];
                        currentAddress.FirstName = list["firstName"];
                        currentAddress.LastName = list["lastName"];
                        currentAddress.EmailAddress = list["emailAddress"];
                        currentAddress.PhoneNumber = list["phoneNumber"];
                        currentAddress.AccountNumber = list["accountNumber"];
                        currentAddress.Country = list["country"];
                        currentAddress.ZipCode = list["zipCode"];
                        currentAddress.Number = list["number"];
                        currentAddress.StreetAddress1 = list["streetAddress1"];
                        currentAddress.StreetAddress2 = list["streetAddress2"];
                        currentAddress.City = list["city"];
                        currentAddress.State = list["state"];
                        currentAddress.IsActive = Convert.ToBoolean(list["isActive"]);
                        currentAddress.CreatedDate = DateTime.Now;


                        context.AddressBooks.Add(currentAddress);
                        context.SaveChanges();
                        addressCount++;
                    }
                }
               
                list.Clear();
            }           
            
            return addressCount;
        }

        public int SaveAddressDetail(AddressBookDto addressDetail)
        {
            AddressBook currentAddress = null;
            using (PIContext context=new PIContext())
            {
                if (addressDetail!=null)
                {
                    currentAddress = context.AddressBooks.SingleOrDefault(n => n.Id == addressDetail.Id); 
                }
             
                if (currentAddress!=null)
                {
                    currentAddress.CompanyName = addressDetail.CompanyName;
                    currentAddress.UserId = addressDetail.UserId;
                    currentAddress.CreatedBy = addressDetail.UserId;
                    currentAddress.Salutation = addressDetail.Salutation;
                    currentAddress.FirstName = addressDetail.FirstName;
                    currentAddress.LastName = addressDetail.LastName;
                    currentAddress.EmailAddress = addressDetail.EmailAddress;
                    currentAddress.PhoneNumber = addressDetail.PhoneNumber;
                    currentAddress.AccountNumber = addressDetail.AccountNumber;

                    //Address Status
                    currentAddress.Country = addressDetail.Country;
                    currentAddress.ZipCode = addressDetail.ZipCode;
                    currentAddress.Number = addressDetail.Number;
                    currentAddress.StreetAddress1 = addressDetail.StreetAddress1;
                    currentAddress.StreetAddress2 = addressDetail.StreetAddress2;
                    currentAddress.City = addressDetail.City;
                    currentAddress.State = addressDetail.State;
                    currentAddress.IsActive = addressDetail.IsActive;
                   // context.AddressBooks.Add(currentAddress);
                    context.SaveChanges();
                 }
                else
                {
                    currentAddress = new AddressBook();
                    currentAddress.CompanyName = addressDetail.CompanyName;
                    currentAddress.UserId = addressDetail.UserId;
                    currentAddress.CreatedBy = addressDetail.UserId;
                    currentAddress.Salutation = addressDetail.Salutation;
                    currentAddress.FirstName = addressDetail.FirstName;
                    currentAddress.LastName = addressDetail.LastName;
                    currentAddress.EmailAddress = addressDetail.EmailAddress;
                    currentAddress.PhoneNumber = addressDetail.PhoneNumber;
                    currentAddress.AccountNumber = addressDetail.AccountNumber;

                    //Address Status                    
                    currentAddress.Country = addressDetail.Country;
                    currentAddress.ZipCode = addressDetail.ZipCode;
                    currentAddress.Number = addressDetail.Number;
                    currentAddress.StreetAddress1 = addressDetail.StreetAddress1;
                    currentAddress.StreetAddress2 = addressDetail.StreetAddress2;
                    currentAddress.City = addressDetail.City;
                    currentAddress.State = addressDetail.State;
                    currentAddress.IsActive = addressDetail.IsActive;
                    currentAddress.CreatedDate = DateTime.Now;
                    context.AddressBooks.Add(currentAddress);
                    context.SaveChanges();

                }

            }
            return 1;

        }

        //get addressbook detail by id
        public AddressBook GetAddressBookById(long Id)
        {           
            using (PIContext context = new PIContext())
            {
                return context.AddressBooks.SingleOrDefault(n => n.Id == Id);
            }

        }


        //get addressbook detail by id
        public PagedList GetFilteredAddresses( string userId, string searchtext, int page = 1, int pageSize = 25)

        {
            var pagedRecord = new PagedList();

            pagedRecord.Content = new List<AddressBookDto>();
            using (PIContext context = new PIContext())
            {
                var content = (from a in context.AddressBooks
                               where a.IsDelete == false &&
                               a.CreatedBy == userId &&
                               (string.IsNullOrEmpty(searchtext) || a.CompanyName.Contains(searchtext) || a.FirstName.Contains(searchtext) || a.LastName.Contains(searchtext))                              
                               orderby a.CreatedDate ascending
                               select a)
                              .ToList();

                foreach (var item in content)
                {
                    pagedRecord.Content.Add(new AddressBookDto
                    {
                        Id = item.Id,
                        CompanyName = item.CompanyName,
                        FullAddress = item.Number + "/ " + item.StreetAddress1 + "/ " + item.StreetAddress2,
                        FullName = item.FirstName + " " + item.LastName,
                        IsActive = item.IsActive,
                        FirstName = item.FirstName,
                        LastName = item.LastName,
                        UserId = item.UserId,
                        Salutation = item.Salutation,
                        EmailAddress = item.EmailAddress,
                        PhoneNumber = item.PhoneNumber,
                        AccountNumber = item.AccountNumber,

                        Country = item.Country,
                        ZipCode = item.ZipCode,
                        Number = item.Number,
                        StreetAddress1 = item.StreetAddress1,
                        StreetAddress2 = item.StreetAddress2,
                        City = item.City,
                        State = item.State

                    });
                }

                // Count
                pagedRecord.TotalRecords = content.Count();

                pagedRecord.CurrentPage = page;
                pagedRecord.PageSize = pageSize;

                return pagedRecord;


            }

        }

        //rturn the address book detail if available
        public AddressBookDto GetAddressBookDtoById(long Id)
        {
           AddressBook currentAddress=  this.GetAddressBookById(Id);
            AddressBookDto resultAddress = new AddressBookDto();
            if (currentAddress!=null)
            {
                resultAddress.CompanyName= currentAddress.CompanyName;
                resultAddress.UserId=currentAddress.UserId;
                resultAddress.Salutation = currentAddress.Salutation;
                resultAddress.FirstName = currentAddress.FirstName;
                resultAddress.LastName = currentAddress.LastName;
                resultAddress.EmailAddress = currentAddress.EmailAddress;
                resultAddress.PhoneNumber = currentAddress.PhoneNumber;
                resultAddress.AccountNumber = currentAddress.AccountNumber;

                //Address Status      
               
                resultAddress.Country = currentAddress.Country;
                resultAddress.ZipCode = currentAddress.ZipCode;
                resultAddress.Number = currentAddress.Number;
                resultAddress.StreetAddress1 = currentAddress.StreetAddress1;
                resultAddress.StreetAddress2 = currentAddress.StreetAddress2;
                resultAddress.City = currentAddress.City;
                resultAddress.State = currentAddress.State;
                resultAddress.IsActive = currentAddress.IsActive;

            }
            return resultAddress;

        }

        //get address book details by excel file
        public int ExportExcelAddressbookDetails()
        {
            return 1;
        }
     
        
        //get address details by userId  
        public byte[] GetAddressBookDetailsByUserId(string userId)
        {
            List<AddressBookDto> addressList = new List<AddressBookDto>();

            using (PIContext context= new PIContext())
            {
                var content = (from addresses in context.AddressBooks
                               where addresses.CreatedBy == userId
                               select addresses).ToList();

                foreach (var item in content)
                {
                    AddressBookDto address = new AddressBookDto()
                    {
                        Id = item.Id,
                        CompanyName = item.CompanyName,                        
                        IsActive = item.IsActive,
                        FirstName = item.FirstName,
                        LastName = item.LastName,
                        UserId = item.UserId,
                        Salutation = item.Salutation,
                        EmailAddress = item.EmailAddress,
                        PhoneNumber = item.PhoneNumber,
                        AccountNumber = item.AccountNumber,

                        Country = item.Country,
                        ZipCode = item.ZipCode,
                        Number = item.Number,
                        StreetAddress1 = item.StreetAddress1,
                        StreetAddress2 = item.StreetAddress2,
                        City = item.City,
                        State = item.State

                    };

                    addressList.Add(address);
                }
            }


            byte[] stream = this.GenerateExcelSheetFromAddressBook(addressList);
            return stream;

        }

        public byte[] GenerateExcelSheetFromAddressBook(List<AddressBookDto> addressBookDtoList)
        {
            using (ExcelPackage pck = new ExcelPackage())
            {
                //Create the worksheet
                ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Addrerss Book");

                //Merging cells and create a center heading for out table
                ws.Cells[2, 1].Value = "Address Book Details";
                ws.Cells[2, 1, 2, 8].Merge = true;
                ws.Cells[2, 1, 2, 8].Style.Font.Bold = true;
                ws.Cells[2, 1, 2, 8].Style.Font.Size = 15;
                ws.Cells[2, 1, 2, 8].Style.Font.Name = "Calibri";
                ws.Cells[2, 1, 2, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                // Set headings.
                ws.Cells["A6"].Value = "Salutation";
                ws.Cells["B6"].Value = "First Name";
                ws.Cells["C6"].Value = "Last Name";
                ws.Cells["D6"].Value = "Company Name";
                ws.Cells["E6"].Value = "Zip Code";
                ws.Cells["F6"].Value = "Number";
                ws.Cells["G6"].Value = "Street Address1";
                ws.Cells["H6"].Value = "Street Address2";
                ws.Cells["I6"].Value = "State";
                ws.Cells["J6"].Value = "Email Adderess";
                ws.Cells["K6"].Value = "Phone Number";

                //Format the header for columns.
                using (ExcelRange rng = ws.Cells["A6:K6"])
                {
                    rng.Style.Font.Bold = true;
                    rng.Style.Fill.PatternType = ExcelFillStyle.Solid;                      //Set Pattern for the background to Solid
                    rng.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(79, 129, 189));  //Set color to dark blue
                    rng.Style.Font.Color.SetColor(Color.White);
                }

                //ws.Cells["A6:H6"].AutoFitColumns();

                // Set data.
                int rowIndex = 6;
                foreach (AddressBookDto addressBook in addressBookDtoList) // Adding Data into rows
                {
                    rowIndex++;

                    var cell = ws.Cells[rowIndex, 1];                    
                    cell.Value = addressBook.Salutation;

                    cell = ws.Cells[rowIndex, 2];
                    cell.Value = addressBook.FirstName;

                    cell = ws.Cells[rowIndex, 3];
                    cell.Value = addressBook.LastName;

                    cell = ws.Cells[rowIndex, 4];
                    cell.Value = addressBook.CompanyName;

                    cell = ws.Cells[rowIndex, 5];
                    cell.Value = addressBook.ZipCode;

                    cell = ws.Cells[rowIndex, 6];
                    cell.Value = addressBook.Number;

                    cell = ws.Cells[rowIndex, 7];
                    cell.Value = addressBook.StreetAddress1;

                    cell = ws.Cells[rowIndex, 8];
                    cell.Value = addressBook.StreetAddress2;

                    cell = ws.Cells[rowIndex, 9];
                    cell.Value = addressBook.State;

                    cell = ws.Cells[rowIndex, 10];
                    cell.Value = addressBook.EmailAddress;

                    cell = ws.Cells[rowIndex, 11];
                    cell.Value = addressBook.PhoneNumber;

                    ws.Row(rowIndex).Height = 120;
                }

                // Set width
                ws.Column(1).Width = 25;
                ws.Column(2).Width = 25;
                ws.Column(3).Width = 25;
                ws.Column(4).Width = 25;
                ws.Column(5).Width = 25;
                ws.Column(6).Width = 22;
                ws.Column(7).Width = 25;
                ws.Column(8).Width = 25;
                ws.Column(9).Width = 20;
                ws.Column(10).Width = 20;
                ws.Column(11).Width = 20;
                
                return pck.GetAsByteArray();
            }
        }

        //update addressBook details with records in excel
        public bool UpdateAddressBookDatafromExcel(string URI, string userId)
        {
           Excel.Workbook MyBook = null;
           Excel.Application MyApp = null;
           Excel.Worksheet MySheet = null;
           int lastRow = 0;

            MyApp = new Excel.Application();
            MyApp.Visible = false;
            MyBook = MyApp.Workbooks.Open(URI);
            MySheet = (Excel.Worksheet)MyBook.Sheets[1]; // Explicit cast is not required here
            lastRow = MySheet.Cells.SpecialCells(Excel.XlCellType.xlCellTypeLastCell).Row;

            //readin from excel
            List<AddressBook> EmpList = new List<AddressBook>();
            for (int index = 2; index <= lastRow; index++)
            {
                System.Array MyValues = (System.Array)MySheet.get_Range("A" +
                   index.ToString(), "D" + index.ToString()).Cells.Value;
                EmpList.Add(new AddressBook
                {
                    CompanyName = MyValues.GetValue(1, 1).ToString(),
                    FirstName = MyValues.GetValue(1, 2).ToString(),
                    LastName = MyValues.GetValue(1, 3).ToString(),
                    AccountNumber = MyValues.GetValue(1, 4).ToString(),
                    CreatedBy= userId,

                });
            }

            return true;

    }

    }
}
