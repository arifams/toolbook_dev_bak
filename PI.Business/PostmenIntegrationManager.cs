﻿using PI.Contract.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PI.Contract.DTOs.RateSheets;
using PI.Contract.DTOs.Shipment;
using System.Net;
using System.IO;
using PI.Contract.DTOs.Postmen;
using System.Web.Script.Serialization;

namespace PI.Business
{
    public class PostmenIntegrationManager : ICarrierIntegrationManager
    {
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
            AddShipmentResponse response = new AddShipmentResponse();
            WebRequest httpWebRequest = WebRequest.Create("https://sandbox-api.postmen.com/v3/labels");
            var request = this.CreateRequestJson(addShipment);
            string json = request;

            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";
            httpWebRequest.Headers["postmen-api-key"] = "8fc7966b-679b-4a57-911d-c5a663229c9e";

            using (StreamWriter streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                streamWriter.Write(json);
                streamWriter.Flush();
                streamWriter.Close();
            }

            HttpWebResponse httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (StreamReader streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                string result = streamReader.ReadToEnd();
                Console.WriteLine(result);
            }

            return response;
        }

        private string CreateRequestJson(ShipmentDto addShipment)
        {
            string Json = "";
            ShipmentRequestDto request = new ShipmentRequestDto();

            request.async = false;

            request.billing = new PMbilling() {
                paid_by = "shipper"
            };           

            request.customs = new PMcustoms() {
                billing = new PMbilling() {
                paid_by = "recipient",
            },
            terms_of_trade = addShipment.GeneralInformation.ShipmentServices,
            purpose = ""           
            };

            request.shipment = new PMShipment()
            {
                ship_from = new PMAddress()
                {

                    contact_name = addShipment.AddressInformation.Consigner.ContactName,
                    street1 = addShipment.AddressInformation.Consigner.Address1,
                    street2 = addShipment.AddressInformation.Consigner.Address2,
                    postal_code = addShipment.AddressInformation.Consigner.Postalcode,
                    state = addShipment.AddressInformation.Consigner.State,
                    country = addShipment.AddressInformation.Consigner.Country,
                    phone = addShipment.AddressInformation.Consigner.ContactNumber,
                    email = addShipment.AddressInformation.Consigner.Email,
                    city = addShipment.AddressInformation.Consigner.City,
                    company_name = addShipment.AddressInformation.Consigner.CompanyName

                },


            ship_to = new PMAddress()
            {
                contact_name = addShipment.AddressInformation.Consignee.ContactName,
                street1 = addShipment.AddressInformation.Consignee.Address1,
                street2 = addShipment.AddressInformation.Consignee.Address2,
                postal_code = addShipment.AddressInformation.Consignee.Postalcode,
                state = addShipment.AddressInformation.Consignee.State,
                country = addShipment.AddressInformation.Consignee.Country,
                phone = addShipment.AddressInformation.Consignee.ContactNumber,
                email = addShipment.AddressInformation.Consignee.Email,
                city = addShipment.AddressInformation.Consignee.City,
                company_name = addShipment.AddressInformation.Consignee.CompanyName
            }

            };


            request.shipment.parcels = new List<PMParcel>();

            foreach (var products in addShipment.PackageDetails.ProductIngredients)
            {

                request.shipment.parcels.Add(new PMParcel()
                {
                    box_type= "custom",
                    description= products.Description,
                    dimension= new PMDimension()
                    {
                       depth=products.Length,
                       height=products.Height,
                       width=products.Width,
                       unit=addShipment.PackageDetails.VolumeCMM==true?"cm":"inch"                   
                        
                    },
                    items= new List<PMItems>()
                    {
                        new PMItems() {
                           description= products.Description,
                           hs_code=addShipment.PackageDetails.HsCode,
                           origin_country=addShipment.AddressInformation.Consigner.Country,
                           quantity=products.Quantity,
                           price= new PMPrice
                           {
                               amount=0,
                               currency=addShipment.CarrierInformation.currency

                           },
                           sku="",
                           weight= new PMWeight()
                           {
                               unit=addShipment.PackageDetails.CmLBS==true?"kg":"lbs",
                               value=products.Weight
                           }

                        }
                    },
                    weight= new PMWeight()
                    {
                        unit = addShipment.PackageDetails.CmLBS == true ? "kg" : "lbs",
                        value = products.Weight
                    }


                });
            }
            
            request.shipment.parcels.FirstOrDefault().items.FirstOrDefault().price.amount = addShipment.CarrierInformation.Price;
            
           JavaScriptSerializer serializer = new JavaScriptSerializer();
           return serializer.Serialize(request);                     
        }


        public string TrackAndTraceShipment(string URL)
        {
            throw new NotImplementedException();
        }
    }

}

