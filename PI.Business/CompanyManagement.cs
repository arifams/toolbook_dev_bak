using AutoMapper;
using Microsoft.AspNet.Identity.EntityFramework;
using PI.Contract.Business;
using PI.Contract.DTOs.Admin;
using PI.Contract.DTOs.Common;
using PI.Contract.DTOs.Company;
using PI.Contract.DTOs.CostCenter;
using PI.Contract.DTOs.Customer;
using PI.Contract.DTOs.Division;
using PI.Contract.DTOs.Role;
using PI.Contract.DTOs.User;
using PI.Contract.Enums;
using PI.Data;
using PI.Data.Entity;
using PI.Data.Entity.Identity;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Dynamic;
using System.Text;
using System.Threading.Tasks;

namespace PI.Business
{
    public class CompanyManagement : ICompanyManagement
    {
        #region Create Comapny Details with default settings
        CommonLogic commonLogics = new CommonLogic();


        public long CreateCompanyDetails(CustomerDto customerCompany)
        {
            using (var context = PIContext.Get())
            {
                //Add Tenant
                Tenant tenant = new Tenant
                {
                    // TenancyName = customerCompany.CompanyCode,
                    CreatedBy = "1",
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
                    CreatedBy = "1",
                    CreatedDate = DateTime.Now,
                    CompanyCode = customerCompany.CompanyCode
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
                    CreatedBy = "1",
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
                        CreatedBy = "1",//sessionHelper.Get<User>().LoginName; // TODO : Get created user.
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
                    CreatedBy = "1",
                    CreatedDate = DateTime.Now,
                };
                context.Divisions.Add(division);
                context.SaveChanges();

                return tenant.Id;
            }

        }

        #endregion


        #region Cost Center Managment

        /// <summary>
        /// Get all costCenters for the tenant coampny
        /// </summary>
        /// <returns></returns>        
        public IList<CostCenterDto> GetAllCostCentersForCompany(string userId)
        {
            IList<CostCenterDto> costCenterList = new List<CostCenterDto>();
            Company currentcompany = commonLogics.GetCompanyByUserId(userId);
            if (currentcompany == null)
            {
                return null;
            }

            using (var context = new PIContext())//PIContext.Get())
            {
                var costcenters = context.CostCenters.Where(c => c.CompanyId == currentcompany.Id &&
                                                                 c.Type == "USER"
                    // TODO: get the company id of the logged in user.
                                                                  && c.IsDelete == false).ToList();


                foreach (var item in costcenters)
                {
                    costCenterList.Add(new CostCenterDto
                    {
                        Id = item.Id,
                        Name = item.Name
                    });
                }
            }

            return costCenterList;
        }


        //Get costcenters by division Id
        public IList<CostCenterDto> GetCostCentersbyDivision(string divisionId)
        {
            IList<CostCenterDto> costCenterList = new List<CostCenterDto>();

            using (var context = new PIContext())
            {
                var costCenters = from costcenter in context.CostCenters
                                  join ccdivision in context.DivisionCostCenters on costcenter.Id equals ccdivision.CostCenterId
                                  where ccdivision.DivisionId.ToString() == divisionId
                                  && ccdivision.IsActive == true
                                  select costcenter;

                foreach (var item in costCenters)
                {
                    costCenterList.Add(new CostCenterDto
                    {
                        Id = item.Id,
                        Name = item.Name
                    });
                }
            }
            return costCenterList;
        }


        /// <summary>
        /// Get all divisions by given filter criteria
        /// </summary>
        /// <param name="searchtext"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="sortBy"></param>
        /// <param name="sortDirection"></param>
        /// <returns></returns>
        public PagedList GetAllCostCenters(long divisionId, string type, string userId, string searchtext, int page = 1, int pageSize = 10,
                                         string sortBy = "Id", string sortDirection = "asc")
        {
            var pagedRecord = new PagedList();
            Company currentcompany = commonLogics.GetCompanyByUserId(userId);
            if (currentcompany == null)
            {
                return pagedRecord;
            }

            pagedRecord.Content = new List<CostCenterDto>();
            bool isBusinessOwner = IsLoggedInAsBusinessOwner(userId);

            using (var context = new PIContext())
            {
                var content = context.CostCenters.Include("BillingAddress").Include("DivisionCostCenters")
                                        .Where(x => x.CompanyId == currentcompany.Id &&
                                                     x.Type == "USER" &&
                                                     x.IsDelete == false &&
                                                    (isBusinessOwner || (!isBusinessOwner && x.DivisionCostCenters.Any(d => d.Divisions.UserInDivisions.Any(u => u.IsActive && u.UserId == userId)))) &&
                                                    (string.IsNullOrEmpty(searchtext) || x.Name.Contains(searchtext)) &&
                                                    (type == "0" || x.IsActive.ToString() == type) &&
                                                    (divisionId == 0 || x.DivisionCostCenters.Any(cd => cd.DivisionId == divisionId && cd.IsDelete == false))
                                                    )
                                            .OrderBy(sortBy + " " + sortDirection)
                                            .ToList();

                string assignedDivForGrid = string.Empty;
                int lastIndexOfBrTag;

                foreach (var item in content)
                {
                    StringBuilder str = new StringBuilder();
                    item.DivisionCostCenters.Where(x => x.IsDelete == false).ToList().ForEach(e => str.Append(e.Divisions.Name + "<br/>"));

                    // Remove last <br/> tag.
                    assignedDivForGrid = str.ToString();
                    lastIndexOfBrTag = assignedDivForGrid.LastIndexOf("<br/>");
                    if (lastIndexOfBrTag != -1)
                        assignedDivForGrid = assignedDivForGrid.Remove(lastIndexOfBrTag);

                    pagedRecord.Content.Add(new CostCenterDto
                    {
                        Id = item.Id,
                        Name = item.Name,
                        CompanyId = item.CompanyId,
                        Description = item.Description,
                        Status = item.Status,
                        Type = item.Type,
                        FullBillingAddress = (item.BillingAddress == null) ? null : item.BillingAddress.Number + " " + item.BillingAddress.StreetAddress1 + " " +
                        item.BillingAddress.StreetAddress2 + " " + item.BillingAddress.City + " " + item.BillingAddress.State + " " + item.BillingAddress.Country,
                        AssignedDivisionsForGrid = assignedDivForGrid,
                        StatusString = item.IsActive ? "Active" : "InActive"
                    });
                }

                // Count
                pagedRecord.TotalRecords = context.CostCenters.Include("DivisionCostCenters").Where(x => x.CompanyId == currentcompany.Id &&
                                                                      x.Type == "USER" && x.IsDelete == false &&
                                                                     (searchtext == null || x.Name.Contains(searchtext)) &&
                                                                     (divisionId == 0 || x.DivisionCostCenters.Any(C => C.DivisionId == divisionId)) &&
                                                                     (type == null || x.IsActive.ToString() == type)).Count();

                pagedRecord.CurrentPage = page;
                pagedRecord.PageSize = pageSize;

                return pagedRecord;
            }
        }



