using OfficeOpenXml;
using OfficeOpenXml.Style;
using PI.Common;
using PI.Contract;
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
    public class AddressBookManagement : IAddressBookManagement
    {
        private PIContext context;
        private ILogger logger;

        public AddressBookManagement(ILogger logger, PIContext _context = null)
        {
            context = _context ?? new PIContext();
            this.logger = logger;
        }

        public PagedList GetAllAddresses(string type, string userId, string searchtext, int page = 1, int pageSize = 25)
        {
            var pagedRecord = new PagedList();

            pagedRecord.Content = new List<AddressBookDto>();

            var content = (from a in context.AddressBooks
                           where a.IsDelete == false && a.UserId == userId &&
                           (string.IsNullOrEmpty(searchtext) || a.CompanyName.Contains(searchtext) || a.FirstName.Contains(searchtext) || a.LastName.Contains(searchtext)) &&
                           (type == null || a.IsActive.ToString() == type)
                           orderby a.CreatedDate ascending
                           select a).ToList();

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

        /// <summary>
        /// Delete an Address
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int DeleteAddress(long id)
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


        /// <summary>
        /// Import Address Book Details
        /// </summary>
        /// <param name="addressDetails"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public int ImportAddressBook(IList<ImportAddressDto> addressDetails, string userId)
        {
            Dictionary<string, string> list = new Dictionary<string, string>();
            AddressBook currentAddress = new AddressBook();
            int addressCount = 0;

            var headerContent = addressDetails[0].CsvContent.ToString();
            string[] headerArray = headerContent.Split(',');

            //remove the header(column) details
            addressDetails.Remove(addressDetails[0]);

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

                if (list.ContainsKey("companyName") && list.ContainsKey("salutation") && list.ContainsKey("firstName") &&
                    list.ContainsKey("lastName") && list.ContainsKey("emailAddress") && list.ContainsKey("phoneNumber") &&
                    list.ContainsKey("accountNumber") && list.ContainsKey("country") && list.ContainsKey("zipCode") &&
                    list.ContainsKey("number") && list.ContainsKey("streetAddress1") && list.ContainsKey("streetAddress2") && list.ContainsKey("city") &&
                    list.ContainsKey("state") && list.ContainsKey("isActive") && list["companyName"] != string.Empty &&
                    list["salutation"] != string.Empty && list["firstName"] != string.Empty && list["lastName"] != string.Empty &&
                    list["emailAddress"] != string.Empty && list["phoneNumber"] != string.Empty && list["accountNumber"] != string.Empty &&
                    list["country"] != string.Empty && list["zipCode"] != string.Empty && list["number"] != string.Empty &&
                    list["streetAddress1"] != string.Empty && list["streetAddress2"] != string.Empty && list["city"] != string.Empty &&
                    list["state"] != string.Empty && list["isActive"] != string.Empty)
                {
                    currentAddress.CompanyName = list["companyName"];
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
                    currentAddress.UserId = userId;
                    currentAddress.CreatedBy = userId;
                    currentAddress.IsActive = true;
                    currentAddress.CreatedDate = DateTime.Now;

                    context.AddressBooks.Add(currentAddress);
                    context.SaveChanges();
                    addressCount++;
                }

                list.Clear();
            }

            return addressCount;
        }


        /// <summary>
        /// Save address details.
        /// </summary>
        /// <param name="addressDetail"></param>
        /// <returns></returns>
        public int SaveAddressDetail(AddressBookDto addressDetail)
        {

            if (addressDetail == null)
            {
                return 0;
            }

            AddressBook currentAddress = context.AddressBooks.SingleOrDefault(n => n.Id == addressDetail.Id);

            if (currentAddress != null)
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

                //Update record
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

                // Add new record
                context.AddressBooks.Add(currentAddress);
                context.SaveChanges();
            }
            return 1;
        }

        
        /// <summary>
        /// Get addressbook detail by id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public AddressBook GetAddressBookById(long Id)
        {
            return context.AddressBooks.SingleOrDefault(n => n.Id == Id);
        }

        
        /// <summary>
        /// Get addressbook detail by filter
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="searchtext"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public PagedList GetFilteredAddresses(string userId, string searchtext, int page = 1, int pageSize = 25)

        {
            var pagedRecord = new PagedList();
            pagedRecord.Content = new List<AddressBookDto>();

            var content = (from a in context.AddressBooks
                           where a.IsDelete == false && a.CreatedBy == userId &&
                           (string.IsNullOrEmpty(searchtext) || a.CompanyName.Contains(searchtext) ||
                            a.FirstName.Contains(searchtext) || a.LastName.Contains(searchtext))
                           orderby a.CreatedDate ascending
                           select a).ToList();

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


        /// <summary>
        /// Get the address book detail if available
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public AddressBookDto GetAddressBookDtoById(long Id)
        {
            AddressBook currentAddress = this.GetAddressBookById(Id);
            AddressBookDto resultAddress = new AddressBookDto();

            if (currentAddress != null)
            {
                resultAddress.CompanyName = currentAddress.CompanyName;
                resultAddress.UserId = currentAddress.UserId;
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


        /// <summary>
        /// Get address details for user as a byte array
        /// </summary>
        /// <param name="type"></param>
        /// <param name="userId"></param>
        /// <param name="searchtext"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public byte[] GetAddressBookDetailsByUserId(string type, string userId, string searchtext, int page = 1, int pageSize = 25)
        {
            List<AddressBookDto> addressList = new List<AddressBookDto>();

            var content = (from a in context.AddressBooks
                           where a.IsDelete == false &&
                           a.UserId == userId &&
                           (string.IsNullOrEmpty(searchtext) || a.CompanyName.Contains(searchtext) || a.FirstName.Contains(searchtext) || a.LastName.Contains(searchtext)) &&
                           (type == null || a.IsActive.ToString() == type)
                           orderby a.CreatedDate ascending
                           select a)
                         .ToList();

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
   
            return this.GenerateExcelSheetFromAddressBook(addressList);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="addressBookDtoList"></param>
        /// <returns></returns>
        private byte[] GenerateExcelSheetFromAddressBook(List<AddressBookDto> addressBookDtoList)
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
                ws.Cells["L6"].Value = "Country";
                ws.Cells["M6"].Value = "Account Number";


                //Format the header for columns.
                using (ExcelRange rng = ws.Cells["A6:M6"])
                {
                    rng.Style.Font.Bold = true;
                    rng.Style.Fill.PatternType = ExcelFillStyle.Solid;                      //Set Pattern for the background to Solid
                    rng.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(79, 129, 189));  //Set color to dark blue
                    rng.Style.Font.Color.SetColor(Color.White);
                }

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

                    cell = ws.Cells[rowIndex, 12];
                    cell.Value = addressBook.Country;

                    cell = ws.Cells[rowIndex, 13];
                    cell.Value = addressBook.AccountNumber;


                    ws.Row(rowIndex).Height = 25;
                }

                // Set width
                for (int i = 1; i <= 13; i++)
                {
                    ws.Column(i).Width = 25;
                }

                return pck.GetAsByteArray();
            }
        }


        /// <summary>
        /// Update addressBook details from excel to DB.
        /// </summary>
        /// <param name="URI"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public bool UpdateAddressBookDatafromExcel(string URI, string userId)
        {
            SLExcelReader excelReaders = new SLExcelReader();
            List<AddressBook> addressList = new List<AddressBook>();

            SLExcelData excelData = excelReaders.ReadExcel(URI);

            if (excelData.DataRows.Count == 0)
            {
                return false;
            }
            foreach (var item in excelData.DataRows)
            {
                if (item.Count != 0)
                {
                    var detailsarray = item.ToArray();
                    if (detailsarray.Length != 0)
                    {
                        addressList.Add(new AddressBook()
                        {
                            Salutation = detailsarray[0].ToString(),
                            FirstName = detailsarray[1].ToString(),
                            LastName = detailsarray[2].ToString(),
                            CompanyName = detailsarray[3].ToString(),
                            ZipCode = detailsarray[4].ToString(),
                            Number = detailsarray[5].ToString(),
                            StreetAddress1 = detailsarray[6].ToString(),
                            StreetAddress2 = detailsarray[7].ToString(),
                            City = detailsarray[8].ToString(),
                            State = detailsarray[9].ToString(),
                            EmailAddress = detailsarray[10].ToString(),
                            PhoneNumber = detailsarray[11].ToString(),
                            Country = detailsarray[12].ToString(),
                            AccountNumber = detailsarray.Length > 13 ? detailsarray[12].ToString() : string.Empty,
                            CreatedBy = userId,
                            CreatedDate = DateTime.Now,
                            UserId = userId,
                            IsActive = true
                        });
                    }
                }

            }

            context.AddressBooks.AddRange(addressList);
            context.SaveChanges();

            return true;
        }


    }

}

