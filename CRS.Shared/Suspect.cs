using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CRS.Shared
{
    public class Suspect
    {
        [Key]
        public int Id { get; set; }
        public string NRC { get; set; } 
        public string Complexion { get; set; }
        public string EyeColor { get; set; }
        public string Occupation { get; set; }
        public string PhysicalAddress { get; set; }
        public List<Case> Cases { get; set; }
        public List<Biometric> Biometrics { get; set; }

    }
}
