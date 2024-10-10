using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Vega.Models
{
    public class DbListing
    {
        [Key][DatabaseGenerated(DatabaseGeneratedOption.Identity)] public int ListingId { get; set; }
        public int CatalogNumber { get; set; }
        public int CourseInternalId { get; set; }
        public DbCourse Course { get; set; } = null!;
        public string SubjectName { get; set; } = null!;
        public DbSubject Subject { get; set; } = null!;

        public string FormattedCatalogNumber() => SubjectName + ' ' + CatalogNumber;
    }
}
