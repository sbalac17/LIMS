using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using LIMS.Models;
using Microsoft.AspNet.Identity;

namespace LIMS.Controllers
{
    [Authorize]
    public class LabsController : ControllerBase
    {
        public async Task<ActionResult> Index(string query = null)
        {
            var userId = HttpContext.User.Identity.GetUserId();

            IQueryable<LabsSearchResult> queryResults;

            if (string.IsNullOrWhiteSpace(query))
            {
                // no query, show all labs the user is a member of
                queryResults =
                    from r in DbContext.LabMembers
                    where r.UserId == userId
                    orderby r.LastOpened descending
                    join lm in DbContext.LabMembers on r.LabId equals lm.LabId
                    where lm.IsLabManager
                    select new LabsSearchResult
                    {
                        Id = r.LabId,
                        IsMember = true,
                        CollegeName = r.Lab.CollegeName,
                        CourseCode = r.Lab.CourseCode,
                        FacultyName = lm.User.UserName,
                        WeekNumber = r.Lab.WeekNumber,
                        TestCode = r.Lab.TestId,
                        Location = r.Lab.Location,
                    };
            }
            else
            {
                // have query, search all labs
                queryResults =
                    from r in DbContext.Labs
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
                        TestCode = r.TestId,
                        Location = r.Location
                    };
            }
            
            var finalResults = await queryResults.ToListAsync();

            if (finalResults.Count == 0 && string.IsNullOrWhiteSpace(query))
                finalResults = null;

            var model = new LabsSearchViewModel
            {
                Query = query,
                Results = finalResults
            };

            return View(model);
        }

        [Authorize(Roles = Roles.Privileged)]
        public ActionResult Create()
        {
            return View(new LabsCreateViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Roles.Privileged)]
        public async Task<ActionResult> Create(LabsCreateViewModel model)
        {
            var test = await DbContext.Tests.FirstOrDefaultAsync(t => t.TestId == model.TestCode);
            if (test == null)
                ModelState.AddModelError(nameof(LabsEditViewModel.TestCode), "Test does not exist.");

            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var userId = HttpContext.User.Identity.GetUserId();
                var user = await UserManager.FindByIdAsync(userId);

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

                DbContext.Labs.Add(lab);
                await DbContext.SaveChangesAsync();

                await LogAsync($"Created lab ID {lab.LabId}");
            }
            catch (Exception e)
            {
                ModelState.AddModelError("", e.ToString());
                return View(model);
            }

