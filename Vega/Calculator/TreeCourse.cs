using Vega.PittAPI;

namespace Vega.Calculator
{
    public class TreeCourse
    {
        public required int Id { get; set; }
        public required Terms Terms { get; set; }

        public List<TreeCourse> Prereqs { get; set; } = [];
        public List<TreeCourse> Coreqs { get; set; } = [];

        public required string Name { get; set; }
    }
}
