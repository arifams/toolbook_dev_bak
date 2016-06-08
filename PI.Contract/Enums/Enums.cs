﻿using System;
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
        Error = 1,
        Pending,
        [Description("Booking confirmation")]
        BookingConfirmation,
        Pickup,
        Transit,
        [Description("Out for delivery")]
        OutForDelivery,
        Delivered,
        Deleted,
        Exception,
        Claim
    }

    public enum ShipmentPaymentType : short
    {
        Invoice = 1,
        PayLane
    }

    public enum Status : short
    {
        Error = 1,
        Success,
        Warning,
        PaymentError,
        SISError
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
        USD = 1,
        EURO,
        YEN,
        Pound
    }

    public enum CarrierType : short
    {
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

}
