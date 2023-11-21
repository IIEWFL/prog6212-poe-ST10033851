using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.Contracts;

namespace CoursePilotWebApp.Models.Domain
{
    public class GraphData
    {
        public int ID { get; set; }

        public String ModuleCode { get; set; }

        public String UserID { get; set; }

        public DateTime WeekStartDate { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public Decimal IdealHours { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public Decimal TotalHoursStudied {get; set;}

    }
}
