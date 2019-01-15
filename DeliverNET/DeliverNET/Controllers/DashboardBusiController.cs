using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DeliverNET.Controllers
{
    [Authorize(Policy = "OwnerOnly")]
    public class DashboardBusiController : Controller
    {
        // Dummy slave dashboard data
        private static Models.ProfileDummies.BusiDashboardDummyModel Dashboard_Dummy = new Models.ProfileDummies.BusiDashboardDummyModel
        {
            expensesSaved = 132,
            totalIncome = 684,
            totalOrders = 80,
            totalOrdersSucceeded = 77,
            totalHoursWorking = 90,
            totalDaysWorking = 13,
            ordersCounterPerDayActive = new int[] { 2, 5, 3, 10, 4, 3, 6, 5, 2, 15, 14, 6, 5 },
            ordersCounterPerDayType = new List<int> { 24, 10, 8, 6, 7, 7, 18 }
        };

        public IActionResult OverviewBusi()
        {
            return View(Dashboard_Dummy);
        }

        public IActionResult OrdersBusi()
        {
            return View(Dashboard_Dummy);
        }

        public IActionResult ProfitBusi()
        {
            return View(Dashboard_Dummy);
        }
    }
}