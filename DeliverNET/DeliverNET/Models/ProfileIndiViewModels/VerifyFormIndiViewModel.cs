using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DeliverNET.Models.ProfileIndiViewModels
{
    public class VerifyFormIndiViewModel
    {
        [Required]
        public OperationalCities OperationalCity { get; set; }

        [Required]
        public OperationRegions OperationalRegion { get; set; }

        [Required]
        public string Credentials { get; set; }
    }

    public enum OperationalCities
    {
        [Display(Name ="Αθήνα")]
        Athens
    }

    public enum OperationRegions
    {
        [Display(Name ="Ιλίσια")]
        Ilisia,
        [Display(Name ="Γουδή")]
        Goudi,
        [Display(Name ="Ζωγράφος")]
        Zografos
    }
}
