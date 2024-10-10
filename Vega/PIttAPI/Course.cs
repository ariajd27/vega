using System.ComponentModel.DataAnnotations;
using Vega.PittAPI.APITypes;

namespace Vega.PittAPI
{
    public class Course
    {
        public string Subject { get; private set; }
        public int CatalogNumber { get; private set; }
        public int InternalId { get; private set; }
        public string FormattedCatalogNumber() => Subject + " " + CatalogNumber.ToString("D4");

        public string Title { get; private set; }
        public string Description { get; private set; }

        public string? Campus { get; private set; }

        public static Dictionary<string, string> campusNames = new()
        {
            { "PIT", "Pittsburgh" },
            { "UPB", "Bradford" },
            { "UPG", "Greensburg" },
            { "UPJ", "Johnstown" },
            { "UPT", "Titusville" }
        };
        public string FormattedCampus() => Campus != null ? campusNames[Campus] : "unlisted";

        public Terms TypicalTerms { get; private set; }

        public decimal MinNumCredits { get; private set; }
        public decimal MaxNumCredits { get; private set; }
        public string FormattedNumCredits() =>
            MinNumCredits == MaxNumCredits ? MinNumCredits.ToString()
            : MinNumCredits.ToString() + "-" + MaxNumCredits.ToString();

        public string? Requirements { get; private set; }
        public IEnumerable<Attribute> Attributes { get; private set; }

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
                    Course newCourse = new() {
                        Subject = subject,
                        CatalogNumber = int.Parse(course.course.catalog_nbr[..4]),
                        InternalId = int.Parse(course.course.crse_id),
                        Title = course.course.descr,
                        Description = course.details.descrlong,
                        Campus = course.details.offerings?[0].campus_cd,
                        TypicalTerms = ParseAPITerms(course.course.typ_offr),
                        MinNumCredits = course.details.units_minimum,
                        MaxNumCredits = course.details.units_maximum,
                        Requirements = course.details.offerings?[0].req_group ?? "",
                        Attributes = course.details.attributes?.Select(x => 
                            new Attribute(x.crse_attribute, x.crse_attribute_descr, x.crse_attribute_value, x.crse_attribute_value_descr)) ?? []
                    };
                    
                    output.Add(newCourse);
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

        public static Terms ParseAPITerms(string apiTerms)
        {
            Terms terms = apiTerms.Contains("FALL") ? Terms.Fall : Terms.Unlisted;
            if (apiTerms.Contains("SPR")) terms |= Terms.Spring;
            if (apiTerms.Contains("SUM")) terms |= Terms.Summer;
            return terms;
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