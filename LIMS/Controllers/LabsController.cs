using System;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Web.Mvc;
using LIMS.DataAccess;
using LIMS.Models;
using Microsoft.AspNet.Identity;

namespace LIMS.Controllers
{
    [Authorize]
    public class LabsController : ControllerBase
    {
        public async Task<ActionResult> Index(string query = null)
        {
            var results = await LabsDao.Find(this, query);
            if (results.Count == 0 && string.IsNullOrWhiteSpace(query))
                results = null;

            var model = new LabsSearchViewModel
            {
                Query = query,
                Results = results
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
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                await LabsDao.Create(this, model);
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
            if (!ModelState.IsValid)
                return View(model);
            
            try
            {
                await LabsDao.Update(this, lab, model);
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
            await LabsDao.Delete(this, lab);

            return RedirectToAction("Index");
        }

        [Route("Labs/{lab:int}/Report")]
        [Authorize(Roles = Roles.Privileged)]
        public async Task<ActionResult> Report(Lab lab)
        {
            return View(await LabsDao.GenerateReport(this, lab));
        }
        
        /**************************************************************/

        [Route("Labs/{lab:int}/Members")]
        [LabMember]
        public async Task<ActionResult> Members(Lab lab)
        {
            var userId = HttpContext.User.Identity.GetUserId();
            var members = await LabsDao.ListMembers(this, lab.LabId);

            var model = new LabsMembersViewModel
            {
                Lab = lab,
                Members = members,
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

            model.Query = query;
            model.Results = await LabsDao.ListAddableMembers(this, lab.LabId, query);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Labs/{lab:int}/Members/Add")]
        [LabMember(LabManager = true)]
        public async Task<ActionResult> PostAddMember(Lab lab, string userId)
        {
            await LabsDao.AddMember(this, lab.LabId, userId);
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
            await LabsDao.RemoveMember(this, lab.LabId, userId);
            return RedirectToAction("Members", new { lab = lab.LabId });
        }

        /**************************************************************/

        [Route("Labs/{lab:int}/Reagents")]
        [LabMember]
        public async Task<ActionResult> Reagents(Lab lab)
        {
            var userId = HttpContext.User.Identity.GetUserId();
            var results = await LabsDao.ListReagents(this, lab.LabId);
            var model = new LabsReagentsViewModel
            {
                Lab = lab,
                UsedReagents = results,
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
            
            model.Query = query;
            model.Results = await ReagentsDao.Find(this, query);
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
            
            try
            {
                await LabsDao.AddReagent(this, lab, reagent, model.Quantity);
            }
            catch (Exception e)
            {
                ModelState.AddModelError("", e);
                return View(model);
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

            if (!ModelState.IsValid)
                return View("RemoveReagent", model);

            try
            {
                await LabsDao.RemoveReagent(this, lab.LabId, usedReagentId, model.ReturnQuantity);
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
            var userId = HttpContext.User.Identity.GetUserId();
            var model = new LabsSamplesViewModel
            {
                Lab = lab,
                Query = query,
                LabSamples = await LabsDao.ListSamples(this, lab.LabId, query),
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
            
            var model = new LabsAddSampleViewModel
            {
                Lab = lab,
                Query = query,
                Results = await LabsDao.ListAddableSamples(this, lab.TestId, query)
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

            await LabsDao.AddSample(this, lab.LabId, sample.SampleId);
            return RedirectToAction("Samples", new { lab = lab.LabId });
        }

        [Route("Labs/{lab:int}/Samples/{sample:int}")]
        [LabMember]
        public async Task<ActionResult> SampleDetails(Lab lab, Sample sample)
        {
            var model = await LabsDao.GetSampleDetails(this, lab.LabId, sample.SampleId);
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
            
            var newModel = await LabsDao.GetSampleDetails(this, lab.LabId, sample.SampleId);
            newModel.Message = model.Message;

            if (!ModelState.IsValid)
                return View("SampleDetails", newModel);

            try
            {
                await LabsDao.PostComment(this, lab.LabId, sample.SampleId, model);
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

            if (!ModelState.IsValid)
                return View(model);

            try
            {
                await LabsDao.UpdateSample(this, lab.LabId, sample.SampleId, model);
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
                await LabsDao.RemoveSample(this, lab.LabId, sample.SampleId);
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
