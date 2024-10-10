using System.ComponentModel.DataAnnotations;
using Vega.PittAPI.APITypes;

namespace Vega.PittAPI
{
    public class Subject(string name, string description)
    {
        public string Name { get; private set; } = name;
        public string Description { get; private set; } = description;

        public static async Task<Subject[]> GetAllSubjectsAsync(bool includeSMS = false, bool includeFTD = false)
        {
            var apiSubjects = await APISubject.GetAllSubjectsAsync();
            return apiSubjects.Where(x => (includeSMS || !x.subject.StartsWith("SMS")) && (includeFTD || !x.subject.StartsWith("FTD")))
                .Select(x => new Subject(x.subject, x.descr)).ToArray();
        }
    }
}