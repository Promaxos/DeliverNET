using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DeliverNET.Data
{
    public class Business
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Address { get; set; }
        public string Geolocation { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public DateTime SignupDate { get; set; }
        public DateTime? VerificationDate { get; set; }
        public string Credentials { get; set; }
        public bool Active { get; set; }

        public BusinessOwner BusinessOwner { get; set; }
        public ICollection<BusinessCashier> BusinessCashier { get; set; }
    }
}
