﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Contract.DTOs.AccountSettings
{
    public class AccountSettingsDto
    {
        public long CustomerId { get; set; }
        public short DefaultLanguageId { get; set; }
        public short DefaultCurrencyId { get; set; }
        public short DefaultTimeZoneId { get; set; }

        public short DefaultVolumeMetricId { get; set; }
        public short DefaultWeightMetricId { get; set; }

        public List<LanguageDto> Languages { get; set; }

        public List<CurrencyDto> Currencies { get; set; }

        public List<TimeZoneDto> TimeZones { get; set; }

    }
}
