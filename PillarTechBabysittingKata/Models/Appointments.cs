using System;
using System.Collections.Generic;

namespace PillarTechBabysittingKata.Models
{
    public partial class Appointments
    {
        public int Id { get; set; }
        public string FamilyId { get; set; }
        public DateTime StartDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public int? TotalCost { get; set; }
    }
}
