using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DeliverNET.Models.ProfileDummies
{
    public class BusiIndixDummyModel
    {
        /* DELIVERER PROPERTIES */
        public bool IsWorking { get; set; }
        public string DelivererUsername { get; set; }  // or ID, whatever


        /* BUSINESSMAN PROPERTIES */
        public string RestaurantName { get; set; }
        public string RestaurantAddress { get; set; }
        public int NumberOfOrders { get; set; }
        public bool IsVerified { get; set; }
        public bool IsCashier { get; set; }



        /* ORDER PROPERTIES */
        public int OrderID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public int FloorNo { get; set; }
        public string DoorName { get; set; }
        public int PaymentTypeId { get; set; }
        public string State { get; set; } // 1.Pending | 2.Accepted | 3.Picked | 4.Delivered | 5.Failed  --> I think Enum
        public float DistanceToShop { get; set; } // Deliverer - Shop
        public float ETA { get; set; } // Deliverer - Shop
        public float Price { get; set; }
        public float CountWatch { get; set; } // fuck fuck fuck fuck fuck fuck fuck fuck fuck fuck fuck fuck fuck fuck fuck fuck fuck fuck
        public DateTime Datetime { get; set; }
    }
}
