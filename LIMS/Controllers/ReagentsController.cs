using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using LIMS.DataAccess;
using LIMS.Models;

namespace LIMS.Controllers
{
    [Authorize]
    public class ReagentsController : ControllerBase
    {
        public async Task<ActionResult> Index(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return View(new ReagentsSearchViewModel());
            }

            var model = new ReagentsSearchViewModel
            {
                Query = query,
                Results = await ReagentsDao.Find(this, query)
            };

            return View(model);
        }
        
        [Authorize(Roles = Roles.Privileged)]
        public ActionResult Create()
        {
            return View(new ReagentsCreateViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Roles.Privileged)]
        public async Task<ActionResult> Create(ReagentsCreateViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            Reagent newReagent = null;

            try
            {
                newReagent = await ReagentsDao.Create(this, model);
            }
            catch (Exception e)
            {
                ModelState.AddModelError("", e.ToString());
                return View(model);
            }

            return RedirectToAction("Details", new { reagent = newReagent?.ReagentId ?? 0 });
        }

        [Route("Reagents/{reagent:int}/")]
        public ActionResult Details(Reagent reagent)
        {
            if (reagent == null)
                return HttpNotFound();

            return View(reagent);
        }

        [Authorize(Roles = Roles.Privileged)]
        [Route("Reagents/{reagent:int}/Edit")]
        public ActionResult Edit(Reagent reagent)
        {
            if (reagent == null)
                return HttpNotFound();

            return View(new ReagentsEditViewModel(reagent));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Roles.Privileged)]
        [Route("Reagents/{reagent:int}/Edit")]
        public async Task<ActionResult> Edit(Reagent reagent, ReagentsEditViewModel model)
        {
            if (reagent == null)
                return HttpNotFound();

            if (!ModelState.IsValid)
                return View(model);

            try
            {
                await ReagentsDao.Update(this, reagent, model);
            }
            catch (Exception e)
            {
                ModelState.AddModelError("", e);
                return View(model);
            }

            return RedirectToAction("Details", new { reagent = reagent.ReagentId });
        }

        [Authorize(Roles = Roles.Administrator)]
        [Route("Reagents/{reagent:int}/Delete")]
        public ActionResult Delete(Reagent reagent)
        {
            if (reagent == null)
                return HttpNotFound();

            return View(reagent);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Roles.Administrator)]
        [Route("Reagents/{reagent:int}/Delete")]
        public async Task<ActionResult> PostDelete(Reagent reagent)
        {
            if (reagent != null)
            {
                DbContext.Reagents.Remove(reagent);
                await DbContext.SaveChangesAsync();

                await LogAsync($"Deleted reagent ID {reagent.ReagentId}");
            }

            return RedirectToAction("Index");
        }
    }
}
