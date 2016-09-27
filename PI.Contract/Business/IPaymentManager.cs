using PI.Contract.DTOs;
using PI.Contract.DTOs.Payment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Contract.Business
{
    public interface IPaymentManager
    {
        OperationResult Charge(PaymentDto paymentDto);
    }
}
