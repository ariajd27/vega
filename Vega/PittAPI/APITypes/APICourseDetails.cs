namespace Vega.PittAPI.APITypes
{
    public class APICourseDetails(string descrlong, int units_minimum, int units_maximum, int units_inc, string grading_basis, string grading_basis_descr,
        string course_title, string rqmnt_designtn, string effdt, APIComponent[] components, APIAttribute[] attributes, APIOfferingDetails[] offerings)
    {
        public readonly string descrlong = descrlong;
        public readonly int units_minimum = units_minimum;
        public readonly int units_maximum = units_maximum;
        public readonly int units_inc = units_inc;
        public readonly string grading_basis = grading_basis;
        public readonly string grading_basis_descr = grading_basis_descr;
        public readonly string course_title = course_title;
        public readonly string rqmnt_designtn = rqmnt_designtn;
        public readonly string effdt = effdt;
        public readonly APIComponent[] components = components;
        public readonly APIAttribute[] attributes = attributes;
        public readonly APIOfferingDetails[] offerings = offerings;

        public static async Task<APICourseDetails> GetCourseDetailsAsync(string internalId, string offeringNumber) =>
            await HttpRequester.MakeHttpRequestAsync<APICourseDetails>
                ($"https://pitcsprd.csps.pitt.edu/psc/pitcsprd/EMPLOYEE/SA/s/WEBLIB_HCX_CM.H_COURSE_CATALOG.FieldFormula.IScript_Catalog" +
                $"CourseDetails?institution=UPITT&course_id={internalId}&effdt=2018-06-30&crse_offer_nbr={offeringNumber}&use_catalog_print=Y");
    }
}
