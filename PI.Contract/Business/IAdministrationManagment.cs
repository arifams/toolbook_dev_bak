using PI.Contract.DTOs;
using PI.Contract.DTOs.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Contract.Business
{
    public interface IAdministrationManagment
    {
        OperationResult ImportRateSheetExcel(string URI);


        /// <summary>
        /// Manage invoice payment setting
        /// </summary>
        /// <param name="comapnyId"></param>
        /// <returns></returns>
        bool ManageInvoicePaymentSetting(long comapnyId);
        PagedList GetAllInvoices(string status, string userId, DateTime? startDate, DateTime? endDate, string shipmentnumber, string businessowner, string invoicenumber);
    }
}
