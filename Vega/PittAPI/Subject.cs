using System.Text.Json.Serialization;
using Vega.PittAPI.APITypes;

namespace Vega.PittAPI
{
    public class Subject
    {
        public string Name { get; }
        public string Description { get; }

        public static async Task<Subject[]> GetAllSubjectsAsync()
        {
            var apiSubjects = await APISubject.GetAllSubjectsAsync();
            return apiSubjects.Select(x => new Subject(x)).ToArray();
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