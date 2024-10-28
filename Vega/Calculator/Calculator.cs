using InputTerm = Vega.Components.Elements.TermInput.Term;

namespace Vega.Calculator
{
    public class Calculator
    {
        private VegaService VegaService;
        private int[] pastCourses;
        private Term[] terms;

        public Calculator(VegaService VegaService, IEnumerable<InputTerm> inputTerms, IEnumerable<int> pastCourses)
        {
            this.VegaService = VegaService;
            terms = inputTerms.Select(t => new Term() { VegaService = this.VegaService, Name = t.ToString(), APITerm = t.Season }).ToArray();
            this.pastCourses = pastCourses.ToArray();
        }

        public async Task Calculate(string[] subjects, IEnumerable<(string subject, int number)> todoCourses)
        {
            var intCourses = (await Task.WhenAll(todoCourses.Select(c => IntCourse.ParseAsync(VegaService, subjects, c.subject, c.number))))
                             .ToArray();
            var treeCourses = new List<TreeCourse>();
            for (int i = 0; i < intCourses.Length; i++)
            {
                if (intCourses[i] == null) continue;
                TreeCourse newCourse = new()
                {
                    Id = intCourses[i]!.Id,
                    Terms = intCourses[i]!.Terms,
                    Name = intCourses[i]!.Name
                };
                newCourse.Prereqs.AddRange(treeCourses.Where(c => intCourses[i]!.Prereqs.Contains(c.Id)));
                newCourse.Coreqs.AddRange(treeCourses.Where(c => intCourses[i]!.Coreqs.Contains(c.Id)));
                treeCourses.Add(newCourse);
            }

            while (treeCourses.Count > 0)
            {
                TreeCourse firstCourse = treeCourses.Where(c => c.Prereqs.Count == 0).First() 
                    ?? throw new Exception("No courses without any prereqs to add to plan!");

                terms.First(t => firstCourse.Terms.HasFlag(t.APITerm)).Courses.Add(firstCourse);

                foreach (var otherCourse in treeCourses.Where(c => c != firstCourse))
                {
                    otherCourse.Prereqs.Remove(firstCourse);
                    otherCourse.Coreqs.Remove(firstCourse);
                }

                treeCourses.Remove(firstCourse);
            }
        }

        public IEnumerable<Term> GetTerms() => terms;
    }
}
