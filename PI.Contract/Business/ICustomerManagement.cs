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
        string VerifyUserLogin(CustomerDto customer);

       // PI.Contract.DTOs.Customer GetCustomerById(long id);


    }
}
