using DeliverNET.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DeliverNET.Data
{
    public class BusinessOwner
    {
        // one-to-one with DeliverNETUser
        public string DeliverNetUserId { get; set; }
        public DeliverNETUser DeliverNETUser { get; set; }
       
        //one-to-one with business
        public int BusinessId { get; set; }
        public Business Business { get; set; }
    }
}
