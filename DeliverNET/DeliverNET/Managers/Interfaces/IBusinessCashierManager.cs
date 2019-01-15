using DeliverNET.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DeliverNET.Managers.Interfaces
{
    public interface IBusinessCashierManager
    {
        // Create
        bool Create(DeliverNETUser user, Business business);

        // Read
        BusinessCashier Get(string id);

        // Update

        // Delete
        bool DeleteAll();
        bool Delete(string id);
    }
}
