using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DeliverNET.Data;
using DeliverNET.Managers.Interfaces;
using DeliverNET.Models;

namespace DeliverNET.Managers
{
    public class DelivererManager : IDelivererManager, IDisposable
    {
        private DeliverNETContext _db;
        public DelivererManager(DeliverNETContext db) => _db = db;

        public bool Create(DeliverNETUser user)
        {
            try
            {
                _db.Deliverers.Add(new Deliverer()
                {
                    Credentials = string.Empty,
                    IsWorking = false,
                    IsDelivering = false,
                    OperationalRegion = string.Empty,
                    Geolocation = string.Empty,
                    DeliverNetUserId = user.Id
                });
                _db.SaveChanges();
            }
            catch (Exception e)
            {
                throw e;
            }

            return true;
        }

        public Deliverer Get(string id)
        {
            Deliverer deliverer;
            try
            {
                deliverer = _db.Deliverers.Find(id);
            }
            catch (Exception e)
            {
                throw e;
            }

            return deliverer;
        }

        public string GetGeolocation(string id)
        {
            string geo;
            try
            {
                geo = _db.Deliverers.Find(id).Geolocation;
            }
            catch (Exception e)
            {
                throw e;
            }

            return geo;
        }

        public bool GetDeliveringStatus(string id)
        {
            bool isDelivering = false;
            try
            {
                Deliverer deliverer = _db.Deliverers.Find(id);
                isDelivering = deliverer.IsDelivering;
            }
            catch (Exception e)
            {
                throw e;
            }

            return isDelivering;
        }

        public bool GetWorkingStatus(string id)
        {
            bool isWorking = false;
            try
            {
                Deliverer deliverer = _db.Deliverers.Find(id);
                isWorking = deliverer.IsWorking;
            }
            catch (Exception e)
            {
                throw e;
            }

            return isWorking;
        }

        public string GetCredentials(string id)
        {
            string result;
            try
            {
                result = _db.Deliverers.Where(d => d.DeliverNetUserId == id).FirstOrDefault().Credentials;
            }
            catch (Exception e)
            {
                throw e;
            }
            return result;
        }

        public bool SetWorkingStatus(string id, bool isWorking)
        {
            Deliverer deliverer;
            try
            {
                deliverer = _db.Deliverers.Find(id);
                deliverer.IsWorking = isWorking;
                _db.SaveChanges();
            }
            catch (Exception e)
            {
                throw e;
            }

            return true;
        }

        public bool SetDeliveringStatus(string id, bool isDelivering)
        {
            Deliverer deliverer;
            try
            {
                deliverer = _db.Deliverers.Find(id);
                deliverer.IsDelivering = isDelivering;
                _db.SaveChanges();
            }
            catch (Exception e)
            {
                throw e;
            }

            return true;
        }

        public bool SetGeolocation(string id, string geolocation)
        {
            Deliverer deliverer;
            try
            {
                deliverer = _db.Deliverers.Find(id);
                deliverer.Geolocation = geolocation;
                _db.SaveChanges();
            }
            catch (Exception e)
            {
                throw e;
            }

            return true;
        }

        public bool SetCredentials(string id, string credentials)
        {
            Deliverer user;
            try
            {
                user = _db.Deliverers.Where(d => d.DeliverNetUserId == id).FirstOrDefault();
                user.Credentials = credentials;
                _db.SaveChanges();
            }
            catch (Exception e)
            {
                throw e;
            }
            return true;
        }

        public bool SetOperatingRegion(string id, string operatingRegion)
        {
            Deliverer deliverer;
            try
            {
                deliverer = _db.Deliverers.Find(id);
                deliverer.OperationalRegion = operatingRegion;
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
            List<Deliverer> deliverers = _db.Deliverers.ToList();
            foreach (Deliverer d in deliverers)
            {
                _db.Deliverers.Remove(d);
            }
            _db.SaveChanges();
            return true;
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
