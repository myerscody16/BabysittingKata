using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PillarTechBabysittingKata.Models
{
    public partial class Appointments
    {
        public int Id { get; set; }
        [Required]
        public string FamilyId { get; set; }
        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public TimeSpan StartTime { get; set; }
        [Required]
        public TimeSpan EndTime { get; set; }
        public int? TotalCost { get; set; }
        public Appointments ShallowCopy()
        {
            return (Appointments)this.MemberwiseClone();
        }
    }
}