            return RedirectToAction("Index");
        }
        
        [Route("Labs/{lab:int}/")]
        [LabMember]
        public ActionResult Details(Lab lab)
        {
            var userId = HttpContext.User.Identity.GetUserId();
            return View(new LabsDetailsViewModel
            {
                Lab = lab,
                IsLabManager = lab.UserIsLabManager(userId)
            });
        }

        [Route("Labs/{lab:int}/Edit")]
        [LabMember(LabManager = true)]
        public ActionResult Edit(Lab lab)
        {
            return View(new LabsEditViewModel(lab));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Labs/{lab:int}/Edit")]
        [LabMember(LabManager = true)]
        public async Task<ActionResult> Edit(Lab lab, LabsEditViewModel model)
        {
            var test = await DbContext.Tests.FirstOrDefaultAsync(t => t.TestId == model.TestCode);
            if (test == null)
                ModelState.AddModelError(nameof(LabsEditViewModel.TestCode), "Test does not exist.");

            if (!ModelState.IsValid)
                return View(model);
            
            try
            {
                lab.CollegeName = model.CollegeName;
                lab.CourseCode = model.CourseCode;
                lab.WeekNumber = model.WeekNumber;
                lab.Test = test;
                lab.Location = model.Location;

                await DbContext.SaveChangesAsync();

                await LogAsync($"Edited lab ID {lab.LabId}");
            }
            catch (Exception e)
            {
                ModelState.AddModelError("", e.ToString());
                return View(model);
            }

            return RedirectToAction("Details", new { lab = lab.LabId });
        }

        [Route("Labs/{lab:int}/Delete")]
        [Authorize(Roles = Roles.Administrator)]
        public ActionResult Delete(Lab lab)
        {
            return View(lab);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Labs/{lab:int}/Delete")]
        [Authorize(Roles = Roles.Administrator)]
        public async Task<ActionResult> ConfirmDelete(Lab lab)
        {
            DbContext.Labs.Remove(lab);
            await DbContext.SaveChangesAsync();

            await LogAsync($"Deleted lab ID {lab.LabId}");

            return RedirectToAction("Index");
        }

        [Route("Labs/{lab:int}/Report")]
        [Authorize(Roles = Roles.Privileged)]
        public async Task<ActionResult> Report(Lab lab)
        {
            var members =
                from lm in DbContext.LabMembers
                where lm.LabId == lab.LabId
                orderby lm.IsLabManager descending, lm.User.UserName
                select new LabsReportViewModel.MemberResult
                {
                    User = lm.User,
                    IsLabManager = lm.IsLabManager
                };

            var labSamples =
                from ls in DbContext.LabSamples
                where ls.LabId == lab.LabId
                orderby ls.Status, ls.AssignedDate
                select new LabsReportViewModel.SampleResult
                {
                    Sample = ls.Sample,
                    Status = ls.Status,
                    AssignedDate = ls.AssignedDate
                };

            var usedReagentQuantities =
                from ur in DbContext.UsedReagents
                where ur.LabId == lab.LabId
                group ur by ur.ReagentId into urGroup
                select new
                {
                    ReagentId = urGroup.Key,
                    Quantity = urGroup.Sum(u => u.Quantity)
                };

            var usedReagents =
                from urQ in usedReagentQuantities
                join rJoin in DbContext.Reagents on urQ.ReagentId equals rJoin.ReagentId into rGroup
                from r in rGroup.DefaultIfEmpty()
                orderby r.Name
                select new LabsReportViewModel.ReagentResult
                {
                    Reagent = r,
                    Quantity = urQ.Quantity
                };

            return View(new LabsReportViewModel
            {
                Lab = lab,
                Members = await members.ToListAsync(),
                LabSamples = await labSamples.ToListAsync(),
                UsedReagents = await usedReagents.ToListAsync()
            });
        }
        
        /**************************************************************/

        [Route("Labs/{lab:int}/Members")]
        [LabMember]
        public async Task<ActionResult> Members(Lab lab)
        {
            var userId = HttpContext.User.Identity.GetUserId();

            var members =
                from lm in DbContext.LabMembers
                where lm.LabId == lab.LabId
                orderby lm.User.UserName
                select new LabsMembersViewModel.Result
                {
                    User = lm.User,
                    IsLabManager = lm.IsLabManager,
                    LastActive = lm.LastOpened
                };

            var model = new LabsMembersViewModel
            {
                Lab = lab,
                Members = await members.ToListAsync(),
                IsLabManager = lab.UserIsLabManager(userId)
            };
            
            return View(model);
        }
        
        [Route("Labs/{lab:int}/Members/Add")]
        [LabMember(LabManager = true)]
        public async Task<ActionResult> AddMember(Lab lab, string query = null)
        {
            var model = new LabsAddMemberViewModel
            {
                Lab = lab,
            };

            if (string.IsNullOrWhiteSpace(query))
            {
                return View(model);
            }

            var results =
                from u in DbContext.Users
                join lmJoin in DbContext.LabMembers.Where(lm => lm.LabId == lab.LabId) on u.Id equals lmJoin.UserId into lmGroup
                from lm in lmGroup.DefaultIfEmpty(null)
                where u.UserName.Contains(query)
                orderby u.UserName
                select new LabsAddMemberViewModel.Result
                {
                    User = u,
                    IsMember = lm != null,
                    IsLabManager = lm != null && lm.IsLabManager
                };

            model.Query = query;
            model.Results = await results.ToListAsync();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Labs/{lab:int}/Members/Add")]
        [LabMember(LabManager = true)]
        public async Task<ActionResult> PostAddMember(Lab lab, string userId)
        {
            var user = await UserManager.FindByIdAsync(userId);
            if (user != null)
            {
                var labMembership = new LabMember
                {
                    Lab = lab,
                    User = user,
                    IsLabManager = false
                };

                DbContext.LabMembers.Add(labMembership);
                await DbContext.SaveChangesAsync();

                await LogAsync($"Added user '{user.UserName}' to lab ID {lab.LabId}");
            }
            
            return RedirectToAction("Members", new { lab = lab.LabId });
        }

        [Route("Labs/{lab:int}/Members/Remove")]
        [LabMember(LabManager = true)]
        public async Task<ActionResult> RemoveMember(Lab lab, string userId)
        {
            var user = await UserManager.FindByIdAsync(userId);
            if (user == null)
                return RedirectToAction("Members", new { lab = lab.LabId });

            var model = new LabsRemoveMemberViewModel
            {
                Lab = lab,
                UserId = userId,
                UserName = user.UserName
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Labs/{lab:int}/Members/Remove")]
        [LabMember(LabManager = true)]
        public async Task<ActionResult> PostRemoveMember(Lab lab, string userId)
        {
            var user = await UserManager.FindByIdAsync(userId);
            if (user != null)
            {
                var labMembership = lab.Members.FirstOrDefault(lm => lm.UserId == userId);
                if (labMembership != null && !labMembership.IsLabManager)
                {
                    DbContext.LabMembers.Remove(labMembership);
                    await DbContext.SaveChangesAsync();

                    await LogAsync($"Removed user '{user.UserName}' from lab ID {lab.LabId}");
                }
            }

            return RedirectToAction("Members", new { lab = lab.LabId });
        }

        /**************************************************************/

        [Route("Labs/{lab:int}/Reagents")]
        [LabMember]
        public async Task<ActionResult> Reagents(Lab lab)
        {
            var userId = HttpContext.User.Identity.GetUserId();

            var reagents =
                from ur in DbContext.UsedReagents
                where ur.LabId == lab.LabId
                orderby ur.UsedDate descending
                select ur;

            var results = reagents.Include(ur => ur.Reagent);

            var model = new LabsReagentsViewModel
            {
                Lab = lab,
                UsedReagents = await results.ToListAsync(),
                IsLabManager = lab.UserIsLabManager(userId)
            };
            
            return View(model);
        }

        [Route("Labs/{lab:int}/Reagents/Add")]
        [LabMember]
        public async Task<ActionResult> AddReagent(Lab lab, string query = null)
        {
            var model = new LabsAddReagentViewModel
            {
                Lab = lab,
            };

            if (string.IsNullOrWhiteSpace(query))
            {
                return View(model);
            }

            var results =
                from r in DbContext.Reagents
                where r.Name.Contains(query)
                orderby r.ExpiryDate ascending
                select r;

            model.Query = query;
            model.Results = await results.ToListAsync();
            return View(model);
        }

        [Route("Labs/{lab:int}/Reagents/Add/{reagent:int}")]
        [LabMember]
        public ActionResult ConfirmAddReagent(Lab lab, Reagent reagent)
        {
            if (reagent == null)
                return RedirectToAction("Reagents", new { lab = lab.LabId });

            return View(new LabsConfirmAddReagentViewModel
            {
                Lab = lab,
                Reagent = reagent,
                Quantity = 1
            });
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Labs/{lab:int}/Reagents/Add/{reagent:int}")]
        [LabMember]
        public async Task<ActionResult> ConfirmAddReagent(Lab lab, Reagent reagent, LabsConfirmAddReagentViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            if (reagent != null)
            {
                var quantity = model.Quantity;

                if (quantity > reagent.Quantity)
                {
                    ModelState.AddModelError(nameof(LabsConfirmAddReagentViewModel.Quantity), "Requested quantity exceeds reagent quantity.");
                    return View(model);
                }

                try
                {
                    var usedReagent = new UsedReagent
                    {
                        Lab = lab,
                        Reagent = reagent,
                        Quantity = quantity,
                        UsedDate = DateTimeOffset.Now,
                    };

                    DbContext.UsedReagents.Add(usedReagent);
                    reagent.Quantity -= quantity;
                    await DbContext.SaveChangesAsync();

                    await LogAsync($"Used {quantity} of reagent ID {reagent.ReagentId} in lab ID {lab.LabId}");
                }
                catch (Exception e)
                {
                    ModelState.AddModelError("", e);
                    return View(model);
                }
            }

            return RedirectToAction("Reagents", new { lab = lab.LabId });
        }

        [Route("Labs/{lab:int}/Reagents/Remove/{usedReagentId:int}")]
        [LabMember(LabManager = true)]
        public async Task<ActionResult> RemoveReagent(Lab lab, [System.Web.Http.FromUri] long usedReagentId)
        {
            var usedReagent = await DbContext.UsedReagents
                .Include(ur => ur.Reagent)
                .FirstOrDefaultAsync(ur => ur.UsedReagentId == usedReagentId);

            if (usedReagent == null)
                return RedirectToAction("Reagents", new { lab = lab.LabId });

            return View(new LabsRemoveReagentViewModel
            {
                Lab = lab,
                Reagent = usedReagent.Reagent,
                UsedReagentId = usedReagentId
            });
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Labs/{lab:int}/Reagents/Remove/{usedReagentId:int}")]
        [LabMember(LabManager = true)]
        public async Task<ActionResult> PostRemoveReagent(Lab lab, [System.Web.Http.FromUri] long usedReagentId, LabsRemoveReagentViewModel model)
        {
            var usedReagent = await DbContext.UsedReagents
                .Include(ur => ur.Reagent)
                .FirstOrDefaultAsync(ur => ur.UsedReagentId == usedReagentId);

            if (usedReagent == null)
                return RedirectToAction("Reagents", new { lab = lab.LabId });

            model.Lab = lab;
            model.Reagent = usedReagent.Reagent;
            
            if (model.ReturnQuantity > usedReagent.Quantity)
                ModelState.AddModelError(nameof(LabsRemoveReagentViewModel.ReturnQuantity), "Return quantity exceeds used quantity.");

            if (!ModelState.IsValid)
                return View("RemoveReagent", model);

            try
            {
                model.Reagent.Quantity += model.ReturnQuantity;
                DbContext.UsedReagents.Remove(usedReagent);

                await DbContext.SaveChangesAsync();

                await LogAsync($"Removed used-reagent ID {usedReagentId}, returning {model.ReturnQuantity} to reagent ID {model.Reagent.ReagentId}");
            }
            catch (Exception e)
            {
                ModelState.AddModelError("", e);
                return RedirectToAction("RemoveReagent", new { usedReagentId });
            }
            
            return RedirectToAction("Reagents", new { lab = lab.LabId });
        }

        /**************************************************************/

        [Route("Labs/{lab:int}/Samples")]
        [LabMember]
        public async Task<ActionResult> Samples(Lab lab, string query = null)
        {
            IQueryable<LabSample> labSamples;

            if (string.IsNullOrWhiteSpace(query))
            {
                labSamples =
                    from ls in DbContext.LabSamples.Include(ls => ls.Sample)
                    where ls.LabId == lab.LabId
                    orderby ls.AssignedDate
                    select ls;
            }
            else
            {
                labSamples =
                    from ls in DbContext.LabSamples.Include(ls => ls.Sample)
                    where ls.LabId == lab.LabId && (ls.Sample.Description.Contains(query) || ls.Notes.Contains(query))
                    orderby ls.AssignedDate
                    select ls;
            }
            
            var userId = HttpContext.User.Identity.GetUserId();

            var model = new LabsSamplesViewModel
            {
                Lab = lab,
                Query = query,
                LabSamples = await labSamples.ToListAsync(),
                IsLabManager = lab.UserIsLabManager(userId)
            };
            
            return View(model);
        }

        [Route("Labs/{lab:int}/Samples/Add")]
        [LabMember(LabManager = true)]
        public async Task<ActionResult> AddSample(Lab lab, string query = null)
        {
            if (string.IsNullOrWhiteSpace(query))
                return View(new LabsAddSampleViewModel { Lab = lab });
            
            var labSamples =
                from s in DbContext.Samples
                where s.TestId == lab.TestId
                join lsJoin in DbContext.LabSamples on s.SampleId equals lsJoin.SampleId into lsGroup
                from ls in lsGroup.DefaultIfEmpty(null)
                where ls == null && s.Description.Contains(query)
                orderby s.AddedDate descending
                select s;

            var results = await labSamples.ToListAsync();

            var model = new LabsAddSampleViewModel
            {
                Lab = lab,
                Query = query,
                Results = results
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Labs/{lab:int}/Samples/Add/{sample:int}")]
        [LabMember(LabManager = true)]
        public async Task<ActionResult> ConfirmAddSample(Lab lab, Sample sample)
        {
            if (sample == null)
                return HttpNotFound();

            var labSample = new LabSample
            {
                Lab = lab,
                Sample = sample,
                AssignedDate = DateTimeOffset.Now,
                Notes = ""
            };

            DbContext.LabSamples.Add(labSample);
            await DbContext.SaveChangesAsync();

            await LogAsync($"Added sample ID {sample.SampleId} to lab ID {lab.LabId}");

            return RedirectToAction("Samples", new { lab = lab.LabId });
        }

        private async Task<LabsSampleDetailsViewModel> SampleDetailsImpl(Lab lab, Sample sample)
        {
            if (sample == null)
                return null;

            var labSample = await DbContext.LabSamples
                .Include(ls => ls.Sample)
                .FirstOrDefaultAsync(ls => ls.LabId == lab.LabId && ls.SampleId == sample.SampleId);

            if (labSample == null)
                return null;

            var source = DbContext.LabSampleComments
                .Include(lsc => lsc.LabSample)
                .Include(lsc => lsc.User);

            var query =
                from lsc in source
                where lsc.LabSample.LabId == labSample.LabId && lsc.LabSample.SampleId == sample.SampleId
                join lmJoin in DbContext.LabMembers on lsc.User.Id equals lmJoin.UserId into lmGroup
                from lm in lmGroup.DefaultIfEmpty(null)
                where lm != null && lm.LabId == labSample.LabId
                select new LabsSampleDetailsViewModel.Result
                {
                    User = lsc.User,
                    Comment = lsc,
                    IsLabManager = lm != null && lm.IsLabManager
                };

            var comments = await query.ToListAsync();

            var userId = HttpContext.User.Identity.GetUserId();
            return new LabsSampleDetailsViewModel
            {
                LabSample = labSample,
                Comments = comments,
                IsLabManager = lab.UserIsLabManager(userId)
            };
        }

        [Route("Labs/{lab:int}/Samples/{sample:int}")]
        [LabMember]
        public async Task<ActionResult> SampleDetails(Lab lab, Sample sample)
        {
            var model = await SampleDetailsImpl(lab, sample);
            if (model == null)
                return HttpNotFound();
            
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Labs/{lab:int}/Samples/{sample:int}")]
        [LabMember]
        public async Task<ActionResult> SamplePostComment(Lab lab, Sample sample, LabsSampleDetailsViewModel model)
        {
            if (sample == null)
                return HttpNotFound();
            
            var newModel = await SampleDetailsImpl(lab, sample);
            newModel.Message = model.Message;

            LabSampleStatus? requestedStatus = null;
            if (model.SelectedButton != 0)
                requestedStatus = (LabSampleStatus)(model.SelectedButton - 1);

            if (!requestedStatus.HasValue && string.IsNullOrWhiteSpace(model.Message))
                ModelState.AddModelError("", "Comment is required.");

            if (!newModel.IsLabManager && requestedStatus.HasValue)
                ModelState.AddModelError("", "Cannot change lab sample status.");

            var userId = HttpContext.User.Identity.GetUserId();
            var user = await UserManager.FindByIdAsync(userId);

            if (!ModelState.IsValid)
                return View("SampleDetails", newModel);

            try
            {
                if (requestedStatus.HasValue)
                    newModel.LabSample.Status = requestedStatus.Value;

                var comment = new LabSampleComment
                {
                    LabSample = newModel.LabSample,
                    User = user,
                    Date = DateTimeOffset.Now,
                    NewStatus = requestedStatus,
                    Message = string.IsNullOrWhiteSpace(newModel.Message) ? null : newModel.Message,
                };

                DbContext.LabSampleComments.Add(comment);
                await DbContext.SaveChangesAsync();

                await LogAsync($"Added comment to lab ID {lab.LabId} sample ID {sample.SampleId}, changing status to {requestedStatus}");
            }
            catch (Exception e)
            {
                ModelState.AddModelError("", e);
                return View("SampleDetails", newModel);
            }

            return RedirectToAction("SampleDetails", new { lab = lab.LabId, sample = sample.SampleId });
        }
        
        [Route("Labs/{lab:int}/Samples/{sample:int}/Edit")]
        [LabMember(LabManager = true)]
        public async Task<ActionResult> EditSample(Lab lab, Sample sample)
        {
            if (sample == null)
                return HttpNotFound();

            var labSample = await DbContext.LabSamples
                .Include(ls => ls.Sample)
                .FirstOrDefaultAsync(ls => ls.LabId == lab.LabId && ls.SampleId == sample.SampleId);

            if (labSample == null)
                return HttpNotFound();
            
            return View(new LabsEditSampleViewModel
            {
                Lab = lab,
                LabSample = labSample,
                Notes = labSample.Notes
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Labs/{lab:int}/Samples/{sample:int}/Edit")]
        [LabMember(LabManager = true)]
        public async Task<ActionResult> EditSample(Lab lab, Sample sample, LabsEditSampleViewModel model)
        {
            if (sample == null)
                return HttpNotFound();

            var labSample = await DbContext.LabSamples
                .Include(ls => ls.Sample)
                .FirstOrDefaultAsync(ls => ls.LabId == lab.LabId && ls.SampleId == sample.SampleId);

            if (labSample == null)
                return HttpNotFound();

            if (!ModelState.IsValid)
                return View(model);

            try
            {
                labSample.Notes = model.Notes;
                await DbContext.SaveChangesAsync();

                await LogAsync($"Edited lab ID {lab.LabId} sample ID {sample.SampleId}");
            }
            catch (Exception e)
            {
                ModelState.AddModelError("", e);
                return View(model);
            }

            return RedirectToAction("Samples", new { lab = lab.LabId });
        }

        [Route("Labs/{lab:int}/Samples/{sample:int}/Remove")]
        [LabMember(LabManager = true)]
        public async Task<ActionResult> RemoveSample(Lab lab, Sample sample)
        {
            if (sample == null)
                return HttpNotFound();

            var labSample = await DbContext.LabSamples
                .Include(ls => ls.Sample)
                .FirstOrDefaultAsync(ls => ls.LabId == lab.LabId && ls.SampleId == sample.SampleId);

            if (labSample == null)
                return HttpNotFound();

            return View(new LabsRemoveSampleViewModel
            {
                Lab = lab,
                LabSample = labSample
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Labs/{lab:int}/Samples/{sample:int}/Remove")]
        [LabMember(LabManager = true)]
        public async Task<ActionResult> PostRemoveSample(Lab lab, Sample sample)
        {
            if (sample == null)
                return HttpNotFound();

            var labSample = await DbContext.LabSamples
                .Include(ls => ls.Sample)
                .FirstOrDefaultAsync(ls => ls.LabId == lab.LabId && ls.SampleId == sample.SampleId);

            if (labSample == null)
                return HttpNotFound();

            try
            {
                DbContext.LabSamples.Remove(labSample);
                await DbContext.SaveChangesAsync();

                await LogAsync($"Removed sample ID {sample.SampleId} from lab ID {lab.LabId}");
            }
            catch (Exception e)
            {
                ModelState.AddModelError("", e);
                return View("RemoveSample", new LabsRemoveSampleViewModel
                {
                    Lab = lab,
                    LabSample = labSample
                });
            }

            return RedirectToAction("Samples", new { lab = lab.LabId });
        }
    }
}
