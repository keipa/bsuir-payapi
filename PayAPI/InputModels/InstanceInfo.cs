using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PayAPI.InputModels
{
    public class InstanceInfo
    {
        public string CardId { get; set; }

        public string CardholderName { get; set; }

        public string DeviceHash { get; set; }

    }
}