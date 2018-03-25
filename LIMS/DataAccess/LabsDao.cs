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

        public static async Task<Lab> Read(IRequestContext context, long labId)
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

        public static async Task<Lab> Delete(IRequestContext context, long labId)
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

        public static async Task<LabsReportViewModel> GenerateReport(IRequestContext context, long labId)
        {
            var lab = await Read(context, labId);
            if (lab == null)
                return null;

            return await GenerateReport(context, lab);
        }

        public static async Task<LabsReportViewModel> GenerateReport(IRequestContext context, Lab lab)
        {
            var members =
                from lm in context.DbContext.LabMembers
                where lm.LabId == lab.LabId
                orderby lm.IsLabManager descending, lm.User.UserName
                select new LabsReportViewModel.MemberResult
                {
                    UserId = lm.UserId,
                    UserName = lm.User.UserName,
                    IsLabManager = lm.IsLabManager
                };

            var labSamples =
                from ls in context.DbContext.LabSamples
                where ls.LabId == lab.LabId
                orderby ls.Status, ls.AssignedDate
                select new LabsReportViewModel.SampleResult
                {
                    Sample = ls.Sample,
                    Status = ls.Status,
                    AssignedDate = ls.AssignedDate
                };

            var usedReagentQuantities =
                from ur in context.DbContext.UsedReagents
                where ur.LabId == lab.LabId
                group ur by ur.ReagentId into urGroup
                select new
                {
                    ReagentId = urGroup.Key,
                    Quantity = urGroup.Sum(u => u.Quantity)
                };

            var usedReagents =
                from urQ in usedReagentQuantities
                join rJoin in context.DbContext.Reagents on urQ.ReagentId equals rJoin.ReagentId into rGroup
                from r in rGroup.DefaultIfEmpty()
                orderby r.Name
                select new LabsReportViewModel.ReagentResult
                {
                    Reagent = r,
                    Quantity = urQ.Quantity
                };

            return new LabsReportViewModel
            {
                Lab = lab,
                Members = await members.ToListAsync(),
                LabSamples = await labSamples.ToListAsync(),
                UsedReagents = await usedReagents.ToListAsync()
            };
        }

        /*******************************************/

        public static async Task<List<LabsMembersViewModel.Result>> ListMembers(IRequestContext context, long labId)
        {
            var members =
                from lm in context.DbContext.LabMembers
                where lm.LabId == labId
                orderby lm.User.UserName
                select new LabsMembersViewModel.Result
                {
                    UserId = lm.UserId,
                    UserName = lm.User.UserName,
                    IsLabManager = lm.IsLabManager,
                    LastActive = lm.LastOpened
                };

            return await members.ToListAsync();
        }

        public static Task<List<LabsAddMemberViewModel.Result>> ListAddableMembers(IRequestContext context, long labId, string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                throw new ArgumentNullException(nameof(query));

            var results =
                from u in context.DbContext.Users
                join lmJoin in context.DbContext.LabMembers.Where(lm => lm.LabId == labId) on u.Id equals lmJoin.UserId into lmGroup
                from lm in lmGroup.DefaultIfEmpty(null)
                where u.UserName.Contains(query)
                orderby u.UserName
                select new LabsAddMemberViewModel.Result
                {
                    UserId = lm.UserId,
                    UserName = lm.User.UserName,
                    IsMember = lm != null,
                    IsLabManager = lm != null && lm.IsLabManager
                };

            return results.ToListAsync();
        }

        public static async Task<LabMember> AddMember(IRequestContext context, long labId, string userId)
        {
            var user = await context.UserManager.FindByIdAsync(userId);
            if (user == null)
                throw new Exception("User does not exist.");

            var labMembership = new LabMember
            {
                LabId = labId,
                User = user,
                IsLabManager = false
            };

            context.DbContext.LabMembers.Add(labMembership);
            await context.DbContext.SaveChangesAsync();

            await context.LogAsync($"Added user '{user.UserName}' to lab ID {labId}");

            return labMembership;
        }

        public static async Task<LabMember> RemoveMember(IRequestContext context, long labId, string userId)
        {
            var user = await context.UserManager.FindByIdAsync(userId);
            if (user != null)
            {
                var labMembership = await context.DbContext.LabMembers
                    .SingleOrDefaultAsync(lm => lm.LabId == labId && lm.UserId == userId);

                if (labMembership != null && !labMembership.IsLabManager)
                {
                    context.DbContext.LabMembers.Remove(labMembership);
                    await context.DbContext.SaveChangesAsync();

                    await context.LogAsync($"Removed user '{user.UserName}' from lab ID {labId}");

                    return labMembership;
                }
            }

            return null;
        }

        /*******************************************/

        public static async Task<List<LabsReagentsViewModel.Result>> ListReagents(IRequestContext context, long labId)
        {
            var reagents =
                from ur in context.DbContext.UsedReagents
                where ur.LabId == labId
                orderby ur.UsedDate descending
                select ur;

            var results = reagents
                .Include(ur => ur.Reagent)
                .Select(ur => new LabsReagentsViewModel.Result
                {
                    UsedReagentId = ur.UsedReagentId,
                    ReagentId = ur.ReagentId,
                    ReagentName = ur.Reagent.Name,
                    ReagentManufacturerCode = ur.Reagent.ManufacturerCode,
                    ReagentExpiryDate = ur.Reagent.ExpiryDate,
                    UsedDate = ur.UsedDate,
                    Quantity = ur.Quantity
                });

            return await results.ToListAsync();
        }

        public static async Task<UsedReagent> AddReagent(IRequestContext context, long labId, long reagentId, int quantity)
        {
            var lab = await Read(context, labId);
            var reagent = await ReagentsDao.Read(context, reagentId);
            return await AddReagent(context, lab, reagent, quantity);
        }
        
        public static async Task<UsedReagent> AddReagent(IRequestContext context, Lab lab, Reagent reagent, int quantity)
        {
            if (reagent == null)
                return null;

            if (quantity > reagent.Quantity)
                throw new ArgumentOutOfRangeException(nameof(quantity), "Requested quantity exceeds reagent quantity.");
                
            var usedReagent = new UsedReagent
            {
                Lab = lab,
                Reagent = reagent,
                Quantity = quantity,
                UsedDate = DateTimeOffset.Now,
            };

            context.DbContext.UsedReagents.Add(usedReagent);
            reagent.Quantity -= quantity;
            await context.DbContext.SaveChangesAsync();

            await context.LogAsync($"Used {quantity} of reagent ID {reagent.ReagentId} in lab ID {lab.LabId}");

            return usedReagent;
        }

        public static async Task<UsedReagent> RemoveReagent(IRequestContext context, long labId, long usedReagentId, int returnQuantity)
        {
            var usedReagent = await context.DbContext.UsedReagents
                .Include(ur => ur.Reagent)
                .FirstOrDefaultAsync(ur => ur.UsedReagentId == usedReagentId);

            if (usedReagent == null)
                return null;
            
            if (returnQuantity > usedReagent.Quantity)
                throw new ArgumentOutOfRangeException(nameof(returnQuantity), "Return quantity exceeds used quantity.");

            usedReagent.Reagent.Quantity += returnQuantity;
            context.DbContext.UsedReagents.Remove(usedReagent);

            await context.DbContext.SaveChangesAsync();

            await context.LogAsync($"Removed used-reagent ID {usedReagentId}, returning {returnQuantity} to reagent ID {usedReagent.Reagent.ReagentId}");

            return usedReagent;
        }

        /*******************************************/

        public static async Task<List<LabsSamplesViewModel.Result>> ListSamples(IRequestContext context, long labId, string query = null)
        {
            IQueryable<LabSample> labSamples;

            if (string.IsNullOrWhiteSpace(query))
            {
                labSamples =
                    from ls in context.DbContext.LabSamples.Include(ls => ls.Sample)
                    where ls.LabId == labId
                    orderby ls.AssignedDate
                    select ls;
            }
            else
            {
                labSamples =
                    from ls in context.DbContext.LabSamples.Include(ls => ls.Sample)
                    where ls.LabId == labId && (ls.Sample.Description.Contains(query) || ls.Notes.Contains(query))
                    orderby ls.AssignedDate
                    select ls;
            }

            return await labSamples
                .Select(ls => new LabsSamplesViewModel.Result
                {
                    LabId = ls.LabId,
                    SampleId = ls.SampleId,
                    SampleTestId = ls.Sample.TestId,
                    SampleDescription = ls.Sample.Description,
                    SampleAddedDate = ls.Sample.AddedDate,
                    AssignedDate = ls.AssignedDate,
                    Status = ls.Status
                })
                .ToListAsync();
        }

        public static async Task<List<Sample>> ListAddableSamples(IRequestContext context, string testId, string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                throw new ArgumentNullException(nameof(query));

            var labSamples =
                from s in context.DbContext.Samples
                where s.TestId == testId
                join lsJoin in context.DbContext.LabSamples on s.SampleId equals lsJoin.SampleId into lsGroup
                from ls in lsGroup.DefaultIfEmpty(null)
                where ls == null && s.Description.Contains(query)
                orderby s.AddedDate descending
                select s;

            return await labSamples.ToListAsync();
        }

        public static async Task<LabSample> AddSample(IRequestContext context, long labId, long sampleId)
        {
            var labSample = new LabSample
            {
                LabId = labId,
                SampleId = sampleId,
                AssignedDate = DateTimeOffset.Now,
                Notes = ""
            };

            context.DbContext.LabSamples.Add(labSample);
            await context.DbContext.SaveChangesAsync();

            await context.LogAsync($"Added sample ID {sampleId} to lab ID {labId}");

            return labSample;
        }

        public static async Task<LabsSampleDetailsViewModel> GetSampleDetails(IRequestContext context, long labId, long sampleId)
        {
            var labSample = await context.DbContext.LabSamples
                .Include(ls => ls.Sample)
                .FirstOrDefaultAsync(ls => ls.LabId == labId && ls.SampleId == sampleId);

            if (labSample == null)
                return null;

            var source = context.DbContext.LabSampleComments
                .Include(lsc => lsc.LabSample)
                .Include(lsc => lsc.User);

            var query =
                from lsc in source
                where lsc.LabSample.LabId == labSample.LabId && lsc.LabSample.SampleId == sampleId
                join lmJoin in context.DbContext.LabMembers on lsc.User.Id equals lmJoin.UserId into lmGroup
                from lm in lmGroup.DefaultIfEmpty(null)
                where lm != null && lm.LabId == labSample.LabId
                select new LabsSampleDetailsViewModel.Result
                {
                    UserId = lsc.User.Id,
                    UserName = lsc.User.UserName,
                    Comment = lsc,
                    IsLabManager = lm != null && lm.IsLabManager
                };
            
            return new LabsSampleDetailsViewModel
            {
                LabSample = labSample,
                Comments = await query.ToListAsync(),
                IsLabManager = await IsLabManager(context, labId)
            };
        }

        public static async Task<LabSampleComment> PostComment(IRequestContext context, long labId, long sampleId, string message, LabSampleStatus? statusChange = null)
        {
            if (!await IsLabManager(context, labId) && statusChange.HasValue)
                throw new InvalidOperationException("Not authorized to change sample status.");

            var labSample = await context.DbContext.LabSamples.SingleOrDefaultAsync(ls => ls.LabId == labId && ls.SampleId == sampleId);
            if (labSample == null)
                return null;

            if (statusChange.HasValue)
                labSample.Status = statusChange.Value;

            var comment = new LabSampleComment
            {
                LabSample = labSample,
                UserId = context.UserId,
                Date = DateTimeOffset.Now,
                NewStatus = statusChange,
                Message = string.IsNullOrWhiteSpace(message) ? null : message,
            };

            context.DbContext.LabSampleComments.Add(comment);
            await context.DbContext.SaveChangesAsync();

            await context.LogAsync($"Added comment to lab ID {labId} sample ID {sampleId}, changing status to {statusChange}");

            return comment;
        }

        private static async Task<bool> IsLabManager(IRequestContext context, long labId)
        {
            var membership = await context.DbContext.LabMembers.SingleOrDefaultAsync(lm => lm.LabId == labId && lm.UserId == context.UserId);
            return membership?.IsLabManager ?? false;
        }
            
    }
}
