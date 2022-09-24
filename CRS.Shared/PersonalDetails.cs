using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text;

namespace CRS.Shared
{
    public class PersonalDetails
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        public string? MiddleName { get; set; }
        [Required, RegularExpression(@"^\d{6}\/\d{2}\/\d{1}$")]
        public string NRC { get; set; }
        public string Gender { get; set; }
        public string PictureUrl { get; set; }
        [Required]
        public DateTime DateOfBirth { get; set; }
        [Required]
        public string PlaceOfBirth { get; set; }
        [Required]
        public string Chief { get; set; }
        public string Village { get; set; }
        public string District { get; set; }
        public DateTime DateOfRegistration { get; set; }
        public int Id { get; set; }
        public string FullName => FirstName + " "+MiddleName +" "+ LastName;
        public Biometric ThumbPrint { get; set; }

    }
}
