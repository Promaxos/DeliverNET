using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DeliverNET.Data
{
    public class Order
    {
        public int Id { get; set; }

        public int BusinessId { get; set; }
        public Business Business { get; set; }

        public string CashierId { get; set; }
        public BusinessCashier Cashier { get; set; }

        public string DelivererId { get; set; }
        public Deliverer Deliverer { get; set; }


        public DateTime Tstamp { get; set; }

        public string Address { get; set; }
        public string Geolocation { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int? FloorNo { get; set; }
        public string DoorName { get; set; }
        public string PhoneNumber { get; set; }

        public int PaymentTypeId { get; set; }
        public float? Tariff { get; set; }
        public float? Price { get; set; }
        public string Comments { get; set; }
        public DateTime? AcceptedTime { get; set; }
        public DateTime? PickupTime { get; set; }
        public DateTime? DeliveredTime { get; set; }
        public bool IsAccepted { get; set; }
        public bool IsPickedup { get; set; }
        public bool IsDelivered { get; set; }

        public bool IsTimedOut { get; set; }
    }


}
