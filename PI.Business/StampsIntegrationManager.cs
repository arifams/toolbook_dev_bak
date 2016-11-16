using PI.Contract.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PI.Contract.DTOs.RateSheets;
using PI.Contract.DTOs.Shipment;
using System.Xml;
using System.Configuration;
using PI.Contract.ProxyClasses;
using PI.Contract.ProxyClasses.SwsimV55;

namespace PI.Business
{
    class StampsIntegrationManager : ICarrierIntegrationManager
    {


        public string StampsComIntegrationId
        {
            get
            {
                return ConfigurationManager.AppSettings["StampsComIntegrationId"].ToString();
            }
        }

        public string StampsComUserName
        {
            get
            {
                return ConfigurationManager.AppSettings["StampsComUserName"].ToString();
            }
        }

        public string StampsComPassword
        {
            get
            {
                return ConfigurationManager.AppSettings["StampsComPassword"].ToString();
            }
        }


        public void DeleteShipment(string shipmentCode)
        {
            throw new NotImplementedException();
        }

        public string GetLabel(string shipmentCode)
        {
            throw new NotImplementedException();
        }

        public ShipmentcostList GetRateSheetForShipment(RateSheetParametersDto rateParameters)
        {
            throw new NotImplementedException();
        }

        public string GetShipmentStatus(string URL, string shipmentCode)
        {
            throw new NotImplementedException();
        }

        public StatusHistoryResponce GetUpdatedShipmentStatusehistory(string carrier, string trackingNumber, string codeShipment, string environment)
        {
            throw new NotImplementedException();
        }

        public AddShipmentResponse SendShipmentDetails(ShipmentDto addShipment)
        {
            AddShipmentResponse shipmentResponse = new AddShipmentResponse();
            AuthenticateUserRequest request = new AuthenticateUserRequest() {
                Credentials = new Credentials()
                {
                    IntegrationID= Guid.Parse(StampsComIntegrationId),
                    Username = StampsComUserName,
                    Password=StampsComPassword
                }
            };
            SwsimV55Soap soapClient = new SwsimV55SoapClient();
            DateTime LastLoginTime = DateTime.Now;
            
            AuthenticateUserResponse AuthenticateResponse = soapClient.AuthenticateUser(request);

            if (AuthenticateResponse.Authenticator!= null)
            {
                CreateIndiciumRequest Indiciumrequest = new CreateIndiciumRequest();
                Indiciumrequest.Item = AuthenticateResponse.Authenticator;
                Indiciumrequest.IntegratorTxID = addShipment.GeneralInformation.ShipmentReferenceName;
                Indiciumrequest.Rate = new RateV20()
                {
                    Amount = addShipment.CarrierInformation.Price,
                    DeclaredValue=addShipment.PackageDetails.DeclaredValue,
                    InsuredValue=addShipment.CarrierInformation.Insurance,
                    ServiceType=ServiceType.USPS,
                    PackageType=PackageTypeV6.Package,
                    ToCountry=addShipment.AddressInformation.Consignee.Country,
                    ToState=addShipment.AddressInformation.Consignee.State,
                    ToZIPCode=addShipment.AddressInformation.Consignee.Postalcode,
                    FromZIPCode=addShipment.AddressInformation.Consigner.Postalcode,                 
                };
                
              CreateIndiciumResponse IndiciumResponse= soapClient.CreateIndicium(Indiciumrequest);
                
            }
            
            return shipmentResponse;
        }
        

