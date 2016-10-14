﻿using PI.Contract.Business;
using PI.Contract.DTOs.Customer;
using PI.Data;
using PI.Data.Entity;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.IdentityModel;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using System.Reflection;
using System.IdentityModel.Protocols.WSTrust;
using System.Configuration;
using PI.Contract.Enums;
using System.Data.Entity;
using PI.Contract;

namespace PI.Business
{
    public class CustomerManagement : ICustomerManagement
    {
        private PIContext context;
        private ILogger logger;

        public CustomerManagement(ILogger logger, PIContext _context = null)
        {          
            context = _context ?? PIContext.Get();
            this.logger = logger;
        }

        public string WebURL
        {
            get
            {
                return ConfigurationManager.AppSettings["BaseWebURL"].ToString();
            }
        }

        public string ServiceURL
        {
            get
            {
                return ConfigurationManager.AppSettings["ServiceURL"].ToString();
            }
        }

        /// <summary>
        /// Get customer by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Customer GetCustomerById(long id)
        {
            return context.Customers.Single(c => c.Id == id);
        }


        public void DeleteCustomer(string userId)
        {
            Customer customer = context.Customers.Where(cu => cu.UserId == userId).FirstOrDefault();
            Address customerAddress = customer != null ? customer.CustomerAddress : null;

            if (customerAddress != null)
                context.Addresses.Remove(customerAddress);

            if (customer != null)
                context.Customers.Remove(customer);

            context.SaveChanges();
        }

