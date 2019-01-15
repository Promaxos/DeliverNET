
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DeliverNET.Data;
using DeliverNET.Managers.Interfaces;
using DeliverNET.Managers.Models;
using DeliverNET.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace DeliverNET.Managers
{
    public class OrderManager : IOrderManager, IDisposable
    {
        private DeliverNETContext _db;
        public OrderManager(DeliverNETContext db) => _db = db;

        // Create
        public bool Create(Order order)
        {
            try
            {
                _db.Orders.Add(order);
                _db.SaveChanges();
            }
            catch (Exception e)
            {
                throw e;
            }

            return true;
        }



        // Read
        public List<Order> GetAll()
        {
            return _db.Orders.ToList();
        }

        public Order Get(int id)
        {
            Order order;
            try
            {
                order = _db.Orders.Find(id);
            }
            catch (Exception e)
            {
                throw e;
            }
            return order;
        }

        public Order Get(DateTime tStamp)
        {
            Order order;
            try
            {
                order = _db.Orders.Where(x => x.Tstamp == tStamp).FirstOrDefault();
            }
            catch (Exception e)
            {
                throw e;
            }
            return order;
        }

        public List<Order> Get(Business business)
        {
            List<Order> orders;
            try
            {
                orders = _db.Orders.Where(o => o.Business.Id == business.Id).ToList();
            }
            catch (Exception e)
            {
                throw e;
            }

            return orders;
        }

        public List<Order> Get(Deliverer deliverer)
        {
            List<Order> orders;
            try
            {
                orders = _db.Orders.Where(o => o.Deliverer.DeliverNetUserId == deliverer.DeliverNetUserId).ToList();
            }
            catch (Exception e)
            {
                throw e;
            }

            return orders;
        }

        public List<Order> GetActive()
        {
            List<Order> orders;
            try
            {
                orders = _db.Orders.Where(o => o.IsTimedOut == false).ToList();
            }
            catch (Exception e)
            {
                throw e;
            }

            return orders;
        }
        
        public List<Order> GetActive(int businessId)
        {
            List<Order> orders;
            try
            {
                orders = _db.Orders
                    .Where(o => o.IsTimedOut == false && o.BusinessId == businessId)
                    .ToList();
            }
            catch (Exception e)
            {
                throw e;
            }

            return orders;
        }

        

        // Update
        public bool UpdateAddress(int id, string newAddress)
        {
            Order order = _db.Orders.Find(id);           
            try
            {
                order.Address = newAddress;
                _db.SaveChanges();
            }
            catch (Exception e)
            {
                throw e;
            }

            return true;
        }

        public bool UpdateReceiver(int id, OrderReceiverInfoModel newReceiverInfo)
        {
            Order order = _db.Orders.Find(id);
            if (order != null)
            {
                try
                {
                    order.FirstName = newReceiverInfo.FirstName;
                    order.LastName = newReceiverInfo.LastName;
                    order.FloorNo = newReceiverInfo.FloorNo;
                    order.DoorName = newReceiverInfo.DoorName;
                    _db.SaveChanges();
                }
                catch (Exception e)
                {
                    throw e;
                }

                return true;
            }

            return false;
        }

        public bool SetIsAccepted(int id, bool status)
        {
            Order order = _db.Orders.Find(id);
            try
            {
                order.IsAccepted = status;
                _db.SaveChanges();
            }
            catch (Exception e)
            {
                throw e;
            }
            return true;
        } 
        
        public bool SetIsPickedUp(int id, bool status)
        {
            Order order = _db.Orders.Find(id);
            try
            {
                order.IsPickedup = status;
                _db.SaveChanges();
            }
            catch (Exception e)
            {
                throw e;
            }
            return true;
        }

        public bool SetIsDelivered(int id,bool status)
        {
            Order order = _db.Orders.Find(id);
            try
            {
                order.IsDelivered = status;
                _db.SaveChanges();
            }
            catch (Exception e)
            { 
                throw e;
            }
            return true;
        }

        public bool SetAcceptedTime(int orderId,DateTime time)
        {
            Order order = _db.Orders.Find(orderId);
            try
            {
                order.AcceptedTime = time;
                _db.SaveChanges();
            }
            catch (Exception e)
            {
                throw e;
            }
            return true;
        }

        public bool SetPickupTime(int orderId, DateTime time)
        {
            Order order = _db.Orders.Find(orderId);
            try
            {
                order.PickupTime = time;
                _db.SaveChanges();
            }
            catch (Exception e)
            {
                throw e;
            }
            return true;
        }

        public bool SetDeliveredTime(int orderId, DateTime time)
        {
            Order order = _db.Orders.Find(orderId);
            try
            {
                order.DeliveredTime = time;
                _db.SaveChanges();
            }
            catch (Exception e)
            {
                throw e;
            }
            return true;
        }

        public bool SetDeliverer(int orderId, Deliverer deliverer)
        {
            Order order = _db.Orders.Find(orderId);
            try
            {
                order.Deliverer = deliverer;
                _db.SaveChanges();
            }
            catch (Exception e)
            {
                throw e;
            }
            return true;
        }

        public bool SetTimeoutState(int id, bool status)
        {
            Order order = _db.Orders.Find(id);
            try
            {
                order.IsTimedOut = status;
                _db.SaveChanges();
            }
            catch (Exception e)
            {
                throw e;
            }
            return true;
        }

        // Delete
        // TODO Remove this method
        public bool DeleteAll()
        {
            List<Order> orders = _db.Orders.ToList();
            foreach (Order o in orders)
            {
                _db.Orders.Remove(o);
            }

            _db.SaveChanges();


            return true;
        }

        public bool Delete(int id)
        {
            Order order = Get(id);
            if (order != null)
            {
                try
                {
                    _db.Orders.Remove(order);
                    _db.SaveChanges();
                }
                catch (Exception e)
                {
                    throw e;
                }

                return true;
            }

            return false;
        }



        #region "Dispose Pattern"
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_db != null)
                {
                    _db.Dispose();
                }
            }
        }
        #endregion
    }
}
