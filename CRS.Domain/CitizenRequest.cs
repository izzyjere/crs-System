using System;
using System.Collections.Generic;

using System.Text;

namespace CRS.Domain
{
    public class CitizenRequest
    {
       
        public string FirstName { get; set; }
        
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public string NRC { get; set; }
        public string Gender { get; set; }  
        public DateTime DateOfBirth { get; set; }    
        public string PlaceOfBirth { get; set; }         
        public string Chief { get; set; }
        public string Village { get; set; }
        public string District { get; set; }
        public DateTime DateOfRegistration { get; set; }
        public int Id { get; set; }
        public string FullName => FirstName + " "+MiddleName +" "+ LastName;
        public byte[] ThumbPrintData { get; set; }
    }
}
