﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaxiBookingService.Common;

namespace TaxiBookingServices.API.Service_Contract
{
    public class LoginResponseDTO : ResponseBase
    {
        public string AccessToken { get; set; }
    }
}
