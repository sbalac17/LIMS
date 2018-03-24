using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using LIMS.Controllers;
using LIMS.Models;

namespace LIMS.DataAccess
{
    public class ReagentsDao
    {
        public static async Task<List<Reagent>> Find(IRequestContext context, string query = null)
        {
            IQueryable<Reagent> results;

            if (string.IsNullOrWhiteSpace(query))
            {
                results = from r in context.DbContext.Reagents
                    orderby r.Name
                    select r;
            }
            else
            {
                results = from r in context.DbContext.Reagents
                    where r.Name.Contains(query)
                    orderby r.Name
                    select r;
            }

            return await results.ToListAsync();
        }

        public static async Task<Reagent> Create(IRequestContext context, ReagentsCreateViewModel model)
        {
            var reagent = new Reagent
            {
                Name = model.Name,
                Quantity = model.Quantity,
                AddedDate = DateTimeOffset.Now,
                ExpiryDate = model.ExpiryDate,
                ManufacturerCode = model.ManufacturerCode,
            };

            context.DbContext.Reagents.Add(reagent);
            await context.DbContext.SaveChangesAsync();
                
            await context.LogAsync($"Created reagent ID {reagent.ReagentId}");

            return reagent;
        }

        public static async Task<Reagent> Read(IRequestContext context, int reagentId)
        {
            var query = from r in context.DbContext.Reagents
                where r.ReagentId == reagentId
                select r;

            return await query.SingleOrDefaultAsync();
        }

        public static async Task<Reagent> Update(IRequestContext context, Reagent reagent, ReagentsEditViewModel model)
        {
            reagent.Name = model.Name;
            reagent.Quantity = model.Quantity;
            reagent.ExpiryDate = model.ExpiryDate;
            reagent.ManufacturerCode = model.ManufacturerCode;

            await context.DbContext.SaveChangesAsync();

            await context.LogAsync($"Edited reagent ID {reagent.ReagentId}");

            return reagent;
        }

        public static async Task<Reagent> Delete(IRequestContext context, int reagentId)
        {
            var reagent = await Read(context, reagentId);
            if (reagent == null)
                return null;

            return await Delete(context, reagent);
        }

        public static async Task<Reagent> Delete(IRequestContext context, Reagent reagent)
        {
            context.DbContext.Reagents.Remove(reagent);
            await context.DbContext.SaveChangesAsync();
            await context.LogAsync($"Deleted reagent ID {reagent.ReagentId}");
            return reagent;
        }
    }
}
