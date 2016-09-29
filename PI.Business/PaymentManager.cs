using PI.Contract.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PI.Contract.DTOs;
using PI.Contract.DTOs.Payment;

using Square.Connect.Api;
using Square.Connect.Model;
using System.Configuration;
using System.Web.Script.Serialization;
using Newtonsoft.Json.Linq;

namespace PI.Business
{
    public class PaymentManager : IPaymentManager
    {
        private static LocationApi locationApi;
        private static TransactionApi _transactionApi;
        //private string _accessToken = "sandbox-sq0atb-m24_A-IqrhzXk1Y8LM2PoA";
        //private string _locationId = "CBASEFV_9FQXS2BGj_6H9k1y2cw";

        private string GetAccessToken
        {
            get
            {
                //return ConfigurationManager.AppSettings["SquareAccessToken"].ToString();
                return "sandbox-sq0atb-m24_A-IqrhzXk1Y8LM2PoA"; // Remove this and enable above one to get from web.config
            }
        }

        public PaymentManager()
        {
            _transactionApi = new TransactionApi();
            locationApi = new LocationApi();
        }

        public OperationResult Charge(PaymentDto paymentDto)
        {
            OperationResult result = new OperationResult();

            // Every payment you process with the SDK must have a unique idempotency key.
            // If you're unsure whether a particular payment succeeded, you can reattempt
            // it with the same idempotency key without worrying about double charging
            // the buyer.
            string uuid = NewIdempotencyKey();

            // Monetary amounts are specified in the smallest unit of the applicable currency.
            // This amount is in cents.
            // Currently this will support for only USD.
            // Convert to cents representation.
            if (paymentDto.CurrencyType == "USD")
                paymentDto.ChargeAmount = paymentDto.ChargeAmount * 100;
            else
            {
                result.Message = "Currency format is not supporting";
                result.Status = Contract.Enums.Status.PaymentError;
                return result;
            }
                
            Money amount = NewMoney(Convert.ToInt32(paymentDto.ChargeAmount), paymentDto.CurrencyType);

            ChargeRequest body = new ChargeRequest(AmountMoney: amount, IdempotencyKey: uuid, CardNonce: paymentDto.CardNonce);

            ChargeResponse response = null;
            string paymentError = null;

            try
            {
                response = _transactionApi.Charge(GetAccessToken, GetLocation()[0].Id, body);
            }
            catch(Exception ex)
            {
                response = new ChargeResponse();

                int indexOfStartJson = ex.Message.IndexOf('{');
                string jsonFormatMessage = ex.Message.Substring(indexOfStartJson);

                dynamic errorMessageList = JObject.Parse(jsonFormatMessage);

                var error = errorMessageList.errors;

                paymentError = error[0].detail;
                string errorCode = error[0].code;
                result.FieldList.Add("errorCode", errorCode);
                //response.Errors.Add(new Error() { Detail = error[0].detail,Category = error[0].category,Code = error[0].code });
            }

            result.FieldList.Add("PaymentKey", uuid);

            if (response.Errors == null && string.IsNullOrEmpty(paymentError))
            {
                result.Message = "Transaction complete\n" + response.ToJson();
                result.Status = Contract.Enums.Status.Success;
            }
            else
            {
                result.Message = string.IsNullOrEmpty(paymentError) ? response.Errors.ToString() : paymentError;
                result.Status = Contract.Enums.Status.PaymentError;
            }

            return result;
        }
        
        private IList<Location> GetLocation()
        {
            return locationApi.ListLocations(GetAccessToken).Locations.ToList();
        }

        private static Money NewMoney(int amount, string currency)
        {
            return new Money(amount, Money.ToCurrencyEnum(currency));
        }

        private static string NewIdempotencyKey()
        {
            return Guid.NewGuid().ToString();
        }
    }
}