        /// <summary>
        /// Get a particular cost center by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public CostCenterDto GetCostCentersById(long id, string userId)
        {
            IList<DivisionDto> divisionList = new List<DivisionDto>();

            using (var context = new PIContext())
            {
                //var divisions = context.Divisions.Where(c => c.CompanyId == 1 &&  // TODO: get comapnyId from Tenanant
                //                                             c.Type == "USER" && c.IsDelete == false).ToList();

                //foreach (var item in divisions)
                //{
                //    divisionList.Add(new DivisionDto
                //    {
                //        Id = item.Id,
                //        Name = item.Name
                //    });
                //}

                divisionList = GetAllActiveDivisionsForCompany(userId);

                if (id == 0)
                {
                    return new CostCenterDto
                    {
                        Id = 0,
                        AllDivisions = divisionList,
                        AssignedDivisionIdList = new List<long>()
                    };
                }

                var costCenter = context.CostCenters.Include("DivisionCostCenters").SingleOrDefault(c => c.Id == id);

                // find and mark assigned div and cost
                foreach (DivisionDto div in divisionList)
                {
                    div.IsAssigned = costCenter.DivisionCostCenters.Where(cd => cd.DivisionId == div.Id
                                                                                            && cd.IsDelete == false).ToList().Count() > 0;
                }

                if (costCenter != null)
                {
                    return new CostCenterDto
                    {
                        Id = costCenter.Id,
                        Name = costCenter.Name,
                        Type = costCenter.Type,
                        PhoneNumber = costCenter.PhoneNumber,
                        Description = costCenter.Description,
                        Status = costCenter.Status,
                        CompanyId = costCenter.CompanyId,
                        BillingAddress = new Contract.DTOs.Address.AddressDto
                        {
                            Number = costCenter.BillingAddress.Number,
                            StreetAddress1 = costCenter.BillingAddress.StreetAddress1,
                            StreetAddress2 = costCenter.BillingAddress.StreetAddress2,
                            City = costCenter.BillingAddress.City,
                            State = costCenter.BillingAddress.State,
                            ZipCode = costCenter.BillingAddress.ZipCode,
                            Country = costCenter.BillingAddress.Country
                        },
                        AllDivisions = divisionList,
                        AssignedDivisionIdList = costCenter.DivisionCostCenters.Select(e => e.DivisionId).ToList()
                    };

                }
            }

            return null;
        }


        /// <summary>
        /// Add/Update costCenter
        /// </summary>
        /// <param name="costCenter"></param>
        /// <returns></returns>
        public int SaveCostCenter(CostCenterDto costCenter)
        {
            long comapnyId = commonLogics.GetCompanyByUserId(costCenter.UserId).Id;

            using (var context = new PIContext())
            {
                // var isSpaceOrEmpty = String.IsNullOrWhiteSpace(costCenter.Description);
                var isSameCostName = context.CostCenters.Where(c => c.CompanyId == comapnyId &&
                                                                  c.Type == "USER" &&
                                                                  c.Id != costCenter.Id &&
                                                                  c.Name == costCenter.Name).SingleOrDefault();
                if (isSameCostName != null)
                {
                    return -1;
                }

                IList<DivisionCostCenter> divcostList = new List<DivisionCostCenter>();

                foreach (var division in costCenter.AssignedDivisionIdList)
                {
                    divcostList.Add(new DivisionCostCenter()
                    {
                        CostCenterId = costCenter.Id,
                        DivisionId = division,
                        IsActive = true,
                        CreatedBy = "1",
                        CreatedDate = DateTime.Now
                    });
                }

                if (costCenter.Id == 0)
                {
                    Company comp = commonLogics.GetCompanyByUserId(costCenter.UserId);

                    CostCenter newCostCenter = new CostCenter()
                    {
                        Name = costCenter.Name,
                        Description = costCenter.Description,
                        PhoneNumber = costCenter.PhoneNumber,
                        Status = costCenter.Status,
                        CompanyId = comp == null ? 0 : comp.Id, //costCenter.CompanyId, TODO H - why?
                        Type = "USER",
                        CreatedDate = DateTime.Now,
                        CreatedBy = "1",// TODO : Get created user.       
                        BillingAddress = (costCenter.BillingAddress == null) ? null : new Address
                        {
                            Country = costCenter.BillingAddress.Country,
                            ZipCode = costCenter.BillingAddress.ZipCode,
                            Number = costCenter.BillingAddress.Number,
                            StreetAddress1 = costCenter.BillingAddress.StreetAddress1,
                            StreetAddress2 = costCenter.BillingAddress.StreetAddress2,
                            City = costCenter.BillingAddress.City,
                            State = costCenter.BillingAddress.State,
                            CreatedDate = DateTime.Now,
                            CreatedBy = "1",//sessionHelper.Get<User>().LoginName; // TODO : Get created user.
                        },
                        IsActive = costCenter.Status == 1 ? true : false,
                    };
                    context.CostCenters.Add(newCostCenter);
                    context.SaveChanges();

                    divcostList.ToList().ForEach(x=> x.CostCenterId = newCostCenter.Id);
                    context.DivisionCostCenters.AddRange(divcostList);
                    context.SaveChanges();


                }
                else
                {
                    CostCenter existingCostCenter = new CostCenter();
                    existingCostCenter = context.CostCenters.SingleOrDefault(d => d.Id == costCenter.Id);

                    //Remove the existing active connection list
                    context.DivisionCostCenters.Include("CostCenters").Where(x => x.CostCenterId == costCenter.Id
                                                                                    && x.CostCenters.IsActive).ToList().ForEach
                                                                    (dc => { dc.IsActive = false; dc.IsDelete = true; });


                    //if (costCenter.AssignedDivisions == null || costCenter.AssignedDivisions.Count() == 0)
                    //{ // Add the default division of the company if user defined divisions are not available.
                    //    var defaultDivision = context.Divisions.Where(c => c.CompanyId == comapnyId
                    //                                                        && c.Type == "SYSTEM").SingleOrDefault();

                    //    existingCostCenter.DivisionCostCenters.Add(new DivisionCostCenter()
                    //    {
                    //        DivisionId = defaultDivision.Id,
                    //        IsActive = true,
                    //        CreatedBy = 1,
                    //        CreatedDate = DateTime.Now
                    //    });
                    //}

                    existingCostCenter.Name = costCenter.Name;
                    existingCostCenter.Description = costCenter.Description;
                    existingCostCenter.PhoneNumber = costCenter.PhoneNumber;
                    existingCostCenter.Status = costCenter.Status;
                    existingCostCenter.IsActive = costCenter.Status == 1 ? true : false;
                    existingCostCenter.CompanyId = costCenter.CompanyId;
                    existingCostCenter.Type = "USER";
                    existingCostCenter.BillingAddress.Number = costCenter.BillingAddress.Number;
                    existingCostCenter.BillingAddress.StreetAddress1 = costCenter.BillingAddress.StreetAddress1;
                    existingCostCenter.BillingAddress.StreetAddress2 = costCenter.BillingAddress.StreetAddress2;
                    existingCostCenter.BillingAddress.City = costCenter.BillingAddress.City;
                    existingCostCenter.BillingAddress.ZipCode = costCenter.BillingAddress.ZipCode;
                    existingCostCenter.BillingAddress.State = costCenter.BillingAddress.State;
                    existingCostCenter.CreatedDate = DateTime.Now;
                    existingCostCenter.CreatedBy = "1"; //sessionHelper.Get<User>().LoginName; 
                    //existingCostCenter.DivisionCostCenters.ToList().AddRange(divcostList);
                    // TODO: Add Assigned Devisions
                    // TODO: Need to handle prevent remove default cost center in div.
                    context.DivisionCostCenters.AddRange(divcostList);

                    context.SaveChanges();

                }

            }

            return 1;
        }


