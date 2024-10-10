using Microsoft.EntityFrameworkCore;
using System.Collections.Immutable;
using Vega.Models;
using Vega.PittAPI;
using Attribute = Vega.PittAPI.Attribute;

namespace Vega
{
    public class VegaService
    {
        private readonly VegaContext context;

        public VegaService(VegaContext context)
        {
            this.context = context;
        }

        public async Task<List<DbSubject>> GetSubjectsAsync()
        {
            if (!context.Subjects.Any()) await BuildDatabase();

            return await context.Subjects.ToListAsync();
        }

        public async Task<List<DbCourse>> GetCoursesAsync(DbAttribute? attribute = null, DbSubject? subject = null)
        {
            List<DbCourse> unfilteredCourses;
            if (attribute != null) unfilteredCourses = context.Attributes.Find(attribute.BothShort)?.Courses ?? [];
            else if (subject != null) unfilteredCourses = context.Subjects.Find(subject.Name)?.Listings.Select(x => x.Course).ToList() ?? [];
            else unfilteredCourses = await context.Courses.ToListAsync();

            return unfilteredCourses;
        }

        public async Task<List<DbAttribute>> GetAttributesAsync()
        {
            return await context.Attributes.ToListAsync();
        }

        private async Task BuildDatabase()
        {
            var subjects = (await Subject.GetAllSubjectsAsync()).Select(x => 
                new DbSubject() { Name = x.Name, Description = x.Description }).ToList();
            await context.Subjects.AddRangeAsync(subjects);

            foreach (var subject in subjects)
            {
                var courses = await Course.GetAllCoursesAsync(subject.Name);

                foreach (var course in courses)
                {
                    var existingCourse = await context.Courses.FindAsync(course.InternalId);

                    if (existingCourse == null)
                    {
                        DbCourse newCourse = new()
                        {
                            InternalId = course.InternalId,
                            Title = course.Title,
                            Description = course.Description,
                            Campus = course.Campus,
                            TypicalTerms = course.TypicalTerms,
                            MinNumCredits = course.MinNumCredits,
                            MaxNumCredits = course.MaxNumCredits
                        };

                        await context.Courses.AddAsync(newCourse);
                        existingCourse = newCourse;
                    }

                    DbListing newListing = new()
                    {
                        CatalogNumber = course.CatalogNumber,
                        CourseInternalId = existingCourse.InternalId,
                        SubjectName = subject.Name
                    };
                    
                    context.Listings.Add(newListing);
                    context.SaveChanges();

                    subject.Listings.Add(newListing);

                    foreach (var attribute in course.Attributes)
                    {
                        string bothShort = attribute.CategoryShort + '.' + attribute.ValueShort;

                        if (await context.Attributes.FindAsync(bothShort) == null)
                        {
                            DbAttribute newAttribute = new()
                            {
                                BothShort = bothShort,
                                CategoryDescr = attribute.CategoryDescr,
                                ValueDescr = attribute.ValueDescr
                            };

                            context.Attributes.Add(newAttribute);
                            context.SaveChanges();

                            existingCourse.Attributes.Add(newAttribute);
                            newAttribute.Courses.Add(existingCourse);
                        }
                    }
                }

                await context.SaveChangesAsync();
            }
        }
    }
}
