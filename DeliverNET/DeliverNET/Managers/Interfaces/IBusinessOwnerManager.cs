using DeliverNET.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DeliverNET.Managers.Interfaces
{
    public interface IBusinessOwnerManager
    {
        bool Create(string userId);
        bool Create(string userId, int businessId);

        BusinessOwner Get(string id);

    }
}
