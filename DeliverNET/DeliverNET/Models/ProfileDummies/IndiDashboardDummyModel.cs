using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DeliverNET.Models.ProfileDummies
{
    public class IndiDashboardDummyModel
    {
        public float totalProfit { get; set; }
        public float profitMinusTips { get; set; }
        public int totalOrders { get; set; }
        public float totalHoursWorking { get; set; }
        public float totalDaysWorking { get; set; }
        public int[] ordersCounterPerDayActive { get; set; }  // each list item represents a day passed
        public float tips { get; set; }
        public List<List<int>> ordersCounterPerMonthPerDay { get; set; }
        public List<int> ordersCounterPerDayType { get; set; }  // starting from list[0] as Sunday, each slot in list represents a day


    }
}
