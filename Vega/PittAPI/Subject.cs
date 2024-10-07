using System.Text.Json.Serialization;
using Vega.PittAPI.APITypes;

namespace Vega.PittAPI
{
    public class Subject
    {
        public string Name { get; }
        public string Description { get; }

        public static async Task<Subject[]> GetAllSubjectsAsync(bool includeSMS = false, bool includeFTD = false)
        {
            var apiSubjects = await APISubject.GetAllSubjectsAsync();
            return apiSubjects.Where(x => (includeSMS || !x.subject.StartsWith("SMS")) && (includeFTD || !x.subject.StartsWith("FTD"))).Select(x => new Subject(x)).ToArray();
        }

        public Subject(APISubject subject)
        {
            Name = subject.subject;
            Description = subject.descr;
        }

        [JsonConstructor]
        public Subject(string name, string description)
        {
            Name = name;
            Description = description;
        }
    }
}