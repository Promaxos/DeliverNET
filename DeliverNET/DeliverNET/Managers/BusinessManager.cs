using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DeliverNET.Data;
using DeliverNET.Managers.Interfaces;
using DeliverNET.Models;

namespace DeliverNET.Managers
{
    public class BusinessManager : IBusinessManager
    {
        private DeliverNETContext _db;
        public BusinessManager(DeliverNETContext db) => _db = db;

        public bool Create(Business business)
        {
            try
            {
                _db.Businesses.Add(business);
                _db.SaveChanges();
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        public bool DeleteAll()
        {
            List<Business> businesses = _db.Businesses.ToList();
            foreach (Business b in businesses)
            {
                _db.Businesses.Remove(b);
            }

            _db.SaveChanges();
            return true;
        }

        public Business Get(int id)
        {
            // TODO try/catch
            return _db.Businesses.Find(id);
        }

        public Business Get(string title)
        {
            // TODO try/catch
            return _db.Businesses.Where(b=>b.Title == title).FirstOrDefault();
        }

        public List<Business> GetAll()
        {
            return _db.Businesses.ToList();
        }

        public bool SetAddress(int id, string address)
        {
            try
            {
                Business business = _db.Businesses.Find(id);
                business.Address = address;
                _db.SaveChanges();
            }
            catch (Exception e)
            {
                throw e;
            }
            return true;
        }

        public bool SetCredentials(int id, string credentials)
        {
            try
            {
                Business business = _db.Businesses.Find(id);
                business.Credentials = credentials;
                _db.SaveChanges();
            }
            catch (Exception e)
            {
                throw e;
            }
            return true;
        }

        public bool SetPhoneNumber(int id, string phoneNumber)
        {
            try
            {
                Business business = _db.Businesses.Find(id);
                business.PhoneNumber = phoneNumber;
                _db.SaveChanges();
            }
            catch (Exception e)
            {
                throw e;
            }
            return true;
        }

        public bool SetTitle(int id, string title)
        {
            try
            {
                Business business = _db.Businesses.Find(id);
                business.Title = title;
                _db.SaveChanges();
            }
            catch (Exception e)
            {
                throw e;
            }
            return true;
        }
    }
}
