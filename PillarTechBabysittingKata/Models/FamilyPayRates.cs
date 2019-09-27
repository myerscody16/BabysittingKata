using System;
using System.Collections.Generic;

namespace PillarTechBabysittingKata.Models
{
    public partial class FamilyPayRates
    {
        public int Id { get; set; }
        public string FamilyLetter { get; set; }
        public int PayRate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
    }
}
