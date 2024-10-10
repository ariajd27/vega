using Microsoft.EntityFrameworkCore;
using Vega.Models;
using Attribute = Vega.PittAPI.Attribute;

namespace Vega
{
    public class VegaContext(DbContextOptions<VegaContext> options) : DbContext(options)
    {
        public DbSet<DbSubject> Subjects { get; set; }
        public DbSet<DbCourse> Courses { get; set; }
        public DbSet<DbListing> Listings { get; set; }
        public DbSet<DbAttribute> Attributes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DbListing>().HasOne(l => l.Course)
                                            .WithMany(c => c.Listings)
                                            .HasForeignKey(l => l.CourseInternalId);

            modelBuilder.Entity<DbListing>().HasOne(l => l.Subject)
                                            .WithMany(c => c.Listings)
                                            .HasForeignKey(l => l.SubjectName);

            modelBuilder.Entity<DbCourse>().HasMany(c => c.Attributes)
                                           .WithMany(a => a.Courses);
        }
    }
}