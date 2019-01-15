using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DeliverNET.Data;

namespace DeliverNET.Managers.Interfaces
{
    public interface IDelivererManager
    {
        // Create
        bool Create(DeliverNETUser user);

        // Read
        Deliverer Get(string id);
        string GetGeolocation(string id);
        bool GetDeliveringStatus(string id);
        bool GetWorkingStatus(string id);
        string GetCredentials(string id);

        // Update
        bool SetWorkingStatus(string id, bool isWorking);
        bool SetDeliveringStatus(string id, bool isDelivering);
        bool SetGeolocation(string id, string geolocation);
        bool SetCredentials(string id, string credentials);
        bool SetOperatingRegion(string id, string operatingRegion);

        // Delete
        bool DeleteAll();
    }
}
