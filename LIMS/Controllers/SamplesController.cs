using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using LIMS.Models;

namespace LIMS.Controllers
{
    [Authorize]
    public class SamplesController : ControllerBase
    {
        public async Task<ActionResult> Index(string query = null)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return View(new SamplesSearchViewModel());
            }

            var queryResults =
                from s in DbContext.Samples
                where s.Description.Contains(query)
                join lsJoin in DbContext.LabSamples on s.SampleId equals lsJoin.SampleId into lsGroup
                from ls in lsGroup.DefaultIfEmpty(null)
                orderby s.AddedDate descending
                select new SamplesSearchViewModel.Result { Sample = s, LabSample = ls };

            var model = new SamplesSearchViewModel
            {
                Query = query,
                Results = await queryResults.ToListAsync()
            };

            return View(model);
        }

        [Authorize(Roles = Roles.Privileged)]
        public ActionResult Create()
        {
            return View(new SamplesCreateViewModel());
        }

        [HttpPost]
        [Authorize(Roles = Roles.Privileged)]
        public async Task<ActionResult> Create(SamplesCreateViewModel model)
        {
            var test = await DbContext.Tests.FirstOrDefaultAsync(t => t.TestId == model.TestId);
            if (test == null)
                ModelState.AddModelError(nameof(SamplesEditViewModel.TestId), "Test does not exist.");

            if (!ModelState.IsValid)
                return View(model);

            long newSampleId;
            try
            {
                var sample = new Sample
                {
                    Test = test,
                    Description = model.Description,
                    AddedDate = model.AddedDate
                };

                DbContext.Samples.Add(sample);
                await DbContext.SaveChangesAsync();

                newSampleId = sample.SampleId;
                await LogAsync($"Sample ID {newSampleId} created");
            }
            catch (Exception e)
            {
                ModelState.AddModelError("", e);
                return View(model);
            }

            return RedirectToAction("Details", new { sample = newSampleId });
        }

        [Route("Samples/{sample:int}/")]
        public ActionResult Details(Sample sample)
        {
            if (sample == null)
                return HttpNotFound();

            return View(new SamplesDetailsViewModel
            {
                Sample = sample
            });
        }

        [Route("Samples/{sample:int}/Edit")]
        [Authorize(Roles = Roles.Privileged)]
        public ActionResult Edit(Sample sample)
        {
            if (sample == null)
                return HttpNotFound();

            return View(new SamplesEditViewModel(sample));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Samples/{sample:int}/Edit")]
        [Authorize(Roles = Roles.Privileged)]
        public async Task<ActionResult> Edit(Sample sample, SamplesEditViewModel model)
        {
            if (sample == null)
                return HttpNotFound();

            var test = await DbContext.Tests.FirstOrDefaultAsync(t => t.TestId == model.TestId);
            if (test == null)
                ModelState.AddModelError(nameof(SamplesEditViewModel.TestId), "Test does not exist.");

            if (!ModelState.IsValid)
                return View(model);

            try
            {
                sample.Test = test;
                sample.Description = model.Description;
                sample.AddedDate = model.AddedDate;

                await DbContext.SaveChangesAsync();

                await LogAsync($"Edited sample ID {sample.SampleId}");
            }
            catch (Exception e)
            {
                ModelState.AddModelError("", e);
                return View(model);
            }

            return RedirectToAction("Details", new { sample = sample.SampleId });
        }

        [Route("Samples/{sample:int}/Delete")]
        [Authorize(Roles = Roles.Administrator)]
        public ActionResult Delete(Sample sample)
        {
            if (sample == null)
                return HttpNotFound();

            return View(sample);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Route("Samples/{sample:int}/Delete")]
        [Authorize(Roles = Roles.Administrator)]
        public async Task<ActionResult> ConfirmDelete(Sample sample)
        {
            if (sample != null)
            {
                DbContext.Samples.Remove(sample);
                await DbContext.SaveChangesAsync();

                await LogAsync($"Deleted sample ID {sample.SampleId}");
            }

            return RedirectToAction("Index");
        }
    }
}
