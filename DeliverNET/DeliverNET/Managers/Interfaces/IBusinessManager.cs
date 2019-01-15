using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DeliverNET.Data;

namespace DeliverNET.Managers.Interfaces
{
    public interface IBusinessManager
    {
        // Create
        bool Create(Business business);

        // Read
        Business Get(int id);
        Business Get(string title);
        List<Business> GetAll();

        // Update
        bool SetTitle(int id, string title);
        bool SetAddress(int id, string address);
        bool SetPhoneNumber(int id, string phoneNumber);
        bool SetCredentials(int id, string credentials);

        // Delete
        bool DeleteAll();
    }
}
