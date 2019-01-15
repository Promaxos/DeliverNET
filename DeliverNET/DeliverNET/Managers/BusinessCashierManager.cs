using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DeliverNET.Data;
using DeliverNET.Managers.Interfaces;
using DeliverNET.Models;

namespace DeliverNET.Managers
{
    public class BusinessCashierManager : IBusinessCashierManager
    {
        private DeliverNETContext _db;
        public BusinessCashierManager(DeliverNETContext db) => _db = db;


        public bool Create(DeliverNETUser user, Business business)
        {
            try
            {
                _db.BusinessCashiers.Add(new BusinessCashier()
                {
                    DeliverNetUserId = user.Id,
                    BusinessId = business.Id
                });
                _db.SaveChanges();
            }
            catch (Exception e)
            {
                throw e;
            }

            return true;
        }

        public BusinessCashier Get(string id)
        {
            BusinessCashier cashier;
            try
            {
                cashier = _db.BusinessCashiers.Find(id);
            }
            catch (Exception e)
            {
                throw e;
            }

            return cashier;
        }

        public bool Delete(string id)
        {
            BusinessCashier cashier;
            try
            {
                cashier = _db.BusinessCashiers.Find(id);
                _db.BusinessCashiers.Remove(cashier);
                _db.SaveChanges();
            }
            catch (Exception e)
            {
                throw e;
            }

            return true;
        }

        public bool DeleteAll()
        {
            List<BusinessCashier> cashiers = _db.BusinessCashiers.ToList();
            foreach (BusinessCashier c in cashiers)
            {
                _db.BusinessCashiers.Remove(c);
            }

            _db.SaveChanges();
            return true;
        }
    }
}
