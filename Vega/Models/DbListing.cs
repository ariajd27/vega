using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Vega.Models
{
    public class DbListing
    {
        [Key][DatabaseGenerated(DatabaseGeneratedOption.Identity)] public int ListingId { get; set; }
        public int CatalogNumber { get; set; }
        public int CourseId { get; set; }
        public virtual DbCourse Course { get; set; }
        public string SubjectName { get; set; } = null!;
        public virtual DbSubject Subject { get; set; }

        public string FormattedCatalogNumber() => SubjectName + ' ' + CatalogNumber.ToString("D4");
    }
}
