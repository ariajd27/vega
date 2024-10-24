﻿using NuGet.Common;
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
        public int MinNumCredits { get; set; }
        public int MaxNumCredits { get; set; }
        public string? Requirements { get; set; }
        public virtual ICollection<DbListing> Listings { get; set; } = [];
        public virtual ICollection<DbAttribute> Attributes { get; set; } = [];

        public string AllCatalogNumbers()
        {
            if (Listings.Count == 0) return "";
            else return Listings.Select(x => x.FormattedCatalogNumber()).Aggregate((x, y) => $"{x}, {y}");
        }
        public string FormattedCampus() => Campus == null ? "unlisted" : Course.campusNames[Campus];
        public string FormattedNumCredits()
        {
            decimal min = MinNumCredits / 2;
            decimal max = MaxNumCredits / 2;

            return min < max ? $"{min:F1} - {max:F1}" : min.ToString("F1");
        }

        //public string FormattedRequirements(string[] subjects)
        //{
        //    var requirements = Tokenizer.Parse(Requirements ?? "", subjects).Select(x => $"[{x}]").Aggregate("", (x, y) => $"{x} {y}");
        //    return requirements.Length == 0 ? "none" : requirements;
        //}
    }
}
