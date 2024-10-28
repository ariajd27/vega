using System.Text.RegularExpressions;
using Vega.Models;
using Vega.PittAPI;

namespace Vega.Calculator
{
    public partial class IntCourse
    {
        public required int Id { get; set; }
        public required Terms Terms { get; set; }

        public List<int> Prereqs { get; set; } = [];
        public List<int> Coreqs { get; set; } = [];

        public required string Name { get; set; }

        public static async Task<IntCourse?> ParseAsync(VegaService VegaService, string[] subjects, string subject, int catalogNumber)
        {
            DbCourse? dbCourse = await VegaService.GetCourseByListingAsync(subject: subject, number: catalogNumber);
            if (dbCourse == null) return null;

            IntCourse newCourse = new()
            {
                Id = dbCourse.PittId,
                Terms = dbCourse.TypicalTerms,
                Name = dbCourse.Listings.First().FormattedCatalogNumber()
            };
            if (dbCourse.Requirements == null) return newCourse;

            ParseMode parseMode = ParseMode.Prereq;
            string parseSubject = "";
            var tokens = dbCourse.Requirements.Split(' ')
                                              .Select(s => String.Concat(s.Where(c => char.IsLetterOrDigit(c))).ToLower())
                                              .ToArray();
            for (int i = 0; i < tokens.Length; i++)
            {
                if (PrereqRegex().IsMatch(tokens[i]))
                {
                    parseMode = ParseMode.Prereq;
                }
                else if (CoreqRegex().IsMatch(tokens[i]))
                {
                    parseMode = ParseMode.Coreq;
                }
                else if (subjects.Contains(tokens[i]))
                {
                    parseSubject = tokens[i];
                }
                else if (tokens[i].All(c => char.IsDigit(c)))
                {
                    DbCourse? parseCourse = await VegaService.GetCourseByListingAsync(parseSubject, int.Parse(tokens[i]));
                    if (parseCourse == null) continue;
                    int parseId = parseCourse.PittId;
                    if (parseMode == ParseMode.Prereq) newCourse.Prereqs.Add(parseId);
                    else newCourse.Coreqs.Add(parseId);
                }
            }

            return newCourse;
        }

        private enum ParseMode
        {
            Prereq,
            Coreq
        }

        [GeneratedRegex(@"pre-?(re)?q(uisite)?s?")]
        private static partial Regex PrereqRegex();

        [GeneratedRegex(@"co?-?req(uisite)?s?")]
        private static partial Regex CoreqRegex();
    }
}
