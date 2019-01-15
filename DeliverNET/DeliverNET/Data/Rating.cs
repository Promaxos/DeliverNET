using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using DeliverNET.Data;

namespace DeliverNET.Data
{
    public class Rating
    {
        
        public int Id { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime Tstamp { get; set; }

        // Rating can be anonymous. Not mapped to user.
        public string Rater { get; set; }

        public DeliverNETUser Ratee { get; set; }

        [Required]
        public string Comment { get; set; }
    }
}