        private string CreateAddShipmentSoapString(ShipmentDto addShipment)
        {

            double weightInLbs = addShipment.PackageDetails.CmLBS == true ? Convert.ToDouble(addShipment.PackageDetails.TotalWeight) * 2.20462: Convert.ToDouble(addShipment.PackageDetails.TotalWeight) ;

            StringBuilder addShipmentXml = new StringBuilder();
            addShipmentXml.Append(@"<soap:Envelope xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">");
            addShipmentXml.Append("<soap:Body> <tns:CreateIndicium>");
            addShipmentXml.Append("<tns:Authenticator>"+ "" + "</tns:Authenticator>");
            addShipmentXml.Append("<tns:IntegratorTxID>"+ addShipment.GeneralInformation.ShipmentReferenceName+"</tns:IntegratorTxID>");

            addShipmentXml.Append("<tns:TrackingNumber/>");
            addShipmentXml.Append("<tns:Rate>");
            addShipmentXml.Append("<tns:FromZIPCode>"+  addShipment.AddressInformation.Consigner.Postalcode+"</tns:FromZIPCode>");
            addShipmentXml.Append("<tns:ToZIPCode>"+ addShipment.AddressInformation.Consignee.Postalcode + "</tns:ToZIPCode>");
            addShipmentXml.Append("<tns:Amount>10</tns:Amount>");
            addShipmentXml.Append("<tns:ServiceType>US-PM</tns:ServiceType>");
            addShipmentXml.Append("<tns:DeliverDays>1-1</tns:DeliverDays>");
            addShipmentXml.Append("<tns:WeightLb>"+ weightInLbs + "</tns:WeightLb>");
            addShipmentXml.Append("<tns:WeightOz>0</tns:WeightOz>");
            addShipmentXml.Append("<tns:PackageType>Package</tns:PackageType>");
            addShipmentXml.Append("<tns:ShipDate>2009-08-31</tns:ShipDate>");
            addShipmentXml.Append("<tns:InsuredValue>"+addShipment.CarrierInformation.Insurance+"</tns:InsuredValue>");
            addShipmentXml.Append("<tns:RectangularShaped>false</tns:RectangularShaped>");
            addShipmentXml.Append("</tns:Rate>");

            addShipmentXml.Append("<tns:From>");
            addShipmentXml.Append("<tns:FullName>"+ addShipment.AddressInformation.Consigner.FirstName+" "+ addShipment.AddressInformation.Consigner.LastName + "</tns:FullName>");
            addShipmentXml.Append("<tns:Address1>" + addShipment.AddressInformation.Consigner.Address1 + "</tns:Address1>");
            addShipmentXml.Append("<tns:Address2>" + addShipment.AddressInformation.Consigner.Address2 + "</tns:Address2>");
            addShipmentXml.Append("<tns:City>"+ addShipment.AddressInformation.Consigner.City+ "</tns:City>");
            addShipmentXml.Append("<tns:State>"+ addShipment.AddressInformation.Consigner.State+ "</tns:State>");
            addShipmentXml.Append("<tns:ZIPCode>"+ addShipment.AddressInformation.Consigner.Postalcode + "</tns:ZIPCode>");
            addShipmentXml.Append("<tns:PhoneNumber>" + addShipment.AddressInformation.Consigner.ContactNumber + "<tns:PhoneNumber/>");
            addShipmentXml.Append("</tns:From>");


            addShipmentXml.Append("<tns:To>");
            addShipmentXml.Append("<tns:FullName>"+ addShipment.AddressInformation.Consignee.FirstName+" "+addShipment.AddressInformation.Consignee.LastName + "</tns:FullName>");
            
            addShipmentXml.Append("<tns:Company>"+ addShipment.AddressInformation.Consignee.CompanyName+"</tns:Company>");
            addShipmentXml.Append("<tns:Address1>"+ addShipment.AddressInformation.Consignee.Address1 + "</tns:Address1>");
            addShipmentXml.Append("<tns:Address2>"+ addShipment.AddressInformation.Consignee.Address2 + "  <tns:Address2/>");
            addShipmentXml.Append("<tns:City>"+ addShipment.AddressInformation.Consignee.City + "</tns:City>");
            addShipmentXml.Append("<tns:State>"+addShipment.AddressInformation.Consignee.State+"</tns:State>");
            addShipmentXml.Append("<tns:ZIPCode>"+addShipment.AddressInformation.Consignee.Postalcode+"</tns:ZIPCode>");         
            addShipmentXml.Append("<tns:Country>"+ addShipment.AddressInformation.Consignee.City + "</tns:Country>");
           // addShipmentXml.Append("<tns:Urbanization/>");
           // addShipmentXml.Append("<tns:PhoneNumber>"+addShipment.AddressInformation.Consignee.ContactNumber+"<tns:PhoneNumber/>");           
          //  addShipmentXml.Append("<tns:CleanseHash>7SWYAzuNh82cWhIQyRFXRNa71HFkZWFkYmVlZg==20070513</tns:CleanseHash>");
            addShipmentXml.Append("</tns:To>");



            addShipmentXml.Append("<Customs>");
            addShipmentXml.Append("<ContentType>Commercial Sample or Gift or Document or Returned Goods or Other or Merchandise or Humanitarian Donation or Dangerous Goods</ContentType>");
            addShipmentXml.Append("<Comments>string</Comments>");
            addShipmentXml.Append("<LicenseNumber>string</LicenseNumber>");
            addShipmentXml.Append("<CertificateNumber>string</CertificateNumber>");
            addShipmentXml.Append(" <InvoiceNumber>string</InvoiceNumber>");
            addShipmentXml.Append(" <OtherDescribe>string</OtherDescribe>");
            
            addShipmentXml.Append("< CustomsLines >");

            foreach (var lineItem in addShipment.PackageDetails.ProductIngredients)
            {

                double weight = addShipment.PackageDetails.CmLBS == true ? Convert.ToDouble(lineItem.Weight) * 2.20462 : Convert.ToDouble(lineItem.Weight);

                addShipmentXml.Append(" <CustomsLine>");
                addShipmentXml.Append(" <Description>"+ lineItem.Description+ "</Description>");
                addShipmentXml.Append("<Quantity>"+lineItem.Quantity+"</Quantity>");
               // addShipmentXml.Append(" <Value></Value>");
                addShipmentXml.Append("<WeightLb>"+ weight.ToString() + "</WeightLb>");
                addShipmentXml.Append("<HSTariffNumber>"+addShipment.CarrierInformation.tariffText+"</HSTariffNumber>");              
                addShipmentXml.Append(" </CustomsLine>");
             
            }
            addShipmentXml.Append(" <CountryOfOrigin>"+addShipment.AddressInformation.Consigner.Country+"</CountryOfOrigin>");
            addShipmentXml.Append("</CustomsLines>");
            
            addShipmentXml.Append("</tns:CreateIndicium>");
            addShipmentXml.Append("</soap:Body>");
            addShipmentXml.Append("</soap:Envelope>");         


            return addShipmentXml.ToString();
        }

        private string CreateGetAuthenticatorSoapString()
        {
            StringBuilder AuthorizeRequest = new StringBuilder();
            AuthorizeRequest.Append(@"<soap:Envelope xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:tns=""http://stamps.com/xml/namespace/2016/09/swsim/swsimv55"">");
            AuthorizeRequest.Append("<soap:Body>");
            AuthorizeRequest.Append("<tns:AuthenticateUser>");
            AuthorizeRequest.Append("<tns:Credentials>");
            AuthorizeRequest.Append("<tns:Credentials> <tns:IntegrationID>"+ StampsComIntegrationId + "</tns:IntegrationID>");
            AuthorizeRequest.Append("<tns:Username>"+ StampsComUserName+"</tns:Username>");
            AuthorizeRequest.Append("<tns:Password>"+ StampsComPassword+ "</tns:Password>");
            AuthorizeRequest.Append("</tns:Credentials>");
            AuthorizeRequest.Append("</tns:AuthenticateUser>");
            AuthorizeRequest.Append("</soap:Body>");
            AuthorizeRequest.Append("</soap:Envelope>");           
            return AuthorizeRequest.ToString();
        }


        public string TrackAndTraceShipment(string URL)
        {
            throw new NotImplementedException();
        }
    }
}
