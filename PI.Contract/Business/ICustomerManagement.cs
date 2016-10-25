using PI.Contract.DTOs.Customer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Contract.Business
{
    public interface ICustomerManagement
    {
        //int VerifyUserLogin(CustomerDto customer);

        string GetJwtToken(string userid, string role, string tenantId, string userName, string companyId);

        string GetJwtTokenFromCurrentToken(string currentToken);

        CustomerDto GetCustomerByCompanyId(int companyId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="loggedInUserId"></param>
        /// <returns></returns>
        string GetThemeColour(string loggedInUserId);

        int SaveCustomer(CustomerDto customer, bool isCustomerRegistration = false);

        byte[] GetBytes(string input);

        void DeleteCustomer(string userId);

    }
}
