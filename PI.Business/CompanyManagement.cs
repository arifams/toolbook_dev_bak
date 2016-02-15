using AutoMapper;
using PI.Contract.Business;
using PI.Contract.DTOs.CostCenter;
using PI.Contract.DTOs.Customer;
using PI.Contract.DTOs.Division;
using PI.Data;
using PI.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Business
{
    public class CompanyManagement : ICompanyManagement
    {
        #region Create Comapny Details with default settings

        public long CreateCompanyDetails(CustomerDto customerCompany)
        {            
            using (var context = PIContext.Get())
            {
                //Add Tenant
                Tenant tenant = new Tenant
                {
                    TenancyName = customerCompany.CompanyCode,
                    CreatedBy = 1,
                    CreatedDate = DateTime.Now,
                    IsActive = true,
                    IsDelete = false,
                    IsCorporateAccount = customerCompany.IsCorporateAccount                    
                };
                context.Tenants.Add(tenant);
                context.SaveChanges();

                //Add Company
                Company company = new Company
                {
                    Name = customerCompany.CompanyName,
                    TenantId = tenant.Id,                    
                    IsActive = true,
                    IsDelete = false,
                    CreatedBy = 1,
                    CreatedDate = DateTime.Now,
                };
                context.Companies.Add(company);
                context.SaveChanges();

                //Add Default CostCenter
                CostCenter costCenter = new CostCenter
                {
                    CompanyId = company.Id,
                    Type = "SYSTEM",
                    Status = 1,
                    IsActive = true,
                    IsDelete = false,
                    CreatedBy = 1,
                    CreatedDate = DateTime.Now,
                    BillingAddress = new Address
                    {
                        Country = customerCompany.CustomerAddress.Country,
                        ZipCode = customerCompany.CustomerAddress.ZipCode,
                        Number = customerCompany.CustomerAddress.Number,
                        StreetAddress1 = customerCompany.CustomerAddress.StreetAddress1,
                        StreetAddress2 = customerCompany.CustomerAddress.StreetAddress2,
                        City = customerCompany.CustomerAddress.City,
                        State = customerCompany.CustomerAddress.State,
                        CreatedDate = DateTime.Now,
                        CreatedBy = 1,//sessionHelper.Get<User>().LoginName; // TODO : Get created user.
                    },
                };
                context.CostCenters.Add(costCenter);
                context.SaveChanges();

                //Add Default Division
                Division division = new Division
                {
                    CompanyId = company.Id,
                    Type = "SYSTEM",
                    DefaultCostCenterId = costCenter.Id,
                    Status = 1,                    
                    IsActive = true,
                    IsDelete = false,
                    CreatedBy = 1,
                    CreatedDate = DateTime.Now,
                };
                context.Divisions.Add(division);
                context.SaveChanges();

                return company.Id;
            }

        }

        #endregion

        #region Cost Center Managment

        /// <summary>
        /// Get all costCenters for the tenant coampny
        /// </summary>
        /// <returns></returns>
        public IList<CostCenterDto> GetAllCostCentersForCompany()
        {
            using (var context = PIContext.Get())
            {
                //var costCenterList = context.CostCenters.Where(x => x.Company.TenantId == 1).ToList();
                //return Mapper.Map<List<CostCenter>, List<CostCenterDto>>(costCenterList);
                return null;
            }
        }


        /// <summary>
        /// Get a particular costCenter by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public CostCenterDto GetCostCenterById(long id)
        {
            using (var context = PIContext.Get())
            {
                var costCenter = context.CostCenters.SingleOrDefault(c => c.Id == id);
                return Mapper.Map<CostCenter, CostCenterDto>(costCenter);
            }
        }


        /// <summary>
        /// Add/Update costCenter
        /// </summary>
        /// <param name="costCenter"></param>
        /// <returns></returns>
        public int SaveCostCenter(CostCenterDto costCenter)
        {

            using (var context = PIContext.Get())
            {
                var sysCostCenter = context.CostCenters.Where(c => c.CompanyId == 1
                                                               && c.Type == "SYSTEM").SingleOrDefault(); //TODO: get the comanyId of the tenant.
                if (costCenter.Id == 0 && sysCostCenter == null)
                {
                    CostCenter newCostCenter = new CostCenter()
                    {
                        Name = costCenter.Name,
                        Description = costCenter.Description,
                        PhoneNumber = costCenter.PhoneNumber,
                        Status = costCenter.Status,
                        CompanyId = costCenter.CompanyId,
                        Type = "USER",
                        CreatedDate = DateTime.Now,
                        CreatedBy = 1,// TODO : Get created user.       
                        BillingAddress = new Address
                        {
                            Country = costCenter.BillingAddress.Country,
                            ZipCode = costCenter.BillingAddress.ZipCode,
                            Number = costCenter.BillingAddress.Number,
                            StreetAddress1 = costCenter.BillingAddress.StreetAddress1,
                            StreetAddress2 = costCenter.BillingAddress.StreetAddress2,
                            City = costCenter.BillingAddress.City,
                            State = costCenter.BillingAddress.State,
                            CreatedDate = DateTime.Now,
                            CreatedBy = 1,//sessionHelper.Get<User>().LoginName; // TODO : Get created user.
                        },
                    };
                    context.CostCenters.Add(newCostCenter);

                    // TODO: Add Assigned Devisions
                }
                else
                {
                    CostCenter existingCostCenter = new CostCenter();

                    if (sysCostCenter != null)
                    {
                        existingCostCenter = sysCostCenter;
                    }
                    else
                    {
                        existingCostCenter = context.CostCenters.SingleOrDefault(d => d.Id == costCenter.Id);
                    }

                    if (costCenter.AssignedDivisions.Count() == 0)
                    { // Add the default division of the company if user defined divisions are not available.
                        var defaultDivision = context.Divisions.Where(c => c.CompanyId == 1
                                                                             && c.Type == "SYSTEM").SingleOrDefault();

                        costCenter.AssignedDivisions.Add(Mapper.Map<Division, DivisionDto>(defaultDivision));
                    }

                    existingCostCenter.Name = costCenter.Name;
                    existingCostCenter.Description = costCenter.Description;
                    existingCostCenter.PhoneNumber = costCenter.PhoneNumber;
                    existingCostCenter.Status = costCenter.Status;
                    existingCostCenter.CompanyId = costCenter.CompanyId;
                    existingCostCenter.Type = "USER";
                    existingCostCenter.CreatedDate = DateTime.Now;
                    existingCostCenter.CreatedBy = 1; //sessionHelper.Get<User>().LoginName; 

                    // TODO: Add Assigned Devisions
                }
                context.SaveChanges();
            }

            return 1;
        }

        #endregion

        #region Division Managment

        /// <summary>
        /// Get all divisions for the tenant coampny
        /// </summary>
        /// <returns></returns>
        public IList<DivisionDto> GetAllDivisionsForCompany()
        {
            using (var context = PIContext.Get())
            {
                var divisionList = context.Divisions.Where(x => x.Company.TenantId == 1).ToList();
                return Mapper.Map<List<Division>, List<DivisionDto>>(divisionList);
            }
        }


        /// <summary>
        /// Get a particular division by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public DivisionDto GetDivisionById(long id)
        {
            using (var context = PIContext.Get())
            {
                var division = context.Divisions.SingleOrDefault(d => d.Id == id);
                return Mapper.Map<Division, DivisionDto>(division);
            }
        }


        /// <summary>
        /// Add/Update division
        /// </summary>
        /// <param name="division"></param>
        /// <returns></returns>
        public int SaveDivision(DivisionDto division)
        {

            using (var context = PIContext.Get())
            {
                var sysDivision = context.Divisions.Where(d => d.CompanyId == 1
                                                               && d.Type == "SYSTEM").SingleOrDefault(); //TODO: get the comanyId of the tenant.
                if (division.Id == 0 && sysDivision == null)
                {
                    Division newDivision = new Division()
                    {
                        Name = division.Name,
                        Description = division.Description,
                        DefaultCostCenterId = division.DefaultCostCenterId,
                        Status = division.Status,
                        CompanyId = division.CompanyId,
                        Type = "USER",
                        CreatedDate = DateTime.Now,
                        CreatedBy = 1,// TODO : Get created user.                       
                    };
                    context.Divisions.Add(newDivision);
                }
                else
                {
                    Division existingDivision = new Division();

                    if (sysDivision != null)
                    {
                        existingDivision = sysDivision;
                    }
                    else
                    {
                        existingDivision = context.Divisions.SingleOrDefault(d => d.Id == division.Id);
                    }

                    if (division.DefaultCostCenterId == 0)
                    {
                        //division.DefaultCostCenterId = context.CostCenter.Where(c => c.CompanyId == 1
                        //                                                             && c.Type == "SYSTEM").SingleOrDefault().Id;
                    }

                    existingDivision.Name = division.Name;
                    existingDivision.Description = division.Description;
                    existingDivision.DefaultCostCenterId = division.DefaultCostCenterId;
                    existingDivision.Status = division.Status;
                    existingDivision.CompanyId = division.CompanyId;
                    existingDivision.Type = "USER";
                    existingDivision.CreatedDate = DateTime.Now;
                    existingDivision.CreatedBy = 1; //sessionHelper.Get<User>().LoginName; 

                }
                context.SaveChanges();
            }

            return 1;
        }

        #endregion

    }
}
