using System;
using System.Collections.Generic;

namespace PillarTechBabysittingKata.Models
{
    public partial class Appointments
    {
        public int Id { get; set; }
        public string FamilyId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int? TotalCost { get; set; }
    }
}
