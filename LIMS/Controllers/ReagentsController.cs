using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
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

            var queryResults =
                from r in DbContext.Reagents
                where r.Name.Contains(query)
                orderby r.Name
                select r;

            var model = new ReagentsSearchViewModel
            {
                Query = query,
                Results = await queryResults.ToListAsync()
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

            long newReagentId;

            try
            {
                var reagent = new Reagent
                {
                    Name = model.Name,
                    Quantity = model.Quantity,
                    AddedDate = DateTimeOffset.Now,
                    ExpiryDate = model.ExpiryDate,
                    ManufacturerCode = model.ManufacturerCode,
                };

                DbContext.Reagents.Add(reagent);
                await DbContext.SaveChangesAsync();

                newReagentId = reagent.ReagentId;
                await LogAsync($"Created reagent ID {newReagentId}");
            }
            catch (Exception e)
            {
                ModelState.AddModelError("", e.ToString());
                return View(model);
            }

            return RedirectToAction("Details", new { reagent = newReagentId });
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
                reagent.Name = model.Name;
                reagent.Quantity = model.Quantity;
                reagent.ExpiryDate = model.ExpiryDate;
                reagent.ManufacturerCode = model.ManufacturerCode;

                await DbContext.SaveChangesAsync();

                await LogAsync($"Edited reagent ID {reagent.ReagentId}");
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
