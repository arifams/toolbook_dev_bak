using PI.Contract.Business;
using PI.Contract.DTOs.AddressBook;
using PI.Contract.DTOs.Common;
using PI.Contract.DTOs.ImportAddress;
using PI.Data;
using PI.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Business
{
    public class AddressBookManagement: IAddressBookManagement
    {

        public PagedList GetAllAddresses(string type, string userId, string searchtext, int page = 1, int pageSize = 10)
                                         
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
                        FullAddress = item.Number + ", " + item.StreetAddress1 + ", " + item.StreetAddress2,
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
                    list.Add(headerArray[i], dataArray[i]);                 
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
            using (PIContext context = PIContext.Get())
            {
                return context.AddressBooks.SingleOrDefault(n => n.Id == Id);
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

    }
}
