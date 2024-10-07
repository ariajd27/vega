using System.Linq;
using System.Text.Json.Serialization;
using System.Xml.Linq;
using Vega.Components.Pages;
using Vega.PittAPI.APITypes;

namespace Vega.PittAPI
{
    public class Course
    {
        public string Subject { get; }
        public int CatalogNumber { get; }
        public int InternalId { get; }
        public string FormattedCatalogNumber() => Subject + " " + CatalogNumber.ToString("D4");

        public string Title { get; }
        public string Description { get; }

        public string Campus { get; }
        public string CampusDescr { get; set; }
        public Terms TypicalTerms { get; }

        public int MinNumCredits { get; }
        public int MaxNumCredits { get; }
        public string FormattedNumCredits() => MinNumCredits == MaxNumCredits ? MinNumCredits.ToString() : MinNumCredits.ToString() + "-" + MaxNumCredits.ToString();

        public static async Task<Course[]> GetAllCoursesAsync(string subject)
        {
            List<Course> output = [];
            var apiCourses = await APICourse.GetAllCoursesAsync(subject);
            List<Task<List<(APICourse course, APICourseDetails details)>>> getCourses = [];
            foreach (var apiCourse in apiCourses)
            {
                getCourses.Add(GetCourseDetailsAsync(apiCourse));
            }
            foreach (var courses in await Task.WhenAll(getCourses))
            {
                foreach (var course in courses)
                {
                    output.Add(new(subject, course.course, course.details));
                }
            }

            return output.ToArray();
        }

        private static async Task<List<(APICourse course, APICourseDetails details)>> GetCourseDetailsAsync(APICourse course)
        {
            List<(APICourse course, APICourseDetails details)> output = [];
            var apiOfferings = course.offerings.Select(x => x.crse_offer_nbr);
            foreach (var apiOffering in apiOfferings)
            {
                output.Add((course, await APICourseDetails.GetCourseDetailsAsync(course.crse_id, apiOffering)));
            }

            return output;
        }

        private Course(string subject, APICourse apiCourse, APICourseDetails apiDetails)
        {
            Subject = subject;
            CatalogNumber = int.Parse(apiCourse.catalog_nbr);
            InternalId = int.Parse(apiCourse.crse_id);
            Title = apiCourse.descr;
            Description = apiDetails.descrlong;
            Campus = apiDetails.offerings is not null ? apiDetails.offerings[0].campus_cd : "unlisted";
            CampusDescr = apiDetails.offerings is not null ? apiDetails.offerings[0].campus : "Unlisted";
            TypicalTerms = ParseAPITerms(apiCourse.typ_offr);
            MinNumCredits = apiDetails.units_minimum;
            MaxNumCredits = apiDetails.units_maximum;
        }

        public static Terms ParseAPITerms(string apiTerms)
        {
            Terms terms = apiTerms.Contains("FALL") ? Terms.Fall : Terms.Unlisted;
            if (apiTerms.Contains("SPR")) terms |= Terms.Spring;
            if (apiTerms.Contains("SUM")) terms |= Terms.Summer;
            return terms;
        }

        [JsonConstructor]
        public Course(string subject, int catalogNumber, int internalId, string title, string description, 
            Terms typicalTerms, string campus, string campusDescr, int minNumCredits, int maxNumCredits)
        {
            Subject = subject;
            CatalogNumber = catalogNumber;
            InternalId = internalId;
            Title = title;
            Description = description;
            Campus = campus;
            CampusDescr = campusDescr;
            TypicalTerms = typicalTerms;
            MinNumCredits = minNumCredits;
            MaxNumCredits = maxNumCredits;
        }
    }

    [Flags]
    public enum Terms
    {
        Unlisted = 0b_000,
        Fall = 0b_001,
        Spring = 0b_010,
        Summer = 0b_100,
        Year = Fall | Spring | Summer
    }
}