using System;
using System.Collections.Generic;

namespace CRS.Domain
{
    public class SuspectRequest
    {
        
        public int Id { get; set; }
        public  int  CitizenId { get; set; }  
        public List<byte[]> Bytes { get; set; }

    }
}
