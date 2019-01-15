using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DeliverNET.Models.ProfileDummies
{
    public class IndiIndixDummyModel
    {
        // TODO :: Εδώ είναι όλα τα dummies που χρησιμοποιεί το Front του IndiIndex μόνο, μαζεμένα όσα
        //         χρειάζονται. Έχω πάρει copy-paste τα props από το model των Orders, επειδή στο view δεν μπορώ
        //         να εισάγω πάνω από ένα μοντέλο με το @modelκτλ. Υπάρχουν αρκετοί τρόποι να γίνει, αλλά όλοι
        //         κάνουν interfere με το back και θα πρέπει να πειράξω πολλά. Leave it to you, χώρισέ τα όπως θες.


        /* DELIVERER PROPERTIES */
        public bool IsWorking { get; set; }
        public string Username { get; set; }  // or ID, whatever
        public int ActiveOrders; // only for our deliverer
        public bool IsVerified { get; set; }


        /* BUSINESSMAN PROPERTIES */
        public string RestaurantName { get; set; }
        public string RestaurantAddress { get; set; }


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
        public float Stopwatch { get; set; } // fuck fuck fuck fuck fuck fuck fuck fuck fuck fuck fuck fuck fuck fuck fuck fuck fuck fuck 


    }
}
