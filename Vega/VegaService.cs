﻿using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Vega.Models;
using Vega.PittAPI;

namespace Vega
{
    public class VegaService(VegaContext context)
    {
        private readonly VegaContext context = context;

        public async Task<List<DbSubject>> GetSubjectsAsync(string? campus = null)
        {
            if (string.IsNullOrEmpty(campus))
            {
                return await context.Subjects.ToListAsync();
            }
            else
            {
                return await context.Subjects.Where(s => s.Listings.Any(l => l.Course.Campus == campus)).ToListAsync();
            }
        }

        public async Task<List<DbCourse>> GetCoursesAsync(string? attributeKey = null, string? subjectKey = null,
            int? courseId = null, int? minCatalogNumber = null, int? maxCatalogNumber = null, int? catalogNumber = null,
            Terms? terms = Terms.Year, decimal? minNumCredits = null, decimal? maxNumCredits = null, string? campus = null)
        {
            List<DbCourse> courses;

            bool filteredBySubject = false;
            bool filteredByCatalogNumber = false;
            bool filteredByAttribute = false;
            bool filteredByCampus = false;

            // have we been given a specific catalog number?
            if (minCatalogNumber != null && maxCatalogNumber != null && catalogNumber == null && minCatalogNumber == maxCatalogNumber)
            {
                catalogNumber = minCatalogNumber;
            }

            // have we been given the course id?
            if (courseId != null)
            {
                courses = context.Courses.Where(c => c.PittId == courseId).ToList();
            }
            else
            {
                // have we been given a specific subject and catalog number?
                if (!subjectKey.IsNullOrEmpty() && catalogNumber != null)
                {
                    filteredBySubject = true;
                    filteredByCatalogNumber = true;
                    courses = context.Courses.Where(c => c.Listings.Any(l => l.SubjectName == subjectKey && l.CatalogNumber == catalogNumber)).ToList();
                }

                // can we cut down by grabbing courses by attribute?
                else
                {
                    // either way, this is as much filtering by attribute as we can do
                    filteredByAttribute = true;

                    if (!attributeKey.IsNullOrEmpty())
                    {
                        courses = context.Attributes.Find(attributeKey)?.Courses.ToList() ?? [];
                    }

                    // can we grab courses by subject?
                    else
                    {
                        // either way, this is as much filtering by subject as we can do
                        filteredBySubject = true;

                        // yes, which is good, it saves ram
                        if (!subjectKey.IsNullOrEmpty())
                        {
                            courses = context.Subjects.Find(subjectKey)?.Listings.Select(x => x.Course).ToList() ?? [];
                        }

                        // fine... can we at least filter by campus?
                        else
                        {
                            // as usual
                            filteredByCampus = true;

                            // this is already awful
                            if (!campus.IsNullOrEmpty())
                            {
                                courses = context.Courses.Where(c => c.Campus == campus).ToList();
                            }

                            // this is going to be so so slow and ram-intensive
                            else
                            {
                                courses = await context.Courses.ToListAsync();
                            }
                        }
                    }
                }
            }

            // filter by attribute
            if (!filteredByAttribute && !attributeKey.IsNullOrEmpty())
            {
                courses = courses.Where(c => c.Attributes.Any(a => a.BothShort == attributeKey)).ToList();
            }

            // filter by subject
            if (!filteredBySubject && !subjectKey.IsNullOrEmpty())
            {
                courses = courses.Where(c => c.Listings.Any(l => l.SubjectName == subjectKey)).ToList();
            }

            // filter by term availability
            courses = courses.Where(c => terms == Terms.Year || (terms | c.TypicalTerms) != 0).ToList();

            // filter by number of credits
            courses = courses.Where(c =>
            {
                if (minNumCredits != null && c.MaxNumCredits < minNumCredits) return false;
                else if (maxNumCredits != null && c.MinNumCredits > maxNumCredits) return false;
                else return true;
            }).ToList();

            // filter by catalog number
            if (!filteredByCatalogNumber)
            {
                courses = courses.Where(c => c.Listings.Any(l =>
                {
                    if (minCatalogNumber != null && l.CatalogNumber < minCatalogNumber) return false;
                    else if (maxCatalogNumber != null && l.CatalogNumber > maxCatalogNumber) return false;
                    else return true;
                })).ToList();
            }

            if (!filteredByCampus && !campus.IsNullOrEmpty())
            {
                courses = courses.Where(c => c.Campus == campus).ToList();
            }

            return courses;
        }

        public async Task<DbCourse?> GetCourseByListingAsync(string subject, int number)
        {
            if (subject.IsNullOrEmpty()) return null;
            var courses = await GetCoursesAsync(subjectKey: subject, catalogNumber: number);
            return courses.FirstOrDefault(c => true, null);
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
                    var existingCourses = context.Courses.Where(c => c.PittId == course.InternalId 
                        && c.Campus == course.Campus && c.TypicalTerms == course.TypicalTerms);
                    var existingCourse = existingCourses.Any() ? existingCourses.First() : null;

                    if (existingCourse == null)
                    {
                        DbCourse newCourse = new()
                        {
                            PittId = course.InternalId,
                            Title = course.Title,
                            Description = course.Description,
                            Campus = course.Campus,
                            TypicalTerms = course.TypicalTerms,
                            MinNumCredits = (int)(2 * course.MinNumCredits),
                            MaxNumCredits = (int)(2 * course.MaxNumCredits),
                            Requirements = course.Requirements
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
