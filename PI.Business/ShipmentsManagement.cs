using PI.Contract.Business;
using PI.Contract.DTOs.RateSheets;
using PI.Contract.DTOs.Shipment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Business
{
    public class ShipmentsManagement
    {
        public ShipmentcostList GetRateSheet(ShipmentDto currentShipment)
        {
            SISIntegrationManager sisManager = new SISIntegrationManager();
            RateSheetParametersDto currentRateSheetDetails = new RateSheetParametersDto();

            if (currentShipment==null)
            {
                return null;
            }
            if (currentShipment.GeneralInformation!=null)
            {
                //  currentRateSheetDetails.type = currentShipment.GeneralInformation.shipmentType;
                // currentRateSheetDetails.
                if (currentShipment.GeneralInformation.Express)
                {
                    currentRateSheetDetails.courier = "UPSDHLFEDTNT";
                }
                else if (currentShipment.GeneralInformation.AirFreight)
                {
                    currentRateSheetDetails.courier_air = "EME";
                }
                else if (currentShipment.GeneralInformation.SeaFreight)
                {
                    currentRateSheetDetails.courier_sea = "EME";
                }
                else if (currentShipment.GeneralInformation.RoadFreight)
                {
                    currentRateSheetDetails.courier_road = "EME";
                }
                else
                {
                    //select all shipment modes
                    currentRateSheetDetails.courier = "UPSDHLFEDTNT";
                    currentRateSheetDetails.courier_air = "EME";
                    currentRateSheetDetails.courier_sea = "EME";
                    currentRateSheetDetails.courier_road = "EME";
                }
            }
            if (currentShipment.AddressInformation!=null)
            {
                //consigner details
                currentRateSheetDetails.address1 = currentShipment.AddressInformation.Consigner.Name.Replace(' ', '%');
                currentRateSheetDetails.address2 = currentShipment.AddressInformation.Consigner.Address1.Replace(' ','%');
                currentRateSheetDetails.address3 = currentShipment.AddressInformation.Consigner.Address2.Replace(' ', '%');
                currentRateSheetDetails.address4 = currentShipment.AddressInformation.Consigner.City.Replace(' ', '%');
                currentRateSheetDetails.street_number_delivery = currentShipment.AddressInformation.Consigner.Number;
                currentRateSheetDetails.postcode_delivery = currentShipment.AddressInformation.Consigner.Postalcode;
                currentRateSheetDetails.country_from = currentShipment.AddressInformation.Consigner.Country;
                currentRateSheetDetails.code_country_from = currentShipment.AddressInformation.Consigner.Country;
           
                //consignee details
                currentRateSheetDetails.address11 = currentShipment.AddressInformation.Consignee.Name.Replace(' ', '%');
                currentRateSheetDetails.address12 = currentShipment.AddressInformation.Consignee.Address1.Replace(' ', '%');
                currentRateSheetDetails.address13 = currentShipment.AddressInformation.Consignee.Address2.Replace(' ', '%');
                currentRateSheetDetails.address14 = currentShipment.AddressInformation.Consignee.City.Replace(' ', '%');
                currentRateSheetDetails.street_number_delivery = currentShipment.AddressInformation.Consignee.Number;
                currentRateSheetDetails.postcode_delivery = currentShipment.AddressInformation.Consignee.Postalcode;
                currentRateSheetDetails.country_to = currentShipment.AddressInformation.Consignee.Country;
                currentRateSheetDetails.code_country_to = currentShipment.AddressInformation.Consignee.Country;


            }
            if (currentShipment.PackageDetails!=null)
            {
                double maxLength = 0;
                double maxWidth = 0;
                double maxHeight = 0;
                double surface = 0;
                double maxdimension = 0;
                double pieces = 0;
                double volume = 0;
                double maxVolume = 0;
                double weight = 0;
                double maxWeight = 0;
                string package = string.Empty;
                int count = 0;
                bool diversed = false;

                foreach (var item in currentShipment.PackageDetails.ProductIngredients)
                {
                    if (count==0)
                    {
                        package = item.ProductType;
                    }
                    if (count>0 && package!=item.ProductType && !diversed)
                    {
                        package = "DIVERSE";
                        diversed = true;
                    }

                    if (item.Length>maxLength)
                    {
                        maxLength = item.Length;
                    }
                    if (item.Width>maxWidth)
                    {
                        maxWidth = item.Width;
                    }
                    if (item.Height>maxHeight)
                    {
                        maxHeight = item.Height;
                    }
                    if (item.Weight>maxWeight)
                    {
                        maxWeight = item.Weight;
                    }
                   

                    surface = surface +(item.Length * item.Width * item.Quantity);
                    pieces = pieces + item.Quantity;
                    volume = volume+ item.Length * item.Width * item.Height * item.Quantity;
                    weight = weight+ item.Weight * item.Quantity;
                    count++;
                }
                maxdimension = maxLength + (maxWidth * 2) + (maxHeight * 2);
                maxVolume = maxLength * maxWidth * maxHeight;

                currentRateSheetDetails.length = maxLength.ToString();
                currentRateSheetDetails.width = maxWidth.ToString();
                currentRateSheetDetails.height = maxHeight.ToString();
                currentRateSheetDetails.max_length= maxLength.ToString();
                currentRateSheetDetails.max_actual_length = maxLength.ToString();
                currentRateSheetDetails.max_width= maxWidth.ToString();
                currentRateSheetDetails.max_height= maxHeight.ToString();
                currentRateSheetDetails.max_weight = maxWeight.ToString();
                currentRateSheetDetails.weight = weight.ToString();
                currentRateSheetDetails.pieces = pieces.ToString();
                currentRateSheetDetails.surface = surface.ToString();
                currentRateSheetDetails.max_dimension = maxdimension.ToString();
                currentRateSheetDetails.volume = volume.ToString();
                currentRateSheetDetails.max_volume = maxVolume.ToString();
                currentRateSheetDetails.value = currentShipment.PackageDetails.DeclaredValue;
                currentRateSheetDetails.package = package;


            }
            //hardcoded values for now

            currentRateSheetDetails.userid = "User@Mitrai.com";
            currentRateSheetDetails.password = "Mitrai462";
            currentRateSheetDetails.output = "XML";
            currentRateSheetDetails.type_xml = "LIST";
            currentRateSheetDetails.vat = "NO";
            currentRateSheetDetails.default_off = "YES";
            currentRateSheetDetails.type = "selectkmnetworkroad";
            currentRateSheetDetails.fieldname4 = "shipment_price";
            currentRateSheetDetails.fieldname1 = "price";
            currentRateSheetDetails.sell_buy = "";          
            currentRateSheetDetails.courier_km = "";     
            currentRateSheetDetails.courier_tariff_base = "";           
            currentRateSheetDetails.courier_date_pickup_transition = "";
            currentRateSheetDetails.language = "EN";
            currentRateSheetDetails.print_button = "";
            currentRateSheetDetails.country_distance = "";
            currentRateSheetDetails.courier_tariff_type = "NLPARUPS:NLPARFED:USPARDHL2:USPARTNT:USPARUPS:USPARFED2:USUPSTNT:USPAREME:USPARPAE";
            
            currentRateSheetDetails.code_currency = "USD";
           
            currentRateSheetDetails.date_pickup = "10-Mar-2016 00:00";//preferredCollectionDate
            currentRateSheetDetails.time_pickup = "12:51";
            currentRateSheetDetails.date_delivery_request = "25-Mar-2016 00:00";
            currentRateSheetDetails.delivery_condition = "DD-DDU-PP";
            currentRateSheetDetails.weight_unit = "kg";
           currentRateSheetDetails.insurance_instruction = "N";
           currentRateSheetDetails.sort = "PRICE";
           currentRateSheetDetails.volume_unit = "cm";
           currentRateSheetDetails.inbound = "N"; 
           currentRateSheetDetails.dg = "NO";
           currentRateSheetDetails.dg_type = "";
           currentRateSheetDetails.account = "";
           currentRateSheetDetails.code_customer = "";
           currentRateSheetDetails.ind_delivery_inside = "";
           currentRateSheetDetails.url = " www2.shipitsmarter.com/taleus/";



            return sisManager.GetRateSheetForShipment(currentRateSheetDetails);

        }

        public string SubmitShipment(ShipmentDto addShipment)
        {
            ICarrierIntegrationManager sisManager = new SISIntegrationManager();
           
            return sisManager.SubmitShipment(addShipment);
        }
      

        
    }
}
