using Vega.PittAPI.APITypes;

namespace Vega.PittAPI
{
    public class Subject(APISubject subject)
    {
        public string Name { get; } = subject.subject;
        public string Description { get; } = subject.descr;

        public Course[]? Courses { get; private set; }
        public bool Polled => Courses == null;

        public async Task<Course[]> GetCourses()
        {
            if (!Polled)
            {
                var apiCourses = await APICourse.GetAllCoursesAsync(Name);
                Courses = apiCourses.Select(x => new Course(this, x)).ToArray();
            }
            
            return Courses;
        }

        public static async Task<Subject[]> GetAllSubjectsAsync()
        {
            var apiSubjects = await APISubject.GetAllSubjectsAsync();
            return apiSubjects.Select(x => new Subject(x)).ToArray();
        }
    }
}