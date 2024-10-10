using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Vega.Models
{
    public class DbSubject
    {
        [Key][DatabaseGenerated(DatabaseGeneratedOption.None)] public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public virtual ICollection<DbListing> Listings { get; set; } = [];
    }
}