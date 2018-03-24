using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using LIMS.DataAccess;
using LIMS.Models;

namespace LIMS.Controllers
{
    [Authorize]
    public class TestsController : ControllerBase
    {
        public async Task<ActionResult> Index(string query = null)
        {
            var results = await TestsDao.Find(this, query);
            if (results.Count == 0 && string.IsNullOrWhiteSpace(query))
                results = null;

            var model = new TestsSearchViewModel
            {
                Query = query,
                Results = results
            };

            return View(model);
        }
        
        [Route("Tests/Create")]
        [Authorize(Roles = Roles.Privileged)]
        public ActionResult Create()
        {
            return View(new TestsCreateViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Tests/Create")]
        [Authorize(Roles = Roles.Privileged)]
        public async Task<ActionResult> Create(TestsCreateViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                await TestsDao.Create(this, model);
            }
            catch (Exception e)
            {
                ModelState.AddModelError("", e);
                return View(model);
            }

            return RedirectToAction("Details", new { test = model.TestCode });
        }

        [Route("Tests/{test}")]
        public ActionResult Details(Test test)
        {
            if (test == null)
                return HttpNotFound();

            return View(test);
        }

        [Route("Tests/{test}/Edit")]
        [Authorize(Roles = Roles.Privileged)]
        public ActionResult Edit(Test test)
        {
            if (test == null)
                return HttpNotFound();

            return View(new TestsEditViewModel(test));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Tests/{test}/Edit")]
        [Authorize(Roles = Roles.Privileged)]
        public async Task<ActionResult> Edit(Test test, TestsEditViewModel model)
        {
            if (test == null)
                return RedirectToAction("Index");

            if (!ModelState.IsValid)
                return View(model);

            try
            {
                await TestsDao.Update(this, test, model);
            }
            catch (Exception e)
            {
                ModelState.AddModelError("", e);
                return View(model);
            }

            return RedirectToAction("Details", new { test = test.TestId });
        }

        [Route("Tests/{test}/Delete")]
        [Authorize(Roles = Roles.Administrator)]
        public ActionResult Delete(Test test, bool cannotDelete = false)
        {
            if (test == null)
                return HttpNotFound();

            return View(new TestsDeleteViewModel
            {
                Test = test,
                CannotDelete = cannotDelete
            });
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Route("Tests/{test}/Delete")]
        [Authorize(Roles = Roles.Administrator)]
        public async Task<ActionResult> ConfirmDelete(Test test)
        {
            if (test != null)
            {
                try
                {
                    await TestsDao.Delete(this, test);
                }
                catch (Exception)
                {
                    return RedirectToAction("Delete", new { test = test.TestId, cannotDelete = true });
                }
            }

            return RedirectToAction("Index");
        }

        [Route("Tests/{test}/AddSample")]
        [Authorize(Roles = Roles.Privileged)]
        public ActionResult AddSample(Test test)
        {
            if (test == null)
                return HttpNotFound();

            return View(new TestsAddSampleViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Tests/{test}/AddSample")]
        [Authorize(Roles = Roles.Privileged)]
        public async Task<ActionResult> AddSample(Test test, TestsAddSampleViewModel model)
        {
            if (test == null)
                return HttpNotFound();

            if (!ModelState.IsValid)
                return View(model);

            long newSampleId;
            try
            {
                var sample = new Sample
                {
                    Test = test,
                    Description = model.Description,
                    AddedDate = model.AddedDate,
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

            return RedirectToAction("Details", "Samples", new { sample = newSampleId });
        }
    }
}
