using System.Data.Entity;
using LIMS.Migrations;
using LIMS.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(LIMS.Startup))]

namespace LIMS
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<ApplicationDbContext, Configuration>());

            using (var roleManager = ApplicationRoleManager.Construct())
            {
                if (!roleManager.RoleExists(Roles.Administrator))
                {
                    roleManager.Create(new IdentityRole(Roles.Administrator));
                }

                if (!roleManager.RoleExists(Roles.Faculty))
                {
                    roleManager.Create(new IdentityRole(Roles.Faculty));
                }
            }

            ConfigureAuth(app);
        }
    }
}
