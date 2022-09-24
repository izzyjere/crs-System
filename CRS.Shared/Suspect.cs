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
        public int CitizenId { get; set; }
        public Citizen Citizen { get; set; }
        public List<Case> Cases { get; set; }
        public List<Biometric> Biometrics { get; set; }

    }
}