        public int DeleteCostCenter(long id)
        {
            using (var context = new PIContext())
            {
                var costCenter = context.CostCenters.SingleOrDefault(d => d.Id == id);

                if (costCenter == null)
                {
                    return -1;
                }
                else
                {
                    //Remove the Assigned division list
                    context.DivisionCostCenters.Where(x => x.CostCenterId == id).ToList().ForEach
                                                                     (dc => { dc.IsActive = false; dc.IsDelete = true; });

                    costCenter.IsActive = false;
                    costCenter.IsDelete = true;

                    context.SaveChanges();

                    return 1;
                }
            }

        }



        #endregion


        #region Division Managment

        /// <summary>
        /// Get all costCenters for the tenant coampny
        /// </summary>
        /// <returns></returns>
        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public IList<DivisionDto> GetAllActiveDivisionsForCompany(string userId)
        {
            IList<DivisionDto> divisionList = new List<DivisionDto>();
            Company currentcompany = commonLogics.GetCompanyByUserId(userId);
            if (currentcompany == null)
            {
                return null;
            }

            using (var context = new PIContext())//PIContext.Get())
            {
                var divisions = context.Divisions.Where(c => c.CompanyId == currentcompany.Id &&
                                                             c.Type == "USER" && c.IsActive == true).ToList();

                foreach (var item in divisions)
                {
                    divisionList.Add(new DivisionDto
                    {
                        Id = item.Id,
                        Name = item.Name
                    });
                }
            }

            return divisionList;
        }

        public IList<DivisionDto> GetAllActiveDivisionsOfUser(string userId)
        {
            IList<DivisionDto> divisionList = new List<DivisionDto>();
            Company currentcompany = commonLogics.GetCompanyByUserId(userId);
            if (currentcompany == null)
            {
                return null;
            }

            using (var context = new PIContext())
            {
                var divisions = context.UsersInDivisions.Where(ud => ud.UserId == userId
                                                                     && !ud.IsDelete
                                                                     && ud.IsActive).ToList();

                foreach (var item in divisions)
                {
                    divisionList.Add(new DivisionDto
                    {
                        Id = item.DivisionId,
                        Name = item.Divisions.Name
                    });
                }
            }

            return divisionList;
        }

        /// <summary>
        /// Get all costCenters for the tenant coampny
        /// </summary>
        /// <returns></returns>
        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<DivisionDto> GetAllDivisionsForCompany(string userId)
        {
            List<DivisionDto> divisionList = new List<DivisionDto>();
            Company currentcompany = commonLogics.GetCompanyByUserId(userId);
            if (currentcompany == null)
            {
                return null;
            }

            using (var context = new PIContext())//PIContext.Get())
            {
                var divisions = context.Divisions.Where(c => c.CompanyId == currentcompany.Id &&
                                                             c.Type == "USER" && c.IsDelete == false).ToList();

                foreach (var item in divisions)
                {
                    divisionList.Add(new DivisionDto
                    {
                        Id = item.Id,
                        Name = item.Name
                    });
                }
            }

            return divisionList;
        }

        //get the assigned division by 
        public IList<DivisionDto> GetAssignedDivisions(string userid)
        {
            IList<DivisionDto> divisionList = new List<DivisionDto>();

            using (var context = new PIContext())
            {
                var divisions = from division in context.Divisions
                                join divUser in context.UsersInDivisions on division.Id equals divUser.DivisionId
                                where divUser.UserId == userid
                                select division;

                foreach (var item in divisions)
                {
                    divisionList.Add(new DivisionDto
                    {
                        Id = item.Id,
                        Name = item.Name
                    });
                }
            }

            return divisionList;
        }


