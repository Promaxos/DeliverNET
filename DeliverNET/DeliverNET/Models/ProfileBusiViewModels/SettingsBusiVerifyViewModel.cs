using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DeliverNET.Models.ProfileBusiViewModels
{
    public class SettingsBusiVerifyViewModel
    {
        [Required]
        [DataType(DataType.Text)]
        public string Title { get; set; }

        [Required]
        [DataType(DataType.Text)]
        public Cities City { get; set; }

        [Required]
        [DataType(DataType.Text)]
        public Areas Area { get; set; }

        [Required]
        [DataType(DataType.Text)]
        public string Address { get; set; }

        [Required]
        [DataType(DataType.PhoneNumber)]
        public string PhoneNumber { get; set; }
    }


    public enum Cities
    {
        Athens,
        Patras,
        Thessaloniki
    }

    public enum Areas
    {
        Area1,
        Area2,
        Area3,
        Area4,
        Area5,
        Area6
    }
}
