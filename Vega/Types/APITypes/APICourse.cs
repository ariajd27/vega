namespace PittAPI.APITypes
{
    public class APICourse(string acad_career, string catalog_nbr, string descr, string crse_id, string crse_offer_nbr, string effdt,
        string typ_offr, string typ_offr_descr, bool has_open_terms, bool multipleOfferings, APIOffering[] offerings)
    {
        public readonly string acad_career = acad_career;
        public readonly string catalog_nbr = catalog_nbr;
        public readonly string descr = descr;
        public readonly string crse_id = crse_id;
        public readonly string crse_offer_nbr = crse_offer_nbr;
        public readonly string effdt = effdt;
        public readonly string typ_offr = typ_offr;
        public readonly string typ_offr_descr = typ_offr_descr;
        public readonly bool has_open_terms = has_open_terms;
        public readonly bool multipleOfferings = multipleOfferings;
        public readonly APIOffering[] offerings = offerings;

        public static async Task<APICourse[]> GetAllCoursesAsync(string subject) =>
            await HttpRequester.RequestAllAsync<APICourse, APICoursesResponse>
            ($"https://pitcsprd.csps.pitt.edu/psc/pitcsprd/EMPLOYEE/SA/s/WEBLIB_HCX_CM.H_COURSE_CATALOG.FieldFormula.IScript_SubjectCourses?institution=UPITT&subject={subject}");
    }
}