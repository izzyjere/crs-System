using System;
using System.Collections.Generic;
using System.Text;

namespace CRS.Shared
{
    public class Case
    {
        public int SuspectId { get; set; }
        public string Offense { get; set; }
        public string PoliceStation { get; set; }
        public DateTime Date { get; set; }
        public Suspect Suspect { get; set; }
        public List<Judgement> Judgements { get; set; }
        public string Court { get; set; }

    }
}
