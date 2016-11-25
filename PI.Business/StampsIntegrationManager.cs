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
using PI.Contract.Enums;

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

            AuthenticateUserRequest request = new AuthenticateUserRequest()
            {
                Credentials = new Credentials()
                {
                    IntegrationID = Guid.Parse(StampsComIntegrationId),
                    Username = StampsComUserName,
                    Password = StampsComPassword
                }
            };
            SwsimV55Soap soapClient = new SwsimV55SoapClient();
            DateTime LastLoginTime = DateTime.Now;

            AuthenticateUserResponse AuthenticateResponse = soapClient.AuthenticateUser(request);
            CancelIndiciumRequest cancelRequest = new CancelIndiciumRequest();
            CancelIndiciumResponse cancelResponse = new CancelIndiciumResponse();

            if (AuthenticateResponse.Authenticator != null)
            {
                cancelRequest.Item = AuthenticateResponse.Authenticator;
                cancelRequest.Item1 = new Guid(shipmentCode);
               
                try
                {
                    cancelResponse = soapClient.CancelIndicium(cancelRequest);
                }
                catch (Exception e)
                {
                    throw;
                }
                
            }

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


        //sending multiple shipments to Stamps at once
        public AddShipmentResponse SendShipmentDetails(ShipmentDto addShipment)
        {
            List<AddShipmentResponse> addShipmentResponseList = new List<AddShipmentResponse>();
            AddShipmentResponse shipmentResponse = new AddShipmentResponse();

            //get the single package
            var package = addShipment.PackageDetails.ProductIngredients.FirstOrDefault();

                AuthenticateUserRequest request = new AuthenticateUserRequest()
                {
                    Credentials = new Credentials()
                    {
                        IntegrationID = Guid.Parse(StampsComIntegrationId),
                        Username = StampsComUserName,
                        Password = StampsComPassword
                    }
                };

                SwsimV55Soap soapClient = new SwsimV55SoapClient();
                DateTime LastLoginTime = DateTime.Now;

                AuthenticateUserResponse AuthenticateResponse = soapClient.AuthenticateUser(request);

                if (AuthenticateResponse.Authenticator != null)
                {

                    CleanseAddressRequest fromAddressRequest = new CleanseAddressRequest()
                    {
                        Address = new Address
                        {
                            FirstName = addShipment.AddressInformation.Consigner.FirstName,
                            LastName = addShipment.AddressInformation.Consigner.LastName,
                            Address1 = addShipment.AddressInformation.Consigner.Address1,
                            Address2 = addShipment.AddressInformation.Consigner.Address2,
                            City = addShipment.AddressInformation.Consigner.City,
                            State = addShipment.AddressInformation.Consigner.State,
                            Country = addShipment.AddressInformation.Consigner.Country,
                            ZIPCode = addShipment.AddressInformation.Consigner.Postalcode
                        },
                        Item = AuthenticateResponse.Authenticator
                    };

                    CleanseAddressResponse fromAddressResponse = soapClient.CleanseAddress(fromAddressRequest);

                    CleanseAddressRequest toAddressRequest = new CleanseAddressRequest()
                    {
                        Address = new Address
                        {
                            FirstName = addShipment.AddressInformation.Consignee.FirstName,
                            LastName = addShipment.AddressInformation.Consignee.LastName,
                            Address1 = addShipment.AddressInformation.Consignee.Address1,
                            Address2 = addShipment.AddressInformation.Consignee.Address2,
                            City = addShipment.AddressInformation.Consignee.City,
                            State = addShipment.AddressInformation.Consignee.State,
                            Country = addShipment.AddressInformation.Consignee.Country,
                            ZIPCode = addShipment.AddressInformation.Consignee.Postalcode
                        },
                        Item = fromAddressResponse.Authenticator
                    };
                    CleanseAddressResponse toAddressResponse = soapClient.CleanseAddress(toAddressRequest);


                    //add Adon to rate object to hide the rate amount from the label
                    AddOnV7 hiddenRateAddOn = new AddOnV7()
                    {
                        Amount = addShipment.CarrierInformation.Price,
                        AddOnType = AddOnTypeV7.SCAHP
                    };

                    AddOnV7[] rateAddonArray;
                    rateAddonArray = new AddOnV7[1];
                    rateAddonArray[0] = hiddenRateAddOn;

                    //ServiceType servicetype= addShipment.CarrierInformation.serviceLevel                 

                    CreateIndiciumRequest Indiciumrequest = new CreateIndiciumRequest();

                    //set authenticator from to address response
                    Indiciumrequest.Item = toAddressResponse.Authenticator;
                    Indiciumrequest.IntegratorTxID = addShipment.GeneralInformation.ShipmentReferenceName;

                    double weightLbs = addShipment.PackageDetails.CmLBS == true ? Convert.ToDouble(package.Weight) * 2.20462 : Convert.ToDouble(package.Weight);

                    double lengthInches = 0;
                    double widthInches = 0;
                    double heightInches = 0;

                    if (addShipment.PackageDetails.VolumeCMM)
                    {
                       lengthInches = Convert.ToDouble(package.Length) * 0.393701;
                       widthInches = Convert.ToDouble(package.Width) * 0.393701;
                       heightInches = Convert.ToDouble(package.Height) * 0.393701;

                    }
                    else
                    {
                       lengthInches = Convert.ToDouble(package.Length);
                       widthInches = Convert.ToDouble(package.Length);
                       heightInches = Convert.ToDouble(package.Length);

                    }




                Indiciumrequest.Rate = new RateV20()
                {
                    Amount = addShipment.CarrierInformation.Price,
                    DeclaredValue = addShipment.PackageDetails.DeclaredValue,
                    InsuredValue = addShipment.CarrierInformation.Insurance,
                    ServiceType = this.GetServiceType(addShipment.CarrierInformation.description),
                    PackageType = this.GetPackageType(package.ProductType, lengthInches, widthInches, heightInches, weightLbs, addShipment.CarrierInformation.description),
                    ToCountry = addShipment.AddressInformation.Consignee.Country,
                    ToState = addShipment.AddressInformation.Consignee.State,
                    ToZIPCode = addShipment.AddressInformation.Consignee.Postalcode,
                    FromZIPCode = addShipment.AddressInformation.Consigner.Postalcode,
                    DeliveryDate = addShipment.CarrierInformation.DeliveryTime != null ? DateTime.Parse(addShipment.CarrierInformation.DeliveryTime.ToString()) : DateTime.Now,
                    AddOns = rateAddonArray,
                    Length = lengthInches,
                    Width = widthInches,
                    Height = heightInches,
                    WeightLb = weightLbs,
                    MaxAmount = Convert.ToDecimal(addShipment.PackageDetails.CarrierCost),
                    // MaxDimensions = package.Length + (package.Width * 2) + (package.Height * 2).ToString(),
                    MaxDimensions = (lengthInches +widthInches +heightInches).ToString(),
                    ShipDate = DateTime.Now.AddDays(3),
                    WeightOz =weightLbs *16,

                    };

                    if (fromAddressResponse.AddressMatch)
                    {
                        //sending clensed address with cleanse hash
                        Indiciumrequest.From = new Address
                        {
                            FirstName = addShipment.AddressInformation.Consigner.FirstName,
                            LastName = addShipment.AddressInformation.Consigner.LastName,
                            FullName = addShipment.AddressInformation.Consigner.FirstName + " " + addShipment.AddressInformation.Consigner.LastName,
                            Company = addShipment.AddressInformation.Consigner.CompanyName,
                            Address1 = fromAddressResponse.Address.Address1,
                            Address2 = fromAddressResponse.Address.Address2,
                            City = fromAddressResponse.Address.City,
                            State = fromAddressResponse.Address.State,
                            Country = fromAddressResponse.Address.Country,
                            PostalCode = fromAddressResponse.Address.PostalCode,
                            PhoneNumber = addShipment.AddressInformation.Consigner.ContactNumber,
                            CleanseHash = fromAddressResponse.Address.CleanseHash
                        };

                    }
                    else
                    {
                        //sending the normal address with overrided hash
                        Indiciumrequest.From = new Address
                        {
                            FirstName = addShipment.AddressInformation.Consigner.FirstName,
                            LastName = addShipment.AddressInformation.Consigner.LastName,
                            FullName = addShipment.AddressInformation.Consigner.FirstName + " " + addShipment.AddressInformation.Consigner.LastName,
                            Company = addShipment.AddressInformation.Consigner.CompanyName,
                            Address1 = addShipment.AddressInformation.Consigner.Address1,
                            Address2 = addShipment.AddressInformation.Consigner.Address2,
                            City = addShipment.AddressInformation.Consigner.City,
                            State = addShipment.AddressInformation.Consigner.State,
                            Country = addShipment.AddressInformation.Consigner.Country,
                            //PostalCode = addShipment.AddressInformation.Consigner.Postalcode,
                            PhoneNumber = addShipment.AddressInformation.Consigner.ContactNumber,
                            OverrideHash = fromAddressResponse.Address.OverrideHash,
                            ZIPCode = addShipment.AddressInformation.Consigner.Postalcode
                        };

                    }

                    if (toAddressResponse.AddressMatch)
                    {
                        Indiciumrequest.To = new Address
                        {
                            FirstName = addShipment.AddressInformation.Consignee.FirstName,
                            LastName = addShipment.AddressInformation.Consignee.LastName,
                            FullName = addShipment.AddressInformation.Consignee.FirstName + " " + addShipment.AddressInformation.Consigner.LastName,
                            Company = addShipment.AddressInformation.Consignee.CompanyName,
                            Address1 = toAddressResponse.Address.Address1,
                            Address2 = toAddressResponse.Address.Address2,
                            City = toAddressResponse.Address.City,
                            State = toAddressResponse.Address.State,
                            Country = toAddressResponse.Address.Country,
                            //  PostalCode = toAddressResponse.Address.PostalCode,
                            PhoneNumber = addShipment.AddressInformation.Consignee.ContactNumber,
                            CleanseHash = toAddressResponse.Address.CleanseHash,
                            ZIPCode = toAddressResponse.Address.PostalCode
                        };

                    }
                    else
                    {
                        Indiciumrequest.To = new Address
                        {
                            FirstName = addShipment.AddressInformation.Consignee.FirstName,
                            LastName = addShipment.AddressInformation.Consignee.LastName,
                            FullName = addShipment.AddressInformation.Consignee.FirstName + " " + addShipment.AddressInformation.Consigner.LastName,
                            Company = addShipment.AddressInformation.Consignee.CompanyName,
                            Address1 = addShipment.AddressInformation.Consignee.Address1,
                            Address2 = addShipment.AddressInformation.Consignee.Address2,
                            City = addShipment.AddressInformation.Consignee.City,
                            State = addShipment.AddressInformation.Consignee.State,
                            Country = addShipment.AddressInformation.Consignee.Country,
                            //  PostalCode = addShipment.AddressInformation.Consignee.Postalcode,
                            PhoneNumber = addShipment.AddressInformation.Consignee.ContactNumber,
                            //   OverrideHash = toAddressResponse.Address.OverrideHash,
                            ZIPCode = addShipment.AddressInformation.Consignee.Postalcode
                        };
                    }


                    if (addShipment.AddressInformation.Consignee.Country != "US" && addShipment.AddressInformation.Consigner.Country != "US")
                    {
                        Indiciumrequest.Customs = new CustomsV4();
                        Indiciumrequest.Customs.CustomsLines = new CustomsLine[1];
                        Indiciumrequest.Customs.CustomsLines[0] = new CustomsLine()
                        {
                            CountryOfOrigin = addShipment.AddressInformation.Consigner.Country,
                            Description = package.Description,
                            //HSTariffNumber = addShipment.PackageDetails.HsCode,
                            Quantity = package.Quantity,
                            WeightLb = addShipment.PackageDetails.CmLBS == true ? Convert.ToDouble(package.Weight) * 2.20462 : Convert.ToDouble(package.Weight),

                            Value = Convert.ToDecimal(addShipment.PackageDetails.CarrierCost),

                        };

                    }


                    CreateIndiciumResponse IndiciumResponse = null;
                    try
                    {
                        IndiciumResponse = soapClient.CreateIndicium(Indiciumrequest);
                        // IndiciumResponse = soapClient.CreateIndicium(sample);
                    }
                    catch (Exception e)
                    {
                        shipmentResponse.AddShipmentXML = e.Message;
                    }

                    if (IndiciumResponse == null)
                    {
                        return null;
                    }

                    if (IndiciumResponse.TrackingNumber != null)
                    {

                        CarrierPickupRequest pickupRequest = new CarrierPickupRequest()
                        {
                            Item = IndiciumResponse.Authenticator,
                            FirstName = addShipment.AddressInformation.Consigner.FirstName,
                            LastName = addShipment.AddressInformation.Consigner.LastName,
                            Address = fromAddressResponse.AddressMatch == true ? fromAddressResponse.Address.Address1 + " " + fromAddressResponse.Address.Address2 : addShipment.AddressInformation.Consigner.Address1 + " " + addShipment.AddressInformation.Consigner.Address2,
                            City = fromAddressResponse.AddressMatch == true ? fromAddressResponse.Address.City : addShipment.AddressInformation.Consigner.City,
                            State = fromAddressResponse.AddressMatch == true ? fromAddressResponse.Address.State : addShipment.AddressInformation.Consigner.State,
                            ZIP = fromAddressResponse.AddressMatch == true ? fromAddressResponse.Address.ZIPCode : addShipment.AddressInformation.Consigner.Postalcode,
                            PhoneNumber = addShipment.AddressInformation.Consigner.ContactNumber,
                            TotalWeightOfPackagesLbs = addShipment.PackageDetails.CmLBS == true ? Convert.ToInt32(Convert.ToDouble(addShipment.PackageDetails.TotalWeight) * 2.20462) : Convert.ToInt32(addShipment.PackageDetails.TotalWeight),
                            
                        };

                        if (Indiciumrequest.Rate.ServiceType == ServiceType.USFC || Indiciumrequest.Rate.ServiceType == ServiceType.USFCI)
                        {
                            pickupRequest.NumberOfFirstClassPackagePieces = 1;
                        }
                        else if (Indiciumrequest.Rate.ServiceType == ServiceType.USPM || Indiciumrequest.Rate.ServiceType == ServiceType.USXM)
                        {
                            pickupRequest.NumberOfPriorityMailPieces = 1;
                        }
                        else if (Indiciumrequest.Rate.ServiceType == ServiceType.USEMI || Indiciumrequest.Rate.ServiceType == ServiceType.USPMI)
                        {
                            pickupRequest.NumberOfInternationalPieces = 1;
                        }
                        else if (Indiciumrequest.Rate.ServiceType == ServiceType.USPS)
                        {
                            pickupRequest.NumberOfParcelSelectPieces = 1;
                        }

                        //sending pickup request for the shipment
                        CarrierPickupResponse pickupResponse = null;

                        try
                        {
                           // pickupResponse = soapClient.CarrierPickup(pickupRequest);
                        }
                        catch (Exception e)
                        {
                            throw;
                        }

                        if (pickupResponse != null)
                        {
                            shipmentResponse.DatePickup = pickupResponse.PickupDate;
                        }
                        shipmentResponse.Awb = IndiciumResponse.TrackingNumber;
                        shipmentResponse.PDF = IndiciumResponse.URL;
                        shipmentResponse.CodeShipment = IndiciumResponse.StampsTxID.ToString();
                      
                    }
                }

            //}
           // addShipmentResponseList.Add(shipmentResponse);

            return shipmentResponse;
        }

        private PackageTypeV6 GetPackageType(string packageType, double length, double width, double height, double weight, string serviceType)
        {
            var weightinoz = weight * 16;

            if (serviceType=="First-Class Mail")
            {
                if (length<15 && width<12 && height < 0.75 && weightinoz <13)
                {
                    return PackageTypeV6.LargeEnvelopeorFlat;
                }
                else if (length < 22 && width < 18 && height < 15 && weightinoz < 15.999 && height>0.75)
                {
                    return PackageTypeV6.Package;
                }

            }
            else if (serviceType== "Priority Mail")
            {
                if ((length+(2*(width+height))<=84)&& weight<70)
                {
                    return PackageTypeV6.LargeEnvelopeorFlat;
                }
                else if ((length + (2 * (width + height)) <= 84) && weight < 70)
                {
                    return PackageTypeV6.Package;
                }
                else if ((length + (2 * (width + height)) >84)&& (length + (2 * (width + height)) < 108) && weight < 70)
                {
                    return PackageTypeV6.LargePackage;
                }

            }
            else if (serviceType == "Priority Mail Express")
            {
                if ((length + (2 * (width + height)) <= 108) && weight < 70)
                {
                    return PackageTypeV6.LargeEnvelopeorFlat;
                }
                else if ((length + (2 * (width + height)) <= 108) && weight < 70)
                {
                    return PackageTypeV6.Package;
                }

            }
            else if (serviceType == "Parcel Select Ground")
            {
                if ((length + (2 * (width + height)) <= 84) && weight < 70)
                {
                    return PackageTypeV6.LargeEnvelopeorFlat;
                }
                else if ((length + (2 * (width + height)) <= 84) && weight < 70)
                {
                    return PackageTypeV6.Package;
                }
                else if ((length + (2 * (width + height)) > 84)&& (length + (2 * (width + height)) < 108) && weight < 70)
                {
                    return PackageTypeV6.LargePackage;
                }
                else if ((length + (2 * (width + height)) > 108) && (length + (2 * (width + height)) < 130) && weight < 70)
                {
                    return PackageTypeV6.OversizedPackage;
                }

            }
            else if (serviceType == "First-Class Package International" || serviceType == "First Class International")
            {
                if (length < 15 && width < 12 && height < 0.75 && weight < 4)
                {
                    return PackageTypeV6.LargeEnvelopeorFlat;
                }
                else if ((length +  width + height) < 36 && weight < 4 && length<24)
                {
                    return PackageTypeV6.Package;
                }
            }
            else if(serviceType== "Priority Mail International")
            {
                if ((length + (2 * (width + height)) <= 108) && weight < 70)
                {
                    return PackageTypeV6.LargeEnvelopeorFlat;
                }
                else if ((length + (2 * (width + height)) <= 108) && weight < 70)
                {
                    return PackageTypeV6.Package;
                }
            }
            else if (serviceType == "Priority Mail Express International")
            {
                if ((length + (2 * (width + height)) <= 108) && weight < 70)
                {
                    return PackageTypeV6.LargeEnvelopeorFlat;
                }
                else if ((length + (2 * (width + height)) <= 108) && weight < 70)
                {
                    return PackageTypeV6.Package;
                }
            }
            

            return PackageTypeV6.Unknown;
        }


        private CreateIndiciumRequest getRequest()
        {
            CreateIndiciumRequest request = new CreateIndiciumRequest()
            {

                IntegratorTxID = "234567890ABCDEF",
                Rate = new RateV20()
                {
                    FromZIPCode = "90405",
                    ToZIPCode = "90066",
                    Amount = 1,
                    ServiceType = ServiceType.USPM,
                    DeliverDays = "2",
                    WeightLb = 12,
                    WeightOz = 0,
                    PackageType = PackageTypeV6.Package,
                    ShipDate = DateTime.Now.AddDays(3),
                    InsuredValue = 10,
                    RectangularShaped = false
                },

                From = new Address
                {
                    FullName = "Some Body",
                    Address1 = "3420 Ocean Park Bl",
                    Address2 = "Ste 1000",
                    City = "Santa Monica",
                    State = "CA",
                    ZIPCode = "90405"
                },
                To = new Address
                {
                    FullName = "GEOFF ANTON",
                    Company = "STAMPS.COM",
                    Address1 = "12959 CORAL TREE PL",
                    City = "LOS ANGELES",
                    State = "CA",
                    ZIPCode = "90066",
                    CheckDigit = "6",

                }

            };
            return request;

        }





        //get the stamps service types
        private ServiceType GetServiceType(string serviceTypeString)
        {
            if (serviceTypeString == "First-Class Mail")
            {
                return ServiceType.USFC;
            }
            else if (serviceTypeString == "First-Class Package International")
            {
                return ServiceType.USFCI;
            }
            else if (serviceTypeString == "Priority Mail")
            {
                return ServiceType.USPM;
            }
            else if (serviceTypeString == "Priority Mail Express")
            {
                return ServiceType.USXM;
            }
            else if (serviceTypeString == "Priority Mail Express International")
            {
                return ServiceType.USEMI;
            }
            else if (serviceTypeString == "Priority Mail International")
            {
                return ServiceType.USPMI;
            }
            else if (serviceTypeString == "Parcel Select Ground")
            {
                return ServiceType.USPS;
            }
            else
            {
                return ServiceType.Unknown;
            }


        }



        private string CreateAddShipmentSoapString(ShipmentDto addShipment)
        {

            double weightInLbs = addShipment.PackageDetails.CmLBS == true ? Convert.ToDouble(addShipment.PackageDetails.TotalWeight) * 2.20462 : Convert.ToDouble(addShipment.PackageDetails.TotalWeight);

            StringBuilder addShipmentXml = new StringBuilder();
            addShipmentXml.Append(@"<soap:Envelope xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">");
            addShipmentXml.Append("<soap:Body> <tns:CreateIndicium>");
            addShipmentXml.Append("<tns:Authenticator>" + "" + "</tns:Authenticator>");
            addShipmentXml.Append("<tns:IntegratorTxID>" + addShipment.GeneralInformation.ShipmentReferenceName + "</tns:IntegratorTxID>");

            addShipmentXml.Append("<tns:TrackingNumber/>");
            addShipmentXml.Append("<tns:Rate>");
            addShipmentXml.Append("<tns:FromZIPCode>" + addShipment.AddressInformation.Consigner.Postalcode + "</tns:FromZIPCode>");
            addShipmentXml.Append("<tns:ToZIPCode>" + addShipment.AddressInformation.Consignee.Postalcode + "</tns:ToZIPCode>");
            addShipmentXml.Append("<tns:Amount>10</tns:Amount>");
            addShipmentXml.Append("<tns:ServiceType>US-PM</tns:ServiceType>");
            addShipmentXml.Append("<tns:DeliverDays>1-1</tns:DeliverDays>");
            addShipmentXml.Append("<tns:WeightLb>" + weightInLbs + "</tns:WeightLb>");
            addShipmentXml.Append("<tns:WeightOz>0</tns:WeightOz>");
            addShipmentXml.Append("<tns:PackageType>Package</tns:PackageType>");
            addShipmentXml.Append("<tns:ShipDate>2009-08-31</tns:ShipDate>");
            addShipmentXml.Append("<tns:InsuredValue>" + addShipment.CarrierInformation.Insurance + "</tns:InsuredValue>");
            addShipmentXml.Append("<tns:RectangularShaped>false</tns:RectangularShaped>");
            addShipmentXml.Append("</tns:Rate>");

            addShipmentXml.Append("<tns:From>");
            addShipmentXml.Append("<tns:FullName>" + addShipment.AddressInformation.Consigner.FirstName + " " + addShipment.AddressInformation.Consigner.LastName + "</tns:FullName>");
            addShipmentXml.Append("<tns:Address1>" + addShipment.AddressInformation.Consigner.Address1 + "</tns:Address1>");
            addShipmentXml.Append("<tns:Address2>" + addShipment.AddressInformation.Consigner.Address2 + "</tns:Address2>");
            addShipmentXml.Append("<tns:City>" + addShipment.AddressInformation.Consigner.City + "</tns:City>");
            addShipmentXml.Append("<tns:State>" + addShipment.AddressInformation.Consigner.State + "</tns:State>");
            addShipmentXml.Append("<tns:ZIPCode>" + addShipment.AddressInformation.Consigner.Postalcode + "</tns:ZIPCode>");
            addShipmentXml.Append("<tns:PhoneNumber>" + addShipment.AddressInformation.Consigner.ContactNumber + "<tns:PhoneNumber/>");
            addShipmentXml.Append("</tns:From>");


            addShipmentXml.Append("<tns:To>");
            addShipmentXml.Append("<tns:FullName>" + addShipment.AddressInformation.Consignee.FirstName + " " + addShipment.AddressInformation.Consignee.LastName + "</tns:FullName>");

            addShipmentXml.Append("<tns:Company>" + addShipment.AddressInformation.Consignee.CompanyName + "</tns:Company>");
            addShipmentXml.Append("<tns:Address1>" + addShipment.AddressInformation.Consignee.Address1 + "</tns:Address1>");
            addShipmentXml.Append("<tns:Address2>" + addShipment.AddressInformation.Consignee.Address2 + "  <tns:Address2/>");
            addShipmentXml.Append("<tns:City>" + addShipment.AddressInformation.Consignee.City + "</tns:City>");
            addShipmentXml.Append("<tns:State>" + addShipment.AddressInformation.Consignee.State + "</tns:State>");
            addShipmentXml.Append("<tns:ZIPCode>" + addShipment.AddressInformation.Consignee.Postalcode + "</tns:ZIPCode>");
            addShipmentXml.Append("<tns:Country>" + addShipment.AddressInformation.Consignee.City + "</tns:Country>");
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
                addShipmentXml.Append(" <Description>" + lineItem.Description + "</Description>");
                addShipmentXml.Append("<Quantity>" + lineItem.Quantity + "</Quantity>");
                // addShipmentXml.Append(" <Value></Value>");
                addShipmentXml.Append("<WeightLb>" + weight.ToString() + "</WeightLb>");
                addShipmentXml.Append("<HSTariffNumber>" + addShipment.CarrierInformation.tariffText + "</HSTariffNumber>");
                addShipmentXml.Append(" </CustomsLine>");

            }
            addShipmentXml.Append(" <CountryOfOrigin>" + addShipment.AddressInformation.Consigner.Country + "</CountryOfOrigin>");
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
            AuthorizeRequest.Append("<tns:Credentials> <tns:IntegrationID>" + StampsComIntegrationId + "</tns:IntegrationID>");
            AuthorizeRequest.Append("<tns:Username>" + StampsComUserName + "</tns:Username>");
            AuthorizeRequest.Append("<tns:Password>" + StampsComPassword + "</tns:Password>");
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
