using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Contract.Enums
{
    public class Enums
    {
        
    }

    public enum ApplicationTypes
    {
        JavaScript = 0,
        NativeConfidential = 1
    };

    public enum ProductType : short
    {
        Box = 1,
        Document,
        Pallet,
        [Description("Euro Pallet")]
        EuroPallet,
        Diverse
    }

    public enum ShipmentService : short
    {
        [Description("DD-DDP-PP")]
        DDDDPPP = 1,
        [Description("DD-DDU-PP")]
        DDDDUPP,
        [Description("DD-CIP-PP")]
        DDCIPPP,
        [Description("DP-CIP-PP")]
        DPCIPPP,
        [Description("DP-CPT-PP")]
        DPCPTPP,
        [Description("PD-CPT-PP")]
        PDCPTPP,
        [Description("PD-CIP-PP")]
        PDCIPPP,
        [Description("PP-CPT-PP")]
        PPCPTPP,
        [Description("PP-CIP-PP")]
        PPCIPPP,
        [Description("DP-FCA-CC")]
        DPFCACC,
        [Description("DF-EXW-CC")]
        DFEXWCC,
        [Description("KMSDY")]
        KMSDY
    }

    public enum ShipmentStatus : short
    {
        None = 0,

        [Description("Booking error")]
        Error = 1,

        [Description("Booking error")]
        Pending,

        [Description("Booking confirmed")]
        BookingConfirmation,

        [Description("Picked up")]
        Pickup,

        [Description("In transit")]
        Transit,

        [Description("Out for delivery")]
        OutForDelivery,

        Delivered,
        Deleted,
        Exception,
        Claim,
        Draft,

        [Description("Booking Processing")]
        Processing
    }


    public enum ShipmentStatusEP : short
    {
        [Description("Pre Transit")]
        pre_transit=1,
        [Description("In Transit")]
        in_transit,
        [Description("Out for Delivry")]
        out_for_delivery,
        [Description("Delivered")]
        delivered,
        [Description("Return to Sender")]
        return_to_sender,
        [Description("Failure")]
        failure,
        [Description("Unknown")]
        unknown

    }

    public enum ShipmentPaymentType : short
    {
        [Description("Invoice")]
        Invoice = 1,
        [Description("Online")]
        Online
    }

    public enum Status : short
    {
        Error = 1,
        Success,
        Warning,
        PaymentError,
        SISError,
        PostmenError,
        Processing,
        Refund
    }

    public enum DocumentType : short
    {
        [Description("SHIPMENT_DOCUMENTS")]
        Shipment = 1,
        [Description("SHIPMENT_LABEL")]
        ShipmentLabel = 2,
        [Description("AddressBook")]
        AddressBook = 3,
        [Description("RATE_SHEET")]
        RateSheet = 4,
        [Description("INVOICE")]
        Invoice = 5,
        [Description("CREDIT_NOTE")]
        CreditNote=6,
        [Description("LOGO")]
        Logo=7,

    }

    public enum CurrencyType : short
    {
        [Description("$")]
        USD = 1,
        [Description("€")]
        EURO,
        [Description("¥")]
        YEN//,
        //Pound
    }

    public enum CarrierType : short
    {
        All = 0,
        Express = 1,
        [Description("Air Freight")]
        AirFreight,
        [Description("Sea Freight")]
        SeaFreight,
        [Description("Road Freight")]
        RoadFreight
    }

    public enum RatesCalculationMethod : short
    {
        WEIGHT = 1,
        KG
    }

    public enum RatesSell : short
    {
        Sell =1,
        Buy
    }

    public enum ReportType : short
    {
        Excel = 1,
        CSV
    }

    public enum AppFunctionality : short
    {
        UserRegistration = 1,
        UserLogin,        
        AddShipment,
        EditShipment,
        AddUser,
        EditUser,
        UserManagement
    }

    public enum InvoiceStatus : short
    {
        None = 0,
        Pending=1,
        Paid,
        Disputed

    }

    public enum PaymentType : short
    {
        Shipment = 1,
        Invoice = 2
    }

}
