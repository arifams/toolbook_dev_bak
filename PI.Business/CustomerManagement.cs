using PI.Contract.Business;
using PI.Contract.DTOs.Customer;
using PI.Data;
using PI.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebMatrix.WebData;

namespace PI.Business
{
    public class CustomerManagement : ICustomerManagement
    {
        public Customer GetCustomerById(long id)
        {
            using (var context = PIContext.Get())
            {
                return context.Customers.Single(c => c.Id == id);
            }
        }

        public int SaveCustomer(CustomerDto customer)
        {
            using (var context = PIContext.Get())
            {
                if (customer.Id == 0)
                {
                    Customer newCustomer = new Customer()
                    {
                        FirstName = customer.FirstName,
                        MiddleName = customer.MiddleName,
                        LastName = customer.LastName,
                        Salutation = customer.Salutation,
                        Email = customer.Email,
                        PhoneNumber = customer.PhoneNumber, 
                        MobileNumber = customer.MobileNumber,
                        CreatedDate = DateTime.Now,
                        CreatedBy = 1,//sessionHelper.Get<User>().LoginName; // TODO : Get created user.
                        UserName = customer.UserName,
                        Password = customer.Password,
                        CustomerAddress = new Address()
                        {
                            Country = customer.CustomerAddress.Country,
                            ZipCode = customer.CustomerAddress.ZipCode,
                            Number = customer.CustomerAddress.Number,
                            StreetAddress1 = customer.CustomerAddress.StreetAddress1,
                            StreetAddress2 = customer.CustomerAddress.StreetAddress2,
                            City = customer.CustomerAddress.City,
                            State = customer.CustomerAddress.State,
                            CreatedDate = DateTime.Now,
                            CreatedBy = 1,//sessionHelper.Get<User>().LoginName; // TODO : Get created user.
                        }
                    };
                    context.Customers.Add(newCustomer);

                    // TODO : temp code.

                   // WebSecurity.CreateUserAndAccount("uname", "pwd");

                    //
                }
                else
                {
                    var existingCustomer = context.Customers.Single(c => c.Id == customer.Id);
                    //Never use context.Customers.Find(id) method to retrieve tenant entities
                    //If we do, that will enable malicious users to modify data belongs to another tenant
                    existingCustomer.FirstName = customer.FirstName;
                    existingCustomer.MiddleName = customer.MiddleName;
                    existingCustomer.LastName = customer.LastName;
                    existingCustomer.Salutation = customer.Salutation;
                    existingCustomer.Email = customer.Email;
                    existingCustomer.PhoneNumber = customer.PhoneNumber;
                    existingCustomer.MobileNumber = customer.MobileNumber;
                    existingCustomer.CreatedDate = DateTime.Now;
                    existingCustomer.CreatedBy = 1; //sessionHelper.Get<User>().LoginName; 

                    existingCustomer.CustomerAddress.Country =  customer.CustomerAddress.Country;
                    existingCustomer.CustomerAddress.ZipCode = customer.CustomerAddress.ZipCode;
                    existingCustomer.CustomerAddress.Number = customer.CustomerAddress.Number;
                    existingCustomer.CustomerAddress.StreetAddress1 = customer.CustomerAddress.StreetAddress1;
                    existingCustomer.CustomerAddress.StreetAddress2 = customer.CustomerAddress.StreetAddress2;
                    existingCustomer.CustomerAddress.City = customer.CustomerAddress.City;
                    existingCustomer.CustomerAddress.State = customer.CustomerAddress.State;
                }
                context.SaveChanges();
            }

            return 1;
        }

        public int VerifyUserLogin(CustomerDto customer)
        {
            using (var context = PIContext.Get())
            {
                var existingCustomer = context.Customers.SingleOrDefault(c => c.UserName == customer.Email && 
                    c.Password == customer.Password);

                if(existingCustomer != null)
                {
                    return 1;
                }
                return 0;
            }
        }
    }
}
