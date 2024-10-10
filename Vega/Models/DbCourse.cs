using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Vega.PittAPI;

namespace Vega.Models
{
    public class DbCourse
    {
        [Key][DatabaseGenerated(DatabaseGeneratedOption.None)] public int InternalId { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public string? Campus { get; set; }
        public Terms TypicalTerms { get; set; }
        public decimal MinNumCredits { get; set; }
        public decimal MaxNumCredits { get; set; }
        public List<DbListing> Listings { get; } = [];
        public List<DbAttribute> Attributes { get; } = [];

        public string AllCatalogNumbers() => Listings.Select(x => $"{x.SubjectName} {x.CatalogNumber}").Aggregate((x, y) => $"{x}, {y}");
        public string FormattedCampus() => Campus == null ? "unlisted" : Course.campusNames[Campus];
        public string FormattedNumCredits() => MinNumCredits < MaxNumCredits ? $"{MinNumCredits} - {MaxNumCredits}" : MinNumCredits.ToString();
    }
}
