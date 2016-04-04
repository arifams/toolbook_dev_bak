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
        Exception
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
        Warning
    }
}
