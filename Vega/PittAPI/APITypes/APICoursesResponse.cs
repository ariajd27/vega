namespace Vega.PittAPI.APITypes
{
    public class APICoursesResponse(APICourse[] courses, string typ_offr_label) : IHttpArrayResponse<APICourse>
    {
        public readonly APICourse[] courses = courses;
        public readonly string typ_offr_label = typ_offr_label;

        public APICourse[] GetContents() => courses;
    }
}