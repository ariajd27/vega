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
            if (attribute != null)
            {
                unfilteredCourses = context.Attributes.Find(attribute.BothShort)?.Courses.ToList() ?? [];
                if (subject != null)
                {
                    unfilteredCourses = unfilteredCourses.Where(c => c.Listings.Any(l => l.SubjectName == subject.Name)).ToList();
                }
            }
            else if (subject != null)
            {
                unfilteredCourses = context.Subjects.Find(subject.Name)?.Listings.Select(x => x.Course).ToList() ?? [];
            }
            else
            {
                unfilteredCourses = await context.Courses.ToListAsync();
            }

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
                    var existingCourse = context.Courses.Find(course.InternalId);

                    if (existingCourse == null)
                    {
                        DbCourse newCourse = new()
                        {
                            PittId = course.InternalId,
                            Title = course.Title,
                            Description = course.Description,
                            Campus = course.Campus,
                            TypicalTerms = course.TypicalTerms,
                            MinNumCredits = course.MinNumCredits,
                            MaxNumCredits = course.MaxNumCredits
                        };

                        context.Courses.Add(newCourse);
                        context.SaveChanges();
                        existingCourse = newCourse;
                    }

                    DbListing newListing = new()
                    {
                        CatalogNumber = course.CatalogNumber,
                        CourseId = existingCourse.Id,
                        SubjectName = subject.Name
                    };
                    
                    context.Listings.Add(newListing);
                    context.SaveChanges();

                    subject.Listings.Add(newListing);
                    existingCourse.Listings.Add(newListing);

                    foreach (var attribute in course.Attributes)
                    {
                        string bothShort = attribute.CategoryShort + '.' + attribute.ValueShort;

                        var existingAttribute = await context.Attributes.FindAsync(bothShort);

                        if (existingAttribute == null)
                        {
                            DbAttribute newAttribute = new()
                            {
                                BothShort = bothShort,
                                CategoryDescr = attribute.CategoryDescr,
                                ValueDescr = attribute.ValueDescr
                            };

                            context.Attributes.Add(newAttribute);
                            context.SaveChanges();
                            existingAttribute = newAttribute;
                        }

                        existingCourse.Attributes.Add(existingAttribute);
                        existingAttribute.Courses.Add(existingCourse);
                        context.SaveChanges();
                    }
                }

                context.SaveChanges();
            }
        }
    }
}
