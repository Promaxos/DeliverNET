using DeliverNET.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DeliverNET.Data
{
    public class Deliverer
    {
        public string Credentials { get; set; }
        public bool IsWorking { get; set; }
        public bool IsDelivering { get; set; }
        public string OperationalRegion { get; set; }
        public string Geolocation { get; set; }

        // one-to-one with DeliverNETUser
        public string DeliverNetUserId { get; set; }
        public DeliverNETUser DeliverNETUser { get; set; }
    }
}
