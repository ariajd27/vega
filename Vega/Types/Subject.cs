using PittAPI.APITypes;
using System.Text.Json.Serialization;

namespace PittAPI
{
    public class Subject
    {
        public string Name { get; private set; }
        public string Description { get; private set; }

        public Dictionary<string, bool> AtCampus { get; private set; }

        public static async Task<Subject[]> GetAllSubjectsAsync(bool includeSMS = false, bool includeFTD = false)
        {
            var apiSubjects = await APISubject.GetAllSubjectsAsync();
            return apiSubjects.Where(x => (includeSMS || !x.subject.StartsWith("SMS")) && (includeFTD || !x.subject.StartsWith("FTD"))).Select(x => new Subject(x)).ToArray();
        }

        public Subject(APISubject subject)
        {
            Name = subject.subject;
            Description = subject.descr;
            AtCampus = [];
        }
    }
}