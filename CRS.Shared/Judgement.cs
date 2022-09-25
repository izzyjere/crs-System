using System;
using System.ComponentModel.DataAnnotations;

namespace CRS.Shared
{
    public class Judgement
    {
        [Key]
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public int CaseId { get; set; }
        public string JudgedBy { get; set; }
        public string Court { get; set; }
        public string  CertifyingOfficer { get; set; } 
        public Verdict Verdict { get; set; }
        public Case Case { get; set; }
    }
    public enum Verdict
    {
        None,
        Guity,
        NotGuity
    }
}
