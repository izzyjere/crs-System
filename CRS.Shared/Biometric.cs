using System;
using System.Collections.Generic;
using System.Text;

namespace CRS.Shared
{
    public class Biometric
    {
        public int OwnerId { get; set; }
        public Guid Guid { get; set; }
        public byte[] Data { get; set; }
    }
}
