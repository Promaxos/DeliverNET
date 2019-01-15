using System;
using System.Collections.Generic;
using DeliverNET.Data;
using DeliverNET.Managers.Models;

namespace DeliverNET.Managers.Interfaces
{
    public interface IOrderManager
    {
        // Create
        bool Create(Order order);

        // Read
        List<Order> GetAll();
        Order Get(int id);
        Order Get(DateTime tStamp);
        List<Order> Get(Business business);
        List<Order> Get(Deliverer deliverer);
        List<Order> GetActive();
        List<Order> GetActive(int businessId);
        
        // Update
        bool UpdateAddress(int id, string newAddress);
        bool UpdateReceiver(int id, OrderReceiverInfoModel newReceiverInfo);
        bool SetIsAccepted(int id, bool status);
        bool SetIsPickedUp(int id, bool status);
        bool SetIsDelivered(int id, bool status);
        bool SetAcceptedTime(int id,DateTime time);
        bool SetPickupTime(int id,DateTime time);
        bool SetDeliveredTime(int id,DateTime time);
        bool SetDeliverer(int id, Deliverer deliverer);
        bool SetTimeoutState(int id, bool status);

        // Delete
        bool DeleteAll();
        bool Delete(int id);
    }
}
