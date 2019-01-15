using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DeliverNET.Data;
using DeliverNET.Managers.Interfaces;
using DeliverNET.Models;

namespace DeliverNET.Managers
{
    public class BusinessOwnerManager : IBusinessOwnerManager
    {
        private readonly DeliverNETContext _db;

        public BusinessOwnerManager(DeliverNETContext db) => _db = db;


        public bool Create(string userId)
        {
            try
            {
                _db.BusinessOwners.Add(new BusinessOwner
                {
                    DeliverNetUserId = userId
                });
                _db.SaveChanges();
            }
            catch (Exception e)
            {
                throw e;
            }
            return true;
        }

        public bool Create(string userId, int businessId)
        {
            try
            {
                _db.BusinessOwners.Add(new BusinessOwner
                {
                    DeliverNetUserId = userId,
                    BusinessId = businessId
                });
                _db.SaveChanges();
            }
            catch (Exception e)
            {
                throw e;
            }
            return true;
        }

        public BusinessOwner Get(string id)
        {
            BusinessOwner owner;
            try
            {
                owner = _db.BusinessOwners.Find(id);
            }
            catch (Exception e)
            {
                throw e;
            }
            return owner;
        }
    }
}
