using System;
using System.Collections.Generic;
using System.Text;

namespace CRS.Domain
{
    public class BiometricRequest
    {
        public int OwnerId { get; set; }
        public Guid Guid { get; set; }
        public byte[] Data { get; set; }
    }
}
