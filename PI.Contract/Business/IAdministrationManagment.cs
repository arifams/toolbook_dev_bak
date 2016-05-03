using PI.Contract.DTOs;
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
    }
}
