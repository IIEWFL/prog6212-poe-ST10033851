using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoursePilotWebApp.Models.Domain
{
    public class StudyRecords
    {
        public int ID { get; set; }

        public String ModuleCode { get; set; }

        public String UserID { get; set; }

        public DateTime StudyDate { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public Decimal HoursStudied { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public Decimal selfStudyHoursleft { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public Decimal IdealHours { get; set; }

        public bool WeekFinished { get; set; }

    }
}
