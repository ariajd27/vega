using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Vega.PittAPI;

namespace Vega.Models
{
    public class DbCourse
    {
        [Key][DatabaseGenerated(DatabaseGeneratedOption.Identity)] public int Id { get; set; }
        public int PittId { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public string? Campus { get; set; }
        public Terms TypicalTerms { get; set; }
        public decimal MinNumCredits { get; set; }
        public decimal MaxNumCredits { get; set; }
        public virtual ICollection<DbListing> Listings { get; set; } = [];
        public virtual ICollection<DbAttribute> Attributes { get; set; } = [];

        public string AllCatalogNumbers()
        {
            if (Listings.Count == 0) return "";
            else return Listings.Select(x => x.FormattedCatalogNumber()).Aggregate((x, y) => $"{x}, {y}");
        }
        public string FormattedCampus() => Campus == null ? "unlisted" : Course.campusNames[Campus];
        public string FormattedNumCredits() => MinNumCredits < MaxNumCredits ? $"{MinNumCredits} - {MaxNumCredits}" : MinNumCredits.ToString();
    }
}