        /// <summary>
        /// Save customer details
        /// </summary>
        /// <param name="customer"></param>
        /// <param name="isCustomerRegistration"></param>
        /// <returns></returns>
        public int SaveCustomer(CustomerDto customer, bool isCustomerRegistration = false)
        {
            Customer newCustomer = null;

            try
            {
                if (customer.Id == 0)
                {
                    newCustomer = new Customer()
                    {
                        FirstName = customer.FirstName,
                        MiddleName = customer.MiddleName,
                        LastName = customer.LastName,
                        Salutation = customer.Salutation,
                        Email = customer.Email,
                        PhoneNumber = customer.PhoneNumber,
                        MobileNumber = customer.MobileNumber,
                        CreatedDate = DateTime.UtcNow,
                        CreatedBy = "1",//sessionHelper.Get<User>().LoginName; // TODO : Get created user.
                        UserId = customer.UserId,
                        //CompanyId = customer.CompanyId,
                        AddressId = (customer.AddressId > 0) ? customer.AddressId : 0,
                        CustomerAddress = (customer.AddressId > 0) ? null : new Address()
                        {
                            Country = customer.CustomerAddress.Country,
                            ZipCode = customer.CustomerAddress.ZipCode,
                            Number = customer.CustomerAddress.Number,
                            StreetAddress1 = customer.CustomerAddress.StreetAddress1,
                            StreetAddress2 = customer.CustomerAddress.StreetAddress2,
                            City = customer.CustomerAddress.City,
                            State = customer.CustomerAddress.State,
                            CreatedDate = DateTime.UtcNow,
                            CreatedBy = "1",//sessionHelper.Get<User>().LoginName; // TODO : Get created user.
                        }
                    };
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
                    existingCustomer.MobileNumber = customer.MobileNumber;
                    existingCustomer.CreatedDate = DateTime.UtcNow;
                    existingCustomer.CreatedBy = "1"; //sessionHelper.Get<User>().LoginName; 
                    existingCustomer.UserId = customer.UserId;

                    existingCustomer.CustomerAddress.Country = customer.CustomerAddress.Country;
                    existingCustomer.CustomerAddress.ZipCode = customer.CustomerAddress.ZipCode;
                    existingCustomer.CustomerAddress.Number = customer.CustomerAddress.Number;
                    existingCustomer.CustomerAddress.StreetAddress1 = customer.CustomerAddress.StreetAddress1;
                    existingCustomer.CustomerAddress.StreetAddress2 = customer.CustomerAddress.StreetAddress2;
                    existingCustomer.CustomerAddress.City = customer.CustomerAddress.City;
                    existingCustomer.CustomerAddress.State = customer.CustomerAddress.State;
                }
                context.SaveChanges();

                //Add Audit Trail Record for customer Registrations
                if (customer.Id == 0 && isCustomerRegistration)
                {
                    context.AuditTrail.Add(new AuditTrail
                    {
                        ReferenceId = newCustomer.Id.ToString(),
                        AppFunctionality = AppFunctionality.UserRegistration,
                        Result = "SUCCESS",
                        CreatedBy = "1",
                        CreatedDate = DateTime.UtcNow
                    });
                }

            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
                throw;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return 1;
        }


        /// <summary>
        /// Get Jwt token for user
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="role"></param>
        /// <param name="tenantId"></param>
        /// <param name="userName"></param>
        /// <param name="companyId"></param>
        /// <returns></returns>
        public string GetJwtToken(string userid, string role, string tenantId, string userName, string companyId)
        {

            var plainTextSecurityKey = "Secretkeyforparcelinternational_base64string_test1";
            var signingKey = new InMemorySymmetricSecurityKey(Encoding.UTF8.GetBytes(plainTextSecurityKey));
            var signingCredentials = new SigningCredentials(signingKey,
                SecurityAlgorithms.HmacSha256Signature, SecurityAlgorithms.Sha256Digest);

            var claimsIdentity = new ClaimsIdentity(new List<Claim>()
              {
                   new Claim("UserId", userid),
                   new Claim(ClaimTypes.Role, role),
                   new Claim("TenantId",tenantId ),
                   new Claim("UserName", userName),
                   new Claim("CompanyId",companyId )
              }, "Custom");

            var securityTokenDescriptor = new SecurityTokenDescriptor()
            {
                AppliesToAddress = WebURL,
                TokenIssuerName = ServiceURL,
                Subject = claimsIdentity,
                SigningCredentials = signingCredentials,
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var plainToken = tokenHandler.CreateToken(securityTokenDescriptor);
            var signedAndEncodedToken = tokenHandler.WriteToken(plainToken);
            return signedAndEncodedToken;
        }

        public byte[] GetBytes(string input)
        {
            var bytes = new byte[input.Length * sizeof(char)];
            Buffer.BlockCopy(input.ToCharArray(), 0, bytes, 0, bytes.Length); return bytes;
        }


        /// <summary>
        /// Get theme colour for user
        /// </summary>
        /// <param name="loggedInUserId"></param>
        /// <returns></returns>
        public string GetThemeColour(string loggedInUserId)
        {
            var existingCustomer = context.Customers.SingleOrDefault(c => c.UserId == loggedInUserId);

            if (existingCustomer == null)
                return null;

            return existingCustomer.SelectedColour;
        }


        /// <summary>
        /// Get customer details
        /// </summary>
        /// <param name="companyId"></param>
        /// <returns></returns>
        public CustomerDto GetCustomerByCompanyId(int companyId)
        {
            string BusinessOwnerId = context.Roles.Where(r => r.Name == "BusinessOwner").Select(r => r.Id).FirstOrDefault();

            var content = (from customer in context.Customers
                           join comapny in context.Companies on customer.User.TenantId equals comapny.TenantId
                           where customer.User.Roles.Any(r => r.RoleId == BusinessOwnerId) &&
                           comapny.Id == companyId
                           select new
                           {
                               Customer = customer,
                               Company = comapny
                           }).SingleOrDefault();

            return new CustomerDto()
            {
                CompanyName = content.Company.Name,
                FirstName = content.Customer.FirstName,
                LastName = content.Customer.LastName,
                Email = content.Customer.Email,
                PhoneNumber = content.Customer.PhoneNumber,
                Salutation = content.Customer.Salutation,
                MobileNumber = content.Customer.MobileNumber,

                CustomerAddress = new Contract.DTOs.Address.AddressDto()
                {
                    Id = content.Customer.CustomerAddress.Id,
                    City = content.Customer.CustomerAddress.City,
                    Country = content.Customer.CustomerAddress.Country,
                    Number = content.Customer.CustomerAddress.Number,
                    State = content.Customer.CustomerAddress.State,
                    StreetAddress1 = content.Customer.CustomerAddress.StreetAddress1,
                    StreetAddress2 = content.Customer.CustomerAddress.StreetAddress2,
                    ZipCode = content.Customer.CustomerAddress.ZipCode
                }
            };
        }

    }
}