        /// <summary>
        /// Get all divisions by given filter criteria
        /// </summary>
        /// <param name="searchtext"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="sortBy"></param>
        /// <param name="sortDirection"></param>
        /// <returns></returns>
        public PagedList GetAllDivisions(long costCenterId, string type, string userId, string searchtext, int page = 1, int pageSize = 10,
                                         string sortBy = "CustomerID", string sortDirection = "asc")
        {
            var pagedRecord = new PagedList();
            Company currentcompany = commonLogics.GetCompanyByUserId(userId);
            bool isBusinessOwner = IsLoggedInAsBusinessOwner(userId);

            if (currentcompany == null)
            {
                return pagedRecord;
            }
            pagedRecord.Content = new List<DivisionDto>();

            using (var context = new PIContext())
            {
                var content = context.Divisions.Include("DivisionCostCenters")
                                        .Where(x => x.CompanyId == currentcompany.Id && x.Type == "USER" &&
                                                    x.IsDelete == false &&
                                                    (isBusinessOwner || !isBusinessOwner && x.UserInDivisions.Any(d => d.IsActive && d.UserId == userId)) &&
                                                    (string.IsNullOrEmpty(searchtext) || x.Name.Contains(searchtext)) &&
                                                    (type == "0" || x.IsActive.ToString() == type) &&
                                                    (costCenterId == 0 || x.DivisionCostCenters.Any(cd => cd.CostCenterId == costCenterId && cd.IsDelete == false))
                                                    )

                                            .OrderBy(sortBy + " " + sortDirection)
                                            .ToList();

                string assosiatedCostCentersForGrid = string.Empty;
                int lastIndexOfBrTag;

                foreach (var item in content)
                {
                    StringBuilder stringResult = new StringBuilder();
                    context.DivisionCostCenters.Include("CostCenters").Where(c => c.DivisionId == item.Id && c.IsDelete == false).ToList().
                                                    ForEach(e => stringResult.Append(e.CostCenters.Name + "<br/>"));
                    // item.DivisionCostCenters.ToList().ForEach(e => stringResult.Append(e.Divisions.Name + "</br>"));

                    // Remove last <br/> tag.
                    assosiatedCostCentersForGrid = stringResult.ToString();
                    lastIndexOfBrTag = assosiatedCostCentersForGrid.LastIndexOf("<br/>");
                    if (lastIndexOfBrTag != -1)
                        assosiatedCostCentersForGrid = assosiatedCostCentersForGrid.Remove(lastIndexOfBrTag);

                    pagedRecord.Content.Add(new DivisionDto
                    {
                        Id = item.Id,
                        Name = item.Name,
                        CompanyId = item.CompanyId,
                        DefaultCostCenterId = item.DefaultCostCenterId,
                        Description = item.Description,
                        Status = item.Status,
                        StatusString = item.IsActive ? "Active" : "InActive",
                        Type = item.Type,
                        NumberOfUsers = item.UserInDivisions.Where(x => x.DivisionId==item.Id && x.IsActive).ToList().Count() + 1, //Add the business owner since he is not in the UserInDivisions table.
                        AssosiatedCostCentersForGrid = assosiatedCostCentersForGrid
                    });
                }

                // Count
                pagedRecord.TotalRecords = context.Divisions
                                           .Where(x => x.CompanyId == currentcompany.Id && x.Type == "USER" && x.IsDelete == false &&
                                                    (searchtext == null || x.Name.Contains(searchtext)) &&
                                                    (costCenterId == 0 || x.DefaultCostCenterId == costCenterId) &&
                                                    (type == null || x.IsActive.ToString() == type)).Count();

                pagedRecord.CurrentPage = page;
                pagedRecord.PageSize = pageSize;
                pagedRecord.TotalPages = (int)Math.Ceiling((decimal)pagedRecord.TotalRecords / pagedRecord.PageSize);

                return pagedRecord;
            }
        }


        /// <summary>
        /// Get all divisions for the tenant coampny
        /// </summary>
        /// <returns></returns>
        //public IList<DivisionDto> GetAllDivisionsForCompany()
        //{
        //    using (var context = PIContext.Get())
        //    {
        //        //var divisionList = context.Divisions.Where(x => x.Company.TenantId == 1).ToList();
        //        //return Mapper.Map<List<Division>, List<DivisionDto>>(divisionList);
        //        return null;
        //    }
        //}


        /// <summary>
        /// Get a particular division by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public DivisionDto GetDivisionById(long id, string userId)
        {
            IList<CostCenterDto> costCenterList = new List<CostCenterDto>();
            long companyId = commonLogics.GetCompanyByUserId(userId).Id;

            using (var context = new PIContext())
            {
                if (id == 0)
                {
                    return new DivisionDto
                    {
                        Id = 0,
                        //AssosiatedCostCenters = costCenterList
                    };
                }


                var division = context.Divisions.Include("DivisionCostCenters").SingleOrDefault(d => d.Id == id);

                division.DivisionCostCenters = context.DivisionCostCenters.Include("CostCenters").Where(d => d.DivisionId == id && d.IsDelete == false).ToList();

                division.DivisionCostCenters.ToList()
                                              .ForEach(c => costCenterList.Add(new CostCenterDto
                                                                {
                                                                    Id = c.CostCenterId,
                                                                    Name = c.CostCenters.Name
                                                                }));

                if (division != null)
                {
                    return new DivisionDto
                    {
                        Id = division.Id,
                        Name = division.Name,
                        Type = division.Type,
                        Description = division.Description,
                        Status = division.Status,
                        DefaultCostCenterId = division.DefaultCostCenterId,
                        CompanyId = division.CompanyId,
                        AssosiatedCostCenters = costCenterList
                    };

                }
            }

            return null;
        }


