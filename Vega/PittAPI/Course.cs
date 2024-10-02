using Vega.PittAPI.APITypes;

namespace Vega.PittAPI
{
    public class Course(Subject subject, APICourse course)
    {
        public Subject Subject { get; } = subject;
        public int CatalogNumber { get; } = int.Parse(course.catalog_nbr);
        public string Title { get; } = course.descr;
        public int InternalId { get; } = int.Parse(course.crse_id);
        public bool[] TypicalTerms { get; } = [course.typ_offr.Contains("FALL"), course.typ_offr.Contains("SPR"), course.typ_offr.Contains("SUM")];
        public int[] Offerings { get; } = course.offerings.Select(x => int.Parse(x.crse_offer_nbr)).ToArray();

        public string DescribeTypicalTerms()
        {
            string[] termNames = ["fall", "spring", "summer"];
            return Enumerable.Range(0, 3).Where(x => TypicalTerms[x]).Select(x => termNames[x]).Aggregate((x, y) => x + ", " + y);
        }

        public async Task GetCourseDetailsAsync(int offeringIndex)
        {
            var courseDetails = await HttpRequester.MakeHttpRequestAsync<APICourseDetails>
                ($"https://pitcsprd.csps.pitt.edu/psc/pitcsprd/EMPLOYEE/SA/s/WEBLIB_HCX_CM.H_COURSE_CATALOG.FieldFormula.IScript_Catalog" +
                $"CourseDetails?institution=UPITT&course_id={InternalId}&effdt=2018-06-30&crse_offer_nbr={Offerings[offeringIndex]}&use_catalog_print=Y");
        }
    }
}