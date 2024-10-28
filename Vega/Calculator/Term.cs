using Vega.PittAPI;

namespace Vega.Calculator
{
    public class Term
    {
        public required VegaService VegaService { get; set; }

        public required string Name { get; set; }
        public required Terms APITerm { get; set; }
        public List<TreeCourse> Courses { get; set; } = [];

        public async Task<int> NumCredits()
        {
            var courses = await Task.WhenAll(Courses.Select(async c => (await VegaService.GetCoursesAsync(courseId: c.Id)).First()));
            return courses.Sum(c => c.MinNumCredits);
        }
    }
}
