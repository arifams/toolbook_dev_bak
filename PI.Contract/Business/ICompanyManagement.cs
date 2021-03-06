﻿using PI.Contract.DTOs;
using PI.Contract.DTOs.Common;
using PI.Contract.DTOs.Company;
using PI.Contract.DTOs.CostCenter;
using PI.Contract.DTOs.Customer;
using PI.Contract.DTOs.Division;
using PI.Contract.DTOs.Node;
using PI.Contract.DTOs.Role;
using PI.Contract.DTOs.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Contract.Business
{
    public interface ICompanyManagement
    {
        string GetRoleName(string roleId);
        bool GetAccountType(string userId);
        void UpdateLastLoginTimeAndAduitTrail(string userId);
        List<RolesDto> GetAllActiveChildRoles(string userId);
        PagedList GetAllUsers(string role, string userId, string status, string searchtext);
        UserResultDto SaveUser(UserDto userDto);
        UserDto GetUserById(string userId, string loggedInUser);
        UserDto LoadUserManagement(string loggedInUser);
        string GetLoggedInUserName(string userId);
        List<DivisionDto> GetAllDivisionsForCompany(string userId);
        PagedList GetAllDivisions(long costCenterId, string type, string userId, string searchtext, int page = 1, int pageSize = 10,
                                         string sortBy = "CustomerID", string sortDirection = "asc");
        long CreateCompanyDetails(CustomerDto customerCompany);
        DivisionDto GetDivisionById(long id, string userId);
        int SaveDivision(DivisionDto division);
        int DeleteDivision(long id);
        int SaveCostCenter(CostCenterDto costCenter);
        IList<CostCenterDto> GetAllCostCentersForCompany(string userId);
        IList<CostCenterDto> GetCostCentersbyDivision(string divisionId);
        long GetDefaultCostCentersbyDivision(string divisionId);
        IList<DivisionDto> GetAssignedDivisions(string userid);
        bool ChangeCompanyStatus(long comapnyId);
        int DeleteCostCenter(long id);
        PagedList GetAllComapnies(PagedList pageList);
        PagedList GetAllCostCenters(long divisionId, string type, string userId, string searchtext, int page = 1, int pageSize = 10,
                                         string sortBy = "Id", string sortDirection = "asc");
        CostCenterDto GetCostCentersById(long id, string userId);       
        PagedList GetAllComapniesForAdminSearch(string searchtext);
        string GetBusinessOwneridbyCompanyId(string companyId);
        CompanyDto GetCompanyByUserID(string userID);
        bool UpdateCompanyLogo(string URL, long customerId);
        NodeDto GetOrganizationStructure(string userId);
        long GetTenantIdByUserId(string userid);
        void SaveUserPhoneCode(UserDto userDto);
        void SaveUserPhoneConfirmation(UserDto userDto);
        void DeleteCompanyDetails(long tenantId,string userId);
    }
}
