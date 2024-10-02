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
        public string Description { get; private set; }

        public bool[] TypicalTerms { get; }
        public string FormattedTypicalTerms()
        {
            string[] termNames = ["fall", "spring", "summer"];
            try
            {
                return Enumerable.Range(0, 3).Where(x => TypicalTerms[x]).Select(x => termNames[x]).Aggregate((x, y) => x + ", " + y);
            }
            catch (InvalidOperationException e)
            {
                return "unlisted";
            }
        }

        public int[] Offerings { get; }

        public int MinNumCredits { get; private set; }
        public int MaxNumCredits { get; private set; }
        public string FormattedNumCredits() => MinNumCredits == MaxNumCredits ? MinNumCredits.ToString() : MinNumCredits.ToString() + "-" + MaxNumCredits.ToString();

        public static async Task<Course[]> GetAllCoursesAsync(string subject)
        {
            var apiCourses = await APICourse.GetAllCoursesAsync(subject);
            return apiCourses.Select(x => new Course(subject, x)).ToArray();
        }

        public async Task GetCourseDetailsAsync(int offeringIndex)
        {
            var courseDetails = await APICourseDetails.GetCourseDetailsAsync(InternalId.ToString(), Offerings[offeringIndex].ToString());

            Description = courseDetails.descrlong;
            MinNumCredits = courseDetails.units_minimum;
            MaxNumCredits = courseDetails.units_maximum;
        }

        public Course(string subject, APICourse course)
        {
            Subject = subject;
            CatalogNumber = int.Parse(course.catalog_nbr);
            InternalId = int.Parse(course.crse_id);
            Title = course.descr;
            TypicalTerms = [course.typ_offr.Contains("FALL"), course.typ_offr.Contains("SPR"), course.typ_offr.Contains("SUM")];
            Offerings = course.offerings.Select(x => int.Parse(x.crse_offer_nbr)).ToArray();
        }

        [JsonConstructor]
        public Course(string subject, int catalogNumber, int internalId, string title, string description, 
            bool[] typicalTerms, int[] offerings, int minNumCredits, int maxNumCredits)
        {
            Subject = subject;
            CatalogNumber = catalogNumber;
            InternalId = internalId;
            Title = title;
            Description = description;
            TypicalTerms = typicalTerms;
            Offerings = offerings;
            MinNumCredits = minNumCredits;
            MaxNumCredits = maxNumCredits;
        }
    }
}