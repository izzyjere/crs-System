using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CRS.Shared
{
    public class Case
    {
        [Key]
        public int Id { get; set; }
        public int SuspectId { get; set; }
        [Required]
        public string Offense { get; set; }
        [Required]
        public string OpenedBy { get; set; }
        [Required]
        public string PoliceStation { get; set; }
        public DateTime Date { get; set; }
        public Suspect Suspect { get; set; }
        public List<Judgement> Judgements { get; set; }         

    }
}
