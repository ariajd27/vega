using Vega.PittAPI;

namespace Vega.PittAPI.APITypes
{
    public class APISubject(string subject, string descr)
    {
        public readonly string subject = subject;
        public readonly string descr = descr;

        public static async Task<APISubject[]> GetAllSubjectsAsync() =>
            await HttpRequester.RequestAllAsync<APISubject, APISubjectsResponse>
            ("https://pitcsprd.csps.pitt.edu/psc/pitcsprd/EMPLOYEE/SA/s/WEBLIB_HCX_CM.H_COURSE_CATALOG.FieldFormula.IScript_CatalogSubjects?institution=UPITT");
    }
}
