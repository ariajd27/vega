namespace Vega.PittAPI
{
    public class Subject(APISubject subject)
    {
        public string Name { get; } = subject.subject;
        public string Description { get; } = subject.descr;

        public Course[]? Courses { get; private set; }
        public bool Polled => Courses == null;

        public async Task GetCourses()
        {
            var apiCourses = await APICourse.GetAllCoursesAsync(Name);
            Courses = apiCourses.Select(x => new Course(this, x)).ToArray();
        }

        public static async Task<Subject[]> GetAllSubjectsAsync()
        {
            var apiSubjects = await APISubject.GetAllSubjectsAsync();
            return apiSubjects.Select(x => new Subject(x)).ToArray();
        }
    }

    public class APISubject(string subject, string descr)
    {
        public readonly string subject = subject;
        public readonly string descr = descr;

        public static async Task<APISubject[]> GetAllSubjectsAsync() => 
            await HttpRequester.RequestAllAsync<APISubject, APISubjectsResponse>
            ("https://pitcsprd.csps.pitt.edu/psc/pitcsprd/EMPLOYEE/SA/s/WEBLIB_HCX_CM.H_COURSE_CATALOG.FieldFormula.IScript_CatalogSubjects?institution=UPITT");
    }

    public class APISubjectsResponse(APISubject[] subjects) : IHttpArrayResponse<APISubject>
    {
        public APISubject[] subjects = subjects;
        public APISubject[] GetContents() => subjects;
    }
}