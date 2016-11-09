using AutoMapper;
using Microsoft.AspNet.Identity.EntityFramework;
using PI.Contract;
using PI.Contract.Business;
using PI.Contract.DTOs.Admin;
using PI.Contract.DTOs.Common;
using PI.Contract.DTOs.Company;
using PI.Contract.DTOs.CostCenter;
using PI.Contract.DTOs.Customer;
using PI.Contract.DTOs.Division;
using PI.Contract.DTOs.Node;
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
        private PIContext context;
        private ILogger logger;
        private ICustomerManagement customerManagement;

        public CompanyManagement(ILogger logger, ICustomerManagement customerManagement, PIContext _context = null)
        {
            context = _context ?? new PIContext();
            this.logger = logger;
            this.customerManagement = customerManagement;

        }

        #region Create/Get Tenant/Comapny Details with default settings

        public void DeleteCompanyDetails(long tenantId, string userId)
        {
            // When initialy create a user, records will enter to below entities.

            Tenant tenant = context.Tenants.Where(t => t.Id == tenantId).FirstOrDefault();
            Company company = context.Companies.Where(c => c.TenantId == tenantId).FirstOrDefault();
            CostCenter costCenter = context.CostCenters.Where(cos => cos.CompanyId == company.Id).FirstOrDefault();
            Address costCenterAddress = costCenter != null ? costCenter.BillingAddress : null;
            Division division = context.Divisions.Where(div => div.CompanyId == company.Id).FirstOrDefault();


            if (costCenter != null)
                context.CostCenters.Remove(costCenter);
            if (division != null)
                context.Divisions.Remove(division);
            if (company != null)
                context.Companies.Remove(company);

            if (costCenterAddress != null)
                context.Addresses.Remove(costCenterAddress);

            if (tenant != null)
                context.Tenants.Remove(tenant);

            context.SaveChanges();
        }

        public long CreateCompanyDetails(CustomerDto customerCompany)
        {
            //Add Tenant
            Tenant tenant = new Tenant
            {
                // TenancyName = customerCompany.CompanyCode,
                CreatedBy = "1",
                CreatedDate = DateTime.UtcNow,
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
                CreatedDate = DateTime.UtcNow,
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
                CreatedDate = DateTime.UtcNow,
                BillingAddress = new Address
                {
                    Country = customerCompany.CustomerAddress.Country,
                    ZipCode = customerCompany.CustomerAddress.ZipCode,
                    Number = customerCompany.CustomerAddress.Number,
                    StreetAddress1 = customerCompany.CustomerAddress.StreetAddress1,
                    StreetAddress2 = customerCompany.CustomerAddress.StreetAddress2,
                    City = customerCompany.CustomerAddress.City,
                    State = customerCompany.CustomerAddress.State,
                    CreatedDate = DateTime.UtcNow,
                    CreatedBy = "1",// TODO : Get created user.
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
                CreatedDate = DateTime.UtcNow,
            };
            context.Divisions.Add(division);
            context.SaveChanges();

            return tenant.Id;
        }

        public long GetTenantIdByUserId(string userid)
        {
            return context.GetTenantIdByUserId(userid);
        }

        #endregion


        #region Cost Center Managment

        /// <summary>
        /// Get all costCenters for the tenant's coampny
        /// </summary>
        /// <returns></returns>        
        public IList<CostCenterDto> GetAllCostCentersForCompany(string userId)
        {
            IList<CostCenterDto> costCenterList = new List<CostCenterDto>();
            Company currentcompany = context.GetCompanyByUserId(userId);
            if (currentcompany == null)
            {
                return null;
            }

            var costcenters = context.CostCenters.Where(c => c.CompanyId == currentcompany.Id &&
                                                             c.Type == "USER"
                                                             && c.IsDelete == false).ToList();

            costcenters.ForEach(cost => costCenterList.Add(new CostCenterDto
            {
                Id = cost.Id,
                Name = cost.Name
            }));

            return costCenterList;
        }


        /// <summary>
        /// Get costcenters by division Id
        /// </summary>
        /// <param name="divisionId"></param>
        /// <returns></returns>
        public IList<CostCenterDto> GetCostCentersbyDivision(string divisionId)
        {
            IList<CostCenterDto> costCenterList = new List<CostCenterDto>();

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
            return costCenterList;
        }


        /// <summary>
        /// Get default costcenter by division Id
        /// </summary>
        /// <param name="divisionId"></param>
        /// <returns></returns>
        public long GetDefaultCostCentersbyDivision(string divisionId)
        {
            long costCenterId = 0;

            costCenterId = (from division in context.Divisions
                            where division.Id.ToString() == divisionId
                            select division.DefaultCostCenterId).FirstOrDefault();

            return costCenterId;
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
        public PagedList GetAllCostCenters(long divisionId, string type, string userId, string searchtext,
                                           int page = 1, int pageSize = 10,
                                           string sortBy = "Id", string sortDirection = "asc")
        {
            var pagedRecord = new PagedList();
            string assignedDivForGrid = string.Empty;
            int lastIndexOfBrTag;

            //CommonLogic.GetCompanyByUserId(userId);
            Company currentcompany = context.GetCompanyByUserId(userId);    //CommonLogic.GetCompanyByUserId(userId);

            if (currentcompany == null)
            {
                return pagedRecord;
            }

            pagedRecord.Content = new List<CostCenterDto>();
            bool isBusinessOwner = IsLoggedInAsBusinessOwner(userId);

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



        /// <summary>
        /// Get a particular cost center by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public CostCenterDto GetCostCentersById(long id, string userId)
        {
            IList<DivisionDto> divisionList = new List<DivisionDto>();

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
                    Status = costCenter.IsActive ? 1 : 2,
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

            return null;
        }


        /// <summary>
        /// Add/Update costCenter
        /// </summary>
        /// <param name="costCenter"></param>
        /// <returns></returns>
        public int SaveCostCenter(CostCenterDto costCenter)
        {
            long comapnyId = context.GetCompanyByUserId(costCenter.UserId).Id;

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
                    CreatedDate = DateTime.UtcNow
                });
            }

            if (costCenter.Id == 0)
            {
                Company comp = context.GetCompanyByUserId(costCenter.UserId);

                CostCenter newCostCenter = new CostCenter()
                {
                    Name = costCenter.Name,
                    Description = costCenter.Description,
                    PhoneNumber = costCenter.PhoneNumber,
                    Status = costCenter.Status,
                    CompanyId = comp == null ? 0 : comp.Id, //costCenter.CompanyId, TODO H - why?
                    Type = "USER",
                    CreatedDate = DateTime.UtcNow,
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
                        CreatedDate = DateTime.UtcNow,
                        CreatedBy = "1",//sessionHelper.Get<User>().LoginName; // TODO : Get created user.
                    },
                    IsActive = costCenter.Status == 1 ? true : false,
                };
                context.CostCenters.Add(newCostCenter);
                context.SaveChanges();

                divcostList.ToList().ForEach(x => x.CostCenterId = newCostCenter.Id);
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
                //        CreatedDate = DateTime.UtcNow
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
                existingCostCenter.CreatedDate = DateTime.UtcNow;
                existingCostCenter.CreatedBy = "1"; //sessionHelper.Get<User>().LoginName; 
                                                    //existingCostCenter.DivisionCostCenters.ToList().AddRange(divcostList);
                                                    // TODO: Add Assigned Devisions
                                                    // TODO: Need to handle prevent remove default cost center in div.
                context.DivisionCostCenters.AddRange(divcostList);

                context.SaveChanges();

            }

            //}

            return 1;
        }


        public int DeleteCostCenter(long id)
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
            Company currentcompany = context.GetCompanyByUserId(userId);

            if (currentcompany == null)
            {
                return null;
            }

            var divisions = context.Divisions.Where(c => c.CompanyId == currentcompany.Id &&
                                                         c.Type == "USER" && c.IsActive == true).ToList();

            divisions.ForEach(x => divisionList.Add(new DivisionDto
            {
                Id = x.Id,
                Name = x.Name
            }));

            return divisionList;
        }


        /// <summary>
        /// Get all active divisions Of user
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public IList<DivisionDto> GetAllActiveDivisionsOfUser(string userId)
        {
            IList<DivisionDto> divisionList = new List<DivisionDto>();
            Company currentcompany = context.GetCompanyByUserId(userId);

            if (currentcompany == null)
            {
                return null;
            }

            var divisions = context.UsersInDivisions.Where(ud => ud.UserId == userId
                                                                 && !ud.IsDelete
                                                                 && ud.IsActive).ToList();

            divisions.ForEach(x => divisionList.Add(new DivisionDto
            {
                Id = x.DivisionId,
                Name = x.Divisions.Name
            }));

            return divisionList;
        }


        /// <summary>
        /// GetAllDivisionsForCompany
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<DivisionDto> GetAllDivisionsForCompany(string userId)
        {
            List<DivisionDto> divisionList = new List<DivisionDto>();
            Company currentcompany = context.GetCompanyByUserId(userId);

            if (currentcompany == null)
            {
                return null;
            }

            var divisions = context.Divisions.Where(c => c.CompanyId == currentcompany.Id &&
                                                         c.Type == "USER" && c.IsDelete == false).ToList();

            divisions.ForEach(x => divisionList.Add(new DivisionDto
            {
                Id = x.Id,
                Name = x.Name
            }));

            return divisionList;
        }


        /// <summary>
        /// Get the assigned division for user
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public IList<DivisionDto> GetAssignedDivisions(string userid)
        {
            IList<DivisionDto> divisionList = new List<DivisionDto>();

            var divisions = context.UsersInDivisions.Where(divUser => divUser.UserId == userid &&
                                                           divUser.Divisions.Type == "USER" &&
                                                           divUser.Divisions.IsDelete == false
                                                           && divUser.Divisions.IsDelete != true
                                                           && divUser.Divisions.IsActive).Select(x => x.Divisions).ToList();

            divisions.ForEach(x => divisionList.Add(new DivisionDto
            {
                Id = x.Id,
                Name = x.Name
            }));

            return divisionList;
        }


        public NodeDto GetOrganizationStructure(string userId)
        {
            NodeDto node = new NodeDto();
            NodeDto managerNode = new NodeDto();
            NodeDto supervisorNode = new NodeDto();
            List<NodeDto> divisionsWithOperatorList = new List<NodeDto>();

            string supervisorRoleId = context.Roles.Where(r => r.Name == "Supervisor").Select(r => r.Id).FirstOrDefault();
            string operatorRoleId = context.Roles.Where(r => r.Name == "Operator").Select(r => r.Id).FirstOrDefault();

            Company currentcompany = context.GetCompanyByUserId(userId);
            var comapnyUserList = context.Users.Where(u => u.TenantId == currentcompany.TenantId && !u.IsDeleted).ToList();

            // Assigned BO
            var businessOwner = comapnyUserList.Where(c => context.GetUserRoleById(c.Id) == "BusinessOwner").SingleOrDefault();

            node.Id = businessOwner.Id;
            node.Type = "businessowner";
            node.Name = "BusinessOwner";
            node.Title = businessOwner.FirstName + " " + businessOwner.LastName;

            // Assigned Managers
            var managerList = comapnyUserList.Where(c => context.GetUserRoleById(c.Id) == "Manager").ToList();

            foreach (var manager in managerList)
            {
                if (node.Children.Count == 0)
                {
                    node.Children.Add(new NodeDto
                    {
                        Id = manager.Id,
                        Type = "manager",
                        Name = "Manager",
                        Title = manager.FirstName + " " + manager.LastName,
                        IsActive = manager.IsActive
                    });
                }
                else
                {
                    // Can have only one children as manager to Business Owner acording to the data structure.
                    node.Children[0].Manager.Add(new NodeDto
                    {
                        Id = manager.Id,
                        Type = "manager",
                        Name = "Manager - " + (manager.IsActive ? "Active" : "Inactive"),  //commonLogics.GetUserRoleById(user.Id);
                        Title = manager.FirstName + " " + manager.LastName
                    });
                }
            }


            // find assigned division to supervisor and non assigned division.
            var supervisorDivisions = context.UsersInDivisions.Where(d => d.Divisions.CompanyId == currentcompany.Id &&
                                                                     d.User.Roles.Any(r => r.RoleId == supervisorRoleId)).ToList();

            var allDivisions = context.Divisions.Where(d => d.CompanyId == currentcompany.Id && d.Type == "USER").ToList();

            var supervisorDivisionsToExclude = context.UsersInDivisions.Where(d => d.Divisions.CompanyId == currentcompany.Id &&
                                                                   d.User.Roles.Any(r => r.RoleId == supervisorRoleId))
                                                                   .Select(z => z.Divisions).ToList();

            // unassigned + operator assigned division
            var unassignedDivisions = allDivisions.Except(supervisorDivisionsToExclude).ToList();


            var unassignedUsers = context.Users.Where(u => u.TenantId == currentcompany.TenantId
                                                           && u.UserInDivisions.Count() == 0).ToList();

            var unassignedSupervisors = unassignedUsers.Where(u => u.Roles.Any(r => r.RoleId == supervisorRoleId)).ToList();

            var unassignedOperators = unassignedUsers.Where(u => u.Roles.Any(r => r.RoleId == operatorRoleId)).ToList();

            if (unassignedSupervisors.Count() > 0)
            {
                List<NodeDto> operatorList = new List<NodeDto>();
                unassignedSupervisors.ForEach(x =>
                                                 operatorList.Add(new NodeDto
                                                 {
                                                     Id = x.Id,
                                                     Type = "supervisor",
                                                     Name = "Supervisor - " + (x.IsActive ? "Active" : "Inactive"),
                                                     Title = x.FirstName + " " + x.LastName
                                                 }));
                if (node.Children.Count() > 0)
                {
                    node.Children[0].Children.AddRange(operatorList);
                }
                else
                {
                    node.Children.AddRange(operatorList); //If there is no manager attach directly to BO.
                }
            }

            if (unassignedOperators.Count() > 0)
            {
                List<NodeDto> operatorList = new List<NodeDto>();
                unassignedOperators.ForEach(x =>
                                                 operatorList.Add(new NodeDto
                                                 {
                                                     Id = x.Id,
                                                     Type = "operator",
                                                     Name = "Operator - " + (x.IsActive ? "Active" : "Inactive"),
                                                     Title = x.FirstName + " " + x.LastName
                                                 }));
                if (node.Children.Count() > 0)
                {
                    node.Children[0].Children.AddRange(operatorList);
                }
                else
                {
                    node.Children.AddRange(operatorList); //If there is no manager attach directly to BO.
                }
            }

            // Assign supervisor
            //supervisorDivisions.Select(x => x.User).ToList()
            //    .ForEach(s => (node.Children.Count() > 0 ? node.Children[0].Children.Add(new NodeDto { Id = 1 }) :
            //node.Children.Add(new NodeDto { Id = s.Id })));

            NodeDto nodeSupervisor = null; /////////////////
            IList<NodeDto> nodeSupervisorList = new List<NodeDto>();
            NodeDto nodeDivison = null;
            IList<Division> nodeSupervisorDivisonsList = null;
            IList<NodeDto> nodeDivisonsList = new List<NodeDto>();
            List<ApplicationUser> nodeDivisionOpreatorList = null;

            foreach (var supervisorDivision in supervisorDivisions.Select(x => x.Divisions).Distinct())
            {
                nodeSupervisor = null;
                // get super
                var supervisors = supervisorDivision.UserInDivisions.Where(u => context.GetUserRoleById(u.UserId) == "Supervisor").Select(u => u.User).ToList();

                foreach (var supervisor in supervisors)
                {
                    if (nodeSupervisor == null)
                    {
                        nodeSupervisor = new NodeDto
                        {
                            Id = supervisor.Id,
                            Type = "supervisor",
                            Name = "Supervisor - " + (supervisor.IsActive ? "Active" : "Inactive"),  //commonLogics.GetUserRoleById(user.Id);
                            Title = supervisor.FirstName + " " + supervisor.LastName
                        };
                    }
                    else
                    {
                        nodeSupervisor.Supervisor.Add
                        (
                            new NodeDto
                            {
                                Id = supervisor.Id,
                                Type = "Supervisor",
                                Name = "Supervisor - " + (supervisor.IsActive ? "Active" : "Inactive"),  //commonLogics.GetUserRoleById(user.Id);
                                Title = supervisor.FirstName + " " + supervisor.LastName,
                            }
                        );
                    }
                }

                //division for supervisors box
                var supDivision = new NodeDto
                {
                    Id = supervisorDivision.Id.ToString(),
                    Type = "division",
                    Name = "Division - " + (supervisorDivision.IsActive ? "Active" : "Inactive"),
                    Title = supervisorDivision.Name,
                    Costcenter = GetCostCentersAsNodes(context, supervisorDivision.Id)
                };

                var operatorList = context.UsersInDivisions.Where(x => x.DivisionId == supervisorDivision.Id &&
                                                   x.User.Roles.Any(r => r.RoleId == operatorRoleId)).Select(x => x.User).ToList();

                //operators for parents's divisons
                operatorList.ForEach(o =>
                    supDivision.Children.Add(new NodeDto
                    {
                        Id = o.Id.ToString(),
                        Type = "operator",
                        Name = "Operator - " + (o.IsActive ? "Active" : "Inactive"),
                        Title = o.FirstName + " " + o.LastName
                    }));

                //attach divisions
                nodeSupervisor.Children.Add(supDivision);

                // attach supervisors
                if (node.Children.Count() > 0)
                {
                    node.Children[0].Children.Add(nodeSupervisor);
                }
                else
                {
                    node.Children.Add(nodeSupervisor); //If there is no manager attach directly to BO.
                }

            }

            // Add Unassigned divisions   
            var nodeDivisionList = new List<NodeDto>();
            List<NodeDto> divisionOperators = null;

            foreach (var division in unassignedDivisions.Distinct())
            {
                divisionOperators = new List<NodeDto>();

                var operatorList = context.UsersInDivisions.Where(x => x.DivisionId == division.Id &&
                               x.User.Roles.Any(r => r.RoleId == operatorRoleId)).Select(x => x.User).ToList();

                //operators for parents's divisons
                operatorList.ForEach(o =>
                    divisionOperators.Add(new NodeDto
                    {
                        Id = o.Id.ToString(),
                        Type = "operator",
                        Name = "Operator - " + (o.IsActive ? "Active" : "Inactive"),
                        Title = o.FirstName + " " + o.LastName,
                    }));

                nodeDivisionList.Add(new NodeDto()
                {
                    Id = division.Id.ToString(),
                    Type = "division",
                    Name = "Division - " + (division.IsActive ? "Active" : "Inactive"),  //commonLogics.GetUserRoleById(user.Id);
                    Title = division.Name,
                    Children = divisionOperators,
                    Costcenter = GetCostCentersAsNodes(context, division.Id)
                });

            }

            if (node.Children.Count() > 0)
            {
                node.Children[0].Children.AddRange(nodeDivisionList);
            }
            else
            {
                node.Children.AddRange(nodeDivisionList); //If there is no manager attach directly to BO.
            }

            return node;
        }

        private List<NodeDto> GetCostCentersAsNodes(PIContext context, long divisionId)
        {
            List<NodeDto> costcenterList = new List<NodeDto>();
            var Costcenters = context.DivisionCostCenters.Where(x => x.DivisionId == divisionId && !x.IsDelete).ToList();

            // Add costcenters for the considered division.
            Costcenters.ForEach(c => costcenterList.Add(new NodeDto
            {
                Id = c.CostCenterId.ToString(),
                Type = "costcenter",
                Name = "Costcenter",
                Title = c.CostCenters.Name,
                IsActive = c.CostCenters.IsActive
            }));

            return costcenterList;
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
            string assosiatedCostCentersForGrid = string.Empty;
            int lastIndexOfBrTag;

            Company currentcompany = context.GetCompanyByUserId(userId);
            bool isBusinessOwner = IsLoggedInAsBusinessOwner(userId);

            if (currentcompany == null)
            {
                return pagedRecord;
            }
            pagedRecord.Content = new List<DivisionDto>();

            var content = context.Divisions.Include("DivisionCostCenters")
                                    .Where(x => x.CompanyId == currentcompany.Id && x.Type == "USER" &&
                                                x.IsDelete == false &&
                                                (isBusinessOwner || !isBusinessOwner &&
                                                x.UserInDivisions.Any(d => d.IsActive && d.UserId == userId)) &&
                                                (string.IsNullOrEmpty(searchtext) || x.Name.Contains(searchtext)) &&
                                                (type == "0" || x.IsActive.ToString() == type) &&
                                                (costCenterId == 0 ||
                                                x.DivisionCostCenters.Any(cd => cd.CostCenterId == costCenterId && cd.IsDelete == false))
                                                )
                                        .OrderBy(sortBy + " " + sortDirection)
                                        .ToList();

            foreach (var item in content)
            {
                StringBuilder stringResult = new StringBuilder();
                context.DivisionCostCenters.Include("CostCenters").Where(c => c.DivisionId == item.Id && c.IsDelete == false).ToList().
                                                ForEach(e => stringResult.Append(e.CostCenters.Name + "<br/>"));

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
                    NumberOfUsers = item.UserInDivisions.Where(x => x.DivisionId == item.Id && x.IsActive).ToList().Count() + 1, //Add the business owner since he is not in the UserInDivisions table.
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


        /// <summary>
        /// Get a particular division by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public DivisionDto GetDivisionById(long id, string userId)
        {
            IList<CostCenterDto> costCenterList = new List<CostCenterDto>();
            long companyId = context.GetCompanyByUserId(userId).Id;

            if (id == 0)
            {
                return new DivisionDto
                {
                    Id = 0,
                    //AssosiatedCostCenters = costCenterList
                };
            }

            var division = context.Divisions.Include("DivisionCostCenters").SingleOrDefault(d => d.Id == id);

            context.DivisionCostCenters.Where(d => d.DivisionId == id && d.IsDelete == false).ToList()
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
                    Status = division.IsActive ? 1 : 2,
                    DefaultCostCenterId = division.DefaultCostCenterId,
                    CompanyId = division.CompanyId,
                    AssosiatedCostCenters = costCenterList
                };

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
            long comapnyId = context.GetCompanyByUserId(division.UserId).Id;

            var isSpaceOrEmpty = String.IsNullOrWhiteSpace(division.Description);
            var isSameDiviName = context.Divisions.Where(d => d.Id != division.Id
                                                            && d.CompanyId == comapnyId
                                                            && d.Type == "USER" &&
                                                            (d.Name == division.Name ||
                                                            (d.Description == division.Description && !isSpaceOrEmpty))).SingleOrDefault();

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
                    CreatedDate = DateTime.UtcNow,
                    CreatedBy = "1",// TODO : Get created user.                       
                };
                context.Divisions.Add(newDivision);
                context.SaveChanges();

                if (!string.IsNullOrEmpty(division.AssignedSupervisorId))
                {
                    context.UsersInDivisions.Add(new UserInDivision
                    {
                        UserId = division.AssignedSupervisorId,
                        DivisionId = newDivision.Id,
                        IsActive = true,
                        CreatedBy = "1",
                        CreatedDate = DateTime.UtcNow
                    });
                }

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
                existingDivision.CreatedDate = DateTime.UtcNow;
                existingDivision.CreatedBy = "1"; //sessionHelper.Get<User>().LoginName; 

            }
            context.SaveChanges();

            return 1;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int DeleteDivision(long id)
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


        #endregion


        #region User Management


        public bool IsLoggedInAsBusinessOwner(string userId)
        {
            string roleId = context.Users.Where(u => u.Id == userId).FirstOrDefault().Roles.FirstOrDefault().RoleId;
            string roleName = context.Roles.Where(r => r.Id == roleId).Select(r => r.Name).FirstOrDefault();

            return (roleName == "BusinessOwner") ? true : false;
        }


        public string GetLoggedInUserName(string userId)
        {
            string userName = null;

            ApplicationUser user = context.Users.Where(u => u.Id == userId).SingleOrDefault();

            if (user != null)
            {
                userName = user.FirstName + " " + user.LastName;
            }

            return userName;
        }


        /// <summary>
        ///  Update LastLoginTime on every successful login.
        /// </summary>
        /// <param name="userId"></param>
        public void UpdateLastLoginTimeAndAduitTrail(string userId)
        {

            ApplicationUser user = context.Users.Where(u => u.Id == userId).SingleOrDefault();

            if (user != null)
            {
                user.LastLoginTime = DateTime.UtcNow;
                context.SaveChanges();
            }

            //Add Audit Trail Record 
            context.AuditTrail.Add(new AuditTrail
            {
                ReferenceId = user.Id.ToString(),
                AppFunctionality = AppFunctionality.UserLogin,
                Result = "SUCCESS",
                CreatedBy = "1",
                CreatedDate = DateTime.UtcNow
            });

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

            ApplicationUser user = context.Users.Where(u => u.Id == userId).SingleOrDefault();

            if (user != null)
            {
                stringUserRoleId = user.Roles.FirstOrDefault().RoleId;
                userRoleId = Guid.Parse(stringUserRoleId);
                userRoleName = context.Roles.Where(r => r.Id == stringUserRoleId).Select(e => e.Name).FirstOrDefault();
                allRoles = context.Roles.ToList();
            }

            /* Removed  for now
            RoleHierarchy roleHierarchy = context.RoleHierarchies.Where(rh => rh.ParentName == userRoleName).FirstOrDefault();
            if (roleHierarchy != null)
            {
                //short orderId = roleHierarchy.Order;
                List<string> roleList = context.RoleHierarchies.Where(rh => roleHierarchy.Order <= rh.Order)
                                       .OrderBy(x => x.Order).Select(e => e.Name).ToList();

                roleList.ForEach(rl => roles.Add(new RolesDto { Id = allRoles.Where(e => e.Name == rl).FirstOrDefault().Id, RoleName = rl }));
            }
            */
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

            roleList = GetAllActiveChildRoles(loggedInUser);

            if (string.IsNullOrEmpty(userId))
            {
                return new UserDto
                {
                    Roles = roleList,
                };
            }

            user = context.Users.SingleOrDefault(c => c.Id == userId);

            // find and mark assigned role for the specific user
            foreach (var role in roleList)
            {
                if (user.Roles.Where(r => r.RoleId == role.Id).ToList().Count() > 0)
                {
                    role.IsSelected = true;
                    assignedRole = role.RoleName;
                }
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
                    AssignedRoleName = assignedRole,
                    IsActualUser = (user.Id == loggedInUser? true : false)
                };

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
            long tenantId = context.GetTenantIdByUserId(userDto.LoggedInUserId);

            UserResultDto result = new UserResultDto();

            var isSameEmail = context.Users.Where(u => u.Id != userDto.Id
                                                           && u.TenantId == tenantId
                                                           && (u.Email == userDto.Email)).SingleOrDefault();

            if (isSameEmail != null)
            {
                result.IsSucess = false;
                result.ErrorMessage = "Exsiting Email";
                return result;
            }

            ApplicationUser appUser = new ApplicationUser();
            if (string.IsNullOrEmpty(userDto.Id)  || userDto.Id == "0")
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
                //appUser.IsActive = userDto.Status == "Active" ? true : false;
                appUser.JoinDate = DateTime.UtcNow;
                appUser.Level = 1;
                appUser.IsDeleted = false;
                appUser.LastLoginTime = (DateTime?)null;

                context.Users.Add(appUser);
                // Save user context.
                context.SaveChanges();


                //var newUser = context.Users.Where(u => u.TenantId == tenantId
                //                                         && (u.Email == userDto.Email)).SingleOrDefault();
                // Add customer record for the newly added user.

                //string roleId = userContext.Roles.Where(r => r.Name == "BusinessOwner").Select(r => r.Id).FirstOrDefault();

                //var businessOwnerRecord = userContext.Users.Where(x => x.TenantId == tenantId
                //                                                     && x.Roles.Any(r => r.RoleId == roleId)).SingleOrDefault();
                

                customerManagement.SaveCustomer(new CustomerDto
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
                    AddressId = 1
                    //businessOwnerRecord.Id
                });
            }
            else
            {
                result.IsAddUser = false;
                //ApplicationUser existingUser = new ApplicationUser();
                appUser = context.Users.SingleOrDefault(u => u.Id == userDto.Id);

                appUser.Salutation = userDto.Salutation;
                appUser.FirstName = userDto.FirstName;
                appUser.MiddleName = userDto.MiddleName;
                appUser.LastName = userDto.LastName;
                appUser.Email = userDto.Email;
                appUser.IsActive = userDto.IsActive;
                //existingUser.JoinDate = DateTime.UtcNow;
                //existingUser.CreatedBy = 1; //TODO: sessionHelper.Get<User>().LoginName; 

                // Save user context.
                context.SaveChanges();
            }

            //Add Audit Trail Record
            context.AuditTrail.Add(new AuditTrail
            {
                ReferenceId = appUser.Id,
                AppFunctionality = string.IsNullOrEmpty(userDto.Id) ? AppFunctionality.AddUser : AppFunctionality.EditUser,
                Result = "SUCCESS",
                CreatedBy = "1",
                CreatedDate = DateTime.UtcNow
            });
            context.SaveChanges();

            result.IsSucess = true;
            result.UserId = appUser.Id;
            return result;
        }


        public UserDto LoadUserManagement(string loggedInUser)
        {
            IList<DivisionDto> assignedDivisionList;
            IList<RolesDto> roleList;

            assignedDivisionList = GetAllActiveDivisionsForCompany(loggedInUser);
            roleList = GetAllActiveChildRoles(loggedInUser);

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
        public PagedList GetAllUsers(string role, string userId, string status, string searchtext)
        {
            var pagedRecord = new PagedList();
            long tenantId = context.GetTenantIdByUserId(userId);

            pagedRecord.Content = new List<UserDto>();

            var content = context.Users.Where(x => 
                                                x.TenantId == tenantId &&  x.IsDeleted == false &&
                                                (string.IsNullOrEmpty(searchtext) || x.FirstName.Contains(searchtext) || x.LastName.Contains(searchtext)) &&
                                                (status == "0" || x.IsActive.ToString() == status) &&
                                                (role == "0" || x.Roles.Any(r => r.RoleId == role)) 
                                                ).ToList();

            foreach (var item in content)
            {
                //if (item.Roles.Count()!=0 && item.Roles.FirstOrDefault().RoleId!= "b1320df1-55f8-46a0-9754-13a0544658d4" && item.Roles.FirstOrDefault().RoleId != "e97b4e4c-45de-4fb2-a322-5b876b7661d0" && item.Roles.FirstOrDefault().RoleId != "336eeffa-57c6-430d-9fa9-575e2a7e9787")
                if (item.Roles.Count() != 0)
                {
                    pagedRecord.Content.Add(new UserDto
                    {
                        Id = item.Id,
                        Salutation = item.Salutation,
                        FirstName = item.FirstName,
                        LastName = item.LastName,
                        Email = item.Email,
                        RoleName = item.Roles.Count() != 0 ? GetRoleName(item.Roles.FirstOrDefault().RoleId) : "",
                        Status = (item.IsActive) ? "Active" : "Inactive",
                        IsActualUser = (item.Id == userId ? true: false),
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
            string userRoleName = context.Roles.Where(r => r.Id == roleId).Select(e => e.Name).FirstOrDefault();
            return userRoleName;
        }



        public bool GetAccountType(string userId)
        {
            ApplicationUser currentuser = null;

            currentuser = context.Users.SingleOrDefault(u => u.Id == userId);

            if (currentuser == null)
            {
                return false;
            }

            Tenant tenant = context.Tenants.SingleOrDefault(t => t.Id == currentuser.TenantId);
            return tenant.IsCorporateAccount;

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
        public PagedList GetAllComapnies(PagedList pageList)
        {
            var pagedRecord = new PagedList();

            pagedRecord.Content = new List<CustomerListDto>();

            string BusinessOwnerId = context.Roles.Where(r => r.Name == "BusinessOwner").Select(r => r.Id).FirstOrDefault();

            string status = pageList.DynamicContent.status.ToString();
            string searchtext = pageList.DynamicContent.searchText.ToString();

            var querableContent = (from customer in context.Customers
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
                           });

            var content = querableContent.OrderBy(d => d.Company.CreatedDate).Skip(pageList.CurrentPage).Take(pageList.PageSize).ToList();

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

            pagedRecord.TotalRecords = querableContent.Count();
            pagedRecord.PageSize = pageList.PageSize;
            pagedRecord.TotalPages = (int)Math.Ceiling((decimal)pagedRecord.TotalRecords / pagedRecord.PageSize);

            return pagedRecord;
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


        public bool ChangeCompanyStatus(long comapnyId)
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
                CreatedDate = DateTime.UtcNow
            });
            context.SaveChanges();

            return comapny.IsActive;
        }

        /// <summary>
        /// Get company by userId
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public CompanyDto GetCompanyByUserID(string userID)
        {
            var currentCompany = context.GetCompanyByUserId(userID);
            return new CompanyDto()
            {
                Id = currentCompany.Id,
                CompanyCode = currentCompany.CompanyCode,
                Name = currentCompany.Name,
                COCNumber = currentCompany.COCNumber,
                VATNumber = currentCompany.VATNumber,
                LogoUrl = currentCompany.LogoUrl
            };

        }


        public string GetBusinessOwneridbyCompanyId(string companyId)
        {
            string userId = string.Empty;

            var tenantId = context.Companies.Where(x => x.Id.ToString() == companyId).SingleOrDefault().TenantId;
            string BusinessOwnerId = context.Roles.Where(r => r.Name == "BusinessOwner").Select(r => r.Id).FirstOrDefault();

            userId = (from user in context.Users
                      where user.TenantId == tenantId
                      && user.Roles.FirstOrDefault().RoleId == BusinessOwnerId
                      select user.Id).SingleOrDefault();

            return userId;
        }


        /// <summary>
        /// Update company logo
        /// </summary>
        /// <param name="URL"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public bool UpdateCompanyLogo(string URL, string userId)
        {
            var currentuser = context.Users.SingleOrDefault(u => u.Id == userId);
            var currentCompany = context.Companies.SingleOrDefault(n => n.TenantId == currentuser.TenantId);
            currentCompany.LogoUrl = URL;
            context.SaveChanges();

            return true;
        }


        public void SaveUserPhoneCode(UserDto userDto)
        {
            var currentuser = context.Users.SingleOrDefault(u => u.Email == userDto.Email);
            currentuser.MobileVerificationCode = userDto.MobileVerificationCode;
            currentuser.MobileVerificationExpiry = DateTime.UtcNow;

            var customer = context.Customers.SingleOrDefault(u => u.Email == userDto.Email);
            
            if (customer.MobileNumber != userDto.MobileNumber)
            {                
                customer.MobileNumber = userDto.MobileNumber;
                currentuser.PhoneNumberConfirmed = false;
            }

            context.SaveChanges();
        }

        public void SaveUserPhoneConfirmation(UserDto userDto)
        {
            var currentuser = context.Users.SingleOrDefault(u => u.Email == userDto.Email);

            currentuser.PhoneNumberConfirmed = true;
            context.SaveChanges();
        }
        #endregion


    }
}
