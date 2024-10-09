using Microsoft.EntityFrameworkCore;
using PittAPI;
using Attribute = PittAPI.Attribute;

namespace Vega
{
    public class VegaContext : DbContext
    {
        public VegaContext(DbContextOptions<VegaContext> options) : base(options) { }

        public DbSet<Subject> Subjects { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Attribute> Attributes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Subject>().ToTable("Subject");
            modelBuilder.Entity<Course>().ToTable("Course");
            modelBuilder.Entity<Attribute>().ToTable("Attribute");
        }
    }
}