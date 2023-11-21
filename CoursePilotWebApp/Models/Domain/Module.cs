using System.ComponentModel.DataAnnotations.Schema;

namespace CoursePilotWebApp.Models.Domain
{
    public class Module
    {
        public int ID { get; set; }
        public String UserID { get; set; }
        public String ModuleCode { get; set; }
        public String moduleName { get; set; }
        public int credits { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public Decimal weeklyClassHours { get; set; }
        public int weeksInSem { get; set; }
        public DateTime startDate { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public Decimal selfStudyHoursPerWeek { get; set; }

    }
}
