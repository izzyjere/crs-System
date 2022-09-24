using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CRS.Shared
{
    public class NRCNumber
    {
        [Key]
        public int Id { get; set; }
        public int Value { get; set; }
    }
}
