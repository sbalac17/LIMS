using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using LIMS.Controllers;
using LIMS.Models;

namespace LIMS.DataAccess
{
    public class SamplesDao
    {
        public static async Task<List<SamplesSearchViewModel.Result>> Find(IRequestContext context, string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                throw new ArgumentNullException(nameof(query));

            var results =
                from s in context.DbContext.Samples
                where s.Description.Contains(query)
                join lsJoin in context.DbContext.LabSamples on s.SampleId equals lsJoin.SampleId into lsGroup
                from ls in lsGroup.DefaultIfEmpty(null)
                orderby s.AddedDate descending
                select new SamplesSearchViewModel.Result { Sample = s, LabSample = ls };

            return await results.ToListAsync();
        }

        public static async Task<Sample> Create(IRequestContext context, SamplesCreateViewModel model)
        {
            var test = await TestsDao.Read(context, model.TestId);
            if (test == null)
                throw new Exception("Test does not exist.");

            var sample = new Sample
            {
                TestId = model.TestId,
                Description = model.Description,
                AddedDate = model.AddedDate,
            };

            context.DbContext.Samples.Add(sample);
            await context.DbContext.SaveChangesAsync();
                
            await context.LogAsync($"Sample ID {sample.SampleId} created");

            return sample;
        }

        public static async Task<Sample> Read(IRequestContext context, long sampleId)
        {
            var query = from s in context.DbContext.Samples
                where s.SampleId == sampleId
                select s;

            return await query.SingleOrDefaultAsync();
        }

        public static async Task<Sample> Update(IRequestContext context, Sample sample, SamplesEditViewModel model)
        {
            var test = await TestsDao.Read(context, model.TestId);
            if (test == null)
                throw new Exception("Test does not exist.");

            sample.Test = test;
            sample.Description = model.Description;
            sample.AddedDate = model.AddedDate;

            await context.DbContext.SaveChangesAsync();

            await context.LogAsync($"Edited sample ID {sample.SampleId}");

            return sample;
        }

        public static async Task<Sample> Delete(IRequestContext context, long sampleId)
        {
            var sample = await Read(context, sampleId);
            if (sample == null)
                return null;

            return await Delete(context, sample);
        }

        public static async Task<Sample> Delete(IRequestContext context, Sample sample)
        {
            context.DbContext.Samples.Remove(sample);
            await context.DbContext.SaveChangesAsync();
            await context.LogAsync($"Deleted sample ID {sample.SampleId}");
            return sample;
        }
    }
}
