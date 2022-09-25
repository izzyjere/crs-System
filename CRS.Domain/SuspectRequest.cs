using System;
using System.Collections.Generic;

namespace CRS.Domain
{
    public class SuspectRequest
    {
        public  int  CitizenId { get; set; }  
        public List<byte[]> Bytes { get; set; }
        public string EyeColor { get; set; }
        public string NRC { get; set; }
        public string Name { get; set; }
        public string Complexion { get; set; }           
        public string Occupation { get; set; }
        public string PhysicalAddress { get; set; }

    }
}
