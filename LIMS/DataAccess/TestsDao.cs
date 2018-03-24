using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using LIMS.Controllers;
using LIMS.Models;

namespace LIMS.DataAccess
{
    public static class TestsDao
    {
        public static async Task<List<TestsSearchViewModel.Result>> Find(IRequestContext context, string query = null)
        {
            IQueryable<Test> queryResults;

            if (string.IsNullOrWhiteSpace(query))
            {
                queryResults =
                    from t in context.DbContext.Tests
                    orderby t.TestId
                    select t;
            }
            else
            {
                queryResults =
                    from t in context.DbContext.Tests
                    where t.TestId.Contains(query) || t.Name.Contains(query) || t.Description.Contains(query)
                    orderby t.TestId
                    select t;
            }

            var results = await queryResults
                .Select(t => new TestsSearchViewModel.Result { TestCode = t.TestId, Name = t.Name })
                .ToListAsync();

            return results;
        }

        public static async Task<Test> Create(IRequestContext context, TestsCreateViewModel model)
        {
            var existingTest = await context.DbContext.Tests.FirstOrDefaultAsync(t => t.TestId == model.TestCode);
            if (existingTest != null)
                throw new Exception("Test code must be unique.");

            var test = new Test
            {
                TestId = model.TestCode,
                Name = model.Name,
                Description = model.Description,
            };

            context.DbContext.Tests.Add(test);
            await context.DbContext.SaveChangesAsync();

            await context.LogAsync($"Test ID '{model.TestCode}' created");

            return test;
        }

        public static async Task<Test> Read(IRequestContext context, string testCode)
        {
            var query = from t in context.DbContext.Tests
                where t.TestId == testCode
                select t;

            return await query.SingleOrDefaultAsync();
        }

        public static async Task<Test> Update(IRequestContext context, Test test, TestsEditViewModel model)
        {
            // changing test code is not allowed.
            test.Name = model.Name;
            test.Description = model.Description;

            await context.DbContext.SaveChangesAsync();

            await context.LogAsync($"Test ID '{test.TestId}' edited");
            return test;
        }

        public static async Task<Test> Delete(IRequestContext context, string testCode)
        {
            var test = await Read(context, testCode);
            if (test == null)
                return null;

            return await Delete(context, test);
        }

        public static async Task<Test> Delete(IRequestContext context, Test test)
        {
            context.DbContext.Tests.Remove(test);
            await context.DbContext.SaveChangesAsync();
            await context.LogAsync($"Deleted test ID '{test.TestId}'");
            return test;
        }
    }
}