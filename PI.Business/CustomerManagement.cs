using PI.Contract.Business;
using PI.Contract.DTOs.Customer;
using PI.Data;
using PI.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public void SaveCustomer(CustomerDto customer)
        {
            using (var context = PIContext.Get())
            {
                if (customer.Id == 0)
                {
                    Customer newCustomer = new Customer();
                    newCustomer.CreatedDate = DateTime.Now;
                    newCustomer.CreatedBy = 1;//sessionHelper.Get<User>().LoginName; 
                    context.Customers.Add(newCustomer);
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
                    existingCustomer.MobileNumber = customer.PhoneNumber;

                    // existingCustomer.CustomerAddress.Country = 
                    // existingCustomer.CustomerAddress.Country =
                    //existingCustomer.CustomerAddress.ZipCode =
                    //existingCustomer.CustomerAddress.Number =
                    //existingCustomer.CustomerAddress.StreetAddress1 =
                    //existingCustomer.CustomerAddress.StreetAddress2 =
                    //existingCustomer.CustomerAddress.City =
                    //existingCustomer.CustomerAddress.State =
                }
                context.SaveChanges();
            }
        }
    }
}
