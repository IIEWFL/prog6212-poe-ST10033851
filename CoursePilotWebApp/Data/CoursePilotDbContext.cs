using CoursePilotWebApp.Models.Domain;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CoursePilotWebApp.Data
{
    public class CoursePilotDbContext : IdentityDbContext
    {
        public CoursePilotDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Module> Modules { get; set; }
        public DbSet<StudyRecords> StudyRecords { get; set; }
        public DbSet<GraphData> GraphData { get; set; }
    }
}
