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
        private static RefundApi refundApi;
        //private string _accessToken = "sandbox-sq0atb-m24_A-IqrhzXk1Y8LM2PoA";
        //private string _locationId = "CBASEFV_9FQXS2BGj_6H9k1y2cw";

        private string GetAccessToken
        {
            get
            {
                return ConfigurationManager.AppSettings["SquareAccessToken"].ToString();
                //return "sandbox-sq0atb-m24_A-IqrhzXk1Y8LM2PoA";
            }
        }

        public PaymentManager()
        {
            _transactionApi = new TransactionApi();
            locationApi = new LocationApi();
            refundApi = new RefundApi();
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
            decimal chargeAmountInCent = 0;
            if (paymentDto.CurrencyType == "USD")
                chargeAmountInCent = paymentDto.ChargeAmount * 100;
            else
            {
                result.Message = "Currency format is not supporting";
                result.Status = Contract.Enums.Status.PaymentError;
                return result;
            }
                
            Money amount = NewMoney(Convert.ToInt32(chargeAmountInCent), paymentDto.CurrencyType);

            ChargeRequest body = new ChargeRequest(AmountMoney: amount, IdempotencyKey: uuid, CardNonce: paymentDto.CardNonce);

            ChargeResponse response = null;
            string paymentError = null;
            string locationId = null;

            try
            {
                locationId = GetLocation()[0].Id;
                response = _transactionApi.Charge(GetAccessToken, locationId, body);
            }
            catch (Exception ex)
            {
                response = new ChargeResponse();

                int indexOfStartJson = ex.Message.IndexOf('{');
                string jsonFormatMessage = ex.Message.Substring(indexOfStartJson);

                dynamic errorMessageList = JObject.Parse(jsonFormatMessage);

                var error = errorMessageList.errors;

                paymentError = error[0].detail;
                string errorCode = error[0].code;
                result.FieldList.Add("errorCode", errorCode);
            }

            result.FieldList.Add("PaymentKey", uuid);
            result.FieldList.Add("TenderId", response.Transaction.Tenders.First().Id);
            result.FieldList.Add("LocationId", locationId);
            result.FieldList.Add("TransactionId", response.Transaction.Id);

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

        public OperationResult Refund(PaymentDto paymentDto)
        {
            string refundUuid = NewIdempotencyKey();

            decimal chargeAmountInCent = paymentDto.ChargeAmount * 100;
            Money amount = NewMoney(Convert.ToInt32(chargeAmountInCent), paymentDto.CurrencyType);

            var refundBody = new CreateRefundRequest(refundUuid, paymentDto.TenderId, null, amount);
            CreateRefundResponse refundResponse = null;
            OperationResult result = new OperationResult();

            try
            {
                refundResponse = refundApi.CreateRefund(GetAccessToken, paymentDto.LocationId, paymentDto.TransactionId, refundBody);

                if (refundResponse.Refund.Status.HasValue)
                    result.Status = refundResponse.Refund.Status.Value == Square.Connect.Model.Refund.StatusEnum.Approved ?
                                    Contract.Enums.Status.Success : Contract.Enums.Status.PaymentError;
                else
                    result.Status = Contract.Enums.Status.PaymentError;
            }
            catch (Exception ex)
            {
                result.Status = Contract.Enums.Status.PaymentError;
                result.Message = ex.Message;
            }

            return result;
        }

        private IList<Location> GetLocation()
        {
            var location = locationApi.ListLocations(GetAccessToken).Locations.ToList();

            // To process the payment, need to have location capability location id.
            var processingCapabilityLocList = location.Where(l => l.Capabilities != null && l.Capabilities.Count != 0).ToList();

            if (processingCapabilityLocList.Count == 0)
                throw new Exception("Currently don't have Credit Card Processing Enabled location");

            return processingCapabilityLocList;
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
