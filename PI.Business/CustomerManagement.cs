using PI.Contract.Business;
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
            try
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
                            CreatedBy = "1",//sessionHelper.Get<User>().LoginName; // TODO : Get created user.
                            UserName = customer.Email,
                            Password = customer.Password,
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
                                CreatedDate = DateTime.Now,
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
                        existingCustomer.CreatedDate = DateTime.Now;
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
            catch (Exception ex) {
                throw ex;
            }

            return 1;
        }

        public int VerifyUserLogin(CustomerDto customer)
        {
            using (var context = PIContext.Get())
            {
                var existingCustomer = context.Customers.SingleOrDefault(c => c.UserName == customer.UserName && 
                    c.Password == customer.Password);

                if(existingCustomer != null)
                {
                    return 1;
                }
                return 0;
            }
        }

        public string GetJwtToken(string userid, string role,string tenantId,string userName,string companyId)
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
                AppliesToAddress = "http://localhost:49995/",
                TokenIssuerName = "http://localhost:55555/",
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
    }
}
