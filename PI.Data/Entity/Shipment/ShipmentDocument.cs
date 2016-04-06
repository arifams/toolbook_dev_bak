using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Data.Entity
{
    public class ShipmentDocument : LongIdBaseEntity
    {
        public long TenantId { get; set; }

        public long ShipmentId { get; set; }

        public string ClientFileName { get; set; }

        public string UploadedFileName { get; set; }

    }
}