        /// <summary>
        /// Add/Update division
        /// </summary>
        /// <param name="division"></param>
        /// <returns></returns>
        public int SaveDivision(DivisionDto division)
        {
            long comapnyId = commonLogics.GetCompanyByUserId(division.UserId).Id;
            using (var context = PIContext.Create())
            {
                var isSpaceOrEmpty = String.IsNullOrWhiteSpace(division.Description);
                var isSameDiviName = context.Divisions.Where(d => d.Id != division.Id
                                                                && d.CompanyId == comapnyId
                                                                && d.Type == "USER" &&
                                                                (d.Name == division.Name || (d.Description == division.Description && !isSpaceOrEmpty))).SingleOrDefault();

                if (isSameDiviName != null)
                {
                    return -1;
                }

                var sysDivision = context.Divisions.Where(d => d.CompanyId == comapnyId
                                                               && d.Type == "SYSTEM").SingleOrDefault(); //TODO: get the comanyId of the tenant.

                // If no cost centers are currently setup, assign the default cost center for the division.
                if (division.DefaultCostCenterId == 0)
                {
                    var defaultCostCntr = context.CostCenters.Where(c => c.CompanyId == comapnyId
                                                                                 && c.Type == "SYSTEM").SingleOrDefault();

                    division.DefaultCostCenterId = (defaultCostCntr != null) ? defaultCostCntr.Id : 0;
                }


                if (division.Id == 0 && sysDivision == null)
                {

                    Division newDivision = new Division()
                    {
                        Name = division.Name,
                        Description = division.Description,
                        DefaultCostCenterId = division.DefaultCostCenterId,
                        Status = division.Status,
                        CompanyId = comapnyId,
                        Type = "USER",
                        IsActive = division.Status == 1 ? true : false,
                        CreatedDate = DateTime.Now,
                        CreatedBy = "1",// TODO : Get created user.                       
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

                    existingDivision.Name = division.Name;
                    existingDivision.Description = division.Description;
                    existingDivision.DefaultCostCenterId = division.DefaultCostCenterId;
                    existingDivision.Status = division.Status;
                    existingDivision.CompanyId = comapnyId;
                    existingDivision.Type = "USER";
                    existingDivision.IsActive = division.Status == 1 ? true : false;
                    existingDivision.CreatedDate = DateTime.Now;
                    existingDivision.CreatedBy = "1"; //sessionHelper.Get<User>().LoginName; 

                }
                context.SaveChanges();
            }

            return 1;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int DeleteDivision(long id)
        {
            using (var context = new PIContext())
            {
                var division = context.Divisions.SingleOrDefault(d => d.Id == id);

                if (division == null)
                {
                    return -1;
                }
                else
                {
                    context.DivisionCostCenters.Where(x => x.DivisionId == id).ToList().ForEach
                                                                                        (dc => { dc.IsActive = false; dc.IsDelete = true; });

                    division.IsActive = false;
                    division.IsDelete = true;

                    context.SaveChanges();

                    return 1;
                }
            }

        }
    

        #endregion


        #region User Management


        public bool IsLoggedInAsBusinessOwner(string userId)
        {
            using (var context = new PIContext())
            {
                string roleId = context.Users.Where(u => u.Id == userId).FirstOrDefault().Roles.FirstOrDefault().RoleId;
                string roleName = context.Roles.Where(r => r.Id == roleId).Select(r => r.Name).FirstOrDefault();

                return (roleName == "BusinessOwner") ? true : false;
            }
        }

        public string GetLoggedInUserName(string userId)
        {
            string userName = null;

            using (PIContext context = new PIContext())
            {
                ApplicationUser user = context.Users.Where(u => u.Id == userId).SingleOrDefault();

                if (user != null)
                {
                    userName = user.FirstName+" "+ user.LastName;
                }
            }

            return userName;
        }


        /// <summary>
        ///  Update LastLoginTime on every successful login.
        /// </summary>
        /// <param name="userId"></param>
        public void UpdateLastLoginTimeAndAduitTrail(string userId)
        {
            using (PIContext context = new PIContext())
            {
                ApplicationUser user = context.Users.Where(u => u.Id == userId).SingleOrDefault();

                if (user != null)
                {
                    user.LastLoginTime = DateTime.Now;
                    context.SaveChanges();
                }


                //Add Audit Trail Record 
                context.AuditTrail.Add(new AuditTrail
                {
                    ReferenceId = user.Id.ToString(),
                    AppFunctionality = AppFunctionality.UserLogin,
                    Result = "SUCCESS",
                    CreatedBy = "1",
                    CreatedDate = DateTime.Now
                });
                
            }
        }

        /// <summary>
        /// Get all roles under the current user role
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<RolesDto> GetAllActiveChildRoles(string userId)
        {
            List<RolesDto> roles = new List<RolesDto>();

            Guid userRoleId;
            string stringUserRoleId;
            string userRoleName = "";
            List<IdentityRole> allRoles = new List<IdentityRole>();

            using (PIContext context = new PIContext())
            {
                ApplicationUser user = context.Users.Where(u => u.Id == userId).SingleOrDefault();

                if (user != null)
                {
                    stringUserRoleId = user.Roles.FirstOrDefault().RoleId;
                    userRoleId = Guid.Parse(stringUserRoleId);
                    userRoleName = context.Roles.Where(r => r.Id == stringUserRoleId).Select(e => e.Name).FirstOrDefault();
                    allRoles = context.Roles.ToList();

                    //foreach (var role in allRoles)
                    //{
                    //    if (userRoleId.CompareTo(Guid.Parse(role.Id)) == 1)
                    //    {
                    //        roles.Add(new RolesDto { Id = role.Id, RoleName = role.Name });
                    //    }
                    //}
                }

                //return roles;
            }


            using (PIContext context = new PIContext())
            {
                RoleHierarchy roleHierarchy = context.RoleHierarchies.Where(rh => rh.ParentName == userRoleName).FirstOrDefault();
                if (roleHierarchy != null)
                {
                    //short orderId = roleHierarchy.Order;
                    List<string> roleList = context.RoleHierarchies.Where(rh => roleHierarchy.Order <= rh.Order).OrderBy(x => x.Order).Select(e => e.Name).ToList();

                    roleList.ForEach(rl => roles.Add(new RolesDto { Id = allRoles.Where(e => e.Name == rl).FirstOrDefault().Id, RoleName = rl }));
                }
            }

            return roles;
        }


        /// <summary>
        /// Get User By Id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public UserDto GetUserById(string userId, string loggedInUser)
        {
            IList<DivisionDto> divisionList = new List<DivisionDto>();
            IList<RolesDto> roleList = new List<RolesDto>();
            ApplicationUser user = new ApplicationUser();
            string assignedRole = null;

            using (var context = new PIContext())
            {
                // Get all divisions, if user role is business owner.
                string roleId = context.Users.Where(u => u.Id == loggedInUser).FirstOrDefault().Roles.FirstOrDefault().RoleId;
                string roleName = context.Roles.Where(r => r.Id == roleId).Select(r => r.Name).FirstOrDefault();
                if (roleName == "BusinessOwner")
                    divisionList = GetAllActiveDivisionsForCompany(loggedInUser);
                else
                {
                    // Get divisions from UsersInDivisions
                    divisionList = GetAllActiveDivisionsOfUser(loggedInUser);
                }

                roleList = GetAllActiveChildRoles(loggedInUser);

                if (string.IsNullOrEmpty(userId))
                {
                    return new UserDto
                    {
                        Divisions = divisionList,
                        Roles = roleList,
                    };
                }

                using (var userContext = new PIContext())
                {
                    user = userContext.Users.SingleOrDefault(c => c.Id == userId);

                    // find and mark assigned divisisons for the specific in user
                    foreach (DivisionDto div in divisionList)
                    {
                        div.IsAssigned = context.UsersInDivisions.Where(ud => ud.DivisionId == div.Id
                                                                              && ud.UserId == userId
                                                                              && ud.IsDelete == false
                                                                              && ud.IsActive).ToList().Count() > 0;
                    }


                    // find and mark assigned role for the specific user
                    foreach (var role in roleList)
                    {
                        if (user.Roles.Where(r => r.RoleId == role.Id).ToList().Count() > 0)
                        {
                            role.IsSelected = true;
                            assignedRole = role.RoleName;
                        }

                    }



                    //string assignedRoleId = user.Roles.FirstOrDefault().RoleId;

                    //using (PIContext appContext = new PIContext())
                    //{
                    //    assignedRole = appContext.Roles.Where(r => r.Id == assignedRoleId).Select(e => e.Name).FirstOrDefault().ToString();
                    //}

                }

                if (user != null)
                {
                    return new UserDto
                    {
                        Id = user.Id,
                        Salutation = user.Salutation,
                        FirstName = user.FirstName,
                        MiddleName = user.MiddleName,
                        LastName = user.LastName,
                        Email = user.Email,
                        IsActive = user.IsActive,
                        Divisions = divisionList,
                        Roles = roleList,
                        AssignedRoleName = assignedRole
                    };

                }
            }

            return null;

        }


        /// <summary>
        /// Add/update specific user
        /// </summary>
        /// <param name="costCenter"></param>
        /// <returns></returns>
        public UserResultDto SaveUser(UserDto userDto)
        {
            long tenantId = commonLogics.GetTenantIdByUserId(userDto.LoggedInUserId);

            UserResultDto result = new UserResultDto();

            using (var userContext = new PIContext())
            {
                var isSameEmail = userContext.Users.Where(u => u.Id != userDto.Id
                                                               && u.TenantId == tenantId
                                                               && (u.Email == userDto.Email)).SingleOrDefault();

                if (isSameEmail != null)
                {
                    result.IsSucess = false;
                    result.ErrorMessage = "Exsiting Email";
                    return result;
                }

                ApplicationUser appUser = new ApplicationUser();
                if (string.IsNullOrEmpty(userDto.Id))
                {
                    result.IsAddUser = true;

                    appUser.TenantId = tenantId;
                    appUser.UserName = userDto.Email;
                    appUser.Salutation = userDto.Salutation;
                    appUser.FirstName = userDto.FirstName;
                    appUser.MiddleName = userDto.MiddleName;
                    appUser.LastName = userDto.LastName;
                    appUser.Email = userDto.Email;
                    appUser.IsActive = userDto.IsActive;
                    appUser.JoinDate = DateTime.Now;
                    appUser.Level = 1;
                    appUser.IsDeleted = false;
                    appUser.LastLoginTime = (DateTime?)null;

                    userContext.Users.Add(appUser);
                    // Save user context.
                    userContext.SaveChanges();

                    // Add customer record for the newly added user.
                    CustomerManagement customerMgr = new CustomerManagement();
                    string roleId = userContext.Roles.Where(r => r.Name == "BusinessOwner").Select(r => r.Id).FirstOrDefault();

                    var businessOwnerRecord = userContext.Users.Where(x => x.TenantId == tenantId
                                                                         && x.Roles.Any(r => r.RoleId == roleId)).SingleOrDefault();


                    customerMgr.SaveCustomer(new CustomerDto
                    {
                        Salutation = userDto.Salutation,
                        FirstName = userDto.FirstName,
                        MiddleName = userDto.MiddleName,
                        LastName = userDto.LastName,
                        Email = userDto.Email,
                        UserName = userDto.Email,
                        Password = userDto.Password,
                        IsCorpAddressUseAsBusinessAddress = true,
                        UserId = appUser.Id,
                        AddressId = 1//businessOwnerRecord.Id
                    });
                }
                else
                {
                    result.IsAddUser = false;
                    //ApplicationUser existingUser = new ApplicationUser();
                    appUser = userContext.Users.SingleOrDefault(u => u.Id == userDto.Id);

                    appUser.Salutation = userDto.Salutation;
                    appUser.FirstName = userDto.FirstName;
                    appUser.MiddleName = userDto.MiddleName;
                    appUser.LastName = userDto.LastName;
                    appUser.Email = userDto.Email;
                    appUser.IsActive = userDto.IsActive;
                    //existingUser.JoinDate = DateTime.Now;
                    //existingUser.CreatedBy = 1; //TODO: sessionHelper.Get<User>().LoginName; 

                    // Save user context.
                    userContext.SaveChanges();
                }


                using (PIContext context = new PIContext())
                {
                    // -- Get whole userdivision list by userid to prevent multiple service call.
                    IList<UserInDivision> udList = context.UsersInDivisions.Where(ud => ud.UserId == appUser.Id && ud.IsActive && !ud.IsDelete).ToList();

                    // Removed division list which unselect
                    udList.Where(div => !userDto.AssignedDivisionIdList.Contains(div.DivisionId)).ToList().ForEach
                                                                    (dc => { dc.IsActive = false; dc.IsDelete = true; }); ;

                    context.SaveChanges();

                    // Get new list need to assign.
                    var newDivisiosnList = userDto.AssignedDivisionIdList.Where(ad => !udList.Select(ud => ud.DivisionId).ToList().Contains(ad)).ToList();

                    IList<UserInDivision> userDivisionList = new List<UserInDivision>();

                    foreach (var divisionId in newDivisiosnList)
                    {
                        userDivisionList.Add(new UserInDivision()
                        {
                            UserId = appUser.Id,
                            DivisionId = divisionId,
                            IsActive = true,
                            CreatedBy = "1",
                            CreatedDate = DateTime.Now
                        });
                    }

                    context.UsersInDivisions.AddRange(userDivisionList);
                    context.SaveChanges();


                    //Add Audit Trail Record
                    context.AuditTrail.Add(new AuditTrail
                    {
                        ReferenceId = appUser.Id,
                        AppFunctionality = string.IsNullOrEmpty(userDto.Id)? AppFunctionality.AddUser : AppFunctionality.EditUser,
                        Result = "SUCCESS",
                        CreatedBy = "1",
                        CreatedDate = DateTime.Now
                    });
                    context.SaveChanges();
                }

                result.IsSucess = true;
                result.UserId = appUser.Id;
                return result;
            }
        }


        public UserDto LoadUserManagement(string loggedInUser)
        {
            IList<DivisionDto> assignedDivisionList;
            IList<RolesDto> roleList;

            using (var context = new PIContext())
            {
                assignedDivisionList = GetAllActiveDivisionsForCompany(loggedInUser);
                roleList = GetAllActiveChildRoles(loggedInUser);
            }

            return new UserDto
            {
                Divisions = assignedDivisionList,
                Roles = roleList,
            };
        }


        /// <summary>
        /// Get all users
        /// </summary>
        /// <param name="division"></param>
        /// <param name="role"></param>
        /// <param name="userId"></param>
        /// <param name="status"></param>
        /// <param name="searchtext"></param>
        /// <returns></returns>
        public PagedList GetAllUsers(long division, string role, string userId, string status, string searchtext)
        {
            var pagedRecord = new PagedList();
            long tenantId = commonLogics.GetTenantIdByUserId(userId);

            pagedRecord.Content = new List<UserDto>();

            using (var context = new PIContext())
            {
                var content = context.Users.Where(x => x.TenantId == tenantId &&
                                                    x.IsDeleted == false &&
                                                    (string.IsNullOrEmpty(searchtext) || x.FirstName.Contains(searchtext) || x.LastName.Contains(searchtext)) &&
                                                    (status == "0" || x.IsActive.ToString() == status) &&
                                                    (role == "0" || x.Roles.Any(r => r.RoleId == role)) &&
                                                    (division == 0 || x.UserInDivisions.Any(r => r.DivisionId == division))
                                                    ).ToList();

                string assignedDivForGrid = string.Empty;
                int lastIndexOfBrTag;

                foreach (var item in content)
                {
                    StringBuilder divisionsString = new StringBuilder();
                    item.UserInDivisions.Where(x => x.IsDelete == false).ToList().ForEach(e => divisionsString.Append(e.Divisions.Name + "<br/>"));


                    // Remove last <br/> tag.
                    assignedDivForGrid = divisionsString.ToString();
                    lastIndexOfBrTag = assignedDivForGrid.LastIndexOf("<br/>");

                    if (lastIndexOfBrTag != -1) { assignedDivForGrid = assignedDivForGrid.Remove(lastIndexOfBrTag); }


                    pagedRecord.Content.Add(new UserDto
                    {
                        Id = item.Id,
                        FirstName = item.FirstName,
                        LastName = item.LastName,
                        RoleName = GetRoleName(item.Roles.FirstOrDefault().RoleId),
                        Status = (item.IsActive) ? "Active" : "Inactive",
                        AssignedDivisionsForGrid = assignedDivForGrid,
                        LastLoginTime = (item.LastLoginTime == null) ? null : item.LastLoginTime.Value.ToString("MM/dd/yyyy   HH:mm:ss tt", CultureInfo.InvariantCulture)
                    });
                }

            }

            return pagedRecord;

        }


        /// <summary>
        /// Get role name by Id
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public string GetRoleName(string roleId)
        {
            using (var userContext = new PIContext())
            {
                string userRoleName = userContext.Roles.Where(r => r.Id == roleId).Select(e => e.Name).FirstOrDefault();

                return userRoleName;
            }
        }



        public bool GetAccountType(string userId)
        {
            ApplicationUser currentuser = null;
            using (PIContext context = new PIContext())
            {
                currentuser = context.Users.SingleOrDefault(u => u.Id == userId);

                if (currentuser == null)
                {
                    return false;
                }

            }

            using (PIContext context = new PIContext())
            {
                Tenant tenant = context.Tenants.SingleOrDefault(t => t.Id == currentuser.TenantId);
                return tenant.IsCorporateAccount;
            }
        }


        /// <summary>
        /// Get all divisions by given filter criteria
        /// </summary>
        /// <param name="searchtext"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="sortBy"></param>
        /// <param name="sortDirection"></param>
        /// <returns></returns>
        public PagedList GetAllComapnies(string status, string searchtext)
        {
            var pagedRecord = new PagedList();

            pagedRecord.Content = new List<CustomerListDto>();

            using (var context = new PIContext())
            {
                string BusinessOwnerId = context.Roles.Where(r => r.Name == "BusinessOwner").Select(r => r.Id).FirstOrDefault();

                var content = (from customer in context.Customers
                               join comapny in context.Companies on customer.User.TenantId equals comapny.TenantId
                               where customer.User.Roles.Any(r => r.RoleId == BusinessOwnerId) &&
                               customer.IsDelete == false &&
                               (string.IsNullOrEmpty(status) || comapny.IsActive.ToString() == status) &&
                               (string.IsNullOrEmpty(searchtext) || customer.FirstName.Contains(searchtext) || customer.LastName.Contains(searchtext)
                                 || comapny.Name.Contains(searchtext))
                               select new
                               {
                                   Customer = customer,
                                   Company = comapny
                               }).ToList();



                foreach (var item in content)
                {

                    int userCount = context.UsersInDivisions.Where(x => x.Divisions.CompanyId == item.Company.Id)
                                    .Select(x => x.UserId).ToList().Count();

                    int shipmentCount = context.Shipments.Where(x => x.Division.CompanyId == item.Company.Id && x.IsActive)
                                    .Select(x => x.Id).ToList().Count();

                    pagedRecord.Content.Add(new CustomerListDto
                    {
                        Id = item.Company.Id,
                        FirstName = item.Customer.FirstName,
                        LastName = item.Customer.LastName,
                        CorporateName = item.Company.Name,
                        City = item.Customer.CustomerAddress.City,
                        Status = item.Company.IsActive,
                        CreatedDate = item.Customer.CreatedDate.ToString("dd/MM/yyyy"),
                        ActiveShipments = shipmentCount,
                        IsInvoiceEnabled = item.Company.IsInvoiceEnabled,
                        AssignedUserCount = ++userCount
                    });
                }

                return pagedRecord;
            }
        }

        /// <summary>
        /// Get all divisions by given filter criteria
        /// </summary>
        /// <param name="searchtext"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="sortBy"></param>
        /// <param name="sortDirection"></param>
        /// <returns></returns>
        public PagedList GetAllComapniesForAdminSearch(string searchtext)
        {
            var pagedRecord = new PagedList();

            pagedRecord.Content = new List<CustomerListDto>();

            using (var context = new PIContext())
            {
                string BusinessOwnerId = context.Roles.Where(r => r.Name == "BusinessOwner").Select(r => r.Id).FirstOrDefault();

                var content = (from customer in context.Customers
                               join comapny in context.Companies on customer.User.TenantId equals comapny.TenantId
                               where customer.User.Roles.Any(r => r.RoleId == BusinessOwnerId) &&
                               customer.IsDelete == false &&                               
                               (string.IsNullOrEmpty(searchtext) || customer.FirstName.Contains(searchtext) || customer.LastName.Contains(searchtext)
                                 || comapny.Name.Contains(searchtext))
                               select new
                               {
                                   Customer = customer,
                                   Company = comapny
                               }).ToList();

                foreach (var item in content)
                {

                    int userCount = context.UsersInDivisions.Where(x => x.Divisions.CompanyId == item.Company.Id)
                                    .Select(x => x.UserId).ToList().Count();

                    int shipmentCount = context.Shipments.Where(x => x.Division.CompanyId == item.Company.Id && x.IsActive)
                                    .Select(x => x.Id).ToList().Count();

                    pagedRecord.Content.Add(new CustomerListDto
                    {
                        Id = item.Company.Id,
                        FirstName = item.Customer.FirstName,
                        LastName = item.Customer.LastName,
                        CorporateName = item.Company.Name,
                        City = item.Customer.CustomerAddress.City,
                        Status = item.Company.IsActive,
                        CreatedDate = item.Customer.CreatedDate.ToString("dd/MM/yyyy"),
                        ActiveShipments = shipmentCount,
                        AssignedUserCount = ++userCount
                    });
                }

                return pagedRecord;
            }
        }


        public bool ChangeCompanyStatus(long comapnyId)
        {
            using (var context = new PIContext())
            {
                var comapny = context.Companies.Where(x => x.Id == comapnyId).SingleOrDefault();
                bool isActivate = !comapny.IsActive;
             
                // Inactivate/activate company
                if (comapny != null)
                {
                    comapny.IsActive = isActivate;
                    context.SaveChanges();
                }

                //ToDo: Inactivate/activate Users, divisions, cost centers
                var divisions = context.Divisions.Where(x => x.CompanyId == comapnyId).ToList();
                divisions.ForEach(x => x.IsActive = isActivate);

                var costCenters = context.CostCenters.Where(x => x.CompanyId == comapnyId).ToList();
                costCenters.ForEach(x => x.IsActive = isActivate);

                var userList = (from user in context.Users
                                join comapnyNew in context.Companies on user.TenantId equals comapnyNew.TenantId
                                where comapnyNew.Id == comapnyId
                                select user).ToList();


                userList.ForEach(x => x.IsActive = isActivate);
                context.SaveChanges();


                //Add Audit Trail Record
                context.AuditTrail.Add(new AuditTrail
                {
                    ReferenceId = comapny.Id.ToString(),
                    AppFunctionality = AppFunctionality.UserManagement,
                    Result = "SUCCESS",
                    CreatedBy = "1",
                    CreatedDate = DateTime.Now
                });
                context.SaveChanges();

                return comapny.IsActive;
            }
        }

        //get the company name by user Id
        public CompanyDto GetCompanyByUserID(string userID)
        {
            var currentCompany = commonLogics.GetCompanyByUserId(userID);
            return new CompanyDto()
            {
                Id= currentCompany.Id,
                CompanyCode=currentCompany.CompanyCode,
                Name=currentCompany.Name,
                COCNumber=currentCompany.COCNumber,
                VATNumber=currentCompany.VATNumber,
                LogoUrl= currentCompany.LogoUrl
            };

        }

        public string GetBusinessOwneridbyCompanyId(string companyId)
        {
            string userId = string.Empty;
            using (PIContext context= new PIContext())
            {
                var tenantId = context.Companies.Where(x => x.Id.ToString() == companyId).SingleOrDefault().TenantId;
                string BusinessOwnerId= context.Roles.Where(r => r.Name == "BusinessOwner").Select(r => r.Id).FirstOrDefault();

                userId = (from user in context.Users
                            where user.TenantId == tenantId
                            && user.Roles.FirstOrDefault().RoleId == BusinessOwnerId
                          select user.Id).SingleOrDefault();                
            }
            
            return userId;
        }


       public bool UpdateCompanyLogo(string URL,string userId)
        {
            using (PIContext context= new PIContext())
            {                
                var currentuser = context.Users.SingleOrDefault(u => u.Id == userId);
                var currentCompany= context.Companies.SingleOrDefault(n => n.TenantId == currentuser.TenantId);
                currentCompany.LogoUrl = URL;               
                context.SaveChanges();
            }

            return true;

        }


        #endregion


    }
}
