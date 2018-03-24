using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using LIMS.Controllers;
using LIMS.Models;

namespace LIMS.DataAccess
{
    public static class LabsDao
    {
        public static async Task<List<LabsSearchResult>> Find(IRequestContext context, string query = null)
        {
            var userId = context.UserId;
            IQueryable<LabsSearchResult> queryResults;

            if (string.IsNullOrWhiteSpace(query))
            {
                // no query, show all labs the user is a member of
                queryResults =
                    from r in context.DbContext.LabMembers
                    where r.UserId == userId
                    orderby r.LastOpened descending
                    join lm in context.DbContext.LabMembers on r.LabId equals lm.LabId
                    where lm.IsLabManager
                    select new LabsSearchResult
                    {
                        Id = r.LabId,
                        IsMember = true,
                        CollegeName = r.Lab.CollegeName,
                        CourseCode = r.Lab.CourseCode,
                        FacultyName = lm.User.UserName,
                        WeekNumber = r.Lab.WeekNumber,
                        TestId = r.Lab.TestId,
                        Location = r.Lab.Location,
                    };
            }
            else
            {
                // have query, search all labs
                queryResults =
                    from r in context.DbContext.Labs
                    where r.CourseCode.Contains(query) || r.TestId.Contains(query)
                    orderby r.TestId
                    let isMember = r.Members.Any(lm => lm.UserId == userId)
                    let labManager = r.Members.FirstOrDefault(lm => lm.IsLabManager)
                    select new LabsSearchResult
                    {
                        Id = r.LabId,
                        IsMember = isMember,
                        CollegeName = r.CollegeName,
                        CourseCode = r.CourseCode,
                        FacultyName = labManager.User.UserName,
                        WeekNumber = r.WeekNumber,
                        TestId = r.TestId,
                        Location = r.Location
                    };
            }

            return await queryResults.ToListAsync();
        }

        public static async Task<Lab> Create(IRequestContext context, LabsCreateViewModel model)
        {
            var test = await context.DbContext.Tests.FirstOrDefaultAsync(t => t.TestId == model.TestId);
            if (test == null)
                throw new Exception("Test does not exist.");

            var user = await context.UserManager.FindByIdAsync(context.UserId);

            var lab = new Lab
            {
                CollegeName = model.CollegeName,
                CourseCode = model.CourseCode,
                WeekNumber = model.WeekNumber,
                Test = test,
                Location = model.Location,
            };

            lab.Members = new List<LabMember>
            {
                new LabMember
                {
                    Lab = lab,
                    User = user,
                    IsLabManager = true,
                }
            };

            context.DbContext.Labs.Add(lab);
            await context.DbContext.SaveChangesAsync();

            await context.LogAsync($"Created lab ID {lab.LabId}");

            return lab;
        }

        public static async Task<Lab> Read(IRequestContext context, int labId)
        {
            var query = from l in context.DbContext.Labs
                where l.LabId == labId
                select l;

            return await query.SingleOrDefaultAsync();
        }

        public static async Task<Lab> Update(IRequestContext context, Lab lab, LabsEditViewModel model)
        {
            var test = await context.DbContext.Tests.FirstOrDefaultAsync(t => t.TestId == model.TestId);
            if (test == null)
                throw new Exception("Test does not exist.");

            lab.CollegeName = model.CollegeName;
            lab.CourseCode = model.CourseCode;
            lab.WeekNumber = model.WeekNumber;
            lab.Test = test;
            lab.Location = model.Location;

            await context.DbContext.SaveChangesAsync();

            await context.LogAsync($"Edited lab ID {lab.LabId}");

            return lab;
        }

        public static async Task<Lab> Delete(IRequestContext context, int labId)
        {
            var lab = await Read(context, labId);
            if (lab == null)
                return null;

            return await Delete(context, lab);
        }

        public static async Task<Lab> Delete(IRequestContext context, Lab lab)
        {
            context.DbContext.Labs.Remove(lab);
            await context.DbContext.SaveChangesAsync();
            await context.LogAsync($"Deleted lab ID {lab.LabId}");
            return lab;
        }
    }
}