using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using LIMS.DataAccess;
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

            var model = new SamplesSearchViewModel
            {
                Query = query,
                Results = await SamplesDao.Find(this, query)
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
            if (!ModelState.IsValid)
                return View(model);

            Sample sample;
            try
            {
                sample = await SamplesDao.Create(this, model);
            }
            catch (Exception e)
            {
                ModelState.AddModelError("", e);
                return View(model);
            }

            return RedirectToAction("Details", new { sample = sample.SampleId });
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
            
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                await SamplesDao.Update(this, sample, model);
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
                await SamplesDao.Delete(this, sample);
            }

            return RedirectToAction("Index");
        }
    }
}
